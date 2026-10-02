from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==120
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.storehouse_approach';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=130,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={
121:('Тропа к складу стала свободнее.','The path to the storehouse is clearer.','Поищу длинный ряд синих или зелёных самоцветов.','I will look for a long row of blue or green gems.'),
122:('Лёд на тропинках больше не мешает.','The ice no longer blocks the paths.','Проверю ледяные клетки по всему полю.','I will check the icy cells across the board.'),
123:('Нижние заслоны разбиты, перья собраны.','The lower barriers are broken and the feathers collected.','Крепкие ящики внизу нужно ударить дважды.','The sturdy crates at the bottom need two hits.'),
124:('Цепи у склада сняты.','The chains by the storehouse are cleared.','Буду собирать красные самоцветы и освобождать цепи.','I will collect red gems and clear the chains.'),
125:('Все три пути к складу открыты.','All three paths to the storehouse are open.','Сохраню ходы для нижних цепей на каждом пути.','I will save moves for the lower chains on each route.'),
126:('У крыльца можно перевести дух.','We can catch our breath by the porch.','Уберу лёд по краям и соберу фиолетовые самоцветы.','I will clear the edge ice and collect purple gems.'),
127:('Самоцветы собраны. До склада совсем близко.','The gems are collected. The storehouse is close now.','На открытом поле можно собрать сильную комбинацию.','The open board has room for a powerful match.'),
128:('Крепкие ящики внизу больше не мешают.','The sturdy lower crates are out of the way.','Использую свободную середину, чтобы добраться до ящиков.','I will use the open centre to reach the crates.'),
129:('Ледяные проходы свободны, перья собраны.','The icy passages are clear and the feathers collected.','Нижний лёд крепче — оставлю на него ходы.','The lower ice is stronger; I will save moves for it.'),
130:('Дорога к припасам открыта.','The road to the supplies is open.','Проверю нижние ящики и оставшийся лёд.','I will check the lower crates and remaining ice.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Дорога к складу открыта. Теперь расчистим тропы от припасов к деревенским домам.' if lang=='ru' else 'The road to the storehouse is open. Now we must clear the paths from the supplies to the village homes.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector03R25';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed storehouse block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed130 rounds; first120 preserved.')
