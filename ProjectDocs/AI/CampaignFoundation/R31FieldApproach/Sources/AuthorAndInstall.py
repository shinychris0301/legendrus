from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==170
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.field_approach';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=180,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={171: ('Цепи за садовой оградой сняты.', 'The chains beyond the orchard fence are cleared.', 'Поищу фиолетовые самоцветы рядом с цепями.', 'I will look for purple gems beside the chains.'), 172: ('Три группы ящиков больше не мешают.', 'The three crate groups are out of the way.', 'Крепкие ящики требуют повторного удара.', 'The sturdy crates need another hit.'), 173: ('Перья собраны, оба пути свободны.', 'The feathers are collected and both ways are clear.', 'Проверю нижний лёд на двух путях.', 'I will check the lower ice on both routes.'), 174: ('Зелёные и жёлтые самоцветы собраны.', 'The green and yellow gems are collected.', 'Открытое поле оставляет место для длинных комбинаций.', 'The open board leaves room for long matches.'), 175: ('Поворот свободен от цепей и ящиков.', 'The turn is clear of chains and crates.', 'Не забуду дальний путь справа.', 'I will remember the distant route on the right.'), 176: ('Можно перевести дух перед дорогой к полю.', 'We can catch our breath before the field road.', 'Поищу красные и фиолетовые самоцветы.', 'I will look for red and purple gems.'), 177: ('Холодная межа больше не преграждает путь.', 'The frozen boundary no longer blocks the way.', 'Внутренние ледяные клетки крепче внешних.', 'The inner ice cells are stronger than the outer ones.'), 178: ('Дорога освобождена от ящиков и льда.', 'The road is clear of crates and ice.', 'Проверю обе преграды, пока есть ходы.', 'I will check both obstacle types while moves remain.'), 179: ('Перо у межи собрано.', 'The feather by the boundary is collected.', 'Оставлю ходы на нижнюю цепь и боковой лёд.', 'I will save moves for the lower chain and side ice.'), 180: ('Выход к полям свободен.', 'The way to the fields is clear.', 'Освобожу нижние ящики и проверю цепи в стороне.', 'I will clear the lower crates and check the chains off to the side.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Путь из сада к полям открыт. Теперь соединим полевые тропы с дорогами к домам, чтобы жители могли проходить без преград.' if lang=='ru' else 'The way from the orchard to the fields is open. Now we must connect the field paths with the roads to the homes so the villagers can pass freely.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R31';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed field-approach block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed180 rounds; first170 preserved.')
