using NUnit.Framework;
using LegendsOfRus.LiveOpsExecution;

namespace LegendsOfRus.Tests.LiveOpsExecution
{
    public sealed class ContentRotationEvaluatorEditModeTests
    {
        static ContentRotationEvidence Good(){return new ContentRotationEvidence{EvidenceRetained=true,EvidenceReviewed=true,RotationPlanId="LOR-LO-ROT-PLAN-1",Entries=new[]{new ContentRotationEntryEvidence{RotationId="LOR-LO-ROT-1",ContentId="A",ContentVersion="1",StartMinute=0,EndMinute=60,RequiredCooldownMinutes=30},new ContentRotationEntryEvidence{RotationId="LOR-LO-ROT-2",ContentId="B",ContentVersion="1",StartMinute=90,EndMinute=150,RequiredCooldownMinutes=0}}};}
        [Test] public void Good_IsSafe(){Assert.IsTrue(ContentRotationEvaluator.Evaluate(Good()).RotationSafe);}
        [Test] public void Good_IsStructural(){Assert.IsTrue(ContentRotationEvaluator.Evaluate(Good()).StructurallyValid);}
        [Test] public void Good_Count(){Assert.AreEqual(2,ContentRotationEvaluator.Evaluate(Good()).EntryCount);}
        [Test] public void Overlap_Blocks(){var e=Good();e.Entries[1].StartMinute=50;Assert.Contains(LiveOpsExecutionBlockerCodes.RotationOverlap,ContentRotationEvaluator.Evaluate(e).Blockers);}
        [Test] public void Cooldown_Blocks(){var e=Good();e.Entries[1].StartMinute=70;Assert.Contains(LiveOpsExecutionBlockerCodes.RotationCooldownInsufficient,ContentRotationEvaluator.Evaluate(e).Blockers);}
        [Test] public void MissingVersion_Blocks(){var e=Good();e.Entries[0].ContentVersion="";Assert.Contains(LiveOpsExecutionBlockerCodes.RotationInvalid,ContentRotationEvaluator.Evaluate(e).Blockers);}
        [Test] public void InvalidWindow_Blocks(){var e=Good();e.Entries[0].EndMinute=0;Assert.Contains(LiveOpsExecutionBlockerCodes.RotationInvalid,ContentRotationEvaluator.Evaluate(e).Blockers);}
        [Test] public void NegativeCooldown_Blocks(){var e=Good();e.Entries[0].RequiredCooldownMinutes=-1;Assert.Contains(LiveOpsExecutionBlockerCodes.RotationInvalid,ContentRotationEvaluator.Evaluate(e).Blockers);}
        [Test] public void BadId_Blocks(){var e=Good();e.Entries[0].RotationId="BAD";Assert.Contains(LiveOpsExecutionBlockerCodes.RotationInvalid,ContentRotationEvaluator.Evaluate(e).Blockers);}
        [Test] public void MissingEvidence_Blocks(){var e=Good();e.EvidenceRetained=false;Assert.IsFalse(ContentRotationEvaluator.Evaluate(e).StructurallyValid);}
        [Test] public void NotReviewed_Blocks(){var e=Good();e.EvidenceReviewed=false;Assert.IsFalse(ContentRotationEvaluator.Evaluate(e).StructurallyValid);}
        [Test] public void NullEntry_Blocks(){var e=Good();e.Entries[0]=null;Assert.Contains(LiveOpsExecutionBlockerCodes.RotationInvalid,ContentRotationEvaluator.Evaluate(e).Blockers);}
    }
}
