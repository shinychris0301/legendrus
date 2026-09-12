using System;

namespace LegendsOfRus.LaunchOperations
{
    public sealed class LaunchObservationBuffer
    {
        readonly LaunchObservationRecord[] records;
        int next;
        int count;
        int sequence;

        public LaunchObservationBuffer(int capacity = LaunchOperationsRequirements.ObservationBufferCapacity)
        {
            if (capacity < 1 || capacity > LaunchOperationsRequirements.ObservationBufferCapacity) throw new ArgumentOutOfRangeException(nameof(capacity));
            records = new LaunchObservationRecord[capacity];
        }

        public int Count => count;
        public int Capacity => records.Length;

        public void Add(LaunchHealthEvaluation evaluation, int vitalsWindowDays, int openHighIncidents, int openCriticalIncidents)
        {
            if (evaluation == null) throw new ArgumentNullException(nameof(evaluation));
            records[next] = new LaunchObservationRecord
            {
                Sequence = ++sequence,
                Classification = evaluation.Classification,
                Recommendation = evaluation.Recommendation,
                VitalsWindowDays = vitalsWindowDays,
                OpenHighIncidents = openHighIncidents,
                OpenCriticalIncidents = openCriticalIncidents
            };
            next = (next + 1) % records.Length;
            if (count < records.Length) count++;
        }

        public LaunchObservationRecord[] Snapshot()
        {
            var result = new LaunchObservationRecord[count];
            int start = count == records.Length ? next : 0;
            for (int i = 0; i < count; i++) result[i] = records[(start + i) % records.Length];
            return result;
        }

        public void Clear()
        {
            Array.Clear(records, 0, records.Length);
            next = 0;
            count = 0;
            sequence = 0;
        }
    }
}
