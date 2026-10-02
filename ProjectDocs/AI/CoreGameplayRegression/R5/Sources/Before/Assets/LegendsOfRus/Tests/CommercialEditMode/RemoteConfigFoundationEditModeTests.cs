using System;
using System.Collections.Generic;
using System.Text;
using LegendsOfRus.CommercialCore.RemoteConfig;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.Commercial
{
    public sealed class RemoteConfigFoundationEditModeTests
    {
        const string ExpectedContentSha256 =
            "3f4229158b3f5516a536ccf31954f417f8be45a64edd2e4cc33946c2d42e07eb";

        [Test]
        public void BundledSnapshot_LoadsDetachedWithExactReviewedBindings()
        {
            string source = BundledSource();

            RemoteConfigLoadResult result =
                new DeterministicLocalRemoteConfigProvider(source).LoadSnapshot();

            Assert.That(result.Succeeded, Is.True, result.Message);
            Assert.That(result.Status, Is.EqualTo(RemoteConfigLoadStatus.LoadedDetachedLocal));
            Assert.That(result.Code, Is.EqualTo(RemoteConfigFailureCodes.None));
            Assert.That(result.RuntimeApplied, Is.False);
            Assert.That(result.SourceSha256, Is.EqualTo(
                DeterministicLocalRemoteConfigProvider.ExpectedBundledSourceSha256
            ));
            Assert.That(result.ContentSha256, Is.EqualTo(ExpectedContentSha256));
            Assert.That(result.Snapshot.IsFallback, Is.False);
            Assert.That(result.Snapshot.IsAuthoritative, Is.False);
            Assert.That(result.Snapshot.ProposalOnly, Is.True);
            Assert.That(result.Snapshot.SchemaVersion, Is.EqualTo(1));
            Assert.That(result.Snapshot.Revision, Is.EqualTo(1));
            Assert.That(result.Snapshot.Environment, Is.EqualTo("development"));
            Assert.That(result.Snapshot.EconomyCatalogId, Is.EqualTo(
                RemoteConfigValidator.RequiredEconomyCatalogId
            ));
            Assert.That(result.Snapshot.EconomyCatalogSourceSha256, Is.EqualTo(
                RemoteConfigValidator.RequiredEconomyCatalogSourceSha256
            ));
        }

        [Test]
        public void BundledSnapshot_HasExactlyTenConservativeBooleanKeys()
        {
            RemoteConfigLoadResult result = LoadOwnHash(BundledSource());
            string[] required = RemoteConfigAllowlist.GetRequiredKeys();

            Assert.That(result.Succeeded, Is.True, result.Message);
            Assert.That(required.Length, Is.EqualTo(10));
            Assert.That(result.Snapshot.Count, Is.EqualTo(required.Length));
            CollectionAssert.AreEqual(required, result.Snapshot.GetKeys());
            for (int i = 0; i < required.Length; i++)
            {
                Assert.That(
                    result.Snapshot.TryGetBoolean(required[i], out bool enabled),
                    Is.True,
                    required[i]
                );
                Assert.That(enabled, Is.False, required[i]);
            }
        }

        [Test]
        public void RepeatedLoadAndCanonicalSerialization_AreByteStable()
        {
            string source = BundledSource();

            RemoteConfigLoadResult first = LoadOwnHash(source);
            RemoteConfigLoadResult second = LoadOwnHash(source);

            Assert.That(first.ContentSha256, Is.EqualTo(second.ContentSha256));
            Assert.That(
                RemoteConfigDeterministicJson.Serialize(first.Snapshot),
                Is.EqualTo(RemoteConfigDeterministicJson.Serialize(second.Snapshot))
            );
            Assert.That(first.ContentSha256, Is.EqualTo(ExpectedContentSha256));
        }

        [Test]
        public void BomAndLineEndingNormalization_PreservePinnedSourceIdentity()
        {
            string normalized = RemoteConfigSourceHash.Normalize(BundledSource());
            string crlfWithBom = "\uFEFF" + normalized.Replace("\n", "\r\n");

            RemoteConfigLoadResult result =
                new DeterministicLocalRemoteConfigProvider(crlfWithBom).LoadSnapshot();

            Assert.That(result.Succeeded, Is.True, result.Message);
            Assert.That(result.SourceSha256, Is.EqualTo(
                DeterministicLocalRemoteConfigProvider.ExpectedBundledSourceSha256
            ));
        }

        [Test]
        public void EntryOrdering_DoesNotChangeCanonicalContentHash()
        {
            string source = BundledSource();
            string reordered = ReverseEntryLines(source);

            RemoteConfigLoadResult first = LoadOwnHash(source);
            RemoteConfigLoadResult second = LoadOwnHash(reordered);

            Assert.That(first.Succeeded, Is.True, first.Message);
            Assert.That(second.Succeeded, Is.True, second.Message);
            Assert.That(second.SourceSha256, Is.Not.EqualTo(first.SourceSha256));
            Assert.That(second.ContentSha256, Is.EqualTo(first.ContentSha256));
            Assert.That(
                RemoteConfigDeterministicJson.Serialize(second.Snapshot),
                Is.EqualTo(RemoteConfigDeterministicJson.Serialize(first.Snapshot))
            );
        }

        [Test]
        public void OrdinaryWhitespaceMutation_FailsPinnedSourceHashClosed()
        {
            string mutated = BundledSource() + " ";

            RemoteConfigLoadResult result =
                new DeterministicLocalRemoteConfigProvider(mutated).LoadSnapshot();

            AssertFallback(
                result,
                RemoteConfigLoadStatus.SourceHashMismatch,
                RemoteConfigFailureCodes.SourceHashMismatch
            );
        }

        [Test]
        public void EmptyAndOversizedSources_ReturnConservativeFallback()
        {
            AssertFallback(
                new DeterministicLocalRemoteConfigProvider(string.Empty).LoadSnapshot(),
                RemoteConfigLoadStatus.SourceMissing,
                RemoteConfigFailureCodes.SourceMissing
            );

            string oversized = new string(
                'x',
                65537
            );
            AssertFallback(
                new DeterministicLocalRemoteConfigProvider(
                    oversized,
                    RemoteConfigSourceHash.ComputeNormalizedSha256(oversized)
                ).LoadSnapshot(),
                RemoteConfigLoadStatus.SourceTooLarge,
                RemoteConfigFailureCodes.SourceTooLarge
            );
        }

        [Test]
        public void MalformedCommentAndTrailingData_AreStrictlyRejected()
        {
            string source = BundledSource();
            string malformed = source.Substring(0, source.Length - 2);
            string comment = source.Replace(
                "{\n  \"schemaVersion\"",
                "{\n  // forbidden\n  \"schemaVersion\""
            );
            string trailing = source + "true";

            AssertJsonInvalid(malformed);
            AssertJsonInvalid(comment);
            AssertJsonInvalid(trailing);
        }

        [Test]
        public void UnknownDuplicateAndMissingTopLevelFields_AreRejectedExactly()
        {
            string source = BundledSource();
            string unknown = source.Replace(
                "  \"entries\": [",
                "  \"unknown\": true,\n  \"entries\": ["
            );
            string duplicate = source.Replace(
                "  \"schemaVersion\": 1,",
                "  \"schemaVersion\": 1,\n  \"schemaVersion\": 1,"
            );
            string missing = RemoveLineContaining(source, "\"snapshotId\"");

            AssertFailureContains(unknown, RemoteConfigFailureCodes.JsonUnknownField);
            AssertFailureContains(duplicate, RemoteConfigFailureCodes.JsonDuplicateField);
            AssertFailureContains(missing, RemoteConfigFailureCodes.JsonMissingField);
        }

        [Test]
        public void UnsupportedSchemaIdentityEnvironmentAndAuthority_FailAtomically()
        {
            string source = BundledSource();

            AssertFailureContains(
                source.Replace("\"schemaVersion\": 1", "\"schemaVersion\": 2"),
                RemoteConfigFailureCodes.UnsupportedSchema
            );
            AssertFailureContains(
                source.Replace(
                    "v1.16-step01-local-defaults",
                    "v1.16-step01-local-mutated"
                ),
                RemoteConfigFailureCodes.SnapshotIdentityInvalid
            );
            AssertFailureContains(
                source.Replace("\"environment\": \"development\"", "\"environment\": \"production\""),
                RemoteConfigFailureCodes.EnvironmentUnsupported
            );
            AssertFailureContains(
                source.Replace("\"proposalOnly\": true", "\"proposalOnly\": false"),
                RemoteConfigFailureCodes.ProposalOnlyRequired
            );
            AssertFailureContains(
                source.Replace("\"authoritative\": false", "\"authoritative\": true"),
                RemoteConfigFailureCodes.AuthorityForbidden
            );
        }

        [Test]
        public void EconomyCatalogIdentityAndHashMismatch_FailClosed()
        {
            string source = BundledSource();

            AssertFailureContains(
                source.Replace("v1.15-step01-proposal", "v1.15-step01-other"),
                RemoteConfigFailureCodes.EconomyBindingMismatch
            );
            AssertFailureContains(
                source.Replace(
                    RemoteConfigValidator.RequiredEconomyCatalogSourceSha256,
                    new string('0', 64)
                ),
                RemoteConfigFailureCodes.EconomyBindingMismatch
            );
        }

        [Test]
        public void UnknownDuplicateMissingAndWrongTypeEntries_FailClosed()
        {
            string source = BundledSource();
            string unknown = source.Replace(
                RemoteConfigKeys.AnalyticsCaptureEnabled,
                "analytics.unknown.enabled"
            );
            string duplicate = source.Replace(
                RemoteConfigKeys.AnalyticsUploadEnabled,
                RemoteConfigKeys.AnalyticsCaptureEnabled
            );
            string missing = RemoveLineContaining(
                source,
                RemoteConfigKeys.AnalyticsUploadEnabled
            );
            string wrongType = source.Replace(
                "{ \"key\": \"analytics.capture.enabled\", \"type\": \"bool\"",
                "{ \"key\": \"analytics.capture.enabled\", \"type\": \"string\""
            );

            AssertFailureContains(unknown, RemoteConfigFailureCodes.UnknownKey);
            AssertFailureContains(duplicate, RemoteConfigFailureCodes.DuplicateKey);
            AssertFailureContains(missing, RemoteConfigFailureCodes.MissingKey);
            AssertFailureContains(wrongType, RemoteConfigFailureCodes.TypeMismatch);
        }

        [Test]
        public void NonCanonicalValueAndUnsafeExternalEnablement_FailClosed()
        {
            string source = BundledSource();
            string nonCanonical = source.Replace(
                "{ \"key\": \"analytics.capture.enabled\", \"type\": \"bool\", \"value\": \"false\" }",
                "{ \"key\": \"analytics.capture.enabled\", \"type\": \"bool\", \"value\": \"False\" }"
            );
            string unsafeUpload = source.Replace(
                "{ \"key\": \"analytics.upload.enabled\", \"type\": \"bool\", \"value\": \"false\" }",
                "{ \"key\": \"analytics.upload.enabled\", \"type\": \"bool\", \"value\": \"true\" }"
            );
            string unsafeIap = source.Replace(
                "{ \"key\": \"monetization.iap.enabled\", \"type\": \"bool\", \"value\": \"false\" }",
                "{ \"key\": \"monetization.iap.enabled\", \"type\": \"bool\", \"value\": \"true\" }"
            );

            AssertFailureContains(nonCanonical, RemoteConfigFailureCodes.ValueInvalid);
            AssertFailureContains(unsafeUpload, RemoteConfigFailureCodes.UnsafeEnablement);
            AssertFailureContains(unsafeIap, RemoteConfigFailureCodes.UnsafeEnablement);
        }

        [Test]
        public void OneInvalidEntry_NeverPartiallyMergesValidPrefix()
        {
            string source = BundledSource().Replace(
                "{ \"key\": \"analytics.upload.enabled\", \"type\": \"bool\", \"value\": \"false\" }",
                "{ \"key\": \"analytics.upload.enabled\", \"type\": \"bool\", \"value\": \"true\" }"
            );

            RemoteConfigLoadResult result = LoadOwnHash(source);

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Snapshot, Is.SameAs(RemoteConfigSnapshot.ConservativeFallback));
            Assert.That(result.Snapshot.IsFallback, Is.True);
            AssertEveryFlagFalse(result.Snapshot);
        }

        [Test]
        public void Fallback_IsImmutableStableAndNeverAuthoritative()
        {
            RemoteConfigSnapshot fallback = RemoteConfigSnapshot.ConservativeFallback;
            string first = RemoteConfigDeterministicJson.Serialize(fallback);
            string[] keys = fallback.GetKeys();
            keys[0] = "mutated";

            Assert.That(fallback.IsFallback, Is.True);
            Assert.That(fallback.IsAuthoritative, Is.False);
            Assert.That(fallback.ProposalOnly, Is.True);
            Assert.That(fallback.GetKeys()[0], Is.Not.EqualTo("mutated"));
            Assert.That(RemoteConfigDeterministicJson.Serialize(fallback), Is.EqualTo(first));
            AssertEveryFlagFalse(fallback);
        }

        static string BundledSource()
        {
            TextAsset source = Resources.Load<TextAsset>(
                DeterministicLocalRemoteConfigProvider.ResourcePath
            );
            Assert.That(source, Is.Not.Null, "Bundled remote-config TextAsset is missing.");
            return source.text;
        }

        static RemoteConfigLoadResult LoadOwnHash(string source)
        {
            return new DeterministicLocalRemoteConfigProvider(
                source,
                RemoteConfigSourceHash.ComputeNormalizedSha256(source)
            ).LoadSnapshot();
        }

        static void AssertJsonInvalid(string source)
        {
            RemoteConfigLoadResult result = LoadOwnHash(source);
            AssertFallback(
                result,
                RemoteConfigLoadStatus.JsonInvalid,
                RemoteConfigFailureCodes.JsonInvalid
            );
        }

        static void AssertFailureContains(string source, string expectedCode)
        {
            RemoteConfigLoadResult result = LoadOwnHash(source);
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.RuntimeApplied, Is.False);
            Assert.That(result.Snapshot.IsFallback, Is.True);
            RemoteConfigViolation[] violations = result.Validation.GetViolations();
            var codes = new List<string>();
            for (int i = 0; i < violations.Length; i++)
                codes.Add(violations[i].Code);
            CollectionAssert.Contains(codes, expectedCode);
            AssertEveryFlagFalse(result.Snapshot);
        }

        static void AssertFallback(
            RemoteConfigLoadResult result,
            RemoteConfigLoadStatus status,
            string code
        )
        {
            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.RuntimeApplied, Is.False);
            Assert.That(result.Status, Is.EqualTo(status));
            Assert.That(result.Code, Is.EqualTo(code));
            Assert.That(result.Snapshot, Is.SameAs(RemoteConfigSnapshot.ConservativeFallback));
            AssertEveryFlagFalse(result.Snapshot);
        }

        static void AssertEveryFlagFalse(RemoteConfigSnapshot snapshot)
        {
            string[] keys = RemoteConfigAllowlist.GetRequiredKeys();
            for (int i = 0; i < keys.Length; i++)
            {
                Assert.That(snapshot.TryGetBoolean(keys[i], out bool value), Is.True);
                Assert.That(value, Is.False, keys[i]);
            }
        }

        static string RemoveLineContaining(string source, string marker)
        {
            string normalized = RemoteConfigSourceHash.Normalize(source);
            string[] lines = normalized.Split('\n');
            var builder = new StringBuilder(normalized.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf(marker, StringComparison.Ordinal) >= 0)
                    continue;
                if (builder.Length > 0)
                    builder.Append('\n');
                builder.Append(lines[i]);
            }
            return builder.ToString();
        }

        static string ReverseEntryLines(string source)
        {
            string normalized = RemoteConfigSourceHash.Normalize(source);
            string[] lines = normalized.Split('\n');
            int first = -1;
            int last = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf("{ \"key\":", StringComparison.Ordinal) >= 0)
                {
                    if (first < 0)
                        first = i;
                    last = i;
                }
            }
            Assert.That(first, Is.GreaterThanOrEqualTo(0));
            Assert.That(last, Is.GreaterThan(first));

            var entries = new List<string>();
            for (int i = first; i <= last; i++)
                entries.Add(lines[i]);
            entries.Reverse();
            for (int i = 0; i < entries.Count; i++)
            {
                string line = entries[i].TrimEnd();
                line = line.TrimEnd(',');
                if (i < entries.Count - 1)
                    line += ",";
                lines[first + i] = line;
            }

            return string.Join("\n", lines);
        }
    }
}
