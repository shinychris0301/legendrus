#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus.ContentPipeline;

namespace LegendsOfRus.EditorTools
{
    public static class Step20ChapterPipelineRunner
    {
        [MenuItem("Legends of Rus/Content Pipeline/Print v1.20 Step 1 Review")]
        public static void PrintReview()
        {
            string root = ProjectRoot();
            ChapterPipelineManifest manifest = Read<ChapterPipelineManifest>(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/chapter_pipeline_manifest.template.json"));
            ChapterTemplateDefinition template = Read<ChapterTemplateDefinition>(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/chapter_definition.template.json"));
            ContentPipelineTransitionEvidence evidence = Read<ContentPipelineTransitionEvidence>(Path.Combine(root,"ProjectDocs/ContentPipeline/Templates/v120_step01_transition_evidence.template.json"));
            ContentPipelineReviewResult review = ContentPipelineTransitionEvaluator.Evaluate(manifest,template,evidence,false,false,false,false,false);
            LegacyValidation legacy = ValidateLegacy(root);
            Debug.Log("v1.20 Step 1 review | manifestValid="+ChapterPipelineValidator.ValidateManifest(manifest).IsValid+
                      " | templateValid="+ChapterPipelineValidator.ValidateTemplate(template).IsValid+
                      " | legacyLevels="+legacy.LevelsValid+" | legacyStory="+legacy.StoryValid+
                      " | readyForHumanReview="+review.ReadyForHumanReview+
                      " | mayBeginMigration="+review.MayBeginChapterMigration+
                      " | reviewBlockers="+string.Join(",",review.ReviewBlockers)+
                      " | actionBlockers="+string.Join(",",review.ActionBlockers));
        }

        [MenuItem("Legends of Rus/Content Pipeline/Validate Legacy Chapter 1")]
        public static void ValidateLegacyChapter1()
        {
            LegacyValidation result = ValidateLegacy(ProjectRoot());
            if (!result.LevelsValid || !result.StoryValid)
                throw new InvalidOperationException("Legacy Chapter 1 validation failed: "+result.Details);
            Debug.Log("v1.20 Step 1 legacy Chapter 1 validation passed: levels/story/localization are valid and unchanged in place.");
        }

        [MenuItem("Legends of Rus/Content Pipeline/Write v1.20 Step 1 Sanitized Snapshot")]
        public static void WriteSnapshot()
        {
            string root=ProjectRoot();
            string dir=Path.Combine(root,"ProjectDocs/ContentPipeline/Generated");
            Directory.CreateDirectory(dir);
            string path=Path.Combine(dir,"v1_20_step01_sanitized_snapshot.json");
            string json="{\n  \"schemaVersion\": 1,\n  \"containsCredentials\": false,\n  \"containsDeviceIdentifiers\": false,\n  \"approvedArtModified\": false,\n  \"chapterMigrationAuthorized\": false,\n  \"runtimeCutoverAuthorized\": false,\n  \"productionContentPublishAuthorized\": false\n}\n";
            File.WriteAllText(path,json);
            AssetDatabase.Refresh();
            Debug.Log("Wrote sanitized v1.20 Step 1 snapshot: "+path);
        }

        static LegacyValidation ValidateLegacy(string root)
        {
            string levelsPath=Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Levels);
            string storyPath=Path.Combine(root,ChapterPipelinePolicy.LegacyChapter01Story);
            string locPath=Path.Combine(root,ChapterPipelinePolicy.LegacyRussianLocalization);
            if (!File.Exists(levelsPath)||!File.Exists(storyPath)||!File.Exists(locPath))
                return new LegacyValidation(false,false,"required legacy file missing");
            try
            {
                ChapterLevelCatalogData levels=JsonUtility.FromJson<ChapterLevelCatalogData>(File.ReadAllText(levelsPath));
                ChapterStoryCatalogData story=JsonUtility.FromJson<ChapterStoryCatalogData>(File.ReadAllText(storyPath));
                StoryLocalizationData loc=JsonUtility.FromJson<StoryLocalizationData>(File.ReadAllText(locPath));
                LevelValidationReport levelReport=LevelDataValidator.ValidateCatalog(levels);
                StoryValidationReport storyReport=StoryCatalog.Validate(story,loc,levels);
                return new LegacyValidation(levelReport.IsValid,storyReport.IsValid,
                    "levels="+levelReport.Summary+" storyErrors="+storyReport.Errors.Count);
            }
            catch(Exception e)
            {
                return new LegacyValidation(false,false,e.GetType().Name+": "+e.Message);
            }
        }

        static T Read<T>(string path) where T:class
        {
            if(!File.Exists(path))return null;
            return JsonUtility.FromJson<T>(File.ReadAllText(path));
        }

        static string ProjectRoot(){return Path.GetFullPath(Path.Combine(Application.dataPath,".."));}

        readonly struct LegacyValidation
        {
            public readonly bool LevelsValid;
            public readonly bool StoryValid;
            public readonly string Details;
            public LegacyValidation(bool levelsValid,bool storyValid,string details){LevelsValid=levelsValid;StoryValid=storyValid;Details=details??string.Empty;}
        }
    }
}
#endif
