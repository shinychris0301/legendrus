from pathlib import Path
import json,uuid
ROOT=Path(__file__).resolve().parents[5]
base=ROOT/'Assets/LegendsOfRus/EditorContent/CampaignCandidates';out=base/'Chapter01Sector01R13';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'Chapter01Sector01R12/levels.json');story=read(base/'Chapter01Sector01/story.json')
locales={lang:read(base/'Chapter01Sector01'/f'story_{lang}.json') for lang in ['ru','en']}
plan=ROOT/'ProjectDocs/AI/CampaignFoundation/R7/Plan';briefs=read(plan/'First50Rounds.json')['rows'][20:30]
layouts={22:('crate',[(1,2),(2,2),(2,3),(3,3),(4,4),(5,4),(5,5),(6,5)],1),
23:('chain',[(1,3),(3,3),(4,4),(6,4)],1),
25:('crate',[(x,y) for y in (2,3,4) for x in (2,3,4,5)],1),
26:('crate',[(1,2),(6,2),(1,4),(6,4)],1),
27:('crate',[(1,3),(2,3),(3,3),(4,4),(5,4),(6,4)],1),
28:('ice',[(x,y) for x in (1,6) for y in (1,2,4,5)],2),
29:('chain',[(x,y) for x in (1,6) for y in (1,2,3,4,5)],1),
30:('crate',[(x,y) for x in (1,2,5,6) for y in (2,3)]+[(3,4),(4,4)],1)}
for b in briefs:
 n=b['round'];l=dict(id=b['key'],chapterId=1,levelId=n,displayName=b['nameRu'],columns=8,rows=7,tileTypes=b['colors'],moveLimit=b['moves'],startingScore=0,boardSeed=3100+n,difficulty=b['difficulty'],difficultyRating={'teach':3,'recovery':2,'normal':4,'hard':6}[b['difficulty']],goals=b['goals'],fixedCells=[],obstacles=[],boardItems=[],rewards=b['reward'],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 if n in layouts:
  kind,coords,layers=layouts[n];l['obstacles']=[dict(x=x,y=y,type=kind,layers=layers) for x,y in coords]
 if n in (23,27,30):l['boardItems']=[dict(x=x,y=1,type='firebird_feather') for x in ((2,5) if n!=30 else (3,4))]
 if n==24:
  # Swap (3,2) down into (3,3): guarded horizontal five, optional Color special.
  cells=[(x,3,0 if x!=3 else 1) for x in range(1,6)]+[(0,3,2),(6,3,3),(3,2,0),(3,1,2)]
  cells += [(x,2,t) for x,t in [(1,1),(2,2),(4,3),(5,4)]]+[(x,4,t) for x,t in [(1,2),(2,3),(3,4),(4,1),(5,2)]]
  l['fixedCells']=[dict(x=x,y=y,tileType=t,special=0) for x,y,t in cells]
 levels['levels'].append(l)
scene=next(s for s in read(plan/'NewSceneDrafts.json')['scenes'] if s['afterRound']==30)
trigger='dialogue.'+scene['id'];levels['levels'][29]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene['id'],trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey=l['speakerKey'],textKey=l['textKey']) for l in scene['lines']]))
story['milestones'].append(dict(id=scene['id']+'.milestone',afterLevelId=30,sceneId=scene['id'],major=True))
reactions=[('У ворот снова светло. Пора открыть дорогу к припасам.','The gate is bright again. Time to reach the supplies.'),('Дорога к амбару свободна.','The path to the granary is clear.'),('Оба пера сохранены. Жар-птице понадобится их тепло.','Both feathers are safe. The Firebird will need their warmth.'),('Пять искр стали единым светом.','Five sparks have become one light.'),('Засов больше не держит двери.','The bar no longer holds the doors shut.'),('Пусть в домах снова пахнет хлебом.','May the homes smell of bread again.'),('Перья больше не затеряются под крышей.','The feathers will no longer be lost beneath the roof.'),('Сквозь окна снова проходит свет.','Light passes through the windows again.'),('Последние цепи сняты с дверей.','The last chains are off the doors.'),('Припасы спасены. Теперь проверим переправу.','The supplies are safe. Now we should check the crossing.')]
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
