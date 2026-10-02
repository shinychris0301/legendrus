#if UNITY_INCLUDE_TESTS
using LegendsOfRus.ContentPipeline;
using NUnit.Framework;

namespace LegendsOfRus.Tests.ContentPipeline
{
    public sealed class ChapterMigrationDryRunEditModeTests
    {
        static ChapterMigrationFileEvidence File(string name)
        {
            string h = new string('a', 64);
            return new ChapterMigrationFileEvidence { LogicalName=name, SourcePath="source/"+name, StagedPath="stage/"+name, SourceSha256=h, StagedSha256=h, ByteCount=10, ExactByteMatch=true };
        }
        static CanonicalChapterPackageManifest Package()
        {
            return new CanonicalChapterPackageManifest {
                PackageId=ChapterMigrationPolicy.PackageId, ChapterId=1, StableId="chapter01", SourceMode="legacy-byte-copy-dry-run",
                CanonicalDefinitionPath=ChapterMigrationPolicy.StagedDefinition, CanonicalDefinitionSha256=new string('b',64),
                CopiedPayloads=new[]{File("levels"),File("story"),File("localization_ru")}, LevelCount=10, StoryMilestoneCount=7, LocalizationEntryCount=49,
                RuntimeStillUsesLegacyResources=true
            };
        }
        static ChapterMigrationDryRunEvidence Evidence()
        {
            return new ChapterMigrationDryRunEvidence { DryRunId=ChapterMigrationPolicy.DryRunId, EvidenceRetained=true, EvidenceReviewed=false,
                PackageManifestSha256=new string('c',64), Package=Package(), LegacyRuntimeStillAuthoritative=true };
        }
        static bool Has(string[] a,string v){for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}

        [Test] public void HealthyPackagePasses(){Assert.IsTrue(ChapterMigrationDryRunValidator.ValidatePackage(Package()).IsStructurallyValid);}
        [Test] public void HealthyEvidencePassesWithoutHumanReview(){Assert.IsTrue(ChapterMigrationDryRunValidator.ValidateEvidence(Evidence()).IsStructurallyValid);}
        [Test] public void MissingPackageFails(){Assert.IsFalse(ChapterMigrationDryRunValidator.ValidatePackage(null).IsStructurallyValid);}
        [Test] public void BadPackageSchemaFails(){var p=Package();p.SchemaVersion=99;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PackageSchemaInvalid));}
        [Test] public void BadPackageIdFails(){var p=Package();p.PackageId="x";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PackageIdentityInvalid));}
        [Test] public void BadStableIdFails(){var p=Package();p.StableId="chapter02";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PackageIdentityInvalid));}
        [Test] public void WrongSourceModeFails(){var p=Package();p.SourceMode="rewrite";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PackageIdentityInvalid));}
        [Test] public void BadDefinitionHashFails(){var p=Package();p.CanonicalDefinitionSha256="bad";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.DefinitionHashInvalid));}
        [Test] public void WrongPayloadCountFails(){var p=Package();p.CopiedPayloads=new[]{File("levels")};Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadCountInvalid));}
        [Test] public void MissingPayloadFails(){var p=Package();p.CopiedPayloads[1]=null;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadMissing));}
        [Test] public void EmptyPayloadPathFails(){var p=Package();p.CopiedPayloads[0].SourcePath="";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadPathInvalid));}
        [Test] public void BadSourceHashFails(){var p=Package();p.CopiedPayloads[0].SourceSha256="bad";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadHashInvalid));}
        [Test] public void BadStagedHashFails(){var p=Package();p.CopiedPayloads[0].StagedSha256="bad";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadHashInvalid));}
        [Test] public void HashMismatchFails(){var p=Package();p.CopiedPayloads[0].StagedSha256=new string('d',64);Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadHashMismatch));}
        [Test] public void ExactMatchFalseFails(){var p=Package();p.CopiedPayloads[0].ExactByteMatch=false;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadHashMismatch));}
        [Test] public void ZeroBytesFails(){var p=Package();p.CopiedPayloads[0].ByteCount=0;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.PayloadByteCountInvalid));}
        [Test] public void WrongLevelCountFails(){var p=Package();p.LevelCount=9;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.LevelCountInvalid));}
        [Test] public void WrongMilestoneCountFails(){var p=Package();p.StoryMilestoneCount=6;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.StoryMilestoneCountInvalid));}
        [Test] public void ZeroLocalizationFails(){var p=Package();p.LocalizationEntryCount=0;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.LocalizationEntryCountInvalid));}
        [Test] public void LegacyAuthorityRequired(){var p=Package();p.RuntimeStillUsesLegacyResources=false;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.LegacyAuthorityLost));}
        [Test] public void RuntimeCutoverRejected(){var p=Package();p.RuntimeCutoverPerformed=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.RuntimeCutoverOccurred));}
        [Test] public void AddressablesMutationRejected(){var p=Package();p.AddressablesMutated=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.AddressablesMutated));}
        [Test] public void ArtTouchRejected(){var p=Package();p.LockedArtTouched=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.LockedArtTouched));}
        [Test] public void BalanceChangeRejected(){var p=Package();p.LevelBalanceChanged=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.LevelBalanceChanged));}
        [Test] public void RewardChangeRejected(){var p=Package();p.RewardsChanged=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidatePackage(p).Blockers,ChapterMigrationBlockerCodes.RewardsChanged));}
        [Test] public void BadEvidenceSchemaFails(){var e=Evidence();e.SchemaVersion=8;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.SchemaInvalid));}
        [Test] public void BadDryRunIdFails(){var e=Evidence();e.DryRunId="bad";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.DryRunIdInvalid));}
        [Test] public void RetainedEvidenceRequired(){var e=Evidence();e.EvidenceRetained=false;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.EvidenceMissing));}
        [Test] public void ManifestHashRequired(){var e=Evidence();e.PackageManifestSha256="bad";Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.PackageManifestHashInvalid));}
        [Test] public void EvidenceLegacyAuthorityRequired(){var e=Evidence();e.LegacyRuntimeStillAuthoritative=false;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.LegacyAuthorityLost));}
        [Test] public void EvidenceCutoverRejected(){var e=Evidence();e.RuntimeCutoverPerformed=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.RuntimeCutoverOccurred));}
        [Test] public void EvidenceAddressablesRejected(){var e=Evidence();e.AddressablesMutated=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.AddressablesMutated));}
        [Test] public void EvidenceArtTouchRejected(){var e=Evidence();e.LockedArtTouched=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.LockedArtTouched));}
        [Test] public void AutoArtRejected(){var e=Evidence();e.AutomaticArtGenerationUsed=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.AutomaticArtGenerationUsed));}
        [Test] public void LocalizationRewriteRejected(){var e=Evidence();e.LocalizationRewritten=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.LocalizationRewritten));}
        [Test] public void EvidenceBalanceChangeRejected(){var e=Evidence();e.LevelBalanceChanged=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.LevelBalanceChanged));}
        [Test] public void EvidenceRewardChangeRejected(){var e=Evidence();e.RewardsChanged=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.RewardsChanged));}
        [Test] public void ProgressionChangeRejected(){var e=Evidence();e.ProgressionChanged=true;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.ProgressionChanged));}
        [Test] public void CriticalBlockersRejected(){var e=Evidence();e.UnresolvedCriticalBlockers=1;Assert.IsTrue(Has(ChapterMigrationDryRunValidator.ValidateEvidence(e).Blockers,ChapterMigrationBlockerCodes.CriticalBlockersOpen));}
        [Test] public void HumanReviewFlagDoesNotChangeStructuralEquivalence(){var e=Evidence();e.EvidenceReviewed=true;Assert.IsTrue(ChapterMigrationDryRunValidator.ValidateEvidence(e).IsStructurallyValid);}
        [Test] public void Sha256LowercaseAccepted(){Assert.IsTrue(ChapterMigrationPolicy.IsSha256(new string('a',64)));}
        [Test] public void Sha256UppercaseAccepted(){Assert.IsTrue(ChapterMigrationPolicy.IsSha256(new string('A',64)));}
        [Test] public void Sha256WrongLengthRejected(){Assert.IsFalse(ChapterMigrationPolicy.IsSha256("abc"));}
        [Test] public void Sha256NonHexRejected(){Assert.IsFalse(ChapterMigrationPolicy.IsSha256(new string('z',64)));}
        [Test] public void StagingRootIsOutsideAssets(){Assert.IsTrue(ChapterMigrationPolicy.StagingRoot.StartsWith("ProjectDocs/"));}
        [Test] public void StagedLevelsStayOutsideResources(){Assert.IsFalse(ChapterMigrationPolicy.StagedLevels.Contains("Resources/"));}
        [Test] public void ExpectedPayloadCountIsThree(){Assert.AreEqual(3,ChapterMigrationPolicy.ExpectedCopiedPayloadCount);}
        [Test] public void ExpectedLevelCountIsTen(){Assert.AreEqual(10,ChapterMigrationPolicy.ExpectedLevelCount);}
        [Test] public void ExpectedMilestonesIsSeven(){Assert.AreEqual(7,ChapterMigrationPolicy.ExpectedStoryMilestoneCount);}
        [Test] public void DryRunIdStable(){Assert.AreEqual("LOR-V120-STEP02-CH01-DRY-RUN",ChapterMigrationPolicy.DryRunId);}
    }
}
#endif
