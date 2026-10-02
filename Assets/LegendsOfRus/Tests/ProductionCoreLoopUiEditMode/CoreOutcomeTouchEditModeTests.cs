#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class CoreOutcomeTouchEditModeTests
    {
        [TestCase("Gameplay","Fresh")] [TestCase("Victory","Fresh")] [TestCase("ChapterComplete","Fresh")]
        [TestCase("Gameplay","Installed")] [TestCase("Victory","Installed")] [TestCase("ChapterComplete","Installed")]
        public void CurrentOwnersKeepNativeIdentityAndInstallTouchTargetsExactlyOnce(string page,string mode)
        {
            var root=mode=="Fresh"?new GameObject("Fresh"+page,typeof(RectTransform)):Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+page+".prefab"));
            try
            {
                Build(root,page);var components=root.GetComponentsInChildren<Component>(true);var buttons=root.GetComponentsInChildren<Button>(true);var targets=buttons.Select(b=>b.targetGraphic).ToArray();var artwork=buttons.Select(b=>b.targetGraphic is Image i?i.sprite:null).ToArray();
                var touch=root.GetComponentsInChildren<ProductionControlTouchTarget>(true);Assert.That(touch.Length,Is.EqualTo(page=="Gameplay"?1:2));foreach(var t in touch){Assert.That(t.Owner,Is.Not.Null);Assert.That(t.transform.parent,Is.SameAs(t.Owner.transform));Assert.That(t.raycastTarget,Is.True);Assert.That(t.color.a,Is.Zero);Assert.That(t.Owner.targetGraphic,Is.Not.SameAs(t));}
                for(int n=0;n<2;n++){Build(root,page);CollectionAssert.AreEquivalent(components,root.GetComponentsInChildren<Component>(true));CollectionAssert.AreEqual(buttons,root.GetComponentsInChildren<Button>(true));CollectionAssert.AreEqual(targets,buttons.Select(b=>b.targetGraphic));CollectionAssert.AreEqual(artwork,buttons.Select(b=>b.targetGraphic is Image i?i.sprite:null));}
                Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);
            }
            finally{Object.DestroyImmediate(root);}
        }
        static void Build(GameObject root,string page)
        {
            if(page=="Gameplay")ProductionGameplayRuntime.BuildInto((RectTransform)root.transform);
            else if(page=="Victory")ProductionVictoryRuntime.BuildInto((RectTransform)root.transform);
            else ProductionChapterCompleteRuntime.BuildInto((RectTransform)root.transform);
        }
    }
}
#endif
