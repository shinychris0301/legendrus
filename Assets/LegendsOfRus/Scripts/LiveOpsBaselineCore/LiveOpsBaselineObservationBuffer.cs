using System;

namespace LegendsOfRus.LiveOpsBaseline
{
    public sealed class LiveOpsBaselineObservationBuffer
    {
        readonly LiveOpsBaselineAggregateObservation[] items;
        int start;
        int count;
        public LiveOpsBaselineObservationBuffer(int capacity=LiveOpsBaselineRequirements.DefaultObservationCapacity)
        {
            if(capacity<1)capacity=1;
            if(capacity>LiveOpsBaselineRequirements.MaxObservationCapacity)capacity=LiveOpsBaselineRequirements.MaxObservationCapacity;
            items=new LiveOpsBaselineAggregateObservation[capacity];
        }
        public int Count{get{return count;}}
        public int Capacity{get{return items.Length;}}
        public void Add(LiveOpsBaselineAggregateObservation item)
        {
            if(item==null)throw new ArgumentNullException("item");
            if(count<items.Length){items[(start+count)%items.Length]=item;count++;return;}
            items[start]=item;start=(start+1)%items.Length;
        }
        public LiveOpsBaselineAggregateObservation[] SnapshotOldestFirst()
        {
            var result=new LiveOpsBaselineAggregateObservation[count];
            for(int i=0;i<count;i++)result[i]=items[(start+i)%items.Length];
            return result;
        }
        public void Clear(){Array.Clear(items,0,items.Length);start=0;count=0;}
    }
}
