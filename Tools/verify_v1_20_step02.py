#!/usr/bin/env python3
from pathlib import Path
import hashlib, json, sys
root=Path(__file__).resolve().parents[1]
fail=[]
def check(ok,msg):
    if not ok: fail.append(msg)
def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()
# required files
req=[
'Assets/LegendsOfRus/Scripts/ContentPipelineCore/ChapterMigrationPolicy.cs',
'Assets/LegendsOfRus/Scripts/ContentPipelineCore/ChapterMigrationTypes.cs',
'Assets/LegendsOfRus/Scripts/ContentPipelineCore/ChapterMigrationBlockerCodes.cs',
'Assets/LegendsOfRus/Scripts/ContentPipelineCore/ChapterMigrationDryRunValidator.cs',
'Assets/LegendsOfRus/Scripts/Editor/Step20ChapterMigrationDryRunRunner.cs',
'Assets/LegendsOfRus/Scripts/Editor/Step20ChapterMigrationDryRunSelfTest.cs',
'Assets/LegendsOfRus/Tests/ContentPipelineEditMode/ChapterMigrationDryRunEditModeTests.cs',
'ProjectDocs/ContentPipeline/Staging/CH01/chapter01_levels.json',
'ProjectDocs/ContentPipeline/Staging/CH01/chapter01_story.json',
'ProjectDocs/ContentPipeline/Staging/CH01/story_ru.json',
'ProjectDocs/ContentPipeline/Staging/CH01/chapter_definition.json',
'ProjectDocs/ContentPipeline/Staging/CH01/chapter_package_manifest.json',
'ProjectDocs/ContentPipeline/Staging/CH01/migration_dry_run_report.json']
for r in req: check((root/r).is_file(),'missing:'+r)
# JSON parse
for p in root.rglob('*.json'):
    try: json.loads(p.read_text(encoding='utf-8'))
    except Exception as e: fail.append('json:'+p.relative_to(root).as_posix()+':'+str(e))
# exact payload equality
pairs=[
('Assets/LegendsOfRus/Resources/Config/chapter01_levels.json','ProjectDocs/ContentPipeline/Staging/CH01/chapter01_levels.json'),
('Assets/LegendsOfRus/Resources/Config/chapter01_story.json','ProjectDocs/ContentPipeline/Staging/CH01/chapter01_story.json'),
('Assets/LegendsOfRus/Resources/Localization/story_ru.json','ProjectDocs/ContentPipeline/Staging/CH01/story_ru.json')]
for a,b in pairs:
    check((root/a).read_bytes()==(root/b).read_bytes(),'byte_mismatch:'+a)
# evidence hashes
pkg=json.loads((root/'ProjectDocs/ContentPipeline/Staging/CH01/chapter_package_manifest.json').read_text())
rep=json.loads((root/'ProjectDocs/ContentPipeline/Staging/CH01/migration_dry_run_report.json').read_text())
check(pkg.get('LevelCount')==10,'level_count')
check(pkg.get('StoryMilestoneCount')==7,'milestone_count')
check(pkg.get('LocalizationEntryCount')==49,'localization_count')
check(pkg.get('RuntimeStillUsesLegacyResources') is True,'legacy_authority')
for k in ['RuntimeCutoverPerformed','AddressablesMutated','LockedArtTouched','LevelBalanceChanged','RewardsChanged']:
    check(pkg.get(k) is False,'package_gate:'+k)
for k in ['RuntimeCutoverPerformed','AddressablesMutated','LockedArtTouched','AutomaticArtGenerationUsed','LocalizationRewritten','LevelBalanceChanged','RewardsChanged','ProgressionChanged']:
    check(rep.get(k) is False,'report_gate:'+k)
for e in pkg.get('CopiedPayloads',[]):
    check(e.get('ExactByteMatch') is True,'not_exact:'+str(e.get('LogicalName')))
    check(e.get('SourceSha256')==e.get('StagedSha256'),'hash_mismatch:'+str(e.get('LogicalName')))
    check(sha(root/e['SourcePath'])==e['SourceSha256'],'source_hash_stale:'+e['LogicalName'])
    check(sha(root/e['StagedPath'])==e['StagedSha256'],'staged_hash_stale:'+e['LogicalName'])
# activation
act=json.loads((root/'Assets/LegendsOfRus/Resources/Config/content_pipeline_activation.json').read_text())
check(act.get('localChapter01DryRunGenerationApproved') is True,'dryrun_not_approved')
check(act.get('localByteEquivalenceValidationApproved') is True,'equivalence_not_approved')
for k in ['chapterMigrationAuthorized','addressablesMutationAuthorized','runtimeContentCutoverAuthorized','automaticChapterGenerationEnabled','automaticArtGenerationEnabled','lockedArtOverwriteAuthorized','automaticLocalizationMutationEnabled','automaticLevelBalanceMutationEnabled','automaticRewardMutationEnabled','productionContentPublishAuthorized','playConsoleActionEnabled']:
    check(act.get(k) is False,'activation_gate:'+k)
# source safety
newsrc=''.join((root/r).read_text(encoding='utf-8') for r in req[:5])
for token in ['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','AssetDatabase.MoveAsset','CreateOrMoveEntry','File.Delete','File.Move','GrantReward']:
    check(token not in newsrc,'banned:'+token)
core=''.join(p.read_text(encoding='utf-8') for p in (root/'Assets/LegendsOfRus/Scripts/ContentPipelineCore').glob('ChapterMigration*.cs'))
check('using UnityEngine' not in core and 'using UnityEditor' not in core,'core_not_engine_free')
# test/selftest inventory
nunit=(root/'Assets/LegendsOfRus/Tests/ContentPipelineEditMode/ChapterMigrationDryRunEditModeTests.cs').read_text().count('[Test]')
selfchecks=(root/'Assets/LegendsOfRus/Scripts/Editor/Step20ChapterMigrationDryRunSelfTest.cs').read_text().count('Check("')
check(nunit==50,'nunit_count:'+str(nunit))
check(selfchecks==80,'selftest_count:'+str(selfchecks))
# retained hotfix05 signatures
check('+ "\\n"' in (root/'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs').read_text(),'hotfix05_runner_missing')
check('\\"packageLockReviewed\\"' in (root/'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildReleaseCandidateSelfTest.cs').read_text(),'hotfix05_selftest_missing')
# milestone and next
check((root/'MILESTONE_VERSION.txt').read_text().strip()=='v1.20-step02-canonical-chapter-package-generator-chapter01-nondestructive-migration-dry-run','milestone')
next3=json.loads((root/'Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_20_STEP03.json').read_text())
check(next3.get('milestone')=='production-ui-asset-reconstruction-and-screenshot-removal','next_step3')
report={'version':'v1.20-step02','passed':not fail,'failures':fail,'nunitTests':nunit,'editorSelfTestChecks':selfchecks,'copiedPayloads':3,'levelCount':pkg.get('LevelCount'),'storyMilestoneCount':pkg.get('StoryMilestoneCount'),'localizationEntryCount':pkg.get('LocalizationEntryCount')}
print(json.dumps(report,indent=2))
sys.exit(0 if not fail else 1)
