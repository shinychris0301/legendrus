using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// Functionality-first demo presentation for final-product routes that do not
    /// yet have approved production art. It deliberately uses simple temporary UI.
    /// </summary>
    public static class DemoFunctionalPageRuntime
    {
        const float W = 941f;
        const float H = 1672f;
        const string Kit = "ProductionUI/Assets/Fix01/Kit/";

        public static bool TryBuild(ScreenId id, RectTransform parent)
        {
            // Step 8.5 keeps meta/progression cutovers outside the core-loop registry.
            // When an explicitly generated cutover state is present, render that
            // production prefab; otherwise preserve the inherited demo/design route.
            if (ProductionMetaPageCutoverRegistry.IsCutover(id))
                return ProductionMetaPageRuntime.TryBuild(id, parent);

            if (parent == null || !IsDemoRoute(id))
                return false;

            AddPanel(parent, "Background", "ui_panel_navy", 0, 0, W, H, new Color32(5, 18, 34, 255));
            AddPanel(parent, "Header", "ui_header_frame", 110, 95, 721, 170, Color.white);
            AddText(parent, Title(id), 145, 130, 651, 82, 42, TextAnchor.MiddleCenter, new Color32(247, 217, 151, 255));
            AddText(parent, "FUNCTIONAL DEMO • production art pending", 180, 278, 581, 42, 18, TextAnchor.MiddleCenter, new Color32(164, 191, 215, 255));
            AddPanel(parent, "Body", "ui_panel_parchment", 105, 350, 731, 870, Color.white);
            AddText(parent, Body(id), 155, 410, 631, 690, 25, TextAnchor.UpperLeft, new Color32(35, 30, 25, 255));
            BuildButtons(id, parent);
            return true;
        }

        public static bool IsDemoRoute(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.SplashTitle:
                case ScreenId.EventDetail:
                case ScreenId.IndividualRanking:
                case ScreenId.TeamHub:
                case ScreenId.TeamRanking:
                case ScreenId.TeamChat:
                case ScreenId.FriendsList:
                case ScreenId.Inventory:
                case ScreenId.SpecialOffer:
                case ScreenId.LanguageSelect:
                case ScreenId.Login:
                case ScreenId.TutorialOverlay:
                case ScreenId.OfflineState:
                case ScreenId.MaintenanceState:
                case ScreenId.EmptyErrorState:
                    return true;
                default:
                    return false;
            }
        }

        static string Title(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.SplashTitle: return "ЛЕГЕНДЫ РУСИ";
                case ScreenId.EventDetail: return "СОБЫТИЕ";
                case ScreenId.IndividualRanking: return "ЛИЧНЫЙ РЕЙТИНГ";
                case ScreenId.TeamHub: return "КОМАНДА";
                case ScreenId.TeamRanking: return "РЕЙТИНГ КОМАНД";
                case ScreenId.TeamChat: return "ЧАТ КОМАНДЫ";
                case ScreenId.FriendsList: return "ДРУЗЬЯ";
                case ScreenId.Inventory: return "ИНВЕНТАРЬ";
                case ScreenId.SpecialOffer: return "СПЕЦПРЕДЛОЖЕНИЕ";
                case ScreenId.LanguageSelect: return "ЯЗЫК";
                case ScreenId.Login: return "ВХОД";
                case ScreenId.TutorialOverlay: return "ОБУЧЕНИЕ";
                case ScreenId.OfflineState: return "НЕТ СЕТИ";
                case ScreenId.MaintenanceState: return "ТЕХНИЧЕСКИЕ РАБОТЫ";
                case ScreenId.EmptyErrorState: return "СОСТОЯНИЕ / ОШИБКА";
                default: return id.ToString().ToUpperInvariant();
            }
        }

        static string Body(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.SplashTitle:
                    return "Boot / title entry. The final title artwork will replace this demo panel.\n\nContinue → Loading → Home.";
                case ScreenId.EventDetail:
                    return "Event objective: complete levels and collect event progress.\n\nDemo reward claim: records a once-per-session acknowledgement only. No currency is granted without production LiveOps authority.\nProduction LiveOps backend remains disconnected.";
                case ScreenId.IndividualRanking:
                    return "1. Иван — 12 540\n2. Василиса — 11 980\n3. Добрыня — 10 420\n4. Алёна — 9 870\n\nLocal demo leaderboard only; production leaderboard requires server authority.";
                case ScreenId.TeamHub:
                    return "Team functional hub.\n\nRoutes: Team Ranking • Team Chat • Friends.\nProduction membership, invitations and team chest require a backend.";
                case ScreenId.TeamRanking:
                    return "1. Богатыри — 84 200\n2. Жар-Птицы — 78 650\n3. Северяне — 71 400\n\nDemo ranking data only.";
                case ScreenId.TeamChat:
                    return "Local demo chat. Send records a local session action and proves the UI/game-flow boundary.\n\nProduction messaging, moderation and persistence are not simulated.";
                case ScreenId.FriendsList:
                    return "Demo friends list:\n• Василиса — online\n• Добрыня — 12m ago\n• Алёна — 1h ago\n\nEnergy request is a local once-per-session demo action.";
                case ScreenId.Inventory:
                    return "Inventory reflects gameplay consumable categories.\n\nBogatyr Mace • Firebird • Living Light • Sea Pearl\n\nCounts remain owned by GameState/save-v6.";
                case ScreenId.SpecialOffer:
                    return "Commercial flow demo. Purchase requests still pass through the existing fail-closed monetization facade. No unverified currency grant is performed.";
                case ScreenId.LanguageSelect:
                    return "Choose the demo language state. Russian is the authored content language. English selection is session-only until a complete localization table is authored.";
                case ScreenId.Login:
                    return "Guest entry is supported for local demo flow. Production account sign-in, recovery, cloud save and identity providers remain explicit external integrations.";
                case ScreenId.TutorialOverlay:
                    return "Tutorial logic remains data-driven by the existing LevelTutorialPresenter. This page is a functional route placeholder for tutorial/help access.";
                case ScreenId.OfflineState:
                    return "Offline demo state. Core local gameplay can continue where data is available. Network-only social/LiveOps/commercial actions must fail safely.";
                case ScreenId.MaintenanceState:
                    return "Maintenance demo state. No destructive action occurs. Player can return to Home or retry after maintenance ends.";
                case ScreenId.EmptyErrorState:
                    return "Reusable empty/error state demo. Retry returns to Home in this local functional build.";
                default:
                    return "Functional demo page.";
            }
        }

        static void BuildButtons(ScreenId id, RectTransform parent)
        {
            switch (id)
            {
                case ScreenId.SplashTitle:
                    AddButtonVisual(parent, "Continue", "ПРОДОЛЖИТЬ", 250, 1320, 441, 120, true); break;
                case ScreenId.EventDetail:
                    AddButtonVisual(parent, "Claim", "ПОЛУЧИТЬ НАГРАДУ", 220, 1270, 501, 120, true); AddBack(parent); break;
                case ScreenId.TeamHub:
                    AddButtonVisual(parent, "Ranking", "РЕЙТИНГ КОМАНД", 220, 760, 501, 100, false);
                    AddButtonVisual(parent, "Chat", "ЧАТ КОМАНДЫ", 220, 885, 501, 100, false);
                    AddButtonVisual(parent, "Friends", "ДРУЗЬЯ", 220, 1010, 501, 100, false); AddBack(parent); break;
                case ScreenId.TeamChat:
                    AddButtonVisual(parent, "Send", "ОТПРАВИТЬ ДЕМО", 545, 1265, 280, 105, true); AddBack(parent); break;
                case ScreenId.FriendsList:
                    AddButtonVisual(parent, "Energy", "ЗАПРОСИТЬ ЭНЕРГИЮ", 220, 1270, 501, 110, true); AddBack(parent); break;
                case ScreenId.SpecialOffer:
                    AddButtonVisual(parent, "Buy", "КУПИТЬ", 250, 1270, 441, 120, true); AddBack(parent); break;
                case ScreenId.LanguageSelect:
                    AddButtonVisual(parent, "Ru", "РУССКИЙ", 220, 780, 501, 105, true);
                    AddButtonVisual(parent, "En", "ENGLISH (DEMO)", 220, 910, 501, 105, false); AddBack(parent); break;
                case ScreenId.Login:
                    AddButtonVisual(parent, "Guest", "ПРОДОЛЖИТЬ КАК ГОСТЬ", 220, 1220, 501, 120, true); AddBack(parent); break;
                case ScreenId.OfflineState:
                case ScreenId.EmptyErrorState:
                    AddButtonVisual(parent, "Retry", "ПОВТОРИТЬ", 250, 1270, 441, 120, true); AddBack(parent); break;
                default:
                    AddBack(parent); break;
            }
        }

        static void AddBack(RectTransform parent)
        {
            AddButtonVisual(parent, "Back", "НАЗАД", 35, 1450, 220, 105, false);
        }

        static void AddButtonVisual(RectTransform parent, string name, string label, float x, float y, float w, float h, bool primary)
        {
            AddPanel(parent, "Button_" + name, primary ? "ui_btn_primary" : "ui_btn_secondary", x, y, w, h, Color.white);
            AddText(parent, label, x + 18, y + 12, w - 36, h - 24, 25, TextAnchor.MiddleCenter, new Color32(247, 220, 161, 255));
        }

        static void AddPanel(RectTransform parent, string name, string key, float x, float y, float w, float h, Color tint)
        {
            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            Set(go.GetComponent<RectTransform>(), x, y, w, h);
            Image image = go.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(Kit + key);
            image.type = image.sprite != null && image.sprite.border.sqrMagnitude > 0f ? Image.Type.Sliced : Image.Type.Simple;
            image.color = image.sprite != null ? tint : new Color32(8, 27, 48, 255);
            image.raycastTarget = false;
        }

        static void AddText(RectTransform parent, string value, float x, float y, float w, float h, int size, TextAnchor anchor, Color color)
        {
            GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Set(go.GetComponent<RectTransform>(), x, y, w, h);
            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (text.font == null) text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = anchor;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
        }

        static void Set(RectTransform rt, float x, float yTop, float width, float height)
        {
            rt.anchorMin = new Vector2(x / W, 1f - (yTop + height) / H);
            rt.anchorMax = new Vector2((x + width) / W, 1f - yTop / H);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
