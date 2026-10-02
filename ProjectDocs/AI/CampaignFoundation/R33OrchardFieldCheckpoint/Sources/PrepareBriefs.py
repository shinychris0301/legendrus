from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(191,'Обходная тропа','The Bypass Path','recovery',34,5,[clear('chain',4),collect(1,16)],obs('chain',[(0,1),(2,4),(5,1),(7,4)]),[],'Four isolated single locks and modest blue collection after190.'),
(192,'Два обхода','Two Bypasses','normal',29,5,[clear('crate',8),clear('ice',4)],obs('crate',[(0,2),(1,2),(2,3),(6,1)])+obs('crate',[(0,5),(1,5),(5,4),(6,4)],2)+obs('ice',[(3,1),(4,2),(3,4),(7,5)]),[],'Unequal single/layered crate groups and separate ice leave a central approach.'),
(193,'Перо у поворота','A Feather by the Bend','normal',29,5,[clear('chain',2),clear('crate',8),feather(1)],obs('chain',[(2,2)])+obs('chain',[(2,5)],2)+obs('crate',[(0,1),(1,3),(3,1),(4,3),(5,4),(6,1),(7,3),(4,5)]),[2],'One left-inner delivery with a strong lower lock and independent side crates.'),
(194,'Самоцветы на обходе','Gems on the Bypass','normal',29,6,[collect(3,24),collect(4,24)],[],[],'Open six-color purple/yellow collection gives room for voluntary specials.'),
(195,'Три обходных пути','Three Bypass Routes','hard',28,6,[clear('crate',6),clear('ice',6),feather(3)],obs('crate',[(0,2),(3,3),(6,1)])+obs('crate',[(0,5),(3,5),(6,5)],2)+obs('ice',[(1,1),(2,3),(4,1),(5,3),(7,4),(4,5)]),[0,3,6],'Three unequal crate deliveries with layered lower gates and separate ice targets.'),
(196,'Передышка у сада','A Rest by the Orchard','recovery',34,5,[collect(0,16),collect(2,16)],[],[],'Open red/green recovery after195.'),
(197,'Крайние проходы','The Outer Passages','normal',29,5,[clear('chain',6),clear('ice',6)],obs('chain',[(0,1),(0,4),(1,5),(6,1),(7,3),(7,5)],2)+obs('ice',[(2,1),(3,2),(4,3),(5,4),(2,5),(5,1)]),[],'Strong outer locks contrast with scattered single ice in the middle.'),
(198,'Сохранить обходы','Keep the Bypasses Open','normal',28,5,[dict(type='survival',target=18),clear('crate',8)],obs('crate',[(1,1),(3,2),(5,3),(7,4)])+obs('crate',[(0,4),(2,5),(4,1),(6,2)],2),[],'Eighteen consumed moves and staggered single/double crates; no elapsed-time timer.'),
(199,'Перья на двух тропах','Feathers on Two Paths','normal',29,5,[clear('ice',4),clear('chain',4),feather(2)],obs('ice',[(2,2),(7,3)])+obs('ice',[(2,5),(7,5)],2)+obs('chain',[(0,3),(4,1),(5,3),(4,5)]),[2,7],'Offset ice deliveries with separate inner locks; preserve clear bottom exits.'),
(200,'Сад, поля и дороги','Orchard, Fields and Roads','hard',30,6,[clear('chain',6),clear('ice',8),feather(3)],obs('chain',[(1,2),(4,3),(7,1)])+obs('chain',[(1,5),(4,5),(7,5)],2)+obs('ice',[(0,1),(2,3),(3,1),(5,2),(6,3),(0,4),(3,4),(6,5)]),[1,4,7],'Three locked delivery routes and separate ice conclude local orchard/field access, without a chapter finale.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 reward=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=int(n==200))
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=20000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=reward))
assert [b['round'] for b in briefs]==list(range(191,201))
for b in briefs:
 pos=[(o['x'],o['y']) for o in b['obstacles']];assert len(pos)==len(set(pos));assert all(0<=x<8 and 0<y<6 for x,y in pos);assert all(o['layers'] in (1,2) for o in b['obstacles'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=b['moveLimit']
  if g['type']=='collect_tile':assert 0<=g['tileType']<b['tileTypes']
 assert all((i['x'],0) not in pos and (i['x'],6) not in pos for i in b['boardItems'])
(base/'RoundBriefs.json').write_text(json.dumps(dict(status='Authored orchard/field checkpoint block; difficulty provisional pending human play',rows=briefs),ensure_ascii=False,indent=2)+'\n')
print('Ten checkpoint briefs structurally checked.')
