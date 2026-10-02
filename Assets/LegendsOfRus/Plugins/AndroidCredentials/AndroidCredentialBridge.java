package com.legendsofrus.credentials;

import android.content.Context;
import android.security.keystore.KeyGenParameterSpec;
import android.security.keystore.KeyProperties;
import android.util.AtomicFile;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.security.KeyStore;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;

/** Unity JNI entry. All filesystem/Keystore operations run on one background executor. */
public final class AndroidCredentialBridge {
    private static final String ALIAS = "lor.backend.credentials.v1";
    private static final ExecutorService WORKER = Executors.newSingleThreadExecutor();
    private AndroidCredentialBridge() { }

    public static final class Operation {
        public volatile boolean done;
        public volatile boolean success;
        public volatile String value;
        private void finish(boolean ok, String text) { success=ok; value=text; done=true; }
    }

    public static Operation beginRead(Context context) {
        final Context app = context.getApplicationContext();
        final Operation operation = new Operation();
        WORKER.execute(() -> {
            try { operation.finish(true, vault(app).read()); }
            catch (Exception ignored) { operation.finish(false,null); }
        });
        return operation;
    }

    public static Operation beginWrite(Context context, String text) {
        final Context app = context.getApplicationContext();
        final Operation operation = new Operation();
        WORKER.execute(() -> {
            try { vault(app).write(text); operation.finish(true,null); }
            catch (Exception ignored) { operation.finish(false,null); }
        });
        return operation;
    }

    private static CredentialVault vault(Context context) throws Exception {
        final File directory = context.getNoBackupFilesDir();
        if (directory == null || (!directory.isDirectory() && !directory.mkdirs()))
            throw new IOException("credential_directory_unavailable");
        final File file = new File(directory,"lor-backend-credentials-v1.bin");
        final AtomicFile atomic = new AtomicFile(file);
        final KeyStore store = KeyStore.getInstance("AndroidKeyStore");
        store.load(null);
        CredentialVault.Keys keys = new CredentialVault.Keys() {
            public SecretKey existing() throws Exception {
                if (!store.containsAlias(ALIAS)) return null;
                java.security.Key key = store.getKey(ALIAS,null);
                if (!(key instanceof SecretKey)) throw new IOException("credential_key_invalid");
                return (SecretKey)key;
            }
            public SecretKey create() throws Exception {
                if (store.containsAlias(ALIAS)) throw new IOException("credential_key_exists");
                KeyGenerator generator = KeyGenerator.getInstance(KeyProperties.KEY_ALGORITHM_AES,"AndroidKeyStore");
                generator.init(new KeyGenParameterSpec.Builder(ALIAS,KeyProperties.PURPOSE_ENCRYPT|KeyProperties.PURPOSE_DECRYPT)
                    .setKeySize(256).setBlockModes(KeyProperties.BLOCK_MODE_GCM)
                    .setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_NONE)
                    .setRandomizedEncryptionRequired(true).build());
                return generator.generateKey();
            }
        };
        CredentialVault.Journal journal = new CredentialVault.Journal() {
            public byte[] read() throws Exception {
                if (!file.exists() && !new File(file.getPath()+".bak").exists() && !new File(file.getPath()+".new").exists()) return null;
                try (FileInputStream input = atomic.openRead(); ByteArrayOutputStream bytes = new ByteArrayOutputStream()) {
                    byte[] chunk = new byte[4096]; int count;
                    while ((count=input.read(chunk))!=-1) {
                        if (bytes.size()+count>CredentialVault.MAX_RECORD_BYTES) throw new IOException("credential_record_size");
                        bytes.write(chunk,0,count);
                    }
                    return bytes.toByteArray();
                }
            }
            public void commit(byte[] value) throws Exception {
                FileOutputStream output = null;
                try {
                    output = atomic.startWrite(); output.write(value); output.getFD().sync();
                    atomic.finishWrite(output); output=null;
                } finally { if (output != null) atomic.failWrite(output); }
            }
        };
        return new CredentialVault(keys,journal,"LegendsOfRus/backend-credentials/v1\n"+context.getPackageName());
    }
}
