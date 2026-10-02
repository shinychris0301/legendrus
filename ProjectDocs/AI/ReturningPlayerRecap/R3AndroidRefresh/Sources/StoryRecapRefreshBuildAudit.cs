#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class StoryRecapRefreshBuildAudit
{
    [Serializable] public sealed class Row { public string path,guid,type; public ulong bytes; }
    [Serializable] public sealed class Record
    {
        public string result,output,unityVersion;
        public int profileRevision,dialogueRevision,gameplayRevision,victoryRevision,stockLayouts,storyScenes,storyMilestones;
        public Row[] assets;
    }
    public static void Build()
    {
        var profile=Resources.Load<GameObject>("ProductionUI/Pages/Profile").GetComponent<LegendsOfRus.ProductionProfileView>();
        var dialogue=Resources.Load<GameObject>("ProductionUI/Pages/Dialogue").GetComponent<LegendsOfRus.ProductionDialogueView>();
        var gameplay=Resources.Load<GameObject>("ProductionUI/Pages/Gameplay").GetComponent<LegendsOfRus.ProductionGameplayView>();
        var victory=Resources.Load<GameObject>("ProductionUI/Pages/Victory").GetComponent<LegendsOfRus.ProductionVictoryView>();
        if(!profile.HasRequiredBindings||profile.Revision!=13||profile.Revision!=LegendsOfRus.ProductionProfileView.CurrentRevision||
           !dialogue.HasRequiredBindings||dialogue.Revision!=2||gameplay.Revision!=24||victory.Revision!=12)
            throw new InvalidOperationException("Installed native page bindings/revisions differ from verified recap baseline.");
        var stocks=gameplay.GetComponentsInChildren<LegendsOfRus.ProductionGameplayStockLayout>(true);
        if(stocks.Length!=4||stocks.Any(s=>s.Badge==null||s.WideBacking==null))
            throw new InvalidOperationException("Expected four bound stock layouts.");
        var story=LegendsOfRus.StoryCatalog.LoadChapter1();
        if(story.scenes.Length!=58||story.milestones.Length!=57)
            throw new InvalidOperationException("Expected the active Chapter 1 story catalogue.");
        // Use the existing approved command unchanged, retaining all policy and build guards.
        var owner=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand")).First(t=>t!=null);
        owner.GetMethod("BuildDevelopmentApk").Invoke(null,null);
        var report=BuildReport.GetLatestReport();
        if(report==null||report.summary.result!=BuildResult.Succeeded)
            throw new InvalidOperationException("Missing successful player BuildReport.");
        var rows=report.packedAssets.SelectMany(p=>p.contents).Select(p=>new Row{path=p.sourceAssetPath,guid=p.sourceAssetGUID.ToString(),type=p.type?.Name,bytes=p.packedSize}).ToArray();
        if(rows.Length==0)throw new InvalidOperationException("Empty packed asset inventory.");
        string folder=Path.Combine(Directory.GetCurrentDirectory(),"ProjectDocs/AI/ReturningPlayerRecap/R3AndroidRefresh/QA");
        File.WriteAllText(Path.Combine(folder,"PackedAssets.json"),JsonUtility.ToJson(new Record{
            result=report.summary.result.ToString(),output=report.summary.outputPath,unityVersion=Application.unityVersion,
            profileRevision=profile.Revision,dialogueRevision=dialogue.Revision,gameplayRevision=gameplay.Revision,
            victoryRevision=victory.Revision,stockLayouts=stocks.Length,storyScenes=story.scenes.Length,storyMilestones=story.milestones.Length,assets=rows},true));
        if(rows.Any(r=>r.path.Contains("/RetiredHome/")||r.path.Contains("/RetiredProgress/")||
            r.path.EndsWith("/HomeAligned/home_hero_firebird.png")||r.path.EndsWith("/HomeAligned/home_firebird_generated.png")||
            r.path.EndsWith("/HomeAligned/progress_fill.png")||r.path.EndsWith("/HomeAligned/progress_frame.png")))
            throw new InvalidOperationException("Retired artwork was packed.");
    }
}
#endif
