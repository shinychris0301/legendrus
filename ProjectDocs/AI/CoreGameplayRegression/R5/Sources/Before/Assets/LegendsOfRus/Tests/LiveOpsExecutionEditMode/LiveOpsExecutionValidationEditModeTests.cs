using NUnit.Framework;
using LegendsOfRus.LiveOpsExecution;

namespace LegendsOfRus.Tests.LiveOpsExecution
{
    public sealed class LiveOpsExecutionValidationEditModeTests
    {
        [Test] public void Rate_Quarter(){Assert.AreEqual(2500,LiveOpsExecutionValidation.RateBasisPoints(1,4));}
        [Test] public void Rate_ZeroTotal(){Assert.AreEqual(0,LiveOpsExecutionValidation.RateBasisPoints(1,0));}
        [Test] public void Rate_ClampsFull(){Assert.AreEqual(10000,LiveOpsExecutionValidation.RateBasisPoints(9,3));}
        [Test] public void Prefix_Valid(){Assert.IsTrue(LiveOpsExecutionValidation.HasPrefix("LOR-LO-EXEC-1",LiveOpsExecutionRequirements.ExecutionPrefix));}
        [Test] public void Prefix_Empty(){Assert.IsFalse(LiveOpsExecutionValidation.HasPrefix("",LiveOpsExecutionRequirements.ExecutionPrefix));}
        [Test] public void MissingExecution_Blocks(){Assert.Contains(LiveOpsExecutionBlockerCodes.ExecutionMissing,LiveOpsExecutionValidation.ValidateExecution(null));}
        [Test] public void MissingRotation_Blocks(){Assert.Contains(LiveOpsExecutionBlockerCodes.RotationMissing,LiveOpsExecutionValidation.ValidateRotation(null));}
        [Test] public void MissingObservation_Blocks(){Assert.Contains(LiveOpsExecutionBlockerCodes.ObservationMissing,LiveOpsExecutionValidation.ValidateObservation(null));}
        [Test] public void Normalize_Negative(){CollectionAssert.AreEqual(new[]{0,3},LiveOpsExecutionValidation.NormalizeCounts(-2,3));}
        [Test] public void Requirements_BasisPoints(){Assert.AreEqual(10000,LiveOpsExecutionRequirements.BasisPointsTotal);}
        [Test] public void Requirements_BufferBounds(){Assert.Greater(LiveOpsExecutionRequirements.MaxObservationCapacity,LiveOpsExecutionRequirements.DefaultObservationCapacity);}
        [Test] public void Requirements_RotationLimit(){Assert.AreEqual(32,LiveOpsExecutionRequirements.MaxRotationEntries);}
    }
}
