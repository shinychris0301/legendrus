# Legends of Rus — v1.14 Step 4

This milestone completes the source-side provider and verification foundation for
audio, haptics, visual fidelity, and representative-device performance. The
existing deterministic gameplay and meta-UI feedback boundaries now feed a
persistent, settings-aware audio adapter and a rate-limited Android haptic
adapter. Unsupported platforms, missing clips, disabled settings, provider
failures, focus loss, and pause remain safe no-op conditions. Resume explicitly
re-enables haptic dispatch after a suspension.

Step 4 does not bundle approved or licensed audio clips. It defines canonical
`Resources/Audio` paths, preloads them once, records missing-clip diagnostics,
and safely stays silent until content is supplied. Audible output and Android
haptic behavior therefore require Unity and device verification.

The fidelity toolchain defines deterministic 16:9, 18:9, 19.5:9, and 20:9
portrait targets. Its matrix writes PNG-plus-metadata evidence and verifies
dimensions, route stability, and safe-area layout only; pixel fidelity is a
separate sidecar-required comparison with explicit Editor and Android
thresholds. Dynamic Gameplay pixel comparison remains pending a canonical board
fixture or dynamic-region mask.

In Editor or a Development Build, F6–F9 start the four fixed-size performance
scenarios and persist deterministic JSON; F10 cancels the active run. Release
builds expose a safe unsupported probe surface and never install the sampler.

Open this folder directly in Unity 6000.5.9f1, then run:

1. `Legends of Rus > Validate Commercial Vertical Slice`
2. `Legends of Rus > Tests > Run v1.14 Step 4 Self-Tests`
3. EditMode and PlayMode tests from Unity Test Runner
4. Run `Legends of Rus > QA > Run v1.14 Step 4 Capture/Layout Matrix (Play Mode)`
5. Compare eligible captures with `Compare Runtime Capture To Approved Design`
   (Gameplay needs a canonical fixture or mask before pixel certification)
6. Verify SFX, music settings, pause/focus behavior, and haptics on Android
7. Record F6–F9 scenarios and inspect the persisted JSON on Android; F10 cancels

The supplied static verifier checks source structure, provider and no-op
contracts, haptic throttling, fidelity targets, performance acceptance policy,
the exact release delta, and protected-file hashes against v1.14 Step 3. Static
checks do not certify Unity compilation, audible content, physical haptic
delivery, screenshot pixel fidelity, or device performance; those gates remain
pending.

Implementation details are in
`Assets/LegendsOfRus/Documentation/V1_14_STEP04_AUDIO_HAPTICS_FIDELITY_VERIFICATION.md`.
