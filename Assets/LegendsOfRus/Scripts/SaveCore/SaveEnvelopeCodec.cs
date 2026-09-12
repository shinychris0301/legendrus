using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace LegendsOfRus.SaveCore
{
    public sealed class SaveEnvelopeCodec
    {
        public const int CurrentEnvelopeVersion = 1;
        public const string EnvelopeMarker = "LOR_SAVE_ENVELOPE";
        public const int MaximumPayloadCharacters = 1024 * 1024;
        public const int MaximumPayloadUtf8Bytes = MaximumPayloadCharacters * 4;
        public const int MaximumPayloadBase64Characters = ((MaximumPayloadUtf8Bytes + 2) / 3) * 4;
        public const int MaximumEnvelopeCharacters = MaximumPayloadBase64Characters + 1024;

        static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        public SaveEnvelopeEncodeResult TryEncode(string payload, int payloadVersion, long revision)
        {
            return TryEncode(payload, payloadVersion, revision, SaveEnvelopeIntent.Ordinary);
        }

        public SaveEnvelopeEncodeResult TryEncode(
            string payload,
            int payloadVersion,
            long revision,
            SaveEnvelopeIntent intent)
        {
            if (payload == null
                || payload.Length > MaximumPayloadCharacters
                || payloadVersion <= 0
                || revision <= 0
                || (intent != SaveEnvelopeIntent.Ordinary && intent != SaveEnvelopeIntent.ReplaceHistory))
                return EncodeFailure(SaveEnvelopeEncodeStatus.InvalidInput, "ENVELOPE_ENCODE_INPUT_INVALID");

            try
            {
                byte[] payloadBytes = StrictUtf8.GetBytes(payload);
                string payloadBase64 = Convert.ToBase64String(payloadBytes);
                string payloadHash = ComputeSha256Hex(payloadBytes);
                string unsignedEnvelope = string.Join(
                    "\n",
                    EnvelopeMarker,
                    "format=" + CurrentEnvelopeVersion.ToString(CultureInfo.InvariantCulture),
                    "intent=" + (intent == SaveEnvelopeIntent.ReplaceHistory ? "replace" : "ordinary"),
                    "payloadVersion=" + payloadVersion.ToString(CultureInfo.InvariantCulture),
                    "revision=" + revision.ToString(CultureInfo.InvariantCulture),
                    "payloadByteCount=" + payloadBytes.Length.ToString(CultureInfo.InvariantCulture),
                    "payloadSha256=" + payloadHash,
                    "payloadBase64=" + payloadBase64);
                string envelopeHash = ComputeSha256Hex(StrictUtf8.GetBytes(unsignedEnvelope));
                return new SaveEnvelopeEncodeResult(
                    SaveEnvelopeEncodeStatus.Success,
                    unsignedEnvelope + "\nenvelopeSha256=" + envelopeHash,
                    "ENVELOPE_ENCODED");
            }
            catch (EncoderFallbackException)
            {
                return EncodeFailure(SaveEnvelopeEncodeStatus.EncodingFailure, "ENVELOPE_PAYLOAD_UTF8_INVALID");
            }
            catch (CryptographicException)
            {
                return EncodeFailure(SaveEnvelopeEncodeStatus.EncodingFailure, "ENVELOPE_HASH_UNAVAILABLE");
            }
        }

        public SaveEnvelopeDecodeResult Decode(string serializedEnvelope, int maximumSupportedPayloadVersion)
        {
            if (string.IsNullOrEmpty(serializedEnvelope))
                return DecodeFailure(SaveEnvelopeDecodeStatus.MissingInput, "ENVELOPE_INPUT_MISSING");

            if (!serializedEnvelope.StartsWith(EnvelopeMarker, StringComparison.Ordinal))
            {
                if (serializedEnvelope.Length > MaximumEnvelopeCharacters)
                    return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_SIZE_LIMIT_EXCEEDED");
                return DecodeFailure(SaveEnvelopeDecodeStatus.NotEnvelope, "ENVELOPE_MARKER_NOT_PRESENT");
            }

            if (maximumSupportedPayloadVersion <= 0)
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_STRUCTURE_INVALID");

            long inspectedFormatVersion;
            if (!TryInspectCanonicalPrefix(
                    serializedEnvelope,
                    out inspectedFormatVersion))
            {
                if (serializedEnvelope.Length > MaximumEnvelopeCharacters)
                    return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_SIZE_LIMIT_EXCEEDED");
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_FORMAT_INVALID");
            }

            if (inspectedFormatVersion > CurrentEnvelopeVersion)
            {
                // Trust the bounded canonical future prefix before applying v1's total
                // size/layout limits. This prevents a valid larger future record from
                // being downgraded to corrupt and replaced by older state, without any
                // split, base64, UTF-8, or hash allocation.
                if (serializedEnvelope.Length > MaximumEnvelopeCharacters)
                {
                    return DecodeFailure(
                        SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion,
                        "ENVELOPE_FORMAT_UNSUPPORTED");
                }

                // A current-layout record whose format digit was merely altered must
                // still authenticate with the current outer hash. A genuinely future,
                // differently shaped record is classified from the bounded canonical
                // marker/format prefix without imposing v1 field or line rules.
                string[] currentShapedFutureLines;
                if (TryGetCurrentShapedLines(serializedEnvelope, out currentShapedFutureLines))
                {
                    SaveEnvelopeDecodeResult integrityFailure =
                        ValidateCurrentStyleOuterIntegrity(currentShapedFutureLines);
                    if (integrityFailure != null)
                        return integrityFailure;
                }

                return DecodeFailure(
                    SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion,
                    "ENVELOPE_FORMAT_UNSUPPORTED");
            }

            if (inspectedFormatVersion != CurrentEnvelopeVersion)
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_FORMAT_INVALID");

            // Bound all v1 line splitting, UTF-8, and base64 allocations.
            if (serializedEnvelope.Length > MaximumEnvelopeCharacters)
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_SIZE_LIMIT_EXCEEDED");

            int newlineCount = 0;
            for (int i = 0; i < serializedEnvelope.Length; i++)
            {
                char character = serializedEnvelope[i];
                if (character == '\r')
                    return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_STRUCTURE_INVALID");
                if (character == '\n' && ++newlineCount > 8)
                    return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_STRUCTURE_INVALID");
            }
            if (newlineCount != 8)
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_STRUCTURE_INVALID");

            string[] lines = serializedEnvelope.Split('\n');
            if (lines.Length != 9 || !string.Equals(lines[0], EnvelopeMarker, StringComparison.Ordinal))
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_STRUCTURE_INVALID");

            SaveEnvelopeDecodeResult outerIntegrityFailure =
                ValidateCurrentStyleOuterIntegrity(lines);
            if (outerIntegrityFailure != null)
                return outerIntegrityFailure;

            int formatVersion;
            if (!TryParsePositiveInt(lines[1], "format=", out formatVersion))
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_FORMAT_INVALID");
            if (formatVersion != CurrentEnvelopeVersion)
                return DecodeFailure(
                    SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion,
                    "ENVELOPE_FORMAT_UNSUPPORTED");

            SaveEnvelopeIntent intent;
            if (string.Equals(lines[2], "intent=ordinary", StringComparison.Ordinal))
                intent = SaveEnvelopeIntent.Ordinary;
            else if (string.Equals(lines[2], "intent=replace", StringComparison.Ordinal))
                intent = SaveEnvelopeIntent.ReplaceHistory;
            else
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_INTENT_INVALID");

            int payloadVersion;
            long revision;
            int payloadByteCount;
            string payloadHash;
            string payloadBase64;
            if (!TryParsePositiveInt(lines[3], "payloadVersion=", out payloadVersion)
                || !TryParsePositiveLong(lines[4], "revision=", out revision)
                || !TryParseNonNegativeInt(lines[5], "payloadByteCount=", out payloadByteCount)
                || !TryGetValue(lines[6], "payloadSha256=", out payloadHash)
                || !TryGetValue(lines[7], "payloadBase64=", out payloadBase64)
                || !IsLowerHexSha256(payloadHash))
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_FIELDS_INVALID");
            }

            if (payloadByteCount > MaximumPayloadUtf8Bytes
                || payloadBase64.Length > MaximumPayloadBase64Characters)
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_SIZE_LIMIT_EXCEEDED");
            }
            int expectedBase64Characters = payloadByteCount == 0
                ? 0
                : ((payloadByteCount + 2) / 3) * 4;
            if (payloadBase64.Length != expectedBase64Characters)
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_SIZE_LIMIT_EXCEEDED");

            byte[] payloadBytes;
            try
            {
                payloadBytes = Convert.FromBase64String(payloadBase64);
            }
            catch (FormatException)
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.InvalidEncoding, "ENVELOPE_BASE64_INVALID");
            }

            if (!string.Equals(Convert.ToBase64String(payloadBytes), payloadBase64, StringComparison.Ordinal))
                return DecodeFailure(SaveEnvelopeDecodeStatus.InvalidEncoding, "ENVELOPE_BASE64_NONCANONICAL");
            if (payloadBytes.Length != payloadByteCount)
                return DecodeFailure(SaveEnvelopeDecodeStatus.IntegrityMismatch, "ENVELOPE_PAYLOAD_LENGTH_MISMATCH");

            try
            {
                string actualPayloadHash = ComputeSha256Hex(payloadBytes);
                if (!FixedTimeEquals(actualPayloadHash, payloadHash))
                    return DecodeFailure(SaveEnvelopeDecodeStatus.IntegrityMismatch, "ENVELOPE_PAYLOAD_HASH_MISMATCH");

                string payload = StrictUtf8.GetString(payloadBytes);
                if (payload.Length > MaximumPayloadCharacters)
                    return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_SIZE_LIMIT_EXCEEDED");
                if (payloadVersion > maximumSupportedPayloadVersion)
                {
                    return new SaveEnvelopeDecodeResult(
                        SaveEnvelopeDecodeStatus.UnsupportedPayloadVersion,
                        null,
                        payloadVersion,
                        revision,
                        intent,
                        "ENVELOPE_PAYLOAD_VERSION_UNSUPPORTED");
                }

                return new SaveEnvelopeDecodeResult(
                    SaveEnvelopeDecodeStatus.Success,
                    payload,
                    payloadVersion,
                    revision,
                    intent,
                    "ENVELOPE_VALID");
            }
            catch (DecoderFallbackException)
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.InvalidEncoding, "ENVELOPE_PAYLOAD_UTF8_INVALID");
            }
            catch (CryptographicException)
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.IntegrityMismatch, "ENVELOPE_HASH_UNAVAILABLE");
            }
        }

        static SaveEnvelopeEncodeResult EncodeFailure(SaveEnvelopeEncodeStatus status, string code)
        {
            return new SaveEnvelopeEncodeResult(status, null, code);
        }

        static SaveEnvelopeDecodeResult DecodeFailure(SaveEnvelopeDecodeStatus status, string code)
        {
            return new SaveEnvelopeDecodeResult(status, null, 0, 0, SaveEnvelopeIntent.Ordinary, code);
        }

        static bool TryInspectCanonicalPrefix(
            string serializedEnvelope,
            out long formatVersion)
        {
            formatVersion = 0;
            int markerLength = EnvelopeMarker.Length;
            if (serializedEnvelope.Length <= markerLength
                || serializedEnvelope[markerLength] != '\n')
            {
                return false;
            }

            const string formatPrefix = "format=";
            int formatStart = markerLength + 1;
            if (serializedEnvelope.Length < formatStart + formatPrefix.Length + 2)
                return false;
            for (int i = 0; i < formatPrefix.Length; i++)
            {
                if (serializedEnvelope[formatStart + i] != formatPrefix[i])
                    return false;
            }

            int digitStart = formatStart + formatPrefix.Length;
            int index = digitStart;
            if (serializedEnvelope[index] < '1' || serializedEnvelope[index] > '9')
                return false;

            long parsed = 0;
            while (index < serializedEnvelope.Length && serializedEnvelope[index] != '\n')
            {
                char character = serializedEnvelope[index];
                if (character < '0' || character > '9')
                    return false;
                int digit = character - '0';
                if (parsed > (long.MaxValue - digit) / 10)
                    return false;
                parsed = (parsed * 10) + digit;
                index++;
            }

            if (index == digitStart
                || index >= serializedEnvelope.Length
                || index + 1 >= serializedEnvelope.Length)
            {
                return false;
            }

            formatVersion = parsed;
            return true;
        }

        static bool TryGetCurrentShapedLines(string serializedEnvelope, out string[] lines)
        {
            lines = null;
            int newlineCount = 0;
            for (int i = 0; i < serializedEnvelope.Length; i++)
            {
                if (serializedEnvelope[i] == '\n' && ++newlineCount > 8)
                    return false;
            }
            if (newlineCount != 8)
                return false;

            string[] candidate = serializedEnvelope.Split('\n');
            if (candidate.Length != 9
                || !string.Equals(candidate[0], EnvelopeMarker, StringComparison.Ordinal)
                || !candidate[2].StartsWith("intent=", StringComparison.Ordinal)
                || !candidate[3].StartsWith("payloadVersion=", StringComparison.Ordinal)
                || !candidate[4].StartsWith("revision=", StringComparison.Ordinal)
                || !candidate[5].StartsWith("payloadByteCount=", StringComparison.Ordinal)
                || !candidate[6].StartsWith("payloadSha256=", StringComparison.Ordinal)
                || !candidate[7].StartsWith("payloadBase64=", StringComparison.Ordinal)
                || !candidate[8].StartsWith("envelopeSha256=", StringComparison.Ordinal))
            {
                return false;
            }

            lines = candidate;
            return true;
        }

        static SaveEnvelopeDecodeResult ValidateCurrentStyleOuterIntegrity(string[] lines)
        {
            string envelopeHash;
            if (!TryGetValue(lines[8], "envelopeSha256=", out envelopeHash)
                || !IsLowerHexSha256(envelopeHash))
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.Malformed, "ENVELOPE_FIELDS_INVALID");
            }
            try
            {
                string unsignedEnvelope = string.Join("\n", lines, 0, 8);
                string actualEnvelopeHash = ComputeSha256Hex(StrictUtf8.GetBytes(unsignedEnvelope));
                if (!FixedTimeEquals(actualEnvelopeHash, envelopeHash))
                    return DecodeFailure(SaveEnvelopeDecodeStatus.IntegrityMismatch, "ENVELOPE_HASH_MISMATCH");
                return null;
            }
            catch (EncoderFallbackException)
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.InvalidEncoding, "ENVELOPE_UTF8_INVALID");
            }
            catch (CryptographicException)
            {
                return DecodeFailure(SaveEnvelopeDecodeStatus.IntegrityMismatch, "ENVELOPE_HASH_UNAVAILABLE");
            }
        }

        static bool TryGetValue(string line, string prefix, out string value)
        {
            value = null;
            if (line == null || !line.StartsWith(prefix, StringComparison.Ordinal))
                return false;
            value = line.Substring(prefix.Length);
            return true;
        }

        static bool TryParsePositiveInt(string line, string prefix, out int value)
        {
            value = 0;
            string text;
            return TryGetValue(line, prefix, out text)
                && int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value)
                && value > 0
                && string.Equals(value.ToString(CultureInfo.InvariantCulture), text, StringComparison.Ordinal);
        }

        static bool TryParseNonNegativeInt(string line, string prefix, out int value)
        {
            value = 0;
            string text;
            return TryGetValue(line, prefix, out text)
                && int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value)
                && value >= 0
                && string.Equals(value.ToString(CultureInfo.InvariantCulture), text, StringComparison.Ordinal);
        }

        static bool TryParsePositiveLong(string line, string prefix, out long value)
        {
            value = 0L;
            string text;
            return TryGetValue(line, prefix, out text)
                && long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out value)
                && value > 0
                && string.Equals(value.ToString(CultureInfo.InvariantCulture), text, StringComparison.Ordinal);
        }

        static bool IsLowerHexSha256(string value)
        {
            if (value == null || value.Length != 64)
                return false;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if ((c < '0' || c > '9') && (c < 'a' || c > 'f'))
                    return false;
            }
            return true;
        }

        static bool FixedTimeEquals(string left, string right)
        {
            if (left == null || right == null || left.Length != right.Length)
                return false;
            int difference = 0;
            for (int i = 0; i < left.Length; i++)
                difference |= left[i] ^ right[i];
            return difference == 0;
        }

        static string ComputeSha256Hex(byte[] bytes)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                    builder.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
                return builder.ToString();
            }
        }
    }
}
