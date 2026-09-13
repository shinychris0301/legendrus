# Independent final Home R3 visual review

Reviewed five actual Unity captures from `ProjectDocs/AI/HomeStyleAlignment/ReferenceRebuildR3/UnityVerification/Home/`, their accompanying JSON metadata, and the original whole `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png`. Capture metadata identifies Unity 6000.5.9f1, OpenGLCore, the active Home prefab and simulated display/safe-area fixtures. This review examined rendered images; it did not rerun Unity tests or perform physical-phone testing.

The final page is materially closer to the original reference than R2 and the first R3 candidate. No blocking clipping, backing protrusion, visible screenshot contamination or lower-background seam was observed in these five captures. This does not establish an exact image or color match, or user visual approval.

| Capture | Direct visual result |
| --- | --- |
| `Home_941x1672_RU.png` | Entire logo crown and lower sapphire are visible. HUD, hero, Start, four navigation cards and footer have clear separation. Russian navigation labels fit their blank plaques without touching metal borders or ornaments. |
| `Home_576x1280_safearea_RU.png` | Header backing stays with the header; the earlier separate large dark rectangle is absent. The intentional dark HUD panel remains. Crown is complete, four cards remain readable, footer medallions and reward badge remain within the displayed layout. Background extends to the physical bottom without a seam. |
| `Home_1080x2400_safearea_RU.png` | Simulated side/top/bottom safe insets produce surrounding scenery. Header remains a bounded panel without an additional detached rectangle. Crown, Start corners, navigation cards and footer ornaments are intact. No exposed backing outside the intended panels was visible. |
| `Home_941x1672_EN.png` | `WORLD MAP`, `SHOP`, `EVENTS`, `CHARACTERS`, `PLAY`, profile and quest copy are complete and legible. `CHARACTERS` clears the plaque edges. No overlapping label/ornament or overflowing English copy was observed. |
| `Home_576x1280_maxbalances.png` | Full `2 147 483 647` gold and gem values are visible over two lines, alongside plus buttons. The extreme-value text is very small at this narrow viewport, but no digits disappear or overlap the adjacent controls. Other labels and frame clearances remain intact. |

## Material and subject comparison

- The logo now uses the reference's uppercase Cyrillic hierarchy, black engraved body, gold filigree and sapphire crown instead of the previously reused Splash castle crest. Its complete top ornament is no longer covered by the HUD.
- Ivan is clean-shaven, dressed in dark blue with gold embroidery and a red cloak. The Firebird perches on his shoulder and now has a fiery orange curved feather sweep, replacing the first R3 candidate's comparatively static vertical tail. The face, pose and feather drawing remain newly generated interpretations.
- Start has a deep emerald surface with muted carved gold and sapphire ornaments. Its wording remains live text. The reference's exact embedded lettering bevel and source font are not reproduced by the current live fallback font.
- Navigation now presents the parchment map and lower-left compass wheel, sapphire-awning shop, red parchment sun-wheel scroll and closed bronze/sapphire noseguard helmet. The refined blank plaques restore larger live labels and consistent visible card widths. Narrower corner ornaments avoid the former caption collision.
- The footer uses dark wood, carved scrolling medallions, red-wax envelope, blue-banded wooden chest and a dedicated bulky hexagonal blue reward gem. The gem no longer reuses the long-pointed top currency diamond. Gold appears aged bronze with controlled highlights; navy panel interiors remain dark and visually solid.

## Remaining differences and limits

The preserved R1 winter background is much brighter and more daytime-oriented than the original reference's dusk village. It remains outside this foreground replacement scope, so the whole page's lighting cannot be called identical to the original. New crest curves, engraved letter shapes, metal bevels, corner filigree, Ivan's facial proportions/pose, Firebird anatomy and individual item details also differ. Their overall family and major silhouettes are closer; exact pixel identity and exact numerical color equivalence have not been established.

Live balances, level, badges and quest availability follow project state rather than the reference's illustrative numbers. The unknown quest measurements/reward display dashes; copying `15/30` or `250` would not prove real progress. The exact reference font remains unverified; the existing shared fallback remains in use. Narrow-device extreme balance text remains small and warrants real-phone readability review.

No source PNG pixels were edited for this review. No new image generation, production edits, APK/AAB build, physical-device capture/input testing, performance testing, or additional Unity test execution was performed by this reviewer. Source-level provenance and black/white/magenta alpha reviews are recorded separately beside this report.

A final test-fixture adjustment refreshed the live reward-badge text geometry synchronously before capture. The finalized 941px Russian and 576px safe-area portraits were re-inspected: no new clipping, label collision, backing protrusion or seam was observed. The artwork and layout coordinates remain the same; the final captures and hashes below supersede the earlier temporary capture set. This reviewer did not rerun Unity tests.

## Reviewed capture hashes

| Capture | SHA-256 |
| --- | --- |
| `Home_941x1672_RU.png` | `1bc38e4dac208dcb7bce6bb2031a4be52332d01f08c53c58eaa0f411a5fc78cc` |
| `Home_576x1280_safearea_RU.png` | `9cf87feb1dfb064f432513505308eab76123ec2056011ee7b2df43d1b82bddb2` |
| `Home_1080x2400_safearea_RU.png` | `570f482ff7b06ddb964c229e5bfb1ed25e119c512006a74ffdf012ff172fe8d6` |
| `Home_941x1672_EN.png` | `55ccc59268735870663c277b1307faa95b62680a58f22c0bdb9e3126698b5bd7` |
| `Home_576x1280_maxbalances.png` | `bbaa3888c33d9a56c129a790af0cc0ee25e86db54e430d4ce89c62680751688b` |
