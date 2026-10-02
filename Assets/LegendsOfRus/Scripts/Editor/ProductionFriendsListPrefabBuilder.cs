#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionFriendsListPrefabBuilder
    {
        public const string PrefabPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/FriendsList.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Friends List Alignment")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Close Play Mode before rebuilding Friends List.");
            AssetDatabase.Refresh();
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned","*.png")){
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=Path.GetFileNameWithoutExtension(file)=="outer_frame_r2"?4096:2048;importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;
                importer.spriteBorder=Path.GetFileNameWithoutExtension(file)=="requests_surround_r4"?new Vector4(26,58,26,26):Path.GetFileNameWithoutExtension(file)=="outer_frame_r2"?new Vector4(160,300,180,200):Path.GetFileNameWithoutExtension(file).EndsWith("_frame")?new Vector4(54,54,54,54):Vector4.zero;importer.SaveAndReimport();
            }
            bool existing=File.Exists(PrefabPath);GameObject root=existing?PrefabUtility.LoadPrefabContents(PrefabPath):new GameObject("FriendsList",typeof(RectTransform));
            try{
                var rect=(RectTransform)root.transform;rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;root.layer=5;
                var marker=root.GetComponent<ProductionCoreLoopPageMarker>()??root.AddComponent<ProductionCoreLoopPageMarker>();marker.Configure(ScreenId.FriendsList);
                var oldView=root.GetComponent<ProductionFriendsListView>();
                if(oldView){var oldLayout=root.GetComponent<ProductionFriendsListLayout>();if(oldLayout)UnityEngine.Object.DestroyImmediate(oldLayout);if(oldView.Content)UnityEngine.Object.DestroyImmediate(oldView.Content.gameObject);UnityEngine.Object.DestroyImmediate(oldView);}
                ProductionFriendsListRuntime.BuildInto(rect);var view=root.GetComponent<ProductionFriendsListView>();view.RefreshContent();if(!view.HasRequiredBindings)throw new InvalidOperationException("Friends List bindings incomplete.");PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);
            }finally{if(existing)PrefabUtility.UnloadPrefabContents(root);else UnityEngine.Object.DestroyImmediate(root);}
            string state="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/FriendsListCutoverState.txt";
            File.WriteAllText(state,"{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:FriendsList\",\n  \"revision\": "+ProductionFriendsListView.CurrentRevision+",\n  \"friendsService\": \"unavailable\"\n}\n");AssetDatabase.ImportAsset(state);AssetDatabase.SaveAssets();
            var saved=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(!saved||!saved.GetComponent<ProductionFriendsListView>().HasRequiredBindings)throw new InvalidOperationException("Friends List prefab readback failed.");Debug.Log("FRIENDS_LIST_FOOTER_SYMBOLS_R7_PASS: native page and cutover readback.");
        }
    }
}
#endif
