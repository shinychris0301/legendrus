using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    /// <summary>
    /// Read-only public state for one approved gameplay HUD goal slot.
    /// </summary>
    public sealed class LevelGoalProgress
    {
        public string Type { get; }
        public int TileType { get; }
        public BoardObstacleKind ObstacleKind { get; }
        public BoardObjectiveItemKind ObjectiveItemKind { get; }
        public int Target { get; }
        public int Current { get; private set; }

        public int Remaining => Math.Max(0, Target - Current);
        public bool IsComplete => Current >= Target;

        internal LevelGoalProgress(LevelGoalData goal, int startingScore)
        {
            if (goal == null) throw new ArgumentNullException(nameof(goal));

            Type = goal.type ?? string.Empty;
            TileType = goal.tileType;
            Target = Math.Max(1, goal.target);

            if (
                !LevelObstacleTypes.TryToBoardKind(
                    goal.obstacleType,
                    out BoardObstacleKind obstacleKind
                )
            )
            {
                obstacleKind = BoardObstacleKind.None;
            }
            ObstacleKind = obstacleKind;

            if (
                !LevelBoardItemTypes.TryToBoardKind(
                    goal.itemType,
                    out BoardObjectiveItemKind objectiveItemKind
                )
            )
            {
                objectiveItemKind = BoardObjectiveItemKind.None;
            }
            ObjectiveItemKind = objectiveItemKind;

            Current = string.Equals(Type, LevelGoalTypes.Score, StringComparison.Ordinal)
                ? Math.Max(0, startingScore)
                : 0;
        }

        internal bool Add(int amount)
        {
            if (amount <= 0 || IsComplete) return false;
            int next = Math.Min(Target, Current + amount);
            if (next == Current) return false;
            Current = next;
            return true;
        }

        internal bool SetAbsolute(int value)
        {
            int next = Math.Min(Target, Math.Max(0, value));
            if (next == Current) return false;
            Current = next;
            return true;
        }

        internal void Restore(int value)
        {
            Current = Math.Min(Target, Math.Max(0, value));
        }
    }

    public sealed class LevelGoalRuntimeSnapshot
    {
        readonly int[] current;

        internal int Count => current.Length;

        internal LevelGoalRuntimeSnapshot(int[] current)
        {
            this.current = current != null
                ? (int[])current.Clone()
                : Array.Empty<int>();
        }

        internal int GetCurrent(int index) => current[index];
    }

    /// <summary>
    /// Deterministic progress tracker for the one-or-two goal contract used by
    /// level data and the approved gameplay HUD.
    /// </summary>
    public sealed class LevelGoalRuntime
    {
        readonly LevelGoalProgress[] slots;

        public int SlotCount => slots.Length;

        public bool AllComplete
        {
            get
            {
                if (slots.Length == 0) return false;
                for (int i = 0; i < slots.Length; i++)
                    if (!slots[i].IsComplete) return false;
                return true;
            }
        }

        public LevelGoalRuntime(LevelData level, int startingScore)
            : this(level != null ? level.goals : null, startingScore)
        {
        }

        public LevelGoalRuntime(
            IReadOnlyList<LevelGoalData> goals,
            int startingScore
        )
        {
            if (goals == null || goals.Count == 0)
            {
                slots = Array.Empty<LevelGoalProgress>();
                return;
            }

            int count = Math.Min(2, goals.Count);
            var valid = new List<LevelGoalProgress>(count);
            for (int i = 0; i < count; i++)
            {
                if (goals[i] == null) continue;
                valid.Add(new LevelGoalProgress(goals[i], startingScore));
            }
            slots = valid.ToArray();
        }

        public LevelGoalProgress GetSlot(int index)
        {
            return index >= 0 && index < slots.Length
                ? slots[index]
                : null;
        }

        public int RemainingAt(int index)
        {
            LevelGoalProgress slot = GetSlot(index);
            return slot != null ? slot.Remaining : 0;
        }

        public bool RecordConsumedMove()
        {
            bool changed = false;
            for (int i = 0; i < slots.Length; i++)
            {
                if (
                    string.Equals(
                        slots[i].Type,
                        LevelGoalTypes.Survival,
                        StringComparison.Ordinal
                    )
                )
                {
                    changed |= slots[i].Add(1);
                }
            }
            return changed;
        }

        public bool ApplyWave(BoardResolutionWave wave, int totalScore)
        {
            if (wave == null) throw new ArgumentNullException(nameof(wave));

            bool changed = false;
            for (int i = 0; i < slots.Length; i++)
            {
                LevelGoalProgress slot = slots[i];
                switch (slot.Type)
                {
                    case LevelGoalTypes.CollectTile:
                        if (
                            slot.TileType >= 0 &&
                            slot.TileType < wave.ClearedByTileType.Count
                        )
                        {
                            changed |= slot.Add(
                                wave.ClearedByTileType[slot.TileType]
                            );
                        }
                        break;

                    case LevelGoalTypes.Score:
                        changed |= slot.SetAbsolute(totalScore);
                        break;

                    case LevelGoalTypes.ClearObstacle:
                        changed |= slot.Add(
                            CountDestroyedObstacles(
                                wave.ObstacleInteractions,
                                slot.ObstacleKind
                            )
                        );
                        break;

                    case LevelGoalTypes.BringDown:
                        changed |= slot.Add(
                            CountCollectedItems(
                                wave.Collapse.CollectedObjectiveItems,
                                slot.ObjectiveItemKind
                            )
                        );
                        break;
                }
            }

            return changed;
        }

        public LevelGoalRuntimeSnapshot CaptureSnapshot()
        {
            var current = new int[slots.Length];
            for (int i = 0; i < slots.Length; i++)
                current[i] = slots[i].Current;
            return new LevelGoalRuntimeSnapshot(current);
        }

        public void RestoreSnapshot(LevelGoalRuntimeSnapshot snapshot)
        {
            if (snapshot == null) throw new ArgumentNullException(nameof(snapshot));
            if (snapshot.Count != slots.Length)
            {
                throw new ArgumentException(
                    "Goal snapshot slot count does not match this runtime.",
                    nameof(snapshot)
                );
            }

            for (int i = 0; i < slots.Length; i++)
                slots[i].Restore(snapshot.GetCurrent(i));
        }

        static int CountDestroyedObstacles(
            IReadOnlyList<BoardObstacleInteractionInstruction> interactions,
            BoardObstacleKind kind
        )
        {
            if (kind == BoardObstacleKind.None) return 0;

            int count = 0;
            for (int i = 0; i < interactions.Count; i++)
            {
                BoardObstacleInteractionInstruction interaction = interactions[i];
                if (interaction.Kind == kind && interaction.Destroyed)
                    count++;
            }
            return count;
        }

        static int CountCollectedItems(
            IReadOnlyList<BoardObjectiveItemCollection> collections,
            BoardObjectiveItemKind kind
        )
        {
            if (kind == BoardObjectiveItemKind.None) return 0;

            int count = 0;
            for (int i = 0; i < collections.Count; i++)
                if (collections[i].Kind == kind) count++;
            return count;
        }
    }
}
