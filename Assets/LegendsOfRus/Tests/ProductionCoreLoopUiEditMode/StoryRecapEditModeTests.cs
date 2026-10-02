using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class StoryRecapEditModeTests
    {
        [Test] public void EmptyUnknownAndUnacknowledgedScenesStayHidden()
        {
            var catalog=StoryCatalog.LoadChapter1(true);
            Assert.That(StoryRecap.Read(null,catalog),Is.Empty);
            var save=new PlayerSaveData();
            save.completedStoryScenes.AddRange(new[]{"unknown","chapter01.finale","bridge-story:chapter01.intro"});
            Assert.That(StoryRecap.Read(save,catalog),Is.Empty);
            save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=500,completions=1});
            Assert.That(StoryRecap.Read(save,catalog),Is.Empty,"Completion does not acknowledge dialogue.");
        }
        [TestCase(0)][TestCase(-1)][TestCase(1)]
        public void MilestoneRequiresItsOwnChapterCompletion(int completions)
        {
            var catalog=StoryCatalog.LoadChapter1();var milestone=catalog.milestones[0];
            var save=new PlayerSaveData();save.completedStoryScenes.Add(milestone.sceneId);
            save.levelProgress.Add(new LevelProgressData{chapterId=2,levelId=milestone.afterLevelId,completions=1});
            Assert.That(StoryRecap.Read(save,catalog),Is.Empty);
            save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=milestone.afterLevelId,completions=completions});
            Assert.That(StoryRecap.Read(save,catalog).Count,Is.EqualTo(completions>0?1:0));
        }
        [Test] public void AllInstalledScenesKeepOrderLocalesAndSourceData()
        {
            var catalog=StoryCatalog.LoadChapter1();var save=new PlayerSaveData();
            foreach(var m in catalog.milestones.Reverse()){
                save.completedStoryScenes.Add(m.sceneId);save.completedStoryScenes.Add(m.sceneId);
                save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=m.afterLevelId,completions=1});}
            save.completedStoryScenes.Add(catalog.introSceneId);
            string before=JsonUtility.ToJson(save),source=JsonUtility.ToJson(catalog);
            var rows=StoryRecap.Read(save,catalog);Assert.That(rows.Count,Is.EqualTo(58));
            Assert.That(rows[0].Scene.id,Is.EqualTo(catalog.introSceneId));Assert.That(rows[57].Scene.id,Is.EqualTo(catalog.completion.finaleSceneId));
            Assert.That(rows.Select(r=>r.AfterLevel),Is.Ordered);
            foreach(var row in rows)foreach(var language in new[]{"ru","en"}){
                var sequence=new StorySequenceRuntime(row.Scene,StoryCatalog.LoadRussian());
                do{var line=sequence.LocalizeCurrentLine(StoryCatalog.LoadDialogueLanguage(language));
                    Assert.That(line.Speaker,Is.Not.Empty);Assert.That(line.Text,Is.Not.Empty);}while(sequence.Advance());}
            rows[0].Scene.lines[0].textKey="local-only";
            Assert.That(JsonUtility.ToJson(save),Is.EqualTo(before));Assert.That(JsonUtility.ToJson(catalog),Is.EqualTo(source));
        }
        [Test] public void SparseLedgerIgnoresFutureAndMalformedEvidenceWithoutNormalization()
        {
            var catalog=StoryCatalog.LoadChapter1();var save=new PlayerSaveData();var first=catalog.milestones[0];var last=catalog.milestones.Last();
            save.completedStoryScenes.AddRange(new[]{null,"",last.sceneId,first.sceneId,first.sceneId,"obsolete"});
            save.levelProgress.Add(null);save.levelProgress.Add(new LevelProgressData{chapterId=1,levelId=first.afterLevelId,completions=1});
            string before=JsonUtility.ToJson(save);var rows=StoryRecap.Read(save,catalog);
            Assert.That(rows.Count,Is.EqualTo(1));Assert.That(rows[0].Scene.id,Is.EqualTo(first.sceneId));Assert.That(JsonUtility.ToJson(save),Is.EqualTo(before));
        }
    }
}
