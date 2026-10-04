from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text();marker='        void SnapshotAndClearSaveJournal()'
assert 'EnergyGrantLargestAmountRespectsExistingCap' not in s
new='''        [TestCase(1)] [TestCase(3)] [TestCase(5)]
        public void EnergyGrantLargestAmountRespectsExistingCap(int energy)
        {
            var state = GameState.Instance;
            state.Save.energy = energy; state.Persist();
            state.AddEnergy(int.MaxValue);
            Assert.That(state.Save.energy, Is.EqualTo(state.Save.energyMax));
            Assert.That(SaveService.Load().energy, Is.EqualTo(state.Save.energyMax));
        }

        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)]
        public void EnergyMutationBlockedSavePreservesBalanceClockAndEvents(int operation)
        {
            var state = GameState.Instance;
            state.Save.energy = operation == 3 ? state.Save.energyMax : 1;
            state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-61).Ticks; state.Persist();
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var present = new bool[PersistedSaveKeys.Length]; var journal = new string[present.Length];
            for (int i = 0; i < journal.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignored = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                if (operation == 3) Assert.That(state.SpendGold(1), Is.False);
                if (operation == 0) state.AddEnergy(2);
                else if (operation == 1) state.RefillEnergy();
                else state.RefreshEnergyFromClock();
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before), "Energy and its clock cannot publish before saving.");
                Assert.That(state.Save, Is.SameAs(original)); Assert.That(events, Is.Zero);
                Assert.That(state.CanPersist, Is.False); var failedWrite = state.LastWriteResult;
                Assert.That(failedWrite.Status, Is.EqualTo(PlayerSaveWriteStatus.WriteBlocked));
                state.AddEnergy(1); state.RefillEnergy(); state.RefreshEnergyFromClock();
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(events, Is.Zero);
                Assert.That(state.LastWriteResult, Is.SameAs(failedWrite));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]) || PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                LogAssert.ignoreFailingMessages = ignored; state.Changed -= changed;
                for (int i = 0; i < journal.Length; i++)
                { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); }
                PlayerPrefs.Save();
            }
        }

        [TestCase(false)] [TestCase(true)]
        public void EnergyClockLargestCapCannotWrapDuringRefreshOrSpending(bool spend)
        {
            var state = GameState.Instance;
            state.Save.energyMax = int.MaxValue; state.Save.energy = int.MaxValue - 1;
            state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-61).Ticks; state.Persist();
            if (spend)
            {
                Assert.That(state.SpendEnergy(), Is.True);
                Assert.That(state.Save.energy, Is.EqualTo(int.MaxValue - 1));
            }
            else
            {
                state.RefreshEnergyFromClock();
                Assert.That(state.Save.energy, Is.EqualTo(int.MaxValue));
            }
            Assert.That(SaveService.Load().energy, Is.EqualTo(state.Save.energy));
        }

'''
assert marker in s;p.write_text(s.replace(marker,new+marker))
