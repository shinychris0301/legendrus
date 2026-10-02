#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class HomeDormantArtworkCleanupBuildAudit
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
        string folder=Path.Combine(Directory.GetCurrentDirectory(),"ProjectDocs/AI/AndroidPackageFootprint/R17HomeDormantArtworkCleanup/QA");
        File.WriteAllText(Path.Combine(folder,"PackedAssets.json"),JsonUtility.ToJson(new Record{result=report.summary.result.ToString(),output=report.summary.outputPath,assets=rows},true));
        if(rows.Any(r=>r.guid=="c799ce05d21b30886a6fa33a9c3e4c33"||r.guid=="4bbb7d70ab8e8c29385b536e7e8dbf2f"||r.guid=="b22f4fdf95dddd63f8c32fab7a753da0"||r.guid=="69135cdae1222fa598c05c4ab1539397"||r.guid=="23aee3e96f6777c45a4cae8a64c33c4c"||r.guid=="717db484c9b631d909a47c6df0c87daf"||r.guid=="d76c3dc6c6b59ff2ca8b7698b698ec03"||r.guid=="2cb9251bbe55ba200b4383179b701814"||r.guid=="94bad924675b43dcadb2595547a51117"||r.guid=="51e28d72f0853db939602ec4f6546f7c"||r.guid=="82085c57945c64ecab5231171e999c10"||r.guid=="03d94585ef770c0a9aca2184b5239550"||r.guid=="f83c954371cabff0f9a3fad969811846"||r.guid=="bbb3a20b4740b869c94d2023f0da4929"||r.guid=="7b33517046ee539d4a86ecfd63012ea5"||r.path.EndsWith("/HomeAligned/home_hero_firebird.png")||r.guid=="072000aa439cc61468feba1c5d67b108"||r.path.Contains("/RetiredHome/")||r.path.EndsWith("/HomeAligned/home_firebird_generated.png")||r.path.Contains("/RetiredProgress/")||r.path.EndsWith("/HomeAligned/progress_fill.png")||r.path.EndsWith("/HomeAligned/progress_frame.png")))
            throw new InvalidOperationException("Retired artwork was packed.");
    }
}
#endif
