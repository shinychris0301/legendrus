from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;p=BASE/'Sources/CaptureNative.py';s=p.read_text()
needle="'ScreenRouter_CommitGameplayVictory']";extra=['GameState_RefreshEnergyFromClock','GameState_AddEnergy','GameState_RefillEnergy','GameState_EnergyRemaining','GameState_TryRefreshEnergyFromClock','GameState_TryAddEnergy','GameState_TryRefillEnergy','GameState_AdvanceEnergyClock']
assert needle in s;s=s.replace(needle,"'ScreenRouter_CommitGameplayVictory',"+','.join(repr(n) for n in extra)+']')
a=s.index("else:assert {'GameState_SpendGold'");b=s.index("local=ROOT/",a)
s=s[:a]+"else:assert set(selected)==set(targets)-{'GameState_TryRefreshEnergyFromClock','GameState_TryAddEnergy','GameState_TryRefillEnergy','GameState_AdvanceEnergyClock'}\n"+s[b:]
s=s.replace("'callsAtomicBalanceCommit':'GameState_TryCommitBalance_' in body", "'callsAtomicBalanceCommit':'GameState_TryCommitBalance_' in body,'callsSharedEnergyClock':'GameState_AdvanceEnergyClock_' in body,'hasWideCapMath':'int64_t' in body and 'Math_Min_' in body,'callsTryEnergyApi':any(n in body for n in ('GameState_TryAddEnergy_','GameState_TryRefillEnergy_','GameState_TryRefreshEnergyFromClock_'))")
s=s.replace("('GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster'):","('GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster','GameState_TryAddEnergy','GameState_TryRefillEnergy','GameState_TryRefreshEnergyFromClock'):")
a=s.index(" assert {'GameState_SpendGold'",s.index(" changed=[owner"));b=s.index(" record['changedNativeMethodsFromBaseline']",a)
s=s[:a]+''' assert {'GameState_SpendEnergy','GameState_AddEnergy','GameState_RefillEnergy','GameState_RefreshEnergyFromClock'}<=set(changed)
 for owner in ('GameState_SpendEnergy','GameState_TryRefreshEnergyFromClock'):
  assert record['methods'][owner]['callsSharedEnergyClock']
 for owner in ('GameState_TryAddEnergy','GameState_AdvanceEnergyClock'):
  assert record['methods'][owner]['hasWideCapMath']
 for owner in ('GameState_AddEnergy','GameState_RefillEnergy','GameState_RefreshEnergyFromClock'):
  assert record['methods'][owner]['callsTryEnergyApi'] and not record['methods'][owner]['callsPersist']
 for name in ('TryAddEnergy','TryRefillEnergy','TryRefreshEnergyFromClock','AdvanceEnergyClock'):
  assert (name+'\\0').encode() in metadata,name
 record['cappedAtomicEnergyAndClockRecoveryPackaged']=True
''' +s[b:];p.write_text(s)
print('Native audit expanded:26previous owners/30new owners, atomic energy, widened cap math and shared clock required.')
