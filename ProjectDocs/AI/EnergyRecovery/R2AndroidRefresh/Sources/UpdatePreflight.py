"""Pin energy-refreshed APK and require every old/new owner proof in canonical offline readiness."""
from pathlib import Path
import json,re
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
a=json.loads((QA/'Artifact.json').read_text());native=json.loads((QA/'AfterNative.json').read_text());assert len(native['methods'])==30
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text().replace('ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/EnergyRecovery/R2AndroidRefresh/QA')
s=re.sub(r"PIN = \{'bytes': \d+,\n       'sha256': '[a-f0-9]+'\}",f"PIN = {{'bytes': {a['bytes']},\n       'sha256': '{a['sha256']}'}}",s,count=1)
s=re.sub(r'^NATIVE_METHODS = .+$','NATIVE_METHODS = '+repr(tuple(native['methods'])),s,flags=re.M)
needle="CHECKED_CREDITS = ('GameState_ClaimDailyReward', 'GameState_TryDoubleLastLevelGoldReward', 'GameState_TryCompleteLevel', 'GameState_TryCredit', 'GameState_TrySettleBoosterCommand')"
assert needle in s;s=s.replace(needle,needle+"\nATOMIC_ENERGY_APIS = ('GameState_TryAddEnergy', 'GameState_TryRefillEnergy', 'GameState_TryRefreshEnergyFromClock')\nWIDE_ENERGY_MATH = ('GameState_TryAddEnergy', 'GameState_AdvanceEnergyClock')\nSHARED_ENERGY_CLOCK = ('GameState_SpendEnergy', 'GameState_TryRefreshEnergyFromClock')\nENERGY_WRAPPERS = ('GameState_AddEnergy', 'GameState_RefillEnergy', 'GameState_RefreshEnergyFromClock')")
s=s.replace("and final.get('nativeMethodCount') == 22", "and final.get('nativeMethodCount') == 30\n            and final.get('cappedAtomicEnergyAndClockRecoveryPackaged') is True\n            and native.get('cappedAtomicEnergyAndClockRecoveryPackaged') is True")
needle="            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')"
assert needle in s;s=s.replace(needle,'''            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False
                    for owner in ATOMIC_ENERGY_APIS)
            and all(native.get('methods', {}).get(owner, {}).get('hasWideCapMath') is True
                    for owner in WIDE_ENERGY_MATH)
            and all(native.get('methods', {}).get(owner, {}).get('callsSharedEnergyClock') is True
                    for owner in SHARED_ENERGY_CLOCK)
            and all(native.get('methods', {}).get(owner, {}).get('callsTryEnergyApi') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False
                    for owner in ENERGY_WRAPPERS)
''' +needle)
s=s.replace('accumulatedRewardAndCreditRecoveryPackaged=bool(matches),','accumulatedRewardAndCreditRecoveryPackaged=bool(matches),\n                      cappedAtomicEnergyAndClockRecoveryPackaged=bool(matches),')
p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text().replace("'nativeMethodCount': 22", "'nativeMethodCount': 30, 'cappedAtomicEnergyAndClockRecoveryPackaged': True")
s=s.replace("'AfterNative.json': {'stage': 'After',", "'AfterNative.json': {'stage': 'After', 'cappedAtomicEnergyAndClockRecoveryPackaged': True,")
s=s.replace("'callsAtomicBalanceCommit': owner in tool.ATOMIC_SPENDS, 'callsPersist': False}","'callsAtomicBalanceCommit': owner in tool.ATOMIC_SPENDS + tool.ATOMIC_ENERGY_APIS, 'callsPersist': False,\n                    'hasWideCapMath': owner in tool.WIDE_ENERGY_MATH,\n                    'callsSharedEnergyClock': owner in tool.SHARED_ENERGY_CLOCK,\n                    'callsTryEnergyApi': owner in tool.ENERGY_WRAPPERS}")
s=s.replace("'nativeMethodCount', 21", "'nativeMethodCount', 29")
needle="    mutations = [";assert needle in s;s=s.replace(needle,needle+"\n        ('unverified energy build proof', 'BuildValidation.json', 'cappedAtomicEnergyAndClockRecoveryPackaged', False),\n        ('unverified energy native proof', 'AfterNative.json', 'cappedAtomicEnergyAndClockRecoveryPackaged', False),")
needle="            ('GameState_TryCredit', 'generatedOverflowCatch', False)):";assert needle in s
s=s.replace(needle,"""            ('GameState_TryCredit', 'generatedOverflowCatch', False),
            ('GameState_TryAddEnergy', 'callsAtomicBalanceCommit', False),
            ('GameState_TryRefillEnergy', 'callsPersist', True),
            ('GameState_TryRefreshEnergyFromClock', 'callsAtomicBalanceCommit', False),
            ('GameState_AdvanceEnergyClock', 'hasWideCapMath', False),
            ('GameState_TryAddEnergy', 'hasWideCapMath', False),
            ('GameState_SpendEnergy', 'callsSharedEnergyClock', False),
            ('GameState_TryRefreshEnergyFromClock', 'callsSharedEnergyClock', False),
            ('GameState_AddEnergy', 'callsTryEnergyApi', False),
            ('GameState_RefillEnergy', 'callsPersist', True),
            ('GameState_RefreshEnergyFromClock', 'callsTryEnergyApi', False),
            ('GameState_TryRefillEnergy', 'packagedMethodBytesMatchCompiledOutput', False)):""")
p.write_text(s);print('Canonical readiness now pins new artifact and30native owners, including atomic/widened/shared energy proof.')
