using System;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    sealed class ContentDeliveryActivationDto
    {
        public int schemaVersion;
        public bool localAddressablesActivationApproved;
        public bool remoteCatalogActivationApproved;
        public bool remoteContentActivationApproved;
        public bool catalogUpdatesActivationApproved;
        public bool finalRuntimeActivationApproved;
    }

    public readonly struct ContentDeliveryActivationSnapshot
    {
        public readonly bool LocalAddressablesApproved;
        public readonly bool RemoteCatalogApproved;
        public readonly bool RemoteContentApproved;
        public readonly bool CatalogUpdatesApproved;
        public readonly bool FinalRuntimeApproved;

        public ContentDeliveryActivationSnapshot(
            bool localAddressablesApproved,
            bool remoteCatalogApproved,
            bool remoteContentApproved,
            bool catalogUpdatesApproved,
            bool finalRuntimeApproved)
        {
            LocalAddressablesApproved = localAddressablesApproved;
            RemoteCatalogApproved = remoteCatalogApproved;
            RemoteContentApproved = remoteContentApproved;
            CatalogUpdatesApproved = catalogUpdatesApproved;
            FinalRuntimeApproved = finalRuntimeApproved;
        }

        public bool MayUseLocalAddressables =>
            LocalAddressablesApproved && !RemoteCatalogApproved &&
            !RemoteContentApproved && !CatalogUpdatesApproved;

        public bool AnyRemoteFeatureApproved =>
            RemoteCatalogApproved || RemoteContentApproved || CatalogUpdatesApproved;
    }

    public static class ContentDeliveryActivation
    {
        public const bool RemoteDeliveryCompiled = false;
        public const bool CatalogUpdatesCompiled = false;

        public static ContentDeliveryActivationSnapshot Load()
        {
            TextAsset asset = Resources.Load<TextAsset>("Config/content_delivery_activation");
            if (asset == null)
                return Disabled();

            try
            {
                ContentDeliveryActivationDto dto =
                    JsonUtility.FromJson<ContentDeliveryActivationDto>(asset.text);
                if (dto == null || dto.schemaVersion != 1)
                    return Disabled();

                // Fail closed if a source file attempts to approve a remote feature while
                // the corresponding production implementation is not compiled.
                if ((dto.remoteCatalogActivationApproved ||
                     dto.remoteContentActivationApproved ||
                     dto.catalogUpdatesActivationApproved) &&
                    (!RemoteDeliveryCompiled || !CatalogUpdatesCompiled))
                    return Disabled();

                return new ContentDeliveryActivationSnapshot(
                    dto.localAddressablesActivationApproved,
                    dto.remoteCatalogActivationApproved,
                    dto.remoteContentActivationApproved,
                    dto.catalogUpdatesActivationApproved,
                    dto.finalRuntimeActivationApproved
                );
            }
            catch
            {
                return Disabled();
            }
        }

        static ContentDeliveryActivationSnapshot Disabled()
        {
            return new ContentDeliveryActivationSnapshot(false, false, false, false, false);
        }
    }
}
