#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class ProjectValidator
    {
        [MenuItem("Legends of Rus/Validate Commercial Vertical Slice")]
        public static void Validate()
        {
            string[] required = {
                "Assets/LegendsOfRus/Resources/DesignScreens/Home.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/WorldMap.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/ChapterMap.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/PreLevel.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Gameplay.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Victory.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Fail.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Shop.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Pause.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/OutOfEnergy.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Settings.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/RewardClaim.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Events.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Characters.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Mail.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/Profile.png",
                "Assets/LegendsOfRus/Resources/DesignScreens/ChapterComplete.png",
                "Assets/LegendsOfRus/Resources/Gameplay/Gems/Gem_Ruby.png",
                "Assets/LegendsOfRus/Resources/Gameplay/Gems/Gem_Sapphire.png",
                "Assets/LegendsOfRus/Resources/Config/chapter01.json",
                "Assets/LegendsOfRus/Resources/Config/chapter01_levels.json",
                "Assets/LegendsOfRus/Resources/Config/chapter01_story.json",
                "Assets/LegendsOfRus/Resources/Localization/story_ru.json",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelData.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelMechanicData.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelBoardFactory.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelTutorialSequence.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Chapter01ProgressionValidator.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs",
                "Assets/LegendsOfRus/Scripts/Editor/ChapterLevelAuthoringWindow.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step11LevelSystemSelfTest.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step12GoalObstacleFoundationSelfTest.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step12ObstacleGoalRuntimeSelfTest.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step12Chapter01CurveSelfTest.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step13StoryDataSelfTest.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step13StoryMilestoneReactionSelfTest.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step13ChapterFinaleCompletionSelfTest.cs",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_12_STEP03.json",
                "Assets/LegendsOfRus/Documentation/V1_12_STEP03_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Documentation/V1_12_STEP03_CHAPTER01_MECHANIC_CURVE.md",
                "Assets/LegendsOfRus/Documentation/V1_12_STEP03_BLOCKED_SWAP_PHASE_FIX.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_12_STEP03.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_13_STEP01.json",
                "Assets/LegendsOfRus/Documentation/V1_13_STEP01_STORY_DATA_CHAPTER_INTRO.md",
                "Assets/LegendsOfRus/Documentation/V1_13_STEP01_UNITY_COMPILE_FIX.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_13_STEP01.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_13_STEP02.json",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_13_STEP01.json",
                "Assets/LegendsOfRus/Documentation/V1_13_STEP01_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Documentation/V1_13_STEP02_STORY_MILESTONES_CHARACTER_REACTIONS.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_13_STEP02.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_13_STEP03.json",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_13_STEP02.json",
                "Assets/LegendsOfRus/Documentation/V1_13_STEP02_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Documentation/V1_13_STEP03_CHAPTER_FINALE_COMPLETION_FLOW.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_13_STEP03.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_14_STEP01.json",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_13_STEP03.json",
                "Assets/LegendsOfRus/Documentation/V1_13_STEP03_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP01_CORE_LOOP_MOTION_FEEDBACK_FOUNDATION.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_14_STEP01.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_14_STEP02.json",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_14_STEP01.json",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP01_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopMotionProfile.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/GameplayMotionEvent.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step14CoreLoopMotionFeedbackSelfTest.cs",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP02_CORE_LOOP_VFX_BOOSTER_FEEDBACK.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_14_STEP02.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_14_STEP03.json",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_14_STEP02.json",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP02_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxCue.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxProfile.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/PooledUiVfxLayer.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/CoreLoopVfxPresenter.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step14CoreLoopVfxBoosterFeedbackSelfTest.cs",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP03_META_UI_MOTION_TRANSITION_POLISH.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_14_STEP03.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_14_STEP04.json",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_14_STEP03.json",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP03_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step14MetaUiMotionTransitionPolishSelfTest.cs",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP04_AUDIO_HAPTICS_FIDELITY_VERIFICATION.md",
                "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_14_STEP04.json",
                "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_15_STEP01.json",
                "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_14_STEP04.json",
                "Assets/LegendsOfRus/Documentation/V1_14_STEP04_CHANGED_FILES.txt",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/UI/AdaptiveCanvasScaler.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/UI/SafeAreaFitter.cs",
                "Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs",
                "Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs",
                "Assets/LegendsOfRus/Scripts/Editor/Step14AudioHapticsFidelityVerificationSelfTest.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/LegendsOfRus.GameplayCore.asmdef",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardDirection.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/GameplayStateMachine.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardPrimitives.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardMechanicPrimitives.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardRandom.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardModel.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardReplayCodec.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardResolutionPrimitives.cs",
                "Assets/LegendsOfRus/Scripts/GameplayCore/BoardResolutionPlanner.cs",
                "Assets/LegendsOfRus/Scripts/Runtime/Gameplay/LevelGoalRuntime.cs",
                "Assets/LegendsOfRus/Tests/EditMode/BoardModelGenerationTests.cs",
                "Assets/LegendsOfRus/Tests/EditMode/BoardModelSwapAndMatchTests.cs",
                "Assets/LegendsOfRus/Tests/EditMode/BoardModelSpecialTests.cs",
                "Assets/LegendsOfRus/Tests/EditMode/BoardModelCollapseAndSnapshotTests.cs",
                "Assets/LegendsOfRus/Tests/EditMode/BoardMechanicStateTests.cs",
                "Assets/LegendsOfRus/Tests/EditMode/BoardObstacleResolutionTests.cs",
                "Assets/LegendsOfRus/Tests/EditMode/BoardResolutionPlannerTests.cs",
                "Assets/LegendsOfRus/Tests/EditMode/LegendsOfRus.GameplayCore.Tests.asmdef",
                "Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/LevelGoalRuntimeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/Chapter01MechanicCurvePlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/StoryDataRuntimePlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/StoryMilestoneReactionPlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/ChapterFinaleCompletionPlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/CoreLoopMotionFeedbackPlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/CoreLoopVfxBoosterFeedbackPlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs",
                "Assets/LegendsOfRus/Tests/PlayMode/AudioHapticsFidelityVerificationPlayModeTests.cs",
                "Assets/Scenes/Bootstrap.unity",
            };

            var missing = new List<string>();
            foreach (var path in required)
                if (AssetDatabase.LoadAssetAtPath<Object>(path) == null)
                    missing.Add(path);

            var levelCatalog = AssetDatabase.LoadAssetAtPath<TextAsset>(
                "Assets/LegendsOfRus/Resources/Config/chapter01_levels.json"
            );
            if (levelCatalog != null)
            {
                try
                {
                    ChapterLevelCatalogData parsed =
                        JsonUtility.FromJson<ChapterLevelCatalogData>(levelCatalog.text);
                    LevelValidationReport report = LevelDataValidator.ValidateCatalog(parsed);
                    for (int i = 0; i < report.Errors.Count; i++)
                        missing.Add("Level catalog: " + report.Errors[i]);
                    LevelValidationReport progression =
                        Chapter01ProgressionValidator.Validate(parsed);
                    for (int i = 0; i < progression.Errors.Count; i++)
                        missing.Add("Chapter 1 progression: " + progression.Errors[i]);
                }
                catch (System.Exception exception)
                {
                    missing.Add("Level catalog JSON: " + exception.Message);
                }
            }

            var storyCatalog = AssetDatabase.LoadAssetAtPath<TextAsset>(
                "Assets/LegendsOfRus/Resources/Config/chapter01_story.json"
            );
            var storyRussian = AssetDatabase.LoadAssetAtPath<TextAsset>(
                "Assets/LegendsOfRus/Resources/Localization/story_ru.json"
            );
            if (storyCatalog != null && storyRussian != null && levelCatalog != null)
            {
                try
                {
                    ChapterStoryCatalogData parsedStory =
                        JsonUtility.FromJson<ChapterStoryCatalogData>(storyCatalog.text);
                    StoryLocalizationData parsedRussian =
                        JsonUtility.FromJson<StoryLocalizationData>(storyRussian.text);
                    ChapterLevelCatalogData parsedLevels =
                        JsonUtility.FromJson<ChapterLevelCatalogData>(levelCatalog.text);
                    StoryValidationReport storyReport = StoryCatalog.Validate(
                        parsedStory,
                        parsedRussian,
                        parsedLevels
                    );
                    for (int i = 0; i < storyReport.Errors.Count; i++)
                        missing.Add("Chapter 1 story: " + storyReport.Errors[i]);
                }
                catch (System.Exception exception)
                {
                    missing.Add("Chapter 1 story JSON: " + exception.Message);
                }
            }

            try { ActiveCampaignContent.Load(forceReload: true); }
            catch (System.Exception exception) { missing.Add("Active campaign: " + exception.Message); }

            if (missing.Count == 0)
                Debug.Log(
                    "Legends of Rus v1.14 Step 4 project-structure and static " +
                    "catalog validation PASSED. Runtime and device QA remain separate."
                );
            else
            {
                Debug.LogError("Missing or invalid assets:\n" + string.Join("\n", missing));
                if (Application.isBatchMode)
                {
                    throw new System.InvalidOperationException(
                        "Legends of Rus project validation failed."
                    );
                }
            }
        }
    }
}
#endif
