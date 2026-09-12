using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LegendsOfRus.CommercialCore.Analytics
{
    public enum AnalyticsValueKind
    {
        String = 0,
        Integer,
        Boolean
    }

    /// <summary>
    /// Immutable typed value. Factory methods deliberately do not normalize
    /// input; non-canonical values are rejected by AnalyticsEventValidator.
    /// </summary>
    public sealed class AnalyticsAttribute
    {
        public string Key { get; }
        public AnalyticsValueKind Kind { get; }
        public string StringValue { get; }
        public long IntegerValue { get; }
        public bool BooleanValue { get; }

        AnalyticsAttribute(
            string key,
            AnalyticsValueKind kind,
            string stringValue,
            long integerValue,
            bool booleanValue
        )
        {
            Key = key;
            Kind = kind;
            StringValue = stringValue;
            IntegerValue = integerValue;
            BooleanValue = booleanValue;
        }

        public static AnalyticsAttribute String(string key, string value)
        {
            return new AnalyticsAttribute(
                key,
                AnalyticsValueKind.String,
                value,
                0L,
                false
            );
        }

        public static AnalyticsAttribute Integer(string key, long value)
        {
            return new AnalyticsAttribute(
                key,
                AnalyticsValueKind.Integer,
                null,
                value,
                false
            );
        }

        public static AnalyticsAttribute Boolean(string key, bool value)
        {
            return new AnalyticsAttribute(
                key,
                AnalyticsValueKind.Boolean,
                null,
                0L,
                value
            );
        }

        internal AnalyticsAttribute DetachedCopy()
        {
            return new AnalyticsAttribute(
                Key,
                Kind,
                StringValue,
                IntegerValue,
                BooleanValue
            );
        }
    }

    /// <summary>
    /// Immutable caller intent. It has no timestamp, sequence, session, or
    /// transport state; the pipeline adds those only after validation.
    /// </summary>
    public sealed class AnalyticsEventDraft
    {
        readonly AnalyticsAttribute[] attributes;
        readonly ReadOnlyCollection<AnalyticsAttribute> readOnlyAttributes;

        public string EventId { get; }
        public IReadOnlyList<AnalyticsAttribute> Attributes => readOnlyAttributes;

        public AnalyticsEventDraft(
            string eventId,
            params AnalyticsAttribute[] attributes
        )
        {
            EventId = eventId;
            this.attributes = Copy(attributes);
            readOnlyAttributes = Array.AsReadOnly(this.attributes);
        }

        internal AnalyticsAttribute[] DetachedAttributes()
        {
            return Copy(attributes);
        }

        static AnalyticsAttribute[] Copy(AnalyticsAttribute[] source)
        {
            if (source == null || source.Length == 0)
                return new AnalyticsAttribute[0];

            var copy = new AnalyticsAttribute[source.Length];
            for (int i = 0; i < source.Length; i++)
                copy[i] = source[i]?.DetachedCopy();
            return copy;
        }
    }

    /// <summary>
    /// Immutable, validated record accepted by an analytics adapter.
    /// </summary>
    public sealed class AnalyticsEventEnvelope
    {
        readonly AnalyticsAttribute[] attributes;
        readonly ReadOnlyCollection<AnalyticsAttribute> readOnlyAttributes;

        public int SchemaVersion { get; }
        public string EventId { get; }
        public string SessionId { get; }
        public long Sequence { get; }
        public long UtcTicks { get; }
        public IReadOnlyList<AnalyticsAttribute> Attributes => readOnlyAttributes;

        internal AnalyticsEventEnvelope(
            int schemaVersion,
            string eventId,
            string sessionId,
            long sequence,
            long utcTicks,
            AnalyticsAttribute[] attributes
        )
        {
            SchemaVersion = schemaVersion;
            EventId = eventId;
            SessionId = sessionId;
            Sequence = sequence;
            UtcTicks = utcTicks;
            this.attributes = Copy(attributes);
            readOnlyAttributes = Array.AsReadOnly(this.attributes);
        }

        public AnalyticsEventEnvelope DetachedCopy()
        {
            return new AnalyticsEventEnvelope(
                SchemaVersion,
                EventId,
                SessionId,
                Sequence,
                UtcTicks,
                attributes
            );
        }

        internal AnalyticsAttribute[] DetachedAttributes()
        {
            return Copy(attributes);
        }

        static AnalyticsAttribute[] Copy(AnalyticsAttribute[] source)
        {
            if (source == null || source.Length == 0)
                return new AnalyticsAttribute[0];

            var copy = new AnalyticsAttribute[source.Length];
            for (int i = 0; i < source.Length; i++)
                copy[i] = source[i]?.DetachedCopy();
            return copy;
        }
    }

    public sealed class AnalyticsRecordResult
    {
        public bool Accepted { get; }
        public string ReasonCode { get; }

        AnalyticsRecordResult(bool accepted, string reasonCode)
        {
            Accepted = accepted;
            ReasonCode = reasonCode ?? AnalyticsReasonCodes.AdapterFailure;
        }

        public static AnalyticsRecordResult Success()
        {
            return new AnalyticsRecordResult(true, AnalyticsReasonCodes.Accepted);
        }

        public static AnalyticsRecordResult Reject(string reasonCode)
        {
            return new AnalyticsRecordResult(false, reasonCode);
        }
    }
}
