using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using LegendsOfRus.SaveCore;
using NUnit.Framework;

namespace LegendsOfRus.Tests.SaveRecovery
{
    [TestFixture]
    public sealed class SaveEnvelopeCodecEditModeTests
    {
        readonly SaveEnvelopeCodec codec = new SaveEnvelopeCodec();

        [Test]
        public void Encode_IsDeterministic_AndUnicodeRoundTrips()
        {
            const string payload = "{\"hero\":\"Иван\",\"rune\":\"ᚱ\",\"line\":\"a\\nb\"}";

            SaveEnvelopeEncodeResult first = codec.TryEncode(payload, 6, 42);
            SaveEnvelopeEncodeResult second = codec.TryEncode(payload, 6, 42);

            Assert.That(first.Success, Is.True);
            Assert.That(second.Envelope, Is.EqualTo(first.Envelope));
            Assert.That(first.Envelope.Contains("\r"), Is.False);
            SaveEnvelopeDecodeResult decoded = codec.Decode(first.Envelope, 6);
            Assert.That(decoded.Success, Is.True);
            Assert.That(decoded.Payload, Is.EqualTo(payload));
            Assert.That(decoded.PayloadVersion, Is.EqualTo(6));
            Assert.That(decoded.Revision, Is.EqualTo(42));
            Assert.That(decoded.Intent, Is.EqualTo(SaveEnvelopeIntent.Ordinary));
        }

        [Test]
        public void PayloadVersion_IsExternalToOpaquePayload()
        {
            const string payload = "{\"saveVersion\":999999,\"opaque\":true}";
            string envelope = Encode(payload, 2, 1);

            SaveEnvelopeDecodeResult decoded = codec.Decode(envelope, 6);

            Assert.That(decoded.Success, Is.True);
            Assert.That(decoded.PayloadVersion, Is.EqualTo(2));
            Assert.That(decoded.Payload, Is.EqualTo(payload));
        }

        [TestCase(null)]
        [TestCase("")]
        public void MissingInput_IsRejected(string input)
        {
            Assert.That(codec.Decode(input, 6).Status, Is.EqualTo(SaveEnvelopeDecodeStatus.MissingInput));
        }

        [Test]
        public void NonEnvelope_IsDistinguishedForLegacyInspection()
        {
            Assert.That(
                codec.Decode("{\"legacy\":true}", 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.NotEnvelope));
        }

        [Test]
        public void TruncatedEnvelope_IsRejected()
        {
            string envelope = Encode("payload", 6, 1);
            string truncated = envelope.Substring(0, envelope.LastIndexOf('\n'));

            Assert.That(codec.Decode(truncated, 6).Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
        }

        [Test]
        public void ExtraLine_IsRejected()
        {
            Assert.That(
                codec.Decode(Encode("payload", 6, 1) + "\nextra=true", 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
        }

        [Test]
        public void CarriageReturn_IsRejected()
        {
            string envelope = Encode("payload", 6, 1).Replace("\n", "\r\n");
            Assert.That(codec.Decode(envelope, 6).Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
        }

        [Test]
        public void AlteredEnvelope_IsDetected()
        {
            string envelope = Encode("payload", 6, 1);
            string tampered = envelope.Replace("payloadVersion=6", "payloadVersion=5");

            Assert.That(
                codec.Decode(tampered, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.IntegrityMismatch));
        }

        [Test]
        public void AlteredPayloadHash_IsDetectedEvenWithResignedEnvelopeHash()
        {
            string envelope = Encode("payload", 6, 1);
            string tampered = ReplaceLineAndResign(envelope, 6, "payloadSha256=" + new string('0', 64));

            SaveEnvelopeDecodeResult decoded = codec.Decode(tampered, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.IntegrityMismatch));
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_PAYLOAD_HASH_MISMATCH"));
        }

        [Test]
        public void AlteredPayloadBytes_AreDetected()
        {
            string envelope = Encode("payload", 6, 1);
            string tampered = envelope.Replace("cGF5bG9hZA==", "cGF5bG9hZQ==");

            Assert.That(
                codec.Decode(tampered, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.IntegrityMismatch));
        }

        [Test]
        public void InvalidBase64_IsRejected()
        {
            string envelope = Encode("payload", 6, 1);
            string tampered = ReplaceLineAndResign(envelope, 7, "payloadBase64=!!!!!!!!!!!!");

            Assert.That(
                codec.Decode(tampered, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.InvalidEncoding));
        }

        [Test]
        public void NonCanonicalBase64_IsRejected()
        {
            string envelope = Encode("p", 6, 1);
            string tampered = ReplaceLineAndResign(envelope, 7, "payloadBase64=cB==");

            Assert.That(
                codec.Decode(tampered, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.InvalidEncoding));
        }

        [Test]
        public void InvalidUtf8Payload_IsRejectedAfterIntegrityValidation()
        {
            string envelope = Encode("p", 6, 1);
            string withBytes = ReplaceLineAndResign(envelope, 5, "payloadByteCount=1");
            withBytes = ReplaceLineAndResign(withBytes, 6, "payloadSha256=" + Sha256Hex(new byte[] { 0xff }));
            withBytes = ReplaceLineAndResign(withBytes, 7, "payloadBase64=/w==");

            Assert.That(
                codec.Decode(withBytes, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.InvalidEncoding));
        }

        [Test]
        public void FuturePayloadVersion_IsReportedWithoutReturningPayload()
        {
            string envelope = Encode("sensitive-payload", 7, 12);

            SaveEnvelopeDecodeResult decoded = codec.Decode(envelope, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.UnsupportedPayloadVersion));
            Assert.That(decoded.Payload, Is.Null);
            Assert.That(decoded.PayloadVersion, Is.EqualTo(7));
            Assert.That(decoded.Revision, Is.EqualTo(12));
            Assert.That(decoded.ToString(), Does.Not.Contain("sensitive-payload"));
        }

        [Test]
        public void UnresignedFutureFormatClaim_IsIntegrityMismatch()
        {
            string envelope = Encode("payload", 6, 1).Replace("format=1", "format=2");
            Assert.That(
                codec.Decode(envelope, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.IntegrityMismatch));
        }

        [Test]
        public void IntegrityValidFutureEnvelopeFormat_IsUnsupported()
        {
            string envelope = ReplaceLineAndResign(Encode("payload", 6, 1), 1, "format=2");
            Assert.That(
                codec.Decode(envelope, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion));
        }

        [Test]
        public void CanonicalDifferentlyShapedFutureEnvelope_IsUnsupportedBeforeV1LayoutValidation()
        {
            string future = SaveEnvelopeCodec.EnvelopeMarker
                + "\nformat=2\nfutureHeader=alpha\nfutureBody=opaque";

            SaveEnvelopeDecodeResult decoded = codec.Decode(future, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion));
            Assert.That(decoded.Payload, Is.Null);
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_FORMAT_UNSUPPORTED"));
        }

        [Test]
        public void OversizedCanonicalFutureEnvelope_RemainsUnsupportedWithoutV1Allocation()
        {
            string future = SaveEnvelopeCodec.EnvelopeMarker
                + "\nformat=2\n"
                + new string('x', SaveEnvelopeCodec.MaximumEnvelopeCharacters);

            SaveEnvelopeDecodeResult decoded = codec.Decode(future, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.UnsupportedEnvelopeVersion));
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_FORMAT_UNSUPPORTED"));
        }

        [TestCase("format=02")]
        [TestCase("format=0")]
        [TestCase("format=-2")]
        [TestCase("format=two")]
        [TestCase("format=9223372036854775808")]
        public void MalformedFutureFormatEvidence_IsNotTrusted(string formatLine)
        {
            string candidate = SaveEnvelopeCodec.EnvelopeMarker
                + "\n" + formatLine + "\nfutureBody=opaque";

            SaveEnvelopeDecodeResult decoded = codec.Decode(candidate, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
        }

        [Test]
        public void FutureFormatWithoutBody_IsMalformedRatherThanTrusted()
        {
            string candidate = SaveEnvelopeCodec.EnvelopeMarker + "\nformat=2\n";
            Assert.That(
                codec.Decode(candidate, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
        }

        [Test]
        public void MarkerPrefixMutation_IsMalformedRatherThanTrusted()
        {
            string candidate = SaveEnvelopeCodec.EnvelopeMarker
                + "_MUTATED\nformat=2\nfutureBody=opaque";
            Assert.That(
                codec.Decode(candidate, 6).Status,
                Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
        }

        [Test]
        public void ReplaceHistoryIntent_IsIntegrityProtectedAndRoundTrips()
        {
            SaveEnvelopeEncodeResult encoded = codec.TryEncode("reset", 6, 8, SaveEnvelopeIntent.ReplaceHistory);
            Assert.That(encoded.Success, Is.True);

            SaveEnvelopeDecodeResult decoded = codec.Decode(encoded.Envelope, 6);

            Assert.That(decoded.Success, Is.True);
            Assert.That(decoded.Intent, Is.EqualTo(SaveEnvelopeIntent.ReplaceHistory));
            string tampered = encoded.Envelope.Replace("intent=replace", "intent=ordinary");
            Assert.That(codec.Decode(tampered, 6).Status, Is.EqualTo(SaveEnvelopeDecodeStatus.IntegrityMismatch));
        }

        [Test]
        public void NonCanonicalNumbers_AreRejected()
        {
            string envelope = ReplaceLineAndResign(
                Encode("payload", 6, 1),
                4,
                "revision=01");
            Assert.That(codec.Decode(envelope, 6).Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
        }

        [Test]
        public void InvalidUtf16Input_CannotBeEncoded()
        {
            SaveEnvelopeEncodeResult encoded = codec.TryEncode("\ud800", 6, 1);
            Assert.That(encoded.Status, Is.EqualTo(SaveEnvelopeEncodeStatus.EncodingFailure));
            Assert.That(encoded.Envelope, Is.Null);
        }

        [Test]
        public void OversizedPayload_IsRejectedBeforeUtf8OrBase64Work()
        {
            string oversized = new string('x', SaveEnvelopeCodec.MaximumPayloadCharacters + 1);

            SaveEnvelopeEncodeResult encoded = codec.TryEncode(oversized, 6, 1);

            Assert.That(encoded.Status, Is.EqualTo(SaveEnvelopeEncodeStatus.InvalidInput));
            Assert.That(encoded.Envelope, Is.Null);
        }

        [Test]
        public void OversizedEnvelope_IsRejectedBeforeLineSplitting()
        {
            string oversized = SaveEnvelopeCodec.EnvelopeMarker
                + new string('x', SaveEnvelopeCodec.MaximumEnvelopeCharacters);

            SaveEnvelopeDecodeResult decoded = codec.Decode(oversized, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_SIZE_LIMIT_EXCEEDED"));
        }

        [Test]
        public void NewlineDenseEnvelope_IsRejectedBeforeSplitAllocationAmplification()
        {
            string newlineBomb = SaveEnvelopeCodec.EnvelopeMarker + new string('\n', 1024);

            SaveEnvelopeDecodeResult decoded = codec.Decode(newlineBomb, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_FORMAT_INVALID"));
        }

        [Test]
        public void OversizedDeclaredPayload_IsRejectedBeforeBase64Decode()
        {
            string envelope = ReplaceLineAndResign(
                Encode("payload", 6, 1),
                5,
                "payloadByteCount=" + (SaveEnvelopeCodec.MaximumPayloadUtf8Bytes + 1));

            SaveEnvelopeDecodeResult decoded = codec.Decode(envelope, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_SIZE_LIMIT_EXCEEDED"));
        }

        [Test]
        public void OversizedBase64Field_IsRejectedBeforeDecodeAllocation()
        {
            string oversized = ReplaceLineAndResign(
                Encode("payload", 6, 1),
                5,
                "payloadByteCount=" + SaveEnvelopeCodec.MaximumPayloadUtf8Bytes);
            oversized = ReplaceLineAndResign(
                oversized,
                7,
                "payloadBase64=" + new string(
                    'A',
                    SaveEnvelopeCodec.MaximumPayloadBase64Characters + 4));

            SaveEnvelopeDecodeResult decoded = codec.Decode(oversized, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_SIZE_LIMIT_EXCEEDED"));
        }

        [Test]
        public void DecodedCharacterLimit_IsEnforcedWithinBoundedByteAllocation()
        {
            byte[] bytes = new UTF8Encoding(false, true).GetBytes(
                new string('x', SaveEnvelopeCodec.MaximumPayloadCharacters + 1));
            string envelope = BuildUncheckedEnvelope(bytes, 6, 1);

            SaveEnvelopeDecodeResult decoded = codec.Decode(envelope, 6);

            Assert.That(decoded.Status, Is.EqualTo(SaveEnvelopeDecodeStatus.Malformed));
            Assert.That(decoded.DiagnosticCode, Is.EqualTo("ENVELOPE_SIZE_LIMIT_EXCEEDED"));
        }

        [TestCase(null, 1, 1)]
        [TestCase("payload", 0, 1)]
        [TestCase("payload", 1, 0)]
        public void InvalidEncodeInputs_ReturnSafeFailure(string payload, int version, long revision)
        {
            SaveEnvelopeEncodeResult result = codec.TryEncode(payload, version, revision);
            Assert.That(result.Status, Is.EqualTo(SaveEnvelopeEncodeStatus.InvalidInput));
            Assert.That(result.Envelope, Is.Null);
        }

        string Encode(string payload, int version, long revision)
        {
            SaveEnvelopeEncodeResult result = codec.TryEncode(payload, version, revision);
            Assert.That(result.Success, Is.True, result.ToString());
            return result.Envelope;
        }

        static string ReplaceLineAndResign(string envelope, int lineIndex, string replacement)
        {
            string[] lines = envelope.Split('\n');
            lines[lineIndex] = replacement;
            string unsigned = string.Join("\n", lines, 0, 8);
            lines[8] = "envelopeSha256=" + Sha256Hex(new UTF8Encoding(false, true).GetBytes(unsigned));
            return string.Join("\n", lines);
        }

        static string BuildUncheckedEnvelope(byte[] payloadBytes, int payloadVersion, long revision)
        {
            string unsigned = string.Join(
                "\n",
                SaveEnvelopeCodec.EnvelopeMarker,
                "format=1",
                "intent=ordinary",
                "payloadVersion=" + payloadVersion.ToString(CultureInfo.InvariantCulture),
                "revision=" + revision.ToString(CultureInfo.InvariantCulture),
                "payloadByteCount=" + payloadBytes.Length.ToString(CultureInfo.InvariantCulture),
                "payloadSha256=" + Sha256Hex(payloadBytes),
                "payloadBase64=" + Convert.ToBase64String(payloadBytes));
            return unsigned + "\nenvelopeSha256="
                + Sha256Hex(new UTF8Encoding(false, true).GetBytes(unsigned));
        }

        static string Sha256Hex(byte[] bytes)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                    builder.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
                return builder.ToString();
            }
        }
    }
}
