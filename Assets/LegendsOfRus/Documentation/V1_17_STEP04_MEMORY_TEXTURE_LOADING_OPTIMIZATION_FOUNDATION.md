# v1.17 Step 4 — Memory, Texture, and Loading Optimization Foundation

## Status

`IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_STEP03_PREREQUISITES_AND_STEP04_UNITY_DEVICE_VERIFICATION`

Candidate boundary:
`MEMORY_TEXTURE_LOADING_OPTIMIZATION_FOUNDATION_SOURCE_CANDIDATE_PRODUCTION_GATES_BLOCKED`

Step 4 was prepared at the user's direction on top of the frozen Step 3 source
candidate. The recorded Step 2 and Step 3 Unity/device evidence required for
formal entry is not retained. This work must therefore be described as
source-implemented and statically verified only until the exact bytes are run
and evidence is retained in Unity 6000.5.9f1 and on representative Android
hardware.

## Implemented foundation

### Engine-free policy and evidence

`LegendsOfRus.OptimizationCore` has no Unity dependency. It contains:

- pinned development guardrails for allocated, reserved, and managed memory;
- a 32 MiB repeated-route steady-growth guardrail;
- a 250 ms screen-route development budget, 750 ms cleanup budget, and 5 s
  bootstrap development budget;
- deterministic inclusive budget classification;
- a fixed-capacity process-memory evidence ring (64 default, 128 compiled max);
- deterministic inspection JSON with sequence/type/numeric measurements only;
- a monotonic bootstrap loading-state tracker.

These constants are investigation thresholds, not proof that a device passes.
Actual production budgets may be tightened only after representative-device
measurements are retained.

### Runtime memory and lifetime control

`RuntimeOptimizationService` is created on the existing persistent bootstrap
root. It is intentionally local and bounded:

- Editor/Development Build sampling uses Unity Profiler allocated/reserved
  process memory plus managed heap estimates;
- `Application.lowMemory` records a minimized local signal and requests cleanup;
- cleanup is coalesced so only one `Resources.UnloadUnusedAssets()` operation is
  active at once;
- routine cleanup waits for at least three retired screens and a ten-second
  cooldown, while a low-memory signal may bypass that routine cooldown;
- one frame is yielded before cleanup so deferred `Destroy` operations can
  retire screen hierarchy references first;
- entering Gameplay or Dialogue does not trigger routine cleanup, avoiding a
  deliberate route-entry hitch on those latency-sensitive paths;
- no explicit `GC.Collect`, file/PlayerPrefs persistence, endpoint, identifier,
  provider SDK, analytics event, or network transport is introduced.

### Loading instrumentation

Bootstrap records the phases Bootstrap → Local State → UI Construction → Ready.
Screen routes record local construction duration. The bounded evidence model is
process-memory only and is not a telemetry product.

### Texture optimization and audit

The existing locked-design importer retains the approved visual bytes and the
inherited Android ASTC 6x6 format while explicitly enforcing:

- `TextureImporterType.Sprite`;
- `isReadable = false` so runtime textures do not retain an unnecessary CPU-side
  readable copy;
- mipmaps disabled for the 2D UI/gameplay art contract;
- maximum runtime texture dimension 2048;
- Android override enabled, maximum 2048, ASTC 6x6.

`TextureOptimizationAudit` scans every `Assets/LegendsOfRus` texture and fails
runtime-art violations without mutating the approved PNGs. Small gameplay/UI
sprite atlas candidates are reported only as informational warnings. SpriteAtlas
or Addressables conversion is deliberately deferred to the next content-delivery
foundation because changing resource identity/grouping is outside Step 4.

### Source inventory

`V1_17_STEP04_SOURCE_TEXTURE_INVENTORY.json` records dimensions, source byte
sizes, source SHA-256 values, Resources membership, and a simple RGBA source
upper-bound estimate. Current inventory:

- approved PNG files: 65;
- PNG files under runtime Resources: 47;
- full design screen PNGs: 22;
- estimated uncompressed RGBA upper bound for all Resources PNGs: 136.78 MiB;
- estimated uncompressed RGBA upper bound for full design screens: 132.04 MiB.

These values are deliberately labeled source estimates. They are not runtime
residency, GPU allocation, Unity heap usage, or representative-device evidence.

## Verification in Unity

From a clean extraction of this exact candidate:

1. Open with Unity `6000.5.9f1` and retain zero-error compilation output.
2. Run `Legends of Rus/Validate Commercial Vertical Slice` and retain the result.
3. Run `Legends of Rus/Tests/Run v1.17 Step 4 Texture Audit`.
4. Run `Legends of Rus/Tests/Run v1.17 Step 4 Self-Tests`.
5. Run the complete EditMode and PlayMode suites and retain NUnit XML with zero
   failures, skips, or inconclusive tests.
6. On a disposable profile, repeat Home → Pre-Level → Gameplay → Victory/Fail →
   Home enough times to expose retained-screen growth. Record initial and final
   allocated/reserved memory after comparable settled points; investigate growth
   above the 32 MiB development guardrail.
7. Exercise a low-memory test path on representative Android hardware or an
   approved development harness and verify gameplay/save continuity after cleanup.
8. Retain bootstrap and representative screen-route timing evidence. Treat the
   250 ms/750 ms/5 s values as development alarms, not certified production SLAs.

Static source verification is run with:

```text
node Tools/verify_v1_17_step04.mjs --baseline <exact-v1.17-step03-project>
```

Static verification does not execute C# and does not substitute for any Unity or
Android evidence above.

## Hard stops preserved

This step does not authorize Addressables/remote content delivery, production
crash transport, analytics upload, provider SDKs, cloud recovery, production
Android build/signing/AAB generation, package-lock approval, economy activation,
commercial activation, or Play publishing. `ResolvedPackageLockApproved` and all
existing commercial/economy runtime activation gates remain false.

The next source milestone after accepted Step 4 verification is **v1.17 Step 5 —
Addressables and Content Delivery Foundation**.
