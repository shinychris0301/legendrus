#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LegendsOfRus.ControlledTestBuild;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18ControlledTestBuildReleaseCandidateSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.18 Step 4 Self-Tests")]
        public static void Run()
        {
            int passed = 0, total = 0;
            var failures = new List<string>();
            Check("Unity version is pinned to 6000.5.9f1", () => Application.unityVersion == "6000.5.9f1", ref passed, ref total, failures);
            Check("Addressables 2.8.1 is retained", () => Read("Packages/manifest.json").Contains("\"com.unity.addressables\": \"2.8.1\""), ref passed, ref total, failures);
            Check("BalanceReview UnityTest hotfix is retained", () => Read("Assets/LegendsOfRus/Tests/BalanceReviewPlayMode/LegendsOfRus.BalanceReviewRuntime.PlayModeTests.asmdef").Contains("TestAssemblies"), ref passed, ref total, failures);
            Check("controlled-test core is engine-free", CoreIsEngineFree, ref passed, ref total, failures);
            Check("requirements snapshot is 2026-09-09", () => ControlledTestBuildRequirements.RequirementsSnapshotDate == "2026-09-09", ref passed, ref total, failures);
            Check("required target API is 36", () => ControlledTestBuildRequirements.RequiredTargetApiLevel == 36, ref passed, ref total, failures);
            Check("AAB is required", () => ControlledTestBuildRequirements.AppBundleRequired, ref passed, ref total, failures);
            Check("IL2CPP is required", () => ControlledTestBuildRequirements.Il2CppRequired, ref passed, ref total, failures);
            Check("ARM64 is required", () => ControlledTestBuildRequirements.Arm64Required, ref passed, ref total, failures);
            Check("16 KB native page compatibility is required", () => ControlledTestBuildRequirements.SixteenKbNativePageSizeRequired, ref passed, ref total, failures);
            Check("package registration date is pinned", () => ControlledTestBuildRequirements.PackageRegistrationEffectiveDate == "2026-09-30", ref passed, ref total, failures);
            Check("default preflight fails closed", DefaultFailsClosed, ref passed, ref total, failures);
            Check("technical plan can pass without signing when technical evidence is complete", TechnicalPlanCanPass, ref passed, ref total, failures);
            Check("signed AAB gate still requires signing approvals", SigningGateBlocks, ref passed, ref total, failures);
            Check("closed-test upload gate still requires artifact evidence", ArtifactGateBlocks, ref passed, ref total, failures);
            Check("valid artifact evidence is accepted", ValidArtifactAccepted, ref passed, ref total, failures);
            Check("non-AAB artifact evidence is rejected", InvalidArtifactRejected, ref passed, ref total, failures);
            Check("local controlled-test preflight is approved", () => Activation().Contains("\"localControlledTestPreflightApproved\": true"), ref passed, ref total, failures);
            Check("package lock review remains false", () => Activation().Contains("\"packageLockReviewed\": false"), ref passed, ref total, failures);
            Check("ProjectSettings review remains false", () => Activation().Contains("\"projectSettingsReviewed\": false"), ref passed, ref total, failures);
            Check("production identity remains unapproved", () => Activation().Contains("\"productionApplicationIdentifierApproved\": false"), ref passed, ref total, failures);
            Check("Play App Signing remains unapproved", () => Activation().Contains("\"playAppSigningApproved\": false"), ref passed, ref total, failures);
            Check("upload-key workflow remains unapproved", () => Activation().Contains("\"uploadKeyWorkflowApproved\": false"), ref passed, ref total, failures);
            Check("production signing remains blocked", () => Activation().Contains("\"productionSigningApproved\": false"), ref passed, ref total, failures);
            Check("controlled-test build remains blocked", () => Activation().Contains("\"controlledTestBuildAuthorized\": false"), ref passed, ref total, failures);
            Check("closed-test upload remains blocked", () => Activation().Contains("\"closedTestUploadAuthorized\": false"), ref passed, ref total, failures);
            Check("store publishing remains blocked", () => Activation().Contains("\"storePublishingAuthorized\": false"), ref passed, ref total, failures);
            Check("Step 4 source never invokes BuildPipeline.BuildPlayer", NoBuildInvocation, ref passed, ref total, failures);
            Check("Step 4 source contains no keystore password fields", NoCredentialFields, ref passed, ref total, failures);
            Check("approved PNG inventory remains 65", () => Directory.GetFiles(Full("Assets/LegendsOfRus"), "*.png", SearchOption.AllDirectories).Length == 65, ref passed, ref total, failures);
            Check("save payload version remains 6", () => Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs").Contains("SaveVersion = 6") || Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs").Contains("CurrentSaveVersion = 6") || Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs").Contains("saveVersion = 6"), ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                Debug.Log("v1.18 Step 4 self-tests passed: " + passed + "/" + total +
                    ". This validates the controlled-test build/preflight source boundary only; no signed AAB or Play upload is authorized.");
                return;
            }
            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError("v1.18 Step 4 self-tests failed: " + passed + "/" + total + " passed.\n" + summary);
            throw new InvalidOperationException("v1.18 Step 4 self-tests failed.\n" + summary);
        }

        static bool CoreIsEngineFree()
        {
            string asm = Read("Assets/LegendsOfRus/Scripts/ControlledTestBuildCore/LegendsOfRus.ControlledTestBuildCore.asmdef");
            if (!asm.Contains("\"noEngineReferences\": true")) return false;
            foreach (string path in Directory.GetFiles(Full("Assets/LegendsOfRus/Scripts/ControlledTestBuildCore"), "*.cs", SearchOption.AllDirectories))
            {
                string s = File.ReadAllText(path);
                if (s.Contains("using UnityEngine") || s.Contains("UnityEditor")) return false;
            }
            return true;
        }

        static bool DefaultFailsClosed()
        {
            ControlledTestBuildResult r = ControlledTestBuildEvaluator.Evaluate(default);
            return !r.MayPrepareReleaseCandidatePlan && !r.MayCreateSignedAppBundle && !r.MayUploadClosedTest && r.PlanBlockers.Length >= 8;
        }

        static ControlledTestBuildEvidence TechnicalEvidence()
        {
            return new ControlledTestBuildEvidence
            {
                SourceStaticVerificationPassed = true, UnityCompilePassed = true, EditorSelfTestPassed = true,
                EditModePassed = true, PlayModePassed = true, RequiredDeviceProfiles = 5, PassedDeviceProfiles = 5,
                PackageLockReviewed = true, ProjectSettingsReviewed = true, TargetApiLevel = 36, AppBundleSelected = true,
                Il2CppEnabled = true, Arm64Enabled = true, SixteenKbPageCompatibilityVerified = true
            };
        }

        static bool TechnicalPlanCanPass()
        {
            ControlledTestBuildResult r = ControlledTestBuildEvaluator.Evaluate(TechnicalEvidence());
            return r.MayPrepareReleaseCandidatePlan && !r.MayCreateSignedAppBundle && !r.MayUploadClosedTest;
        }

        static bool SigningGateBlocks()
        {
            ControlledTestBuildResult r = ControlledTestBuildEvaluator.Evaluate(TechnicalEvidence());
            return Array.IndexOf(r.BuildBlockers, ControlledTestBuildBlockerCodes.ProductionSigningMissing) >= 0;
        }

        static bool ArtifactGateBlocks()
        {
            ControlledTestBuildEvidence e = TechnicalEvidence();
            e.ProductionApplicationIdentifierApproved = true; e.VersioningApproved = true; e.PlayAppSigningApproved = true;
            e.UploadKeyWorkflowApproved = true; e.ProductionSigningApproved = true; e.ControlledTestBuildAuthorized = true;
            ControlledTestBuildResult r = ControlledTestBuildEvaluator.Evaluate(e);
            return r.MayCreateSignedAppBundle && !r.MayUploadClosedTest &&
                Array.IndexOf(r.UploadBlockers, ControlledTestBuildBlockerCodes.ArtifactEvidenceMissing) >= 0;
        }

        static bool ValidArtifactAccepted()
        {
            var e = new ControlledTestArtifactEvidence
            {
                FileName = "LegendsOfRus-closed-test.aab", Sha256 = new string('a', 64), SizeBytes = 1024,
                VersionName = "0.1.0", VersionCode = 10, TargetApiLevel = 36, Signed = true, AppBundle = true,
                Arm64Included = true, SixteenKbPageCompatibilityVerified = true
            };
            return ControlledTestArtifactValidator.Validate(e).Valid;
        }

        static bool InvalidArtifactRejected()
        {
            var e = new ControlledTestArtifactEvidence { FileName = "bad.apk", Sha256 = "bad", SizeBytes = 1 };
            return !ControlledTestArtifactValidator.Validate(e).Valid;
        }

        static bool NoBuildInvocation()
        {
            string root = Full("Assets/LegendsOfRus/Scripts/ControlledTestBuildCore");
            string editor = Read("Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs");
            return !editor.Contains("BuildPipeline.BuildPlayer") &&
                Array.TrueForAll(Directory.GetFiles(root, "*.cs"), p => !File.ReadAllText(p).Contains("BuildPipeline.BuildPlayer"));
        }

        static bool NoCredentialFields()
        {
            string s = Read("Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs") +
                Read("Assets/LegendsOfRus/Scripts/ControlledTestBuildCore/ControlledTestBuildModels.cs");
            return !s.Contains("keystorePass") && !s.Contains("keyaliasPass") && !s.Contains("keystoreName") && !s.Contains("keyaliasName");
        }

        static string Activation() => Read("Assets/LegendsOfRus/Resources/Config/controlled_test_build_activation.json");
        static string Full(string relative) => Path.GetFullPath(Path.Combine(Application.dataPath, "..", relative));
        static string Read(string relative) => File.ReadAllText(Full(relative));
        static void Check(string name, Func<bool> condition, ref int passed, ref int total, List<string> failures)
        {
            total++;
            try { if (condition()) { passed++; return; } }
            catch (Exception ex) { failures.Add("FAIL: " + name + " (" + ex.GetType().Name + ": " + ex.Message + ")"); return; }
            failures.Add("FAIL: " + name);
        }
    }
}
#endif
