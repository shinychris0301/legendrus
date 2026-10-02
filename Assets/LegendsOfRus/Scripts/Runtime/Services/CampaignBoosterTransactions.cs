using System;
namespace LegendsOfRus
{
    public enum CampaignBoosterState { None, Reserved, Applied, Refunded }
    [Serializable] public sealed class CampaignBoosterReceipt
    {
        public long sequence, runSequence;
        public BoosterType type;
        public CampaignBoosterState state;
    }
    public static class CampaignBoosterTransactions
    {
        public static int Count(PlayerSaveData p, BoosterType type)
        {
            switch(type) { case BoosterType.BogatyrMace:return p.boosterMace; case BoosterType.Firebird:return p.boosterFirebird;
                case BoosterType.LivingLight:return p.boosterLivingLight; case BoosterType.SeaPearl:return p.boosterSeaPearl; default:return 0; }
        }
        internal static bool Known(BoosterType type) => type == BoosterType.BogatyrMace || type == BoosterType.Firebird || type == BoosterType.LivingLight || type == BoosterType.SeaPearl;
        internal static void Add(PlayerSaveData p, BoosterType type, int delta)
        {
            checked { switch(type) { case BoosterType.BogatyrMace:p.boosterMace+=delta;break;case BoosterType.Firebird:p.boosterFirebird+=delta;break;
                case BoosterType.LivingLight:p.boosterLivingLight+=delta;break;case BoosterType.SeaPearl:p.boosterSeaPearl+=delta;break;default:throw new ArgumentException(); } }
        }
        internal static string Validate(CampaignPlayerSave save)
        {
            var b=save.booster;
            if(b==null || !Known(b.type) || b.sequence<0 || b.runSequence<0 || b.runSequence>save.runSequence || b.state<CampaignBoosterState.None || b.state>CampaignBoosterState.Refunded) return "campaign_booster_invalid";
            if(b.state==CampaignBoosterState.None) return b.sequence==0 && b.runSequence==0 ? null : "campaign_booster_invalid";
            if(b.sequence<1 || b.runSequence<1) return "campaign_booster_invalid";
            if(b.state==CampaignBoosterState.Reserved && (save.run.state!=CampaignRunState.Running || b.runSequence!=save.runSequence)) return "campaign_booster_orphaned";
            return null;
        }
    }
    public sealed partial class CampaignSaveSession
    {
        public CampaignSaveOperation ReserveBooster(long run, long expectedSequence, BoosterType type)
        {
            if(!TryLoad(out var save,out var error))return Blocked(error);
            var b=save.booster;
            if(run<=0 || run!=save.runSequence || !CampaignBoosterTransactions.Known(type) || expectedSequence<0 || expectedSequence==long.MaxValue)return Blocked("campaign_booster_invalid");
            if(b.sequence==expectedSequence+1) return b.runSequence==run && b.type==type ? NoChange("campaign_booster_already_reserved") : Blocked("campaign_booster_conflict");
            if(save.run.state!=CampaignRunState.Running || b.sequence!=expectedSequence || b.state==CampaignBoosterState.Reserved)return Blocked("campaign_booster_not_ready");
            if(CampaignBoosterTransactions.Count(save.player,type)<1)return Blocked("campaign_booster_empty");
            CampaignBoosterTransactions.Add(save.player,type,-1);
            save.booster=new CampaignBoosterReceipt{sequence=expectedSequence+1,runSequence=run,type=type,state=CampaignBoosterState.Reserved};
            return Commit(save);
        }
        public CampaignSaveOperation SettleBooster(long run,long sequence,bool applied)
        {
            if(!TryLoad(out var save,out var error))return Blocked(error);
            var b=save.booster;var target=applied?CampaignBoosterState.Applied:CampaignBoosterState.Refunded;
            if(sequence<=0 || b.sequence!=sequence || b.runSequence!=run || save.runSequence!=run)return Blocked("campaign_booster_stale");
            if(b.state==target)return NoChange("campaign_booster_already_settled");
            if(b.state!=CampaignBoosterState.Reserved || save.run.state!=CampaignRunState.Running)return Blocked("campaign_booster_conflict");
            try { if(!applied)CampaignBoosterTransactions.Add(save.player,b.type,1); }
            catch(OverflowException){return Blocked("campaign_booster_overflow");}
            b.state=target;return Commit(save);
        }
        // Only call after disposing the lost board; reserved effects were never committed to a round result.
        public CampaignSaveOperation RecoverReservedBooster()
        {
            if(!TryLoad(out var save,out var error))return Blocked(error);
            return save.booster.state==CampaignBoosterState.Reserved ? SettleBooster(save.runSequence,save.booster.sequence,false) : NoChange("campaign_booster_no_recovery");
        }
    }
    public interface ICampaignBoardInventory
    {
        bool Blocked { get; }
        bool Consume(BoosterType type);
        bool Complete();
        bool Refund();
    }
    public sealed class CampaignBoardInventory : ICampaignBoardInventory
    {
        readonly CampaignSaveSession session; readonly long run;
        long pending;
        public bool Blocked {get;private set;}
        public CampaignBoardInventory(CampaignSaveSession session,long run){this.session=session;this.run=run;}
        public bool Consume(BoosterType type)
        {
            if(Blocked || pending!=0)return false;
            if(!session.TryLoad(out var save,out _)){Blocked=true;return false;}
            var result=session.ReserveBooster(run,save.booster.sequence,type);
            if(!result.Committed){Blocked=!session.TryLoad(out _,out _);return false;}
            pending=save.booster.sequence+1;return true;
        }
        bool Settle(bool applied)
        {
            if(Blocked)return false;if(pending==0)return true;
            if(!session.SettleBooster(run,pending,applied).Committed){Blocked=true;return false;}
            pending=0;return true;
        }
        public bool Complete()=>Settle(true);
        public bool Refund()=>Settle(false);
    }
}
