from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector02R18';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==60

def collect(t,n):return dict(type='collect_tile',tileType=t,target=n)
def clear(t,n):return dict(type='clear_obstacle',obstacleType=t,target=n)
def feather(n):return dict(type='bring_down',itemType='firebird_feather',target=n)
def obs(t,coords,layers=1):return [dict(x=x,y=y,type=t,layers=layers) for x,y in coords]
# Accessible side routes and short layered barriers; no new rules or art.
rows=[
(61,'Край дальней улицы','The Far Street','recovery',34,5,[clear('ice',4),collect(2,16)],obs('ice',[(1,1),(6,1),(1,5),(6,5)]),[], 'Corner ice and modest green collection provide recovery after60.'),
(62,'Два низких забора','Two Low Fences','normal',29,5,[clear('crate',8),collect(0,20)],obs('crate',[(0,2),(3,2),(4,4),(7,4)])+obs('crate',[(1,2),(2,2),(5,4),(6,4)],2),[], 'Two offset short shelves with layered inner crates and accessible ends.'),
(63,'Боковые калитки','Side Gates','normal',29,5,[clear('chain',6),feather(2)],obs('chain',[(1,2),(1,4),(2,4),(6,3),(6,5),(5,5)]),[1,6], 'Choose a side lane and clear staggered lower gates for two deliveries.'),
(64,'Незамёрзший проход','The Unfrozen Passage','normal',28,6,[clear('ice',10),collect(1,20)],obs('ice',[(x,y) for x in (0,1) for y in (1,3)]+[(x,y) for x in (6,7) for y in (2,4)]+[(2,5),(5,5)]),[], 'Side ice wings leave central space for making specials.'),
(65,'Три порога','Three Doorsteps','hard',28,6,[clear('crate',9),feather(3)],obs('crate',[(1,2),(1,3),(3,1),(3,3),(6,2),(6,4)])+obs('crate',[(1,5),(3,5),(6,5)],2),[1,3,6], 'Three staggered delivery lanes each end in a two-hit crate gate.'),
(66,'Свет в окнах','Light in the Windows','recovery',34,5,[collect(1,16),collect(3,16)],[],[], 'Open five-color recovery with two modest targets.'),
(67,'Долгое тепло','Lasting Warmth','normal',27,5,[dict(type='survival',target=16),clear('chain',6)],obs('chain',[(1,2),(3,2),(5,2),(2,4),(4,4),(6,4)]),[], 'Six alternating locks; survive16 consumed moves, never a real-time countdown.'),
(68,'Два поленника','Two Woodpiles','normal',29,6,[clear('crate',6),clear('ice',6)],obs('crate',[(1,2),(2,2),(1,3),(5,3),(6,3),(6,4)],2)+obs('ice',[(3,1),(4,1),(3,3),(4,3),(3,5),(4,5)]),[], 'Small layered crate islands with clearable ice between, rather than full walls.'),
(69,'Искры у ворот','Sparks at the Gate','normal',29,5,[clear('chain',4),feather(2),collect(0,20)],obs('chain',[(2,3),(2,5),(5,2),(5,5)],2),[2,5], 'Prepare two layered exit routes; finish red collection without more obstacles.'),
(70,'Тепло дальней улицы','Warmth on the Far Street','hard',28,6,[clear('crate',10),clear('ice',6),feather(2)],obs('crate',[(x,2) for x in (0,1,2,3)]+[(x,4) for x in (4,5,6,7)]+[(2,5),(5,5)])+obs('ice',[(1,1),(3,1),(4,3),(6,3),(1,5),(6,5)]),[2,5], 'Two offset shelf routes and side ice; apply accessible-end clearing to two deliveries.')]
briefs=[]
for n,ru,en,difficulty,moves,colors,goals,obstacles,lanes,intent in rows:
 reward=dict(gold=160 if difficulty=='recovery' else 220 if difficulty=='hard' else 180,gems=3 if difficulty=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=1 if n==70 else 0,boosterSeaPearl=0)
 l=dict(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=ru,columns=8,rows=7,tileTypes=colors,moveLimit=moves,startingScore=0,boardSeed=7100+n,difficulty=difficulty,difficultyRating={'recovery':2,'normal':4,'hard':6}[difficulty],goals=goals,fixedCells=[],obstacles=obstacles,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],rewards=reward,starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l);briefs.append(dict(round=n,nameRu=ru,nameEn=en,intent=intent,difficulty=difficulty,moves=moves,colors=colors,goals=goals,reward=reward))
scene='campaign.chapter01.far_street';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=70,sceneId=scene,major=True))
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='В домах на дальней улице снова тепло. Теперь расчистим проходы между дворами, чтобы соседи могли помогать друг другу.' if lang=='ru' else 'The houses on the far street are warm again. Now let us clear the paths between the courtyards so the neighbors can help one another.'
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
print('Installed 70 rounds; first 60 preserved.')
