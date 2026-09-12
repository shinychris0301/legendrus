# Existing Home audit — Visual Page 03

This inspection supports the authorized Home design review. It is not a new Unity runtime verification or an approval of the historical references.

The active route remains Bootstrap → Splash → Loading → Home. `ProductionCoreLoopPageRegistry` selects `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab`. That prefab is a childless bootstrap: `ProductionUiFix01PageBootstrap.Awake` invokes `ProductionUiFix01Runtime.BuildInto`, and `BuildHome` owns the current composition. Its Editor owner is `Step20ProductionUiFix01SerializedPrefabBuilder.cs`. `Resources/ProductionUI/Wave01Pages/Home.prefab` is historical and inactive. Old reconstruction-matrix/marker claims do not prove current asset quality or final visual acceptance.

## Current behavior to preserve

| Control or value | Current source/behavior | Review treatment |
| --- | --- | --- |
| Gold, gems, energy | `DynamicHudBinder` consumes `GameState.Save` and refreshes on state changes; energy uses the existing clock-based service | Fresh-save fixture: 12,540 gold, 350 gems, 5/5 energy; never described as the user's save |
| Hero name | Hardcoded `Иван` | Retain the label and exact approved Ivan source art |
| Level | `Уровень 28` is hardcoded decoration; saved `level` is highest unlocked level | Show fixture `Уровень 1`; eventual implementation must bind real save state |
| Profile | `ScreenRouter.Show(Profile)` | Visible profile control |
| Settings | `ScreenRouter.Show(Settings)` | Approved Splash gear art |
| Play | `ScreenRouter.OpenChapterMap` | Preserve story/chapter gates; do not jump straight into gameplay |
| Four navigation actions | `WorldMap`, `Shop`, `Events`, `Characters` | Existing labels and destinations retained |
| Mail / reward | `Mail`, `DailyReward` | Visible buttons without invented unread/claim badges |
| Quest | `Соберите синие самоцветы`, static 220px fill on 420px track; no quest binder found | Keep wording without pretending a numeric progress/reward state exists |
| Currency pills / quest | No Home click bindings | Information surfaces, not extra actions |

There are nine Home route actions. They currently use separate transparent Hotspot objects. A subsequent production Home view should bind those actions to its visible native controls while retaining ScreenRouter as the navigation authority.

`OpenChapterMap` preserves pending post-victory dialogue and chapter-completion routing. Later level opening checks chapter-intro/pending-story state. Energy is spent at the existing StartLevel gate, with OutOfEnergy handling. None of these should be bypassed by a new Home Play button.

Home text currently has no RU/EN Home localization table or language-change consumer. The offline review uses live HTML text and a proposed English table; production should adopt the existing `DemoFeatureState.LanguageCode` boundary and shared typography. Source-font identity remains unverified.

Only the Splash meta-page cutover state exists locally. Several downstream destinations still use DesignScreens presentation plus inherited actions. Working Home navigation does not establish their production-UI completion. Mail claim remains an unavailable toast; character selection remains a toast rather than a persisted hero choice. DailyReward has an existing saved UTC claim path granting 500 gold, 20 gems and 1 energy; this review does not fabricate a claim-ready badge.

## Examined reference and asset problems

`Art/ReferenceOnly/ApprovedScreens/Home.png` and `Resources/DesignScreens/Home.png` are identical 941×1672 references, SHA-256 `7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8`. Their folder name does not establish approval for the aligned Home. The old concept uses a different uppercase logo/Ivan treatment, larger enclosing border, fixed resource/quest values and badges.

| Existing files under Assets/LegendsOfRus | Observed result |
| --- | --- |
| `Art/Production/Pages/Home/environment_home.png`; `Resources/ProductionUI/Assets/Fix01/bg_home_clean.png`; `Art/Production/Wave01Refined/Home/environment_clean.png` | Identical 941×1672 smeared environments with ghosted logo/hero/UI remnants and blur/stretch seams |
| `Art/Production/Pages/Home/hero_ivan_firebird.png`; `Resources/ProductionUI/Assets/Fix01/hero_ivan_firebird.png` | Screenshot-derived masked foreground, not a fully opaque screen. Actual black/white/magenta composite reveals retained sky/buildings, a straight cut near the bird's head, and CTA ornament below the torso |
| Six `Art/Production/Wave01Refined/Home/home_*.png` components and matching `Art/Production/CoreLoop/ReferenceMatch/home_*.png` | Opaque screenshot strips with surrounding scenery and burned-in text/values/badges; matching pairs are byte-identical |
| `Art/Production/Pages/Home/page_asset_manifest.json` | Its `existing-screenshot-free-production` description conflicts with examined pixels |
| `Resources/ProductionUI/Assets/Fix01/Icons/` and shared kit | Separate small graphic assets, but much flatter than the painterly approved family |

The masked foreground's alpha channel alone did not establish isolation. Its stored RGB initially resembled a full screenshot in the image viewer; the actual compositing check establishes the narrower, accurate finding of contaminated masked art. See `QA/ExistingHomeMask_Rejected.png`, clearly labeled asset QA rather than a page design.

Reusable sources include the actual approved Splash logo, Ivan, Firebird, emerald button and gear. Their source files are unchanged. No historical Home screenshot or screenshot-region crop is used by the new review HTML.

## Integration requirements after review

- Update the real Home prefab and its existing owner/configuration together; preserve GUIDs and the current startup/Canvas/EventSystem/router.
- Serialize clean artwork, live localized text and visible controls. Do not replace Home with the offline preview PNG.
- Bind resource/level displays to actual state; decide and implement any quest-state work explicitly before showing quest progress or rewards.
- Update the Home resource-preparation contract together with the asset cutover. Loading currently measures the Home prefab plus 22 texture operations from `ProductionUiFix01Runtime.GetHomeTextureResourceKeys`; retaining obsolete keys would give misleading preparation coverage.
- Add Home-specific full-Canvas scenery/shading and safe-area-aware controls. Preserve shared scaler behavior and verify reference/tall/safe-area layouts, input and Splash → Loading → Home after implementation.

Checks for this audit: instructions/source/config/prefab reads, hashes, PNG dimensions/alpha, direct image inspection, and browser composites. No Unity import/compilation, Play/Edit Mode tests, Android build or physical-device action was performed for Home review.
