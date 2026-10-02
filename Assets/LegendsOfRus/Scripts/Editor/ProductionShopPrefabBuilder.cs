#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionShopPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Shop.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Shop Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Shop.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/ShopAligned","*.png"))
            {
                var ti=AssetImporter.GetAtPath(file) as TextureImporter;if(ti==null)throw new InvalidOperationException(file);
                LockedDesignImporter.ConfigureShopTexture(ti);ti.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("Shop",typeof(RectTransform));
            try
            {
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.Shop);
                ProductionShopRuntime.BuildInto(rect);var view=root.GetComponent<ProductionShopView>();view.RefreshContent();
                if(!view.HasRequiredBindings)throw new InvalidOperationException("Shop serialized bindings are incomplete.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ShopCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:Shop\",\n  \"revision\": 2,\n  \"purchaseCatalog\": \"unavailable\"\n}\n");
            AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionShopView>().HasRequiredBindings)throw new InvalidOperationException("Shop prefab readback failed.");
            Debug.Log("SHOP_R2_REBUILD_PASS: serialized native page, source sprites, revision and cutover readback.");
        }
    }
}
#endif
