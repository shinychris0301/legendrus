# Initial Edit Mode failures

Unmodified baseline: 1,890 passed, 11 failed; no skips. Full messages remain in results.xml.

- `LegendsOfRus.Tests.ProductionCoreLoopUi.ProductionUiFix01DeliveryEditModeTests.ExactlyTenSerializedPageFilesAreShipped`: Expected: 10
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ProductionUiFix01DeliveryEditModeTests.GameplayLiveBoardBoundaryIsStillPresent`: Expected: String containing "SetPixelRect(rt, 79, 357, 786, 942)"
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ReferenceMatchCoreLoopUiEditModeTests.GeneratedReferenceMatchPrefabIsProductionOnlyWhenBuildHasRun(Home,6)`: Expected: 6
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ReferenceMatchCoreLoopUiEditModeTests.GeneratedReferenceMatchPrefabIsProductionOnlyWhenBuildHasRun(WorldMap,4)`: Expected: 4
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ReferenceMatchCoreLoopUiEditModeTests.GeneratedReferenceMatchPrefabIsProductionOnlyWhenBuildHasRun(ChapterMap,4)`: Expected: 4
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ReferenceMatchR3CoreLoopUiEditModeTests.CorrectedInteractionHotspotsMatchR3Geometry`: Expected: String containing "Hotspot(parent, "Boosters", 110, 850, 720, 285"
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ReferenceMatchR3CoreLoopUiEditModeTests.GameplayStillUsesLiveMatch3BoardOverlay`: Expected: String containing "SetPixelRect(rt, 79, 357, 786, 942);"
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ReferenceMatchR3CoreLoopUiEditModeTests.GeneratedR3PrefabUsesExpectedLiveRuntimeBoundary(PreLevel,12,False,False)`: Expected: 12
- `LegendsOfRus.Tests.ProductionCoreLoopUi.ReferenceMatchR3CoreLoopUiEditModeTests.GeneratedR3PrefabUsesExpectedLiveRuntimeBoundary(Gameplay,5,True,False)`: Expected: 5
- `LegendsOfRus.Tests.ProductionCoreLoopUi.SharedBoosterArtEditModeTests.SharedAssetsAreUncompressedAndRuntimeMappingsAgree`: Expected: same as <mace (UnityEngine.Sprite)>
- `LegendsOfRus.Tests.SaveRecovery.SaveJournalLoadEditModeTests.EqualRevisionReplacementStaging_DoesNotBlockReplacedBackup`: Expected: True

Inspection: historical UI geometry/component counts, expanded delivery directory and approved framed Inventory icon make ten assertions obsolete. One recovery fixture used Ordinary intent for the backup despite replacement writes preserving an identical replacement envelope. Correct the fixture while retaining rejection of mismatched intent/content; do not loosen production recovery safeguards.
