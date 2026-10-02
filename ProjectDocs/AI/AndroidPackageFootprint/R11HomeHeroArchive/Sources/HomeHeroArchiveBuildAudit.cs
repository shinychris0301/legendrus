#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class HomeHeroArchiveBuildAudit
{
    [Serializable] public sealed class Row { public string path,guid,type;public ulong bytes; }
    [Serializable] public sealed class Record { public string result,output;public Row[] assets; }
    public static void Build()
    {
        // Invoke the existing approved entry unchanged; every normal guard remains active.
        var owner=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("LegendsOfRus.AndroidBuild.AndroidDevelopmentBuildCommand")).First(t=>t!=null);
        owner.GetMethod("BuildDevelopmentApk").Invoke(null,null);
        var report=BuildReport.GetLatestReport();
        if(report==null||report.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("Missing successful player build report.");
        var rows=report.packedAssets.SelectMany(p=>p.contents).Select(p=>new Row{path=p.sourceAssetPath,guid=p.sourceAssetGUID.ToString(),type=p.type?.Name,bytes=p.packedSize}).ToArray();
        if(rows.Length==0)throw new InvalidOperationException("Build report has no packed asset inventory.");
        string folder=Path.Combine(Directory.GetCurrentDirectory(),"ProjectDocs/AI/AndroidPackageFootprint/R11HomeHeroArchive/QA");
        File.WriteAllText(Path.Combine(folder,"PackedAssets.json"),JsonUtility.ToJson(new Record{result=report.summary.result.ToString(),output=report.summary.outputPath,assets=rows},true));
        if(rows.Any(r=>r.guid=="7b33517046ee539d4a86ecfd63012ea5"||r.path.EndsWith("/HomeAligned/home_hero_firebird.png")||r.guid=="072000aa439cc61468feba1c5d67b108"||r.path.Contains("/RetiredHome/")||r.path.EndsWith("/HomeAligned/home_firebird_generated.png")||r.path.Contains("/RetiredProgress/")||r.path.EndsWith("/HomeAligned/progress_fill.png")||r.path.EndsWith("/HomeAligned/progress_frame.png")))
            throw new InvalidOperationException("Retired artwork was packed.");
    }
}
#endif
