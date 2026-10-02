using System;

namespace LegendsOfRus.GameplayCore
{
    public interface IBoardRandom
    {
        int Next(int minInclusive, int maxExclusive);
    }

    public sealed class SeededBoardRandom : IBoardRandom
    {
        readonly Random random;

        public int Seed { get; }

        public SeededBoardRandom(int seed)
        {
            Seed = seed;
            random = new Random(seed);
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));

            return random.Next(minInclusive, maxExclusive);
        }
    }

    /// <summary>
    /// Predictable sequence source for deterministic unit tests.
    /// Values are wrapped into the requested range.
    /// </summary>
    public sealed class SequenceBoardRandom : IBoardRandom
    {
        readonly int[] values;
        int index;

        public SequenceBoardRandom(params int[] values)
        {
            this.values = values != null && values.Length > 0
                ? (int[])values.Clone()
                : new[] { 0 };
        }

        public int Next(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
                throw new ArgumentOutOfRangeException(nameof(maxExclusive));

            int range = maxExclusive - minInclusive;
            int value = values[index++ % values.Length];
            int wrapped = ((value % range) + range) % range;
            return minInclusive + wrapped;
        }
    }

    public static class BoardSeedUtility
    {
        public static int CreateRuntimeSeed(int chapter, int level, int entropy)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + chapter;
                hash = hash * 31 + level;
                hash = hash * 31 + entropy;
                return hash;
            }
        }
    }
}
