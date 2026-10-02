using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Reference-sized navigation artwork. Existing buttons, labels and routes remain the owners.</summary>
    public static class ProductionNavigationStyle
    {
        public const string ArtRoot = "ProductionUI/NavigationAligned/";
        static Sprite Art(string key) => Resources.Load<Sprite>(ArtRoot + key);
        static Sprite Home(string key) => Resources.Load<Sprite>(ProductionHomeRuntime.ArtRoot + key);

        public static void Apply(Component page)
        {
            if (page is ProductionDailyRewardView sourceDaily && sourceDaily.PlayerHeader != null) return;
            if (page is ProductionSpecialOfferView sourceOffer && sourceOffer.PlayerHeader != null) return;
            if (page is ProductionShopView sourceShop && sourceShop.Header != null) return;
            if (page is ProductionChapterMapView sourceChapter && sourceChapter.SourceControls) return;
            if (page is ProductionWorldMapView sourceMap && sourceMap.SourceComposition) return;
            if (page is ProductionHomeView sourceHome && sourceHome.SourcePlay != null) return;
            if (page is ProductionHomeView home && home.HeroName != null && home.GoldPlusButton != null && home.GetComponent<ProductionHomeLayout>() != null)
            {
                Header(home.GetComponent<ProductionHomeLayout>().Hud, home.ProfileButton, home.HeroName, home.LevelText,
                    new[] { home.GoldText, home.GemsText, home.EnergyText }, new[] { home.GoldPlusButton, home.GemsPlusButton, home.EnergyPlusButton }, home.SettingsButton, false);
                home.Revision = ProductionHomeView.CurrentRevision;
            }
            else if (page is ProductionWorldMapView world && world.HasRequiredBindings)
            {
                Header(world.Hud, world.ProfileButton, world.HeroName, world.LevelText, new[] { world.GoldText, world.GemsText, world.EnergyText }, world.PlusButtons, world.SettingsButton, false);
                Back(world.BackButton, 10, 146, 104);
                Footer(world.Footer, world.FooterOrnament, 272);
            }
            else if (page is ProductionChapterMapView chapter && chapter.HasRequiredBindings)
            {
                Header(chapter.Hud, chapter.ProfileButton, chapter.HeroName, chapter.LevelText, new[] { chapter.GoldText, chapter.GemsText, chapter.EnergyText }, chapter.PlusButtons, chapter.SettingsButton, false);
                Back(chapter.BackButton, 22, 42, 144);
                Footer(chapter.Footer, chapter.FooterOrnament, 184);
            }
            else if (page is ProductionPreLevelView pre && pre.HasRequiredBindings && !pre.SourceControls)
            {
                Header(pre.Hud, pre.ProfileButton, pre.HeroName, pre.LevelText, new[] { pre.GoldText, pre.GemsText, pre.EnergyText }, pre.PlusButtons, pre.SettingsButton, false);
                Back(pre.BackButton, 22, 42, 144);
                Footer(pre.Footer, pre.FooterOrnament, 184);
            }
            else if (page is ProductionFailView fail && fail.HasRequiredBindings)
                Header(fail.Hud, fail.Profile, fail.HeroName, fail.Level, fail.Resources, fail.Plus, fail.Settings, false);
            else if (page is ProductionShopView shop && shop.HasRequiredBindings)
            {
                Header(shop.Body, shop.ProfileButton, shop.HeroName, shop.LevelText, new[] { shop.GoldText, shop.GemsText, shop.EnergyText }, shop.PlusButtons, shop.SettingsButton, true);
                Back(shop.BackButton, 22, 1388, 74);
                shop.BackText.gameObject.SetActive(false);
                for (int i = 0; i < shop.NavLabels.Length; i++)
                {
                    var plaque = shop.NavButtons[i].transform.Find("LabelPlaque").GetComponent<Image>();
                    plaque.sprite = Resources.Load<Sprite>("ProductionUI/ShopAligned/tab");
                    Place(plaque.rectTransform, -2, 125, 177, 48);plaque.preserveAspect = true;
                    Label(shop.NavLabels[i], 3, 132, 167, 32, 22, TextAnchor.MiddleCenter);
                }
            }
            else if (page is ProductionDailyRewardView daily && daily.HasRequiredBindings)
                Header(daily.Body, daily.ProfileButton, daily.HeroName, daily.LevelText, new[] { daily.GoldText, daily.GemsText, daily.EnergyText }, daily.PlusButtons, daily.SettingsButton, true);
            else if (page is ProductionSpecialOfferView offer && offer.HasRequiredBindings)
            {
                var frame = offer.Body.Find("NativeHudBacking").GetComponent<Image>();SetArt(frame, "header", 0, 17, 941, 130);
                Back(offer.BackButton, 26, 34, 102);offer.BackText.gameObject.SetActive(false);
                string[] keys = { "coin", "gem" };
                for (int i = 0; i < 2; i++)
                    SetArt(offer.Body.Find("ResourceIcon_" + i).GetComponent<Image>(), keys[i], 245 + i * 224, 51, 64, 64);
                // The previously hidden upper-right energy indicator stays hidden.
            }
        }

        static void Header(RectTransform hud, Button profile, Text name, Text level, Text[] values, Button[] plus, Button settings, bool flat)
        {
            Image frame = hud.Find(flat ? (hud.Find("Hud") != null ? "Hud" : "NativeHudBacking") : "ReferenceArtwork").GetComponent<Image>();
            SetArt(frame, "header", 0, 4, 941, 130);
            if (flat)
            {
                // Match Home's full-size existing portrait and pendant; no new character is generated.
                Place((RectTransform)profile.transform, 8, 6, 164, 233);
                var hit = profile.GetComponent<Image>();hit.color = Color.clear;hit.sprite = Home("avatar_ring");hit.raycastTarget = true;
                var oldRing = profile.transform.Find("PortraitRing");if (oldRing != null) oldRing.gameObject.SetActive(false);
                var ring = Ensure(profile.transform, "NavigationPortraitFrame");ring.sprite = Home("avatar_ring");ring.preserveAspect = true;
                Place(ring.rectTransform, -20, -6, 194, 250.4f);profile.targetGraphic = ring;
                var clip = (RectTransform)profile.transform.Find("PortraitClip");Place(clip, 10, 20, 134, 134);
                var panel = clip.GetComponent<ProductionHomePanel>();panel.Radius = 67;
                var portrait = clip.GetComponentInChildren<Image>(true);Place(portrait.rectTransform, 0, 0, 134, 134);clip.SetAsFirstSibling();
                Label(name, 180, 28, 151, 42, 34, TextAnchor.MiddleLeft);
                Label(level, 180, 71, 151, 30, 24, TextAnchor.MiddleLeft);
            }
            else
            {
                Label(name, 172, 28, 151, 42, 34, TextAnchor.MiddleLeft);
                Label(level, 172, 71, 151, 30, 24, TextAnchor.MiddleLeft);
            }
            string[] groups = { "Gold", "Gems", "Energy" }, keys = { "coin", "gem", "energy" };
            float[] xs = { 345, 533, 707 }, widths = { 177, 160, 146 };
            for (int i = 0; i < 3; i++)
            {
                float x = xs[i], w = widths[i];
                Transform group = flat ? hud : hud.Find(groups[i] + "Resource");
                if (!flat) Place((RectTransform)group, x, 45, w, 53);
                var capsule = group.Find(flat ? "ResourceCapsule_" + i : "CapsuleArtwork").GetComponent<Image>();
                SetArt(capsule, "capsule", flat ? x : 0, flat ? 45 : 0, w, 53);capsule.preserveAspect = false;
                var icon = group.Find(flat ? "ResourceIcon_" + i : "Icon").GetComponent<Image>();
                SetArt(icon, keys[i], flat ? x : 0, flat ? 40 : -5, i == 2 ? 42 : 53, 61);
                float numberLeft = i == 2 ? 48 : 58;
                Label(values[i], (flat ? x : 0) + numberLeft, flat ? 51 : 6, (w - numberLeft - 47) * 2, 78, 50, TextAnchor.MiddleCenter);
                // Double-density glyphs preserve readable ordinary balances while fitting every digit of int.MaxValue.
                values[i].rectTransform.localScale = Vector3.one * .5f;
                values[i].resizeTextForBestFit = true;values[i].resizeTextMinSize = 17;values[i].resizeTextMaxSize = 50;
                Place((RectTransform)plus[i].transform, (flat ? x : 0) + w - 42, flat ? 49 : 4, 43, 43);
                var plusImage = plus[i].GetComponent<Image>();SetArt(plusImage, "plus", (flat ? x : 0) + w - 42, flat ? 49 : 4, 43, 43);
                plusImage.raycastTarget = true;plus[i].targetGraphic = plusImage;
                var oldArt = plus[i].transform.Find("ReferenceArtwork");if (oldArt != null) oldArt.gameObject.SetActive(false);
            }
            SetArt(settings.GetComponent<Image>(), "settings", 853, 28, 85, 85);settings.targetGraphic = settings.GetComponent<Image>();settings.targetGraphic.raycastTarget = true;
        }

        static void Back(Button button, float x, float y, float size)
        {
            var image = button.GetComponent<Image>();image.sprite = Resources.Load<Sprite>("ProductionUI/DailyRewardAligned/back_r2");
            image.color = Color.white;image.preserveAspect = true;image.raycastTarget = true;button.targetGraphic = image;
            Place(image.rectTransform, x, y, size, size * 133 / 140);
            foreach (string child in new[] { "BackArrow", "BackMedallion" })
            { var old = button.transform.Find(child);if (old != null) old.gameObject.SetActive(false); }
        }
        static void Footer(RectTransform footer, Image old, float y)
        {
            old.enabled = false;
            var trim = Ensure(footer, "NavigationBottomOrnament");trim.sprite = Resources.Load<Sprite>("ProductionUI/ShopAligned/footer_ornament");
            trim.preserveAspect = true;Place(trim.rectTransform, 3, y, 935, 44);trim.transform.SetAsFirstSibling();
        }
        static Image Ensure(Transform parent, string name)
        {
            var found = parent.Find(name);if (found != null) return found.GetComponent<Image>();
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));go.layer = 5;go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();image.raycastTarget = false;return image;
        }
        static void SetArt(Image image, string key, float x, float y, float w, float h)
        { image.sprite = Art(key);image.color = Color.white;image.preserveAspect = true;image.type = Image.Type.Simple;Place(image.rectTransform, x, y, w, h); }
        static void Label(Text text, float x, float y, float w, float h, int size, TextAnchor anchor)
        {
            Place(text.rectTransform, x, y, w, h);text.fontSize = size;text.alignment = anchor;text.color = new Color32(245,222,175,255);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;text.verticalOverflow = VerticalWrapMode.Truncate;text.resizeTextForBestFit = false;
        }
        static void Place(RectTransform rect, float x, float y, float w, float h)
        {
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0,1);rect.anchoredPosition = new Vector2(x,-y);rect.sizeDelta = new Vector2(w,h);rect.localScale = Vector3.one;
        }
    }
}
