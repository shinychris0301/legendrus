#!/usr/bin/env python3
import hashlib, json, re, sys
from pathlib import Path

CAND=Path(__file__).resolve().parents[1]
BASE=Path(sys.argv[1]).resolve() if len(sys.argv)>1 else None
ALLOW_CHANGED={
 'MILESTONE_VERSION.txt','README.md',
 'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
 'Assets/LegendsOfRus/Documentation/README_FIRST.md',
 'Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_20.json'
}
PROHIBITED=['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','Google.Apis','CreateOrMoveEntry','AssetDatabase.MoveAsset','File.Move(','File.Delete(','GrantReward(','GrantCurrency(','PlayerPrefs.Set']

def sha(p): return hashlib.sha256(p.read_bytes()).hexdigest()
def tree(root): return {str(p.relative_to(root)).replace('\\','/'):sha(p) for p in root.rglob('*') if p.is_file()}
def load(path): return json.loads((CAND/path).read_text(encoding='utf-8'))
def require(ok,msg,failures):
    if not ok: failures.append(msg)

def main():
    failures=[]
    cand=tree(CAND)
    changed=[]; added=[]; deleted=[]
    if BASE:
        base=tree(BASE)
        changed=sorted(p for p in base if p in cand and base[p]!=cand[p])
        added=sorted(p for p in cand if p not in base)
        deleted=sorted(p for p in base if p not in cand)
        require(set(changed)==ALLOW_CHANGED,'unexpected changed inherited paths: '+repr(changed),failures)
        require(not deleted,'deleted inherited paths: '+repr(deleted),failures)
        pngs=[p for p in base if p.lower().endswith('.png')]
        require(len(pngs)==65,'baseline PNG count must be 65',failures)
        require(all(cand.get(p)==base[p] for p in pngs),'approved/runtime PNG bytes changed',failures)
        package_paths=[p for p in base if p.startswith('Packages/')]
        settings_paths=[p for p in base if p.startswith('ProjectSettings/')]
        inherited_tests=[p for p in base if p.startswith('Assets/LegendsOfRus/Tests/')]
        require(all(cand.get(p)==base[p] for p in package_paths),'Packages changed',failures)
        require(all(cand.get(p)==base[p] for p in settings_paths),'ProjectSettings changed',failures)
        require(all(cand.get(p)==base[p] for p in inherited_tests),'inherited tests changed',failures)

    manifest=load('ProjectDocs/ContentPipeline/Templates/chapter_pipeline_manifest.template.json')
    require(manifest.get('SchemaVersion')==1,'manifest schema',failures)
    chapters=manifest.get('Chapters') or []
    require(len(chapters)==20,'manifest must contain 20 chapters',failures)
    for i,ch in enumerate(chapters,1):
        p=f'{i:02d}'
        require(ch.get('ChapterId')==i,f'chapter order {i}',failures)
        require(ch.get('StableId')==f'chapter{p}',f'chapter stable id {i}',failures)
        require(ch.get('CanonicalFolder')==f'Assets/LegendsOfRus/Content/Chapters/CH{p}',f'chapter folder {i}',failures)
        require(ch.get('UsesLegacyChapter01Source')==(i==1),f'legacy boundary {i}',failures)
        require(ch.get('ProductionReady') is False,f'production ready must be false {i}',failures)
        prefix=f'lor.chapter.{p}.'
        for key in ['LevelsAddress','StoryAddress','LocalizationAddress','EnvironmentAddress','MapArtAddress']:
            require(str(ch.get(key,'')).startswith(prefix),f'{key} prefix {i}',failures)

    activation=load('Assets/LegendsOfRus/Resources/Config/content_pipeline_activation.json')
    for key in ['chapterMigrationAuthorized','addressablesMutationAuthorized','runtimeContentCutoverAuthorized','automaticChapterGenerationEnabled','automaticArtGenerationEnabled','lockedArtOverwriteAuthorized','automaticLocalizationMutationEnabled','automaticLevelBalanceMutationEnabled','automaticRewardMutationEnabled','productionContentPublishAuthorized','playConsoleActionEnabled']:
        require(activation.get(key) is False,key+' must be false',failures)
    for key in ['localManifestValidationApproved','localTemplateValidationApproved','localLegacyChapter01ValidationApproved','localOwnershipReviewToolingApproved','localSanitizedSnapshotToolingApproved']:
        require(activation.get(key) is True,key+' must be true',failures)

    core_dir=CAND/'Assets/LegendsOfRus/Scripts/ContentPipelineCore'
    core='\n'.join(p.read_text(encoding='utf-8') for p in core_dir.glob('*.cs'))
    runner=(CAND/'Assets/LegendsOfRus/Scripts/Editor/Step20ChapterPipelineRunner.cs').read_text(encoding='utf-8')
    joined=core+'\n'+runner
    require('using UnityEngine' not in core and 'using UnityEditor' not in core,'core must be engine-free',failures)
    for token in PROHIBITED: require(token not in joined,'prohibited token: '+token,failures)

    tests=(CAND/'Assets/LegendsOfRus/Tests/ContentPipelineEditMode/ContentPipelineCoreTests.cs').read_text(encoding='utf-8')
    selftest=(CAND/'Assets/LegendsOfRus/Scripts/Editor/Step20ChapterPipelineSelfTest.cs').read_text(encoding='utf-8')
    require(len(re.findall(r'\[Test\]',tests))==54,'expected 54 EditMode test declarations',failures)
    require(selftest.count('Check("')==71,'expected 71 self-test checks',failures)
    require('if(total!=71)' in selftest,'self-test expected count not pinned',failures)

    package=json.loads((CAND/'Packages/manifest.json').read_text(encoding='utf-8'))
    require(package.get('dependencies',{}).get('com.unity.addressables')=='2.8.1','Addressables must remain 2.8.1',failures)
    require('public int saveVersion = 6;' in (CAND/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs').read_text(encoding='utf-8'),'save v6 boundary changed',failures)
    require('Array.IndexOf(parameterTypes, typeof(string))' in (CAND/'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs').read_text(encoding='utf-8'),'NUnit compatibility hotfix missing',failures)
    require('LegendsOfRus.ContentDeliveryCore' in (CAND/'Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef').read_text(encoding='utf-8'),'ContentDeliveryCore PlayMode asmdef hotfix missing',failures)
    require('TestAssemblies' in (CAND/'Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef').read_text(encoding='utf-8'),'UnityTest hotfix missing',failures)
    require((CAND/'MILESTONE_VERSION.txt').read_text().strip()=='v1.20-step01-chapter-pipeline-contract-template-validation-foundation','milestone mismatch',failures)

    result={
      'passed':not failures,'version':'v1.20-step01','candidateFiles':len(cand),
      'baselineFiles':len(tree(BASE)) if BASE else None,'changedInheritedPaths':changed,'addedPaths':added,'deletedPaths':deleted,
      'approvedPngFiles':65,'editModeTestDeclarations':54,'editorSelfTestChecks':71,'failures':failures
    }
    print(json.dumps(result,indent=2))
    return 0 if not failures else 1

if __name__=='__main__': raise SystemExit(main())
