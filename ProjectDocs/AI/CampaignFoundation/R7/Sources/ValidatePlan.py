from pathlib import Path
import json,hashlib,re
root=Path.cwd();b=root/'ProjectDocs/AI/CampaignFoundation/R7';checks=[]
def read(p):return json.loads(p.read_text())
def check(name,condition):
 if not condition:raise AssertionError(name)
 checks.append(name)
plan=read(b/'Plan/First50Rounds.json');rows=plan['rows'];legacy=read(b/'Sources/LegacyLevels.json')
check('50 unique contiguous stable round IDs',len(rows)==50 and [r['round'] for r in rows]==list(range(1,51)) and len({r['key'] for r in rows})==50)
check('Draft held; no campaign publication',plan['status'].startswith('held-') and all(not r['campaignPublished'] for r in rows))
check('Exactly10 authored baselines;40 unbuilt briefs',sum(r['boardAuthored'] for r in rows)==10 and all(not r['boardAuthored'] for r in rows[10:]))
for r,l in zip(rows,legacy):
 check('Preserved baseline round '+str(r['round']),r['reward']==l['rewards'] and r['goals']==l['goals'] and r['moves']==l['moveLimit'] and r['legacyBoardSeed']==l['boardSeed'] and r['legacyId']==l['id'])
check('Current supported goal types in held baseline',all(g['type'] in {'collect_tile','score','clear_obstacle','bring_down','survival'} for r in rows for g in r['goals']))
check('Positive targets; feasible survival budgets',all(g['target']>0 and (g['type']!='survival' or g['target']<=r['moves']) for r in rows for g in r['goals']))
check('Within intended initial move/color bounds',all(20<=r['moves']<=35 and 5<=r['colors']<=6 for r in rows))
check('Recovery follows each new hard except terminal50',all(r['round']==50 or rows[r['round']]['difficulty']=='recovery' for r in rows[10:] if r['difficulty']=='hard'))
budget=read(b/'QA/RewardBudget.json');check('Reward aggregates match',budget['first50']=={k:sum(r['reward'][k] for r in rows) for k in rows[0]['reward']})
map=read(b/'Plan/StoryMapping.json')['existingScenes'];check('All8 legacy scenes mapped',len(map)==8 and len({s['sceneId'] for s in map})==8)
check('Only old finale moved to500',all(s['proposedAfterRound']==(500 if s['sceneId']=='chapter01.finale' else s['legacyAfterRound']) for s in map))
new=read(b/'Plan/NewSceneDrafts.json')['scenes'];check('Six bilingual scene drafts',len(new)==6 and all(l['ru'] and l['en'] for s in new for l in s['lines']))
check('Draft scene IDs unique and separate from legacy',len({s['id'] for s in new})==6 and not ({s['id'] for s in new}&{s['sceneId'] for s in map}))
ru={x['key']:x['value'] for x in read(root/'Assets/LegendsOfRus/Resources/Localization/story_ru.json')['entries']};en={x['key']:x['value'] for x in read(root/'Assets/LegendsOfRus/Resources/Localization/story_en.json')['entries']};loc=read(b/'Plan/LocalizationDraft.json')['entries']
check('All23 actual EN gaps covered once',len(loc)==23 and {x['key'] for x in loc}==set(ru)-set(en))
check('Russian strings preserved; format placeholders equal',all(x['ru']==ru[x['key']] and sorted(re.findall(r'\{\d+\}',x['ru']))==sorted(re.findall(r'\{\d+\}',x['en'])) for x in loc))
for source in read(b/'Sources/Inputs.json'):check('Input unchanged: '+source['path'],hashlib.sha256((root/source['path']).read_bytes()).hexdigest()==source['sha256'])
before=read(b/'Sources/BeforeHashes.json');after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file()}
scope={'unchangedCount':sum(after.get(k)==v for k,v in before.items()),'modified':[k for k,v in before.items() if k in after and after[k]!=v],'deleted':[k for k in before if k not in after],'added':[k for k in after if k not in before]}
check('No Assets/Packages/ProjectSettings changes',not scope['modified'] and not scope['deleted'] and not scope['added'])
(b/'QA/Scope.json').write_text(json.dumps(scope,indent=2)+'\n');(b/'QA/Validation.json').write_text(json.dumps({'passed':True,'checks':checks,'count':len(checks),'unityRun':False,'balanceVerified':False,'newPlayableRounds':0},indent=2)+'\n')
print(f'{len(checks)} static assertions passed; {scope["unchangedCount"]} runtime/project files unchanged. No Unity or balance test.')
