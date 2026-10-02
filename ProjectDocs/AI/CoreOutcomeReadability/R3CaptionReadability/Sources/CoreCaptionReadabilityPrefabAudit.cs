#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus;
public static class CoreCaptionReadabilityPrefabAudit
{
    public static void Apply()
    {
        if(Application.isPlaying)throw new InvalidOperationException("Idle Editor required.");
        foreach(var page in new[]{"Gameplay","Victory"})
        {
            string path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+page+".prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                if(page=="Gameplay")ProductionGameplaySourceControls.ApplyReadability(root.GetComponent<ProductionGameplayView>());
                else ProductionVictoryReferenceStyle.ApplyCaptionReadability(root.GetComponent<ProductionVictoryView>());
                if(page=="Gameplay")root.GetComponent<ProductionGameplayView>().Revision=ProductionGameplayView.CurrentRevision;
                else root.GetComponent<ProductionVictoryView>().Revision=ProductionVictoryView.CurrentRevision;
                bool saved;PrefabUtility.SaveAsPrefabAsset(root,path,out saved);if(!saved)throw new IOException(path);
            }
            finally{PrefabUtility.UnloadPrefabContents(root);}
        }
        AssetDatabase.SaveAssets();
        File.WriteAllText("ProjectDocs/AI/CoreOutcomeReadability/R3CaptionReadability/QA/PrefabInstallation.json","{\"installed\":true,\"pages\":[\"Gameplay\",\"Victory\"],\"existingOwnersUsed\":true,\"artworkRebuilt\":false}");
    }
}
#endif
