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
    public sealed class RetiredHomeDormantArtworkEditModeTests
    {
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        const string Archive="Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/";
        static readonly string[] Names={"nav_map_generated","nav_shop_generated","nav_events_generated","nav_characters_generated","mail_medallion_generated","reward_medallion_generated","quest_reward_frame","quest_reward_gem","home_map_foliage"};
        [Serializable] public sealed class DependencyRecord {public string name,guid;public int width,height,mips;public string[] roots,consumers;}
        [Serializable] public sealed class Built {public string mode;public string[] requested;public bool valid;}
        static void Save(string name,object record)
        {
            var folder=Environment.GetEnvironmentVariable("LOR_HOME_DORMANT_ARTWORK_CLEANUP_DIR");
            if(!string.IsNullOrEmpty(folder))File.WriteAllText(Path.Combine(folder,name+".json"),JsonUtility.ToJson(record,true));
        }
        static GameObject Installed(string page)=>UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab"));
        static Button[] Buttons(ProductionHomeView v)=>new[]{v.WorldMapButton,v.ShopButton,v.EventsButton,v.CharactersButton,v.MailButton,v.DailyRewardButton};
        static Image[] Dormant(ProductionHomeView v)=>Buttons(v).Select((b,i)=>b.transform.Find(i<4?"ReferenceArtwork":"Medallion").GetComponent<Image>()).Concat(new[]{v.QuestRewardFrame,v.QuestRewardIcon,v.MapFoliage}).ToArray();
        static void Check(ProductionHomeView v)
        {
            Assert.That(v.HasRequiredBindings,Is.True);
            foreach(var image in Dormant(v)){Assert.That(image.sprite,Is.Null);Assert.That(image.enabled,Is.False);}
            foreach(var b in Buttons(v)){Assert.That(b.targetGraphic.enabled,Is.True);Assert.That(((Image)b.targetGraphic).sprite,Is.Not.Null);Assert.That(b.targetGraphic.name,Is.EqualTo("OriginalSourceArtwork"));}
            foreach(var image in v.GetComponentsInChildren<Image>(true))if(image.sprite!=null)
                Assert.That(Names,Does.Not.Contain(image.sprite.name));
            Assert.That(v.QuestFrame.sprite,Is.SameAs(v.SourceRussianQuest).Or.SameAs(v.SourceBlankQuest));
        }
        [TestCase("nav_map_generated","c799ce05d21b30886a6fa33a9c3e4c33",1254,1254)]
        [TestCase("nav_shop_generated","4bbb7d70ab8e8c29385b536e7e8dbf2f",1254,1254)]
        [TestCase("nav_events_generated","b22f4fdf95dddd63f8c32fab7a753da0",1254,1254)]
        [TestCase("nav_characters_generated","69135cdae1222fa598c05c4ab1539397",1254,1254)]
        [TestCase("mail_medallion_generated","23aee3e96f6777c45a4cae8a64c33c4c",1254,1254)]
        [TestCase("reward_medallion_generated","717db484c9b631d909a47c6df0c87daf",1254,1254)]
        [TestCase("quest_reward_frame","d76c3dc6c6b59ff2ca8b7698b698ec03",1095,1437)]
        [TestCase("quest_reward_gem","2cb9251bbe55ba200b4383179b701814",1230,1278)]
        [TestCase("home_map_foliage","94bad924675b43dcadb2595547a51117",1024,1536)]
        public void ArchivesKeepExactIdentityImportsAndExcludeDependencyConsumers(string name,string guid,int width,int height)
        {
            var path=Archive+name+".png";Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.Null);Assert.That(AssetDatabase.AssetPathToGUID(path),Is.EqualTo(guid));
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);Assert.That(sprite,Is.Not.Null);Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(width,height)));Assert.That(sprite.texture.format,Is.EqualTo(TextureFormat.RGBA32));Assert.That(sprite.texture.mipmapCount,Is.EqualTo(11));
            var ti=(TextureImporter)AssetImporter.GetAtPath(path);var android=ti.GetPlatformTextureSettings("Android");Assert.That(ti.mipmapEnabled,Is.True);Assert.That(ti.filterMode,Is.EqualTo(FilterMode.Trilinear));Assert.That(ti.isReadable,Is.False);Assert.That(android.overridden,Is.True);Assert.That(android.maxTextureSize,Is.EqualTo(4096));Assert.That(android.format,Is.EqualTo(TextureImporterFormat.RGBA32));
            var ext=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};var roots=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&ext.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();var consumers=roots.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(path)).ToArray();Assert.That(consumers,Is.Empty);
            Save(name,new DependencyRecord{name=name,guid=guid,width=width,height=height,mips=sprite.texture.mipmapCount,roots=roots,consumers=consumers});
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeConstructsWithoutAnyOfNineRetiredKeys(string mode)
        {
            var root=mode=="Fresh"?new GameObject("FreshHome",typeof(RectTransform)):Installed("Home");var requested=new List<string>();
            try
            {
                Func<string,Sprite> load=key=>{requested.Add(key);foreach(var name in Names)Assert.That(key,Is.Not.EqualTo(ProductionHomeRuntime.ArtRoot+name));return Resources.Load<Sprite>(key);};
                if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,load);
                else{var v=root.GetComponent<ProductionHomeView>();if(mode=="LegacyMigration")v.SourcePlay=null;ProductionHomeRuntime.UpgradeReferenceForeground(v,load);}
                var view=root.GetComponent<ProductionHomeView>();Check(view);Assert.That(requested.Count,Is.EqualTo(mode=="CurrentMigration"?0:9));Assert.That(view.HasSceneLightingBindings&&view.HasReferenceBackdropBindings,Is.True);
                Save(mode,new Built{mode=mode,requested=requested.ToArray(),valid=true});
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("RuntimeOwner")] [TestCase("SourceOwner")]
        public void RepeatedOwnersClearStaleSpritesPreservingObjectsComponentsAndLiveTargets(string mode)
        {
            var root=Installed("Home");
            try
            {
                var v=root.GetComponent<ProductionHomeView>();var dormant=Dormant(v);var controls=Buttons(v);var targets=controls.Select(b=>b.targetGraphic).ToArray();var live=targets.Cast<Image>().Select(i=>i.sprite).ToArray();var components=root.GetComponentsInChildren<Component>(true);
                for(int repeat=0;repeat<2;repeat++)
                {
                    for(int i=0;i<Names.Length;i++)dormant[i].sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Archive+Names[i]+".png");
                    if(mode=="RuntimeOwner")ProductionHomeRuntime.BuildInto((RectTransform)root.transform);else ProductionHomeSourceComposition.Apply(v);
                    Check(v);CollectionAssert.AreEqual(components,root.GetComponentsInChildren<Component>(true));CollectionAssert.AreEqual(dormant,Dormant(v));CollectionAssert.AreEqual(targets,controls.Select(b=>b.targetGraphic));CollectionAssert.AreEqual(live,targets.Cast<Image>().Select(i=>i.sprite));
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void ClearingHelperProtectsLiveControlTargetsAndEnabledFieldImages()
        {
            var root=Installed("Home");
            try
            {
                var v=root.GetComponent<ProductionHomeView>();var dormant=Dormant(v);var buttons=Buttons(v);var original=buttons.Select(b=>b.targetGraphic).ToArray();
                for(int i=0;i<9;i++){dormant[i].sprite=AssetDatabase.LoadAssetAtPath<Sprite>(Archive+Names[i]+".png");if(i<6)buttons[i].targetGraphic=dormant[i];else dormant[i].enabled=true;}
                var clear=typeof(ProductionHomeRuntime).GetMethod("ClearDormantArtwork",BindingFlags.Static|BindingFlags.NonPublic);Assert.That(clear,Is.Not.Null);clear.Invoke(null,new object[]{v});foreach(var image in dormant)Assert.That(image.sprite,Is.Not.Null);
                for(int i=0;i<9;i++){if(i<6)buttons[i].targetGraphic=original[i];else dormant[i].enabled=false;}
                clear.Invoke(null,new object[]{v});Check(v);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void HistoricalDiagnosticsValidateCurrentArtAndInactiveHistoricalEffects()
        {
            var root=Installed("Home");
            try
            {
                var v=root.GetComponent<ProductionHomeView>();Assert.That(v.HasReferenceBackdropBindings&&v.HasSceneLightingBindings,Is.True);var env=v.Environment.sprite;v.Environment.sprite=v.Logo.sprite;Assert.That(v.HasReferenceBackdropBindings||v.HasSceneLightingBindings,Is.False);v.Environment.sprite=env;
                var lighting=v.Ivan.GetComponent<ProductionHomeSceneLighting>();lighting.enabled=true;Assert.That(v.HasSceneLightingBindings,Is.False);lighting.enabled=false;v.MapFoliage.enabled=true;Assert.That(v.HasReferenceBackdropBindings,Is.False);v.MapFoliage.enabled=false;Check(v);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("WorldMap")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void FreshAndInstalledClonedHeadersExcludeRetiredArtwork(string page)
        {
            var fresh=new GameObject("Fresh"+page,typeof(RectTransform));var installed=Installed(page);
            try
            {
                var owner=Type.GetType("LegendsOfRus.Production"+page+"Runtime, LegendsOfRus.Runtime",true);owner.GetMethod("BuildInto").Invoke(null,new object[]{(RectTransform)fresh.transform});
                foreach(var root in new[]{fresh,installed})
                {
                    var type=Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true);Assert.That((bool)type.GetProperty("HasRequiredBindings").GetValue(root.GetComponent(type)),Is.True);
                    foreach(var header in root.GetComponentsInChildren<ProductionPlayerHeader>(true)){Assert.That(header.HasBindings,Is.True);Assert.That(header.Backing.sprite.name,Is.EqualTo("source_header"));}
                    foreach(var image in root.GetComponentsInChildren<Image>(true))if(image.sprite!=null)Assert.That(Names,Does.Not.Contain(image.sprite.name));
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(fresh);UnityEngine.Object.DestroyImmediate(installed);}
        }
        [Test] public void RetainNineLiveSharedResourcesAndLoginGameplayCurrencyDependencies()
        {
            foreach(var name in new[]{"home_logo","avatar_portrait","avatar_ring","resource_capsule","resource_plus","settings_medallion","coin_icon","gems_icon","energy_icon"})Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.Not.Null);
            var oldRoot="Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/";
            Assert.That(AssetDatabase.GetDependencies(Pages+"Login.prefab",true),Does.Contain(oldRoot+"home_logo.png"));Assert.That(AssetDatabase.GetDependencies(Pages+"Gameplay.prefab",true),Does.Contain(oldRoot+"avatar_portrait.png"));Assert.That(AssetDatabase.GetDependencies(Pages+"PreLevel.prefab",true),Does.Contain(oldRoot+"coin_icon.png"));
            foreach(var name in Names)Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(ProductionHomeRuntime.ArtRoot+name));
        }
    }
}
#endif
