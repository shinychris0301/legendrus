using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Unity.Profiling;
using UnityEngine.InputSystem;
#endif

namespace LegendsOfRus
{
    public enum RuntimePerformanceScenario
    {
        GameplayIdle = 0,
        GameplayActive,
        MetaIdle,
        MetaTransition
    }

    public enum RuntimePerformanceVerdict
    {
        Incomplete = 0,
        Pass,
        Fail
    }

    /// <summary>
    /// Fixed verification policy for the v1.14 60 fps fidelity gate. The
    /// sampler remains observational: none of these values are applied to
    /// Application, QualitySettings, Time, or gameplay state.
    /// </summary>
    public static class RuntimePerformanceProfile
    {
        public const int TargetFramesPerSecond = 60;
        public const double FrameBudgetMilliseconds = 1000.0 / TargetFramesPerSecond;
        public const double P95BudgetMilliseconds = 18.0;
        public const double P99BudgetMilliseconds = 25.0;
        public const double SevereFrameMilliseconds = 1000.0 / 30.0;
        public const double MaximumSevereFrameRatio = 0.01;
        public const double MinimumGpuCoverageRatio = 0.90;

        public const int MinimumPassingSamples = 600;
        public const int DefaultWarmupFrames = 120;
        public const int DefaultSampleFrames = 1800;
        public const int MaximumWarmupFrames = 600;
        public const int MaximumSamples = 3600;

        public static int ClampWarmupFrames(int value)
        {
            if (value < 0) return 0;
            return value > MaximumWarmupFrames ? MaximumWarmupFrames : value;
        }

        public static int ClampSampleFrames(int value)
        {
            if (value < 1) return 1;
            return value > MaximumSamples ? MaximumSamples : value;
        }

        public static bool RequiresZeroManagedAllocation(
            RuntimePerformanceScenario scenario
        )
        {
            return
                scenario == RuntimePerformanceScenario.GameplayIdle ||
                scenario == RuntimePerformanceScenario.MetaIdle;
        }
    }

    public readonly struct RuntimePerformanceMetricSummary
    {
        public int Count { get; }
        public double Minimum { get; }
        public double Mean { get; }
        public double P50 { get; }
        public double P95 { get; }
        public double P99 { get; }
        public double Maximum { get; }

        public RuntimePerformanceMetricSummary(
            int count,
            double minimum,
            double mean,
            double p50,
            double p95,
            double p99,
            double maximum
        )
        {
            Count = count;
            Minimum = minimum;
            Mean = mean;
            P50 = p50;
            P95 = p95;
            P99 = p99;
            Maximum = maximum;
        }
    }

    public readonly struct RuntimePerformanceAllocationSummary
    {
        public int Count { get; }
        public long TotalBytes { get; }
        public long MaximumBytes { get; }
        public int AllocatingFrameCount { get; }
        public double MeanBytes { get; }

        public RuntimePerformanceAllocationSummary(
            int count,
            long totalBytes,
            long maximumBytes,
            int allocatingFrameCount
        )
        {
            Count = count;
            TotalBytes = totalBytes;
            MaximumBytes = maximumBytes;
            AllocatingFrameCount = allocatingFrameCount;
            MeanBytes = count > 0 ? totalBytes / (double)count : 0.0;
        }
    }

    /// <summary>
    /// Immutable summary created only after sampling has stopped. JSON output
    /// deliberately excludes timestamps, random identifiers, and locale-
    /// dependent formatting so equal samples produce equal text.
    /// </summary>
    public readonly struct RuntimePerformanceReport
    {
        public const int SchemaVersion = 1;

        public RuntimePerformanceScenario Scenario { get; }
        public RuntimePerformanceVerdict Verdict { get; }
        public int ConfiguredTargetFrameRate { get; }
        public int RequestedWarmupFrames { get; }
        public int AcceptedWarmupFrames { get; }
        public int RequestedSampleFrames { get; }
        public int CapturedSampleFrames { get; }
        public int RejectedZeroTimestampCount { get; }
        public int RejectedDuplicateTimestampCount { get; }
        public int InvalidCpuSampleCount { get; }
        public bool Interrupted { get; }
        public bool FrameTimingSupported { get; }
        public bool GcCounterSupported { get; }
        public bool GpuTimingSupported { get; }
        public double GpuCoverageRatio { get; }
        public int OverBudgetFrameCount { get; }
        public int SevereFrameCount { get; }
        public double OverBudgetFrameRatio { get; }
        public double SevereFrameRatio { get; }
        public RuntimePerformanceMetricSummary CpuTotalMilliseconds { get; }
        public RuntimePerformanceMetricSummary CpuMainMilliseconds { get; }
        public RuntimePerformanceMetricSummary CpuRenderMilliseconds { get; }
        public RuntimePerformanceMetricSummary PresentWaitMilliseconds { get; }
        public RuntimePerformanceMetricSummary GpuMilliseconds { get; }
        public RuntimePerformanceAllocationSummary ManagedAllocationBytes { get; }

        public RuntimePerformanceReport(
            RuntimePerformanceScenario scenario,
            RuntimePerformanceVerdict verdict,
            int configuredTargetFrameRate,
            int requestedWarmupFrames,
            int acceptedWarmupFrames,
            int requestedSampleFrames,
            int capturedSampleFrames,
            int rejectedZeroTimestampCount,
            int rejectedDuplicateTimestampCount,
            int invalidCpuSampleCount,
            bool interrupted,
            bool frameTimingSupported,
            bool gcCounterSupported,
            bool gpuTimingSupported,
            double gpuCoverageRatio,
            int overBudgetFrameCount,
            int severeFrameCount,
            double overBudgetFrameRatio,
            double severeFrameRatio,
            RuntimePerformanceMetricSummary cpuTotalMilliseconds,
            RuntimePerformanceMetricSummary cpuMainMilliseconds,
            RuntimePerformanceMetricSummary cpuRenderMilliseconds,
            RuntimePerformanceMetricSummary presentWaitMilliseconds,
            RuntimePerformanceMetricSummary gpuMilliseconds,
            RuntimePerformanceAllocationSummary managedAllocationBytes
        )
        {
            Scenario = scenario;
            Verdict = verdict;
            ConfiguredTargetFrameRate = configuredTargetFrameRate;
            RequestedWarmupFrames = requestedWarmupFrames;
            AcceptedWarmupFrames = acceptedWarmupFrames;
            RequestedSampleFrames = requestedSampleFrames;
            CapturedSampleFrames = capturedSampleFrames;
            RejectedZeroTimestampCount = rejectedZeroTimestampCount;
            RejectedDuplicateTimestampCount = rejectedDuplicateTimestampCount;
            InvalidCpuSampleCount = invalidCpuSampleCount;
            Interrupted = interrupted;
            FrameTimingSupported = frameTimingSupported;
            GcCounterSupported = gcCounterSupported;
            GpuTimingSupported = gpuTimingSupported;
            GpuCoverageRatio = gpuCoverageRatio;
            OverBudgetFrameCount = overBudgetFrameCount;
            SevereFrameCount = severeFrameCount;
            OverBudgetFrameRatio = overBudgetFrameRatio;
            SevereFrameRatio = severeFrameRatio;
            CpuTotalMilliseconds = cpuTotalMilliseconds;
            CpuMainMilliseconds = cpuMainMilliseconds;
            CpuRenderMilliseconds = cpuRenderMilliseconds;
            PresentWaitMilliseconds = presentWaitMilliseconds;
            GpuMilliseconds = gpuMilliseconds;
            ManagedAllocationBytes = managedAllocationBytes;
        }

        public string ToDeterministicJson()
        {
            var builder = new StringBuilder(2048);
            builder.Append("{\n");
            AppendInteger(builder, "schema_version", SchemaVersion, true);
            AppendString(builder, "scenario", ScenarioToken(Scenario), true);
            AppendString(builder, "verdict", VerdictToken(Verdict), true);
            AppendInteger(
                builder,
                "configured_target_frame_rate",
                ConfiguredTargetFrameRate,
                true
            );
            AppendInteger(
                builder,
                "requested_warmup_frames",
                RequestedWarmupFrames,
                true
            );
            AppendInteger(
                builder,
                "accepted_warmup_frames",
                AcceptedWarmupFrames,
                true
            );
            AppendInteger(
                builder,
                "requested_sample_frames",
                RequestedSampleFrames,
                true
            );
            AppendInteger(
                builder,
                "captured_sample_frames",
                CapturedSampleFrames,
                true
            );
            AppendInteger(
                builder,
                "rejected_zero_timestamps",
                RejectedZeroTimestampCount,
                true
            );
            AppendInteger(
                builder,
                "rejected_duplicate_timestamps",
                RejectedDuplicateTimestampCount,
                true
            );
            AppendInteger(
                builder,
                "invalid_cpu_samples",
                InvalidCpuSampleCount,
                true
            );
            AppendBoolean(builder, "interrupted", Interrupted, true);
            AppendBoolean(
                builder,
                "frame_timing_supported",
                FrameTimingSupported,
                true
            );
            AppendBoolean(
                builder,
                "gc_counter_supported",
                GcCounterSupported,
                true
            );
            AppendBoolean(
                builder,
                "gpu_timing_supported",
                GpuTimingSupported,
                true
            );
            AppendDouble(builder, "gpu_coverage_ratio", GpuCoverageRatio, true);
            AppendInteger(
                builder,
                "over_budget_frame_count",
                OverBudgetFrameCount,
                true
            );
            AppendInteger(builder, "severe_frame_count", SevereFrameCount, true);
            AppendDouble(
                builder,
                "over_budget_frame_ratio",
                OverBudgetFrameRatio,
                true
            );
            AppendDouble(builder, "severe_frame_ratio", SevereFrameRatio, true);
            AppendMetric(builder, "cpu_total_ms", CpuTotalMilliseconds, true);
            AppendMetric(builder, "cpu_main_ms", CpuMainMilliseconds, true);
            AppendMetric(builder, "cpu_render_ms", CpuRenderMilliseconds, true);
            AppendMetric(
                builder,
                "present_wait_ms",
                PresentWaitMilliseconds,
                true
            );
            AppendMetric(builder, "gpu_ms", GpuMilliseconds, true);
            AppendAllocation(
                builder,
                "managed_allocation_bytes",
                ManagedAllocationBytes,
                true
            );
            builder.Append("  \"gate\": {\n");
            AppendInteger(
                builder,
                "target_fps",
                RuntimePerformanceProfile.TargetFramesPerSecond,
                true,
                4
            );
            AppendDouble(
                builder,
                "frame_budget_ms",
                RuntimePerformanceProfile.FrameBudgetMilliseconds,
                true,
                4
            );
            AppendDouble(
                builder,
                "p95_budget_ms",
                RuntimePerformanceProfile.P95BudgetMilliseconds,
                true,
                4
            );
            AppendDouble(
                builder,
                "p99_budget_ms",
                RuntimePerformanceProfile.P99BudgetMilliseconds,
                true,
                4
            );
            AppendDouble(
                builder,
                "severe_frame_ms",
                RuntimePerformanceProfile.SevereFrameMilliseconds,
                true,
                4
            );
            AppendDouble(
                builder,
                "maximum_severe_frame_ratio",
                RuntimePerformanceProfile.MaximumSevereFrameRatio,
                true,
                4
            );
            AppendDouble(
                builder,
                "minimum_gpu_coverage_ratio",
                RuntimePerformanceProfile.MinimumGpuCoverageRatio,
                true,
                4
            );
            AppendInteger(
                builder,
                "minimum_samples",
                RuntimePerformanceProfile.MinimumPassingSamples,
                false,
                4
            );
            builder.Append("  }\n");
            builder.Append('}');
            return builder.ToString();
        }

        static void AppendMetric(
            StringBuilder builder,
            string name,
            RuntimePerformanceMetricSummary metric,
            bool comma
        )
        {
            builder.Append("  \"").Append(name).Append("\": {\n");
            AppendInteger(builder, "count", metric.Count, true, 4);
            AppendDouble(builder, "minimum", metric.Minimum, true, 4);
            AppendDouble(builder, "mean", metric.Mean, true, 4);
            AppendDouble(builder, "p50", metric.P50, true, 4);
            AppendDouble(builder, "p95", metric.P95, true, 4);
            AppendDouble(builder, "p99", metric.P99, true, 4);
            AppendDouble(builder, "maximum", metric.Maximum, false, 4);
            builder.Append("  }");
            builder.Append(comma ? ",\n" : "\n");
        }

        static void AppendAllocation(
            StringBuilder builder,
            string name,
            RuntimePerformanceAllocationSummary allocation,
            bool comma
        )
        {
            builder.Append("  \"").Append(name).Append("\": {\n");
            AppendInteger(builder, "count", allocation.Count, true, 4);
            AppendLong(builder, "total", allocation.TotalBytes, true, 4);
            AppendLong(builder, "maximum", allocation.MaximumBytes, true, 4);
            AppendInteger(
                builder,
                "allocating_frame_count",
                allocation.AllocatingFrameCount,
                true,
                4
            );
            AppendDouble(builder, "mean", allocation.MeanBytes, false, 4);
            builder.Append("  }");
            builder.Append(comma ? ",\n" : "\n");
        }

        static void AppendString(
            StringBuilder builder,
            string name,
            string value,
            bool comma,
            int indent = 2
        )
        {
            AppendIndent(builder, indent);
            builder.Append('"').Append(name).Append("\": \"").Append(value).Append('"');
            builder.Append(comma ? ",\n" : "\n");
        }

        static void AppendBoolean(
            StringBuilder builder,
            string name,
            bool value,
            bool comma,
            int indent = 2
        )
        {
            AppendIndent(builder, indent);
            builder.Append('"').Append(name).Append("\": ");
            builder.Append(value ? "true" : "false");
            builder.Append(comma ? ",\n" : "\n");
        }

        static void AppendInteger(
            StringBuilder builder,
            string name,
            int value,
            bool comma,
            int indent = 2
        )
        {
            AppendIndent(builder, indent);
            builder.Append('"').Append(name).Append("\": ");
            builder.Append(value.ToString(CultureInfo.InvariantCulture));
            builder.Append(comma ? ",\n" : "\n");
        }

        static void AppendLong(
            StringBuilder builder,
            string name,
            long value,
            bool comma,
            int indent = 2
        )
        {
            AppendIndent(builder, indent);
            builder.Append('"').Append(name).Append("\": ");
            builder.Append(value.ToString(CultureInfo.InvariantCulture));
            builder.Append(comma ? ",\n" : "\n");
        }

        static void AppendDouble(
            StringBuilder builder,
            string name,
            double value,
            bool comma,
            int indent = 2
        )
        {
            AppendIndent(builder, indent);
            builder.Append('"').Append(name).Append("\": ");
            builder.Append(value.ToString("0.######", CultureInfo.InvariantCulture));
            builder.Append(comma ? ",\n" : "\n");
        }

        static void AppendIndent(StringBuilder builder, int count)
        {
            for (int index = 0; index < count; index++) builder.Append(' ');
        }

        static string ScenarioToken(RuntimePerformanceScenario scenario)
        {
            switch (scenario)
            {
                case RuntimePerformanceScenario.GameplayIdle:
                    return "gameplay_idle";
                case RuntimePerformanceScenario.GameplayActive:
                    return "gameplay_active";
                case RuntimePerformanceScenario.MetaIdle:
                    return "meta_idle";
                case RuntimePerformanceScenario.MetaTransition:
                    return "meta_transition";
                default:
                    return "unknown";
            }
        }

        static string VerdictToken(RuntimePerformanceVerdict verdict)
        {
            switch (verdict)
            {
                case RuntimePerformanceVerdict.Pass:
                    return "PASS";
                case RuntimePerformanceVerdict.Fail:
                    return "FAIL";
                default:
                    return "INCOMPLETE";
            }
        }
    }

    /// <summary>
    /// Pure, fixed-capacity accumulator used by the runtime probe and tests.
    /// Begin allocates nothing; all arrays are allocated by the constructor.
    /// TryPushSample performs only primitive validation and array writes.
    /// </summary>
    public sealed class RuntimePerformanceAccumulator
    {
        readonly int capacity;
        readonly double[] cpuTotalSamples;
        readonly double[] cpuMainSamples;
        readonly double[] cpuRenderSamples;
        readonly double[] presentWaitSamples;
        readonly double[] gpuSamples;
        readonly long[] gcSamples;
        readonly bool[] cpuSampleValid;
        readonly bool[] gpuSampleValid;
        readonly bool[] gcSampleValid;
        readonly double[] percentileScratch;

        RuntimePerformanceScenario scenario;
        int configuredTargetFrameRate;
        int requestedWarmupFrames;
        int requestedSampleFrames;
        int acceptedWarmupFrames;
        int capturedSampleFrames;
        int rejectedZeroTimestampCount;
        int rejectedDuplicateTimestampCount;
        int invalidCpuSampleCount;
        int gpuValidSampleCount;
        int gcValidSampleCount;
        int overBudgetFrameCount;
        int severeFrameCount;
        ulong lastFrameTimestamp;
        bool interrupted;
        bool frameTimingSupported;
        bool gcCounterSupported;
        bool configured;
        bool sampling;

        public RuntimePerformanceAccumulator(
            int maximumSamples = RuntimePerformanceProfile.MaximumSamples
        )
        {
            capacity = maximumSamples < 1
                ? 1
                : Math.Min(maximumSamples, RuntimePerformanceProfile.MaximumSamples);
            cpuTotalSamples = new double[capacity];
            cpuMainSamples = new double[capacity];
            cpuRenderSamples = new double[capacity];
            presentWaitSamples = new double[capacity];
            gpuSamples = new double[capacity];
            gcSamples = new long[capacity];
            cpuSampleValid = new bool[capacity];
            gpuSampleValid = new bool[capacity];
            gcSampleValid = new bool[capacity];
            percentileScratch = new double[capacity];
        }

        public int Capacity => capacity;
        public bool IsSampling => sampling;
        public bool IsComplete =>
            configured &&
            !sampling &&
            capturedSampleFrames == requestedSampleFrames;
        public int RequestedWarmupFrames => requestedWarmupFrames;
        public int AcceptedWarmupFrames => acceptedWarmupFrames;
        public int RequestedSampleFrames => requestedSampleFrames;
        public int CapturedSampleFrames => capturedSampleFrames;
        public int RejectedZeroTimestampCount => rejectedZeroTimestampCount;
        public int RejectedDuplicateTimestampCount => rejectedDuplicateTimestampCount;

        public void Begin(
            RuntimePerformanceScenario newScenario,
            int warmupFrames,
            int sampleFrames,
            int targetFrameRate,
            bool hasFrameTiming,
            bool hasGcCounter
        )
        {
            scenario = newScenario;
            configuredTargetFrameRate = targetFrameRate;
            requestedWarmupFrames = RuntimePerformanceProfile.ClampWarmupFrames(
                warmupFrames
            );
            requestedSampleFrames = Math.Min(
                RuntimePerformanceProfile.ClampSampleFrames(sampleFrames),
                capacity
            );
            acceptedWarmupFrames = 0;
            capturedSampleFrames = 0;
            rejectedZeroTimestampCount = 0;
            rejectedDuplicateTimestampCount = 0;
            invalidCpuSampleCount = 0;
            gpuValidSampleCount = 0;
            gcValidSampleCount = 0;
            overBudgetFrameCount = 0;
            severeFrameCount = 0;
            lastFrameTimestamp = 0;
            interrupted = false;
            frameTimingSupported = hasFrameTiming;
            gcCounterSupported = hasGcCounter;
            configured = true;
            sampling = hasFrameTiming;
        }

        public bool TryPushSample(
            ulong frameStartTimestamp,
            double cpuTotalMilliseconds,
            double cpuMainMilliseconds,
            double cpuRenderMilliseconds,
            double presentWaitMilliseconds,
            double gpuMilliseconds,
            long managedAllocationBytes
        )
        {
            if (!sampling) return false;

            if (frameStartTimestamp == 0)
            {
                rejectedZeroTimestampCount++;
                return false;
            }

            if (
                lastFrameTimestamp != 0 &&
                frameStartTimestamp <= lastFrameTimestamp
            )
            {
                rejectedDuplicateTimestampCount++;
                return false;
            }

            lastFrameTimestamp = frameStartTimestamp;
            if (acceptedWarmupFrames < requestedWarmupFrames)
            {
                acceptedWarmupFrames++;
                return true;
            }

            int index = capturedSampleFrames;
            if (index >= requestedSampleFrames || index >= capacity)
            {
                sampling = false;
                return false;
            }

            bool validCpu =
                IsFinitePositive(cpuTotalMilliseconds) &&
                IsFiniteNonNegative(cpuMainMilliseconds) &&
                IsFiniteNonNegative(cpuRenderMilliseconds) &&
                IsFiniteNonNegative(presentWaitMilliseconds);

            cpuSampleValid[index] = validCpu;
            if (validCpu)
            {
                cpuTotalSamples[index] = cpuTotalMilliseconds;
                cpuMainSamples[index] = cpuMainMilliseconds;
                cpuRenderSamples[index] = cpuRenderMilliseconds;
                presentWaitSamples[index] = presentWaitMilliseconds;
            }
            else
            {
                cpuTotalSamples[index] = 0.0;
                cpuMainSamples[index] = 0.0;
                cpuRenderSamples[index] = 0.0;
                presentWaitSamples[index] = 0.0;
                invalidCpuSampleCount++;
            }

            bool validGpu = IsFinitePositive(gpuMilliseconds);
            gpuSampleValid[index] = validGpu;
            gpuSamples[index] = validGpu ? gpuMilliseconds : 0.0;
            if (validGpu) gpuValidSampleCount++;

            if (validCpu)
            {
                double pacingMilliseconds = validGpu
                    ? Math.Max(cpuTotalMilliseconds, gpuMilliseconds)
                    : cpuTotalMilliseconds;
                if (
                    pacingMilliseconds >
                    RuntimePerformanceProfile.FrameBudgetMilliseconds
                ) overBudgetFrameCount++;
                if (
                    pacingMilliseconds >
                    RuntimePerformanceProfile.SevereFrameMilliseconds
                ) severeFrameCount++;
            }

            bool validGc = gcCounterSupported && managedAllocationBytes >= 0;
            gcSampleValid[index] = validGc;
            gcSamples[index] = validGc ? managedAllocationBytes : 0L;
            if (validGc) gcValidSampleCount++;

            capturedSampleFrames++;
            if (capturedSampleFrames >= requestedSampleFrames) sampling = false;
            return true;
        }

        public void StopIncomplete()
        {
            interrupted = true;
            sampling = false;
        }

        public void Stop()
        {
            sampling = false;
        }

        public bool TryBuildReport(out RuntimePerformanceReport report)
        {
            if (!configured || sampling)
            {
                report = default(RuntimePerformanceReport);
                return false;
            }

            RuntimePerformanceMetricSummary cpuTotal = BuildMetric(
                cpuTotalSamples,
                cpuSampleValid,
                capturedSampleFrames
            );
            RuntimePerformanceMetricSummary cpuMain = BuildMetric(
                cpuMainSamples,
                cpuSampleValid,
                capturedSampleFrames
            );
            RuntimePerformanceMetricSummary cpuRender = BuildMetric(
                cpuRenderSamples,
                cpuSampleValid,
                capturedSampleFrames
            );
            RuntimePerformanceMetricSummary presentWait = BuildMetric(
                presentWaitSamples,
                cpuSampleValid,
                capturedSampleFrames
            );
            RuntimePerformanceMetricSummary gpu = BuildMetric(
                gpuSamples,
                gpuSampleValid,
                capturedSampleFrames
            );
            RuntimePerformanceAllocationSummary allocation = BuildAllocation();

            int validCpuCount = cpuTotal.Count;
            double overBudgetRatio = validCpuCount > 0
                ? overBudgetFrameCount / (double)validCpuCount
                : 0.0;
            double severeRatio = validCpuCount > 0
                ? severeFrameCount / (double)validCpuCount
                : 0.0;
            double gpuCoverageRatio = capturedSampleFrames > 0
                ? gpuValidSampleCount / (double)capturedSampleFrames
                : 0.0;
            bool gpuSupported =
                gpuCoverageRatio >= RuntimePerformanceProfile.MinimumGpuCoverageRatio;

            RuntimePerformanceVerdict verdict = ResolveVerdict(
                cpuTotal,
                gpu,
                allocation,
                severeRatio,
                gpuSupported
            );

            report = new RuntimePerformanceReport(
                scenario,
                verdict,
                configuredTargetFrameRate,
                requestedWarmupFrames,
                acceptedWarmupFrames,
                requestedSampleFrames,
                capturedSampleFrames,
                rejectedZeroTimestampCount,
                rejectedDuplicateTimestampCount,
                invalidCpuSampleCount,
                interrupted,
                frameTimingSupported,
                gcCounterSupported,
                gpuSupported,
                gpuCoverageRatio,
                overBudgetFrameCount,
                severeFrameCount,
                overBudgetRatio,
                severeRatio,
                cpuTotal,
                cpuMain,
                cpuRender,
                presentWait,
                gpu,
                allocation
            );
            return true;
        }

        RuntimePerformanceVerdict ResolveVerdict(
            RuntimePerformanceMetricSummary cpuTotal,
            RuntimePerformanceMetricSummary gpu,
            RuntimePerformanceAllocationSummary allocation,
            double severeRatio,
            bool gpuSupported
        )
        {
            bool complete =
                !interrupted &&
                capturedSampleFrames == requestedSampleFrames &&
                capturedSampleFrames >= RuntimePerformanceProfile.MinimumPassingSamples;
            bool requiredMetricsAvailable =
                frameTimingSupported &&
                cpuTotal.Count == capturedSampleFrames &&
                invalidCpuSampleCount == 0 &&
                gcCounterSupported &&
                gcValidSampleCount == capturedSampleFrames &&
                gpuSupported;
            bool targetConfigured =
                configuredTargetFrameRate ==
                RuntimePerformanceProfile.TargetFramesPerSecond;

            if (!complete || !requiredMetricsAvailable || !targetConfigured)
                return RuntimePerformanceVerdict.Incomplete;

            bool framePacingPasses =
                cpuTotal.P95 <= RuntimePerformanceProfile.P95BudgetMilliseconds &&
                cpuTotal.P99 <= RuntimePerformanceProfile.P99BudgetMilliseconds &&
                gpu.P95 <= RuntimePerformanceProfile.P95BudgetMilliseconds &&
                gpu.P99 <= RuntimePerformanceProfile.P99BudgetMilliseconds &&
                severeRatio <= RuntimePerformanceProfile.MaximumSevereFrameRatio;
            bool allocationPasses =
                !RuntimePerformanceProfile.RequiresZeroManagedAllocation(scenario) ||
                allocation.TotalBytes == 0L;

            return framePacingPasses && allocationPasses
                ? RuntimePerformanceVerdict.Pass
                : RuntimePerformanceVerdict.Fail;
        }

        RuntimePerformanceMetricSummary BuildMetric(
            double[] samples,
            bool[] valid,
            int sourceCount
        )
        {
            int count = 0;
            double sum = 0.0;
            for (int index = 0; index < sourceCount; index++)
            {
                if (!valid[index]) continue;
                double value = samples[index];
                percentileScratch[count++] = value;
                sum += value;
            }

            if (count == 0) return default(RuntimePerformanceMetricSummary);

            Array.Sort(percentileScratch, 0, count);
            return new RuntimePerformanceMetricSummary(
                count,
                percentileScratch[0],
                sum / count,
                NearestRank(percentileScratch, count, 0.50),
                NearestRank(percentileScratch, count, 0.95),
                NearestRank(percentileScratch, count, 0.99),
                percentileScratch[count - 1]
            );
        }

        RuntimePerformanceAllocationSummary BuildAllocation()
        {
            long total = 0L;
            long maximum = 0L;
            int allocatingFrames = 0;
            for (int index = 0; index < capturedSampleFrames; index++)
            {
                if (!gcSampleValid[index]) continue;
                long value = gcSamples[index];
                if (long.MaxValue - total < value) total = long.MaxValue;
                else total += value;
                if (value > maximum) maximum = value;
                if (value > 0L) allocatingFrames++;
            }

            return new RuntimePerformanceAllocationSummary(
                gcValidSampleCount,
                total,
                maximum,
                allocatingFrames
            );
        }

        static double NearestRank(double[] sorted, int count, double percentile)
        {
            int rank = (int)Math.Ceiling(percentile * count) - 1;
            if (rank < 0) rank = 0;
            if (rank >= count) rank = count - 1;
            return sorted[rank];
        }

        static bool IsFinitePositive(double value)
        {
            return
                value > 0.0 &&
                !double.IsNaN(value) &&
                !double.IsInfinity(value);
        }

        static bool IsFiniteNonNegative(double value)
        {
            return
                value >= 0.0 &&
                !double.IsNaN(value) &&
                !double.IsInfinity(value);
        }
    }

    /// <summary>
    /// Opt-in representative-device sampler. Development bootstraps may
    /// install this component; it remains idle until Begin is called or an
    /// Editor/Development Build verification shortcut is pressed.
    /// </summary>
    public sealed class RuntimePerformanceProbe : MonoBehaviour
    {
        const string ReportDirectoryName = "PerformanceQARuns/v1_14_step04";

        readonly RuntimePerformanceAccumulator accumulator =
            new RuntimePerformanceAccumulator();

        bool sampling;
        bool reportPending;
        bool hasCachedReport;
        bool persistReportWhenReady;
        int captureAttemptCount;
        int maximumCaptureAttempts;
        RuntimePerformanceReport cachedReport;
        string lastReportPath;
        string lastReportError;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        readonly FrameTiming[] frameTimings = new FrameTiming[1];
        ProfilerRecorder gcAllocationRecorder;
        bool gcRecorderCreated;
        bool gcRecorderSupported;
#endif

        public static bool IsCompiled
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return true;
#else
                return false;
#endif
            }
        }

        public bool IsSampling => sampling;
        public bool HasReport => reportPending || hasCachedReport;
        public int SamplesCaptured => accumulator.CapturedSampleFrames;
        public int SamplesRequested => accumulator.RequestedSampleFrames;
        public int RejectedZeroTimestampCount =>
            accumulator.RejectedZeroTimestampCount;
        public int RejectedDuplicateTimestampCount =>
            accumulator.RejectedDuplicateTimestampCount;
        public string LastReportPath => lastReportPath;
        public string LastReportError => lastReportError;

        void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            enabled = true;
#else
            enabled = false;
#endif
        }

        public bool Begin(
            RuntimePerformanceScenario scenario,
            int warmupFrames = RuntimePerformanceProfile.DefaultWarmupFrames,
            int sampleFrames = RuntimePerformanceProfile.DefaultSampleFrames
        )
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return BeginInternal(scenario, warmupFrames, sampleFrames, false);
#else
            return false;
#endif
        }

        /// <summary>
        /// Starts a run and writes its deterministic JSON report when the run
        /// completes, is cancelled, or cannot access required frame timing.
        /// File work occurs only after sampling has stopped.
        /// </summary>
        public bool BeginAndPersist(
            RuntimePerformanceScenario scenario,
            int warmupFrames = RuntimePerformanceProfile.DefaultWarmupFrames,
            int sampleFrames = RuntimePerformanceProfile.DefaultSampleFrames
        )
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return BeginInternal(scenario, warmupFrames, sampleFrames, true);
#else
            return false;
#endif
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        bool BeginInternal(
            RuntimePerformanceScenario scenario,
            int warmupFrames,
            int sampleFrames,
            bool persistReport
        )
        {
            if (sampling) return false;

            reportPending = false;
            hasCachedReport = false;
            persistReportWhenReady = persistReport;
            cachedReport = default(RuntimePerformanceReport);
            lastReportPath = null;
            lastReportError = null;
            captureAttemptCount = 0;
            int clampedWarmup = RuntimePerformanceProfile.ClampWarmupFrames(
                warmupFrames
            );
            int clampedSamples = RuntimePerformanceProfile.ClampSampleFrames(
                sampleFrames
            );
            maximumCaptureAttempts =
                (clampedWarmup + clampedSamples + 16) * 4;

            DisposeGcRecorder();
            bool frameTimingAvailable = FrameTimingManager.IsFeatureEnabled();
            gcRecorderSupported = false;
            try
            {
                gcAllocationRecorder = ProfilerRecorder.StartNew(
                    ProfilerCategory.Memory,
                    "GC Allocated In Frame",
                    1
                );
                gcRecorderCreated = true;
                gcRecorderSupported =
                    gcAllocationRecorder.Valid && gcAllocationRecorder.IsRunning;
            }
            catch (Exception)
            {
                gcAllocationRecorder = default(ProfilerRecorder);
                gcRecorderCreated = false;
                gcRecorderSupported = false;
            }

            accumulator.Begin(
                scenario,
                clampedWarmup,
                clampedSamples,
                Application.targetFrameRate,
                frameTimingAvailable,
                gcRecorderSupported
            );

            if (!frameTimingAvailable)
            {
                accumulator.Stop();
                DisposeGcRecorder();
                reportPending = true;
                PersistReportIfRequested();
                return false;
            }

            sampling = true;
            enabled = true;
            return true;
        }
#endif

        public void Cancel()
        {
            if (!sampling) return;
            StopSampling(true);
        }

        public bool TryGetReport(out RuntimePerformanceReport report)
        {
            if (sampling)
            {
                report = default(RuntimePerformanceReport);
                return false;
            }

            if (!hasCachedReport)
            {
                if (!reportPending || !accumulator.TryBuildReport(out cachedReport))
                {
                    report = default(RuntimePerformanceReport);
                    return false;
                }

                hasCachedReport = true;
                reportPending = false;
            }

            report = cachedReport;
            return true;
        }

        public bool TryGetDeterministicJson(out string json)
        {
            if (!TryGetReport(out RuntimePerformanceReport report))
            {
                json = null;
                return false;
            }

            json = report.ToDeterministicJson();
            return true;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (!sampling)
            {
                if (keyboard == null) return;

                if (keyboard.f6Key.wasPressedThisFrame)
                    BeginAndPersist(RuntimePerformanceScenario.GameplayIdle);
                else if (keyboard.f7Key.wasPressedThisFrame)
                    BeginAndPersist(RuntimePerformanceScenario.GameplayActive);
                else if (keyboard.f8Key.wasPressedThisFrame)
                    BeginAndPersist(RuntimePerformanceScenario.MetaIdle);
                else if (keyboard.f9Key.wasPressedThisFrame)
                    BeginAndPersist(RuntimePerformanceScenario.MetaTransition);
                return;
            }

            if (keyboard != null && keyboard.f10Key.wasPressedThisFrame)
            {
                StopSampling(true);
                return;
            }

            captureAttemptCount++;
            if (captureAttemptCount > maximumCaptureAttempts)
            {
                StopSampling(true);
                return;
            }

            FrameTimingManager.CaptureFrameTimings();
            uint count = FrameTimingManager.GetLatestTimings(1, frameTimings);
            if (count == 0) return;

            FrameTiming timing = frameTimings[0];
            long allocatedBytes =
                gcRecorderSupported && gcAllocationRecorder.Count > 0
                ? gcAllocationRecorder.LastValue
                : -1L;
            accumulator.TryPushSample(
                timing.frameStartTimestamp,
                timing.cpuFrameTime,
                timing.cpuMainThreadFrameTime,
                timing.cpuRenderThreadFrameTime,
                timing.cpuMainThreadPresentWaitTime,
                timing.gpuFrameTime,
                allocatedBytes
            );

            if (!accumulator.IsSampling) StopSampling(false);
        }
#endif

        void OnApplicationPause(bool paused)
        {
            if (paused && sampling) StopSampling(true);
        }

        void OnApplicationFocus(bool focused)
        {
            if (!focused && sampling) StopSampling(true);
        }

        void OnDisable()
        {
            if (sampling) StopSampling(true);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            else DisposeGcRecorder();
#endif
        }

        void OnDestroy()
        {
            if (sampling) StopSampling(true);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            else DisposeGcRecorder();
#endif
        }

        void StopSampling(bool incomplete)
        {
            if (!sampling) return;

            sampling = false;
            if (incomplete) accumulator.StopIncomplete();
            else accumulator.Stop();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            DisposeGcRecorder();
#endif
            reportPending = true;
            PersistReportIfRequested();
        }

        void PersistReportIfRequested()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!persistReportWhenReady || sampling) return;

            persistReportWhenReady = false;
            if (!TryGetReport(out RuntimePerformanceReport report))
            {
                lastReportPath = null;
                lastReportError = "Performance report was not available.";
                return;
            }

            try
            {
                string directory = Path.Combine(
                    Application.persistentDataPath,
                    ReportDirectoryName
                );
                Directory.CreateDirectory(directory);
                string path = Path.Combine(
                    directory,
                    ScenarioFileToken(report.Scenario) + ".json"
                );
                File.WriteAllText(path, report.ToDeterministicJson());
                lastReportPath = path;
                lastReportError = null;
            }
            catch (Exception exception)
            {
                lastReportPath = null;
                lastReportError =
                    exception.GetType().Name + ": " + exception.Message;
            }
#endif
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        static string ScenarioFileToken(RuntimePerformanceScenario scenario)
        {
            switch (scenario)
            {
                case RuntimePerformanceScenario.GameplayIdle:
                    return "gameplay_idle";
                case RuntimePerformanceScenario.GameplayActive:
                    return "gameplay_active";
                case RuntimePerformanceScenario.MetaIdle:
                    return "meta_idle";
                case RuntimePerformanceScenario.MetaTransition:
                    return "meta_transition";
                default:
                    return "unknown";
            }
        }

        void DisposeGcRecorder()
        {
            if (!gcRecorderCreated) return;
            gcAllocationRecorder.Dispose();
            gcAllocationRecorder = default(ProfilerRecorder);
            gcRecorderCreated = false;
            gcRecorderSupported = false;
        }
#endif
    }
}
