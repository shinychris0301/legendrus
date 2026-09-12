#if UNITY_EDITOR
using System.Text;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public sealed class SpecialCombinationPresetDebugger : EditorWindow
    {
        int selectedIndex;
        Vector2 scroll;
        string output = "Select a QA preset and plan it.";

        [MenuItem("Legends of Rus/Tests/Open Special Combination Preset Debugger")]
        public static void Open()
        {
            GetWindow<SpecialCombinationPresetDebugger>("Special QA Presets");
        }

        void OnGUI()
        {
            var presets = BoardQAPresetFactory.SpecialCombinationPresets;
            string[] labels = new string[presets.Count];
            for (int i = 0; i < presets.Count; i++)
                labels[i] = presets[i].DisplayName;

            selectedIndex = EditorGUILayout.Popup("Preset", selectedIndex, labels);

            if (GUILayout.Button("Plan selected combination"))
                Plan(presets[selectedIndex]);

            if (GUILayout.Button("Run all preset validations"))
                RunAll();

            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.TextArea(output, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        void Plan(BoardQAPreset preset)
        {
            BoardModel model = From(preset.Snapshot);
            BoardResolutionPlan plan = new BoardResolutionPlanner(model).PlanSwap(
                preset.Primary,
                preset.Secondary
            );

            var builder = new StringBuilder();
            builder.AppendLine(preset.Id);
            builder.AppendLine(preset.Description);
            builder.AppendLine(plan.ToDebugString());

            for (int i = 0; i < plan.Waves.Count; i++)
            {
                BoardResolutionWave wave = plan.Waves[i];
                builder.Append("Wave ").Append(wave.ComboIndex)
                    .Append(": combination=").Append(wave.Combination)
                    .Append(", clear=").Append(wave.ClearedCells.Count)
                    .Append(", activations=").Append(wave.Activations.Count)
                    .Append(", score=").Append(wave.ScoreDelta)
                    .AppendLine();
            }

            output = builder.ToString();
        }

        void RunAll()
        {
            var builder = new StringBuilder();
            foreach (BoardQAPreset preset in BoardQAPresetFactory.SpecialCombinationPresets)
            {
                BoardModel model = From(preset.Snapshot);
                BoardResolutionPlan plan = new BoardResolutionPlanner(model).PlanSwap(
                    preset.Primary,
                    preset.Secondary
                );

                BoardResolutionWave first = plan.Waves.Count > 0 ? plan.Waves[0] : null;
                bool ok =
                    plan.IsResolved &&
                    first != null &&
                    first.Combination == preset.ExpectedCombination &&
                    first.ClearedCells.Count >= preset.ExpectedMinimumClear;

                builder.Append(ok ? "PASS " : "FAIL ")
                    .Append(preset.Id)
                    .Append(" clear=")
                    .Append(first?.ClearedCells.Count ?? 0)
                    .AppendLine();
            }

            output = builder.ToString();
        }

        static BoardModel From(BoardSnapshot snapshot)
        {
            var model = new BoardModel(
                snapshot.Width,
                snapshot.Height,
                snapshot.TileTypeCount,
                new SeededBoardRandom(4004)
            );
            model.RestoreSnapshot(snapshot);
            return model;
        }
    }
}
#endif
