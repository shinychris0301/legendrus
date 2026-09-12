using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LegendsOfRus.CommercialCore.RemoteConfig
{
    /// <summary>
    /// Small strict JSON parser for the fixed local remote-config schema. It
    /// deliberately rejects duplicate members and does not depend on a Unity
    /// serializer that could silently ignore unknown input.
    /// </summary>
    internal static class StrictRemoteConfigJson
    {
        public const int MaximumSourceCharacters = 65536;

        static readonly string[] RootFields =
        {
            "schemaVersion",
            "snapshotId",
            "revision",
            "environment",
            "proposalOnly",
            "authoritative",
            "economyCatalogId",
            "economyCatalogSourceSha256",
            "entries",
        };

        static readonly string[] EntryFields = { "key", "type", "value" };

        public static bool TryParse(
            string source,
            out RemoteConfigDocumentData document,
            out RemoteConfigViolation violation
        )
        {
            document = null;
            violation = null;
            if (string.IsNullOrEmpty(source))
            {
                violation = new RemoteConfigViolation(
                    RemoteConfigFailureCodes.SourceMissing,
                    "$",
                    "The local remote-config source is empty."
                );
                return false;
            }
            if (source.Length > MaximumSourceCharacters)
            {
                violation = new RemoteConfigViolation(
                    RemoteConfigFailureCodes.SourceTooLarge,
                    "$",
                    "The local remote-config source exceeds 65,536 characters."
                );
                return false;
            }

            string parseSource = source[0] == '\uFEFF'
                ? source.Substring(1)
                : source;
            var parser = new Parser(parseSource);
            JsonValue root;
            if (!parser.TryParse(out root, out violation))
                return false;

            return TryReadDocument(root, out document, out violation);
        }

        static bool TryReadDocument(
            JsonValue root,
            out RemoteConfigDocumentData document,
            out RemoteConfigViolation violation
        )
        {
            document = null;
            violation = null;
            if (root == null || root.Kind != JsonKind.Object)
            {
                violation = TypeViolation("$", "The root value must be an object.");
                return false;
            }
            if (!ValidateExactFields(root.ObjectValue, RootFields, "$", out violation))
                return false;

            var parsed = new RemoteConfigDocumentData();
            if (!ReadInt32(root, "schemaVersion", "$.schemaVersion", out parsed.SchemaVersion, out violation))
                return false;
            if (!ReadString(root, "snapshotId", "$.snapshotId", out parsed.SnapshotId, out violation))
                return false;
            if (!ReadInt32(root, "revision", "$.revision", out parsed.Revision, out violation))
                return false;
            if (!ReadString(root, "environment", "$.environment", out parsed.Environment, out violation))
                return false;
            if (!ReadBoolean(root, "proposalOnly", "$.proposalOnly", out parsed.ProposalOnly, out violation))
                return false;
            if (!ReadBoolean(root, "authoritative", "$.authoritative", out parsed.Authoritative, out violation))
                return false;
            if (!ReadString(root, "economyCatalogId", "$.economyCatalogId", out parsed.EconomyCatalogId, out violation))
                return false;
            if (!ReadString(
                root,
                "economyCatalogSourceSha256",
                "$.economyCatalogSourceSha256",
                out parsed.EconomyCatalogSourceSha256,
                out violation
            ))
            {
                return false;
            }

            JsonValue entriesValue = root.ObjectValue["entries"];
            if (entriesValue.Kind != JsonKind.Array)
            {
                violation = TypeViolation("$.entries", "The entries value must be an array.");
                return false;
            }
            var entries = new RemoteConfigEntryData[entriesValue.ArrayValue.Count];
            for (int i = 0; i < entries.Length; i++)
            {
                string path = "$.entries[" + i.ToString(CultureInfo.InvariantCulture) + "]";
                JsonValue entryValue = entriesValue.ArrayValue[i];
                if (entryValue == null || entryValue.Kind != JsonKind.Object)
                {
                    violation = TypeViolation(path, "Every entry must be an object.");
                    return false;
                }
                if (!ValidateExactFields(
                    entryValue.ObjectValue,
                    EntryFields,
                    path,
                    out violation
                ))
                {
                    return false;
                }

                var entry = new RemoteConfigEntryData();
                if (!ReadString(entryValue, "key", path + ".key", out entry.Key, out violation))
                    return false;
                if (!ReadString(entryValue, "type", path + ".type", out entry.Type, out violation))
                    return false;
                if (!ReadString(entryValue, "value", path + ".value", out entry.Value, out violation))
                    return false;
                entries[i] = entry;
            }

            parsed.Entries = entries;
            document = parsed;
            return true;
        }

        static bool ValidateExactFields(
            Dictionary<string, JsonValue> values,
            string[] expected,
            string path,
            out RemoteConfigViolation violation
        )
        {
            violation = null;
            var allowed = new HashSet<string>(expected, StringComparer.Ordinal);
            var actual = new string[values.Count];
            values.Keys.CopyTo(actual, 0);
            Array.Sort(actual, StringComparer.Ordinal);
            for (int i = 0; i < actual.Length; i++)
            {
                if (!allowed.Contains(actual[i]))
                {
                    violation = new RemoteConfigViolation(
                        RemoteConfigFailureCodes.JsonUnknownField,
                        ChildPath(path, actual[i]),
                        "Unknown JSON member '" + actual[i] + "'."
                    );
                    return false;
                }
            }
            for (int i = 0; i < expected.Length; i++)
            {
                if (!values.ContainsKey(expected[i]))
                {
                    violation = new RemoteConfigViolation(
                        RemoteConfigFailureCodes.JsonMissingField,
                        ChildPath(path, expected[i]),
                        "Required JSON member '" + expected[i] + "' is missing."
                    );
                    return false;
                }
            }
            return true;
        }

        static bool ReadString(
            JsonValue parent,
            string name,
            string path,
            out string value,
            out RemoteConfigViolation violation
        )
        {
            value = string.Empty;
            violation = null;
            JsonValue item = parent.ObjectValue[name];
            if (item.Kind != JsonKind.String)
            {
                violation = TypeViolation(path, "The value must be a JSON string.");
                return false;
            }
            value = item.StringValue;
            return true;
        }

        static bool ReadBoolean(
            JsonValue parent,
            string name,
            string path,
            out bool value,
            out RemoteConfigViolation violation
        )
        {
            value = false;
            violation = null;
            JsonValue item = parent.ObjectValue[name];
            if (item.Kind != JsonKind.Boolean)
            {
                violation = TypeViolation(path, "The value must be a JSON boolean.");
                return false;
            }
            value = item.BooleanValue;
            return true;
        }

        static bool ReadInt32(
            JsonValue parent,
            string name,
            string path,
            out int value,
            out RemoteConfigViolation violation
        )
        {
            value = 0;
            violation = null;
            JsonValue item = parent.ObjectValue[name];
            if (item.Kind != JsonKind.Number)
            {
                violation = TypeViolation(path, "The value must be a JSON integer.");
                return false;
            }
            if (
                !int.TryParse(
                    item.NumberValue,
                    NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture,
                    out value
                ) ||
                !string.Equals(
                    value.ToString(CultureInfo.InvariantCulture),
                    item.NumberValue,
                    StringComparison.Ordinal
                )
            )
            {
                violation = TypeViolation(path, "The value must be a canonical Int32 integer.");
                return false;
            }
            return true;
        }

        static RemoteConfigViolation TypeViolation(string path, string message)
        {
            return new RemoteConfigViolation(
                RemoteConfigFailureCodes.JsonTypeMismatch,
                path,
                message
            );
        }

        static string ChildPath(string parent, string child)
        {
            return parent == "$" ? "$." + child : parent + "." + child;
        }

        enum JsonKind
        {
            Object,
            Array,
            String,
            Number,
            Boolean,
            Null
        }

        sealed class JsonValue
        {
            public JsonKind Kind;
            public Dictionary<string, JsonValue> ObjectValue;
            public List<JsonValue> ArrayValue;
            public string StringValue;
            public string NumberValue;
            public bool BooleanValue;
        }

        sealed class Parser
        {
            const int MaximumDepth = 16;
            const int MaximumNodes = 2048;
            const int MaximumStringCharacters = 16384;

            readonly string source;
            int index;
            int nodeCount;
            RemoteConfigViolation failure;

            public Parser(string source)
            {
                this.source = source ?? string.Empty;
            }

            public bool TryParse(
                out JsonValue value,
                out RemoteConfigViolation violation
            )
            {
                value = null;
                violation = null;
                SkipWhitespace();
                if (!ParseValue("$", 0, out value))
                {
                    violation = failure ?? Invalid("$", "The JSON source is invalid.");
                    return false;
                }
                SkipWhitespace();
                if (index != source.Length)
                {
                    violation = Invalid(
                        "$",
                        "Trailing content is not permitted after the root value."
                    );
                    return false;
                }
                return true;
            }

            bool ParseValue(string path, int depth, out JsonValue value)
            {
                value = null;
                if (depth > MaximumDepth)
                    return Fail(path, "The JSON nesting depth exceeds 16.");
                if (++nodeCount > MaximumNodes)
                    return Fail(path, "The JSON document contains too many values.");

                SkipWhitespace();
                if (index >= source.Length)
                    return Fail(path, "A JSON value was expected.");

                char current = source[index];
                if (current == '{')
                    return ParseObject(path, depth, out value);
                if (current == '[')
                    return ParseArray(path, depth, out value);
                if (current == '"')
                {
                    string parsed;
                    if (!ParseString(path, out parsed))
                        return false;
                    value = new JsonValue
                    {
                        Kind = JsonKind.String,
                        StringValue = parsed
                    };
                    return true;
                }
                if (current == 't')
                    return ParseLiteral(path, "true", JsonKind.Boolean, true, out value);
                if (current == 'f')
                    return ParseLiteral(path, "false", JsonKind.Boolean, false, out value);
                if (current == 'n')
                    return ParseLiteral(path, "null", JsonKind.Null, false, out value);
                if (current == '-' || (current >= '0' && current <= '9'))
                    return ParseNumber(path, out value);
                return Fail(path, "An unsupported token begins the JSON value.");
            }

            bool ParseObject(string path, int depth, out JsonValue value)
            {
                value = null;
                index++;
                var members = new Dictionary<string, JsonValue>(StringComparer.Ordinal);
                SkipWhitespace();
                if (Consume('}'))
                {
                    value = new JsonValue { Kind = JsonKind.Object, ObjectValue = members };
                    return true;
                }

                while (true)
                {
                    SkipWhitespace();
                    if (index >= source.Length || source[index] != '"')
                        return Fail(path, "An object member name must be a JSON string.");
                    string name;
                    if (!ParseString(path, out name))
                        return false;
                    string memberPath = ChildPath(path, name);
                    if (members.ContainsKey(name))
                    {
                        failure = new RemoteConfigViolation(
                            RemoteConfigFailureCodes.JsonDuplicateField,
                            memberPath,
                            "Duplicate JSON member '" + name + "'."
                        );
                        return false;
                    }

                    SkipWhitespace();
                    if (!Consume(':'))
                        return Fail(memberPath, "A colon must follow an object member name.");
                    JsonValue member;
                    if (!ParseValue(memberPath, depth + 1, out member))
                        return false;
                    members.Add(name, member);

                    SkipWhitespace();
                    if (Consume('}'))
                        break;
                    if (!Consume(','))
                        return Fail(path, "A comma or closing brace was expected.");
                    SkipWhitespace();
                    if (index < source.Length && source[index] == '}')
                        return Fail(path, "Trailing commas are not permitted in objects.");
                }

                value = new JsonValue { Kind = JsonKind.Object, ObjectValue = members };
                return true;
            }

            bool ParseArray(string path, int depth, out JsonValue value)
            {
                value = null;
                index++;
                var items = new List<JsonValue>();
                SkipWhitespace();
                if (Consume(']'))
                {
                    value = new JsonValue { Kind = JsonKind.Array, ArrayValue = items };
                    return true;
                }

                while (true)
                {
                    string itemPath = path + "[" +
                        items.Count.ToString(CultureInfo.InvariantCulture) + "]";
                    JsonValue item;
                    if (!ParseValue(itemPath, depth + 1, out item))
                        return false;
                    items.Add(item);

                    SkipWhitespace();
                    if (Consume(']'))
                        break;
                    if (!Consume(','))
                        return Fail(path, "A comma or closing bracket was expected.");
                    SkipWhitespace();
                    if (index < source.Length && source[index] == ']')
                        return Fail(path, "Trailing commas are not permitted in arrays.");
                }

                value = new JsonValue { Kind = JsonKind.Array, ArrayValue = items };
                return true;
            }

            bool ParseString(string path, out string result)
            {
                result = string.Empty;
                if (!Consume('"'))
                    return Fail(path, "A JSON string was expected.");

                var builder = new StringBuilder();
                while (index < source.Length)
                {
                    char current = source[index++];
                    if (current == '"')
                    {
                        result = builder.ToString();
                        return true;
                    }
                    if (current < 0x20)
                        return Fail(path, "Unescaped control characters are forbidden in strings.");
                    if (current == '\\')
                    {
                        if (index >= source.Length)
                            return Fail(path, "The string ends after an escape marker.");
                        char escape = source[index++];
                        switch (escape)
                        {
                            case '"': builder.Append('"'); break;
                            case '\\': builder.Append('\\'); break;
                            case '/': builder.Append('/'); break;
                            case 'b': builder.Append('\b'); break;
                            case 'f': builder.Append('\f'); break;
                            case 'n': builder.Append('\n'); break;
                            case 'r': builder.Append('\r'); break;
                            case 't': builder.Append('\t'); break;
                            case 'u':
                                if (!AppendUnicodeEscape(path, builder))
                                    return false;
                                break;
                            default:
                                return Fail(path, "The string contains an invalid escape sequence.");
                        }
                    }
                    else
                    {
                        if (char.IsSurrogate(current))
                            return Fail(path, "Raw surrogate code units are forbidden in strings.");
                        builder.Append(current);
                    }
                    if (builder.Length > MaximumStringCharacters)
                        return Fail(path, "A JSON string exceeds 16,384 characters.");
                }
                return Fail(path, "The JSON string is not terminated.");
            }

            bool AppendUnicodeEscape(string path, StringBuilder builder)
            {
                int first;
                if (!ReadHexQuad(out first))
                    return Fail(path, "A Unicode escape must contain four hexadecimal digits.");

                char firstChar = (char)first;
                if (char.IsLowSurrogate(firstChar))
                    return Fail(path, "A low surrogate cannot appear without a high surrogate.");
                if (!char.IsHighSurrogate(firstChar))
                {
                    builder.Append(firstChar);
                    return true;
                }

                if (
                    index + 2 > source.Length ||
                    source[index] != '\\' ||
                    source[index + 1] != 'u'
                )
                {
                    return Fail(path, "A high surrogate must be followed by a low surrogate escape.");
                }
                index += 2;
                int second;
                if (!ReadHexQuad(out second) || !char.IsLowSurrogate((char)second))
                    return Fail(path, "A high surrogate must be followed by a low surrogate escape.");
                builder.Append(firstChar);
                builder.Append((char)second);
                return true;
            }

            bool ReadHexQuad(out int value)
            {
                value = 0;
                if (index + 4 > source.Length)
                    return false;
                for (int i = 0; i < 4; i++)
                {
                    int digit = HexValue(source[index + i]);
                    if (digit < 0)
                        return false;
                    value = (value << 4) | digit;
                }
                index += 4;
                return true;
            }

            bool ParseNumber(string path, out JsonValue value)
            {
                value = null;
                int start = index;
                if (Consume('-') && index >= source.Length)
                    return Fail(path, "A minus sign must be followed by a number.");

                if (index < source.Length && source[index] == '0')
                {
                    index++;
                    if (index < source.Length && source[index] >= '0' && source[index] <= '9')
                        return Fail(path, "Leading zeroes are forbidden in JSON numbers.");
                }
                else
                {
                    int digits = ConsumeDigits();
                    if (digits == 0)
                        return Fail(path, "A JSON number requires an integer component.");
                }

                if (Consume('.'))
                {
                    if (ConsumeDigits() == 0)
                        return Fail(path, "A fraction requires at least one digit.");
                }
                if (index < source.Length && (source[index] == 'e' || source[index] == 'E'))
                {
                    index++;
                    if (index < source.Length && (source[index] == '+' || source[index] == '-'))
                        index++;
                    if (ConsumeDigits() == 0)
                        return Fail(path, "An exponent requires at least one digit.");
                }

                value = new JsonValue
                {
                    Kind = JsonKind.Number,
                    NumberValue = source.Substring(start, index - start)
                };
                return true;
            }

            bool ParseLiteral(
                string path,
                string literal,
                JsonKind kind,
                bool booleanValue,
                out JsonValue value
            )
            {
                value = null;
                if (index + literal.Length > source.Length)
                    return Fail(path, "An incomplete JSON literal was found.");
                for (int i = 0; i < literal.Length; i++)
                {
                    if (source[index + i] != literal[i])
                        return Fail(path, "An invalid JSON literal was found.");
                }
                index += literal.Length;
                value = new JsonValue { Kind = kind, BooleanValue = booleanValue };
                return true;
            }

            int ConsumeDigits()
            {
                int start = index;
                while (
                    index < source.Length &&
                    source[index] >= '0' &&
                    source[index] <= '9'
                )
                {
                    index++;
                }
                return index - start;
            }

            void SkipWhitespace()
            {
                while (index < source.Length)
                {
                    char current = source[index];
                    if (
                        current != ' ' &&
                        current != '\t' &&
                        current != '\r' &&
                        current != '\n'
                    )
                    {
                        return;
                    }
                    index++;
                }
            }

            bool Consume(char expected)
            {
                if (index >= source.Length || source[index] != expected)
                    return false;
                index++;
                return true;
            }

            bool Fail(string path, string message)
            {
                if (failure == null)
                    failure = Invalid(path, message);
                return false;
            }

            static RemoteConfigViolation Invalid(string path, string message)
            {
                return new RemoteConfigViolation(
                    RemoteConfigFailureCodes.JsonInvalid,
                    path,
                    message
                );
            }

            static int HexValue(char value)
            {
                if (value >= '0' && value <= '9')
                    return value - '0';
                if (value >= 'a' && value <= 'f')
                    return value - 'a' + 10;
                if (value >= 'A' && value <= 'F')
                    return value - 'A' + 10;
                return -1;
            }
        }
    }
}
