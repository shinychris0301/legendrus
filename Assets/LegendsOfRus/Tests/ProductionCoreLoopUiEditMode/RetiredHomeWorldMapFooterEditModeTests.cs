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
    public sealed class RetiredHomeWorldMapFooterEditModeTests
    {
        const string Name="footer_base_generated";
        const string Archive="Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/footer_base_generated.png";
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        [Serializable] public sealed class Dependencies {public string[] roots,consumers;}
        [Serializable] public sealed class Built {public string mode;public string[] requested;public bool valid;}
        static void Save(string name,object value)
        {
            var folder=Environment.GetEnvironmentVariable("LOR_HOME_WORLDMAP_FOOTER_CLEANUP_DIR");
            if(!string.IsNullOrEmpty(folder))File.WriteAllText(Path.Combine(folder,name+".json"),JsonUtility.ToJson(value,true));
        }
        static GameObject Installed(string page)=>UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab"));
        static Image Legacy(GameObject root)=>root.transform.Find(ProductionWorldMapRuntime.ContentName+"/FooterOrnament").GetComponent<Image>();
        static void CheckWorld(GameObject root)
        {
            var view=root.GetComponent<ProductionWorldMapView>();var legacy=Legacy(root);
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(legacy.sprite,Is.Null);Assert.That(legacy.gameObject.activeSelf,Is.False);
            Assert.That(legacy,Is.Not.SameAs(view.FooterOrnament));Assert.That(view.FooterOrnament.sprite.name,Is.EqualTo("footer_backing"));Assert.That(view.FooterOrnament.enabled&&view.FooterOrnament.gameObject.activeInHierarchy,Is.True);
            foreach(var image in root.GetComponentsInChildren<Image>(true))if(image.sprite!=null)Assert.That(AssetDatabase.GetAssetPath(image.sprite),Is.Not.EqualTo(Archive));
        }
        [Test] public void ArchiveKeepsExactIdentityAndImportsWithoutConsumers()
        {
            Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+Name),Is.Null);Assert.That(AssetDatabase.AssetPathToGUID(Archive),Is.EqualTo("51e28d72f0853db939602ec4f6546f7c"));
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Archive);Assert.That(sprite,Is.Not.Null);Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(2143,734)));Assert.That(sprite.texture.format,Is.EqualTo(TextureFormat.RGBA32));Assert.That(sprite.texture.mipmapCount,Is.EqualTo(12));
            var importer=(TextureImporter)AssetImporter.GetAtPath(Archive);var android=importer.GetPlatformTextureSettings("Android");Assert.That(importer.mipmapEnabled,Is.True);Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Trilinear));Assert.That(importer.isReadable,Is.False);Assert.That(android.overridden,Is.True);Assert.That(android.maxTextureSize,Is.EqualTo(4096));Assert.That(android.format,Is.EqualTo(TextureImporterFormat.RGBA32));
            var ext=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};var roots=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&ext.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();var consumers=roots.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(Archive)).ToArray();Assert.That(consumers,Is.Empty);Save("Dependencies",new Dependencies{roots=roots,consumers=consumers});
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeDoesNotRequestRetiredFooter(string mode)
        {
            var root=mode=="Fresh"?new GameObject("FreshHome",typeof(RectTransform)):Installed("Home");var requested=new List<string>();
            try
            {
                Func<string,Sprite> load=key=>{requested.Add(key);Assert.That(key,Is.Not.EqualTo(ProductionHomeRuntime.ArtRoot+Name));return Resources.Load<Sprite>(key);};
                if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,load);
                else{var view=root.GetComponent<ProductionHomeView>();if(mode=="LegacyMigration")view.SourcePlay=null;ProductionHomeRuntime.UpgradeReferenceForeground(view,load);}
                var home=root.GetComponent<ProductionHomeView>();Assert.That(home.HasRequiredBindings,Is.True);Assert.That(home.FooterBase.sprite.name,Is.EqualTo("source_footer_carving"));Assert.That(home.FooterBase.enabled,Is.True);Assert.That(requested.Count,Is.EqualTo(mode=="CurrentMigration"?0:9));
                Save(mode,new Built{mode=mode,requested=requested.ToArray(),valid=true});
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Fresh")] [TestCase("Installed")]
        public void WorldMapKeepsOnlyCurrentFooterArtwork(string mode)
        {
            var root=mode=="Fresh"?new GameObject("FreshWorldMap",typeof(RectTransform)):Installed("WorldMap");
            try{ProductionWorldMapRuntime.BuildInto((RectTransform)root.transform);CheckWorld(root);}
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("RuntimeOwner")] [TestCase("SourceOwner")]
        public void RepeatedOwnerClearsReinjectedDormantSpriteAndKeepsLiveControls(string mode)
        {
            var root=Installed("WorldMap");
            try
            {
                var view=root.GetComponent<ProductionWorldMapView>();var live=view.FooterOrnament;var sprite=live.sprite;var legacy=Legacy(root);var archived=AssetDatabase.LoadAssetAtPath<Sprite>(Archive);
                var components=root.GetComponentsInChildren<Component>(true);var targets=root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray();
                for(int repeat=0;repeat<2;repeat++)
                {
                    legacy.sprite=archived;if(mode=="RuntimeOwner")ProductionWorldMapRuntime.BuildInto((RectTransform)root.transform);else ProductionWorldMapSourceComposition.Apply(view);
                    CheckWorld(root);Assert.That(view.FooterOrnament,Is.SameAs(live));Assert.That(live.sprite,Is.SameAs(sprite));CollectionAssert.AreEqual(components,root.GetComponentsInChildren<Component>(true));CollectionAssert.AreEqual(targets,root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray());
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void LegacyWorldMapSourceMigrationClearsOnlyHistoricalFooter()
        {
            var root=Installed("WorldMap");
            try
            {
                var view=root.GetComponent<ProductionWorldMapView>();var legacy=Legacy(root);var stage=view.Content.Find("SourceComposition");Assert.That(stage,Is.Not.Null);
                UnityEngine.Object.DestroyImmediate(stage.gameObject);view.Cards=null;view.ChapterScroll=null;view.ChapterAction=null;view.FooterOrnament=legacy;legacy.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Archive);
                Assert.That(view.SourceComposition,Is.False);ProductionWorldMapSourceComposition.Apply(view);CheckWorld(root);
                Assert.That(view.Content.Cast<Transform>().Count(t=>t.name=="SourceComposition"),Is.EqualTo(1));
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void ClearingHelperProtectsFooterThatIsStillTheLiveField()
        {
            var root=Installed("WorldMap");
            try
            {
                var view=root.GetComponent<ProductionWorldMapView>();var live=view.FooterOrnament;var legacy=Legacy(root);var archived=AssetDatabase.LoadAssetAtPath<Sprite>(Archive);legacy.sprite=archived;view.FooterOrnament=legacy;
                var helper=typeof(ProductionWorldMapRuntime).GetMethod("ClearDormantFooterArtwork",BindingFlags.Static|BindingFlags.NonPublic);Assert.That(helper,Is.Not.Null);helper.Invoke(null,new object[]{view});Assert.That(legacy.sprite,Is.SameAs(archived));
                view.FooterOrnament=live;helper.Invoke(null,new object[]{view});CheckWorld(root);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void LoadingAndClonedHeaderGraphsExcludeArchiveAndRetainSharedResources()
        {
            Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(ProductionHomeRuntime.ArtRoot+Name));
            foreach(var page in new[]{"Home","WorldMap","ChapterMap","PreLevel","SplashTitle","Loading"})Assert.That(AssetDatabase.GetDependencies(Pages+page+".prefab",true),Does.Not.Contain(Archive));
            foreach(var name in new[]{"home_logo","avatar_portrait","avatar_ring","resource_capsule","resource_plus","settings_medallion","coin_icon","gems_icon","energy_icon"})Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.Not.Null);
        }
    }
}
#endif
