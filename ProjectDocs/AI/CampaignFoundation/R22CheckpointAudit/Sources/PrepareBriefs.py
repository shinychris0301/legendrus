from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
ob=lambda kind,coords,layers=1:[dict(x=x,y=y,type=kind,layers=layers) for x,y in coords]
collect=lambda color,n:dict(type='collect_tile',tileType=color,target=n)
clear=lambda kind,n:dict(type='clear_obstacle',obstacleType=kind,target=n)
delivery=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
rows=[
(101,'Тропа к мельнице','Path to the Mill','recovery',34,5,[collect(0,16),collect(3,16)],[],[], 'Open recovery after100.'),
(102,'Склад у дороги','Roadside Store','normal',29,5,[clear('crate',10),collect(2,20)],ob('crate',[(0,2),(1,2),(2,2),(2,3),(2,4),(5,2),(5,3),(5,4),(6,4),(7,4)]),[], 'Two offset stockpile gates with accessible ends.'),
(103,'Крепкие ворота','Sturdy Gates','normal',29,5,[clear('crate',6),collect(1,20)],ob('crate',[(1,2),(2,2),(2,3),(5,3),(5,4),(6,4)],2),[], 'Six layered crates contrast damage hits with cleared-cell goals.'),
(104,'Две тропы у мельницы','Two Mill Paths','normal',29,5,[clear('ice',6),delivery(2)],ob('ice',[(2,2),(2,5),(1,3),(5,3),(5,5),(6,2)]),[2,5], 'Separated delivery lanes with side support-cell ice.'),
(105,'Двор за воротами','The Yard beyond the Gate','hard',28,6,[clear('crate',8),clear('chain',4),delivery(2)],ob('crate',[(0,2),(1,2),(2,2),(3,2),(4,4),(5,4),(6,4),(7,4)])+ob('chain',[(1,3),(1,5),(6,2),(6,5)]),[1,6], 'Two deliveries through offset shelves and individual locks.'),
(106,'Передышка у мельницы','A Rest near the Mill','recovery',34,5,[clear('ice',4),collect(2,16)],ob('ice',[(0,2),(2,5),(5,1),(7,4)]),[], 'Scattered surface ice with an open center.'),
(107,'Искры на ветру','Sparks in the Wind','normal',29,6,[collect(0,24),collect(1,24)],[],[], 'Open six-color space for optional special creation, no scripted opening.'),
(108,'Ледяные ворота','Icy Gates','normal',29,6,[clear('ice',8),clear('chain',4)],ob('ice',[(1,1),(2,1),(5,4),(6,4)])+ob('ice',[(1,2),(2,2),(5,5),(6,5)],2)+ob('chain',[(3,1),(4,2),(3,4),(4,5)]),[], 'Layered ice clusters and separate central locks.'),
(109,'Три дороги к припасам','Three Supply Paths','normal',29,5,[clear('crate',9),delivery(3)],ob('crate',[(1,1),(1,3),(1,5),(4,2),(4,4),(4,5),(6,1),(6,4),(6,5)]),[1,4,6], 'Three differently spaced delivery gates.'),
(110,'Дорога к мельнице открыта','The Mill Road Opens','hard',30,6,[clear('crate',8),clear('ice',6),delivery(2)],ob('crate',[(0,1),(1,1),(6,2),(7,2),(0,4),(1,4),(6,5),(7,5)])+ob('ice',[(x,y) for x in (2,5) for y in (2,3,5)]),[2,5], 'Side stockpiles and two ice delivery routes mark local mill access.')]
briefs=[]
for n,ru,en,d,moves,colors,goals,obstacles,lanes,intent in rows:
 b=dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=moves,tileTypes=colors,columns=8,rows=7,boardSeed=11000+n,goals=goals,obstacles=obstacles,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=1 if n==110 else 0,boosterSeaPearl=0))
 coords=[(o['x'],o['y']) for o in obstacles];assert len(coords)==len(set(coords));assert all(0<=x<8 and 0<=y<7 for x,y in coords)
 for g in goals:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in obstacles)==g['target']
  if g['type']=='bring_down':assert len(lanes)==g['target']
 assert all((x,0) not in coords and (x,6) not in coords for x in lanes)
 briefs.append(b)
assert [b['round'] for b in briefs]==list(range(101,111))
assert [b['round'] for b in briefs if b['difficulty']=='recovery']==[101,106]
assert sum(b['rewards']['gold'] for b in briefs)==1840
assert sum(b['rewards']['gems'] for b in briefs)==6
assert sum(b['rewards']['boosterLivingLight'] for b in briefs)==1
(base/'Round101To110Briefs.json').write_text(json.dumps(dict(status='Draft design only; not installed, imported into Unity or solver-verified',rows=briefs),ensure_ascii=False,indent=2)+'\n')
(base/'QA/DraftChecks.json').write_text(json.dumps(dict(passed=True,draftRounds=10,checks=['Distinct round IDs','Valid unique obstacle coordinates','Goal counts match obstacle/item quantities','Delivery starts/exits free','Recovery101/106','Budget1840gold/6gems/1LivingLight'],unityValidated=False,installed=False),indent=2)+'\n');print('Ten draft briefs structurally checked; no game content installed.')
