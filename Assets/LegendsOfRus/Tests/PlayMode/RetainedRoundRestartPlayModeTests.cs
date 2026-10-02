using System;
using System.Collections;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.PlayMode
{
    // Three independent Editor processes share only an isolated save namespace.
    public sealed class RetainedRoundRestartPlayModeTests
    {
        const BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
        string Checkpoint => Environment.GetEnvironmentVariable("LOR_PERSISTENCE_CHECKPOINT");
        void Guard(string phase)
        {
            if (Environment.GetEnvironmentVariable("LOR_PERSISTENCE_PHASE") != phase) Assert.Ignore("Requires the named isolated restart phase.");
            string config=Environment.GetEnvironmentVariable("LOR_LOADING_TEST_CONFIG_ROOT");
            Assert.That(config,Does.StartWith("/tmp/lor-retained-round-r1/"));
            Assert.That(Application.persistentDataPath,Does.StartWith(config+"/"));
            Assert.That(Environment.GetEnvironmentVariable("LOR_PERSISTENCE_PHASE"),Is.EqualTo(phase));
            Assert.That(Checkpoint,Does.StartWith("/tmp/lor-retained-round-r1/"));
        }
        void Settle(){for(int i=0;i<24;i++)ScreenRouter.Instance.MotionPresenter.TickForTesting(MetaUiMotionProfile.MaximumTickSeconds);Canvas.ForceUpdateCanvases();}
        [UnityTest] public IEnumerator WriteFailedRound()
        {
            Guard("retained-write");var state=GameState.Instance;state.ResetProgress();state.Save.highestUnlockedLevel=2;
            state.SelectLevel(1,2);Assert.That(state.SpendEnergy(),Is.True);
            ScreenRouter.Instance.Show(ScreenId.Gameplay);Settle();yield return null;
            var board=ScreenRouter.Instance.Viewport.GetComponentInChildren<Match3Board>();
            Assert.That(board,Is.Not.Null);
            // Controlled near-failure fixture; the owning board's end-state and router save path are real.
            typeof(Match3Board).GetField("moves",Private).SetValue(board,0);
            typeof(Match3Board).GetField("score",Private).SetValue(board,567);
            if (board.Phase == LegendsOfRus.GameplayCore.GameplayPhase.Paused) board.TryResume();
            yield return (IEnumerator)typeof(Match3Board).GetMethod("CheckEndState",Private).Invoke(board,null);Settle();yield return null;
            Assert.That(ScreenRouter.Instance.Viewport.GetComponentInChildren<ProductionFailView>(),Is.Not.Null);
            Assert.That(state.TryGetRetainedRound(out var retained),Is.True);Assert.That(state.GetLevelProgress(1,2).failures,Is.Zero);
            Assert.That(state.HasUnsavedRetainedRound,Is.False);
            File.WriteAllText(Checkpoint,JsonUtility.ToJson(state.Save));
        }
        [UnityTest] public IEnumerator ReadFailureAfterRestartAndDecline()
        {
            Guard("retained-read");var expected=JsonUtility.FromJson<PlayerSaveData>(File.ReadAllText(Checkpoint));var state=GameState.Instance;
            Assert.That(state.TryGetRetainedRound(out var restored),Is.True);
            Assert.That(JsonUtility.ToJson(restored),Is.EqualTo(JsonUtility.ToJson(expected.retainedRound)));
            Assert.That(state.Save.energy,Is.EqualTo(expected.energy));
            ScreenRouter.Instance.Show(ScreenId.Home);Settle();yield return null;
            var view=ScreenRouter.Instance.Viewport.GetComponentInChildren<ProductionFailView>();Assert.That(view,Is.Not.Null);
            Assert.That(view.Score.text,Does.Contain("567"));Assert.That(state.GetLevelProgress(1,2).attempts,Is.EqualTo(1));
            Assert.That(state.GetLevelProgress(1,2).failures,Is.Zero);
            view.Exit.onClick.Invoke();view.Exit.onClick.Invoke();Settle();yield return null;
            Assert.That(state.TryGetRetainedRound(out _),Is.False);Assert.That(state.GetLevelProgress(1,2).failures,Is.EqualTo(1));
            Assert.That(state.Save.energy,Is.EqualTo(expected.energy));Assert.That(state.Save.gold,Is.EqualTo(expected.gold));
        }
        [UnityTest] public IEnumerator ReadDeclinedRoundAfterSecondRestart()
        {
            Guard("retained-cleared");var state=GameState.Instance;
            Assert.That(state.Save.retainedRound,Is.Null);Assert.That(state.GetLevelProgress(1,2).failures,Is.EqualTo(1));
            Assert.That(state.GetLevelProgress(1,2).attempts,Is.EqualTo(1));
            ScreenRouter.Instance.Show(ScreenId.Home);Settle();yield return null;
            Assert.That(ScreenRouter.Instance.Viewport.GetComponentInChildren<ProductionFailView>(),Is.Null);
        }
    }
}
