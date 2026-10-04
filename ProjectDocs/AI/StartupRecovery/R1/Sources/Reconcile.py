"""Reconcile actual final Unity cases; count focused repeats and restart phases once."""
from pathlib import Path
import json,xml.etree.ElementTree as ET
BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
read=lambda n:json.loads((QA/n).read_text())
groups=('Core','Settings','Navigation','Retained','Edit','LanguageWrite','LanguageRead')
seen={};by_mode={'PlayMode':set(),'EditMode':set()};counts={}
for name in groups:
    summary=read(name+'/summary.json');result=summary['result'];assert summary['exitCode']==0 and result['failed']==result['skipped']=='0' and result['result']=='Passed'
    cases=ET.parse(QA/name/'results.xml').findall('.//test-case');names=[c.get('fullname') for c in cases]
    assert len(names)==len(set(names))==int(result['passed']) and all(c.get('result')=='Passed' for c in cases)
    counts[name]=len(names)
    for key in names:
        if key in seen:assert name=='LanguageRead' and seen[key]=='LanguageWrite'
        seen[key]=name
    by_mode[summary['mode']].update(names)
focus={c.get('fullname') for n in ('Focus','BoundariesCorrected') for c in ET.parse(QA/n/'results.xml').findall('.//test-case')}
assert len(focus)==15 and focus<=by_mode['PlayMode']
for name,count in (('Focus',5),('BoundariesCorrected',10)):
    summary=read(name+'/summary.json');assert summary['exitCode']==0 and summary['result']['passed']==str(count) and summary['result']['failed']==summary['result']['skipped']=='0'
r=read('Reproduce/summary.json');assert r['exitCode']==2 and r['result']['failed']=='5' and r['result']['passed']=='0'
assert all('same as' in r['message'] for r in r['nonPassing'])
initial=read('Boundaries/summary.json');assert initial['result']['passed']=='7' and initial['result']['failed']=='3'
assert all('unknown.scene' in r['message'] for r in initial['nonPassing'])
write=read('LanguageWrite/summary.json');read_phase=read('LanguageRead/summary.json')
assert write['isolatedConfig']==read_phase['isolatedConfig']=='/tmp/lor-startup-recovery-r1/LanguageRestart/config'
assert counts['LanguageWrite']==counts['LanguageRead']==1
result={'result':'Passed','groups':counts,'playPassed':len(by_mode['PlayMode']),'editPassed':len(by_mode['EditMode']),'totalDistinctPassed':len(seen),'successfulFinalLaunchers':7,'failed':0,'skipped':0,'focusedDuplicatesCountedAgain':False,'languageRestartPhaseExecutionsPassed':2,'languageRestartCaseCountedOnce':True,'languageHealthyPersistenceVerifiedAcrossFreshUnityProcesses':True,'languageRejectedSavingNotYetVerified':True,'preFixFailuresReproduced':5,'initialBoundaryExpectationCorrection':{'initialPassed':7,'initialFailed':3,'correctedPassed':10,'reason':'StoryProgress.Normalize preserves nonempty unknown scene IDs; fixture expectation corrected, production policy retained.'}}
assert result['totalDistinctPassed']==result['playPassed']+result['editPassed']
(QA/'TestReconciliation.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2))
