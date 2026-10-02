using System;
using System.Collections;
using NUnit.Framework;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public class SparkSandboxPlayModeTests
    {
        GameObject root;
        [UnityTearDown] public IEnumerator Cleanup(){if(root)UnityEngine.Object.Destroy(root);yield return null;}
        [UnityTest] public IEnumerator SquareCreationAndAllSparkCombinationsReachRecordedOutcome()
        {
            root=new GameObject("SparkCapture",typeof(RectTransform),typeof(Canvas));
            var s=MechanicsSandbox.Open((RectTransform)root.transform,()=>{},true);yield return null;
            for(int fixture=9;fixture<16;fixture++)
            {
                s.Reset(fixture,GameplayRuleVersion.PrototypeR2);yield return null;
                if(fixture==10)MechanicsSandboxPlayModeTests.Capture(root,"Spark_Before",576,1280);
                bool accepted=fixture==9?s.Board.SwipeCell(4,3,Vector2Int.left):fixture>=11&&fixture<=14?s.Board.SwipeCell(3,3,Vector2Int.right):s.Board.TapCell(3,3);
                Assert.That(accepted,Is.True,"fixture "+fixture);
                if(fixture==10)
                    for(int i=0;i<16;i++){yield return new WaitForSecondsRealtime(.025f);MechanicsSandboxPlayModeTests.Capture(root,"Spark_Flight_"+i.ToString("D2"),576,1280);}
                float end=Time.realtimeSinceStartup+20;while(s.Board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;
                Assert.That(s.Board.HasActiveCommand,Is.False,"fixture "+fixture);Assert.That(s.Board.Moves,Is.EqualTo(49));Assert.That(s.Board.CanAcceptPointerInput,Is.True);
                Assert.That(s.Board.BoardStateHash,Is.EqualTo(s.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));
                var flights=s.Board.GetComponentInChildren<SparkFlightView>();Assert.That(flights,Is.Not.Null);
                if(fixture!=9)Assert.That(flights.FlightCount,Is.GreaterThan(0));
                if(fixture==15)Assert.That(flights.FlightCount,Is.GreaterThanOrEqualTo(2));
                if(fixture==10){Assert.That(flights.LastTarget,Is.EqualTo(new BoardPosition(6,1)));MechanicsSandboxPlayModeTests.Capture(root,"Spark_After",576,1280);}
                if(fixture==9)MechanicsSandboxPlayModeTests.Capture(root,"Spark_Created",576,1280);
            }
            s.Reset(0,GameplayRuleVersion.Legacy);yield return null;Assert.That(s.Board.GetComponentInChildren<SparkFlightView>(),Is.Null);
        }
    }
}
