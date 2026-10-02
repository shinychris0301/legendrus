from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==190
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.orchard_field_paths';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=200,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={191: ('Цепи на обходной тропе сняты.', 'The chains on the bypass path are cleared.', 'Поищу синие самоцветы рядом с оставшимися цепями.', 'I will look for blue gems beside the remaining chains.'), 192: ('Оба обхода свободны от ящиков и льда.', 'Both bypasses are clear of crates and ice.', 'Нижние группы ящиков крепче верхних.', 'The lower crate groups are stronger than the upper ones.'), 193: ('Перо у поворота собрано.', 'The feather by the bend is collected.', 'Сохраню ходы для нижней цепи и боковых ящиков.', 'I will save moves for the lower chain and side crates.'), 194: ('Фиолетовые и жёлтые самоцветы собраны.', 'The purple and yellow gems are collected.', 'На открытом поле поищу длинную комбинацию.', 'I will look for a long match on the open board.'), 195: ('Три обходных пути открыты.', 'The three bypass routes are open.', 'Проверю крепкие ящики внизу каждого пути.', 'I will check the sturdy crates at the bottom of each route.'), 196: ('У сада можно ненадолго отдохнуть.', 'We can rest briefly by the orchard.', 'Поищу красные и зелёные самоцветы поблизости.', 'I will look for red and green gems nearby.'), 197: ('Крайние проходы больше не скованы цепями.', 'The outer passages are no longer bound by chains.', 'На краях цепи крепче — оставлю на них ходы.', 'The outer chains are stronger; I will save moves for them.'), 198: ('Обходы освобождены от ящиков.', 'The bypasses are clear of crates.', 'Проверю ящики, которым нужен ещё один удар.', 'I will check the crates that need another hit.'), 199: ('Перья с обеих троп собраны.', 'The feathers from both paths are collected.', 'Нижний лёд на двух путях ещё может задерживать перья.', 'The lower ice on both routes may still hold up the feathers.'), 200: ('Сад и поля связаны свободными дорогами.', 'Clear roads connect the orchard and fields.', 'Освобожу нижние цепи и проверю оставшийся лёд.', 'I will clear the lower chains and check the remaining ice.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Дороги у сада и полей свободны, но у реки всё ещё держится тень. Проверим тропу к старым оберегам деревни.' if lang=='ru' else "The roads by the orchard and fields are clear, but a shadow still lingers by the river. We must check the path to the village's old wards."
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R33';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed orchard/field checkpoint block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed200 rounds; first190 preserved.')
