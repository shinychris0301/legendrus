namespace LegendsOfRus.DiagnosticsCore
{
    /// <summary>
    /// Strict validation for the deliberately small diagnostic schema. Values
    /// are rejected as supplied; they are never trimmed, coerced, or repaired.
    /// </summary>
    public static class ErrorReportValidator
    {
        public const int CurrentSchemaVersion =
            ErrorReportSchema.CurrentSchemaVersion;

        public static ErrorReportOperationResult ValidateDraft(
            ErrorReportDraft draft
        )
        {
            if (draft == null)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.DraftNull
                );
            if (!IsValidKind(draft.Kind))
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.KindInvalid
                );
            if (!IsValidSubsystem(draft.Subsystem))
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.SubsystemInvalid
                );
            if (!IsValidErrorCode(draft.ErrorCode))
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.ErrorCodeInvalid
                );
            if (!IsValidSiteCode(draft.SiteCode))
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.SiteCodeInvalid
                );
            if (!IsReviewedTuple(draft))
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.CodeTupleInvalid
                );
            return ErrorReportOperationResult.Success(
                ErrorReportReasonCodes.Captured
            );
        }

        public static ErrorReportOperationResult ValidateRecord(
            ErrorReportRecord record
        )
        {
            if (record == null)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.RecordNull
                );
            if (record.SchemaVersion != ErrorReportSchema.CurrentSchemaVersion)
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.SchemaVersionUnsupported
                );

            ErrorReportOperationResult draftValidation = ValidateDraft(
                record.ToDraft()
            );
            if (!draftValidation.Accepted)
                return draftValidation;
            if (!IsLowerHexSha256(record.Fingerprint))
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.FingerprintInvalid
                );
            string expectedFingerprint;
            if (
                !ErrorReportDeterminism.TryComputeFingerprint(
                    record.ToDraft(),
                    out expectedFingerprint
                ) ||
                !string.Equals(record.Fingerprint, expectedFingerprint)
            )
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.FingerprintInvalid
                );
            }
            if (
                record.Sequence < 1L ||
                record.FirstUtcTicks < 1L ||
                record.LastUtcTicks < record.FirstUtcTicks ||
                record.OccurrenceCount < 1L ||
                record.DeliveryAttempts < 0 ||
                record.DeliveryAttempts >
                    ErrorReportPolicy.MaximumDeliveryAttemptsLimit ||
                record.NextAttemptUtcTicks < 1L ||
                !IsValidDeliveryState(record.DeliveryState)
            )
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.RecordInvalid
                );
            }
            if (
                record.DeliveryState == ErrorReportDeliveryState.Pending &&
                (record.DeliveryAttempts != 0 ||
                 record.NextAttemptUtcTicks != record.FirstUtcTicks)
            )
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.RecordInvalid
                );
            }
            if (
                record.DeliveryState == ErrorReportDeliveryState.RetryScheduled &&
                (record.DeliveryAttempts < 1 ||
                 record.DeliveryAttempts >=
                    ErrorReportPolicy.MaximumDeliveryAttemptsLimit ||
                 record.NextAttemptUtcTicks <= record.FirstUtcTicks)
            )
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.RecordInvalid
                );
            }
            if (
                (record.DeliveryState == ErrorReportDeliveryState.RetryExhausted ||
                 record.DeliveryState == ErrorReportDeliveryState.PermanentlyRejected) &&
                (record.DeliveryAttempts < 1 ||
                 record.NextAttemptUtcTicks != long.MaxValue)
            )
            {
                return ErrorReportOperationResult.Reject(
                    ErrorReportReasonCodes.RecordInvalid
                );
            }

            return ErrorReportOperationResult.Success(
                ErrorReportReasonCodes.Captured,
                record.Sequence,
                record.Fingerprint
            );
        }

        public static bool IsValidErrorCode(ErrorReportCode value)
        {
            switch (value)
            {
                case ErrorReportCode.BootstrapFailure:
                case ErrorReportCode.RuntimeInvariantFailure:
                case ErrorReportCode.ManagedCrashUnhandled:
                case ErrorReportCode.Match3PlanFailure:
                case ErrorReportCode.SaveReadFailure:
                case ErrorReportCode.SaveWriteFailure:
                case ErrorReportCode.SaveMigrationFailure:
                case ErrorReportCode.EconomyTransactionFailure:
                case ErrorReportCode.CommercialAdapterFailure:
                case ErrorReportCode.UiPresentationFailure:
                case ErrorReportCode.AudioPlaybackFailure:
                case ErrorReportCode.ContentLoadFailure:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsValidSiteCode(ErrorReportSiteCode value)
        {
            switch (value)
            {
                case ErrorReportSiteCode.BootstrapInitialize:
                case ErrorReportSiteCode.RuntimeInvariantCheck:
                case ErrorReportSiteCode.RuntimeUnhandled:
                case ErrorReportSiteCode.Match3ResolvePlan:
                case ErrorReportSiteCode.SaveReadDecode:
                case ErrorReportSiteCode.SaveWriteCommit:
                case ErrorReportSiteCode.SaveMigrationApply:
                case ErrorReportSiteCode.EconomyTransactionCommit:
                case ErrorReportSiteCode.CommercialAdapterInvoke:
                case ErrorReportSiteCode.UiPresentationUpdate:
                case ErrorReportSiteCode.AudioPlaybackInvoke:
                case ErrorReportSiteCode.ContentResourceLoad:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsReviewedTuple(ErrorReportDraft draft)
        {
            if (draft == null)
                return false;
            switch (draft.ErrorCode)
            {
                case ErrorReportCode.BootstrapFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Bootstrap &&
                        draft.SiteCode == ErrorReportSiteCode.BootstrapInitialize;
                case ErrorReportCode.RuntimeInvariantFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Runtime &&
                        draft.SiteCode == ErrorReportSiteCode.RuntimeInvariantCheck;
                case ErrorReportCode.ManagedCrashUnhandled:
                    return draft.Subsystem == ErrorReportSubsystem.Runtime &&
                        draft.SiteCode == ErrorReportSiteCode.RuntimeUnhandled;
                case ErrorReportCode.Match3PlanFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Gameplay &&
                        draft.SiteCode == ErrorReportSiteCode.Match3ResolvePlan;
                case ErrorReportCode.SaveReadFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Save &&
                        draft.SiteCode == ErrorReportSiteCode.SaveReadDecode;
                case ErrorReportCode.SaveWriteFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Save &&
                        draft.SiteCode == ErrorReportSiteCode.SaveWriteCommit;
                case ErrorReportCode.SaveMigrationFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Save &&
                        draft.SiteCode == ErrorReportSiteCode.SaveMigrationApply;
                case ErrorReportCode.EconomyTransactionFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Economy &&
                        draft.SiteCode == ErrorReportSiteCode.EconomyTransactionCommit;
                case ErrorReportCode.CommercialAdapterFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Commercial &&
                        draft.SiteCode == ErrorReportSiteCode.CommercialAdapterInvoke;
                case ErrorReportCode.UiPresentationFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Ui &&
                        draft.SiteCode == ErrorReportSiteCode.UiPresentationUpdate;
                case ErrorReportCode.AudioPlaybackFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Audio &&
                        draft.SiteCode == ErrorReportSiteCode.AudioPlaybackInvoke;
                case ErrorReportCode.ContentLoadFailure:
                    return draft.Subsystem == ErrorReportSubsystem.Content &&
                        draft.SiteCode == ErrorReportSiteCode.ContentResourceLoad;
                default:
                    return false;
            }
        }

        public static bool IsLowerHexSha256(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 64)
                return false;
            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                if (
                    (character < '0' || character > '9') &&
                    (character < 'a' || character > 'f')
                )
                {
                    return false;
                }
            }
            return true;
        }

        static bool IsValidKind(ErrorReportKind kind)
        {
            return kind == ErrorReportKind.NonFatal ||
                kind == ErrorReportKind.ManagedCrash;
        }

        static bool IsValidSubsystem(ErrorReportSubsystem subsystem)
        {
            switch (subsystem)
            {
                case ErrorReportSubsystem.Bootstrap:
                case ErrorReportSubsystem.Runtime:
                case ErrorReportSubsystem.Gameplay:
                case ErrorReportSubsystem.Save:
                case ErrorReportSubsystem.Economy:
                case ErrorReportSubsystem.Commercial:
                case ErrorReportSubsystem.Ui:
                case ErrorReportSubsystem.Audio:
                case ErrorReportSubsystem.Content:
                    return true;
                default:
                    return false;
            }
        }

        static bool IsValidDeliveryState(ErrorReportDeliveryState state)
        {
            switch (state)
            {
                case ErrorReportDeliveryState.Pending:
                case ErrorReportDeliveryState.RetryScheduled:
                case ErrorReportDeliveryState.RetryExhausted:
                case ErrorReportDeliveryState.PermanentlyRejected:
                    return true;
                default:
                    return false;
            }
        }
    }
}
