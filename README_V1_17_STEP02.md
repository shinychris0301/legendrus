# Legends of Rus — v1.17 Step 2

## Save Migration and Recovery Foundation

Status: `IMPLEMENTED_IN_SOURCE_PENDING_UNITY_SAVE_RECOVERY_VERIFICATION`

Candidate status:
`SAVE_V6_ENVELOPE_V1_RECOVERY_FOUNDATION_PRODUCTION_ACTIVATION_BLOCKED`

This milestone replaces the inherited single-value, best-effort save path with
a versioned storage envelope, explicit migration registry, staged writes,
committed backup recovery, structured diagnostics, and fail-closed handling of
unsupported future data. The player payload remains version 6 and the inherited
primary key remains `LegendsOfRus_Save_v1`; envelope version 1 is a separate
storage format version.

The user authorized Step 2 after reporting that the previous candidate ran
without errors. That report is not retained Unity evidence for these Step 2
source bytes. The Step 1 Unity-owned `ProjectSettings.asset` and resolved
`packages-lock.json` are still absent, the compiled
`ResolvedPackageLockApproved` gate remains `false`, and no Android preflight or
APK build is authorized.

### Shipped boundary

- `LegendsOfRus.SaveCore` is engine-free and owns deterministic envelope and
  journal decisions.
- Envelope v1 records operation intent, payload version, monotonic revision,
  UTF-8 byte count, payload SHA-256, canonical Base64 payload, and envelope
  SHA-256.
- SHA-256 detects accidental corruption. It is not authentication, encryption,
  anti-tamper protection, or proof that data came from a trusted server.
- Normal writes stage and flush a new envelope, preserve the prior valid
  primary as backup, commit and flush the new primary, then remove staging.
- A valid primary is authoritative. A missing primary is an explicit no-save
  boundary and never resurrects backup-only history. Staging is interruption
  evidence and is never promoted by load. A committed backup may recover an
  invalid primary only when recovery is safe.
- Explicit history replacement writes the reset state into both primary and
  backup so a later corruption cannot resurrect pre-reset progress.
- Unversioned and raw v1-v5 payloads retain migration compatibility. A raw
  current-v6 import must contain the complete typed 32-field DTO with valid
  top-level field types and no duplicate known field. Migration is
  copy-on-write, follows the existing v1-to-v6 semantics, and persists only
  after a valid result.
- A canonical future raw payload or future envelope remains authoritative
  unsupported-version evidence. Canonical differently shaped and oversized
  future envelopes are classified before v1 layout parsing and block downgrade.
- If a reset primary is durably flushed but backup or staging cleanup then
  fails, `PrimaryDurablyCommitted` allows GameState to adopt the reset while
  retaining the incomplete-journal warning and non-persistable status.
- Unsupported future envelope or payload versions, storage failures, and
  unrecoverable corruption are explicit states. Every write rescans the journal
  and is blocked while that evidence remains, so an in-memory fallback cannot
  overwrite it. Clearing the block requires explicit external remediation.
- `Load()` and `Save(PlayerSaveData)` remain as compatibility facades; the
  structured APIs expose the actual source, status, phase, revision, and stable
  diagnostic code.

This is local recovery for the existing PlayerPrefs save only. It does not add
cloud save, account recovery, cross-device sync, rollback UI, production
telemetry, a durable commercial-entitlement ledger, or economy authority.

### Verification

From a separate clean extraction of the Step 2 source project:

```text
node Tools/verify_v1_17_step02.mjs --source-only
```

Release-boundary verification also requires the immutable Step 1 project:

```text
node Tools/verify_v1_17_step02.mjs --baseline <v1.17-step01-project>
```

The verifier pins the 457-file Step 1 tree and its source verifier, validates
the exact Step 2 delta, protects activation/configuration/package/scene/PNG
bytes, confirms all activation gates remain false, and confirms the inherited
package-lock approval gate remains false. Twelve inherited PlayMode fixtures
are the only permitted inherited test changes; each snapshots, clears, and
restores the primary, backup, and staging namespace without `DeleteAll`, while
every other inherited test remains byte-identical. Static discovery and source
test counts are inventory, not Unity execution results.

Then open a working copy in exactly Unity 6000.5.9f1 and retain:

1. compilation with zero errors;
2. `Legends of Rus/Validate Commercial Vertical Slice` output;
3. `Legends of Rus/Tests/Run v1.17 Step 2 Self-Tests` output;
4. complete EditMode and PlayMode NUnit XML with zero failures, skips, or
   inconclusive tests;
5. manual first-launch, legacy-save, normal relaunch, backup-recovery,
   unrecoverable-corruption, future-version, explicit-reset, offline,
   suspend/resume, and forced-termination checks on disposable profiles;
6. proof that corrupted or future raw evidence is retained and is not replaced
   by an automatically saved default.

Use synthetic test profiles only. Keep copies of raw primary, backup, and
staging values before destructive corruption tests. Do not use
`PlayerPrefs.DeleteAll()` on a real profile.

Legacy GameState mutator booleans still report gameplay acceptance rather than
durable commit. Copy-on-write mutation transactions that adopt state only after
a successful structured write remain required before closed/player-facing
testing or any transactional-durability claim.

Detailed contracts and acceptance evidence are in
`Assets/LegendsOfRus/Documentation/V1_17_STEP02_SAVE_MIGRATION_RECOVERY_FOUNDATION.md`.
