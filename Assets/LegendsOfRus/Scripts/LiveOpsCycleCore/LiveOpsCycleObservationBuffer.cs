using System;

namespace LegendsOfRus.LiveOpsCycle
{
    public sealed class LiveOpsCycleObservationBuffer
    {
        readonly LiveOpsCycleAggregateObservation[] items;int count;int next;
        public int Capacity{get{return items.Length;}} public int Count{get{return count;}}
        public LiveOpsCycleObservationBuffer(int requestedCapacity=LiveOpsCycleRequirements.DefaultObservationCapacity)
        {
            int c=requestedCapacity<1?1:requestedCapacity;if(c>LiveOpsCycleRequirements.MaxObservationCapacity)c=LiveOpsCycleRequirements.MaxObservationCapacity;items=new LiveOpsCycleAggregateObservation[c];
        }
        public void Add(LiveOpsCycleAggregateObservation value){if(value==null)return;items[next]=value;next=(next+1)%items.Length;if(count<items.Length)count++;}
        public LiveOpsCycleAggregateObservation[] SnapshotOldestFirst(){var r=new LiveOpsCycleAggregateObservation[count];int start=count==items.Length?next:0;for(int i=0;i<count;i++)r[i]=items[(start+i)%items.Length];return r;}
        public void Clear(){Array.Clear(items,0,items.Length);count=0;next=0;}
    }
}
