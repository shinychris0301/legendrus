using System;
using LegendsOfRus.GameplayCore;
using UnityEngine;

namespace LegendsOfRus
{
    public enum CoreLoopFeedbackCue
    {
        SwapAccepted = 0,
        SwapRejected,
        InvalidSwap,
        MatchResolved,
        CascadeResolved,
        FallSettled,
        SpecialCreated,
        SpecialActivated,
        Reshuffled,
        CommandRecovered,
        BoosterActivated,
        BoosterCompleted,
        BoosterRejected
    }

    public sealed class CoreLoopFeedbackSignal
    {
        public CoreLoopFeedbackCue Cue { get; }
        public int Magnitude { get; }
        public int CascadeDepth { get; }
        public string PlanHash { get; }
        public GameplayBoosterKind BoosterKind { get; }
        public int Sequence { get; }
        public SpecialGemKind SpecialKind { get; }
        public SpecialCombinationKind CombinationKind { get; }

        public CoreLoopFeedbackSignal(
            CoreLoopFeedbackCue cue,
            int magnitude,
            int cascadeDepth,
            string planHash,
            GameplayBoosterKind boosterKind = GameplayBoosterKind.None,
            int sequence = 1,
            SpecialGemKind specialKind = SpecialGemKind.None,
            SpecialCombinationKind combinationKind = SpecialCombinationKind.None
        )
        {
            Cue = cue;
            Magnitude = Mathf.Max(0, magnitude);
            CascadeDepth = Mathf.Max(0, cascadeDepth);
            PlanHash = planHash ?? string.Empty;
            BoosterKind = boosterKind;
            Sequence = sequence < 1 ? 1 : sequence;
            SpecialKind = specialKind;
            CombinationKind = combinationKind;
        }
    }

    public interface ICoreLoopSfxSink
    {
        void Play(CoreLoopFeedbackSignal signal);
    }

    public interface ICoreLoopHapticSink
    {
        void Pulse(CoreLoopFeedbackSignal signal);
    }

    sealed class NullCoreLoopSfxSink : ICoreLoopSfxSink
    {
        public static readonly NullCoreLoopSfxSink Instance =
            new NullCoreLoopSfxSink();

        NullCoreLoopSfxSink() { }

        public void Play(CoreLoopFeedbackSignal signal) { }
    }

    sealed class NullCoreLoopHapticSink : ICoreLoopHapticSink
    {
        public static readonly NullCoreLoopHapticSink Instance =
            new NullCoreLoopHapticSink();

        NullCoreLoopHapticSink() { }

        public void Pulse(CoreLoopFeedbackSignal signal) { }
    }

    /// <summary>
    /// Optional output boundary for core-loop sound and haptics. It ships
    /// with no-op sinks, so missing clips, mobile APIs, or providers can never
    /// stop a board command. Settings are read on every signal.
    /// </summary>
    public sealed class CoreLoopFeedbackService : MonoBehaviour
    {
        public static CoreLoopFeedbackService Instance { get; private set; }

        ICoreLoopSfxSink sfxSink = NullCoreLoopSfxSink.Instance;
        ICoreLoopHapticSink hapticSink = NullCoreLoopHapticSink.Instance;
        bool warnedSfxFailure;
        bool warnedHapticFailure;
        bool warnedObserverFailure;

        public int SignalCount { get; private set; }
        public CoreLoopFeedbackSignal LastSignal { get; private set; }

        public event Action<CoreLoopFeedbackSignal> SignalPublished;

        public bool MusicEnabled => IsSettingEnabled(save => save.music);
        public bool SfxEnabled => IsSettingEnabled(save => save.sfx);
        public bool VibrationEnabled => IsSettingEnabled(save => save.vibration);

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            ResetSinks();
        }

        void OnDestroy()
        {
            if (Instance != this) return;
            ResetSinks();
            Instance = null;
        }

        public void ConfigureSinks(
            ICoreLoopSfxSink newSfxSink,
            ICoreLoopHapticSink newHapticSink
        )
        {
            sfxSink = newSfxSink ?? NullCoreLoopSfxSink.Instance;
            hapticSink = newHapticSink ?? NullCoreLoopHapticSink.Instance;
            warnedSfxFailure = false;
            warnedHapticFailure = false;
            warnedObserverFailure = false;
        }

        public void ResetSinks()
        {
            sfxSink = NullCoreLoopSfxSink.Instance;
            hapticSink = NullCoreLoopHapticSink.Instance;
            warnedSfxFailure = false;
            warnedHapticFailure = false;
            warnedObserverFailure = false;
        }

        public static void Publish(GameplayMotionEvent motion)
        {
            CoreLoopFeedbackService service = Instance;
            if (
                service == null ||
                !TryCreateSignal(motion, out CoreLoopFeedbackSignal signal, out bool haptic)
            )
            {
                return;
            }

            service.Emit(signal, haptic);
        }

        public static bool TryCreateSignal(
            GameplayMotionEvent motion,
            out CoreLoopFeedbackSignal signal,
            out bool requestHaptic
        )
        {
            signal = null;
            requestHaptic = false;
            if (motion == null) return false;

            CoreLoopFeedbackCue cue;
            switch (motion.Kind)
            {
                case GameplayMotionKind.SwapStarted:
                    cue = CoreLoopFeedbackCue.SwapAccepted;
                    break;
                case GameplayMotionKind.SwapRejected:
                    cue = CoreLoopFeedbackCue.SwapRejected;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.SwapReturnStarted:
                    cue = CoreLoopFeedbackCue.InvalidSwap;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.MatchWaveStarted:
                    // CascadeStarted is emitted immediately before later
                    // waves. It owns that wave-level cue so one deterministic
                    // board beat cannot request two back-to-back pulses.
                    if (
                        motion.ComboIndex > 1 ||
                        motion.BoosterKind != GameplayBoosterKind.None
                    ) return false;
                    cue = CoreLoopFeedbackCue.MatchResolved;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.CascadeStarted:
                    cue = CoreLoopFeedbackCue.CascadeResolved;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.FallCompleted:
                    cue = CoreLoopFeedbackCue.FallSettled;
                    break;
                case GameplayMotionKind.SpecialCreationStarted:
                    cue = CoreLoopFeedbackCue.SpecialCreated;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.SpecialActivationStarted:
                    // Match3Board emits one semantic hook per deterministic
                    // activation instruction. Only the first carries the batch
                    // magnitude, preventing dozens of audio/haptic requests for
                    // color-special combinations.
                    if (
                        motion.AffectedCount == 0 ||
                        motion.BoosterKind != GameplayBoosterKind.None
                    ) return false;
                    cue = CoreLoopFeedbackCue.SpecialActivated;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.ReshuffleCompleted:
                    cue = CoreLoopFeedbackCue.Reshuffled;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.CommandRecovered:
                    cue = CoreLoopFeedbackCue.CommandRecovered;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.BoosterActivated:
                    cue = CoreLoopFeedbackCue.BoosterActivated;
                    requestHaptic = true;
                    break;
                case GameplayMotionKind.BoosterCompleted:
                    cue = CoreLoopFeedbackCue.BoosterCompleted;
                    break;
                case GameplayMotionKind.BoosterRejected:
                    cue = CoreLoopFeedbackCue.BoosterRejected;
                    requestHaptic = true;
                    break;
                default:
                    return false;
            }

            signal = new CoreLoopFeedbackSignal(
                cue,
                motion.AffectedCount,
                motion.ComboIndex,
                motion.PlanHash,
                motion.BoosterKind,
                motion.Sequence,
                motion.SpecialKind,
                motion.CombinationKind
            );
            return true;
        }

        void Emit(CoreLoopFeedbackSignal signal, bool requestHaptic)
        {
            LastSignal = signal;
            SignalCount++;
            SafePublish(signal);

            if (SfxEnabled)
            {
                try
                {
                    sfxSink.Play(signal);
                }
                catch (Exception exception)
                {
                    if (!warnedSfxFailure)
                    {
                        warnedSfxFailure = true;
                        Debug.LogWarning(
                            "Core-loop SFX sink failed and was isolated: " +
                            exception.Message
                        );
                    }
                }
            }

            if (requestHaptic && VibrationEnabled)
            {
                try
                {
                    hapticSink.Pulse(signal);
                }
                catch (Exception exception)
                {
                    if (!warnedHapticFailure)
                    {
                        warnedHapticFailure = true;
                        Debug.LogWarning(
                            "Core-loop haptic sink failed and was isolated: " +
                            exception.Message
                        );
                    }
                }
            }
        }

        void SafePublish(CoreLoopFeedbackSignal signal)
        {
            Action<CoreLoopFeedbackSignal> handlers = SignalPublished;
            if (handlers == null) return;

            foreach (Delegate candidate in handlers.GetInvocationList())
            {
                try
                {
                    ((Action<CoreLoopFeedbackSignal>)candidate)(signal);
                }
                catch (Exception exception)
                {
                    if (!warnedObserverFailure)
                    {
                        warnedObserverFailure = true;
                        Debug.LogWarning(
                            "Core-loop feedback observer failed and was isolated: " +
                            exception.Message
                        );
                    }
                }
            }
        }

        static bool IsSettingEnabled(Func<PlayerSaveData, bool> selector)
        {
            GameState state = GameState.Instance;
            return state != null && state.Save != null && selector(state.Save);
        }
    }
}
