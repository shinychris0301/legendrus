# Visual Page 20 — Individual Ranking R1

Implemented native «Личный рейтинг»: sample standings, live shared header, Overall/Friends tabs, circular portraits, raised gold-rim rank badges and a fixed player row. Existing Profile entry and Back-to-Home behavior are preserved. Gameplay and other accepted pages are unchanged.

## Design and behavior

The available legacy ranking preview supplies the page hierarchy; no dedicated original reference was found. Current accepted source-derived Events title/card/tab/status artwork, existing portraits and shared navigation replace the old generic preview. Background is a separate clean environment. Text, ranks, points, tabs and buttons are native Unity controls; no full-screen screenshot or reference region with hotspots is used.

Seven Overall rows are labelled sample standings. They use deterministic example positions/points, independent of wallet gold. The player's real rank and ranking score remain “—”, with an unavailable label. Friends shows an unavailable state. There is no backend, real season schedule, friend roster, claimable reward or fake countdown. No progress/economy/claim state is written by tab navigation.

All captions support RU/EN. Current header balances and profile level reflect the save. Two existing character portraits repeat in sample rows; they are illustrations, not real player records. Native circular masks preserve source aspect ratios. Gold-rim badges reuse the accepted native raised-badge renderer.

[Reference assessment](ReferenceComparison.md) · [Asset provenance](AssetProvenance.json).

## Scenario workflow

This pass uses the workflow's reuse step: **17 direct serialized sprite references reuse existing PNGs unchanged**. Source-derived Events artwork retains its upstream Scenario records. No new source extraction or generation was needed; **zero new API requests, zero credits spent and zero image edits**. Existing extraction QA is historical upstream evidence, not a new API or alpha-processing run. Actual Unity portrait masks were inspected.

Exact serif identity is unknown. There is no claim of matching a missing original ranking reference exactly. Source-surface repairs and native badge geometry remain approximations within the accepted family.

## Changed files

Added these production/test files and their eight `.meta` files:

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionIndividualRankingRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionIndividualRankingView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionIndividualRankingLayout.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionIndividualRankingPrefabBuilder.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/IndividualRanking.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/IndividualRankingCutoverState.txt`
- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/IndividualRankingAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/IndividualRankingAlignmentPlayModeTests.cs`

Modified existing production file: `ScreenRouter.cs`, limited to native Individual Ranking binding and suppressing the duplicate legacy resource header on this page.

Added this review/report/provenance/QA/verification folder. Updated CurrentTask, SessionLog and the workflow reuse note. Baseline4921 Assets files:4920 unchanged,one existing router file changed,16 additions,zero deletions. All earlier PNGs, metadata and page prefabs remain byte-identical to the turn snapshot. New GUIDs are unique. [Scope audit](QA/ScopeAudit.json).

## Verification performed

Unity **6000.5.9f1**, isolated test storage:

- Native prefab rebuild, script compilation and serialized readback passed in runs1 and3.
- Final run3: **2 Individual Ranking EditMode +5 PlayMode tests passed**, zero failed/skipped.
- Run1: **2 Event Detail EditMode +5 PlayMode regression tests passed**. That page was unchanged afterward.
- Across14 distinct targeted tests, every latest result passed: [test-by-test evidence](QA/LatestTestResults.json).
- Initial run1 had one new Profile-entry test failure. Its synthetic input omitted pointer-down/up, so the existing AnimatedHotspot correctly rejected it. The test was corrected to use the real press/release/click sequence; it passed in the focused run2 and final run3. Production Profile input code was not changed. Run1 logs/XML retain the failure.
- Final production text change made Friends footer category-specific and removed service-implementation wording. The final ranking suite and captures include that change.

Tests cover native bindings, screenshot-free dependencies, builder idempotence, Profile→Ranking entry and Back→Home, header destinations, duplicate-click/rebinding guards, tab/empty-state behavior, unchanged save/claim state, real resource balances, maximum balances, Russian/English text, rank-score separation, safe-area containment, row separation, text fit, button-centre raycasts and full environment coverage.

Saved **8 final Ranking PNG/JSON pairs**: standardRU/EN,576×1280 safe-areaRU/EN,1080×2400 safe-areaRU, maximum balances, FriendsRU and Friends576safeEN. Saved6 Event Detail regression pairs. Final standardRU and Friends576safeEN were manually inspected; initial standardRU/tallEN/FriendsRU also inspected. Other final captures received automated layout checks.

Assets hash/scope checks,17 reused sprite hashes, eight new GUID uniqueness, source whitespace and review-link checks passed. ProjectSettings/Packages git status is unchanged from the initial snapshot; no Android settings or guards were edited.

[Final rebuild command](UnityVerification/run3/Import/Command.json) · [Final EditMode XML](UnityVerification/run3/EditMode/results.xml) · [Final PlayMode XML](UnityVerification/run3/PlayMode/results.xml).

## Not performed

No APK build, physical-phone verification, full test suite, review-page browser execution, performance/memory profiling, calibrated color comparison, exact-font identification, fresh Scenario access check, real leaderboard submission/fetch or online ranking integration. No new transparent asset was created; upstream alpha-background tests were not rerun.

## Next action

Review the native portrait and tab states. Live leaderboard work remains for the feature phase: define season scoring, authoritative result submission, ranking snapshots, tie rules, privacy/friends scope and actual player placement before enabling real standings.
