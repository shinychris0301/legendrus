using System;
using UnityEngine;

namespace LegendsOfRus
{
    public readonly struct MetaUiFeedbackSignal
    {
        public MetaUiMotionCue Cue { get; }
        public MetaUiControlRole Role { get; }
        public MetaUiMotionPreset Preset { get; }
        public string Target { get; }

        public MetaUiFeedbackSignal(
            MetaUiMotionCue cue,
            MetaUiControlRole role,
            MetaUiMotionPreset preset,
            string target
        )
        {
            Cue = cue;
            Role = role;
            Preset = preset;
            Target = target ?? string.Empty;
        }
    }

    public interface IMetaUiSfxSink
    {
        void Play(MetaUiFeedbackSignal signal);
    }

    public interface IMetaUiHapticSink
    {
        void Pulse(MetaUiFeedbackSignal signal);
    }

    sealed class NullMetaUiSfxSink : IMetaUiSfxSink
    {
        public static readonly NullMetaUiSfxSink Instance =
            new NullMetaUiSfxSink();

        NullMetaUiSfxSink() { }

        public void Play(MetaUiFeedbackSignal signal) { }
    }

    sealed class NullMetaUiHapticSink : IMetaUiHapticSink
    {
        public static readonly NullMetaUiHapticSink Instance =
            new NullMetaUiHapticSink();

        NullMetaUiHapticSink() { }

        public void Pulse(MetaUiFeedbackSignal signal) { }
    }

    /// <summary>
    /// Provider-neutral output boundary for accepted meta UI actions and major
    /// presentation reveals. Start-phase filtering ensures one physical cue per
    /// logical action even though motion traces also publish completion states.
    /// </summary>
    public sealed class MetaUiFeedbackService : MonoBehaviour
    {
        public static MetaUiFeedbackService Instance { get; private set; }

        IMetaUiSfxSink sfxSink = NullMetaUiSfxSink.Instance;
        IMetaUiHapticSink hapticSink = NullMetaUiHapticSink.Instance;
        bool warnedSfxFailure;
        bool warnedHapticFailure;
        bool warnedObserverFailure;

        public int SignalCount { get; private set; }
        public bool HasLastSignal { get; private set; }
        public MetaUiFeedbackSignal LastSignal { get; private set; }

        public event Action<MetaUiFeedbackSignal> SignalPublished;

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
            IMetaUiSfxSink newSfxSink,
            IMetaUiHapticSink newHapticSink
        )
        {
            sfxSink = newSfxSink ?? NullMetaUiSfxSink.Instance;
            hapticSink = newHapticSink ?? NullMetaUiHapticSink.Instance;
            warnedSfxFailure = false;
            warnedHapticFailure = false;
            warnedObserverFailure = false;
        }

        public void ResetSinks()
        {
            sfxSink = NullMetaUiSfxSink.Instance;
            hapticSink = NullMetaUiHapticSink.Instance;
            warnedSfxFailure = false;
            warnedHapticFailure = false;
            warnedObserverFailure = false;
        }

        public static void Publish(
            MetaUiMotionTrace trace,
            MetaUiControlRole role
        )
        {
            MetaUiFeedbackService service = Instance;
            if (service == null ||
                !TryCreateSignal(trace, role, out MetaUiFeedbackSignal signal,
                    out bool requestHaptic))
            {
                return;
            }

            service.Emit(signal, requestHaptic);
        }

        public static bool TryCreateSignal(
            MetaUiMotionTrace trace,
            MetaUiControlRole role,
            out MetaUiFeedbackSignal signal,
            out bool requestHaptic
        )
        {
            signal = default;
            requestHaptic = false;

            if (trace.Cue == MetaUiMotionCue.ButtonActivated &&
                trace.Phase == MetaUiTransitionPhase.Entering)
            {
                if (role == MetaUiControlRole.Gameplay)
                    return false;

                requestHaptic =
                    role == MetaUiControlRole.Primary ||
                    role == MetaUiControlRole.Reward ||
                    role == MetaUiControlRole.OneShotReward ||
                    role == MetaUiControlRole.MapNode;
                signal = new MetaUiFeedbackSignal(
                    trace.Cue,
                    role,
                    trace.Preset,
                    trace.Target
                );
                return true;
            }

            bool starts = trace.Phase == MetaUiTransitionPhase.Entering;
            bool exits = trace.Phase == MetaUiTransitionPhase.Exiting;
            if (!starts && !exits)
                return false;

            switch (trace.Preset)
            {
                case MetaUiMotionPreset.ModalStandard:
                    if (
                        (trace.Cue == MetaUiMotionCue.ModalEnter && !starts) ||
                        (trace.Cue == MetaUiMotionCue.ModalExit && !exits) ||
                        (trace.Cue != MetaUiMotionCue.ModalEnter &&
                            trace.Cue != MetaUiMotionCue.ModalExit)
                    )
                        return false;
                    requestHaptic = starts;
                    break;

                case MetaUiMotionPreset.ModalReward:
                    if (
                        (trace.Cue == MetaUiMotionCue.RewardReveal && !starts) ||
                        (trace.Cue == MetaUiMotionCue.ModalExit && !exits) ||
                        (trace.Cue != MetaUiMotionCue.RewardReveal &&
                            trace.Cue != MetaUiMotionCue.ModalExit)
                    )
                        return false;
                    requestHaptic = starts;
                    break;

                case MetaUiMotionPreset.OutcomeVictory:
                    if (!starts || trace.Cue != MetaUiMotionCue.VictoryReveal)
                        return false;
                    requestHaptic = true;
                    break;

                case MetaUiMotionPreset.OutcomeFailure:
                    if (!starts || trace.Cue != MetaUiMotionCue.FailureReveal)
                        return false;
                    requestHaptic = true;
                    break;

                case MetaUiMotionPreset.OutcomeReward:
                    if (!starts || trace.Cue != MetaUiMotionCue.RewardReveal)
                        return false;
                    requestHaptic = true;
                    break;

                case MetaUiMotionPreset.ChapterComplete:
                    if (!starts ||
                        trace.Cue != MetaUiMotionCue.ChapterCompleteReveal)
                        return false;
                    requestHaptic = true;
                    break;

                default:
                    return false;
            }

            signal = new MetaUiFeedbackSignal(
                trace.Cue,
                role,
                trace.Preset,
                trace.Target
            );
            return true;
        }

        void Emit(MetaUiFeedbackSignal signal, bool requestHaptic)
        {
            LastSignal = signal;
            HasLastSignal = true;
            SignalCount++;
            SafePublish(signal);

            if (IsSettingEnabled(save => save.sfx))
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
                            "Meta UI SFX sink failed and was isolated: " +
                            exception.Message
                        );
                    }
                }
            }

            if (requestHaptic && IsSettingEnabled(save => save.vibration))
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
                            "Meta UI haptic sink failed and was isolated: " +
                            exception.Message
                        );
                    }
                }
            }
        }

        void SafePublish(MetaUiFeedbackSignal signal)
        {
            Action<MetaUiFeedbackSignal> handlers = SignalPublished;
            if (handlers == null) return;

            foreach (Delegate candidate in handlers.GetInvocationList())
            {
                try
                {
                    ((Action<MetaUiFeedbackSignal>)candidate)(signal);
                }
                catch (Exception exception)
                {
                    if (!warnedObserverFailure)
                    {
                        warnedObserverFailure = true;
                        Debug.LogWarning(
                            "Meta UI feedback observer failed and was isolated: " +
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
