#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class ProductionNavigationPrefabBuilder
    {
        public static readonly string[] Pages = { "Home", "WorldMap", "ChapterMap", "PreLevel", "Fail", "Shop", "SpecialOffer", "DailyReward" };
        [MenuItem("Legends of Rus/Production UI/Rebuild Navigation Alignment")]
        public static void Rebuild()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play Mode before rebuilding navigation.");
            AssetDatabase.Refresh();
            foreach (string file in System.IO.Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/NavigationAligned", "*.png"))
            {
                var importer = AssetImporter.GetAtPath(file) as TextureImporter;
                LockedDesignImporter.ConfigureShopTexture(importer);importer.SaveAndReimport();
            }
            foreach (string page in Pages)
            {
                string path = "Assets/LegendsOfRus/Resources/ProductionUI/Pages/" + page + ".prefab";
                var root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    foreach (var component in root.GetComponents<MonoBehaviour>()) ProductionNavigationStyle.Apply(component);
                    if (page == "Home")
                    {
                        var marker = root.GetComponent<ProductionReferenceMatchPageMarker>();
                        marker.Configure(ScreenId.Home, marker.ReferenceSha256, ProductionHomeRuntime.GetTextureResourceKeys().Count);
                    }
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
                var saved = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (saved == null) throw new InvalidOperationException("Navigation prefab readback failed: " + page);
                foreach (var component in saved.GetComponents<MonoBehaviour>())
                {
                    var contract = component.GetType().GetProperty("HasRequiredBindings");
                    if (contract != null && contract.PropertyType == typeof(bool) && !(bool)contract.GetValue(component))
                        throw new InvalidOperationException("Navigation binding contract failed: " + page + " / " + component.GetType().Name);
                }
                Debug.Log("NAVIGATION_R1_SAVED " + page);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("NAVIGATION_R1_REBUILD_PASS; Gameplay/Splash/Loading excluded.");
        }
    }
}
#endif
