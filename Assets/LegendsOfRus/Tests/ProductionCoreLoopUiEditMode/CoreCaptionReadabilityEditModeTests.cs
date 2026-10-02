#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class CoreCaptionReadabilityEditModeTests
    {
        [TestCase("Gameplay", true)] [TestCase("Gameplay", false)]
        [TestCase("Victory", true)] [TestCase("Victory", false)]
        public void CaptionOwnersPreserveNativeObjectsControlsAndFontsOnRepeatedBuild(string page, bool fresh)
        {
            var root = fresh ? new GameObject("Fresh" + page, typeof(RectTransform))
                : Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/LegendsOfRus/Resources/ProductionUI/Pages/" + page + ".prefab"));
            try
            {
                Build(root, page);
                var components = root.GetComponentsInChildren<Component>(true);
                var buttons = root.GetComponentsInChildren<Button>(true);
                var targets = buttons.Select(button => button.targetGraphic).ToArray();
                var texts = root.GetComponentsInChildren<Text>(true);
                var fonts = texts.Select(text => text.font).ToArray();
                for (int repeat = 0; repeat < 2; repeat++)
                {
                    Build(root, page);
                    CollectionAssert.AreEquivalent(components, root.GetComponentsInChildren<Component>(true));
                    CollectionAssert.AreEqual(buttons, root.GetComponentsInChildren<Button>(true));
                    CollectionAssert.AreEqual(targets, buttons.Select(button => button.targetGraphic));
                    CollectionAssert.AreEquivalent(texts, root.GetComponentsInChildren<Text>(true));
                    CollectionAssert.AreEqual(fonts, texts.Select(text => text.font));
                }
                Assert.That(root.GetComponentsInChildren<Canvas>(true), Is.Empty);
                if (page == "Gameplay")
                {
                    var view = root.GetComponent<ProductionGameplayView>();
                    Assert.That(view.HasRequiredBindings, Is.True);
                    Assert.That(view.Revision, Is.EqualTo(ProductionGameplayView.CurrentRevision));
                    for (int i = 0; i < 4; i++)
                    {
                        var card = view.Footer.Find("BoosterCard_" + i);
                        Assert.That(card.Cast<Transform>().Count(child => child.name == "CaptionBacking"), Is.EqualTo(1));
                        var label = card.Find("BoosterLabel").GetComponent<Text>();
                        Assert.That(label.fontSize, Is.EqualTo(46));
                        Assert.That(label.resizeTextForBestFit, Is.False);
                        Assert.That(label.GetComponent<ProductionGameplayCaptionFit>().ExpandedCaption, Is.True);
                        Assert.That(card.GetComponent<ProductionGameplayBoosterCard>().Target, Is.SameAs(view.BoosterButtons[i]));
                    }
                }
                else
                {
                    var view = root.GetComponent<ProductionVictoryView>();
                    Assert.That(view.HasRequiredBindings, Is.True);
                    Assert.That(view.Revision, Is.EqualTo(ProductionVictoryView.CurrentRevision));
                    Assert.That(view.RewardsPanel.sprite, Is.SameAs(ProductionVictoryReferenceStyle.Load("panel")));
                    Assert.That(view.RewardsHeading.enabled, Is.True);
                    Assert.That(view.RewardsArea.Find("ScoreHeading").GetComponent<Text>().enabled, Is.True);
                    Assert.That(view.RewardsArea.Find("BoosterHeading").GetComponent<Text>().enabled, Is.True);
                }
            }
            finally { Object.DestroyImmediate(root); }
        }
        static void Build(GameObject root, string page)
        {
            if (page == "Gameplay") ProductionGameplayRuntime.BuildInto((RectTransform)root.transform);
            else ProductionVictoryRuntime.BuildInto((RectTransform)root.transform);
        }
    }
}
#endif
