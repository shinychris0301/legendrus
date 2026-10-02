# Events R1 — original-reference fidelity review

## Verdict

R1 matches the broad dark navy, antique gold and fairy-tale theme. It does **not** closely reproduce the original composition or finish. The previous 17 passing Unity tests establish behavior and layout constraints, not reference fidelity. Visual acceptance is not established.

Compared the original `Assets/LegendsOfRus/Resources/DesignScreens/Events.png` directly with the recorded actual Unity `UnityVerification/Events/Events_941x1672_RU.png`, both941×1672. Also inspected owning runtime, serialized prefab, capture metadata and review-page linkage. All six current Events asset hashes match the generation provenance; reference hash matches recorded source. This review did not launch Unity or create a new capture.

## Visible discrepancies

| Priority | Element | Reference vs current | Recommended correction |
|---|---|---|---|
| High | Card proportions | First two reference cards are taller; R1 makes all four275px high. Lower cards consequently begin much higher, exposing more scenery above the footer. | Restore the unequal reference row heights and vertical rhythm. Reflow the footer while preserving a visible Back control. |
| High | Events heading | Reference has large raised/beveled gold lettering and a richer, broader plaque. R1 has smaller flat pale-gold type and a simpler plaque. | Match visible plaque footprint and lettering scale/position; improve gold depth with the project font, whose exact source identity remains unknown. |
| High | Event typography | Reference event names and countdowns have stronger gold weight/depth; R1 is flatter and lighter, with weaker hierarchy. Single-line merchant/knight names sit differently because every card shares the same two-line title box. | Give one-/two-line titles appropriate alignment and reproduce title/status/body hierarchy. |
| High | Status/clock row | Reference has clock icons and narrow bordered status plaques; R1 replaces them with an unframed line of text. | Restore the clock and framed status treatment with truthful date-pending text. Lack of a schedule does not require removing this visual structure. |
| High | Reward strip | Each reference card contains a substantial row of reward illustrations; R1 replaces the entire row with a small wrapped message. | Restore its visual space and ornamentation. Use clearly labelled previews only if the reward concepts are approved; keep rewards unannounced otherwise. No invented amounts or grants. |
| Medium | Event paintings | All four subjects are recognizable, but framing differs substantially. R1 Firebird is smaller within a wider city view; Baba Yaga's face/hat/gesture and cauldron differ; merchant pose/costume/city differ; knight/shield angle and architecture differ. | First restore card/image geometry; then refine only the most visible pose/framing differences through the established Scenario workflow. Avoid regenerating everything at once. |
| Medium | Card rails/corners | Source frames have their own heavier corner shapes and richer edging. R1 repeats a thin generic-for-this-page frame with prominent corner curls. | Refine the isolated frame against the original, keeping corner proportions through slicing. |
| Medium | Tabs | Source uses angular gold-edged plaques with textured blue fill across the selected face. R1 uses rounded corner curls and an inset solid blue highlight. | Match plaque silhouette and textured selection treatment. Current category remains Upcoming until there are actual active events. |
| Medium | Action buttons | Source buttons sit farther right and lower within its taller first rows, with different endcaps, gold trim and larger raised captions. R1 reuses wider/shallower Shop buttons and flat labels. | Align footprint/caption scale after card geometry is corrected; preserve disabled/preview behavior. |
| Medium | Player header | Shared header broadly aligns, but the portrait/pendant differ and the source's small progress bar beneath the player level is absent. | Audit whether an actual player-XP progress value exists before restoring a live bar. Preserve the accepted character and never display a fabricated progress fraction. |
| Medium | Background/color distribution | Source shows a dense violet dusk city behind the heading. R1 exposes starry sky and much more orange foliage/path below the cards. Merchant/knight painting palettes and lighting also differ. | Correct occupied layout area first, then compare page-local environment framing/tone. No blanket darkening is justified from these differing compositions alone. |
| Low | Footer | Source has compact centered wording enclosed in finer gold flourishes; R1 has a different full-width ornament and added circular Back button. | Integrate Back into a compact reference-style footer while keeping native navigation. |

### Approximate vertical geometry at941×1672

Reference extents below are manually read from the original image and rounded; current extents are the owning runtime's rect coordinates, consistent with the capture. They are not pixel-perfect segmentation measurements.

| Card | Reference y range (approx.) | R1 rect y range | Main effect |
|---|---:|---:|---|
| Firebird | 385–720 | 391–666 | About60px too short |
| Baba Yaga | 735–1055 | 677–952 | Starts about58px too high; about45px too short |
| Merchant | 1070–1336 | 963–1238 | Starts about107px too high |
| Ice Tournament | 1348–1614 | 1249–1524 | Starts about99px too high |

The title Image also preserves aspect inside a526×166 rect. With the987×339 sprite this yields a narrower visible face than the rect; use the visible artwork bounds when centering/enlarging it rather than assuming the rect equals the plaque outline.

## Intentional state differences

The reference's active-event timers, premium ribbon, participation captions and reward examples are illustrative. Current runtime has no authoritative event schedule/recipes. Upcoming, date-pending labels, disabled participation and empty Active/Completed are therefore appropriate for the current functionality. They are not evidence that the missing visual clock/reward structures must remain absent.

Player level, gold and energy differ because the capture uses real isolated-save values. Do not replace these with reference sample values. First card previews the existing local EventDetail; it is not a live event. EventDetail itself is outside this audit.

## Recommended R2 scope

**Card proportions, heading and status/reward hierarchy first.** Restore row sizes and image footprints, align title/plaque and live lettering, and rebuild the clock/status and reward-strip structure while retaining honest availability. Then review a new Unity capture before spending calls on another character/scenery pass. Gameplay and all other approved pages remain excluded.

## Checks performed / not performed

Performed: current instructions/docs and dirty-tree inspection; direct original vs saved Unity image review; runtime/prefab structure inspection; capture metadata and review-link verification; six asset hashes and original-reference hash comparison.

Not performed: production edits, Scenario calls, fresh Unity import/build/tests/captures, APK/AAB, physical-device testing, browser execution, calibrated color analysis or exact-font identification. Earlier17 tests remain historical verification of R1, not new results from this review. Only QA/documentation files, CurrentTask and SessionLog updated.
