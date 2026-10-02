#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class FreshPageBuilderEditModeTests
    {
        const string PageRoot="Assets/LegendsOfRus/Resources/ProductionUI/Pages/";
        static GameObject Prefab(string page)=>AssetDatabase.LoadAssetAtPath<GameObject>(PageRoot+page+".prefab");
        static Component View(GameObject root,string page)=>root.GetComponent(Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true));
        static void Build(GameObject root,string page)
        {
            var rect=(RectTransform)root.transform;
            switch(page)
            {
                case "WorldMap":ProductionWorldMapRuntime.BuildInto(rect);break;
                case "ChapterMap":ProductionChapterMapRuntime.BuildInto(rect);break;
                case "PreLevel":ProductionPreLevelRuntime.BuildInto(rect);break;
                default:throw new ArgumentException(page);
            }
        }
        static void AssertValid(GameObject root,string page)
        {
            var view=View(root,page);
            Assert.That((bool)view.GetType().GetProperty("HasRequiredBindings").GetValue(view),Is.True,page);
            Assert.That(view.GetType().GetField("Revision").GetValue(view),Is.EqualTo(view.GetType().GetField("CurrentRevision").GetRawConstantValue()));
            var header=(ProductionPlayerHeader)view.GetType().GetField("Header").GetValue(view);
            Assert.That(header.HasBindings,Is.True);
            foreach(var component in new Component[]{header.Profile,header.Settings,header.PlayerName,header.Level}.Concat(header.Balances).Concat(header.Plus))
                Assert.That(component.transform.IsChildOf(root.transform),Is.True,component.name+" must belong to the constructed page");
            Assert.That(header.Balances.Distinct().Count(),Is.EqualTo(3));
            Assert.That(header.Plus.Distinct().Count(),Is.EqualTo(3));
            Assert.That(root.GetComponentsInChildren<Canvas>(true),Is.Empty);
            Assert.That(root.GetComponentsInChildren<EventSystem>(true),Is.Empty);
        }
        static void SameRect(RectTransform a,RectTransform b)
        {
            Assert.That(a.anchorMin,Is.EqualTo(b.anchorMin),a.name);Assert.That(a.anchorMax,Is.EqualTo(b.anchorMax),a.name);
            Assert.That(a.pivot,Is.EqualTo(b.pivot),a.name);Assert.That(a.anchoredPosition,Is.EqualTo(b.anchoredPosition),a.name);
            Assert.That(a.sizeDelta,Is.EqualTo(b.sizeDelta),a.name);Assert.That(a.localScale,Is.EqualTo(b.localScale),a.name);
        }
        [TestCase("WorldMap")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void FreshPageMatchesInstalledHeaderAndControls(string page)
        {
            var reference=UnityEngine.Object.Instantiate(Prefab(page));
            var fresh=new GameObject("Fresh_"+page,typeof(RectTransform));
            try
            {
                ((RectTransform)fresh.transform).sizeDelta=((RectTransform)reference.transform).sizeDelta;
                Build(fresh,page);Build(reference,page);AssertValid(fresh,page);AssertValid(reference,page);
                var a=View(fresh,page);var b=View(reference,page);
                var left=(ProductionPlayerHeader)a.GetType().GetField("Header").GetValue(a);
                var right=(ProductionPlayerHeader)b.GetType().GetField("Header").GetValue(b);
                foreach(var pair in left.Capsules.Concat(new[]{left.Avatar,left.Backing}).Zip(right.Capsules.Concat(new[]{right.Avatar,right.Backing}),(x,y)=>(x,y)))
                {
                    Assert.That(pair.x.sprite,Is.SameAs(pair.y.sprite));Assert.That(pair.x.color,Is.EqualTo(pair.y.color));SameRect(pair.x.rectTransform,pair.y.rectTransform);
                }
                foreach(var pair in left.Balances.Concat(new[]{left.PlayerName,left.Level,left.Experience}).Zip(right.Balances.Concat(new[]{right.PlayerName,right.Level,right.Experience}),(x,y)=>(x,y)))
                {
                    Assert.That(pair.x.font,Is.SameAs(pair.y.font));Assert.That(pair.x.fontSize,Is.EqualTo(pair.y.fontSize));
                    Assert.That(pair.x.text,Is.EqualTo(pair.y.text));SameRect(pair.x.rectTransform,pair.y.rectTransform);
                }
                // Compare the public navigation/control slots by purpose, independently of hierarchy names.
                foreach(var field in a.GetType().GetFields().Where(f=>f.FieldType==typeof(Button)||f.FieldType==typeof(Button[])))
                {
                    var xs=field.FieldType==typeof(Button)?new[]{(Button)field.GetValue(a)}:(Button[])field.GetValue(a);
                    var ys=field.FieldType==typeof(Button)?new[]{(Button)field.GetValue(b)}:(Button[])field.GetValue(b);
                    Assert.That(xs?.Length,Is.EqualTo(ys?.Length),field.Name);
                    if(xs==null)continue;
                    for(int i=0;i<xs.Length;i++)
                    {
                        Assert.That(xs[i]!=null,Is.EqualTo(ys[i]!=null),field.Name);if(xs[i]==null)continue;
                        Assert.That(xs[i].targetGraphic,Is.Not.Null,field.Name);
                        SameRect((RectTransform)xs[i].transform,(RectTransform)ys[i].transform);
                        Assert.That(xs[i].targetGraphic.raycastTarget,Is.EqualTo(ys[i].targetGraphic.raycastTarget),field.Name);
                    }
                }
            }
            finally{UnityEngine.Object.DestroyImmediate(fresh);UnityEngine.Object.DestroyImmediate(reference);}
        }
        [TestCase("WorldMap")] [TestCase("ChapterMap")] [TestCase("PreLevel")]
        public void InstalledBuilderPreservesEveryComponentAndControlBinding(string page)
        {
            var root=UnityEngine.Object.Instantiate(Prefab(page));
            try
            {
                var original=root.GetComponentsInChildren<Component>(true);
                var buttons=root.GetComponentsInChildren<Button>(true);
                var targets=buttons.Select(b=>b.targetGraphic).ToArray();
                Build(root,page);Build(root,page);AssertValid(root,page);
                CollectionAssert.AreEquivalent(original,root.GetComponentsInChildren<Component>(true));
                CollectionAssert.AreEqual(targets,buttons.Select(b=>b.targetGraphic).ToArray());
                var owner=Type.GetType("LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder, Assembly-CSharp-Editor",true);
                Assert.That((bool)owner.GetMethod("PageNeedsBuild",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{Enum.Parse(typeof(ScreenId),page)}),Is.False);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
    }
}
#endif
