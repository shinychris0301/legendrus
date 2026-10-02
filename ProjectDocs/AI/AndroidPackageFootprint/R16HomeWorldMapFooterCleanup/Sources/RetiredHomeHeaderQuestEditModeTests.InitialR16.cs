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
    public sealed class RetiredHomeHeaderQuestEditModeTests
    {
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        const string Archive="Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/";
        static readonly string[] Names={"hud_frame","quest_panel"};
        [Serializable] public sealed class Dependencies {public string name;public string[] roots,consumers;}
        [Serializable] public sealed class Built {public string mode;public string[] requested;public bool valid;}
        static void Save(string name,object record)
        {
            var folder=Environment.GetEnvironmentVariable("LOR_HOME_HEADER_QUEST_CLEANUP_DIR");
            if(!string.IsNullOrEmpty(folder))File.WriteAllText(Path.Combine(folder,name+".json"),JsonUtility.ToJson(record,true));
        }
        static GameObject Installed(string page)=>UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab"));
        static void Check(ProductionHomeView view)
        {
            Assert.That(view.HasRequiredBindings,Is.True);Assert.That(view.HasGeneratedControlBindings,Is.True);Assert.That(view.HasReferenceForegroundBindings,Is.True);
            Assert.That(view.HudFrame.sprite.name,Is.EqualTo("source_header"));Assert.That(view.HudFrame.enabled,Is.True);
            Assert.That(view.QuestFrame.sprite,Is.SameAs(view.SourceRussianQuest).Or.SameAs(view.SourceBlankQuest));Assert.That(view.QuestFrame.enabled,Is.True);
            foreach(var image in view.GetComponentsInChildren<Image>(true))if(image.sprite!=null)foreach(var name in Names)
                Assert.That(AssetDatabase.GetAssetPath(image.sprite),Is.Not.EqualTo(Archive+name+".png"));
        }
        [TestCase("hud_frame","82085c57945c64ecab5231171e999c10",2176,723)]
        [TestCase("quest_panel","03d94585ef770c0a9aca2184b5239550",2206,713)]
        public void ArchivesPreserveIdentityImportsAndHaveNoConsumers(string name,string guid,int width,int height)
        {
            var path=Archive+name+".png";Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.Null);Assert.That(AssetDatabase.AssetPathToGUID(path),Is.EqualTo(guid));
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);Assert.That(sprite,Is.Not.Null);Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(width,height)));
            Assert.That(sprite.texture.format,Is.EqualTo(TextureFormat.RGBA32));Assert.That(sprite.texture.mipmapCount,Is.EqualTo(12));
            var ti=(TextureImporter)AssetImporter.GetAtPath(path);var android=ti.GetPlatformTextureSettings("Android");
            Assert.That(ti.mipmapEnabled,Is.True);Assert.That(ti.filterMode,Is.EqualTo(FilterMode.Trilinear));Assert.That(ti.isReadable,Is.False);
            Assert.That(android.overridden,Is.True);Assert.That(android.maxTextureSize,Is.EqualTo(4096));Assert.That(android.format,Is.EqualTo(TextureImporterFormat.RGBA32));
            var ext=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};
            var roots=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&ext.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();
            var consumers=roots.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(path)).ToArray();Assert.That(consumers,Is.Empty);Save(name,new Dependencies{name=name,roots=roots,consumers=consumers});
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeConstructsWithoutEitherRetiredKey(string mode)
        {
            var root=mode=="Fresh"?new GameObject("FreshHome",typeof(RectTransform)):Installed("Home");var requested=new List<string>();
            try
            {
                Func<string,Sprite> load=key=>{requested.Add(key);foreach(var name in Names)Assert.That(key,Is.Not.EqualTo(ProductionHomeRuntime.ArtRoot+name));return Resources.Load<Sprite>(key);};
                if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,load);
                else{var view=root.GetComponent<ProductionHomeView>();if(mode=="LegacyMigration")view.SourcePlay=null;ProductionHomeRuntime.UpgradeReferenceForeground(view,load);}
                Check(root.GetComponent<ProductionHomeView>());Assert.That(requested.Count,Is.EqualTo(mode=="CurrentMigration"?0:19));
                Save(mode,new Built{mode=mode,requested=requested.ToArray(),valid=true});
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("WorldMap")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void FreshAndInstalledClonedHeadersKeepCurrentArtwork(string page)
        {
            var fresh=new GameObject("Fresh"+page,typeof(RectTransform));var installed=Installed(page);
            try
            {
                var owner=Type.GetType("LegendsOfRus.Production"+page+"Runtime, LegendsOfRus.Runtime",true);owner.GetMethod("BuildInto").Invoke(null,new object[]{(RectTransform)fresh.transform});
                foreach(var root in new[]{fresh,installed})
                {
                    var type=Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true);var view=root.GetComponent(type);
                    Assert.That((bool)type.GetProperty("HasRequiredBindings").GetValue(view),Is.True);
                    foreach(var header in root.GetComponentsInChildren<ProductionPlayerHeader>(true)){Assert.That(header.HasBindings,Is.True);Assert.That(header.Backing.sprite.name,Is.EqualTo("source_header"));}
                    foreach(var image in root.GetComponentsInChildren<Image>(true))if(image.sprite!=null)foreach(var name in Names)Assert.That(AssetDatabase.GetAssetPath(image.sprite),Is.Not.EqualTo(Archive+name+".png"));
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(fresh);UnityEngine.Object.DestroyImmediate(installed);}
        }
        [Test] public void RepeatedHomeOwnerKeepsComponentsAndLiveTargets()
        {
            var root=Installed("Home");
            try
            {
                var view=root.GetComponent<ProductionHomeView>();var components=root.GetComponentsInChildren<Component>(true);var targets=root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray();
                for(int i=0;i<2;i++){ProductionHomeRuntime.BuildInto((RectTransform)root.transform);ProductionHomeRuntime.UpgradeReferenceForeground(view,Resources.Load<Sprite>);Check(view);}
                CollectionAssert.AreEqual(components,root.GetComponentsInChildren<Component>(true));CollectionAssert.AreEqual(targets,root.GetComponentsInChildren<Selectable>(true).Select(s=>s.targetGraphic).ToArray());
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void HistoricalControlDiagnosticUsesCurrentHeaderContract()
        {
            var root=Installed("Home");
            try
            {
                var view=root.GetComponent<ProductionHomeView>();Check(view);var correct=view.HudFrame.sprite;
                view.HudFrame.sprite=Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+"coin_icon");Assert.That(view.HasGeneratedControlBindings,Is.False);Assert.That(view.HasRequiredBindings,Is.False);
                view.HudFrame.sprite=correct;Check(view);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void LoadingGraphsQuestVariantsAndRetainedResourcesRemainCurrent()
        {
            foreach(var page in new[]{"Home","WorldMap","ChapterMap","PreLevel","SplashTitle","Loading"})foreach(var name in Names)
                Assert.That(AssetDatabase.GetDependencies(Pages+page+".prefab",true),Does.Not.Contain(Archive+name+".png"));
            foreach(var name in Names)Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(ProductionHomeRuntime.ArtRoot+name));
            foreach(var name in new[]{"home_logo","avatar_portrait","avatar_ring","resource_capsule","resource_plus","settings_medallion","coin_icon","gems_icon","energy_icon","quest_reward_frame"})Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.Not.Null);
            var view=AssetDatabase.LoadAssetAtPath<GameObject>(Pages+"Home.prefab").GetComponent<ProductionHomeView>();
            Assert.That(view.SourceRussianQuest.name,Is.EqualTo("source_quest_ru"));Assert.That(view.SourceBlankQuest.name,Is.EqualTo("source_quest_blank"));
        }
        [Test] public void MissingUnrelatedQuestRewardStillBlocksFreshAndLegacyConstruction()
        {
            foreach(var mode in new[]{"Fresh","Legacy"})
            {
                var root=mode=="Fresh"?new GameObject("DeniedHome",typeof(RectTransform)):Installed("Home");
                try
                {
                    Func<string,Sprite> load=key=>key==ProductionHomeRuntime.ArtRoot+"quest_reward_frame"?null:Resources.Load<Sprite>(key);
                    if(mode=="Fresh")Assert.Throws<InvalidOperationException>(()=>ProductionHomeRuntime.BuildInto((RectTransform)root.transform,load));
                    else{var view=root.GetComponent<ProductionHomeView>();view.SourcePlay=null;Assert.Throws<InvalidOperationException>(()=>ProductionHomeRuntime.UpgradeReferenceForeground(view,load));}
                }
                finally{UnityEngine.Object.DestroyImmediate(root);}
            }
        }
    }
}
#endif
