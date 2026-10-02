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
    // Keep archived art out of player resources; builders operate on disposable instances.
    public sealed class RetiredHomeHeroEditModeTests
    {
        const string Candidate="Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/home_hero_firebird.png";
        const string Key="ProductionUI/HomeAligned/home_hero_firebird";
        const string Home="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab";
        static string Output=>Environment.GetEnvironmentVariable("LOR_HOME_HERO_ARCHIVE_DIR");
        [Serializable] public sealed class Dependencies { public string[] owners,consumers; }
        [Serializable] public sealed class BuildRecord { public string mode;public string[] requested;public bool valid,firebirdInactive,firebirdEmpty;public string currentHero; }
        [Serializable] public sealed class Imported { public int width,height,mips;public string format,guid; }
        [Test] public void ArchivedCandidateKeepsImportsAndHasNoSerializedConsumers()
        {
            Assert.That(Resources.Load<Sprite>(Key),Is.Null);
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Candidate);Assert.That(sprite,Is.Not.Null);
            Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(1254,1254)));
            Assert.That(sprite.texture.mipmapCount,Is.EqualTo(11));
            Assert.That(sprite.texture.format,Is.EqualTo(TextureFormat.RGBA32));
            var importer=(TextureImporter)AssetImporter.GetAtPath(Candidate);
            var android=importer.GetPlatformTextureSettings("Android");
            Assert.That(importer.mipmapEnabled,Is.True);
            Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Trilinear));
            Assert.That(importer.isReadable,Is.False);
            Assert.That(importer.spriteBorder,Is.EqualTo(Vector4.zero));
            Assert.That(android.overridden,Is.True);
            Assert.That(android.format,Is.EqualTo(TextureImporterFormat.RGBA32));
            Assert.That(android.maxTextureSize,Is.EqualTo(4096));
            var record=new Imported{width=sprite.texture.width,height=sprite.texture.height,mips=sprite.texture.mipmapCount,
                format=sprite.texture.format.ToString(),guid=AssetDatabase.AssetPathToGUID(Candidate)};
            Assert.That(record.guid,Is.EqualTo("7b33517046ee539d4a86ecfd63012ea5"));
            if(!string.IsNullOrEmpty(Output))File.WriteAllText(Path.Combine(Output,"Imported.json"),JsonUtility.ToJson(record,true));
            var extensions=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};
            var owners=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&extensions.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();
            var consumers=owners.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(Candidate)).ToArray();
            if(!string.IsNullOrEmpty(Output))File.WriteAllText(Path.Combine(Output,"Dependencies.json"),JsonUtility.ToJson(new Dependencies{owners=owners,consumers=consumers},true));
            Assert.That(owners.Length,Is.GreaterThan(90));Assert.That(consumers,Is.Empty);
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeConstructionDoesNotRequestCandidate(string mode)
        {
            var requests=new List<string>();
            Func<string,Sprite> loader=key=>{requests.Add(key);Assert.That(key,Is.Not.EqualTo(Key));return key==Key?null:Resources.Load<Sprite>(key);};
            var root=mode=="Fresh"?new GameObject("AuditHome",typeof(RectTransform)):UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Home));
            try
            {
                if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,loader);
                else
                {
                    var old=root.GetComponent<ProductionHomeView>();
                    if(mode=="LegacyMigration")old.SourcePlay=null;
                    ProductionHomeRuntime.UpgradeReferenceForeground(old,loader);
                }
                var view=root.GetComponent<ProductionHomeView>();
                var record=new BuildRecord{mode=mode,requested=requests.ToArray(),valid=view.HasRequiredBindings,
                    firebirdInactive=!view.Firebird.gameObject.activeSelf,firebirdEmpty=view.Firebird.sprite==null,
                    currentHero=AssetDatabase.GetAssetPath(view.Ivan.sprite)};
                if(!string.IsNullOrEmpty(Output))File.WriteAllText(Path.Combine(Output,mode+".json"),JsonUtility.ToJson(record,true));
                Assert.That(record.valid&&record.firebirdInactive&&record.firebirdEmpty,Is.True);
                Assert.That(view.HasReferenceForegroundBindings,Is.True);
                Assert.That(record.currentHero,Is.EqualTo("Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/source_hero.png"));
                Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(Key));
                Assert.That(root.GetComponentsInChildren<Image>(true).Any(i=>i.sprite!=null&&AssetDatabase.GetAssetPath(i.sprite)==Candidate),Is.False);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void ApprovedSplashAndLoadingUseOtherResources()
        {
            foreach(string page in new[]{"Home","SplashTitle","Loading"})
            {
                string path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+page+".prefab";
                var dependencies=AssetDatabase.GetDependencies(path,true);
                Assert.That(dependencies,Does.Not.Contain(Candidate),page);
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Assert.That(prefab,Is.Not.Null);
                Assert.That(prefab.GetComponentsInChildren<Image>(true).Any(i=>i.sprite!=null),Is.True);
            }
            var home=AssetDatabase.LoadAssetAtPath<GameObject>(Home).GetComponent<ProductionHomeView>();
            Assert.That(home.HasRequiredBindings,Is.True);Assert.That(home.Firebird.sprite,Is.Null);
        }
        [TestCase("Fresh")] [TestCase("Installed")]
        public void LoginKeepsTheSharedLogo(string mode)
        {
            var root=mode=="Fresh"?new GameObject("FreshLogin",typeof(RectTransform)):UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LegendsOfRus/Resources/ProductionUI/Pages/Login.prefab"));
            try
            {
                ProductionLoginRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponent<ProductionLoginView>().HasRequiredBindings,Is.True);
                var logo=root.GetComponentsInChildren<Image>(true).Single(i=>i.name=="ApprovedLogo");
                Assert.That(logo.sprite,Is.SameAs(Resources.Load<Sprite>("ProductionUI/HomeAligned/home_logo")));
                Assert.That(logo.enabled&&logo.gameObject.activeInHierarchy,Is.True);
                Assert.That(logo.color.a,Is.EqualTo(1f));
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void RepeatedBuildKeepsInstalledComponentsAndCurrentHero()
        {
            var root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Home));
            try
            {
                var components=root.GetComponentsInChildren<Component>(true);var v=root.GetComponent<ProductionHomeView>();var image=v.Ivan;var sprite=image.sprite;
                Func<string,Sprite> denied=key=>{Assert.Fail("Valid installed Home should not request legacy sprites: "+key);return null;};
                ProductionHomeRuntime.BuildInto((RectTransform)root.transform,denied);
                ProductionHomeRuntime.BuildInto((RectTransform)root.transform,denied);
                CollectionAssert.AreEqual(components,root.GetComponentsInChildren<Component>(true));
                Assert.That(v.Ivan,Is.SameAs(image));Assert.That(v.Ivan.sprite,Is.SameAs(sprite));Assert.That(v.HasRequiredBindings,Is.True);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void ForegroundDiagnosticRejectsWrongCurrentHero()
        {
            var root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Home));
            try
            {
                var v=root.GetComponent<ProductionHomeView>();Assert.That(v.HasReferenceForegroundBindings,Is.True);
                v.Ivan.sprite=Resources.Load<Sprite>("ProductionUI/HomeAligned/home_logo");
                Assert.That(v.HasReferenceForegroundBindings,Is.False);Assert.That(v.HasRequiredBindings,Is.False);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }

    }
}
#endif
