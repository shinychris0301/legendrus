#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep11ReleaseCandidate
    {
        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11/Evaluate RC Stabilization Status")]
        public static void Evaluate()
        {
            Step20ProductionAssetStep11Evidence.RcStatus status=Step20ProductionAssetStep11Evidence.EvaluateAndWrite();
            string message="Development RC: "+status.developmentDecision+"\nStore-release RC: "+status.storeDecision+"\nActive routes: "+status.active+"/36\nApproved active routes: "+status.activeApproved+"/"+status.active+".";
            Debug.Log(message);
            if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus",message,"OK");
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11/Create Development RC Freeze")]
        public static void CreateDevelopmentRcFreeze()
        {
            EnsureEditorSafe();
            Step20ProductionAssetStep11Evidence.RcStatus status=Step20ProductionAssetStep11Evidence.EvaluateAndWrite();
            if(status.developmentDecision!="READY_FOR_DEVELOPMENT_RC_FREEZE")throw new InvalidOperationException("Development RC freeze is not eligible: "+status.developmentDecision+". Complete regression/Android gates for the current integration first.");
            string lockPath=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep11Evidence.DevelopmentRcLock);
            string manifestPath=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep11Evidence.DevelopmentRcManifest);
            if(File.Exists(lockPath)||File.Exists(manifestPath))throw new InvalidOperationException("A Step 11 development RC freeze already exists. Validate it or clear it before creating another freeze.");
            if(!EditorUtility.DisplayDialog("Legends of Rus","Freeze the CURRENT integration fingerprint as the v1.20 Step 11 development release candidate?\n\nThis does not activate any additional page. Any later cutover/rollback will invalidate this freeze.","Freeze RC","Cancel"))return;
            Directory.CreateDirectory(Path.GetDirectoryName(manifestPath));
            string manifest=BuildManifest(status);
            File.WriteAllText(manifestPath,manifest);
            string manifestHash=Step20ProductionAssetStep11Evidence.Sha256File(manifestPath);
            File.WriteAllText(lockPath,"v1.20-step11-development-rc-freeze\nintegration-fingerprint="+status.fingerprint+"\nmanifest-sha256="+manifestHash+"\nrecorded-at-utc="+DateTime.UtcNow.ToString("o")+"\n");
            AssetDatabase.Refresh();
            Debug.Log("v1.20 Step 11 development RC freeze created for integration fingerprint "+status.fingerprint+". Store-release readiness remains: "+status.storeDecision+".");
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11/Validate Development RC Freeze")]
        public static void ValidateDevelopmentRcFreeze()
        {
            List<string> failures=Step20ProductionAssetStep11Evidence.ValidateDevelopmentRcFreeze();
            Step20ProductionAssetStep11Evidence.RcStatus status=Step20ProductionAssetStep11Evidence.EvaluateAndWrite();
            if(status.developmentDecision!="READY_FOR_DEVELOPMENT_RC_FREEZE")failures.Add("Current integration no longer satisfies development RC regression gates: "+status.developmentDecision+".");
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 11 development RC freeze VALID for current integration fingerprint. Store-release decision: "+status.storeDecision+".");
            if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Development RC freeze is valid for the current integration.\n\nStore-release status: "+status.storeDecision,"OK");
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11/Clear Development RC Freeze")]
        public static void ClearDevelopmentRcFreeze()
        {
            EnsureEditorSafe();
            if(!EditorUtility.DisplayDialog("Legends of Rus","Clear the Step 11 development RC freeze/manifest? This does not change production routes or rollback snapshots.","Clear Freeze","Cancel"))return;
            DeleteIfExists(Step20ProductionAssetStep11Evidence.DevelopmentRcLock);
            DeleteIfExists(Step20ProductionAssetStep11Evidence.DevelopmentRcManifest);
            AssetDatabase.Refresh();
            Debug.Log("v1.20 Step 11 development RC freeze cleared.");
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11/Apply Evidence-Eligible Non-Core Cutovers")]
        public static void ApplyEligibleCutovers()
        {
            EnsureEditorSafe();
            ClearFreezeIfPresentOrThrow();
            Step20ProductionAssetStep103CutoverReadiness.ReadinessResult readiness=Step20ProductionAssetStep103CutoverReadiness.EvaluateAndWrite();
            if(readiness.eligibleNonCore<=0)throw new InvalidOperationException("No non-core page currently satisfies the Step 10.3 evidence-qualified cutover gate.");
            Step20ProductionAssetStep103RollbackCutover.ApplyEligible();
        }

        [MenuItem("Legends of Rus/Release Candidate/v1.20 Step 11/Roll Back Step 10.3 Cutover Batch")]
        public static void RollBackStep103Batch()
        {
            EnsureEditorSafe();
            ClearFreezeIfPresentOrThrow();
            Step20ProductionAssetStep103RollbackCutover.RollBackAll();
        }

        static string BuildManifest(Step20ProductionAssetStep11Evidence.RcStatus status)
        {
            var active=new List<string>();
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                ScreenId id=Step20ProductionAssetStep103Evidence.Pages[i].id;
                if(ProductionCoreLoopPageRegistry.IsCutover(id)||ProductionMetaPageCutoverRegistry.IsCutover(id))active.Add(id.ToString());
            }
            string manifestHash=HashIfExists("Packages/manifest.json"), lockHash=HashIfExists("Packages/packages-lock.json"), projectVersionHash=HashIfExists("ProjectSettings/ProjectVersion.txt");
            var sb=new StringBuilder();
            sb.Append("{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step11-development-release-candidate-freeze\",\n")
              .Append("  \"integrationFingerprint\": \"").Append(status.fingerprint).Append("\",\n")
              .Append("  \"developmentRcDecision\": \"").Append(status.developmentDecision).Append("\",\n")
              .Append("  \"storeReleaseRcDecisionAtFreeze\": \"").Append(status.storeDecision).Append("\",\n")
              .Append("  \"activeProductionRoutes\": ").Append(status.active).Append(",\n")
              .Append("  \"activeRoutesWithApprovedReference\": ").Append(status.activeApproved).Append(",\n")
              .Append("  \"activeRoutesWithRuntimeEvidence\": ").Append(status.activeRuntime).Append(",\n")
              .Append("  \"packagesManifestSha256\": \"").Append(manifestHash).Append("\",\n")
              .Append("  \"packagesLockSha256\": \"").Append(lockHash).Append("\",\n")
              .Append("  \"projectVersionSha256\": \"").Append(projectVersionHash).Append("\",\n")
              .Append("  \"activePages\": [");
            for(int i=0;i<active.Count;i++){if(i>0)sb.Append(',');sb.Append('"').Append(active[i]).Append('"');}
            sb.Append("],\n  \"frozenAtUtc\": \"").Append(DateTime.UtcNow.ToString("o")).Append("\"\n}\n");
            return sb.ToString();
        }

        static string HashIfExists(string relative){string abs=Step20ProductionAssetStep103Evidence.Absolute(relative);return File.Exists(abs)?Step20ProductionAssetStep11Evidence.Sha256File(abs):string.Empty;}
        static void ClearFreezeIfPresentOrThrow(){if(File.Exists(Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep11Evidence.DevelopmentRcLock)))throw new InvalidOperationException("A Step 11 RC freeze exists. Clear it explicitly before any cutover or rollback.");}
        static void DeleteIfExists(string relative){string abs=Step20ProductionAssetStep103Evidence.Absolute(relative);if(File.Exists(abs))File.Delete(abs);}
        static void EnsureEditorSafe(){if(Application.isBatchMode)throw new InvalidOperationException("Step 11 RC cutover/freeze actions cannot run in batch mode.");if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before Step 11 RC actions.");}
    }
}
#endif
