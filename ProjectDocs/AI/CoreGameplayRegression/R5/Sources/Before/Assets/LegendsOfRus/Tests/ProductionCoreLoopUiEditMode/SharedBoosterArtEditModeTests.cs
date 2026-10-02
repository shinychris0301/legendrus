#if UNITY_EDITOR
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Tests.ProductionCoreLoopUi
{
    public sealed class SharedBoosterArtEditModeTests
    {
        [Test]public void CanonicalPagesHaveNoSupersededBoosterSpriteReferences()
        {
            foreach(var file in Directory.GetFiles("Assets/LegendsOfRus/Resources/ProductionUI/Pages","*.prefab"))
            foreach(var component in AssetDatabase.LoadAssetAtPath<GameObject>(file).GetComponentsInChildren<Component>(true))
            {
                if(component==null)continue;
                var so=new SerializedObject(component);var p=so.GetIterator();
                while(p.Next(true))if(p.propertyType==SerializedPropertyType.ObjectReference&&p.objectReferenceValue is Sprite sprite)
                {
                    var path=AssetDatabase.GetAssetPath(sprite);const string prefix="Assets/LegendsOfRus/Resources/";
                    if(!path.StartsWith(prefix)||!path.EndsWith(".png"))continue;
                    Assert.That(ProductionBoosterArt.Replacement(path.Substring(prefix.Length,path.Length-prefix.Length-4)),Is.Null,file+" "+component.name+" "+path);
                }
            }
        }
        [Test]public void SharedAssetsAreUncompressedAndRuntimeMappingsAgree()
        {
            for(int i=0;i<8;i++)foreach(bool framed in new[]{false,true})
            {
                var sprite=ProductionBoosterArt.Icon(i,framed);var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite));
                Assert.That(importer.mipmapEnabled,Is.False);Assert.That(importer.alphaIsTransparency,Is.True);Assert.That(importer.textureCompression,Is.EqualTo(TextureImporterCompression.Uncompressed));
            }
            for(int i=0;i<4;i++)
            {
                Assert.That(ProductionGameplayArt.Booster(i),Is.SameAs(ProductionBoosterArt.Icon(i)));
                Assert.That(ProductionPreLevelSourceControls.Load(ProductionBoosterArt.Names[i]),Is.SameAs(ProductionBoosterArt.Icon(i,true)));
                Assert.That(ProductionPauseRuntime.Load(ProductionBoosterArt.Names[i]+"_r2"),Is.SameAs(ProductionBoosterArt.Icon(i,true)));
                Assert.That(ProductionInventoryRuntime.ItemArt(i+20),Is.SameAs(ProductionBoosterArt.Icon(i,true)));
            }
            Assert.That(ProductionBoosterArt.Replacement("GameplayAligned/gem_gold_r8"),Is.Null);
            Assert.That(ProductionBoosterArt.Replacement("CharactersAligned/hero_firebird_r7"),Is.Null);
        }
    }
}
#endif
