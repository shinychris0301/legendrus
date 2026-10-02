#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class IndividualRankingAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/IndividualRanking.prefab";
        [Test]public void NativeRankingUsesLiveLabelsAndNoReferenceDependencies()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionIndividualRankingView>();Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.IndividualRanking),Is.EqualTo("ProductionUI/Pages/IndividualRanking"));
            Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(11));
            Assert.That(v.Typography,Is.SameAs(Resources.Load<GameObject>("ProductionUI/Pages/Events").GetComponent<ProductionEventsView>().Typography));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
            foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);
            foreach(var image in v.RowPortraits)Assert.That(image.sprite.name,Does.StartWith("portrait_"));
            Assert.That(v.PrizeFrames.Length,Is.EqualTo(3));Assert.That(v.RulesPanel.activeSelf,Is.False);
            Assert.That(v.Header,Is.Not.Null);Assert.That(v.Header.HasBindings,Is.True);
            Assert.That(v.Header.Backing.sprite,Is.SameAs(Resources.Load<GameObject>("ProductionUI/Pages/Home").GetComponent<ProductionHomeView>().GetComponentInChildren<ProductionPlayerHeader>(true).Backing.sprite));
            Assert.That(v.Header.Balances[2],Is.SameAs(v.EnergyText));
            Assert.That(v.RulesButton.image.sprite.name,Is.EqualTo("rules_original_ru"));
            Assert.That(v.RulesButtonText.enabled,Is.False,"Original Russian lettering must not be overprinted");
            foreach(var card in v.PrizeFrames)Assert.That(card.Find("Gem").GetComponent<Image>().sprite,Is.SameAs(ProductionGameplayArt.Gem(1)));
            AssertSelectedFonts(v);
            foreach(var font in new[]{v.PlayerNamesFont,v.HeadingFont,v.RankNumbersFont}){
                Assert.That(AssetDatabase.GetAssetPath(font),Does.StartWith("Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingFonts/"));
                Assert.That(((TrueTypeFontImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(font))).includeFontData,Is.True);
            }
            for(int i=0;i<14;i++)Assert.That(v.Ranks[i].enabled,Is.EqualTo(i>=3),"Original podium numeral must not be overprinted");
            Assert.That(v.PlayerRank.text,Is.EqualTo("12"));Assert.That(v.PlayerScore.text,Is.EqualTo("512 430"));
        }
        static void AssertSelectedFonts(ProductionIndividualRankingView v)
        {
            Assert.That(v.Heading.font,Is.SameAs(v.HeadingFont));
            Assert.That(v.HeroName.font,Is.SameAs(v.PlayerNamesFont));
            foreach(var text in v.Names)Assert.That(text.font,Is.SameAs(v.PlayerNamesFont));
            foreach(var text in v.Ranks)Assert.That(text.font,Is.SameAs(v.RankNumbersFont));
        }
        [Test]public void BuilderIsIdempotentAndNoLiveRankingIsInvented()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try{
                int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionIndividualRankingRuntime.BuildInto((RectTransform)root.transform);ProductionIndividualRankingRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));
                var v=root.GetComponent<ProductionIndividualRankingView>();Assert.That(v.HasLiveLeaderboard,Is.False);
                Assert.That(v.Body.Find("CarvedFooter").GetSiblingIndex(),Is.LessThan(v.BackButton.transform.GetSiblingIndex()),"Repeated rebuild must not cover Back with the carving");
                Assert.That(v.Body.Find("CarvedFooter").GetSiblingIndex(),Is.LessThan(v.RulesButton.transform.GetSiblingIndex()));
                Assert.That(v.Rows.Length,Is.EqualTo(14));Assert.That(v.Period,Is.EqualTo(IndividualRankingPeriod.Season));
                v.SelectPeriod(IndividualRankingPeriod.Day);v.RefreshContent();AssertSelectedFonts(v);
                Assert.Throws<System.ArgumentOutOfRangeException>(()=>v.SelectPeriod((IndividualRankingPeriod)42));
            }finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
