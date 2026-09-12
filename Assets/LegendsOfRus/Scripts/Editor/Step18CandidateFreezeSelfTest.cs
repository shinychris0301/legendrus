#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using LegendsOfRus.CandidateFreeze;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18CandidateFreezeSelfTest
    {
        [UnityEditor.MenuItem("Legends of Rus/Tests/Run v1.18 Step 5 Self-Tests")]
        public static void Run()
        {
            int passed=0,total=0; var failures=new List<string>();
            Check("CandidateFreezeCore is engine-free", CoreIsEngineFree, ref passed, ref total, failures);
            Check("freeze schema version is 1", () => CandidateFreezeRequirements.SchemaVersion==1, ref passed, ref total, failures);
            Check("five physical profiles are required", () => CandidateFreezeRequirements.RequiredPhysicalProfileCount==5, ref passed, ref total, failures);
            Check("minimum device session is five minutes", () => CandidateFreezeRequirements.MinimumTestDurationMinutes==5, ref passed, ref total, failures);
            Check("freeze hash algorithm is SHA-256", () => CandidateFreezeRequirements.HashAlgorithm=="SHA-256", ref passed, ref total, failures);
            Check("candidate rc01 id is accepted", () => CandidateFreezeEvaluator.IsCandidateId("v1.18-step05-rc01"), ref passed, ref total, failures);
            Check("candidate rc00 id is rejected", () => !CandidateFreezeEvaluator.IsCandidateId("v1.18-step05-rc00"), ref passed, ref total, failures);
            Check("default freeze fails closed", () => !CandidateFreezeEvaluator.Evaluate(default).MayFreezeCandidate, ref passed, ref total, failures);
            Check("complete evidence may freeze", CompleteEvidenceCanFreeze, ref passed, ref total, failures);
            Check("valid five-profile device set passes", ValidDeviceSetPasses, ref passed, ref total, failures);
            Check("different artifact hash fails device set", DifferentArtifactFails, ref passed, ref total, failures);
            Check("duplicate profile fails device set", DuplicateProfileFails, ref passed, ref total, failures);
            Check("crash evidence fails device set", CrashEvidenceFails, ref passed, ref total, failures);
            Check("blocking visual defect fails device set", VisualDefectFails, ref passed, ref total, failures);
            Check("device template exists", () => File.Exists(Full("ProjectDocs/ClosedTestEvidence/physical_device_evidence.template.json")), ref passed, ref total, failures);
            Check("artifact template exists", () => File.Exists(Full("ProjectDocs/ClosedTestEvidence/signed_aab_evidence.template.json")), ref passed, ref total, failures);
            Check("artifact staging contains no AAB", () => Directory.GetFiles(Full("ProjectDocs/ClosedTestEvidence/Artifacts"),"*.aab",SearchOption.TopDirectoryOnly).Length==0, ref passed, ref total, failures);
            Check("activation enables local validation", () => Activation().Contains("\"localEvidenceValidationApproved\": true"), ref passed, ref total, failures);
            Check("activation enables local freeze tooling", () => Activation().Contains("\"localFreezeManifestToolingApproved\": true"), ref passed, ref total, failures);
            Check("Unity compile evidence remains false", () => Activation().Contains("\"unityCompilePassed\": false"), ref passed, ref total, failures);
            Check("Editor self-test evidence remains false", () => Activation().Contains("\"editorSelfTestPassed\": false"), ref passed, ref total, failures);
            Check("EditMode evidence remains false", () => Activation().Contains("\"editModePassed\": false"), ref passed, ref total, failures);
            Check("PlayMode evidence remains false", () => Activation().Contains("\"playModePassed\": false"), ref passed, ref total, failures);
            Check("signed AAB evidence remains false", () => Activation().Contains("\"signedAppBundleEvidenceRetained\": false"), ref passed, ref total, failures);
            Check("physical device evidence remains false", () => Activation().Contains("\"physicalDeviceEvidenceRetained\": false"), ref passed, ref total, failures);
            Check("production signing remains false", () => Activation().Contains("\"productionSigningApproved\": false"), ref passed, ref total, failures);
            Check("controlled build remains false", () => Activation().Contains("\"controlledTestBuildAuthorized\": false"), ref passed, ref total, failures);
            Check("candidate freeze remains false", () => Activation().Contains("\"candidateFreezeAuthorized\": false"), ref passed, ref total, failures);
            Check("closed-test upload remains false", () => Activation().Contains("\"closedTestUploadAuthorized\": false"), ref passed, ref total, failures);
            Check("store publishing remains false", () => Activation().Contains("\"storePublishingAuthorized\": false"), ref passed, ref total, failures);
            Check("freeze runner has no build invocation", () => !Runner().Contains("BuildPipeline.BuildPlayer"), ref passed, ref total, failures);
            Check("freeze runner has no network client", () => !Runner().Contains("UnityWebRequest") && !Runner().Contains("HttpClient"), ref passed, ref total, failures);
            Check("freeze runner has no signing secret fields", () => !Runner().Contains("keystorePass") && !Runner().Contains("keyaliasPass"), ref passed, ref total, failures);
            Check("freeze runner has no device identifiers", () => !Runner().Contains("deviceUniqueIdentifier") && !Runner().Contains("advertisingIdentifier") && !Runner().Contains("SystemInfo.deviceModel"), ref passed, ref total, failures);
            Check("approved PNG inventory remains 65", () => Directory.GetFiles(Full("Assets/LegendsOfRus"),"*.png",SearchOption.AllDirectories).Length==65, ref passed, ref total, failures);
            Check("save payload version remains 6", () => Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs").Contains("SaveVersion = 6") || Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs").Contains("CurrentSaveVersion = 6") || Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs").Contains("saveVersion = 6"), ref passed, ref total, failures);

            if(failures.Count==0){ Debug.Log("v1.18 Step 5 self-tests passed: "+passed+"/"+total+". Candidate freeze remains blocked until retained signed-AAB and physical-device evidence plus explicit authorization exist."); return; }
            string summary=string.Join("\n",failures.ToArray()); Debug.LogError("v1.18 Step 5 self-tests failed: "+passed+"/"+total+" passed.\n"+summary); throw new InvalidOperationException("v1.18 Step 5 self-tests failed.\n"+summary);
        }

        static bool CoreIsEngineFree()
        {
            string asm=Read("Assets/LegendsOfRus/Scripts/CandidateFreezeCore/LegendsOfRus.CandidateFreezeCore.asmdef");
            if(!asm.Contains("\"noEngineReferences\": true")) return false;
            foreach(string path in Directory.GetFiles(Full("Assets/LegendsOfRus/Scripts/CandidateFreezeCore"),"*.cs",SearchOption.AllDirectories))
            { string s=File.ReadAllText(path); if(s.Contains("using UnityEngine")||s.Contains("UnityEditor")) return false; }
            return true;
        }
        static CandidateFreezeEvidence Complete() => new CandidateFreezeEvidence
        {
            CandidateId="v1.18-step05-rc01",SourceTreeSha256=Hash('a'),PackagesLockSha256=Hash('b'),ProjectSettingsSha256=Hash('c'),StoreChecklistSha256=Hash('d'),
            SignedArtifactEvidenceValid=true,DeviceEvidenceComplete=true,UnityCompilePassed=true,EditorSelfTestPassed=true,EditModePassed=true,PlayModePassed=true,
            StoreReleaseChecklistApproved=true,PrivacyDataSafetyApproved=true,ProductionApplicationIdentifierApproved=true,VersioningApproved=true,
            ProductionSigningApproved=true,ControlledTestBuildAuthorized=true,CandidateFreezeAuthorized=true
        };
        static bool CompleteEvidenceCanFreeze()=>CandidateFreezeEvaluator.Evaluate(Complete()).MayFreezeCandidate;
        static PhysicalDeviceEvidenceSet Set()
        {
            string h=Hash('a'); return new PhysicalDeviceEvidenceSet{ArtifactSha256=h,Records=new[]{R("android_low_16_9_720p",720,1280,h),R("android_mid_18_9_1080p",1080,2160,h),R("android_mid_19_5_9_1080p",1080,2340,h),R("android_high_20_9_1080p",1080,2400,h),R("android_high_20_9_1440p",1440,3200,h)}};
        }
        static PhysicalDeviceEvidenceRecord R(string id,int w,int h,string sha)=>new PhysicalDeviceEvidenceRecord{ProfileId=id,ArtifactSha256=sha,ScreenWidth=w,ScreenHeight=h,TestDurationMinutes=10,FreshInstallPassed=true,CoreLoopPassed=true,SuspendResumePassed=true,NoCrashObserved=true,NoBlockingVisualDefectObserved=true,TesterAttested=true};
        static bool ValidDeviceSetPasses()=>PhysicalDeviceEvidenceValidator.Validate(Set(),Hash('a')).Valid;
        static bool DifferentArtifactFails(){var s=Set();s.Records[0].ArtifactSha256=Hash('b');return !PhysicalDeviceEvidenceValidator.Validate(s,Hash('a')).Valid;}
        static bool DuplicateProfileFails(){var s=Set();s.Records[4].ProfileId=s.Records[0].ProfileId;return !PhysicalDeviceEvidenceValidator.Validate(s,Hash('a')).Valid;}
        static bool CrashEvidenceFails(){var s=Set();s.Records[0].NoCrashObserved=false;return !PhysicalDeviceEvidenceValidator.Validate(s,Hash('a')).Valid;}
        static bool VisualDefectFails(){var s=Set();s.Records[0].NoBlockingVisualDefectObserved=false;return !PhysicalDeviceEvidenceValidator.Validate(s,Hash('a')).Valid;}
        static string Hash(char c)=>new string(c,64);
        static string Activation()=>Read("Assets/LegendsOfRus/Resources/Config/candidate_freeze_activation.json");
        static string Runner()=>Read("Assets/LegendsOfRus/Scripts/Editor/Step18CandidateFreezeEvidenceRunner.cs");
        static string Full(string relative)=>Path.GetFullPath(Path.Combine(Application.dataPath,"..",relative));
        static string Read(string relative)=>File.ReadAllText(Full(relative));
        static void Check(string name,Func<bool> condition,ref int passed,ref int total,List<string> failures){total++;try{if(condition()){passed++;return;}}catch(Exception ex){failures.Add("FAIL: "+name+" ("+ex.GetType().Name+": "+ex.Message+")");return;}failures.Add("FAIL: "+name);}
    }
}
#endif
