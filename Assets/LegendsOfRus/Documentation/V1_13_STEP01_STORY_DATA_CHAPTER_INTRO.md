# v1.13 Step 1 — Story Data and Chapter Intro

## Released flow

On the first attempt to open a Chapter 1 level, `ScreenRouter` asks
`StoryService` to start `chapter01.intro`. The three localized lines advance on
the approved Dialogue screen and return to `PreLevel`. Completing the final
line stores the scene ID in save-version 5 data, so the intro does not repeat.

After a victory, the last completed level's first `dialogue.*` trigger is
resolved to a story scene. Completed scenes are skipped on replay. Ordinary
scenes return to `ChapterMap`; the Chapter 1 finale returns to
`ChapterComplete`.

## Data boundary

- `Config/chapter01_story.json` owns scene IDs, triggers, line order,
  localization keys, and return routes.
- `Localization/story_ru.json` owns Russian speaker names and narrative copy.
- `StoryData.cs` loads, localizes, validates, and sequences those records.
- `PlayerSaveData.completedStoryScenes` owns persistent completion state.

The story catalog covers the intro plus all seven released Chapter 1 dialogue
triggers. Validation rejects missing/duplicate IDs, duplicate triggers, invalid
routes, empty scenes, missing Russian keys, and level dialogue triggers without
a matching scene.

## Visual boundary

Every approved PNG remains byte-identical to v1.12 Step 3. The existing
Dialogue composition is reused unchanged. Two runtime UI masks cover only the
baked speaker and narrative text regions, then render the active Russian line
from localization data. Gameplay art, board geometry, HUD, mechanics, balance,
and tutorial sequencing are unchanged.

## Verification

`ProjectValidator` includes the story catalog, Russian localization, runtime,
self-test, and PlayMode coverage. `Step13StoryDataSelfTest` checks validation,
trigger coverage, localization, intro sequencing, persistence idempotence, and
return routes. `StoryDataRuntimePlayModeTests` covers the same contracts at
runtime, including first-launch completion and non-repetition.
