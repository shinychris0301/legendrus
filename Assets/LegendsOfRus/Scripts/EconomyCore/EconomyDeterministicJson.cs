using System;
using System.Globalization;
using System.Text;

namespace LegendsOfRus.EconomyCore
{
    public static class EconomyDeterministicJson
    {
        public static string Serialize(EconomyCatalogData value)
        {
            var json = new StringBuilder(4096);
            WriteCatalog(json, value);
            return json.ToString();
        }

        public static string Serialize(EconomyValidationReport value)
        {
            var json = new StringBuilder(2048);
            WriteValidation(json, value);
            return json.ToString();
        }

        public static string Serialize(EconomySimulationReport value)
        {
            var json = new StringBuilder(4096);
            WriteSimulation(json, value);
            return json.ToString();
        }

        public static string Serialize(EconomyComparisonReport value)
        {
            var json = new StringBuilder(2048);
            WriteComparison(json, value);
            return json.ToString();
        }

        public static string Serialize(EconomyLedgerSnapshot value)
        {
            var json = new StringBuilder(1024);
            WriteLedger(json, value);
            return json.ToString();
        }

        static void WriteCatalog(StringBuilder json, EconomyCatalogData value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('{');
            Property(json, "schemaVersion", value.schemaVersion); Comma(json);
            Property(json, "catalogId", value.catalogId); Comma(json);
            Property(json, "proposalOnly", value.proposalOnly); Comma(json);
            Property(json, "authoritative", value.authoritative); Comma(json);
            Name(json, "currencies"); WriteCurrencies(json, value.currencies); Comma(json);
            Name(json, "startingBalances"); WriteAmounts(json, value.startingBalances); Comma(json);
            Name(json, "energy"); WriteEnergy(json, value.energy); Comma(json);
            Name(json, "boosters"); WriteBoosters(json, value.boosters); Comma(json);
            Name(json, "continueRule"); WriteContinue(json, value.continueRule); Comma(json);
            Name(json, "dailyReward"); WriteDaily(json, value.dailyReward); Comma(json);
            Name(json, "rewardDoubling"); WriteDoubling(json, value.rewardDoubling); Comma(json);
            Name(json, "progression"); WriteProgression(json, value.progression);
            json.Append('}');
        }

        static void WriteCurrencies(StringBuilder json, EconomyCurrencyData[] values)
        {
            EconomyCurrencyData[] ordered = values == null
                ? new EconomyCurrencyData[0]
                : (EconomyCurrencyData[])values.Clone();
            Array.Sort(ordered, CompareCurrencies);
            json.Append('[');
            for (int i = 0; i < ordered.Length; i++)
            {
                if (i > 0) Comma(json);
                if (ordered[i] == null) { json.Append("null"); continue; }
                json.Append('{'); Property(json, "id", ordered[i].id); json.Append('}');
            }
            json.Append(']');
        }

        static void WriteAmounts(StringBuilder json, EconomyAmountData[] values)
        {
            EconomyAmountData[] ordered = values == null
                ? new EconomyAmountData[0]
                : (EconomyAmountData[])values.Clone();
            Array.Sort(ordered, CompareAmounts);
            json.Append('[');
            for (int i = 0; i < ordered.Length; i++)
            {
                if (i > 0) Comma(json);
                WriteAmount(json, ordered[i]);
            }
            json.Append(']');
        }

        static void WriteAmount(StringBuilder json, EconomyAmountData value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('{'); Property(json, "id", value.id); Comma(json); Property(json, "amount", value.amount); json.Append('}');
        }

        static void WriteEnergy(StringBuilder json, EconomyEnergyData value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('{');
            Property(json, "capacity", value.capacity); Comma(json);
            Property(json, "startingAmount", value.startingAmount); Comma(json);
            Property(json, "levelEntryCost", value.levelEntryCost); Comma(json);
            Property(json, "retryCost", value.retryCost); Comma(json);
            Property(json, "refillIntervalMinutes", value.refillIntervalMinutes);
            json.Append('}');
        }

        static void WriteBoosters(StringBuilder json, EconomyBoosterData[] values)
        {
            EconomyBoosterData[] ordered = values == null
                ? new EconomyBoosterData[0]
                : (EconomyBoosterData[])values.Clone();
            Array.Sort(ordered, CompareBoosters);
            json.Append('[');
            for (int i = 0; i < ordered.Length; i++)
            {
                if (i > 0) Comma(json);
                EconomyBoosterData value = ordered[i];
                if (value == null) { json.Append("null"); continue; }
                json.Append('{');
                Property(json, "id", value.id); Comma(json);
                Property(json, "startingQuantity", value.startingQuantity); Comma(json);
                Property(json, "purchaseEnabled", value.purchaseEnabled); Comma(json);
                Property(json, "priceDefined", value.priceDefined); Comma(json);
                Name(json, "price"); WriteAmount(json, value.price);
                json.Append('}');
            }
            json.Append(']');
        }

        static void WriteContinue(StringBuilder json, EconomyContinueData value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('{');
            Property(json, "enabled", value.enabled); Comma(json);
            Property(json, "priceDefined", value.priceDefined); Comma(json);
            Name(json, "price"); WriteAmount(json, value.price); Comma(json);
            Property(json, "grantedMoves", value.grantedMoves); Comma(json);
            Property(json, "maxPerAttempt", value.maxPerAttempt); Comma(json);
            Property(json, "freeExitAndRetry", value.freeExitAndRetry);
            json.Append('}');
        }

        static void WriteDaily(StringBuilder json, EconomyDailyRewardData value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('{');
            Property(json, "enabled", value.enabled); Comma(json);
            Property(json, "cadenceUtcDays", value.cadenceUtcDays); Comma(json);
            Name(json, "grants"); WriteAmounts(json, value.grants); Comma(json);
            Property(json, "energyGrant", value.energyGrant);
            json.Append('}');
        }

        static void WriteDoubling(StringBuilder json, EconomyRewardDoublingData value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('{');
            Property(json, "enabled", value.enabled); Comma(json);
            Property(json, "eligibleCurrencyId", value.eligibleCurrencyId); Comma(json);
            Property(json, "multiplier", value.multiplier); Comma(json);
            Property(json, "maxClaimsPerReward", value.maxClaimsPerReward); Comma(json);
            Property(json, "optional", value.optional);
            json.Append('}');
        }

        static void WriteProgression(StringBuilder json, EconomyProgressionData value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('{');
            Property(json, "chapterId", value.chapterId); Comma(json);
            Property(json, "normalProgressRequiresPurchase", value.normalProgressRequiresPurchase); Comma(json);
            Name(json, "levels"); WriteLevels(json, value.levels);
            json.Append('}');
        }

        static void WriteLevels(StringBuilder json, EconomyProgressionLevelData[] values)
        {
            EconomyProgressionLevelData[] ordered = values == null
                ? new EconomyProgressionLevelData[0]
                : (EconomyProgressionLevelData[])values.Clone();
            Array.Sort(ordered, CompareLevels);
            json.Append('[');
            for (int i = 0; i < ordered.Length; i++)
            {
                if (i > 0) Comma(json);
                EconomyProgressionLevelData value = ordered[i];
                if (value == null) { json.Append("null"); continue; }
                json.Append('{');
                Property(json, "id", value.id); Comma(json);
                Property(json, "levelNumber", value.levelNumber); Comma(json);
                Property(json, "requiresStore", value.requiresStore); Comma(json);
                Property(json, "requiresAd", value.requiresAd); Comma(json);
                Property(json, "requiresPurchase", value.requiresPurchase); Comma(json);
                Property(json, "requiredBoosterId", value.requiredBoosterId); Comma(json);
                Property(json, "requiredBoosterQuantity", value.requiredBoosterQuantity); Comma(json);
                Name(json, "firstClearGrants"); WriteAmounts(json, value.firstClearGrants); Comma(json);
                Name(json, "firstClearBoosterGrants"); WriteBoosterAmounts(json, value.firstClearBoosterGrants);
                json.Append('}');
            }
            json.Append(']');
        }

        static void WriteBoosterAmounts(StringBuilder json, EconomyBoosterAmountData[] values)
        {
            EconomyBoosterAmountData[] ordered = values == null
                ? new EconomyBoosterAmountData[0]
                : (EconomyBoosterAmountData[])values.Clone();
            Array.Sort(ordered, CompareBoosterAmounts);
            json.Append('[');
            for (int i = 0; i < ordered.Length; i++)
            {
                if (i > 0) Comma(json);
                EconomyBoosterAmountData value = ordered[i];
                if (value == null) { json.Append("null"); continue; }
                json.Append('{'); Property(json, "id", value.id); Comma(json); Property(json, "amount", value.amount); json.Append('}');
            }
            json.Append(']');
        }

        static void WriteValidation(StringBuilder json, EconomyValidationReport value)
        {
            if (value == null) { json.Append("null"); return; }
            EconomyViolationData[] ordered = value.violations == null
                ? new EconomyViolationData[0]
                : (EconomyViolationData[])value.violations.Clone();
            Array.Sort(ordered, CompareViolations);
            json.Append('{'); Property(json, "verdict", value.verdict.ToString()); Comma(json); Name(json, "violations"); json.Append('[');
            for (int i = 0; i < ordered.Length; i++)
            {
                if (i > 0) Comma(json);
                EconomyViolationData item = ordered[i];
                if (item == null) { json.Append("null"); continue; }
                json.Append('{');
                Property(json, "code", item.code); Comma(json);
                Property(json, "severity", item.severity.ToString()); Comma(json);
                Property(json, "path", item.path); Comma(json);
                Property(json, "message", item.message);
                json.Append('}');
            }
            json.Append(']'); json.Append('}');
        }

        static void WriteLedger(StringBuilder json, EconomyLedgerSnapshot value)
        {
            if (value == null) { json.Append("null"); return; }
            string[] keys = value.appliedKeys == null ? new string[0] : (string[])value.appliedKeys.Clone();
            Array.Sort(keys, StringComparer.Ordinal);
            json.Append('{'); Name(json, "balances"); WriteAmounts(json, value.balances); Comma(json); Name(json, "appliedKeys"); WriteStrings(json, keys); json.Append('}');
        }

        static void WriteSimulation(StringBuilder json, EconomySimulationReport value)
        {
            if (value == null) { json.Append("null"); return; }
            EconomyCheckData[] checks = value.checks == null ? new EconomyCheckData[0] : (EconomyCheckData[])value.checks.Clone();
            EconomyMetricData[] metrics = value.metrics == null ? new EconomyMetricData[0] : (EconomyMetricData[])value.metrics.Clone();
            Array.Sort(checks, CompareChecks); Array.Sort(metrics, CompareMetrics);
            json.Append('{');
            Property(json, "catalogId", value.catalogId); Comma(json);
            Property(json, "scenarioId", value.scenarioId); Comma(json);
            Property(json, "verdict", value.verdict.ToString()); Comma(json);
            Name(json, "validation"); WriteValidation(json, value.validation); Comma(json);
            Name(json, "checks"); WriteChecks(json, checks); Comma(json);
            Name(json, "metrics"); WriteMetrics(json, metrics); Comma(json);
            Name(json, "endingWallet"); WriteLedger(json, value.endingWallet); Comma(json);
            Name(json, "endingBoosters"); WriteBoosterAmounts(json, value.endingBoosters); Comma(json);
            Property(json, "endingEnergy", value.endingEnergy); Comma(json);
            Property(json, "elapsedMinutes", value.elapsedMinutes); Comma(json);
            Name(json, "trace"); WriteStrings(json, value.trace);
            json.Append('}');
        }

        static void WriteChecks(StringBuilder json, EconomyCheckData[] values)
        {
            json.Append('[');
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0) Comma(json);
                EconomyCheckData item = values[i];
                if (item == null) { json.Append("null"); continue; }
                json.Append('{'); Property(json, "id", item.id); Comma(json); Property(json, "verdict", item.verdict.ToString()); Comma(json); Property(json, "code", item.code); Comma(json); Property(json, "message", item.message); json.Append('}');
            }
            json.Append(']');
        }

        static void WriteMetrics(StringBuilder json, EconomyMetricData[] values)
        {
            json.Append('[');
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0) Comma(json);
                EconomyMetricData item = values[i];
                if (item == null) { json.Append("null"); continue; }
                json.Append('{'); Property(json, "id", item.id); Comma(json); Property(json, "value", item.value); Comma(json); Property(json, "unit", item.unit); json.Append('}');
            }
            json.Append(']');
        }

        static void WriteComparison(StringBuilder json, EconomyComparisonReport value)
        {
            if (value == null) { json.Append("null"); return; }
            EconomyComparisonEntryData[] entries = value.entries == null ? new EconomyComparisonEntryData[0] : (EconomyComparisonEntryData[])value.entries.Clone();
            Array.Sort(entries, CompareComparisonEntries);
            json.Append('{'); Property(json, "catalogId", value.catalogId); Comma(json); Property(json, "verdict", value.verdict.ToString()); Comma(json); Name(json, "entries"); json.Append('[');
            for (int i = 0; i < entries.Length; i++)
            {
                if (i > 0) Comma(json);
                EconomyComparisonEntryData item = entries[i];
                if (item == null) { json.Append("null"); continue; }
                json.Append('{'); Property(json, "path", item.path); Comma(json); Property(json, "status", item.status.ToString()); Comma(json); Property(json, "runtimeValue", item.runtimeValue); Comma(json); Property(json, "proposalValue", item.proposalValue); Comma(json); Property(json, "source", item.source); json.Append('}');
            }
            json.Append(']'); json.Append('}');
        }

        static void WriteStrings(StringBuilder json, string[] values)
        {
            if (values == null) values = new string[0];
            json.Append('[');
            for (int i = 0; i < values.Length; i++) { if (i > 0) Comma(json); String(json, values[i]); }
            json.Append(']');
        }

        static void Property(StringBuilder json, string name, string value) { Name(json, name); String(json, value); }
        static void Property(StringBuilder json, string name, long value) { Name(json, name); json.Append(value.ToString(CultureInfo.InvariantCulture)); }
        static void Property(StringBuilder json, string name, bool value) { Name(json, name); json.Append(value ? "true" : "false"); }
        static void Name(StringBuilder json, string value) { String(json, value); json.Append(':'); }
        static void Comma(StringBuilder json) { json.Append(','); }

        static void String(StringBuilder json, string value)
        {
            if (value == null) { json.Append("null"); return; }
            json.Append('"');
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case '"': json.Append("\\\""); break;
                    case '\\': json.Append("\\\\"); break;
                    case '\b': json.Append("\\b"); break;
                    case '\f': json.Append("\\f"); break;
                    case '\n': json.Append("\\n"); break;
                    case '\r': json.Append("\\r"); break;
                    case '\t': json.Append("\\t"); break;
                    default:
                        if (c < 32) json.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else json.Append(c);
                        break;
                }
            }
            json.Append('"');
        }

        static int CompareCurrencies(EconomyCurrencyData left, EconomyCurrencyData right) { return CompareNullableIds(left == null ? null : left.id, right == null ? null : right.id); }
        static int CompareBoosters(EconomyBoosterData left, EconomyBoosterData right) { return CompareNullableIds(left == null ? null : left.id, right == null ? null : right.id); }
        static int CompareAmounts(EconomyAmountData left, EconomyAmountData right)
        {
            int id = CompareNullableIds(left == null ? null : left.id, right == null ? null : right.id);
            if (id != 0 || left == null || right == null) return id;
            return left.amount.CompareTo(right.amount);
        }
        static int CompareBoosterAmounts(EconomyBoosterAmountData left, EconomyBoosterAmountData right)
        {
            int id = CompareNullableIds(left == null ? null : left.id, right == null ? null : right.id);
            if (id != 0 || left == null || right == null) return id;
            return left.amount.CompareTo(right.amount);
        }
        static int CompareLevels(EconomyProgressionLevelData left, EconomyProgressionLevelData right)
        {
            if (ReferenceEquals(left, right)) return 0;
            if (left == null) return -1;
            if (right == null) return 1;
            int number = left.levelNumber.CompareTo(right.levelNumber);
            return number != 0 ? number : string.Compare(left.id, right.id, StringComparison.Ordinal);
        }
        static int CompareViolations(EconomyViolationData left, EconomyViolationData right)
        {
            if (ReferenceEquals(left, right)) return 0; if (left == null) return -1; if (right == null) return 1;
            int path = string.Compare(left.path, right.path, StringComparison.Ordinal); return path != 0 ? path : string.Compare(left.code, right.code, StringComparison.Ordinal);
        }
        static int CompareChecks(EconomyCheckData left, EconomyCheckData right) { return CompareNullableIds(left == null ? null : left.id, right == null ? null : right.id); }
        static int CompareMetrics(EconomyMetricData left, EconomyMetricData right) { return CompareNullableIds(left == null ? null : left.id, right == null ? null : right.id); }
        static int CompareComparisonEntries(EconomyComparisonEntryData left, EconomyComparisonEntryData right)
        {
            if (ReferenceEquals(left, right)) return 0; if (left == null) return -1; if (right == null) return 1;
            int path = string.Compare(left.path, right.path, StringComparison.Ordinal); return path != 0 ? path : string.Compare(left.source, right.source, StringComparison.Ordinal);
        }
        static int CompareNullableIds(string left, string right)
        {
            if (ReferenceEquals(left, right)) return 0;
            if (left == null) return -1;
            if (right == null) return 1;
            return string.Compare(left, right, StringComparison.Ordinal);
        }
    }
}
