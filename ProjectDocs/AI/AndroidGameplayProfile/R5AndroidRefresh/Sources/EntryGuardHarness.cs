// Actual profile/factory source compiled with fake JNI, content and preferences.
using System;
using System.Collections.Generic;
using System.Linq;
using LegendsOfRus;

namespace UnityEngine
{
    public enum RuntimeInitializeLoadType { SubsystemRegistration }
    public sealed class RuntimeInitializeOnLoadMethodAttribute : Attribute { public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType _) {} }
    public static class Debug { public static void Log(string _) {} }
    public static class PlayerPrefs
    {
        public static Dictionary<string,string> Data = new Dictionary<string,string>();
        public static int Writes; public static bool ThrowOwner;
        public static bool HasKey(string key) => Data.ContainsKey(key);
        public static string GetString(string key,string fallback) => HasKey(key) ? Data[key] : fallback;
        public static void SetString(string key,string value) { Check(key); if (ThrowOwner && key.EndsWith("Owner")) throw new Exception("owner failure"); Data[key]=value; }
        public static void DeleteKey(string key) { Check(key); Data.Remove(key); }
        public static void Save() {}
        static void Check(string key) { if (!key.StartsWith(DevelopmentGameplayProfile.Prefix)) throw new Exception("normal write"); Writes++; }
    }
    public class AndroidJavaObject : IDisposable
    {
        public static string Command,Package="com.example.legendsofrus.v117debug";
        public static int Flags=2,Calls; public static bool Throw;
        public T Call<T>(string name,params object[] args)
        {
            Calls++; if (Throw) throw new Exception("JNI failure"); object value;
            switch(name) { case "getStringExtra":value=Command;break; case "getPackageName":value=Package;break;
                case "getIntent":case "getApplicationInfo":value=new AndroidJavaObject();break; default:throw new Exception(name); }
            return (T)value;
        }
        public void Call(string name,params object[] args) { Calls++; if(name!="removeExtra")throw new Exception(name); Command=null; }
        public T Get<T>(string name) { Calls++;return (T)(object)Flags; }
        public void Dispose() {}
    }
    public sealed class AndroidJavaClass:AndroidJavaObject { public AndroidJavaClass(string _) {} public T GetStatic<T>(string _) { Calls++;return (T)(object)new AndroidJavaObject(); } }
}
namespace LegendsOfRus
{
    public sealed class PlayerSaveData
    {
        public long lastEnergyUtcTicks; public int energy=5,gold=12540,gems=350,boosterMace=7,boosterFirebird=5,boosterLivingLight=8,boosterSeaPearl=10;
        public int stars,lastCompletedChapter,lastCompletedLevel,lastRewardGold,lastRewardGems,storyStage,selectedLevel=1,highestUnlockedLevel=1,level=1;
        public bool lastRewardEligibleForDouble;
        public List<LevelProgressData> levelProgress=new List<LevelProgressData>();
        public List<string> completedStoryScenes=new List<string>(); public List<int> acknowledgedChapterCompletions=new List<int>();
    }
    public sealed class LevelProgressData { public int chapterId,levelId,bestScore,bestStars,attempts,completions; public bool rewardClaimed; }
    public sealed class LevelRewardData { public int gold,gems,boosterMace,boosterFirebird,boosterLivingLight,boosterSeaPearl; }
    public sealed class LevelData { public int chapterId=1,levelId;public LevelRewardData rewards=new LevelRewardData();public int EvaluateStars(int _) => 3;public bool HasStoryTrigger(string _) => true; }
    public sealed class ChapterLevelCatalogData { public int chapterId=1;public LevelData[] levels; }
    public sealed class StorySceneData { public string id,trigger;public object[] lines=new object[]{new object()}; }
    public sealed class StoryMilestoneData { public int afterLevelId;public string sceneId; }
    public sealed class Completion { public int finalLevelId=500;public string finaleSceneId="scene57"; }
    public sealed class ChapterStoryCatalogData { public int chapterId=1;public string introSceneId="intro";public StorySceneData[] scenes;public StoryMilestoneData[] milestones;public Completion completion=new Completion(); }
    public static class ActiveCampaignContent
    {
        public static int Calls;public static bool Fail,Invalid;
        public static FakeCatalog Load() { Calls++;if(Fail)throw new Exception("content read failed");return new FakeCatalog(); }
    }
    public sealed class FakeCatalog
    {
        public ChapterLevelCatalogData LoadChapter(int _) => new ChapterLevelCatalogData { levels=Enumerable.Range(1,ActiveCampaignContent.Invalid?1:500).Select(i=>new LevelData{levelId=i}).ToArray() };
        public ChapterStoryCatalogData LoadStory(int _) => new ChapterStoryCatalogData {
            scenes=new[]{new StorySceneData{id="intro",trigger="intro"}}.Concat(Enumerable.Range(1,57).Select(i=>new StorySceneData{id="scene"+i,trigger="trigger"+i})).ToArray(),
            milestones=Enumerable.Range(1,57).Select(i=>new StoryMilestoneData{afterLevelId=i==57?500:i,sceneId="scene"+i}).ToArray() };
    }
    public sealed class WriteResult { public bool Success; }
    public static class SaveService
    {
        public const string BackupKey="LegendsOfRus_Save_v1_Backup",StagingKey="LegendsOfRus_Save_v1_Staging";
        public static int Calls;public static bool Fail;public static PlayerSaveData Candidate;
        public static bool TryReplaceHistory(PlayerSaveData data,out WriteResult result)
        {
            Calls++;Candidate=data;
            var key=DevelopmentGameplayProfile.PreferenceKey("LegendsOfRus_Save_v1");if(!key.StartsWith(DevelopmentGameplayProfile.Prefix))throw new Exception("normal selected");
            result=new WriteResult{Success=!Fail};if(!Fail)UnityEngine.PlayerPrefs.SetString(key,"synthetic-QA");return !Fail;
        }
    }
    public static class LanguagePreference { public const string Key="LegendsOfRus_Language_v1"; }
    public static class PlayerPrefsComplianceStateStore { public const string Key="LegendsOfRus_Compliance_v1"; }
}
public static class EntryGuardHarness
{
    static void Require(bool condition,string reason) { if(!condition)throw new Exception(reason); }
    public static int Main(string[] args)
    {
        string scenario=args[0],command="fresh"; bool blocked=false,mutatingFailure=false;
        string marker=DevelopmentGameplayProfile.Prefix+"Owner";
        UnityEngine.PlayerPrefs.Data["normal"]="sentinel";
        switch(scenario)
        {
            case "normal":command=null;break; case "fresh":break; case "empty-energy":command=scenario;break;
            case "resume":command=scenario;UnityEngine.PlayerPrefs.Data[marker]="disposable-gameplay-v1";break;
            case "unknown":command="stock-1001";blocked=true;break;
            case "missing-owner":command="resume";blocked=true;break;
            case "wrong-owner":command="resume";UnityEngine.PlayerPrefs.Data[marker]="other";blocked=true;break;
            case "wrong-package":UnityEngine.AndroidJavaObject.Package="wrong";blocked=true;break;
            case "not-debuggable":UnityEngine.AndroidJavaObject.Flags=0;blocked=true;break;
            case "jni-failure":UnityEngine.AndroidJavaObject.Throw=true;blocked=true;break;
            case "write-failure":SaveService.Fail=true;blocked=mutatingFailure=true;break;
            case "fresh-foreign":UnityEngine.PlayerPrefs.Data[marker]="other";break;
            default:
                var prefixes=new[]{"owned-","foreign-","emptyowner-","preparefail-","contentfail-","commitfail-","markerfail-"};
                string prefix=prefixes.FirstOrDefault(p=>scenario.StartsWith(p));command=prefix==null?scenario:scenario.Substring(prefix.Length);
                Require(new[]{"stock-999","stock-1000","stock-9999","stock-max","story-early","story-finale"}.Contains(command),"unknown scenario");
                if(prefix=="owned-")UnityEngine.PlayerPrefs.Data[marker]="disposable-gameplay-v1";
                if(prefix=="foreign-"){UnityEngine.PlayerPrefs.Data[marker]="other";blocked=true;}
                if(prefix=="emptyowner-"){UnityEngine.PlayerPrefs.Data[marker]="";blocked=true;}
                if(prefix=="preparefail-"){ActiveCampaignContent.Fail=true;blocked=true;}
                if(prefix=="contentfail-"){ActiveCampaignContent.Invalid=true;blocked=true;}
                if(prefix=="commitfail-"){SaveService.Fail=true;blocked=mutatingFailure=true;}
                if(prefix=="markerfail-"){UnityEngine.PlayerPrefs.ThrowOwner=true;blocked=mutatingFailure=true;}
                break;
        }
        UnityEngine.AndroidJavaObject.Command=command;
#if !DEVELOPMENT_BUILD
        Require(!DevelopmentGameplayProfile.Active,"release active");
        Require(UnityEngine.AndroidJavaObject.Calls==0&&UnityEngine.PlayerPrefs.Writes==0&&SaveService.Calls==0&&ActiveCampaignContent.Calls==0,"release side effect");
        Require(typeof(EntryGuardHarness).Assembly.GetType("LegendsOfRus.DevelopmentGameplayFixtures")==null,"release factory included");
        Require(typeof(EntryGuardHarness).Assembly.GetType("LegendsOfRus.DevelopmentGameplayProfileBadge")==null,"release badge included");
#else
        if(blocked)
        {
            bool first=false,second=false;
            try{var active=DevelopmentGameplayProfile.Active;}catch{first=true;}
            UnityEngine.AndroidJavaObject.Command=null;
            try{DevelopmentGameplayProfile.PreferenceKey("normal");}catch{second=true;}
            Require(first&&second,"failed initialization selected normal");
            if(!mutatingFailure)Require(UnityEngine.PlayerPrefs.Writes==0&&SaveService.Calls==0,"rejection wrote data");
            if(mutatingFailure)Require(!UnityEngine.PlayerPrefs.HasKey(marker),"owner published after failed initialization");
        }
        else
        {
            bool expected=scenario!="normal";
            Require(DevelopmentGameplayProfile.Active==expected,"activation");UnityEngine.AndroidJavaObject.Command=expected?null:"fresh";
            Require(DevelopmentGameplayProfile.Active==expected,"mid-process change");
            Require(SaveService.Calls==(!expected||scenario=="resume"?0:1),"commit count");
            if(expected&&scenario!="resume")Require(UnityEngine.PlayerPrefs.GetString(marker,"")=="disposable-gameplay-v1","owner absent");
            if(command=="empty-energy")Require(SaveService.Candidate.energy==0,"energy");
            if(command!=null&&command.StartsWith("stock-"))Require(SaveService.Candidate.boosterSeaPearl==(command=="stock-max"?int.MaxValue:int.Parse(command.Substring(6))),"stock");
        }
#endif
        Require(UnityEngine.PlayerPrefs.Data["normal"]=="sentinel","normal fingerprint");
        Console.WriteLine("PASS "+scenario);return 0;
    }
}
