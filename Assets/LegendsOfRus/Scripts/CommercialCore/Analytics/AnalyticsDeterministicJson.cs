using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LegendsOfRus.CommercialCore.Analytics
{
    /// <summary>
    /// Pure canonical representation for deterministic tests and sandbox
    /// inspection. It returns strings only and never writes or transmits them.
    /// </summary>
    public static class AnalyticsDeterministicJson
    {
        public static string Serialize(AnalyticsEventEnvelope envelope)
        {
            if (envelope == null)
                return "null";

            var json = new StringBuilder(512);
            WriteEnvelope(json, envelope);
            return json.ToString();
        }

        public static string SerializeTrace(
            IReadOnlyList<AnalyticsEventEnvelope> envelopes
        )
        {
            var json = new StringBuilder(
                envelopes == null ? 2 : Math.Max(2, envelopes.Count * 256)
            );
            json.Append('[');
            if (envelopes != null)
            {
                for (int i = 0; i < envelopes.Count; i++)
                {
                    if (i > 0) json.Append(',');
                    WriteEnvelope(json, envelopes[i]);
                }
            }
            json.Append(']');
            return json.ToString();
        }

        static void WriteEnvelope(
            StringBuilder json,
            AnalyticsEventEnvelope envelope
        )
        {
            if (envelope == null)
            {
                json.Append("null");
                return;
            }

            json.Append('{');
            Name(json, "schemaVersion");
            Number(json, envelope.SchemaVersion);
            Comma(json);
            Name(json, "eventId");
            String(json, envelope.EventId);
            Comma(json);
            Name(json, "sessionId");
            String(json, envelope.SessionId);
            Comma(json);
            Name(json, "sequence");
            Number(json, envelope.Sequence);
            Comma(json);
            Name(json, "utcTicks");
            Number(json, envelope.UtcTicks);
            Comma(json);
            Name(json, "attributes");
            WriteAttributes(json, envelope.Attributes);
            json.Append('}');
        }

        static void WriteAttributes(
            StringBuilder json,
            IReadOnlyList<AnalyticsAttribute> attributes
        )
        {
            json.Append('[');
            if (attributes != null)
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (i > 0) Comma(json);
                    AnalyticsAttribute attribute = attributes[i];
                    if (attribute == null)
                    {
                        json.Append("null");
                        continue;
                    }

                    json.Append('{');
                    Name(json, "key");
                    String(json, attribute.Key);
                    Comma(json);
                    Name(json, "kind");
                    String(json, Kind(attribute.Kind));
                    Comma(json);
                    Name(json, "value");
                    WriteValue(json, attribute);
                    json.Append('}');
                }
            }
            json.Append(']');
        }

        static void WriteValue(StringBuilder json, AnalyticsAttribute attribute)
        {
            switch (attribute.Kind)
            {
                case AnalyticsValueKind.String:
                    String(json, attribute.StringValue);
                    break;
                case AnalyticsValueKind.Integer:
                    Number(json, attribute.IntegerValue);
                    break;
                case AnalyticsValueKind.Boolean:
                    json.Append(attribute.BooleanValue ? "true" : "false");
                    break;
                default:
                    json.Append("null");
                    break;
            }
        }

        static string Kind(AnalyticsValueKind kind)
        {
            switch (kind)
            {
                case AnalyticsValueKind.String:
                    return "string";
                case AnalyticsValueKind.Integer:
                    return "integer";
                case AnalyticsValueKind.Boolean:
                    return "boolean";
                default:
                    return "unknown";
            }
        }

        static void Name(StringBuilder json, string name)
        {
            String(json, name);
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
