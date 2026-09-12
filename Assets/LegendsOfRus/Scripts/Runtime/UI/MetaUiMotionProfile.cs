using UnityEngine;

namespace LegendsOfRus
{
    public static class MetaUiMotionProfile
    {
        public const int MaximumRegisteredMotions = 128;
        public const float MaximumTickSeconds = 0.05f;
        public const float MaximumTransitionSeconds = 0.45f;
        public const float MaximumOffsetPixels = 32f;

        public static MetaUiMotionPreset ResolveScreenPreset(ScreenId id)
        {
            switch (id)
            {
                case ScreenId.Victory:
                    return MetaUiMotionPreset.ScreenVictory;
                case ScreenId.Fail:
                    return MetaUiMotionPreset.ScreenFailure;
                case ScreenId.ChapterComplete:
                    return MetaUiMotionPreset.ScreenChapterComplete;
                default:
                    return MetaUiMotionPreset.ScreenStandard;
            }
        }

        public static MetaUiMotionPreset ResolveModalPreset(ScreenId id)
        {
            return id == ScreenId.RewardClaim
                ? MetaUiMotionPreset.ModalReward
                : MetaUiMotionPreset.ModalStandard;
        }

        public static MetaUiTransitionStyle ResolveTransition(MetaUiMotionPreset preset)
        {
            switch (preset)
            {
                case MetaUiMotionPreset.ScreenVictory:
                    return AlphaOnly(0f, 0.24f, 0.12f, true);
                case MetaUiMotionPreset.ScreenFailure:
                    return AlphaOnly(0f, 0.20f, 0.12f, true);
                case MetaUiMotionPreset.ScreenChapterComplete:
                    return AlphaOnly(0f, 0.26f, 0.12f, true);
                case MetaUiMotionPreset.ModalStandard:
                    return AlphaOnly(0f, 0.16f, 0.13f, true);
                case MetaUiMotionPreset.ModalReward:
                    return AlphaOnly(0f, 0.22f, 0.15f, true);
                case MetaUiMotionPreset.OutcomeVictory:
                    return Overlay(0.10f, 0.23f, 0.14f, 0.94f, new Vector2(0f, -18f), true);
                case MetaUiMotionPreset.OutcomeFailure:
                    return Overlay(0.08f, 0.20f, 0.14f, 0.97f, new Vector2(0f, 14f), false);
                case MetaUiMotionPreset.OutcomeReward:
                    return Overlay(0.12f, 0.25f, 0.15f, 0.90f, new Vector2(0f, -12f), true);
                case MetaUiMotionPreset.ChapterComplete:
                    return Overlay(0.12f, 0.26f, 0.15f, 0.92f, new Vector2(0f, -18f), true);
                default:
                    return AlphaOnly(0f, 0.18f, 0.12f, true);
            }
        }

        public static MetaUiPressStyle ResolvePress(MetaUiControlRole role)
        {
            switch (role)
            {
                case MetaUiControlRole.Primary:
                    return Press(0.055f, 0.11f, 0.94f, 1.05f, 0.13f, 0.20f, true);
                case MetaUiControlRole.Back:
                    return Press(0.045f, 0.09f, 0.96f, 1.02f, 0.09f, 0.14f, true);
                case MetaUiControlRole.Reward:
                case MetaUiControlRole.OneShotReward:
                    return Press(0.060f, 0.14f, 0.93f, 1.08f, 0.15f, 0.24f, true);
                case MetaUiControlRole.MapNode:
                    // Leave a small cushion after the 0.16-second local pulse
                    // before routing. Avoid a giant highlight across the broad
                    // map fallback hotspot.
                    return Press(0.18f, 0.13f, 1f, 1f, 0.001f, 0.001f, false);
                case MetaUiControlRole.Gameplay:
                    // Preserve the established core-loop input latency and avoid
                    // adding a new meta flourish over gameplay booster artwork.
                    return Press(0.065f, 0.08f, 0.972f, 0.965f, 0.105f, 0.105f, false);
                default:
                    return Press(0.050f, 0.10f, 0.955f, 1.035f, 0.10f, 0.16f, true);
            }
        }

        public static float EaseOutCubic(float value)
        {
            float p = Mathf.Clamp01(value);
            float inverse = 1f - p;
            return 1f - inverse * inverse * inverse;
        }

        public static float EaseInCubic(float value)
        {
            float p = Mathf.Clamp01(value);
            return p * p * p;
        }

        public static float EaseOutBack(float value)
        {
            float p = Mathf.Clamp01(value) - 1f;
            const float overshoot = 1.35f;
            return 1f + p * p * ((overshoot + 1f) * p + overshoot);
        }

        public static float ClampTick(float deltaTime)
        {
            return Mathf.Clamp(deltaTime, 0f, MaximumTickSeconds);
        }

        static MetaUiTransitionStyle AlphaOnly(
            float delay,
            float enter,
            float exit,
            bool enablesInteraction
        )
        {
            return new MetaUiTransitionStyle(
                delay,
                enter,
                exit,
                0f,
                1f,
                Vector2.zero,
                1f,
                Vector2.zero,
                false,
                enablesInteraction
            );
        }

        static MetaUiTransitionStyle Overlay(
            float delay,
            float enter,
            float exit,
            float startScale,
            Vector2 startOffset,
            bool overshoot
        )
        {
            return new MetaUiTransitionStyle(
                delay,
                enter,
                exit,
                0f,
                startScale,
                startOffset,
                0.98f,
                -startOffset * 0.35f,
                overshoot,
                false
            );
        }

        static MetaUiPressStyle Press(
            float invokeDelay,
            float releaseDuration,
            float pressedScale,
            float activatedScale,
            float pressedAlpha,
            float activatedAlpha,
            bool showsHighlight
        )
        {
            return new MetaUiPressStyle(
                invokeDelay,
                releaseDuration,
                pressedScale,
                activatedScale,
                new Color(1f, 0.78f, 0.24f, pressedAlpha),
                new Color(1f, 0.88f, 0.45f, activatedAlpha),
                showsHighlight
            );
        }
    }
}
