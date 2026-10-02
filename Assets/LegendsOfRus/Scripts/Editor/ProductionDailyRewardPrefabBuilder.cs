#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionDailyRewardPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/DailyReward.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild DailyReward Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before DailyReward rebuild.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned","*.png"))
            {
                var importer=AssetImporter.GetAtPath(file) as TextureImporter;if(importer==null)throw new InvalidOperationException(file);
                LockedDesignImporter.ConfigureShopTexture(importer);if(file.EndsWith("/card.png",StringComparison.Ordinal))importer.spriteBorder=new Vector4(70,70,70,70);importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);var root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("DailyReward",typeof(RectTransform));
            try
            {
                var r=(RectTransform)root.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.DailyReward);
                ProductionDailyRewardRuntime.BuildInto(r);var v=root.GetComponent<ProductionDailyRewardView>();v.RefreshContent();if(!v.HasRequiredBindings)throw new InvalidOperationException("DailyReward bindings missing.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/DailyRewardCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:DailyReward\",\n  \"revision\": 5\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionDailyRewardView>().HasRequiredBindings)throw new InvalidOperationException("DailyReward prefab readback failed.");
            var savedView=saved.GetComponent<ProductionDailyRewardView>();
            if(savedView.BackText.gameObject.activeSelf||savedView.BackButton.GetComponent<UnityEngine.UI.Image>().sprite!=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned/back_r2.png"))throw new InvalidOperationException("DailyReward Back icon readback failed.");
            Debug.Log("DAILY_REWARD_PROGRESS_ENDS_R4_REBUILD_PASS");
        }
    }
}
#endif
