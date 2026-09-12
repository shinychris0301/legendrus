using System;
using System.Collections.Generic;
using LegendsOfRus.ContentDelivery;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    sealed class ContentDeliveryManifestDto
    {
        public int schemaVersion;
        public ContentDeliveryManifestEntryDto[] entries;
    }

    [Serializable]
    sealed class ContentDeliveryManifestEntryDto
    {
        public string address;
        public string legacyResourceKey;
        public string group;
        public string[] labels;
        public string tier;
        public string kind;
        public bool addressableReady;
    }

    public sealed class RuntimeContentCatalog
    {
        readonly Dictionary<string, ContentCatalogEntry> entries;

        RuntimeContentCatalog(Dictionary<string, ContentCatalogEntry> values)
        {
            entries = values;
        }

        public int Count => entries.Count;

        public bool TryGet(string address, out ContentCatalogEntry entry)
        {
            return entries.TryGetValue(address ?? string.Empty, out entry);
        }

        public static bool TryLoad(out RuntimeContentCatalog catalog, out string reason)
        {
            catalog = null;
            reason = string.Empty;

            TextAsset asset = Resources.Load<TextAsset>("Config/content_delivery_manifest");
            if (asset == null)
            {
                reason = "manifest_missing";
                return false;
            }

            ContentDeliveryManifestDto dto;
            try
            {
                dto = JsonUtility.FromJson<ContentDeliveryManifestDto>(asset.text);
            }
            catch
            {
                reason = "manifest_json_invalid";
                return false;
            }

            if (dto == null || dto.schemaVersion != ContentDeliveryPolicy.ManifestSchemaVersion ||
                dto.entries == null)
            {
                reason = "manifest_schema_invalid";
                return false;
            }

            var parsed = new List<ContentCatalogEntry>(dto.entries.Length);
            for (int i = 0; i < dto.entries.Length; i++)
            {
                ContentDeliveryManifestEntryDto source = dto.entries[i];
                if (source == null ||
                    !Enum.TryParse(source.tier, false, out ContentDeliveryTier tier) ||
                    !Enum.TryParse(source.kind, false, out ContentAssetKind kind))
                {
                    reason = "manifest_entry_invalid";
                    return false;
                }

                parsed.Add(new ContentCatalogEntry
                {
                    address = source.address,
                    legacyResourceKey = source.legacyResourceKey,
                    group = source.group,
                    labels = source.labels ?? Array.Empty<string>(),
                    tier = tier,
                    kind = kind,
                    addressableReady = source.addressableReady
                });
            }

            ContentCatalogValidationResult validation =
                ContentDeliveryCatalogValidator.Validate(parsed);
            if (!validation.IsValid)
            {
                reason = validation.Reason;
                return false;
            }

            var index = new Dictionary<string, ContentCatalogEntry>(StringComparer.Ordinal);
            for (int i = 0; i < parsed.Count; i++)
                index.Add(parsed[i].address, parsed[i]);

            catalog = new RuntimeContentCatalog(index);
            return true;
        }
    }
}
