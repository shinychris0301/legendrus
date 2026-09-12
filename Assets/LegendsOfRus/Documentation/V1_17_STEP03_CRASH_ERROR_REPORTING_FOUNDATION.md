# v1.17 Step 3 — Crash and Error Reporting Foundation

## Status

`IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_PREREQUISITE_AND_STEP03_UNITY_VERIFICATION`

Candidate boundary:
`CRASH_ERROR_REPORTING_FOUNDATION_DETACHED_PRODUCTION_TRANSPORT_BLOCKED`

The user authorized continued source development, but the frozen v1.17 Step 2
Unity and representative-device evidence required by the recorded Step 3 entry
gate is not retained. This work is therefore an at-risk, detached pre-entry
source candidate. It must not be represented as a formally entered,
Unity-verified, player-facing, or production-ready milestone.

## Implemented boundary

- A new `LegendsOfRus.DiagnosticsCore` assembly is engine-free and has no
  dependency on Unity, Runtime, SaveCore, CommercialCore, analytics, consent,
  monetization, or a provider SDK.
- Capture accepts a four-field draft containing only closed incident,
  subsystem, `ErrorReportCode`, and `ErrorReportSiteCode` enum values, plus a
  separately injected UTC tick value. Reviewed uppercase/lowercase wire codes
  are generated internally; callers cannot supply code strings.
- Records contain only schema/order information, a deterministic fingerprint,
  the validated code tuple, bounded occurrence data, and bounded delivery
  state. There is no message, stack trace, exception object, dictionary,
  attachment, identifier, payload, or user-entered text field.
- A lower-hex SHA-256 fingerprint is derived only from the canonical validated
  code tuple. Raw logs, messages, stacks, paths, payloads, and identifiers are
  never fingerprint inputs.
- The process-memory buffer has a conservative default capacity of 64 and a
  compiled maximum of 256. A full buffer rejects a new incident and preserves
  existing evidence; an eligible duplicate may still coalesce without growth.
- Duplicate coalescing uses an inclusive ten-minute default window. The first
  timestamp and sequence remain stable, the last timestamp and bounded
  occurrence count advance, and delivery state is not reset.
- Hard retention is measured from first occurrence, so a repeated incident
  cannot keep itself indefinitely. The default is seven days and the compiled
  maximum is thirty days. Expiry is evaluated when capture, flush, or explicit
  prune receives an injected timestamp; there is no timer or background expiry.
- Retry state uses injected time, a default maximum of five attempts, and
  deterministic exponential backoff from 30 seconds up to one hour. Offline,
  disabled, network-blocked, and not-yet-due cases do not consume attempts.
- Canonical JSON is an inspection and test representation only. It uses fixed
  key order, invariant numbers, sequence order, and no file or network I/O.
- Adapter capability checks, sends, null results, and thrown exceptions are
  contained as stable results. Delivery handles at most one due record per
  call.

## Deliberately absent

There is no `Application.logMessageReceived` or
`Application.logMessageReceivedThreaded` subscription. Existing Console
output may contain exception text, filesystem paths, QA replay tokens, device
metadata, and future provider details; sweeping it into a report would violate
the data-minimization contract.

There is also no runtime bootstrap, GameObject, background thread, timer, file
persistence, endpoint, credential, network client, SDK, native plugin, crash
handler, or provider binding. The public API cannot inject a transport. The
only shipped production path uses the disabled transport, and the compiled
network-transport gate is `false`. An internal friend-only test seam establishes
failure semantics without authorizing a production provider.

The buffer is process memory, not durable crash recovery. A later reviewed step
must separately decide whether minimized records may be persisted, which
consent and regional rules apply, which provider is permitted, how retention
and deletion work, and how native crashes are handled. None of that is implied
by this foundation.

## Data-minimization rules

The public model intentionally cannot carry:

- exception messages, stack traces, Console conditions, or unrestricted logs;
- player, account, installation, device, advertising, session, or analytics
  identifiers;
- save JSON, envelope text, PlayerPrefs values, board replay tokens,
  screenshots, paths, filenames, or device metadata;
- purchase, receipt, transaction, product, placement, reward, proof, or
  entitlement material;
- credentials, secrets, tokens, authorization headers, endpoints, consent
  snapshots, IP addresses, email addresses, or user-entered/localized text.

Callers must map a known failure site to the reviewed enum catalog before
capture. Adding a new code or site requires a reviewed source change; arbitrary
strings have no representation in the draft contract.
Ordinary invalid player actions, such as a rejected Match-3 swap, are not
errors and must not be reported.

## Verification

Run the source boundary check from a clean extraction:

```text
node Tools/verify_v1_17_step03.mjs --source-only
```

Release-boundary verification also requires the exact frozen Step 2 project:

```text
node Tools/verify_v1_17_step03.mjs --baseline <v1.17-step02-project>
```

Static verification pins reviewed source markers and discovers test declarations
for deterministic fingerprints and serialization, strict public-model
minimization, bounded policy constants, capacity, deduplication, retention,
retry/offline behavior, adapter isolation, assembly boundaries, the exact
changed-file delta, inherited test equality, and every existing production hard
stop. It does not execute C# behavior; static discovery and source test counts
are not Unity execution evidence.

Before this milestone can pass:

1. retain and accept all exact Step 2 prerequisite evidence;
2. compile the exact Step 3 candidate in Unity 6000.5.9f1 with zero errors;
3. retain the project validator and Step 3 Editor self-test output;
4. retain complete EditMode and PlayMode NUnit XML with zero failures, skips,
   or inconclusive tests;
5. verify fixed-clock capacity, deduplication, retention, offline, retry,
   exhaustion, permanent failure, throwing/null adapter, and eventual-success
   cases;
6. confirm pure PlayMode tests create no GameObject and preserve the three save
   keys byte-for-byte;
7. confirm no raw data, provider, persistence, log subscription, or network
   path is present.

## Hard stops

`ResolvedPackageLockApproved` remains `false`. Production crash transport,
runtime economy, commercial providers, cloud recovery, Android preflight and
build, signing, AAB generation, Play publishing, and all final activation
remain unauthorized. Step 3 also makes no transactional save-durability claim;
copy-on-write gameplay mutation transactions are still required before
closed/player-facing testing.
