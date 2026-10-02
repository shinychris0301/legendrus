#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class SpecialOfferAlignmentEditModeTests
    {
        const string Path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/SpecialOffer.prefab";
        [Test]public void ActiveOfferHasNativeControlsAndCleanReusableArt()
        {
            var root=AssetDatabase.LoadAssetAtPath<GameObject>(Path);Assert.That(root,Is.Not.Null);
            var v=root.GetComponent<ProductionSpecialOfferView>();Assert.That(v.HasRequiredBindings,Is.True);
            Assert.That(ProductionMetaPageCutoverRegistry.ResourceKey(ScreenId.SpecialOffer),Is.EqualTo("ProductionUI/Pages/SpecialOffer"));
            Assert.That(root.GetComponent<ProductionSpecialOfferLayout>().View,Is.SameAs(v));
            Assert.That(root.GetComponent<ProductionCoreLoopPageMarker>().ScreenshotFree,Is.True);
            Assert.That(root.GetComponentsInChildren<Button>(true).Length,Is.EqualTo(8));
            Assert.That(v.PlayerHeader.HasBindings,Is.True);
            Assert.That(v.Hud.GetComponent<CanvasGroup>().blocksRaycasts,Is.False);
            Assert.That(v.Hud.GetComponent<CanvasGroup>().interactable,Is.False);
            Assert.That(v.PlayerHeader.Capsules[2].gameObject.activeSelf,Is.True);
            Assert.That(v.ShopButton.gameObject.activeSelf,Is.False);
            Assert.That(v.Header.gameObject.activeSelf,Is.True);
            Assert.That(v.Header.sprite.name,Is.EqualTo("header_ru_r3"));
            Assert.That(v.PurchaseButton.GetComponent<Image>().sprite.name,Is.EqualTo("buy_lettered_r3"));
            Assert.That(v.PurchaseText.enabled,Is.False);
            Assert.That(v.Hero.sprite.name,Is.EqualTo("hero_source_r2"));
            Assert.That(v.OfferCard.sprite.name,Is.EqualTo("panel_body_r3"));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);Assert.That(root.GetComponentsInChildren<AnimatedHotspot>(true),Is.Empty);
            foreach(var dep in AssetDatabase.GetDependencies(Path,true)){Assert.That(dep,Does.Not.Contain("ReferenceOnly"));Assert.That(dep,Does.Not.Contain("DesignScreens"));Assert.That(dep,Does.Not.Contain("Wave02"));}
            foreach(var image in root.GetComponentsInChildren<Image>(true))Assert.That(image.sprite,Is.Not.Null,image.name);
        }
        [Test]public void RepeatedBuildPreservesObjectsAndNeverInventsAnOffer()
        {
            var root=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Path));
            try
            {
                int count=root.GetComponentsInChildren<Transform>(true).Length;
                ProductionSpecialOfferRuntime.BuildInto((RectTransform)root.transform);ProductionSpecialOfferRuntime.BuildInto((RectTransform)root.transform);
                Assert.That(root.GetComponentsInChildren<Transform>(true).Length,Is.EqualTo(count));
                var v=root.GetComponent<ProductionSpecialOfferView>();Assert.That(v.CanPurchase,Is.False);Assert.That(v.PurchaseButton.interactable,Is.False);
                foreach(var amount in v.RewardAmounts)Assert.That(amount.text,Is.EqualTo("—"));
                Assert.That(ProductionSpecialOfferView.OfferId,Is.EqualTo("starter"));
                foreach(var t in root.GetComponentsInChildren<Text>(true)){Assert.That(t.text,Does.Not.Contain("₽"));Assert.That(t.text,Does.Not.Contain("%"));Assert.That(t.text,Does.Not.Contain("02:48"));}
            }
            finally{Object.DestroyImmediate(root);}
        }
    }
}
#endif
