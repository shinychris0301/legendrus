# Visual Page22 — Team Ranking R1

Implemented a native Team Ranking page reached from Team Hub, based on the discovered original Guild Ranking reference. Three podiums, ranks4–10, guild emblems, member/contribution/reward columns, a highlighted sample guild, filter tabs, Back and Guild Details are separate Unity controls/sprites. The account header displays live balances and level. Guild standings, membership and rewards remain clearly labelled examples.

## Artwork and workflow

17 successful Scenario `model_pixa-background-removal` jobs cost **51CU**. No RGB generation jobs. Original source RGB is retained outside recorded text-removal masks; Scenario provides alpha only. Local cleanup removes neighboring fragments and restores opaque panel interiors. Original Russian title and fixed podium numerals1–3 remain baked source art; other text is live RU/EN. The English title uses a blank source plaque. Details caption cleanup uses bounded local inpaint; an earlier visibly tiled donor patch was rejected.18 separate runtime PNGs, enlarged3× with interpolation (no new source detail).

Sources, requests, status/billing/output records and repair masks are under `Sources/`. [Provenance](AssetProvenance.json), [Scenario audit](QA/ScenarioAudit.json), [reference comparison](QA/ReferenceFidelityReview.md). Clean background, account header, tabs, table/row/reward frames and footer ornament are reused from existing page resources, with existing GUIDs preserved. Body/names use the embedded user-selected Roboto Serif600; English title FreeSerif Bold; ranks Bitter600. No OS font files copied.

## Active implementation

- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamRankingView.cs`: live header, localization, explicit preview state, filters, guarded navigation, details overlay.
- `ProductionTeamRankingRuntime.cs` and `ProductionTeamRankingLayout.cs` in the same directory: native hierarchy and safe-area composition.
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTeamRankingPrefabBuilder.cs`: import/rebuild/readback owner.
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamRanking.prefab`: active native prefab, revision1.
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TeamRankingCutoverState.txt`: enabled revision1 route.
- `Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned/`:18 sprites and their metas.
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`: native binding, page-owned header exclusion and Back dismisses details before returning to Team Hub.
- New TeamRanking Edit/Play Mode tests under the existing production UI test assemblies.

No screenshot page, invisible-hotspot interface, duplicate Canvas or EventSystem. No guild data/reward writes. Filters change selection only; the details overlay explains the sample state. Profile/Settings/Shop/Energy routes and Back use existing navigation services. Multiple binding/click attempts do not duplicate route callbacks.

## Checks actually performed

Unity **6000.5.9f1** fresh batch import, compilation, prefab rebuild and serialized readback passed. Final **6/6 Edit Mode +15/15 Play Mode** tests passed, zero failures/skips: Team Ranking2+5, Team Hub2+4, Individual Ranking2+6. XML, commands and logs are in `UnityVerification/`. Tests use isolated saves.

Team Ranking checks include complete bindings, screenshot-free dependencies, idempotent construction, live balance refresh, correct font roles, RU/EN fitting, row bounds, safe-area background coverage, reachable button raycasts, single-commit routes, filter state, details input shielding, Back dismissal order, Team Hub entry, maximum balances and no unintended save changes. Nine final Team Ranking captures cover941×1672,576×1280 and1080×2400, RU/EN, safe areas, details, Local/Week and maximum balances. Eight Team Hub and ten Individual Ranking regression captures are retained.

Original/native RGB equality outside declared repair masks and18 runtime/source-final hashes passed. Alpha inspected on black/white/magenta. Final RU main, safe EN main and safe EN details inspected manually; other final captures checked automatically. Early missing body-font binding and English contribution-column overflow were corrected; final source/visual checks include the cleaned details button. Scoped whitespace and source syntax checks performed. Standalone review browser checks are recorded in `QA/BrowserReview.json`.

[Scope audit](QA/ScopeAudit.json):53 new Assets files including metas, one existing production file changed (`ScreenRouter.cs`), no deletions. All other pre-existing Assets, packages and ProjectSettings match task-start hashes, including Gameplay, Splash, other page prefabs, font assets and existing metas/GUIDs. Existing unrelated local changes preserved.

## Not performed / limitations

No APK build, physical-phone test, all-project or all-pages test run, device performance profiling, calibrated color verification or live leaderboard/backend testing. Scope is the UI implementation. Online rankings, actual guild membership, season scheduling, period-specific data, contributions and reward grants remain later feature work. The original font identity is unverified; R1 retains documented visual differences and awaits user review.
