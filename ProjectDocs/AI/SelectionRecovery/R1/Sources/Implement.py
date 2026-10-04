from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs';s=p.read_text();start=s.index('        void NormalizeLevelProgress()');end=s.index('        void NormalizeTutorialProgress()',start)
body=s[start:end].replace('void NormalizeLevelProgress()','static void NormalizeLevelProgress(PlayerSaveData save)').replace('Save.','save.').replace('CalculateTotalStars();','CalculateTotalStars(save);')
s=s[:start]+'        void NormalizeLevelProgress() => NormalizeLevelProgress(Save);\n\n'+body+s[end:]
s=s.replace('            if (ContinuationBlocksNewAttempt()) return false;\n            if (TryGetRetainedRound(out var retained)', '            if (!canPersist || ContinuationBlocksNewAttempt()) return false;\n            if (TryGetRetainedRound(out var retained)',1)
old='''            Save.selectedChapter = chapterId;
            Save.selectedLevel = levelId;
            Persist();
            return true;
        }

        public void EnsureSelectedLevelIsUnlocked()
        {
            NormalizeLevelProgress();
            Persist();
        }'''
new='''            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            candidate.selectedChapter = chapterId;
            candidate.selectedLevel = levelId;
            return TryCommitBalance(candidate);
        }

        public void EnsureSelectedLevelIsUnlocked() => TryEnsureSelectedLevelIsUnlocked();

        public bool TryEnsureSelectedLevelIsUnlocked()
        {
            if (!canPersist) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            NormalizeLevelProgress(candidate);
            return TryCommitBalance(candidate);
        }'''
assert old in s;s=s.replace(old,new,1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapView.cs';s=p.read_text();s=s.replace('Action openSelectedLevel,Action back,Action<string> notify)', 'Func<bool> openSelectedLevel,Action back,Action<string> notify,Action selectionRejected=null)',1)
s=s.replace('Bind(PlayButton,openSelectedLevel,true);','Bind(PlayButton,()=>{committed=openSelectedLevel();},false);',1)
s=s.replace('''                    GameState.Instance.SelectLevel(1,level);
                    committed=true;openSelectedLevel();''','''                    if(!GameState.Instance.SelectLevel(1,level))
                    {
                        selectionRejected?.Invoke();return;
                    }
                    committed=openSelectedLevel();''',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs';s=p.read_text();s=s.replace('()=>NavigateBackFrom(ScreenId.ChapterMap),chapterMap.Notice.Show);','()=>NavigateBackFrom(ScreenId.ChapterMap),chapterMap.Notice.Show,()=>ShowErrorDialog());',1)
s=s.replace('''            GameState.Instance.EnsureSelectedLevelIsUnlocked();
            if (!GameState.Instance.SpendEnergy())''','''            if (!GameState.Instance.TryEnsureSelectedLevelIsUnlocked()) { ShowErrorDialog(); return; }
            if (!GameState.Instance.SpendEnergy())''',1)
s=s.replace('''        void OpenSelectedLevel()
        {
            GameState.Instance.EnsureSelectedLevelIsUnlocked();''','''        bool OpenSelectedLevel()
        {
            if (HasActiveModalBlocker || IsScreenTransitioning) return false;
            if (!GameState.Instance.TryEnsureSelectedLevelIsUnlocked()) { ShowErrorDialog(); return false; }''',1)
s=s.replace('''            else if (TryShowPendingChapterFlow())
                return;
            else
                Show(ScreenId.PreLevel);
        }

        void OpenChapterMap()''','''            else if (TryShowPendingChapterFlow())
                return true;
            else
                Show(ScreenId.PreLevel);
            return current != ScreenId.ChapterMap;
        }

        void OpenChapterMap()''',1);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Tests/PlayMode/PreLevelAlignmentPlayModeTests.cs';s=p.read_text();s=s.replace('                LogAssert.Expect(LogType.Error,new System.Text.RegularExpressions.Regex("Player save write blocked:"));\n','',1);p.write_text(s)
print('Atomic selection/normalization; native map/start owners acknowledge rejected saving.')
