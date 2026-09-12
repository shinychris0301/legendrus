using System;

namespace LegendsOfRus.CommercialCore.RemoteConfig
{
    /// <summary>
    /// Deterministic provider for a caller-supplied bundled text snapshot. It
    /// performs no external access, persistence, or runtime registration.
    /// </summary>
    public sealed class DeterministicLocalRemoteConfigProvider : IRemoteConfigProvider
    {
        public const string ResourcePath = "Config/remote_config_local";
        public const string ExpectedBundledSourceSha256 =
            "3d8c8c676217bd87e884fdeaa5663871cf54cf6daac23b6584fcbd62f44a8536";

        readonly string sourceJson;
        readonly string expectedSourceSha256;

        public DeterministicLocalRemoteConfigProvider(string sourceJson)
            : this(sourceJson, ExpectedBundledSourceSha256)
        {
        }

        /// <summary>
        /// Supports deterministic offline validation of an explicitly bound
        /// fixture. The caller-supplied digest proves content identity only; it
        /// is not a signature or authority grant, and every result stays detached.
        /// </summary>
        public DeterministicLocalRemoteConfigProvider(
            string sourceJson,
            string expectedSourceSha256
        )
        {
            this.sourceJson = sourceJson;
            this.expectedSourceSha256 = expectedSourceSha256 ?? string.Empty;
        }

        public RemoteConfigLoadResult LoadSnapshot()
        {
            if (string.IsNullOrEmpty(sourceJson))
            {
                return Failed(
                    RemoteConfigLoadStatus.SourceMissing,
                    new RemoteConfigViolation(
                        RemoteConfigFailureCodes.SourceMissing,
                        "$",
                        "The local remote-config source is empty."
                    ),
                    string.Empty
                );
            }
            if (sourceJson.Length > StrictRemoteConfigJson.MaximumSourceCharacters)
            {
                return Failed(
                    RemoteConfigLoadStatus.SourceTooLarge,
                    new RemoteConfigViolation(
                        RemoteConfigFailureCodes.SourceTooLarge,
                        "$",
                        "The local remote-config source exceeds 65,536 characters."
                    ),
                    string.Empty
                );
            }

            string sourceHash = RemoteConfigSourceHash.ComputeNormalizedSha256(sourceJson);
            if (
                !IsLowerHexSha256(expectedSourceSha256) ||
                !string.Equals(
                    sourceHash,
                    expectedSourceSha256,
                    StringComparison.Ordinal
                )
            )
            {
                return Failed(
                    RemoteConfigLoadStatus.SourceHashMismatch,
                    new RemoteConfigViolation(
                        RemoteConfigFailureCodes.SourceHashMismatch,
                        "$",
                        "The normalized local source SHA-256 does not match its reviewed binding."
                    ),
                    sourceHash
                );
            }

            RemoteConfigDocumentData document;
            RemoteConfigViolation parseViolation;
            if (!StrictRemoteConfigJson.TryParse(
                sourceJson,
                out document,
                out parseViolation
            ))
            {
                RemoteConfigLoadStatus status = ParseFailureStatus(parseViolation);
                return Failed(status, parseViolation, sourceHash);
            }

            RemoteConfigValidationReport validation =
                RemoteConfigValidator.Validate(document);
            if (!validation.IsValid)
            {
                RemoteConfigViolation[] violations = validation.GetViolations();
                RemoteConfigViolation first = violations.Length == 0
                    ? new RemoteConfigViolation(
                        RemoteConfigFailureCodes.ValueInvalid,
                        "$",
                        "The local snapshot failed validation."
                    )
                    : violations[0];
                return new RemoteConfigLoadResult(
                    RemoteConfigLoadStatus.ValidationFailed,
                    first.Code,
                    "The local snapshot failed validation with " +
                        validation.ViolationCount + " violation(s).",
                    sourceHash,
                    RemoteConfigSnapshot.ConservativeFallback.ContentSha256,
                    RemoteConfigSnapshot.ConservativeFallback,
                    validation
                );
            }

            RemoteConfigSnapshot snapshot;
            if (!RemoteConfigValidator.TryBuildSnapshot(
                document,
                validation,
                out snapshot
            ))
            {
                return Failed(
                    RemoteConfigLoadStatus.ValidationFailed,
                    new RemoteConfigViolation(
                        RemoteConfigFailureCodes.ValueInvalid,
                        "$",
                        "The validated snapshot could not be converted atomically."
                    ),
                    sourceHash
                );
            }

            return new RemoteConfigLoadResult(
                RemoteConfigLoadStatus.LoadedDetachedLocal,
                RemoteConfigFailureCodes.None,
                "The immutable local snapshot passed strict detached validation.",
                sourceHash,
                snapshot.ContentSha256,
                snapshot,
                validation
            );
        }

        static RemoteConfigLoadResult Failed(
            RemoteConfigLoadStatus status,
            RemoteConfigViolation violation,
            string sourceHash
        )
        {
            RemoteConfigViolation safeViolation = violation ??
                new RemoteConfigViolation(
                    RemoteConfigFailureCodes.JsonInvalid,
                    "$",
                    "The local remote-config source was rejected."
                );
            var validation = new RemoteConfigValidationReport(
                new[] { safeViolation }
            );
            return new RemoteConfigLoadResult(
                status,
                safeViolation.Code,
                safeViolation.Message,
                sourceHash,
                RemoteConfigSnapshot.ConservativeFallback.ContentSha256,
                RemoteConfigSnapshot.ConservativeFallback,
                validation
            );
        }

        static RemoteConfigLoadStatus ParseFailureStatus(
            RemoteConfigViolation violation
        )
        {
            if (violation == null)
                return RemoteConfigLoadStatus.JsonInvalid;
            if (violation.Code == RemoteConfigFailureCodes.SourceMissing)
                return RemoteConfigLoadStatus.SourceMissing;
            if (violation.Code == RemoteConfigFailureCodes.SourceTooLarge)
                return RemoteConfigLoadStatus.SourceTooLarge;
            return RemoteConfigLoadStatus.JsonInvalid;
        }

        static bool IsLowerHexSha256(string value)
        {
            if (value == null || value.Length != 64)
                return false;
            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];
                if (
                    (current < '0' || current > '9') &&
                    (current < 'a' || current > 'f')
                )
                {
                    return false;
                }
            }
            return true;
        }
    }
}
