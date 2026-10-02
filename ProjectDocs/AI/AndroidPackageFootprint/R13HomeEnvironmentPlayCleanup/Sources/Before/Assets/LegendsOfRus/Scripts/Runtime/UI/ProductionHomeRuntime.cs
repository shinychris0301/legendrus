using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Single owner of Home's native hierarchy and reference-sized foreground composition.</summary>
    public static class ProductionHomeRuntime
    {
        public const string ContentName = "HOME_ALIGNED_CONTENT";
        public const string ArtRoot = "ProductionUI/HomeAligned/";
        public const string PlayGoldMaterialResource = ArtRoot + "home_play_gold";
        public const string HeroLightingMaterialResource = ArtRoot + "home_hero_lighting";
        public const string FoliageLightingMaterialResource = ArtRoot + "home_foliage_lighting";
        public const string SceneLightingShader = "LegendsOfRus/UI/HomeSceneLighting";
        public const string TypographyResource = "ProductionUI/SplashTitleConfig/SplashTitleTypography";
        // Home-only neutral scene tone; alpha, source artwork and UI lighting stay intact.
        public static readonly Color EnvironmentTone = new Color32(235, 235, 235, 255);
        public static readonly Color HeroTone = new Color32(242, 242, 242, 255);
        static readonly string[] TextureNames = {
            "home_environment_generated", "nav_map_generated", "nav_shop_generated", "nav_events_generated",
            "nav_characters_generated", "mail_medallion_generated", "reward_medallion_generated",
            "home_logo", "home_play_button", "avatar_portrait", "hud_frame", "avatar_ring",
            "resource_capsule", "quest_panel", "coin_icon", "gems_icon", "energy_icon", "settings_medallion",
            "resource_plus", "quest_reward_frame", "quest_reward_gem", "footer_base_generated",
            "home_map_foliage"
        };
        static readonly string[] TextureKeys = Array.ConvertAll(ProductionHomeSourceComposition.Names,
            name => ProductionHomeSourceComposition.ArtRoot + name).Concat(ProductionHomeSourceComposition.ShortcutResources).ToArray();
        public static IReadOnlyList<string> GetTextureResourceKeys() => Array.AsReadOnly(TextureKeys);
        static readonly Color Gold = new Color32(232, 209, 162, 255);

        public static void BuildInto(RectTransform root, Func<string, Sprite> loadSprite = null, SplashTitleTypography typography = null)
        {
            ProductionHomeView existing = root.GetComponent<ProductionHomeView>();
            ProductionHomeLayout oldLayout = root.GetComponent<ProductionHomeLayout>();
            if (existing != null && existing.HasRequiredBindings && oldLayout != null && oldLayout.HasRequiredBindings) { ClearDormantProgress(existing); return; }
            if (root.Find(ContentName) != null)
                throw new InvalidOperationException("Home aligned bindings are incomplete; repair the existing Home prefab outside Play Mode.");
            if (loadSprite == null) loadSprite = Resources.Load<Sprite>;
            if (typography == null) typography = Resources.Load<SplashTitleTypography>(TypographyResource);
            if (typography == null) throw new InvalidOperationException("Home requires shared Splash typography.");
            Dictionary<string, Sprite> sprites = LoadSprites(loadSprite);
            ProductionHomeView view = existing != null ? existing : root.gameObject.AddComponent<ProductionHomeView>();
            view.Typography = typography;
            RectTransform content = Rect(root, ContentName, 0, 0, 941, 1672);
            content.anchorMin = Vector2.zero; content.anchorMax = Vector2.one;
            content.offsetMin = content.offsetMax = Vector2.zero;
            view.Environment = Art(content, "WinterVillageEnvironment", sprites["home_environment_generated"], 0, 0, 941, 1672);
            // Retain the historical object names so full builds and in-place migrations agree.
            view.Firebird = Art(content, "ApprovedFirebird", null, 495, 442, 550, 825);
            view.Firebird.gameObject.SetActive(false);
            view.Ivan = Art(content, "ApprovedIvan", null, -20, 474, 967, 967);
            view.LowerShade = Rect(content, "LowerContrast", 0, 982, 941, 690).gameObject.AddComponent<ProductionHomeShade>();
            view.LowerShade.color = Color.clear; view.LowerShade.raycastTarget = false;
            view.Logo = Art(content, "ApprovedSplashLogo", sprites["home_logo"], 205.5f, 110, 530, 530f * 1041 / 1511);
            view.HudFrame = Frame(content, "HomeHud", sprites["hud_frame"], 0, 0, 941, 130);
            RectTransform h = view.HudFrame.rectTransform;
            ProductionHomePanel profile = Panel(h, "ProfileButton", 8, 0, 320, 233, 0);
            profile.Frame = false; profile.LeftColor = profile.RightColor = Color.clear;
            view.ProfileButton = Button(profile);
            view.AvatarFrame = Art(profile.rectTransform, "PortraitFrame", sprites["avatar_ring"], -20, 0, 194, 194f * 1425 / 1104);
            view.ProfileButton.targetGraphic = view.AvatarFrame;
            view.HeroName = Label(profile.rectTransform, "HeroName", "Иван", 172, 28, 145, 42, 34, typography, TextAnchor.MiddleLeft);
            view.LevelText = Label(profile.rectTransform, "SavedLevel", "Уровень —", 172, 71, 145, 29, 24, typography, TextAnchor.MiddleLeft);
            view.GoldText = Resource(h, "Gold", sprites["resource_capsule"], sprites["coin_icon"], typography,
                out view.GoldCapsule, out view.GoldIcon);
            view.GemsText = Resource(h, "Gems", sprites["resource_capsule"], sprites["gems_icon"], typography,
                out view.GemsCapsule, out view.GemsIcon);
            view.EnergyText = Resource(h, "Energy", sprites["resource_capsule"], sprites["energy_icon"], typography,
                out view.EnergyCapsule, out view.EnergyIcon);
            view.SettingsButton = Button(Art(h, "SettingsButton", sprites["settings_medallion"], 853, 28, 85, 85));
            view.PlayButton = Button(Art(content, "PlayButton", sprites["home_play_button"], 185.5f, 1230 - 570f * 803 / 1959, 570, 570f * 803 / 1959));
            view.PlayButton.GetComponent<ProductionHomeButtonFeedback>().Role = MetaUiControlRole.Primary;
            view.PlayText = Label(view.PlayButton.transform as RectTransform, "PlayLabel", "ИГРАТЬ", 91, 570f * 803 / 1959 - 157, 388 / .92f, 106, 90, typography);
            RectTransform nav = Rect(content, "Navigation", 51, 1230, 839, 222);
            view.WorldMapButton = Nav(nav, "WorldMap", sprites["nav_map_generated"], "КАРТА МИРА", typography, out view.WorldMapText);
            view.ShopButton = Nav(nav, "Shop", sprites["nav_shop_generated"], "МАГАЗИН", typography, out view.ShopText);
            view.EventsButton = Nav(nav, "Events", sprites["nav_events_generated"], "СОБЫТИЯ", typography, out view.EventsText);
            view.CharactersButton = Nav(nav, "Characters", sprites["nav_characters_generated"], "ПЕРСОНАЖИ", typography, out view.CharactersText);
            RectTransform footer = Rect(content, "Footer", 0, 1450, 941, 222);
            view.MailButton = Medallion(footer, "MailButton", sprites["mail_medallion_generated"]);
            view.QuestFrame = Frame(footer, "QuestPanel", sprites["quest_panel"], 177, 0, 587, 210);
            view.QuestText = Label(view.QuestFrame.rectTransform, "QuestCopy", "Соберите синие самоцветы", 38, 31, 398, 43, 31, typography);
            view.DailyRewardButton = Medallion(footer, "DailyRewardButton", sprites["reward_medallion_generated"]);
            ProductionHomeLayout layout = oldLayout != null ? oldLayout : root.gameObject.AddComponent<ProductionHomeLayout>();
            layout.Content = content; layout.Hud = h; layout.Logo = view.Logo.rectTransform;
            layout.Ivan = view.Ivan.rectTransform; layout.Firebird = view.Firebird.rectTransform;
            layout.Play = view.PlayButton.transform as RectTransform; layout.Navigation = nav; layout.Footer = footer;
            layout.Environment = view.Environment; layout.Shade = view.LowerShade;
            AlignReferenceComposition(view, sprites);
            if (Application.isPlaying) { view.RefreshContent(); layout.ApplyNow(); }
        }

        /// <summary>Editor-only in-place migration. Existing objects, button routes and prefab IDs survive.</summary>
        public static void UpgradeReferenceForeground(ProductionHomeView view, Func<string, Sprite> loadSprite)
        {
            if (Application.isPlaying) throw new InvalidOperationException("Home asset migration must run outside Play Mode.");
            if (view == null || view.Revision < 2 || view.Revision > ProductionHomeView.CurrentRevision)
                throw new InvalidOperationException("Home reference migration requires an installed aligned Home view.");
            if (view.SourcePlay != null)
            {
                ProductionHomeSourceComposition.Apply(view);
                if (!view.HasRequiredBindings) throw new InvalidOperationException("Home source migration produced incomplete bindings.");
                return;
            }
            AlignReferenceComposition(view, LoadSprites(loadSprite));
            if (!view.HasRequiredBindings) throw new InvalidOperationException("Home reference migration produced incomplete bindings.");
        }

        static void AlignReferenceComposition(ProductionHomeView view, Dictionary<string, Sprite> sprites)
        {
            var layout = view.GetComponent<ProductionHomeLayout>();
            RectTransform content = view.transform.Find(ContentName) as RectTransform;
            SetArt(view.Environment, sprites["home_environment_generated"], 0, 0, 941, 1672);
            SetArt(view.Logo, sprites["home_logo"], 205.5f, 110, 530, 530f * 1041 / 1511);
            SetArt(view.Ivan, null, -20, 474, 967, 967);
            view.Environment.color = EnvironmentTone;
            view.Ivan.color = HeroTone;
            ProductionHomeArtFade oldFade = view.Ivan.GetComponent<ProductionHomeArtFade>();
            if (oldFade != null) oldFade.enabled = false;
            // Home now uses its own complete reference character and perched Firebird.
            // Historical native objects/components survive, but no old foreground is drawn.
            SplashTitleCharacterFade splashFade = view.Ivan.GetComponent<SplashTitleCharacterFade>();
            if (splashFade == null) splashFade = view.Ivan.gameObject.AddComponent<SplashTitleCharacterFade>();
            splashFade.BottomFraction = .08f; splashFade.enabled = true;
            view.LowerShade.color = Color.clear;
            view.Firebird.sprite = null;
            view.Firebird.gameObject.SetActive(false);
            RectTransform hud = content.Find("HomeHud") as RectTransform;
            Image hudContainer = EnsureFrame(hud, null);
            hudContainer.color = Color.clear;
            SetRect(hud, 0, 0, 941, 130);
            // The complete source contains a narrow bar inside real transparent padding.
            // Preserve the existing header container and all live child coordinates.
            float hudScale = 941f / 2132;
            view.HudFrame = EnsureArt(hud, "ReferenceArtwork", sprites["hud_frame"],
                -22 * hudScale, 8 - 198 * hudScale, 2176 * hudScale, 723 * hudScale);
            view.HudFrame.transform.SetAsFirstSibling();
            RectTransform profile = view.ProfileButton.transform as RectTransform;
            SetRect(profile, 8, 0, 320, 233);
            ProductionHomePanel profileHit = profile.GetComponent<ProductionHomePanel>();
            profileHit.Frame = false; profileHit.LeftColor = profileHit.RightColor = Color.clear;
            view.AvatarFrame = EnsureFrame(profile.Find("PortraitFrame"), sprites["avatar_ring"]);
            SetArt(view.AvatarFrame, sprites["avatar_ring"], -20, 0, 194, 194f * 1425 / 1104);
            view.ProfileButton.targetGraphic = view.AvatarFrame;
            AlignReferencePortrait(view, sprites["avatar_portrait"]);
            SetLabel(view.HeroName, 172, 28, 145, 42, 34, TextAnchor.MiddleLeft);
            SetLabel(view.LevelText, 172, 71, 145, 29, 24, TextAnchor.MiddleLeft);
            view.ProfileProgressFrame = EnsureArt(profile, "ProfileProgressFrame", null, 169, 87, 164, 54.6667f);
            view.ProfileProgressText = EnsureLabel(view.ProfileProgressFrame.rectTransform, "UnavailableExperience", "—", 24, 17, 116, 20, 18, view.Typography);
            EnsureBacking(content, "HudOpaqueNavyBacking", 5, 5, 931, 117, 0, hud.GetSiblingIndex());
            layout.HudBacking = content.Find("HudOpaqueNavyBacking") as RectTransform;
            AlignResource(view, hud, "Gold", 345, 177, sprites, out view.GoldCapsule, out view.GoldIcon, view.GoldText, out view.GoldPlusButton);
            AlignResource(view, hud, "Gems", 533, 160, sprites, out view.GemsCapsule, out view.GemsIcon, view.GemsText, out view.GemsPlusButton);
            AlignResource(view, hud, "Energy", 707, 146, sprites, out view.EnergyCapsule, out view.EnergyIcon, view.EnergyText, out view.EnergyPlusButton);
            SetArt(view.SettingsButton.GetComponent<Image>(), sprites["settings_medallion"], 853, 28, 85, 85);
            Image playHit = view.PlayButton.GetComponent<Image>();
            SetArt(playHit, null, 185.5f, 1230 - 570f * 803 / 1959, 570, 570f * 803 / 1959);
            playHit.color = Color.clear; playHit.raycastTarget = true;
            // Match the measured139px emerald face while preserving the established
            // native hitbox and live text center; the taller ornament is decoration.
            Image playArt = EnsureArt(playHit.rectTransform, "ReferenceArtwork", sprites["home_play_button"],
                0, 991 - (1230 - 570f * 803 / 1959), 570, 264);
            playArt.preserveAspect = false; playArt.raycastTarget = false;
            playArt.transform.SetAsFirstSibling(); view.PlayButton.targetGraphic = playArt;
            AlignReferencePlayLabel(view);
            view.Logo.transform.SetSiblingIndex(hud.GetSiblingIndex() + 1);
            SetRect(layout.Navigation, 51, 1230, 839, 222);
            Button[] navigation = { view.WorldMapButton, view.ShopButton, view.EventsButton, view.CharactersButton };
            Text[] labels = { view.WorldMapText, view.ShopText, view.EventsText, view.CharactersText };
            string[] navNames = { "nav_map_generated", "nav_shop_generated", "nav_events_generated", "nav_characters_generated" };
            // The shared reference card frame maps its full unchanged square PNG to
            // 224x255: its authored inner body becomes193x192, matching the reference.
            // All captions share one ink baseline while native hit areas stay disjoint.
            const float cardArtY = 174 - 981f / 1254 * 255;
            const float captionTop = 174 + 1.5f / 1254 * 255 - 16.5f;
            for (int i = 0; i < navigation.Length; i++)
            {
                RectTransform card = navigation[i].transform as RectTransform;
                Image hit = EnsureFrame(card, null);
                SetRect(card, i * 212, 0, 203, 222);
                hit.color = Color.clear; hit.raycastTarget = true;
                Image artwork = EnsureArt(card, "ReferenceArtwork", sprites[navNames[i]],
                    -10.5f, cardArtY, 224, 255);
                artwork.preserveAspect = false; artwork.raycastTarget = false;
                artwork.transform.SetAsFirstSibling(); navigation[i].targetGraphic = artwork;
                float textRatio = i == 0 || i == 3 ? .94f : 1;
                SetLabel(labels[i], 4, captionTop, 195 / textRatio, 33, 24);
                labels[i].rectTransform.localScale = new Vector3(.5f * textRatio, .5f, 1);
                labels[i].color = Gold;
                SplashTitleTextGeometry captionGeometry = labels[i].GetComponent<SplashTitleTextGeometry>();
                if (captionGeometry == null) captionGeometry = labels[i].gameObject.AddComponent<SplashTitleTextGeometry>();
                captionGeometry.CenterHorizontally = captionGeometry.CenterVertically = captionGeometry.SnapToScreenPixels = true;
                captionGeometry.OpticalOffset = Vector2.zero;
                DisableLegacyArt(card.Find("Illustration")); DisableLegacyArt(card.Find("CornerOrnaments"));
                EnsureBacking(card.parent as RectTransform, card.name + "OpaqueNavyBacking", i * 212 + 20, 22, 163, 174, 8, card.GetSiblingIndex());
            }
            RectTransform eventCard = view.EventsButton.transform as RectTransform;
            view.EventsBadge = EnsureNativeDisk(eventCard, "ReferenceEventMarker", 146, -5, 46, new Color32(126, 22, 7, 255));
            view.EventsBadgeText = EnsureLabel(view.EventsBadge.rectTransform, "EventMarkerCopy", "!", 0, 0, 46, 46, 38, view.Typography);
            view.EventsBadgeText.color = new Color32(248, 224, 160, 255);
            view.EventsBadgeText.fontStyle = FontStyle.Bold;
            AlignBadgeLabel(view.EventsBadgeText);
            view.EventsBadge.gameObject.SetActive(true);
            SetRect(layout.Footer, 0, 1450, 941, 222);
            // A whole independently painted surround joins the three existing footer
            // controls. It is decoration behind the original native hit surfaces.
            float footerScale = 941f / 2136;
            view.FooterBase = EnsureArt(layout.Footer, "ConnectedCarvedBase", sprites["footer_base_generated"],
                -4 * footerScale, 222 - 708 * footerScale, 2143 * footerScale, 734 * footerScale);
            view.FooterBase.raycastTarget = false;
            view.FooterBase.transform.SetAsFirstSibling();
            EnsureBacking(layout.Footer, "FooterOpaqueBacking", 0, 222 - 362 * footerScale, 941, 362 * footerScale, 0, 0, new Color32(6, 8, 11, 255));
            layout.FooterBacking = layout.Footer.Find("FooterOpaqueBacking") as RectTransform;
            layout.FooterBase = view.FooterBase.rectTransform;
            AlignMedallion(view.MailButton, sprites["mail_medallion_generated"], 0, 12, 181);
            AlignMedallion(view.DailyRewardButton, sprites["reward_medallion_generated"], 758, 12, 181);
            view.QuestFrame = EnsureFrame(view.QuestText.transform.parent, sprites["quest_panel"]);
            // Place the complete final source around its measured(21,56)-(2187,657)
            // wood frame. The separate live children use reference page coordinates.
            float questX = 182 - 21f / 2166 * 572, questY = 8 - 56f / 601 * 182;
            SetRect(view.QuestFrame.rectTransform, questX, questY, 2206f / 2166 * 572, 713f / 601 * 182);
            EnsureBacking(layout.Footer, "QuestOpaqueWoodBacking", 200, 21, 539, 144, 4,
                view.QuestFrame.transform.GetSiblingIndex(), new Color32(22, 13, 7, 255));
            SetLabel(view.QuestText, 220 - questX, 39 - questY, 398 / .8f, 54, 34);
            view.QuestText.rectTransform.localScale = new Vector3(.4f, .5f, 1);
            view.QuestText.fontStyle = FontStyle.Bold;
            view.QuestText.color = new Color32(231, 223, 197, 255);
            RectTransform quest = view.QuestFrame.rectTransform;
            view.QuestProgressFrame = EnsureArt(quest, "QuestProgressFrame", null, 224 - questX, 67 - questY, 377, 126);
            EnsureBacking(quest, "ProgressOpaqueTroughBacking", 253 - questX, 118 - questY, 320, 20, 3,
                view.QuestProgressFrame.transform.GetSiblingIndex(), new Color32(19, 12, 4, 255));
            view.QuestProgressFill = EnsureArt(view.QuestProgressFrame.rectTransform, "MeasuredFill", null, 17, 6, 343, 114.3333f);
            view.QuestProgressFill.preserveAspect = false; view.QuestProgressFill.type = Image.Type.Filled;
            view.QuestProgressFill.fillMethod = Image.FillMethod.Horizontal; view.QuestProgressFill.fillOrigin = 0;
            view.QuestProgressFill.fillAmount = 0; view.QuestProgressFill.gameObject.SetActive(false);
            view.QuestProgressText = EnsureLabel(view.QuestProgressFrame.rectTransform, "MeasuredValue", "— / —", 25, 44, 327, 34, 29, view.Typography);
            view.QuestProgressText.color = new Color32(236, 228, 204, 255);
            float rewardX = 612 - 189f / 712 * 114, rewardY = 38 - 193f / 1053 * 132;
            view.QuestRewardFrame = EnsureArt(quest, "QuestRewardFrame", sprites["quest_reward_frame"],
                rewardX - questX, rewardY - questY, 1095f / 712 * 114, 1437f / 1053 * 132);
            view.QuestRewardFrame.preserveAspect = false;
            EnsureBacking(quest, "RewardOpaqueNavyBacking", 620 - questX, 49 - questY, 96, 113, 5,
                view.QuestRewardFrame.transform.GetSiblingIndex());
            float rewardGemScale = 77f / 797;
            view.QuestRewardIcon = EnsureArt(view.QuestRewardFrame.rectTransform, "RewardGem", sprites["quest_reward_gem"],
                669 - 613.5f * rewardGemScale - rewardX, 85 - 660.5f * rewardGemScale - rewardY,
                1230 * rewardGemScale, 1278 * rewardGemScale);
            view.QuestRewardText = EnsureLabel(view.QuestRewardFrame.rectTransform, "RewardValue", "—", 623.5f - rewardX, 130 - rewardY, 91, 36, 34, view.Typography);
            RectTransform reward = view.DailyRewardButton.transform as RectTransform;
            view.DailyRewardBadge = EnsureNativeDisk(reward, "AvailableRewardBadge", 132, 0, 44, new Color32(110, 16, 7, 255));
            view.DailyRewardBadgeText = EnsureLabel(view.DailyRewardBadge.rectTransform, "AvailableCount", "1", 0, 0, 44, 44, 29, view.Typography);
            AlignBadgeLabel(view.DailyRewardBadgeText);
            view.DailyRewardBadge.gameObject.SetActive(false);
            AlignReferenceBackdrop(view, layout, sprites["home_map_foliage"]);
            AlignSceneLighting(view.Ivan, HeroLightingMaterialResource);
            AlignSceneLighting(view.MapFoliage, FoliageLightingMaterialResource);
            view.Revision = ProductionHomeView.CurrentRevision;
            ProductionHomeSourceComposition.Apply(view);
        }

        static void AlignSceneLighting(Image image, string materialResource)
        {
            Material material = Resources.Load<Material>(materialResource);
            if (material == null || material.shader.name != SceneLightingShader)
                throw new InvalidOperationException("Missing Home scene-lighting material: " + materialResource);
            image.material = material;
            ProductionHomeSceneLighting lighting = image.GetComponent<ProductionHomeSceneLighting>();
            if (lighting == null) lighting = image.gameObject.AddComponent<ProductionHomeSceneLighting>();
            lighting.Strength = 1f; lighting.Padding = .03f; lighting.enabled = true;
        }

        static void AlignReferenceBackdrop(ProductionHomeView view, ProductionHomeLayout layout, Sprite foliage)
        {
            // The wash follows the full environment plate on every aspect ratio. As
            // its child, it can never dim the separately layered hero or controls.
            RectTransform environment = view.Environment.rectTransform;
            Transform found = environment.Find("BackgroundTone");
            view.BackgroundShade = found != null ? found.GetComponent<ProductionHomeBackdropShade>() :
                Rect(environment, "BackgroundTone", 0, 0, 941, 1672).gameObject.AddComponent<ProductionHomeBackdropShade>();
            RectTransform shade = view.BackgroundShade.rectTransform;
            shade.anchorMin = Vector2.zero; shade.anchorMax = Vector2.one;
            shade.offsetMin = shade.offsetMax = Vector2.zero;
            view.BackgroundShade.color = new Color32(4, 9, 16, 255);
            view.BackgroundShade.Strength = 1;
            view.BackgroundShade.raycastTarget = false;
            view.BackgroundShade.enabled = true; shade.gameObject.SetActive(true);
            shade.SetAsFirstSibling();

            // Keep the complete isolated sprig above the hero, but below BOTH the
            // existing opaque card backings and their artwork. Navigation already
            // follows the safe bottom, so this decoration needs no layout owner.
            view.MapFoliage = EnsureArt(layout.Navigation, "MapSideFoliage", foliage, -68, -15, 117, 234);
            view.MapFoliage.raycastTarget = false;
            view.MapFoliage.enabled = true;
            view.MapFoliage.gameObject.SetActive(true);
            view.MapFoliage.transform.SetAsFirstSibling();
        }

        static void AlignReferencePortrait(ProductionHomeView view, Sprite referencePortrait)
        {
            // The independently generated profile uses its complete source through the
            // existing native circular mask. All earlier prefab identities survive.
            RectTransform frame = view.AvatarFrame.rectTransform;
            RectTransform profile = frame.parent as RectTransform;
            Transform found = frame.Find("PortraitClip");
            if (found == null) found = profile.Find("PortraitClip");
            ProductionHomePanel clip = found != null ? found.GetComponent<ProductionHomePanel>() :
                Panel(profile, "PortraitClip", 10, 26, 134, 134, 67);
            // The measured reference opening is source(181,151)-(922,904).
            // This slightly larger native mask draws BEHIND the ring, so the complete
            // gold rim and lower heraldic pendant remain above the reference portrait.
            clip.transform.SetParent(profile, false);
            SetRect(clip.rectTransform, 10, 26, 134, 134);
            clip.Frame = false; clip.Radius = 67; clip.LeftColor = clip.RightColor = new Color32(7, 24, 42, 255);
            clip.raycastTarget = false; clip.gameObject.SetActive(true);
            Mask mask = clip.GetComponent<Mask>();
            if (mask == null) mask = clip.gameObject.AddComponent<Mask>();
            mask.enabled = true; mask.showMaskGraphic = true;
            Image portrait = EnsureArt(clip.rectTransform, "ApprovedIvanPortrait", referencePortrait,
                0, 0, 134, 134);
            portrait.raycastTarget = false; portrait.gameObject.SetActive(true);
            int frameSibling = frame.GetSiblingIndex();
            clip.transform.SetSiblingIndex(frameSibling - (clip.transform.GetSiblingIndex() < frameSibling ? 1 : 0));
        }

        static void AlignReferencePlayLabel(ProductionHomeView view)
        {
            // Reference wording stays live and localizable over the separate emerald face.
            // The actual Unity mesh measured333.5px at.92; .87 brings it to315.4px.
            // Compensate the local rect width to retain its physical center and height.
            const float horizontalRatio = .87f;
            SetLabel(view.PlayText, 91, 570f * 803 / 1959 - 157, 388 / horizontalRatio, 106, 90);
            view.PlayText.rectTransform.localScale = new Vector3(horizontalRatio * .5f, .5f, 1);
            view.PlayText.text = "ИГРАТЬ";
            view.PlayText.font = view.Typography.StartFont != null ? view.Typography.StartFont : view.Typography.RegularFont;
            view.PlayText.fontStyle = FontStyle.Bold;
            view.PlayText.color = new Color32(244, 199, 100, 255);
            view.PlayText.resizeTextForBestFit = false;
            view.PlayText.horizontalOverflow = HorizontalWrapMode.Overflow;
            view.PlayText.verticalOverflow = VerticalWrapMode.Overflow;
            SplashTitleTextGeometry geometry = view.PlayText.GetComponent<SplashTitleTextGeometry>();
            if (geometry == null) geometry = view.PlayText.gameObject.AddComponent<SplashTitleTextGeometry>();
            geometry.enabled = true; geometry.CenterHorizontally = geometry.CenterVertically = geometry.SnapToScreenPixels = true;
            geometry.OpticalOffset = Vector2.zero;
            Shadow shadow = view.PlayText.GetComponent<Shadow>();
            if (shadow == null) shadow = view.PlayText.gameObject.AddComponent<Shadow>();
            shadow.enabled = true; shadow.effectColor = new Color32(34, 15, 2, 230);
            shadow.effectDistance = new Vector2(0, -2); shadow.useGraphicAlpha = true;
            ProductionHomeGoldText gold = view.PlayText.GetComponent<ProductionHomeGoldText>();
            if (gold == null) gold = view.PlayText.gameObject.AddComponent<ProductionHomeGoldText>();
            gold.enabled = true;
            gold.BeveledFace = true;
            view.PlayText.material = Resources.Load<Material>(PlayGoldMaterialResource);
            if (view.PlayText.material == null || view.PlayText.material.shader.name != "LegendsOfRus/UI/HomePlayGold")
                throw new InvalidOperationException("Home requires its live Play gold material.");
            ColorBlock colors = view.PlayButton.colors;
            colors.normalColor = Color.white; colors.highlightedColor = new Color(.98f, .98f, .96f, 1);
            colors.selectedColor = Color.white; colors.pressedColor = new Color(.76f, .80f, .86f, 1);
            colors.disabledColor = new Color(.46f, .46f, .46f, .7f); colors.colorMultiplier = 1; colors.fadeDuration = .08f;
            view.PlayButton.colors = colors;
        }

        static void AlignResource(ProductionHomeView view, RectTransform hud, string name, float x, float width,
            Dictionary<string, Sprite> sprites, out Image capsule, out Image icon, Text text, out Button plus)
        {
            Image resourceRoot = EnsureFrame(hud.Find(name + "Resource"), null);
            resourceRoot.color = Color.clear;
            SetRect(resourceRoot.rectTransform, x, 45, width, 53);
            // Keep the native number/icon/plus geometry independent of PNG margins.
            // Near-opaque source bounds are (29,101)-(2143,618) in2172x724 pixels;
            // transform the complete sprite so its aged gold capsule is width x 53.
            capsule = EnsureArt(resourceRoot.rectTransform, "CapsuleArtwork", sprites["resource_capsule"],
                -29f / 2114 * width, -101f / 517 * 53, 2172f / 2114 * width, 724f / 517 * 53);
            capsule.preserveAspect = false; capsule.raycastTarget = false;
            capsule.transform.SetAsFirstSibling();
            EnsureBacking(hud, name + "OpaqueNavyBacking", x + 8, 55, width - 16, 33, 16.5f,
                resourceRoot.transform.GetSiblingIndex());
            icon = resourceRoot.transform.Find("Icon").GetComponent<Image>();
            SetArt(icon, sprites[name == "Gold" ? "coin_icon" : name == "Gems" ? "gems_icon" : "energy_icon"], 0, -5, 53, 61);
            if (name == "Gems")
            {
                SetArt(icon, sprites["gems_icon"], 0, -5, 53, 66);
                icon.preserveAspect = false;
            }
            if (name == "Energy")
            {
                float scale = 61f / 1038;
                SetArt(icon, sprites["energy_icon"], 2 - 360 * scale, -2 - 121 * scale, 1254 * scale, 1254 * scale);
            }
            SetLabel(text, 58, 6, width - 106, 39, name == "Gold" ? 25 : 26, TextAnchor.MiddleCenter);
            text.resizeTextForBestFit = true; text.resizeTextMinSize = 17; text.resizeTextMaxSize = name == "Gold" ? 50 : 52;
            Image plusHit = EnsureArt(resourceRoot.rectTransform, "ResourcePlus", null, width - 43, 4, 43, 43);
            plusHit.color = Color.clear; plusHit.raycastTarget = true;
            plus = plusHit.GetComponent<Button>();
            if (plus == null) plus = Button(plusHit);
            float plusScale = 43f / 970;
            Image plusArt = EnsureArt(plusHit.rectTransform, "ReferenceArtwork", sprites["resource_plus"],
                -142 * plusScale, -127 * plusScale, 1254 * plusScale, 1254 * plusScale);
            plusArt.raycastTarget = false; plus.targetGraphic = plusArt;
        }

        static void DisableLegacyArt(Transform target)
        {
            if (target == null) return;
            foreach (Image image in target.GetComponentsInChildren<Image>(true)) image.sprite = null;
            target.gameObject.SetActive(false);
        }
        static void AlignMedallion(Button button, Sprite sprite, float x, float y, float size)
        {
            RectTransform rect = button.transform as RectTransform;
            SetRect(rect, x, y, size, size);
            ProductionHomePanel hit = button.GetComponent<ProductionHomePanel>();
            hit.Radius = 0; hit.Frame = false; hit.LeftColor = hit.RightColor = Color.clear;
            Mask oldMask = button.GetComponent<Mask>();
            if (oldMask != null) oldMask.enabled = false;
            EnsureBacking(rect, "OpaqueNavyBacking", 20, 20, size - 40, size - 40, (size - 40) * .5f, 0);
            Image image = button.transform.Find("Medallion").GetComponent<Image>();
            bool gift = sprite.name == "reward_medallion_generated";
            float artSize = gift ? 194 : 186;
            SetArt(image, sprite, (size - artSize) * .5f, gift ? -2 : 2, artSize, artSize);
            button.targetGraphic = image;
        }
        // Retain the serialized containers and live text bindings, without loading retired art.
        internal static void ClearDormantProgress(ProductionHomeView view)
        {
            if (view.ProfileProgressFrame != null) view.ProfileProgressFrame.sprite = null;
            if (view.QuestProgressFrame != null) view.QuestProgressFrame.sprite = null;
            if (view.QuestProgressFill != null) view.QuestProgressFill.sprite = null;
        }

        static Dictionary<string, Sprite> LoadSprites(Func<string, Sprite> loadSprite)
        {
            var result = new Dictionary<string, Sprite>();
            foreach (string name in TextureNames)
            {
                Sprite sprite = loadSprite(ArtRoot + name);
                if (sprite == null) throw new InvalidOperationException("Home requires a standalone imported Sprite: " + ArtRoot + name);
                result.Add(name, sprite);
            }
            return result;
        }
        static Image EnsureFrame(Transform target, Sprite sprite)
        {
            if (target == null) throw new InvalidOperationException("Installed Home control transform is missing.");
            Image image = target.GetComponent<Image>();
            if (image == null)
            {
                ProductionHomePanel old = target.GetComponent<ProductionHomePanel>();
                if (old != null) UnityEngine.Object.DestroyImmediate(old);
                image = target.gameObject.AddComponent<Image>();
            }
            ConfigureFrame(image, sprite);
            return image;
        }
        static Image EnsureArt(RectTransform parent, string name, Sprite sprite, float x, float y, float w, float h)
        {
            Transform found = parent.Find(name);
            Image image = found != null ? found.GetComponent<Image>() : Art(parent, name, sprite, x, y, w, h);
            SetArt(image, sprite, x, y, w, h);
            return image;
        }
        static Text EnsureLabel(RectTransform parent, string name, string copy, float x, float y, float w, float h, int size, SplashTitleTypography typography)
        {
            Transform found = parent.Find(name);
            Text text = found != null ? found.GetComponent<Text>() : Label(parent, name, copy, x, y, w, h, size, typography);
            SetLabel(text, x, y, w, h, size); return text;
        }
        static ProductionHomePanel EnsureNativeDisk(RectTransform parent, string name, float x, float y, float size, Color color)
        {
            Transform found = parent.Find(name);
            ProductionHomePanel panel = found != null ? found.GetComponent<ProductionHomePanel>() : Panel(parent, name, x, y, size, size, size * .5f);
            SetRect(panel.rectTransform, x, y, size, size);
            panel.Frame = true; panel.RaisedBadge = true;
            panel.BorderColor = new Color32(184, 132, 53, 255);
            panel.LeftColor = panel.RightColor = color; panel.raycastTarget = false;
            return panel;
        }
        static void AlignBadgeLabel(Text label)
        {
            label.color = new Color32(248, 224, 160, 255);
            SplashTitleTextGeometry geometry = label.GetComponent<SplashTitleTextGeometry>();
            if (geometry == null) geometry = label.gameObject.AddComponent<SplashTitleTextGeometry>();
            geometry.enabled = true;
            geometry.CenterHorizontally = geometry.CenterVertically = geometry.SnapToScreenPixels = true;
            geometry.OpticalOffset = Vector2.zero;
            Shadow shadow = label.GetComponent<Shadow>();
            if (shadow == null) shadow = label.gameObject.AddComponent<Shadow>();
            shadow.enabled = true; shadow.effectColor = new Color32(51, 9, 3, 230);
            shadow.effectDistance = new Vector2(1.6f, -2f); shadow.useGraphicAlpha = true;
            ProductionHomeGoldText gold = label.GetComponent<ProductionHomeGoldText>();
            if (gold == null) gold = label.gameObject.AddComponent<ProductionHomeGoldText>();
            gold.enabled = true; gold.TopColor = new Color32(255, 250, 224, 255);
            gold.BottomColor = new Color32(232, 184, 95, 255);
        }
        static void EnsureBacking(RectTransform parent, string name, float x, float y, float w, float h, float radius, int sibling, Color? tint = null)
        {
            Transform found = parent.Find(name);
            ProductionHomePanel panel = found != null ? found.GetComponent<ProductionHomePanel>() : Panel(parent, name, x, y, w, h, radius);
            SetRect(panel.rectTransform, x, y, w, h); panel.Radius = radius;
            panel.Frame = false; panel.LeftColor = panel.RightColor = tint ?? new Color32(5, 10, 18, 255);
            if (panel.transform.GetSiblingIndex() < sibling) sibling--;
            panel.transform.SetSiblingIndex(sibling);
        }
        static Text Resource(RectTransform parent, string name, Sprite capsule, Sprite sprite, SplashTitleTypography typography, out Image panel, out Image icon)
        {
            panel = Frame(parent, name + "Resource", capsule, 0, 45, 177, 53);
            icon = Art(panel.rectTransform, "Icon", sprite, 0, -5, 53, 61);
            return Label(panel.rectTransform, name + "Value", "—", 58, 6, 81, 39, 25, typography);
        }
        static Button Nav(RectTransform parent, string name, Sprite sprite, string copy, SplashTitleTypography typography, out Text text)
        {
            Image panel = Frame(parent, name + "Button", sprite, 0, 0, 203, 222);
            text = Label(panel.rectTransform, name + "Label", copy, 7, 151, 189, 29, 23, typography);
            return Button(panel);
        }
        static Button Medallion(RectTransform parent, string name, Sprite sprite)
        {
            ProductionHomePanel hit = Panel(parent, name, 0, 12, 181, 181, 0);
            hit.Frame = false; hit.LeftColor = hit.RightColor = Color.clear;
            Image image = Art(hit.rectTransform, "Medallion", sprite, 0, 0, 181, 181);
            Button button = Button(hit); button.targetGraphic = image; return button;
        }
        static Button Button(Graphic graphic)
        {
            graphic.raycastTarget = true;
            Button button = graphic.gameObject.AddComponent<Button>(); button.targetGraphic = graphic;
            graphic.gameObject.AddComponent<ProductionHomeButtonFeedback>();
            Navigation navigation = button.navigation; navigation.mode = Navigation.Mode.None; button.navigation = navigation;
            return button;
        }
        static RectTransform Rect(RectTransform parent, string name, float x, float y, float width, float height)
        {
            GameObject go = new GameObject(name, typeof(RectTransform)); go.layer = 5; go.transform.SetParent(parent, false);
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            SetRect(rect, x, y, width, height); return rect;
        }
        static void SetRect(RectTransform rect, float x, float y, float width, float height)
        { rect.anchoredPosition = new Vector2(x, -y); rect.sizeDelta = new Vector2(width, height); }
        static void SetArt(Image image, Sprite sprite, float x, float y, float w, float h)
        {
            SetRect(image.rectTransform, x, y, w, h); image.sprite = sprite; image.color = Color.white;
            image.type = Image.Type.Simple; image.preserveAspect = true; image.useSpriteMesh = false;
        }
        static Image Art(RectTransform parent, string name, Sprite sprite, float x, float y, float width, float height)
        {
            Image image = Rect(parent, name, x, y, width, height).gameObject.AddComponent<Image>();
            SetArt(image, sprite, x, y, width, height); image.raycastTarget = false; return image;
        }
        static Image Frame(RectTransform parent, string name, Sprite sprite, float x, float y, float width, float height)
        {
            Image image = Rect(parent, name, x, y, width, height).gameObject.AddComponent<Image>();
            ConfigureFrame(image, sprite); return image;
        }
        static void ConfigureFrame(Image image, Sprite sprite)
        {
            image.sprite = sprite; image.color = Color.white; image.raycastTarget = false;
            image.preserveAspect = false; image.useSpriteMesh = false; image.type = Image.Type.Simple;
            image.pixelsPerUnitMultiplier = 1;
        }
        static ProductionHomePanel Panel(RectTransform parent, string name, float x, float y, float w, float h, float radius)
        {
            ProductionHomePanel panel = Rect(parent, name, x, y, w, h).gameObject.AddComponent<ProductionHomePanel>();
            panel.color = Color.white; panel.raycastTarget = false; panel.Radius = radius; return panel;
        }
        static void SetLabel(Text text, float x, float y, float w, float h, int size, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            SetRect(text.rectTransform, x, y, w * 2, h * 2); text.rectTransform.localScale = new Vector3(.5f, .5f, 1);
            text.fontSize = size * 2; text.alignment = alignment; text.raycastTarget = false;
        }
        static Text Label(RectTransform parent, string name, string value, float x, float y, float w, float h, int size,
            SplashTitleTypography typography, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            Text text = Rect(parent, name, x, y, w, h).gameObject.AddComponent<Text>();
            SetLabel(text, x, y, w, h, size, alignment); text.text = value; text.font = typography.RegularFont;
            text.alignByGeometry = true; text.color = Gold; text.supportRichText = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate; return text;
        }
    }
}
