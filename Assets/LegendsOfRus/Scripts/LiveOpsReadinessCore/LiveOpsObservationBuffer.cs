using System;

namespace LegendsOfRus.LiveOpsReadiness
{
    public sealed class LiveOpsObservationBuffer
    {
        readonly LiveOpsAggregateObservation[] items;
        int count;
        int next;
        public LiveOpsObservationBuffer(int capacity = LiveOpsReadinessRequirements.DefaultObservationCapacity)
        {
            if (capacity < 1 || capacity > LiveOpsReadinessRequirements.MaxObservationCapacity) throw new ArgumentOutOfRangeException("capacity");
            items = new LiveOpsAggregateObservation[capacity];
        }
        public int Count { get { return count; } }
        public int Capacity { get { return items.Length; } }
        public void Add(LiveOpsAggregateObservation observation)
        {
            if (observation == null) throw new ArgumentNullException("observation");
            items[next] = observation; next=(next+1)%items.Length; if(count<items.Length)count++;
        }
        public LiveOpsAggregateObservation[] SnapshotOldestFirst()
        {
            var result=new LiveOpsAggregateObservation[count]; int start=count==items.Length?next:0;
            for(int i=0;i<count;i++)result[i]=items[(start+i)%items.Length]; return result;
        }
        public void Clear(){Array.Clear(items,0,items.Length);count=0;next=0;}
    }
}
