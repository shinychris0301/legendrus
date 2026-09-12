# Unity 6000.5.9f1 warning cleanup — v1.5

At this stage the messages are warnings, not compiler errors.

## Code warnings fixed in v1.5

### Object.FindFirstObjectByType
Changed to:

`Object.FindAnyObjectByType<ScreenRouter>()`

We only need any existing ScreenRouter, so deterministic instance-ID ordering is unnecessary.

### Dynamic Batching PlayerSettings API
The old editor utility called:

- `PlayerSettings.SetDynamicBatchingForPlatform`
- `PlayerSettings.GetDynamicBatchingForPlatform`

Unity 6000.5 deprecates those APIs. v1.5 no longer calls them.

## One-time Unity Player settings

After opening the project and allowing compilation to finish:

### 1. New Input System

Run:

`Legends of Rus > Unity 6 > Apply Input System (New)`

Then restart Unity once if requested.

Manual equivalent:

`Edit > Project Settings > Player > Other Settings > Configuration`

Set:

`Active Input Handling = Input System Package (New)`

### 2. Dynamic Batching

Open:

`Edit > Project Settings > Player > Other Settings > Rendering`

Turn OFF:

`Dynamic Batching`

Do this for the active Android Player / Build Profile if the setting is overridden there.

## Expected result

After the editor restart and recompilation:

- no CS0618 warning from RuntimeBootstrap
- no CS0618 warnings from Unity6ProjectSettingsMigration
- no "project uses Input Manager" warning
- no Dynamic Batching deprecation warning

The project should then be ready for Play Mode testing.
