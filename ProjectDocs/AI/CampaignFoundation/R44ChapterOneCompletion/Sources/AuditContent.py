from pathlib import Path
import json, hashlib
ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
ACTIVE=ROOT/'Assets/LegendsOfRus/Resources/CampaignActive'
read=lambda p:json.loads(p.read_text())
levels=read(ACTIVE/'levels.json')['levels'];story=read(ACTIVE/'story.json')
old_levels=read(BASE/'Sources/levels.before.json')['levels'];old_story=read(BASE/'Sources/story.before.json')
assert [l['levelId'] for l in levels]==list(range(1,501))
assert levels[:200]==old_levels
for key in ['scenes','milestones','reactions']:assert story[key][:len(old_story[key])]==old_story[key]
assert story['introSceneId']==old_story['introSceneId']
assert len(story['scenes'])==58 and len(story['milestones'])==57 and len(story['reactions'])==1000
assert story['completion']['finalLevelId']==500
assert [l['levelId'] for l in levels if 'chapter.complete' in l['storyTriggers']]==[500]
scenes={s['id']:s for s in story['scenes']}
assert len(scenes)==len(story['scenes'])
assert set(scenes)=={story['introSceneId']}|{m['sceneId'] for m in story['milestones']}
assert [m['afterLevelId'] for m in story['milestones']]==sorted(set(m['afterLevelId'] for m in story['milestones']))
for m in story['milestones']:assert scenes[m['sceneId']]['trigger'] in levels[m['afterLevelId']-1]['storyTriggers']
for n in range(1,501):assert sorted(r['outcome'] for r in story['reactions'] if r['levelId']==n)==['failure','victory']
required={line[k] for s in scenes.values() for line in s['lines'] for k in ['speakerKey','textKey']}
required|={r[k] for r in story['reactions'] for k in ['speakerKey','textKey']}
required|={story['completion'][k] for k in ['summaryTitleKey','summaryFormatKey','rewardNoteKey']}
required|={f'campaign.round{n:03}.name' for n in range(11,501)}
for lang in ['ru','en']:
    entries=read(ACTIVE/f'story_{lang}.json')['entries'];values={e['key']:e['value'] for e in entries}
    assert len(values)==len(entries) and all(v.strip() for v in values.values())
    assert all(values.get(k,'').strip() for k in required)
    old=read(BASE/f'Sources/story_{lang}.before.json')['entries']
    assert all(values[e['key']]==e['value'] for e in old)
briefs=read(BASE/'RoundBriefs.json')['rows'];assert len(briefs)==300
for b in briefs:assert b['level']==levels[b['round']-1]
candidate=ROOT/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01CompleteR44'
for name in ['levels','story','story_ru','story_en']:assert (candidate/(name+'.json')).read_bytes()==(ACTIVE/(name+'.json')).read_bytes()
sectors=[]
for first in range(201,501,50):
    ls=levels[first-1:first+49]
    sectors.append({'first':first,'last':first+49,'moveLimits':sorted({l['moveLimit'] for l in ls}),
        'rewards':{k:sum(l['rewards'][k] for l in ls) for k in ls[0]['rewards']}})
report={'passed':True,'installedRounds':500,'preservedRounds':200,'preservedLocaleEntriesPerLanguage':632,
    'scenes':58,'milestones':57,'reactions':1000,'explicitRequiredKeys':len(required),'localeEntriesPerLanguage':len(entries),
    'finaleRound':500,'chapter2Installed':False,'newRoundBriefs':300,'sectors':sectors,
    'scope':'Source checks and candidate parity. No human difficulty or device claim.'}
(BASE/'QA/ContentAudit.json').write_text(json.dumps(report,indent=2)+'\n')
print(json.dumps(report,indent=2))
