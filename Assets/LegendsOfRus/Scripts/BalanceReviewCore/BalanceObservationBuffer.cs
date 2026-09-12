using System;

namespace LegendsOfRus.BalanceReview
{
    public enum BalanceObservationCode
    {
        None = 0,
        LevelStarted = 1,
        LevelWon = 2,
        LevelFailed = 3,
        TutorialShown = 4,
        TutorialCompleted = 5
    }

    [Serializable]
    public struct BalanceObservation
    {
        public long Sequence;
        public BalanceObservationCode Code;
        public int LevelNumber;
        public int AttemptNumber;
        public int MovesRemaining;
        public int MoveLimit;
        public int Score;
        public int TutorialKey;

        public BalanceObservation(
            long sequence,
            BalanceObservationCode code,
            int levelNumber,
            int attemptNumber,
            int movesRemaining,
            int moveLimit,
            int score,
            int tutorialKey)
        {
            Sequence = sequence;
            Code = code;
            LevelNumber = levelNumber;
            AttemptNumber = attemptNumber;
            MovesRemaining = movesRemaining;
            MoveLimit = moveLimit;
            Score = score;
            TutorialKey = tutorialKey;
        }
    }

    public sealed class BalanceObservationBuffer
    {
        public const int DefaultCapacity = 128;
        public const int MaxCapacity = 256;

        readonly BalanceObservation[] entries;
        int count;
        int writeIndex;
        long nextSequence = 1;

        public BalanceObservationBuffer() : this(DefaultCapacity) { }

        public BalanceObservationBuffer(int capacity)
        {
            if (capacity < 1 || capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity));
            entries = new BalanceObservation[capacity];
        }

        public int Capacity => entries.Length;
        public int Count => count;

        public bool TryRecord(
            BalanceObservationCode code,
            int levelNumber,
            int attemptNumber = 0,
            int movesRemaining = 0,
            int moveLimit = 0,
            int score = 0,
            int tutorialKey = 0)
        {
            if (code == BalanceObservationCode.None || levelNumber < 1)
                return false;
            if (attemptNumber < 0 || movesRemaining < 0 || moveLimit < 0 || score < 0)
                return false;
            if ((code == BalanceObservationCode.LevelStarted ||
                 code == BalanceObservationCode.LevelWon ||
                 code == BalanceObservationCode.LevelFailed) && attemptNumber < 1)
                return false;
            if ((code == BalanceObservationCode.TutorialShown ||
                 code == BalanceObservationCode.TutorialCompleted) && tutorialKey == 0)
                return false;

            entries[writeIndex] = new BalanceObservation(
                nextSequence++, code, levelNumber, attemptNumber,
                movesRemaining, moveLimit, score, tutorialKey);
            writeIndex = (writeIndex + 1) % entries.Length;
            if (count < entries.Length)
                count++;
            return true;
        }

        public BalanceObservation[] Snapshot()
        {
            var result = new BalanceObservation[count];
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
