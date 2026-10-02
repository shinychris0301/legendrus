#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    // Disposable audit: only unsaved instances are mutated. No asset saves/import changes.
    public sealed class ProgressDependencyAuditEditModeTests
    {
        const string Root="Assets/LegendsOfRus/Resources/ProductionUI/";
        static string Output=>Environment.GetEnvironmentVariable("LOR_PROGRESS_AUDIT_DIR");
        static bool Retired(Image i)=>i.sprite!=null &&
            new[]{Root+"HomeAligned/progress_frame.png",Root+"HomeAligned/progress_fill.png"}.Contains(AssetDatabase.GetAssetPath(i.sprite));
        static Component View(GameObject go,string page)=>go.GetComponent(Type.GetType("LegendsOfRus.Production"+page+"View, LegendsOfRus.Runtime",true));
        static bool Valid(Component v)=>(bool)v.GetType().GetProperty("HasRequiredBindings").GetValue(v);
        static void Build(GameObject go,string page)
        {
            if(page=="Home") { ProductionHomeRuntime.BuildInto((RectTransform)go.transform);return; }
            Type.GetType("LegendsOfRus.Production"+page+"Runtime, LegendsOfRus.Runtime",true)
                .GetMethod("BuildInto",BindingFlags.Public|BindingFlags.Static).Invoke(null,new object[]{(RectTransform)go.transform});
        }
        static void Save(string name,object value)=>File.WriteAllText(Path.Combine(Output,name+".json"),JsonUtility.ToJson(value,true));
        [Serializable] public sealed class PageRecord
        {
            public string page; public string[] dependencies,images; public int revision;
            public bool currentValid,ownerNeedsBuild,clearedValid,repairedValid,identitiesPreserved;
            public int repairedTextureReferences;
        }
        [TestCase("Home",3)] [TestCase("WorldMap",1)] [TestCase("ChapterMap",1)]
        [TestCase("PreLevel",1)] [TestCase("Gameplay",4)] [TestCase("Fail",1)]
        public void ActivePrefabAndUnsavedSpriteRemoval(string page,int expected)
        {
            var path=Root+"Pages/"+page+".prefab";
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var view=View(prefab,page);
            var owner=Type.GetType("LegendsOfRus.Editor.Step20ProductionUiFix01SerializedPrefabBuilder, Assembly-CSharp-Editor",true);
            var record=new PageRecord{page=page,currentValid=Valid(view),
                revision=(int)view.GetType().GetField("Revision").GetValue(view),
                dependencies=AssetDatabase.GetDependencies(path,true).Where(p=>p==Root+"HomeAligned/progress_fill.png"||p==Root+"HomeAligned/progress_frame.png").OrderBy(p=>p).ToArray(),
                ownerNeedsBuild=(bool)owner.GetMethod("PageNeedsBuild",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{Enum.Parse(typeof(ScreenId),page)})};
            Assert.That(record.currentValid,Is.True);Assert.That(record.ownerNeedsBuild,Is.False);
            Assert.That(record.revision,Is.EqualTo(view.GetType().GetField("CurrentRevision").GetRawConstantValue()));
            Assert.That(record.dependencies.Length,Is.EqualTo(page=="Home"||page=="Gameplay"?2:1));
            var clone=UnityEngine.Object.Instantiate(prefab);
            try
            {
                var images=clone.GetComponentsInChildren<Image>(true).Where(Retired).ToArray();
                Assert.That(images.Length,Is.EqualTo(expected));
                record.images=images.Select(i=>AnimationUtility.CalculateTransformPath(i.transform,clone.transform)).ToArray();
                Assert.That(images.All(i=>!i.enabled||!i.gameObject.activeInHierarchy),Is.True);
                var objects=clone.GetComponentsInChildren<Transform>(true).ToArray();
                foreach(var i in images)i.sprite=null;
                record.clearedValid=Valid(View(clone,page));
                if(page=="Home")ProductionHomeRuntime.UpgradeReferenceForeground(clone.GetComponent<ProductionHomeView>(),Resources.Load<Sprite>);
                Build(clone,page);
                record.repairedValid=Valid(View(clone,page));
                record.repairedTextureReferences=clone.GetComponentsInChildren<Image>(true).Count(Retired);
                record.identitiesPreserved=objects.SequenceEqual(clone.GetComponentsInChildren<Transform>(true));
                Save("Prefab_"+page,record);
                Assert.That(record.clearedValid&&record.repairedValid,Is.True);
                Assert.That(record.repairedTextureReferences,Is.Zero);
                Assert.That(record.identitiesPreserved,Is.True);
            }
            finally{UnityEngine.Object.DestroyImmediate(clone);}
        }
        [Serializable] public sealed class FreshRecord
        {
            public string page,error;public bool valid,activeRoot,validAfterPreLevelComposition;
            public int textureReferences;public string[] nullObjectFields,headerTextPaths;
        }
        [TestCase("Home",3)] [TestCase("WorldMap",1)] [TestCase("ChapterMap",1)]
        [TestCase("PreLevel",1)] [TestCase("Gameplay",4)] [TestCase("Fail",1)]
        public void FreshBuildersReintroduceDormantTextures(string page,int expected)
        {
            var root=new GameObject("Audit_"+page,typeof(RectTransform));
            try
            {
                // Active-root probe rules out disabled hierarchy setup as the failure cause.
                try { Build(root,page); }
                catch(Exception error)
                {
                    var v=View(root,page);
                    var failure=new FreshRecord{page=page,error=(error.InnerException??error).ToString(),activeRoot=root.activeSelf,
                        valid=v!=null&&Valid(v),textureReferences=root.GetComponentsInChildren<Image>(true).Count(Retired),
                        nullObjectFields=v.GetType().GetFields().Where(f=>typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType)&&(UnityEngine.Object)f.GetValue(v)==null).Select(f=>f.Name).ToArray(),
                        headerTextPaths=root.GetComponentsInChildren<Text>(true).Where(t=>t.name.Contains("Value")).Select(t=>AnimationUtility.CalculateTransformPath(t.transform,root.transform)).ToArray()};
                    if(page=="PreLevel")
                    {
                        ProductionPreLevelSourceControls.Apply((ProductionPreLevelView)v);
                        failure.validAfterPreLevelComposition=Valid(v);
                    }
                    Save("FreshFailure_"+page,failure);
                    throw;
                }
                var record=new FreshRecord{page=page,valid=Valid(View(root,page)),textureReferences=root.GetComponentsInChildren<Image>(true).Count(Retired)};
                Save("Fresh_"+page,record);
                Assert.That(record.valid,Is.True);Assert.That(record.textureReferences,Is.EqualTo(expected));
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [TestCase("progress_fill")] [TestCase("progress_frame")]
        public void HomeColdBuildRejectsMissingRetiredResource(string missing)
        {
            var root=new GameObject("Audit_Missing",typeof(RectTransform));
            try
            {
                root.SetActive(false);
                var error=Assert.Throws<InvalidOperationException>(()=>ProductionHomeRuntime.BuildInto((RectTransform)root.transform,
                    key=>key==ProductionHomeRuntime.ArtRoot+missing?null:Resources.Load<Sprite>(key)));
                StringAssert.Contains(ProductionHomeRuntime.ArtRoot+missing,error.Message);
                File.WriteAllText(Path.Combine(Output,"Missing_"+missing+".txt"),error.Message);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void GameplayStillRequiresHiddenStateComponents()
        {
            var clone=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Pages/Gameplay.prefab"));
            try
            {
                var v=clone.GetComponent<ProductionGameplayView>();
                v.GoalProgress.sprite=null;v.StarProgress.sprite=null;
                Assert.That(v.HasRequiredBindings,Is.True);
                var star=v.StarProgress;v.StarProgress=null;Assert.That(v.HasRequiredBindings,Is.False);v.StarProgress=star;
                var goal=v.GoalProgress;v.GoalProgress=null;Assert.That(v.HasRequiredBindings,Is.False);v.GoalProgress=goal;
                Assert.That(v.HasRequiredBindings,Is.True);
            }
            finally{UnityEngine.Object.DestroyImmediate(clone);}
        }
        [Test] public void LoadingPreparationUsesIndependentProgressArtwork()
        {
            Assert.That(ProductionHomeRuntime.GetTextureResourceKeys().Any(k=>k==ProductionHomeRuntime.ArtRoot+"progress_fill"||k==ProductionHomeRuntime.ArtRoot+"progress_frame"),Is.False);
            var dependencies=AssetDatabase.GetDependencies(Root+"Pages/Loading.prefab",true);
            Assert.That(dependencies.Any(p=>p==Root+"HomeAligned/progress_fill.png"||p==Root+"HomeAligned/progress_frame.png"),Is.False);
        }
    }
}
#endif
