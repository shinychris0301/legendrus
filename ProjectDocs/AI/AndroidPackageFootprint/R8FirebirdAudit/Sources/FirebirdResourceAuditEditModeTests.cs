#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    // Temporary read-only audit. Builders operate on disposable instances only.
    public sealed class FirebirdResourceAuditEditModeTests
    {
        const string Candidate="Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/home_firebird_generated.png";
        const string Key="ProductionUI/HomeAligned/home_firebird_generated";
        const string Home="Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab";
        static string Output=>Environment.GetEnvironmentVariable("LOR_FIREBIRD_AUDIT_DIR");
        [Serializable] public sealed class Dependencies { public string[] owners,consumers; }
        [Serializable] public sealed class BuildRecord { public string mode;public string[] requested;public bool valid,firebirdInactive,firebirdEmpty;public string currentHero; }
        [Serializable] public sealed class Imported { public int width,height,mips;public string format,guid; }
        [Test] public void PackedCandidateStillLoadsButHasNoSerializedConsumers()
        {
            var sprite=Resources.Load<Sprite>(Key);Assert.That(sprite,Is.Not.Null);
            var record=new Imported{width=sprite.texture.width,height=sprite.texture.height,mips=sprite.texture.mipmapCount,
                format=sprite.texture.format.ToString(),guid=AssetDatabase.AssetPathToGUID(Candidate)};
            Assert.That(record.guid,Is.EqualTo("072000aa439cc61468feba1c5d67b108"));
            File.WriteAllText(Path.Combine(Output,"Imported.json"),JsonUtility.ToJson(record,true));
            var extensions=new[]{".prefab",".unity",".asset",".mat",".controller",".overrideController",".playable",".spriteatlas",".anim"};
            var owners=AssetDatabase.GetAllAssetPaths().Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)&&extensions.Contains(Path.GetExtension(p))).OrderBy(p=>p).ToArray();
            var consumers=owners.Where(p=>AssetDatabase.GetDependencies(p,true).Contains(Candidate)).ToArray();
            File.WriteAllText(Path.Combine(Output,"Dependencies.json"),JsonUtility.ToJson(new Dependencies{owners=owners,consumers=consumers},true));
            Assert.That(owners.Length,Is.GreaterThan(90));Assert.That(consumers,Is.Empty);
        }
        [TestCase("Fresh")] [TestCase("LegacyMigration")] [TestCase("CurrentMigration")]
        public void HomeConstructionDoesNotRequestCandidate(string mode)
        {
            var requests=new List<string>();
            Func<string,Sprite> loader=key=>{requests.Add(key);Assert.That(key,Is.Not.EqualTo(Key));return key==Key?null:Resources.Load<Sprite>(key);};
            var root=mode=="Fresh"?new GameObject("AuditHome",typeof(RectTransform)):UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Home));
            try
            {
                if(mode=="Fresh")ProductionHomeRuntime.BuildInto((RectTransform)root.transform,loader);
                else
                {
                    var old=root.GetComponent<ProductionHomeView>();
                    if(mode=="LegacyMigration")old.SourcePlay=null;
                    ProductionHomeRuntime.UpgradeReferenceForeground(old,loader);
                }
                var view=root.GetComponent<ProductionHomeView>();
                var record=new BuildRecord{mode=mode,requested=requests.ToArray(),valid=view.HasRequiredBindings,
                    firebirdInactive=!view.Firebird.gameObject.activeSelf,firebirdEmpty=view.Firebird.sprite==null,
                    currentHero=AssetDatabase.GetAssetPath(view.Ivan.sprite)};
                File.WriteAllText(Path.Combine(Output,mode+".json"),JsonUtility.ToJson(record,true));
                Assert.That(record.valid&&record.firebirdInactive&&record.firebirdEmpty,Is.True);
                Assert.That(record.currentHero,Is.EqualTo("Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/source_hero.png"));
                Assert.That(ProductionHomeRuntime.GetTextureResourceKeys(),Does.Not.Contain(Key));
                Assert.That(root.GetComponentsInChildren<Image>(true).Any(i=>i.sprite!=null&&AssetDatabase.GetAssetPath(i.sprite)==Candidate),Is.False);
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void ApprovedSplashAndLoadingUseOtherResources()
        {
            foreach(string page in new[]{"Home","SplashTitle","Loading"})
            {
                string path="Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+page+".prefab";
                var dependencies=AssetDatabase.GetDependencies(path,true);
                Assert.That(dependencies,Does.Not.Contain(Candidate),page);
                var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Assert.That(prefab,Is.Not.Null);
                Assert.That(prefab.GetComponentsInChildren<Image>(true).Any(i=>i.sprite!=null),Is.True);
            }
            var home=AssetDatabase.LoadAssetAtPath<GameObject>(Home).GetComponent<ProductionHomeView>();
            Assert.That(home.HasRequiredBindings,Is.True);Assert.That(home.Firebird.sprite,Is.Null);
        }
    }
}
#endif
