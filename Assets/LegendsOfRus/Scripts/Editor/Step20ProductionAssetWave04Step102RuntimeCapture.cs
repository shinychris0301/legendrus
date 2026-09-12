#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    [InitializeOnLoad]
    public static class Step20ProductionAssetWave04Step102RuntimeCapture
    {
        public const string OutputRoot="ProjectDocs/V1_20_STEP10_2/RuntimeEvidence";
        const string RequestedKey="LegendsOfRus.Step102.Wave04.RuntimeCaptureRequested";const string HarnessName="__LegendsOfRus_Step102_Wave04_RuntimeCaptureHarness";
        static Step20ProductionAssetWave04Step102RuntimeCapture(){EditorApplication.playModeStateChanged-=OnPlayModeChanged;EditorApplication.playModeStateChanged+=OnPlayModeChanged;EditorApplication.update-=Poll;EditorApplication.update+=Poll;}
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 10.2 Wave 4 Unity Runtime Four-Aspect Capture")]
        public static void StartCapture(){if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before starting Step 10.2 runtime capture.");if(Step20ProductionAssetWave04PrefabBuilder.NeedsBuild())Step20ProductionAssetWave04PrefabBuilder.BuildAll(false);var failures=Step20ProductionAssetWave04PrefabBuilder.ValidateAll();failures.AddRange(Step20ProductionAssetWave04Step102BoundaryGate.Validate(false));if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));string abs=AbsoluteOutputRoot();PreserveReadmeThenClearEvidence(abs);Directory.CreateDirectory(abs);SessionState.SetBool(RequestedKey,true);Debug.Log("v1.20 Step 10.2: entering Play Mode for 28 passive Wave 4 runtime captures. No settings, identity, network, tutorial-completion, provider, or economy action will be invoked.");EditorApplication.EnterPlaymode();}
        static void PreserveReadmeThenClearEvidence(string abs){if(!Directory.Exists(abs))return;foreach(string file in Directory.GetFiles(abs)){if(string.Equals(Path.GetFileName(file),"README.md",StringComparison.OrdinalIgnoreCase))continue;File.Delete(file);}foreach(string dir in Directory.GetDirectories(abs))Directory.Delete(dir,true);}
        static void OnPlayModeChanged(PlayModeStateChange state){if(!SessionState.GetBool(RequestedKey,false))return;if(state==PlayModeStateChange.EnteredPlayMode){if(GameObject.Find(HarnessName)==null){GameObject go=new GameObject(HarnessName);Step102Wave04RuntimeCaptureHarness h=go.AddComponent<Step102Wave04RuntimeCaptureHarness>();h.Begin(AbsoluteOutputRoot());}}else if(state==PlayModeStateChange.EnteredEditMode){string pass=Path.Combine(AbsoluteOutputRoot(),"UNITY_RUNTIME_CAPTURE_EXECUTED.json");string fail=Path.Combine(AbsoluteOutputRoot(),"UNITY_RUNTIME_CAPTURE_FAILED.txt");SessionState.SetBool(RequestedKey,false);if(File.Exists(pass)){Debug.Log("v1.20 Step 10.2 runtime capture evidence created: 28/28. Next run Account/System Boundary Verification, visual report, then Final 36-Page Reconstruction Audit.");if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Step 10.2 runtime capture completed: 28/28. Next run the boundary verification and final 36-page audit.","OK");}else if(File.Exists(fail))Debug.LogError("v1.20 Step 10.2 runtime capture failed. See "+fail);}}
        static void Poll(){if(!SessionState.GetBool(RequestedKey,false)||!EditorApplication.isPlaying)return;string abs=AbsoluteOutputRoot();if(File.Exists(Path.Combine(abs,"UNITY_RUNTIME_CAPTURE_EXECUTED.json"))||File.Exists(Path.Combine(abs,"UNITY_RUNTIME_CAPTURE_FAILED.txt")))EditorApplication.ExitPlaymode();}
        public static string AbsoluteOutputRoot(){return Path.Combine(Directory.GetCurrentDirectory(),OutputRoot.Replace('/',Path.DirectorySeparatorChar));}
    }
}
#endif
