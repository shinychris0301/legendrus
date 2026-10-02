from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==160
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.orchard_paths';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=170,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={161: ('Лёд между деревьями больше не мешает.', 'The ice between the trees is out of the way.', 'Поищу красные самоцветы возле оставшегося льда.', 'I will look for red gems beside the remaining ice.'), 162: ('Оба пера собраны, дорожки свободны.', 'Both feathers are collected and the paths are clear.', 'Проверю ящики на обоих путях и цепи в середине.', 'I will check the crates on both routes and the central chains.'), 163: ('В тени больше нет ледяных преград.', 'No icy barriers remain in the shade.', 'На второй полосе лёд крепче — оставлю на него ходы.', 'The ice in the second group is stronger; I will save moves for it.'), 164: ('Перо из крайнего ряда собрано.', 'The feather from the outer row is collected.', 'Сохраню ходы для нижней цепи справа.', 'I will save moves for the lower chain on the right.'), 165: ('Все три прохода освобождены ото льда.', 'All three passages are clear of ice.', 'Проверю нижние ледяные клетки на каждом пути.', 'I will check the lower ice cells on each route.'), 166: ('Под яблонями стало спокойнее.', 'It is quieter beneath the apple trees.', 'Поищу рядом синие и зелёные самоцветы.', 'I will look for nearby blue and green gems.'), 167: ('Красные и жёлтые самоцветы собраны.', 'The red and yellow gems are collected.', 'На открытом поле поищу длинную комбинацию.', 'I will look for a long match on the open board.'), 168: ('Обход свободен от ящиков и цепей.', 'The way around is clear of crates and chains.', 'Крепкие ящики требуют ещё одного удара.', 'The sturdy crates need another hit.'), 169: ('Проходы больше не скованы цепями.', 'The paths are no longer bound by chains.', 'Проверю нижние цепи, пока ещё есть ходы.', 'I will check the lower chains while moves remain.'), 170: ('Дорожки сада свободны, можно идти к полям.', 'The orchard paths are clear; we can head toward the fields.', 'Освобожу три выхода и проверю отдельные ледяные клетки.', 'I will clear the three exits and check the separate ice cells.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Дорожки между деревьями свободны. Теперь проверим путь из сада к полям — там ещё остались преграды.' if lang=='ru' else 'The paths between the trees are clear. Now we must check the way from the orchard to the fields; obstacles still remain there.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R30';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed orchard-row block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed170 rounds; first160 preserved.')
