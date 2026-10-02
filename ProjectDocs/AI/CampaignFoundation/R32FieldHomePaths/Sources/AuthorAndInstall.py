from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==180
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.field_home_paths';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=190,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={181: ('Лёд на тропе к домам больше не мешает.', 'The ice on the path to the homes is out of the way.', 'Поищу жёлтые самоцветы рядом со льдом.', 'I will look for yellow gems beside the ice.'), 182: ('Обе дороги освобождены от преград.', 'Both roads are clear of obstacles.', 'Слева ящики крепче; справа ещё остался лёд.', 'The crates on the left are stronger; ice still remains on the right.'), 183: ('Перья между дорогами собраны.', 'The feathers between the roads are collected.', 'Проверю нижние цепи и лёд по краям.', 'I will check the lower chains and the ice at the edges.'), 184: ('Красные и синие самоцветы собраны.', 'The red and blue gems are collected.', 'На открытой поляне есть место для длинной комбинации.', 'The open clearing has room for a long match.'), 185: ('Три дороги к домам свободны.', 'The three roads to the homes are clear.', 'Оставлю ходы на крепкие нижние ящики.', 'I will save moves for the sturdy lower crates.'), 186: ('На тихой дороге можно перевести дух.', 'We can catch our breath on the quiet road.', 'Поищу синие и фиолетовые самоцветы поблизости.', 'I will look for blue and purple gems nearby.'), 187: ('Цепи у развилки сняты.', 'The chains at the fork are cleared.', 'Проверю крепкие цепи по диагонали.', 'I will check the stronger diagonal chains.'), 188: ('Лёд больше не разделяет дороги.', 'The ice no longer separates the roads.', 'Проверю клетки с оставшимся вторым слоем льда.', 'I will check the cells with a second layer of ice still remaining.'), 189: ('Оба боковых выхода свободны, перья собраны.', 'Both side exits are clear and the feathers collected.', 'Проверю нижний лёд на каждой стороне.', 'I will check the lower ice on each side.'), 190: ('Полевые тропы снова ведут к домам.', 'The field paths lead to the homes again.', 'Освобожу нижние цепи на трёх путях и проверю ящики.', 'I will clear the lower chains on the three routes and check the crates.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Полевые тропы соединены с дорогами к домам. Осталось проверить обходные пути у сада и полей, чтобы ни один проход не остался закрытым.' if lang=='ru' else 'The field paths now connect with the roads to the homes. We still need to check the bypasses by the orchard and fields so no passage remains blocked.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R32';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed field-to-home block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed190 rounds; first180 preserved.')
