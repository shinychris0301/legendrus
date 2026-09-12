using System;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    sealed class ClosedTestQaActivationDto
    {
        public int schemaVersion;
        public bool localDeviceProfileCaptureApproved;
        public bool localObservationBufferApproved;
        public bool remoteQaUploadApproved;
        public bool productionQaProviderApproved;
        public bool privacyDataSafetyApproved;
        public bool storeListingAssetsApproved;
        public bool productionSigningApproved;
        public bool controlledTestBuildAuthorized;
        public bool storePublishingAuthorized;
    }

    public readonly struct ClosedTestQaActivationSnapshot
    {
        public readonly bool LocalDeviceProfileCaptureApproved;
        public readonly bool LocalObservationBufferApproved;
        public readonly bool RemoteQaUploadApproved;
        public readonly bool ProductionQaProviderApproved;
        public readonly bool PrivacyDataSafetyApproved;
        public readonly bool StoreListingAssetsApproved;
        public readonly bool ProductionSigningApproved;
        public readonly bool ControlledTestBuildAuthorized;
        public readonly bool StorePublishingAuthorized;

        public ClosedTestQaActivationSnapshot(
            bool localDeviceProfileCaptureApproved,
            bool localObservationBufferApproved,
            bool remoteQaUploadApproved,
            bool productionQaProviderApproved,
            bool privacyDataSafetyApproved,
            bool storeListingAssetsApproved,
            bool productionSigningApproved,
            bool controlledTestBuildAuthorized,
            bool storePublishingAuthorized)
        {
            LocalDeviceProfileCaptureApproved = localDeviceProfileCaptureApproved;
            LocalObservationBufferApproved = localObservationBufferApproved;
            RemoteQaUploadApproved = remoteQaUploadApproved;
            ProductionQaProviderApproved = productionQaProviderApproved;
            PrivacyDataSafetyApproved = privacyDataSafetyApproved;
            StoreListingAssetsApproved = storeListingAssetsApproved;
            ProductionSigningApproved = productionSigningApproved;
            ControlledTestBuildAuthorized = controlledTestBuildAuthorized;
            StorePublishingAuthorized = storePublishingAuthorized;
        }
    }

    public static class ClosedTestQaActivation
    {
        public const bool RemoteQaTransportCompiled = false;
        public const bool ProductionQaProviderCompiled = false;
        public const bool ProductionSigningCompiled = false;
        public const bool StorePublishingCompiled = false;

        public static ClosedTestQaActivationSnapshot Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Config/closed_test_qa_activation");
            if (asset == null)
                return Disabled();

            try
            {
                ClosedTestQaActivationDto dto =
                    JsonUtility.FromJson<ClosedTestQaActivationDto>(asset.text);
                if (dto == null || dto.schemaVersion != 1)
                    return Disabled();

                if ((dto.remoteQaUploadApproved && !RemoteQaTransportCompiled) ||
                    (dto.productionQaProviderApproved && !ProductionQaProviderCompiled) ||
                    (dto.productionSigningApproved && !ProductionSigningCompiled) ||
                    (dto.storePublishingAuthorized && !StorePublishingCompiled))
                    return Disabled();

                return new ClosedTestQaActivationSnapshot(
                    dto.localDeviceProfileCaptureApproved,
                    dto.localObservationBufferApproved,
                    dto.remoteQaUploadApproved,
                    dto.productionQaProviderApproved,
                    dto.privacyDataSafetyApproved,
                    dto.storeListingAssetsApproved,
                    dto.productionSigningApproved,
                    dto.controlledTestBuildAuthorized,
                    dto.storePublishingAuthorized);
            }
            catch
            {
                return Disabled();
            }
        }

        static ClosedTestQaActivationSnapshot Disabled()
        {
            return new ClosedTestQaActivationSnapshot(
                false, false, false, false, false, false, false, false, false);
        }
    }
}
