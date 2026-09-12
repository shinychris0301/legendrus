using LegendsOfRus.IncidentResponse;
using NUnit.Framework;

namespace LegendsOfRus.Tests.IncidentResponse
{
    public sealed class IncidentResponseValidationEditModeTests
    {
        [Test] public void Sha256_AcceptsLowercaseHex() => Assert.That(IncidentResponseValidation.IsSha256(new string('a', 64)), Is.True);
        [Test] public void Sha256_AcceptsUppercaseHex() => Assert.That(IncidentResponseValidation.IsSha256(new string('A', 64)), Is.True);
        [Test] public void Sha256_RejectsShort() => Assert.That(IncidentResponseValidation.IsSha256("abc"), Is.False);
        [Test] public void Sha256_RejectsNonHex() => Assert.That(IncidentResponseValidation.IsSha256(new string('z', 64)), Is.False);
        [Test] public void IncidentId_AcceptsCanonical() => Assert.That(IncidentResponseValidation.IsIncidentId("LOR-INC-0001"), Is.True);
        [Test] public void IncidentId_RejectsZero() => Assert.That(IncidentResponseValidation.IsIncidentId("LOR-INC-0000"), Is.False);
        [Test] public void IncidentId_RejectsWrongDigits() => Assert.That(IncidentResponseValidation.IsIncidentId("LOR-INC-001"), Is.False);
        [Test] public void HotfixId_AcceptsCanonical() => Assert.That(IncidentResponseValidation.IsHotfixId("LOR-HF-0001"), Is.True);
        [Test] public void HotfixCandidate_AcceptsCanonical() => Assert.That(IncidentResponseValidation.IsHotfixCandidateId("v1.19-hotfix-rc01"), Is.True);
        [Test] public void HotfixCandidate_RejectsZero() => Assert.That(IncidentResponseValidation.IsHotfixCandidateId("v1.19-hotfix-rc00"), Is.False);
        [Test] public void BaseCandidate_AcceptsVersionStyle() => Assert.That(IncidentResponseValidation.IsBaseCandidateId("v1.18-step05-rc01"), Is.True);
        [Test] public void BaseCandidate_RejectsSpaces() => Assert.That(IncidentResponseValidation.IsBaseCandidateId("bad candidate"), Is.False);
        [Test] public void RolloutPercent_AcceptsOne() => Assert.That(IncidentResponseValidation.IsRolloutPercent(1), Is.True);
        [Test] public void RolloutPercent_AcceptsHundred() => Assert.That(IncidentResponseValidation.IsRolloutPercent(100), Is.True);
        [Test] public void RolloutPercent_RejectsZero() => Assert.That(IncidentResponseValidation.IsRolloutPercent(0), Is.False);
        [Test] public void RolloutPercent_RejectsOverHundred() => Assert.That(IncidentResponseValidation.IsRolloutPercent(101), Is.False);
    }
}
