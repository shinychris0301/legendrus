# Shared Booster Icons R1

Delivered 2026-09-23. The user requested freshly extracted active booster icons, consistent use across game pages, and a fidelity check of the four unavailable previews.

## Implemented

The original `booster select.png` supplies one shared art family: hammer, Firebird, shield and potion for the four existing inventory tools; scroll, lightning orb, bomb and swap for the four previews. Each has a framed medallion and a tool-only sprite. Framed variants are used in Booster Selection, Pre-Level, Pause and Failure; isolated subjects are used in Gameplay cards and rewards/details. The mapping retains the established tool functions and quantities. Preview tools remain unavailable and noninteractive.

`ProductionBoosterArt` is the shared source. Existing page loaders/builders now resolve the same artwork, and the active prefabs have been migrated without replacing their serialized controls. Dynamic consumers of `ProductionGameplayArt.Booster` also receive the new subjects. Failure and Pause use separate native stock badges because the illustrative badges have been removed from the artwork.

| Page | Integration |
|---|---|
| Booster Selection | Four shared medallions plus four shared preview medallions |
| Pre-Level | Booster row, inspection and bonus icons |
| Gameplay | Four isolated inventory tools in existing cards |
| Pause / Failure | Shared medallions with live native stock badges |
| Daily Reward | Actual booster reward icons |
| Victory / Reward Received / Chapter Complete | Awarded booster icons |
| Inventory / Profile | Owned booster list and details |
| Tutorial / Help | Inventory booster explanations |
| Mail | Hammer and potion attachment examples; still labelled examples |

This changes inventory-booster artwork. Match-created board specials, character portraits, collectible artifacts and category-navigation compositions have separate meanings and retain their artwork.

## Extraction and fidelity

Two new Scenario Pixa alpha jobs were accepted: complete medallion atlas and subject-crop atlas, **3 CU each / 6 CU total**, with no paid retries or RGB regeneration. Pixa retained ring/backdrop fragments in the subject-crop output; it was not accepted as a finished cutout. Tool-only contours were subsequently traced locally against the source. Framed variants retain original source RGB and use Scenario alpha with bounded cleanup. The opaque navy medallion faces are intentional.

The baked count and plus circles obscure portions of each lower rim. These small areas use unobstructed upper-rim donor pixels from the same medallion. Fine outer specks and leftover circle fragments were cleared. A false transparency hole in the bomb preview was restored from the original opaque artwork. Central tool subjects, source colors and bottom sapphire positions were checked against the original.

All eight tool-only assets retain **57,635 visible source pixels with zero RGB mismatches**; see [OriginalSubjectRGB.json](QA/OriginalSubjectRGB.json). This measures retained colors, not proof that the alpha contours or whole page match perfectly. Hidden rim repairs, fine silhouettes and source resolution remain limitations. Enlarging the small original subjects cannot reveal detail absent from the source. No exact source-font claim is made.

Transparent art was inspected over black, white and magenta. [Asset comparison](AssetReview.html) allows inspection of the original crop, framed result and isolated subject for each tool. This diagnostic page is distinct from the actual Unity page captures in the [main review](SharedBoosterIcons_R1_Review.html).

## Verification

Unity **6000.5.9f1**:

- Verify02: **39 EditMode + 70 PlayMode tests passed** across the affected core UI, reward and collection flows.
- Mail01 follow-up: **4 EditMode + 6 PlayMode tests passed**, including the shared-art reference audit again.
- **117 distinct targeted tests passed**, 119 successful executions including two repeated shared-art tests.
- Canonical prefab audit finds no superseded booster sprite references covered by the shared mapping. Importers use transparent, uncompressed sprites without mipmaps. Inventory, Gameplay, Pre-Level and Pause mappings agree.
- Existing stock, selection/inspection, consumption, selected-round energy gate, rewards, navigation and localization checks pass. No new booster functions, purchases or grants were added.
- Native Booster Selection, Gameplay, Pre-Level, Pause, Failure, Daily Reward, Inventory, Profile, Tutorial and Mail captures were visually inspected. Further reward and safe-area captures are included for review.
- Browser checks passed for **17 page/reference views and 8 asset comparisons**, with no JavaScript errors or horizontal overflow in the main portrait review.
- Scoped source whitespace checks passed.

Verify01 initially had 38/39 EditMode passes: the Failure test expected the superseded sprite and baked badge. Its assertions now verify the shared sprite and native badge. This failure is retained in the QA evidence. Subsequent runs passed. No Android build, physical-device run or full project test-suite run was performed.

## Scope

Compared with the start-of-task snapshot: **18 existing Assets files modified, 39 files added, no files deleted**. Added resources comprise 16 PNGs with metas, their folder meta, and the shared runtime/editor/test scripts with metas. Eight canonical prefabs changed: BoosterSelect, PreLevel, Gameplay, Pause, Fail, DailyReward, ChapterComplete and Mail. Other consumers receive shared sprites through their existing dynamic loaders. Old artwork and existing metas/GUIDs remain intact. No gameplay/economy data, packages, rendering backend or startup architecture changed.

Exact paths, hashes and outcomes: [ScopeAndVerification.json](QA/ScopeAndVerification.json), [Provenance.json](Sources/Provenance.json). Reproduction scripts and Scenario request/job/billing records are stored under Sources. Full logs for the two verification stages and the initial failed assertion are under QA.

Next: user visual review of the shared icon family. The next screen in the earlier sequence remains Visual Page 18 — Events.
