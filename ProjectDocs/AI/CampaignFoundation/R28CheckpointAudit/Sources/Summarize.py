from pathlib import Path
import json,hashlib,difflib,xml.etree.ElementTree as ET
base=Path(__file__).resolve().parent.parent;root=base.parents[3]
p=base/'QA/Verify01/Checkpoint/results.xml';tree=ET.parse(p).getroot();a=tree.attrib
assert a['result']=='Passed' and int(a['passed'])==16 and int(a['failed'])==int(a['skipped'])==0
names=[t.attrib['fullname'] for t in tree.iter('test-case')];assert len(names)==len(set(names))==16
report=dict(unity='6000.5.9f1',passed=16,failed=0,skipped=0,newFailureRetryCases=3,existingCheckpointCases=13,durationSeconds=float(a['duration']),results=str(p.relative_to(base)),scope='Service-level persisted checkpoint lifecycle, explicit earlier-progress/loss fixtures; not board-play or human difficulty',rendering='Prior authorized process-only Vulkan and descriptor4096; project settings unchanged',saveStorage='Isolated test store; live player saves untouched',notRun=['Full project suite','R27 completion replays','Android device/build','Human31–150 difficulty','Visual/pointer acceptance','Draft151–160 Unity validation'])
(base/'QA/Verification.json').write_text(json.dumps(report,indent=2)+'\n')
active=root/'Assets/LegendsOfRus/Resources/CampaignActive';candidate=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector03R27'
files=['levels.json','story.json','story_ru.json','story_en.json']
assert all((active/n).read_bytes()==(candidate/n).read_bytes() for n in files)
prior=base.parent/'R27MillCheckpoint';old=json.loads((prior/'QA/Verification.json').read_text());assert old['totalPassed']==234
counts={}
for suite in old['suites']:
 attrs=ET.parse(prior/suite['results']).getroot().attrib;assert attrs['result']=='Passed' and int(attrs['passed'])==suite['passed'];counts[suite['name']]=int(attrs['passed'])
difficulty=json.loads((prior/'QA/DifficultyScreening.json').read_text());assert difficulty['completionWitnesses']==450 and len(difficulty['rows'])==150
(base/'QA/RetainedEvidence.json').write_text(json.dumps(dict(source='../R27MillCheckpoint',rerun=False,suites=counts,completionWitnesses=450,meaning='Prior legal replay solutions, not human win rates',activeCandidateByteEquality=True,activeHashes={n:hashlib.sha256((active/n).read_bytes()).hexdigest() for n in files}),indent=2)+'\n')
p=root/'Assets/LegendsOfRus/Tests/CampaignPlayMode/CampaignCheckpointLifecycleTests.cs';before=(base/'Sources/CampaignCheckpointLifecycleTests.before.cs').read_text()
(base/'Sources/TestChanges.patch').write_text(''.join(difflib.unified_diff(before.splitlines(True),p.read_text().splitlines(True),fromfile='before/CampaignCheckpointLifecycleTests.cs',tofile='after/CampaignCheckpointLifecycleTests.cs')))
print(json.dumps(report,indent=2))
