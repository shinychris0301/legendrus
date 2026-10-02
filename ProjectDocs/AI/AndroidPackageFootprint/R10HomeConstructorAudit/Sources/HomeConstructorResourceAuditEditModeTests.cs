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
    // Temporary read-only probe: only disposable instances are constructed or changed.
    public sealed class HomeConstructorResourceAuditEditModeTests
    {
        const string Folder="Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/";
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        static readonly string[] Names={"home_logo","home_hero_firebird"};
        [Serializable] public sealed class Candidate { public string name,guid,format;public int width,height,mips;public string[] owners,consumers; }
        [Serializable] public sealed class Construction { public string mode,logo,hero;public bool valid;public string[] requested; }
        [Serializable] public sealed class Rejection { public string mode,key,message;public string[] requested; }
        [Serializable] public sealed class LoginRecord { public string mode,sprite;public bool valid,enabled,active;public float alpha; }
        static void Save(string name,object value)=>File.WriteAllText(Path.Combine(Environment.GetEnvironmentVariable("LOR_HOME_CONSTRUCTOR_AUDIT_DIR"),name+".json"),JsonUtility.ToJson(value,true));
        static GameObject Home(string mode)=>mode=="Fresh"?new GameObject("AuditHome",typeof(RectTransform)):UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+"Home.prefab"));
        static void Build(GameObject root,string mode,Func<string,Sprite> loader)
        {
            if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,loader);
            else {
                var view=root.GetComponent<ProductionHomeView>();
                if(mode=="LegacyMigration")view.SourcePlay=null;
                ProductionHomeRuntime.UpgradeReferenceForeground(view,loader);
            }
        }
        [TestCase("home_logo","65814c4f46e84f2869f51de2ad998f46")]
        [TestCase("home_hero_firebird","7b33517046ee539d4a86ecfd63012ea5")]
        public void ImportedCandidatesHaveDifferentDependencyOwners(string name,string guid)
        {
            string path=Folder+name+".png";
            var sprite=Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name);Assert.That(sprite,Is.Not.Null);
            Assert.That(AssetDatabase.AssetPathToGUID(path),Is.EqualTo(guid));
            var extensions=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};
            var roots=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&extensions.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();
            var consumers=roots.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(path)).ToArray();
            Save(name,new Candidate{name=name,guid=guid,width=sprite.texture.width,height=sprite.texture.height,mips=sprite.texture.mipmapCount,format=sprite.texture.format.ToString(),owners=roots,consumers=consumers});
            Assert.That(roots.Length,Is.GreaterThan(90));
            CollectionAssert.AreEqual(name=="home_logo"?new[]{Pages+"Login.prefab"}:Array.Empty<string>(),consumers);
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void CompletedHomeUsesCurrentArtButLegacyPathsStillLoadBoth(string mode)
        {
            var root=Home(mode);var requests=new List<string>();
            try {
                Build(root,mode,key=>{requests.Add(key);return Resources.Load<Sprite>(key);});
                var v=root.GetComponent<ProductionHomeView>();
                var record=new Construction{mode=mode,logo=AssetDatabase.GetAssetPath(v.Logo.sprite),hero=AssetDatabase.GetAssetPath(v.Ivan.sprite),valid=v.HasRequiredBindings,requested=requests.ToArray()};Save(mode,record);
                Assert.That(v.HasRequiredBindings,Is.True);
                Assert.That(record.logo,Is.EqualTo("Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/source_logo.png"));
                Assert.That(record.hero,Is.EqualTo("Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/source_hero.png"));
                foreach(string name in Names){string key=ProductionHomeRuntime.ArtRoot+name;
                    Assert.That(requests.Count(k=>k==key),Is.EqualTo(mode=="CurrentMigration"?0:1));
                    Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(key));
                    Assert.That(root.GetComponentsInChildren<Image>(true).Any(i=>i.sprite!=null&&AssetDatabase.GetAssetPath(i.sprite)==Folder+name+".png"),Is.False);
                }
            } finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Fresh","home_logo")] [TestCase("Fresh","home_hero_firebird")]
        [TestCase("LegacyMigration","home_logo")] [TestCase("LegacyMigration","home_hero_firebird")]
        public void MissingCandidateStillBlocksLegacyConstruction(string mode,string name)
        {
            var root=Home(mode);var requests=new List<string>();string denied=ProductionHomeRuntime.ArtRoot+name;
            try {
                var error=Assert.Throws<InvalidOperationException>(()=>Build(root,mode,key=>{requests.Add(key);return key==denied?null:Resources.Load<Sprite>(key);}));
                Assert.That(error.Message,Is.EqualTo("Home requires a standalone imported Sprite: "+denied));
                Assert.That(requests,Does.Contain(denied));Save(mode+"_Missing_"+name,new Rejection{mode=mode,key=denied,message=error.Message,requested=requests.ToArray()});
            } finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void InstalledHomeBuildDoesNotRequestLegacyAssets()
        {
            var root=Home("Installed");
            try {ProductionHomeRuntime.BuildInto((RectTransform)root.transform,key=>{Assert.Fail("Unexpected load: "+key);return null;});Assert.That(root.GetComponent<ProductionHomeView>().HasRequiredBindings,Is.True);}
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Fresh")] [TestCase("Installed")]
        public void LoginRetainsTheCandidateAsVisibleArtwork(string mode)
        {
            var root=mode=="Fresh"?new GameObject("AuditLogin",typeof(RectTransform)):UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+"Login.prefab"));
            try {
                ProductionLoginRuntime.BuildInto((RectTransform)root.transform);
                var view=root.GetComponent<ProductionLoginView>();var logo=root.GetComponentsInChildren<Image>(true).Single(i=>i.name=="ApprovedLogo");
                var record=new LoginRecord{mode=mode,sprite=AssetDatabase.GetAssetPath(logo.sprite),valid=view.HasRequiredBindings,enabled=logo.enabled,active=logo.gameObject.activeInHierarchy,alpha=logo.color.a};Save("Login_"+mode,record);
                Assert.That(record.sprite,Is.EqualTo(Folder+"home_logo.png"));Assert.That(record.valid&&record.enabled&&record.active,Is.True);Assert.That(record.alpha,Is.EqualTo(1f));
            } finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void InstalledHomeSplashAndLoadingHaveNeitherDependency()
        {
            foreach(string page in new[]{"Home","SplashTitle","Loading"})
                foreach(string name in Names)Assert.That(AssetDatabase.GetDependencies(Pages+page+".prefab",true),Does.Not.Contain(Folder+name+".png"),page);
        }
    }
}
#endif
