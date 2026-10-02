#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionRewardReceivedPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/RewardClaim.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Reward Received")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before Reward Received rebuild.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/RewardReceivedAligned","*.png"))
            {var importer=AssetImporter.GetAtPath(file) as TextureImporter;LockedDesignImporter.ConfigureShopTexture(importer);importer.SaveAndReimport();}
            bool exists=File.Exists(PrefabPath);var root=exists?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("RewardClaim",typeof(RectTransform));
            try
            {
                var r=(RectTransform)root.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.RewardClaim);
                ProductionRewardReceivedRuntime.BuildInto(r);
                if(!root.GetComponent<ProductionRewardReceivedView>().HasRequiredBindings)throw new InvalidOperationException("Reward Received bindings missing.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(exists)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/RewardClaimCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:RewardClaim\",\n  \"revision\": 2\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionRewardReceivedView>().HasRequiredBindings)throw new InvalidOperationException("Reward Received readback failed.");
            Debug.Log("REWARD_RECEIVED_REFERENCE_R1_REBUILD_PASS");
        }
    }
}
#endif
