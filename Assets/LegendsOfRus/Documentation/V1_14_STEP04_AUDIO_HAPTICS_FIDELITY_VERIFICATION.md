# v1.14 Step 4 — Audio, Haptics, and Fidelity Verification

## Boundary

Step 4 integrates presentation providers and verification instrumentation. It
does not change approved PNGs, Chapter 1 data, board rules, level balance,
navigation destinations, economy or rewards, story order or copy, save schema,
scenes, or project settings. The package manifest declares Unity's built-in
audio module for the concrete adapter and Android JNI module for the guarded
device haptic bridge; all other dependencies remain unchanged.

## Audio and feedback provider

- `ProductionFeedbackProvider` is a persistent concrete SFX sink for both the
  core-loop and meta-UI services. Initialization prewarms four two-dimensional
  SFX voices, one looping music source, and one provider-owned listener. That
  listener disables itself whenever another active scene listener exists.
- Canonical `Resources/Audio` paths are loaded once. Cue-time paths use fixed
  arrays, bounded cooldowns, and round-robin voice selection rather than
  creating sources or loading resources during feedback playback.
- Match size, cascade depth, button role, map selection, reward, victory,
  failure, and Chapter Complete map to restrained volume, pitch, and cooldown
  recipes. Mapping changes presentation only; it cannot alter the command or
  route that emitted the cue.
- Saved SFX, music, and vibration settings are enforced at dispatch time.
  Pause, focus loss, and setting changes gate or suspend output while keeping
  the installed sinks bound. Component disable, provider replacement, and
  teardown restore provider-neutral no-op sinks.
- Missing clips are counted and skipped. Step 4 contains no approved or
  licensed SFX/music files, so source integration is complete while audible
  output remains intentionally silent until content is supplied and verified.

## Haptics

- `PlatformHapticProvider` translates semantic Selection, Confirm, Emphasis,
  and Reject intents. A monotonic 60 ms rate limiter suppresses equal or weaker
  duplicate requests while allowing a stronger intent to replace a weaker one.
- Android uses `View.performHapticFeedback` on Unity's UI thread. The platform,
  OEM, and system touch-feedback setting select the physical effect; no raw
  vibration duration is hard-coded and no vibration permission is requested.
- Pending Android work carries a generation token. Pause, focus loss, disposal,
  or provider replacement invalidates stale callbacks before they touch the
  Unity-owned player view.
- Suspension gates all new requests until `Resume` is called; resuming starts a
  fresh throttle window. `AcceptedPulseCount` means the bridge accepted or
  queued a semantic request. It does not prove that the OS or hardware produced
  a physical pulse.
- Editor and unsupported targets use a null bridge. All bridge failures are
  isolated from gameplay, meta actions, and shutdown.

## Meta-UI feedback boundary

`MetaUiFeedbackService` converts only accepted start-phase button actions and
supported modal/outcome reveals into provider signals. Completion traces and
gameplay-role hotspots do not emit duplicate meta cues. Like the core-loop
service, it preserves safe null sinks, setting gates, and per-listener exception
isolation.

## Fidelity captures and comparison

- `FidelityVerification` defines four portrait targets: 1080×1920, 1080×2160,
  1080×2340, and 1080×2400. A pure fit calculation evaluates the locked
  941×1672 design viewport inside a normalized safe area with a one-pixel layout
  tolerance.
- `AdaptiveCanvasScaler` uses the same limiting-dimension rule and
  `SafeAreaFitter` uses the shared safe-area normalization, keeping runtime
  layout aligned with the fidelity evaluator. The scaler applies the safe-area
  fit directly so inset changes do not alter the locked 941×1672 canvas units.
- `QACaptureService` captures the rendered end-of-frame output with `ReadPixels`
  and writes a PNG plus JSON sidecar. Metadata records screen/modal identity,
  actual and requested dimensions, safe area, viewport, canvas scale, route
  generation, transition state, Unity/device/graphics context, and layout
  result. A generation token cancels stale requests, and a requested-resolution
  mismatch fails before any mislabeled PNG or sidecar is written.
- `FidelityVerificationRunner` is a four-target capture/layout matrix. It checks
  dimensions, route generation, transition stability, and layout, then restores
  the original resolution. Its PASS explicitly does not claim pixel fidelity.
- `DesignReferenceComparator` requires a valid v1.14 Step 4 metadata sidecar,
  verifies that it names the selected PNG and records no active transition,
  crops the recorded viewport, aspect-fits the approved artwork inside it, and
  publishes a grayscale diff plus JSON metrics. Editor limits are mean RGB
  error ≤0.025, p95 ≤0.06, and bad-pixel fraction ≤1%; Android limits are ≤0.04,
  ≤0.10, and ≤3%, with 0.10 as the bad-pixel threshold.

Gameplay contains a generated board that cannot truthfully match the fixed board
inside the approved PNG. Gameplay pixel certification therefore remains pending
a canonical visual fixture or a reviewed dynamic-region mask. The capture/layout
matrix can still verify its dimensions, route, transitions, and safe-area fit.

No runtime capture or visual comparison was executed in this environment.
Passing source invariants is not a fidelity result.

## Representative-device performance gate

`RuntimePerformanceProbe` exposes a deterministic unsupported stub in release
builds. The real sampler is compiled and bootstrap-installed only for the Editor
or development builds. It listens for verification shortcuts but starts
sampling only after `Begin`, `BeginAndPersist`, or F6–F9. Its accumulator
preallocates fixed arrays for at most 3,600 frames and rejects zero or duplicate
frame timestamps. The default run is 120 warmup frames plus 1,800 measured
frames.

- F6: Gameplay Idle
- F7: Gameplay Active
- F8: Meta Idle
- F9: Meta Transition
- F10: cancel the active run as INCOMPLETE

Shortcut runs persist deterministic JSON under
`Application.persistentDataPath/PerformanceQARuns/v1_14_step04`, one stable file
per scenario. File I/O occurs only after sampling stops.

A PASS requires all of the following:

- configured `Application.targetFrameRate` equal to 60 (this is configuration
  evidence, not a measured frame rate);
- at least 600 complete samples;
- valid CPU and managed-allocation samples for every measured frame;
- GPU timing coverage of at least 90%;
- CPU total and GPU p95 ≤18 ms and p99 ≤25 ms;
- frames slower than 33.333 ms at or below 1%;
- zero aggregate `GC Allocated In Frame` bytes across Gameplay Idle and Meta
  Idle sample windows.

These thresholds form a 60 fps tolerance gate over the sampled distribution and
tail. They do not guarantee that every frame completes within 16.667 ms.

Allocation counters are aggregate-window evidence, not attribution to a single
component. Gameplay Active and Meta Transition allocation fields are diagnostic
and do not apply the idle zero-allocation rule; accepted core signals and the
Android haptic bridge may allocate during active interaction. A non-development
build, unavailable FrameTiming, missing required counters, insufficient samples,
or interruption produces INCOMPLETE, never a false PASS. The probe is
observational and does not mutate quality, time, gameplay, or economy state. No
representative Android profile was executed in this environment.

## Verification status

Static verification parses every project JSON file, scans C# delimiters and
preprocessor blocks, checks provider/audio-and-Android-JNI-module, haptic,
fidelity, performance, and safe-no-op contracts, counts Unity tests and Editor
self-checks, compares all
65 PNGs, hashes protected gameplay/story/economy/save/scene/project-setting
boundaries, and verifies the exact release manifest against v1.14 Step 3.

Unity 6000.5.9f1 compilation, Unity Test Runner, audible content, capture
comparison, Android haptics, and representative-device profiling remain pending
release gates.
