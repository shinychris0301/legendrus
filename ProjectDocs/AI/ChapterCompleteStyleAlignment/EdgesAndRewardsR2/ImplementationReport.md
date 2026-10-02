# Chapter Complete R2 — edges and reward hierarchy

Implemented the approved refinement in the active Unity prefab, preserving existing serialized objects and GUIDs. [Open actual Unity review](ChapterComplete_EdgesAndRewards_R2_Review.html).

## Changed

- Header source extraction expanded to retain curved corner scrollwork, removing the y193 straight cutoff. Firebird extraction expanded rightward, restoring visible feather/tail tips. Original source RGB retained; only alpha/sprite bounds repaired. English blank faces reuse R1's localized surface repairs.
- Original “НАГРАДЫ” lettering and gold ornaments isolated with bounded local alpha extraction. Added three sprites: rewards_heading_ru, rewards_heading_blank, reward_divider. Replaced four header/foreground sprite files in place, preserving metas/GUIDs.
- Six reward cards now134×139 at native aspect ratio,80px icons,30px counts. Short reward heading and one live received-reward/completion line restore a clearer hierarchy. No categories, amounts, grants, acknowledgement or routes changed.
- Runtime/view revision2. The owning serialized prefab builder applies an editor-only in-place upgrade, then validates/saves/readbacks; repeated runtime builds do not duplicate content. New heading/divider bindings participate in revision validation.
- ChapterCompleteAlignmentPlayModeTests now additionally check card/button, icon/quantity and summary/button overlap.

## Scenario and provenance

Two successful `model_pixa-background-removal` API jobs, header and foreground, **3 CU each / 6 CU total**. No scene regeneration, generative redraw or resubmission. Source regions and hash recorded in [Provenance.json](Sources/Provenance.json); API request/job/status/billing/output records in `Sources/Scenario/`. These background-removal jobs use an image and `outputFormat: rgba`, not a textual generation prompt. Original RGB is combined with Scenario alpha; duplicated chapter-ribbon/footer-rail pixels are masked locally under the approved extraction workflow. Local color/luminance alpha isolates the gold heading and divider.

## Verification actually performed

Unity6000.5.9f1 final **run2** used isolated config/save storage.

| Check | Result |
|---|---|
| Compilation, Chapter Complete rebuild, revision2 save/readback | Passed |
| Chapter Complete + Pause Edit Mode | 4/4 passed; zero skipped/failed |
| Chapter Complete + Pause Play Mode | 6/6 passed; zero skipped/failed |
| Native controls/raycasts, repeated display/localization, real totals, acknowledgement without duplicate rewards | Passed |
| Safe areas, text containment, control aspect ratios and new non-overlap assertions | Passed across six captures |
| Alpha inspection over black/white/magenta | Completed for main and fallback assets |
| Seven installed sprite hashes; original Russian header/foreground RGB | Passed |
| Header final bottom alpha boundary / foreground final right alpha boundary | Zero pixels above128; cutoff checks passed |
| Prior metas, prefab IDs, R1 evidence and unaffected production hashes | Preserved; see FinalAudit.json |

Saved six Chapter Complete and six Pause PNG/JSON capture pairs, command records, logs and test XML under `UnityVerification/`. Visually examined final referenceRU/tallEN/referenceEN/tallRU/in-progress captures. The1080×2400 capture was generated and layout-tested, not separately visually examined. Portrait review local paths and Python syntax checked; no browser interaction.

Run1 compiled/rebuilt and passed4Edit+4/6Play tests. New overlap assertion caught the summary rectangle starting3px before the button's rectangle ended. Summary moved below the buttons and its font/height adjusted; final run2 passed4Edit+6Play. Earlier failed evidence retained in `QA/EarlierRuns/run1/`. The initial preparation attempt used a Python executable without Pillow, exited before changes/API calls, and was rerun with the installed imaging environment.

## Scope audit

Against4557task-start production files:4548unchanged,9existing changed,6new,none deleted. All2390prior meta files and all160original prefab objects preserved; final prefab168objects;3unique new GUIDs. All39R1 evidence hashes remain unchanged. Final4563production hashes and35Unity evidence hashes recorded.

Existing files changed: four ChapterCompleteAligned header/foreground PNGs; Pages/ChapterComplete.prefab; ProductionChapterCompleteRuntime.cs; ProductionChapterCompleteView.cs; Step20ProductionUiFix01SerializedPrefabBuilder.cs; ChapterCompleteAlignmentPlayModeTests.cs. New files: three reward-heading/divider PNGs and their metas. Exact paths in [FinalAudit.json](QA/FinalAudit.json). Other page prefabs/art, R1 environment/buttons/teaser, routing/services, Android guards, ProjectSettings and Packages preserved.

**Not performed:** APK/AAB build, Android-device test, full project suite, repeat of the12chapter-finale tests from R1, extended manual play, browser interaction, performance profiling, live monetization, exact font identification or calibrated color measurement. Remaining reference differences documented separately. Next step: visual review of R2, followed by physical-device readability/touch verification. Failure continuation stays scheduled after UI completion.
