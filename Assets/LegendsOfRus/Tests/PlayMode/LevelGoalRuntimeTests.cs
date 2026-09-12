using System;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.PlayMode
{
    public sealed class LevelGoalRuntimeTests
    {
        [Test]
        public void CollectAndScoreGoals_UseWaveCountsAndAbsoluteScore()
        {
            var runtime = new LevelGoalRuntime(
                new[]
                {
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.CollectTile,
                        tileType = 1,
                        target = 3
                    },
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.Score,
                        target = 1200
                    }
                },
                1000
            );

            runtime.ApplyWave(CreateWave(new[] { 0, 2, 0 }), 1120);

            Assert.That(runtime.RemainingAt(0), Is.EqualTo(1));
            Assert.That(runtime.RemainingAt(1), Is.EqualTo(80));
            Assert.That(runtime.AllComplete, Is.False);

            runtime.ApplyWave(CreateWave(new[] { 0, 1, 0 }), 1220);

            Assert.That(runtime.AllComplete, Is.True);
        }

        [Test]
        public void ClearObstacleGoal_CountsMatchingDestructionsOnly()
        {
            var runtime = new LevelGoalRuntime(
                new[]
                {
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.ClearObstacle,
                        obstacleType = LevelObstacleTypes.Ice,
                        target = 1
                    }
                },
                0
            );
            var interactions = new[]
            {
                new BoardObstacleInteractionInstruction(
                    new BoardPosition(0, 0),
                    BoardObstacleKind.Ice,
                    2,
                    1,
                    BoardObstacleHitCause.DirectClear
                ),
                new BoardObstacleInteractionInstruction(
                    new BoardPosition(1, 0),
                    BoardObstacleKind.Crate,
                    1,
                    0,
                    BoardObstacleHitCause.AdjacentMatch
                ),
                new BoardObstacleInteractionInstruction(
                    new BoardPosition(2, 0),
                    BoardObstacleKind.Ice,
                    1,
                    0,
                    BoardObstacleHitCause.DirectClear
                )
            };

            runtime.ApplyWave(
                CreateWave(new int[3], interactions),
                0
            );

            Assert.That(runtime.RemainingAt(0), Is.Zero);
            Assert.That(runtime.AllComplete, Is.True);
        }

        [Test]
        public void BringDownAndSurvivalGoals_UseExitEventsAndConsumedMoves()
        {
            var runtime = new LevelGoalRuntime(
                new[]
                {
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.BringDown,
                        itemType = LevelBoardItemTypes.FirebirdFeather,
                        target = 2
                    },
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.Survival,
                        target = 2
                    }
                },
                0
            );

            runtime.RecordConsumedMove();
            runtime.ApplyWave(
                CreateWave(
                    new int[3],
                    null,
                    new[]
                    {
                        new BoardObjectiveItemCollection(
                            new BoardPosition(0, 2),
                            new BoardPosition(0, 3),
                            BoardObjectiveItemKind.FirebirdFeather
                        )
                    }
                ),
                0
            );

            Assert.That(runtime.RemainingAt(0), Is.EqualTo(1));
            Assert.That(runtime.RemainingAt(1), Is.EqualTo(1));

            runtime.RecordConsumedMove();
            runtime.ApplyWave(
                CreateWave(
                    new int[3],
                    null,
                    new[]
                    {
                        new BoardObjectiveItemCollection(
                            new BoardPosition(1, 2),
                            new BoardPosition(1, 3),
                            BoardObjectiveItemKind.FirebirdFeather
                        )
                    }
                ),
                0
            );

            Assert.That(runtime.AllComplete, Is.True);
        }

        [Test]
        public void GoalSnapshot_RestoresAtomicCommandProgress()
        {
            var runtime = new LevelGoalRuntime(
                new[]
                {
                    new LevelGoalData
                    {
                        type = LevelGoalTypes.Survival,
                        target = 3
                    }
                },
                0
            );
            runtime.RecordConsumedMove();
            LevelGoalRuntimeSnapshot snapshot = runtime.CaptureSnapshot();

            runtime.RecordConsumedMove();
            runtime.RecordConsumedMove();
            Assert.That(runtime.AllComplete, Is.True);

            runtime.RestoreSnapshot(snapshot);

            Assert.That(runtime.RemainingAt(0), Is.EqualTo(2));
            Assert.That(runtime.AllComplete, Is.False);
        }

        static BoardResolutionWave CreateWave(
            IReadOnlyList<int> clearedByType,
            IReadOnlyList<BoardObstacleInteractionInstruction> interactions = null,
            IReadOnlyList<BoardObjectiveItemCollection> collections = null
        )
        {
            var collapse = new BoardCollapseResult();
            if (collections != null)
            {
                for (int i = 0; i < collections.Count; i++)
                    collapse.CollectedObjectiveItems.Add(collections[i]);
            }

            return new BoardResolutionWave(
                1,
                SpecialCombinationKind.None,
                BoardBoosterCommandKind.None,
                Array.Empty<BoardMatchRun>(),
                Array.Empty<BoardPosition>(),
                Array.Empty<BoardSpecialCreationInstruction>(),
                Array.Empty<BoardSpecialActivationInstruction>(),
                Array.Empty<BoardClearedCellInstruction>(),
                interactions ?? Array.Empty<BoardObstacleInteractionInstruction>(),
                clearedByType,
                0,
                null,
                collapse,
                null
            );
        }
    }
}
