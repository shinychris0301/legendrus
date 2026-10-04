"""Pin offline handoff to the verified persistence APK and require all new compiled proof."""
from pathlib import Path
import ast,json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
a=json.loads((QA/'Artifact.json').read_text()); assert json.loads((QA/'BuildValidation.json').read_text())['result']=='Passed'
policy=json.loads((BASE/'Sources/PersistenceNativePolicy.json').read_text()); owners=tuple(policy['extraOwners']); flags={k:tuple(v) for k,v in policy['flags'].items()}
before=json.loads((QA/'ArtifactBefore.json').read_text())
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace('ProjectDocs/AI/SettingsRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/PersistenceRecovery/R2AndroidRefresh/QA').replace(str(before['bytes']),str(a['bytes'])).replace(before['sha256'],a['sha256'])
needle='ATOMIC_SPENDS = ';assert needle in s
s=s.replace(needle,'NATIVE_METHODS += '+repr(owners)+'\nPERSISTENCE_FLAGS = '+repr(flags)+'\n'+needle,1)
s=s.replace("final.get('nativeMethodCount') == 68","final.get('nativeMethodCount') == 72\n            and final.get('acknowledgedOrdinaryPersistenceRecoveryPackaged') is True\n            and native.get('acknowledgedOrdinaryPersistenceRecoveryPackaged') is True\n            and final.get('persistenceOwnerRegionsVerified') == 4\n            and native.get('persistenceOwnerRegionsVerified') == 4",1)
needle="            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')";assert needle in s
s=s.replace(needle,"            and all(native.get('methods', {}).get(owner, {}).get(flag) is True for owner,flags in PERSISTENCE_FLAGS.items() for flag in flags)\n            and native.get('methods', {}).get('GameState_TryPersist', {}).get('callsAtomicBalanceCommit') is True\n            and native.get('methods', {}).get('GameState_TryPersist', {}).get('callsPersist') is False\n"+needle,1)
s=s.replace('                      atomicSettingsPreferenceRecoveryPackaged=bool(matches),','                      acknowledgedOrdinaryPersistenceRecoveryPackaged=bool(matches),\n                      atomicSettingsPreferenceRecoveryPackaged=bool(matches),',1)
ast.parse(s);p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace("'nativeMethodCount': 68", "'nativeMethodCount': 72,'acknowledgedOrdinaryPersistenceRecoveryPackaged':True,'persistenceOwnerRegionsVerified':4",1)
s=s.replace("'stage': 'After'", "'stage': 'After','acknowledgedOrdinaryPersistenceRecoveryPackaged':True,'persistenceOwnerRegionsVerified':4",1)
s=s.replace("'callsAtomicBalanceCommit': owner in tool.ATOMIC_SPENDS +", "'callsAtomicBalanceCommit': owner in ('GameState_TryPersist',) + tool.ATOMIC_SPENDS +",1)
needle='                    **{flag:True for flag in tool.SETTINGS_FLAGS.get(owner,())},';assert needle in s
s=s.replace(needle,needle+'\n                    **{flag:True for flag in tool.PERSISTENCE_FLAGS.get(owner,())},',1)
mutations=[('unverified persistence build proof','BuildValidation.json','acknowledgedOrdinaryPersistenceRecoveryPackaged',False),('unverified persistence native proof','AfterNative.json','acknowledgedOrdinaryPersistenceRecoveryPackaged',False),('wrong persistence build region count','BuildValidation.json','persistenceOwnerRegionsVerified',3),('wrong persistence native region count','AfterNative.json','persistenceOwnerRegionsVerified',3)]
s=s.replace('    mutations = [','    mutations = [\n'+''.join('        '+repr(r)+',\n' for r in mutations),1)
mutations=[(o,k,False) for o,keys in flags.items() for k in keys]+[(o,'packagedMethodBytesMatchCompiledOutput',False) for o in owners]+[('GameState_TryPersist','callsAtomicBalanceCommit',False),('GameState_TryPersist','callsPersist',True)]
needle="            ('GameState_CompleteTutorial', 'callsAtomicBalanceCommit', False),";assert needle in s
s=s.replace(needle,''.join('            '+repr(r)+',\n' for r in mutations)+needle,1)
ast.parse(s);p.write_text(s)
print('Canonical offline preflight pins new persistence owners, guard/acknowledgement proof and all earlier recovery.')
