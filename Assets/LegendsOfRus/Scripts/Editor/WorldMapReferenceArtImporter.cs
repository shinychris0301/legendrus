#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    public static class WorldMapReferenceArtImporter
    {
        public static void Import()
        {
            const string directory="Assets/LegendsOfRus/Resources/ProductionUI/WorldMapReferenceAligned";
            foreach(string file in Directory.GetFiles(directory,"*.png"))
            {
                AssetDatabase.ImportAsset(file,ImportAssetOptions.ForceSynchronousImport);
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.isReadable=false;
                importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;
                importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;
                var settings=new TextureImporterSettings();importer.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);importer.SaveAndReimport();
            }
        }
    }
}
#endif
