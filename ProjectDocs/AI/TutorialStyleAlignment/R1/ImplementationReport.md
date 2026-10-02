# Visual Page 31 — Tutorial / Help R1

Implemented the user-approved adaptation of `tuturial.png`: its dark navy panel, gold crest/perimeter, red Close and emerald control now frame a four-section Help guide. The supplied image depicts a Special Offer; commercial content was intentionally replaced with localized instruction, not replicated as a purchase UI.

## Behavior and integration

Splash Help opens the native `TutorialOverlay` page. Tabs and Previous/Next navigate Basics, Specials, Boosters and Obstacles. Done, Close and system Back restore the origin; Splash and Home origins and repeated route refresh were verified. Illustrations use current Gameplay sprites. Reading Help does not spend inventory, change save/progress or mark in-round tutorials complete. Existing `LevelTutorialPresenter` and Gameplay UI/rules are untouched.

New active prefab `Resources/ProductionUI/Pages/TutorialOverlay.prefab`, owning `ProductionTutorialPrefabBuilder`, runtime/view/responsive layout, revision1 cutover state, three sprites/metas and targeted Edit/Play tests. One existing source changed: `ScreenRouter.cs` replaces the legacy Help hotspot and retains the return origin.23 new Assets files; no deletions. Prior asset GUIDs/metas, all other page prefabs/art, Gameplay, packages, project settings and Android guards match the starting hashes. [Exact scope](QA/ScopeAudit.json).

## Art / Scenario

Three completed Scenario `model_pixa-background-removal` jobs, **9 CU total**, no RGB generation or duplicate submission. Original borders/crest/control RGB retained outside recorded repair regions. Occupied panel areas repaired using a tiled source navy patch; hidden upper-right border reconstructed by mirroring its left counterpart. Old lettering removed locally from the emerald button. Black/white/magenta transparency checked. Initial stretched donor and broad title mask were corrected before final verification; first Unity evidence retained. [Provenance](AssetProvenance.json) · [Job/cost audit](QA/ScenarioAudit.json).

Existing source-derived ranking card/tab/navigation resources and clean background reused unchanged. Source font identity remains unknown; approved embedded fonts render all instructional text live in Russian and English. Repaired texture, layout and new headings are adaptations, not pixel-exact source reconstruction. The clean environment is reused, not the screenshot background.

## Checks performed

- Unity6000.5.9f1 import/compile, canonical prefab rebuild and saved binding readback passed.
- Verify02: **4 Edit Mode + 8 Play Mode checks passed**, zero failures/skips, across Tutorial and Login. Includes native bindings/cutover, no screenshot dependencies or extra Canvas/hotspots, builder idempotence, all sections/locales, Previous lower bound, Next/Done, Close/system Back, duplicate binding/action guards, repeated origin route, unchanged saved state, button raycasts and safe-area containment. Login regression includes password visibility/clearing, unavailable actions, guest entry and origin navigation.
- Nine actual Tutorial portraits across941×1672 RU,576×1280 EN simulated safe area,1080×2400 RU simulated safe area. Seven Login regression captures retained. Final RU obstacles and tall EN specials visually inspected; earlier RU basics and EN boosters also reviewed.
- Source input hashes, original RGB outside final repair masks, three asset hashes and complete Assets/Packages/ProjectSettings scope audited.
- Review browser390px validation recorded in QA/BrowserReview.json. Source script syntax and scoped whitespace checked.

## Checks not performed

No APK build, physical phone/touch testing, full-project test suite, profiling, online/backend checks or interactive in-board tutorial redesign. This page is a read-only guide with working navigation, not a playable practice board.

Next: user visual review of Tutorial / Help R1. Existing post-UI Failure continuation, SMS and account integration tasks remain scheduled separately.
