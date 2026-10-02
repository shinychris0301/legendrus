#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionProfilePrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Profile Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Profile.");
            AssetDatabase.Refresh();
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned","*.png")){
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;
                importer.spriteBorder=Path.GetFileNameWithoutExtension(file)=="detail_r3_frame"?new Vector4(24,22,24,22):Path.GetFileNameWithoutExtension(file)=="relic_collection_r2_frame"?new Vector4(32,20,32,28):Path.GetFileNameWithoutExtension(file).EndsWith("_frame")?new Vector4(54,54,54,54):Vector4.zero;importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("Profile",typeof(RectTransform));
            try{
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.Profile);
                var oldView=root.GetComponent<ProductionProfileView>();
                if(oldView){var oldLayout=root.GetComponent<ProductionProfileLayout>();if(oldLayout)UnityEngine.Object.DestroyImmediate(oldLayout);if(oldView.Content)UnityEngine.Object.DestroyImmediate(oldView.Content.gameObject);UnityEngine.Object.DestroyImmediate(oldView);}
                ProductionProfileRuntime.BuildInto(rect);var view=root.GetComponent<ProductionProfileView>();view.RefreshContent();if(!view.HasRequiredBindings)throw new InvalidOperationException("Profile bindings incomplete.");PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:Profile\",\n  \"revision\": 13,\n  \"profileService\": \"unavailable\"\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(!saved||!saved.GetComponent<ProductionProfileView>().HasRequiredBindings)throw new InvalidOperationException("Profile prefab readback failed.");Debug.Log("PROFILE_R1_REBUILD_PASS: native page and cutover readback.");
        }
    }
}
#endif
