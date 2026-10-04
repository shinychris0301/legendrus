from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs';s=p.read_text();marker='        void SnapshotAndClearSaveJournal()'
new='''        [TestCase(0)] [TestCase(1)] [TestCase(2)] [TestCase(3)] [TestCase(4)]
        public void NarrativeBlockedWriteKeepsAcknowledgementPendingAndDialogueVisible(int operation)
        {
            var state = GameState.Instance; var story = StoryCatalog.LoadChapter1();
            StoryService.ResetRuntime();
            if (operation == 1 || operation == 3)
            {
                state.Save.highestUnlockedLevel = 500;
                state.Save.levelProgress.Add(new LevelProgressData { chapterId = 1, levelId = 500, completions = 1, bestStars = 3, rewardClaimed = true });
                state.Save.lastCompletedChapter = 1; state.Save.lastCompletedLevel = 500;
                Assert.That(SaveService.TrySave(state.Save, out _), Is.True);
                if (operation == 1) Assert.That(state.CompleteStoryScene(story.completion.finaleSceneId), Is.True);
            }
            if (operation == 2)
            {
                var level = ChapterLevelCatalog.Find(1, 1); state.CompleteLevel(level, level.starScoreThresholds[0]);
            }
            bool service = operation >= 2;
            if (service)
            {
                Assert.That(operation == 4 ? StoryService.TryBeginChapterIntro() : StoryService.TryBeginPostVictoryDialogue(), Is.True);
                var scene = StoryCatalog.FindScene(StoryService.ActiveSceneId);
                for (int i = 1; i < scene.lines.Length; i++) Assert.That(StoryService.AdvanceDialogue(), Is.EqualTo(StoryAdvanceResult.MoreLines));
            }
            string sceneId = StoryService.ActiveSceneId;
            var finalLine = StoryService.CurrentLine;
            var original = state.Save; string before = JsonUtility.ToJson(original);
            var present = new bool[PersistedSaveKeys.Length]; var journal = new string[present.Length];
            for (int i = 0; i < journal.Length; i++)
            { present[i] = PlayerPrefs.HasKey(PersistedSaveKeys[i]); journal[i] = PlayerPrefs.GetString(PersistedSaveKeys[i]); }
            var future = JsonUtility.FromJson<PlayerSaveData>(before); future.saveVersion = 999;
            foreach (string key in PersistedSaveKeys) PlayerPrefs.DeleteKey(key);
            string raw = JsonUtility.ToJson(future); PlayerPrefs.SetString(PersistedSaveKeys[0], raw); PlayerPrefs.Save();
            int events = 0; System.Action changed = () => events++; state.Changed += changed;
            bool ignored = LogAssert.ignoreFailingMessages;
            try
            {
                LogAssert.ignoreFailingMessages = true;
                bool acknowledged = service ? StoryService.AdvanceDialogue().ToString() != "SaveRejected" :
                    operation == 0 ? state.CompleteStoryScene(story.introSceneId) : state.AcknowledgeChapterCompletion(1);
                Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before), "Rejected story/chapter acknowledgement must remain unread/pending.");
                Assert.That(acknowledged, Is.False); Assert.That(events, Is.Zero); Assert.That(state.Save, Is.SameAs(original));
                Assert.That(state.CanPersist, Is.False); var failedWrite = state.LastWriteResult;
                if (service)
                {
                    Assert.That(StoryService.ActiveSceneId, Is.EqualTo(sceneId));
                    Assert.That(StoryService.CurrentLine.Text, Is.EqualTo(finalLine.Text));
                    Assert.That(StoryService.AdvanceDialogue().ToString(), Is.EqualTo("SaveRejected"));
                }
                else Assert.That(operation == 0 ? state.CompleteStoryScene(story.introSceneId) : state.AcknowledgeChapterCompletion(1), Is.False);
                Assert.That(state.LastWriteResult, Is.SameAs(failedWrite)); Assert.That(JsonUtility.ToJson(state.Save), Is.EqualTo(before));
                Assert.That(PlayerPrefs.GetString(PersistedSaveKeys[0]), Is.EqualTo(raw));
                Assert.That(PlayerPrefs.HasKey(PersistedSaveKeys[1]) || PlayerPrefs.HasKey(PersistedSaveKeys[2]), Is.False);
            }
            finally
            {
                StoryService.ResetRuntime(); state.Changed -= changed; LogAssert.ignoreFailingMessages = ignored;
                for (int i = 0; i < journal.Length; i++)
                { PlayerPrefs.DeleteKey(PersistedSaveKeys[i]); if (present[i]) PlayerPrefs.SetString(PersistedSaveKeys[i], journal[i]); }
                PlayerPrefs.Save();
            }
        }

'''
assert marker in s and 'NarrativeBlockedWriteKeepsAcknowledgementPending' not in s;p.write_text(s.replace(marker,new+marker))
