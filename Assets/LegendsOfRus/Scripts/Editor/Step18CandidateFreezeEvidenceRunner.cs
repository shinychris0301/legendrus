#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LegendsOfRus.CandidateFreeze;
using LegendsOfRus.ControlledTestBuild;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18CandidateFreezeEvidenceRunner
    {
        const string EvidenceDir = "ProjectDocs/ClosedTestEvidence";
        const string ArtifactDir = EvidenceDir + "/Artifacts";
        const string ArtifactEvidenceFile = EvidenceDir + "/signed_aab_evidence.json";
        const string DeviceEvidenceFile = EvidenceDir + "/physical_device_evidence.json";
        const string ManifestFile = EvidenceDir + "/CANDIDATE_FREEZE_MANIFEST.json";
        const string ManifestHashFile = EvidenceDir + "/CANDIDATE_FREEZE_MANIFEST.sha256";

        [Serializable] sealed class Activation
        {
            public int schemaVersion;
            public string candidateId = string.Empty;
            public bool localEvidenceValidationApproved;
            public bool localFreezeManifestToolingApproved;
            public bool unityCompilePassed;
            public bool editorSelfTestPassed;
            public bool editModePassed;
            public bool playModePassed;
            public bool signedAppBundleEvidenceRetained;
            public bool physicalDeviceEvidenceRetained;
            public bool packageLockReviewed;
            public bool projectSettingsReviewed;
            public bool storeReleaseChecklistApproved;
            public bool privacyDataSafetyApproved;
            public bool productionApplicationIdentifierApproved;
            public bool versioningApproved;
            public bool productionSigningApproved;
            public bool controlledTestBuildAuthorized;
            public bool candidateFreezeAuthorized;
            public bool closedTestUploadAuthorized;
            public bool storePublishingAuthorized;
        }

        [MenuItem("Legends of Rus/QA/Print v1.18 Step 5 Candidate Freeze Preflight")]
        public static void PrintPreflight()
        {
            EvaluationContext context = EvaluateCurrent();
            Debug.Log(Format(context));
        }

        [MenuItem("Legends of Rus/QA/Generate v1.18 Step 5 Candidate Freeze Manifest")]
        public static void GenerateFreezeManifest()
        {
            EvaluationContext context = EvaluateCurrent();
            if (!context.FreezeResult.MayFreezeCandidate)
                throw new InvalidOperationException("Candidate freeze is blocked.\n" + string.Join("\n", context.FreezeResult.Blockers));
            if (context.ArtifactValidation == null || !context.ArtifactValidation.Valid || context.DeviceValidation == null || !context.DeviceValidation.Valid)
                throw new InvalidOperationException("Validated artifact and physical-device evidence are required before candidate freeze.");

            var manifest = new CandidateFreezeManifest
            {
                CandidateId = context.ActivationData.candidateId,
                SourceTreeSha256 = context.SourceTreeSha256,
                PackagesLockSha256 = context.PackagesLockSha256,
                ProjectSettingsSha256 = context.ProjectSettingsSha256,
                StoreChecklistSha256 = context.StoreChecklistSha256,
                ArtifactFileName = context.ArtifactEvidence.FileName,
                ArtifactSha256 = context.ArtifactEvidence.Sha256,
                ArtifactSizeBytes = context.ArtifactEvidence.SizeBytes,
                VersionName = context.ArtifactEvidence.VersionName,
                VersionCode = context.ArtifactEvidence.VersionCode,
                TargetApiLevel = context.ArtifactEvidence.TargetApiLevel,
                DeviceEvidenceSha256 = Sha256File(Full(DeviceEvidenceFile)),
                PassedDeviceProfiles = context.DeviceValidation.PassedProfiles,
                Frozen = true
            };
            string output = JsonUtility.ToJson(manifest, true) + "\n";
            File.WriteAllText(Full(ManifestFile), output, new UTF8Encoding(false));
            File.WriteAllText(Full(ManifestHashFile), Sha256Bytes(Encoding.UTF8.GetBytes(output)) + "  CANDIDATE_FREEZE_MANIFEST.json\n", new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log("v1.18 Step 5 candidate freeze manifest written. This freezes local evidence only; it does not upload or publish the candidate.");
        }

        sealed class EvaluationContext
        {
            public Activation ActivationData;
            public ControlledTestArtifactEvidence ArtifactEvidence;
            public ControlledTestArtifactValidationResult ArtifactValidation;
            public DeviceEvidenceValidationResult DeviceValidation;
            public CandidateFreezeResult FreezeResult;
            public string SourceTreeSha256 = string.Empty;
            public string PackagesLockSha256 = string.Empty;
            public string ProjectSettingsSha256 = string.Empty;
            public string StoreChecklistSha256 = string.Empty;
            public string[] IntakeIssues = Array.Empty<string>();
        }

        static EvaluationContext EvaluateCurrent()
        {
            var context = new EvaluationContext();
            var issues = new List<string>();
            context.ActivationData = LoadActivation();
            context.SourceTreeSha256 = HashSourceTree();
            context.PackagesLockSha256 = HashFileIfPresent("Packages/packages-lock.json");
            context.ProjectSettingsSha256 = HashDirectory("ProjectSettings");
            context.StoreChecklistSha256 = HashFileIfPresent("Assets/LegendsOfRus/Documentation/V1_18_STEP03_RELEASE_CHECKLIST.json");

            string[] aabs = Directory.Exists(Full(ArtifactDir)) ? Directory.GetFiles(Full(ArtifactDir), "*.aab", SearchOption.TopDirectoryOnly) : Array.Empty<string>();
            if (aabs.Length != 1) issues.Add("Expected exactly one retained .aab under " + ArtifactDir + ", found " + aabs.Length + ".");
            if (!File.Exists(Full(ArtifactEvidenceFile))) issues.Add("Missing " + ArtifactEvidenceFile + ".");
            if (!File.Exists(Full(DeviceEvidenceFile))) issues.Add("Missing " + DeviceEvidenceFile + ".");

            if (aabs.Length == 1 && File.Exists(Full(ArtifactEvidenceFile)))
            {
                context.ArtifactEvidence = JsonUtility.FromJson<ControlledTestArtifactEvidence>(File.ReadAllText(Full(ArtifactEvidenceFile)));
                var fi = new FileInfo(aabs[0]);
                context.ArtifactEvidence.FileName = fi.Name;
                context.ArtifactEvidence.SizeBytes = fi.Length;
                context.ArtifactEvidence.Sha256 = Sha256File(fi.FullName);
                context.ArtifactValidation = ControlledTestArtifactValidator.Validate(context.ArtifactEvidence);
                if (!context.ArtifactValidation.Valid) issues.AddRange(context.ArtifactValidation.Issues);
                if (File.Exists(Full(DeviceEvidenceFile)))
                {
                    PhysicalDeviceEvidenceSet set = JsonUtility.FromJson<PhysicalDeviceEvidenceSet>(File.ReadAllText(Full(DeviceEvidenceFile)));
                    context.DeviceValidation = PhysicalDeviceEvidenceValidator.Validate(set, context.ArtifactEvidence.Sha256);
                    if (!context.DeviceValidation.Valid) issues.AddRange(context.DeviceValidation.Issues);
                }
            }

            var e = new CandidateFreezeEvidence
            {
                CandidateId = context.ActivationData == null ? string.Empty : context.ActivationData.candidateId,
                SourceTreeSha256 = context.SourceTreeSha256,
                PackagesLockSha256 = context.PackagesLockSha256,
                ProjectSettingsSha256 = context.ProjectSettingsSha256,
                StoreChecklistSha256 = context.StoreChecklistSha256,
                SignedArtifactEvidenceValid = context.ArtifactValidation != null && context.ArtifactValidation.Valid && context.ActivationData != null && context.ActivationData.signedAppBundleEvidenceRetained,
                DeviceEvidenceComplete = context.DeviceValidation != null && context.DeviceValidation.Valid && context.ActivationData != null && context.ActivationData.physicalDeviceEvidenceRetained,
                UnityCompilePassed = context.ActivationData != null && context.ActivationData.unityCompilePassed,
                EditorSelfTestPassed = context.ActivationData != null && context.ActivationData.editorSelfTestPassed,
                EditModePassed = context.ActivationData != null && context.ActivationData.editModePassed,
                PlayModePassed = context.ActivationData != null && context.ActivationData.playModePassed,
                StoreReleaseChecklistApproved = context.ActivationData != null && context.ActivationData.storeReleaseChecklistApproved,
                PrivacyDataSafetyApproved = context.ActivationData != null && context.ActivationData.privacyDataSafetyApproved,
                ProductionApplicationIdentifierApproved = context.ActivationData != null && context.ActivationData.productionApplicationIdentifierApproved,
                VersioningApproved = context.ActivationData != null && context.ActivationData.versioningApproved,
                ProductionSigningApproved = context.ActivationData != null && context.ActivationData.productionSigningApproved,
                ControlledTestBuildAuthorized = context.ActivationData != null && context.ActivationData.controlledTestBuildAuthorized,
                CandidateFreezeAuthorized = context.ActivationData != null && context.ActivationData.candidateFreezeAuthorized
            };
            context.FreezeResult = CandidateFreezeEvaluator.Evaluate(e);
            context.IntakeIssues = issues.ToArray();
            return context;
        }

        static string Format(EvaluationContext c)
        {
            var sb = new StringBuilder();
            sb.AppendLine("v1.18 Step 5 — Candidate Freeze Preflight");
            sb.AppendLine("Candidate: " + (c.ActivationData == null ? "<activation missing>" : c.ActivationData.candidateId));
            sb.AppendLine("May freeze: " + c.FreezeResult.MayFreezeCandidate);
            sb.AppendLine("Source tree SHA-256: " + c.SourceTreeSha256);
            sb.AppendLine("Packages lock SHA-256: " + c.PackagesLockSha256);
            sb.AppendLine("ProjectSettings SHA-256: " + c.ProjectSettingsSha256);
            sb.AppendLine("Store checklist SHA-256: " + c.StoreChecklistSha256);
            if (c.ArtifactValidation != null) sb.AppendLine("AAB evidence valid: " + c.ArtifactValidation.Valid);
            if (c.DeviceValidation != null) sb.AppendLine("Physical profiles: " + c.DeviceValidation.PassedProfiles + "/" + c.DeviceValidation.RequiredProfiles);
            if (c.IntakeIssues.Length > 0) sb.AppendLine("Evidence intake issues:\n- " + string.Join("\n- ", c.IntakeIssues));
            if (c.FreezeResult.Blockers.Length > 0) sb.AppendLine("Freeze blockers:\n- " + string.Join("\n- ", c.FreezeResult.Blockers));
            sb.AppendLine("Closed-test upload/publishing are separate authorizations and are not performed by this tool.");
            return sb.ToString();
        }

        static Activation LoadActivation()
        {
            string p = Full("Assets/LegendsOfRus/Resources/Config/candidate_freeze_activation.json");
            return File.Exists(p) ? JsonUtility.FromJson<Activation>(File.ReadAllText(p)) : null;
        }

        static string HashSourceTree()
        {
            var roots = new[] { "Assets", "Packages", "ProjectSettings", "Tools" };
            using (var sha = SHA256.Create())
            {
                var lines = new List<string>();
                foreach (string root in roots)
                {
                    string abs = Full(root);
                    if (!Directory.Exists(abs)) continue;
                    foreach (string file in Directory.GetFiles(abs, "*", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal))
                    {
                        string rel = file.Substring(Full("").Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\','/');
                        lines.Add(rel + "\t" + Sha256File(file));
                    }
                }
                return Sha256Bytes(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
            }
        }

        static string HashDirectory(string relative)
        {
            string abs = Full(relative);
            if (!Directory.Exists(abs)) return string.Empty;
            var lines = Directory.GetFiles(abs, "*", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal)
                .Select(x => x.Substring(abs.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\','/') + "\t" + Sha256File(x));
            return Sha256Bytes(Encoding.UTF8.GetBytes(string.Join("\n", lines)));
        }
        static string HashFileIfPresent(string relative) => File.Exists(Full(relative)) ? Sha256File(Full(relative)) : string.Empty;
        static string Sha256File(string path) { using (var sha=SHA256.Create()) return ToHex(sha.ComputeHash(File.ReadAllBytes(path))); }
        static string Sha256Bytes(byte[] bytes) { using (var sha=SHA256.Create()) return ToHex(sha.ComputeHash(bytes)); }
        static string ToHex(byte[] bytes) { var sb=new StringBuilder(bytes.Length*2); for(int i=0;i<bytes.Length;i++) sb.Append(bytes[i].ToString("x2")); return sb.ToString(); }
        static string Full(string relative) => Path.GetFullPath(Path.Combine(Application.dataPath, "..", relative ?? string.Empty));
    }
}
#endif
