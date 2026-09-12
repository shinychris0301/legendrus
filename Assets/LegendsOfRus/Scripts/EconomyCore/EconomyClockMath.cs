using System;
using System.Globalization;

namespace LegendsOfRus.EconomyCore
{
    public static class EconomyEnergyMath
    {
        public static EconomyEnergyResult Advance(
            EconomyEnergyData config,
            long currentEnergy,
            long lastRefillTick,
            long nowTick,
            long ticksPerMinute
        )
        {
            var result = new EconomyEnergyResult
            {
                valid = false,
                code = EconomyViolationCodes.EnergyInvalid,
                energy = currentEnergy,
                lastRefillTick = lastRefillTick
            };
            if (
                config == null ||
                config.capacity <= 0 ||
                config.refillIntervalMinutes <= 0 ||
                ticksPerMinute <= 0 ||
                currentEnergy < 0 ||
                currentEnergy > config.capacity
            )
            {
                return result;
            }

            if (nowTick < lastRefillTick)
            {
                result.valid = true;
                result.code = EconomyViolationCodes.ClockRollback;
                result.clockRollback = true;
                result.minutesUntilNext = config.refillIntervalMinutes;
                return result;
            }

            try
            {
                long intervalTicks = checked(config.refillIntervalMinutes * ticksPerMinute);
                if (currentEnergy >= config.capacity)
                {
                    result.valid = true;
                    result.code = string.Empty;
                    result.energy = config.capacity;
                    result.lastRefillTick = nowTick;
                    result.minutesUntilNext = 0;
                    return result;
                }

                long elapsedTicks = checked(nowTick - lastRefillTick);
                long available = elapsedTicks / intervalTicks;
                long room = config.capacity - currentEnergy;
                long gained = available < room ? available : room;
                result.valid = true;
                result.code = string.Empty;
                result.gained = gained;
                result.energy = currentEnergy + gained;
                if (result.energy >= config.capacity)
                {
                    result.lastRefillTick = nowTick;
                    result.minutesUntilNext = 0;
                }
                else
                {
                    result.lastRefillTick = checked(lastRefillTick + checked(gained * intervalTicks));
                    long remainderTicks = checked(nowTick - result.lastRefillTick);
                    long leftTicks = checked(intervalTicks - remainderTicks);
                    result.minutesUntilNext = DivideRoundUp(leftTicks, ticksPerMinute);
                }
                return result;
            }
            catch (OverflowException)
            {
                result.code = EconomyViolationCodes.Overflow;
                return result;
            }
        }

        static long DivideRoundUp(long value, long divisor)
        {
            if (value <= 0) return 0;
            return 1 + ((value - 1) / divisor);
        }
    }

    public static class EconomyRewardCadence
    {
        public static EconomyDailyClaimResult TryClaimDaily(
            EconomyDailyRewardData config,
            long currentUtcDayIndex,
            long lastClaimUtcDayIndex,
            long currentEnergy,
            long energyCapacity,
            EconomyLedger ledger
        )
        {
            var result = new EconomyDailyClaimResult
            {
                claimDayIndex = lastClaimUtcDayIndex,
                energy = currentEnergy,
                wallet = ledger == null ? new EconomyLedgerSnapshot() : ledger.Snapshot()
            };
            if (
                config == null ||
                ledger == null ||
                !config.enabled ||
                config.cadenceUtcDays <= 0 ||
                config.energyGrant < 0 ||
                currentUtcDayIndex < 0 ||
                lastClaimUtcDayIndex < -1 ||
                currentEnergy < 0 ||
                energyCapacity <= 0 ||
                currentEnergy > energyCapacity
            )
            {
                result.code = EconomyViolationCodes.InvalidTransaction;
                return result;
            }
            if (lastClaimUtcDayIndex != -1 && currentUtcDayIndex < lastClaimUtcDayIndex)
            {
                result.code = EconomyViolationCodes.ClockRollback;
                return result;
            }
            try
            {
                if (
                    lastClaimUtcDayIndex != -1 &&
                    checked(currentUtcDayIndex - lastClaimUtcDayIndex) < config.cadenceUtcDays
                )
                {
                    result.code = EconomyViolationCodes.DailyAlreadyClaimed;
                    return result;
                }
                long added = checked(currentEnergy + config.energyGrant);
                long nextEnergy = added > energyCapacity ? energyCapacity : added;
                EconomyTransactionResult grant = ledger.TryGrant(
                    "daily:" + currentUtcDayIndex.ToString(CultureInfo.InvariantCulture),
                    config.grants ?? new EconomyAmountData[0]
                );
                if (!grant.applied)
                {
                    result.code = grant.code;
                    result.wallet = grant.snapshot;
                    return result;
                }
                result.energy = nextEnergy;
                result.claimed = true;
                result.code = string.Empty;
                result.claimDayIndex = currentUtcDayIndex;
                result.wallet = grant.snapshot;
                return result;
            }
            catch (OverflowException)
            {
                result.code = EconomyViolationCodes.Overflow;
                return result;
            }
        }
    }
}
