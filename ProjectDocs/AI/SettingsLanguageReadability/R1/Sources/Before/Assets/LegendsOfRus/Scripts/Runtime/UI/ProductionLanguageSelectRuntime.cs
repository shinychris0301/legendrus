using System;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus
{
    public static class ProductionLanguageSelectRuntime
    {
        public static void BuildInto(RectTransform root)
        {
            var existing = root.GetComponent<ProductionLanguageSelectView>();
            if (existing)
            {
                if (!existing.HasRequiredBindings) throw new InvalidOperationException("Repair Language Select outside Play Mode.");
                existing.Refresh(); return;
            }
            // Reuse actual approved resources, never a screenshot backdrop.
            var splash = Resources.Load<GameObject>("ProductionUI/Pages/SplashTitle");
            var settings = Resources.Load<GameObject>("ProductionUI/Pages/Settings").GetComponent<ProductionSettingsView>();
            if (!splash || !splash.GetComponent<SplashTitleView>() || !splash.GetComponent<SplashTitleView>().HasRequiredBindings || !settings || !settings.HasRequiredBindings) throw new InvalidOperationException("Approved Splash and Settings are required.");
            var v = root.gameObject.AddComponent<ProductionLanguageSelectView>();
            var background = UnityEngine.Object.Instantiate(splash, root, false);
            background.name = "ApprovedSplashBackdrop";
            var rect = (RectTransform)background.transform; rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            v.BackdropView = background.GetComponent<SplashTitleView>();
            v.Backdrop = background.GetComponent<CanvasGroup>();
            if (!v.Backdrop) v.Backdrop = background.AddComponent<CanvasGroup>();
            v.Backdrop.interactable = false; v.Backdrop.blocksRaycasts = false;
            v.Shield = Rect(root, "InputShield"); v.Shield.sizeDelta = new Vector2(941,1672);
            var shield = v.Shield.gameObject.AddComponent<Image>(); shield.color = new Color(0,0,0,.6f); shield.raycastTarget = true;
            var choices = UnityEngine.Object.Instantiate(settings.LanguageChoices, root, false);
            choices.name = "LanguageChoices"; choices.SetActive(true); v.Body = (RectTransform)choices.transform;
            v.Panel = v.Body.Find("LanguagePanel").GetComponent<Image>();
            v.Heading = v.Body.Find("LanguageHeading").GetComponent<Text>();
            v.Russian = v.Body.Find("Language_0").GetComponent<Button>();
            v.English = v.Body.Find("Language_1").GetComponent<Button>();
            v.Close = v.Body.Find("CloseLanguage").GetComponent<Button>();
            // Keep the standalone modal target consistent with Settings, without resizing the source art.
            v.Close.image.raycastPadding = new Vector4(-3,0,-3,0);
            v.TitleFont = settings.TitleFont;
            foreach(var button in new[]{v.Russian,v.English,v.Close}){
                button.transition=Selectable.Transition.None;
                button.targetGraphic.CrossFadeColor(Color.white,0,true,true);
                if(!button.GetComponent<ProductionMapButtonFeedback>())button.gameObject.AddComponent<ProductionMapButtonFeedback>();
            }
            var layout = root.gameObject.AddComponent<ProductionLanguageSelectLayout>(); layout.View = v;
            v.Refresh(); layout.ApplyNow();
        }
        static RectTransform Rect(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform)); go.layer = 5; go.transform.SetParent(parent,false);
            var r = (RectTransform)go.transform; r.anchorMin = r.anchorMax = r.pivot = new Vector2(0,1); return r;
        }
    }
}
