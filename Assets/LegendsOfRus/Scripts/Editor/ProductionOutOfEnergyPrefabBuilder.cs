#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionOutOfEnergyPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/OutOfEnergy.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Out of Energy")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before Out of Energy rebuild.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/OutOfEnergyAligned","*.png"))
            {var importer=AssetImporter.GetAtPath(file) as TextureImporter;LockedDesignImporter.ConfigureShopTexture(importer);importer.SaveAndReimport();}
            bool exists=File.Exists(PrefabPath);var root=exists?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("OutOfEnergy",typeof(RectTransform));
            try
            {
                var r=(RectTransform)root.transform;r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.OutOfEnergy);
                ProductionOutOfEnergyRuntime.BuildInto(r);
                if(!root.GetComponent<ProductionOutOfEnergyView>().HasRequiredBindings)throw new InvalidOperationException("Out of Energy bindings missing.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(exists)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/OutOfEnergyCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:OutOfEnergy\",\n  \"revision\": 2\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionOutOfEnergyView>().HasRequiredBindings)throw new InvalidOperationException("Out of Energy readback failed.");
            Debug.Log("OUT_OF_ENERGY_REFERENCE_R1_REBUILD_PASS");
        }
    }
}
#endif
