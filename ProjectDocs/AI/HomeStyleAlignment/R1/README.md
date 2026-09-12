# Home R1 — Visual Page 03 design review

Status: **ready for visual review; offline HTML composition, not Unity, not yet approved.** Prepared following the user's “okay continue” to the proposed Home reference/asset audit and standalone portrait review. This phase does not change or establish acceptance of the delivered Splash/Loading implementation or Android APK.

Open [the standalone Russian portrait](Home_Alignment_R1_OFFLINE_NOT_UNITY.png) or [the interactive offline preview](Home_Alignment_R1.html). HTML must remain at this repository location so its relative links resolve. Its nine visible buttons identify existing destinations in the review note; they do not call Unity, alter saves, or navigate the game. The main portrait has no review collage or implementation labels over the design.

## Composition and sources

The welcoming winter village uses the accepted Splash's blue snow, warm sunrise, dark lacquer, antique gold and sapphire family. The exact approved logo, Ivan, Firebird, emerald button and gear are referenced directly from `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/`; their pixels, imports and GUIDs are unchanged. Home-specific placement changes only this offline composition. Native HTML supplies text, information panels and visible controls. The portrait PNG is review evidence and must never become a runtime screen background.

| Element | Selected source | Surface treatment |
| --- | --- | --- |
| Village | `home_environment_generated.png` | New clean 941×1672 RGB environment; no embedded UI or characters |
| Map | `nav_map_generated.png` | New 1254×1254 RGBA illustration with actual outer transparency |
| Shop | `nav_shop_generated.png` | New 1254×1254 RGB illustration on intentional opaque navy |
| Events | `nav_events_generated.png` | New 1254×1254 RGBA banner with actual outer transparency |
| Characters | `nav_characters_generated.png` | New 1254×1254 RGB helmet illustration on intentional opaque navy |
| Mail | `mail_medallion_generated.png` | New 1254×1254 RGBA medallion, transparent outside; navy interior preserved |
| Daily reward | `reward_medallion_generated.png` | New 1254×1254 RGB medallion with intentional opaque navy |
| Resource glyphs | Existing `Assets/Fix01/Icons/icon_coin.png`, `icon_gem.png`, `icon_energy.png` under ProductionUI | Existing simple small HUD pictograms |
| HUD, navigation frames, quest surface, lower shade | Native HTML/CSS | Navy/gold panels and full-height background/shade treatment |

The footer uses circular HTML image presentation; complete medallion rims and tips remain visible. This does not turn the reward source into a transparent PNG. Ivan's lower edge uses a presentation fade; the approved source is unmodified. No screenshot crop, destructive background-removal script or OS font file is used.

Exact prompts, reference paths, selected outputs and rejected generation attempts are preserved in `BackgroundPrompt.json`, `NavArtA_Prompts.json`, `NavArtB_Prompts.json`, `MailArt_Prompts.json` and `RewardArt_Prompts.json`. Early shop/helmet/reward outputs contained baked checkerboards; the selected corrected outputs intentionally retain navy surfaces. Final mail correction has real transparency. Those distinctions are confirmed by pixel alpha histograms and visual composites, not inferred from extensions.

Typography uses the browser's local serif fallback stack. It is not claimed to be the exact source font or a Unity font assignment. No fonts were copied or redistributed. Russian labels are live; English labels are proposed review translations. Production localization remains future work.

## Data and behavior boundaries

12,540 gold, 350 gems, 5/5 energy and level 1 are fixtures from the project's fresh-save defaults, not readings of the user's save. Home already has real currency and energy services; later implementation must bind them. The old Home level 28 and quest fill were hardcoded decoration. The preview retains `Соберите синие самоцветы` without inventing quest progress, rewards, unread mail or claim badges.

Existing actions retained in the preview map: Profile, Settings, **OpenChapterMap** (Play), WorldMap, Shop, Events, Characters, Mail and DailyReward. Play must preserve existing chapter/story gates; energy remains controlled at the existing level-start boundary.

The [existing Home audit](ExistingHomeAudit.md) identifies the active bootstrap prefab/runtime/Editor owner, rejected historical art and integration requirements. Historical “screenshot-free” manifest descriptions conflict with inspected assets. The old Home reference supplied village context only; it is not used by the HTML.

## Checks performed

- Read project instructions, handoff, current task and reference provenance; inspected working-tree changes before edits. Recorded root and Unity 6000.5.9f1 (b57deb96f08d).
- Traced active Home ownership, state, routing and Loading's Home resource-preparation contract by source/prefab inspection.
- Examined reference/source hashes, PNG dimensions/color types and current Home art, including its contaminated masked foreground over contrasting backgrounds.
- Rendered the final HTML in local Chrome via CDP at four fixtures: Russian 941×1672; Russian 576×1280 with simulated 50px top/24px bottom insets; Russian 1080×2340; English 941×1672. Insets are review fixtures, not measured phone properties.
- All four fixtures passed image decoding, nine button center hit tests and pointer-click action mappings, control safe-area bounds, avatar containment, label horizontal overflow and page overflow checks. These assertions cover the HTML only. Results and geometry are in `QA/BrowserChecks.json` and the adjacent capture JSON files.
- Visually inspected the final main/tall portraits and English layout. No visible clipping, label overlap or bottom shading seam in the inspected fixtures. Root and a separate agent inspected the final mail/reward controls at reference and phone size.
- Inspected six new foreground/control illustrations plus approved Ivan/Firebird over black, white and saturated magenta. `QA/AlphaQA.png` is clearly an asset QA sheet, not the page deliverable. `QA/AlphaQA.json` records alpha histograms. No visible baked checkerboard or scenery contamination in selected artwork; intentional navy surfaces retained.
- Rechecked all 78 captured production hashes and production working-tree status against the pre-change record. Details, exact selected-source hashes and the review file inventory are in `ProvenanceAndChecks.json`.

Reproduce the browser fixtures from the project root with Node 22 and the existing local Chrome:

```sh
node ProjectDocs/AI/HomeStyleAlignment/R1/QA/CapturePreview.mjs
```

The helper uses a temporary local browser profile and local file URLs, with no installed dependencies or external services. It regenerates offline screenshots and QA JSON; it is not a Unity test.

## Not performed and next action

No production Home implementation/import, prefab migration, Unity compilation, Edit/Play Mode tests, real game navigation or save changes, Android build, installation, physical-phone testing, performance test, full regression or source-font identity verification was performed in this phase. Browser responsiveness does not establish Unity/device responsiveness. Splash, Loading, Android guards, packages, Player/signing settings, existing source assets and GUIDs remain unchanged by this review.

Next: review the standalone Home portrait with the user. Following acceptance or requested adjustments, implement the real Home prefab and its existing owner together with localized live state and visible controls. Update the obsolete 22-texture Home preload contract alongside the asset cutover so Loading measures the resources actually used. Preserve startup/router/services and verify the full Splash → Loading → Home path and portrait/safe-area behavior in Unity. No Home approval is inferred from completion of this review package.
