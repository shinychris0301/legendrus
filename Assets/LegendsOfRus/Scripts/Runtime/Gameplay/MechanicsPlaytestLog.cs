using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace LegendsOfRus
{
    [Serializable] public sealed class MechanicsAttempt
    {
        public int number,round,ruleVersion,moveBudget,movesRemaining,score;
        public string title,startedUtc,finishedUtc,outcome="ongoing",endReason="",openingHash,observedHash,phase;
        public double activeSeconds;
        public bool snapshotSettled;
        public string difficulty="unrated",goalClarity="unrated",effectClarity="unrated";
        public int[] goalsRemaining;
        // Optional campaign context; absent/default in historical sandbox sessions.
        public int chapter,committedAttempt,inventoryBoostersUsed;
        public long campaignRun;
        public string levelFingerprint,specialClarity="unrated";
    }
    [Serializable] public sealed class MechanicsPlaytestSession
    {
        public int schemaVersion=1,droppedAttempts;
        public string sessionId,source,applicationVersion,contentVersion="MechanicsRoundsR4-RulesR4-PresentationR25",createdUtc;
        public int campaignStartRound, campaignEndRound, fixtureCompletedThrough;
        public string collectionContext;
        public List<MechanicsAttempt> attempts=new List<MechanicsAttempt>();
        public MechanicsAttempt[] activeAttempts=Array.Empty<MechanicsAttempt>();
    }
    // Local development data. No GameState, PlayerPrefs or network dependency.
    public sealed class MechanicsPlaytestLog
    {
        public const int Capacity=200;
        readonly MechanicsPlaytestSession session;
        public MechanicsAttempt Current {get;private set;}
        bool finished;
        int sequence;
        public int CompletedCount=>session.attempts.Count;
        public string SessionId=>session.sessionId;
        public string Source=>session.source;
        public MechanicsPlaytestLog(bool automated,string version,string contentVersion=null,bool campaign=false)
        {
            session=new MechanicsPlaytestSession{sessionId=Guid.NewGuid().ToString("N"),
                source=campaign?(automated?"automated_campaign":"manual_campaign"):(automated?"automated_test":"manual_sandbox"),
                applicationVersion=version,createdUtc=DateTime.UtcNow.ToString("O")};
            if(!string.IsNullOrWhiteSpace(contentVersion))session.contentVersion=contentVersion;
        }
        public void SetCampaignContext(int start, int end, int fixtureThrough, string context)
        {
            session.campaignStartRound = start; session.campaignEndRound = end;
            session.fixtureCompletedThrough = fixtureThrough; session.collectionContext = context;
        }
        public void Begin(int round,string title,int rules,int moves,string hash,int[] remaining)
        {
            Finish("interrupted","replaced");
            Current=new MechanicsAttempt{number=++sequence,round=round,title=title,ruleVersion=rules,moveBudget=moves,movesRemaining=moves,openingHash=hash,observedHash=hash,phase="Idle",snapshotSettled=true,startedUtc=DateTime.UtcNow.ToString("O"),goalsRemaining=(int[])remaining.Clone()};
            finished=false;session.activeAttempts=new[]{Current};
        }
        public void Observe(int moves,int score,string phase,string hash,int[] goals,bool settled)
        {
            if(Current==null||finished)return;
            Current.movesRemaining=moves;Current.score=score;Current.phase=phase;Current.observedHash=hash;Current.goalsRemaining=(int[])goals.Clone();Current.snapshotSettled=settled;
        }
        public void Tick(double seconds,bool foreground)
        {if(Current!=null&&!finished&&foreground&&seconds>0&&!double.IsInfinity(seconds)&&!double.IsNaN(seconds))Current.activeSeconds+=seconds;}
        public void Finish(string outcome,string reason)
        {
            if(Current==null||finished)return;
            if(outcome!="won"&&outcome!="lost"&&outcome!="interrupted")throw new ArgumentException("Unknown attempt outcome.",nameof(outcome));
            Current.outcome=outcome;Current.endReason=reason;Current.finishedUtc=DateTime.UtcNow.ToString("O");finished=true;session.activeAttempts=Array.Empty<MechanicsAttempt>();
            session.attempts.Add(Current);if(session.attempts.Count>Capacity){session.attempts.RemoveAt(0);session.droppedAttempts++;}
        }
        public void Rate(string field,string value)
        {
            if(Current==null)return;
            bool Valid(params string[] values)=>Array.IndexOf(values,value)>=0;
            switch(field)
            {
                case "difficulty":if(!Valid("unrated","easy","fair","hard"))throw new ArgumentException(nameof(value));Current.difficulty=value;break;
                case "goals":if(!Valid("unrated","clear","unclear"))throw new ArgumentException(nameof(value));Current.goalClarity=value;break;
                case "specials":if(!Valid("unrated","clear","unclear"))throw new ArgumentException(nameof(value));Current.specialClarity=value;break;
                case "effects":if(!Valid("unrated","readable","busy"))throw new ArgumentException(nameof(value));Current.effectClarity=value;break;
                default:throw new ArgumentException(nameof(field));
            }
        }
        public string ToJson()=>JsonUtility.ToJson(session,true);
        public string Export(string directory)
        {
            Directory.CreateDirectory(directory);
            string path=Path.Combine(directory,"playtest-"+session.sessionId+"-"+Guid.NewGuid().ToString("N")+".json");string temp=path+".tmp";
            try{File.WriteAllText(temp,ToJson());File.Move(temp,path);return path;}
            finally{if(File.Exists(temp))File.Delete(temp);}
        }
    }
}
