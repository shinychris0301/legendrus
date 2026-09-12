using System;
using System.Collections;
using LegendsOfRus.OptimizationCore;
using UnityEngine;
using UnityEngine.Profiling;

namespace LegendsOfRus
{
    /// <summary>
    /// Bounded local-only runtime optimization probe and resource lifetime
    /// coordinator. It never writes evidence to disk and never sends data.
    /// Low-memory cleanup/signals are active in all builds; periodic sampling
    /// and budget-comparison logs are limited to Editor/development builds.
    /// </summary>
    public sealed class RuntimeOptimizationService : MonoBehaviour
    {
        public static RuntimeOptimizationService Instance { get; private set; }

        readonly BoundedOptimizationEvidenceBuffer evidence =
            new BoundedOptimizationEvidenceBuffer();
        readonly LoadingStateTracker loadingState = new LoadingStateTracker();

        double bootStartedAt = -1.0;
        double screenRouteStartedAt = -1.0;
        double nextMemorySampleAt;
        double lastCleanupCompletedAt = double.NegativeInfinity;
        int retiredScreensPendingCleanup;
        int lowMemorySignalCount;
        int cleanupCount;
        bool cleanupRunning;
        bool cleanupRequested;
        bool forceCleanupRequested;
        MemoryBudgetBand lastLoggedBand = MemoryBudgetBand.Unknown;

        public int EvidenceCount { get { return evidence.Count; } }
        public int LowMemorySignalCount { get { return lowMemorySignalCount; } }
        public int CleanupCount { get { return cleanupCount; } }
        public bool CleanupRunning { get { return cleanupRunning; } }
        public RuntimeLoadingPhase LoadingPhase { get { return loadingState.Phase; } }
        public double LoadingProgress01 { get { return loadingState.Progress01; } }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            Application.lowMemory += HandleLowMemory;
            nextMemorySampleAt = Time.realtimeSinceStartupAsDouble;
        }

        void OnDestroy()
        {
            if (Instance != this)
                return;

            Application.lowMemory -= HandleLowMemory;
            Instance = null;
        }

        void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            double now = Time.realtimeSinceStartupAsDouble;
            if (now >= nextMemorySampleAt)
            {
                nextMemorySampleAt = now +
                    OptimizationPolicy.MemorySampleIntervalSeconds;
                CaptureMemorySample(true);
            }
#endif
        }

        public void BeginBootstrap()
        {
            if (bootStartedAt >= 0.0)
                return;

            bootStartedAt = Time.realtimeSinceStartupAsDouble;
            loadingState.TryAdvance(RuntimeLoadingPhase.Bootstrap, 0.15);
        }

        public void MarkLocalStateReady()
        {
            loadingState.TryAdvance(RuntimeLoadingPhase.LocalState, 0.55);
        }

        public void MarkUiConstructionReady()
        {
            loadingState.TryAdvance(RuntimeLoadingPhase.UiConstruction, 0.85);
        }

        public void CompleteBootstrap()
        {
            if (bootStartedAt < 0.0)
                return;

            loadingState.TryAdvance(RuntimeLoadingPhase.Ready, 1.0);
            double milliseconds =
                (Time.realtimeSinceStartupAsDouble - bootStartedAt) * 1000.0;
            evidence.AddLoad(OptimizationLoadKind.Bootstrap, milliseconds);
            bootStartedAt = -1.0;
        }

        public void MarkBootstrapFailed()
        {
            loadingState.TryAdvance(RuntimeLoadingPhase.Failed, 1.0);
            bootStartedAt = -1.0;
        }

        public void BeginScreenRoute()
        {
            screenRouteStartedAt = Time.realtimeSinceStartupAsDouble;
        }

        public void CompleteScreenRoute(bool cleanupSafe)
        {
            if (screenRouteStartedAt >= 0.0)
            {
                double milliseconds =
                    (Time.realtimeSinceStartupAsDouble - screenRouteStartedAt) *
                    1000.0;
                evidence.AddLoad(
                    OptimizationLoadKind.ScreenRoute,
                    milliseconds
                );
                screenRouteStartedAt = -1.0;
            }

            if (cleanupSafe &&
                retiredScreensPendingCleanup >=
                    OptimizationPolicy.RetiredScreensBeforeCleanup)
            {
                RequestCleanup(false);
            }
        }

        public void NotifyScreenRetired()
        {
            if (retiredScreensPendingCleanup < int.MaxValue)
                retiredScreensPendingCleanup++;
        }

        public OptimizationEvidenceSample[] SnapshotEvidence()
        {
            return evidence.Snapshot();
        }

        public string EvidenceJson()
        {
            return evidence.ToDeterministicJson();
        }

        public void RequestDevelopmentCleanup()
        {
            RequestCleanup(true);
        }

        void HandleLowMemory()
        {
            if (lowMemorySignalCount < int.MaxValue)
                lowMemorySignalCount++;

            CaptureMemorySample(false, true);
            RequestCleanup(true);
        }

        void RequestCleanup(bool force)
        {
            if (force)
                forceCleanupRequested = true;
            cleanupRequested = true;

            if (!cleanupRunning)
                StartCoroutine(CleanupWhenSafe());
        }

        IEnumerator CleanupWhenSafe()
        {
            cleanupRunning = true;
            try
            {
                while (cleanupRequested)
                {
                    bool force = forceCleanupRequested;
                    cleanupRequested = false;
                    forceCleanupRequested = false;

                    double now = Time.realtimeSinceStartupAsDouble;
                    double sinceLast = now - lastCleanupCompletedAt;
                    if (!force &&
                        sinceLast < OptimizationPolicy.MinimumCleanupIntervalSeconds)
                    {
                        continue;
                    }

                    // Destroy is deferred until end-of-frame. Waiting one frame
                    // ensures retired screen roots no longer keep Resources
                    // sprites reachable before Unity scans unused assets.
                    yield return null;

                    double startedAt = Time.realtimeSinceStartupAsDouble;
                    AsyncOperation operation = Resources.UnloadUnusedAssets();
                    if (operation != null)
                        yield return operation;

                    double completedAt = Time.realtimeSinceStartupAsDouble;
                    evidence.AddLoad(
                        OptimizationLoadKind.ResourceCleanup,
                        (completedAt - startedAt) * 1000.0
                    );
                    cleanupCount++;
                    retiredScreensPendingCleanup = 0;
                    lastCleanupCompletedAt = completedAt;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    CaptureMemorySample(true);
#endif
                }
            }
            finally
            {
                cleanupRunning = false;
            }
        }

        void CaptureMemorySample(bool logBandChanges, bool lowMemory = false)
        {
            long allocated = Profiler.GetTotalAllocatedMemoryLong();
            long reserved = Profiler.GetTotalReservedMemoryLong();
            long managed = GC.GetTotalMemory(false);

            OptimizationEvidenceSample sample = lowMemory
                ? evidence.AddLowMemory(allocated, reserved, managed)
                : evidence.AddMemory(allocated, reserved, managed);

            // An actual low-memory signal is NOT an Editor budget comparison.
            // Keep it visible in every build; HandleLowMemory still requests cleanup.
            if (lowMemory)
            {
                Debug.LogWarning(
                    "Legends of Rus low-memory signal (" +
                    (Application.isEditor ? "Editor" : "Player") + "): allocated=" +
                    ToMiB(allocated) + " MiB, reserved=" + ToMiB(reserved) +
                    " MiB, managed=" + ToMiB(managed) +
                    " MiB. Bounded resource cleanup requested.", this);
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (logBandChanges && sample.MemoryBand != lastLoggedBand)
            {
                lastLoggedBand = sample.MemoryBand;
                if (sample.MemoryBand == MemoryBudgetBand.Warning ||
                    sample.MemoryBand == MemoryBudgetBand.Critical)
                {
                    string metrics = "allocated=" + ToMiB(allocated) +
                        " MiB, reserved=" + ToMiB(reserved) + " MiB, managed=" +
                        ToMiB(managed) + " MiB. Exceeded guardrails: " +
                        ExceededGuardrails(allocated, reserved, managed) + ".";
#if UNITY_EDITOR
                    string message = "Legends of Rus EDITOR memory observation: " + metrics +
                        " Comparison band=" + sample.MemoryBand +
                        ". Editor measurements are not an Android Player budget verdict." +
                        " Profile a Development Build on the target device. Evidence remains local and bounded.";
                    // Retain the observation and data; do not present an ordinary
                    // Editor threshold crossing as a device failure. Critical
                    // comparisons remain yellow even in the Editor.
                    if (sample.MemoryBand == MemoryBudgetBand.Critical)
                        Debug.LogWarning(message, this);
                    else
                        Debug.Log(message, this);
#else
                    Debug.LogWarning(
                        "Legends of Rus PLAYER memory budget " + sample.MemoryBand +
                        ": " + metrics + " Evidence remains local and bounded.", this);
#endif
                }
            }
#endif
        }

        static string ExceededGuardrails(long allocated, long reserved, long managed)
        {
            string result = "";
            AppendExceeded(ref result, "allocated", allocated,
                OptimizationPolicy.AllocatedWarningBytes, OptimizationPolicy.AllocatedCriticalBytes);
            AppendExceeded(ref result, "reserved", reserved,
                OptimizationPolicy.ReservedWarningBytes, OptimizationPolicy.ReservedCriticalBytes);
            AppendExceeded(ref result, "managed", managed,
                OptimizationPolicy.ManagedWarningBytes, OptimizationPolicy.ManagedCriticalBytes);
            return string.IsNullOrEmpty(result) ? "none" : result;
        }

        static void AppendExceeded(ref string text, string metric, long actual, long warning, long critical)
        {
            if (actual < warning) return;
            long threshold = actual >= critical ? critical : warning;
            if (!string.IsNullOrEmpty(text)) text += "; ";
            text += metric + " >= " + ToMiB(threshold) + " MiB (" +
                (actual >= critical ? "critical" : "warning") + ")";
        }

        static long ToMiB(long bytes)
        {
            if (bytes <= 0L)
                return 0L;
            return bytes / OptimizationPolicy.Mebibyte;
        }
    }
}
