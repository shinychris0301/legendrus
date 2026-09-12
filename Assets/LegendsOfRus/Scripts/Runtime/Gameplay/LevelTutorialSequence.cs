using System;
using System.Collections.Generic;

namespace LegendsOfRus
{
    /// <summary>
    /// Deterministic, presentation-independent tutorial queue. A trigger may be
    /// signalled repeatedly, but each authored step is queued at most once.
    /// </summary>
    public sealed class LevelTutorialSequence
    {
        readonly LevelTutorialStepData[] steps;
        readonly Queue<int> pending = new Queue<int>();
        readonly HashSet<int> scheduled = new HashSet<int>();

        public LevelTutorialSequence(LevelTutorialStepData[] authoredSteps)
        {
            steps = authoredSteps ?? Array.Empty<LevelTutorialStepData>();
        }

        public bool HasPending => pending.Count > 0;
        public int PendingCount => pending.Count;
        public int ScheduledCount => scheduled.Count;

        public int Signal(string trigger)
        {
            if (string.IsNullOrEmpty(trigger)) return 0;

            int added = 0;
            for (int i = 0; i < steps.Length; i++)
            {
                LevelTutorialStepData step = steps[i];
                if (
                    step == null ||
                    scheduled.Contains(i) ||
                    !string.Equals(step.trigger, trigger, StringComparison.Ordinal)
                )
                {
                    continue;
                }

                scheduled.Add(i);
                pending.Enqueue(i);
                added++;
            }

            return added;
        }

        public LevelTutorialStepData DequeueNext()
        {
            return pending.Count > 0 ? steps[pending.Dequeue()] : null;
        }
    }
}
