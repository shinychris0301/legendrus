from pathlib import Path
import json, hashlib, re, xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[5]
out=root/'ProjectDocs/AI/DevelopmentStageReview/R1/QA'; out.mkdir(exist_ok=True)
res=root/'Assets/LegendsOfRus/Resources'
def read(p): return json.loads(p.read_text())
levels=read(res/'Config/chapter01_levels.json'); story=read(res/'Config/chapter01_story.json'); regions=read(res/'ProductionUI/WorldMapReferenceAligned/chapters.json')['chapters']
locales={lang:{e['key']:e['value'] for e in read(res/f'Localization/story_{lang}.json')['entries']} for lang in ['ru','en']}
ids={x['levelId'] for x in levels['levels']}; scenes={x['id'] for x in story['scenes']}
refs=set()
def keys(v):
 if isinstance(v,dict):
  for k,x in v.items():
   if k.endswith('Key') and isinstance(x,str): refs.add(x)
   else: keys(x)
 elif isinstance(v,list):
  for x in v: keys(x)
keys(story)
checks={
 'unique_level_ids':len(ids)==len(levels['levels']),
 'contiguous_active_level_ids':sorted(ids)==list(range(1,len(ids)+1)),
 'all_20_region_ids':sorted(x['id'] for x in regions)==list(range(1,21)),
 'unique_scene_ids':len(scenes)==len(story['scenes']),
 'milestone_scene_and_level_references':all(x['sceneId'] in scenes and x['afterLevelId'] in ids for x in story['milestones']),
 'completion_scene_and_level_references':story['completion']['finalLevelId'] in ids and story['completion']['finaleSceneId'] in scenes,
 'reaction_level_references':all(x['levelId'] in ids for x in story['reactions']),
 'russian_story_key_coverage':not(refs-locales['ru'].keys()),
 'english_dialogue_key_coverage':all(x[k] in locales['en'] for s in story['scenes'] for x in s['lines'] for k in ['speakerKey','textKey'])}
evidence=[]
for name in ['CoreJourneyAudit','PersistenceLifecycleAudit','TextClarityAudit','AndroidDeviceAudit']:
 for p in sorted((root/f'ProjectDocs/AI/{name}/R1/QA').glob('**/results.xml')):
  r=ET.parse(p).getroot(); evidence.append({'path':str(p.relative_to(root)),**{k:r.get(k) for k in ['total','passed','failed','skipped','result']}})
activations={p.name:read(p) for p in sorted((res/'Config').glob('*activation.json'))}
source_docs=[]
for p in sorted((root/'ProjectDocs/SourceDocuments').glob('*Scenario*.docx')):
 source_docs.append({'path':str(p.relative_to(root)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
paths=[res/'Config/chapter01_levels.json',res/'Config/chapter01_story.json',res/'Config/chapter01.json',res/'Localization/story_ru.json',res/'Localization/story_en.json',res/'ProductionUI/WorldMapReferenceAligned/chapters.json',root/'Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelData.cs',root/'Assets/LegendsOfRus/Scripts/ContentPipelineCore/ChapterPipelinePolicy.cs']
data={'auditDate':'2026-09-26','method':'Local source and existing verification evidence review; no fresh Unity/device run','unity':(root/'ProjectSettings/ProjectVersion.txt').read_text().splitlines()[0],'nativePagePrefabs':sorted(p.stem for p in (res/'ProductionUI/Pages').glob('*.prefab')),'activeLevelCatalogs':[str(p.relative_to(root)) for p in (res/'Config').glob('chapter*_levels.json')],'activeStoryCatalogs':[str(p.relative_to(root)) for p in (res/'Config').glob('chapter*_story.json')],'playableLevels':len(ids),'plannedLevelsPerChapter':500,'plannedChapters':20,'plannedRoundsTotal':10000,'missingAuthoredRoundsAgainstTarget':10000-len(ids),'storyScenes':len(scenes),'storyMilestones':len(story['milestones']),'storyReactions':len(story['reactions']),'storyFinaleLevel':story['completion']['finalLevelId'],'regions':regions,'goalTypes':sorted({g['type'] for l in levels['levels'] for g in l['goals']}),'obstacleTypes':sorted({o['type'] for l in levels['levels'] for o in l.get('obstacles',[])}),'checks':checks,'missingStoryKeys':{lang:sorted(refs-d.keys()) for lang,d in locales.items()},'existingUnityResults':evidence,'activationManifests':activations,'scenarioDocuments':source_docs,'inspectedSourceHashes':{str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for p in paths}}
(out/'Inventory.json').write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n')
print(json.dumps({'checks':checks,'nativePages':len(data['nativePagePrefabs']),'playableLevels':len(ids),'storyScenes':len(scenes),'englishMissingStoryKeys':len(data['missingStoryKeys']['en'])},indent=2))
assert all(checks.values())
