using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LegendsOfRus.GameplayCore;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    public sealed class RetainedRoundData
    {
        public int schema = 1;
        public string attemptId, levelFingerprint, stateHash;
        public int chapter, level, attempt, rules, seed, score, moves;
        public string randomContract = "snapshot-action-v1";
        public int[] tiles, specials, obstacles, layers, items, goals;

        public RetainedRoundData Copy() => JsonUtility.FromJson<RetainedRoundData>(JsonUtility.ToJson(this));
        public static string Fingerprint(LevelData level)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(JsonUtility.ToJson(level)))).Replace("-", "");
        }
        public BoardSnapshot Snapshot(LevelData definition) => new BoardSnapshot(definition.columns, definition.rows,
            definition.tileTypes, (int[])tiles.Clone(), specials.Select(n => (SpecialGemKind)n).ToArray(),
            obstacles.Select(n => (BoardObstacleKind)n).ToArray(), layers.Select(n => (byte)n).ToArray(),
            items.Select(n => (BoardObjectiveItemKind)n).ToArray(), stateHash);

        public bool IsCompatible(LevelData definition)
        {
            if (definition == null || schema != 1 || !Guid.TryParseExact(attemptId,"N",out _) || attempt < 1 ||
                chapter != definition.chapterId || level != definition.levelId || moves != 0 || score < 0 ||
                rules != (int)Match3Board.CurrentGameplayRules || randomContract != "snapshot-action-v1" ||
                levelFingerprint != Fingerprint(definition)) return false;
            int count = definition.columns * definition.rows;
            if (count < 9 || count > 256 || new[]{tiles,specials,obstacles,layers,items}.Any(a => a == null || a.Length != count) ||
                goals == null || definition.goals == null || goals.Length != definition.goals.Length) return false;
            bool incomplete = false;
            for (int i=0;i<goals.Length;i++) {
                if (goals[i]<0 || goals[i]>definition.goals[i].target) return false;
                incomplete |= goals[i]<definition.goals[i].target;
            }
            if (!incomplete) return false;
            for (int i=0;i<count;i++) {
                if (tiles[i]<BoardModel.EmptyTile || tiles[i]>=definition.tileTypes ||
                    !Enum.IsDefined(typeof(SpecialGemKind),specials[i]) || !Enum.IsDefined(typeof(BoardObstacleKind),obstacles[i]) ||
                    !Enum.IsDefined(typeof(BoardObjectiveItemKind),items[i]) || layers[i]<0 || layers[i]>3 ||
                    (obstacles[i]==0)!=(layers[i]==0)) return false;
            }
            try {
                var model = new BoardModel(definition.columns,definition.rows,definition.tileTypes,new SeededBoardRandom(seed));
                model.SquareMatches = true; model.StandaloneSpecials = true; model.RestoreSnapshot(Snapshot(definition));
                return model.ComputeStateHash()==stateHash && !model.HasAnyMatches();
            } catch (ArgumentException) { return false; }
        }
    }
}
