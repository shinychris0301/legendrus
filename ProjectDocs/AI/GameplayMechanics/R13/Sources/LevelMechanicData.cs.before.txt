using System;
using LegendsOfRus.GameplayCore;

namespace LegendsOfRus
{
    public static class LevelMechanicKeys
    {
        public const string Match = "match";
        public const string Score = "score";
        public const string Ice = "ice";
        public const string Chain = "chain";
        public const string Crate = "crate";
        public const string BringDown = "bring_down";
        public const string Survival = "survival";

        public static readonly string[] Chapter01Rollout =
        {
            Match,
            Score,
            Ice,
            Chain,
            Crate,
            BringDown,
            Survival
        };

        public static bool IsSupported(string mechanic)
        {
            for (int i = 0; i < Chapter01Rollout.Length; i++)
                if (string.Equals(mechanic, Chapter01Rollout[i], StringComparison.Ordinal))
                    return true;

            return false;
        }
    }

    public static class LevelTutorialTriggerTypes
    {
        public const string LevelStart = "level_start";
        public const string FirstMove = "first_move";
        public const string FirstGoalProgress = "first_goal_progress";
        public const string FirstObstacleHit = "first_obstacle_hit";
        public const string FirstItemCollected = "first_item_collected";

        public static bool IsSupported(string trigger)
        {
            return
                string.Equals(trigger, LevelStart, StringComparison.Ordinal) ||
                string.Equals(trigger, FirstMove, StringComparison.Ordinal) ||
                string.Equals(trigger, FirstGoalProgress, StringComparison.Ordinal) ||
                string.Equals(trigger, FirstObstacleHit, StringComparison.Ordinal) ||
                string.Equals(trigger, FirstItemCollected, StringComparison.Ordinal);
        }
    }

    public static class LevelGoalTypes
    {
        public const string CollectTile = "collect_tile";
        public const string Score = "score";
        public const string ClearObstacle = "clear_obstacle";
        public const string BringDown = "bring_down";
        public const string Survival = "survival";

        public static bool IsSupported(string type)
        {
            return
                string.Equals(type, CollectTile, StringComparison.Ordinal) ||
                string.Equals(type, Score, StringComparison.Ordinal) ||
                string.Equals(type, ClearObstacle, StringComparison.Ordinal) ||
                string.Equals(type, BringDown, StringComparison.Ordinal) ||
                string.Equals(type, Survival, StringComparison.Ordinal);
        }
    }

    public static class LevelObstacleTypes
    {
        public const string RunicSeal = "runic_seal";
        public const string Ice = "ice";
        public const string Chain = "chain";
        public const string Crate = "crate";

        public static bool TryToBoardKind(string type, out BoardObstacleKind kind)
        {
            if (string.Equals(type, RunicSeal, StringComparison.Ordinal)) { kind = BoardObstacleKind.RunicSeal; return true; }
            if (string.Equals(type, Ice, StringComparison.Ordinal))
            {
                kind = BoardObstacleKind.Ice;
                return true;
            }
            if (string.Equals(type, Chain, StringComparison.Ordinal))
            {
                kind = BoardObstacleKind.Chain;
                return true;
            }
            if (string.Equals(type, Crate, StringComparison.Ordinal))
            {
                kind = BoardObstacleKind.Crate;
                return true;
            }

            kind = BoardObstacleKind.None;
            return false;
        }

        public static int MaximumLayers(string type)
        {
            if (string.Equals(type, RunicSeal, StringComparison.Ordinal)) return 2;
            if (string.Equals(type, Ice, StringComparison.Ordinal)) return 2;
            if (string.Equals(type, Chain, StringComparison.Ordinal)) return 2;
            if (string.Equals(type, Crate, StringComparison.Ordinal)) return 3;
            return 0;
        }
    }

    public static class LevelBoardItemTypes
    {
        public const string FirebirdFeather = "firebird_feather";

        public static bool TryToBoardKind(
            string type,
            out BoardObjectiveItemKind kind
        )
        {
            if (string.Equals(type, FirebirdFeather, StringComparison.Ordinal))
            {
                kind = BoardObjectiveItemKind.FirebirdFeather;
                return true;
            }

            kind = BoardObjectiveItemKind.None;
            return false;
        }
    }

    [Serializable]
    public sealed class LevelObstacleData
    {
        public int x;
        public int y;
        public string type = LevelObstacleTypes.Ice;
        public int layers = 1;
    }

    [Serializable]
    public sealed class LevelBoardItemData
    {
        public int x;
        public int y;
        public string type = LevelBoardItemTypes.FirebirdFeather;
    }
}
