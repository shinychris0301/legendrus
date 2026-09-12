#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using LegendsOfRus.GameplayCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step14CoreLoopVfxBoosterFeedbackSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.14 Step 2 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check(
                "default pool is bounded",
                () =>
                    CoreLoopVfxProfile.DefaultPoolCapacity > 0 &&
                    CoreLoopVfxProfile.DefaultPoolCapacity <=
                        CoreLoopVfxProfile.MaximumPoolCapacity,
                ref passed,
                ref total,
                failures
            );
            Check(
                "burst count fits the default pool",
                () =>
                    CoreLoopVfxProfile.MaximumBurstEffectsPerCue > 0 &&
                    CoreLoopVfxProfile.MaximumBurstEffectsPerCue <=
                        CoreLoopVfxProfile.DefaultPoolCapacity,
                ref passed,
                ref total,
                failures
            );
            Check(
                "cascade response is restrained",
                () => CoreLoopVfxProfile.MaximumCascadePulses >= 1 &&
                    CoreLoopVfxProfile.MaximumCascadePulses <= 4,
                ref passed,
                ref total,
                failures
            );
            Check(
                "all concrete cue styles are bounded",
                AllConcreteStylesAreBounded,
                ref passed,
                ref total,
                failures
            );

            Check(
                "row special has a dedicated cue",
                () => ResolveSpecial(SpecialGemKind.Row) == CoreLoopVfxCue.SpecialRowSweep,
                ref passed,
                ref total,
                failures
            );
            Check(
                "column special has a dedicated cue",
                () => ResolveSpecial(SpecialGemKind.Column) == CoreLoopVfxCue.SpecialColumnSweep,
                ref passed,
                ref total,
                failures
            );
            Check(
                "burst special has a dedicated cue",
                () => ResolveSpecial(SpecialGemKind.Burst) == CoreLoopVfxCue.SpecialBurstPulse,
                ref passed,
                ref total,
                failures
            );
            Check(
                "color special has a dedicated cue",
                () => ResolveSpecial(SpecialGemKind.Color) == CoreLoopVfxCue.SpecialColorWash,
                ref passed,
                ref total,
                failures
            );

            CheckBooster(
                "mace row activation is distinct",
                GameplayBoosterKind.BogatyrMaceRow,
                CoreLoopVfxCue.BoosterMaceRow,
                CoreLoopVfxCue.BoosterMaceCompleted,
                ref passed,
                ref total,
                failures
            );
            CheckBooster(
                "mace column activation is distinct",
                GameplayBoosterKind.BogatyrMaceColumn,
                CoreLoopVfxCue.BoosterMaceColumn,
                CoreLoopVfxCue.BoosterMaceCompleted,
                ref passed,
                ref total,
                failures
            );
            CheckBooster(
                "Firebird lifecycle is distinct",
                GameplayBoosterKind.Firebird,
                CoreLoopVfxCue.BoosterFirebird,
                CoreLoopVfxCue.BoosterFirebirdCompleted,
                ref passed,
                ref total,
                failures
            );
            CheckBooster(
                "Living Light lifecycle is distinct",
                GameplayBoosterKind.LivingLight,
                CoreLoopVfxCue.BoosterLivingLight,
                CoreLoopVfxCue.BoosterLivingLightCompleted,
                ref passed,
                ref total,
                failures
            );
            CheckBooster(
                "Sea Pearl lifecycle is distinct",
                GameplayBoosterKind.SeaPearl,
                CoreLoopVfxCue.BoosterSeaPearl,
                CoreLoopVfxCue.BoosterSeaPearlCompleted,
                ref passed,
                ref total,
                failures
            );
            Check(
                "booster rejection has a dedicated cue",
                () => CoreLoopVfxPresenter.ResolveCue(
                    Motion(
                        GameplayMotionKind.BoosterRejected,
                        GameplayBoosterKind.Firebird
                    )
                ) == CoreLoopVfxCue.BoosterRejected,
                ref passed,
                ref total,
                failures
            );

            GameplayMotionEvent semanticMotion = new GameplayMotionEvent(
                17,
                GameplayMotionKind.SpecialActivationStarted,
                new BoardPosition(3, 4),
                new BoardPosition(3, 4),
                3,
                2,
                SpecialGemKind.Burst,
                "step14-self-test",
                SpecialCombinationKind.BurstWithBurst,
                GameplayBoosterKind.None
            );
            Check(
                "pure mapper preserves semantic payload",
                () =>
                {
                    if (!CoreLoopVfxMapper.TryCreateRequest(
                        semanticMotion,
                        out CoreLoopVfxRequest request
                    )) return false;
                    return
                        request.MotionSequence == 17 &&
                        request.Cue == CoreLoopVfxCue.SpecialBurstPulse &&
                        request.Primary == new BoardPosition(3, 4) &&
                        request.Magnitude == 2 &&
                        request.CascadeDepth == 3 &&
                        request.SpecialKind == SpecialGemKind.Burst &&
                        request.CombinationKind == SpecialCombinationKind.BurstWithBurst &&
                        request.PlanHash == "step14-self-test";
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "pure mapper rejects null input",
                () => !CoreLoopVfxMapper.TryCreateRequest(null, out _),
                ref passed,
                ref total,
                failures
            );

            Check(
                "booster feedback keeps booster identity",
                () =>
                {
                    GameplayMotionEvent activated = Motion(
                        GameplayMotionKind.BoosterActivated,
                        GameplayBoosterKind.Firebird
                    );
                    return CoreLoopFeedbackService.TryCreateSignal(
                        activated,
                        out CoreLoopFeedbackSignal signal,
                        out bool haptic
                    ) &&
                    signal.Cue == CoreLoopFeedbackCue.BoosterActivated &&
                    signal.BoosterKind == GameplayBoosterKind.Firebird &&
                    haptic;
                },
                ref passed,
                ref total,
                failures
            );
            Check(
                "booster completion is not a haptic duplicate",
                () =>
                {
                    GameplayMotionEvent completed = Motion(
                        GameplayMotionKind.BoosterCompleted,
                        GameplayBoosterKind.SeaPearl
                    );
                    return CoreLoopFeedbackService.TryCreateSignal(
                        completed,
                        out CoreLoopFeedbackSignal signal,
                        out bool haptic
                    ) &&
                    signal.Cue == CoreLoopFeedbackCue.BoosterCompleted &&
                    signal.BoosterKind == GameplayBoosterKind.SeaPearl &&
                    !haptic;
                },
                ref passed,
                ref total,
                failures
            );

            Check(
                "board exposes its local VFX presenter",
                () => typeof(Match3Board).GetProperty("VfxPresenter") != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "presenter accepts board initialization",
                () => typeof(CoreLoopVfxPresenter).GetMethod(
                    "Initialize",
                    new[] { typeof(Match3Board) }
                ) != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "presenter exposes deterministic clock seam",
                () => typeof(CoreLoopVfxPresenter).GetMethod(
                    "TickForTesting",
                    new[] { typeof(float) }
                ) != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "presenter exposes idempotent cleanup",
                () =>
                    typeof(CoreLoopVfxPresenter).GetMethod("ResetEffects") != null &&
                    typeof(CoreLoopVfxPresenter).GetMethod("ClearAll") != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "presenter exposes bounded pool diagnostics",
                () =>
                    HasPublicProperty(typeof(CoreLoopVfxPresenter), "PoolCapacity") &&
                    HasPublicProperty(typeof(CoreLoopVfxPresenter), "CreatedEffectCount") &&
                    HasPublicProperty(typeof(CoreLoopVfxPresenter), "AvailableEffectCount") &&
                    HasPublicProperty(typeof(CoreLoopVfxPresenter), "ActiveEffectCount") &&
                    HasPublicProperty(typeof(CoreLoopVfxPresenter), "PeakActiveEffectCount") &&
                    HasPublicProperty(typeof(CoreLoopVfxPresenter), "DroppedEffectCount"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "presenter exposes semantic traces",
                () =>
                    typeof(CoreLoopVfxPresenter).GetEvent("EffectTracePublished") != null &&
                    HasPublicProperty(typeof(CoreLoopVfxPresenter), "LastEffectTrace"),
                ref passed,
                ref total,
                failures
            );
            Check(
                "pool exposes raycast-safety state",
                () =>
                    HasPublicProperty(typeof(PooledUiVfxLayer), "IsRaycastSafe") &&
                    typeof(PooledUiVfxLayer).GetMethod("ResetEffects") != null,
                ref passed,
                ref total,
                failures
            );
            Check(
                "VFX types contain no save dependency",
                () =>
                    typeof(CoreLoopVfxPresenter).Assembly == typeof(GameState).Assembly &&
                    typeof(CoreLoopVfxRequest).GetProperty("PlanHash") != null &&
                    typeof(CoreLoopVfxRequest).GetProperty("BoosterKind") != null,
                ref passed,
                ref total,
                failures
            );

            if (failures.Count == 0)
            {
                Debug.Log(
                    $"v1.14 Step 2 core-loop VFX and booster feedback " +
                    $"self-tests passed: {passed}/{total}."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        $"v1.14 Step 2 self-tests passed: {passed}/{total}",
                        "OK"
                    );
                }
                return;
            }

            Debug.LogError(
                $"v1.14 Step 2 self-tests failed: {passed}/{total}\n" +
                string.Join("\n", failures)
            );
            if (Application.isBatchMode)
            {
                throw new InvalidOperationException(
                    "v1.14 Step 2 self-tests failed."
                );
            }
        }

        static void CheckBooster(
            string name,
            GameplayBoosterKind booster,
            CoreLoopVfxCue activation,
            CoreLoopVfxCue completion,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            Check(
                name,
                () =>
                    CoreLoopVfxPresenter.ResolveCue(
                        Motion(GameplayMotionKind.BoosterActivated, booster)
                    ) == activation &&
                    CoreLoopVfxPresenter.ResolveCue(
                        Motion(GameplayMotionKind.BoosterCompleted, booster)
                    ) == completion,
                ref passed,
                ref total,
                failures
            );
        }

        static CoreLoopVfxCue ResolveSpecial(SpecialGemKind special)
        {
            return CoreLoopVfxPresenter.ResolveCue(
                new GameplayMotionEvent(
                    1,
                    GameplayMotionKind.SpecialActivationStarted,
                    new BoardPosition(3, 4),
                    new BoardPosition(3, 4),
                    1,
                    1,
                    special,
                    "step14-self-test"
                )
            );
        }

        static GameplayMotionEvent Motion(
            GameplayMotionKind kind,
            GameplayBoosterKind booster
        )
        {
            return new GameplayMotionEvent(
                1,
                kind,
                new BoardPosition(3, 4),
                new BoardPosition(3, 4),
                1,
                booster == GameplayBoosterKind.SeaPearl ? 5 : 9,
                SpecialGemKind.None,
                booster == GameplayBoosterKind.SeaPearl
                    ? string.Empty
                    : "step14-self-test",
                SpecialCombinationKind.None,
                booster
            );
        }

        static bool AllConcreteStylesAreBounded()
        {
            foreach (CoreLoopVfxCue cue in Enum.GetValues(typeof(CoreLoopVfxCue)))
            {
                if (cue == CoreLoopVfxCue.None) continue;
                CoreLoopVfxStyle style = CoreLoopVfxProfile.Resolve(cue);
                if (
                    style.DurationSeconds < 0.01f ||
                    style.DurationSeconds > 0.75f ||
                    style.StartScale < 0f ||
                    style.StartScale > 2f ||
                    style.EndScale < 0f ||
                    style.EndScale > 2f ||
                    style.StartColor.a < 0f ||
                    style.StartColor.a > 1f ||
                    style.EndColor.a < 0f ||
                    style.EndColor.a > 1f
                ) return false;
            }
            return true;
        }

        static bool HasPublicProperty(Type type, string name)
        {
            return type.GetProperty(
                name,
                BindingFlags.Instance | BindingFlags.Public
            ) != null;
        }

        static void Check(
            string name,
            Func<bool> test,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (test()) passed++;
                else failures.Add("FAIL " + name);
            }
            catch (Exception exception)
            {
                failures.Add("ERROR " + name + ": " + exception.Message);
            }
        }
    }
}
#endif
