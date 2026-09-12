#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave01SelfTest
    {
        const string Milestone = "v1.20-step07-production-asset-reconstruction-wave00-wave01";
        const string ExpectedRegistrySha = "aee233191e2696acdf944fbb207cbfb6580ccc80a80941e960b13f09058a8938";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 7 Wave 0/1 Asset Reconstruction Self-Test")]
        public static void Run()
        {
            var fail=new List<string>(); int total=0,pass=0;
            Check(ref total,ref pass,fail,"milestone",Read("MILESTONE_VERSION.txt").Trim()==Milestone);
            Check(ref total,ref pass,fail,"runtime registry byte frozen",Sha("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRegistry.cs")==ExpectedRegistrySha);
            Check(ref total,ref pass,fail,"active cutover still 10",ProductionCoreLoopPageRegistry.CutoverPageCount==10);
            Check(ref total,ref pass,fail,"Splash candidate only",!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.SplashTitle));
            Check(ref total,ref pass,fail,"Dialogue candidate only",!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Dialogue));
            Check(ref total,ref pass,fail,"Wave0 manifest",File.Exists(P("Assets/LegendsOfRus/Art/Production/UI/Wave0Foundation/wave0_foundation_manifest.json")));
            Check(ref total,ref pass,fail,"Wave1 manifest",File.Exists(P("Assets/LegendsOfRus/Documentation/V1_20_STEP07_WAVE01_ASSET_MANIFEST.json")));
            Check(ref total,ref pass,fail,"layout specs",File.Exists(P("Assets/LegendsOfRus/Documentation/V1_20_STEP07_WAVE01_LAYOUT_SPECS.json")));
            string[] keys={"SplashTitle","Loading","Home","WorldMap","LevelSelect","PreLevel","Gameplay","Pause","Victory","Fail","ChapterComplete","Dialogue"};
            foreach(string k in keys) Check(ref total,ref pass,fail,"page manifest "+k,File.Exists(P("Assets/LegendsOfRus/Art/Production/Pages/"+k+"/page_asset_manifest.json")));
            Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);
            List<string> pf=Step20ProductionAssetWave01PrefabBuilder.ValidateAll();
            Check(ref total,ref pass,fail,"12 candidate prefabs dependency-clean",pf.Count==0);
            if(pf.Count>0)fail.AddRange(pf);
            Check(ref total,ref pass,fail,"no AAB bundled",Directory.GetFiles(Directory.GetCurrentDirectory(),"*.aab",SearchOption.AllDirectories).Length==0);
            if(fail.Count>0){ string s=string.Join(Environment.NewLine,fail.ToArray()); Debug.LogError("v1.20 Step 7 Wave 0/1 self-test failed "+pass+"/"+total+Environment.NewLine+s); throw new InvalidOperationException(s); }
            Debug.Log("v1.20 Step 7 Wave 0/1 source + candidate-prefab self-test passed: "+pass+"/"+total+". Runtime cutover remains disabled pending Step 6 executable acceptance and Wave 1 visual/device signoff.");
        }
        static void Check(ref int t,ref int p,List<string> f,string n,bool ok){t++;if(ok)p++;else f.Add("FAIL: "+n);}
        static string P(string r){return Path.Combine(Directory.GetCurrentDirectory(),r.Replace('/',Path.DirectorySeparatorChar));}
        static string Read(string r){return File.ReadAllText(P(r));}
        static string Sha(string r){using(var s=SHA256.Create())using(var f=File.OpenRead(P(r)))return BitConverter.ToString(s.ComputeHash(f)).Replace("-","").ToLowerInvariant();}
    }
}
#endif
