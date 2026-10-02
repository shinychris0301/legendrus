# Team Hub R4 — Footer Crest and Motto

Implemented the user's approved next refinement in the native Team Hub page. The footer now uses the original blue/gold guild shield, red lacquer motto plaque, Russian lettering and information icon. English uses live localized text on a cleared source-material plaque. Back and Friends remain distinct working buttons; the information icon opens the existing modal with a general guild heading and the honest preview/unavailable explanation.

The plaque and crest use a uniform0.8 scale relative to their source bounds to reserve room for the existing navigation. This is an intentional layout adaptation, not a claim of pixel-identical footer placement. The shared carved backing is retained; crest, plaque, motto, information and navigation follow the safe-area bottom on tall screens.

## Implementation and scope

Four new PNGs plus metadata: `footer_crest_r4`, `motto_ru_r4`, `motto_blank_r4`, `guild_info_r4` under `ProductionUI/TeamHubAligned`. Original crest/info RGB and Russian motto pixels are verified unchanged. Hidden plaque end and removed English/info footprints use clean red lacquer from the same original, avoiding a new generation. First surface draft was replaced after visual review found repeating border lines in its donor patch. No Scenario requests/credits.

Updated native owners: `ProductionTeamHubReferenceLayout`, `ProductionTeamHubLayout`, `ProductionTeamHubView`, `ProductionTeamHubPrefabBuilder`; active TeamHub prefab and cutover now revision5. Updated both TeamHub test files for fourteen controls, source/blank localization, non-overlap and information-modal behaviour. Existing binding/route guards apply to the new information button. No backend, live membership, reward or currency behaviour was added.

Exact changed paths and preservation results are in `QA/ScopeAndVerification.json` and `QA/PreservationChecks.json`. Existing artwork/metas/fonts, other page prefabs and previous Event Detail/Ranking work remain unchanged relative to this task's snapshot.

## Verification

Unity6000.5.9f1 Verify02 is the final run; Verify01 passed before the red-surface refinement. Final narrow rebuild/readback and **7 Edit Mode + 10 Play Mode = 17 tests passed**, zero failures/skips, including Individual Ranking regression. Coverage includes localized text fit, safe-area/button bounds, footer non-overlap, repeated rebuild, source sprite assignments, actual button routes, modal input blocking and no save/reward mutation.

Eight TeamHub and ten Ranking native captures were produced with isolated saves. RU941×1672 and narrow EN576×1280 safe-area views were visually inspected. Four new sprites were inspected over black, white and magenta; source RGB/lettering and hashes checked. Standalone review presents one portrait at a time with ten views and original/before toggle.

No Android build, physical-device, performance or full-project suite ran. Guild functionality remains a labelled preview. Remaining fidelity differences are documented separately.
