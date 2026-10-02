from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent
read=lambda p:json.loads(p.read_text())
levels=read(base/'levels.before.json');story=read(base/'story.before.json');locales={l:read(base/f'story_{l}.before.json') for l in ['ru','en']}
assert len(levels['levels'])==110
briefs=read(base.parent/'RoundBriefs.json')['rows']
for b in briefs:
 n=b['round'];l={k:b[k] for k in ['columns','rows','tileTypes','moveLimit','boardSeed','difficulty','goals','obstacles','boardItems','fixedCells','rewards']}
 l.update(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=b['nameRu'],startingScore=0,difficultyRating={'recovery':2,'normal':4,'hard':6}[b['difficulty']],starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
 levels['levels'].append(l)
scene='campaign.chapter01.mill_yard';trigger='dialogue.'+scene
levels['levels'][-1]['storyTriggers']=[trigger]
story['scenes'].append(dict(id=scene,trigger=trigger,returnRoute='ChapterMap',lines=[dict(speakerKey='speaker.ivan',textKey=scene+'.01')]))
story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=120,sceneId=scene,major=True))
# New reactions refer to actual goals without implying new production or restoration mechanics.
reactions={
111:('Лёд у ворот растаял. Войдём во двор.','The ice by the gate is gone. Let us enter the yard.','Поищу синие самоцветы рядом со льдом.','I will look for blue gems beside the ice.'),
112:('Боковые проходы и середина двора свободны.','The side passages and the centre of the yard are clear.','Не забуду про ящики между двумя проходами.','I must remember the crates between the two passages.'),
113:('Даже крепкие ящики удалось разбить.','Even the sturdy crates have been broken.','Буду собирать красные самоцветы возле ящиков.','I will match red gems beside the crates.'),
114:('Все три прохода открыты.','All three passages are open.','Проверю, где цепи ещё удерживают самоцветы.','I will check where chains still hold the gems.'),
115:('Перья прошли через дальние ворота.','The feathers have passed through the far gate.','Ящики у краёв крепкие — оставлю ходы на второй удар.','The edge crates are sturdy; I will save moves for a second hit.'),
116:('Здесь спокойно. Можно продолжать путь.','It is quiet here. We can continue on our way.','Поищу длинный ряд зелёных или фиолетовых самоцветов.','I will look for a long row of green or purple gems.'),
117:('Искры помогли освободить края двора.','The sparks helped clear the edges of the yard.','В середине есть место для сильной комбинации.','There is room in the centre for a powerful match.'),
118:('Лёд в середине двора больше не мешает.','The ice in the centre of the yard is gone.','Сначала разобью оба слоя льда.','I will break both layers of ice first.'),
119:('Перья собраны, проходы у ограды открыты.','The feathers are collected and the fence passages are open.','Проверю нижние цепи и ящики в середине.','I will check the lower chains and the central crates.'),
120:('Мельничный двор свободен. Пора идти к складу.','The mill yard is clear. It is time to head to the storehouse.','Сначала открою путь перьям, затем уберу крепкие ящики.','I will open the feather routes first, then clear the sturdy crates.')}

for lang,data in locales.items():
 entries={e['key']:e['value'] for e in data['entries']}
 entries[scene+'.01']='Мельничный двор расчищен. Осталось открыть дорогу к складу, где хранятся деревенские припасы.' if lang=='ru' else 'The mill yard is clear. Next we must open the road to the storehouse where the village keeps its supplies.'
 for b in briefs:
  n=b['round'];entries[f'campaign.round{n:03}.name']=b['nameRu' if lang=='ru' else 'nameEn']
  for i,outcome in enumerate(['victory','failure']):
   key=f'reaction.campaign.chapter01.round{n:03}.{outcome}';entries[key]=reactions[n][i*2+(lang=='en')]
   if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
 data['entries']=[dict(key=k,value=v) for k,v in entries.items()]
out=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector03R24';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{k}',v) for k,v in locales.items()]]:
 p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,root/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
 meta=Path(str(p)+'.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(base.parent/'RoundBriefs.json').write_text(json.dumps({'status':'Installed mill-yard block; difficulty remains provisional pending human play','rows':briefs},ensure_ascii=False,indent=2)+'\n')
print('Installed120 rounds; first110 preserved.')
