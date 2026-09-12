#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03SelfTest
    {
        static readonly ScreenId[] Pages={ScreenId.Events,ScreenId.EventDetail,ScreenId.IndividualRanking,ScreenId.TeamHub,ScreenId.TeamRanking,ScreenId.TeamChat,ScreenId.FriendsList,ScreenId.Mail};
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9 Wave 3 Asset Reconstruction Self-Test")]
        public static void Run(){Step20ProductionAssetWave03PrefabBuilder.BuildAll(false); int checks=0; Require(Pages.Length==8,"Wave 3 page count must be 8.");checks++; Require(ProductionCutoverFreezeGuard.MatchesExpectedCount(10),"Accepted core registry changed.");checks++; Require(File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_WAVE03_ASSET_MANIFEST.json"),"Wave 3 manifest missing.");checks++; Require(File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_WAVE03_REQUIREMENT_COVERAGE.json"),"Wave 3 coverage missing.");checks++; foreach(ScreenId id in Pages){string p=Step20ProductionAssetWave03PrefabBuilder.CandidateRoot+"/"+id+".prefab"; GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(p); Require(prefab!=null,"Missing Wave 3 prefab: "+p);checks++; var m=prefab.GetComponent<ProductionCoreLoopPageMarker>(); Require(m!=null&&m.ScreenId==id&&m.ScreenshotFree,"Marker mismatch: "+id);checks++; Require(prefab.GetComponentsInChildren<Text>(true).Length>0,"No live text: "+id);checks++; Require(prefab.GetComponentsInChildren<Button>(true).Length==0,"Visual prefab contains Button behavior: "+id);checks++; foreach(string dep in AssetDatabase.GetDependencies(p,true)){string n=dep.Replace('\\','/'); Require(!n.Contains("/Art/ReferenceOnly/")&&!n.Contains("/Resources/DesignScreens/")&&!n.Contains("/ProjectDocs/"),"Forbidden runtime reference dependency: "+dep);checks++;}} Debug.Log("v1.20 Step 9 Wave 3 self-test PASS. Checks="+checks+". Candidates=8. No active runtime cutover.");}
        static void Require(bool c,string m){if(!c)throw new InvalidOperationException("v1.20 Step 9 self-test: "+m);}
    }
}
#endif
