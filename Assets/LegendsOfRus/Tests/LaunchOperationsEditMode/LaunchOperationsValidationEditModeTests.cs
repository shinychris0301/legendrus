using LegendsOfRus.LaunchOperations;
using NUnit.Framework;

namespace LegendsOfRus.Tests.LaunchOperations
{
    public sealed class LaunchOperationsValidationEditModeTests
    {
        const string HashA = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

        [Test] public void Sha256_LowercaseAccepted() => Assert.That(LaunchOperationsValidation.IsSha256(HashA), Is.True);
        [Test] public void Sha256_UppercaseAccepted() => Assert.That(LaunchOperationsValidation.IsSha256(HashA.ToUpperInvariant()), Is.True);
        [Test] public void Sha256_ShortRejected() => Assert.That(LaunchOperationsValidation.IsSha256("abc"), Is.False);
        [Test] public void Sha256_NonHexRejected() => Assert.That(LaunchOperationsValidation.IsSha256(new string('z', 64)), Is.False);
        [Test] public void Candidate_Rc01Accepted() => Assert.That(LaunchOperationsValidation.IsCandidateId("v1.18-step05-rc01"), Is.True);
        [Test] public void Candidate_Rc99Accepted() => Assert.That(LaunchOperationsValidation.IsCandidateId("v1.18-step05-rc99"), Is.True);
        [Test] public void Candidate_Rc00Rejected() => Assert.That(LaunchOperationsValidation.IsCandidateId("v1.18-step05-rc00"), Is.False);
        [Test] public void Candidate_WrongPrefixRejected() => Assert.That(LaunchOperationsValidation.IsCandidateId("v1.19-step01-rc01"), Is.False);
        [Test] public void Percent_BoundsAccepted() => Assert.That(LaunchOperationsValidation.IsPercent(0) && LaunchOperationsValidation.IsPercent(100), Is.True);
        [Test] public void Percent_OutsideBoundsRejected() => Assert.That(LaunchOperationsValidation.IsPercent(-0.01) || LaunchOperationsValidation.IsPercent(100.01), Is.False);
    }
}
