from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01R16';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==40
plan=root/'ProjectDocs/AI/CampaignFoundation/R7/Plan';briefs=read(plan/'First50Rounds.json')['rows'][40:50]
layouts={
42:('crate',[(x,1) for x in (1,2,5,6)]+[(x,5) for x in (1,2,5,6)]+[(0,3),(7,3)]),
43:('chain',[(x,y) for x in (1,2) for y in (1,2)]+[(x,y) for x in (5,6) for y in (3,4)]+[(3,5),(4,5)]),
44:('ice',[(x,y) for x in (1,3,6) for y in (2,4)]),
45:('crate',[(x,y) for x in (1,3,6) for y in (2,3,5)]+[(4,5)]),
46:('ice',[(0,1),(7,1),(0,5),(7,5)]),
48:('crate',[(x,y) for x in (1,2) for y in (1,2,4,5)]),
50:('crate',[(x,y) for x in (1,3,6) for y in (2,3,4,5)])}
for b in briefs:
 n=b['round'];l=dict(id=b['key'],chapterId=1,levelId=n,displayName=b['nameRu'],columns=8,rows=7,tileTypes=b['colors'],moveLimit=b['moves'],startingScore=0,boardSeed=5100+n,difficulty=b['difficulty'],difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],goals=b['goals'],fixedCells=[],obstacles=[],boardItems=[],rewards=b['reward'],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 if n in layouts:
  kind,coords=layouts[n];l['obstacles']=[dict(x=x,y=y,type=kind,layers=2 if n in (45,50) and y==5 else 1) for x,y in coords]
 if n in (45,50):l['boardItems']=[dict(x=x,y=0,type='firebird_feather') for x in (1,3,6)]
 if n==48:l['obstacles'] += [dict(x=x,y=y,type='chain',layers=1) for x in (5,6) for y in (1,2,4,5)]
 levels['levels'].append(l)
scene=next(s for s in read(plan/'NewSceneDrafts.json')['scenes'] if s['afterRound']==50)
trigger='dialogue.'+scene['id'];levels['levels'][49]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene['id'],trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey=l['speakerKey'],textKey=l['textKey']) for l in scene['lines']]))
story['milestones'].append(dict(id=scene['id']+'.milestone',afterLevelId=50,sceneId=scene['id'],major=True))
texts=[('У домов снова тепло. Продолжим путь.','The houses are warm again. Let us continue.'),('Угольки сохранены. Они помогут зажечь огонь.','The embers are safe. They will help light the beacon.'),('Замки сняты. Дорога к огню открыта.','The locks are gone. The path to the fire is open.'),('Мы сохранили тепло среди льда.','We kept the warmth alive amid the ice.'),('Три пера свободны. Их свет нам пригодится.','Three feathers are free. Their light will help us.'),('Лёд отступил от сада. Можно перевести дух.','The ice has receded from the orchard. We can catch our breath.'),('Искры собраны. До огня осталось немного.','The sparks are gathered. The beacon is close.'),('Путь расчищен. Пора зажечь сигнальный огонь.','The path is clear. It is time to light the beacon.'),('Свет не угас. Сделаем последний шаг к огню.','The light held. One more step to the beacon.'),('Сигнальный огонь зажжён. Деревне ещё нужна помощь.','The beacon is lit. The village still needs our help.')]
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 for line in scene['lines']:entries[line['textKey']]=line[lang]
 for b,pair in zip(briefs,texts):
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for outcome in ['victory','failure']:
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=pair[0 if lang=='ru' else 1] if outcome=='victory' else ('Попробую снова. Сначала открою путь к цели.' if lang=='ru' else 'I will try again. First, I must open a path to the goal.')
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
print('Installed 50 rounds; first 40 preserved.')
