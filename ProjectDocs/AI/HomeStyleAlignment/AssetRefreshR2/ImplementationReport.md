# Home control assets R2

The user explicitly requested generating and replacing the remaining Home assets from the references, excluding the background, character, logo and Start button. This pass creates fourteen individual control PNGs and connects them to the existing Home prefab, live text, state and navigation. The reference remains Home R1; the approved Splash crest supplies the antique-gold/navy material family.

Complete: the active Home prefab and owner agree on revision 4. Final **22/22 Edit Mode and 20/20 Play Mode checks passed**, with zero failures/skips. Fourteen generated controls are installed, captions fit their new frames, and actual Unity portraits have been reviewed. Final user visual acceptance is not inferred. Main portrait: `UnityVerification/Home/Home_941x1672_RU.png`.

## Assets and integration

All fourteen selected source PNGs are saved under `Sources/` and copied unchanged into `Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/`. Six existing control filenames are replaced while retaining their GUIDs; eight new Home-specific resources are added. `SelectedAssets.json` records exact paths, dimensions and hashes.

| Generated resource | Use |
| --- | --- |
| `hud_frame.png` | Blank navy/gold top HUD, sliced so corners retain their proportions |
| `avatar_ring.png` | Frame around the existing Ivan portrait crop |
| `resource_capsule.png` | Shared empty capsule behind the three live resource counters |
| `coin_icon.png`, `gems_icon.png`, `energy_icon.png` | Separate gold, sapphire and energy symbols |
| `settings_medallion.png` | Existing Settings action with a Home-specific generated sprite |
| Four `nav_*_generated.png` files | Complete map/shop/events/characters cards with blank areas for live labels |
| `mail_medallion_generated.png`, `reward_medallion_generated.png` | Existing Mail and DailyReward actions |
| `quest_panel.png` | Empty framed surface behind the existing localized quest copy |

The background, Ivan, Firebird, approved logo and Play/Start artwork remain the same resources with the same authored geometry. No new character, background, logo or Start asset was generated. Home's previous shared gear and legacy resource glyph dependencies are replaced by Home-specific sprites; those shared assets remain untouched.

`ProductionHomeRuntime`, `ProductionHomeView`, and the existing `Step20ProductionUiFix01SerializedPrefabBuilder` share the control composition and revision checks. The owner migrates existing objects in place. Generated sprites replace visible native frame substitutes; native masks, opacity backings and input graphics remain functional UI infrastructure. The existing nine actions, chapter/story gates, save/energy services and shared typography remain.

Labels and counters are live and localizable. Four navigation labels move up 7 logical pixels and change from 25 to 23 logical font size to fit the new ornamented cards. Wording, button positions, other labels and protected artwork remain unchanged. No reference screenshot or screenshot-region crop is a runtime dependency.

Loading now prepares 19 distinct texture requests and one prefab request through its existing measurable loading flow. No timer, new startup system, duplicate Canvas/EventSystem or alternative navigation flow is introduced.

## Generation and alpha verification

Built-in imagegen was used throughout. Exact prompts, reference roles and tool output paths are in `Sources/FramePrompts.json`, `SmallControlPrompts.json` and `NavTransparencyCorrectionPrompts.json`. Initial `NavPrompts.json` and `NavSourceChecks.json` are marked superseded; `FinalNavSourceChecks.json` verifies the final card selection. Rejected checkerboards and initial margin variants are diagnostic history, outside runtime dependencies. No fallback API/CLI, OS font copying or post-generation source-pixel edits were used.

Selected resources were inspected over black, white and saturated magenta, including intended control sizes. HUD and quest surfaces are deliberately opaque RGB. Transparent sprites retain actual RGBA; some generated interiors are near-opaque at alpha 252–253, so native opaque navy backings preserve panel opacity where necessary. QA images are explicitly offline diagnostic compositions, not page implementations or Unity captures.

The Home-only importer preserves source dimensions and uses FullRect sprites, BoxFilter mipmaps and trilinear filtering for the controls. The three 2172×724 frame/capsule sources use a 4096 import limit; square controls use 2048. HUD/quest/capsule have explicit slicing borders. The existing environment remains bilinear without mipmaps. This configuration does not change shared Splash/Loading imports, packages, Player settings, signing or Android guards.

## Verification and limits

The installed Unity **6000.5.9f1 (b57deb96f08d)** compiled/imported the final sources. The Home-only owner saved and read back revision 4 outside Play Mode. Tests used isolated `/tmp` save/preferences storage and the real graphics renderer, without accessing the user's normal save.

- **22/22 Edit Mode:** 15 individual layer import/source cases, four Home structural/binding/resource/protected-art cases, and three existing Splash wiring cases. Checked exact selected PNG bytes, dimensions/alpha/import formats/slicing, nineteen prepared textures, prefab identity, opaque backing order/containment and excluded artwork geometry.
- **20/20 Play Mode:** seven Home cases and thirteen existing Splash/Loading/preparation cases. Exercised actual prepared resources, live balances/energy/level and reentry, RU/EN, all nine pointer routes with repeated taps and once-only feedback, chapter/story/energy gates, real Splash → Loading → Home, progress/cancellation/error/retry, and actual rendering.
- Seven Home captures cover 941×1672 RU, 576×1280 with simulated top50/bottom24 insets, 1080×2340, 1080×2400 with simulated side/top/bottom insets, English, Russian roundtrip and maximum balances. Bounds, actual caption glyph completeness/placement, hit centers, background/shade coverage and repeated-layout stability passed. These are Unity display fixtures, not phone captures.
- Direct and independent visual review found no caption/ornament collision, backing protrusion, checkerboard, clipped contour, lower seam or safe-area overlap. Extreme balances fit as two lines inside the capsules; they are necessarily small at the 576px fixture.
- Decoded pixel comparison against the preceding Home delivery found **366,230 changed pixels, all confined to the HUD, navigation and footer control rectangles; zero outside those areas**. The Russian language roundtrip has zero differing pixels. The Splash comparison differs at only 49 pixels, each at most 1/255 in a channel; all protected Splash source hashes remain exact.

Final summaries are `UnityVerification/FinalEditMode.json` and `FinalPlayMode.json`; actual captures and metadata are in `Home/`, `LoadingRegression/` and `SplashRegression/`. Initial revision 3 evidence is retained as historical `InitialRevision3*` files. That first passing run led to the final caption/transparent-edge correction; its images are not the final delivery.

`BeforeHashes.json` and `PreservationChecks.json` document source/GUID/object preservation. `SourceChangeManifest.json` lists the exact authored production/source/test files; `ExistingSourceChanges.diff` compares changed existing source and prefab files to the start of this task. The final audit checked 131 baseline files: 115 unchanged and exactly 16 intended modifications. All 64 recorded existing GUIDs remain unchanged; eight new GUIDs are unique. Of 50 original RectTransforms, 42 remain, eight obsolete navigation illustration/ornament children were removed, and six backing RectTransforms were added. Only four retained navigation label positions and font sizes change; seven protected object groups and their 32 component blocks remain exact. The authored production/source/test inventory is 32 files: 16 modified and 16 added (eight PNGs plus metadata). Source/metadata preservation is independently recorded in `QA/FinalSourceAudit.json`.

Authored C# and documentation whitespace checks passed. Unscoped Git whitespace output contains Unity's ordinary serialized blank fields; those remain as saved by Unity. No staging, commit, reset or removal of user changes was performed. User edits and earlier acceptance decisions are retained.

No APK/AAB build, physical-phone installation/rendering/input test, performance/full project regression suite, release/custom-signing validation, physical display-resolution-change text-refresh test or exact font-identity verification was performed for this refresh. The previous APK predates the generated Home controls. Final visual acceptance remains with the user.
