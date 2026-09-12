using System;

namespace LegendsOfRus.LiveOpsOutcome
{
    public sealed class LiveOpsOutcomeObservationBuffer
    {
        readonly LiveOpsOutcomeAggregateObservation[] items;
        int count;
        int next;
        public int Count{get{return count;}}
        public int Capacity{get{return items.Length;}}
        public LiveOpsOutcomeObservationBuffer(int capacity=LiveOpsOutcomeRequirements.DefaultObservationCapacity)
        {
            if(capacity<1)capacity=1;
            if(capacity>LiveOpsOutcomeRequirements.MaxObservationCapacity)capacity=LiveOpsOutcomeRequirements.MaxObservationCapacity;
            items=new LiveOpsOutcomeAggregateObservation[capacity];
        }
        public void Add(LiveOpsOutcomeAggregateObservation value){if(value==null)return;items[next]=value;next=(next+1)%items.Length;if(count<items.Length)count++;}
        public LiveOpsOutcomeAggregateObservation[] SnapshotOldestFirst(){var r=new LiveOpsOutcomeAggregateObservation[count];int start=count==items.Length?next:0;for(int i=0;i<count;i++)r[i]=items[(start+i)%items.Length];return r;}
        public void Clear(){Array.Clear(items,0,items.Length);count=0;next=0;}
    }
}
