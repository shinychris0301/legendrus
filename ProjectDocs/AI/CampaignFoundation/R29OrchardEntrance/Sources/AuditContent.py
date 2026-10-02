from pathlib import Path
import json,hashlib
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
read=lambda p:json.loads(p.read_text())
active=root/'Assets/LegendsOfRus/Resources/CampaignActive';levels=read(active/'levels.json')['levels'];story=read(active/'story.json');definition=read(active/'definition.json')
assert [l['levelId'] for l in levels]==list(range(1,161))
scenes={s['id']:s for s in story['scenes']};assert len(scenes)==len(story['scenes'])==24
ms=story['milestones'];assert len(ms)==23 and [m['afterLevelId'] for m in ms]==sorted(set(m['afterLevelId'] for m in ms))
assert {story['introSceneId']}|{m['sceneId'] for m in ms}==set(scenes)
for m in ms:assert scenes[m['sceneId']]['trigger'] in levels[m['afterLevelId']-1]['storyTriggers']
assert len(story['reactions'])==320
for n in range(1,161):assert sorted(r['outcome'] for r in story['reactions'] if r['levelId']==n)==['failure','victory']
required={line['textKey'] for s in scenes.values() for line in s['lines']}|{line['speakerKey'] for s in scenes.values() for line in s['lines']}|{r['textKey'] for r in story['reactions']}
for lang in ['ru','en']:
 entries=read(active/f'story_{lang}.json')['entries'];values={e['key']:e['value'] for e in entries};assert len(values)==len(entries);assert all(values.get(k,'').strip() for k in required)
assert not story.get('completion') and definition['Chapters'][0]['FinaleRound']==500
blocks=[]
for start in [1,51,101,151]:
 ls=levels[start-1:start+49];blocks.append(dict(first=start,last=min(start+49,len(levels)),rewards={k:sum(l['rewards'][k] for l in ls) for k in ls[0]['rewards']}))
report=dict(passed=True,installedRounds=160,scenes=24,milestones=23,reactions=320,bilingualRequiredKeys=len(required),unreferencedScenes=0,chapterFinaleRound=500,installedChapterFinale=False,blocks=blocks)
(base/'QA/ContentAudit.json').write_text(json.dumps(report,indent=2)+'\n')
p=Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests/Sessions/manual_campaign');files=[];malformed=[];later=[]
for f in sorted(p.glob('*.json')):
 try:d=read(f)
 except (ValueError,OSError) as e:malformed.append(dict(file=f.name,error=type(e).__name__));continue
 rounds=sorted(set(a['round'] for a in d.get('attempts',[]) if isinstance(a,dict) and isinstance(a.get('round'),int)))
 files.append(dict(file=f.name,sha256=hashlib.sha256(f.read_bytes()).hexdigest(),source=d.get('source'),contentVersion=d.get('contentVersion'),rounds=rounds))
 if d.get('source')=='manual_campaign' and any(31<=n<=160 for n in rounds):later.append(f.name)
(base/'QA/ManualEvidence.json').write_text(json.dumps(dict(folder=str(p),files=files,malformed=malformed,filesCovering31To160=later,scope='Default export folder only; absence here does not prove no external playtests exist.'),indent=2)+'\n')
print(json.dumps(report,indent=2));print('Manual files:',len(files),'later-block files:',len(later),'malformed:',len(malformed))
