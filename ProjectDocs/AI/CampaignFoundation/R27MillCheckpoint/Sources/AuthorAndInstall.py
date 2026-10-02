from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==140
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.mill_routes_secured';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=150,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={
141:('Лёд у мельницы больше не мешает.','The ice by the mill is out of the way.','Поищу зелёные самоцветы возле льда.','I will look for green gems beside the ice.'),
142:('Разрозненные тропы снова соединены.','The separate paths are joined again.','Проверю оба поворота и лёд между ними.','I will check both bends and the ice between them.'),
143:('Средний проход открыт, перо собрано.','The middle passage is open and the feather collected.','Нижняя цепь крепче — оставлю на неё ходы.','The lower chain is stronger; I will save moves for it.'),
144:('Синие и фиолетовые самоцветы собраны.','The blue and purple gems are collected.','На открытом поле поищу длинную комбинацию.','I will look for a long match on the open board.'),
145:('Все три ледяных прохода свободны.','All three icy passages are clear.','Проверю нижний лёд на каждом пути пера.','I will check the lower ice on each feather route.'),
146:('Можно перевести дух перед последними преградами.','We can catch our breath before the remaining barriers.','Поищу рядом красные и синие самоцветы.','I will look for nearby red and blue gems.'),
147:('Последние цепи на этом участке сняты.','The remaining chains on this stretch are cleared.','В середине цепи крепче, чем по краям.','The chains in the centre are stronger than those at the edges.'),
148:('Дорога освобождена от крепких ящиков.','The road is clear of sturdy crates.','Проверю ящики внизу, чтобы не оставить преград.','I will check the lower crates for remaining barriers.'),
149:('Перья собраны, развилка свободна.','The feathers are collected and the fork is clear.','Освобожу оба выхода и проверю отдельные ледяные клетки.','I will clear both exits and check the isolated ice cells.'),
150:('Мельница, склад и дома связаны свободными дорогами.','Clear roads connect the mill, storehouse and homes.','Сохраню ходы для крепких цепей внизу.','I will save moves for the strong lower chains.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Дороги между мельницей, складом и домами свободны. Теперь проверим тропы к саду и полям: деревне ещё нужна наша помощь.' if lang=='ru' else 'The roads between the mill, storehouse and homes are clear. Now we must check the paths to the orchard and fields: the village still needs our help.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector03R27';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed mill checkpoint block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed150 rounds; first140 preserved.')
