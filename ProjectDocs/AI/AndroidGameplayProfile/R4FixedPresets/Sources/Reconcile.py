"""Reconcile current Unity executions, independent references and compiled guards."""
from pathlib import Path
import hashlib
import json
import xml.etree.ElementTree as ET
ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
def read(path): return json.loads(path.read_text())
def sha(path): return hashlib.sha256(path.read_bytes()).hexdigest()
plan=read(BASE/'FixturePlan.json');phases=read(BASE/'Sources/PhasePlan.json')
assert len(phases)==19
runs=[];names=set()
for name,count in [(p['name'],1) for p in phases]+[('EditVerified',50),('NativeVerified',8)]:
    folder=BASE/'QA'/name;summary=read(folder/'summary.json')
    assert summary['exitCode']==0 and not summary['nonPassing'],name
    result=summary['result']
    assert result['result']=='Passed' and int(result['passed'])==count and int(result['failed'])==int(result['skipped'])==0,name
    names.update(t.get('fullname') for t in ET.parse(folder/'results.xml').findall('.//test-case'))
    runs.append({'name':name,'passed':count,'exitCode':0,'durationSeconds':summary['durationSeconds'],
                 'actualUnityProcess':True,'isolatedConfig':summary['isolatedConfig']})
rows=[]
for preset in plan['presets']:
    name=preset['selector'];folder=BASE/'QA/Fixtures'
    payload=folder/(name+'.player.json');data=read(payload)
    reference=read(ROOT/'ProjectDocs/AI/AndroidGameplayProfile/R3FixturePreparation/QA/Fixtures'/(name+'.player.json'))
    reference['lastEnergyUtcTicks']=data['lastEnergyUtcTicks']
    assert data==reference,name
    writer=read(folder/(name+'.write.json'));reader=read(folder/(name+'.read.json'))
    assert data['saveVersion']==6 and data['energy']==4
    assert data['selectedChapter']==1 and data['selectedLevel']==data['highestUnlockedLevel']==preset['selectedRound']
    assert data['stars']==preset['stars'] and len(data['levelProgress'])==preset['completedRounds']
    assert len(data['completedStoryScenes'])==preset['recapScenes'] and not data['continuationTransactions']
    assert writer['normalFingerprintsPreserved'] and reader['normalFingerprintsPreserved'] and reader['restoredPayloadExactly']
    assert writer['recapScenes']==reader['recapScenes']==preset['recapScenes']
    if name.startswith('story-'):
        assert reader['nativeRecapPreservesJournal'] and reader['replayGrantRejected']
    else:
        assert {data[k] for k in ['boosterMace','boosterFirebird','boosterLivingLight','boosterSeaPearl']}=={preset['allFourBoosterQuantities']}
    rows.append({'selector':name,'dtoPath':str(payload.relative_to(BASE)),'sha256':sha(payload),
                 'completedRounds':preset['completedRounds'],'recapScenes':preset['recapScenes'],'stars':data['stars'],
                 'gold':data['gold'],'gems':data['gems'],'fullIndependentR3ReferenceMatchesExceptClock':True,
                 'exactColdProcessRestoration':True,'normalFingerprintsPreserved':True,
                 'nativeRecapChecked':name.startswith('story-'),'implementedEditorCommand':True,'currentApkIncludesCommand':False})
guards=read(BASE/'QA/EntryGuards/summary.json')
assert guards['passed']==120 and guards['failed']==0 and guards['actualProfileAndFactorySourceCompiled']
assert guards['fakeJniContentAndPreferences'] and not guards['realAndroidExecution'] and not guards['actualUnityReleaseBuild']
initial=read(BASE/'QA/EditFixtures/summary.json')
assert initial['exitCode']==2 and int(initial['result']['passed'])==49 and int(initial['result']['failed'])==1
assert len(initial['nonPassing'])==1 and '("overflow")' in initial['nonPassing'][0]['name']
images=[{'path':str(p.relative_to(BASE)),'bytes':p.stat().st_size,'sha256':sha(p)} for p in sorted((BASE/'QA/NativeVerified').rglob('*.png'))]
(BASE/'QA/Captures.json').write_text(json.dumps(images,indent=2)+'\n')
result={'clientTaskDate':'2026-10-02','result':'Passed','unitySeparateProcessesPassed':21,
        'unityEditModePassed':50,'unityNativePlayModePassed':8,'unityColdProcessPhaseChecksPassed':19,
        'unitySuccessfulCaseExecutions':77,'unityDistinctTestCasesPassed':len(names),
        'processExecutionsAreNotNineteenDistinctTestNames':True,'implementedPresets':rows,'runs':runs,
        'compiledEntryGuardChecksPassed':120,'fakeJniContentAndPreferences':True,'realAndroidExecution':False,
        'initialReflectionOverflowFixtureFailureRetainedAndCorrected':True,'originalCommandsPreserved':True,
        'currentApkIncludesNewSelectors':False,'newAndroidBuild':False,'humanDifficultyOrPhoneAcceptance':False,
        'nativeUnityPngCount':len(images)}
(BASE/'QA/Reconciliation.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({k:v for k,v in result.items() if k not in ['runs','implementedPresets']},indent=2))
