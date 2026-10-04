"""Extend existing build/native audits to the real Settings preference and callback owners."""
from pathlib import Path
import ast,json
BASE=Path(__file__).resolve().parent
owners=['GameState_TryTogglePreference']+['ProductionSettingsView_'+n for n in ('TogglePreference','Unbind','InputReady','Listen','RefreshInfo','Bind')]
flags={'GameState_TryTogglePreference':['validatesPreferenceBeforeClone','respectsPreferenceWriteLock','changesAllFourPreferenceCandidates','callsAtomicBalanceCommit'],
'ProductionSettingsView_TogglePreference':['guardsPreferenceReentry','refreshesAfterAcknowledgement','rejectionUsesExistingInfoPanel'],
'ProductionSettingsView_Unbind':['retiresBindingBeforeRemovingListeners'],
'ProductionSettingsView_InputReady':['blocksPreferenceCommitReentry','preservesModalTransitionGuard'],
'ProductionSettingsView_Listen':['capturesBindingOwner','ownedSettingsCallbackCompiledAndPackaged'],
'ProductionSettingsView_RefreshInfo':['existingLocalizedErrorBranchPackaged'],
'ProductionSettingsView_Bind':['retiresPreviousBindingsBeforeRebinding']}
(BASE/'SettingsNativePolicy.json').write_text(json.dumps({'extraOwners':owners,'flags':flags},indent=2)+'\n')
p=BASE/'CaptureNative.py';s=p.read_text();s=s.replace('pattern=re.compile(', 'targets += '+repr(owners)+'\npattern=re.compile(',1).replace('LevelTutorialPresenter|ProductionChapterMapView)','LevelTutorialPresenter|ProductionChapterMapView|ProductionSettingsView)',1)
a=s.index("else:assert set(selected)==");b=s.index('\nlocal=',a);s=s[:a]+"else:assert set(selected)==set(targets)-{'GameState_TryTogglePreference'}\n"+s[b:]
a=s.index(" assert {'GameState_RegisterLevelAttempt'");b=s.index('\n for owner',a);s=s[:a]+" assert {'ProductionSettingsView_TogglePreference','ProductionSettingsView_Unbind','ProductionSettingsView_InputReady','ProductionSettingsView_Listen','ProductionSettingsView_RefreshInfo'}<=set(changed)"+s[b:]
block=r'''
if stage=='After':
 policy=json.loads((BASE/'Sources/SettingsNativePolicy.json').read_text())
 pref=body('GameState_TryTogglePreference');clone=call(pref,'JsonUtility_FromJson_TisPlayerSaveData')
 flags('GameState_TryTogglePreference',validatesPreferenceBeforeClone=pref.index('ArgumentOutOfRangeException_')<clone,respectsPreferenceWriteLock=pref.index('___canPersist')<clone,changesAllFourPreferenceCandidates=all('___'+n+' =' in pref for n in ('music','sfx','vibration','notifications')))
 ui=body('ProductionSettingsView_TogglePreference');commit=call(ui,'GameState_TryTogglePreference');refresh=call(ui,'ProductionSettingsView_RefreshState');info=call(ui,'ProductionSettingsView_ShowInfo')
 flags('ProductionSettingsView_TogglePreference',guardsPreferenceReentry='ProductionSettingsView_CanAct_' in ui[:commit] and ui.index('___preferenceChangeInProgress = (bool)1;')<commit and '___preferenceChangeInProgress = (bool)0;' in ui[commit:refresh],refreshesAfterAcknowledgement=commit<refresh<info,rejectionUsesExistingInfoPanel='7, NULL);' in ui[info:] and 'ScreenRouter_Show' not in ui)
 unbind=body('ProductionSettingsView_Unbind')
 flags('ProductionSettingsView_Unbind',retiresBindingBeforeRemovingListeners=unbind.index('___bindingOwner =')<call(unbind,'UnityEvent_RemoveListener'))
 ready=body('ProductionSettingsView_InputReady')
 flags('ProductionSettingsView_InputReady',blocksPreferenceCommitReentry='___preferenceChangeInProgress' in ready,preservesModalTransitionGuard=all(t in ready for t in ('ScreenRouter_get_ActiveModal_','ScreenRouter_get_IsModalTransitioning_','ScreenRouter_get_HasActiveModalBlocker_','ScreenRouter_get_IsScreenTransitioning_')))
 listening=body('ProductionSettingsView_Listen')
 flags('ProductionSettingsView_Listen',capturesBindingOwner=listening.index('___bindingOwner')<listening.index('___owner ='))
 binding=body('ProductionSettingsView_Bind')
 flags('ProductionSettingsView_Bind',retiresPreviousBindingsBeforeRebinding=call(binding,'ProductionSettingsView_Unbind')<call(binding,'ProductionSettingsView_Listen'))
 settings_callbacks={}
 for m in all_methods.finditer(source):
  opening=source.index('{',m.start());depth=1;end=opening+1
  while depth:
   if source[end]=='{':depth+=1
   elif source[end]=='}':depth-=1
   end+=1
  cb=source[m.start():end]
  if not all(t in cb for t in ('___owner','___bindingOwner','ProductionSettingsView_InputReady_','___overlay','Action_Invoke_')):continue
  label='OwnedSettingsListener';assert not settings_callbacks
  address,size=symbols[m.group(1)]
  with local.open('rb') as f:native=method(f,load_segments(f),address,size)
  assert size>0 and native==method(pstream,psegments,address,size)
  assert cb.index('___bindingOwner')<call(cb,'ProductionSettingsView_InputReady')<call(cb,'Action_Invoke')
  assert all(t in cb for t in ('Selectable_IsInteractable_','___InfoPanel','GameObject_get_activeSelf_'))
  assert m.group(1) in listening
  (BASE/'Sources'/('After'+label+'.cpp')).write_text(cb+'\n')
  settings_callbacks[label]={'symbol':m.group(1),'nativeMethodBytes':size,'nativeMethodSha256':hashlib.sha256(native).hexdigest(),'generatedMethodSha256':hashlib.sha256(cb.encode()).hexdigest(),'packagedMethodBytesMatchCompiledOutput':True,'rejectsRetiredBindingBeforeInputAndAction':True,'checksButtonAndInfoShieldBeforeAction':True}
 assert set(settings_callbacks)=={'OwnedSettingsListener'}
 record['settingsCallbacks']=settings_callbacks
 flags('ProductionSettingsView_Listen',ownedSettingsCallbackCompiledAndPackaged=True)
 info=body('ProductionSettingsView_RefreshInfo')
 flags('ProductionSettingsView_RefreshInfo',existingLocalizedErrorBranchPackaged='___infoKind' in info and '7)' in info and all(t in info for t in ('___LanguageChoices','___GeneralInfoContent','___InfoHeading','___InfoDescription','___CloseInfoLabel')))
 # Decode actual Unity metadata strings; do not accept method/type names alone as content proof.
 offset,size,count,literal_offset,literal_bytes,literal_count=struct.unpack_from('<6I',metadata,8);indices=struct.unpack_from('<'+str(count)+'I',metadata,offset)
 assert indices[0]==0 and indices[-1]==literal_bytes
 literals={metadata[literal_offset+a:literal_offset+b].decode('utf-8',errors='replace') for a,b in zip(indices,indices[1:])}
 assert all(t in literals for t in ('AN ERROR OCCURRED','ПРОИЗОШЛА ОШИБКА','Action could not be completed.\nClose this message and try again.','Действие не выполнено.\nЗакройте окно и попробуйте снова.'))
 assert all((n+'\0').encode() in metadata for n in ('TryTogglePreference','bindingOwner','preferenceChangeInProgress'))
 for owner,keys in policy['flags'].items():assert all(record['methods'][owner].get(k) is True for k in keys),(owner,record['methods'][owner])
 assert not record['methods']['GameState_TryTogglePreference']['callsPersist']
 record['atomicSettingsPreferenceRecoveryPackaged']=True
 record['settingsOwnerRegionsVerified']=len(policy['extraOwners'])
'''
needle="(QA/(stage+'Native.json')).write_text";assert needle in s;s=s.replace(needle,block+'\n'+needle,1);ast.parse(s);p.write_text(s)
p=BASE/'SettingsRefreshBuildAudit.cs';s=p.read_text().replace('public int profileRevision,','public int settingsRevision,profileRevision,',1)
s=s.replace('        var profile=','        var settings=Resources.Load<GameObject>("ProductionUI/Pages/Settings").GetComponent<LegendsOfRus.ProductionSettingsView>();\n        if(!settings.HasRequiredBindings||settings.Revision!=5||settings.Revision!=LegendsOfRus.ProductionSettingsView.CurrentRevision)\n            throw new InvalidOperationException("Installed Settings bindings/revision differ.");\n        var profile=',1).replace('            profileRevision=profile.Revision,','            settingsRevision=settings.Revision,profileRevision=profile.Revision,',1);p.write_text(s)
p=BASE/'ComparePacked.py';s=p.read_text().replace("assert record['result'] == 'Succeeded'","assert record['result'] == 'Succeeded' and record['settingsRevision']==5",1).replace("'RewardClaim'):","'RewardClaim', 'Settings'):",1).replace('seven page','eight page');p.write_text(s)
for name in ('VerifyBuild.py','VerifyFinal.py'):
 p=BASE/name;s=p.read_text().replace('==7 and all','==8 and all').replace('SevenPage','EightPage').replace('sevenNativePages','eightNativePages')
 if name=='VerifyBuild.py':
  s=s.replace("with zipfile.ZipFile(apk) as z:","settings=json.loads((BASE/'Sources/SettingsNativePolicy.json').read_text());assert native['atomicSettingsPreferenceRecoveryPackaged'] and native['settingsOwnerRegionsVerified']==7\nassert all(native['methods'][o][flag] is True for o,keys in settings['flags'].items() for flag in keys)\nassert set(native['settingsCallbacks'])=={'OwnedSettingsListener'} and all(r['packagedMethodBytesMatchCompiledOutput'] for r in native['settingsCallbacks'].values())\nassert not native['methods']['GameState_TryTogglePreference']['callsPersist']\nwith zipfile.ZipFile(apk) as z:",1)
  s=s.replace("'atomicRoundLifecycleRecoveryPackaged':True", "'atomicSettingsPreferenceRecoveryPackaged':True,'settingsOwnerRegionsVerified':7,'settingsCallbackRegionsVerified':1,'atomicRoundLifecycleRecoveryPackaged':True",1)
 else:
  s=s.replace("('atomicRoundLifecycleRecoveryPackaged'","('atomicSettingsPreferenceRecoveryPackaged','atomicRoundLifecycleRecoveryPackaged'",1).replace("'nativeOwnersVerified':68","'nativeOwnersVerified':68,'settingsCallbackRegionsVerified':1",1)
 ast.parse(s);p.write_text(s)
print('Settings native audit prepared: 67 before/68 after owners, seven Settings regions and one owned callback.')
