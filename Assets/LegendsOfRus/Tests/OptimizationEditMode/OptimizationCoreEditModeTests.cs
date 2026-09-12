using System;
using LegendsOfRus.OptimizationCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.Optimization
{
    [TestFixture]
    public sealed class OptimizationCoreEditModeTests
    {
        [Test]
        public void Policy_IsPinnedAndValid()
        {
            Assert.That(OptimizationPolicy.IsValid, Is.True);
            Assert.That(OptimizationPolicy.SchemaVersion, Is.EqualTo(1));
            Assert.That(OptimizationPolicy.AllocatedWarningBytes,
                Is.EqualTo(384L * 1024L * 1024L));
            Assert.That(OptimizationPolicy.AllocatedCriticalBytes,
                Is.EqualTo(512L * 1024L * 1024L));
            Assert.That(OptimizationPolicy.RuntimeTextureMaximumDimension,
                Is.EqualTo(2048));
        }

        [Test]
        public void MemoryEvaluator_UsesInclusiveWarningAndCriticalBoundaries()
        {
            Assert.That(
                MemoryBudgetEvaluator.Evaluate(
                    OptimizationPolicy.AllocatedWarningBytes - 1,
                    0,
                    0
                ),
                Is.EqualTo(MemoryBudgetBand.WithinBudget)
            );
            Assert.That(
                MemoryBudgetEvaluator.Evaluate(
                    OptimizationPolicy.AllocatedWarningBytes,
                    0,
                    0
                ),
                Is.EqualTo(MemoryBudgetBand.Warning)
            );
            Assert.That(
                MemoryBudgetEvaluator.Evaluate(
                    OptimizationPolicy.AllocatedCriticalBytes,
                    0,
                    0
                ),
                Is.EqualTo(MemoryBudgetBand.Critical)
            );
        }

        [Test]
        public void MemoryEvaluator_RejectsNegativeSamplesAsUnknown()
        {
            Assert.That(
                MemoryBudgetEvaluator.Evaluate(-1, 0, 0),
                Is.EqualTo(MemoryBudgetBand.Unknown)
            );
        }

        [Test]
        public void SteadyGrowth_UsesInclusiveMaximum()
        {
            Assert.That(MemoryBudgetEvaluator.IsSteadyGrowthWithinBudget(
                OptimizationPolicy.MaximumSteadyGrowthBytes), Is.True);
            Assert.That(MemoryBudgetEvaluator.IsSteadyGrowthWithinBudget(
                OptimizationPolicy.MaximumSteadyGrowthBytes + 1L), Is.False);
            Assert.That(MemoryBudgetEvaluator.IsSteadyGrowthWithinBudget(-1L), Is.True);
        }

        [TestCase(OptimizationLoadKind.ScreenRoute, 250.0, OptimizationLoadVerdict.Pass)]
        [TestCase(OptimizationLoadKind.ScreenRoute, 250.001, OptimizationLoadVerdict.Fail)]
        [TestCase(OptimizationLoadKind.ResourceCleanup, 750.0, OptimizationLoadVerdict.Pass)]
        [TestCase(OptimizationLoadKind.Bootstrap, 5000.001, OptimizationLoadVerdict.Fail)]
        public void LoadingEvaluator_IsDeterministic(
            OptimizationLoadKind kind,
            double duration,
            OptimizationLoadVerdict expected
        )
        {
            Assert.That(LoadingBudgetEvaluator.Evaluate(kind, duration),
                Is.EqualTo(expected));
        }

        [Test]
        public void LoadingEvaluator_InvalidValuesAreIncomplete()
        {
            Assert.That(LoadingBudgetEvaluator.Evaluate(
                OptimizationLoadKind.ScreenRoute, -1.0),
                Is.EqualTo(OptimizationLoadVerdict.Incomplete));
            Assert.That(LoadingBudgetEvaluator.Evaluate(
                OptimizationLoadKind.ScreenRoute, double.NaN),
                Is.EqualTo(OptimizationLoadVerdict.Incomplete));
            Assert.That(LoadingBudgetEvaluator.Evaluate(
                (OptimizationLoadKind)999, 1.0),
                Is.EqualTo(OptimizationLoadVerdict.Incomplete));
        }

        [Test]
        public void Buffer_RejectsInvalidCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BoundedOptimizationEvidenceBuffer(0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BoundedOptimizationEvidenceBuffer(
                    OptimizationPolicy.MaximumEvidenceCapacity + 1));
        }

        [Test]
        public void Buffer_OverwritesOldestWhenFull()
        {
            var buffer = new BoundedOptimizationEvidenceBuffer(2);
            buffer.AddMemory(1, 2, 3);
            buffer.AddMemory(4, 5, 6);
            buffer.AddMemory(7, 8, 9);

            OptimizationEvidenceSample[] snapshot = buffer.Snapshot();
            Assert.That(snapshot, Has.Length.EqualTo(2));
            Assert.That(snapshot[0].Sequence, Is.EqualTo(2));
            Assert.That(snapshot[1].Sequence, Is.EqualTo(3));
            Assert.That(snapshot[0].AllocatedBytes, Is.EqualTo(4));
            Assert.That(snapshot[1].AllocatedBytes, Is.EqualTo(7));
        }

        [Test]
        public void CanonicalJson_IsStableAndMinimized()
        {
            var first = new BoundedOptimizationEvidenceBuffer(2);
            var second = new BoundedOptimizationEvidenceBuffer(2);
            first.AddMemory(10, 20, 30);
            first.AddLoad(OptimizationLoadKind.ScreenRoute, 4.25);
            second.AddMemory(10, 20, 30);
            second.AddLoad(OptimizationLoadKind.ScreenRoute, 4.25);

            string a = first.ToDeterministicJson();
            string b = second.ToDeterministicJson();
            Assert.That(a, Is.EqualTo(b));
            Assert.That(a, Does.Contain("\"schemaVersion\":1"));
            Assert.That(a, Does.Not.Contain("player"));
            Assert.That(a, Does.Not.Contain("device"));
            Assert.That(a, Does.Not.Contain("timestamp"));
            Assert.That(a, Does.Not.Contain("message"));
            Assert.That(a, Does.Not.Contain("stack"));
            Assert.That(a, Does.Not.Contain("payload"));
        }

        [Test]
        public void LoadingState_ProgressAndPhaseAreMonotonic()
        {
            var state = new LoadingStateTracker();
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.Bootstrap, 0.15), Is.True);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.LocalState, 0.55), Is.True);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.Bootstrap, 0.75), Is.False);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.UiConstruction, 0.40), Is.False);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.Ready, 0.99), Is.False);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.UiConstruction, 0.85), Is.True);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.Ready, 1.0), Is.True);
            Assert.That(state.Phase, Is.EqualTo(RuntimeLoadingPhase.Ready));
            Assert.That(state.Progress01, Is.EqualTo(1.0));
        }

        [Test]
        public void LoadingState_FailureIsTerminal()
        {
            var state = new LoadingStateTracker();
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.Bootstrap, 0.15), Is.True);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.Failed, 0.15), Is.True);
            Assert.That(state.TryAdvance(RuntimeLoadingPhase.Ready, 1.0), Is.False);
            Assert.That(state.Phase, Is.EqualTo(RuntimeLoadingPhase.Failed));
        }
    }
}
