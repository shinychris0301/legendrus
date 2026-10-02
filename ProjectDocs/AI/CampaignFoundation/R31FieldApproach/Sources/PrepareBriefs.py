from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(171,'За садовой оградой','Beyond the Orchard Fence','recovery',34,5,[clear('chain',4),collect(3,16)],obs('chain',[(1,1),(3,4),(5,2),(7,5)]),[],'Four separated single locks and modest purple collection after170.'),
(172,'Группы ящиков','Crate Groups','normal',29,5,[clear('crate',6),collect(1,20)],obs('crate',[(0,2),(1,2),(3,4),(4,4),(6,1),(7,1)],2),[],'Three separated pairs of layered crates leave broad open approaches for special creation.'),
(173,'Два пути к полям','Two Ways to the Fields','normal',29,5,[clear('ice',4),clear('crate',6),feather(2)],obs('ice',[(0,2),(6,3)])+obs('ice',[(0,5),(6,5)],2)+obs('crate',[(2,1),(3,2),(4,3),(2,4),(4,5),(7,2)]),[0,6],'Unequal outer ice delivery lanes contrast with a separate crate task.'),
(174,'Самоцветы у межи','Gems by the Field Boundary','normal',29,6,[collect(2,24),collect(4,24)],[],[],'Open six-color green/yellow collection gives space between layered-gate tasks.'),
(175,'Поворот к полям','The Turn toward the Fields','hard',28,6,[clear('chain',6),clear('crate',8),feather(3)],obs('chain',[(2,2),(3,3),(7,1)])+obs('chain',[(2,5),(3,5),(7,5)],2)+obs('crate',[(0,1),(1,3),(4,1),(5,2),(6,3),(0,4),(4,4),(6,5)]),[2,3,7],'Two nearby delivery lanes and a distant third share stronger lower locks; side crates remain a separate priority.'),
(176,'Утро над полем','Morning over the Field','recovery',34,5,[collect(0,16),collect(3,16)],[],[],'Open red/purple recovery after175; no crop collection is implied.'),
(177,'Холодная межа','The Frozen Boundary','normal',29,5,[clear('ice',8),clear('chain',4)],obs('ice',[(0,1),(7,1),(1,5),(6,5)])+obs('ice',[(2,2),(5,2),(2,4),(5,4)],2)+obs('chain',[(3,1),(4,3),(0,4),(7,4)]),[],'Inner double ice, outer single ice and separate locks support a choice of first target.'),
(178,'Сохранить дорогу к полю','Keep the Field Road Open','normal',28,5,[dict(type='survival',target=18),clear('crate',6),clear('ice',4)],obs('crate',[(1,1),(2,2),(3,3),(5,3),(6,4),(7,5)])+obs('ice',[(0,3),(3,1),(4,5),(7,2)]),[],'Eighteen consumed moves with two scattered obstacle tasks; no timer and no full-width wall.'),
(179,'Перо у межи','A Feather by the Boundary','normal',29,5,[clear('chain',2),clear('ice',6),feather(1)],obs('chain',[(3,2)])+obs('chain',[(3,5)],2)+obs('ice',[(0,1),(1,3),(5,1),(6,3)])+obs('ice',[(1,5),(6,5)],2),[3],'One middle delivery with a strong lower lock and separate layered ice at both sides.'),
(180,'Выход к полям','The Way to the Fields','hard',30,6,[clear('crate',6),clear('chain',6),feather(3)],obs('crate',[(1,2),(4,3),(6,1)])+obs('crate',[(1,5),(4,5),(6,5)],2)+obs('chain',[(0,3),(2,1),(3,4),(5,2),(7,3),(5,5)]),[1,4,6],'Three unequal crate delivery routes and off-route locks establish access to the fields without claiming all paths complete.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=int(n==180),boosterLivingLight=0,boosterSeaPearl=0)
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=18000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
assert [b['round'] for b in briefs]==list(range(171,181))
for b in briefs:
 pos=[(o['x'],o['y']) for o in b['obstacles']];assert len(pos)==len(set(pos));assert all(0<=x<8 and 0<y<6 for x,y in pos);assert all(o['layers'] in (1,2) for o in b['obstacles'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=b['moveLimit']
  if g['type']=='collect_tile':assert 0<=g['tileType']<b['tileTypes']
 assert all((i['x'],0) not in pos and (i['x'],6) not in pos for i in b['boardItems'])
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored field-approach block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten field-approach briefs structurally checked.')
