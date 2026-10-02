# Error Dialog — Responsive Audit R2

Continued after Maintenance with the existing active EmptyErrorState prefab. No production defect was found in this audit. The approved panel, source Russian title/button lettering, repaired parchment, whole sprites and existing revision 2 remain unchanged; no prefab rebuild was necessary.

## Verification added

Extended ErrorDialogAlignmentPlayModeTests.cs with native EventSystem raycasts and pointer-click dispatch for disabled Retry/Support, shade clicks, Close edge taps and enabled Retry edge taps in Russian and English. These are automated Unity input-handler checks, not physical touchscreen input.

The checks cover short 941×1450, reference 941×1672, narrow 576×1280 and tall 1080×2400 displays, with simulated safe areas. They assert full sprite bounds, safe placement, active targets at least 44 rendered pixels, non-overlapping buttons, complete live text, all four screen corners blocked by the shade, relocalization, preservation of the underlying Home instance/route/save and exactly one Retry callback after modal dismissal. Native Retry can then route to Shop. Existing route-change cancellation, Back, duplicate-request rejection, disabled press feedback and source-lettering tests also passed.

Capture validation now occurs after rendering, ensuring raycast depth reflects the capture camera layout. Captures are saved before navigation callbacks execute.

## Results

Verify01: Unity 6000.5.9f1, isolated test configuration/save storage. Two EditMode and eight PlayMode tests passed: 10 total, zero failed/skipped. EditMode checked active prefab bindings, cutover, no reference-image dependencies/extra Canvas and runtime builder idempotence. Sixteen actual Unity captures were produced. Twelve standalone review views and native-pixel toggle passed browser checks without overflow or script errors.

Inspected narrow English with Retry enabled, tall Russian after language change, short English, the prior narrow capture and the original reference. Panel ornaments, buttons and descriptions fit. Support remains disabled; it is not treated as an active minimum-size target. Retry remains disabled unless a caller supplies an action.

All six matching R1 captures have pixel-identical sampled opaque panel interiors. Full screenshots differ: current Home safe-edge placement and other previously changed Home details appear behind the shade; three reference-sized generic/pressed captures differ by only 10–118 pixels with maximum channel delta one. See QA/BaselineComparison.json for exact bounds/counts. This is not a claim of full-screen pixel identity.

## Change scope and limits

Only Assets/LegendsOfRus/Tests/PlayMode/ErrorDialogAlignmentPlayModeTests.cs changed under Assets compared with the start-of-task snapshot; 7,195 other files remained unchanged, none added/deleted. Added this review, scripts, reports, captures and QA evidence; updated CurrentTask.md and SessionLog.md. No runtime, prefab, builder, art, font or meta changes, generation or Scenario calls.

No physical Android/device, backend, support-service, performance or full-suite testing. The existing router intentionally defers Error Dialog integration into Gameplay; this audit does not change that behavior. Reconstructed parchment/English surfaces and fallback font differences remain. Next: Visual Page 35 — Story Dialogues responsive/navigation audit.
