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

        [Test]
        public void ThirdGoalGatesCompletionAndRestoresWithCommandSnapshot()
        {
            var runtime=new LevelGoalRuntime(new[]{
                new LevelGoalData{type=LevelGoalTypes.CollectTile,tileType=0,target=3},
                new LevelGoalData{type=LevelGoalTypes.Score,target=100},
                new LevelGoalData{type=LevelGoalTypes.CollectTile,tileType=2,target=4}},0);
            runtime.ApplyWave(CreateWave(new[]{3,0,1}),100);
            Assert.That(runtime.SlotCount,Is.EqualTo(3));
            Assert.That(runtime.GetSlot(0).IsComplete&&runtime.GetSlot(1).IsComplete,Is.True);
            Assert.That(runtime.AllComplete,Is.False,"The unfinished third mission must prevent victory.");
            var before=runtime.CaptureSnapshot();
            runtime.ApplyWave(CreateWave(new[]{0,0,3}),120);
            Assert.That(runtime.AllComplete,Is.True);
            runtime.RestoreSnapshot(before);
            Assert.That(runtime.AllComplete,Is.False);Assert.That(runtime.RemainingAt(2),Is.EqualTo(3));
        }

        [Test]
        public void StagedChapterBoardsRemainPlayableAcrossSeeds()
        {
            var catalog=ChapterLevelCatalog.LoadChapter1();
            var report=LevelDataValidator.ValidateCampaignCatalog(catalog);
            Assert.That(report.IsValid,Is.True,string.Join("\n",report.Errors));
            Assert.That(catalog.levels.Length,Is.EqualTo(500));
            Assert.That(Chapter01ProgressionValidator.Validate(LegacyChapterTestFixture.Levels()).IsValid,Is.True,"The ten-round teaching validator applies to the retained historical fixture.");
            foreach(var level in catalog.levels)
            {
                Assert.That(level.columns,Is.EqualTo(8));Assert.That(level.rows,Is.EqualTo(7));
                Assert.That(level.goals.Length,Is.InRange(1,3),level.id);
                for(int offset=0;offset<12;offset++)
                {
                    var board=LevelBoardFactory.Create(level,level.boardSeed+offset,rules:Match3Board.CurrentGameplayRules);
                    Assert.That(board.HasAnyMatches(),Is.False,level.id);Assert.That(board.HasLegalMove(),Is.True,level.id);
                    foreach(var cell in level.fixedCells)
                    {
                        var position=new BoardPosition(cell.x,cell.y);
                        Assert.That(board.GetTile(position),Is.EqualTo(cell.tileType));
                        Assert.That((int)board.GetSpecial(position),Is.EqualTo(cell.special));
                    }
                }
            }
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
