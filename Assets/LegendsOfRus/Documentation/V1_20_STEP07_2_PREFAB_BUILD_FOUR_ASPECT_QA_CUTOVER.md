# v1.20 Step 7.2 — Candidate Prefab Build, Four-Aspect Visual QA, Cutover Decision

## Result

- Serialized Wave 1 candidate prefabs physically shipped: **12/12**.
- Active `Resources/ProductionUI/Pages` prefabs: **unchanged from Step 7**.
- Runtime production registry: **unchanged; 10 active cutover pages**.
- Source geometry containment across 9:16, 19.5:9, 20:9 and tablet 10:16: **48/48 PASS**.
- Direct approved page references available: **9/12**.
- Strict direct-reference visual proxy: **0/9 PASS**.
- Production Cyrillic typography: **pending licensed font import**.
- Unity actual capture: **pending execution in Unity 6000.5.9f1**.
- Android device visual/touch/back acceptance: **pending**.
- **Wave 1 active visual cutover decision: NO-GO.**

The no-go decision is intentional. It protects the accepted game logic from being covered by a visually incomplete candidate layer. Step 7.2 does not change the active production registry.

## Unity execution

1. Open the exact Step 7.2 project in Unity 6000.5.9f1 and allow asset import.
2. Run `Legends of Rus > Production UI > Validate v1.20 Step 7.2 Wave 1 Candidate Prefabs`.
3. Run `Legends of Rus > Tests > Run v1.20 Step 7.2 Candidate Prefab + Cutover Gate Self-Test`.
4. Run `Legends of Rus > Tests > Run v1.20 Step 7.2 Four-Aspect Candidate Visual QA` to produce 48 actual Unity captures.
5. Run the existing functional EditMode/PlayMode suites and Android Back/touch verification.
6. Do not activate Wave 1 until every blocker in `V1_20_STEP07_2_CUTOVER_DECISION.json` is closed.
