# Independent R5 Unity visual review

Reviewed 2026-09-13 by the navigation-asset subagent, independently of the runtime integration. This is a direct visual review of actual Unity PNG captures supplied by root. No source, prefab or image pixels were edited during this review.

The R5 page visibly implements all four proposed revisions and improves the rejected R4 result. No blocking overlap, clipped text, disconnected lower background seam or unreadable ordinary balance was found in the five inspected fixtures. This is not a claim of exact reference fidelity or user visual acceptance. Root reported 32/32 Edit Mode and 21/21 Play Mode passes; this reviewer did not independently rerun those tests and does not use them as evidence of visual similarity.

## Images inspected

Directly viewed the original whole `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png`, R4 actual Unity `Home_941x1672_RU.png` and `Home_1080x2400_safearea_RU.png`, and these five actual R5 images under `/tmp/lor-home-r5/playmode/Home/`:

| Capture | Dimensions | SHA-256 |
| --- | --- | --- |
| Home_941x1672_RU.png | 941 × 1672 | 96f4233a3c53ace7adad9086ce56f9ed04e88afd25d049b9b521ac4359add731 |
| Home_576x1280_safearea_RU.png | 576 × 1280 | 8f8dc9b9074258165dee792b80a1d73df81f890b29a2f86f6e11d9ec82a61683 |
| Home_1080x2400_safearea_RU.png | 1080 × 2400 | 291c4aa33e3aa0432f2c2cdf3f09290d47a6b883058d537c9ec690990798ca5b |
| Home_941x1672_EN.png | 941 × 1672 | 367347bc65c9460c16dd1e5a5e2adf2af89d971d54ed7802a63a1dac0197d237 |
| Home_576x1280_maxbalances.png | 576 × 1280 | a45eec9575acd9237d23646439f008949e2eee384a8095734749fef7e2f811cb |

The 1080 × 2400 image was displayed by the image-view tool at 922 × 2048; the original dimensions and hashes above were read directly from the PNG bytes. Smaller fixtures were inspected at their native supplied dimensions.

## Assessment of authorized revisions

| Revision | Observed result | Remaining difference |
| --- | --- | --- |
| Broader Firebird and right-side tail | Strong improvement. The new bird has a broad luminous orange-gold wing, pronounced layered feathers and a long outward/downward tail along the right side. Its claws visibly contact Ivan's shoulder fur. It no longer forms R4's narrow inward curl across his torso. | The wing is more widely spread than the reference's folded-wing pose. Some peripheral feathers continue beyond the right display boundary in the reference-sized portrait; this reads as foreground bleed, not an isolated accidental clipping defect. The reference has finer, denser filament detail and a different orange balance. |
| Aged HUD and navigation | Heavier carved/engraved corner pieces, deep bronze recesses and near-black navy surfaces are visible. The shop has a three-quarter view; the event scroll angles in space; the helmet has dark steel and engraved gold. The HUD is less flat at its corners and capsule ends. | The coin, plus controls, avatar and settings still have brighter, cleaner metal than the reference. The new navigation lower plaques project farther outward, and their baselines stagger slightly because each generated card's painted blank plate sits at a different height. Captions remain legible and inside those plates. |
| Connected footer | A continuous dark wood base and carved bronze-gold ornament now span behind the mailbox, quest section and reward chest to the bottom edge. This removes R4's floating isolated medallions and large plain lower gap. | The reference uses a denser interlocked foliage treatment and a more compact quest frame. The retained mailbox/chest medallions remain rounder and brighter. Quest/XP unavailable markers still differ from the reference's example values; fabricated values were not expected. |
| Reduced empty sky on tall screens | Ivan and the Firebird sit materially closer beneath the crest in both tall safe-area fixtures. The crest-to-head gap is much smaller than the equivalent R4 tall capture; central composition is more continuous. | The preserved winter plate still produces bright sky and a daylight/sunset atmosphere. Safe-top background remains visible above the HUD where the simulated insets require it. It is not evidence of the original reference's dusk environment being reproduced. |

## Layout and readability

- Russian and English Start/navigation/quest captions remain visible, clear of artwork silhouettes and inside their intended surfaces in the inspected reference-size captures. Ivan's face and the logo are unobscured. The Firebird overlaps decorative art and passes behind foreground controls without covering their live text.
- Both tall safe-area captures show complete native controls and footer text. The full-width lower panel has an intentional upper gold border; no accidental gap to the environment or truncated bottom background was visible.
- Ordinary gold, gems and energy values are clear of their plus buttons. In the 576-pixel maximum-balance fixture, all digits of `2 147 483 647` are present on two lines in each relevant capsule, with clearance from the plus controls. They are very small at this extreme; completeness in a desktop PNG does not establish comfortable phone readability.
- The exact shared Splash character, crest and Start appear consistently with R4, without an observed redesign. This review did not independently prove byte identity or GUID preservation; root's source audit owns those checks.

## Limits

The mandatory approved Splash Ivan/crest/Start and preserved winter background differ from the original Home reference by design constraints. The new isolated art is independently painted and therefore differs in exact geometry, ornamentation and colors. The reference's clean-shaven alternate Ivan, uppercase logo and dusk background must not be silently reinstated to claim a match.

This review performed direct image comparison plus PNG dimension/hash reads only. It did not perform fresh Unity execution, interact with controls, inspect on a physical Android phone, rebuild an APK/AAB, test performance, identify the exact font or compute pixel/color identity against the reference. Final user visual approval remains outstanding.
