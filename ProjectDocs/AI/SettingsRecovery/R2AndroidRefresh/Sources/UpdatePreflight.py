"""Pin the verified Settings APK and require its actual transaction/callback inclusion proof."""
from pathlib import Path
import ast,json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
a=json.loads((QA/'Artifact.json').read_text());assert json.loads((QA/'BuildValidation.json').read_text())['result']=='Passed'
policy=json.loads((BASE/'Sources/SettingsNativePolicy.json').read_text());owners=tuple(policy['extraOwners']);flags={k:tuple(v) for k,v in policy['flags'].items()}
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace('ProjectDocs/AI/RoundLifecycleRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/SettingsRecovery/R2AndroidRefresh/QA').replace('766031502',str(a['bytes'])).replace('71e042f7eaef9edba33c16916c843aa28737b5432666f166aec1a59a0b3b3984',a['sha256'])
s=s.replace('ATOMIC_SPENDS = ', 'NATIVE_METHODS += '+repr(owners)+'\nSETTINGS_FLAGS = '+repr(flags)+'\nATOMIC_SPENDS = ',1)
s=s.replace("final.get('nativeMethodCount') == 61", "final.get('nativeMethodCount') == 68\n            and final.get('atomicSettingsPreferenceRecoveryPackaged') is True\n            and native.get('atomicSettingsPreferenceRecoveryPackaged') is True")
needle="            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')";assert needle in s
s=s.replace(needle,"""            and all(native.get('methods', {}).get(owner, {}).get(flag) is True for owner,flags in SETTINGS_FLAGS.items() for flag in flags)
            and native.get('methods', {}).get('GameState_TryTogglePreference', {}).get('callsPersist') is False
            and set(native.get('settingsCallbacks', {})) == {'OwnedSettingsListener'}
            and all(row.get('packagedMethodBytesMatchCompiledOutput') is True
                    and row.get('rejectsRetiredBindingBeforeInputAndAction') is True
                    and row.get('checksButtonAndInfoShieldBeforeAction') is True
                    for row in native.get('settingsCallbacks', {}).values())
            and packed.get('settingsRevision') == 5
"""+needle,1)
s=s.replace('                      atomicRoundLifecycleRecoveryPackaged=bool(matches),','                      atomicSettingsPreferenceRecoveryPackaged=bool(matches),\n                      atomicRoundLifecycleRecoveryPackaged=bool(matches),',1);ast.parse(s);p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace("'nativeMethodCount': 61", "'nativeMethodCount': 68,'atomicSettingsPreferenceRecoveryPackaged': True",1)
s=s.replace("'stage': 'After','atomicRound", "'stage': 'After','atomicSettingsPreferenceRecoveryPackaged': True,'settingsCallbacks':{'OwnedSettingsListener':{'packagedMethodBytesMatchCompiledOutput':True,'rejectsRetiredBindingBeforeInputAndAction':True,'checksButtonAndInfoShieldBeforeAction':True}},'atomicRound",1)
s=s.replace("                    'hasWideCapMath':", "                    **{flag:True for flag in tool.SETTINGS_FLAGS.get(owner,())},\n                    'hasWideCapMath':",1)
s=s.replace("'PackedAssets.json': {'result': 'Succeeded'", "'PackedAssets.json': {'settingsRevision':5,'result': 'Succeeded'",1)
s=s.replace('    mutations = [',"""    mutations = [
        ('unverified settings build proof','BuildValidation.json','atomicSettingsPreferenceRecoveryPackaged',False),
        ('unverified settings native proof','AfterNative.json','atomicSettingsPreferenceRecoveryPackaged',False),
        ('missing owned settings callback proof','AfterNative.json','settingsCallbacks',{}),
        ('rejected owned settings callback native bytes','AfterNative.json','settingsCallbacks',{'OwnedSettingsListener':{'packagedMethodBytesMatchCompiledOutput':False,'rejectsRetiredBindingBeforeInputAndAction':True,'checksButtonAndInfoShieldBeforeAction':True}}),
        ('rejected settings callback retired-binding guard','AfterNative.json','settingsCallbacks',{'OwnedSettingsListener':{'packagedMethodBytesMatchCompiledOutput':True,'rejectsRetiredBindingBeforeInputAndAction':False,'checksButtonAndInfoShieldBeforeAction':True}}),
        ('rejected settings callback info/input guard','AfterNative.json','settingsCallbacks',{'OwnedSettingsListener':{'packagedMethodBytesMatchCompiledOutput':True,'rejectsRetiredBindingBeforeInputAndAction':True,'checksButtonAndInfoShieldBeforeAction':False}}),
        ('wrong settings prefab revision','PackedAssets.json','settingsRevision',4),""",1)
mutations=[(o,k,False) for o,keys in flags.items() for k in keys]+[('GameState_TryTogglePreference','callsPersist',True)]+[(o,'packagedMethodBytesMatchCompiledOutput',False) for o in owners]
needle="            ('GameState_CompleteTutorial', 'callsAtomicBalanceCommit', False),";assert needle in s
s=s.replace(needle,''.join('            '+repr(r)+',\n' for r in mutations)+needle,1)
ast.parse(s);p.write_text(s)
print('Pinned canonical offline preflight to Settings source/compiled owners and owned callback evidence.')
