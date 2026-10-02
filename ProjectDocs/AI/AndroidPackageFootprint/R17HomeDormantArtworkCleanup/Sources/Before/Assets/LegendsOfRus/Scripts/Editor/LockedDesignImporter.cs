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
            // Archived Home art retains its original import contract and GUIDs, outside Resources.
            if (assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredProgress/progress_fill.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredProgress/progress_frame.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/home_firebird_generated.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/home_hero_firebird.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/home_environment_generated.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/home_play_button.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/hud_frame.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/quest_panel.png" ||
                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/footer_base_generated.png")
            {
                ConfigureHomeAlignmentTexture(ti, System.IO.Path.GetFileNameWithoutExtension(assetPath));
                return;
            }

            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.alphaIsTransparency = true;
            // Runtime art is never sampled from CPU code. Keeping Read/Write
            // disabled avoids an unnecessary CPU-addressable texture copy.
            ti.isReadable = false;
            ti.mipmapEnabled = false;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;

            if(assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/"))
            {
                ConfigureFailTexture(ti);return;
            }

            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/"))
            { ConfigureChapterCompleteTexture(ti);return; }
            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/ShopAligned/") ||
                assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/SpecialOfferAligned/") ||
                assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned/"))
            { ConfigureShopTexture(ti);return; }
            if(assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/IndividualRankingAligned/"))
            {
                ConfigureShopTexture(ti);string name=System.IO.Path.GetFileNameWithoutExtension(assetPath);
                if(name=="row")ti.spriteBorder=new Vector4(10,7,10,7);
                if(name=="row_player")ti.spriteBorder=new Vector4(18,10,18,10);
                if(name=="reward_frame")ti.spriteBorder=new Vector4(48,48,48,48);
                if(name=="reward_panel"||name=="table_frame")ti.spriteBorder=new Vector4(80,80,80,80);
                return;
            }
            if(assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/EventsAligned/"))
            {
                ConfigureShopTexture(ti);
                if(System.IO.Path.GetFileNameWithoutExtension(assetPath)=="frame")ti.spriteBorder=new Vector4(64,64,64,64);
                string name=System.IO.Path.GetFileNameWithoutExtension(assetPath);
                if(name=="frame_r3"||name=="frame_ornaments_r3")ti.spriteBorder=new Vector4(40,40,40,40);
                if(name=="tab_r3")ti.spriteBorder=new Vector4(28,20,28,20);
                if(name=="status_r3")ti.spriteBorder=new Vector4(10,8,12,8);
                return;
            }
            // Pause's extracted lettering/filigree retains source pixels on every platform.
            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/PauseAligned/"))
            {
                ConfigureFailTexture(ti);return;
            }

            if(assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/VictoryReferenceR1/"))
            { ConfigureVictoryReferenceTexture(ti);return; }

            // Victory's versioned sources retain native pixels and transparent edges.
            // Limit this contract to the new assets; earlier page imports stay unchanged.
            if (assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/environment_r3.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/firebird_r3.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/resource_header_r3.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/banner_r4.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/continue_r5.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/continue_r7.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/firebird_r7.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/video_r5.png" ||
                assetPath == "Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/footer_r5.png")
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                ConfigureWorldMapTexture(ti, name == "environment_r3" ? "environment" : name);
                return;
            }

            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/"))
            {
                GameplayAlignmentArtImporter.Configure(ti, System.IO.Path.GetFileNameWithoutExtension(assetPath));
                return;
            }

            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/PreLevelAligned/"))
            {
                ConfigureWorldMapTexture(ti, System.IO.Path.GetFileNameWithoutExtension(assetPath));
                return;
            }

            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapAligned/"))
            {
                ConfigureWorldMapTexture(ti, System.IO.Path.GetFileNameWithoutExtension(assetPath));
                return;
            }

            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/WorldMapAligned/"))
            {
                ConfigureWorldMapTexture(ti, System.IO.Path.GetFileNameWithoutExtension(assetPath));
                return;
            }

            // Home's generated controls keep their isolated edges and intentional navy surfaces.
            // Both automatic import and the Home-only owner use this narrowly scoped contract.
            if (assetPath.StartsWith("Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/"))
            {
                ConfigureHomeAlignmentTexture(ti, System.IO.Path.GetFileNameWithoutExtension(assetPath));
                return;
            }

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

        internal static void ConfigureVictoryReferenceTexture(TextureImporter ti)
        {
            string name=System.IO.Path.GetFileNameWithoutExtension(ti.assetPath);
            ConfigureWorldMapTexture(ti,name);ti.mipmapEnabled=false;ti.filterMode=FilterMode.Bilinear;
            ti.spriteBorder=name=="card"||name=="resource_card"?new Vector4(14,14,14,14):Vector4.zero;
        }

        internal static void ConfigureShopTexture(TextureImporter ti)
        {
            ConfigureFailTexture(ti);ti.mipmapEnabled=false;ti.filterMode=FilterMode.Bilinear;
        }

        internal static void ConfigureChapterCompleteTexture(TextureImporter ti)
        {
            ConfigureFailTexture(ti);
            if(System.IO.Path.GetFileNameWithoutExtension(ti.assetPath)=="footer")
            {
                ti.spriteBorder=new Vector4(80,80,80,80);
            }
        }

        internal static void ConfigureFailTexture(TextureImporter ti)
        {
            ti.textureType=TextureImporterType.Sprite;ti.spriteImportMode=SpriteImportMode.Single;
            ti.alphaSource=TextureImporterAlphaSource.FromInput;ti.alphaIsTransparency=true;ti.isReadable=false;
            ti.spritePixelsPerUnit=100;ti.wrapMode=TextureWrapMode.Clamp;ti.filterMode=FilterMode.Trilinear;
            ti.mipmapEnabled=!System.IO.Path.GetFileName(ti.assetPath).StartsWith("environment_");ti.maxTextureSize=2048;
            ti.textureCompression=TextureImporterCompression.Uncompressed;
            var settings=new TextureImporterSettings();ti.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;ti.SetTextureSettings(settings);
            var android=ti.GetPlatformTextureSettings("Android");android.overridden=true;android.maxTextureSize=2048;android.format=TextureImporterFormat.RGBA32;ti.SetPlatformTextureSettings(android);
        }

        internal static void ConfigureWorldMapTexture(TextureImporter ti, string name)
        {
            ti.textureType = TextureImporterType.Sprite;
            ti.textureShape = TextureImporterShape.Texture2D;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.alphaSource = TextureImporterAlphaSource.FromInput;
            ti.alphaIsTransparency = true;
            ti.maxTextureSize = 4096;
            ti.sRGBTexture = true;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.spritePixelsPerUnit = 100;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.isReadable = false;
            ti.wrapMode = TextureWrapMode.Clamp;
            bool minifiedControl = name != "environment";
            ti.mipmapEnabled = minifiedControl;
            ti.mipmapFilter = TextureImporterMipFilter.BoxFilter;
            ti.filterMode = minifiedControl ? FilterMode.Trilinear : FilterMode.Bilinear;
            var settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteGenerateFallbackPhysicsShape = false;
            settings.spriteBorder = Vector4.zero;
            ti.SetTextureSettings(settings);
            var android = ti.GetPlatformTextureSettings("Android");
            android.overridden = true;
            android.maxTextureSize = 4096;
            android.format = TextureImporterFormat.RGBA32;
            android.textureCompression = TextureImporterCompression.Uncompressed;
            ti.SetPlatformTextureSettings(android);
        }

        internal static void ConfigureHomeAlignmentTexture(TextureImporter ti, string name)
        {
            bool minifiedControl = name != "home_environment_generated";
            // Every Home foreground is an individually generated isolated sprite. Retain
            // all source pixels and alpha, including intentionally opaque panel interiors.
            bool hasAlpha = minifiedControl;
            int maxSize = minifiedControl ? 4096 : 2048;
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.alphaSource = TextureImporterAlphaSource.FromInput;
            ti.alphaIsTransparency = hasAlpha;
            ti.maxTextureSize = maxSize;
            ti.sRGBTexture = true;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.spritePixelsPerUnit = 100;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.isReadable = false;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.mipmapEnabled = minifiedControl;
            ti.mipmapFilter = TextureImporterMipFilter.BoxFilter;
            ti.filterMode = minifiedControl ? FilterMode.Trilinear : FilterMode.Bilinear;
            var settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            settings.spriteGenerateFallbackPhysicsShape = false;
            // Home sources contain complete ornamented frames at their final proportions.
            // Do not stretch corner details using the previous generic nine-slice borders.
            settings.spriteBorder = Vector4.zero;
            ti.SetTextureSettings(settings);
            var android = ti.GetPlatformTextureSettings("Android");
            android.overridden = true;
            android.maxTextureSize = maxSize;
            android.format = hasAlpha ? TextureImporterFormat.RGBA32 : TextureImporterFormat.RGB24;
            android.textureCompression = TextureImporterCompression.Uncompressed;
            ti.SetPlatformTextureSettings(android);
        }
    }
}
#endif
