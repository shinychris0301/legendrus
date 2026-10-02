#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionCharactersPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Characters.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Characters Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Characters.");
            AssetDatabase.Refresh();
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/CharactersAligned","*.png")){
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;
                importer.spriteBorder=(Path.GetFileNameWithoutExtension(file).EndsWith("_frame")||Path.GetFileNameWithoutExtension(file)=="detail_frame_r2"||Path.GetFileNameWithoutExtension(file)=="detail_frame_r4"||Path.GetFileNameWithoutExtension(file)=="card_border_r2"||Path.GetFileNameWithoutExtension(file)=="card_normal_r6"||Path.GetFileNameWithoutExtension(file)=="portrait_frame_r7")?new Vector4(22,22,22,22):Vector4.zero;importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("Characters",typeof(RectTransform));
            try{
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.Characters);
                var oldView=root.GetComponent<ProductionCharactersView>();
                if(oldView){var oldLayout=root.GetComponent<ProductionCharactersLayout>();if(oldLayout)UnityEngine.Object.DestroyImmediate(oldLayout);if(oldView.Content)UnityEngine.Object.DestroyImmediate(oldView.Content.gameObject);UnityEngine.Object.DestroyImmediate(oldView);}
                ProductionCharactersRuntime.BuildInto(rect);var view=root.GetComponent<ProductionCharactersView>();view.RefreshContent();if(!view.HasRequiredBindings)throw new InvalidOperationException("Characters bindings incomplete.");PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/CharactersCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:Characters\",\n  \"revision\": 16,\n  \"heroOwnershipService\": \"unavailable\"\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(!saved||!saved.GetComponent<ProductionCharactersView>().HasRequiredBindings)throw new InvalidOperationException("Characters prefab readback failed.");Debug.Log("CHARACTERS_RESPONSIVE_R4_REBUILD_PASS: native page and cutover readback.");
        }
    }
}
#endif
