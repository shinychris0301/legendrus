using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using LegendsOfRus.GameplayCore;
namespace LegendsOfRus
{
    // Opt-in development observer. Never writes a campaign/player journal or drives gameplay.
    public sealed class CampaignPlaytestRecorder : MonoBehaviour
    {
        public MechanicsPlaytestLog Log {get;private set;}
        public string CheckpointPath=>storage?.CheckpointPath;
        public string SaveStatus {get;private set;}
        public int EndRound {get;private set;}
        public int StartRound {get;private set;}
        public string ContentVersion {get;private set;}
        public static string DefaultDirectory=>Path.Combine(Application.persistentDataPath,"CampaignPlaytests");
        MechanicsPlaytestStorage storage;
        CampaignSaveSession session;
        Match3Board board;
        int[] startInventory;
        bool focused=true,paused;
        float nextSave;
        string settledHash;
        public static CampaignPlaytestRecorder Attach(CampaignDevelopmentUI ui,CampaignSaveSession session,string contentFingerprint,string directory=null,bool automated=false,int endRound=0,int startRound=11)
        {
            if(ui==null||session==null||string.IsNullOrWhiteSpace(contentFingerprint))throw new ArgumentException("Explicit preview, session and content fingerprint required.");
            if(ui.PlaytestRecorder!=null)throw new InvalidOperationException("Preview is already recording.");
            if(endRound<0 || startRound<1 || (endRound>0 && startRound>endRound))throw new ArgumentOutOfRangeException(nameof(endRound));
            var r=ui.gameObject.AddComponent<CampaignPlaytestRecorder>();r.session=session;r.EndRound=endRound;r.StartRound=startRound;r.ContentVersion=contentFingerprint;
            r.Log=new MechanicsPlaytestLog(automated||Application.isBatchMode,Application.version,contentFingerprint,true);
            r.storage=new MechanicsPlaytestStorage(r.Log,directory??DefaultDirectory);ui.PlaytestRecorder=r;r.SaveNow();return r;
        }
        public static string Fingerprint(string value)
        {using(var hash=SHA256.Create())return BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-","").ToLowerInvariant();}
        public void UseSession(CampaignSaveSession reopened){session=reopened??throw new ArgumentNullException(nameof(reopened));}
        int[] Inventory()
        {
            if(!session.TryLoad(out var save,out _))return null;
            var p=save.player;return new[]{p.boosterMace,p.boosterFirebird,p.boosterLivingLight,p.boosterSeaPearl};
        }
        public void Begin(LevelData level,int attempt,long run,Match3Board current)
        {
            Interrupt("replaced");board=current;startInventory=Inventory();settledHash=null;
            Log.Begin(level.levelId,level.displayName,(int)current.RuleVersion,current.Moves,current.BoardStateHash,Remaining());
            var a=Log.Current;a.chapter=level.chapterId;a.committedAttempt=attempt;a.campaignRun=run;
            a.levelFingerprint=Fingerprint(JsonUtility.ToJson(level));a.inventoryBoostersUsed=startInventory==null?-1:0;SaveNow();
        }
        int[] Remaining()=>board?Enumerable.Range(0,board.GoalCount).Select(i=>board.GetGoalSlot(i).Remaining).ToArray():Array.Empty<int>();
        void Observe()
        {
            if(!board||Log?.Current==null||Log.Current.outcome!="ongoing")return;
            Log.Observe(board.Moves,board.Score,board.Phase.ToString(),board.BoardStateHash,Remaining(),!board.HasActiveCommand);
            var now=Inventory();Log.Current.inventoryBoostersUsed=now==null||startInventory==null?-1:Enumerable.Range(0,4).Sum(i=>Math.Max(0,startInventory[i]-now[i]));
        }
        public void Complete(CampaignBoardResult result)
        {
            if(Log?.Current==null||Log.Current.outcome!="ongoing")return;
            Observe();Log.Finish(result.Won?"won":"lost","board_result");SaveNow();board=null;
        }
        public void Interrupt(string reason)
        {
            if(Log?.Current==null||Log.Current.outcome!="ongoing")return;
            Observe();Log.Finish("interrupted",reason);SaveNow();board=null;
        }
        void Update()
        {
            if(!board||Log?.Current==null||Log.Current.outcome!="ongoing")return;
            Log.Tick(Time.unscaledDeltaTime,focused&&!paused&&board.Phase!=GameplayPhase.Paused);
            if(!board.HasActiveCommand&&settledHash!=board.LastPlanFinalStateHash)
            {settledHash=board.LastPlanFinalStateHash;Observe();SaveNow();}
            else if(Time.realtimeSinceStartup>=nextSave){Observe();SaveNow();}
        }
        public bool SaveNow()
        {
            if(storage==null)return false;nextSave=Time.realtimeSinceStartup+2;
            try{storage.Save();SaveStatus="Saved locally";return true;}
            catch(Exception e) when(e is IOException||e is UnauthorizedAccessException||e is ArgumentException||e is NotSupportedException)
            {SaveStatus="Local save failed. Retry Save; gameplay continues.";return false;}
        }
        public void Rate(string field,string value){Log.Rate(field,value);SaveNow();}
        void OnApplicationFocus(bool value){focused=value;if(!value){Observe();SaveNow();}}
        void OnApplicationPause(bool value){paused=value;if(value){Observe();SaveNow();}}
        void OnApplicationQuit()=>Interrupt("application_quit");
        void OnDisable()=>Interrupt("preview_closed");
        void OnDestroy()=>Interrupt("destroyed");
    }
}
