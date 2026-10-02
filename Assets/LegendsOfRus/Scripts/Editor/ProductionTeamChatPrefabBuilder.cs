#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionTeamChatPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamChat.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Team Chat Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Team Chat.");
            AssetDatabase.Refresh();
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/TeamChatAligned","*.png")){
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=Path.GetFileNameWithoutExtension(file)=="chat_surround_r2"?4096:2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;
                importer.spriteBorder=Path.GetFileNameWithoutExtension(file)=="chat_surround_r2"?new Vector4(60,28,64,32):(Path.GetFileNameWithoutExtension(file).EndsWith("_frame")||Path.GetFileNameWithoutExtension(file)=="message_frame_r3")?new Vector4(66,54,66,54):Vector4.zero;importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("TeamChat",typeof(RectTransform));
            try{
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.TeamChat);
                var oldView=root.GetComponent<ProductionTeamChatView>();
                if(oldView){var oldLayout=root.GetComponent<ProductionTeamChatLayout>();if(oldLayout)UnityEngine.Object.DestroyImmediate(oldLayout);if(oldView.Content)UnityEngine.Object.DestroyImmediate(oldView.Content.gameObject);UnityEngine.Object.DestroyImmediate(oldView);}
                ProductionTeamChatRuntime.BuildInto(rect);var view=root.GetComponent<ProductionTeamChatView>();view.RefreshContent();if(!view.HasRequiredBindings)throw new InvalidOperationException("Team Chat bindings incomplete.");PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/TeamChatCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:TeamChat\",\n  \"revision\": "+ProductionTeamChatView.CurrentRevision+",\n  \"chatService\": \"unavailable\"\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(!saved||!saved.GetComponent<ProductionTeamChatView>().HasRequiredBindings)throw new InvalidOperationException("Team Chat prefab readback failed.");Debug.Log("TEAM_CHAT_RESPONSIVE_AUDIT_R5_PASS: native page and cutover readback.");
        }
    }
}
#endif
