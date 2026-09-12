using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Live Loading labels and measured progress; ScreenRouter owns work and navigation.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionLoadingView : MonoBehaviour
    {
        public const int CurrentRevision = 3;
        public int Revision = CurrentRevision;
        public SplashTitleTypography Typography;
        public Image Environment;
        public Image Logo;
        public Image Ivan;
        public Image Firebird;
        public Image TipPanel;
        public ProductionLoadingShade LowerShade;
        public Text TipTitle;
        public Text TipBody;
        public Text StatusText;
        public Text PercentText;
        public ProductionLoadingProgressBar ProgressBar;
        public Button RetryButton;
        public Text RetryText;

        public bool HasRequiredBindings => Revision == CurrentRevision && Typography != null &&
            Environment != null && Environment.sprite != null && Logo != null && Logo.sprite != null &&
            Ivan != null && Ivan.sprite != null && Firebird != null && Firebird.sprite != null &&
            TipPanel != null && TipPanel.sprite != null && LowerShade != null &&
            LowerShade.GetComponent<CanvasRenderer>() != null && TipTitle != null && TipBody != null &&
            StatusText != null && PercentText != null && ProgressBar != null &&
            ProgressBar.GetComponent<CanvasRenderer>() != null &&
            GetComponent<ProductionLoadingShadeLayout>() != null &&
            GetComponent<ProductionLoadingShadeLayout>().Shade == LowerShade &&
            RetryButton != null && RetryText != null;

        UnityAction retryListener;
        string lastLanguage;
        bool indeterminate = true;
        bool failed;
        float progress;

        void OnEnable() { RefreshContent(); }
        void Update()
        {
            if (lastLanguage != DemoFeatureState.LanguageCode) RefreshContent();
        }
        void OnDisable() { UnbindRetry(); }

        public void BindRetry(Action retry)
        {
            UnbindRetry();
            if (RetryButton == null) return;
            retryListener = () =>
            {
                if (!isActiveAndEnabled || !failed || retry == null) return;
                // Disable before invoking: a second tap cannot start another attempt.
                SetIndeterminate();
                retry();
            };
            RetryButton.onClick.AddListener(retryListener);
        }

        void UnbindRetry()
        {
            if (RetryButton != null && retryListener != null)
                RetryButton.onClick.RemoveListener(retryListener);
            retryListener = null;
        }

        public void SetProgress(float progress01)
        {
            if (float.IsNaN(progress01) || float.IsInfinity(progress01)) return;
            progress = Mathf.Clamp01(progress01);
            indeterminate = false;
            failed = false;
            RefreshState();
        }

        public void SetIndeterminate()
        {
            progress = 0f;
            indeterminate = true;
            failed = false;
            RefreshState();
        }

        public void ShowError()
        {
            failed = true;
            indeterminate = false;
            RefreshState();
        }

        public void RefreshContent()
        {
            if (!HasRequiredBindings) return;
            lastLanguage = DemoFeatureState.LanguageCode;
            bool en = lastLanguage == "en";
            TipTitle.text = en ? "TIP" : "СОВЕТ";
            TipBody.text = en
                ? "Tip: combine special gems to clear the darkness faster."
                : "Совет: объединяйте особые самоцветы,\nчтобы очищать тьму быстрее.";
            RetryText.text = en ? "Try again" : "Повторить";
            string source;
            Font font = Typography.Resolve(false, out source);
            Text[] labels = { TipTitle, TipBody, StatusText, PercentText, RetryText };
            foreach (Text label in labels) { label.font = font; label.raycastTarget = false; }
            RefreshState();
        }

        void RefreshState()
        {
            if (ProgressBar == null || StatusText == null || PercentText == null || RetryButton == null) return;
            bool en = DemoFeatureState.LanguageCode == "en";
            StatusText.text = failed ? (en ? "Unable to load" : "Не удалось загрузить")
                : (en ? "Loading..." : "Загрузка...");
            ProgressBar.SetProgress(progress, indeterminate);
            PercentText.gameObject.SetActive(!indeterminate && !failed);
            // Never round incomplete work up to 100%.
            PercentText.text = Mathf.FloorToInt(progress * 100f) + "%";
            RetryButton.gameObject.SetActive(failed);
            RetryButton.interactable = failed;
        }
    }
}
