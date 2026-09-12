using System;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace LegendsOfRus
{
    /// <summary>
    /// Semantic haptic intents ordered from least to most important. The
    /// platform bridge, rather than gameplay or UI code, chooses the device
    /// effect used to express an intent.
    /// </summary>
    public enum HapticFeedbackIntent
    {
        None = 0,
        Selection = 1,
        Confirm = 2,
        Emphasis = 3,
        Reject = 4
    }

    /// <summary>
    /// Injectable monotonic clock used by the deterministic rate limiter.
    /// </summary>
    public interface IHapticClock
    {
        double RealtimeSeconds { get; }
    }

    /// <summary>
    /// Replaceable platform seam. Implementations own any native work they
    /// schedule and must invalidate work that has not run when suspended.
    /// </summary>
    public interface IPlatformHapticBridge : IDisposable
    {
        bool IsSupported { get; }
        bool TryPerform(HapticFeedbackIntent intent);
        void Suspend();
    }

    /// <summary>
    /// Shared, provider-neutral haptics for deterministic board and meta-UI
    /// feedback. A stronger intent may replace a weaker intent during the
    /// 60 ms guard window; equal or weaker requests are suppressed.
    /// </summary>
    public sealed class PlatformHapticProvider :
        ICoreLoopHapticSink,
        IMetaUiHapticSink,
        IDisposable
    {
        public const double MinimumIntervalSeconds = 0.06d;

        readonly IHapticClock clock;
        readonly IPlatformHapticBridge bridge;

        double lastAcceptedTime = double.NegativeInfinity;
        HapticFeedbackIntent lastAcceptedIntent = HapticFeedbackIntent.None;
        bool suspended;
        bool disposed;

        public PlatformHapticProvider(
            IHapticClock hapticClock,
            IPlatformHapticBridge platformBridge
        )
        {
            clock = hapticClock ?? UnityRealtimeHapticClock.Instance;
            bridge = platformBridge ?? NullPlatformHapticBridge.Instance;
        }

        public bool IsSupported => !disposed && !suspended && bridge.IsSupported;
        public bool IsDisposed => disposed;
        public bool IsSuspended => suspended;
        public int AcceptedPulseCount { get; private set; }
        public int ThrottledPulseCount { get; private set; }
        public HapticFeedbackIntent LastAcceptedIntent => lastAcceptedIntent;

        public static PlatformHapticProvider CreateDefault()
        {
            return new PlatformHapticProvider(
                UnityRealtimeHapticClock.Instance,
                CreateDefaultBridge()
            );
        }

        public static IPlatformHapticBridge CreateDefaultBridge()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return new AndroidViewHapticBridge();
#else
            return NullPlatformHapticBridge.Instance;
#endif
        }

        public void Pulse(CoreLoopFeedbackSignal signal)
        {
            TryPulse(ResolveIntent(signal));
        }

        public void Pulse(MetaUiFeedbackSignal signal)
        {
            TryPulse(ResolveIntent(signal));
        }

        public bool TryPulse(HapticFeedbackIntent intent)
        {
            if (
                disposed ||
                suspended ||
                intent == HapticFeedbackIntent.None ||
                !bridge.IsSupported
            )
            {
                return false;
            }

            double now = clock.RealtimeSeconds;
            if (double.IsNaN(now) || double.IsInfinity(now))
                now = lastAcceptedTime == double.NegativeInfinity
                    ? 0d
                    : lastAcceptedTime;

            // A reset or replacement test clock begins a new deterministic
            // window instead of suppressing feedback indefinitely.
            if (now < lastAcceptedTime)
            {
                lastAcceptedTime = double.NegativeInfinity;
                lastAcceptedIntent = HapticFeedbackIntent.None;
            }

            double elapsed = now - lastAcceptedTime;
            if (
                elapsed < MinimumIntervalSeconds &&
                Priority(intent) <= Priority(lastAcceptedIntent)
            )
            {
                ThrottledPulseCount++;
                return false;
            }

            if (!bridge.TryPerform(intent))
                return false;

            lastAcceptedTime = now;
            lastAcceptedIntent = intent;
            AcceptedPulseCount++;
            return true;
        }

        /// <summary>
        /// Invalidates queued native work, enters a suspended state, and starts
        /// a fresh throttle window. Resume must be called before another pulse
        /// can reach the native bridge.
        /// </summary>
        public void Suspend()
        {
            if (disposed) return;

            suspended = true;
            lastAcceptedTime = double.NegativeInfinity;
            lastAcceptedIntent = HapticFeedbackIntent.None;
            try
            {
                bridge.Suspend();
            }
            catch (Exception)
            {
                // Application pause and teardown must remain non-failing even
                // when a replaceable native bridge is faulty.
            }
        }

        public void Resume()
        {
            if (disposed) return;

            suspended = false;
            lastAcceptedTime = double.NegativeInfinity;
            lastAcceptedIntent = HapticFeedbackIntent.None;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            suspended = true;

            try
            {
                bridge.Suspend();
            }
            catch (Exception)
            {
                // Continue to dispose the bridge after a failed suspension.
            }

            try
            {
                bridge.Dispose();
            }
            catch (Exception)
            {
                // Shutdown must not strand or fault the persistent root.
            }

            lastAcceptedTime = double.NegativeInfinity;
            lastAcceptedIntent = HapticFeedbackIntent.None;
        }

        public static HapticFeedbackIntent ResolveIntent(
            CoreLoopFeedbackSignal signal
        )
        {
            if (signal == null) return HapticFeedbackIntent.None;

            switch (signal.Cue)
            {
                case CoreLoopFeedbackCue.SwapRejected:
                case CoreLoopFeedbackCue.InvalidSwap:
                case CoreLoopFeedbackCue.CommandRecovered:
                case CoreLoopFeedbackCue.BoosterRejected:
                    return HapticFeedbackIntent.Reject;

                case CoreLoopFeedbackCue.MatchResolved:
                case CoreLoopFeedbackCue.CascadeResolved:
                    return HapticFeedbackIntent.Confirm;

                case CoreLoopFeedbackCue.SpecialCreated:
                case CoreLoopFeedbackCue.SpecialActivated:
                case CoreLoopFeedbackCue.Reshuffled:
                case CoreLoopFeedbackCue.BoosterActivated:
                    return HapticFeedbackIntent.Emphasis;

                default:
                    return HapticFeedbackIntent.None;
            }
        }

        public static HapticFeedbackIntent ResolveIntent(
            MetaUiFeedbackSignal signal
        )
        {
            switch (signal.Cue)
            {
                case MetaUiMotionCue.ButtonActivated:
                    switch (signal.Role)
                    {
                        case MetaUiControlRole.MapNode:
                            // MetaUiFeedbackService emits the accepted map-node
                            // action once; its visual MapNodePulse trace is not
                            // forwarded as a second provider signal.
                            return HapticFeedbackIntent.Confirm;
                        case MetaUiControlRole.Gameplay:
                            // Gameplay controls use the core-loop boundary.
                            return HapticFeedbackIntent.None;
                        case MetaUiControlRole.Primary:
                            return HapticFeedbackIntent.Confirm;
                        case MetaUiControlRole.Reward:
                        case MetaUiControlRole.OneShotReward:
                            return HapticFeedbackIntent.Emphasis;
                        default:
                            return HapticFeedbackIntent.Selection;
                    }

                case MetaUiMotionCue.MapNodePulse:
                    return HapticFeedbackIntent.Confirm;

                case MetaUiMotionCue.VictoryReveal:
                case MetaUiMotionCue.RewardReveal:
                case MetaUiMotionCue.ChapterCompleteReveal:
                    return HapticFeedbackIntent.Emphasis;

                case MetaUiMotionCue.FailureReveal:
                    return HapticFeedbackIntent.Reject;

                case MetaUiMotionCue.ModalEnter:
                    return HapticFeedbackIntent.Selection;

                default:
                    return HapticFeedbackIntent.None;
            }
        }

        static int Priority(HapticFeedbackIntent intent)
        {
            switch (intent)
            {
                case HapticFeedbackIntent.Selection:
                    return 1;
                case HapticFeedbackIntent.Confirm:
                    return 2;
                case HapticFeedbackIntent.Emphasis:
                    return 3;
                case HapticFeedbackIntent.Reject:
                    return 4;
                default:
                    return 0;
            }
        }
    }

    sealed class UnityRealtimeHapticClock : IHapticClock
    {
        public static readonly UnityRealtimeHapticClock Instance =
            new UnityRealtimeHapticClock();

        UnityRealtimeHapticClock() { }

        public double RealtimeSeconds => Time.realtimeSinceStartupAsDouble;
    }

    sealed class NullPlatformHapticBridge : IPlatformHapticBridge
    {
        public static readonly NullPlatformHapticBridge Instance =
            new NullPlatformHapticBridge();

        NullPlatformHapticBridge() { }

        public bool IsSupported => false;
        public bool TryPerform(HapticFeedbackIntent intent) => false;
        public void Suspend() { }
        public void Dispose() { }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// Android's action-oriented View API is OEM-tuned, respects system touch
    /// feedback settings, and does not require a vibration permission.
    /// </summary>
    sealed class AndroidViewHapticBridge : IPlatformHapticBridge
    {
        const int VirtualKeyEffect = 1;
        const int ConfirmEffect = 16;
        const int RejectEffect = 17;

        readonly object sync = new object();

        HapticFeedbackIntent pendingIntent;
        int sdkLevel = -1;
        int generation;
        bool dispatchQueued;
        bool disposed;
        bool warnedFailure;

        public AndroidViewHapticBridge() { }

        public bool IsSupported
        {
            get
            {
                lock (sync)
                    return !disposed;
            }
        }

        public bool TryPerform(HapticFeedbackIntent intent)
        {
            if (intent == HapticFeedbackIntent.None) return false;

            bool schedule = false;
            int scheduledGeneration = 0;
            lock (sync)
            {
                if (disposed) return false;

                if (
                    pendingIntent == HapticFeedbackIntent.None ||
                    IntentPriority(intent) > IntentPriority(pendingIntent)
                )
                {
                    pendingIntent = intent;
                }

                if (!dispatchQueued)
                {
                    dispatchQueued = true;
                    scheduledGeneration = generation;
                    schedule = true;
                }
            }

            if (!schedule) return true;

            try
            {
                AndroidApplication.InvokeOnUIThread(
                    () => FlushPendingOnUiThread(scheduledGeneration)
                );
                return true;
            }
            catch (Exception exception)
            {
                lock (sync)
                {
                    if (scheduledGeneration == generation)
                    {
                        dispatchQueued = false;
                        pendingIntent = HapticFeedbackIntent.None;
                    }
                }
                WarnOnce(exception);
                return false;
            }
        }

        public void Suspend()
        {
            lock (sync)
            {
                generation++;
                pendingIntent = HapticFeedbackIntent.None;
                dispatchQueued = false;
                // A callback already handed to Android carries the previous
                // generation and exits without consuming post-resume work.
            }
        }

        public void Dispose()
        {
            lock (sync)
            {
                if (disposed) return;
                disposed = true;
                generation++;
                pendingIntent = HapticFeedbackIntent.None;
                dispatchQueued = false;
            }
        }

        void FlushPendingOnUiThread(int scheduledGeneration)
        {
            lock (sync)
            {
                if (scheduledGeneration != generation)
                    return;
                dispatchQueued = false;
                if (disposed || pendingIntent == HapticFeedbackIntent.None)
                    return;

                HapticFeedbackIntent intent = pendingIntent;
                pendingIntent = HapticFeedbackIntent.None;

                try
                {
                    // The frame-layout wrapper is ours and is disposed after
                    // each short call. The Unity player bridge itself is owned
                    // by Unity and must not be disposed.
                    AndroidJavaObject unityPlayer = AndroidApplication.unityPlayer;
                    if (unityPlayer == null) return;
                    if (sdkLevel < 0)
                    {
                        using (
                            var version =
                                new AndroidJavaClass("android.os.Build$VERSION")
                        )
                        {
                            sdkLevel = version.GetStatic<int>("SDK_INT");
                        }
                    }
                    using (
                        AndroidJavaObject frameLayout =
                            unityPlayer.Call<AndroidJavaObject>("getFrameLayout")
                    )
                    {
                        if (frameLayout == null) return;
                        frameLayout.Call<bool>(
                            "performHapticFeedback",
                            ResolveAndroidEffect(intent)
                        );
                    }
                }
                catch (Exception exception)
                {
                    WarnOnce(exception);
                }
            }
        }

        int ResolveAndroidEffect(HapticFeedbackIntent intent)
        {
            if (sdkLevel < 30) return VirtualKeyEffect;
            return intent == HapticFeedbackIntent.Reject
                ? RejectEffect
                : ConfirmEffect;
        }

        static int IntentPriority(HapticFeedbackIntent intent)
        {
            return (int)intent;
        }

        void WarnOnce(Exception exception)
        {
            lock (sync)
            {
                if (warnedFailure) return;
                warnedFailure = true;
            }
            Debug.LogWarning(
                "Android view haptic feedback is unavailable and was isolated: " +
                exception.Message
            );
        }
    }
#endif
}
