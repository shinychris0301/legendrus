#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using LegendsOfRus;
public static class InstallStockPrefab
{
    public static void Apply()
    {
        if(Application.isPlaying)throw new InvalidOperationException("Idle Editor required.");
        const string path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab";
        var root=PrefabUtility.LoadPrefabContents(path);
        try
        {
            var v=root.GetComponent<ProductionGameplayView>();
            ProductionGameplaySourceControls.ApplyReadability(v);
            v.Revision=ProductionGameplayView.CurrentRevision;
            bool saved;PrefabUtility.SaveAsPrefabAsset(root,path,out saved);if(!saved)throw new IOException(path);
        }
        finally{PrefabUtility.UnloadPrefabContents(root);}
        AssetDatabase.SaveAssets();
        File.WriteAllText("ProjectDocs/AI/CoreOutcomeReadability/R5LargeStock/QA/PrefabInstallation.json","{\"installed\":true,\"page\":\"Gameplay\",\"revision\":24,\"owner\":\"ProductionGameplaySourceControls.ApplyReadability\",\"artworkRebuilt\":false}");
    }
}
#endif
