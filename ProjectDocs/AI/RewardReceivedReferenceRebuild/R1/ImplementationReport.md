# Reward Received reference rebuild R1

Implemented Visual Page 15 after the user approved continuing from Daily Reward R4. The active RewardClaim prefab and its owning builder/runtime now use revision 2.

## Design and behavior

- Restored the original carved gold/navy frame, Russian heading and complete ЗАБРАТЬ button, including its lower sapphire ornament.
- Fresh Scenario isolation retains the original chest, coins and gems. A separate source glow layer restores rays and bright sparkles behind the chest.
- Fine gold cards display actual receipt quantities. Coin, gem and energy icons are extracted from the original; existing booster artwork/functions are retained.
- Russian static lettering uses source pixels as previously approved. English and dynamic receipt text remain live/localizable. An empty receipt uses a live Close control and hides the chest, glow and source heading.
- Confirmation closes the existing receipt without awarding twice. Seven-day grants, capped energy, persistence and navigation remain intact.
- The modal dims the actual underlying Daily Reward page, which supplies the shared player header. No duplicate header or screenshot backdrop was introduced.

## Asset workflow

Original: `Sources/OriginalReference.png`, copied from the existing approved `Resources/DesignScreens/RewardClaim.png`. Three accepted Scenario jobs: chest alpha 3 CU, panel alpha 3 CU, bounded hidden-material repair 13 CU; **19 CU total, no paid retries**. A local output-directory collision was corrected before repair submission; it did not create another generation job. Output atlas regions were inspected and remapped; generated content is confined to cleared navy/green interiors. Intentionally opaque panel faces remain opaque.

Black, white and magenta alpha previews were inspected. Original RGB validation found **0 mismatches across 157,970 visible pixels** in the chest, Russian title/button and three currency icons. See `Sources/AssetProvenance.json` and `QA/OriginalRGBCheck.json`.

## Verification

Unity 6000.5.9f1 rebuilt and read back the real prefab. Final Verify02 passed **7 EditMode + 12 PlayMode = 19 targeted tests**, covering Daily Reward and Reward Received. Captures include RU/EN, tall safe areas, Day 1, Day 7, capped energy, no receipt and a clearly labelled seven-type diagnostic fixture. Tests cover grant-once behavior, failed persistence, modal input and repeated-build layering.

The first visual pass caught missing white sparkle cores and card labels too close to the gold rim. Both were corrected before Verify02, with icon-centering and label-clearance assertions added. The final Russian standard and English tall captures were visually inspected. The HTML review decoded all **11 views**, with no browser errors or horizontal overflow.

Task-start hash comparison: **8 existing Assets files modified, 20 added, none deleted**. All 150 prior prefab records retained (158 total). Existing GUID/metas and other page prefabs were unchanged by this task. Full changed-file list: `QA/ScopeAndVerification.json`. No Android build, physical-device test or full project suite was run.

## Remaining reference differences

The original contains nine illustrative rewards; the game displays only rewards actually granted, using up to seven supported kinds. Live counts and card arrangement therefore vary. The underlying page is the actual Daily Reward screen, rather than the reference Home scene. Repaired hidden textures, fine alpha edges, extracted glow and live English typography remain approximations; full pixel-perfect fidelity is not claimed. Tall layouts preserve aspect ratio with additional scenery margins.

Next: user visual review; then Visual Page 16 — Out of Energy if continuing the screen sequence. No further generation is scheduled.
