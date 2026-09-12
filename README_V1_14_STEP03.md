# Legends of Rus — v1.14 Step 3

This milestone adds deterministic, presentation-only motion to the meta UI while
preserving the approved 941×1672 artwork and responsive safe-area viewport.
Buttons receive repeatable press/tap feedback, map interactions receive a local
non-raycast pulse, full-screen content is input-gated while entering, and modal
blockers remain active throughout alpha-only entry and exit transitions.

Victory, failure, reward, and Chapter Complete screens use distinct bounded
reveals. All progress, reward, story, navigation, and save decisions still occur
in their existing authoritative services before presentation begins. The motion
layer cannot grant currency, select levels, acknowledge story, resume a board,
or change a route.

Open this folder directly in Unity 6000.5.9f1, then run:

1. `Legends of Rus > Validate Commercial Vertical Slice`
2. `Legends of Rus > Tests > Run v1.14 Step 3 Self-Tests`
3. EditMode and PlayMode tests from Unity Test Runner
4. Capture comparison at 16:9, 18:9, 19.5:9, and 20:9
5. Representative portrait Android allocation and frame-pacing checks

The supplied static verifier checks source structure, the fixed motion clock,
input gates, modal blocker lifetime, map/outcome presentation isolation, exact
changed-file scope, and byte identity of protected files against v1.14 Step 2.
Unity compilation, Test Runner execution, capture comparison, and device
profiling remain required release gates.

Implementation details are in
`Assets/LegendsOfRus/Documentation/V1_14_STEP03_META_UI_MOTION_TRANSITION_POLISH.md`.
