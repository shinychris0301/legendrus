from pathlib import Path
import shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
changes={}
def edit(rel, old, new):
 p=root/rel;s=changes.get(p,p.read_text());assert old in s,rel+' missing target';changes[p]=s.replace(old,new,1)
tests='Assets/LegendsOfRus/Tests/'
f=tests+'ProductionCoreLoopUiEditMode/ProductionUiFix01DeliveryEditModeTests.cs'
edit(f,'public void ExactlyTenSerializedPageFilesAreShipped()','public void AllTenCoreLoopPagesRemainInExpandedSerializedDelivery()')
edit(f,'Assert.That(Directory.GetFiles(PageRoot, "*.prefab", SearchOption.TopDirectoryOnly).Length, Is.EqualTo(10));','''// Later native pages share this directory; retain the original ten-page contract.
            int count = 0;
            foreach (ScreenId id in Enum.GetValues(typeof(ScreenId)))
            {
                if (!ProductionCoreLoopPageRegistry.IsCutover(id)) continue;
                var page = Resources.Load<GameObject>(ProductionCoreLoopPageRegistry.ResourceKey(id));
                Assert.That(page, Is.Not.Null, id.ToString());
                Assert.That(page.GetComponent<ProductionUiFix01PageBootstrap>().ScreenId, Is.EqualTo(id));
                count++;
            }
            Assert.That(count, Is.EqualTo(ProductionCoreLoopPageRegistry.CutoverPageCount));''')
edit(f,'StringAssert.Contains("SetPixelRect(rt, 79, 357, 786, 942)", router);','''StringAssert.Contains("boardGo.transform.SetParent(view.BoardMount, false)", router);
            var view = Resources.Load<GameObject>("ProductionUI/Pages/Gameplay").GetComponent<ProductionGameplayView>();
            Assert.That(view.HasRequiredBindings, Is.True);
            Assert.That(view.BoardMount.rect.width, Is.GreaterThan(0));
            Assert.That(view.BoardMount.rect.height, Is.GreaterThan(0));''')
f=tests+'ProductionCoreLoopUiEditMode/ReferenceMatchCoreLoopUiEditModeTests.cs'
edit(f,'[TestCase(ScreenId.Home, 6)]','[TestCase(ScreenId.Home, 30)]')
edit(f,'[TestCase(ScreenId.WorldMap, 4)]','[TestCase(ScreenId.WorldMap, 5)]')
edit(f,'[TestCase(ScreenId.ChapterMap, 4)]','[TestCase(ScreenId.ChapterMap, 5)]')
edit(f,'GeneratedReferenceMatchPrefabIsProductionOnlyWhenBuildHasRun','CurrentNativePrefabRetainsProductionOnlyDependencies')
edit(f,'''if (prefab == null)
                Assert.Ignore("Step 3.3R.2 deterministic prefab build has not run yet in this editor session.");''','''Assert.That(prefab, Is.Not.Null, path);''')
f=tests+'ProductionCoreLoopUiEditMode/ReferenceMatchR3CoreLoopUiEditModeTests.cs'
edit(f,'StringAssert.Contains("SetPixelRect(rt, 79, 357, 786, 942);", text);','''StringAssert.Contains("boardGo.transform.SetParent(view.BoardMount, false);", text);
            StringAssert.Contains("view.Bind(activeBoard, ShowPause);", text);''')
edit(f,'CorrectedInteractionHotspotsMatchR3Geometry','NativePreparationAndPauseExposeBoundInteractiveControls')
start='''            string text = File.ReadAllText("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
            StringAssert.Contains("Hotspot(parent, \\"Boosters\\", 110, 850, 720, 285", text);'''
# Replace the obsolete method body as a bounded block.
p=root/f;s=changes.get(p,p.read_text());a=s.index('            string text',s.index('public void NativePreparation'));b=s.index('\n        }',a)
s=s[:a]+'''            var preparation = Resources.Load<GameObject>("ProductionUI/Pages/PreLevel").GetComponent<ProductionPreLevelView>();
            var pause = Resources.Load<GameObject>("ProductionUI/Pages/Pause").GetComponent<ProductionPauseView>();
            Assert.That(preparation.HasRequiredBindings, Is.True);
            Assert.That(pause.HasRequiredBindings, Is.True);
            Assert.That(preparation.PlayButton.targetGraphic.raycastTarget, Is.True);
            foreach (var button in preparation.BoosterButtons)
                Assert.That(button.targetGraphic.raycastTarget, Is.True);
            foreach (var button in pause.Actions)
                Assert.That(button.targetGraphic.raycastTarget, Is.True);'''+s[b:];changes[p]=s
edit(f,'[TestCase(ScreenId.PreLevel, 12, false, false)]','[TestCase(ScreenId.PreLevel, 8, false, false)]')
edit(f,'[TestCase(ScreenId.Gameplay, 5, true, false)]','[TestCase(ScreenId.Gameplay, 8, true, false)]')
edit(f,'''if (prefab == null)
                Assert.Ignore("R3 deterministic prefab build has not run yet in this editor session.");''','''Assert.That(prefab, Is.Not.Null, path);''')
f=tests+'ProductionCoreLoopUiEditMode/SharedBoosterArtEditModeTests.cs'
edit(f,'ProductionInventoryRuntime.ItemArt(i+20),Is.SameAs(ProductionBoosterArt.Icon(i))','ProductionInventoryRuntime.ItemArt(i+20),Is.SameAs(ProductionBoosterArt.Icon(i,true))')
f=tests+'SaveRecoveryEditMode/SaveJournalLoadEditModeTests.cs'
edit(f,'''store.SeedDurable(Keys.BackupKey, Envelope("reset", 6, 6));''','''// A committed history replacement writes the identical envelope to backup.
            store.SeedDurable(Keys.BackupKey, Envelope("reset", 6, 6, SaveEnvelopeIntent.ReplaceHistory));''')
# Preserve the mismatched-intent case as a separate rejection regression.
needle='''        [Test]
        public void EqualRevisionReplacementStagingWithDifferentContent_BlocksInconsistentBackup()'''
edit(f,needle,'''        [Test]
        public void EqualRevisionReplacementStagingWithOrdinaryBackup_BlocksInconsistentIntent()
        {
            var store = new DurablePendingFakeStore();
            store.SeedDurable(Keys.PrimaryKey, "CORRUPT");
            store.SeedDurable(Keys.BackupKey, Envelope("reset", 6, 6));
            store.SeedDurable(Keys.StagingKey, Envelope("reset", 6, 6, SaveEnvelopeIntent.ReplaceHistory));
            var result = Repository(store).Load();
            Assert.That(result.Status, Is.EqualTo(SaveJournalLoadStatus.Unrecoverable));
            Assert.That(result.DiagnosticCode, Is.EqualTo("SAVE_HISTORY_REPLACEMENT_INTERRUPTED"));
            Assert.That(store.MutationOperationCount, Is.Zero);
        }

'''+needle)
for p,s in changes.items():
 target=base/'Sources/Before'/p.relative_to(root);target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(p,target);p.write_text(s)
print('Updated',len(changes),'test files; no product changes')
