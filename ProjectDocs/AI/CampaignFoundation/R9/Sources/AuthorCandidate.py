from pathlib import Path
import json,copy,uuid
root=Path(__file__).resolve().parents[5]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01'
out.mkdir(parents=True,exist_ok=True)
plan=root/'ProjectDocs/AI/CampaignFoundation/R7/Plan'
def read(p):return json.loads(p.read_text())
def write(name,obj):(out/name).write_text(json.dumps(obj,ensure_ascii=False,indent=2)+'\n')
levels=read(root/'Assets/LegendsOfRus/Resources/Config/chapter01_levels.json')
briefs=read(plan/'First50Rounds.json')['rows'][10:20]
# Explicit layouts: each pocket leaves adjacent match lanes; y increases down the board.
layouts={13:('ice',[(1,3),(2,3),(1,4),(5,3),(6,3),(6,4)],1),14:('chain',[(1,y) for y in (2,3,4)]+[(6,y) for y in (2,3,4)],1),15:('ice',[(x,y) for y in (2,3) for x in (2,3,4,5)]+[(3,4),(4,4)],2),16:('ice',[(0,0),(7,0),(0,6),(7,6)],1),17:('chain',[(x,y) for x in (1,2,5,6) for y in (2,4)],1),18:('crate',[(x,y) for x in (1,6) for y in (3,4,5)],1),19:('crate',[(x,y) for x in (1,2,5,6) for y in (3,4)],1)}
for b in briefs:
 n=b['round'];l=dict(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],columns=8,rows=7,tileTypes=b['colors'],moveLimit=b['moves'],startingScore=0,boardSeed=2100+n,difficulty=b['difficulty'],difficultyRating={'teach':3,'recovery':2,'normal':4,'hard':6}[b['difficulty']],goals=b['goals'],fixedCells=[],obstacles=[],boardItems=[],rewards=b['reward'],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 if n in layouts:
  kind,coords,layers=layouts[n];l['obstacles']=[dict(x=x,y=y,type=kind,layers=layers) for x,y in coords]
 if n==20:
  l['obstacles']=[dict(x=x,y=y,type='ice',layers=2) for x in (2,3,4,5) for y in (2,4)]+[dict(x=x,y=y,type='chain',layers=1) for x in (0,7) for y in (2,3,4)]
 if n==12:
  # (3,2) -> (3,3) completes a horizontal four without a starting match.
  l['fixedCells']=[dict(x=x,y=y,tileType=t,special=0) for x,y,t in [(1,3,0),(2,3,0),(3,3,1),(4,3,0),(3,2,0),(0,3,2),(5,3,3),(1,2,1),(2,2,2),(4,2,3),(1,4,2),(2,4,3),(3,4,4),(4,4,1),(3,1,2)]]
 if n==18:
  # (3,2) -> (3,3) completes a five-cell T for a Burst.
  l['fixedCells']=[dict(x=x,y=y,tileType=t,special=0) for x,y,t in [(2,3,1),(3,3,2),(4,3,1),(3,2,1),(3,4,1),(3,5,1),(5,3,4),(2,4,2),(4,4,3),(2,5,3),(4,5,4),(3,6,2),(3,1,3)]]
 levels['levels'].append(l)
story=read(root/'Assets/LegendsOfRus/Resources/Config/chapter01_story.json')
story['scenes']=[s for s in story['scenes'] if s['id']!='chapter01.finale']
story['milestones']=[m for m in story['milestones'] if m['afterLevelId']!=10];story['completion']=None
ru=read(root/'Assets/LegendsOfRus/Resources/Localization/story_ru.json');en=read(root/'Assets/LegendsOfRus/Resources/Localization/story_en.json')
locales={'ru':{e['key']:e['value'] for e in ru['entries']},'en':{e['key']:e['value'] for e in en['entries']}}
for e in read(plan/'LocalizationDraft.json')['entries']:locales['en'][e['key']]=e['en']
for s in read(plan/'NewSceneDrafts.json')['scenes']:
 n=s['afterRound']
 if n not in (2,10,20):continue
 trigger='dialogue.'+s['id'];story['scenes'].append(dict(id=s['id'],trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey=l['speakerKey'],textKey=l['textKey']) for l in s['lines']]))
 story['milestones'].append(dict(id=s['id']+'.milestone',afterLevelId=n,sceneId=s['id'],major=n in (10,20)))
 levels['levels'][n-1]['storyTriggers']=[trigger]
 for line in s['lines']:
  for lang in locales:locales[lang][line['textKey']]=line[lang]
# The old board and reward stay identical; only its candidate presentation stops claiming finale.
levels['levels'][9]['displayName']='Спасти деревенский очаг';levels['levels'][9]['difficulty']='hard'
for outcome,ruText,enText in [('victory','Один очаг спасён. Теперь сбережём остальные огни деревни.',"One hearth is safe. Now we must protect the village's other fires."),('failure','Этот очаг ещё во тьме. Я попробую другой путь.','This hearth is still in darkness. I will try another way.')]:
 key='reaction.campaign.chapter01.hearth.'+outcome
 for r in story['reactions']:
  if r['levelId']==10 and r['outcome']==outcome:r['textKey']=key
 locales['ru'][key]=ruText;locales['en'][key]=enText
# Short round-specific reactions are localized content, not additional automatic dialogue.
reactions=[('Пепел ещё хранит тепло. Пусть оно вернётся в дома.','The ash still holds warmth. Let it return to the homes.'),('Искры собрались в ряд. Теперь их свет пробьёт лёд.','The sparks have lined up. Their light can now break the ice.'),('Между льдинами открылся путь.','A path has opened between the ice.'),('Цепи больше не держат тепло.','The chains no longer hold back the warmth.'),('Толстый лёд разбит. До воды уже ближе.','The thick ice is broken. The water is closer now.'),('Чистая вода поможет сохранить огонь.','Clear water will help keep the fire alive.'),('Ворот свободен. Можно поднять ведро.','The windlass is free. We can raise the bucket.'),('Осколки разлетелись, и путь к воде открыт.','The fragments scattered, opening a path to the water.'),('Вёдра готовы. Осталось убрать последний лёд.','The buckets are ready. Only the last ice remains.'),('Первый ручей снова чист. Деревня оживает.','The first stream is clear again. The village is coming back to life.')]
for b,(rt,et) in zip(briefs,reactions):
 n=b['round']
 for outcome in ('victory','failure'):
  key=f'reaction.campaign.chapter01.round{n:03}.{outcome}'
  story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
  locales['ru'][key]=rt if outcome=='victory' else 'Попробую снова. Сначала найду путь к цели.'
  locales['en'][key]=et if outcome=='victory' else 'I will try again. First, I must find a path to the goal.'
 for lang in locales:locales[lang][f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
story['milestones'].sort(key=lambda m:m['afterLevelId'])
# Keep only referenced story keys plus candidate names; no dormant finale copy in this package.
keys={k for s in story['scenes'] for l in s['lines'] for k in (l['speakerKey'],l['textKey'])}|{k for r in story['reactions'] for k in (r['speakerKey'],r['textKey'])}|{f'campaign.round{n:03}.name' for n in range(11,21)}
write('levels.json',levels);write('story.json',story)
for lang in locales:write('story_'+lang+'.json',dict(schemaVersion=1,locale=lang,entries=[dict(key=k,value=v) for k,v in locales[lang].items() if k in keys]))
# Author stable metas only when absent.
for p in [root/'Assets/LegendsOfRus/EditorContent',out.parent,out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():
  meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n' if p.is_dir() else 'TextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n'))
print(out)
