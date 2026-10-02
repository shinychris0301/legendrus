using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    internal sealed class MechanicsPlaytestFeedback
    {
        readonly GameObject root;
        readonly Text summary,difficulty,goals,effects,export;
        readonly MechanicsSandbox owner;
        public MechanicsPlaytestFeedback(MechanicsSandbox owner,Transform parent)
        {
            this.owner=owner;
            root=new GameObject("PlaytestFeedback",typeof(RectTransform),typeof(Image));root.transform.SetParent(parent,false);
            var r=(RectTransform)root.transform;r.anchorMin=new Vector2(.075f,.165f);r.anchorMax=new Vector2(.925f,.295f);r.offsetMin=r.offsetMax=Vector2.zero;
            root.GetComponent<Image>().color=new Color(.02f,.04f,.08f,.96f);
            summary=Label("AttemptSummary",0,.70f,1,1);
            difficulty=Button("Difficulty",0,.31f,1f/3,.69f,()=>Cycle("difficulty",owner.AttemptLog.Current.difficulty,new[]{"unrated","easy","fair","hard"}));
            goals=Button("GoalClarity",1f/3,.31f,2f/3,.69f,()=>Cycle("goals",owner.AttemptLog.Current.goalClarity,new[]{"unrated","clear","unclear"}));
            effects=Button("EffectClarity",2f/3,.31f,1,.69f,()=>Cycle("effects",owner.AttemptLog.Current.effectClarity,new[]{"unrated","readable","busy"}));
            Button("Save",0,0,1f/3,.30f,()=>{owner.SavePlaytest();Refresh(true);}).text="Save now";
            export=Button("Export",1f/3,0,2f/3,.30f,()=>owner.ExportPlaytest());
            Button("ExportLocation",2f/3,0,1,.30f,()=>{GUIUtility.systemCopyBuffer=owner.LastExportPath??owner.CheckpointPath??MechanicsSandbox.PlaytestExportDirectory;}).text="Copy location";
        }
        Text Label(string name,float x0,float y0,float x1,float y1)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Text));go.transform.SetParent(root.transform,false);var r=(RectTransform)go.transform;r.anchorMin=new Vector2(x0,y0);r.anchorMax=new Vector2(x1,y1);r.offsetMin=new Vector2(3,2);r.offsetMax=new Vector2(-3,-2);
            var t=go.GetComponent<Text>();t.font=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");t.fontSize=22;t.resizeTextForBestFit=true;t.resizeTextMinSize=12;t.resizeTextMaxSize=22;t.alignment=TextAnchor.MiddleCenter;t.color=new Color32(242,213,155,255);t.raycastTarget=false;return t;
        }
        Text Button(string name,float x0,float y0,float x1,float y1,Action action){var t=Label(name,x0,y0,x1,y1);var b=t.gameObject.AddComponent<Button>();t.raycastTarget=true;b.targetGraphic=t;b.onClick.AddListener(()=>action());return t;}
        void Cycle(string key,string current,string[] values){owner.RatePlaytest(key,values[(Array.IndexOf(values,current)+1)%values.Length]);Refresh(true);}
        public void Refresh(bool visible)
        {
            root.SetActive(visible);if(!visible||owner.AttemptLog.Current==null)return;
            var a=owner.AttemptLog.Current;
            summary.text=$"Attempt {a.number} · {a.outcome} · {a.movesRemaining}/{a.moveBudget} moves\n{owner.SaveStatus} · {owner.AttemptLog.CompletedCount} recorded";
            difficulty.text="Difficulty\n"+a.difficulty;goals.text="Goals\n"+a.goalClarity;effects.text="Effects\n"+a.effectClarity;
            export.text=owner.ExportStatus??"Export JSON";
        }
    }
}
