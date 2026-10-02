#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionEventsPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Events.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Events Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Events.");
            AssetDatabase.Refresh();
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned","*.png"))
            {
                var ti=AssetImporter.GetAtPath(file) as TextureImporter;if(ti==null)throw new InvalidOperationException(file);
                LockedDesignImporter.ConfigureShopTexture(ti);
                string name=Path.GetFileNameWithoutExtension(file);
                if(name=="frame")ti.spriteBorder=new Vector4(64,64,64,64);
                if(name=="frame_r3"||name=="frame_ornaments_r3")ti.spriteBorder=new Vector4(40,40,40,40);
                if(name=="frame_ornaments_r5")ti.spriteBorder=new Vector4(48,48,48,48);
                if(name=="tab_r3")ti.spriteBorder=new Vector4(28,20,28,20);
                if(name=="status_r3")ti.spriteBorder=new Vector4(10,8,12,8);ti.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("Events",typeof(RectTransform));
            try
            {
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.Events);
                ProductionEventsRuntime.BuildInto(rect);var view=root.GetComponent<ProductionEventsView>();view.RefreshContent();
                if(!view.HasRequiredBindings)throw new InvalidOperationException("Events serialized bindings are incomplete.");
                PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }
            finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/EventsCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:Events\",\n  \"revision\": 9,\n  \"eventSchedule\": \"unavailable\"\n}\n");
            AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(saved==null||!saved.GetComponent<ProductionEventsView>().HasRequiredBindings)throw new InvalidOperationException("Events prefab readback failed.");
            Debug.Log("EVENTS_R9_REBUILD_PASS: serialized native page, source sprites, revision and cutover readback.");
        }
    }
}
#endif
