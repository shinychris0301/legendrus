using System;

namespace LegendsOfRus.QA
{
    public enum ClosedTestObservationCode
    {
        None = 0,
        AppBoot = 1,
        HomeReached = 2,
        LevelStarted = 3,
        TutorialStepShown = 4,
        LevelWon = 5,
        LevelFailed = 6,
        RetrySelected = 7,
        SessionEnded = 8
    }

    [Serializable]
    public struct ClosedTestObservation
    {
        public long Sequence;
        public ClosedTestObservationCode Code;
        public int LevelNumber;
        public int PrimaryValue;
        public int SecondaryValue;

        public ClosedTestObservation(
            long sequence,
            ClosedTestObservationCode code,
            int levelNumber,
            int primaryValue,
            int secondaryValue)
        {
            Sequence = sequence;
            Code = code;
            LevelNumber = levelNumber;
            PrimaryValue = primaryValue;
            SecondaryValue = secondaryValue;
        }
    }

    public sealed class ClosedTestObservationBuffer
    {
        public const int DefaultCapacity = 64;
        readonly ClosedTestObservation[] entries;
        int count;
        int writeIndex;
        long nextSequence = 1;

        public ClosedTestObservationBuffer() : this(DefaultCapacity) { }

        public ClosedTestObservationBuffer(int capacity)
        {
            if (capacity < 1 || capacity > 256)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            entries = new ClosedTestObservation[capacity];
        }

        public int Capacity => entries.Length;
        public int Count => count;

        public bool TryRecord(
            ClosedTestObservationCode code,
            int levelNumber = 0,
            int primaryValue = 0,
            int secondaryValue = 0)
        {
            if (code == ClosedTestObservationCode.None ||
                levelNumber < 0)
                return false;

            entries[writeIndex] = new ClosedTestObservation(
                nextSequence++, code, levelNumber, primaryValue, secondaryValue);
            writeIndex = (writeIndex + 1) % entries.Length;
            if (count < entries.Length)
                count++;
            return true;
        }

        public ClosedTestObservation[] Snapshot()
        {
            var result = new ClosedTestObservation[count];
            if (count == 0)
                return result;

            int start = count == entries.Length ? writeIndex : 0;
            for (int i = 0; i < count; i++)
                result[i] = entries[(start + i) % entries.Length];
            return result;
        }

        public void Clear()
        {
            Array.Clear(entries, 0, entries.Length);
            count = 0;
            writeIndex = 0;
            nextSequence = 1;
        }
    }
}
