from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(131,'От склада к домам','From the Storehouse to the Homes','recovery',34,5,[clear('crate',4),collect(1,16)],obs('crate',[(0,1),(2,5),(5,1),(7,4)]),[], 'Four isolated crates and blue collection provide recovery after130.'),
(132,'Левая тропа','The Left Path','normal',29,5,[clear('ice',8),feather(1)],obs('ice',[(1,2),(1,5),(4,1),(5,1),(6,2),(4,3),(5,4),(6,5)]),[1], 'Single left delivery route competes with a separate right-side ice task.'),
(133,'Ящики у поворота','Crates at the Bend','normal',29,5,[clear('crate',6),collect(2,22)],obs('crate',[(x,y) for x in (0,1,2) for y in (2,4)],2),[], 'Two short layered shelves on the left leave the right half open for special creation.'),
(134,'Разные дороги','Different Roads','normal',29,5,[clear('chain',8),collect(1,22)],obs('chain',[(1,1),(2,2),(3,3),(1,5),(5,1),(6,2),(5,4),(7,5)]),[], 'Uneven lock groups and blue collection let the player choose an approach.'),
(135,'Два обхода','Two Detours','hard',28,6,[clear('crate',8),clear('ice',6),feather(2)],obs('crate',[(0,2),(0,4),(5,3),(5,5)],2)+obs('crate',[(1,3),(2,3),(6,1),(7,1)])+obs('ice',[(2,1),(3,2),(4,1),(2,5),(4,4),(7,4)]),[0,5], 'Offset layered delivery gates plus independent ice patches form the first peak.'),
(136,'Тихая тропинка','A Quiet Path','recovery',34,5,[collect(2,16),collect(3,16)],[],[], 'Open green/purple recovery after135.'),
(137,'Искры над развилкой','Sparks above the Fork','normal',29,6,[collect(0,24),collect(1,24),clear('ice',4)],obs('ice',[(1,1),(5,2),(2,4),(6,5)],2),[], 'Four scattered two-hit ice cells in open six-color special-making space.'),
(138,'Три поворота','Three Turns','normal',29,5,[clear('chain',6),clear('crate',4),feather(3)],obs('chain',[(1,2),(3,3),(7,1)])+obs('chain',[(1,5),(3,5),(7,4)],2)+obs('crate',[(0,3),(2,1),(4,4),(6,2)]),[1,3,7], 'Three uneven delivery routes with staggered gates and a small separate crate task.'),
(139,'Сберечь проходы','Keep the Passages Clear','normal',28,5,[dict(type='survival',target=18),clear('ice',6),clear('chain',4)],obs('ice',[(0,1),(2,2),(4,3),(6,4),(1,4),(5,1)])+obs('chain',[(1,2),(3,5),(5,4),(7,2)]),[], 'Scattered ice and locks during18 consumed moves; no real-time timer.'),
(140,'Тропы к домам','Paths to the Homes','hard',30,6,[clear('crate',8),clear('chain',6),feather(2)],obs('crate',[(x,y) for x in (1,2,3) for y in (2,4)])+obs('crate',[(2,5),(7,5)],2)+obs('chain',[(0,1),(4,2),(5,1),(6,3),(7,2),(5,5)]),[2,7], 'Left crate shelves and right locks separate two uneven delivery routes at the checkpoint.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=int(n==140),boosterLivingLight=0,boosterSeaPearl=0)
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=14000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
for b in briefs:
 positions=[(o['x'],o['y']) for o in b['obstacles']];assert len(positions)==len(set(positions))
 assert all(0<=x<8 and 0<=y<7 for x,y in positions)
 assert all((i['x'],0) not in positions and (i['x'],6) not in positions for i in b['boardItems'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=b['moveLimit']
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored provision-path block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten provision-path briefs structurally checked.')
