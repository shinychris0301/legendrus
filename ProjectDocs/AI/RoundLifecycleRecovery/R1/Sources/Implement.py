from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs';s=p.read_text();start=s.index('        public int RegisterLevelAttempt(');end=s.index('        public LevelCompletionResult CompleteLevel(',start)
new='''        public int RegisterLevelAttempt(LevelData levelData) =>
            TryRegisterLevelAttempt(levelData, out var attempt) ? attempt : 0;

        bool CanRegisterLevelAttempt(LevelData levelData)
        {
            if (levelData == null || !canPersist) return false;
            if (ContinuationBlocksNewAttempt()) throw new InvalidOperationException("Resolve or recover the continuation reservation before starting another round.");
            if (TryGetRetainedRound(out _)) throw new InvalidOperationException("Resolve the retained attempt before starting another round.");
            return IsLevelUnlocked(levelData.chapterId, levelData.levelId);
        }

        public bool TryRegisterLevelAttempt(LevelData levelData, out int attempt)
        {
            attempt = 0;
            if (!CanRegisterLevelAttempt(levelData)) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            if (!TryPrepareLevelAttempt(candidate, levelData, out var preparedAttempt) ||
                !TryCommitRoundCandidate(candidate, clearRetainedAttempt: true)) return false;
            attempt = preparedAttempt;
            return true;
        }

        // Normal paid-in-energy entry commits the debit and attempt together before board construction.
        public bool TryStartLevel(LevelData levelData, out int attempt, out bool insufficientEnergy)
        {
            attempt = 0;
            insufficientEnergy = false;
            if (!CanRegisterLevelAttempt(levelData) || HasPendingBoosterCommand) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            long now = DateTime.UtcNow.Ticks;
            AdvanceEnergyClock(candidate, now);
            if (candidate.energy < 1) { insufficientEnergy = true; return false; }
            bool wasFull = candidate.energy >= candidate.energyMax;
            candidate.energy--;
            if (wasFull) candidate.lastEnergyUtcTicks = now;
            if (!TryPrepareLevelAttempt(candidate, levelData, out var preparedAttempt) ||
                !TryCommitRoundCandidate(candidate, clearRetainedAttempt: true)) return false;
            attempt = preparedAttempt;
            return true;
        }

        static LevelProgressData CandidateLevelProgress(PlayerSaveData candidate, LevelData levelData)
        {
            if (candidate.levelProgress == null) candidate.levelProgress = new List<LevelProgressData>();
            var progress = candidate.levelProgress.Find(p => p != null &&
                p.chapterId == levelData.chapterId && p.levelId == levelData.levelId);
            if (progress == null)
            {
                progress = new LevelProgressData { chapterId = levelData.chapterId, levelId = levelData.levelId };
                candidate.levelProgress.Add(progress);
            }
            return progress;
        }

        static bool TryPrepareLevelAttempt(PlayerSaveData candidate, LevelData levelData, out int attempt)
        {
            attempt = 0;
            candidate.retainedRound = null; // Discard only incompatible optional checkpoints on commitment.
            candidate.selectedChapter = levelData.chapterId;
            candidate.selectedLevel = levelData.levelId;
            var progress = CandidateLevelProgress(candidate, levelData);
            try
            {
                progress.attempts = checked(progress.attempts + 1);
                if (progress.attempts > 1) progress.retries = checked(progress.retries + 1);
            }
            catch (OverflowException) { return false; }
            attempt = progress.attempts;
            return attempt > 0;
        }

        bool TryCommitRoundCandidate(PlayerSaveData candidate, LevelFailureResult failure = null, bool clearRetainedAttempt = false)
        {
            if (!canPersist) return false;
            bool written = SaveService.TrySave(candidate, out var result);
            LastWriteResult = result;
            if (!written && !result.PrimaryDurablyCommitted) { canPersist = false; return false; }
            Save = candidate;
            if (clearRetainedAttempt) unsavedRetainedRound = null;
            if (failure != null) LastFailureResult = failure;
            canPersist = written;
            Changed?.Invoke();
            return true;
        }

        public void RecordLevelFailure(LevelData levelData, int score, IReadOnlyList<int> goalProgress = null) =>
            TryRecordLevelFailure(levelData, score, goalProgress);

        public bool TryRecordLevelFailure(LevelData levelData, int score, IReadOnlyList<int> goalProgress = null)
        {
            if (!canPersist) return false;
            if (ContinuationBlocksNewAttempt()) throw new InvalidOperationException("Finalize the retained continuation transaction first.");
            if (levelData == null || !IsLevelUnlocked(levelData.chapterId, levelData.levelId)) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            var progress = CandidateLevelProgress(candidate, levelData);
            if (progress.attempts == 0) progress.attempts = 1;
            try { progress.failures = checked(progress.failures + 1); }
            catch (OverflowException) { return false; }
            progress.bestScore = Mathf.Max(progress.bestScore, score);
            return TryCommitRoundCandidate(candidate, new LevelFailureResult(levelData, score, goalProgress));
        }

'''
s=s[:start]+new+s[end:];p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.RetainedRound.cs';s=p.read_text();old='            progress.failures++;progress.bestScore=Math.Max(progress.bestScore,data.score);candidate.retainedRound=null;';new='''            try { progress.failures=checked(progress.failures+1); }
            catch (OverflowException) { return false; }
            progress.bestScore=Math.Max(progress.bestScore,data.score);candidate.retainedRound=null;''';assert old in s;p.write_text(s.replace(old,new,1))
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Gameplay/Match3Board.cs';s=p.read_text();old='''            LevelData levelData,
            Action<int> onWin,
            Action<int> onFail
        )''';new='''            LevelData levelData,
            Action<int> onWin,
            Action<int> onFail,
            int? registeredAttempt = null
        )''';assert old in s;s=s.replace(old,new,1)
s=s.replace('            InitializeCore(levelData, onWin, onFail, null);','''            if (registeredAttempt.HasValue && registeredAttempt.Value < 1)
                throw new ArgumentException("A committed ordinary attempt must be positive.");
            if (registeredAttempt.HasValue && (levelData == null || GameState.Instance == null ||
                GameState.Instance.GetLevelProgress(levelData.chapterId, levelData.levelId)?.attempts != registeredAttempt.Value))
                throw new ArgumentException("The ordinary attempt must match the committed progress ledger.");
            InitializeCore(levelData, onWin, onFail, registeredAttempt,
                preRegisteredOrdinaryAttempt: registeredAttempt.HasValue);''',1)
s=s.replace('int? committedAttempt, RetainedRoundData retained = null)','int? committedAttempt, RetainedRoundData retained = null, bool preRegisteredOrdinaryAttempt = false)',1)
s=s.replace('            campaignManaged = committedAttempt.HasValue;','            campaignManaged = committedAttempt.HasValue && !preRegisteredOrdinaryAttempt;',1)
s=s.replace('            retainedAttemptNumber = attempt;', '''            if (attempt < 1) throw new InvalidOperationException("Round attempt could not be committed.");
            retainedAttemptNumber = attempt;''',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs';s=p.read_text();assert '        Match3Board activeBoard;' in s;s=s.replace('        Match3Board activeBoard;','''        Match3Board activeBoard;
        bool levelStartInProgress;
        LevelData pendingGameplayLevel;
        int pendingGameplayAttempt;''',1)
start=s.index('        void StartLevel()');end=s.index('        bool OpenSelectedLevel()',start)
oldbody=s[start:end];body=oldbody[oldbody.index('        {\n')+10:oldbody.rfind('        }')]
startdebit=body.index('            if (!GameState.Instance.SpendEnergy())');enddebit=body.index('            Show(ScreenId.Gameplay);',startdebit)
replacement='''            LevelData selected = ChapterLevelCatalog.ResolveSelected(GameState.Instance.Save);
            if (!GameState.Instance.TryStartLevel(selected, out var attempt, out var insufficientEnergy))
            {
                if (!insufficientEnergy) { ShowErrorDialog(); return; }
                if (selected != null)
                    PrivacyAwareAnalytics.TryEnergyGateBlocked(selected.id, 1, Mathf.Max(0, GameState.Instance.Save.energy));
                ShowModal(ScreenId.OutOfEnergy);
                return;
            }
            pendingGameplayLevel = selected;
            pendingGameplayAttempt = attempt;
'''
body=body[:startdebit]+replacement+body[enddebit:]
newbody='''        void StartLevel()
        {
            if (levelStartInProgress) return;
            levelStartInProgress = true;
            try
            {
'''+''.join('    '+line if line.strip() else line for line in body.splitlines(True))+'''            }
            finally
            {
                pendingGameplayLevel = null;
                pendingGameplayAttempt = 0;
                levelStartInProgress = false;
            }
        }

'''
s=s[:start]+newbody+s[end:]
s=s.replace('''            activeBoard.Initialize(
                onWin:''','''            LevelData startedLevel = pendingGameplayLevel ?? ChapterLevelCatalog.ResolveSelected(GameState.Instance.Save);
            int? startedAttempt = pendingGameplayAttempt > 0 ? (int?)pendingGameplayAttempt : null;
            pendingGameplayLevel = null;
            pendingGameplayAttempt = 0;
            activeBoard.Initialize(
                levelData: startedLevel,
                onWin:''',1)
s=s.replace('''                }
            );

            view.Bind(activeBoard, ShowPause);''','''                },
                registeredAttempt: startedAttempt
            );

            view.Bind(activeBoard, ShowPause);''',1);p.write_text(s)
print('Atomic start/registration/failure and checked retained loss; ordinary board ownership retained.')
