using System;
using System.Linq;
using NUnit.Framework;
using LegendsOfRus.GameplayCore;

namespace LegendsOfRus.Tests
{
    public class SparkMechanicsTests
    {
        static BoardModel Board(int seed=1)
        {
            var b=new BoardModel(8,7,6,new SeededBoardRandom(seed)){SquareMatches=true};
            for(int y=0;y<7;y++)for(int x=0;x<8;x++)b.SetCell(new BoardPosition(x,y),new BoardCellData((x+2*y)%5));
            return b;
        }
        static BoardResolutionPlanner Planner(BoardModel b)=>new BoardResolutionPlanner(b,null,GameplayRuleVersion.PrototypeR2);
        static BoardPosition P(int x,int y)=>new BoardPosition(x,y);
        static void Set(BoardModel b,int x,int y,int color=1,SpecialGemKind kind=SpecialGemKind.None)=>b.SetCell(P(x,y),new BoardCellData(color,kind));
        static void SquareSetup(BoardModel b)
        {
            Set(b,2,2,5);Set(b,3,2,5);Set(b,2,3,5);Set(b,3,3,3);Set(b,4,3,5);
        }
        [Test] public void HintFindsOnlySquareMoveAndDoesNotReshuffleIt()
        {
            var b=new BoardModel(4,4,5,new SeededBoardRandom(1));
            int[] tiles={0,1,2,3,1,4,4,0,2,4,3,4,3,0,1,2};
            for(int i=0;i<tiles.Length;i++)b.SetCell(P(i%4,i/4),new BoardCellData(tiles[i]));
            foreach(int i in new[]{6,7,9})b.SetObstacle(P(i%4,i/4),new BoardObstacleData(BoardObstacleKind.Chain,1));
            Assert.That(b.HasLegalMove(),Is.False);
            b.SquareMatches=true;
            Assert.That(b.TryFindLegalAction(out var from,out var to),Is.True);
            Assert.That(from,Is.EqualTo(P(2,2)));Assert.That(to,Is.EqualTo(P(3,2)));
            Assert.That(b.EnsurePlayableBoard().Attempts,Is.Zero);
            Assert.That(Planner(b).PlanSwap(from,to).IsResolved,Is.True);
        }

        [Test] public void SquareOnlySwapParticipatesInLegalMoveSearchAndCreation()
        {
            var b=Board();SquareSetup(b);string hash=b.ComputeStateHash();
            Assert.That(b.FindRuns(),Is.Empty);Assert.That(b.SwapCreatesMatch(P(4,3),P(3,3)),Is.True);Assert.That(b.HasLegalMove(),Is.True);
            var plan=Planner(b).PlanSwap(P(4,3),P(3,3));
            Assert.That(plan.IsResolved,Is.True);Assert.That(plan.Waves[0].Creations.Single().Kind,Is.EqualTo(SpecialGemKind.Spark));
            Assert.That(plan.Waves[0].Creations.Single().Position,Is.EqualTo(P(3,3)));Assert.That(b.ComputeStateHash(),Is.EqualTo(hash));
            b.SquareMatches=false;Assert.That(b.SwapCreatesMatch(P(4,3),P(3,3)),Is.False);
            Assert.That(new BoardResolutionPlanner(b).PlanSwap(P(4,3),P(3,3)).IsResolved,Is.False);
        }
        [Test] public void ConnectedSquaresCreateOneSpark()
        {
            var b=Board();for(int y=2;y<=3;y++)for(int x=2;x<=4;x++)Set(b,x,y,5);
            var creations=b.DetermineSpecialCreations(b.FindRuns(),P(2,2),true);
            Assert.That(creations.Count,Is.EqualTo(1));Assert.That(creations.Values.Single(),Is.EqualTo(SpecialGemKind.Spark));
        }
        [TestCase(4,SpecialGemKind.Spark)][TestCase(5,SpecialGemKind.Color)]
        public void ConnectedPriorityRespectsSquareAndFive(int length,SpecialGemKind expected)
        {
            var b=Board();for(int x=1;x<1+length;x++)Set(b,x,2,5);Set(b,1,3,5);Set(b,2,3,5);
            var c=b.DetermineSpecialCreations(b.FindRuns(),null,true);Assert.That(c.Count,Is.EqualTo(1));Assert.That(c.Values.Single(),Is.EqualTo(expected));
        }
        [Test] public void IntersectionBeatsSquareAndExistingSpecialIsNotOverwritten()
        {
            var b=Board();for(int x=1;x<=3;x++)Set(b,x,2,5);for(int y=1;y<=3;y++)Set(b,2,y,5);Set(b,1,3,5);
            Assert.That(b.DetermineSpecialCreations(b.FindRuns(),null,true).Values.Single(),Is.EqualTo(SpecialGemKind.Burst));
            Set(b,2,2,5,SpecialGemKind.Row);Assert.That(b.DetermineSpecialCreations(b.FindRuns(),P(2,2),true),Is.Empty);
        }
        [Test] public void TapTargetsObstacleAndClearsOnlyLaunchCrossAndTarget()
        {
            var b=Board();Set(b,3,3,1,SpecialGemKind.Spark);b.SetObstacle(P(6,1),new BoardObstacleData(BoardObstacleKind.Ice,2));
            var p=Planner(b).PlanTapSpecial(P(3,3));var w=p.Waves[0];var f=w.Activations.Single();
            Assert.That(f.Target,Is.EqualTo(P(6,1)));Assert.That(w.ClearedCells.Count,Is.EqualTo(6));Assert.That(w.ObstacleInteractions.Single().LayersAfter,Is.EqualTo(1));Assert.That(p.ConsumesMove,Is.True);
        }
        [Test] public void LockedSparkRejectsAndVersionMismatchFailsClosed()
        {
            var b=Board();Set(b,3,3,1,SpecialGemKind.Spark);b.SetObstacle(P(3,3),new BoardObstacleData(BoardObstacleKind.Chain,1));
            Assert.That(Planner(b).PlanTapSpecial(P(3,3)).ConsumesMove,Is.False);
            Assert.Throws<ArgumentException>(()=>new BoardResolutionPlanner(b,null,GameplayRuleVersion.PrototypeR1));
        }
        [Test] public void TwoSparksHaveThreeUniqueTargetsAndReplayPinsThem()
        {
            var b=Board();Set(b,3,3,1,SpecialGemKind.Spark);Set(b,4,3,2,SpecialGemKind.Spark);
            var cmd=new MechanicsReplayCommand{Rules=GameplayRuleVersion.PrototypeR2,BeforeToken=b.CreateSnapshot().ToReplayToken(),X=3,Y=3,ToX=4,ToY=3};
            var p=cmd.Resolve();cmd.ExpectedPlanHash=p.PlanHash;
            var flights=p.Waves[0].Activations.Where(a=>a.Kind==SpecialGemKind.Spark).ToArray();Assert.That(flights.Length,Is.EqualTo(3));Assert.That(flights.Select(f=>f.Target).Distinct().Count(),Is.EqualTo(3));
            Assert.That(cmd.Resolve().FinalSnapshot.StateHash,Is.EqualTo(p.FinalSnapshot.StateHash));
            Assert.That(BoardReplayCodec.Decode(p.FinalSnapshot.ToReplayToken()).StateHash,Is.EqualTo(p.FinalSnapshot.StateHash));
        }
        [TestCase(SpecialGemKind.Row)][TestCase(SpecialGemKind.Column)][TestCase(SpecialGemKind.Burst)]
        public void SparkCarriesPartnerToLanding(SpecialGemKind payload)
        {
            var b=Board();Set(b,3,3,1,SpecialGemKind.Spark);Set(b,4,3,2,payload);b.SetObstacle(P(6,1),new BoardObstacleData(BoardObstacleKind.Ice,2));
            var wave=Planner(b).PlanSwap(P(3,3),P(4,3)).Waves[0];var flight=wave.Activations.Single(a=>a.Kind==SpecialGemKind.Spark);
            Assert.That(flight.Payload,Is.EqualTo(payload));Assert.That(flight.Target,Is.EqualTo(P(6,1)));
            Assert.That(wave.ClearedCells.Any(c=>c.Position==(payload==SpecialGemKind.Column?P(6,6):payload==SpecialGemKind.Row?P(0,1):P(5,0))),Is.True);
            Assert.That(wave.Activations.Any(a=>a.Position==P(3,3)&&a.Kind==payload),Is.False,"Payload must not also fire at its source");
        }
        [Test] public void ColorTransformsChosenGroupIntoFlights()
        {
            var b=Board();Set(b,3,3,1,SpecialGemKind.Spark);Set(b,4,3,2,SpecialGemKind.Color);
            int count=b.CreateSnapshot().Tiles.Count(x=>x==1);
            var wave=Planner(b).PlanSwap(P(3,3),P(4,3)).Waves[0];
            Assert.That(wave.Activations.Count(a=>a.Kind==SpecialGemKind.Spark),Is.EqualTo(count));
            Assert.That(wave.Activations.Where(a=>a.Kind==SpecialGemKind.Spark).All(a=>a.Target.HasValue),Is.True);
        }
        [Test] public void SparkReachedByMatchFiresAndProducesResolvedDestination()
        {
            var b=Board();Set(b,1,2,2);Set(b,2,2,2,SpecialGemKind.Spark);Set(b,3,2,3);Set(b,3,3,2);
            var plan=Planner(b).PlanSwap(P(3,3),P(3,2));
            Assert.That(plan.Waves[0].Activations.Any(a=>a.Kind==SpecialGemKind.Spark&&a.Target.HasValue),Is.True);
        }
        [Test] public void MatchingSquareOnlyBoardIsReshuffledWithoutLeavingSquares()
        {
            var b=Board();for(int x=1;x<=2;x++)for(int y=1;y<=2;y++)Set(b,x,y,5);
            Assert.That(b.HasAnyMatches(),Is.True);Assert.That(b.EnsurePlayableBoard().Attempts,Is.GreaterThan(0));Assert.That(b.HasAnyMatches(),Is.False);Assert.That(b.HasLegalMove(),Is.True);
        }
        [Test] public void InventoryEffectTriggersSparkWithoutSpendingMove()
        {
            var b=Board();Set(b,3,3,1,SpecialGemKind.Spark);
            var p=Planner(b).PlanBooster(new BoardBoosterRequest(BoardBoosterCommandKind.FirebirdArea,P(3,3)));
            Assert.That(p.IsResolved,Is.True);Assert.That(p.ConsumesMove,Is.False);
            Assert.That(p.Waves[0].Activations.Any(a=>a.Kind==SpecialGemKind.Spark&&a.Target.HasValue),Is.True);
        }
        [Test] public void RefillsCanCreateSparkInLaterCascade()
        {
            for(int seed=1;seed<=256;seed++)
            {
                var b=Board(seed);b.GeneratePlayableBoard();Set(b,3,3,1,SpecialGemKind.Burst);
                var p=Planner(b).PlanTapSpecial(P(3,3));
                if(!p.Waves.Skip(1).Any(w=>w.Creations.Any(c=>c.Kind==SpecialGemKind.Spark)))continue;
                var final=Board();final.RestoreSnapshot(p.FinalSnapshot);
                Assert.That(final.HasAnyMatches(),Is.False);
                Assert.That(p.PlanHash,Is.EqualTo(Planner(b).PlanTapSpecial(P(3,3)).PlanHash));
                TestContext.WriteLine("Square cascade verified with generated board seed "+seed);
                return;
            }
            Assert.Fail("No square cascade exercised within deterministic seed set.");
        }

        [Test] public void EdgeLaunchAndDenseChainResolveDeterministically()
        {
            for(int seed=1;seed<=12;seed++)
            {
                var b=Board(seed);b.GeneratePlayableBoard();
                Set(b,0,0,1,SpecialGemKind.Spark);Set(b,7,6,2,SpecialGemKind.Spark);
                var p=Planner(b).PlanTapSpecial(P(0,0));Assert.That(p.PlanHash,Is.EqualTo(Planner(b).PlanTapSpecial(P(0,0)).PlanHash));
                var final=Board();final.RestoreSnapshot(p.FinalSnapshot);Assert.That(final.HasAnyMatches(),Is.False);
                Assert.That(p.Waves.SelectMany(w=>w.Activations).Where(a=>a.Kind==SpecialGemKind.Spark).All(a=>a.Target.HasValue&&b.IsInside(a.Target.Value)),Is.True);
            }
        }
    }
}
