#if UNITY_EDITOR
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public sealed class BoardModelDebugWindow : EditorWindow
    {
        int width = 8;
        int height = 9;
        int tileTypes = 6;
        int seed = 11002;
        Vector2 scroll;
        string boardText = string.Empty;
        string hash = string.Empty;
        string replayToken = string.Empty;

        [MenuItem("Legends of Rus/Tests/Open Deterministic Board Debugger")]
        public static void Open()
        {
            GetWindow<BoardModelDebugWindow>("Board Debugger");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("v1.10 Step 2 — Deterministic BoardModel", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Generate a board by seed, copy its replay token, and reproduce the same board in QA.",
                MessageType.Info
            );

            width = EditorGUILayout.IntSlider("Width", width, 3, 12);
            height = EditorGUILayout.IntSlider("Height", height, 3, 14);
            tileTypes = EditorGUILayout.IntSlider("Tile types", tileTypes, 3, 9);
            seed = EditorGUILayout.IntField("Seed", seed);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate playable board"))
                Generate();
            if (GUILayout.Button("Random seed"))
            {
                seed = System.Environment.TickCount;
                Generate();
            }
            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrEmpty(boardText)) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State hash", hash);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("Board", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(boardText, GUILayout.MinHeight(180));
            EditorGUILayout.LabelField("Replay token", EditorStyles.boldLabel);
            EditorGUILayout.TextArea(replayToken, GUILayout.MinHeight(110));
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Copy replay token"))
            {
                EditorGUIUtility.systemCopyBuffer = replayToken;
                ShowNotification(new GUIContent("Replay token copied"));
            }

            if (GUILayout.Button("Validate replay token"))
            {
                BoardSnapshot decoded = BoardReplayCodec.Decode(replayToken);
                var restored = new BoardModel(
                    decoded.Width,
                    decoded.Height,
                    decoded.TileTypeCount,
                    new SeededBoardRandom(1)
                );
                restored.RestoreSnapshot(decoded);

                bool valid = restored.ComputeStateHash() == decoded.StateHash;
                EditorUtility.DisplayDialog(
                    "Replay validation",
                    valid ? "Replay token restored the exact board state." : "Replay hash mismatch.",
                    "OK"
                );
            }
        }

        void Generate()
        {
            var model = new BoardModel(
                width,
                height,
                tileTypes,
                new SeededBoardRandom(seed)
            );
            model.GeneratePlayableBoard();

            BoardSnapshot snapshot = model.CreateSnapshot();
            boardText = model.ToDebugString();
            hash = snapshot.StateHash;
            replayToken = snapshot.ToReplayToken();
        }
    }
}
#endif
