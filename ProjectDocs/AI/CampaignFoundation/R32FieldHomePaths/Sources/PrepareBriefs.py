from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(181,'Тропа к домам','The Path to the Homes','recovery',34,5,[clear('ice',4),collect(4,16)],obs('ice',[(0,2),(2,5),(4,1),(7,3)]),[],'Four separated single ice cells and modest yellow collection after180.'),
(182,'Левая и правая дорога','The Left and Right Roads','normal',29,5,[clear('crate',6),clear('ice',4)],obs('crate',[(0,1),(1,1),(1,2),(2,3),(0,4),(1,5)],2)+obs('ice',[(5,1),(7,2),(5,4),(6,5)]),[],'Layered crates on the left and scattered ice on the right leave a central workspace and a choice of first task.'),
(183,'Перья между дорогами','Feathers between the Roads','normal',29,5,[clear('chain',4),clear('ice',6),feather(2)],obs('chain',[(3,2),(5,3)])+obs('chain',[(3,5),(5,5)],2)+obs('ice',[(0,1),(1,3),(2,4),(6,1),(7,3),(7,5)]),[3,5],'Two inner locked deliveries contrast with outer ice, keeping both sides useful.'),
(184,'Открытая поляна','The Open Clearing','normal',29,6,[collect(0,24),collect(1,24)],[],[],'Open six-color red/blue collection between layered-gate tasks.'),
(185,'Три дороги к домам','Three Roads to the Homes','hard',28,6,[clear('crate',6),clear('chain',6),feather(3)],obs('crate',[(0,2),(4,3),(7,1)])+obs('crate',[(0,5),(4,5),(7,5)],2)+obs('chain',[(1,1),(2,3),(3,4),(5,1),(6,3),(2,5)]),[0,4,7],'Unequal delivery lanes with strong lower crates and off-route locks; no solid wall.'),
(186,'Тихая дорога','The Quiet Road','recovery',34,5,[collect(1,16),collect(3,16)],[],[],'Open blue/purple recovery after185.'),
(187,'Цепи у развилки','Chains at the Fork','normal',29,5,[clear('chain',8),clear('crate',4)],obs('chain',[(0,1),(7,2),(1,5),(6,4)])+obs('chain',[(2,2),(3,3),(4,4),(5,5)],2)+obs('crate',[(1,2),(6,1),(2,4),(5,3)]),[],'Four strong diagonal locks, four weaker outer locks and separated crates allow alternative clearing priorities.'),
(188,'Сохранить связь дорог','Keep the Roads Connected','normal',28,5,[dict(type='survival',target=18),clear('ice',8)],obs('ice',[(0,1),(2,2),(5,3),(7,4)])+obs('ice',[(1,4),(3,5),(4,1),(6,2)],2),[],'Eighteen consumed moves with staggered single/double ice; no clock or timed progress.'),
(189,'Боковые выходы','Side Exits','normal',29,5,[clear('ice',4),clear('crate',6),feather(2)],obs('ice',[(1,2),(6,3)])+obs('ice',[(1,5),(6,5)],2)+obs('crate',[(0,3),(2,1),(3,3),(4,4),(5,2),(7,4)]),[1,6],'Two separated ice delivery routes and offset crates support more than one route order.'),
(190,'Поля и дома','Fields and Homes','hard',30,6,[clear('chain',6),clear('crate',8),feather(3)],obs('chain',[(2,2),(5,3),(7,1)])+obs('chain',[(2,5),(5,5),(7,5)],2)+obs('crate',[(0,1),(1,3),(3,1),(4,2),(6,3),(0,4),(3,4),(6,5)]),[2,5,7],'Three unevenly spaced locked delivery lanes and side crates reconnect field-to-home paths; remaining bypass checks follow.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=int(n==190),boosterSeaPearl=0)
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=19000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
assert [b['round'] for b in briefs]==list(range(181,191))
for b in briefs:
 pos=[(o['x'],o['y']) for o in b['obstacles']];assert len(pos)==len(set(pos));assert all(0<=x<8 and 0<y<6 for x,y in pos);assert all(o['layers'] in (1,2) for o in b['obstacles'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=b['moveLimit']
  if g['type']=='collect_tile':assert 0<=g['tileType']<b['tileTypes']
 assert all((i['x'],0) not in pos and (i['x'],6) not in pos for i in b['boardItems'])
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored field-to-home block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten field-to-home briefs structurally checked.')
