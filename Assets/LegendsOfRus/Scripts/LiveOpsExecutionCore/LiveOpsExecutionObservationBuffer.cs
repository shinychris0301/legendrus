using System;

namespace LegendsOfRus.LiveOpsExecution
{
    public sealed class LiveOpsExecutionObservationBuffer
    {
        readonly LiveOpsExecutionAggregateObservation[] values;int count,next;
        public int Count{get{return count;}}
        public int Capacity{get{return values.Length;}}
        public LiveOpsExecutionObservationBuffer(int capacity=LiveOpsExecutionRequirements.DefaultObservationCapacity)
        {
            if(capacity<1)capacity=1;if(capacity>LiveOpsExecutionRequirements.MaxObservationCapacity)capacity=LiveOpsExecutionRequirements.MaxObservationCapacity;values=new LiveOpsExecutionAggregateObservation[capacity];
        }
        public void Add(LiveOpsExecutionAggregateObservation value){if(value==null)return;values[next]=value;next=(next+1)%values.Length;if(count<values.Length)count++;}
        public LiveOpsExecutionAggregateObservation[] SnapshotOldestFirst(){var r=new LiveOpsExecutionAggregateObservation[count];int start=count==values.Length?next:0;for(int i=0;i<count;i++)r[i]=values[(start+i)%values.Length];return r;}
        public void Clear(){Array.Clear(values,0,values.Length);count=0;next=0;}
    }
}
