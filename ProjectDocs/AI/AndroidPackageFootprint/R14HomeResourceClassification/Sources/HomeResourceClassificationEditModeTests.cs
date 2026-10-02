#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class HomeResourceClassificationEditModeTests
    {
        const string Art="Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/";
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        static string[] Names => (string[])typeof(ProductionHomeRuntime).GetField("TextureNames",BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
        [Serializable] public sealed class Observation {public string resource,objectPath,slot;public bool enabled,active,activeWithinRoot,targetGraphic;public float alpha,ancestorGroupAlpha,width,height;}
        [Serializable] public sealed class PageRecord {public string page,mode;public bool valid;public string[] requested;public Observation[] images;}
        [Serializable] public sealed class Inventory {public string name,guid,textureFormat;public int width,height,mips,androidMax,androidFormat;public string[] roots,consumers;public bool freshDenialConfirmed,legacyDenialConfirmed;}
        [Serializable] public sealed class Prefabs {public string[] roots;public PageRecord[] pages;}
        static string Folder=>Environment.GetEnvironmentVariable("LOR_HOME_RESOURCE_CLASSIFICATION_DIR");
        static void Save(string name,object record){Assert.That(Folder,Is.Not.Null.And.Not.Empty);File.WriteAllText(Path.Combine(Folder,name+".json"),JsonUtility.ToJson(record,true));}
        static string[] roots;
        static Dictionary<string,string[]> dependencies;
        [OneTimeSetUp] public void Prepare()
        {
            var ext=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};
            roots=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&ext.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();
            dependencies=roots.ToDictionary(p=>p,p=>AssetDatabase.GetDependencies(p,true));
            Assert.That(Names.Length,Is.EqualTo(21));
        }
        static GameObject Installed(string page)=>UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab"));
        static string ObjectPath(Transform t,Transform root)=>t==root?root.name:ObjectPath(t.parent,root)+"/"+t.name;
        static bool ActiveWithinRoot(Transform t,Transform root)
        {for(var current=t;current!=null;current=current.parent){if(!current.gameObject.activeSelf)return false;if(current==root)return true;}return false;}
        static Observation[] Observe(GameObject root)
        {
            var result=new List<Observation>();
            foreach(var image in root.GetComponentsInChildren<Image>(true))
            foreach(var slot in new[]{"sprite","overrideSprite"})
            {
                var sprite=slot=="sprite"?image.sprite:image.overrideSprite;if(sprite==null)continue;
                var path=AssetDatabase.GetAssetPath(sprite);if(!path.StartsWith(Art,StringComparison.Ordinal))continue;
                var resource=Path.GetFileNameWithoutExtension(path);if(!Names.Contains(resource))continue;
                float alpha=1;foreach(var group in image.GetComponentsInParent<CanvasGroup>(true)){alpha*=group.alpha;if(group.ignoreParentGroups)break;}
                result.Add(new Observation{resource=resource,objectPath=ObjectPath(image.transform,root.transform),slot=slot,enabled=image.enabled,active=image.gameObject.activeInHierarchy,activeWithinRoot=ActiveWithinRoot(image.transform,root.transform),alpha=image.color.a,ancestorGroupAlpha=alpha,width=image.rectTransform.rect.width,height=image.rectTransform.rect.height,targetGraphic=root.GetComponentsInChildren<Selectable>(true).Any(s=>s.targetGraphic==image)});
            }
            return result.ToArray();
        }
        static bool Valid(GameObject root,string page)
        {
            var type=Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true);var view=root.GetComponent(type);Assert.That(view,Is.Not.Null);
            return (bool)type.GetProperty("HasRequiredBindings").GetValue(view);
        }
        static void Build(GameObject root,string page)
        {
            var type=Type.GetType("LegendsOfRus.Production"+page+"Runtime, LegendsOfRus.Runtime",true);
            var method=type.GetMethod("BuildInto");var args=method.GetParameters().Select(p=>p.IsOptional?Type.Missing:(object)(RectTransform)root.transform).ToArray();method.Invoke(null,args);
        }
        [TestCase("nav_map_generated")] [TestCase("nav_shop_generated")] [TestCase("nav_events_generated")] [TestCase("nav_characters_generated")]
        [TestCase("mail_medallion_generated")] [TestCase("reward_medallion_generated")] [TestCase("home_logo")] [TestCase("avatar_portrait")]
        [TestCase("hud_frame")] [TestCase("avatar_ring")] [TestCase("resource_capsule")] [TestCase("quest_panel")]
        [TestCase("coin_icon")] [TestCase("gems_icon")] [TestCase("energy_icon")] [TestCase("settings_medallion")]
        [TestCase("resource_plus")] [TestCase("quest_reward_frame")] [TestCase("quest_reward_gem")] [TestCase("footer_base_generated")] [TestCase("home_map_foliage")]
        public void CandidateInventoryAndMissingKeyProbes(string name)
        {
            var path=Art+name+".png";var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);Assert.That(sprite,Is.Not.Null);var ti=(TextureImporter)AssetImporter.GetAtPath(path);var android=ti.GetPlatformTextureSettings("Android");
            Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.SameAs(sprite));
            foreach(var mode in new[]{"Fresh","Legacy"})
            {
                var root=mode=="Fresh"?new GameObject("DeniedHome",typeof(RectTransform)):Installed("Home");bool denied=false;
                try
                {
                    Func<string,Sprite> load=key=>{if(key==ProductionHomeRuntime.ArtRoot+name){denied=true;return null;}return Resources.Load<Sprite>(key);};
                    InvalidOperationException error;
                    if(mode=="Fresh")error=Assert.Throws<InvalidOperationException>(()=>ProductionHomeRuntime.BuildInto((RectTransform)root.transform,load));
                    else{var view=root.GetComponent<ProductionHomeView>();view.SourcePlay=null;error=Assert.Throws<InvalidOperationException>(()=>ProductionHomeRuntime.UpgradeReferenceForeground(view,load));}
                    Assert.That(denied,Is.True);Assert.That(error.Message,Does.Contain(name));
                }
                finally{UnityEngine.Object.DestroyImmediate(root);}
            }
            Save(name,new Inventory{name=name,guid=AssetDatabase.AssetPathToGUID(path),textureFormat=sprite.texture.format.ToString(),width=sprite.texture.width,height=sprite.texture.height,mips=sprite.texture.mipmapCount,androidMax=android.maxTextureSize,androidFormat=(int)android.format,roots=roots,consumers=roots.Where(p=>dependencies[p].Contains(path)).ToArray(),freshDenialConfirmed=true,legacyDenialConfirmed=true});
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeCompletedConstruction(string mode)
        {
            var root=mode=="Fresh"?new GameObject("FreshHome",typeof(RectTransform)):Installed("Home");var requests=new List<string>();
            try
            {
                Func<string,Sprite> load=key=>{requests.Add(key);return Resources.Load<Sprite>(key);};
                if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,load);
                else{var view=root.GetComponent<ProductionHomeView>();if(mode=="LegacyMigration")view.SourcePlay=null;ProductionHomeRuntime.UpgradeReferenceForeground(view,load);}
                Assert.That(Valid(root,"Home"),Is.True);
                Assert.That(requests.Count,Is.EqualTo(mode=="CurrentMigration"?0:21));
                Save(mode,new PageRecord{page="Home",mode=mode,valid=true,requested=requests.ToArray(),images=Observe(root)});
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("WorldMap")] [TestCase("ChapterMap")] [TestCase("PreLevel")] [TestCase("Gameplay")] [TestCase("Fail")]
        [TestCase("Shop")] [TestCase("SpecialOffer")] [TestCase("DailyReward")] [TestCase("Login")] [TestCase("Victory")]
        public void FreshAndInstalledSharedConsumers(string page)
        {
            var fresh=new GameObject("Fresh"+page,typeof(RectTransform));var installed=Installed(page);
            try
            {
                Build(fresh,page);Assert.That(Valid(fresh,page),Is.True);Assert.That(Valid(installed,page),Is.True);
                Save("Fresh"+page,new PageRecord{page=page,mode="Fresh",valid=true,images=Observe(fresh)});
                Save("Installed"+page,new PageRecord{page=page,mode="Installed",valid=true,images=Observe(installed)});
            }
            finally{UnityEngine.Object.DestroyImmediate(fresh);UnityEngine.Object.DestroyImmediate(installed);}
        }
        [Test] public void WorldMapDormantFooterClearingPreservesInstalledOwner()
        {
            var root=Installed("WorldMap");
            try
            {
                var view=root.GetComponent<ProductionWorldMapView>();var image=root.transform.Find(ProductionWorldMapRuntime.ContentName+"/FooterOrnament").GetComponent<Image>();
                Assert.That(image,Is.Not.SameAs(view.FooterOrnament));
                Assert.That(AssetDatabase.GetAssetPath(image.sprite),Is.EqualTo(Art+"footer_base_generated.png"));Assert.That(image.enabled,Is.False);Assert.That(image.gameObject.activeSelf,Is.False);
                var components=root.GetComponentsInChildren<Component>(true);var targets=root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray();
                image.sprite=null;Build(root,"WorldMap");Assert.That(image.sprite,Is.Null);Assert.That(Valid(root,"WorldMap"),Is.True);
                CollectionAssert.AreEqual(components,root.GetComponentsInChildren<Component>(true));CollectionAssert.AreEqual(targets,root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray());
                Save("WorldMapFooterClear",new PageRecord{page="WorldMap",mode="DisposableSpriteClear",valid=true,images=Observe(root)});
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void HomeDormantSpriteClearingPreservesCurrentMigration()
        {
            var root=Installed("Home");
            try
            {
                var images=root.GetComponentsInChildren<Image>(true).Where(i=>i.sprite!=null&&AssetDatabase.GetAssetPath(i.sprite).StartsWith(Art,StringComparison.Ordinal)).ToArray();
                // Home's completed composition has only disabled old sprites in this folder.
                foreach(var image in images){Assert.That(image.enabled,Is.False);image.sprite=null;}
                var components=root.GetComponentsInChildren<Component>(true);var targets=root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray();
                Build(root,"Home");ProductionHomeRuntime.UpgradeReferenceForeground(root.GetComponent<ProductionHomeView>(),Resources.Load<Sprite>);
                Assert.That(Valid(root,"Home"),Is.True);foreach(var image in images)Assert.That(image.sprite,Is.Null);
                CollectionAssert.AreEqual(components,root.GetComponentsInChildren<Component>(true));CollectionAssert.AreEqual(targets,root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray());
                Save("HomeDormantClear",new PageRecord{page="Home",mode="DisposableSpriteClear",valid=true,images=Observe(root)});
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void SerializedImageInventoryAcrossEveryPrefab()
        {
            var pages=new List<PageRecord>();
            foreach(var path in roots.Where(p=>p.EndsWith(".prefab",StringComparison.Ordinal)))
            {
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(prefab!=null)pages.Add(new PageRecord{page=path,mode="PrefabAsset",images=Observe(prefab)});
            }
            Assert.That(pages.Count,Is.GreaterThan(20));Save("PrefabImages",new Prefabs{roots=roots,pages=pages.ToArray()});
        }
    }
}
#endif
