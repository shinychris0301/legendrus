using System;
using System.Globalization;
using System.Text;

namespace LegendsOfRus.OptimizationCore
{
    public enum OptimizationEvidenceKind
    {
        MemorySample = 0,
        LowMemorySignal = 1,
        Bootstrap = 2,
        ScreenRoute = 3,
        ResourceCleanup = 4
    }

    public readonly struct OptimizationEvidenceSample
    {
        public long Sequence { get; }
        public OptimizationEvidenceKind Kind { get; }
        public long AllocatedBytes { get; }
        public long ReservedBytes { get; }
        public long ManagedBytes { get; }
        public double DurationMilliseconds { get; }
        public MemoryBudgetBand MemoryBand { get; }
        public OptimizationLoadVerdict LoadVerdict { get; }

        public OptimizationEvidenceSample(
            long sequence,
            OptimizationEvidenceKind kind,
            long allocatedBytes,
            long reservedBytes,
            long managedBytes,
            double durationMilliseconds,
            MemoryBudgetBand memoryBand,
            OptimizationLoadVerdict loadVerdict
        )
        {
            Sequence = sequence;
            Kind = kind;
            AllocatedBytes = allocatedBytes;
            ReservedBytes = reservedBytes;
            ManagedBytes = managedBytes;
            DurationMilliseconds = durationMilliseconds;
            MemoryBand = memoryBand;
            LoadVerdict = loadVerdict;
        }
    }

    /// <summary>
    /// Fixed-capacity, process-memory-only evidence. Oldest samples are
    /// overwritten when full so profiling cannot grow memory without bound.
    /// </summary>
    public sealed class BoundedOptimizationEvidenceBuffer
    {
        readonly OptimizationEvidenceSample[] samples;
        int start;
        int count;
        long nextSequence = 1L;

        public int Capacity { get { return samples.Length; } }
        public int Count { get { return count; } }

        public BoundedOptimizationEvidenceBuffer(int capacity)
        {
            if (capacity < 1 || capacity > OptimizationPolicy.MaximumEvidenceCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            samples = new OptimizationEvidenceSample[capacity];
        }

        public BoundedOptimizationEvidenceBuffer()
            : this(OptimizationPolicy.DefaultEvidenceCapacity)
        {
        }

        public OptimizationEvidenceSample AddMemory(
            long allocatedBytes,
            long reservedBytes,
            long managedBytes
        )
        {
            MemoryBudgetBand band = MemoryBudgetEvaluator.Evaluate(
                allocatedBytes,
                reservedBytes,
                managedBytes
            );
            return Add(new OptimizationEvidenceSample(
                nextSequence++,
                OptimizationEvidenceKind.MemorySample,
                allocatedBytes,
                reservedBytes,
                managedBytes,
                0.0,
                band,
                OptimizationLoadVerdict.Incomplete
            ));
        }

        public OptimizationEvidenceSample AddLowMemory(
            long allocatedBytes,
            long reservedBytes,
            long managedBytes
        )
        {
            MemoryBudgetBand band = MemoryBudgetEvaluator.Evaluate(
                allocatedBytes,
                reservedBytes,
                managedBytes
            );
            return Add(new OptimizationEvidenceSample(
                nextSequence++,
                OptimizationEvidenceKind.LowMemorySignal,
                allocatedBytes,
                reservedBytes,
                managedBytes,
                0.0,
                band,
                OptimizationLoadVerdict.Incomplete
            ));
        }

        public OptimizationEvidenceSample AddLoad(
            OptimizationLoadKind kind,
            double durationMilliseconds
        )
        {
            OptimizationEvidenceKind evidenceKind;
            switch (kind)
            {
                case OptimizationLoadKind.Bootstrap:
                    evidenceKind = OptimizationEvidenceKind.Bootstrap;
                    break;
                case OptimizationLoadKind.ScreenRoute:
                    evidenceKind = OptimizationEvidenceKind.ScreenRoute;
                    break;
                case OptimizationLoadKind.ResourceCleanup:
                    evidenceKind = OptimizationEvidenceKind.ResourceCleanup;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind));
            }

            return Add(new OptimizationEvidenceSample(
                nextSequence++,
                evidenceKind,
                0L,
                0L,
                0L,
                durationMilliseconds,
                MemoryBudgetBand.Unknown,
                LoadingBudgetEvaluator.Evaluate(kind, durationMilliseconds)
            ));
        }

        OptimizationEvidenceSample Add(OptimizationEvidenceSample sample)
        {
            if (count < samples.Length)
            {
                int index = (start + count) % samples.Length;
                samples[index] = sample;
                count++;
            }
            else
            {
                samples[start] = sample;
                start = (start + 1) % samples.Length;
            }
            return sample;
        }

        public OptimizationEvidenceSample[] Snapshot()
        {
            var result = new OptimizationEvidenceSample[count];
            for (int i = 0; i < count; i++)
                result[i] = samples[(start + i) % samples.Length];
            return result;
        }

        public string ToDeterministicJson()
        {
            OptimizationEvidenceSample[] snapshot = Snapshot();
            var builder = new StringBuilder(256 + snapshot.Length * 192);
            builder.Append("{\"schemaVersion\":")
                .Append(OptimizationPolicy.SchemaVersion)
                .Append(",\"capacity\":")
                .Append(Capacity)
                .Append(",\"count\":")
                .Append(snapshot.Length)
                .Append(",\"samples\":[");

            for (int i = 0; i < snapshot.Length; i++)
            {
                if (i > 0) builder.Append(',');
                AppendSample(builder, snapshot[i]);
            }
            builder.Append("]}");
            return builder.ToString();
        }

        static void AppendSample(StringBuilder builder, OptimizationEvidenceSample value)
        {
            builder.Append("{\"sequence\":").Append(value.Sequence)
                .Append(",\"kind\":\"").Append(KindToken(value.Kind)).Append('"')
                .Append(",\"allocatedBytes\":").Append(value.AllocatedBytes)
                .Append(",\"reservedBytes\":").Append(value.ReservedBytes)
                .Append(",\"managedBytes\":").Append(value.ManagedBytes)
                .Append(",\"durationMilliseconds\":")
                .Append(value.DurationMilliseconds.ToString("0.###", CultureInfo.InvariantCulture))
                .Append(",\"memoryBand\":\"").Append(MemoryBandToken(value.MemoryBand)).Append('"')
                .Append(",\"loadVerdict\":\"").Append(LoadVerdictToken(value.LoadVerdict)).Append("\"}");
        }

        static string KindToken(OptimizationEvidenceKind value)
        {
            switch (value)
            {
                case OptimizationEvidenceKind.MemorySample: return "memory";
                case OptimizationEvidenceKind.LowMemorySignal: return "low_memory";
                case OptimizationEvidenceKind.Bootstrap: return "bootstrap";
                case OptimizationEvidenceKind.ScreenRoute: return "screen_route";
                case OptimizationEvidenceKind.ResourceCleanup: return "resource_cleanup";
                default: return "unknown";
            }
        }

        static string MemoryBandToken(MemoryBudgetBand value)
        {
            switch (value)
            {
                case MemoryBudgetBand.WithinBudget: return "within_budget";
                case MemoryBudgetBand.Warning: return "warning";
                case MemoryBudgetBand.Critical: return "critical";
                default: return "unknown";
            }
        }

        static string LoadVerdictToken(OptimizationLoadVerdict value)
        {
            switch (value)
            {
                case OptimizationLoadVerdict.Pass: return "pass";
                case OptimizationLoadVerdict.Fail: return "fail";
                default: return "incomplete";
            }
        }
    }
}
