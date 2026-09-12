#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using LegendsOfRus.AndroidBuild;
using LegendsOfRus.CommercialRuntime;
using LegendsOfRus.SaveCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step17SaveMigrationRecoveryFoundationSelfTest
    {
        const string PrimaryKey = "selftest.primary";
        const string BackupKey = "selftest.backup";
        const string StagingKey = "selftest.staging";
        const long FixedUtcTicks = 638923680000000000L;

        [MenuItem("Legends of Rus/Tests/Run v1.17 Step 2 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("payload v6 and envelope v1 are independently pinned",
                VersionsAreIndependentlyPinned,
                ref passed, ref total, failures);
            Check("legacy PlayerPrefs primary key identity is exact",
                LegacyPrimaryKeyIsExact,
                ref passed, ref total, failures);
            Check("current payload inspection does not mutate its source",
                CurrentMigrationIsNonMutating,
                ref passed, ref total, failures);
            Check("legacy migration returns v6 without mutating its source",
                LegacyMigrationIsNonMutating,
                ref passed, ref total, failures);
            Check("future payload diagnostics block without mutation",
                FutureMigrationIsBlockedWithoutMutation,
                ref passed, ref total, failures);
            Check("envelope encoding and decoding are deterministic",
                EnvelopeRoundTripIsDeterministic,
                ref passed, ref total, failures);
            Check("tamper diagnostics do not alter supplied evidence",
                EnvelopeTamperDiagnosticIsNonMutating,
                ref passed, ref total, failures);
            Check("primary journal diagnostics are read-only",
                PrimaryJournalLoadIsReadOnly,
                ref passed, ref total, failures);
            Check("backup recovery diagnostics are read-only",
                BackupRecoveryIsReadOnly,
                ref passed, ref total, failures);
            Check("unrecoverable diagnostics preserve corrupt evidence",
                UnrecoverableLoadIsReadOnly,
                ref passed, ref total, failures);
            Check("runtime economy activation remains disabled",
                EconomyActivationRemainsDisabled,
                ref passed, ref total, failures);
            Check("commercial provider activation remains disabled",
                CommercialActivationRemainsDisabled,
                ref passed, ref total, failures);
            Check("production Android activation remains disabled",
                AndroidProductionRemainsDisabled,
                ref passed, ref total, failures);
            Check("resolved package-lock approval remains false",
                ResolvedPackageLockApprovalRemainsFalse,
                ref passed, ref total, failures);
            Check("Unity version boundary remains exactly 6000.5.9f1",
                UnityVersionBoundaryIsExact,
                ref passed, ref total, failures);
            Check("save sources never call PlayerPrefs.DeleteAll",
                SaveSourcesNeverDeleteAllPlayerPrefs,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log(
                    "v1.17 Step 2 save migration/recovery foundation " +
                    "self-tests passed: " + passed + "/" + total +
                    ". Payload v6 is wrapped by envelope v1; economy, " +
                    "commercial, package-lock, and production Android " +
                    "activation remain blocked."
                );
                if (!Application.isBatchMode)
                {
                    EditorUtility.DisplayDialog(
                        "Legends of Rus — Tests",
                        "v1.17 Step 2 self-tests passed: " + passed + "/" +
                        total + ".\n\nThis is an Editor self-test only. It does " +
                        "not claim Test Runner, crash-interruption, device, " +
                        "package-lock approval, or activation evidence.",
                        "OK"
                    );
                }
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError(
                "v1.17 Step 2 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary
            );
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Test failures",
                    summary,
                    "OK"
                );
            }
            throw new InvalidOperationException(
                "v1.17 Step 2 self-tests failed."
            );
        }

        static bool VersionsAreIndependentlyPinned()
        {
            return
                new PlayerSaveData().saveVersion == 6 &&
                PlayerSaveMigrationRegistry.CurrentPayloadVersion == 6 &&
                SaveEnvelopeCodec.CurrentEnvelopeVersion == 1 &&
                PlayerSaveMigrationRegistry.CurrentPayloadVersion !=
                    SaveEnvelopeCodec.CurrentEnvelopeVersion;
        }

        static bool LegacyPrimaryKeyIsExact()
        {
            FieldInfo key = typeof(SaveService).GetField(
                "Key",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return
                key != null &&
                key.IsPrivate &&
                key.IsLiteral &&
                Equals(key.GetRawConstantValue(), "LegendsOfRus_Save_v1");
        }

        static bool CurrentMigrationIsNonMutating()
        {
            var source = new PlayerSaveData
            {
                saveVersion = 6,
                gold = 4321,
                gems = 123,
                lastEnergyUtcTicks = FixedUtcTicks
            };
            source.completedStoryScenes.Add("selftest.current");
            string before = JsonUtility.ToJson(source);

            PlayerSaveMigrationResult result =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    null,
                    FixedUtcTicks
                );

            return
                result != null &&
                result.Succeeded &&
                !result.Changed &&
                result.SourceVersion == 6 &&
                result.ReasonCode == PlayerSaveMigrationRegistry.AlreadyCurrent &&
                result.Data != null &&
                !ReferenceEquals(source, result.Data) &&
                result.Data.saveVersion == 6 &&
                result.Data.gold == 4321 &&
                JsonUtility.ToJson(source) == before;
        }

        static bool LegacyMigrationIsNonMutating()
        {
            var source = new PlayerSaveData
            {
                saveVersion = 1,
                gold = 2468,
                gems = 135,
                energyMax = 0,
                lastEnergyUtcTicks = 0
            };
            string before = JsonUtility.ToJson(source);

            PlayerSaveMigrationResult result =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    null,
                    FixedUtcTicks
                );

            return
                result != null &&
                result.Succeeded &&
                result.Changed &&
                result.SourceVersion == 1 &&
                result.ReasonCode == PlayerSaveMigrationRegistry.Migrated &&
                result.Data != null &&
                result.Data.saveVersion == 6 &&
                result.Data.gold == 2468 &&
                result.Data.gems == 135 &&
                result.Data.lastEnergyUtcTicks == FixedUtcTicks &&
                source.saveVersion == 1 &&
                source.lastEnergyUtcTicks == 0 &&
                JsonUtility.ToJson(source) == before;
        }

        static bool FutureMigrationIsBlockedWithoutMutation()
        {
            var source = new PlayerSaveData
            {
                saveVersion = 7,
                gold = 777,
                lastEnergyUtcTicks = FixedUtcTicks
            };
            string before = JsonUtility.ToJson(source);

            PlayerSaveMigrationResult result =
                PlayerSaveMigrationRegistry.Migrate(
                    source,
                    null,
                    FixedUtcTicks
                );

            return
                result != null &&
                !result.Succeeded &&
                !result.Changed &&
                result.SourceVersion == 7 &&
                result.ReasonCode ==
                    PlayerSaveMigrationRegistry.FutureVersionBlocked &&
                source.saveVersion == 7 &&
                source.gold == 777 &&
                JsonUtility.ToJson(source) == before;
        }

        static bool EnvelopeRoundTripIsDeterministic()
        {
            string payload = JsonUtility.ToJson(new PlayerSaveData
            {
                saveVersion = 6,
                gold = 606,
                lastEnergyUtcTicks = FixedUtcTicks
            });
            string before = payload;
            var codec = new SaveEnvelopeCodec();
            SaveEnvelopeEncodeResult first = codec.TryEncode(payload, 6, 41);
            SaveEnvelopeEncodeResult second = codec.TryEncode(payload, 6, 41);
            if (
                first == null ||
                second == null ||
                !first.Success ||
                !second.Success
            )
            {
                return false;
            }

            SaveEnvelopeDecodeResult decoded = codec.Decode(first.Envelope, 6);
            return
                first.Envelope == second.Envelope &&
                decoded != null &&
                decoded.Success &&
                decoded.Payload == payload &&
                decoded.PayloadVersion == 6 &&
                decoded.Revision == 41 &&
                payload == before;
        }

        static bool EnvelopeTamperDiagnosticIsNonMutating()
        {
            var codec = new SaveEnvelopeCodec();
            SaveEnvelopeEncodeResult encoded = codec.TryEncode(
                "{\"saveVersion\":6,\"gold\":91}",
                6,
                9
            );
            if (encoded == null || !encoded.Success)
                return false;

            const string prefix = "payloadSha256=";
            int index = encoded.Envelope.IndexOf(
                prefix,
                StringComparison.Ordinal
            );
            if (index < 0)
                return false;
            index += prefix.Length;
            char replacement = encoded.Envelope[index] == '0' ? '1' : '0';
            string tampered =
                encoded.Envelope.Substring(0, index) + replacement +
                encoded.Envelope.Substring(index + 1);
            string before = tampered;

            SaveEnvelopeDecodeResult decoded = codec.Decode(tampered, 6);
            return
                decoded != null &&
                !decoded.Success &&
                decoded.Status == SaveEnvelopeDecodeStatus.IntegrityMismatch &&
                !string.IsNullOrEmpty(decoded.DiagnosticCode) &&
                tampered == before;
        }

        static bool PrimaryJournalLoadIsReadOnly()
        {
            var codec = new SaveEnvelopeCodec();
            SaveEnvelopeEncodeResult encoded = codec.TryEncode(
                "{\"saveVersion\":6,\"gold\":51}",
                6,
                5
            );
            if (encoded == null || !encoded.Success)
                return false;

            var store = new ProbeStore();
            store.Seed(PrimaryKey, encoded.Envelope);
            SaveJournalLoadResult result = Repository(store, codec).Load();
            return
                result != null &&
                result.Status == SaveJournalLoadStatus.LoadedPrimary &&
                result.Source == SaveJournalSlot.Primary &&
                result.PayloadVersion == 6 &&
                result.Revision == 5 &&
                result.Payload == "{\"saveVersion\":6,\"gold\":51}" &&
                store.IsUntouched &&
                store.Matches(PrimaryKey, encoded.Envelope) &&
                store.Count == 1;
        }

        static bool BackupRecoveryIsReadOnly()
        {
            var codec = new SaveEnvelopeCodec();
            SaveEnvelopeEncodeResult backup = codec.TryEncode(
                "{\"saveVersion\":6,\"gold\":37}",
                6,
                3
            );
            if (backup == null || !backup.Success)
                return false;

            const string corruptPrimary = "corrupt-primary-evidence";
            var store = new ProbeStore();
            store.Seed(PrimaryKey, corruptPrimary);
            store.Seed(BackupKey, backup.Envelope);
            SaveJournalLoadResult result = Repository(store, codec).Load();
            return
                result != null &&
                result.Status == SaveJournalLoadStatus.RecoveredBackup &&
                result.Source == SaveJournalSlot.Backup &&
                result.PayloadVersion == 6 &&
                result.Revision == 3 &&
                result.Payload == "{\"saveVersion\":6,\"gold\":37}" &&
                store.IsUntouched &&
                store.Matches(PrimaryKey, corruptPrimary) &&
                store.Matches(BackupKey, backup.Envelope) &&
                store.Count == 2;
        }

        static bool UnrecoverableLoadIsReadOnly()
        {
            const string corruptPrimary = "corrupt-primary-evidence";
            const string corruptBackup = "corrupt-backup-evidence";
            var store = new ProbeStore();
            store.Seed(PrimaryKey, corruptPrimary);
            store.Seed(BackupKey, corruptBackup);
            SaveJournalLoadResult result = Repository(
                store,
                new SaveEnvelopeCodec()
            ).Load();
            return
                result != null &&
                result.Status == SaveJournalLoadStatus.Unrecoverable &&
                !result.Success &&
                result.Payload == null &&
                !string.IsNullOrEmpty(result.DiagnosticCode) &&
                store.IsUntouched &&
                store.Matches(PrimaryKey, corruptPrimary) &&
                store.Matches(BackupKey, corruptBackup) &&
                store.Count == 2;
        }

        static bool EconomyActivationRemainsDisabled()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                RuntimeEconomyActivationGate.ManifestResourcePath
            );
            if (asset == null)
                return false;
            RuntimeEconomyActivationManifestData manifest =
                JsonUtility.FromJson<RuntimeEconomyActivationManifestData>(
                    asset.text
                );
            RuntimeEconomyActivationResult decision =
                RuntimeEconomyActivationGate.Load();
            return
                manifest != null &&
                !manifest.compilationVerified &&
                !manifest.editorSelfTestsPassed &&
                !manifest.editModeTestsPassed &&
                !manifest.playModeTestsPassed &&
                !manifest.runtimeActivationApproved &&
                decision != null &&
                !decision.IsRuntimeActivated &&
                decision.UsesLegacyFallback;
        }

        static bool CommercialActivationRemainsDisabled()
        {
            TextAsset asset = Resources.Load<TextAsset>(
                CommercialRuntimeActivationGate.ResourcePath
            );
            if (asset == null)
                return false;
            CommercialActivationMirror value =
                JsonUtility.FromJson<CommercialActivationMirror>(asset.text);
            FieldInfo compiled = typeof(CommercialRuntimeActivationGate)
                .GetField(
                    "ExternalProviderActivationCompiled",
                    BindingFlags.Static | BindingFlags.NonPublic
                );
            return
                value != null &&
                value.environment == "disabled" &&
                !value.v115RuntimeEconomyApproved &&
                !value.v116Step1CompilationVerified &&
                !value.v116Step1ProjectValidatorPassed &&
                !value.v116Step1EditorSelfTestsPassed &&
                !value.v116Step1EditModeTestsPassed &&
                !value.v116Step1PlayModeTestsPassed &&
                !value.providerSelectionApproved &&
                !value.legalPrivacyApproved &&
                !value.consentFlowApproved &&
                !value.productIdentifiersApproved &&
                !value.rewardedPlacementsApproved &&
                !value.regionalBehaviorApproved &&
                !value.securityReviewPassed &&
                !value.representativeDeviceFailurePathsPassed &&
                !value.saveMigrationApproved &&
                !value.durableFulfillmentApproved &&
                !value.analyticsActivationApproved &&
                !value.iapActivationApproved &&
                !value.rewardedAdsActivationApproved &&
                !value.finalRuntimeActivationApproved &&
                IsPrivateFalseConstant(compiled);
        }

        static bool AndroidProductionRemainsDisabled()
        {
            AndroidBuildPolicyLoadResult loaded =
                AndroidBuildPolicyLoader.Load();
            AndroidBuildFoundationPolicy value =
                loaded != null ? loaded.Policy : null;
            FieldInfo compiled = typeof(AndroidBuildGuard).GetField(
                "ProductionAndroidBuildCompiled",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return
                loaded != null &&
                loaded.IsValid &&
                value != null &&
                !value.allowReleaseApk &&
                !value.allowAppBundle &&
                !value.allowGradleExport &&
                !value.allowCustomSigning &&
                !value.productionApplicationIdentifierApproved &&
                !value.versioningApproved &&
                !value.sdkPolicyApproved &&
                !value.signingIdentityApproved &&
                !value.playAppSigningApproved &&
                !value.nativePluginInventoryApproved &&
                !value.sixteenKbPageCompatibilityVerified &&
                !value.dataSafetyApproved &&
                !value.productionBuildApproved &&
                IsPrivateFalseConstant(compiled);
        }

        static bool ResolvedPackageLockApprovalRemainsFalse()
        {
            FieldInfo approval = typeof(AndroidBuildStateReader).GetField(
                "ResolvedPackageLockApproved",
                BindingFlags.Static | BindingFlags.NonPublic
            );
            return IsPrivateFalseConstant(approval);
        }

        static bool UnityVersionBoundaryIsExact()
        {
            AndroidBuildPolicyLoadResult loaded =
                AndroidBuildPolicyLoader.Load();
            return
                Application.unityVersion == "6000.5.9f1" &&
                RuntimeEconomyActivationGate.RequiredUnityVersion ==
                    "6000.5.9f1" &&
                CommercialRuntimeActivationGate.RequiredUnityVersion ==
                    "6000.5.9f1" &&
                loaded != null &&
                loaded.Policy != null &&
                loaded.Policy.unityVersion == "6000.5.9f1";
        }

        static bool SaveSourcesNeverDeleteAllPlayerPrefs()
        {
            var forbidden = new Regex(
                @"PlayerPrefs\s*\.\s*DeleteAll\s*\(",
                RegexOptions.CultureInvariant
            );
            string root = Path.Combine(Application.dataPath, "LegendsOfRus");
            if (!Directory.Exists(root))
                return false;
            string[] files = Directory.GetFiles(
                root,
                "*.cs",
                SearchOption.AllDirectories
            );
            for (int i = 0; i < files.Length; i++)
            {
                if (forbidden.IsMatch(File.ReadAllText(files[i])))
                    return false;
            }
            return true;
        }

        static SaveJournalRepository Repository(
            ProbeStore store,
            SaveEnvelopeCodec codec
        )
        {
            return new SaveJournalRepository(
                store,
                codec,
                new RejectingLegacyInspector(),
                new SaveJournalKeys(PrimaryKey, BackupKey, StagingKey),
                6
            );
        }

        static bool IsPrivateFalseConstant(FieldInfo field)
        {
            return
                field != null &&
                field.IsPrivate &&
                field.IsLiteral &&
                Equals(field.GetRawConstantValue(), false);
        }

        static void Check(
            string name,
            Func<bool> assertion,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (assertion())
                {
                    passed++;
                    return;
                }
                failures.Add("FAIL: " + name);
            }
            catch (Exception exception)
            {
                failures.Add(
                    "ERROR: " + name + " (" +
                    exception.GetType().Name + ": " + exception.Message + ")"
                );
            }
        }

        sealed class RejectingLegacyInspector : ILegacySavePayloadInspector
        {
            public LegacyPayloadInspection Inspect(string serializedPayload)
            {
                return LegacyPayloadInspection.Invalid();
            }
        }

        sealed class ProbeStore : ISaveJournalStore
        {
            readonly Dictionary<string, string> values =
                new Dictionary<string, string>(StringComparer.Ordinal);

            public int Count { get { return values.Count; } }
            public int WriteCount { get; private set; }
            public int DeleteCount { get; private set; }
            public int FlushCount { get; private set; }
            public bool IsUntouched
            {
                get
                {
                    return
                        WriteCount == 0 &&
                        DeleteCount == 0 &&
                        FlushCount == 0;
                }
            }

            public void Seed(string key, string value)
            {
                values[key] = value;
            }

            public bool Matches(string key, string expected)
            {
                string value;
                return
                    values.TryGetValue(key, out value) &&
                    string.Equals(value, expected, StringComparison.Ordinal);
            }

            public bool TryRead(string key, out string value)
            {
                return values.TryGetValue(key, out value);
            }

            public void Write(string key, string value)
            {
                WriteCount++;
                values[key] = value;
            }

            public void Delete(string key)
            {
                DeleteCount++;
                values.Remove(key);
            }

            public void Flush()
            {
                FlushCount++;
            }
        }

        [Serializable]
        sealed class CommercialActivationMirror
        {
            public string environment = string.Empty;
            public bool v115RuntimeEconomyApproved;
            public bool v116Step1CompilationVerified;
            public bool v116Step1ProjectValidatorPassed;
            public bool v116Step1EditorSelfTestsPassed;
            public bool v116Step1EditModeTestsPassed;
            public bool v116Step1PlayModeTestsPassed;
            public bool providerSelectionApproved;
            public bool legalPrivacyApproved;
            public bool consentFlowApproved;
            public bool productIdentifiersApproved;
            public bool rewardedPlacementsApproved;
            public bool regionalBehaviorApproved;
            public bool securityReviewPassed;
            public bool representativeDeviceFailurePathsPassed;
            public bool saveMigrationApproved;
            public bool durableFulfillmentApproved;
            public bool analyticsActivationApproved;
            public bool iapActivationApproved;
            public bool rewardedAdsActivationApproved;
            public bool finalRuntimeActivationApproved;
        }
    }
}
#endif
