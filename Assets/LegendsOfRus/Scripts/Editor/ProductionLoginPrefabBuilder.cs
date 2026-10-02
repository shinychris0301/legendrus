#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionLoginPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Login.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Login Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Login.");
            AssetDatabase.Refresh();
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/LoginAligned","*.png")){
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;
                importer.spriteBorder=Path.GetFileNameWithoutExtension(file).EndsWith("_frame")?new Vector4(54,54,54,54):Vector4.zero;importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("Login",typeof(RectTransform));
            try{
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.Login);
                // Rebuild the owned hierarchy, preserving the prefab root and GUID.
                var old=root.GetComponent<ProductionLoginView>();
                if(old){var layout=root.GetComponent<ProductionLoginLayout>();if(layout)UnityEngine.Object.DestroyImmediate(layout);if(old.Content)UnityEngine.Object.DestroyImmediate(old.Content.gameObject);UnityEngine.Object.DestroyImmediate(old);}
                ProductionLoginRuntime.BuildInto(rect);var view=root.GetComponent<ProductionLoginView>();view.RefreshContent();if(!view.HasRequiredBindings)throw new InvalidOperationException("Login bindings incomplete.");PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/LoginCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:Login\",\n  \"revision\": 3,\n  \"authenticationService\": \"unavailable\"\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(!saved||!saved.GetComponent<ProductionLoginView>().HasRequiredBindings)throw new InvalidOperationException("Login prefab readback failed.");Debug.Log("LOGIN_RESPONSIVE_R2_REBUILD_PASS: native page and cutover readback.");
        }
    }
}
#endif
