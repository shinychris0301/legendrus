from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==130
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.provision_paths';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=140,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={
131:('Первые ящики на пути к домам убраны.','The first crates on the way to the homes are cleared.','Поищу синие самоцветы рядом с ящиками.','I will look for blue gems beside the crates.'),
132:('Перо собрано, лёд на соседней тропе растаял.','The feather is collected and the ice on the neighbouring path is gone.','Не забуду про лёд справа от пути пера.','I must remember the ice to the right of the feather route.'),
133:('Поворот свободен даже от крепких ящиков.','Even the sturdy crates at the bend are cleared.','На свободной стороне можно создать сильный предмет.','I can create a powerful item on the open side.'),
134:('Цепи на разных дорогах больше не мешают.','The chains on the different roads are out of the way.','Выберу цепи, рядом с которыми есть синие самоцветы.','I will choose chains with blue gems nearby.'),
135:('Оба обхода открыты, перья собраны.','Both detours are open and the feathers collected.','Проверю оба пути: крепкие ящики стоят на разной высоте.','I will check both routes: their sturdy crates sit at different heights.'),
136:('Здесь можно спокойно продолжить путь.','We can continue along this quiet path.','Поищу длинный ряд зелёных или фиолетовых самоцветов.','I will look for a long row of green or purple gems.'),
137:('Лёд у развилки разбит, самоцветы собраны.','The ice at the fork is broken and the gems collected.','Крепкий лёд потребует повторного удара.','The strong ice will need another hit.'),
138:('Все три пути для перьев открыты.','All three feather routes are open.','Сначала проверю крепкие цепи ближе к выходам.','I will check the stronger chains near the exits first.'),
139:('Проходы освобождены от льда и цепей.','The passages are clear of ice and chains.','Проверю всё поле, чтобы не пропустить отдельную преграду.','I will check the whole board for isolated barriers.'),
140:('Тропы от склада ведут к домам.','The paths from the storehouse lead to the homes.','Разобью ящики слева и проверю цепи справа.','I will break the crates on the left and check the chains on the right.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Тропы от склада к домам расчищены. Осталось соединить их с дорогой от мельницы и убрать последние преграды.' if lang=='ru' else 'The paths from the storehouse to the homes are clear. We still need to connect them to the mill road and remove the remaining barriers.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector03R26';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed provision-path block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed140 rounds; first130 preserved.')
