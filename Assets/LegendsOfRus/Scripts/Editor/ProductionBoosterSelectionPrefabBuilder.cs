#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionBoosterSelectionPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/BoosterSelect.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Booster Selection")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before Booster Selection rebuild.");
            AssetDatabase.Refresh();
            // Pre-Level already owns its working catalog entry; do not rebuild another page for this visual pass.
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/BoosterSelectionAligned","*.png"))
            {var importer=AssetImporter.GetAtPath(file) as TextureImporter;LockedDesignImporter.ConfigureShopTexture(importer);importer.SaveAndReimport();}
            bool exists=File.Exists(PrefabPath);var root=exists?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("BoosterSelection",typeof(RectTransform));
            try
            {
                var r=(RectTransform)root.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.BoosterSelect);
                ProductionBoosterSelectionRuntime.BuildInto(r);
                if(!root.GetComponent<ProductionBoosterSelectionView>().HasRequiredBindings)throw new InvalidOperationException("Booster Selection bindings missing.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(exists)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/BoosterSelectCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:BoosterSelect\",\n  \"revision\": 2\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionBoosterSelectionView>().HasRequiredBindings)throw new InvalidOperationException("Booster Selection readback failed.");
            Debug.Log("BOOSTER_SELECTION_REFERENCE_R1_REBUILD_PASS");
        }
    }
}
#endif
