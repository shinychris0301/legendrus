#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionEventDetailPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/EventDetail.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild EventDetail Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding EventDetail.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/EventDetailAligned","*.png"))
            {
                var ti=AssetImporter.GetAtPath(file) as TextureImporter;
                LockedDesignImporter.ConfigureShopTexture(ti);ti.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("EventDetail",typeof(RectTransform));
            try
            {
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.EventDetail);
                ProductionEventDetailRuntime.BuildInto(rect);var view=root.GetComponent<ProductionEventDetailView>();view.RefreshContent();
                if(!view.HasRequiredBindings)throw new InvalidOperationException("EventDetail serialized bindings are incomplete.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EventDetailCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:EventDetail\",\n  \"revision\": 8,\n  \"eventSchedule\": \"unavailable\"\n}\n");
            AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionEventDetailView>().HasRequiredBindings)throw new InvalidOperationException("EventDetail prefab readback failed.");
            Debug.Log("EVENT_DETAIL_R8_REBUILD_PASS: serialized native page, source sprites, revision and cutover readback.");
        }
    }
}
#endif
