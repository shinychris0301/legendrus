using System;
using System.Linq;
using UnityEngine;

namespace LegendsOfRus
{
    public sealed partial class GameState
    {
        RetainedRoundData unsavedRetainedRound;
        internal Func<PlayerSaveData,PlayerSaveWriteResult> RetainedWriteOverride;
        public bool HasUnsavedRetainedRound => unsavedRetainedRound != null;
        bool OwnsRetained(RetainedRoundData data) => data != null &&
            GetLevelProgress(data.chapter,data.level)?.attempts == data.attempt &&
            Save.selectedChapter == data.chapter && Save.selectedLevel == data.level;

        public bool TryGetRetainedRound(out RetainedRoundData data)
        {
            var source = unsavedRetainedRound ?? Save.retainedRound;
            if (!OwnsRetained(source) || !source.IsCompatible(ChapterLevelCatalog.Find(source.chapter,source.level))) { data=null; return false; }
            data=source.Copy();return true;
        }
        public bool RestoreRetainedFailurePresentation()
        {
            if (!TryGetRetainedRound(out var data)) return false;
            LastFailureResult=new LevelFailureResult(ChapterLevelCatalog.Find(data.chapter,data.level),data.score,data.goals);
            return true;
        }
        // The board is already settled and input-locked. A failed write retains a memory copy
        // for retry; it never masquerades as a durable checkpoint or increments loss counters.
        public bool TrySuspendRound(RetainedRoundData data)
        {
            if (!OwnsRetained(data) || !data.IsCompatible(ChapterLevelCatalog.Find(data.chapter,data.level))) return false;
            var current=unsavedRetainedRound??Save.retainedRound;
            if (current!=null && !string.IsNullOrEmpty(current.attemptId) && JsonUtility.ToJson(current)!=JsonUtility.ToJson(data)) return false;
            if (unsavedRetainedRound==null && current!=null && !string.IsNullOrEmpty(current.attemptId)) return true;
            unsavedRetainedRound=data.Copy();RestoreRetainedFailurePresentation();
            var candidate=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));candidate.retainedRound=data.Copy();
            if (!CommitRetained(candidate)) return false;
            unsavedRetainedRound=null;return true;
        }
        public bool TryFinalizeRetainedFailure(string attemptId)
        {
            if (!TryGetRetainedRound(out var data) || data.attemptId!=attemptId) return false;
            var candidate=JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            var progress=candidate.levelProgress.Single(p=>p.chapterId==data.chapter&&p.levelId==data.level);
            progress.failures++;progress.bestScore=Math.Max(progress.bestScore,data.score);candidate.retainedRound=null;
            if (!CommitRetained(candidate)) return false;
            unsavedRetainedRound=null;
            PrivacyAwareAnalytics.TryLevelFail(ChapterLevelCatalog.Find(data.chapter,data.level).id,data.attempt,data.score);
            BalanceTelemetryReviewService.Instance?.TryRecordLevelFail(ChapterLevelCatalog.Find(data.chapter,data.level),data.attempt,data.score);
            return true;
        }
        bool CommitRetained(PlayerSaveData candidate)
        {
            PlayerSaveWriteResult result;
            if (RetainedWriteOverride!=null) result=RetainedWriteOverride(candidate);
            else SaveService.TrySave(candidate,out result);
            LastWriteResult=result;
            if (result==null || !result.Success&&!result.PrimaryDurablyCommitted) { canPersist=false;return false; }
            Save=candidate;unsavedRetainedRound=null;canPersist=result.Success;Changed?.Invoke();return true;
        }
    }
}
