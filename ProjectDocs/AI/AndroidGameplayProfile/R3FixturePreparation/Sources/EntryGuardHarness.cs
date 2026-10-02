// Standalone compilation of the actual profile source. Fake JNI/preferences only;
// this checks conditional entry logic, not Android JNI or real save durability.
using System;
using System.Collections.Generic;
using LegendsOfRus;
namespace UnityEngine
{
    public enum RuntimeInitializeLoadType { SubsystemRegistration }
    public sealed class RuntimeInitializeOnLoadMethodAttribute : Attribute
    { public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType _) {} }
    public static class Debug { public static void Log(string _) {} }
    public static class PlayerPrefs
    {
        public static readonly Dictionary<string,string> Data = new Dictionary<string,string>();
        public static int Writes;
        public static string GetString(string k,string fallback) => Data.ContainsKey(k)?Data[k]:fallback;
        public static void SetString(string k,string v) { Check(k);Data[k]=v; }
        public static void DeleteKey(string k) { Check(k);Data.Remove(k); }
        public static void Save() {}
        static void Check(string key)
        {
            if (!key.StartsWith(DevelopmentGameplayProfile.Prefix)) throw new Exception("normal key mutation");
            Writes++;
        }
    }
    public class AndroidJavaObject : IDisposable
    {
        public static string Command, Package = "com.example.legendsofrus.v117debug";
        public static int Flags = 2, Calls;
        public static bool Throw;
        public T Call<T>(string name, params object[] args)
        {
            Calls++;if(Throw)throw new Exception("simulated JNI unavailable");
            object value;
            switch(name)
            {
                case "getStringExtra": value=Command;break;
                case "getPackageName":value=Package;break;
                case "getIntent":case "getApplicationInfo":value=new AndroidJavaObject();break;
                default:throw new Exception("Unexpected JNI method");
            }
            return (T)value;
        }
        public void Call(string name,params object[] args)
        { Calls++;if(name!="removeExtra")throw new Exception("Unexpected void JNI method");Command=null; }
        public T Get<T>(string name) { Calls++;return (T)(object)Flags; }
        public void Dispose() {}
    }
    public sealed class AndroidJavaClass : AndroidJavaObject
    {
        public AndroidJavaClass(string _) {}
        public T GetStatic<T>(string _) { Calls++;return (T)(object)new AndroidJavaObject(); }
    }
}
namespace LegendsOfRus
{
    public sealed class PlayerSaveData { public long lastEnergyUtcTicks;public int energy=5; }
    public sealed class WriteResult { public bool Success=true; }
    public static class SaveService
    {
        public const string BackupKey="LegendsOfRus_Save_v1_Backup",StagingKey="LegendsOfRus_Save_v1_Staging";
        public static int Calls;public static int Energy;public static bool Fail;
        public static bool TryReplaceHistory(PlayerSaveData fixture,out WriteResult result)
        {
            Calls++; Energy=fixture.energy;
            if(!DevelopmentGameplayProfile.PreferenceKey("LegendsOfRus_Save_v1").StartsWith(DevelopmentGameplayProfile.Prefix))throw new Exception("normal save selected");
            result=new WriteResult { Success=!Fail };return !Fail;
        }
    }
    public static class LanguagePreference { public const string Key="LegendsOfRus_Language_v1"; }
    public static class PlayerPrefsComplianceStateStore { public const string Key="LegendsOfRus_Compliance_v1"; }
}
public static class EntryGuardHarness
{
    static void Require(bool condition,string code) { if(!condition)throw new Exception(code); }
    public static int Main(string[] args)
    {
        string scenario=args[0];bool blocked=false;
        UnityEngine.AndroidJavaObject.Command="fresh";
        switch(scenario)
        {
            case "normal":UnityEngine.AndroidJavaObject.Command=null;break;
            case "fresh":break;
            case "empty-energy":UnityEngine.AndroidJavaObject.Command="empty-energy";break;
            case "resume":UnityEngine.AndroidJavaObject.Command="resume";UnityEngine.PlayerPrefs.Data[DevelopmentGameplayProfile.Prefix+"Owner"]="disposable-gameplay-v1";break;
            case "stock-999": case "stock-1000": case "stock-9999": case "stock-max": case "story-early": case "story-finale":
                UnityEngine.AndroidJavaObject.Command=scenario;blocked=true;break;
            case "unknown":UnityEngine.AndroidJavaObject.Command="reset-all";blocked=true;break;
            case "missing-owner":UnityEngine.AndroidJavaObject.Command="resume";blocked=true;break;
            case "wrong-owner":UnityEngine.AndroidJavaObject.Command="resume";UnityEngine.PlayerPrefs.Data[DevelopmentGameplayProfile.Prefix+"Owner"]="other";blocked=true;break;
            case "wrong-package":UnityEngine.AndroidJavaObject.Package="com.example.other";blocked=true;break;
            case "not-debuggable":UnityEngine.AndroidJavaObject.Flags=0;blocked=true;break;
            case "jni-failure":UnityEngine.AndroidJavaObject.Throw=true;blocked=true;break;
            case "write-failure":SaveService.Fail=true;blocked=true;break;
            default:throw new Exception("Unknown harness scenario");
        }
#if !DEVELOPMENT_BUILD
        Require(!DevelopmentGameplayProfile.Active,"release active");
        Require(UnityEngine.AndroidJavaObject.Calls==0,"release JNI call");
        Require(UnityEngine.PlayerPrefs.Writes==0&&SaveService.Calls==0,"release write");
        Require(typeof(EntryGuardHarness).Assembly.GetType("LegendsOfRus.DevelopmentGameplayProfileBadge")==null,"release badge included");
#else
        if(blocked)
        {
            bool first=false,second=false;
            try { var active=DevelopmentGameplayProfile.Active; }catch { first=true; }
            UnityEngine.AndroidJavaObject.Command=null;
            try { DevelopmentGameplayProfile.PreferenceKey("normal"); }catch { second=true; }
            Require(first&&second,"failed initialization fell through to normal");
            if(scenario!="write-failure")Require(UnityEngine.PlayerPrefs.Writes==0&&SaveService.Calls==0,"refused request mutated data");
        }
        else
        {
            bool expected=scenario!="normal";
            Require(DevelopmentGameplayProfile.Active==expected,"activation mismatch");
            UnityEngine.AndroidJavaObject.Command=expected?null:"fresh";
            Require(DevelopmentGameplayProfile.Active==expected,"profile changed mid-process");
            Require(SaveService.Calls==((scenario=="fresh"||scenario=="empty-energy")?1:0),"fixture calls");
            if(scenario=="empty-energy")Require(SaveService.Energy==0,"energy fixture");
        }
#endif
        Console.WriteLine("PASS "+scenario);return 0;
    }
}
