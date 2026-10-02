#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus;
public static class CoreOutcomeReadabilityPrefabAudit
{
    public static void Apply()
    {
        if(Application.isPlaying)throw new InvalidOperationException("Idle Editor required.");
        foreach(var page in new[]{"Gameplay","Victory","ChapterComplete"})
        {
            string path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+page+".prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                if(page=="Gameplay")ProductionGameplaySourceControls.ApplyReadability(root.GetComponent<ProductionGameplayView>());
                else if(page=="Victory")ProductionVictoryReferenceStyle.ApplyTouchTargets(root.GetComponent<ProductionVictoryView>());
                else ProductionChapterCompleteRuntime.ApplyTouchTargets(root.GetComponent<ProductionChapterCompleteView>());
                bool saved;PrefabUtility.SaveAsPrefabAsset(root,path,out saved);if(!saved)throw new IOException(path);
            }
            finally{PrefabUtility.UnloadPrefabContents(root);}
        }
        AssetDatabase.SaveAssets();
        File.WriteAllText("ProjectDocs/AI/CoreOutcomeReadability/R1/QA/PrefabInstallation.json","{\"installed\":true,\"pages\":[\"Gameplay\",\"Victory\",\"ChapterComplete\"],\"existingOwnersUsed\":true,\"artworkRebuilt\":false}");
    }
}
#endif
