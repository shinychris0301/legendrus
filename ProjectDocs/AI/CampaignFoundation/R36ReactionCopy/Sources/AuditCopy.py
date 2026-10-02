from pathlib import Path
from collections import defaultdict
import json,hashlib
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
active=root/'Assets/LegendsOfRus/Resources/CampaignActive';candidate=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01EditorialR36'
read=lambda p:json.loads(p.read_text())
rows=read(base/'AppliedCopy.json')['entries'];allowed={r['key'] for r in rows};locales={}
for name in ['definition','levels','story']:assert (active/f'{name}.json').read_bytes()==(base/'Sources'/f'{name}.before.json').read_bytes()
for name in ['levels','story','story_ru','story_en']:assert (active/f'{name}.json').read_bytes()==(candidate/f'{name}.json').read_bytes()
story=read(active/'story.json');levels=read(active/'levels.json')['levels']
for lang in ['ru','en']:
 old=read(base/'Sources'/f'story_{lang}.before.json');new=read(active/f'story_{lang}.json');oldvalues={e['key']:e['value'] for e in old['entries']};newvalues={e['key']:e['value'] for e in new['entries']}
 assert list(oldvalues)==list(newvalues) and len(newvalues)==len(new['entries'])
 changed={k for k in oldvalues if oldvalues[k]!=newvalues[k]};assert changed==allowed
 for row in rows:assert newvalues[row['key']]==row[lang]
 for e in new['entries']:
  if e['key'] in allowed:e['value']=oldvalues[e['key']]
 assert new==old
 texts=defaultdict(list)
 for reaction in story['reactions']:texts[newvalues[reaction['textKey']]].append(reaction['id'])
 duplicates=[dict(text=k,reactions=v,count=len(v)) for k,v in texts.items() if len(v)>1]
 locales[lang]=dict(changedEntries=len(changed),distinctBlockTexts=len({newvalues[k] for k in allowed}),unmodifiedEntries=len(newvalues)-len(changed),duplicateGroups=len(duplicates),repeatedOccurrences=sum(d['count']-1 for d in duplicates),largestRemainingGroup=max(d['count'] for d in duplicates),duplicates=duplicates)
required={r[k] for r in story['reactions'] for k in ['speakerKey','textKey']}|{line[k] for s in story['scenes'] for line in s['lines'] for k in ['speakerKey','textKey']}|{f'campaign.round{n:03d}.name' for n in range(11,201)}
for lang in ['ru','en']:
 values={e['key']:e['value'] for e in read(active/f'story_{lang}.json')['entries']};assert all(values.get(k,'').strip() for k in required)
result=dict(passed=True,levelsUnchanged=200,storyBytesUnchanged=True,definitionBytesUnchanged=True,candidateParity=True,requiredBilingualKeys=len(required),locales=locales)
(base/'QA/CopyAudit.json').write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n')
p=Path('/home/k2/.config/unity3d/DefaultCompany/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL/CampaignPlaytests/Sessions/manual_campaign');files=[];malformed=[]
for f in sorted(p.glob('*.json')):
 try:d=read(f)
 except (ValueError,OSError):malformed.append(f.name);continue
 rounds=sorted({a['round'] for a in d.get('attempts',[])+d.get('activeAttempts',[]) if isinstance(a,dict) and isinstance(a.get('round'),int)})
 files.append(dict(file=f.name,sha256=hashlib.sha256(f.read_bytes()).hexdigest(),source=d.get('source'),contentVersion=d.get('contentVersion'),rounds=rounds))
manual=dict(folder=str(p),files=files,malformed=malformed,filesCovering31To200=[f['file'] for f in files if f['source']=='manual_campaign' and any(31<=n<=200 for n in f['rounds'])],scope='Default export folder only; external locations not inspected.')
(base/'QA/ManualEvidence.json').write_text(json.dumps(manual,indent=2)+'\n')
print(json.dumps(dict(passed=True,changed={k:v['changedEntries'] for k,v in locales.items()},remainingRepeatedOccurrences={k:v['repeatedOccurrences'] for k,v in locales.items()},laterManualFiles=len(manual['filesCovering31To200'])),indent=2))
