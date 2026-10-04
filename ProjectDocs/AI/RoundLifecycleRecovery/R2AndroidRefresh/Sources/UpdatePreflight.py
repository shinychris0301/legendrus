"""Pin the inspected round lifecycle APK and require each compiled transaction owner."""
from pathlib import Path
import ast,json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
a=json.loads((QA/'Artifact.json').read_text());assert json.loads((QA/'BuildValidation.json').read_text())['result']=='Passed'
policy=json.loads((BASE/'Sources/RoundNativePolicy.json').read_text());owners=tuple(policy['extraOwners']);apis=tuple(policy['atomicApis']);flags={k:tuple(v) for k,v in policy['flags'].items()}
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace('ProjectDocs/AI/SelectionRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/RoundLifecycleRecovery/R2AndroidRefresh/QA').replace('766023250',str(a['bytes'])).replace('0c7c454dc83154e622fff646ac865a2c03fee5b5852ab5f46be24703bf48fb7a',a['sha256'])
s=s.replace('ATOMIC_SPENDS = ', 'NATIVE_METHODS += '+repr(owners)+'\nATOMIC_SPENDS = ',1)
s=s.replace('PACKAGE = ', 'ATOMIC_ROUND_APIS = '+repr(apis)+'\nROUND_FLAGS = '+repr(flags)+'\nPACKAGE = ',1)
s=s.replace("final.get('nativeMethodCount') == 49", "final.get('nativeMethodCount') == 61\n            and final.get('atomicRoundLifecycleRecoveryPackaged') is True\n            and native.get('atomicRoundLifecycleRecoveryPackaged') is True")
needle="            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')"
s=s.replace(needle,"""            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicRoundCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False for owner in ATOMIC_ROUND_APIS)
            and all(native.get('methods', {}).get(owner, {}).get(flag) is True for owner,flags in ROUND_FLAGS.items() for flag in flags)
"""+needle,1)
s=s.replace('                      atomicSelectionNormalizationRecoveryPackaged=bool(matches),','                      atomicRoundLifecycleRecoveryPackaged=bool(matches),\n                      atomicSelectionNormalizationRecoveryPackaged=bool(matches),',1)
ast.parse(s);p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace("'nativeMethodCount': 49", "'nativeMethodCount': 61,'atomicRoundLifecycleRecoveryPackaged': True",1)
s=s.replace("'stage': 'After','atomicSelection", "'stage': 'After','atomicRoundLifecycleRecoveryPackaged': True,'atomicSelection",1)
s=s.replace("                    'hasWideCapMath':", "                    'callsAtomicRoundCommit': owner in tool.ATOMIC_ROUND_APIS,\n                    **{flag:True for flag in tool.ROUND_FLAGS.get(owner,())},\n                    'hasWideCapMath':",1)
# A shared-clock flag for TryStartLevel must override the narrower historical clock owner list.
s=s.replace("'callsSharedEnergyClock': owner in tool.SHARED_ENERGY_CLOCK,", "'callsSharedEnergyClock': owner in tool.SHARED_ENERGY_CLOCK or owner in ('GameState_TryStartLevel',),",1)
s=s.replace('    mutations = [',"""    mutations = [
        ('unverified round lifecycle build proof','BuildValidation.json','atomicRoundLifecycleRecoveryPackaged',False),
        ('unverified round lifecycle native proof','AfterNative.json','atomicRoundLifecycleRecoveryPackaged',False),""",1)
mutations=[(o,'callsAtomicRoundCommit',False) for o in apis]+[(o,'callsPersist',True) for o in apis]+[(o,k,False) for o,keys in flags.items() for k in keys]+[('GameState_TryStartLevel','packagedMethodBytesMatchCompiledOutput',False)]
needle="            ('GameState_CompleteTutorial', 'callsAtomicBalanceCommit', False),";assert needle in s
s=s.replace(needle,''.join('            '+repr(r)+',\n' for r in mutations)+needle,1)
ast.parse(s);p.write_text(s)
print('Current offline preflight now requires61owners, five retained callbacks and combined round/failure recovery.')
