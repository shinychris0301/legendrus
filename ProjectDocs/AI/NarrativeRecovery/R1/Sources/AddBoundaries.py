from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text();marker='        void SnapshotAndClearSaveJournal()'
new='''        [TestCase(false)] [TestCase(true)]
        public void NarrativeAcknowledgementCommitsOnceWithoutOtherSaveMutation(bool chapter)
        {
            var state = GameState.Instance; var story = StoryCatalog.LoadChapter1();
            if (chapter)
            {
                state.Save.highestUnlockedLevel = 500;
                state.Save.levelProgress.Add(new LevelProgressData { chapterId = 1, levelId = 500, completions = 1, bestStars = 3, rewardClaimed = true });
                Assert.That(state.CompleteStoryScene(story.completion.finaleSceneId), Is.True);
            }
            var expected = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(state.Save));
            if (chapter) expected.acknowledgedChapterCompletions.Add(1);
            else expected.completedStoryScenes.Add(story.introSceneId);
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(chapter ? state.AcknowledgeChapterCompletion(1) : state.CompleteStoryScene(story.introSceneId), Is.True);
                Assert.That(events, Is.EqualTo(1));
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(JsonUtility.ToJson(expected)));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(expected)));
                var committed = state.LastWriteResult;
                Assert.That(chapter ? state.AcknowledgeChapterCompletion(1) : state.CompleteStoryScene(story.introSceneId), Is.False);
                Assert.That(events, Is.EqualTo(1)); Assert.That(state.LastWriteResult, Is.SameAs(committed));
            }
            finally { state.Changed -= changed; }
        }

        [Test]
        public void NarrativeInvalidAndIneligibleAcknowledgementsDoNotInitializeOrWrite()
        {
            var state = GameState.Instance; state.Save.completedStoryScenes = null; state.Save.acknowledgedChapterCompletions = null;
            string before = JsonUtility.ToJson(state.Save); var write = state.LastWriteResult;
            Assert.That(state.CompleteStoryScene(null) || state.CompleteStoryScene("") || state.CompleteStoryScene(" "), Is.False);
            Assert.That(state.AcknowledgeChapterCompletion(-1) || state.AcknowledgeChapterCompletion(0) || state.AcknowledgeChapterCompletion(1) || state.AcknowledgeChapterCompletion(2), Is.False);
            Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.LastWriteResult, Is.SameAs(write));
            Assert.That(state.CompleteStoryScene(StoryCatalog.LoadChapter1().introSceneId), Is.True);
            Assert.That(SaveService.Load().completedStoryScenes, Does.Contain(StoryCatalog.LoadChapter1().introSceneId));
        }

        [Test]
        public void NarrativeExternallyCommittedActiveSceneReturnsWithoutDuplicateGrantOrEvent()
        {
            var state = GameState.Instance; StoryService.ResetRuntime(); Assert.That(StoryService.TryBeginChapterIntro(), Is.True);
            var scene = StoryCatalog.FindScene(StoryService.ActiveSceneId);
            for (int i = 1; i < scene.lines.Length; i++) Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
            Assert.That(state.CompleteStoryScene(scene.id), Is.True); string before = JsonUtility.ToJson(state.Save); var write = state.LastWriteResult;
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            try
            {
                Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.PreLevel));
                Assert.That(StoryService.ActiveSceneId, Is.Empty); Assert.That(StoryService.CurrentLine, Is.Null);
                Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.None));
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before)); Assert.That(state.LastWriteResult, Is.SameAs(write)); Assert.That(events, Is.Zero);
            }
            finally { state.Changed -= changed; StoryService.ResetRuntime(); }
        }

'''
assert marker in s and 'NarrativeAcknowledgementCommitsOnceWithoutOtherSaveMutation' not in s;s=s.replace(marker,new+marker)
# Extend the five rejection cases with actual fresh GameState recovery after restoring only their synthetic journal.
start=s.index('        public void NarrativeBlockedWriteKeepsAcknowledgementPendingAndDialogueVisible(');end=s.index('        [TestCase(false)] [TestCase(true)]\n        public void NarrativeAcknowledgement',start)
needle='''                PlayerPrefs.Save();
            }
        }

''';region=s[start:end];assert region.endswith(needle)
region=region[:-len(needle)]+'''                PlayerPrefs.Save();
            }
            var freshObject = new GameObject("FreshNarrativeRecoveryState");
            try
            {
                var fresh = freshObject.AddComponent<GameState>(); Assert.That(fresh.CanPersist, Is.True);
                int gold = fresh.Save.gold, gems = fresh.Save.gems, energy = fresh.Save.energy;
                if (operation == 1)
                {
                    Assert.That(fresh.HasPendingChapterCompletion(1), Is.True);
                    Assert.That(StoryService.AcknowledgeChapterComplete(), Is.True);
                    Assert.That(StoryService.AcknowledgeChapterComplete(), Is.False);
                    Assert.That(fresh.GetChapterCompletionPhase(1), Is.EqualTo(ChapterCompletionPhase.Acknowledged));
                }
                else if (service)
                {
                    Assert.That(operation == 4 ? StoryService.TryBeginChapterIntro() : StoryService.TryBeginPostVictoryDialogue(), Is.True);
                    Assert.That(StoryService.ActiveSceneId, Is.EqualTo(sceneId));
                    var scene = StoryCatalog.FindScene(sceneId);
                    for (int i = 1; i < scene.lines.Length; i++) Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
                    Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(operation == 4 ? StoryAdvanceResult.PreLevel : operation == 3 ? StoryAdvanceResult.ChapterComplete : StoryAdvanceResult.ChapterMap));
                    Assert.That(fresh.HasCompletedStoryScene(sceneId), Is.True);
                    Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.None));
                }
                else
                {
                    Assert.That(fresh.HasCompletedStoryScene(story.introSceneId), Is.False);
                    Assert.That(fresh.CompleteStoryScene(story.introSceneId), Is.True);
                    Assert.That(fresh.CompleteStoryScene(story.introSceneId), Is.False);
                }
                Assert.That(fresh.Save.gold, Is.EqualTo(gold)); Assert.That(fresh.Save.gems, Is.EqualTo(gems)); Assert.That(fresh.Save.energy, Is.EqualTo(energy));
                Assert.That(JsonUtility.ToJson(SaveService.Load()), Is.EqualTo(JsonUtility.ToJson(fresh.Save)));
            }
            finally
            {
                StoryService.ResetRuntime(); typeof(GameState).GetProperty("Instance").SetValue(null, state); Object.Destroy(freshObject);
            }
        }

'''
s=s[:start]+region+s[end:];p.write_text(s)
print('Added commit/no-op/invalid/duplicate boundaries and fresh GameState recovery to all five rejected-write cases.')
