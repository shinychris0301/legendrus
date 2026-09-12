# v1.20 Step 7.4 — Unity Runtime Capture, Dynamic Map-State Decomposition, and Conditional Wave 1 Cutover

This milestone converts the remaining Wave 1 visual activation work into an evidence-gated process. It does **not** pre-apply a cutover in the distributed project.

## What changed

- Step 7.4 candidate builder version added; Unity import rebuilds the 12 Wave 1 candidates.
- World Map now owns 15 live visual slots windowed over 20 logical chapters.
- Chapter Map now owns 7 live visual slots windowed over the current 10 authored Chapter 1 levels.
- Map node number, star count, lock state and selected state are live UI bound to `GameState`; the painted route/environment is decorative only.
- The approved painted node/star pixels are covered at their measured anchors by opaque live state assets, so they are never authoritative progression state.
- Unity Play-Mode capture harness renders 12 candidates at four aspect targets (48 captures) without `ScreenCapture`.
- Runtime fidelity analyzer compares the 9 directly referenced pages; Gameplay uses the non-board shell exception.
- Conditional development cutover can replace only the eight directly referenced pages already present in the active registry.
- Cutover is explicit and reversible; the original active prefab YAML is backed up before replacement.
- Post-cutover physical Android acceptance is mandatory. If Android QA fails, roll back.

## Unity execution order

1. Let Unity finish import/compile. The candidate builder should repair the Step 7.4 candidates automatically.
2. `Legends of Rus > Tests > Run v1.20 Step 7.4 Source Readiness Self-Test`
3. `Legends of Rus > Tests > Run v1.20 Step 7.4 Unity Runtime Four-Aspect Capture`
4. `Legends of Rus > Tests > Analyze v1.20 Step 7.4 Unity Runtime Capture Fidelity`
5. `Legends of Rus > Tests > Run v1.20 Step 7.4 Dynamic Map-State QA`
6. `Legends of Rus > Production UI > v1.20 Step 7.4 > Evaluate Conditional Wave 1 Cutover`
7. If eligible and the captures look correct, use `Apply Eligible 8-Page Development Cutover`.
8. Re-run the full Step 5 functional QA / EditMode / PlayMode suites.
9. Build to physical Android and test safe area, touch, visible/system Back, dynamic maps and Cyrillic rendering.
10. If device QA passes, use `Record Post-Cutover Physical Android QA PASS`; if not, use `Roll Back Eligible 8-Page Cutover`.
11. Re-run `Evaluate Conditional Wave 1 Cutover` to record accepted state.

## Four target captures

- 941 × 1672 (9:16 design reference)
- 1080 × 2340 (19.5:9)
- 1080 × 2400 (20:9)
- 1600 × 2560 (10:16 tablet)

## Cutover scope

Eligible in Step 7.4: Loading, Home, WorldMap, ChapterMap, PreLevel, Gameplay, Victory, Fail.

Deferred: SplashTitle, Pause and ChapterComplete still lack direct approved references. Dialogue has a direct reference but remains candidate-only because changing the active registry is intentionally outside this visual-only checkpoint.

The active registry remains at the accepted 10-page baseline until a later explicit registry milestone.
