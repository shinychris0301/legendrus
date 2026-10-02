package com.legendsofrus.credentials;

import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import java.util.Arrays;
import java.nio.charset.StandardCharsets;

/** Host JCE tests for production envelope/policy, not AndroidKeyStore or AtomicFile execution. */
public final class CredentialVaultHostTests {
    interface Check { void run() throws Exception; }
    static int passed;
    static void check(boolean ok) { if(!ok) throw new AssertionError("check_failed"); }
    static void fails(Check check) throws Exception { try { check.run(); } catch(Exception expected) { return; } throw new AssertionError("expected_failure"); }
    static void test(String name, Check action) throws Exception { action.run(); passed++; System.out.println("PASS "+name); }
    static final class Keys implements CredentialVault.Keys {
        SecretKey key; int created;
        public SecretKey existing() { return key; }
        public SecretKey create() throws Exception { KeyGenerator g=KeyGenerator.getInstance("AES");g.init(256);key=g.generateKey();created++;return key; }
    }
    static final class Journal implements CredentialVault.Journal {
        byte[] value; int failure; int commits;
        public byte[] read() { return value==null?null:value.clone(); }
        public void commit(byte[] next) throws Exception {
            commits++;
            if(failure==1) throw new Exception("before_commit");
            if(failure==3) return; // Platform API silently failed; readback must detect it.
            value=next.clone();
            if(failure==2) throw new Exception("after_commit");
        }
    }
    static final class Fixture {
        final Keys keys=new Keys();final Journal journal=new Journal();
        CredentialVault vault=new CredentialVault(keys,journal,"fixture.app/domain");
    }
    public static void main(String[] args) throws Exception {
        test("pristine_read_creates_no_key",()->{Fixture f=new Fixture();check(f.vault.read()==null);check(f.keys.created==0);});
        test("ciphertext_round_trip_and_reopen",()->{Fixture f=new Fixture();f.vault.write("synthetic-session-данные");check(f.vault.read().equals("synthetic-session-данные"));check(new CredentialVault(f.keys,f.journal,"fixture.app/domain").read().equals(f.vault.read()));check(!new String(f.journal.value,StandardCharsets.UTF_8).contains("synthetic-session"));});
        test("fresh_nonce_every_write",()->{Fixture f=new Fixture();f.vault.write("same");byte[] old=f.journal.read();f.vault.write("same");check(!Arrays.equals(Arrays.copyOfRange(old,4,16),Arrays.copyOfRange(f.journal.value,4,16)));check(f.keys.created==1);});
        test("tampered_ciphertext_rejected_without_overwrite",()->{Fixture f=new Fixture();f.vault.write("payload");f.journal.value[20]^=1;byte[] bad=f.journal.read();fails(()->f.vault.read());fails(()->f.vault.write("replacement"));check(Arrays.equals(bad,f.journal.value));});
        test("tampered_header_rejected",()->{Fixture f=new Fixture();f.vault.write("payload");f.journal.value[3]=99;fails(()->f.vault.read());});
        test("truncated_and_oversized_records_rejected",()->{Fixture f=new Fixture();f.vault.write("payload");byte[] good=f.journal.read();for(int n:new int[]{0,3,16,32,good.length-1,CredentialVault.MAX_RECORD_BYTES+1}){f.journal.value=Arrays.copyOf(good,n);fails(()->f.vault.read());}});
        test("package_domain_bound_by_gcm",()->{Fixture f=new Fixture();f.vault.write("payload");fails(()->new CredentialVault(f.keys,f.journal,"other.app/domain").read());});
        test("wrong_key_rejected",()->{Fixture f=new Fixture();f.vault.write("payload");f.keys.create();fails(()->f.vault.read());});
        test("missing_key_never_regenerated",()->{Fixture f=new Fixture();f.vault.write("payload");f.keys.key=null;fails(()->f.vault.read());fails(()->f.vault.write("replacement"));check(f.keys.created==1);});
        test("missing_record_does_not_create_new_identity",()->{Fixture f=new Fixture();f.vault.write("payload");f.journal.value=null;fails(()->f.vault.read());fails(()->f.vault.write("replacement"));check(f.keys.created==1);});
        test("failed_first_write_is_unavailable_not_empty",()->{Fixture f=new Fixture();f.journal.failure=1;fails(()->f.vault.write("pending-bootstrap"));fails(()->f.vault.read());check(f.keys.created==1);});
        test("failure_before_commit_preserves_previous",()->{Fixture f=new Fixture();f.vault.write("old");f.journal.failure=1;fails(()->f.vault.write("pending-rotation"));check(f.vault.read().equals("old"));});
        test("failure_after_commit_preserves_retryable_pending",()->{Fixture f=new Fixture();f.vault.write("old");f.journal.failure=2;fails(()->f.vault.write("pending-rotation"));check(f.vault.read().equals("pending-rotation"));});
        test("silent_write_failure_detected_by_readback",()->{Fixture f=new Fixture();f.vault.write("old");f.journal.failure=3;fails(()->f.vault.write("pending-rotation"));check(f.vault.read().equals("old"));});
        test("payload_bounds_before_key_creation",()->{Fixture f=new Fixture();fails(()->f.vault.write(null));fails(()->f.vault.write(""));fails(()->f.vault.write(new String(new char[CredentialVault.MAX_TEXT_BYTES+1])));check(f.keys.created==0);});
        test("maximum_payload_round_trip",()->{Fixture f=new Fixture();String text=new String(new char[CredentialVault.MAX_TEXT_BYTES]).replace('\0','a');f.vault.write(text);check(text.equals(f.vault.read()));check(f.journal.value.length==CredentialVault.MAX_RECORD_BYTES);});
        test("authenticated_tag_tamper_rejected",()->{Fixture f=new Fixture();f.vault.write("payload");f.journal.value[f.journal.value.length-1]^=1;fails(()->f.vault.read());});
        System.out.println("Passed "+passed+" host tests; Android device execution not performed.");
    }
}
