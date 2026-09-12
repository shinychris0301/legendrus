# Splash/Title — Uploaded Project Audit and Limited Source Patch

## Result

**Requested R2 visual implementation: BLOCKED, not completed.**

The supplied project has been inspected and two existing C# files have been changed for navigation and honest validation. The supplied artwork has not been re-created, cleaned, cut, regenerated, or accepted as true-transparent R2 artwork. The saved Splash prefab has not been activated or rebuilt, because the available sources do not satisfy the requested asset requirements.

## What was actually provided

| Item | Finding |
|---|---|
| Archive | `LegendsOfRus_Unity_v1_20_VisualPage01R_SplashTitle_ReferenceAccurate_FULL.zip` |
| Entries | 2,991 |
| PNG files | 739 |
| Project version recorded | `ProjectSettings/ProjectVersion.txt`: `6000.5.9f1` |
| Installed Editor in this inspection environment | No Unity executable found; installation/version cannot be runtime-confirmed here |
| Enabled build scene | `Assets/Scenes/Bootstrap.unity` |
| Direct approved Splash reference | `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/SplashTitle.png` |
| Actual reference dimensions | 941 × 1672 pixels |
| Active R sprite directory | `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleFinal/` |
| Active manifest | `SplashTitleFinal/manifest.json`, schemaVersion 2, 18 layers |
| Font binaries in supplied project | None found (`.ttf`, `.otf`, `.woff`, `.woff2`) |
| Nested asset archives | None found |
| Later R2 control names from the prior conversation | `start_button_base.png`, `language_selector_base.png`, `tagline_divider.png`: not found |
| Later R3 rebuilder | `Step20SplashTitleR3PrefabRebuilder.cs`: not found |

Those absent filenames alone are not the diagnosis. The active source code and the visible pixels also confirm that this is the older R implementation: version/language/start/settings/account/help/tagline/legal labels are loaded as raster sprites, characters share a combined base, and several masks retain obvious background fragments.

The search was across the extracted project, not just one proposed R2 folder. This does not imply the resources are absent from the user's computer or from earlier packages not included in this upload.

## Actual startup and rendering ownership

The uploaded build scene contains scene settings, not a manually placed Splash controller. `RuntimeBootstrap.Boot`, registered for `AfterSceneLoad`, creates or locates the services, initializes `ScreenRouter`, and calls `Show(ScreenId.SplashTitle)`.

Before this patch, the active path was:

```text
Assets/Scenes/Bootstrap.unity
  -> RuntimeBootstrap.Boot
  -> ScreenRouter.Initialize / Show(SplashTitle)
  -> ScreenRouter.BuildDesignLayer
  -> DesignScreenRegistry.ResourceKey (empty because the meta cutover is enabled)
  -> DemoFunctionalPageRuntime.TryBuild
  -> ProductionMetaPageRuntime.TryBuild
  -> ProductionSplashTitleRuntime.TryBuild
  -> Resources.Load<Sprite>("ProductionUI/SplashTitleFinal/..."), AddImage calls
  -> ScreenRouter.BuildInteractionLayer: five separate Hotspot objects
```

**The original router did reach the production splash indirectly.** The earlier preliminary observation that it bypassed the entire production splash route was too broad. What it bypasses is the **saved canonical prefab**, because `ProductionMetaPageRuntime` special-cases Splash into the procedural builder.

After the source patch, `BuildDesignLayer` dispatches core production pages, then enabled meta production pages, then legacy/demo presentation directly. The remainder of the legacy Splash construction is unchanged. This simplification is not presented as a completed visual repair.

### Saved prefab that is currently bypassed

`Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab` has the root name `Wave01_SplashTitle`. Its source dependencies include the older `Art/Production/Pages/SplashTitle/environment_title.png`, `hero_firebird_title.png`, `logo_plaque.png`, `ambient_sparks.png`, and a Wave0Foundation primary button. It also has repeated `LiveText` child names.

Activating that prefab as-is would not implement the requested R2 screen. It was therefore left unchanged rather than replacing the active page with a different, older layout. The Wave01 candidate prefab was also left unchanged.

### Builder ownership

The requested `Step20SplashTitleR3PrefabRebuilder.cs` is not supplied. Existing older candidate/cutover tooling remains in the project. This patch does not claim to have solved R3 rebuild ownership or proven domain-reload persistence. Those checks must be completed against the actual newer R2/R3 assets and implementation, not guessed from names or previous conversation text.

## Asset-to-element inventory

All paths below are relative to `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleFinal/`. The machine-readable inventory gives complete paths, source hashes, pixel dimensions, reference rectangles, and alpha statistics.

| Intended element | Supplied source | Current status / blocking issue |
|---|---|---|
| BackgroundEnvironment | `background_clean.png` | RGB environment with conspicuous blurred removal patches. Opaque backgrounds are allowed; the issue is the un-restored image content. |
| Firebird | `firebird.png` | Mask also contains sky/branches and a corner of a character. |
| Ivan and Masha | `characters_upper.png` | Combined base, not two isolated characters; background and holes cut around UI are visible. |
| Ivan motion | `character_male_micro.png` | Partial overlay with background, not an independent complete Ivan character. |
| Masha motion | `character_female_micro.png` | Partial overlay, not an independent complete Masha character. |
| Character glow | `character_rim_glow.png` | Shared-region effect, preserved but not newly accepted. |
| Firebird aura / shimmer | `firebird_aura.png`, `firebird_shimmer.png` | Legacy-region effects; separate edge acceptance remains pending. |
| TitleLogo | `title_logo.png` | Candidate retained unchanged. Decorative title lettering may remain artwork; detailed acceptance is not claimed. |
| VersionText | `version_v1_20_0.png` | Fixed raster version, not a live version-source binding. |
| LanguageSelector | `language_frame.png` | Raster Russian label; blue polygon/background remains outside the frame. |
| StartButton | `start_button_frame.png` | Raster “Начать” text and surrounding cloth/background included in the mask. |
| Start glow | `start_glow.png` | Derived from the older control region; preserved, not newly accepted. |
| SettingsButton | `icon_settings.png` | Icon and label in one raster layer; dark background surrounds the icon. |
| AccountButton | `icon_account.png` | Icon and label share a raster layer; cannot serve as a live label without the correct separate source. |
| HelpButton | `icon_help.png` | Icon and label share a raster layer. |
| Decorative divider + TaglineText | `tagline_ornament.png` | Divider and wording combined; pieces of neighboring labels remain at its top edge. |
| LegalText | `legal_ru.png` | Rasterized copyright line, not a live text element. |
| SupportingText | No distinct source/binding located in the active builder | The reference has one tagline below the divider; no extra supporting copy was invented. |

Source alpha percentages in `asset_inventory.json` describe file pixels, not whether isolation is correct. A polygonal crop can have many transparent pixels and still retain the very background fragments that the user rejected.

### Evidence files

- `AlphaInspection_StartButton.jpg`: the actual supplied Start control on black, white and magenta.
- `AlphaInspection_Controls.jpg`: logo and text/control resources on those three backgrounds.
- `AlphaInspection_CharactersVFX.jpg`: characters, Firebird and effect resources on those three backgrounds.
- `BackgroundSourceInspection.jpg`: the supplied background by itself.

These are **source-asset inspection sheets, not Unity runtime captures and not replacement game artwork**. They were made by compositing the existing RGBA files for inspection. Source PNG bytes were not altered. All sheets live under `ProjectDocs`, outside runtime resources.

## Changes made

### 1. Existing `ScreenRouter.cs`

Path: `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`.

The direct production-meta dispatcher removes reliance on the demo dispatcher for enabled production pages. All five Splash callbacks now capture the creating route generation and reject a stale callback, a callback from another screen, or a callback while a modal is present. The first navigation increments the generation, invalidating other pending Splash navigation actions.

The source-bound Back helper now observes the existing Splash auxiliary return override. The override is retained while the directly opened auxiliary page refreshes and cleared whenever that page is left, not only on navigation to Home.

Existing services, Start → Loading → Home behavior, Loading completion generation guard, Canvas/safe-area infrastructure, input backend, and destination callbacks are otherwise preserved. The old hotspot geometry remains deliberately unchanged; **that is a limitation, not fulfillment of the requested real R2 button assembly**.

### 2. Existing `Step20SplashTitleProductionSelfTest.cs`

Path: `Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleProductionSelfTest.cs`.

The old test rejected live text and used filenames plus a source reconstruction metric to claim visual success. The replacement separates source checks from production readiness. The readiness menu reports the procedural-prefab bypass, active legacy raster layers, combined character base, missing approved production font, and obsolete saved candidate instead of claiming visual acceptance.

The original menu path is preserved. A separate source-only check menu is added. A source check passing does not establish that buttons were clicked, the Editor compiled, fonts rendered, or runtime frames matched.

### Added files

`README_SPLASH_R_INTEGRATION_FIX01.md`, `Tools/verify_splash_r_integration_fix01.py`, and the contents of this audit folder. `source_changes.diff` contains the exact two-file patch. No asset-generation script or new runtime art is included.

## Functionality and typography limitations found in the supplied code

All five requested destination routes exist in source. However, `SetDemoLanguage` explicitly describes demo language state and pending full localization. `ContinueAsDemoGuest` explicitly creates a guest demo state and says a cloud account is not connected. The help route is the existing functional-demo tutorial page. This patch preserves those destinations; it does not claim to add authentication or full localization.

`Art/Fonts/Production/README.md` explicitly documents an empty production-font slot and a development-only built-in fallback. No approved font binary was found. No font was downloaded, substituted or redistributed. The actual approved wording visible in the reference must be preserved during the later live-text assembly.

## Validation and preservation

| Check | Result |
|---|---|
| Static source/archive assertions | 19/19 passed; details in `source_verification.json` |
| Lexical delimiter balance for both changed C# files | Checked; not a C# compiler or Unity assembly check |
| Existing changed files | Exactly the two C# files listed above |
| Existing `.meta` files preserved | 682, byte-for-byte |
| Original project files deleted | 0 |
| Artwork / prefabs / scenes / package files / ProjectSettings changed | 0 |
| Actual configured reference geometry read | 941 × 1672 |
| Unity compilation | Not run: no Unity executable in this environment |
| Startup and all five buttons exercised in Play Mode | Not run |
| 9:16 / 9:19.5 / 9:20 device-layout checks | Not run |
| Domain reload / restart / automatic rebuild persistence | Not run |
| Actual Unity screenshots / reference overlay | Not captured |
| R2 transparent-asset / live-text visual acceptance | Blocked; not claimed |

The patch preserves existing `.meta` files rather than inventing replacement GUIDs. The uploaded archive does not contain a `ScreenRouter.cs.meta`; the overlay does not create or overwrite one, so an existing local metadata file is preserved when the patch is merged.

The full-project archive contains the same limited patch as the small patch archive. Neither archive is labeled or accepted as the completed R2 implementation.

## Input needed to finish the requested visual implementation

Provide the already-created **Visual Page 01-R2 — Splash/Title True Transparent Asset Reconstruction** asset package, with its existing metadata/manifests where available, or the newer R3/FIX project that contains those resources. The current project's exact file structure and legacy routing have now been established. There is no need to regenerate artwork or repeat a style prompt; the missing input is the actual existing newer asset files.
