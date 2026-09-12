using System;

namespace LegendsOfRus.PostLaunchReview
{
    public sealed class PostLaunchAggregateBuffer
    {
        readonly AggregateObservation[] items;
        int next;
        int count;
        public int Capacity { get { return items.Length; } }
        public int Count { get { return count; } }

        public PostLaunchAggregateBuffer(int capacity = PostLaunchReviewRequirements.DefaultBufferCapacity)
        {
            if (capacity <= 0 || capacity > PostLaunchReviewRequirements.MaxBufferCapacity) throw new ArgumentOutOfRangeException("capacity");
            items = new AggregateObservation[capacity];
        }

        public void Add(AggregateObservation value)
        {
            if (value == null) throw new ArgumentNullException("value");
            items[next] = Clone(value);
            next = (next + 1) % items.Length;
            if (count < items.Length) count++;
        }

        public AggregateObservation[] SnapshotOldestFirst()
        {
            var result = new AggregateObservation[count];
            int start = count == items.Length ? next : 0;
            for (int i=0;i<count;i++) result[i] = Clone(items[(start+i)%items.Length]);
            return result;
        }

        public void Clear()
        {
            Array.Clear(items, 0, items.Length); next=0; count=0;
        }

        static AggregateObservation Clone(AggregateObservation v)
        {
            return new AggregateObservation { DayIndex=v.DayIndex, ActiveUsers=v.ActiveUsers, Sessions=v.Sessions, LevelStarts=v.LevelStarts, LevelCompletes=v.LevelCompletes, SoftCurrencyEarned=v.SoftCurrencyEarned, SoftCurrencySpent=v.SoftCurrencySpent };
        }
    }
}
