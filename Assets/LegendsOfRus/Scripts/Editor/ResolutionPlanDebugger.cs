#if UNITY_EDITOR
using System.Text;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public sealed class ResolutionPlanDebugger : EditorWindow
    {
        string replayToken = string.Empty;
        int ax;
        int ay;
        int bx = 1;
        int by;
        BoardBoosterCommandKind booster = BoardBoosterCommandKind.MaceRow;
        string output = "Paste a BoardSnapshot replay token or generate a sample board.";
        Vector2 scroll;

        [MenuItem("Legends of Rus/Tests/Open Resolution Plan Debugger")]
        static void Open()
        {
            GetWindow<ResolutionPlanDebugger>("Resolution Plan Debugger");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField(
                "v1.10 Step 3 — Deterministic Resolution Plans",
                EditorStyles.boldLabel
            );
            EditorGUILayout.HelpBox(
                "A board replay token plus one command is enough to reproduce " +
                "the exact swap, match waves, special activations, clear list, " +
                "collapse instructions, spawns, score delta and final board hash.",
                MessageType.Info
            );

            replayToken = EditorGUILayout.TextArea(
                replayToken,
                GUILayout.MinHeight(70f)
            );

            if (GUILayout.Button("Generate Sample Board"))
            {
                var sample = new BoardModel(
                    8,
                    9,
                    6,
                    new SeededBoardRandom(11003)
                );
                sample.GeneratePlayableBoard();
                replayToken = sample.CreateSnapshot().ToReplayToken();
                output = sample.ToDebugString();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Swap Command", EditorStyles.boldLabel);
            ax = EditorGUILayout.IntField("A X", ax);
            ay = EditorGUILayout.IntField("A Y", ay);
            bx = EditorGUILayout.IntField("B X", bx);
            by = EditorGUILayout.IntField("B Y", by);

            if (GUILayout.Button("Plan Swap"))
                PlanSwap();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Booster Command", EditorStyles.boldLabel);
            booster = (BoardBoosterCommandKind)EditorGUILayout.EnumPopup(
                "Booster",
                booster
            );

            if (GUILayout.Button("Plan Booster at A"))
                PlanBooster();

            EditorGUILayout.Space();
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.TextArea(
                output,
                GUILayout.ExpandHeight(true),
                GUILayout.MinHeight(280f)
            );
            EditorGUILayout.EndScrollView();
        }

        void PlanSwap()
        {
            if (!TryCreateModel(out BoardModel model)) return;

            BoardResolutionPlan plan = new BoardResolutionPlanner(model).PlanSwap(
                new BoardPosition(ax, ay),
                new BoardPosition(bx, by)
            );
            output = Format(plan);
        }

        void PlanBooster()
        {
            if (!TryCreateModel(out BoardModel model)) return;

            BoardResolutionPlan plan = new BoardResolutionPlanner(model).PlanBooster(
                new BoardBoosterRequest(
                    booster,
                    new BoardPosition(ax, ay)
                )
            );
            output = Format(plan);
        }

        bool TryCreateModel(out BoardModel model)
        {
            model = null;
            if (!BoardReplayCodec.TryDecode(replayToken, out BoardSnapshot snapshot))
            {
                output = "Replay token could not be decoded.";
                return false;
            }

            model = new BoardModel(
                snapshot.Width,
                snapshot.Height,
                snapshot.TileTypeCount,
                new SeededBoardRandom(1)
            );
            model.RestoreSnapshot(snapshot);
            return true;
        }

        string Format(BoardResolutionPlan plan)
        {
            var builder = new StringBuilder();
            builder.AppendLine(plan.ToDebugString());
            builder.AppendLine($"Before: {plan.BeforeSnapshot?.StateHash}");
            builder.AppendLine($"After swap: {plan.SnapshotAfterSwap?.StateHash}");

            for (int i = 0; i < plan.Waves.Count; i++)
            {
                BoardResolutionWave wave = plan.Waves[i];
                builder.AppendLine();
                builder.AppendLine(
                    $"Wave {wave.ComboIndex}: combination={wave.Combination}, " +
                    $"booster={wave.Booster}, score={wave.ScoreDelta}"
                );
                builder.AppendLine(
                    $"  runs={wave.Runs.Count}, matched={wave.MatchedCells.Count}, " +
                    $"creations={wave.Creations.Count}, activations={wave.Activations.Count}, " +
                    $"cleared={wave.ClearedCells.Count}, " +
                    $"obstacleHits={wave.ObstacleInteractions.Count}"
                );
                builder.AppendLine(
                    $"  collapse moves={wave.Collapse.Moves.Count}, " +
                    $"spawns={wave.Collapse.Spawns.Count}, " +
                    $"itemsCollected={wave.Collapse.CollectedObjectiveItems.Count}"
                );
                builder.AppendLine(
                    $"  afterClear={wave.SnapshotAfterClear?.StateHash}, " +
                    $"afterCollapse={wave.SnapshotAfterCollapse?.StateHash}"
                );
            }

            builder.AppendLine();
            builder.AppendLine(
                $"Reshuffle attempts={plan.Reshuffle.Attempts}, " +
                $"regenerated={plan.Reshuffle.Regenerated}"
            );
            builder.AppendLine($"Final: {plan.FinalSnapshot?.StateHash}");
            builder.AppendLine($"Plan hash: {plan.PlanHash}");
            return builder.ToString();
        }
    }
}
#endif
