package com.legendsofrus.credentials;

import java.nio.ByteBuffer;
import java.nio.charset.CodingErrorAction;
import java.nio.charset.StandardCharsets;
import java.security.GeneralSecurityException;
import java.util.Arrays;
import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.GCMParameterSpec;

/** Cipher envelope and fail-closed journal policy. No Android dependency for host verification. */
public final class CredentialVault {
    public interface Keys {
        SecretKey existing() throws Exception;
        SecretKey create() throws Exception;
    }
    public interface Journal {
        byte[] read() throws Exception;
        void commit(byte[] value) throws Exception;
    }
    public static final int MAX_TEXT_BYTES = 16384;
    public static final int MAX_RECORD_BYTES = MAX_TEXT_BYTES + 32;
    private static final byte[] MAGIC = { 'L', 'R', 'C', 1 };
    private final Keys keys;
    private final Journal journal;
    private final byte[] domain;

    public CredentialVault(Keys keys, Journal journal, String domain) {
        this.keys = keys; this.journal = journal;
        this.domain = domain.getBytes(StandardCharsets.UTF_8);
    }

    public synchronized String read() throws Exception {
        byte[] record = journal.read();
        SecretKey key = keys.existing();
        if (record == null) {
            if (key != null) throw new GeneralSecurityException("credential_record_missing");
            return null;
        }
        if (key == null) throw new GeneralSecurityException("credential_key_missing");
        return decrypt(record, key);
    }

    public synchronized void write(String text) throws Exception {
        if (text == null) throw new GeneralSecurityException("credential_payload_missing");
        byte[] plain = text.getBytes(StandardCharsets.UTF_8);
        try {
            if (plain.length == 0 || plain.length > MAX_TEXT_BYTES)
                throw new GeneralSecurityException("credential_payload_size");
            byte[] before = journal.read();
            SecretKey key = keys.existing();
            if (before != null) {
                if (key == null) throw new GeneralSecurityException("credential_key_missing");
                decrypt(before, key); // Never overwrite a damaged record or regenerate its key.
            } else if (key != null) {
                throw new GeneralSecurityException("credential_record_missing");
            } else {
                key = keys.create();
            }
            Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding");
            cipher.init(Cipher.ENCRYPT_MODE, key); // Provider-generated fresh random IV.
            byte[] iv = cipher.getIV();
            if (iv == null || iv.length != 12) throw new GeneralSecurityException("credential_iv_invalid");
            cipher.updateAAD(domain); cipher.updateAAD(MAGIC);
            byte[] encrypted = cipher.doFinal(plain);
            byte[] record = ByteBuffer.allocate(16 + encrypted.length).put(MAGIC).put(iv).put(encrypted).array();
            journal.commit(record);
            // AtomicFile.finishWrite has no success result. Verify exact durable content before
            // telling Unity it may send the corresponding network request.
            if (!Arrays.equals(record, journal.read())) throw new GeneralSecurityException("credential_commit_unconfirmed");
        } finally { Arrays.fill(plain, (byte)0); }
    }

    private String decrypt(byte[] record, SecretKey key) throws Exception {
        if (record.length < 33 || record.length > MAX_RECORD_BYTES || !Arrays.equals(MAGIC, Arrays.copyOf(record,4)))
            throw new GeneralSecurityException("credential_envelope_invalid");
        Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding");
        cipher.init(Cipher.DECRYPT_MODE, key, new GCMParameterSpec(128, Arrays.copyOfRange(record,4,16)));
        cipher.updateAAD(domain); cipher.updateAAD(MAGIC);
        byte[] plain = cipher.doFinal(record,16,record.length-16);
        try {
            return StandardCharsets.UTF_8.newDecoder().onMalformedInput(CodingErrorAction.REPORT)
                .onUnmappableCharacter(CodingErrorAction.REPORT).decode(ByteBuffer.wrap(plain)).toString();
        } finally { Arrays.fill(plain,(byte)0); }
    }
}
