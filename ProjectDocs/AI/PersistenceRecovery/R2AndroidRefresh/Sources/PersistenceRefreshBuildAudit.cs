#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Temporary audit entry: delegates to the installed development command with every guard intact.
public static class PersistenceRefreshBuildAudit
{
    [Serializable] public sealed class Row { public string path,guid,type; public ulong bytes; }
    [Serializable] public sealed class Fixture
    {
        public string selector; public int completedRounds,acknowledgedScenes,selectedLevel,energy;
        public int[] stocks;
    }
    [Serializable] public sealed class Record
    {
        public string result,output,unityVersion;
        public int settingsRevision,profileRevision,dialogueRevision,gameplayRevision,victoryRevision,stockLayouts,storyScenes,storyMilestones,dailyRewardRevision,receiptRevision;
        public Fixture[] fixedPresets; public Row[] assets;
    }
    public static void Build()
    {
        var settings=Resources.Load<GameObject>("ProductionUI/Pages/Settings").GetComponent<LegendsOfRus.ProductionSettingsView>();
        if(!settings.HasRequiredBindings||settings.Revision!=5||settings.Revision!=LegendsOfRus.ProductionSettingsView.CurrentRevision)
            throw new InvalidOperationException("Installed Settings bindings/revision differ.");
        var profile=Resources.Load<GameObject>("ProductionUI/Pages/Profile").GetComponent<LegendsOfRus.ProductionProfileView>();
        var dialogue=Resources.Load<GameObject>("ProductionUI/Pages/Dialogue").GetComponent<LegendsOfRus.ProductionDialogueView>();
        var gameplay=Resources.Load<GameObject>("ProductionUI/Pages/Gameplay").GetComponent<LegendsOfRus.ProductionGameplayView>();
        var victory=Resources.Load<GameObject>("ProductionUI/Pages/Victory").GetComponent<LegendsOfRus.ProductionVictoryView>();
        var daily=Resources.Load<GameObject>("ProductionUI/Pages/DailyReward").GetComponent<LegendsOfRus.ProductionDailyRewardView>();
        var receipt=Resources.Load<GameObject>("ProductionUI/Pages/RewardClaim").GetComponent<LegendsOfRus.ProductionRewardReceivedView>();
        if(!daily.HasRequiredBindings||daily.Revision!=5||!receipt.HasRequiredBindings||receipt.Revision!=2)
            throw new InvalidOperationException("Installed daily/receipt owner bindings differ.");
        if(!profile.HasRequiredBindings||profile.Revision!=13||profile.Revision!=LegendsOfRus.ProductionProfileView.CurrentRevision||
           !dialogue.HasRequiredBindings||dialogue.Revision!=2||gameplay.Revision!=24||victory.Revision!=12)
            throw new InvalidOperationException("Installed page revisions/bindings differ from accepted baseline.");
        var stocks=gameplay.GetComponentsInChildren<LegendsOfRus.ProductionGameplayStockLayout>(true);
        if(stocks.Length!=4||stocks.Any(s=>s.Badge==null||s.WideBacking==null))
            throw new InvalidOperationException("Expected four bound stock layouts.");
        var story=LegendsOfRus.StoryCatalog.LoadChapter1();
        if(story.scenes.Length!=58||story.milestones.Length!=57)
            throw new InvalidOperationException("Expected active Chapter 1 story catalogue.");
        var factory=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("LegendsOfRus.DevelopmentGameplayFixtures")).First(t=>t!=null);
        var create=factory.GetMethod("Create",BindingFlags.Static|BindingFlags.NonPublic,null,new[]{typeof(string),typeof(long)},null);
        if(create==null)throw new InvalidOperationException("Fixed fixture source is missing.");
        string[] selectors={"stock-999","stock-1000","stock-9999","stock-max","story-early","story-finale"};
        int[] quantities={999,1000,9999,int.MaxValue};
        var fixtures=selectors.Select((selector,index)=>{
            // Pure candidate preparation: no GameState, preferences, saves or ownership publication.
            var candidate=(LegendsOfRus.PlayerSaveData)create.Invoke(null,new object[]{selector,DateTime.UtcNow.Ticks});
            var row=new Fixture{selector=selector,completedRounds=candidate.levelProgress.Count(p=>p.completions>0),
                acknowledgedScenes=candidate.completedStoryScenes.Count,selectedLevel=candidate.selectedLevel,
                energy=candidate.energy,stocks=new[]{candidate.boosterMace,candidate.boosterFirebird,candidate.boosterLivingLight,candidate.boosterSeaPearl}};
            if(row.energy!=4||row.completedRounds!=(index<4?0:index==4?8:500)||
                row.acknowledgedScenes!=(index<4?0:index==4?3:58)||
                index<4&&row.stocks.Any(value=>value!=quantities[index]))
                throw new InvalidOperationException("Fixed fixture candidate differs from reviewed scope: "+selector);
            return row;
        }).ToArray();
        var owner=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand")).First(t=>t!=null);
        owner.GetMethod("BuildDevelopmentApk").Invoke(null,null);
        var report=BuildReport.GetLatestReport();
        if(report==null||report.summary.result!=BuildResult.Succeeded)
            throw new InvalidOperationException("Missing successful player BuildReport.");
        var rows=report.packedAssets.SelectMany(p=>p.contents).Select(p=>new Row{path=p.sourceAssetPath,guid=p.sourceAssetGUID.ToString(),type=p.type?.Name,bytes=p.packedSize}).ToArray();
        if(rows.Length==0)throw new InvalidOperationException("Empty packed asset inventory.");
        string folder=Path.Combine(Directory.GetCurrentDirectory(),"ProjectDocs/AI/PersistenceRecovery/R2AndroidRefresh/QA");
        File.WriteAllText(Path.Combine(folder,"PackedAssets.json"),JsonUtility.ToJson(new Record{
            result=report.summary.result.ToString(),output=report.summary.outputPath,unityVersion=Application.unityVersion,
            settingsRevision=settings.Revision,profileRevision=profile.Revision,dialogueRevision=dialogue.Revision,gameplayRevision=gameplay.Revision,
            victoryRevision=victory.Revision,stockLayouts=stocks.Length,storyScenes=story.scenes.Length,
            dailyRewardRevision=daily.Revision,receiptRevision=receipt.Revision,
            storyMilestones=story.milestones.Length,fixedPresets=fixtures,assets=rows},true));
        if(rows.Any(r=>r.path.Contains("/RetiredHome/")||r.path.Contains("/RetiredProgress/")||
            r.path.EndsWith("/HomeAligned/home_hero_firebird.png")||r.path.EndsWith("/HomeAligned/home_firebird_generated.png")||
            r.path.EndsWith("/HomeAligned/progress_fill.png")||r.path.EndsWith("/HomeAligned/progress_frame.png")))
            throw new InvalidOperationException("Retired artwork was packed.");
    }
}
#endif
