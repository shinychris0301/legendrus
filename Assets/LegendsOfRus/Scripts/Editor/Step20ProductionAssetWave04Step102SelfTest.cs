#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave04Step102SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 10.2 Wave 4 Runtime QA Readiness Self-Test")]
        public static void Run(){if(Step20ProductionAssetWave04PrefabBuilder.NeedsBuild())Step20ProductionAssetWave04PrefabBuilder.BuildAll(false);var f=new List<string>();f.AddRange(Step20ProductionAssetWave04PrefabBuilder.ValidateAll());f.AddRange(Step20ProductionAssetWave04Step102BoundaryGate.Validate(false));string assetRoot=Step20ProductionAssetWave04PrefabBuilder.AssetRoot;int pngs=Directory.Exists(assetRoot)?Directory.GetFiles(assetRoot,"*.png",SearchOption.AllDirectories).Length:0;if(pngs!=67)f.Add("Expected exactly 67 Wave 4 PNG candidates, found "+pngs+".");string coverage="Assets/LegendsOfRus/Documentation/V1_20_STEP10_WAVE04_REQUIREMENT_COVERAGE.json";if(!File.Exists(coverage)||File.ReadAllText(coverage).IndexOf("\"covered\": 41",StringComparison.Ordinal)<0||File.ReadAllText(coverage).IndexOf("\"total\": 41",StringComparison.Ordinal)<0)f.Add("Wave 4 requirement coverage must remain 41/41.");ScreenId[] pages={ScreenId.Settings,ScreenId.LanguageSelect,ScreenId.Login,ScreenId.TutorialOverlay,ScreenId.OfflineState,ScreenId.MaintenanceState,ScreenId.EmptyErrorState};foreach(ScreenId id in pages){string p=Step20ProductionAssetWave04PrefabBuilder.CandidateRoot+"/"+id+".prefab";GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(p);if(prefab==null){f.Add("Missing Wave 4 candidate: "+id);continue;}if(prefab.GetComponentsInChildren<Button>(true).Length!=0)f.Add(id+": Button authority detected.");if(prefab.GetComponentsInChildren<EventTrigger>(true).Length!=0)f.Add(id+": EventTrigger authority detected.");}if(f.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,f.ToArray()));Debug.Log("v1.20 Step 10.2 source readiness PASS: 7 candidates, 67 PNGs, 41/41 requirements, frozen account/system authority, 28 Unity runtime captures pending, approved Wave 4 references=0/7.");if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Step 10.2 source readiness PASS. Next run Account/System Boundary Verification and the 28-capture runtime QA.","OK");}
    }
}
#endif
