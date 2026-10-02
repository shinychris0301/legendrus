from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector02R21';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==90

def collect(t,n):return dict(type='collect_tile',tileType=t,target=n)
def clear(t,n):return dict(type='clear_obstacle',obstacleType=t,target=n)
def feather(n):return dict(type='bring_down',itemType='firebird_feather',target=n)
def obs(t,coords,layers=1):return [dict(x=x,y=y,type=t,layers=layers) for x,y in coords]
# Consolidate delivery, layers and open special-making space; existing mechanics only.
rows=[
(91,'Тихое утро','A Quiet Morning','recovery',34,5,[clear('ice',4),collect(3,16)],obs('ice',[(0,1),(7,1),(0,5),(7,5)]),[], 'Corner ice and modest purple collection provide recovery after90.'),
(92,'Подступы к колодцу','Approaches to the Well','normal',29,5,[clear('ice',8),clear('chain',4)],obs('ice',[(1,1),(2,1),(5,1),(6,1),(1,5),(2,5),(5,5),(6,5)])+obs('chain',[(3,2),(4,2),(3,4),(4,4)]),[], 'Separated ice approaches surround a small central lock task.'),
(93,'Две дороги к воде','Two Roads to Water','normal',29,5,[clear('crate',8),feather(2)],obs('crate',[(1,2),(2,2),(2,3),(5,3),(6,3),(6,4)])+obs('crate',[(2,5),(6,5)],2),[2,6], 'Offset delivery lanes with layered lower gates and short side shelves.'),
(94,'Искры над улицей','Sparks over the Street','normal',29,6,[collect(0,24),collect(2,24)],[],[], 'Open six-color board for player-created specials and two collection goals.'),
(95,'Три пути к свету','Three Paths to Light','hard',28,6,[clear('chain',6),clear('ice',6),feather(3)],obs('chain',[(x,2) for x in (0,4,7)])+obs('chain',[(x,5) for x in (0,4,7)],2)+obs('ice',[(1,1),(1,3),(3,3),(5,3),(6,1),(6,4)]),[0,4,7], 'Three locked delivery routes with support-cell ice and two-hit lower locks.'),
(96,'Передышка у воды','A Rest by the Water','recovery',34,5,[collect(2,16),collect(1,16)],[],[], 'Open green/blue recovery after95.'),
(97,'Лёд между поленницами','Ice between the Woodpiles','normal',29,5,[clear('crate',8),clear('ice',4)],obs('crate',[(0,1),(1,1),(6,1),(7,1),(0,5),(1,5),(6,5),(7,5)])+obs('ice',[(3,2),(4,2),(3,4),(4,4)],2),[], 'Small corner crate groups contrast with repeated hits on central ice.'),
(98,'Сберечь дороги','Keep the Roads Clear','normal',28,5,[dict(type='survival',target=18),clear('chain',6)],obs('chain',[(1,1),(3,2),(5,1),(2,4),(4,5),(6,4)]),[], 'Six alternating locks during18 consumed moves; no elapsed-time mechanic.'),
(99,'Последние искры','The Remaining Sparks','normal',29,5,[clear('ice',8),feather(2),collect(0,22)],obs('ice',[(1,1),(1,3),(1,5),(2,4),(6,2),(6,4),(6,5),(5,1)]),[1,6], 'Wide deliveries and a red target combine routes and collection.'),
(100,'Колодец и очаги','Well and Hearths','hard',30,6,[clear('crate',8),clear('chain',6),feather(3)],obs('crate',[(0,2),(2,2),(5,3),(7,3)])+obs('crate',[(0,4),(2,4),(5,5),(7,5)],2)+obs('chain',[(x,y) for x in (1,3,6) for y in (2,5)]),[1,3,6], 'Three delivery routes with chain gates and separate layered crate tasks close sector2.')]
briefs=[]
for n,ru,en,difficulty,moves,colors,goals,obstacles,lanes,intent in rows:
 reward=dict(gold=160 if difficulty=='recovery' else 220 if difficulty=='hard' else 180,gems=3 if difficulty=='hard' else 0,boosterMace=0,boosterFirebird=1 if n==100 else 0,boosterLivingLight=0,boosterSeaPearl=0)
 l=dict(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=ru,columns=8,rows=7,tileTypes=colors,moveLimit=moves,startingScore=0,boardSeed=10100+n,difficulty=difficulty,difficultyRating={'recovery':2,'normal':4,'hard':6}[difficulty],goals=goals,fixedCells=[],obstacles=obstacles,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],rewards=reward,starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l);briefs.append(dict(round=n,nameRu=ru,nameEn=en,intent=intent,difficulty=difficulty,moves=moves,colors=colors,goals=goals,reward=reward))
scene='campaign.chapter01.well_and_hearths';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=100,sceneId=scene,major=True))
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Колодец и очаги снова связаны свободными дорогами. Теперь доберёмся до мельницы: деревне нужны мука и припасы.' if lang=='ru' else 'Clear paths connect the well and hearths again. Now we must reach the mill: the village needs flour and supplies.'
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
print('Installed 100 rounds; first 90 preserved.')
