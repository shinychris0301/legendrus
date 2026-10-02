from pathlib import Path
from collections import defaultdict
import json, hashlib, shutil, subprocess, xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
active=root/'Assets/LegendsOfRus/Resources/CampaignActive'; previous=base.parent/'R33OrchardFieldCheckpoint'
read=lambda p:json.loads(p.read_text())
write=lambda name,data:(base/'QA'/name).write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n')
story=read(active/'story.json');scenes={s['id']:s for s in story['scenes']};levels=read(active/'levels.json')['levels'];locales={lang:{e['key']:e['value'] for e in read(active/f'story_{lang}.json')['entries']} for lang in ['ru','en']}
required={line[key] for s in scenes.values() for line in s['lines'] for key in ['textKey','speakerKey']}|{r[key] for r in story['reactions'] for key in ['textKey','speakerKey']}|{f'campaign.round{n:03d}.name' for n in range(11,201)}
for lang,values in locales.items():assert all(values.get(k,'').strip() for k in required),lang
report={}
for lang,values in locales.items():
 groups=defaultdict(list)
 for r in story['reactions']:groups[values[r['textKey']]].append(dict(round=r['levelId'],outcome=r['outcome'],key=r['textKey']))
 duplicate=[dict(text=txt,count=len(refs),references=refs) for txt,refs in groups.items() if len(refs)>1]
 duplicate.sort(key=lambda x:-x['count'])
 report[lang]=dict(reactions=len(story['reactions']),uniqueTexts=len(groups),duplicateGroups=len(duplicate),repeatedOccurrences=sum(g['count']-1 for g in duplicate),groups=duplicate)
write('EditorialAudit.json',report)
backlog=[dict(afterRound=m['afterLevelId'],sceneId=m['sceneId'],lines=len(scenes[m['sceneId']]['lines']),words={lang:sum(len(values[line['textKey']].split()) for line in scenes[m['sceneId']]['lines']) for lang,values in locales.items()}) for m in story['milestones']]
write('BacklogAudit.json',dict(milestones=len(backlog),lines=sum(x['lines'] for x in backlog),words={lang:sum(x['words'][lang] for x in backlog) for lang in locales},entries=backlog,routeFinding='One unread scene per TryShowPendingChapterFlow call. Dialogue completion returns directly to ChapterMap. Re-entering via OpenSelectedLevel or OpenChapterMap checks the next unread scene. No automatic continuous 27-scene sequence.',routeEvidence='Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs:1125-1198',scope='Static source/data review; no new pointer or screen capture audit.'))
files=['levels.json','story.json','story_ru.json','story_en.json'];candidate=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R33'
parity={f:active.joinpath(f).read_bytes()==candidate.joinpath(f).read_bytes() for f in files};assert all(parity.values())
suites=[]
for p in sorted((previous/'QA/Verify01').glob('*/results.xml')):
 a=ET.parse(p).getroot().attrib;assert a['result']=='Passed' and int(a['failed'])==0
 suites.append(dict(suite=p.parent.name,passed=int(a['passed']),failed=int(a['failed']),skipped=int(a['skipped']),sha256=hashlib.sha256(p.read_bytes()).hexdigest(),path=str(p.relative_to(root))))
assert sum(s['passed'] for s in suites)==302
witnesses=list((previous/'QA/Verify01/Active/Journey').glob('*Witness.json'));assert len(witnesses)==600
write('RetainedEvidence.json',dict(rerun=False,activeCandidateByteParity=parity,requiredLocalizedKeysIncludingRoundNames=len(required),priorUnitySuites=suites,priorWitnessFiles=len(witnesses),warning='R33 evidence checked on disk; not a new 302-test or 600-replay run. Solver witnesses do not establish human difficulty.'))
adb=shutil.which('adb')
if adb is None:
 p=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb')
 if p.exists():adb=str(p)
device=dict(adbAvailable=adb is not None,installationAttempted=False,buildAttempted=False,scope='Read-only adb devices query. No serial numbers retained; historical APK does not verify current campaign content.')
if adb:
 try:
  result=subprocess.run([adb,'devices'],capture_output=True,text=True,timeout=20)
  statuses=[line.split()[1] for line in result.stdout.splitlines() if len(line.split())>=2 and not line.startswith(('List of devices','*'))]
  device.update(queryExitCode=result.returncode,attachedDevices=len(statuses),authorizedDevices=statuses.count('device'),unauthorizedDevices=statuses.count('unauthorized'))
 except subprocess.TimeoutExpired:device['query']='timeout'
write('DeviceEvidence.json',device)
print(json.dumps(dict(requiredKeys=len(required),duplicateGroups={k:v['duplicateGroups'] for k,v in report.items()},backlogScenes=len(backlog),backlogLines=sum(x['lines'] for x in backlog),retainedTests=sum(s['passed'] for s in suites),retainedWitnesses=len(witnesses),device=device),indent=2))
