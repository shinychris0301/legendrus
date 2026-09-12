#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public sealed class LockedDesignImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/LegendsOfRus/")) return;

            var ti = (TextureImporter)assetImporter;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.alphaIsTransparency = true;
            // Runtime art is never sampled from CPU code. Keeping Read/Write
            // disabled avoids an unnecessary CPU-addressable texture copy.
            ti.isReadable = false;
            ti.mipmapEnabled = false;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;

            // Splash R3 uses the exact supplied source PNGs for visual review.
            // Do not let the generic 2048/ASTC rule resize the 2172px control artwork.
            // This override is deliberately limited to the reconstructed Splash folder.
            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/"))
            {
                ti.maxTextureSize = 4096;
                // These are display-colour PNGs, never linear data/masks.
                // Keep the project's existing global colour-space choice unchanged.
                ti.sRGBTexture = true;
                ti.npotScale = TextureImporterNPOTScale.None;
                ti.spritePixelsPerUnit = 100;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                var settings = new TextureImporterSettings();
                ti.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                ti.SetTextureSettings(settings);
                var splashAndroid = ti.GetPlatformTextureSettings("Android");
                splashAndroid.overridden = true;
                splashAndroid.maxTextureSize = 4096;
                splashAndroid.format = TextureImporterFormat.RGBA32;
                ti.SetPlatformTextureSettings(splashAndroid);
                return;
            }

            if (assetPath.Contains("/ReferenceOnly/") || assetPath.Contains("/Resources/DesignScreens/"))
            {
                ti.maxTextureSize = 2048;
                ti.spritePixelsPerUnit = 100;
            }
            else
            {
                ti.maxTextureSize = 2048;
                ti.spritePixelsPerUnit = 100;
            }

            var android = ti.GetPlatformTextureSettings("Android");
            android.overridden = true;
            android.maxTextureSize = 2048;
            android.format = TextureImporterFormat.ASTC_6x6;
            android.compressionQuality = 70;
            ti.SetPlatformTextureSettings(android);
        }
    }
}
#endif
