from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(121,'Тропа к складу','Path to the Storehouse','recovery',34,5,[collect(1,16),collect(2,16)],[],[], 'Open blue/green recovery after120.'),
(122,'Лёд на тропинках','Ice on the Paths','normal',29,5,[clear('ice',8),collect(2,20)],obs('ice',[(0,2),(2,1),(4,2),(6,1),(1,4),(3,5),(5,4),(7,5)]),[], 'Scattered single-layer ice contrasts with the previous central shelves.'),
(123,'Нижние заслоны','Lower Barriers','normal',29,5,[clear('crate',4),clear('ice',4),feather(2)],obs('crate',[(2,4),(2,5),(6,4),(6,5)],2)+obs('ice',[(1,1),(4,2),(5,3),(7,1)]),[2,6], 'Two paired layered lower gates and scattered support ice; open upper working space.'),
(124,'Цепи у склада','Chains by the Storehouse','normal',29,5,[clear('chain',6),collect(0,20)],obs('chain',[(1,1),(4,1),(6,2),(2,4),(4,5),(7,4)]),[], 'Separated locks across both halves encourage choices while collecting red.'),
(125,'Три пути к припасам','Three Paths to Supplies','hard',28,6,[clear('chain',6),clear('crate',6),feather(3)],obs('chain',[(x,2) for x in (0,3,7)])+obs('chain',[(x,5) for x in (0,3,7)],2)+obs('crate',[(1,1),(2,3),(4,1),(5,3),(6,2),(6,4)]),[0,3,7], 'Three asymmetrically spaced deliveries with stronger lower locks and separate crates.'),
(126,'Передышка у крыльца','A Rest by the Porch','recovery',34,5,[clear('ice',4),collect(3,16)],obs('ice',[(0,1),(7,2),(0,4),(7,5)]),[], 'Four side ice cells and a modest purple target after125.'),
(127,'Самоцветы у порога','Gems by the Threshold','normal',29,6,[collect(0,24),collect(2,24)],[],[], 'Open six-color board offers space for deliberate special creation.'),
(128,'Крепкие нижние ящики','Sturdy Lower Crates','normal',29,5,[clear('crate',8),collect(1,22)],obs('crate',[(x,y) for x in (1,2,5,6) for y in (3,5)],2),[], 'Four short two-hit lower stacks leave centre and upper rows open.'),
(129,'Ледяные проходы','Icy Passages','normal',29,5,[clear('ice',6),clear('chain',4),feather(2)],obs('ice',[(1,1),(1,3),(5,2),(5,4)])+obs('ice',[(1,5),(5,5)],2)+obs('chain',[(2,2),(3,4),(6,1),(7,3)]),[1,5], 'Offset ice delivery lanes strengthen near the bottom; separate locks remain a second task.'),
(130,'Дорога к припасам','The Road to Supplies','hard',30,6,[clear('crate',6),clear('ice',8),feather(2)],obs('crate',[(2,5),(5,5)],2)+obs('crate',[(1,4),(3,4),(4,2),(6,2)])+obs('ice',[(0,1),(7,1),(1,2),(6,4),(3,1),(4,5),(0,4),(7,5)]),[2,5], 'Mixed checkpoint combines lower layered delivery gates with scattered crate and ice targets.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=int(n==130),boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0)
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=13000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
for b in briefs:
 positions=[(o['x'],o['y']) for o in b['obstacles']];assert len(positions)==len(set(positions))
 assert all(0<=x<8 and 0<=y<7 for x,y in positions)
 assert all((i['x'],0) not in positions and (i['x'],6) not in positions for i in b['boardItems'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored storehouse block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten storehouse briefs structurally checked.')
