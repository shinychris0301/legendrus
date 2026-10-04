"""Seed isolated clock fixtures without invoking unrelated live HUD event callbacks."""
from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text()
a=s.index('        [TestCase(1)] [TestCase(3)] [TestCase(5)]\n        public void EnergyGrantLargestAmount');b=s.index('        void SnapshotAndClearSaveJournal()',a)
region=s[a:b].replace('state.Persist();','Assert.That(SaveService.TrySave(state.Save, out _), Is.True);')
region=region.replace('state.Save.energy = 1; state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-61).Ticks; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);\n            var expected','state.Save.energy = 1; state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(operation == 2 ? -61 : 0).Ticks; Assert.That(SaveService.TrySave(state.Save, out _), Is.True);\n            var expected')
p.write_text(s[:a]+region+s[b:])
