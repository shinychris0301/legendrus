#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionTutorialPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/TutorialOverlay.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Tutorial Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Tutorial.");
            AssetDatabase.Refresh();
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal","*.png")){
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;
                importer.spriteBorder=Path.GetFileNameWithoutExtension(file).EndsWith("_frame")?new Vector4(54,54,54,54):Vector4.zero;importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("TutorialOverlay",typeof(RectTransform));
            try{
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.TutorialOverlay);
                var existingView=root.GetComponent<ProductionTutorialView>();if(existingView)existingView.Revision=0;
                ProductionTutorialRuntime.BuildInto(rect);var view=root.GetComponent<ProductionTutorialView>();view.RefreshContent();if(!view.HasRequiredBindings)throw new InvalidOperationException("Tutorial bindings incomplete.");PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TutorialOverlayCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:TutorialOverlay\",\n  \"revision\": 7\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(!saved||!saved.GetComponent<ProductionTutorialView>().HasRequiredBindings)throw new InvalidOperationException("Tutorial prefab readback failed.");Debug.Log("TUTORIAL_RESPONSIVE_R6_REBUILD_PASS: native page and cutover readback.");
        }
    }
}
#endif
