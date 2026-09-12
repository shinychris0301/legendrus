#if UNITY_EDITOR
using System;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class GameplayCoreSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.10 Step 1 Self-Tests")]
        public static void Run()
        {
            int passed = 0;

            Expect(
                BoardDirectionResolver.ResolveNormalized(new Vector2(0.8f, 0.1f)) == BoardDirection.Right,
                "right swipe"
            );
            passed++;

            Expect(
                BoardDirectionResolver.ResolveNormalized(new Vector2(-0.8f, 0.1f)) == BoardDirection.Left,
                "left swipe"
            );
            passed++;

            Expect(
                BoardDirectionResolver.ResolveNormalized(new Vector2(0.1f, 0.8f)) == BoardDirection.Up,
                "up swipe"
            );
            passed++;

            Expect(
                BoardDirectionResolver.ResolveNormalized(new Vector2(0.1f, -0.8f)) == BoardDirection.Down,
                "down swipe"
            );
            passed++;

            Expect(
                BoardDirectionResolver.ToBoardDelta(BoardDirection.Up) == new Vector2Int(0, -1),
                "up board delta"
            );
            passed++;

            Expect(
                BoardDirectionResolver.ToBoardDelta(BoardDirection.Down) == new Vector2Int(0, 1),
                "down board delta"
            );
            passed++;

            var state = new GameplayStateMachine();
            Expect(state.TryTransition(GameplayPhase.Swapping), "idle -> swapping");
            Expect(!state.CanAcceptPointerInput, "input blocked while swapping");
            Expect(state.TryTransition(GameplayPhase.ResolvingMatches), "swapping -> resolving");
            Expect(!state.TryPause(), "resolution cannot be paused mid-command");
            Expect(state.TryTransition(GameplayPhase.Falling), "resolving -> falling");
            Expect(state.TryTransition(GameplayPhase.ResolvingMatches), "falling -> resolving");
            Expect(state.TryTransition(GameplayPhase.Idle), "resolving -> idle");
            Expect(state.CanAcceptPointerInput, "input restored in idle");
            passed += 8;

            Debug.Log($"Legends of Rus v1.10 Step 1 self-tests PASSED ({passed}/14).");
            EditorUtility.DisplayDialog(
                "Legends of Rus — Tests",
                $"v1.10 Step 1 self-tests passed: {passed}/14",
                "OK"
            );
        }

        static void Expect(bool condition, string name)
        {
            if (!condition)
                throw new InvalidOperationException($"Gameplay core self-test failed: {name}");
        }
    }
}
#endif
