from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==100
briefs=read(base.parents[1]/'R22CheckpointAudit/Round101To110Briefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.mill_approach';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=110,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={
101:('Тропа стала свободнее. Мельница уже близко.','The path is clearer. The mill is close now.','Попробую собрать самоцветы в длинный ряд.','I will try making a longer row of gems.'),
102:('Ящики больше не преграждают дорогу.','The crates no longer block the road.','Начну с ящиков у открытого прохода.','I will start with the crates beside the open passage.'),
103:('Крепкие ящики тоже поддались.','Even the sturdy crates gave way.','Крепким ящикам нужен ещё один удар.','The sturdy crates need another hit.'),
104:('Перья собраны, лёд на пути растаял.','The feathers are collected and the ice on the path is gone.','Сначала освобожу клетки под перьями.','I will clear the cells beneath the feathers first.'),
105:('За цепями нашёлся путь к мельнице.','There was a way to the mill beyond the chains.','Разобью преграды и открою путь перьям.','I will break the barriers and open a path for the feathers.'),
106:('Можно перевести дух и идти дальше.','We can catch our breath and move on.','Сначала уберу оставшийся лёд.','I will clear the remaining ice first.'),
107:('Самоцветы собраны. Продолжим путь.','The gems are collected. Let us continue.','Поищу комбинацию для сильного предмета.','I will look for a match that creates a powerful item.'),
108:('Лёд и цепи больше не мешают.','The ice and chains are out of the way.','Отмечу, где лёд ещё держится в два слоя.','I will watch for ice that still has two layers.'),
109:('Все три пути для перьев свободны.','All three feather routes are clear.','Нужно проверить каждый путь до самого низа.','I need to check each route all the way to the bottom.'),
110:('Мы добрались до мельничного двора.','We have reached the mill yard.','Освобожу проходы и сохраню ходы для льда.','I will open the passages and save moves for the ice.')}
for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Дорога к мельнице открыта. Теперь расчистим двор, чтобы жители могли добраться до припасов.' if lang=='ru' else 'The road to the mill is open. Now we must clear the yard so the villagers can reach the supplies.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector03R23';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed R22 drafts; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed110 rounds; first100 preserved.')
