#if UNITY_EDITOR
using System;
using LegendsOfRus.ContentDelivery;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    [InitializeOnLoad]
    public static class Step17AddressablesContentDeliveryConfigurator
    {
        const string ProbePath =
            "Assets/LegendsOfRus/ContentDelivery/Validation/local_content_probe.txt";
        const string SessionKey = "LegendsOfRus.Step17.ContentDeliverySynced";

        static readonly string[] Labels =
        {
            ContentDeliveryPolicy.LocalLabel,
            ContentDeliveryPolicy.BootLabel,
            ContentDeliveryPolicy.ScreenLabel,
            ContentDeliveryPolicy.GameplayLabel,
            ContentDeliveryPolicy.Chapter01Label,
            ContentDeliveryPolicy.ValidationLabel
        };

        static readonly string[] Groups =
        {
            ContentDeliveryPolicy.BootLocalGroup,
            ContentDeliveryPolicy.SharedDeferredGroup,
            ContentDeliveryPolicy.Chapter01DeferredGroup,
            ContentDeliveryPolicy.ValidationLocalGroup
        };

        static Step17AddressablesContentDeliveryConfigurator()
        {
            EditorApplication.delayCall += TrySessionSync;
        }

        [MenuItem("Legends of Rus/Content Delivery/Sync v1.17 Step 5 Local Foundation")]
        public static void SyncFromMenu()
        {
            Sync(true);
        }

        [MenuItem("Legends of Rus/Content Delivery/Validate v1.17 Step 5 Local Foundation")]
        public static void ValidateFromMenu()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                throw new InvalidOperationException("Addressables settings have not been created.");

            if (string.IsNullOrEmpty(settings.RemoteCatalogBuildPath.Id) ||
                string.IsNullOrEmpty(settings.RemoteCatalogLoadPath.Id))
                throw new InvalidOperationException("Catalog profile path references are missing.");
            if (settings.BuildRemoteCatalog)
                throw new InvalidOperationException("Remote catalogs are not approved for the local foundation.");

            for (int i = 0; i < Groups.Length; i++)
            {
                if (settings.FindGroup(Groups[i]) == null)
                    throw new InvalidOperationException("Missing Addressables group: " + Groups[i]);
            }

            string probeGuid = AssetDatabase.AssetPathToGUID(ProbePath);
            AddressableAssetEntry entry = settings.FindAssetEntry(probeGuid);
            if (entry == null ||
                !string.Equals(entry.address, ContentDeliveryPolicy.ValidationProbeAddress,
                    StringComparison.Ordinal))
                throw new InvalidOperationException("Local validation probe is not configured.");

            Debug.Log("v1.17 Step 5 Addressables local foundation validation passed. " +
                "Remote catalog/content delivery remains disabled.");
        }

        static void TrySessionSync()
        {
            if (SessionState.GetBool(SessionKey, false))
                return;
            SessionState.SetBool(SessionKey, true);
            try
            {
                Sync(false);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Step 5 Addressables foundation sync deferred: " + exception.Message);
            }
        }

        static void Sync(bool log)
        {
            AddressableAssetSettings settings =
                AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings == null)
                throw new InvalidOperationException("Unable to create Addressables settings.");

            // Build-layout generation evaluates the remote build reference even when
            // remote catalogs are disabled. Older serialized empty IDs bypass the
            // package's null-only initialization. Repair only missing references;
            // keep custom paths, profile values and remote activation unchanged.
            if (string.IsNullOrEmpty(settings.RemoteCatalogBuildPath.Id))
                settings.RemoteCatalogBuildPath.SetVariableByName(
                    settings, AddressableAssetSettings.kRemoteBuildPath);
            if (string.IsNullOrEmpty(settings.RemoteCatalogLoadPath.Id))
                settings.RemoteCatalogLoadPath.SetVariableByName(
                    settings, AddressableAssetSettings.kRemoteLoadPath);

            for (int i = 0; i < Labels.Length; i++)
                settings.AddLabel(Labels[i]);

            for (int i = 0; i < Groups.Length; i++)
                EnsureGroup(settings, Groups[i]);

            AddressableAssetGroup validationGroup =
                settings.FindGroup(ContentDeliveryPolicy.ValidationLocalGroup);
            string probeGuid = AssetDatabase.AssetPathToGUID(ProbePath);
            if (string.IsNullOrEmpty(probeGuid))
                throw new InvalidOperationException("Validation probe asset is missing.");

            AddressableAssetEntry probe =
                settings.CreateOrMoveEntry(probeGuid, validationGroup);
            probe.address = ContentDeliveryPolicy.ValidationProbeAddress;
            probe.SetLabel(ContentDeliveryPolicy.LocalLabel, true, true);
            probe.SetLabel(ContentDeliveryPolicy.ValidationLabel, true, true);

            // Step 5 intentionally does not move or mark any current Resources asset.
            // Those assets remain on the existing Resources fallback path until a later,
            // explicitly reviewed migration checkpoint.
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            if (log)
                Debug.Log("v1.17 Step 5 local Addressables foundation synchronized. " +
                    "Only the validation probe is addressable; production Resources migration is deferred.");
        }

        static AddressableAssetGroup EnsureGroup(
            AddressableAssetSettings settings,
            string groupName)
        {
            AddressableAssetGroup existing = settings.FindGroup(groupName);
            if (existing != null)
                return existing;

            return settings.CreateGroup(
                groupName,
                false,
                false,
                false,
                null,
                typeof(BundledAssetGroupSchema),
                typeof(ContentUpdateGroupSchema)
            );
        }
    }
}
#endif
