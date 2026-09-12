using System;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    sealed class BalanceTelemetryReviewActivationDto
    {
        public int schemaVersion;
        public bool localObservationBufferApproved;
        public bool localReviewReportApproved;
        public bool remoteTelemetryUploadApproved;
        public bool productionAnalyticsBridgeApproved;
        public bool automaticBalanceMutationApproved;
        public bool tutorialExperimentMutationApproved;
    }

    public readonly struct BalanceTelemetryReviewActivationSnapshot
    {
        public readonly bool LocalObservationBufferApproved;
        public readonly bool LocalReviewReportApproved;
        public readonly bool RemoteTelemetryUploadApproved;
        public readonly bool ProductionAnalyticsBridgeApproved;
        public readonly bool AutomaticBalanceMutationApproved;
        public readonly bool TutorialExperimentMutationApproved;

        public BalanceTelemetryReviewActivationSnapshot(
            bool localObservationBufferApproved,
            bool localReviewReportApproved,
            bool remoteTelemetryUploadApproved,
            bool productionAnalyticsBridgeApproved,
            bool automaticBalanceMutationApproved,
            bool tutorialExperimentMutationApproved)
        {
            LocalObservationBufferApproved = localObservationBufferApproved;
            LocalReviewReportApproved = localReviewReportApproved;
            RemoteTelemetryUploadApproved = remoteTelemetryUploadApproved;
            ProductionAnalyticsBridgeApproved = productionAnalyticsBridgeApproved;
            AutomaticBalanceMutationApproved = automaticBalanceMutationApproved;
            TutorialExperimentMutationApproved = tutorialExperimentMutationApproved;
        }
    }

    public static class BalanceTelemetryReviewActivation
    {
        public const bool RemoteTelemetryTransportCompiled = false;
        public const bool ProductionAnalyticsBridgeCompiled = false;
        public const bool AutomaticBalanceMutationCompiled = false;
        public const bool TutorialExperimentMutationCompiled = false;

        public static BalanceTelemetryReviewActivationSnapshot Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Config/balance_telemetry_review_activation");
            if (asset == null)
                return Disabled();

            try
            {
                BalanceTelemetryReviewActivationDto dto =
                    JsonUtility.FromJson<BalanceTelemetryReviewActivationDto>(asset.text);
                if (dto == null || dto.schemaVersion != 1)
                    return Disabled();

                if ((dto.remoteTelemetryUploadApproved && !RemoteTelemetryTransportCompiled) ||
                    (dto.productionAnalyticsBridgeApproved && !ProductionAnalyticsBridgeCompiled) ||
                    (dto.automaticBalanceMutationApproved && !AutomaticBalanceMutationCompiled) ||
                    (dto.tutorialExperimentMutationApproved && !TutorialExperimentMutationCompiled))
                    return Disabled();

                return new BalanceTelemetryReviewActivationSnapshot(
                    dto.localObservationBufferApproved,
                    dto.localReviewReportApproved,
                    dto.remoteTelemetryUploadApproved,
                    dto.productionAnalyticsBridgeApproved,
                    dto.automaticBalanceMutationApproved,
                    dto.tutorialExperimentMutationApproved);
            }
            catch
            {
                return Disabled();
            }
        }

        static BalanceTelemetryReviewActivationSnapshot Disabled()
        {
            return new BalanceTelemetryReviewActivationSnapshot(
                false, false, false, false, false, false);
        }
    }
}
