from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==150
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.orchard_entrance';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=160,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={151: ('Лёд на садовой тропе растаял.', 'The ice on the orchard path has melted.', 'Поищу зелёные самоцветы рядом с оставшимся льдом.', 'I will look for green gems beside the remaining ice.'), 152: ('Оба входа свободны от ящиков.', 'Both entrances are clear of crates.', 'В середине есть место для длинной комбинации.', 'There is room in the centre for a long match.'), 153: ('Перья у калитки собраны.', 'The feathers by the gate are collected.', 'Нижние цепи крепче — проверю оба прохода.', 'The lower chains are stronger; I will check both passages.'), 154: ('Самоцветы сияют сквозь ветви.', 'The gems shine through the branches.', 'Поищу комбинацию из синих или фиолетовых самоцветов.', 'I will look for a match of blue or purple gems.'), 155: ('Все три пути для перьев расчищены.', 'All three feather routes are clear.', 'Оставлю ходы на крепкие ящики внизу.', 'I will save moves for the sturdy lower crates.'), 156: ('В тихом уголке можно перевести дух.', 'We can catch our breath in the quiet corner.', 'Красные и зелёные самоцветы помогут закончить сбор.', 'Red and green gems will help finish the collection.'), 157: ('Цепи больше не преграждают путь.', 'The chains no longer block the path.', 'Начну с крепких цепей ближе к середине.', 'I will start with the stronger chains near the centre.'), 158: ('Проход свободен от ледяных преград.', 'The passage is clear of icy barriers.', 'Проверю клетки, где ещё остался второй слой льда.', 'I will check the cells that still have a second layer of ice.'), 159: ('Боковая дорожка свободна, перья собраны.', 'The side path is clear and the feathers are collected.', 'Проверю цепи в стороне от путей перьев.', 'I will check the chains away from the feather routes.'), 160: ('Вход в сад открыт для жителей.', 'The orchard entrance is open to the villagers.', 'Сначала освобожу нижние цепи на трёх путях.', 'I will clear the lower chains on the three routes first.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Вход в сад открыт. Теперь расчистим дорожки между деревьями, а затем проверим путь к полям.' if lang=='ru' else 'The orchard entrance is open. Now we must clear the paths between the trees, then check the way to the fields.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R29';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed orchard entrance block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed160 rounds; first150 preserved.')
