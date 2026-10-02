"""Reconcile the six reference DTOs and fourteen actual Unity process phases."""
from pathlib import Path
import hashlib
import json

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
def read(path): return json.loads(path.read_text())
plan=read(BASE/'FixturePlan.json')
phases=read(BASE/'Sources/PhasePlan.json')
assert len(phases)==14
runs=[]
for phase in phases:
    summary=read(BASE/'QA'/phase['name']/'summary.json')
    assert summary['exitCode']==0 and not summary['nonPassing']
    assert summary['result']['result']=='Passed' and int(summary['result']['passed'])==1
    assert int(summary['result']['failed'])==0 and int(summary['result']['skipped'])==0
    runs.append({'name':phase['name'],'passed':1,'exitCode':0,'durationSeconds':summary['durationSeconds'],
                 'actualUnityProcess':True,'isolatedConfig':summary['isolatedConfig']})
rows=[]
for preset in plan['presets']:
    name=preset['selector'];folder=BASE/'QA/Fixtures'
    payload=folder/(name+'.player.json');data=read(payload)
    writer=read(folder/(name+'.write.json'));reader=read(folder/(name+'.read.json'))
    assert data['saveVersion']==plan['activePayloadVersion']==6
    assert data['energy']==plan['energy']==4
    assert data['selectedChapter']==1 and data['selectedLevel']==preset['selectedRound']
    assert data['highestUnlockedLevel']==preset['selectedRound'] and data['stars']==preset['stars']
    progress=data['levelProgress'];count=preset['completedRounds']
    assert len(progress)==count and {row['levelId'] for row in progress}==set(range(1,count+1))
    assert all(row['chapterId']==1 and row['completions']==row['attempts']==1 and row['bestStars']==3 and row['rewardClaimed'] for row in progress)
    assert len(data['completedStoryScenes'])==preset['recapScenes']
    assert not data['continuationTransactions']
    assert not data.get('retainedRound') or not data['retainedRound'].get('attemptId')
    assert writer['normalFingerprintsPreserved'] and reader['normalFingerprintsPreserved'] and reader['restoredPayloadExactly']
    assert writer['recapScenes']==reader['recapScenes']==preset['recapScenes']
    if 'allFourBoosterQuantities' in preset:
        assert {data[key] for key in ['boosterMace','boosterFirebird','boosterLivingLight','boosterSeaPearl']}=={preset['allFourBoosterQuantities']}
        assert data['gold']==12540 and data['gems']==350 and not data['acknowledgedChapterCompletions']
    else:
        assert reader['nativeRecapPreservesJournal'] and reader['replayGrantRejected']
        assert data['acknowledgedChapterCompletions']==([1] if preset['chapterAcknowledged'] else [])
    rows.append({'selector':name,'dtoPath':str(payload.relative_to(BASE)),
                 'sha256':hashlib.sha256(payload.read_bytes()).hexdigest(),
                 'completedRounds':count,'recapScenes':preset['recapScenes'],'stars':data['stars'],
                 'gold':data['gold'],'gems':data['gems'],'exactColdProcessRestoration':True,
                 'normalFingerprintsPreserved':True,'nativeRecapChecked':name.startswith('story-'),
                 'implementedPlayerCommand':False})
guards=read(BASE/'QA/EntryGuards/summary.json')
assert guards['passed']==34 and guards['failed']==0 and guards['fakeJniAndPreferences'] and not guards['realAndroidExecution']
initial=read(BASE/'QA/NormalBefore/summary.json')
assert initial['exitCode']==1 and 'result' not in initial
assert "'Is' does not contain a definition for 'AnyOf'" in (BASE/'QA/NormalBefore/unity.log').read_text()
result={'clientTaskDate':'2026-10-02','result':'Passed','unitySeparateProcessesPassed':14,
        'processExecutionsAreNotFourteenDistinctTestNames':True,'rehearsedPresets':rows,'runs':runs,
        'compiledEntryGuardChecksPassed':34,'fakeJniAndPreferences':True,'realAndroidExecution':False,
        'initialTemporaryNunitCompileFailureRetained':True,'currentPlayerCommandSetUnchanged':True,
        'newReleaseBuildOrAndroidActivationVerified':False,'humanDifficultyOrPhoneAcceptance':False}
(BASE/'QA/Reconciliation.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({'unitySeparateProcessesPassed':14,'rehearsedPresets':len(rows),'compiledEntryGuardChecksPassed':34}))
