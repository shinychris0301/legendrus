from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs';s=p.read_text();a=s.index('        public bool CompleteTutorial(');b=s.index('        public bool HasCompletedStoryScene(',a)
s=s[:a]+'''        public bool CompleteTutorial(string tutorialId)
        {
            if (!canPersist || string.IsNullOrWhiteSpace(tutorialId) || HasCompletedTutorial(tutorialId))
                return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            if (candidate.completedTutorialSteps == null)
                candidate.completedTutorialSteps = new List<string>();
            candidate.completedTutorialSteps.Add(tutorialId);
            return TryCommitBalance(candidate);
        }

'''+s[b:];p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs';s=p.read_text();s=s.replace('        public LevelTutorialStepData CurrentStep', '        public event Action<LevelTutorialPresenter> AcknowledgementRejected;\n\n        public LevelTutorialStepData CurrentStep',1)
s=s.replace('            action.onClick.AddListener(Advance);','''            GameObject presentedOverlay = overlay;
            action.onClick.AddListener(() =>
            {
                if (overlay == presentedOverlay && CurrentStep == step) Advance();
            });''',1)
s=s.replace('''        public void Advance()
        {
            if (CurrentStep != null && board != null)''','''        public void Advance()
        {
            if (CurrentStep == null || board == null || !IsPresenting) return;
            if (CurrentStep != null && board != null)''',1)
s=s.replace('''                if (completedNow)
                {''','''                if (!completedNow && (state == null || !state.HasCompletedTutorial(CurrentStep.id)))
                {
                    AcknowledgementRejected?.Invoke(this);
                    return;
                }
                if (completedNow)
                {''',1)
s=s.replace('''            if (overlay != null) Destroy(overlay);
            overlay = null;''','''            if (overlay != null) { overlay.SetActive(false); Destroy(overlay); }
            overlay = null;''',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs';s=p.read_text();marker='        bool IsGameplayInventoryError(Match3Board board)'
s=s.replace(marker,'''        void ShowGameplayTutorialError(Match3Board board, LevelTutorialPresenter presenter)
        {
            if (board == null || board != activeBoard || current != ScreenId.Gameplay ||
                board.Phase != LegendsOfRus.GameplayCore.GameplayPhase.Paused ||
                presenter == null || !presenter.IsPresenting || presenter.CurrentStep == null ||
                contentRoot == null || contentRoot.GetComponentInChildren<LevelTutorialPresenter>() != presenter ||
                modalMotionState != ModalMotionState.Closed || IsScreenTransitioning)
                return;
            if (!ShowModal(ScreenId.EmptyErrorState)) return;
            // Dismissing the error leaves this exact tutorial and its board pause intact.
            var error = modalVisualRoot.GetComponentInChildren<ProductionErrorDialogView>();
            error.Bind(CloseModal, null);
        }

'''+marker,1)
s=s.replace('''            tutorialGo.GetComponent<LevelTutorialPresenter>().Initialize(
                activeBoard,
                activeBoard.CurrentLevel
            );''','''            var tutorial = tutorialGo.GetComponent<LevelTutorialPresenter>();
            tutorial.AcknowledgementRejected += failed => ShowGameplayTutorialError(board, failed);
            tutorial.Initialize(activeBoard, activeBoard.CurrentLevel);''',1);p.write_text(s)
print('Atomic tutorial acknowledgement, retained rejection/queue/pause, stale-control guard and owned native error route installed.')
