#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class ProductionSharedBoosterArtBuilder
    {
        [MenuItem("Legends of Rus/Production UI/Align Shared Booster Badges")]
        public static void RebuildBadges()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before updating badges.");
            foreach(var name in new[]{"BoosterSelect","PreLevel","Pause","Fail","Gameplay"})
            {
                string file="Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+name+".prefab";
                var root=PrefabUtility.LoadPrefabContents(file);
                try
                {
                    foreach(var badge in root.GetComponentsInChildren<ProductionPreLevelBadge>(true))
                    {
                        var label=badge.GetComponentInChildren<UnityEngine.UI.Text>(true);
                        if(label==null&&badge.transform.parent.Find("Stock")!=null)
                            label=badge.transform.parent.Find("Stock").GetComponent<UnityEngine.UI.Text>();
                        ProductionPreLevelBadge.AlignCount(label,badge.rectTransform);
                    }
                    PrefabUtility.SaveAsPrefabAsset(root,file);
                }
                finally{PrefabUtility.UnloadPrefabContents(root);}
            }
            AssetDatabase.SaveAssets();
            Debug.Log("SHARED_BOOSTER_BADGE_ALIGNMENT_PASS");
        }
        [MenuItem("Legends of Rus/Production UI/Apply Shared Booster Art")]
        public static void Rebuild()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before updating booster prefabs.");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/SharedBoosters","*.png"))
            {var importer=(TextureImporter)AssetImporter.GetAtPath(file);LockedDesignImporter.ConfigureShopTexture(importer);importer.SaveAndReimport();}
            var changed=new List<string>();
            foreach(string file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/Pages","*.prefab"))
            {
                var root=PrefabUtility.LoadPrefabContents(file);bool dirty=false;
                try
                {
                    // Replace sprite references only; retain every existing serialized binding.
                    foreach(var component in root.GetComponentsInChildren<Component>(true))
                    {
                        if(component==null)continue;
                        var serialized=new SerializedObject(component);var property=serialized.GetIterator();bool modified=false;
                        while(property.Next(true))
                        {
                            if(property.propertyType!=SerializedPropertyType.ObjectReference)continue;
                            var sprite=property.objectReferenceValue as Sprite;if(sprite==null)continue;
                            string path=AssetDatabase.GetAssetPath(sprite);const string prefix="Assets/LegendsOfRus/Resources/";
                            if(!path.StartsWith(prefix,StringComparison.Ordinal)||!path.EndsWith(".png",StringComparison.Ordinal))continue;
                            var next=ProductionBoosterArt.Replacement(path.Substring(prefix.Length,path.Length-prefix.Length-4));
                            if(next==null||next==sprite)continue;
                            property.objectReferenceValue=next;modified=true;
                        }
                        if(modified){serialized.ApplyModifiedPropertiesWithoutUndo();dirty=true;}
                    }
                    if(root.GetComponent<ProductionBoosterSelectionView>()!=null){ProductionBoosterSelectionRuntime.BuildInto((RectTransform)root.transform);dirty=true;}
                    if(root.GetComponent<ProductionPauseView>()!=null){ProductionPauseRuntime.BuildInto((RectTransform)root.transform);dirty=true;}
                    if(root.GetComponent<ProductionFailView>()!=null){ProductionFailRuntime.BuildInto((RectTransform)root.transform);dirty=true;}
                    if(dirty){PrefabUtility.SaveAsPrefabAsset(root,file);changed.Add(file);}
                }
                finally{PrefabUtility.UnloadPrefabContents(root);}
            }
            AssetDatabase.SaveAssets();
            var report=Environment.GetEnvironmentVariable("LOR_SHARED_BOOSTER_AUDIT");
            if(!string.IsNullOrEmpty(report))File.WriteAllLines(report,changed);
            Debug.Log("SHARED_BOOSTER_ART_REBUILD_PASS "+changed.Count+" prefabs");
        }
    }
}
#endif
