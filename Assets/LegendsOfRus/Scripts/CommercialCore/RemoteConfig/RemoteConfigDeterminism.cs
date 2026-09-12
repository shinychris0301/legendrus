using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace LegendsOfRus.CommercialCore.RemoteConfig
{
    public static class RemoteConfigSourceHash
    {
        /// <summary>
        /// Removes one leading Unicode BOM and normalizes CRLF/CR to LF. It
        /// intentionally does not trim whitespace or add a terminal newline.
        /// </summary>
        public static string Normalize(string source)
        {
            string normalized = source ?? string.Empty;
            if (normalized.Length > 0 && normalized[0] == '\uFEFF')
                normalized = normalized.Substring(1);
            return normalized.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        public static string ComputeNormalizedSha256(string source)
        {
            return ComputeUtf8Sha256(Normalize(source));
        }

        internal static string ComputeUtf8Sha256(string value)
        {
            byte[] bytes = new UTF8Encoding(false).GetBytes(value ?? string.Empty);
            byte[] digest;
            using (SHA256 sha256 = SHA256.Create())
                digest = sha256.ComputeHash(bytes);

            var result = new StringBuilder(digest.Length * 2);
            for (int i = 0; i < digest.Length; i++)
                result.Append(digest[i].ToString("x2", CultureInfo.InvariantCulture));
            return result.ToString();
        }
    }

    public static class RemoteConfigDeterministicJson
    {
        public static string Serialize(RemoteConfigSnapshot snapshot)
        {
            if (snapshot == null)
                return "null";

            var json = new StringBuilder(2048);
            json.Append('{');
            Property(json, "schemaVersion", snapshot.SchemaVersion); Comma(json);
            Property(json, "snapshotId", snapshot.SnapshotId); Comma(json);
            Property(json, "revision", snapshot.Revision); Comma(json);
            Property(json, "environment", snapshot.Environment); Comma(json);
            Property(json, "proposalOnly", snapshot.ProposalOnly); Comma(json);
            Property(json, "authoritative", snapshot.IsAuthoritative); Comma(json);
            Property(json, "economyCatalogId", snapshot.EconomyCatalogId); Comma(json);
            Property(
                json,
                "economyCatalogSourceSha256",
                snapshot.EconomyCatalogSourceSha256
            );
            Comma(json);
            Name(json, "entries");
            WriteEntries(json, snapshot.DetachedValues());
            json.Append('}');
            return json.ToString();
        }

        public static string ComputeContentSha256(RemoteConfigSnapshot snapshot)
        {
            return RemoteConfigSourceHash.ComputeUtf8Sha256(Serialize(snapshot));
        }

        static void WriteEntries(
            StringBuilder json,
            IDictionary<string, RemoteConfigValue> values
        )
        {
            string[] keys;
            if (values == null || values.Count == 0)
            {
                keys = new string[0];
            }
            else
            {
                keys = new string[values.Count];
                values.Keys.CopyTo(keys, 0);
                Array.Sort(keys, StringComparer.Ordinal);
            }

            json.Append('[');
            for (int i = 0; i < keys.Length; i++)
            {
                if (i > 0)
                    Comma(json);
                RemoteConfigValue value = values[keys[i]];
                json.Append('{');
                Property(json, "key", keys[i]); Comma(json);
                Property(json, "type", TypeName(value.Kind)); Comma(json);
                Property(json, "value", CanonicalValue(value));
                json.Append('}');
            }
            json.Append(']');
        }

        static string TypeName(RemoteConfigValueKind kind)
        {
            switch (kind)
            {
                case RemoteConfigValueKind.Boolean:
                    return "bool";
                case RemoteConfigValueKind.Int64:
                    return "int64";
                case RemoteConfigValueKind.String:
                    return "string";
                default:
                    throw new InvalidOperationException("Unknown remote-config value kind.");
            }
        }

        static string CanonicalValue(RemoteConfigValue value)
        {
            switch (value.Kind)
            {
                case RemoteConfigValueKind.Boolean:
                    return value.BooleanValue ? "true" : "false";
                case RemoteConfigValueKind.Int64:
                    return value.Int64Value.ToString(CultureInfo.InvariantCulture);
                case RemoteConfigValueKind.String:
                    return value.StringValue;
                default:
                    throw new InvalidOperationException("Unknown remote-config value kind.");
            }
        }

        static void Name(StringBuilder json, string name)
        {
            String(json, name);
            json.Append(':');
        }

        static void Property(StringBuilder json, string name, string value)
        {
            Name(json, name);
            String(json, value ?? string.Empty);
        }

        static void Property(StringBuilder json, string name, int value)
        {
            Name(json, name);
            json.Append(value.ToString(CultureInfo.InvariantCulture));
        }

        static void Property(StringBuilder json, string name, bool value)
        {
            Name(json, name);
            json.Append(value ? "true" : "false");
        }

        static void String(StringBuilder json, string value)
        {
            json.Append('"');
            string text = value ?? string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                char current = text[i];
                switch (current)
                {
                    case '"': json.Append("\\\""); break;
                    case '\\': json.Append("\\\\"); break;
                    case '\b': json.Append("\\b"); break;
                    case '\f': json.Append("\\f"); break;
                    case '\n': json.Append("\\n"); break;
                    case '\r': json.Append("\\r"); break;
                    case '\t': json.Append("\\t"); break;
                    default:
                        if (current < 0x20)
                        {
                            json.Append("\\u");
                            json.Append(((int)current).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            json.Append(current);
                        }
                        break;
                }
            }
            json.Append('"');
        }

        static void Comma(StringBuilder json)
        {
            json.Append(',');
        }
    }
}
