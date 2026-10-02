#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionLanguageSelectPrefabBuilder
    {
        public const string PrefabPath = "Assets/LegendsOfRus/Resources/ProductionUI/Pages/LanguageSelect.prefab";
        [MenuItem("Legends of Rus/Production UI/Rebuild Language Selection")]
        public static void Rebuild()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Close Play Mode before rebuilding Language Select.");
            AssetDatabase.Refresh(); bool existing = File.Exists(PrefabPath);
            var root = existing ? PrefabUtility.LoadPrefabContents(PrefabPath) : new GameObject("LanguageSelect", typeof(RectTransform));
            try
            {
                root.layer = 5; var r = (RectTransform)root.transform;
                r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero;
                var marker = root.GetComponent<ProductionCoreLoopPageMarker>() ?? root.AddComponent<ProductionCoreLoopPageMarker>(); marker.Configure(ScreenId.LanguageSelect);
                var old=root.GetComponent<ProductionLanguageSelectView>();
                if(old){
                    var layout=root.GetComponent<ProductionLanguageSelectLayout>();if(layout)UnityEngine.Object.DestroyImmediate(layout);
                    if(old.Body)UnityEngine.Object.DestroyImmediate(old.Body.gameObject);
                    if(old.Shield)UnityEngine.Object.DestroyImmediate(old.Shield.gameObject);
                    if(old.Backdrop)UnityEngine.Object.DestroyImmediate(old.Backdrop.gameObject);
                    UnityEngine.Object.DestroyImmediate(old);
                }
                ProductionLanguageSelectRuntime.BuildInto(r);
                if (!root.GetComponent<ProductionLanguageSelectView>().HasRequiredBindings) throw new InvalidOperationException("Incomplete Language Select.");
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally { if (existing) PrefabUtility.UnloadPrefabContents(root); else UnityEngine.Object.DestroyImmediate(root); }
            string state = "Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/LanguageSelectCutoverState.txt";
            File.WriteAllText(state, "{\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:LanguageSelect\",\n  \"revision\": 3\n}\n");
            AssetDatabase.ImportAsset(state); AssetDatabase.SaveAssets();
            var saved = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (!saved.GetComponent<ProductionLanguageSelectView>().HasRequiredBindings) throw new InvalidOperationException("Language Select readback failed.");
            Debug.Log("LANGUAGE_SELECT_RESPONSIVE_R2_REBUILD_PASS");
        }
    }
}
#endif
