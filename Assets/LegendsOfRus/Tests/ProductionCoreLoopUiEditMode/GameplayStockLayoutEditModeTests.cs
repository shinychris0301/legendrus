#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class GameplayStockLayoutEditModeTests
    {
        [TestCase(true)] [TestCase(false)]
        public void FreshAndInstalledOwnersRetainIdentityAndExactCountsAcrossStockTransitions(bool fresh)
        {
            var root = fresh ? new GameObject("StockOwner", typeof(RectTransform))
                : Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab"));
            try
            {
                ProductionGameplayRuntime.BuildInto((RectTransform)root.transform);
                var v = root.GetComponent<ProductionGameplayView>();
                var components = root.GetComponentsInChildren<Component>(true);
                var buttons = v.BoosterButtons.ToArray();
                var fonts = v.BoosterCounts.Select(c => c.font).ToArray();
                foreach (string value in new[] { "999", "1 000", "9 999", "2 147 483 647", "0" })
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var count = v.BoosterCounts[i];
                        var stock = count.GetComponent<ProductionGameplayStockLayout>();
                        Assert.That(stock, Is.Not.Null);
                        count.text = value; stock.ApplyNow();
                        bool wide = value.Length > 3;
                        Assert.That(stock.Badge.enabled, Is.EqualTo(!wide));
                        Assert.That(stock.WideBacking.enabled, Is.EqualTo(wide));
                        Assert.That(stock.WideBacking.raycastTarget, Is.False);
                        Assert.That(count.text, Is.EqualTo(value), "Presentation must never abbreviate or round inventory.");
                        Assert.That(count.font, Is.SameAs(fonts[i]));
                        Assert.That(count.transform.parent, Is.SameAs(stock.Badge.transform));
                        Assert.That(stock.Badge.GetComponentsInChildren<ProductionGameplayStockLayout>(true).Length, Is.EqualTo(1));
                    }
                    ProductionGameplaySourceControls.ApplyReadability(v);
                    CollectionAssert.AreEquivalent(components, root.GetComponentsInChildren<Component>(true));
                    CollectionAssert.AreEqual(buttons, v.BoosterButtons);
                }
                ProductionGameplayRuntime.BuildInto((RectTransform)root.transform);
                CollectionAssert.AreEquivalent(components, root.GetComponentsInChildren<Component>(true));
                Assert.That(v.HasRequiredBindings, Is.True);
                Assert.That(v.Revision, Is.EqualTo(24));
            }
            finally { Object.DestroyImmediate(root); }
        }
    }
}
#endif
