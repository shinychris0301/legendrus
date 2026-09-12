#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.ContentPipeline;

namespace LegendsOfRus.EditorTools
{
    public static class Step20ChapterPipelineSelfTest
    {
        static int total,passed;
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 1 Self-Tests")]
        public static void Run()
        {
            total=passed=0;
            var m=Manifest(); var t=Template(); var e=Evidence();
            Check("manifest healthy",ChapterPipelineValidator.ValidateManifest(m).IsValid);
            Check("template healthy",ChapterPipelineValidator.ValidateTemplate(t).IsValid);
            Check("evidence healthy",ContentPipelineTransitionEvaluator.ValidateEvidence(e).IsValid);
            Check("chapter count 20",m.Chapters.Length==20);
            Check("first stable id",m.Chapters[0].StableId=="chapter01");
            Check("last stable id",m.Chapters[19].StableId=="chapter20");
            Check("chapter1 legacy",m.Chapters[0].UsesLegacyChapter01Source);
            Check("chapter2 not legacy",!m.Chapters[1].UsesLegacyChapter01Source);
            Check("chapter folder",ChapterPipelinePolicy.ChapterFolder(8)=="Assets/LegendsOfRus/Content/Chapters/CH08");
            Check("address prefix",ChapterPipelinePolicy.ChapterAddressPrefix(8)=="lor.chapter.08.");
            Check("sha valid",ChapterPipelinePolicy.IsSha256(new string('a',64)));
            Check("sha invalid",!ChapterPipelinePolicy.IsSha256("bad"));
            var badCount=Manifest();badCount.ExpectedChapterCount=19;Check("manifest count reject",Has(ChapterPipelineValidator.ValidateManifest(badCount).Blockers,ChapterPipelineBlockerCodes.ChapterCountInvalid));
            var badOrder=Manifest();badOrder.Chapters[2].ChapterId=4;Check("order reject",Has(ChapterPipelineValidator.ValidateManifest(badOrder).Blockers,ChapterPipelineBlockerCodes.ChapterOrderInvalid));
            var badStable=Manifest();badStable.Chapters[1].StableId="ch2";Check("stable id reject",Has(ChapterPipelineValidator.ValidateManifest(badStable).Blockers,ChapterPipelineBlockerCodes.StableIdInvalid));
            var badAddr=Manifest();badAddr.Chapters[1].LevelsAddress="lor.chapter.03.levels";Check("address prefix reject",Has(ChapterPipelineValidator.ValidateManifest(badAddr).Blockers,ChapterPipelineBlockerCodes.AddressPrefixInvalid));
            var dupAddr=Manifest();dupAddr.Chapters[1].LevelsAddress=dupAddr.Chapters[0].LevelsAddress;Check("address duplicate reject",Has(ChapterPipelineValidator.ValidateManifest(dupAddr).Blockers,ChapterPipelineBlockerCodes.AddressDuplicate));
            var badFolder=Manifest();badFolder.Chapters[2].CanonicalFolder="bad";Check("folder reject",Has(ChapterPipelineValidator.ValidateManifest(badFolder).Blockers,ChapterPipelineBlockerCodes.CanonicalFolderInvalid));
            var badLegacy=Manifest();badLegacy.Chapters[1].UsesLegacyChapter01Source=true;Check("legacy boundary reject",Has(ChapterPipelineValidator.ValidateManifest(badLegacy).Blockers,ChapterPipelineBlockerCodes.LegacySourceBoundaryInvalid));
            var prod=Manifest();prod.Chapters[1].ProductionReady=true;Check("ownership required when production",Has(ChapterPipelineValidator.ValidateManifest(prod).Blockers,ChapterPipelineBlockerCodes.OwnershipIncomplete));
            var badLevelCount=Template();badLevelCount.LevelCount=0;Check("template level count reject",Has(ChapterPipelineValidator.ValidateTemplate(badLevelCount).Blockers,ChapterPipelineBlockerCodes.TemplateLevelCountInvalid));
            var badFinale=Template();badFinale.FinaleLevelId=11;Check("template finale reject",Has(ChapterPipelineValidator.ValidateTemplate(badFinale).Blockers,ChapterPipelineBlockerCodes.TemplateFinaleInvalid));
            var badMilestone=Template();badMilestone.StoryMilestoneLevels=new[]{1,3,3,10};Check("template milestones reject",Has(ChapterPipelineValidator.ValidateTemplate(badMilestone).Blockers,ChapterPipelineBlockerCodes.TemplateMilestoneInvalid));
            var missingReview=Evidence();missingReview.MigrationPlanReviewed=false;Check("migration evidence reject",Has(ContentPipelineTransitionEvaluator.ValidateEvidence(missingReview).Blockers,ChapterPipelineBlockerCodes.MigrationPlanMissing));
            var critical=Evidence();critical.UnresolvedCriticalBlockers=1;Check("critical blocker reject",Has(ContentPipelineTransitionEvaluator.ValidateEvidence(critical).Blockers,ChapterPipelineBlockerCodes.CriticalBlockersOpen));
            var gated=ContentPipelineTransitionEvaluator.Evaluate(m,t,e,false,false,false,false,false);
            Check("review structurally ready",gated.ReadyForHumanReview);
            Check("migration blocked",!gated.MayBeginChapterMigration);
            Check("migration action blocker",Has(gated.ActionBlockers,ChapterPipelineBlockerCodes.ChapterMigrationNotAuthorized));
            Check("addressables action blocker",Has(gated.ActionBlockers,ChapterPipelineBlockerCodes.AddressablesMutationNotAuthorized));
            Check("runtime cutover blocker",Has(gated.ActionBlockers,ChapterPipelineBlockerCodes.RuntimeCutoverNotAuthorized));
            Check("art gen blocker",Has(gated.ActionBlockers,ChapterPipelineBlockerCodes.AutomaticArtGenerationNotAuthorized));
            Check("art overwrite blocker",Has(gated.ActionBlockers,ChapterPipelineBlockerCodes.LockedArtOverwriteNotAuthorized));

            string root=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
            string activation=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Resources/Config/content_pipeline_activation.json"));
            string core=ReadDirectory(Path.Combine(root,"Assets/LegendsOfRus/Scripts/ContentPipelineCore"));
            string runner=File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step20ChapterPipelineRunner.cs"));
            string joined=core+runner;
            Check("core asmdef",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/ContentPipelineCore/LegendsOfRus.ContentPipelineCore.asmdef")));
            Check("runner exists",File.Exists(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Editor/Step20ChapterPipelineRunner.cs")));
            Check("manifest template",File.Exists(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/chapter_pipeline_manifest.template.json")));
            Check("chapter template",File.Exists(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/chapter_definition.template.json")));
            Check("level template",File.Exists(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/chapter_levels.template.json")));
            Check("story template",File.Exists(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/chapter_story.template.json")));
            Check("localization template",File.Exists(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/chapter_localization_ru.template.json")));
            Check("transition template",File.Exists(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/v120_step01_transition_evidence.template.json")));
            Check("migration plan",File.Exists(Path.Combine(root,"ProjectDocs/ContentPipeline/MIGRATION_PLAN.md")));
            Check("legacy levels remain",File.Exists(Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Levels)));
            Check("legacy story remains",File.Exists(Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Story)));
            Check("legacy localization remains",File.Exists(Path.Combine(root,ChapterPipelinePolicy.LegacyRussianLocalization)));
            Check("activation migration false",activation.Contains("\"chapterMigrationAuthorized\": false"));
            Check("activation addressables false",activation.Contains("\"addressablesMutationAuthorized\": false"));
            Check("activation runtime cutover false",activation.Contains("\"runtimeContentCutoverAuthorized\": false"));
            Check("activation auto chapter false",activation.Contains("\"automaticChapterGenerationEnabled\": false"));
            Check("activation auto art false",activation.Contains("\"automaticArtGenerationEnabled\": false"));
            Check("activation locked overwrite false",activation.Contains("\"lockedArtOverwriteAuthorized\": false"));
            Check("activation localization false",activation.Contains("\"automaticLocalizationMutationEnabled\": false"));
            Check("activation level balance false",activation.Contains("\"automaticLevelBalanceMutationEnabled\": false"));
            Check("activation reward false",activation.Contains("\"automaticRewardMutationEnabled\": false"));
            Check("activation publish false",activation.Contains("\"productionContentPublishAuthorized\": false"));
            Check("activation play false",activation.Contains("\"playConsoleActionEnabled\": false"));
            Check("local validation true",activation.Contains("\"localManifestValidationApproved\": true"));
            Check("banned BuildPipeline.BuildPlayer",!joined.Contains("BuildPipeline.BuildPlayer"));
            Check("banned UnityWebRequest",!joined.Contains("UnityWebRequest"));
            Check("banned HttpClient",!joined.Contains("HttpClient"));
            Check("banned AddressableAssetSettings.CreateOrMoveEntry",!joined.Contains("CreateOrMoveEntry"));
            Check("banned AssetDatabase.MoveAsset",!joined.Contains("AssetDatabase.MoveAsset"));
            Check("banned File.Move",!joined.Contains("File.Move"));
            Check("banned File.Delete",!joined.Contains("File.Delete"));
            Check("banned GrantReward",!joined.Contains("GrantReward"));
            Check("engine free core",!core.Contains("using UnityEngine")&&!core.Contains("using UnityEditor"));
            Check("addressables 2.8.1",File.ReadAllText(Path.Combine(root,"Packages/manifest.json")).Contains("\"com.unity.addressables\": \"2.8.1\""));
            Check("save v6",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs")).Contains("public int saveVersion = 6;"));
            Check("nunit hotfix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs")).Contains("Array.IndexOf(parameterTypes, typeof(string))"));
            Check("content asmdef hotfix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef")).Contains("LegendsOfRus.ContentDeliveryCore"));
            Check("unitytest hotfix",File.ReadAllText(Path.Combine(root,"Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef")).Contains("TestAssemblies"));
            Check("milestone",File.ReadAllText(Path.Combine(root,"MILESTONE_VERSION.txt")).Trim()=="v1.20-step01-chapter-pipeline-contract-template-validation-foundation");
            if(total!=71)throw new Exception("v1.20 Step 1 self-test count mismatch: "+total+" expected 71");
            Debug.Log("v1.20 Step 1 self-tests passed: "+passed+"/"+total);
        }

        static ChapterPipelineManifest Manifest(){var m=new ChapterPipelineManifest{ManifestId="LOR-V120-MANIFEST",ExpectedChapterCount=20,Chapters=new ChapterPipelineEntry[20]};for(int i=1;i<=20;i++){string p=i.ToString("00");m.Chapters[i-1]=new ChapterPipelineEntry{ChapterId=i,StableId="chapter"+p,DisplayNameKey="chapter."+p+".title",LevelsAddress="lor.chapter."+p+".levels",StoryAddress="lor.chapter."+p+".story",LocalizationAddress="lor.chapter."+p+".localization.ru",EnvironmentAddress="lor.chapter."+p+".environment",MapArtAddress="lor.chapter."+p+".map_art",CanonicalFolder=ChapterPipelinePolicy.ChapterFolder(i),UsesLegacyChapter01Source=i==1};}return m;}
        static ChapterTemplateDefinition Template(){return new ChapterTemplateDefinition{ChapterId=2,StableId="chapter02",DisplayNameKey="chapter.02.title",LevelCount=10,FirstPlayableLevel=1,FinaleLevelId=10,StoryMilestoneLevels=new[]{1,3,6,10},LevelsAddress="lor.chapter.02.levels",StoryAddress="lor.chapter.02.story",LocalizationAddress="lor.chapter.02.localization.ru",EnvironmentAddress="lor.chapter.02.environment",MapArtAddress="lor.chapter.02.map_art"};}
        static ContentPipelineTransitionEvidence Evidence(){return new ContentPipelineTransitionEvidence{EvidenceRetained=true,EvidenceReviewed=true,TransitionId="LOR-V120-STEP01",SourceTreeSha256=new string('a',64),LegacyChapter01LevelsValidated=true,LegacyChapter01StoryValidated=true,LegacyRussianLocalizationValidated=true,ManifestContractReviewed=true,TemplateContractReviewed=true,StableIdConventionReviewed=true,AddressablesNamespaceReviewed=true,ContentOwnershipReviewed=true,LocalizationKeyConventionReviewed=true,QaAutomationReviewed=true,MigrationPlanReviewed=true,LockedArtNonDestructiveBoundaryReviewed=true};}
        static bool Has(string[] a,string v){if(a==null)return false;for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
        static string ReadDirectory(string p){string s="";foreach(string f in Directory.GetFiles(p,"*.cs"))s+=File.ReadAllText(f);return s;}
        static void Check(string name,bool ok){total++;if(!ok)throw new Exception("v1.20 Step 1 self-test failed: "+name);passed++;}
    }
}
#endif
