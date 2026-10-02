from pathlib import Path
import json,uuid
ROOT=Path(__file__).resolve().parents[5]
base=ROOT/'Assets/LegendsOfRus/EditorContent/CampaignCandidates';out=base/'Chapter01Sector01R14';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'Chapter01Sector01R13/levels.json');story=read(base/'Chapter01Sector01R13/story.json')
locales={lang:read(base/'Chapter01Sector01R13'/f'story_{lang}.json') for lang in ['ru','en']}
plan=ROOT/'ProjectDocs/AI/CampaignFoundation/R7/Plan';briefs=read(plan/'First50Rounds.json')['rows'][30:40]
layouts={32:('chain',[(1,2),(1,3),(2,3),(5,3),(6,3),(6,4)],1),
33:('ice',[(x,y) for x in (2,5) for y in (2,3,4)],1),
34:('crate',[(x,y) for x in (1,3,6) for y in (2,3,4)],1),
35:('chain',[(x,y) for x in (1,3,6) for y in (2,3,4)]+[(4,4)],1),
37:('ice',[(x,y) for x in (1,6) for y in (1,2,4,5)],1),
38:('crate',[(x,y) for x in (1,2,5,6) for y in (2,3)],1),
40:('ice',[(x,y) for x in (0,7) for y in (1,2,3,4,5)],2)}
for b in briefs:
 n=b['round'];l=dict(id=b['key'],chapterId=1,levelId=n,displayName=b['nameRu'],columns=8,rows=7,tileTypes=b['colors'],moveLimit=b['moves'],startingScore=0,boardSeed=4100+n,difficulty=b['difficulty'],difficultyRating={'teach':3,'recovery':2,'normal':4,'hard':6}[b['difficulty']],goals=b['goals'],fixedCells=[],obstacles=[],boardItems=[],rewards=b['reward'],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 if n in layouts:
  kind,coords,layers=layouts[n];l['obstacles']=[dict(x=x,y=y,type=kind,layers=layers) for x,y in coords]
 if n in (33,35,36,39):
  coords={33:[(2,1),(5,1)],35:[(1,1),(3,1),(6,1)],36:[(3,2)],39:[(2,0),(5,2)]}[n]
  l['boardItems']=[dict(x=x,y=y,type='firebird_feather') for x,y in coords]
 if n==38:l['obstacles'] += [dict(x=x,y=y,type='chain',layers=1) for x in (2,3,4,5,6,7) for y in (4,)]
 if n==40:l['obstacles'] += [dict(x=x,y=y,type='crate',layers=1) for x in (2,3,4,5) for y in (2,4)]
 if n==37:
  # Adjacent preplaced row/column items: optional combined swap, not scripted autoplay.
  l['fixedCells']=[dict(x=3,y=3,tileType=0,special=1),dict(x=4,y=3,tileType=1,special=2)]
 levels['levels'].append(l)
scene=next(s for s in read(plan/'NewSceneDrafts.json')['scenes'] if s['afterRound']==40)
trigger='dialogue.'+scene['id'];levels['levels'][39]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene['id'],trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey=l['speakerKey'],textKey=l['textKey']) for l in scene['lines']]))
story['milestones'].append(dict(id=scene['id']+'.milestone',afterLevelId=40,sceneId=scene['id'],major=True))
reactions=[('Тропа снова видна. Пора спуститься к реке.','The path is visible again. Time to head down to the river.'),('Цепи больше не скрывают проход к воде.','The chains no longer hide the path to the water.'),('Перья выбрались изо льда. Сохраним их тепло.','The feathers are free of the ice. Let us preserve their warmth.'),('Мостки свободны. Можно пройти к переправе.','The walkways are clear. We can reach the crossing.'),('Тень отступила от перьев. Путь ещё не окончен.','The shadow has drawn back from the feathers. The journey is not over.'),('В тихой воде снова отражается свет.','Light is reflected in the quiet water again.'),('Два потока света открыли дорогу.','Two streams of light have opened the way.'),('Переправа больше не скована.','The crossing is no longer bound.'),('Перья укажут путь к сигнальному огню.','The feathers will show the way to the beacon.'),('Берег удержан. Теперь нужен сигнальный огонь.','The bank holds. Now we need a beacon.')]
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 for line in scene['lines']:entries[line['textKey']]=line[lang]
 for b,texts in zip(briefs,reactions):
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for outcome in ['victory','failure']:
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}'
   entries[key]=texts[0 if lang=='ru' else 1] if outcome=='victory' else ('Попробую снова. Сначала открою путь к цели.' if lang=='ru' else 'I will try again. First, I must open a path to the goal.')
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:(out/(name+'.json')).write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n')
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
print(out)
