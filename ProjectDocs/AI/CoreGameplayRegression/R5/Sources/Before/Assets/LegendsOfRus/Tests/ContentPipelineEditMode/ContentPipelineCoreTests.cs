using NUnit.Framework;
using LegendsOfRus.ContentPipeline;

namespace LegendsOfRus.Tests.ContentPipeline
{
    public sealed partial class ContentPipelineCoreTests
    {
        [Test] public void Policy_ExpectedChapterCount_Is20(){Assert.That(ChapterPipelinePolicy.ExpectedChapterCount,Is.EqualTo(20));}
        [Test] public void Policy_ChapterStableId_IsZeroPadded(){Assert.That(ChapterPipelinePolicy.ChapterStableId(8),Is.EqualTo("chapter08"));}
        [Test] public void Policy_AddressPrefix_IsStable(){Assert.That(ChapterPipelinePolicy.ChapterAddressPrefix(8),Is.EqualTo("lor.chapter.08."));}
        [Test] public void Policy_ChapterFolder_IsStable(){Assert.That(ChapterPipelinePolicy.ChapterFolder(20),Is.EqualTo("Assets/LegendsOfRus/Content/Chapters/CH20"));}
        [Test] public void Policy_Sha256_AcceptsLowerHex(){Assert.That(ChapterPipelinePolicy.IsSha256(new string('a',64)),Is.True);}
        [Test] public void Policy_Sha256_AcceptsUpperHex(){Assert.That(ChapterPipelinePolicy.IsSha256(new string('A',64)),Is.True);}
        [Test] public void Policy_Sha256_RejectsWrongLength(){Assert.That(ChapterPipelinePolicy.IsSha256("abc"),Is.False);}
        [Test] public void Policy_Sha256_RejectsNonHex(){Assert.That(ChapterPipelinePolicy.IsSha256(new string('z',64)),Is.False);}

        [Test] public void Manifest_Healthy_IsValid(){Assert.That(ChapterPipelineValidator.ValidateManifest(Manifest()).IsValid,Is.True);}
        [Test] public void Manifest_Null_IsRejected(){Assert.That(Has(ChapterPipelineValidator.ValidateManifest(null).Blockers,ChapterPipelineBlockerCodes.ManifestMissing),Is.True);}
        [Test] public void Manifest_BadSchema_IsRejected(){var m=Manifest();m.SchemaVersion=9;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ManifestSchemaInvalid),Is.True);}
        [Test] public void Manifest_MissingId_IsRejected(){var m=Manifest();m.ManifestId="";Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ManifestIdMissing),Is.True);}
        [Test] public void Manifest_WrongExpectedCount_IsRejected(){var m=Manifest();m.ExpectedChapterCount=19;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ChapterCountInvalid),Is.True);}
        [Test] public void Manifest_WrongArrayCount_IsRejected(){var m=Manifest();m.Chapters=new ChapterPipelineEntry[19];Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ChapterCountInvalid),Is.True);}
        [Test] public void Manifest_NullEntry_IsRejected(){var m=Manifest();m.Chapters[5]=null;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ChapterEntryMissing),Is.True);}
        [Test] public void Manifest_OutOfRangeChapter_IsRejected(){var m=Manifest();m.Chapters[0].ChapterId=0;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ChapterIdInvalid),Is.True);}
        [Test] public void Manifest_DuplicateChapterId_IsRejected(){var m=Manifest();m.Chapters[1].ChapterId=1;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ChapterIdDuplicate),Is.True);}
        [Test] public void Manifest_NonContiguousOrder_IsRejected(){var m=Manifest();m.Chapters[1].ChapterId=3;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.ChapterOrderInvalid),Is.True);}
        [Test] public void Manifest_BadStableId_IsRejected(){var m=Manifest();m.Chapters[1].StableId="chapter2";Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.StableIdInvalid),Is.True);}
        [Test] public void Manifest_DuplicateStableId_IsRejected(){var m=Manifest();m.Chapters[1].StableId=m.Chapters[0].StableId;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.StableIdDuplicate),Is.True);}
        [Test] public void Manifest_MissingDisplayNameKey_IsRejected(){var m=Manifest();m.Chapters[1].DisplayNameKey="";Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.DisplayNameKeyMissing),Is.True);}
        [Test] public void Manifest_MissingAddress_IsRejected(){var m=Manifest();m.Chapters[1].StoryAddress="";Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.AddressMissing),Is.True);}
        [Test] public void Manifest_WrongAddressPrefix_IsRejected(){var m=Manifest();m.Chapters[1].StoryAddress="lor.chapter.03.story";Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.AddressPrefixInvalid),Is.True);}
        [Test] public void Manifest_DuplicateAddress_IsRejected(){var m=Manifest();m.Chapters[1].StoryAddress=m.Chapters[0].StoryAddress;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.AddressDuplicate),Is.True);}
        [Test] public void Manifest_BadCanonicalFolder_IsRejected(){var m=Manifest();m.Chapters[1].CanonicalFolder="bad";Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.CanonicalFolderInvalid),Is.True);}
        [Test] public void Manifest_OnlyChapter1MayUseLegacySource(){var m=Manifest();m.Chapters[1].UsesLegacyChapter01Source=true;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.LegacySourceBoundaryInvalid),Is.True);}
        [Test] public void Manifest_Chapter1MustUseLegacySourceInStep1(){var m=Manifest();m.Chapters[0].UsesLegacyChapter01Source=false;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.LegacySourceBoundaryInvalid),Is.True);}
        [Test] public void Manifest_NonProductionEntry_DoesNotRequireOwnershipApproval(){var m=Manifest();Assert.That(ChapterPipelineValidator.ValidateManifest(m).IsValid,Is.True);}
        [Test] public void Manifest_ProductionEntry_RequiresOwnershipApproval(){var m=Manifest();m.Chapters[1].ProductionReady=true;Assert.That(Has(ChapterPipelineValidator.ValidateManifest(m).Blockers,ChapterPipelineBlockerCodes.OwnershipIncomplete),Is.True);}
        [Test] public void Manifest_ProductionEntry_WithOwnership_IsValid(){var m=Manifest();var e=m.Chapters[1];e.ProductionReady=true;e.DataOwnershipReviewed=e.ArtOwnershipReviewed=e.LocalizationOwnershipReviewed=e.QaOwnershipReviewed=true;Assert.That(ChapterPipelineValidator.ValidateManifest(m).IsValid,Is.True);}

        [Test] public void Template_Healthy_IsValid(){Assert.That(ChapterPipelineValidator.ValidateTemplate(Template()).IsValid,Is.True);}
        [Test] public void Template_Null_IsRejected(){Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(null).Blockers,ChapterPipelineBlockerCodes.TemplateMissing),Is.True);}
        [Test] public void Template_BadSchema_IsRejected(){var t=Template();t.SchemaVersion=2;Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateSchemaInvalid),Is.True);}
        [Test] public void Template_BadChapterId_IsRejected(){var t=Template();t.ChapterId=21;Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateIdentityInvalid),Is.True);}
        [Test] public void Template_BadStableId_IsRejected(){var t=Template();t.StableId="bad";Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateIdentityInvalid),Is.True);}
        [Test] public void Template_MissingDisplayNameKey_IsRejected(){var t=Template();t.DisplayNameKey="";Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateIdentityInvalid),Is.True);}
        [Test] public void Template_ZeroLevels_IsRejected(){var t=Template();t.LevelCount=0;Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateLevelCountInvalid),Is.True);}
        [Test] public void Template_TooManyLevels_IsRejected(){var t=Template();t.LevelCount=101;Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateLevelCountInvalid),Is.True);}
        [Test] public void Template_FirstPlayableBeyondCount_IsRejected(){var t=Template();t.FirstPlayableLevel=11;Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateFinaleInvalid),Is.True);}
        [Test] public void Template_FinaleBeyondCount_IsRejected(){var t=Template();t.FinaleLevelId=11;Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateFinaleInvalid),Is.True);}
        [Test] public void Template_MilestoneOutOfRange_IsRejected(){var t=Template();t.StoryMilestoneLevels=new[]{1,11};Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateMilestoneInvalid),Is.True);}
        [Test] public void Template_DuplicateMilestone_IsRejected(){var t=Template();t.StoryMilestoneLevels=new[]{1,3,3};Assert.That(Has(ChapterPipelineValidator.ValidateTemplate(t).Blockers,ChapterPipelineBlockerCodes.TemplateMilestoneInvalid),Is.True);}
        [Test] public void Template_EmptyMilestones_AreAllowed(){var t=Template();t.StoryMilestoneLevels=new int[0];Assert.That(ChapterPipelineValidator.ValidateTemplate(t).IsValid,Is.True);}

        [Test] public void Transition_Healthy_IsValid(){Assert.That(ContentPipelineTransitionEvaluator.ValidateEvidence(Evidence()).IsValid,Is.True);}
        [Test] public void Transition_Null_IsRejected(){Assert.That(Has(ContentPipelineTransitionEvaluator.ValidateEvidence(null).Blockers,ChapterPipelineBlockerCodes.EvidenceMissing),Is.True);}
        [Test] public void Transition_NotReviewed_IsRejected(){var e=Evidence();e.EvidenceReviewed=false;Assert.That(Has(ContentPipelineTransitionEvaluator.ValidateEvidence(e).Blockers,ChapterPipelineBlockerCodes.EvidenceNotReviewed),Is.True);}
        [Test] public void Transition_BadHash_IsRejected(){var e=Evidence();e.SourceTreeSha256="bad";Assert.That(Has(ContentPipelineTransitionEvaluator.ValidateEvidence(e).Blockers,ChapterPipelineBlockerCodes.SourceHashInvalid),Is.True);}
        [Test] public void Transition_LegacyValidationRequired(){var e=Evidence();e.LegacyChapter01StoryValidated=false;Assert.That(Has(ContentPipelineTransitionEvaluator.ValidateEvidence(e).Blockers,ChapterPipelineBlockerCodes.LegacyValidationMissing),Is.True);}
        [Test] public void Transition_AddressablesReviewRequired(){var e=Evidence();e.AddressablesNamespaceReviewed=false;Assert.That(Has(ContentPipelineTransitionEvaluator.ValidateEvidence(e).Blockers,ChapterPipelineBlockerCodes.AddressablesReviewMissing),Is.True);}
        [Test] public void Transition_LockedArtBoundaryRequired(){var e=Evidence();e.LockedArtNonDestructiveBoundaryReviewed=false;Assert.That(Has(ContentPipelineTransitionEvaluator.ValidateEvidence(e).Blockers,ChapterPipelineBlockerCodes.LockedArtBoundaryMissing),Is.True);}
        [Test] public void Transition_CriticalBlockersReject(){var e=Evidence();e.UnresolvedCriticalBlockers=1;Assert.That(Has(ContentPipelineTransitionEvaluator.ValidateEvidence(e).Blockers,ChapterPipelineBlockerCodes.CriticalBlockersOpen),Is.True);}

        [Test] public void Review_HealthyEvidence_IsStructurallyReady(){var r=ContentPipelineTransitionEvaluator.Evaluate(Manifest(),Template(),Evidence(),false,false,false,false,false);Assert.That(r.ReadyForHumanReview,Is.True);}
        [Test] public void Review_ActionGates_BlockMigration(){var r=ContentPipelineTransitionEvaluator.Evaluate(Manifest(),Template(),Evidence(),false,false,false,false,false);Assert.That(r.MayBeginChapterMigration,Is.False);}
        [Test] public void Review_AllActionGates_CanAuthorizeMigration(){var r=ContentPipelineTransitionEvaluator.Evaluate(Manifest(),Template(),Evidence(),true,true,true,true,true);Assert.That(r.MayBeginChapterMigration,Is.True);}
    }

    static class ContentPipelineFixtures
    {
        public static ChapterPipelineManifest Manifest(){var m=new ChapterPipelineManifest{ManifestId="LOR-V120-MANIFEST",ExpectedChapterCount=20,Chapters=new ChapterPipelineEntry[20]};for(int i=1;i<=20;i++){string p=i.ToString("00");m.Chapters[i-1]=new ChapterPipelineEntry{ChapterId=i,StableId="chapter"+p,DisplayNameKey="chapter."+p+".title",LevelsAddress="lor.chapter."+p+".levels",StoryAddress="lor.chapter."+p+".story",LocalizationAddress="lor.chapter."+p+".localization.ru",EnvironmentAddress="lor.chapter."+p+".environment",MapArtAddress="lor.chapter."+p+".map_art",CanonicalFolder=ChapterPipelinePolicy.ChapterFolder(i),UsesLegacyChapter01Source=i==1};}return m;}
        public static ChapterTemplateDefinition Template(){return new ChapterTemplateDefinition{ChapterId=2,StableId="chapter02",DisplayNameKey="chapter.02.title",LevelCount=10,FirstPlayableLevel=1,FinaleLevelId=10,StoryMilestoneLevels=new[]{1,3,6,10},LevelsAddress="lor.chapter.02.levels",StoryAddress="lor.chapter.02.story",LocalizationAddress="lor.chapter.02.localization.ru",EnvironmentAddress="lor.chapter.02.environment",MapArtAddress="lor.chapter.02.map_art"};}
        public static ContentPipelineTransitionEvidence Evidence(){return new ContentPipelineTransitionEvidence{EvidenceRetained=true,EvidenceReviewed=true,TransitionId="LOR-V120-STEP01",SourceTreeSha256=new string('a',64),LegacyChapter01LevelsValidated=true,LegacyChapter01StoryValidated=true,LegacyRussianLocalizationValidated=true,ManifestContractReviewed=true,TemplateContractReviewed=true,StableIdConventionReviewed=true,AddressablesNamespaceReviewed=true,ContentOwnershipReviewed=true,LocalizationKeyConventionReviewed=true,QaAutomationReviewed=true,MigrationPlanReviewed=true,LockedArtNonDestructiveBoundaryReviewed=true};}
        public static bool Has(string[] a,string v){if(a==null)return false;for(int i=0;i<a.Length;i++)if(a[i]==v)return true;return false;}
    }

    public sealed partial class ContentPipelineCoreTests
    {
        static ChapterPipelineManifest Manifest(){return ContentPipelineFixtures.Manifest();}
        static ChapterTemplateDefinition Template(){return ContentPipelineFixtures.Template();}
        static ContentPipelineTransitionEvidence Evidence(){return ContentPipelineFixtures.Evidence();}
        static bool Has(string[] a,string v){return ContentPipelineFixtures.Has(a,v);}
    }
}
