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
    // Temporary read-only fixture: all instance modifications are discarded.
    public sealed class HomeEnvironmentPlayAuditEditModeTests
    {
        const string Folder="Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/";
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        const string Play="home_play_button";
        static readonly string[] Names={"home_environment_generated",Play};
        [Serializable] public sealed class Candidate {public string name,guid,format;public int width,height,mips;public string[] roots,consumers;}
        [Serializable] public sealed class Observation {public string path,sprite;public bool enabled,active,target,raycast;}
        [Serializable] public sealed class Built {public string page,mode,environment,currentPlay;public bool valid;public string[] requested;public Observation[] retained;}
        [Serializable] public sealed class Missing {public string mode,key,message;public string[] requested;}
        [Serializable] public sealed class Cleared {public string page;public int images;public bool validBefore,validAfter,identitiesRetained,stillClearedAfterBuilder;}
        static void Save(string name,object record)=>File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("LOR_HOME_ENVIRONMENT_PLAY_AUDIT_DIR"),name+".json"),JsonUtility.ToJson(record,true));
        static GameObject Instance(string page)=>UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab"));
        static GameObject Home(string mode)=>mode=="Fresh"?new GameObject("AuditHome",typeof(RectTransform)):Instance("Home");
        static void HomeBuild(GameObject root,string mode,Func<string,Sprite> loader)
        {
            if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,loader);
            else{var v=root.GetComponent<ProductionHomeView>();if(mode=="LegacyMigration")v.SourcePlay=null;ProductionHomeRuntime.UpgradeReferenceForeground(v,loader);}
        }
        static Component View(GameObject root,string page)=>root.GetComponent(Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true));
        static bool Valid(GameObject root,string page)=>(bool)View(root,page).GetType().GetProperty("HasRequiredBindings").GetValue(View(root,page));
        static Button ButtonOf(GameObject root,string page)=>(Button)View(root,page).GetType().GetField("PlayButton").GetValue(View(root,page));
        static Image SourceOf(GameObject root,string page)=>(Image)View(root,page).GetType().GetField("SourcePlay").GetValue(View(root,page));
        static void BuildPage(GameObject root,string page)
        {
            if(page=="Home"){ProductionHomeRuntime.BuildInto((RectTransform)root.transform);return;}
            Type.GetType("LegendsOfRus.Production"+page+"Runtime, LegendsOfRus.Runtime",true).GetMethod("BuildInto").Invoke(null,new object[]{(RectTransform)root.transform});
        }
        static string PathOf(Transform child,Transform root){var parts=new List<string>();for(var t=child;t!=root;t=t.parent)parts.Add(t.name);parts.Reverse();return string.Join("/",parts);}
        static Image[] Retained(GameObject root)=>root.GetComponentsInChildren<Image>(true).Where(i=>i.sprite!=null&&AssetDatabase.GetAssetPath(i.sprite)==Folder+Play+".png").ToArray();
        static Observation Observe(Image image,GameObject root)
        {
            var parent=image.GetComponentInParent<Button>(true);
            return new Observation{path=PathOf(image.transform,root.transform),sprite=AssetDatabase.GetAssetPath(image.sprite),enabled=image.enabled,active=image.gameObject.activeInHierarchy,raycast=image.raycastTarget,target=parent!=null&&parent.targetGraphic==image};
        }
        static void CheckCurrent(GameObject root,string page)
        {
            Assert.That(Valid(root,page),Is.True);var button=ButtonOf(root,page);var source=SourceOf(root,page);
            Assert.That(button.targetGraphic,Is.SameAs(source));Assert.That(source.enabled&&source.gameObject.activeInHierarchy&&source.raycastTarget,Is.True);
            Assert.That(source.sprite,Is.Not.Null);Assert.That(AssetDatabase.GetAssetPath(source.sprite),Is.Not.EqualTo(Folder+Play+".png"));
            Assert.That(root.GetComponentsInChildren<Image>(true).Any(i=>i.sprite!=null&&AssetDatabase.GetAssetPath(i.sprite)==Folder+Names[0]+".png"),Is.False);
        }
        [TestCase("home_environment_generated","f83c954371cabff0f9a3fad969811846")]
        [TestCase(Play,"bbb3a20b4740b869c94d2023f0da4929")]
        public void CandidateImportAndRecursiveDependencies(string name,string guid)
        {
            var path=Folder+name+".png";var sprite=Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name);Assert.That(sprite,Is.Not.Null);Assert.That(AssetDatabase.AssetPathToGUID(path),Is.EqualTo(guid));
            var extensions=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};
            var roots=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&extensions.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();
            var consumers=roots.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(path)).ToArray();
            Save(name,new Candidate{name=name,guid=guid,width=sprite.texture.width,height=sprite.texture.height,mips=sprite.texture.mipmapCount,format=sprite.texture.format.ToString(),roots=roots,consumers=consumers});
            Assert.That(roots.Length,Is.GreaterThan(90));CollectionAssert.AreEqual(name==Play?new[]{Pages+"ChapterMap.prefab",Pages+"Home.prefab",Pages+"PreLevel.prefab"}:Array.Empty<string>(),consumers);
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeFinishesWithCurrentArtAndRetainsDisabledPlaySprite(string mode)
        {
            var root=Home(mode);var requests=new List<string>();
            try{
                HomeBuild(root,mode,key=>{requests.Add(key);return Resources.Load<Sprite>(key);});CheckCurrent(root,"Home");var v=root.GetComponent<ProductionHomeView>();var retained=Retained(root);
                Assert.That(retained.Length,Is.EqualTo(1));foreach(var image in retained){Assert.That(image.enabled,Is.False);Assert.That(Observe(image,root).target,Is.False);}
                foreach(var name in Names){var key=ProductionHomeRuntime.ArtRoot+name;Assert.That(requests.Count(k=>k==key),Is.EqualTo(mode=="CurrentMigration"?0:1));Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(key));}
                Assert.That(AssetDatabase.GetAssetPath(v.Environment.sprite),Is.EqualTo("Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/clean_environment.png"));
                Save("Home_"+mode,new Built{page="Home",mode=mode,valid=v.HasRequiredBindings,environment=AssetDatabase.GetAssetPath(v.Environment.sprite),currentPlay=AssetDatabase.GetAssetPath(v.SourcePlay.sprite),requested=requests.ToArray(),retained=retained.Select(i=>Observe(i,root)).ToArray()});
            }finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Fresh","home_environment_generated")] [TestCase("LegacyMigration","home_environment_generated")]
        [TestCase("Fresh",Play)] [TestCase("LegacyMigration",Play)]
        public void MissingCandidateStillBlocksFreshAndLegacyHome(string mode,string name)
        {
            var root=Home(mode);var requests=new List<string>();var denied=ProductionHomeRuntime.ArtRoot+name;
            try{
                var error=Assert.Throws<InvalidOperationException>(()=>HomeBuild(root,mode,key=>{requests.Add(key);return key==denied?null:Resources.Load<Sprite>(key);}));
                Assert.That(error.Message,Is.EqualTo("Home requires a standalone imported Sprite: "+denied));Save("Missing_"+mode+"_"+name,new Missing{mode=mode,key=denied,message=error.Message,requested=requests.ToArray()});
            }finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void InstalledHomeDoesNotInvokeLegacyLoader()
        {
            var root=Instance("Home");try{ProductionHomeRuntime.BuildInto((RectTransform)root.transform,key=>{Assert.Fail("Unexpected load: "+key);return null;});CheckCurrent(root,"Home");}finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Home")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void InstalledPageRetainsExactlyOneDisabledUnusedImage(string page)
        {
            var root=Instance(page);
            try{CheckCurrent(root,page);var images=Retained(root);Assert.That(images.Length,Is.EqualTo(1));var observations=images.Select(i=>Observe(i,root)).ToArray();
                foreach(var o in observations){Assert.That(o.enabled||o.target||o.raycast,Is.False);}
                Save(page+"_Installed",new Built{page=page,mode="Installed",valid=Valid(root,page),currentPlay=AssetDatabase.GetAssetPath(SourceOf(root,page).sprite),retained=observations});
            }finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void FreshMapPageClonesTheDisabledImage(string page)
        {
            var root=new GameObject("Fresh"+page,typeof(RectTransform));
            try{BuildPage(root,page);CheckCurrent(root,page);var images=Retained(root);Assert.That(images.Length,Is.EqualTo(1));foreach(var image in images)Assert.That(image.enabled,Is.False);
                Save(page+"_Fresh",new Built{page=page,mode="Fresh",valid=Valid(root,page),currentPlay=AssetDatabase.GetAssetPath(SourceOf(root,page).sprite),retained=images.Select(i=>Observe(i,root)).ToArray()});
            }finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Home")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void SpriteOnlyClearingPreservesControlsAndStaysClearedInInstalledBuilder(string page)
        {
            var root=Instance(page);
            try{var objects=root.GetComponentsInChildren<Component>(true);var button=ButtonOf(root,page);var target=button.targetGraphic;var images=Retained(root);Assert.That(images.Length,Is.EqualTo(1));var before=Valid(root,page);
                foreach(var image in images)image.sprite=null;CheckCurrent(root,page);CollectionAssert.AreEqual(objects,root.GetComponentsInChildren<Component>(true));Assert.That(button.targetGraphic,Is.SameAs(target));
                BuildPage(root,page);CheckCurrent(root,page);Assert.That(images.All(i=>i.sprite==null),Is.True);Assert.That(ButtonOf(root,page),Is.SameAs(button));Assert.That(button.targetGraphic,Is.SameAs(target));
                Save(page+"_Cleared",new Cleared{page=page,images=images.Length,validBefore=before,validAfter=Valid(root,page),identitiesRetained=true,stillClearedAfterBuilder=true});
            }finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void SplashAndLoadingUseOtherArt()
        {
            foreach(var page in new[]{"SplashTitle","Loading"})foreach(var name in Names)Assert.That(AssetDatabase.GetDependencies(Pages+page+".prefab",true),Does.Not.Contain(Folder+name+".png"));
            var splash=AssetDatabase.LoadAssetAtPath<GameObject>(Pages+"SplashTitle.prefab").GetComponent<SplashTitleView>();Assert.That(splash.HasRequiredBindings,Is.True);Assert.That(splash.StartButton.targetGraphic,Is.Not.Null);
            var image=(Image)splash.StartButton.targetGraphic;Assert.That(AssetDatabase.GetAssetPath(image.sprite),Is.Not.EqualTo(Folder+Play+".png"));
        }
    }
}
#endif
