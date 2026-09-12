#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>Step 10.2 authority gate for account/language/tutorial/system-state visual candidates.</summary>
    public static class Step20ProductionAssetWave04Step102BoundaryGate
    {
        const string RelativeRoot="ProjectDocs/V1_20_STEP10_2/RuntimeEvidence";
        const string PassLock=RelativeRoot+"/ACCOUNT_SYSTEM_BOUNDARY_PASS.lock";
        const string ResultJson=RelativeRoot+"/ACCOUNT_SYSTEM_BOUNDARY_RESULT.json";
        static readonly ScreenId[] Pages={ScreenId.Settings,ScreenId.LanguageSelect,ScreenId.Login,ScreenId.TutorialOverlay,ScreenId.OfflineState,ScreenId.MaintenanceState,ScreenId.EmptyErrorState};
        static readonly FrozenFile[] Frozen={
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs","6b66f9d6ad08c15b087489685cd2056b9b479e3389c19ce2773094b958861a02"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs","655879000c576ec5bf317a85ce8743b1b06afa0aeba374c8c278fc5fa5bcf4e3"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs","d8853d33a49a6d21d414fab1830b957d032d047ed242b259af47d749414b6163"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs","86c02a50a752302c9448ffb0574200810e0529bba994796e91033dccdb48518d"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRegistry.cs","aee233191e2696acdf944fbb207cbfb6580ccc80a80941e960b13f09058a8938"),
            new FrozenFile("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMetaPageCutoverRegistry.cs","fc1bdb088b71d5d0998fcafbcecc4a49ba38088ff0bce48245e9f42cfb2ee4b3")};
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 10.2 Account-System Boundary Verification")]
        public static void RunMenu(){var f=Validate(true);if(f.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,f.ToArray()));}
        public static List<string> Validate(bool writeEvidence)
        {
            var failures=new List<string>();
            foreach(FrozenFile frozen in Frozen){if(!File.Exists(frozen.path)){failures.Add("Missing frozen authority file: "+frozen.path);continue;}string actual=Sha256(frozen.path);if(!string.Equals(actual,frozen.sha256,StringComparison.OrdinalIgnoreCase))failures.Add("Authority drift: "+frozen.path+" expected "+frozen.sha256+" but found "+actual+".");}
            string game=Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs");RequireContains(game,"public void ToggleMusic()","Settings music must remain GameState-owned.",failures);RequireContains(game,"public void ToggleSfx()","Settings SFX must remain GameState-owned.",failures);RequireContains(game,"public void ToggleVibration()","Settings vibration must remain GameState-owned.",failures);RequireContains(game,"public void ToggleNotifications()","Settings notifications must remain GameState-owned.",failures);RequireContains(game,"public bool CompleteTutorial(string tutorialId)","Tutorial completion must remain GameState/save-owned.",failures);
            string demo=Read("Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs");RequireContains(demo,"public static void SetLanguage(string code)","Demo language state boundary missing.",failures);RequireContains(demo,"public static void SignInGuest()","Guest demo state boundary missing.",failures);if(demo.IndexOf("MonetizationService",StringComparison.Ordinal)>=0||demo.IndexOf("VerifiedEntitlementGate",StringComparison.Ordinal)>=0)failures.Add("DemoFeatureState must not reference commercial fulfillment authority.");
            string router=Read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");RequireContains(router,"GameState.Instance.ToggleMusic()","Settings hotspot must still call GameState.ToggleMusic.",failures);RequireContains(router,"DemoFeatureState.SetLanguage(code)","LanguageSelect must remain demo-session state.",failures);RequireContains(router,"DemoFeatureState.SignInGuest()","Login guest path must remain demo-session state.",failures);RequireContains(router,"Гостевая демо-сессия активна. Облачная учётная запись не подключена.","Login must explicitly remain cloud-disconnected demo behavior.",failures);RequireContains(router,"case ScreenId.OfflineState:","Offline presentation route missing.",failures);RequireContains(router,"case ScreenId.MaintenanceState:","Maintenance presentation route missing.",failures);RequireContains(router,"case ScreenId.EmptyErrorState:","Empty/error presentation route missing.",failures);
            string tutorial=Read("Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs");RequireContains(tutorial,"GameState.Instance.HasCompletedTutorial(step.id)","Tutorial presenter must respect saved completion.",failures);RequireContains(tutorial,"board.TryPause()","Tutorial presenter must retain gameplay pause semantics.",failures);
            if(!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))failures.Add("Accepted core-loop production registry changed; expected 10 routes.");
            foreach(ScreenId id in Pages){string path=Step20ProductionAssetWave04PrefabBuilder.CandidateRoot+"/"+id+".prefab";GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(prefab==null){failures.Add("Missing Wave 4 candidate: "+path);continue;}if(prefab.GetComponentsInChildren<Button>(true).Length!=0)failures.Add(path+": candidate contains Button behavior; authority must remain in ScreenRouter/GameState/demo boundaries.");if(prefab.GetComponentsInChildren<EventTrigger>(true).Length!=0)failures.Add(path+": candidate contains EventTrigger behavior.");foreach(string dep in AssetDatabase.GetDependencies(path,true)){string n=dep.Replace('\\','/');if(n.Contains("/Art/ReferenceOnly/")||n.Contains("/Resources/DesignScreens/")||n.Contains("/ProjectDocs/"))failures.Add(path+": forbidden reference/QA dependency "+dep);}}
            if(writeEvidence)WriteEvidence(failures);
            if(writeEvidence&&failures.Count==0){Debug.Log("v1.20 Step 10.2 Account/System boundary PASS: frozen ownership + 7/7 passive candidates.");if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Step 10.2 Account/System boundary PASS. Settings/account/tutorial/system visual candidates remain passive and authority remains unchanged.","OK");}
            return failures;
        }
        public static string AbsolutePassLock(){return Path.Combine(Directory.GetCurrentDirectory(),PassLock.Replace('/',Path.DirectorySeparatorChar));}
        static void WriteEvidence(List<string> failures){string absRoot=Path.Combine(Directory.GetCurrentDirectory(),RelativeRoot.Replace('/',Path.DirectorySeparatorChar));Directory.CreateDirectory(absRoot);string pass=AbsolutePassLock();if(File.Exists(pass))File.Delete(pass);string json="{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step10.2-wave04-account-system-boundary\",\n  \"passed\": "+(failures.Count==0?"true":"false")+",\n  \"frozenAuthorityFiles\": 6,\n  \"candidatePages\": 7,\n  \"candidateButtonsExpected\": 0,\n  \"candidateEventTriggersExpected\": 0,\n  \"settingsPersistedByGameState\": true,\n  \"guestIdentityProviderActivated\": false,\n  \"languageBackendActivated\": false,\n  \"networkAuthorityActivated\": false,\n  \"remoteConfigMaintenanceAuthorityActivated\": false,\n  \"activeWave04CutoverAuthorized\": false,\n  \"failureCount\": "+failures.Count+"\n}\n";File.WriteAllText(Path.Combine(absRoot,"ACCOUNT_SYSTEM_BOUNDARY_RESULT.json"),json);if(failures.Count==0)File.WriteAllText(pass,"v1.20-step10.2-account-system-boundary-pass\nactive-wave04-cutover-authorized=false\nidentity-network-authority-activated=false\n");AssetDatabase.Refresh();}
        static string Read(string p){return File.Exists(p)?File.ReadAllText(p):string.Empty;}static void RequireContains(string s,string n,string m,List<string> f){if(s.IndexOf(n,StringComparison.Ordinal)<0)f.Add(m);}static string Sha256(string p){using(SHA256 sha=SHA256.Create())using(FileStream st=File.OpenRead(p)){return BitConverter.ToString(sha.ComputeHash(st)).Replace("-",string.Empty).ToLowerInvariant();}}
        readonly struct FrozenFile{public readonly string path,sha256;public FrozenFile(string p,string s){path=p;sha256=s;}}
    }
}
#endif
