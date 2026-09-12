#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave04SelfTest
    {
        static readonly ScreenId[] Pages={ScreenId.Settings,ScreenId.LanguageSelect,ScreenId.Login,ScreenId.TutorialOverlay,ScreenId.OfflineState,ScreenId.MaintenanceState,ScreenId.EmptyErrorState};
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 10 Wave 4 Asset Reconstruction Self-Test")]
        public static void Run(){Step20ProductionAssetWave04PrefabBuilder.BuildAll(false); int checks=0; Require(Pages.Length==7,"Wave 4 page count must be 7.");checks++; Require(ProductionCutoverFreezeGuard.MatchesExpectedCount(10),"Accepted core registry changed.");checks++; Require(File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP10_WAVE04_ASSET_MANIFEST.json"),"Wave 4 manifest missing.");checks++; Require(File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP10_WAVE04_REQUIREMENT_COVERAGE.json"),"Wave 4 coverage missing.");checks++; foreach(ScreenId id in Pages){string p=Step20ProductionAssetWave04PrefabBuilder.CandidateRoot+"/"+id+".prefab"; GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(p); Require(prefab!=null,"Missing Wave 4 prefab: "+p);checks++; var m=prefab.GetComponent<ProductionCoreLoopPageMarker>(); Require(m!=null&&m.ScreenId==id&&m.ScreenshotFree,"Marker mismatch: "+id);checks++; Require(prefab.GetComponentsInChildren<Text>(true).Length>0,"No live text: "+id);checks++; Require(prefab.GetComponentsInChildren<Button>(true).Length==0,"Visual prefab contains Button behavior: "+id);checks++; foreach(string dep in AssetDatabase.GetDependencies(p,true)){string n=dep.Replace('\\','/'); Require(!n.Contains("/Art/ReferenceOnly/")&&!n.Contains("/Resources/DesignScreens/")&&!n.Contains("/ProjectDocs/"),"Forbidden runtime reference dependency: "+dep);checks++;}} Debug.Log("v1.20 Step 10 Wave 4 self-test PASS. Checks="+checks+". Candidates=7. No active runtime cutover.");}
        static void Require(bool c,string m){if(!c)throw new InvalidOperationException("v1.20 Step 10 self-test: "+m);}
    }
}
#endif
