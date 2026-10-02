# Visual Page 35 — Story Dialogue R1

The active Dialogue route now uses a native Unity prefab with separate character sprites, a clean hall plate, a source-derived parchment/nameplate frame, localized text and a real Continue button. It replaces the legacy screenshot presentation and opaque text-cover rectangles. No full reference or scenery-contaminated UI rectangle is used at runtime.

## Reference and artwork

The existing `Art/ReferenceOnly/ApprovedScreens/Dialogue.png` was inspected and copied unchanged into Sources. Its source upload history is not inferred from its folder name. The layout follows its Ivan/Dobrynya composition, navy/gold speaker plaque, parchment and emerald control.

Scenario supplied four source-alpha extractions (Ivan, Dobrynya, panel, Continue) and one clean hall generation: **25 CU total**. Original character RGB and visible ornament pixels remain unchanged outside recorded repairs. The hall reconstructs scenery hidden by the characters/UI. The first background upload disconnected before generation; retry succeeded. The first Dobrynya POST disconnected without a job ID; a job-history check matched every recent accepted job to the other requests, establishing non-acceptance before replacement. No duplicate accepted generation was submitted.

Panel lettering/body/button removal uses documented source texture reconstruction with smooth lighting; the initial repeating donor pattern and button rectangle were corrected before final delivery. Source masks, immutable inputs, jobs, billing and output hashes are retained. Opaque panel faces remain opaque. Alpha was inspected on black, white and magenta. Occluded lower character edges are faded/removed behind the panel; Dobrynya retains the adjoining source chest prop. Hidden bodies are not claimed to be fully recovered.

For later scenes, the approved Loading landscape and isolated Firebird are reused unchanged. An initial Events Firebird candidate was rejected after Unity review because its cropped lower edge became visible at this scale. Ivan, Dobrynya or Firebird appears when applicable; narrator, elder and raven lines show scenery instead of an incorrect character portrait. Dedicated elder/raven artwork and scene-specific locations remain future art work.

## Runtime and story behavior

- Canonical prefab: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Dialogue.prefab`; owning builder: `ProductionDialoguePrefabBuilder`; revision 1 and Dialogue cutover agree.
- `ScreenRouter` binds the native button to the existing `ContinueAfterDialogue`. It no longer creates an invisible legacy Continue hotspot or screenshot text masks for this prefab.
- Existing Russian story content, milestones, completion saving and return routes are retained. Intro leads to Pre-Level; intermediate milestones return to Chapter Map; the eligible finale leads to Chapter Complete.
- Native input guards prevent a retiring page or repeated click from advancing multiple lines. Existing active-dialogue Back behavior remains unchanged.
- Added English speaker/dialogue entries for all 26 story keys. Locale switching reads the current sequence position without restarting or advancing it. Existing Russian `CurrentLine` and outcome/reaction consumers remain unchanged; new localized accessor is used by this page only.
- Existing selected embedded fonts are reused. Exact source typeface and embossed lettering are not claimed. Russian wording follows the game catalog, which differs from the sample reference paragraph.
- A pre-existing story validator defect surfaced in the existing malformed-catalog test. A null/empty milestone scene ID now reports validation errors rather than throwing from dictionary lookup. No gameplay rules or progress policy changed.

## Verification

Unity 6000.5.9f1 import/compilation and native prefab rebuild/readback passed. Verify02 passed **3 Edit Mode + 19 Play Mode = 22 tests**, including the new dialogue checks and existing story-data/milestone regressions. The final Firebird asset substitution passed a fresh rebuild plus all 7 Dialogue checks in Verify03; see QA/TestSummary.json for results.

Tests cover native prefab bindings, screenshot-free dependencies, idempotent construction, both language catalogs, repeated/disabled input, RU/EN switching without save mutation, reference and tall safe-area layouts/raycast targets, complete rendered text, intro progression, all seven milestones and established destinations. Tests use isolated save storage and simulated displays. Initial Verify01 failures (test skipping level 2 and the existing validator null case) are retained, followed by corrected passing results.

Fresh Unity captures include RU/EN 941×1672, safe EN 576×1280, safe RU 1080×2400, Firebird dialogue and finale narration. Review presents one portrait at a time, with the original separately selectable. Browser validation at 390px passed all 7 views with no errors, missing links or horizontal overflow. Source-script syntax and scoped source whitespace checks passed.

Not performed: APK build, physical phone/touch or hardware Back tests, full-project/adjacent-page suites, performance profiling, chapters beyond the existing Chapter 1 story catalog, or new backend features. This is not a pixel-perfect replica of every story scene.

## Scope

Existing production changes are limited to ScreenRouter.cs, StoryData.cs and StoryService.cs. Added Dialogue-only runtime/view/layout/builder, five PNGs and metas, prefab/cutover, English story JSON and targeted tests. Existing sprites, prefabs, GUIDs, Gameplay/Splash, packages, project settings and Android guards remain unchanged. Exact file lists are in QA/ScopeAudit.json. Task/session/provenance/workflow notes and review artifacts are updated separately.
