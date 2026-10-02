#if UNITY_EDITOR
using System;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class RetiredProgressArtworkEditModeTests
    {
        const string Pages="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        public static bool IsRetired(Image i)=>i.name=="ProfileProgressFrame"||i.name=="QuestProgressFrame"||i.name=="MeasuredFill"||
            i.name=="GoalProgress"||i.name=="StarProgress"||i.name=="GoalProgressFrame"||i.name=="StarProgressFrame";
        static void Build(GameObject root,string page)
        {
            if(page=="Home") { ProductionHomeRuntime.BuildInto((RectTransform)root.transform,key=>
                {Assert.That(key,Does.Not.EndWith("/progress_fill").And.Not.EndWith("/progress_frame"));return Resources.Load<Sprite>(key);});return; }
            Type.GetType("LegendsOfRus.Production"+page+"Runtime, LegendsOfRus.Runtime",true).GetMethod("BuildInto")
                .Invoke(null,new object[]{(RectTransform)root.transform});
        }
        static void Check(GameObject root,string page)
        {
            var view=root.GetComponent(Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true));
            Assert.That((bool)view.GetType().GetProperty("HasRequiredBindings").GetValue(view),Is.True);
            Assert.That(view.GetType().GetField("Revision").GetValue(view),Is.EqualTo(view.GetType().GetField("CurrentRevision").GetRawConstantValue()));
            var images=root.GetComponentsInChildren<Image>(true).Where(IsRetired).ToArray();Assert.That(images,Is.Not.Empty);
            foreach(var image in images){Assert.That(image.sprite,Is.Null,image.name);Assert.That(image.enabled&&image.gameObject.activeInHierarchy,Is.False,image.name);}
        }
        [TestCase("Home")] [TestCase("WorldMap")] [TestCase("ChapterMap")] [TestCase("PreLevel")] [TestCase("Gameplay")] [TestCase("Fail")]
        public void SerializedAndFreshPagesHaveNoRetiredTextureDependency(string page)
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab");Check(prefab,page);
            foreach(var path in AssetDatabase.GetDependencies(Pages+page+".prefab",true))
                Assert.That(System.IO.Path.GetFileName(path),Is.Not.EqualTo("progress_fill.png").And.Not.EqualTo("progress_frame.png"));
            var root=new GameObject("Fresh_"+page,typeof(RectTransform));
            try {Build(root,page);Check(root,page);} finally {UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("Home")] [TestCase("WorldMap")] [TestCase("ChapterMap")] [TestCase("PreLevel")] [TestCase("Gameplay")] [TestCase("Fail")]
        public void ExistingBuilderClearsStaleArtWithoutReplacingComponents(string page)
        {
            var root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+page+".prefab"));
            try
            {
                var objects=root.GetComponentsInChildren<Component>(true);
                var images=root.GetComponentsInChildren<Image>(true).Where(IsRetired).ToArray();
                var sample=Resources.Load<Sprite>(ProductionHomeSourceComposition.ArtRoot+"source_gold");Assert.That(sample,Is.Not.Null);
                foreach(var image in images)image.sprite=sample;
                Build(root,page);Check(root,page);Build(root,page);Check(root,page);
                CollectionAssert.AreEquivalent(objects,root.GetComponentsInChildren<Component>(true));
            }
            finally {UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("progress_fill","50ba28375a9a4edb1be246997041faea")]
        [TestCase("progress_frame","4585318c279622051a6d988eb18e05e3")]
        public void ArchivedTexturesKeepIdentityAndImportsOutsideResources(string name,string guid)
        {
            string path="Assets/LegendsOfRus/Art/ReferenceOnly/RetiredProgress/"+name+".png";
            Assert.That(Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot+name),Is.Null);
            Assert.That(AssetDatabase.AssetPathToGUID(path),Is.EqualTo(guid));
            var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);Assert.That(sprite,Is.Not.Null);
            Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(2172,724)));
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);var android=importer.GetPlatformTextureSettings("Android");
            Assert.That(importer.mipmapEnabled,Is.True);Assert.That(importer.filterMode,Is.EqualTo(FilterMode.Trilinear));
            Assert.That(android.format,Is.EqualTo(TextureImporterFormat.RGBA32));Assert.That(android.maxTextureSize,Is.EqualTo(4096));
            Assert.That(importer.isReadable,Is.False);Assert.That(importer.spriteBorder,Is.EqualTo(Vector4.zero));
        }
        [Test] public void HomeLegacyMigrationDoesNotRequestRetiredSprites()
        {
            var root=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Pages+"Home.prefab"));
            try
            {
                var view=root.GetComponent<ProductionHomeView>();view.SourcePlay=null;
                ProductionHomeRuntime.UpgradeReferenceForeground(view,key=>
                {Assert.That(key,Does.Not.EndWith("/progress_fill").And.Not.EndWith("/progress_frame"));return Resources.Load<Sprite>(key);});
                Check(root,"Home");
            }
            finally {UnityEngine.Object.DestroyImmediate(root);}
        }
    }
}
#endif
