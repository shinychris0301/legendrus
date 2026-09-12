#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class BoardModelSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.10 Step 2 BoardModel Self-Tests")]
        public static void Run()
        {
            int passed = 0;

            var first = new BoardModel(8, 9, 6, new SeededBoardRandom(11002));
            var second = new BoardModel(8, 9, 6, new SeededBoardRandom(11002));
            first.GeneratePlayableBoard();
            second.GeneratePlayableBoard();

            Expect(first.ComputeStateHash() == second.ComputeStateHash(), "same seed hash"); passed++;
            Expect(first.ToDebugString() == second.ToDebugString(), "same seed board"); passed++;
            Expect(!first.HasAnyMatches(), "generated board has no matches"); passed++;
            Expect(first.HasLegalMove(), "generated board has legal move"); passed++;

            string beforeLegalQuery = first.ComputeStateHash();
            int beforeVersion = first.MutationVersion;
            first.HasLegalMove();
            Expect(first.ComputeStateHash() == beforeLegalQuery, "legal-move query preserves board"); passed++;
            Expect(first.MutationVersion == beforeVersion, "legal-move query preserves version"); passed++;

            var swap = new BoardModel(3, 3, 3, new SeededBoardRandom(1));
            swap.LoadRows("001", "001", "110");
            string swapBefore = swap.ComputeStateHash();
            Expect(
                swap.SwapCreatesMatch(new BoardPosition(2, 1), new BoardPosition(2, 2)),
                "known swap creates match"
            ); passed++;
            Expect(swap.ComputeStateHash() == swapBefore, "swap query restores state"); passed++;

            var four = new BoardModel(5, 5, 5, new SeededBoardRandom(2));
            four.LoadRows("0000.", ".....", ".....", ".....", ".....");
            var preferred = new BoardPosition(1, 0);
            var fourCreation = four.DetermineSpecialCreations(four.FindRuns(), preferred);
            Expect(fourCreation[preferred] == SpecialGemKind.Row, "horizontal four creates row special"); passed++;

            var five = new BoardModel(5, 5, 5, new SeededBoardRandom(2));
            five.LoadRows("11111", ".....", ".....", ".....", ".....");
            preferred = new BoardPosition(3, 0);
            var fiveCreation = five.DetermineSpecialCreations(five.FindRuns(), preferred);
            Expect(fiveCreation[preferred] == SpecialGemKind.Color, "five creates color special"); passed++;

            var tShape = new BoardModel(5, 5, 5, new SeededBoardRandom(2));
            tShape.LoadRows("..2..", "..2..", "22222", ".....", ".....");
            var intersection = new BoardPosition(2, 2);
            var tCreation = tShape.DetermineSpecialCreations(tShape.FindRuns(), intersection);
            Expect(tCreation[intersection] == SpecialGemKind.Burst, "T intersection creates burst"); passed++;

            var chain = new BoardModel(4, 4, 4, new SeededBoardRandom(3));
            chain.LoadRows("0123", "1230", "2301", "3012");
            chain.SetSpecial(new BoardPosition(1, 1), SpecialGemKind.Row);
            chain.SetSpecial(new BoardPosition(3, 1), SpecialGemKind.Column);
            var triggered = new List<BoardPosition>();
            var clear = chain.ExpandSpecialEffects(new[] { new BoardPosition(1, 1) }, triggered);
            Expect(triggered.Count == 2, "special chain triggers two specials"); passed++;
            Expect(clear.Count == 7, "row-column chain clears seven cells"); passed++;

            var collapse = new BoardModel(3, 4, 5, new SequenceBoardRandom(0, 1, 2, 3, 4));
            collapse.LoadRows(".1.", "0..", ".2.", "3.4");
            BoardCollapseResult collapseResult = collapse.CollapseAndRefill();
            Expect(collapseResult.Spawns.Count == 7, "collapse refills seven cells"); passed++;
            Expect(collapse.GetTile(new BoardPosition(0, 3)) == 3, "bottom tile remains bottom"); passed++;
            Expect(collapse.GetTile(new BoardPosition(0, 2)) == 0, "upper tile falls down"); passed++;

            var dead = new BoardModel(3, 3, 3, new SeededBoardRandom(123));
            dead.LoadRows("001", "001", "122");
            Expect(!dead.HasLegalMove(), "dead preset has no legal move"); passed++;
            BoardReshuffleResult reshuffle = dead.EnsurePlayableBoard();
            Expect(reshuffle.Attempts > 0, "dead board reshuffles"); passed++;
            Expect(!dead.HasAnyMatches() && dead.HasLegalMove(), "reshuffled board is playable"); passed++;

            BoardSnapshot snapshot = first.CreateSnapshot();
            string token = snapshot.ToReplayToken();
            BoardSnapshot decoded = BoardReplayCodec.Decode(token);
            Expect(decoded.Width == 8 && decoded.Height == 9, "replay dimensions decode"); passed++;
            Expect(decoded.StateHash == snapshot.StateHash, "replay hash decodes"); passed++;

            var restored = new BoardModel(8, 9, 6, new SeededBoardRandom(999));
            restored.RestoreSnapshot(decoded);
            Expect(restored.ComputeStateHash() == snapshot.StateHash, "replay restores exact state"); passed++;

            const int expected = 22;
            Expect(passed == expected, "self-test count");

            Debug.Log(
                $"Legends of Rus v1.10 Step 2 BoardModel self-tests PASSED " +
                $"({passed}/{expected}). Seed={first.Seed}, hash={first.ComputeStateHash()}"
            );
            EditorUtility.DisplayDialog(
                "Legends of Rus — Tests",
                $"v1.10 Step 2 BoardModel self-tests passed: {passed}/{expected}\n\n" +
                $"Seed: {first.Seed}\nHash: {first.ComputeStateHash()}",
                "OK"
            );
        }

        static void Expect(bool condition, string name)
        {
            if (!condition)
                throw new InvalidOperationException($"BoardModel self-test failed: {name}");
        }
    }
}
#endif
