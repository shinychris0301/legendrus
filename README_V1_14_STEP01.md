# Legends of Rus — v1.14 Step 1

This milestone establishes the deterministic motion and feedback foundation for
the Match-3 core loop. Swaps, blocked rejections, invalid returns, match waves,
cascades, special creation and activation, falls, reshuffles, and recovery now
publish ordered presentation hooks without changing board rules.

It also fixes the intermittent gesture-loss paths found in the gameplay shell:
cell raycast seams, the bottom-row booster overlap, early diagonal commitment,
unconfirmed swipe latching, stale pointer ownership, and display-only UI masks
that intercepted taps.

Open this folder directly in Unity 6000.5.9f1, then run:

1. `Legends of Rus > Validate Commercial Vertical Slice`
2. `Legends of Rus > Tests > Run v1.14 Step 1 Self-Tests`
3. EditMode and PlayMode tests from Unity Test Runner

Step 1 supplies provider-neutral SFX and haptic interfaces with safe no-op
defaults. It intentionally does not add audio clips, a mobile haptic provider,
or new artwork yet.

Implementation details are in
`Assets/LegendsOfRus/Documentation/V1_14_STEP01_CORE_LOOP_MOTION_FEEDBACK_FOUNDATION.md`.

