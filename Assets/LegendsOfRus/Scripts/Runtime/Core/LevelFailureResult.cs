using System;
using System.Collections.Generic;

namespace LegendsOfRus
{
    /// <summary>Transient, copied progress for the failed attempt; never saved or rewarded.</summary>
    public sealed class LevelFailureResult
    {
        public LevelData Level { get; }
        public int Score { get; }
        public bool HasGoalProgress { get; }
        readonly int[] progress;
        public LevelFailureResult(LevelData level,int score,IReadOnlyList<int> current)
        {
            Level=level??throw new ArgumentNullException(nameof(level));Score=Math.Max(0,score);
            int count=level.goals?.Length??0;progress=new int[count];
            HasGoalProgress=current!=null&&current.Count==count;
            for(int i=0;i<count;i++)progress[i]=HasGoalProgress?Math.Max(0,Math.Min(level.goals[i].target,current[i])):0;
        }
        public int CurrentAt(int index)=>HasGoalProgress&&index>=0&&index<progress.Length?progress[index]:-1;
    }
}
