from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
collect=lambda t,n:dict(type='collect_tile',tileType=t,target=n)
clear=lambda t,n:dict(type='clear_obstacle',obstacleType=t,target=n)
feather=lambda n:dict(type='bring_down',itemType='firebird_feather',target=n)
obs=lambda t,xy,l=1:[dict(x=x,y=y,type=t,layers=l) for x,y in xy]
rows=[
(151,'У садовой тропы','At the Orchard Path','recovery',34,5,[clear('ice',4),collect(2,16)],obs('ice',[(1,1),(3,3),(5,5),(7,2)]),[], 'Scattered single ice with modest green collection after the150 checkpoint.'),
(152,'Два входа в сад','Two Orchard Entrances','normal',29,5,[clear('crate',8),collect(0,22)],obs('crate',[(1,1),(2,1),(1,2),(2,3),(5,3),(6,4),(5,5),(6,5)]),[], 'Offset crate groups suggest two entrances; central columns3/4 remain open for special creation.'),
(153,'Перья у калитки','Feathers by the Gate','normal',29,5,[clear('chain',4),clear('ice',4),feather(2)],obs('chain',[(2,2),(6,3)])+obs('chain',[(2,5),(6,5)],2)+obs('ice',[(0,2),(4,1),(4,4),(7,4)]),[2,6], 'Two unequal delivery lanes with layered lower locks and a separate surface-ice task.'),
(154,'Свет сквозь ветви','Light through the Branches','normal',29,6,[collect(1,24),collect(3,24)],[],[], 'Open six-color board gives a breather from gates while encouraging voluntary specials.'),
(155,'Повороты садовой тропы','Orchard Path Bends','hard',28,6,[clear('crate',8),clear('ice',6),feather(3)],obs('crate',[(0,2),(3,3),(7,2),(1,3),(6,3)])+obs('crate',[(0,5),(3,5),(7,5)],2)+obs('ice',[(2,1),(4,1),(5,2),(1,4),(4,4),(6,5)]),[0,3,7], 'Three separated delivery routes with strong lower crate gates; no continuous wall.'),
(156,'Тихий уголок','A Quiet Corner','recovery',34,5,[collect(0,16),collect(2,16)],[],[], 'Open red/green recovery after155; preserve generous move budget.'),
(157,'Разомкнуть цепи','Unfasten the Chains','normal',29,5,[clear('chain',8),collect(1,22)],obs('chain',[(1,1),(6,1),(0,4),(7,4)])+obs('chain',[(2,3),(5,3),(2,5),(5,5)],2),[], 'Separate inner lower locks and single outer locks require choosing a clearing priority.'),
(158,'Сохранить проход','Keep the Passage Clear','normal',28,5,[dict(type='survival',target=18),clear('ice',6)],obs('ice',[(1,2),(3,1),(5,2),(2,4),(4,5),(6,4)],2),[], 'Six scattered two-stage ice cells during18 consumed moves; no timer or automatic recovery.'),
(159,'Боковая дорожка','The Side Path','normal',29,5,[clear('crate',6),clear('chain',4),feather(2)],obs('crate',[(1,2),(4,3),(0,4),(6,1)])+obs('crate',[(1,5),(4,5)],2)+obs('chain',[(2,1),(5,2),(6,4),(7,5)]),[1,4], 'Asymmetric crate deliveries contrasted with off-route locks; delivery starts and exits stay free.'),
(160,'Вход в сад открыт','The Orchard Entrance Is Open','hard',30,6,[clear('chain',6),clear('crate',8),feather(3)],obs('chain',[(x,2) for x in (0,3,5)])+obs('chain',[(x,5) for x in (0,3,5)],2)+obs('crate',[(1,1),(2,3),(4,1),(6,3),(7,1),(1,4),(4,4),(7,4)]),[0,3,5], 'Local access checkpoint combines learned tasks without introducing harvest or helper mechanics.')]
briefs=[]
for n,ru,en,d,m,c,g,o,lanes,intent in rows:
 rewards=dict(gold=160 if d=='recovery' else 220 if d=='hard' else 180,gems=3 if d=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=int(n==160))
 briefs.append(dict(round=n,nameRu=ru,nameEn=en,difficulty=d,moveLimit=m,tileTypes=c,columns=8,rows=7,boardSeed=16000+n,goals=g,obstacles=o,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in lanes],fixedCells=[],intent=intent,rewards=rewards))
assert [b['round'] for b in briefs]==list(range(151,161))
assert len({b['boardSeed'] for b in briefs})==10
for b in briefs:
 positions=[(o['x'],o['y']) for o in b['obstacles']];assert len(positions)==len(set(positions))
 assert all(0<=x<8 and 0<y<6 for x,y in positions)
 assert all(o['layers'] in (1,2) for o in b['obstacles'])
 assert all((i['x'],0) not in positions and (i['x'],6) not in positions for i in b['boardItems'])
 for g in b['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in b['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(b['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=b['moveLimit']
  if g['type']=='collect_tile':assert 0<=g['tileType']<b['tileTypes']
rewards={k:sum(b['rewards'][k] for b in briefs) for k in briefs[0]['rewards']}
assert rewards==dict(gold=1840,gems=6,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=1)
(base/'Round151To160Briefs.json').write_text(json.dumps(dict(status='Draft only; not installed, Unity-validated, solved or human-rated',rows=briefs),ensure_ascii=False,indent=2)+'\n')
(base/'QA/DraftChecks.json').write_text(json.dumps(dict(passed=True,rounds=10,scope='Bounds, unique obstacle positions/seeds, layers, goal counts, free delivery starts/exits, survival move budget and reward total only',rewards=rewards,unityValidated=False,installed=False),indent=2)+'\n')
print('Ten draft briefs structurally checked; no gameplay installation.')
