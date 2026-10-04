"""Use one capped clock candidate for refresh/spending and atomic energy credits/refills."""
from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs';s=p.read_text()
a=s.index('        public void RefreshEnergyFromClock()');b=s.index('        public TimeSpan EnergyRemaining()',a)
s=s[:a]+'''        public void RefreshEnergyFromClock() => TryRefreshEnergyFromClock();

        // A healthy clock with no due refill succeeds without a write or notification.
        public bool TryRefreshEnergyFromClock()
        {
            if (!canPersist) return false;
            if (Save.energy == Save.energyMax) return true;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            if (!AdvanceEnergyClock(candidate, DateTime.UtcNow.Ticks)) return true;
            return TryCommitBalance(candidate);
        }

        static bool AdvanceEnergyClock(PlayerSaveData candidate, long now)
        {
            if (candidate.energy >= candidate.energyMax)
            {
                if (candidate.energy == candidate.energyMax) return false;
                candidate.energy = candidate.energyMax;
                candidate.lastEnergyUtcTicks = now;
                return true;
            }

            long interval = TimeSpan.FromMinutes(EnergyRefillMinutes).Ticks;
            long elapsed = Math.Max(0, now - candidate.lastEnergyUtcTicks);
            long gained = elapsed / interval;
            if (gained <= 0) return false;

            candidate.energy = (int)Math.Min(candidate.energyMax, (long)candidate.energy + gained);
            candidate.lastEnergyUtcTicks = candidate.energy >= candidate.energyMax
                ? now : candidate.lastEnergyUtcTicks + gained * interval;
            return true;
        }

''' +s[b:]
a=s.index('            if (candidate.energy >= candidate.energyMax)',s.index('        public bool SpendEnergy('));b=s.index('            if (candidate.energy < amount)',a)
s=s[:a]+'            AdvanceEnergyClock(candidate, now);\n'+s[b:]
a=s.index('        public void AddEnergy(');b=s.index('        // Compatibility entry points',a)
s=s[:a]+'''        public void AddEnergy(int amount) => TryAddEnergy(amount);

        public bool TryAddEnergy(int amount)
        {
            if (!canPersist) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            // Energy has an existing cap; widen before addition so the grant cannot wrap.
            candidate.energy = (int)Math.Max(0L, Math.Min(candidate.energyMax,
                (long)candidate.energy + Mathf.Max(0, amount)));
            if (candidate.energy >= candidate.energyMax)
                candidate.lastEnergyUtcTicks = DateTime.UtcNow.Ticks;
            return TryCommitBalance(candidate);
        }

        public void RefillEnergy() => TryRefillEnergy();

        public bool TryRefillEnergy()
        {
            if (!canPersist) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            candidate.energy = candidate.energyMax;
            candidate.lastEnergyUtcTicks = DateTime.UtcNow.Ticks;
            return TryCommitBalance(candidate);
        }

''' +s[b:]
s=s.replace('// A committed primary is already a credit even if journal cleanup','// A committed primary is already a balance change even if journal cleanup')
p.write_text(s)
print('Existing energy wrappers retained; three Try APIs use durable candidate commitment and widened cap math.')
