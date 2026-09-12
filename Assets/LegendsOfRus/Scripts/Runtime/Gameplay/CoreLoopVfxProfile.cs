using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Provider-neutral visual profile for the premium core-loop feedback pass.
    /// All values affect presentation only.
    /// </summary>
    public static class CoreLoopVfxProfile
    {
        public const int DefaultPoolCapacity = 32;
        public const int MaximumPoolCapacity = 128;
        public const int MaximumBurstEffectsPerCue = 3;
        public const int MaximumCascadePulses = 3;

        static readonly Color Gold = new Color(1f, 0.80f, 0.34f, 0.72f);
        static readonly Color PaleGold = new Color(1f, 0.94f, 0.68f, 0.64f);
        static readonly Color Reject = new Color(0.95f, 0.25f, 0.20f, 0.62f);
        static readonly Color Bronze = new Color(0.76f, 0.43f, 0.17f, 0.78f);
        static readonly Color Fire = new Color(1f, 0.31f, 0.08f, 0.78f);
        static readonly Color LivingLight = new Color(1f, 0.96f, 0.62f, 0.68f);
        static readonly Color SeaPearl = new Color(0.30f, 0.82f, 1f, 0.26f);
        static readonly Color Reshuffle = new Color(0.44f, 0.57f, 0.92f, 0.48f);
        static readonly Color Recovery = new Color(1f, 0.65f, 0.22f, 0.42f);

        public static CoreLoopVfxStyle Resolve(CoreLoopVfxCue cue)
        {
            switch (cue)
            {
                case CoreLoopVfxCue.SwapTrail:
                    return Fade(PaleGold, 0.16f, 0.72f, 1.04f);
                case CoreLoopVfxCue.SwapCommitGlint:
                    return Pulse(PaleGold, 0.18f, 0.48f, 1.26f);
                case CoreLoopVfxCue.SwapRejectPulse:
                    return Pulse(Reject, 0.22f, 0.76f, 1.12f);
                case CoreLoopVfxCue.MatchBurst:
                    return Pulse(Gold, 0.25f, 0.38f, 1.34f, 18f);
                case CoreLoopVfxCue.MatchResolveGlint:
                    return Fade(PaleGold, 0.20f, 0.62f, 1.18f);
                case CoreLoopVfxCue.CascadePulse:
                    return Pulse(
                        new Color(1f, 0.86f, 0.38f, 0.34f),
                        0.34f,
                        0.90f,
                        1.08f
                    );
                case CoreLoopVfxCue.MajorCombinationPulse:
                    return Pulse(
                        new Color(1f, 0.98f, 0.86f, 0.52f),
                        0.42f,
                        0.86f,
                        1.13f
                    );
                case CoreLoopVfxCue.SpecialCreationHalo:
                    return Pulse(PaleGold, 0.32f, 0.56f, 1.44f, 25f);
                case CoreLoopVfxCue.SpecialRowSweep:
                case CoreLoopVfxCue.SpecialColumnSweep:
                    return Fade(Gold, 0.28f, 0.82f, 1.04f);
                case CoreLoopVfxCue.SpecialBurstPulse:
                    return Pulse(Fire, 0.34f, 0.50f, 1.28f, 12f);
                case CoreLoopVfxCue.SpecialColorWash:
                    return Pulse(
                        new Color(0.92f, 0.78f, 1f, 0.44f),
                        0.38f,
                        0.92f,
                        1.06f
                    );
                case CoreLoopVfxCue.SpecialActivationPulse:
                    return Pulse(Gold, 0.28f, 0.58f, 1.30f);
                case CoreLoopVfxCue.FallTrail:
                    return Fade(
                        new Color(0.85f, 0.91f, 1f, 0.36f),
                        0.20f,
                        0.72f,
                        1.02f
                    );
                case CoreLoopVfxCue.FallSettleSpark:
                    return Pulse(PaleGold, 0.20f, 0.42f, 1.20f, 16f);
                case CoreLoopVfxCue.ReshuffleWash:
                    return Pulse(Reshuffle, 0.38f, 0.94f, 1.05f);
                case CoreLoopVfxCue.ReshuffleSettle:
                    return Fade(PaleGold, 0.24f, 0.90f, 1.02f);
                case CoreLoopVfxCue.RecoveryWash:
                    return Pulse(Recovery, 0.30f, 0.96f, 1.03f);
                case CoreLoopVfxCue.BoosterMaceRow:
                case CoreLoopVfxCue.BoosterMaceColumn:
                    return Fade(Bronze, 0.34f, 0.76f, 1.08f);
                case CoreLoopVfxCue.BoosterFirebird:
                    return Pulse(Fire, 0.46f, 0.42f, 1.42f, 22f);
                case CoreLoopVfxCue.BoosterLivingLight:
                    return Pulse(LivingLight, 0.50f, 0.92f, 1.08f);
                case CoreLoopVfxCue.BoosterSeaPearl:
                    return Pulse(SeaPearl, 0.22f, 0.82f, 1.16f);
                case CoreLoopVfxCue.BoosterMaceCompleted:
                    return Fade(PaleGold, 0.24f, 0.62f, 1.16f);
                case CoreLoopVfxCue.BoosterFirebirdCompleted:
                    return Pulse(
                        new Color(1f, 0.67f, 0.22f, 0.60f),
                        0.28f,
                        0.54f,
                        1.25f
                    );
                case CoreLoopVfxCue.BoosterLivingLightCompleted:
                    return Fade(LivingLight, 0.34f, 0.84f, 1.09f);
                case CoreLoopVfxCue.BoosterSeaPearlCompleted:
                    return Fade(SeaPearl, 0.20f, 0.84f, 1.12f);
                case CoreLoopVfxCue.BoosterRejected:
                    return Pulse(Reject, 0.24f, 0.72f, 1.14f);
                default:
                    return Fade(Color.clear, 0.01f, 1f, 1f);
            }
        }

        public static bool IsBoosterCue(CoreLoopVfxCue cue)
        {
            return
                cue >= CoreLoopVfxCue.BoosterMaceRow &&
                cue <= CoreLoopVfxCue.BoosterRejected;
        }

        static CoreLoopVfxStyle Fade(
            Color color,
            float duration,
            float startScale,
            float endScale,
            float rotation = 0f
        )
        {
            Color end = color;
            end.a = 0f;
            return new CoreLoopVfxStyle(
                color,
                end,
                duration,
                startScale,
                endScale,
                -rotation * 0.5f,
                rotation,
                false
            );
        }

        static CoreLoopVfxStyle Pulse(
            Color color,
            float duration,
            float startScale,
            float endScale,
            float rotation = 0f
        )
        {
            Color end = color;
            end.a = 0f;
            return new CoreLoopVfxStyle(
                color,
                end,
                duration,
                startScale,
                endScale,
                -rotation * 0.5f,
                rotation,
                true
            );
        }
    }
}
