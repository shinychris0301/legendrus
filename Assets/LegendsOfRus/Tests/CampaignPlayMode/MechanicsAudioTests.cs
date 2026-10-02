using System;
using System.Collections;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public class MechanicsAudioTests
    {
        GameObject root, ownedState, ownedFeedback, listener;
        GameState state;
        PlayerSaveData oldSave;
        MechanicsSandbox sandbox;
        PrototypeBoardAudio Audio => sandbox.Board.GetComponent<PrototypeBoardAudio>();
        [SetUp] public void Setup()
        {
            state = GameState.Instance;
            if (!state) { ownedState = new GameObject("AudioTestState"); state = ownedState.AddComponent<GameState>(); }
            oldSave = state.Save; state.Save = new PlayerSaveData { sfx = true, vibration = true };
            if (!CoreLoopFeedbackService.Instance) { ownedFeedback = new GameObject("AudioTestFeedback"); ownedFeedback.AddComponent<CoreLoopFeedbackService>(); }
            if (UnityEngine.Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length == 0)
            { listener = new GameObject("TestListener"); listener.AddComponent<AudioListener>(); }
            root = new GameObject("AudioSandbox", typeof(RectTransform), typeof(Canvas));
            sandbox = MechanicsSandbox.Open((RectTransform)root.transform, () => {}, true);
        }
        [UnityTearDown] public IEnumerator Cleanup()
        {
            if (root) UnityEngine.Object.Destroy(root);
            if (state) state.Save = oldSave;
            if (ownedState) UnityEngine.Object.Destroy(ownedState);
            if (ownedFeedback) UnityEngine.Object.Destroy(ownedFeedback);
            if (listener) UnityEngine.Object.Destroy(listener);
            yield return null;
        }
        void Send(GameplayMotionKind kind, SpecialGemKind special = SpecialGemKind.None, GameplayBoosterKind booster = GameplayBoosterKind.None)
        {
            typeof(PrototypeBoardAudio).GetMethod("Motion", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Audio,
                new object[] { new GameplayMotionEvent(1, kind, new BoardPosition(3,3), new BoardPosition(4,3), 1, 1, special, "audio-test", boosterKind: booster) });
        }
        void Hit(BoardObstacleKind kind, int before, int after)
        {
            typeof(PrototypeBoardAudio).GetMethod("Obstacle", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Audio,
                new object[] { new BoardObstacleInteractionInstruction(new BoardPosition(3,3), kind, before, after, BoardObstacleHitCause.DirectClear) });
        }
        IEnumerator Beat(float seconds=.07f) { yield return new WaitForSecondsRealtime(seconds); yield return null; }
        [UnityTest] public IEnumerator AllClipsImportAndLegacyHasNoLocalAudio()
        {
            foreach (PrototypeBoardAudio.Cue cue in Enum.GetValues(typeof(PrototypeBoardAudio.Cue)))
            {
                var clip=Resources.Load<AudioClip>("MechanicsAudioR11/"+cue.ToString().ToLowerInvariant());
                Assert.That(clip, Is.Not.Null); Assert.That(clip.length, Is.InRange(.09f,.60f));
                Assert.That(clip.channels, Is.EqualTo(1));
            }
            Assert.That(Audio.GetComponents<AudioSource>().Length, Is.EqualTo(4));
            sandbox.Reset(0, GameplayRuleVersion.Legacy); yield return null;
            Assert.That(Audio, Is.Null); Assert.That(sandbox.Board.GetComponents<AudioSource>(), Is.Empty);
        }
        [UnityTest] public IEnumerator ActualBombAndSparkKeepPlansAndImpactTiming()
        {
            foreach(int fixture in new[]{1,10})
            {
                sandbox.Reset(fixture, GameplayRuleVersion.PrototypeR3); yield return null;
                var audio=Audio;
                MechanicsSandboxPlayModeTests.Capture(root,"AudioR11_"+fixture+"_Before",576,1280);
                float activatedAt=Time.unscaledTime;
                Assert.That(sandbox.Board.TapCell(3,3), Is.True);
                Assert.That(audio.PendingCount, Is.GreaterThan(0));
                Assert.That(audio.PlayedCount, Is.Zero, "no sound before scheduled presentation beat");
                yield return Beat(fixture==1?.18f:.08f);
                Assert.That(audio.LastPlayedAt(fixture==1?PrototypeBoardAudio.Cue.Bomb:PrototypeBoardAudio.Cue.Flight),
                    Is.GreaterThanOrEqualTo(activatedAt+(fixture==1?PrototypeImpactView.Anticipation:0)-.002f));
                MechanicsSandboxPlayModeTests.Capture(root,"AudioR11_"+fixture+"_Active",576,1280);
                float end=Time.realtimeSinceStartup+20;
                while(sandbox.Board.HasActiveCommand && Time.realtimeSinceStartup<end)yield return null;
                Assert.That(sandbox.Board.HasActiveCommand, Is.False);
                Assert.That(sandbox.Board.BoardStateHash, Is.EqualTo(sandbox.Board.LastMechanicsCommand.Resolve().FinalSnapshot.StateHash));
                Assert.That(audio.PlayedCount, Is.GreaterThan(1));
                if(fixture==10)Assert.That(audio.LastPlayedAt(PrototypeBoardAudio.Cue.Arrival),Is.GreaterThanOrEqualTo(activatedAt+SparkFlightView.Duration-.002f));
            }
        }
        [UnityTest] public IEnumerator MuteFocusAndDisableCancelPendingWithoutReplay()
        {
            Send(GameplayMotionKind.SpecialActivationStarted, SpecialGemKind.Spark);
            state.Save.sfx=false; yield return Beat(); Assert.That(Audio.PendingCount, Is.Zero); Assert.That(Audio.PlayedCount, Is.Zero);
            state.Save.sfx=true; yield return Beat(.6f); Assert.That(Audio.PlayedCount, Is.Zero);
            Send(GameplayMotionKind.SpecialActivationStarted, SpecialGemKind.Spark);
            Audio.SendMessage("OnApplicationFocus",false); yield return Beat(); Assert.That(Audio.PendingCount,Is.Zero);
            Audio.SendMessage("OnApplicationFocus",true);
            Send(GameplayMotionKind.SpecialActivationStarted, SpecialGemKind.Burst);
            Audio.enabled=false; yield return Beat(.2f); Assert.That(Audio.PendingCount,Is.Zero); Assert.That(Audio.PlayedCount,Is.Zero);
            Audio.enabled=true; yield return Beat(); Assert.That(Audio.PlayedCount,Is.Zero);
            Send(GameplayMotionKind.SpecialActivationStarted,SpecialGemKind.Spark);
            Audio.SendMessage("OnApplicationPause",true); yield return Beat(); Assert.That(Audio.PendingCount,Is.Zero);
            Audio.SendMessage("OnApplicationPause",false); yield return Beat(.6f); Assert.That(Audio.PlayedCount,Is.Zero);
        }
        [UnityTest] public IEnumerator BatchCoalescingAndPriorityKeepFourVoicesBounded()
        {
            for(int i=0;i<100;i++)Send(GameplayMotionKind.SpecialActivationStarted,SpecialGemKind.Burst);
            Assert.That(Audio.PendingCount, Is.EqualTo(1)); yield return Beat(.18f);
            Assert.That(Audio.PlayedCount, Is.EqualTo(1)); Assert.That(Audio.GetComponents<AudioSource>().Length,Is.EqualTo(4));
            Send(GameplayMotionKind.CommandRecovered); Assert.That(Audio.PendingCount,Is.Zero);
            foreach(var source in Audio.GetComponents<AudioSource>())Assert.That(source.isPlaying,Is.False);
        }
        [UnityTest] public IEnumerator MaterialsAndBoostersHaveDistinctCuesAndDamageStrength()
        {
            Hit(BoardObstacleKind.RunicSeal,2,2); yield return Beat(); Assert.That(Audio.PlayedCount,Is.Zero,"immune hit must stay silent");
            var kinds=new[]{BoardObstacleKind.Crate,BoardObstacleKind.Ice,BoardObstacleKind.Chain,BoardObstacleKind.RunicSeal};
            var cues=new[]{PrototypeBoardAudio.Cue.Wood,PrototypeBoardAudio.Cue.Ice,PrototypeBoardAudio.Cue.Metal,PrototypeBoardAudio.Cue.Seal};
            for(int i=0;i<kinds.Length;i++)
            {
                Hit(kinds[i],2,1); yield return Beat(); Assert.That(Audio.LastCue,Is.EqualTo(cues[i]));
                var sources=Audio.GetComponents<AudioSource>(); Assert.That(Array.Exists(sources,s=>s.clip&&s.clip.name==cues[i].ToString().ToLowerInvariant()&&s.pitch==.82f),Is.True);
                yield return Beat(.4f); Hit(kinds[i],1,0); yield return Beat();
                Assert.That(Array.Exists(sources,s=>s.clip&&s.clip.name==cues[i].ToString().ToLowerInvariant()&&s.pitch==1),Is.True);
                yield return Beat(.4f);
            }
            var boosters=new[]{GameplayBoosterKind.BogatyrMaceRow,GameplayBoosterKind.Firebird,GameplayBoosterKind.LivingLight,GameplayBoosterKind.SeaPearl};
            var expected=new[]{PrototypeBoardAudio.Cue.Mace,PrototypeBoardAudio.Cue.Firebird,PrototypeBoardAudio.Cue.Color,PrototypeBoardAudio.Cue.Renewal};
            for(int i=0;i<boosters.Length;i++){Send(GameplayMotionKind.BoosterActivated,booster:boosters[i]);yield return Beat(.18f);Assert.That(Audio.LastCue,Is.EqualTo(expected[i]));yield return Beat(.6f);}
        }
        sealed class Sink : ICoreLoopSfxSink, ICoreLoopHapticSink
        { public int Sounds,Pulses; public void Play(CoreLoopFeedbackSignal s)=>Sounds++; public void Pulse(CoreLoopFeedbackSignal s)=>Pulses++; }
        [Test] public void LocalAudioRetainsSharedHapticsWithoutDuplicateSfx()
        {
            var service=CoreLoopFeedbackService.Instance;var sink=new Sink();service.ConfigureSinks(sink,sink);
            var motion=new GameplayMotionEvent(1,GameplayMotionKind.BoosterActivated,new BoardPosition(),new BoardPosition(),1,1,SpecialGemKind.None,"test",boosterKind:GameplayBoosterKind.SeaPearl);
            CoreLoopFeedbackService.Publish(motion,true); Assert.That(sink.Sounds,Is.Zero);Assert.That(sink.Pulses,Is.EqualTo(1));
            CoreLoopFeedbackService.Publish(motion);Assert.That(sink.Sounds,Is.EqualTo(1));Assert.That(sink.Pulses,Is.EqualTo(2));
            service.ResetSinks();
        }
    }
}
