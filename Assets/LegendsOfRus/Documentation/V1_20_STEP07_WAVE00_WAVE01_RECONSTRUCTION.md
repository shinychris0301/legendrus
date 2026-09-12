# v1.20 Step 7 — Production Asset Reconstruction Wave 0/1

Step 7 authors the shared visual foundation and the 12-page core-loop/story-spine candidate reconstruction while preserving the Step 6 functional baseline.

## Wave 0 — shared foundation

The existing canonical kit is normalized into explicit idle/pressed/disabled button states, shared panels, controls, ornaments, resource/navigation icons, VFX hooks, safe-area rules, and a live-Cyrillic typography contract. No font binaries are bundled. Final licensed Cyrillic type selection remains a visual-cutover prerequisite.

## Wave 1 — 12 pages

Pages: SplashTitle, Loading, Home, WorldMap, LevelSelect, PreLevel, Gameplay, Pause, Victory, Fail, ChapterComplete, Dialogue.

All Step 6 Wave 1 functional asset requirements are mapped to screenshot-free page-specific or Wave 0 shared assets. Nine pages have direct approved references. SplashTitle, Pause, and ChapterComplete remain style-system-derived candidate compositions until a direct approved reference is supplied/accepted; they are not falsely labeled reference-matched.

## Runtime policy

`Assets/LegendsOfRus/Resources/ProductionUI/Pages/` and `ProductionCoreLoopPageRegistry.cs` remain byte-frozen from Step 6. New visual prefabs are generated under `ProductionUI/Wave01Pages/` and are inactive. This makes art review possible without changing navigation, gameplay, save/economy, or backend behavior.

Full-screen QA previews live only in `ProjectDocs/V1_20_STEP07/Wave01Previews/`; candidate runtime prefabs are forbidden from depending on that folder, `Art/ReferenceOnly`, or `Resources/DesignScreens`.
