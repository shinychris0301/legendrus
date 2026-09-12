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
    public static class Step20ProductionAssetStep103AndroidRegression
    {
        public const string Root = "ProjectDocs/V1_20_STEP10_3/AndroidRegression";
        public const string FunctionalLock = Root + "/FULL_FUNCTIONAL_REGRESSION_PASS.lock";
        public const string AndroidLock = Root + "/PHYSICAL_ANDROID_REGRESSION_PASS.lock";
        public const string FinalDecision = Root + "/FINAL_ANDROID_REGRESSION_ACCEPTANCE.json";

        [MenuItem("Legends of Rus/Tests/v1.20 Step 10.3/Record Full Functional Regression PASS")]
        public static void RecordFunctionalPass()
        {
            EnsureEditorSafe();
            List<string> authority = Step20ProductionAssetStep103Evidence.ValidateGlobalAuthorityBoundaries();
            List<string> activeBatch = Step20ProductionAssetStep103RollbackCutover.ValidateActiveBatch();
            authority.AddRange(activeBatch);
            if(authority.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,authority.ToArray()));
            if(!EditorUtility.DisplayDialog("Legends of Rus","Confirm you completed the full EditMode + PlayMode + Step 5 demo-flow regression on the CURRENT integration state with no errors/regressions.\n\nThis records evidence only; it does not change gameplay or routing.","Record PASS","Cancel"))return;
            WriteLock(FunctionalLock,"v1.20-step10.3-full-functional-regression-pass");
            DeleteIfExists(AndroidLock); DeleteIfExists(FinalDecision); AssetDatabase.Refresh();
            Debug.Log("v1.20 Step 10.3 full functional regression PASS recorded for integration fingerprint "+CurrentIntegrationFingerprint()+".");
        }

        [MenuItem("Legends of Rus/Tests/v1.20 Step 10.3/Record Physical Android Regression PASS")]
        public static void RecordAndroidPass()
        {
            EnsureEditorSafe();
            ValidateLockOrThrow(FunctionalLock,"Functional regression PASS");
            string checklist =
                "Confirm physical Android testing passed on the CURRENT integration state:\n\n"+
                "• cold boot / Splash / Loading / Home\n• safe area/cutout at tested device ratios\n• Home ↔ World Map ↔ Chapter Map + visible/system Back\n• Pre-Level → Gameplay, 4-way swipe, boosters, obstacles, pause/resume/restart\n• Victory/Fail/Chapter Complete\n• Shop/Special Offer fail-closed purchase authority\n• Events/rankings/team/chat/friends/mail authority boundaries\n• Settings persistence, language, guest login, tutorial\n• Offline/Maintenance/Error routes\n• Cyrillic/live text, background/resume, save/load, energy/progression\n• no unauthorized currency/energy/reward grant\n\nRecord PASS only if all tested successfully.";
            if(!EditorUtility.DisplayDialog("Legends of Rus",checklist,"Record Android PASS","Cancel"))return;
            WriteLock(AndroidLock,"v1.20-step10.3-physical-android-regression-pass"); DeleteIfExists(FinalDecision); AssetDatabase.Refresh();
            Debug.Log("v1.20 Step 10.3 physical Android regression PASS recorded for integration fingerprint "+CurrentIntegrationFingerprint()+".");
        }

        [MenuItem("Legends of Rus/Tests/v1.20 Step 10.3/Evaluate Final Android Regression Acceptance")]
        public static void EvaluateFinalAcceptance()
        {
            var failures=new List<string>();
            try{ValidateLockOrThrow(FunctionalLock,"Functional regression PASS");}catch(Exception e){failures.Add(e.Message);}
            try{ValidateLockOrThrow(AndroidLock,"Physical Android regression PASS");}catch(Exception e){failures.Add(e.Message);}
            failures.AddRange(Step20ProductionAssetStep103Evidence.ValidateGlobalAuthorityBoundaries());
            failures.AddRange(Step20ProductionAssetStep103RollbackCutover.ValidateActiveBatch());
            string decision=failures.Count==0?"ACCEPTED_CURRENT_INTEGRATION_ANDROID_REGRESSION":"NO_GO_ANDROID_REGRESSION_ACCEPTANCE";
            string json="{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step10.3-full-android-regression\",\n  \"decision\": \""+decision+"\",\n  \"integrationFingerprint\": \""+CurrentIntegrationFingerprint()+"\",\n  \"functionalRegressionPass\": "+Bool(File.Exists(Absolute(FunctionalLock)))+",\n  \"physicalAndroidRegressionPass\": "+Bool(File.Exists(Absolute(AndroidLock)))+",\n  \"failureCount\": "+failures.Count+"\n}\n";
            File.WriteAllText(Absolute(FinalDecision),json); AssetDatabase.Refresh();
            if(failures.Count>0)throw new InvalidOperationException(decision+"\n"+string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 10.3 Android regression acceptance PASS for current integration fingerprint.");
            if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Step 10.3 final Android regression acceptance PASS for the current integration state.","OK");
        }

        public static void ClearAcceptanceLocks()
        {
            DeleteIfExists(FunctionalLock); DeleteIfExists(AndroidLock); DeleteIfExists(FinalDecision);
        }

        public static string CurrentIntegrationFingerprint()
        {
            var sb=new StringBuilder(); sb.Append("core=10;");
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                ScreenId id=Step20ProductionAssetStep103Evidence.Pages[i].id;
                bool active=ProductionCoreLoopPageRegistry.IsCutover(id)||ProductionMetaPageCutoverRegistry.IsCutover(id);
                if(!active)continue;
                sb.Append(id).Append('=');
                string prefab=Absolute("Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+id+".prefab");
                sb.Append(File.Exists(prefab)?Sha256File(prefab):"missing").Append(';');
            }
            using(var sha=SHA256.Create())
            {
                byte[] hash=sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())); return BitConverter.ToString(hash).Replace("-",string.Empty).ToLowerInvariant();
            }
        }

        static void WriteLock(string relative,string magic)
        {
            string abs=Absolute(relative);Directory.CreateDirectory(Path.GetDirectoryName(abs));
            File.WriteAllText(abs,magic+"\nintegration-fingerprint="+CurrentIntegrationFingerprint()+"\nrecorded-at-utc="+DateTime.UtcNow.ToString("o")+"\n");
        }

        static void ValidateLockOrThrow(string relative,string label)
        {
            string abs=Absolute(relative);if(!File.Exists(abs))throw new InvalidOperationException(label+" is missing.");
            string token="integration-fingerprint="+CurrentIntegrationFingerprint();if(File.ReadAllText(abs).IndexOf(token,StringComparison.Ordinal)<0)throw new InvalidOperationException(label+" was recorded for a different integration state; rerun regression after the latest cutover/rollback.");
        }

        static string Sha256File(string path){using(var sha=SHA256.Create())using(var stream=File.OpenRead(path)){return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-",string.Empty).ToLowerInvariant();}}
        static void DeleteIfExists(string relative){string p=Absolute(relative);if(File.Exists(p))File.Delete(p);}
        static string Absolute(string relative){return Step20ProductionAssetStep103Evidence.Absolute(relative);}
        static string Bool(bool value){return value?"true":"false";}
        static void EnsureEditorSafe(){if(Application.isBatchMode)throw new InvalidOperationException("Manual Step 10.3 acceptance cannot be recorded in batch mode.");if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before recording Step 10.3 acceptance.");}
    }
}
#endif
