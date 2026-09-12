# Legends of Rus — v1.17 Step 4

## Memory, Texture, and Loading Optimization Foundation

Status:
`IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_STEP03_PREREQUISITES_AND_STEP04_UNITY_DEVICE_VERIFICATION`

This source candidate adds a bounded, data-minimized optimization foundation
without changing approved artwork, gameplay rules, save schema, package lock,
economy authority, or commercial/provider activation.

Implemented in source:

- an engine-free `LegendsOfRus.OptimizationCore` with pinned development memory
  and load guardrails, deterministic budget evaluation, a fixed-capacity
  process-memory evidence ring, and monotonic loading-state tracking;
- a single runtime optimization service that samples memory only in Editor or
  Development Build, listens for Unity low-memory notifications, and coalesces
  `Resources.UnloadUnusedAssets()` cleanup rather than allowing overlapping
  cleanup work;
- screen-retirement accounting so routine cleanup happens only after a bounded
  number of retired screens and outside Gameplay/Dialogue route entry;
- bootstrap and screen-route timing evidence with no player/device identifiers,
  persistence, upload, provider SDK, or background network work;
- a texture audit for the locked runtime art contract, including Sprite import,
  Read/Write disabled, mipmaps disabled, 2048 maximum, and the inherited Android
  ASTC 6x6 override;
- dependency-free Editor self-tests plus EditMode and PlayMode regression tests;
- an immutable source texture inventory. The 47 Resources PNGs have an
  estimated 136.78 MiB uncompressed RGBA source upper bound, of which the 22
  full design screens account for 132.04 MiB. This is not a measured resident
  memory value and must not be reported as device usage.

The v1.17 Step 2 and Step 3 retained Unity/device prerequisites are still absent.
This Step 4 package therefore remains an at-risk source candidate. Static source
verification can prove the protected baseline and source invariants, but Unity
compilation, test execution, actual device memory, load timing, and low-memory
behavior must still be retained from Unity 6000.5.9f1 and representative Android
hardware.

Start with
`Assets/LegendsOfRus/Documentation/V1_17_STEP04_MEMORY_TEXTURE_LOADING_OPTIMIZATION_FOUNDATION.md`.
