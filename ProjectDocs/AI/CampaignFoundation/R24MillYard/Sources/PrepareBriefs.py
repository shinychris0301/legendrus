from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(111,'У ворот мельницы','At the Mill Gate','recovery',34,5,[clear('ice',4),collect(1,16)],obs('ice',[(1,1),(6,1),(2,5),(5,5)]),[], 'Small separated ice patches and blue collection provide recovery after110.'),
(112,'Обходные тропы','Paths around the Yard','normal',29,5,[clear('ice',4),clear('crate',6),feather(2)],obs('ice',[(0,2),(0,4),(7,2),(7,4)])+obs('crate',[(3,2),(4,2),(2,3),(5,3),(3,4),(4,4)]),[0,7], 'Edge deliveries are separate from a hollow central crate group.'),
(113,'Крепкие ящики','Sturdy Crates','normal',29,5,[clear('crate',8),collect(0,20)],obs('crate',[(2,2),(3,2),(4,2),(5,2),(2,4),(3,4),(4,4),(5,4)],2),[], 'Two central layered shelves leave a working row between them and open sides.'),
(114,'Три прохода во дворе','Three Yard Passages','normal',29,5,[clear('chain',6),clear('ice',4),feather(3)],obs('chain',[(x,y) for x in (1,4,6) for y in (2,5)])+obs('ice',[(2,3),(3,3),(5,1),(5,4)]),[1,4,6], 'Three unevenly spaced locked routes with separate support ice.'),
(115,'Дальние ворота','The Far Gate','hard',28,6,[clear('crate',4),clear('ice',8),feather(2)],obs('crate',[(0,3),(0,5),(7,3),(7,5)],2)+obs('ice',[(2,1),(3,1),(4,1),(5,1),(2,4),(3,4),(4,4),(5,4)]),[0,7], 'Two-hit edge delivery gates and central ice require separate clearing priorities.'),
(116,'Тихий уголок','A Quiet Corner','recovery',34,5,[collect(2,16),collect(3,16)],[],[], 'Open green/purple collection after the first peak.'),
(117,'Искры над двором','Sparks over the Yard','normal',29,6,[collect(0,24),collect(1,24),clear('chain',4)],obs('chain',[(0,2),(7,2),(0,4),(7,4)]),[], 'Open central six-color space for special creation with four separate edge locks.'),
(118,'Очистить середину','Clear the Centre','normal',28,5,[dict(type='survival',target=18),clear('ice',6)],obs('ice',[(2,2),(3,2),(4,2),(3,4),(4,4),(5,4)],2),[], 'Six two-layer ice cells during18 consumed moves; no real-time survival timer.'),
(119,'Перья у ограды','Feathers by the Fence','normal',29,5,[clear('chain',4),clear('crate',6),feather(2)],obs('chain',[(1,2),(6,2)])+obs('chain',[(1,5),(6,5)],2)+obs('crate',[(3,1),(4,1),(2,3),(5,3),(3,5),(4,5)]),[1,6], 'Outer locks strengthen at the bottom, while a separate central crate task remains accessible.'),
(120,'Свободный мельничный двор','The Mill Yard Is Clear','hard',30,6,[clear('chain',4),clear('crate',8),feather(2)],obs('chain',[(0,2),(0,5),(7,2),(7,5)])+obs('crate',[(x,2) for x in (2,3,4,5)])+obs('crate',[(x,4) for x in (2,3,4,5)],2),[0,7], 'Checkpoint separates edge feather routes from central shelves with stronger lower crates.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=int(n==120))
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=12000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
for b in briefs:
 positions=[(o['x'],o['y']) for o in b['obstacles']];assert len(positions)==len(set(positions))
 assert all(0<=x<8 and 0<=y<7 for x,y in positions)
 assert all((i['x'],0) not in positions and (i['x'],6) not in positions for i in b['boardItems'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored mill-yard block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten mill-yard briefs structurally checked.')
