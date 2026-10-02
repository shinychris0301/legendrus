from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector02R17';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==50

def collect(t,n):return dict(type='collect_tile',tileType=t,target=n)
def clear(t,n):return dict(type='clear_obstacle',obstacleType=t,target=n)
def feather(n):return dict(type='bring_down',itemType='firebird_feather',target=n)
def obs(t,coords,layers=1):return [dict(x=x,y=y,type=t,layers=layers) for x,y in coords]
# Five rows of content parameters are deliberately different from the previous sector:
# alternating shelves, separated locks, stepped crates, choice of exits, and a mixed final route.
rows=[
(51,'Утро у колодца','Morning at the Well','recovery',34,5,[collect(1,16),collect(2,16)],[],[], 'Open recovery after the beacon; two modest color goals.'),
(52,'Ледяные уступы','Icy Ledges','normal',29,5,[clear('ice',8),collect(1,20)],obs('ice',[(x,2) for x in (0,2,4,6)]+[(x,4) for x in (1,3,5,7)]),[], 'Alternating ice shelves leave a free neighboring cell on each side.'),
(53,'Две задвижки','Two Latches','normal',29,5,[clear('chain',6),feather(2)],obs('chain',[(x,y) for x in (2,5) for y in (2,4,5)]),[2,5], 'Two feather lanes; open separated locks before the delivery.'),
(54,'Ступени к воде','Steps to the Water','normal',28,6,[clear('crate',9),collect(2,20)],obs('crate',[(1,1),(2,1),(3,1),(3,3),(4,3),(5,3),(5,5),(6,5),(7,5)]),[], 'Three short offset crate shelves; work from open sides instead of a solid wall.'),
(55,'Нижний затвор','The Lower Gate','hard',27,6,[clear('chain',4),clear('ice',6),feather(2)],obs('chain',[(2,4),(2,5),(5,4),(5,5)],2)+obs('ice',[(x,y) for x in (1,3,6) for y in (2,3)]),[2,5], 'Two-layer lower locks require repeated hits; ice occupies adjacent support lanes.'),
(56,'Тёплый двор','A Warm Courtyard','recovery',34,5,[collect(0,16),collect(3,16)],[],[], 'Open recovery: reset attention before the second set of route puzzles.'),
(57,'Сберечь тепло','Keep the Warmth','normal',27,5,[dict(type='survival',target=16),clear('ice',6)],obs('ice',[(x,3) for x in (0,1,3,4,6,7)]),[], 'Existing move-count survival, never a timer; two gaps through a shallow ice band.'),
(58,'Проход между дворами','Between the Courtyards','normal',29,6,[clear('crate',8),clear('chain',6)],obs('crate',[(x,y) for x in (1,2) for y in (1,2,4,5)])+obs('chain',[(x,y) for x in (5,6) for y in (2,3,4)]),[], 'Crates on the left and locks on the right create two different clearing tasks.'),
(59,'Свет над водой','Light over the Water','normal',29,5,[feather(2),clear('ice',6)],obs('ice',[(x,y) for x in (1,6) for y in (2,4,5)]),[1,6], 'Wide feather lanes with separated ice; choose the more accessible route first.'),
(60,'Открытый колодец','The Open Well','hard',28,6,[clear('crate',8),clear('chain',6),feather(2)],obs('crate',[(x,y) for x in (0,2,5,7) for y in (2,4)])+obs('chain',[(x,y) for x in (3,4) for y in (2,4,5)]),[3,4], 'Central delivery locks with side crate islands; combine both learned route tasks.')]
briefs=[]
for n,ru,en,difficulty,moves,colors,goals,obstacles,lanes,intent in rows:
 reward=dict(gold=160 if difficulty=='recovery' else 220 if difficulty=='hard' else 180,gems=3 if difficulty=='hard' else 0,boosterMace=0,boosterFirebird=1 if n==60 else 0,boosterLivingLight=0,boosterSeaPearl=0)
 l=dict(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=ru,columns=8,rows=7,tileTypes=colors,moveLimit=moves,startingScore=0,boardSeed=6100+n,difficulty=difficulty,difficultyRating={'recovery':2,'normal':4,'hard':6}[difficulty],goals=goals,fixedCells=[],obstacles=obstacles,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],rewards=reward,starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l);briefs.append(dict(round=n,nameRu=ru,nameEn=en,intent=intent,difficulty=difficulty,moves=moves,colors=colors,goals=goals,reward=reward))
scene='campaign.chapter01.well_path';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=60,sceneId=scene,major=True))
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='К колодцу снова можно подойти. Теперь поможем домам у дальней улицы сохранить тепло.' if lang=='ru' else 'We can reach the well again. Now let us help the houses on the far street keep their warmth.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for outcome in ['victory','failure']:
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}'
   entries[key]=('Путь стал свободнее. Продолжим помогать деревне.' if lang=='ru' else 'The way is clearer. Let us keep helping the village.') if outcome=='victory' else ('Сначала освобожу проход, затем вернусь к остальным целям.' if lang=='ru' else 'I will open a passage first, then return to the other goals.')
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Authored rounds; difficulty provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed 60 rounds; first 50 preserved.')
