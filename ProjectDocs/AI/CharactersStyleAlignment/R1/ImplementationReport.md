# Visual Page36 — Characters / Hero Collection R1

Implemented the existing Characters route as a native page. [Review actual Unity captures](Characters_Alignment_R1_Review.html).

## Reference and scope

Project root `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`, recorded and used Unity6000.5.9f1. Read project instructions, historical handoff/current task, provenance and current Scenario workflow; inspected the existing dirty tree before edits. The old handoff's Loading scope is historical: the current user approved Characters after Story Dialogue. Existing Characters was a legacy screenshot with two placeholder hotspots, despite its presence in the route list.

Dedicated original: `/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_00_51 PM (1).png`,941×1672, copied unchanged to Sources and registered in REFERENCE_PROVENANCE.json. The full image remains documentation-only.

## Implemented

- Canonical `Resources/ProductionUI/Pages/Characters.prefab`, matching revision1 builder/view/runtime/layout and enabled meta-page cutover. Root marker is screenshot-free; no legacy hotspots or duplicate Canvas/EventSystem.
- Large approved Ivan/Firebird composition and eight independently isolated source portraits. Source title/frame/action shapes, dark scene, antique gold and existing embedded fonts.
- All/Rare/Legendary tabs and selectable preview cards. The selected detail changes name, portrait, description and concept rarity without changing the current hero or save.
- Current hero is Ivan, matching the existing Profile baseline. No hero-ownership, combat-stat, upgrade or bonus service exists. Stats show em dashes, Upgrade and Select remain disabled, booster illustrations are labelled examples, collection cards are marked previews. Future slots are labelled Coming later.
- Live RU/EN text; real saved header balances and level. Existing player/Settings, coin/gem Shop-category, energy, Back, WorldMap, Shop, Events and TeamHub routes. Repeated navigation commits once.
- Collection information overlay blocks underlying actions; native Close and router Back dismiss it before leaving the page. Layout fits the safe area with a full-cover clean environment.

The optional scope question received no reply before implementation. The conservative preview assumption was stated while continuing, consistent with the user's existing Profile approval. Functional hero rules and services are recorded in [HERO-COLLECTION-01](../../DeferredFeatures/HERO-COLLECTION-01.md) for a separate post-UI pass; no new economy was invented.

## Scenario and artwork

14 accepted Pixa source-alpha jobs, **42CU** total. No RGB generation, new character generation or duplicate paid submissions. One portrait output download was recovered from its existing accepted job. [API audit](QA/ScenarioAudit.json).

14 sprites: title, portrait/detail/card frames, Upgrade/Select surfaces, and eight portraits. Immutable submitted inputs, original regions/hashes, repair masks and output hashes are retained. Scenario alpha is combined with original source RGB. Bounded local repairs clear text/occupied interiors, remove detached residue, preserve intentionally opaque panel surfaces, and define the title silhouette. Normalized low-contrast grain replaces contaminated frame-interior donor marks. Source colors outside repair masks remain exact. [Checks](QA/AssetChecks.json) · [Provenance](AssetProvenance.json).

Reused unchanged: Home Ivan/Firebird and footer icons; Shop native header controls; ranking clean background, shared surfaces and local fonts; DailyReward Back; Login lock; existing booster icons. Black, white and magenta alpha composites were inspected. No OS/container font files added.

## Verification performed

- Unity6000.5.9f1 import/compilation, builder execution, prefab save/readback and enabled cutover: passed.
- Final Verify04: **2 Edit Mode + 5 Play Mode = 7 distinct tests passed**, zero failures/skips. [Full summary and history](QA/TestSummary.json).
- Edit checks: serialized bindings/sprites, disabled upgrade/select, no screenshot dependencies/legacy hotspots/nested Canvas, repeated runtime build idempotence.
- Play checks: real saved header updates; RU/EN complete text rendering; background coverage and safe-area bounds; actual UI raycast targets; every hero preview and rarity filter; unavailable actions grant/spend nothing; invalid requests ignored; overlay input blocking and Back dismissal; all header/footer routes and proper coin/gem Shop categories; duplicate/rebound/disabled callback guards and unchanged save.
- Seven final actual Unity captures: RU/EN941×1672; safeEN576×1280; safeRU1080×2400; Rare/MashaRU; FirebirdRU; information overlay safeEN. Simulated displays use the real router/native prefab and isolated storage.
- Earlier failures retained: Verify01 caught inherited header font sizing; Verify02 caught the English future-slot label's height. Fixed with approved font assignments and bounded text fitting. Verify03 passed; Verify04 repeated after the currency category route correction.
- Source asset masks/input integrity and 42CU billing audit passed. Four source Python files parsed; scoped source whitespace check passed. Browser review check recorded in QA/BrowserCheck.json.

## Remaining visual differences

This is a source-aligned implementation, not an exact screenshot replica. Main Ivan/Firebird pose and clean scene are reused approved artwork. Full-body art for the other heroes does not exist in this source, so their source card busts appear as smaller previews; original resolution limits enlarged detail. Concealed panel faces/title-lettering background are reconstructed. Typography follows selected project assignments, not a verified exact source font. Reference stars/stat values/skill levels/bonuses are replaced with honest preview/unavailable content. Footer art uses approved icons and TeamHub routing rather than implying a new guild service. Portrait/card sizes and spacing accommodate both languages. Future high-resolution portraits should preserve these source identities.

## Scope and checks not performed

[Scope audit](QA/ScopeAudit.json): exactly one existing production file changed, `ScreenRouter.cs`;45new files under Assets, including new code/art/prefab/cutover/tests and their metas. No deletions. Existing art/metas/GUIDs/prefabs, Gameplay, Splash, packages/settings and Android guards unchanged against the pre-task hash snapshot. Existing local changes were preserved.

Not performed: APK/build, physical-phone/touch or hardware-Back testing, full-project suite, profiling, real hero ownership/upgrade/backend integration, or exhaustive unrelated-page visual regression. No claim of those features being complete. Next: user visual review of the standalone R1 page.
