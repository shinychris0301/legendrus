using System;
using UnityEngine;

namespace LegendsOfRus
{
    public enum MetaUiMotionCue
    {
        None = 0,
        ScreenEnter = 1,
        ScreenExit = 2,
        ModalEnter = 3,
        ModalExit = 4,
        ButtonPress = 5,
        ButtonRelease = 6,
        ButtonActivated = 7,
        MapNodePulse = 8,
        VictoryReveal = 9,
        FailureReveal = 10,
        RewardReveal = 11,
        ChapterCompleteReveal = 12
    }

    public enum MetaUiMotionPreset
    {
        ScreenStandard = 0,
        ScreenVictory = 1,
        ScreenFailure = 2,
        ScreenChapterComplete = 3,
        ModalStandard = 4,
        ModalReward = 5,
        OutcomeVictory = 6,
        OutcomeFailure = 7,
        OutcomeReward = 8,
        ChapterComplete = 9
    }

    public enum MetaUiControlRole
    {
        Standard = 0,
        Primary = 1,
        Back = 2,
        Reward = 3,
        MapNode = 4,
        Gameplay = 5,
        OneShotReward = 6
    }

    public enum MetaUiTransitionPhase
    {
        Idle = 0,
        Entering = 1,
        Entered = 2,
        Exiting = 3,
        Exited = 4,
        Cancelled = 5
    }

    public readonly struct MetaUiTransitionStyle
    {
        public float DelaySeconds { get; }
        public float EnterDurationSeconds { get; }
        public float ExitDurationSeconds { get; }
        public float StartAlpha { get; }
        public float StartScale { get; }
        public Vector2 StartOffset { get; }
        public float ExitScale { get; }
        public Vector2 ExitOffset { get; }
        public bool OvershootScale { get; }
        public bool EnablesInteraction { get; }

        public MetaUiTransitionStyle(
            float delaySeconds,
            float enterDurationSeconds,
            float exitDurationSeconds,
            float startAlpha,
            float startScale,
            Vector2 startOffset,
            float exitScale,
            Vector2 exitOffset,
            bool overshootScale,
            bool enablesInteraction
        )
        {
            DelaySeconds = delaySeconds;
            EnterDurationSeconds = enterDurationSeconds;
            ExitDurationSeconds = exitDurationSeconds;
            StartAlpha = startAlpha;
            StartScale = startScale;
            StartOffset = startOffset;
            ExitScale = exitScale;
            ExitOffset = exitOffset;
            OvershootScale = overshootScale;
            EnablesInteraction = enablesInteraction;
        }
    }

    public readonly struct MetaUiPressStyle
    {
        public float InvokeDelaySeconds { get; }
        public float ReleaseDurationSeconds { get; }
        public float PressedScale { get; }
        public float ActivatedScale { get; }
        public Color PressedColor { get; }
        public Color ActivatedColor { get; }
        public bool ShowsHighlight { get; }

        public MetaUiPressStyle(
            float invokeDelaySeconds,
            float releaseDurationSeconds,
            float pressedScale,
            float activatedScale,
            Color pressedColor,
            Color activatedColor,
            bool showsHighlight
        )
        {
            InvokeDelaySeconds = invokeDelaySeconds;
            ReleaseDurationSeconds = releaseDurationSeconds;
            PressedScale = pressedScale;
            ActivatedScale = activatedScale;
            PressedColor = pressedColor;
            ActivatedColor = activatedColor;
            ShowsHighlight = showsHighlight;
        }
    }

    public readonly struct MetaUiMotionTrace
    {
        public int Sequence { get; }
        public MetaUiMotionCue Cue { get; }
        public MetaUiTransitionPhase Phase { get; }
        public MetaUiMotionPreset Preset { get; }
        public string Target { get; }

        public MetaUiMotionTrace(
            int sequence,
            MetaUiMotionCue cue,
            MetaUiTransitionPhase phase,
            MetaUiMotionPreset preset,
            string target
        )
        {
            Sequence = sequence;
            Cue = cue;
            Phase = phase;
            Preset = preset;
            Target = target ?? string.Empty;
        }

        public string StableKey
        {
            get
            {
                return Sequence + ":" + Cue + ":" + Phase + ":" + Preset + ":" + Target;
            }
        }
    }

    public readonly struct MetaUiMotionDiagnostics
    {
        public int RegisteredCount { get; }
        public int PeakRegisteredCount { get; }
        public int DroppedRegistrationCount { get; }
        public int TickCount { get; }

        public MetaUiMotionDiagnostics(
            int registeredCount,
            int peakRegisteredCount,
            int droppedRegistrationCount,
            int tickCount
        )
        {
            RegisteredCount = registeredCount;
            PeakRegisteredCount = peakRegisteredCount;
            DroppedRegistrationCount = droppedRegistrationCount;
            TickCount = tickCount;
        }
    }
}
