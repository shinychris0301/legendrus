from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(141,'Утро у мельницы','Morning by the Mill','recovery',34,5,[clear('ice',4),collect(2,16)],obs('ice',[(0,2),(2,5),(5,1),(7,3)]),[], 'Four isolated ice cells and modest green collection after140.'),
(142,'Соединить тропы','Join the Paths','normal',29,5,[clear('crate',8),clear('ice',4)],obs('crate',[(1,1),(2,1),(1,2),(1,3),(6,3),(6,4),(5,5),(6,5)])+obs('ice',[(3,2),(4,1),(3,5),(4,4)]),[], 'Two offset short crate bends and separate inner ice leave the centre open.'),
(143,'Средний проход','The Middle Passage','normal',29,5,[clear('chain',2),clear('crate',6),feather(1)],obs('chain',[(4,2)])+obs('chain',[(4,5)],2)+obs('crate',[(0,2),(1,2),(2,3),(5,1),(6,3),(7,4)]),[4], 'One central locked delivery contrasts with separate crate groups on both sides.'),
(144,'Самоцветы над дорогой','Gems above the Road','normal',29,6,[collect(1,24),collect(3,24)],[],[], 'Open six-color blue/purple collection for voluntary special creation.'),
(145,'Три дороги к мельнице','Three Roads to the Mill','hard',28,6,[clear('ice',6),clear('crate',8),feather(3)],obs('ice',[(x,2) for x in (1,4,6)])+obs('ice',[(x,5) for x in (1,4,6)],2)+obs('crate',[(0,1),(2,1),(3,3),(5,3),(7,1),(0,4),(2,4),(7,4)]),[1,4,6], 'Three ice delivery routes with layered lower gates and separate crate tasks.'),
(146,'Передышка на дороге','A Rest on the Road','recovery',34,5,[collect(0,16),collect(1,16)],[],[], 'Open red/blue recovery after145.'),
(147,'Последние цепи','The Remaining Chains','normal',29,5,[clear('chain',8),collect(2,22)],obs('chain',[(0,1),(7,2),(1,5),(6,4)])+obs('chain',[(3,2),(4,2),(3,4),(4,4)],2),[], 'Four stronger inner locks and four separated outer locks with green collection.'),
(148,'Сохранить дорогу','Keep the Road Clear','normal',28,5,[dict(type='survival',target=18),clear('crate',6)],obs('crate',[(1,2),(2,2),(5,3),(6,3),(2,5),(5,5)],2),[], 'Six layered crates during18 consumed moves; no elapsed-time timer.'),
(149,'Перья на развилке','Feathers at the Fork','normal',29,5,[clear('crate',6),clear('ice',6),feather(2)],obs('crate',[(0,2),(5,2),(1,3),(6,3)])+obs('crate',[(0,5),(5,5)],2)+obs('ice',[(2,1),(3,2),(4,4),(7,1),(2,5),(7,5)]),[0,5], 'Offset crate delivery gates and separate scattered ice reinforce route planning.'),
(150,'Мельница, склад и дома','Mill, Storehouse and Homes','hard',30,6,[clear('chain',6),clear('crate',8),feather(3)],obs('chain',[(x,2) for x in (1,3,6)])+obs('chain',[(x,5) for x in (1,3,6)],2)+obs('crate',[(0,1),(2,1),(4,1),(5,3),(7,3),(0,4),(2,4),(4,4)]),[1,3,6], 'Three staggered-width delivery routes with strong lower locks and side crates conclude sector3 locally.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=int(n==150),boosterSeaPearl=0)
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=15000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
for b in briefs:
 positions=[(o['x'],o['y']) for o in b['obstacles']];assert len(positions)==len(set(positions))
 assert all(0<=x<8 and 0<=y<7 for x,y in positions)
 assert all((i['x'],0) not in positions and (i['x'],6) not in positions for i in b['boardItems'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=b['moveLimit']
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored mill checkpoint block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten mill-checkpoint briefs structurally checked.')
