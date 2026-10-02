using System;
using System.Linq;
using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    public sealed partial class Match3Board
    {
        string retainedAttemptId;
        int retainedAttemptNumber;
        public RetainedRoundData CaptureFailedRound()
        {
            if (campaignManaged || activeCommand!=null || Phase!=GameplayPhase.Failed || moves!=0 || goalRuntime.AllComplete)
                throw new InvalidOperationException("Only a settled normal-game failure can be retained.");
            var board=model.CreateSnapshot();
            return new RetainedRoundData { attemptId=retainedAttemptId,attempt=retainedAttemptNumber,
                chapter=currentLevel.chapterId,level=currentLevel.levelId,levelFingerprint=RetainedRoundData.Fingerprint(currentLevel),
                rules=(int)ruleVersion,seed=model.Seed??0,score=score,moves=moves,stateHash=board.StateHash,
                tiles=board.Tiles,specials=board.Specials.Select(n=>(int)n).ToArray(),obstacles=board.Obstacles.Select(n=>(int)n).ToArray(),
                layers=board.ObstacleLayers.Select(n=>(int)n).ToArray(),items=board.ObjectiveItems.Select(n=>(int)n).ToArray(),
                goals=Enumerable.Range(0,GoalCount).Select(i=>GetGoalSlot(i).Current).ToArray() };
        }
        // Reconstructs the suspended board without registering an attempt, spending energy,
        // granting moves or enabling input. CONT-02 must supply a durable continuation entitlement.
        public void InitializeRetained(RetainedRoundData data,Action<int> win,Action<int> fail)
        {
            if (model!=null) throw new InvalidOperationException("Restore requires a new board instance.");
            if (!GameState.Instance.TryGetRetainedRound(out var current) ||
                UnityEngine.JsonUtility.ToJson(data)!=UnityEngine.JsonUtility.ToJson(current))
                throw new ArgumentException("Retained attempt is stale or incompatible.");
            ruleVersion=CurrentGameplayRules;useCurrentBoardArt=true;campaignInventory=null;
            InitializeCore(ChapterLevelCatalog.Find(data.chapter,data.level),win,fail,null,data);
        }
    }
}
