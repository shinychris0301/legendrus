from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs';s=p.read_text();a=s.index('        public bool CompleteStoryScene(');b=s.index('        public ChapterCompletionPhase GetChapterCompletionPhase',a)
s=s[:a]+'''        public bool CompleteStoryScene(string sceneId)
        {
            if (!canPersist || string.IsNullOrWhiteSpace(sceneId) || HasCompletedStoryScene(sceneId))
                return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            if (candidate.completedStoryScenes == null)
                candidate.completedStoryScenes = new List<string>();
            if (!StoryProgress.Complete(candidate.completedStoryScenes, sceneId)) return false;
            return TryCommitBalance(candidate);
        }

''' +s[b:]
a=s.index('        public bool AcknowledgeChapterCompletion(');b=s.index('        public ChapterCompletionSummary GetChapterCompletionSummary',a)
s=s[:a]+'''        public bool AcknowledgeChapterCompletion(int chapterId)
        {
            if (!canPersist || GetChapterCompletionPhase(chapterId) != ChapterCompletionPhase.PresentationPending)
                return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            if (candidate.acknowledgedChapterCompletions == null)
                candidate.acknowledgedChapterCompletions = new List<int>();
            if (candidate.acknowledgedChapterCompletions.Contains(chapterId)) return false;
            candidate.acknowledgedChapterCompletions.Add(chapterId);
            candidate.acknowledgedChapterCompletions.Sort();
            return TryCommitBalance(candidate);
        }

''' +s[b:];p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs';s=p.read_text().replace('        ChapterComplete\n','        ChapterComplete,\n        SaveRejected\n',1)
s=s.replace('            if (activeSequence.Advance()) return StoryAdvanceResult.MoreLines;','''            if (activeSequence.LineIndex + 1 < activeSequence.Scene.lines.Length && activeSequence.Advance())
                return StoryAdvanceResult.MoreLines;''',1)
s=s.replace('''            activeSequence = null;
            GameState state = GameState.Instance;
            bool completedNow = state != null && state.CompleteStoryScene(completed?.id);''','''            GameState state = GameState.Instance;
            bool completedNow = state != null && state.CompleteStoryScene(completed?.id);
            // Keep the current final line and unread eligibility on a rejected save.
            if (!completedNow && (state == null || !state.HasCompletedStoryScene(completed?.id)))
                return StoryAdvanceResult.SaveRejected;
            activeSequence.Advance();
            activeSequence = null;''',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs';s=p.read_text();needle='''            switch (StoryService.AdvanceDialogue())
            {
''';assert needle in s;s=s.replace(needle,needle+'''                case StoryAdvanceResult.SaveRejected:
                    var dialogue = contentRoot.GetComponentInChildren<ProductionDialogueView>();
                    // Re-arm the existing one-shot control without replacing the current page.
                    if (dialogue != null) dialogue.Bind(ContinueAfterDialogue);
                    ShowErrorDialog();
                    break;
''',1)
s=s.replace('''                Toast("Не удалось сохранить завершение главы. Попробуйте ещё раз.");
                return;''','''                ShowErrorDialog();
                return;''',1);p.write_text(s)
print('Atomic story/chapter acknowledgement; failed final-line save retains sequence/page with existing error modal.')
