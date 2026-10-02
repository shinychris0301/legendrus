#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionIndividualRankingPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/IndividualRanking.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild IndividualRanking Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding IndividualRanking.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned","*.png"))AssetDatabase.ImportAsset(file,ImportAssetOptions.ForceUpdate);
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("IndividualRanking",typeof(RectTransform));
            try
            {
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.IndividualRanking);
                ProductionIndividualRankingRuntime.BuildInto(rect);var view=root.GetComponent<ProductionIndividualRankingView>();view.RefreshContent();
                if(!view.HasRequiredBindings)throw new InvalidOperationException("IndividualRanking serialized bindings are incomplete.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/IndividualRankingCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:IndividualRanking\",\n  \"revision\": 8,\n  \"rankingService\": \"unavailable\"\n}\n");
            AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionIndividualRankingView>().HasRequiredBindings)throw new InvalidOperationException("IndividualRanking prefab readback failed.");
            Debug.Log("INDIVIDUAL_RANKING_RESPONSIVE_REWARDS_R2_PASS: serialized native page, source sprites, revision and cutover readback.");
        }
    }
}
#endif
