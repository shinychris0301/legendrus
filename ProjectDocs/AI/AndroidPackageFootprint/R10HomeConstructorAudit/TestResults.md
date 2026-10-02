# Legacy Home constructor resource audit R10 — 2026-10-01

## Result

Neither audited resource can be archived unchanged today. `home_logo` is visible Login artwork. `home_hero_firebird` is superseded in completed Home, but still required by fresh construction and legacy migration. Removing it now makes both paths throw. The next safe implementation is to remove that temporary hero dependency first, then archive only the hero after regression checks. Preserve the logo and current source art.

This was the scheduled read-only audit, not the cleanup implementation. No production source, asset, prefab, importer, settings, or APK changed.

## Inspected assets

Both actual PNGs were inspected visually, their headers/import settings read, and their Unity imports loaded. Both are RGBA PNGs imported as RGBA32 with 11 mip levels and the existing Android 4096 override. Neither has another byte-identical PNG among the task-start project inputs.

| Candidate under HomeAligned | Source dimensions | Source bytes | R9 packed object bytes | Serialized consumers |
| --- | --- | ---: | ---: | --- |
| home_logo.png | 1511×1041 | 2,479,175 | 8,385,016 | Login.prefab |
| home_hero_firebird.png | 1254×1254 | 2,558,162 | 8,384,680 | None |

Each candidate has three rows in the R9 packed report. These are reported object bytes, not predicted compressed APK or memory savings. Full hashes, GUIDs, import metadata and packed rows: [asset inventory](QA/Assets.json).

## Source and dependency trace

Scanned **5,620 text files** under Assets, Packages and ProjectSettings, recording [21 name/GUID matches](QA/References.json) and [loader sites](QA/LoaderSites.json). Name matches also include historical `home_logo_scene.png` resources, which are distinct assets. Only the exact candidate logo GUID occurs in a serialized consumer: the active Login prefab. The hero GUID occurs only in its own metadata. Unity independently checked **99 serialized roots recursively for each candidate**, confirming Login as the only logo consumer and no hero consumers.

`ProductionHomeRuntime.TextureNames` includes both candidates. `LoadSprites` requires a non-null sprite for every entry. Fresh `BuildInto` and legacy `UpgradeReferenceForeground` load all 24 legacy sprites, initially assign these two, then call `ProductionHomeSourceComposition.Apply`. That owner replaces them with `HomeReferenceAligned/source_logo` and `source_hero`. Completed Home retains neither candidate on any Image. Current migration bypasses the legacy loader; installed Home's valid builder path returns without requesting sprites.

`ProductionHomeView.HasRequiredBindings` validates the current composition. Its separate old `HasReferenceForegroundBindings` property still names both candidates; the full C# scan found only its definition, no caller. A future cleanup must account for this obsolete diagnostic without weakening current binding checks.

Normal loading follows `HomeLoadingPreparation` → `ProductionUiFix01Runtime.GetHomeTextureResourceKeys` → `ProductionHomeRuntime.GetTextureResourceKeys`. This list contains current source composition and shortcut resources, excluding both legacy candidates. The serialized Home/Splash/Loading dependency graphs also exclude them. Editor Home import/validation uses the same current key list, while fresh/legacy prefab construction still delegates to the runtime owner and can hit the legacy requirement.

`ProductionLoginRuntime.BuildInto` directly loads `ProductionUI/HomeAligned/home_logo` into `ApprovedLogo`; its art helper throws if the sprite is missing. The active Login prefab serializes that same GUID. Unity verifies fresh and installed Login preserve that image enabled, active, and fully opaque, with valid page bindings. This is a structural Editor observation, not a new rendered Login acceptance.

Reviewed shared Home loaders and arguments in Gameplay, Victory, Shop, DailyReward, SpecialOffer and NavigationStyle; none requests either candidate. Gameplay's bulk Sprite loader is scoped to its own sheet resources. The Home importer path rule controls import settings and does not establish a runtime consumer.

## Unity verification

Unity **6000.5.9f1 (b57deb96f08d)**, isolated batch/nographics Edit Mode. Temporary fixture installed only for this run, removed with its metadata afterward. All construction/migration probes used disposable instances; no prefab or scene save.

**13 passed, zero failed/skipped.** [Run summary](QA/EditorAudit/summary.json) · [NUnit results](QA/EditorAudit/results.xml).

| Cases | Count | Outcome |
| --- | ---: | --- |
| Candidate import and recursive dependencies | 2 | Logo: Login only; hero: no serialized consumers |
| Home fresh / legacy / current migration | 3 | Valid current art; legacy delegate requests 24 / 24 / 0 |
| Deny one candidate in fresh / legacy loader | 4 | Expected missing-sprite exception for the denied key |
| Installed Home build | 1 | No legacy loader call, valid bindings |
| Fresh / installed Login | 2 | Candidate logo retained enabled/active at alpha 1 |
| Home / Splash / Loading dependency exclusion | 1 | Neither candidate in installed graphs |

The four missing-resource cases pass by asserting the existing failure. They do **not** prove removal is safe or represent four unexpected test failures. No production repair was made. Evidence records include the exact requested keys and exception messages.

## Scope and limits

[Scope verification](QA/Scope.json): **all 7,782 task-start Assets/Packages/ProjectSettings files unchanged; zero changed, added or deleted.** The temporary fixture/meta and any newly generated test scene were removed. Added only audit sources/evidence and documentation. Raw logs are in `/tmp/lor-home-constructor-audit-r10/`; generated caches are excluded from this evidence.

R9 APK remains **830,122,766 bytes**, SHA256 `6d0e866931cf46cd5f2386b9ba0c6a67887bf92a0fa05c4d6ba92e156adb934c`. No new APK or size saving. No Play Mode, rendered comparison, physical-device execution, memory/performance measurement, commit or deployment. Prior R9 runtime/build results remain historical. Python syntax, evidence links and documentation whitespace checks passed.

## Next implementation

Remove only the temporary home_hero_firebird dependency from Home fresh construction and legacy migration, reconcile the obsolete foreground validation, and preserve current source_hero artwork/object bindings. Add missing-key regressions, verify Home/Splash/Loading and retained Login logo, then archive only the hero PNG/meta with exact GUID/bytes/import preservation and measure guarded APK membership/size. Keep home_logo in Resources because Login actively uses it. No coin downsizing or blanket compression. Phone execution/hosting remain deferred; final effects last. Human Chapter1 pacing, font provenance and Android lifecycle/performance/readability remain open.
