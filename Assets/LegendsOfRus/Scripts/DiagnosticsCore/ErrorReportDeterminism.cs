using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace LegendsOfRus.DiagnosticsCore
{
    /// <summary>
    /// Canonical fingerprinting and JSON inspection. It returns strings only;
    /// it never writes, logs, persists, or transmits diagnostic data.
    /// </summary>
    public static class ErrorReportDeterminism
    {
        static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        const string Hex = "0123456789abcdef";

        public static bool TryComputeFingerprint(
            ErrorReportDraft draft,
            out string fingerprint
        )
        {
            fingerprint = string.Empty;
            ErrorReportOperationResult validation =
                ErrorReportValidator.ValidateDraft(draft);
            if (!validation.Accepted)
                return false;

            try
            {
                string canonical =
                    ErrorReportSchema.CurrentSchemaVersion.ToString(
                        CultureInfo.InvariantCulture
                    ) + "\n" +
                    Kind(draft.Kind) + "\n" +
                    Subsystem(draft.Subsystem) + "\n" +
                    ErrorCode(draft.ErrorCode) + "\n" +
                    SiteCode(draft.SiteCode);
                byte[] bytes = StrictUtf8.GetBytes(canonical);
                byte[] hash;
                using (SHA256 sha = SHA256.Create())
                    hash = sha.ComputeHash(bytes);
                var value = new char[hash.Length * 2];
                for (int i = 0; i < hash.Length; i++)
                {
                    value[i * 2] = Hex[hash[i] >> 4];
                    value[i * 2 + 1] = Hex[hash[i] & 0x0f];
                }
                fingerprint = new string(value);
                return true;
            }
            catch (Exception)
            {
                fingerprint = string.Empty;
                return false;
            }
        }

        public static string Serialize(ErrorReportRecord record)
        {
            if (record == null)
                return "null";
            var json = new StringBuilder(512);
            WriteRecord(json, record);
            return json.ToString();
        }

        public static string SerializeTrace(
            IReadOnlyList<ErrorReportRecord> records
        )
        {
            var ordered = new List<ErrorReportRecord>();
            if (records != null)
            {
                for (int i = 0; i < records.Count; i++)
                {
                    ErrorReportRecord record = records[i];
                    ordered.Add(record == null ? null : record.DetachedCopy());
                }
                ordered.Sort(CompareRecords);
            }
            var json = new StringBuilder(Math.Max(2, ordered.Count * 384));
            json.Append('[');
            for (int i = 0; i < ordered.Count; i++)
            {
                if (i > 0)
                    json.Append(',');
                WriteRecord(json, ordered[i]);
            }
            json.Append(']');
            return json.ToString();
        }

        static void WriteRecord(StringBuilder json, ErrorReportRecord record)
        {
            if (record == null)
            {
                json.Append("null");
                return;
            }

            json.Append('{');
            Name(json, "schemaVersion"); Number(json, record.SchemaVersion);
            Comma(json); Name(json, "sequence"); Number(json, record.Sequence);
            Comma(json); Name(json, "fingerprint"); String(json, record.Fingerprint);
            Comma(json); Name(json, "kind"); String(json, Kind(record.Kind));
            Comma(json); Name(json, "subsystem");
            String(json, Subsystem(record.Subsystem));
            Comma(json); Name(json, "errorCode");
            String(json, ErrorCode(record.ErrorCode));
            Comma(json); Name(json, "siteCode");
            String(json, SiteCode(record.SiteCode));
            Comma(json); Name(json, "firstUtcTicks");
            Number(json, record.FirstUtcTicks);
            Comma(json); Name(json, "lastUtcTicks");
            Number(json, record.LastUtcTicks);
            Comma(json); Name(json, "occurrenceCount");
            Number(json, record.OccurrenceCount);
            Comma(json); Name(json, "deliveryState");
            String(json, DeliveryState(record.DeliveryState));
            Comma(json); Name(json, "deliveryAttempts");
            Number(json, record.DeliveryAttempts);
            Comma(json); Name(json, "nextAttemptUtcTicks");
            Number(json, record.NextAttemptUtcTicks);
            json.Append('}');
        }

        internal static string Kind(ErrorReportKind kind)
        {
            switch (kind)
            {
                case ErrorReportKind.NonFatal: return "nonfatal";
                case ErrorReportKind.ManagedCrash: return "managed_crash";
                default: return "unknown";
            }
        }

        internal static string Subsystem(ErrorReportSubsystem subsystem)
        {
            switch (subsystem)
            {
                case ErrorReportSubsystem.Bootstrap: return "bootstrap";
                case ErrorReportSubsystem.Runtime: return "runtime";
                case ErrorReportSubsystem.Gameplay: return "gameplay";
                case ErrorReportSubsystem.Save: return "save";
                case ErrorReportSubsystem.Economy: return "economy";
                case ErrorReportSubsystem.Commercial: return "commercial";
                case ErrorReportSubsystem.Ui: return "ui";
                case ErrorReportSubsystem.Audio: return "audio";
                case ErrorReportSubsystem.Content: return "content";
                default: return "unknown";
            }
        }

        internal static string DeliveryState(ErrorReportDeliveryState state)
        {
            switch (state)
            {
                case ErrorReportDeliveryState.Pending: return "pending";
                case ErrorReportDeliveryState.RetryScheduled:
                    return "retry_scheduled";
                case ErrorReportDeliveryState.RetryExhausted:
                    return "retry_exhausted";
                case ErrorReportDeliveryState.PermanentlyRejected:
                    return "permanently_rejected";
                default: return "unknown";
            }
        }

        internal static string ErrorCode(ErrorReportCode code)
        {
            switch (code)
            {
                case ErrorReportCode.BootstrapFailure: return "BOOTSTRAP_FAILURE";
                case ErrorReportCode.RuntimeInvariantFailure:
                    return "RUNTIME_INVARIANT_FAILURE";
                case ErrorReportCode.ManagedCrashUnhandled:
                    return "MANAGED_CRASH_UNHANDLED";
                case ErrorReportCode.Match3PlanFailure:
                    return "MATCH3_PLAN_FAILURE";
                case ErrorReportCode.SaveReadFailure: return "SAVE_READ_FAILURE";
                case ErrorReportCode.SaveWriteFailure: return "SAVE_WRITE_FAILURE";
                case ErrorReportCode.SaveMigrationFailure:
                    return "SAVE_MIGRATION_FAILURE";
                case ErrorReportCode.EconomyTransactionFailure:
                    return "ECONOMY_TRANSACTION_FAILURE";
                case ErrorReportCode.CommercialAdapterFailure:
                    return "COMMERCIAL_ADAPTER_FAILURE";
                case ErrorReportCode.UiPresentationFailure:
                    return "UI_PRESENTATION_FAILURE";
                case ErrorReportCode.AudioPlaybackFailure:
                    return "AUDIO_PLAYBACK_FAILURE";
                case ErrorReportCode.ContentLoadFailure: return "CONTENT_LOAD_FAILURE";
                default: return "UNKNOWN";
            }
        }

        internal static string SiteCode(ErrorReportSiteCode site)
        {
            switch (site)
            {
                case ErrorReportSiteCode.BootstrapInitialize:
                    return "bootstrap.initialize";
                case ErrorReportSiteCode.RuntimeInvariantCheck:
                    return "runtime.invariant.check";
                case ErrorReportSiteCode.RuntimeUnhandled: return "runtime.unhandled";
                case ErrorReportSiteCode.Match3ResolvePlan:
                    return "match3.resolve.plan";
                case ErrorReportSiteCode.SaveReadDecode: return "save.read.decode";
                case ErrorReportSiteCode.SaveWriteCommit: return "save.write.commit";
                case ErrorReportSiteCode.SaveMigrationApply:
                    return "save.migration.apply";
                case ErrorReportSiteCode.EconomyTransactionCommit:
                    return "economy.transaction.commit";
                case ErrorReportSiteCode.CommercialAdapterInvoke:
                    return "commercial.adapter.invoke";
                case ErrorReportSiteCode.UiPresentationUpdate:
                    return "ui.presentation.update";
                case ErrorReportSiteCode.AudioPlaybackInvoke:
                    return "audio.playback.invoke";
                case ErrorReportSiteCode.ContentResourceLoad:
                    return "content.resource.load";
                default: return "unknown";
            }
        }

        static int CompareRecords(ErrorReportRecord left, ErrorReportRecord right)
        {
            if (ReferenceEquals(left, right))
                return 0;
            if (left == null)
                return 1;
            if (right == null)
                return -1;
            int order = left.Sequence.CompareTo(right.Sequence);
            if (order != 0)
                return order;
            order = string.CompareOrdinal(left.Fingerprint, right.Fingerprint);
            if (order != 0)
                return order;
            return string.CompareOrdinal(Serialize(left), Serialize(right));
        }

        static void Name(StringBuilder json, string value)
        {
            String(json, value);
            json.Append(':');
        }

        static void Number(StringBuilder json, long value)
        {
            json.Append(value.ToString(CultureInfo.InvariantCulture));
        }

        static void Comma(StringBuilder json)
        {
            json.Append(',');
        }

        static void String(StringBuilder json, string value)
        {
            if (value == null)
            {
                json.Append("null");
                return;
            }
            json.Append('"');
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                switch (character)
                {
                    case '"': json.Append("\\\""); break;
                    case '\\': json.Append("\\\\"); break;
                    case '\b': json.Append("\\b"); break;
                    case '\f': json.Append("\\f"); break;
                    case '\n': json.Append("\\n"); break;
                    case '\r': json.Append("\\r"); break;
                    case '\t': json.Append("\\t"); break;
                    default:
                        if (character < 0x20)
                        {
                            json.Append("\\u");
                            json.Append(
                                ((int)character).ToString(
                                    "x4",
                                    CultureInfo.InvariantCulture
                                )
                            );
                        }
                        else
                        {
                            json.Append(character);
                        }
                        break;
                }
            }
            json.Append('"');
        }
    }
}
