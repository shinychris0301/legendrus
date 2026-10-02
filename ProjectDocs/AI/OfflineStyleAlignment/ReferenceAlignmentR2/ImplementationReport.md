# Offline / No Connection — Reference Alignment R2

The real original is now used for the panel, crossed-plug emblem, emerald Retry and blue Home buttons. Four Scenario alpha-isolation jobs cost **12CU**, with no RGB regeneration or paid resubmission. One CDN download failed and the same completed output was recovered. Source, prepared inputs, masks, request/job/billing records and hashes are retained.

The native page follows the original 941×1672 panel/control positions. The existing approved player/resource header displays saved balances and routes to Profile, Settings, Shop currencies and Out of Energy. Localized text remains live. Original panel faces were cleared with source navy texture and feathered repairs; dividers retained. The medallion retains its opaque face and locally recovered blue halo. Runtime dependencies are separate sprites, never the full original.

## Verification

- Unity 6000.5.9f1 compilation/import, native prefab rebuild, serialized readback and revision2 cutover passed.
- Final Verify03: **5 Edit Mode + 9 Play Mode = 14 tests passed**, zero failures/skips. Seven Offline checks and seven Tutorial regressions.
- Six actual Offline captures: RU941×1672, EN576×1280 safe area and RU1080×2400 safe area; offline, retry and detected-network states. Nine Tutorial regression captures.
- Verified Retry/probe counts, focus/network changes, RU/EN refresh, Home/system Back, rebinding/repeated input guards, saved balances, all header routes, safe-area input targets and rendered text, unchanged save/progress.
- Browser390px: all7views loaded without errors, missing links or horizontal overflow; source-script syntax and scoped whitespace checks passed.
- Alpha inspected over black, white and magenta; original RGB outside recorded repair masks and immutable submitted inputs verified. Existing asset GUIDs/metas, other pages, packages/settings and Android guards unchanged against the turn-start snapshot.
- Verify01 caught missing header font assignments (8/9 Play checks passed); fixed locally. Verify02 passed all14 checks. Final title size and divider cleanup were rerun in Verify03.

Not performed: APK build, physical phone/touch/networking, actual airplane-mode/captive-portal checks, backend connectivity/health requests, full-project tests or profiling.

## Deliberate differences and remaining limits

The approved clean forest scenery and shared header/portrait are reused; they do not reproduce the reference city backdrop or exact portrait. The selected embedded serif fonts approximate the original and remain unverified as exact. Source panel text-removal texture and recovered halo are reconstructed locally. Text counters show saved data rather than reference sample values.

The source email and green server indicator are unverified. Footer instead says support unavailable and server status unknown. Retry checks device network availability only; detected network explicitly does not establish game-service access. No global offline interceptor or health endpoint was added. Backend integration and configured support details remain future work. No extra Close button, matching the reference's Home action.

## Changed production files

- `Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/OfflineAlignmentEditModeTests.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/OfflineAlignmentPlayModeTests.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionOfflinePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionOfflineView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionOfflineRuntime.cs`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/OfflineState.prefab`
- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/OfflineStateCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/panel.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/emblem.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/retry.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/emblem.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/home.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/panel.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/retry.png.meta`
- `Assets/LegendsOfRus/Resources/ProductionUI/OfflineAligned/home.png`
