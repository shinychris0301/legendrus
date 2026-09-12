# v1.17 Step 2 — Save Migration and Recovery Foundation

## Decision

Implementation status:
`IMPLEMENTED_IN_SOURCE_PENDING_UNITY_SAVE_RECOVERY_VERIFICATION`

Candidate status:
`SAVE_V6_ENVELOPE_V1_RECOVERY_FOUNDATION_PRODUCTION_ACTIVATION_BLOCKED`

Step 2 is implemented as a local save-integrity and recovery foundation. It
does not authorize production Android, economy, commercial, provider, or cloud
behavior. The user authorized development to continue after reporting that the
previous candidate ran without errors, but no retained Step 1 Unity,
package-lock, Android-preflight, APK, or representative-device evidence is
present in this source candidate. `ResolvedPackageLockApproved=false` remains a
hard stop for Android preflight and build.

Unity compilation, project validation, the Step 2 Editor self-test, complete
EditMode and PlayMode execution, raw-profile recovery checks, forced-termination
checks, and representative-device results for these exact Step 2 bytes are
`NOT_RETAINED`.

## Compatibility contract

The persisted player payload remains `PlayerSaveData` version 6. No recovery,
revision, checksum, provider, entitlement, or activation metadata is added to
that DTO. This preserves the v1.15 detached economy coordinator and the v1.16
commercial identity boundary, both of which depend on the existing payload.

The inherited primary PlayerPrefs key is still
`LegendsOfRus_Save_v1`. Backup and staging are implementation companions; they
do not replace the public identity of the save. The compatibility methods
`PlayerSaveData Load()` and `void Save(PlayerSaveData)` remain available while
structured results expose failure and recovery facts to new callers.

Storage envelope version 1 is independent of payload version 6. A future
envelope revision therefore does not require a gameplay-schema bump, and a
future gameplay schema cannot be silently interpreted as the current payload.

Raw import validation is version-aware. Unversioned payloads map to the legacy
v1 migration path, and raw v1-v5 payloads retain their historical permissive
field compatibility. A raw current-v6 import must contain all 32 known top-level
fields, use the expected top-level value types, and contain no duplicate known
field. This prevents JsonUtility field initializers from turning a partial v6
fragment into an apparently complete current save. A canonical future raw
version remains unsupported-future evidence even though its schema is unknown,
so an older backup cannot silently downgrade it.

## Storage envelope

The engine-free envelope has a deterministic line representation containing:

- fixed marker and envelope-format version;
- explicit ordinary or history-replacement intent;
- payload version and positive monotonic revision;
- exact UTF-8 payload byte count;
- lower-case SHA-256 of the payload bytes;
- canonical Base64 of the payload bytes;
- lower-case SHA-256 of the unsigned envelope.

Decode rejects malformed structure, non-canonical encodings, invalid UTF-8,
length/hash mismatches, unsupported envelope versions, and unsupported payload
versions with stable diagnostic codes. Payload text is capped at 1,048,576
characters; UTF-8 bytes, Base64 text, and total envelope size have corresponding
upper bounds before expensive decode/allocation paths. Reads do not repair,
promote, delete, rewrite, or migrate storage as a side effect.

The decoder inspects a bounded canonical marker/format prefix before applying
v1-specific line, size, Base64, and hash rules. A canonical differently shaped
or oversized future envelope is therefore classified as unsupported future
evidence and blocks downgrade; a current-shaped record with only a changed
format digit must still pass the applicable outer-integrity check.

The hashes provide deterministic corruption detection only. They do not provide
authenticity, confidentiality, anti-cheat protection, replay protection against
an attacker who can rewrite all fields, or trusted commercial fulfillment.

## Journal and recovery contract

The local journal has three roles:

| Slot | Meaning | Load authority |
|---|---|---|
| Primary | Last committed candidate | Authoritative when valid and supported |
| Backup | Prior committed candidate, or reset-equivalent history | Recovery only when policy permits |
| Staging | Candidate being written | Interruption evidence; never promoted by load |

An ordinary save performs explicit write/flush phases: stage the new envelope,
preserve a valid previous primary as backup, commit the new primary, and remove
staging. Each failure reports its phase. This protocol narrows partial-write
failure modes and preserves a committed candidate under the tested store model;
it is not a claim that PlayerPrefs supplies a general transactional database or
that every operating-system/storage failure is survivable.

Load behavior is deterministic:

- a valid supported primary wins;
- a future envelope or payload in any journal slot blocks downgrade and write;
- staging is never treated as committed data;
- a missing primary is an explicit no-save boundary and never revives a
  backup-only history;
- a valid committed backup may recover an invalid primary when a newer
  replacement-intent staging candidate does not make that recovery unsafe;
- no valid committed candidate yields an explicit unrecoverable state;
- a storage read failure is distinct from corruption and from no save.

An explicit reset uses history replacement. Its integrity-protected replacement
intent and revision remain in durable staging across the primary/backup update
window. If interruption leaves an older backup behind, load treats that backup
as unsafe and does not resurrect it. Once replacement completes, the reset
payload is committed as both primary and backup before staging cleanup, so a
later primary corruption can recover only the reset-equivalent history.
Replacement recovery additionally requires equal-revision staging and backup
records to be byte-identical; conflicting valid records at the same revision
are unrecoverable rather than guessed.

Once the primary write flush returns successfully, a later replacement-backup
or staging-cleanup failure reports `PrimaryDurablyCommitted=true` alongside a
non-success status. ResetProgress may then adopt the reset state in memory
without pretending the journal fully succeeded; `CanPersist` remains false and
`LastWriteResult` retains the incomplete redundancy/cleanup result.

## Migration registry

The migration registry clones the loaded DTO before applying the inherited
steps and returns a result instead of persisting:

- v1 to v2: energy capacity, booster defaults, and settings defaults;
- v2 to v3: selected/unlocked level state and legacy level progress;
- v3 to v4: completed tutorial steps;
- v4 to v5: completed story scenes;
- v5 to v6: acknowledged chapter completions.

Energy timestamp fallback is supplied explicitly. Legacy level conversion is a
provided operation so migration can be tested without storage mutation. Null,
negative, malformed, failed, and future inputs do not produce a persistable
migration result. A current v6 payload remains v6; Step 2 does not rebase
balances, inventory, rewards, or progression.

## Runtime failure policy

The runtime adapter classifies load, parse, migration, and write outcomes with
stable diagnostic codes. A recoverable backup result may initialize gameplay
from validated data. An unrecoverable, storage-failure, parse-failure, migration-
failure, or unsupported-future result may expose a conservative in-memory
default so the application can remain operable. Every normal write rescans the
journal and remains blocked while that evidence exists. The default therefore
cannot overwrite the raw evidence through normal GameState persistence;
clearing the block requires explicit external remediation or removal of the
evidence.

Write failures never authorize a partial candidate. Null input, an invalid
payload version, a future-version barrier, revision exhaustion, encoding
failure, and storage failure all fail closed. Callers retain the existing
compatibility surface, but structured APIs are the evidence-bearing boundary.
Legacy GameState mutator return values describe gameplay acceptance, not proof
of durable commit; durability-sensitive callers must inspect `CanPersist` and
`LastWriteResult`, or use the structured SaveService result. Commercial
activation remains blocked, so no entitlement may rely on those gameplay
booleans.

The legacy mutators still change the live DTO before calling persistence. Step
2 does not provide general copy-on-write gameplay mutation transactions or
rollback of an accepted mutation after a failed write. Such transactions must
clone state, apply and validate the mutation, persist the candidate through the
structured API, and adopt it only after the required commit result. They are a
hard prerequisite for closed/player-facing testing and for any claim of
transactional mutation durability.

## Activation and production boundary

The following remain false or absent:

- runtime economy authority and all v1.15 activation evidence flags;
- commercial providers, transport, capabilities, verification approvals, and
  final activation;
- durable IAP/rewarded-ad entitlement completion;
- Android package-lock approval, production identity, release APK/AAB, Gradle
  export, custom signing, Play publishing, and production approval;
- cloud save, account identity, cross-device merge, remote rollback, and
  support-assisted recovery;
- production crash/error transport.

No approved PNG, scene, resource catalog, economy/commercial activation file,
commercial configuration, Packages manifest, ProjectVersion, or
EditorBuildSettings byte is changed by Step 2.

## Source verification boundary

The Step 2 verifier requires the exact immutable Step 1 project for release
verification. It pins:

- 457 baseline files;
- Step 1 tree SHA-256
  `780e70e665604a414cd0c1887f223134a8c7aaff7cf648d81d3bdae25b007739`;
- Step 1 verifier SHA-256
  `9b99862be8e73e304edbc6a4203a207c5989839344282b4f04fea8bea1e77ee0`.

It runs that pinned verifier with `--source-only`, validates the exact Step 2
changed-file manifest, protects inherited sensitive files and all 65 approved
PNGs, rejects generated/package/plugin roots, and scans the Step 2 delta for
forbidden reset-all, cloud/provider, and activation changes. Exactly 12
inherited PlayMode fixtures may differ from Step 1; each must statically
snapshot, clear, and restore all three save keys without `PlayerPrefs.DeleteAll`,
and every other inherited test source or assembly must remain identical. Static
source checks do not prove Unity compiled or executed the candidate.

## Required Unity evidence

On one frozen Step 2 candidate in exactly Unity 6000.5.9f1, retain:

1. source archive SHA-256 and successful baseline verifier report;
2. compilation with zero errors;
3. project validator output;
4. Step 2 Editor self-test output;
5. complete EditMode and PlayMode NUnit XML with zero failed, skipped, or
   inconclusive tests;
6. deterministic envelope round-trip, tamper, truncation, future-version,
   migration v1-v6, current-v6 preservation, and interruption test results;
7. disposable-profile evidence for legacy import, normal relaunch, backup
   recovery, unrecoverable write guard, future-version write guard, explicit
   reset, offline, suspend/resume, and forced termination;
8. proof that raw corrupt/future primary, backup, and staging evidence survives
   a blocked boot without automatic default persistence.

Do not corrupt a real player profile. Export all three raw journal values before
each destructive test, use a disposable namespace/device profile, and restore
or destroy only that disposable profile afterward. Never use
`PlayerPrefs.DeleteAll()` as a test cleanup shortcut.

## Deferred work

- cloud/account recovery and cross-device conflict resolution;
- encrypted or authenticated local storage and platform keystore integration;
- user-facing recovery/support tooling and privacy/legal copy;
- durable atomic commercial entitlement and balance reconciliation;
- copy-on-write gameplay mutation transactions with failed-write rollback;
- closed/player-facing save testing and any transactional-durability claim;
- production crash/error reporting and consent-aware transport;
- production Android configuration, signing, package-lock approval, build, and
  publishing.

After the Step 2 Unity checkpoint is retained, the next development milestone
is v1.17 Step 3 — Crash and Error Reporting Foundation.
