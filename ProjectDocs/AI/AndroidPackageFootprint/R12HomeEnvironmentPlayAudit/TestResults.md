# Home legacy environment and Play dependency audit R12 — 2026-10-01

## Findings

Both resources still block fresh Home construction and legacy migration if missing. The environment has no serialized consumer. The Play sprite is additionally retained on exactly three disabled, non-intercepting Images in Home, ChapterMap and PreLevel. These require coordinated owner/prefab cleanup before archival.

Sprite-only clearing on disposable instances preserves the current bindings, component identities and live button targets on all three pages. Installed builder calls keep the cleared sprites empty. Fresh map construction instead clones the historical Image from the unchanged source prefab, so clearing a single installed instance is insufficient.

This completes the scheduled read-only audit. No production file or asset changed, and neither resource was archived.

## Assets examined

Both actual PNGs were viewed and their headers/imports loaded. No byte-identical other PNG exists among task-start project inputs. Full GUID/hash/import and R11 packed rows: [inventory](QA/Assets.json).

| Resource in HomeAligned | Source dimensions | Source bytes | Unity import | Reported packed object bytes |
| --- | --- | ---: | --- | ---: |
| home_environment_generated.png | 941×1672 | 3,017,340 | RGB24, 1 mip, Android2048 override | 4,720,680 |
| home_play_button.png | 1959×803 | 1,642,086 | RGBA32, 11 mips, Android4096 override | 8,384,424 |

Each contributes three rows in R11. Combined13,105,104reported object bytes are **not** measured compressed APK or memory savings. The environment's no-mipmap/RGB contract differs from the button's RGBA/mipmap contract; any future archive must preserve both through exact importer rules.

## Consumers and construction

Scanned **5,622 text files**, recording [13 name/GUID reference lines](QA/References.json) and [dynamic loader sites](QA/LoaderSites.json). Unity recursively checked **99 serialized roots per candidate**. Environment consumers:0. Play consumers:the active Home, ChapterMap and PreLevel prefabs only.

Home's23-entry legacy `TextureNames` list still contains both. `LoadSprites` throws for a null result. Fresh construction assigns the old environment and initially the old button; `AlignReferenceComposition` reassigns the environment and creates/sets `PlayButton/ReferenceArtwork`. Current `ProductionHomeSourceComposition.Apply` installs `HomeReferenceAligned/clean_environment`, uses `OriginalSourceArtwork` for the live Play control and disables the old graphics. The historical ReferenceArtwork keeps its sprite dependency despite being disabled.

Fresh and forced legacy Home each request23legacy resources; current migration requests0through the legacy delegate. Completed Home has no old environment Image, uses current live Play art, and retains one disabled old Play Image. Denying either resource produces the expected `Home requires a standalone imported Sprite` exception in both fresh and legacy paths.

ChapterMap fresh construction clones Home's PlayButton hierarchy. PreLevel fresh construction clones ChapterMap's Footer/PlayButton. Their source-control owners disable the inherited graphics and install current controls but retain the old ReferenceArtwork sprite. Installed and fresh probes each find exactly one such Image. The active targetGraphic uses HomeReferenceAligned/source_play_ru on Home and ChapterMapReferenceAligned/play on ChapterMap/PreLevel.

| Page | Retained Image path beneath page content | Enabled | Button target | Raycast |
| --- | --- | --- | --- | --- |
| Home | PlayButton/ReferenceArtwork | false | false | false |
| ChapterMap | Navigation/PlayButton/ReferenceArtwork | false | false | false |
| PreLevel | Navigation/PlayButton/ReferenceArtwork | false | false | false |

The retained GameObjects are active; their Image components are disabled. Live current source artwork remains enabled, active and raycast-enabled with the Button's targetGraphic pointing to it. All three views validate.

The current loading contract follows HomeLoadingPreparation → ProductionUiFix01Runtime.GetHomeTextureResourceKeys → ProductionHomeRuntime.GetTextureResourceKeys. Both legacy keys are absent from that current list. However, the Home prefab dependency graph still includes the old button. Exclusion from the explicit preparation list alone does not make the asset absent from the prefab or package.

Reviewed shared loaders in Gameplay, Victory, Shop, DailyReward, SpecialOffer, NavigationStyle and Login; no additional direct request for either candidate was identified. Gameplay's LoadAll is scoped to its own sheet resources. The editor Home importer/validator loops current source keys. LockedDesignImporter's special environment-name case controls its import contract. SplashTitleR3Validation also mentions home_play_button only as an aspect-ratio exception for a shared historical button; it is not a load/restore site. Actual Splash/Loading dependency graphs exclude both candidates and the installed Splash target uses other artwork. Approved Splash is preserved.

## Unity results and tooling correction

Unity **6000.5.9f1 (b57deb96f08d)**; isolated batch/nographics Edit Mode. Temporary fixture operates only on disposable instances and performs no prefab/scene save.

[Initial run](QA/EditorAudit/summary.json):18passed,1failed,0skipped. The failure was an audit fixture `TargetParameterCountException`: its reflection helper supplied only the RectTransform to Home.BuildInto, which has optional loader/typography arguments. Corrected the helper to call Home directly, preserving the initial source as `Sources/HomeEnvironmentPlayAuditEditModeTests.Initial.cs`. No production repair.

[Targeted corrected run](QA/ClearingVerification/summary.json):all3sprite-clearing cases passed,0failed/skipped. [Combined record](QA/CombinedVerification.json):**19distinct cases verified with latest result Passed**,22executions including the initial fixture error and two repeated clearing cases. This is not a claim that the first run was all green or that19cases were rerun afterward.

| Coverage | Distinct cases |
| --- | ---: |
| Import/recursive dependencies | 2 |
| Home fresh/legacy/current construction | 3 |
| Missing resource in fresh/legacy Home | 4 |
| Installed Home loader bypass | 1 |
| Retained Image/live target on three installed pages | 3 |
| Fresh ChapterMap/PreLevel clone path | 2 |
| Sprite-only clearing and installed builder behavior | 3 |
| Actual Splash/Loading dependency exclusion | 1 |

The four missing-resource checks pass by asserting the existing exception. They establish that archival requires a code fix; they do not verify safe missing-key behavior today. Clearing observations and requested-key lists are retained in the run directories.

## Scope and limits

[Final scope](QA/Scope.json):**all7,784task-start Assets/Packages/ProjectSettings files unchanged;0changed/added/deleted**. Temporary fixture/meta removed after each run. Added audit sources/evidence and updated documentation only. Raw logs remain in `/tmp/lor-home-environment-play-audit-r12/`; generated caches are excluded.

R11 APK unchanged:**824,676,966bytes**, SHA256 `64d521032ac0414a45aafa59225aa8ab3d96a1cafa2c4cf12493c687450915e5`. No new build or size saving. No Play Mode, rendered comparison, native click acceptance, physical-device execution, memory/performance measurement, commit or deployment. Source asset inspection is not a production-page render. Previous runtime/build acceptance remains historical. Python syntax, JSON/evidence links and documentation whitespace checks passed.

## Next implementation

Remove the two temporary Home loader dependencies (home_environment_generated and home_play_button) from fresh/legacy construction. Clear exactly one disabled ReferenceArtwork sprite assignment in each active Home/ChapterMap/PreLevel prefab and align their owning composition/control paths so cloning or migration cannot restore it. Preserve all objects, live targetGraphics, current environment/Play artwork and revisions. Add missing-key/identity regressions and native three-page plus Splash/Loading render/input checks; then archive both PNG/meta pairs with exact GUID/bytes and their distinct import contracts, followed by guarded packed-membership/size/signing checks. Preserve Login logo and currency imports; no blanket compression. Phone execution/hosting deferred, final effects last; human pacing, font provenance and Android lifecycle/performance/readability remain open.
