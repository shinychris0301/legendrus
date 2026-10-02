from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector02R20';out.mkdir(exist_ok=True)
def read(p):return json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==80

def collect(t,n):return dict(type='collect_tile',tileType=t,target=n)
def clear(t,n):return dict(type='clear_obstacle',obstacleType=t,target=n)
def feather(n):return dict(type='bring_down',itemType='firebird_feather',target=n)
def obs(t,coords,layers=1):return [dict(x=x,y=y,type=t,layers=layers) for x,y in coords]
# Delivery exit spacing and layered ice; existing rules and art only.
rows=[
(81,'У домашнего огня','By the Hearth','recovery',34,5,[collect(0,16),collect(1,16)],[],[], 'Open red/blue recovery after80.'),
(82,'Лёд у крыльца','Ice by the Porch','normal',29,5,[clear('ice',8),collect(2,20)],obs('ice',[(0,1),(1,1),(2,1),(5,3),(6,3),(7,3),(1,5),(6,5)]),[], 'Offset side ice shelves leave a working center.'),
(83,'Узкая тропа','The Narrow Path','normal',29,5,[clear('ice',4),clear('crate',6),feather(1)],obs('ice',[(3,y) for y in (1,2,4,5)],2)+obs('crate',[(1,2),(1,3),(1,4),(5,1),(5,2),(5,3)]),[3], 'One central delivery lane through layered ice, with side crate tasks.'),
(84,'Засовы у домов','Household Bolts','normal',28,6,[clear('chain',8),collect(3,20)],obs('chain',[(1,1),(1,2),(2,3),(2,4),(5,2),(5,3),(6,4),(6,5)]),[], 'Two offset vertical lock groups leave the middle open.'),
(85,'Три дороги к очагам','Three Hearth Routes','hard',28,6,[clear('ice',9),feather(3)],obs('ice',[(x,y) for x in (0,3,7) for y in (2,3)])+obs('ice',[(x,5) for x in (0,3,7)],2),[0,3,7], 'Widely spaced edge/center deliveries each have a layered lower ice gate.'),
(86,'Тихий переулок','A Quiet Lane','recovery',34,5,[clear('chain',4),collect(1,16)],obs('chain',[(1,2),(6,2),(1,4),(6,4)]),[], 'Four isolated locks and an open center provide recovery.'),
(87,'Дрова у порога','Wood at the Doorstep','normal',29,5,[clear('crate',10),collect(0,22)],obs('crate',[(0,1),(1,1),(2,1),(3,3),(4,3),(5,3),(5,5),(6,5),(7,5),(2,4)]),[], 'Short staggered shelves can be approached from multiple sides.'),
(88,'Лёд и засовы','Ice and Bolts','normal',29,6,[clear('chain',6),clear('ice',6)],obs('chain',[(x,y) for x in (1,2) for y in (1,3,5)])+obs('ice',[(x,y) for x in (5,6) for y in (2,3)])+obs('ice',[(5,5),(6,5)],2),[], 'Separated chain and ice tasks contrast damage behavior without blocking the center.'),
(89,'Обходной путь','The Side Route','normal',29,5,[clear('crate',8),feather(2)],obs('crate',[(1,2),(2,2),(1,3),(4,3),(5,3),(5,4)])+obs('crate',[(1,5),(5,5)],2),[1,5], 'Asymmetric two-lane delivery with short shelves and layered bottom gates.'),
(90,'Негаснущие очаги','Steady Hearths','hard',28,6,[clear('ice',8),clear('chain',6),feather(2)],obs('ice',[(0,1),(1,1),(5,1),(6,1),(0,4),(1,4),(5,4),(6,4)])+obs('chain',[(x,y) for x in (2,3) for y in (2,3,5)]),[2,3], 'Adjacent delivery routes through locks with separate side ice zones.')]
briefs=[]
for n,ru,en,difficulty,moves,colors,goals,obstacles,lanes,intent in rows:
 reward=dict(gold=160 if difficulty=='recovery' else 220 if difficulty=='hard' else 180,gems=3 if difficulty=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=1 if n==90 else 0)
 l=dict(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=ru,columns=8,rows=7,tileTypes=colors,moveLimit=moves,startingScore=0,boardSeed=9100+n,difficulty=difficulty,difficultyRating={'recovery':2,'normal':4,'hard':6}[difficulty],goals=goals,fixedCells=[],obstacles=obstacles,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],rewards=reward,starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l);briefs.append(dict(round=n,nameRu=ru,nameEn=en,intent=intent,difficulty=difficulty,moves=moves,colors=colors,goals=goals,reward=reward))
scene='campaign.chapter01.hearth_routes';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=90,sceneId=scene,major=True))
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Дороги к очагам свободны, и огни не гаснут. Осталось соединить их с колодцем, чтобы вся улица могла держаться вместе.' if lang=='ru' else 'The paths to the hearths are clear, and the fires are holding. We must connect them to the well so the whole street can stand together.'
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
print('Installed 90 rounds; first 80 preserved.')
