from pathlib import Path
import json,hashlib
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
read=lambda p:json.loads(p.read_text())
active=root/'Assets/LegendsOfRus/Resources/CampaignActive';candidate=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector04R31'
levels=read(active/'levels.json')['levels'];old=read(base/'Sources/levels.before.json')['levels'];story=read(active/'story.json');previous=read(base/'Sources/story.before.json')
assert len(levels)==180 and levels[:170]==old
assert [l['levelId'] for l in levels]==list(range(1,181)) and len({l['id'] for l in levels})==180
for name in ['levels','story','story_ru','story_en']:assert (active/(name+'.json')).read_bytes()==(candidate/(name+'.json')).read_bytes()
for key,value in previous.items():
 if isinstance(value,list):assert story[key][:len(value)]==value,key
 else:assert story[key]==value,key
for lang in ['ru','en']:
 before={e['key']:e['value'] for e in read(base/f'Sources/story_{lang}.before.json')['entries']};after=read(active/f'story_{lang}.json')['entries'];keys=[e['key'] for e in after];assert len(keys)==len(set(keys));after={e['key']:e['value'] for e in after};assert all(after[k]==v for k,v in before.items())
for l in levels[170:]:
 positions=[(o['x'],o['y']) for o in l['obstacles']];assert len(positions)==len(set(positions))
 assert all(0<=x<8 and 0<=y<7 for x,y in positions)
 for g in l['goals']:
  if g['type']=='clear_obstacle':assert sum(o['type']==g['obstacleType'] for o in l['obstacles'])==g['target']
  if g['type']=='bring_down':assert len(l['boardItems'])==g['target']
  if g['type']=='survival':assert g['target']<=l['moveLimit']
 assert all((i['x'],i['y']) not in positions and i['y']==0 for i in l['boardItems'])
 assert all((i['x'],6) not in positions for i in l['boardItems'])
reward={k:sum(l['rewards'][k] for l in levels[170:]) for k in levels[-1]['rewards']};assert reward==dict(gold=1840,gems=6,boosterMace=0,boosterFirebird=1,boosterLivingLight=0,boosterSeaPearl=0)
assert levels[170]['difficulty']=='recovery' and levels[175]['difficulty']=='recovery'
assert story['milestones'][-1]['afterLevelId']==180 and not story.get('completion')
report={'passed':True,'checks':['Exact preservation of first170 levels and prior story/locales','Contiguous unique1–180 IDs','Active/candidate byte equality for four JSONs','Unique obstacle positions and valid coordinates','Goal quantities equal obstacle/item counts','Feather top cells and bottom exits free','Survival targets within budgets','Recovery after170/175','Bilingual key uniqueness','No chapter finale','Prior resource metas and settings preserved'],'newFirstClearRewards':reward}
# Check all pre-existing files except the explicitly authorized content/test edits.
before=read(base/'Sources/BeforeHashes.json');allowed={f'Assets/LegendsOfRus/Resources/CampaignActive/{n}.json' for n in ['levels','story','story_ru','story_en']}
allowed|={'Assets/LegendsOfRus/Tests/CampaignPlayMode/ActiveCampaignIntegrationTests.cs','Assets/LegendsOfRus/Tests/CampaignPlayMode/CampaignCheckpointLifecycleTests.cs','Assets/LegendsOfRus/Tests/CampaignPlayMode/CampaignBoardIntegrationTests.cs','Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs'}
changed=[]
for name,h in before.items():
 if name in allowed:continue
 p=root/name
 # Test runner can alter this generated linker file; CleanAndAudit restores it.
 if name.startswith('Assets/AddressableAssetsData/link.xml'):continue
 assert p.exists() and hashlib.sha256(p.read_bytes()).hexdigest()==h,name
report['protectedFilesChecked']=len(before)-len(allowed)
(base/'QA/ContentChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
