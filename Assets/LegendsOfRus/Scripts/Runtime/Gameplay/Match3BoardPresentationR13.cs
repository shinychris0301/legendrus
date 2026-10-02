using System.Collections;
using System.Collections.Generic;
using LegendsOfRus.GameplayCore;
using UnityEngine;
using SpecialGem = LegendsOfRus.GameplayCore.SpecialGemKind;

namespace LegendsOfRus
{
    public sealed partial class Match3Board
    {
        public BoardPresentationTimeline LastPresentationTimeline { get; private set; }
        public float PresentationWaveStartedAt { get; private set; }

        IEnumerator AnimateCausalWave(BoardResolutionWave wave, BoardResolutionPlan plan)
        {
            var timeline = new BoardPresentationTimeline(wave, model.CreateSnapshot());
            LastPresentationTimeline = timeline;
            var gathered = new HashSet<BoardPosition>();
            if(wave.Creations.Count>0)yield return GatherForCreations(wave,plan.PlanHash,gathered);
            var impacts=GetComponentInChildren<PrototypeImpactView>();
            // Combination energy travels between the actual ingredient cells before release.
            if(wave.ComboIndex==1 && wave.Combination!=SpecialCombinationKind.None &&
               plan.PrimaryPosition!=plan.SecondaryPosition &&
               model.GetSpecial(plan.PrimaryPosition)!=SpecialGem.None && model.GetSpecial(plan.SecondaryPosition)!=SpecialGem.None)
            {
                impacts?.PresentLink(plan.PrimaryPosition,plan.SecondaryPosition,.18f);
                yield return new WaitForSecondsRealtime(.18f);
            }
            BoardPosition colorOrigin=plan.PrimaryPosition;
            if(model.GetSpecial(plan.SecondaryPosition)==SpecialGem.Color)colorOrigin=plan.SecondaryPosition;
            for(int i=0;i<wave.Activations.Count;i++)if(timeline.Converted[i])
                impacts?.PresentLink(colorOrigin,wave.Activations[i].Position,.22f);

            var activated=new bool[wave.Activations.Count];
            var converted=new bool[activated.Length];
            var payloads=new bool[activated.Length];
            var convertedKinds=new Dictionary<BoardPosition,SpecialGem>();
            for(int i=0;i<activated.Length;i++)if(timeline.Converted[i])convertedKinds[wave.Activations[i].Position]=wave.Activations[i].Kind;
            var cleared=new bool[wave.ClearedCells.Count];
            var obstacles=new bool[wave.ObstacleInteractions.Count];
            PublishMotion(GameplayMotionKind.MatchWaveStarted,ToView(plan.PrimaryPosition),ToView(plan.SecondaryPosition),wave.ComboIndex,wave.ClearedCells.Count,SpecialGem.None,plan.PlanHash,wave.Combination);
            PresentationWaveStartedAt=Time.unscaledTime;
            float elapsed=0;
            while(elapsed<=timeline.Duration+.02f)
            {
                for(int i=0;i<activated.Length;i++)
                {
                    var a=wave.Activations[i];var p=a.Position;
                    if(timeline.Converted[i]&&!converted[i]&&elapsed>=.22f)
                    {
                        converted[i]=true;
                        PresentConvertedSpecial(p,a.Kind);
                        PublishMotion(GameplayMotionKind.SpecialConversionStarted,ToView(colorOrigin),ToView(p),wave.ComboIndex,1,a.Kind,plan.PlanHash,wave.Combination);
                    }
                    if(!activated[i]&&elapsed>=timeline.ActivationTimes[i])
                    {
                        activated[i]=true;
                        if(a.Kind==SpecialGem.Color)
                            foreach(var cell in wave.ClearedCells)
                                if(wave.Combination==SpecialCombinationKind.ColorWithColor || cell.Cell.Special==SpecialGem.None&&cell.Cell.TileType==a.TileType)
                                    impacts?.PresentLink(p,cell.Position,BoardPresentationTimeline.Sweep*Mathf.Max(Mathf.Abs(cell.Position.X-p.X),Mathf.Abs(cell.Position.Y-p.Y))/Mathf.Max(columns,rows),BoardPresentationTimeline.Charge);
                        if(a.Kind==SpecialGem.Spark)
                        {
                            gemImages[p.X,p.Y].enabled=false;specialImages[p.X,p.Y].enabled=false;
                            GetComponentInChildren<SparkFlightView>()?.Present(a);
                        }
                        PublishMotion(GameplayMotionKind.SpecialActivationStarted,ToView(p),a.Target.HasValue?ToView(a.Target.Value):ToView(p),wave.ComboIndex,1,a.Kind,plan.PlanHash,wave.Combination,GameplayBoosterKind.None,null,a.Payload);
                    }
                    if(activated[i] && a.Kind!=SpecialGem.Spark)
                    {
                        float t=elapsed-timeline.ActivationTimes[i];
                        visualRoots[p.X,p.Y].localScale=Vector3.one*(t<.12f?Mathf.Lerp(1,.82f,t/.12f):Mathf.Lerp(1.25f,1,Mathf.Clamp01((t-.12f)/.18f)));
                    }
                    if(a.Kind==SpecialGem.Spark && a.Payload!=SpecialGem.None && a.Target.HasValue && !payloads[i]&&elapsed>=timeline.ActivationTimes[i]+SparkFlightView.Duration)
                    {
                        payloads[i]=true;
                        PublishMotion(GameplayMotionKind.SpecialPayloadImpact,ToView(a.Target.Value),ToView(a.Target.Value),wave.ComboIndex,1,a.Payload,plan.PlanHash,wave.Combination);
                    }
                }
                List<BoardClearedCellInstruction> batch=null;
                for(int i=0;i<cleared.Length;i++)
                {
                    if(elapsed<timeline.ClearTimes[i])continue;
                    var c=wave.ClearedCells[i];var p=c.Position;
                    if(!cleared[i])
                    {
                        cleared[i]=true;
                        if(!gathered.Contains(p))
                        {
                            if(batch==null)batch=new List<BoardClearedCellInstruction>();
                            // Emit the visible identity, not the ordinary gem that was replaced by a conversion.
                            batch.Add(convertedKinds.TryGetValue(p,out var kind)?new BoardClearedCellInstruction(p,new BoardCellData(c.Cell.TileType,kind)):c);
                        }
                    }
                    float t=Mathf.Clamp01((elapsed-timeline.ClearTimes[i])/.18f);
                    // Contact squash, sharp release, then a short fade into the textured fragments.
                    visualGroups[p.X,p.Y].alpha=gathered.Contains(p)?0:1-Mathf.Clamp01(t*2.5f);
                    visualRoots[p.X,p.Y].localScale=t<.16f?new Vector3(1.28f,.76f,1):Vector3.one*Mathf.Lerp(1.2f,.35f,(t-.16f)/.84f);
                }
                if(batch!=null)PublishMotion(GameplayMotionKind.CellsImpacted,ToView(batch[0].Position),ToView(batch[0].Position),wave.ComboIndex,batch.Count,SpecialGem.None,plan.PlanHash,wave.Combination,GameplayBoosterKind.None,batch);
                for(int i=0;i<obstacles.Length;i++) if(!obstacles[i]&&elapsed>=timeline.ObstacleTimes[i])
                {
                    obstacles[i]=true;var o=wave.ObstacleInteractions[i];
                    SafeInvoke(ObstacleInteracted,o,"ObstacleInteracted");
                    // Presentation damage updates at contact; the model remains immutable until the wave commits.
                    cellMechanics[o.Position.X,o.Position.Y].Refresh(new BoardObstacleData(o.LayersAfter==0?BoardObstacleKind.None:o.Kind,o.LayersAfter));
                }
                yield return null;elapsed+=Time.unscaledDeltaTime;
            }
        }

        void PresentConvertedSpecial(BoardPosition p,SpecialGem kind)
        {
            var icon=specialImages[p.X,p.Y];gemImages[p.X,p.Y].enabled=false;
            icon.enabled=true;icon.color=Color.white;icon.preserveAspect=true;
            icon.sprite=PrototypeBoardArt.Load(kind==SpecialGem.Burst?"bomb":kind==SpecialGem.Spark?"spark":"line");
            icon.rectTransform.localRotation=Quaternion.Euler(0,0,kind==SpecialGem.Column?90:0);
            icon.rectTransform.anchorMin=Vector2.one*-.04f;icon.rectTransform.anchorMax=Vector2.one*1.04f;
            icon.rectTransform.offsetMin=icon.rectTransform.offsetMax=Vector2.zero;
        }

        IEnumerator GatherForCreations(BoardResolutionWave wave,string hash,HashSet<BoardPosition> gathered)
        {
            var destinations=new Dictionary<BoardPosition,BoardPosition>();
            foreach(var creation in wave.Creations)
            {
                // Follow connected match runs, so a separate same-color match never flies into this creation.
                var connected=new HashSet<BoardPosition>{creation.Position};
                bool changed=true;
                while(changed)
                {
                    changed=false;
                    foreach(var run in wave.Runs)
                    {
                        if(run.Type!=creation.CellBefore.TileType)continue;
                        bool touches=false;foreach(var p in run.Cells)if(connected.Contains(p)){touches=true;break;}
                        if(touches)foreach(var p in run.Cells)if(connected.Add(p))changed=true;
                    }
                }
                foreach(var p in connected)
                    if(p!=creation.Position&&!destinations.ContainsKey(p)&&!IsCreationCell(wave,p))destinations.Add(p,creation.Position);
                PrepareCreation(creation);
                PublishMotion(GameplayMotionKind.SpecialGatherStarted,ToView(creation.Position),ToView(creation.Position),wave.ComboIndex,connected.Count,creation.Kind,hash);
            }
            float elapsed=0;
            while(elapsed<SpecialCreationGraphic.GatherSeconds)
            {
                float t=Mathf.SmoothStep(0,1,elapsed/SpecialCreationGraphic.GatherSeconds);
                foreach(var creation in wave.Creations)visualGroups[creation.Position.X,creation.Position.Y].alpha=1-t;
                foreach(var pair in destinations)
                {
                    var rt=visualRoots[pair.Key.X,pair.Key.Y];var target=visualRoots[pair.Value.X,pair.Value.Y];
                    Vector2 delta=rt.parent.InverseTransformPoint(target.position);
                    Vector2 side=new Vector2(-delta.y,delta.x).normalized;
                    rt.anchoredPosition=delta*t+side*Mathf.Sin(t*Mathf.PI)*Mathf.Min(delta.magnitude*.12f,rt.rect.width*.18f);
                    rt.localScale=Vector3.one*Mathf.Lerp(1,.12f,t);visualGroups[pair.Key.X,pair.Key.Y].alpha=1-Mathf.SmoothStep(0,1,Mathf.Clamp01((t-.65f)/.35f));
                }
                yield return null;elapsed+=Time.unscaledDeltaTime;
            }
            foreach(var pair in destinations){gathered.Add(pair.Key);visualGroups[pair.Key.X,pair.Key.Y].alpha=0;}
        }
        static bool IsCreationCell(BoardResolutionWave wave,BoardPosition p){foreach(var c in wave.Creations)if(c.Position==p)return true;return false;}
    }
}
