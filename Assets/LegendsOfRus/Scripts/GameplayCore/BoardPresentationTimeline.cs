using System;
using System.Collections.Generic;

namespace LegendsOfRus.GameplayCore
{
    /// <summary>Derived presentation times only. Never mutates or serializes puzzle state.</summary>
    public sealed class BoardPresentationTimeline
    {
        public const float Charge = .12f, Sweep = .28f, Flight = .52f, Conversion = .34f;
        public readonly float[] ActivationTimes, ClearTimes, ObstacleTimes;
        public readonly bool[] Converted;
        public float Duration { get; private set; }
        public int UnlinkedActivations { get; private set; }
        readonly BoardResolutionWave wave;
        readonly BoardSnapshot before;
        readonly int span;

        public BoardPresentationTimeline(BoardResolutionWave wave, BoardSnapshot before)
        {
            this.wave = wave; this.before = before; span = Math.Max(before.Width, before.Height);
            int n = wave.Activations.Count;
            ActivationTimes = new float[n]; Converted = new bool[n];
            bool conversion = IsConversion(wave.Combination);
            for (int i=0;i<n;i++)
            {
                var a=wave.Activations[i];
                Converted[i]=conversion && a.ForcedByCombination && a.Kind!=SpecialGemKind.Color &&
                    before.Specials[a.Position.Y*before.Width+a.Position.X]==SpecialGemKind.None;
                ActivationTimes[i]=a.ForcedByCombination ? (Converted[i]?Conversion:0) : float.PositiveInfinity;
                foreach(var p in wave.MatchedCells) if(p==a.Position) ActivationTimes[i]=0;
            }
            // Earliest visible contact, independent of the core's spatially sorted instruction list.
            for(int pass=0;pass<n;pass++)
            {
                bool changed=false;
                for(int i=0;i<n;i++) if(!float.IsPositiveInfinity(ActivationTimes[i]))
                for(int j=0;j<n;j++)
                {
                    if(i==j || wave.Activations[j].ForcedByCombination) continue;
                    float at=ActivationTimes[i]+Contact(wave.Activations[i],wave.Activations[j].Position);
                    if(at<ActivationTimes[j]){ActivationTimes[j]=at;changed=true;}
                }
                if(!changed) break;
            }
            for(int i=0;i<n;i++)
            {
                if(float.IsPositiveInfinity(ActivationTimes[i])){UnlinkedActivations++;ActivationTimes[i]=0;}
                Duration=Math.Max(Duration,ActivationTimes[i]+(wave.Activations[i].Kind==SpecialGemKind.Spark?Flight+Charge+Sweep:Charge+Sweep));
            }
            ClearTimes=new float[wave.ClearedCells.Count];
            for(int i=0;i<ClearTimes.Length;i++)
            {
                var p=wave.ClearedCells[i].Position;float at=FirstContact(p);
                // A hit special remains visible through its own wind-up, then releases.
                for(int j=0;j<n;j++)if(wave.Activations[j].Position==p)
                    at=Math.Max(at,ActivationTimes[j]+(wave.Activations[j].Kind==SpecialGemKind.Spark?0:Charge));
                ClearTimes[i]=at;Duration=Math.Max(Duration,at+.18f);
            }
            ObstacleTimes=new float[wave.ObstacleInteractions.Count];
            for(int i=0;i<ObstacleTimes.Length;i++)
                ObstacleTimes[i]=wave.ObstacleInteractions[i].Cause==BoardObstacleHitCause.AdjacentMatch?0:FirstContact(wave.ObstacleInteractions[i].Position);
        }
        public static bool IsConversion(SpecialCombinationKind c)=>c==SpecialCombinationKind.ColorWithRow||c==SpecialCombinationKind.ColorWithColumn||c==SpecialCombinationKind.ColorWithBurst||c==SpecialCombinationKind.ColorWithSpark;
        float FirstContact(BoardPosition p)
        {
            foreach(var m in wave.MatchedCells)if(m==p)return 0;
            float at=float.PositiveInfinity;
            for(int i=0;i<ActivationTimes.Length;i++)at=Math.Min(at,ActivationTimes[i]+Contact(wave.Activations[i],p));
            // Consumed combo ingredients and direct full-board clears are not all explicit activations.
            return float.IsPositiveInfinity(at)?(wave.Combination==SpecialCombinationKind.ColorWithColor?Charge+Sweep:0):at;
        }
        float Contact(BoardSpecialActivationInstruction a, BoardPosition p)
        {
            if(a.Kind==SpecialGemKind.Spark)
            {
                float launch=Math.Abs(p.X-a.Position.X)+Math.Abs(p.Y-a.Position.Y)<=1?Charge:float.PositiveInfinity;
                if(!a.Target.HasValue)return launch;
                float arrival=a.Target.Value==p?Flight:float.PositiveInfinity;
                if(a.Payload!=SpecialGemKind.None)arrival=Math.Min(arrival,Flight+Footprint(a.Payload,a.Target.Value,p,a.TileType));
                return Math.Min(launch,arrival);
            }
            return Footprint(a.Kind,a.Position,p,a.TileType);
        }
        float Footprint(SpecialGemKind kind,BoardPosition origin,BoardPosition p,int color)
        {
            int dx=Math.Abs(p.X-origin.X),dy=Math.Abs(p.Y-origin.Y),distance=Math.Max(dx,dy);
            if(kind==SpecialGemKind.Row&&dy==0||kind==SpecialGemKind.Column&&dx==0)
                return Charge+Sweep*distance/span;
            if(kind==SpecialGemKind.Burst)
            {
                int radius=wave.Combination==SpecialCombinationKind.BurstWithBurst?4:2;
                if(distance<=radius)return Charge+Sweep*distance/radius;
            }
            if(kind==SpecialGemKind.Color)
            {
                int index=p.Y*before.Width+p.X;
                if(wave.Combination==SpecialCombinationKind.ColorWithColor||
                   before.Tiles[index]==color&&before.Specials[index]==SpecialGemKind.None)
                    return Charge+Sweep*distance/span;
            }
            return origin==p?Charge:float.PositiveInfinity;
        }
    }
}
