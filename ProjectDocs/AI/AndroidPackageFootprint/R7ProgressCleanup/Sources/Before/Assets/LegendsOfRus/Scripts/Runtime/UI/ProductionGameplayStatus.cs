using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>One replaceable Gameplay message, above the board and outside all controls.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionGameplayStatus : MonoBehaviour
    {
        public ProductionGameplayView View;
        public RectTransform Panel;
        public Text Label;
        public bool HasRequiredBindings => View != null && Panel != null && Label != null;
        public bool IsVisible => HasRequiredBindings && Panel.gameObject.activeSelf;
        public string SourceMessage { get; private set; }
        float expires;
        bool instruction;
        string language;

        static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            { "Булава: коснитесь клетки для ряда или проведите вверх/вниз для столбца.", "Mace: tap a cell for a row, or swipe up/down for a column." },
            { "Перо Жар-Птицы: выберите клетку — очистится область 3×3.", "Firebird Feather: choose a cell to clear a 3×3 area." },
            { "Живой Свет: выберите самоцвет — будут очищены все самоцветы этого цвета.", "Living Light: choose a gem to clear all gems of that color." },
            { "Булава Богатыря сокрушает преграды.", "The Bogatyr's Mace crushes the obstacles." },
            { "Перо Жар-Птицы вспыхивает живым огнём.", "The Firebird Feather bursts into living flame." },
            { "Живой Свет очищает выбранный цвет.", "Living Light clears the selected color." },
            { "Жемчужина Морского Царя: +5 ходов.", "Sea King's Pearl: +5 moves." },
            { "Жемчужина закончилась.", "No pearls remaining." },
            { "Этот бустер закончился.", "No boosters of this kind remaining." },
            { "Бустер отменён.", "Booster canceled." },
            { "Выберите заполненную клетку.", "Choose an occupied cell." },
            { "Нет доступных ходов — поле перемешано.", "No available moves — the board was reshuffled." },
            { "Бустер восстановлен после ошибки. Повторите действие.", "The booster was restored after an error. Please try again." },
            { "Ход восстановлен после ошибки анимации. Повторите действие.", "The move was restored after an animation error. Please try again." }
        };

        public void Show(string message)
        {
            if (!HasRequiredBindings) return;
            SourceMessage = message ?? string.Empty;
            // Arming instructions remain readable until the next board message.
            instruction = SourceMessage == "Булава: коснитесь клетки для ряда или проведите вверх/вниз для столбца." ||
                SourceMessage == "Перо Жар-Птицы: выберите клетку — очистится область 3×3." ||
                SourceMessage == "Живой Свет: выберите самоцвет — будут очищены все самоцветы этого цвета.";
            expires = instruction ? float.PositiveInfinity : Time.unscaledTime + 2.2f;
            RefreshLanguage();
            Panel.gameObject.SetActive(!string.IsNullOrEmpty(SourceMessage));
        }

        public void Clear()
        {
            SourceMessage = null;
            if (HasRequiredBindings) { Label.text = string.Empty; Panel.gameObject.SetActive(false); }
        }

        void Update()
        {
            if (!IsVisible) return;
            if (instruction && View.Board != null && !View.Board.IsBoosterArmed) { Clear(); return; }
            if (language != DemoFeatureState.LanguageCode) RefreshLanguage();
            if (Time.unscaledTime >= expires) Clear();
        }
        void OnDisable() => Clear();
        void RefreshLanguage()
        {
            language = DemoFeatureState.LanguageCode;
            Label.text = language == "en" && English.TryGetValue(SourceMessage, out string translated)
                ? translated : SourceMessage;
        }
    }
}
