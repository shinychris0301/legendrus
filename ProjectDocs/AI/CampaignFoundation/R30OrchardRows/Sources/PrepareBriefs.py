from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(161,'Между деревьями','Between the Trees','recovery',34,5,[clear('ice',4),collect(0,16)],obs('ice',[(0,1),(2,4),(5,2),(7,5)]),[],'Scattered single ice and modest red collection after160.'),
(162,'Две дорожки','Two Paths','normal',29,5,[clear('crate',6),clear('chain',4),feather(2)],obs('crate',[(1,2),(5,4),(0,3),(6,2),(1,5),(5,5)])+obs('chain',[(3,1),(4,2),(3,4),(4,5)]),[1,5],'Two unequal crate delivery lanes flank a separate central lock task; choose which to advance first.'),
(163,'Лёд в тени','Ice in the Shade','normal',29,5,[clear('ice',8),collect(3,20)],obs('ice',[(1,1),(2,2),(3,3),(4,4)])+obs('ice',[(3,1),(4,2),(5,3),(6,4)],2),[],'Two staggered diagonal ice groups contrast single and double layers without a solid barrier.'),
(164,'Крайний ряд','The Outer Row','normal',29,5,[clear('chain',3),clear('crate',6),feather(1)],obs('chain',[(7,2),(7,3)])+obs('chain',[(7,5)],2)+obs('crate',[(0,1),(1,2),(2,3),(3,4),(4,5),(5,2)]),[7],'One far-right locked delivery and separate left-to-centre crates leave room to build specials.'),
(165,'Три прохода в саду','Three Orchard Passages','hard',28,6,[clear('ice',6),clear('crate',8),feather(3)],obs('ice',[(2,2),(4,3),(7,1)])+obs('ice',[(2,5),(4,5),(7,5)],2)+obs('crate',[(0,2),(1,3),(3,1),(5,2),(6,3),(0,5),(3,4),(6,5)]),[2,4,7],'Staggered upper ice gates with layered lower gates across three unequal delivery routes.'),
(166,'Под яблонями','Beneath the Apple Trees','recovery',34,5,[collect(1,16),collect(2,16)],[],[],'Open blue/green recovery after165; no harvest objective is implied.'),
(167,'Самоцветы на солнце','Gems in the Sun','normal',29,6,[collect(0,24),collect(4,24)],[],[],'Open six-color red/yellow collection supports voluntary special creation.'),
(168,'Обход у деревьев','A Way around the Trees','normal',29,5,[clear('crate',8),clear('chain',4)],obs('crate',[(1,1),(2,1),(1,2),(2,3),(5,3),(6,4),(5,5),(6,5)],2)+obs('chain',[(4,1),(3,2),(4,4),(3,5)]),[],'Layered crate bends and offset central locks create choices without filling every lane.'),
(169,'Удержать проходы','Keep the Paths Open','normal',28,5,[dict(type='survival',target=18),clear('chain',6)],obs('chain',[(0,2),(2,1),(4,3),(6,2),(1,5),(5,5)],2),[],'Six separated layered locks during18 consumed moves; no timer.'),
(170,'Дорожки сада','Orchard Paths','hard',30,6,[clear('crate',6),clear('ice',8),feather(3)],obs('crate',[(0,2),(2,3),(5,2)])+obs('crate',[(0,5),(2,5),(5,5)],2)+obs('ice',[(1,1),(3,1),(4,2),(6,1),(7,3),(1,4),(4,4),(6,5)]),[0,2,5],'Three crate delivery lanes and separate ice tasks conclude orchard-row access before the field approach.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=int(n==170),boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0)
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=17000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
assert [b['round'] for b in briefs]==list(range(161,171))
for b in briefs:
 pos=[(o['x'],o['y']) for o in b['obstacles']];assert len(pos)==len(set(pos));assert all(0<=x<8 and 0<y<6 for x,y in pos)
 assert all(o['layers'] in (1,2) for o in b['obstacles'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=b['moveLimit']
  if g['type']=='collect_tile':assert 0<=g['tileType']<b['tileTypes']
 assert all((i['x'],0) not in pos and (i['x'],6) not in pos for i in b['boardItems'])
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored orchard-row block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten orchard-row briefs structurally checked.')
