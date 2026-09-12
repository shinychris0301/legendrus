#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave01Step72SelfTest
    {
        const string Milestone = "v1.20-step07.2-candidate-prefab-four-aspect-qa-cutover-decision";
        const string RegistrySha = "aee233191e2696acdf944fbb207cbfb6580ccc80a80941e960b13f09058a8938";
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 7.2 Candidate Prefab + Cutover Gate Self-Test")]
        public static void Run()
        {
            var fail=new List<string>(); int t=0,p=0;
            Check(ref t,ref p,fail,"milestone",Read("MILESTONE_VERSION.txt").Trim()==Milestone);
            Check(ref t,ref p,fail,"runtime registry byte frozen",Sha("Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRegistry.cs")==RegistrySha);
            Check(ref t,ref p,fail,"active registry still 10",ProductionCoreLoopPageRegistry.CutoverPageCount==10);
            string[] names={"SplashTitle","Loading","Home","WorldMap","ChapterMap","PreLevel","Gameplay","Pause","Victory","Fail","ChapterComplete","Dialogue"};
            foreach(string n in names) Check(ref t,ref p,fail,"serialized candidate "+n,File.Exists(P("Assets/LegendsOfRus/Resources/ProductionUI/Wave01Pages/"+n+".prefab")));
            var pf=Step20ProductionAssetWave01PrefabBuilder.ValidateAll(); Check(ref t,ref p,fail,"candidate dependency validation",pf.Count==0); if(pf.Count>0)fail.AddRange(pf);
            string d=Read("Assets/LegendsOfRus/Documentation/V1_20_STEP07_2_CUTOVER_DECISION.json");
            Check(ref t,ref p,fail,"cutover is explicitly NO-GO",d.Contains("\"decision\": \"NO_GO\""));
            Check(ref t,ref p,fail,"no cutover of Splash",!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.SplashTitle));
            Check(ref t,ref p,fail,"no cutover of Dialogue",!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Dialogue));
            Check(ref t,ref p,fail,"no full-screen reference dependency token",!DirectoryText("Assets/LegendsOfRus/Resources/ProductionUI/Wave01Pages","Art/ReferenceOnly")&&!DirectoryText("Assets/LegendsOfRus/Resources/ProductionUI/Wave01Pages","Resources/DesignScreens"));
            if(fail.Count>0){string s=string.Join("\n",fail.ToArray());Debug.LogError("Step 7.2 self-test failed "+p+"/"+t+"\n"+s);throw new InvalidOperationException(s);} Debug.Log("v1.20 Step 7.2 source self-test passed: "+p+"/"+t+". Cutover remains NO-GO by design.");
        }
        static void Check(ref int t,ref int p,List<string> f,string n,bool ok){t++;if(ok)p++;else f.Add("FAIL: "+n);} static string P(string r){return Path.Combine(Directory.GetCurrentDirectory(),r.Replace('/',Path.DirectorySeparatorChar));} static string Read(string r){return File.ReadAllText(P(r));}
        static string Sha(string r){using(var s=SHA256.Create())using(var f=File.OpenRead(P(r)))return BitConverter.ToString(s.ComputeHash(f)).Replace("-","").ToLowerInvariant();}
        static bool DirectoryText(string root,string token){foreach(string f in Directory.GetFiles(P(root),"*.prefab",SearchOption.AllDirectories))if(File.ReadAllText(f).Contains(token))return true;return false;}
    }
}
#endif
