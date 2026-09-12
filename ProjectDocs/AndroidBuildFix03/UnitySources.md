# Official Unity documentation consulted

Build options and ordered callbacks:

```text
https://docs.unity3d.com/6000.5/Documentation/ScriptReference/BuildOptions.AutoRunPlayer.html
https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Build.IOrderedCallback-callbackOrder.html
https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Build.BuildPlayerProcessor.html
https://docs.unity3d.com/6000.0/Documentation/ScriptReference/EditorApplication-isPlayingOrWillChangePlaymode.html
```

Addressables 2.8 documentation and lock-file ownership:

```text
https://docs.unity3d.com/Packages/com.unity.addressables@2.8/manual/build-player-builds.html
https://docs.unity3d.com/Packages/com.unity.addressables@2.8/manual/builds-full-build.html
https://docs.unity3d.com/6000.0/Documentation/Manual/upm-conflicts-auto.html
```

The archive manifest declares Addressables 2.8.1. The actual installed version in the user's project was not inferred from the PackageCache suffix in the log. Export Build Diagnostics reports the registered version. Official documentation does not establish that the project's own v1.17 policy has been approved.
