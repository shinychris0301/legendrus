using System;
using System.Globalization;
namespace LegendsOfRus
{
    public readonly struct DailyRewardGrant
    {
        public readonly int Gold, Gems, Energy, Mace, Firebird, LivingLight, SeaPearl;
        public DailyRewardGrant(int gold=0,int gems=0,int energy=0,int mace=0,int firebird=0,int livingLight=0,int seaPearl=0)
        {Gold=gold;Gems=gems;Energy=energy;Mace=mace;Firebird=firebird;LivingLight=livingLight;SeaPearl=seaPearl;}
    }
    public static class DailyRewardSchedule
    {
        public static DailyRewardGrant Grant(int day)
        {
            switch(day)
            {
                case 1:return new DailyRewardGrant(gold:500);
                case 2:return new DailyRewardGrant(gems:20);
                case 3:return new DailyRewardGrant(energy:1);
                case 4:return new DailyRewardGrant(livingLight:1);
                case 5:return new DailyRewardGrant(seaPearl:1);
                case 6:return new DailyRewardGrant(mace:2);
                case 7:return new DailyRewardGrant(gold:1000,gems:40,firebird:1);
                default:throw new ArgumentOutOfRangeException(nameof(day));
            }
        }
        static bool Last(PlayerSaveData s,out DateTime date)=>DateTime.TryParseExact(s.lastDailyRewardUtc,"yyyyMMdd",CultureInfo.InvariantCulture,DateTimeStyles.None,out date);
        public static bool CanClaim(PlayerSaveData s,DateTime utc)=>s!=null&&(!Last(s,out var last)||last.Date<utc.Date);
        public static int Day(PlayerSaveData s,DateTime utc)
        {
            if(s==null||!Last(s,out var last))return 1;
            int previous=Math.Max(1,Math.Min(7,s.dailyRewardCycleDay)); // Legacy save: existing claim is Day 1.
            if(last.Date>=utc.Date)return previous;
            return last.Date==utc.Date.AddDays(-1)?previous%7+1:1;
        }
        public static int MonthClaims(PlayerSaveData s,DateTime utc)=>s!=null&&s.dailyRewardMonthUtc==utc.ToString("yyyyMM",CultureInfo.InvariantCulture)?Math.Max(0,s.dailyRewardMonthClaims):0;
        public static bool TryApply(PlayerSaveData s,DateTime utc)
        {
            if(!CanClaim(s,utc))return false;
            int day=Day(s,utc);var grant=Grant(day);
            // Calculate first so overflow cannot leave a partially granted reward.
            int gold=checked(s.gold+grant.Gold),gems=checked(s.gems+grant.Gems);
            int mace=checked(s.boosterMace+grant.Mace),firebird=checked(s.boosterFirebird+grant.Firebird);
            int light=checked(s.boosterLivingLight+grant.LivingLight),pearl=checked(s.boosterSeaPearl+grant.SeaPearl);
            int count=checked(MonthClaims(s,utc)+1);
            s.gold=gold;s.gems=gems;s.boosterMace=mace;s.boosterFirebird=firebird;s.boosterLivingLight=light;s.boosterSeaPearl=pearl;
            // Preserve the existing energy cap; never reduce an over-cap balance on non-energy days.
            if(grant.Energy>0&&s.energy<s.energyMax)s.energy+=Math.Min(grant.Energy,s.energyMax-s.energy);
            s.dailyRewardCycleDay=day;s.lastDailyRewardUtc=utc.ToString("yyyyMMdd",CultureInfo.InvariantCulture);
            s.dailyRewardMonthUtc=utc.ToString("yyyyMM",CultureInfo.InvariantCulture);s.dailyRewardMonthClaims=count;
            return true;
        }
    }
}
