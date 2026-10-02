using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Central timing contract for the first premium-feel pass. These values
    /// affect presentation time only; board resolution remains plan-driven.
    /// </summary>
    public static class CoreLoopMotionProfile
    {
        public const float SelectionPulseSeconds = 0.12f;
        public const float SwapSeconds = 0.14f;
        public const float InvalidSwapReturnSeconds = 0.12f;
        public const float RejectedSwapSeconds = 0.14f;
        public const float ClearSeconds = 0.15f;
        public const float SpecialCreationSeconds = 0.22f;
        public const float SpecialActivationHoldSeconds = 0.18f;
        public const float FallSeconds = 0.18f;
        public const float RejectedOffsetInCells = 0.10f;
        public const float SwipeAxisDominance = 1.15f;

        public static float EaseOutCubic(float value)
        {
            float t = Mathf.Clamp01(value);
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        public static float EaseInOutSine(float value)
        {
            float t = Mathf.Clamp01(value);
            return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
        }
    }
}
