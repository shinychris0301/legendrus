#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class EventDetailAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/EventDetail.prefab";
        [Test]public void NativeDetailReusesApprovedAssetsAndHasNoReferenceDependencies()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionEventDetailView>();Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.EventDetail),Is.EqualTo("ProductionUI/Pages/EventDetail"));
            Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(8));
            Assert.That(v.Typography,Is.SameAs(Resources.Load<GameObject>("ProductionUI/Pages/Events").GetComponent<ProductionEventsView>().Typography));
            var events=Resources.Load<GameObject>("ProductionUI/Pages/Events").GetComponent<ProductionEventsView>();
            Assert.That(v.Header,Is.Not.Null);Assert.That(v.Header.HasBindings,Is.True);
            Assert.That(v.Header.Balances[2],Is.SameAs(v.EnergyText));
            Assert.That(v.Environment.sprite,Is.SameAs(events.Environment.sprite));
            Assert.That(v.Environment.color,Is.EqualTo(events.Environment.color));
            Assert.That(v.Title.sprite,Is.SameAs(ProductionEventDetailRuntime.DetailArt("title")));
            Assert.That(v.RewardExamples.gameObject.activeSelf,Is.False);
            Assert.That(v.PlayerNamesFont,Is.SameAs(Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames")));
            Assert.That(v.SectionFont,Is.SameAs(Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title")));
            Assert.That(v.PlayerNamesFont.dynamic,Is.True);Assert.That(v.SectionFont.dynamic,Is.True);
            for(int i=0;i<5;i++)Assert.That(v.Body.Find("Milestones/Chest_"+i).GetComponent<Image>().sprite,Is.SameAs(ProductionEventDetailRuntime.DetailArt("chest"+i+"_r5")));
            Assert.That(v.Body.Find("FestivalScene/SceneWindow/SourceFirebird").GetComponent<Image>().sprite,Is.SameAs(ProductionEventDetailRuntime.DetailArt("firebird_r7")));
            Assert.That(v.Body.Find("PremiumPanel/UnavailablePass").GetComponent<Button>().interactable,Is.False);
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));}
            foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);
            Assert.That(v.ActionButton.onClick.GetPersistentEventCount(),Is.Zero);
        }
        [Test]public void RepeatedBuildKeepsBindingsAndUnavailableState()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try{
                int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionEventDetailRuntime.BuildInto((RectTransform)root.transform);ProductionEventDetailRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));
                var v=root.GetComponent<ProductionEventDetailView>();Assert.That(v.HasRequiredBindings,Is.True);
                Assert.That(v.CanParticipate,Is.False);Assert.That(v.ActionButton.interactable,Is.False);
            }finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
