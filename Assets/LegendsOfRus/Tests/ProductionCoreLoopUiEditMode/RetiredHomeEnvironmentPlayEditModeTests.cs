#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class RetiredHomeEnvironmentPlayEditModeTests
    {
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        const string Archive="Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/";
        static readonly string[] Names={"home_environment_generated","home_play_button"};
        [Serializable] public sealed class Dependencies {public string name;public string[] roots,consumers;}
        [Serializable] public sealed class Built {public string mode;public string[] requested;public bool valid;}
        static void Save(string name,object record)
        {
            string folder=Environment.GetEnvironmentVariable("LOR_HOME_ENVIRONMENT_PLAY_CLEANUP_DIR");
            if(!string.IsNullOrEmpty(folder))File.WriteAllText(Path.Combine(folder,name+".json"),JsonUtility.ToJson(record,true));
        }
        static GameObject Installed(string page)=>UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab"));
        static Component View(GameObject root,string page)=>root.GetComponent(Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true));
        static Button Play(GameObject root,string page)=>(Button)View(root,page).GetType().GetField("PlayButton").GetValue(View(root,page));
        static Image Source(GameObject root,string page)=>(Image)View(root,page).GetType().GetField("SourcePlay").GetValue(View(root,page));
        static Image Legacy(GameObject root,string page)=>Play(root,page).transform.Find("ReferenceArtwork").GetComponent<Image>();
        static bool Valid(GameObject root,string page)=>(bool)View(root,page).GetType().GetProperty("HasRequiredBindings").GetValue(View(root,page));
        static void Build(GameObject root,string page)
        {
            if(page=="Home"){ProductionHomeRuntime.BuildInto((RectTransform)root.transform);return;}
            Type.GetType("LegendsOfRus.Production"+page+"Runtime, LegendsOfRus.Runtime",true).GetMethod("BuildInto").Invoke(null,new object[]{(RectTransform)root.transform});
        }
        static void Check(GameObject root,string page)
        {
            Assert.That(Valid(root,page),Is.True);var legacy=Legacy(root,page);Assert.That(legacy.sprite,Is.Null);Assert.That(legacy.enabled,Is.False);Assert.That(legacy.raycastTarget,Is.False);
            var source=Source(root,page);Assert.That(Play(root,page).targetGraphic,Is.SameAs(source));Assert.That(source.enabled&&source.gameObject.activeInHierarchy&&source.raycastTarget,Is.True);
            foreach(var i in root.GetComponentsInChildren<Image>(true))if(i.sprite!=null)
                Assert.That(AssetDatabase.GetAssetPath(i.sprite),Does.Not.StartWith(Archive));
        }
        [TestCase("home_environment_generated","f83c954371cabff0f9a3fad969811846",941,1672,2048,false)]
        [TestCase("home_play_button","bbb3a20b4740b869c94d2023f0da4929",1959,803,4096,true)]
        public void ArchivedAssetsKeepIdentityAndDistinctImportsWithoutConsumers(string name,string guid,int width,int height,int max,bool mips)
        {
            string path=Archive+name+".png";Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.Null);Assert.That(AssetDatabase.AssetPathToGUID(path),Is.EqualTo(guid));
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);Assert.That(sprite,Is.Not.Null);Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(width,height)));
            Assert.That(sprite.texture.mipmapCount,Is.EqualTo(mips?11:1));Assert.That(sprite.texture.format,Is.EqualTo(mips?TextureFormat.RGBA32:TextureFormat.RGB24));
            var ti=(TextureImporter)AssetImporter.GetAtPath(path);var android=ti.GetPlatformTextureSettings("Android");
            Assert.That(ti.mipmapEnabled,Is.EqualTo(mips));Assert.That(ti.filterMode,Is.EqualTo(mips?FilterMode.Trilinear:FilterMode.Bilinear));Assert.That(ti.isReadable,Is.False);
            Assert.That(android.overridden,Is.True);Assert.That(android.maxTextureSize,Is.EqualTo(max));Assert.That(android.format,Is.EqualTo(mips?TextureImporterFormat.RGBA32:TextureImporterFormat.RGB24));
            var extensions=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};
            var roots=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&extensions.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();
            var consumers=roots.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(path)).ToArray();Assert.That(consumers,Is.Empty);Save(name,new Dependencies{name=name,roots=roots,consumers=consumers});
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeConstructsWithoutEitherRetiredRequestAndClearsStalePlayArt(string mode)
        {
            var root=mode=="Fresh"?new GameObject("FreshHome",typeof(RectTransform)):Installed("Home");var requests=new List<string>();
            try
            {
                if(mode!="Fresh")Legacy(root,"Home").sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Archive+"home_play_button.png");
                Func<string,Sprite> load=key=>{requests.Add(key);foreach(var name in Names)Assert.That(key,Is.Not.EqualTo(ProductionHomeRuntime.ArtRoot+name));return Resources.Load<Sprite>(key);};
                if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,load);
                else {var v=root.GetComponent<ProductionHomeView>();if(mode=="LegacyMigration")v.SourcePlay=null;ProductionHomeRuntime.UpgradeReferenceForeground(v,load);}
                Check(root,"Home");Assert.That(root.GetComponent<ProductionHomeView>().Environment.sprite.name,Is.EqualTo("clean_environment"));
                Save(mode,new Built{mode=mode,requested=requests.ToArray(),valid=Valid(root,"Home")});
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Home")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void SerializedAndFreshPagesHaveNoRetiredDependencies(string page)
        {
            foreach(var name in Names)Assert.That(AssetDatabase.GetDependencies(Pages+page+".prefab",true),Does.Not.Contain(Archive+name+".png"));
            var installed=Installed(page);var fresh=new GameObject("Fresh"+page,typeof(RectTransform));
            try{Check(installed,page);Build(fresh,page);Check(fresh,page);}
            finally{UnityEngine.Object.DestroyImmediate(installed);UnityEngine.Object.DestroyImmediate(fresh);}
        }
        [TestCase("Home")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void RepeatedInstalledBuilderClearsStaleArtAndKeepsObjectsAndTarget(string page)
        {
            var root=Installed(page);
            try
            {
                var components=root.GetComponentsInChildren<Component>(true);var button=Play(root,page);var target=button.targetGraphic;var legacy=Legacy(root,page);
                var sample=AssetDatabase.LoadAssetAtPath<Sprite>(Archive+"home_play_button.png");Assert.That(sample,Is.Not.Null);
                for(int repeat=0;repeat<2;repeat++){legacy.sprite=sample;Build(root,page);Check(root,page);Assert.That(button.targetGraphic,Is.SameAs(target));}
                CollectionAssert.AreEqual(components,root.GetComponentsInChildren<Component>(true));Assert.That(Play(root,page),Is.SameAs(button));Assert.That(Legacy(root,page),Is.SameAs(legacy));
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void LoadingKeysAndApprovedSplashExcludeRetiredResources()
        {
            foreach(var name in Names)Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(ProductionHomeRuntime.ArtRoot+name));
            foreach(var page in new[]{"SplashTitle","Loading"})foreach(var name in Names)Assert.That(AssetDatabase.GetDependencies(Pages+page+".prefab",true),Does.Not.Contain(Archive+name+".png"));
        }
    }
}
#endif
