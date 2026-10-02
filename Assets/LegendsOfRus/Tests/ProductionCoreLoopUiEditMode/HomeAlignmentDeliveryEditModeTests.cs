#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class HomeAlignmentDeliveryEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab";
        ProductionHomeView Load()=>AssetDatabase.LoadAssetAtPath<GameObject>(Path).GetComponent<ProductionHomeView>();
        [Test] public void ActiveHomePreservesIdentityAndHasCompleteSerializedViewAndLayout()
        {
            var v=Load();Assert.That(AssetDatabase.AssetPathToGUID(Path),Is.EqualTo("a3c6628ddb0a3bcc3d254d89a5b54043"));
            Assert.That(v.HasRequiredBindings,Is.True);Assert.That(v.GetComponent<ProductionHomeLayout>().SourceComposition,Is.True);
            Assert.That(v.GetComponentsInChildren<Button>(true),Is.EquivalentTo(v.Buttons()));Assert.That(v.Buttons().Length,Is.EqualTo(12));
            Assert.That(v.GetComponentsInChildren<Canvas>(true),Is.Empty);
            foreach(var button in v.Buttons()){Assert.That(button.targetGraphic.enabled,Is.True,button.name);Assert.That(button.targetGraphic.raycastTarget,Is.True,button.name);Assert.That(button.targetGraphic.transform.IsChildOf(button.transform),Is.True,button.name);}
            Type owner=Type.GetType("LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder, Assembly-CSharp-Editor",true);
            Assert.That((bool)owner.GetMethod("PageNeedsBuild",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{ScreenId.Home}),Is.False);
        }
        [Test] public void ReapplyingSourceMigrationKeepsTheSameTwelveButtons()
        {
            var root=UnityEngine.Object.Instantiate(Load().gameObject);
            try {
                var v=root.GetComponent<ProductionHomeView>();var original=v.Buttons();
                ProductionHomeSourceComposition.Apply(v);ProductionHomeSourceComposition.Apply(v);
                Assert.That(v.HasRequiredBindings,Is.True);
                Assert.That(v.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(12));
                CollectionAssert.AreEqual(original,v.Buttons());
            } finally { UnityEngine.Object.DestroyImmediate(root); }
        }
        [Test] public void PreparationCoversEveryVisibleSourceAndBothLanguages()
        {
            var keys=ProductionHomeRuntime.GetTextureResourceKeys();Assert.That(keys.Distinct().Count(),Is.EqualTo(keys.Count));
            var sprites=keys.Select(Resources.Load<Sprite>).ToArray();Assert.That(sprites.All(s=>s!=null),Is.True);
            var v=Load();foreach(var image in v.GetComponentsInChildren<Image>(true).Where(i=>i.isActiveAndEnabled && i.sprite!=null))
                Assert.That(sprites.Contains(image.sprite),Is.True,image.name+" missing from actual measured preparation");
            foreach(var sprite in v.SourceBlankCards.Concat(v.SourceRussianCards).Concat(new[]{v.SourceRussianPlay,v.SourceBlankPlay,v.SourceRussianQuest,v.SourceBlankQuest}))Assert.That(sprites.Contains(sprite),Is.True);
        }
        [Test] public void OriginalSpritesKeepWholePixelDimensionsAndNoMipBlur()
        {
            foreach(var key in ProductionHomeRuntime.GetTextureResourceKeys()){
                var p="Assets/LegendsOfRus/Resources/"+key+".png";var importer=(TextureImporter)AssetImporter.GetAtPath(p);var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(p);
                importer.GetSourceTextureWidthAndHeight(out int w,out int h);
                Assert.That(sprite.rect.size,Is.EqualTo(new Vector2(w,h)),p);Assert.That(sprite.border,Is.EqualTo(Vector4.zero));
                Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));Assert.That(importer.mipmapEnabled,Is.False);Assert.That(importer.sRGBTexture,Is.True);
            }
        }
        // Name retained for the existing Splash/Loading cross-page regression runner.
        [Test] public void HomeReferenceIdentityUsesIndependentWholeSpritesAndLivePlayText()
        {
            var v=Load();Assert.That(v.Logo.sprite.name,Is.EqualTo("source_logo"));Assert.That(v.Ivan.sprite.name,Is.EqualTo("source_hero"));
            Assert.That(v.SourceRussianPlay.name,Is.EqualTo("source_play_ru"));Assert.That(v.SourceBlankPlay.name,Is.EqualTo("source_play_blank"));
            Assert.That(v.PlayText,Is.Not.Null);Assert.That(v.Environment.color,Is.EqualTo(Color.white));Assert.That(v.Ivan.color,Is.EqualTo(Color.white));
            Assert.That(v.ReferenceBodyFont,Is.Not.Null);Assert.That(v.ReferenceTitleFont,Is.Not.Null);
            Assert.That(v.Typography,Is.SameAs(Resources.Load<SplashTitleTypography>(ProductionHomeRuntime.TypographyResource)));
        }
        [Test] public void RegisteredOverlappingLayersStayAtOriginalSourceCoordinates()
        {
            var v=Load();Assert.That(v.Ivan.rectTransform.anchoredPosition,Is.EqualTo(new Vector2(0,-466)));
            Assert.That(v.Ivan.rectTransform.sizeDelta,Is.EqualTo(new Vector2(941,990)));
            Assert.That(v.SourcePlay.rectTransform.sizeDelta,Is.EqualTo(new Vector2(584,242)));
            Assert.That(v.PlayButton.GetComponent<RectTransform>().anchoredPosition,Is.EqualTo(new Vector2(178,-998)));
            Assert.That(v.GetComponent<ProductionHomeLayout>().Navigation.anchoredPosition,Is.EqualTo(new Vector2(0,-1214)));
            Assert.That(v.GetComponent<ProductionHomeLayout>().Footer.anchoredPosition,Is.EqualTo(new Vector2(0,-1454)));
            Assert.That(v.MapFoliage.enabled,Is.False);Assert.That(v.BackgroundShade.enabled,Is.False);
        }
    }
}
#endif
