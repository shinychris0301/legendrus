from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector02R19';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==70

def collect(t,n):return dict(type='collect_tile',tileType=t,target=n)
def clear(t,n):return dict(type='clear_obstacle',obstacleType=t,target=n)
def feather(n):return dict(type='bring_down',itemType='firebird_feather',target=n)
def obs(t,coords,layers=1):return [dict(x=x,y=y,type=t,layers=layers) for x,y in coords]
# Two small clearing zones with open space to form specials; existing mechanics only.
rows=[
(71,'Соседний двор','The Neighboring Courtyard','recovery',34,5,[collect(2,16),collect(3,16)],[],[], 'Open recovery after70 with modest green and purple goals.'),
(72,'Навстречу соседям','Meeting the Neighbors','normal',29,5,[clear('crate',10),collect(0,20)],obs('crate',[(1,2),(2,2),(1,3),(1,4),(2,4),(5,2),(6,2),(6,3),(5,4),(6,4)]),[], 'Facing three-sided crate groups leave their inner approaches open.'),
(73,'Двойная наледь','Thick Ice','normal',29,5,[clear('ice',6),collect(1,18)],obs('ice',[(1,1),(2,1),(2,2),(5,4),(5,5),(6,5)],2),[], 'Two offset ice groups need repeated damage; count cleared cells, not layers.'),
(74,'Замки на тропинках','Locks on the Paths','normal',28,6,[clear('chain',8),collect(0,20)],obs('chain',[(0,1),(1,2),(2,3),(3,4),(4,2),(5,3),(6,4),(7,5)]),[], 'Two broken diagonal lock paths with free neighboring cells.'),
(75,'Две запертые калитки','Two Locked Gates','hard',28,6,[clear('crate',6),clear('chain',4),feather(2)],obs('crate',[(1,2),(2,2),(3,2),(4,3),(5,3),(6,3)])+obs('chain',[(2,4),(5,4)])+obs('chain',[(2,5),(5,5)],2),[2,5], 'Short crate shelves followed by two-layer lower locks along two delivery routes.'),
(76,'Солнечный уголок','A Sunny Corner','recovery',34,5,[clear('ice',4),collect(0,16)],obs('ice',[(0,2),(0,4),(7,2),(7,4)]),[], 'Four side ice cells leave an open center for recovery.'),
(77,'Место для искр','Room for Sparks','normal',29,5,[collect(2,24),collect(0,24)],[],[], 'Open board leaves room to make specials while collecting two colors.'),
(78,'Три преграды','Three Barriers','normal',29,6,[clear('crate',4),clear('chain',4),clear('ice',4)],obs('crate',[(1,1),(2,1),(1,2),(2,2)])+obs('chain',[(5,1),(6,1),(5,2),(6,2)])+obs('ice',[(2,4),(3,4),(4,4),(5,4)]),[], 'Three small zones contrast existing crate, chain and ice damage rules.'),
(79,'Три светлые тропы','Three Bright Paths','normal',29,5,[clear('ice',6),feather(3)],obs('ice',[(1,2),(1,5),(3,3),(3,5),(6,2),(6,4)]),[1,3,6], 'Choose among three staggered delivery paths with two ice cells each.'),
(80,'Открытые дворы','Open Courtyards','hard',28,6,[clear('crate',8),clear('chain',6),feather(2)],obs('crate',[(1,1),(2,1),(1,4),(2,4),(5,2),(6,2),(5,5),(6,5)])+obs('chain',[(3,2),(3,3),(3,5),(4,1),(4,3),(4,4)]),[3,4], 'Side crate courtyards and central delivery locks connect both clearing zones.')]
briefs=[]
for n,ru,en,difficulty,moves,colors,goals,obstacles,lanes,intent in rows:
 reward=dict(gold=160 if difficulty=='recovery' else 220 if difficulty=='hard' else 180,gems=3 if difficulty=='hard' else 0,boosterMace=1 if n==80 else 0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0)
 l=dict(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=ru,columns=8,rows=7,tileTypes=colors,moveLimit=moves,startingScore=0,boardSeed=8100+n,difficulty=difficulty,difficultyRating={'recovery':2,'normal':4,'hard':6}[difficulty],goals=goals,fixedCells=[],obstacles=obstacles,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],rewards=reward,starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l);briefs.append(dict(round=n,nameRu=ru,nameEn=en,intent=intent,difficulty=difficulty,moves=moves,colors=colors,goals=goals,reward=reward))
scene='campaign.chapter01.courtyard_paths';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=80,sceneId=scene,major=True))
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Проходы между дворами открыты. Соседи снова могут помогать друг другу. Теперь сбережём пути к очагам, пока тьма не отступит.' if lang=='ru' else 'The paths between the courtyards are open. The neighbors can help one another again. Now we must keep the hearth routes clear until the darkness recedes.'
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
print('Installed 80 rounds; first 70 preserved.')
