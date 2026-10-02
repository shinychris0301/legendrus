# Android protected credentials — implementation and limits

2026-09-29 local date. Continues the approved own-backend/RuStore account work. The Android storage adapter is implemented; no connected Android device was available to validate it on-device. No production endpoint or purchase activation is installed.

## Storage and recovery

`AndroidCredentialBridge` runs serialized background operations using Android Keystore AES-256-GCM and Android `AtomicFile`. The encrypted envelope contains a version marker, a provider-generated 12-byte nonce and authenticated ciphertext/tag. Package-specific additional authenticated data prevents cross-package substitution. The key remains in Android Keystore; encrypted records live in `Context.getNoBackupFilesDir()`. No storage permission, game-save field, PlayerPrefs token, hardcoded encryption key or plaintext fallback was added.

Before reporting save success, the adapter syncs the file, finishes the atomic write and reads the committed ciphertext back for exact comparison. A subsequent read verifies the GCM authentication tag and envelope limits. Disk/key failures, invalid envelopes, changed tags and malformed records return failure, not an empty account. Existing damaged ciphertext or a missing key is never silently overwritten/rekeyed. The bridge's single executor serializes reads and writes in this game process; multi-process writers are not supported.

A pristine vault requires both no key and no record. If interruption occurs after first key creation but before the first record commits, the vault reports unavailable on restart. It does not silently generate another identity. This deliberately conservative case needs a future explicit recovery/reset choice. Loss of a key, application data or the device is not solved by encryption. Linked-account recovery remains separate work. This implementation is not a rollback-resistant server authority; the server's session-family/replay rules remain responsible for rejecting obsolete credentials.

`AndroidBackendCredentialStore` validates the decrypted session shape and implements the new asynchronous credential-store interface. Synchronous entry points fail closed to prevent accidental main-thread Keystore/disk use. `BackendSessionClient` now awaits asynchronous loads and writes while retaining support for existing synchronous verification fixtures. Pending bootstrap/rotation must persist before a network call; final credential persistence must succeed before wallet use. Access tokens remain memory-only.

`BackendAccountService.TryConfigureAndroid` is the explicit composition entry for a future approved HTTPS endpoint. In the Editor it returns false, and normal startup remains unconfigured. It does not add UI listeners, claim linked sign-in is ready, or overwrite local game progress. No new scene, Canvas or EventSystem was introduced.

## Java plugin and verification boundary

Two Java source plugins are installed under `Assets/LegendsOfRus/Plugins/AndroidCredentials`, with Android-only importer metadata. They use existing Android APIs and add no downloaded SDK/package or native `.so` library. Actual Unity importer checks verify the selected platform. Current project minification remains disabled; before a minified release, JNI classes/methods/Operation fields need retained-name rules and a packaging test. Existing Android build approval guards remain intact.

The production `CredentialVault` envelope/policy core is exercised on the host with real JCE AES-GCM and injected keys/journal faults. The bridge also compiles against the installed Android API36 jar. These checks do **not** execute Android Keystore, Android AtomicFile, JNI on a phone, Unity's Android Gradle packaging, or power-loss durability. No APK was built or installed in this task. `adb devices` reported no attached device.

The actual loopback Unity/backend request and separate-process lost-response tests cover the session protocol using disposable fixture storage, not the Android adapter. Keep that distinction in acceptance reports. No normal saves or real credentials were inspected or used.

## References

Android's [Keystore documentation](https://developer.android.com/privacy-and-security/keystore) describes non-exportable key operations and recommends moving them off the main thread; hardware-backed protection depends on the device and is not claimed here. [AtomicFile](https://developer.android.com/reference/android/util/AtomicFile.html) requires caller-managed synchronization. [No-backup storage](https://developer.android.com/reference/android/content/Context#getNoBackupFilesDir()) keeps these files outside automatic backup. Unity documents [Java source plugins](https://docs.unity.com/en-us/engine/6000.5/manual/platform-specific/android/developing/plugins-for/plugin-types/java-source-plugins) as Android-build inputs. Consulted2026-09-29; build/device acceptance remains necessary.

## Next step

Review the updated plugin inventory through the existing development-APK guard, build the permitted debug verification artifact, and perform Android Keystore/JNI restart, interrupted-write, corruption and app-lifecycle checks when a device is available. Then verify the configured hosted HTTPS endpoint and add explicit linked-account recovery before enabling online account UI or commerce. Do not treat the local loopback server as deployed infrastructure. Campaign, art, gameplay rules and effects remain unchanged; effects stay last.
