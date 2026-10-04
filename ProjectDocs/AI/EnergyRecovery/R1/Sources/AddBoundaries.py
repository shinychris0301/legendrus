from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text()
old='''                state.AddEnergy(1); state.RefillEnergy(); state.RefreshEnergyFromClock();
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));'''
new='''                state.AddEnergy(1); state.RefillEnergy(); state.RefreshEnergyFromClock();
                Assert.That(state.TryAddEnergy(1) || state.TryRefillEnergy() || state.TryRefreshEnergyFromClock(), Is.False);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));'''
assert old in s;s=s.replace(old,new)
marker='        void SnapshotAndClearSaveJournal()'
new='''        [TestCase(0)] [TestCase(1)] [TestCase(2)]
        public void EnergyTryApisCommitOnceAndPreserveUnrelatedSave(int operation)
        {
            var state = GameState.Instance;
            state.Save.energy = 1; state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-61).Ticks; state.Persist();
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            long before = System.DateTime.UtcNow.Ticks;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                bool committed = operation == 0 ? state.TryAddEnergy(2) : operation == 1 ? state.TryRefillEnergy() : state.TryRefreshEnergyFromClock();
                Assert.That(committed, Is.True); Assert.That(events, Is.EqualTo(1));
                expected.energy = operation == 1 ? expected.energyMax : 3;
                if (operation == 1)
                {
                    Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(before, System.DateTime.UtcNow.Ticks));
                    expected.lastEnergyUtcTicks = state.Save.lastEnergyUtcTicks;
                }
                else if (operation == 2) expected.lastEnergyUtcTicks += System.TimeSpan.FromMinutes(60).Ticks;
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
            }
            finally { state.Changed -= changed; }
        }

        [TestCase(-10, 1)] [TestCase(29, 1)] [TestCase(30, 2)] [TestCase(61, 3)] [TestCase(1000, 5)]
        public void EnergyClockBoundariesPreserveRemainderAndReload(int minutes, int expectedEnergy)
        {
            var state = GameState.Instance;
            state.Save.energy = 1; state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddMinutes(-minutes).Ticks; state.Persist();
            var original = state.Save; long oldTicks = original.lastEnergyUtcTicks;
            var priorWrite = state.LastWriteResult; long before = System.DateTime.UtcNow.Ticks;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(state.TryRefreshEnergyFromClock(), Is.True);
                Assert.That(state.Save.energy, Is.EqualTo(expectedEnergy));
                Assert.That(events, Is.EqualTo(expectedEnergy > 1 ? 1 : 0));
                if (expectedEnergy == 1)
                { Assert.That(state.Save, Is.SameAs(original)); Assert.That(state.LastWriteResult, Is.SameAs(priorWrite)); }
                if (expectedEnergy == state.Save.energyMax)
                    Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(before, System.DateTime.UtcNow.Ticks));
                else Assert.That(state.Save.lastEnergyUtcTicks, Is.EqualTo(oldTicks + (expectedEnergy - 1) * System.TimeSpan.FromMinutes(GameState.EnergyRefillMinutes).Ticks));
                var committed = state.LastWriteResult; long settledTicks = state.Save.lastEnergyUtcTicks;
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(state.Save)));
                Assert.That(state.TryRefreshEnergyFromClock(), Is.True);
                Assert.That(state.Save.lastEnergyUtcTicks, Is.EqualTo(settledTicks));
                Assert.That(state.LastWriteResult, Is.SameAs(committed));
                Assert.That(events, Is.EqualTo(expectedEnergy > 1 ? 1 : 0));
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void EnergyAtFullDoesNotRewriteClockAndSpendingStartsRealCountdown()
        {
            var state = GameState.Instance;
            state.Save.lastEnergyUtcTicks = System.DateTime.UtcNow.AddHours(-2).Ticks; state.Persist();
            var original = state.Save; string before = JsonUtility.ToJson(original); var write = state.LastWriteResult;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(state.TryRefreshEnergyFromClock(), Is.True); state.RefreshEnergyFromClock();
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.Save, Is.SameAs(original));
                Assert.That(state.LastWriteResult, Is.SameAs(write)); Assert.That(events, Is.Zero);
                Assert.That(state.EnergyRemaining(), Is.EqualTo(System.TimeSpan.Zero));
                long start = System.DateTime.UtcNow.Ticks; Assert.That(state.SpendEnergy(), Is.True);
                Assert.That(state.Save.energy, Is.EqualTo(4)); Assert.That(events, Is.EqualTo(1));
                Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(start, System.DateTime.UtcNow.Ticks));
                Assert.That(state.EnergyRemaining().TotalMinutes, Is.InRange(29.9, 30.0));
                Assert.That(SaveService.Load().lastEnergyUtcTicks, Is.EqualTo(state.Save.lastEnergyUtcTicks));
            }
            finally { state.Changed -= changed; }
        }

        [TestCase(int.MinValue)] [TestCase(-1)] [TestCase(0)] [TestCase(int.MaxValue)]
        public void EnergyGrantNonpositiveAndLargestAmountsRetainCapBehavior(int amount)
        {
            var state = GameState.Instance; Assert.That(state.SpendEnergy(2), Is.True);
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            long before = System.DateTime.UtcNow.Ticks;
            Assert.That(state.TryAddEnergy(amount), Is.True);
            if (amount > 0)
            {
                expected.energy = expected.energyMax;
                Assert.That(state.Save.lastEnergyUtcTicks, Is.InRange(before, System.DateTime.UtcNow.Ticks));
                expected.lastEnergyUtcTicks = state.Save.lastEnergyUtcTicks;
            }
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
            Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
        }

'''
assert 'EnergyTryApisCommitOnceAndPreserveUnrelatedSave' not in s;p.write_text(s.replace(marker,new+marker))
print('Added13acknowledgement/clock/cap boundaries; existing rejected-save tests now check all Try APIs.')
