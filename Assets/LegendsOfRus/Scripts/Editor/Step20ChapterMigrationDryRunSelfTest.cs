#if UNITY_EDITOR
using System;
using System.IO;
using System.Security.Cryptography;
using LegendsOfRus.ContentPipeline;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ChapterMigrationDryRunSelfTest
    {
        static int total, passed;
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 2 Self-Tests")]
        public static void Run()
        {
            total=0;passed=0;
            string root=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
            Step20ChapterMigrationDryRunRunner.GenerateDryRun();
            string reportPath=Path.Combine(root,ChapterMigrationPolicy.StagedDryRunReport);
            var evidence=JsonUtility.FromJson<ChapterMigrationDryRunEvidence>(File.ReadAllText(reportPath));
            Check("evidence valid",ChapterMigrationDryRunValidator.ValidateEvidence(evidence).IsStructurallyValid);
            Check("package valid",ChapterMigrationDryRunValidator.ValidatePackage(evidence.Package).IsStructurallyValid);
            Check("three payloads",evidence.Package.CopiedPayloads.Length==3);
            Check("ten levels",evidence.Package.LevelCount==10);
            Check("seven milestones",evidence.Package.StoryMilestoneCount==7);
            Check("localization entries",evidence.Package.LocalizationEntryCount>=1);
            Check("legacy authority",evidence.LegacyRuntimeStillAuthoritative&&evidence.Package.RuntimeStillUsesLegacyResources);
            Check("no runtime cutover",!evidence.RuntimeCutoverPerformed&&!evidence.Package.RuntimeCutoverPerformed);
            Check("no addressables mutation",!evidence.AddressablesMutated&&!evidence.Package.AddressablesMutated);
            Check("no art touch",!evidence.LockedArtTouched&&!evidence.Package.LockedArtTouched);
            Check("no auto art",!evidence.AutomaticArtGenerationUsed);
            Check("no localization rewrite",!evidence.LocalizationRewritten);
            Check("no balance change",!evidence.LevelBalanceChanged&&!evidence.Package.LevelBalanceChanged);
            Check("no reward change",!evidence.RewardsChanged&&!evidence.Package.RewardsChanged);
            Check("no progression change",!evidence.ProgressionChanged);
            Check("no critical blockers",evidence.UnresolvedCriticalBlockers==0);
            Check("report exists",File.Exists(reportPath));
            Check("manifest exists",File.Exists(Path.Combine(root,ChapterMigrationPolicy.StagedPackageManifest)));
            Check("definition exists",File.Exists(Path.Combine(root,ChapterMigrationPolicy.StagedDefinition)));
            Check("staged levels exists",File.Exists(Path.Combine(root,ChapterMigrationPolicy.StagedLevels)));
            Check("staged story exists",File.Exists(Path.Combine(root,ChapterMigrationPolicy.StagedStory)));
            Check("staged localization exists",File.Exists(Path.Combine(root,ChapterMigrationPolicy.StagedLocalization)));
            Check("levels exact",Same(root,ChapterPipelinePolicy.LegacyChapter01Levels,ChapterMigrationPolicy.StagedLevels));
            Check("story exact",Same(root,ChapterPipelinePolicy.LegacyChapter01Story,ChapterMigrationPolicy.StagedStory));
            Check("localization exact",Same(root,ChapterPipelinePolicy.LegacyRussianLocalization,ChapterMigrationPolicy.StagedLocalization));
            Check("levels sha",evidence.Package.CopiedPayloads[0].SourceSha256==Sha(Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Levels)));
            Check("story sha",evidence.Package.CopiedPayloads[1].SourceSha256==Sha(Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Story)));
            Check("loc sha",evidence.Package.CopiedPayloads[2].SourceSha256==Sha(Path.Combine(root,ChapterPipelinePolicy.LegacyRussianLocalization)));
            string activation=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/content_pipeline_activation.json"));
            Check("dry run approved",activation.Contains("\"localChapter01DryRunGenerationApproved\": true"));
            Check("equivalence approved",activation.Contains("\"localByteEquivalenceValidationApproved\": true"));
            Check("migration false",activation.Contains("\"chapterMigrationAuthorized\": false"));
            Check("addressables false",activation.Contains("\"addressablesMutationAuthorized\": false"));
            Check("cutover false",activation.Contains("\"runtimeContentCutoverAuthorized\": false"));
            Check("art false",activation.Contains("\"automaticArtGenerationEnabled\": false"));
            Check("locked overwrite false",activation.Contains("\"lockedArtOverwriteAuthorized\": false"));
            Check("localization mutation false",activation.Contains("\"automaticLocalizationMutationEnabled\": false"));
            Check("balance mutation false",activation.Contains("\"automaticLevelBalanceMutationEnabled\": false"));
            Check("reward mutation false",activation.Contains("\"automaticRewardMutationEnabled\": false"));
            Check("publish false",activation.Contains("\"productionContentPublishAuthorized\": false"));
            Check("play false",activation.Contains("\"playConsoleActionEnabled\": false"));
            string core=ReadDirectory(Path.Combine(root,"Assets/LegendsOfRus/Scripts/ContentPipelineCore"));
            string runner=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step20ChapterMigrationDryRunRunner.cs"));
            string joined=core+runner;
            Check("engine-free core",!core.Contains("using UnityEngine")&&!core.Contains("using UnityEditor"));
            Check("no BuildPlayer",!joined.Contains("BuildPipeline.BuildPlayer"));
            Check("no UnityWebRequest",!joined.Contains("UnityWebRequest"));
            Check("no HttpClient",!joined.Contains("HttpClient"));
            Check("no move asset",!joined.Contains("AssetDatabase.MoveAsset"));
            Check("no addressables entry mutation",!joined.Contains("CreateOrMoveEntry"));
            Check("no file delete",!joined.Contains("File.Delete"));
            Check("no file move",!joined.Contains("File.Move"));
            Check("no reward grant",!joined.Contains("GrantReward"));
            Check("no economy mutation",!joined.Contains("SetCurrency")&&!joined.Contains("AddCurrency"));
            Check("staging under ProjectDocs",ChapterMigrationPolicy.StagingRoot.StartsWith("ProjectDocs/"));
            Check("legacy levels still exist",File.Exists(Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Levels)));
            Check("legacy story still exist",File.Exists(Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Story)));
            Check("legacy loc still exist",File.Exists(Path.Combine(root,ChapterPipelinePolicy.LegacyRussianLocalization)));
            Check("addressables 2.8.1",File.ReadAllText(Path.Combine(root,"Packages/manifest.json")).Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("save v6",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")).Contains("public int saveVersion = 6;"));
            Check("nunit fix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")).Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            Check("content asmdef fix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")).Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("unitytest fix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef")).Contains("TestAssemblies"));
            Check("step18 string hotfix runner",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs")).Contains("+ \"\\n\""));
            Check("step18 string hotfix selftest",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildReleaseCandidateSelfTest.cs")).Contains("\\\"packageLockReviewed\\\""));
            Check("milestone",File.ReadAllText(Path.Combine(root,"MILESTONE_VERSION.txt")).Trim()=="v1.20-step02-canonical-chapter-package-generator-chapter01-nondestructive-migration-dry-run");
            Check("next step3",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_20_STEP03.json")));
            Check("step2 readme",File.Exists(Path.Combine(root,"README_V1_20_STEP02.md")));
            Check("dry run doc",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Documentation/V1_20_STEP02_CHAPTER01_MIGRATION_DRY_RUN.md")));
            Check("staging readme",File.Exists(Path.Combine(root,ChapterMigrationPolicy.StagingRoot,"README.md")));
            Check("definition hash valid",ChapterMigrationPolicy.IsSha256(evidence.Package.CanonicalDefinitionSha256));
            Check("manifest hash valid",ChapterMigrationPolicy.IsSha256(evidence.PackageManifestSha256));
            Check("dry run id",evidence.DryRunId==ChapterMigrationPolicy.DryRunId);
            Check("package id",evidence.Package.PackageId==ChapterMigrationPolicy.PackageId);
            Check("chapter id",evidence.Package.ChapterId==1);
            Check("stable id",evidence.Package.StableId=="chapter01");
            Check("source mode",evidence.Package.SourceMode=="legacy-byte-copy-dry-run");
            Check("human review intentionally false",!evidence.EvidenceReviewed);
            Check("no runtime file staged in Assets",!ChapterMigrationPolicy.StagedLevels.StartsWith("Assets/"));
            Check("no runtime story staged in Assets",!ChapterMigrationPolicy.StagedStory.StartsWith("Assets/"));
            Check("no runtime loc staged in Assets",!ChapterMigrationPolicy.StagedLocalization.StartsWith("Assets/"));
            Check("expected payload count",ChapterMigrationPolicy.ExpectedCopiedPayloadCount==3);
            Check("expected level count",ChapterMigrationPolicy.ExpectedLevelCount==10);
            Check("expected milestone count",ChapterMigrationPolicy.ExpectedStoryMilestoneCount==7);
            if(total!=80) throw new Exception("v1.20 Step 2 self-test count mismatch: "+total+" expected 80");
            Debug.Log("v1.20 Step 2 self-tests passed: "+passed+"/"+total);
        }
        static bool Same(string root,string a,string b){return Sha(Path.Combine(root,a))==Sha(Path.Combine(root,b));}
        static string Sha(string path){using(var sha=SHA256.Create())using(var s=File.OpenRead(path)){return BitConverter.ToString(sha.ComputeHash(s)).Replace("-",string.Empty).ToLowerInvariant();}}
        static string ReadDirectory(string p){string s="";foreach(string f in Directory.GetFiles(p,"*.cs"))s+=File.ReadAllText(f);return s;}
        static void Check(string name,bool ok){total++;if(!ok)throw new Exception("v1.20 Step 2 self-test failed: "+name);passed++;}
    }
}
#endif
