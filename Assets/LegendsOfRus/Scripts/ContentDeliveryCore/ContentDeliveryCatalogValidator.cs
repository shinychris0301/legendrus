using System;
using System.Collections.Generic;

namespace LegendsOfRus.ContentDelivery
{
    public static class ContentDeliveryCatalogValidator
    {
        public static ContentCatalogValidationResult Validate(
            IReadOnlyList<ContentCatalogEntry> entries)
        {
            if (entries == null)
                return Invalid("catalog_missing");
            if (entries.Count == 0 || entries.Count > ContentDeliveryPolicy.MaximumEntries)
                return Invalid("catalog_count_invalid");

            var addresses = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < entries.Count; i++)
            {
                ContentCatalogEntry entry = entries[i];
                if (entry == null)
                    return Invalid("entry_null");
                if (!ContentDeliveryPolicy.IsAddressValid(entry.address))
                    return Invalid("address_invalid");
                if (!addresses.Add(entry.address))
                    return Invalid("address_duplicate");
                if (!ContentDeliveryPolicy.IsKnownGroup(entry.group))
                    return Invalid("group_unknown");
                if (!HasLabel(entry.labels, ContentDeliveryPolicy.LocalLabel))
                    return Invalid("local_label_missing");
                if (entry.addressableReady && entry.tier != ContentDeliveryTier.ValidationLocal)
                    return Invalid("production_entry_prematurely_addressable");
                if (!entry.addressableReady &&
                    string.IsNullOrWhiteSpace(entry.legacyResourceKey))
                    return Invalid("fallback_missing");
            }

            return new ContentCatalogValidationResult(true, "ok");
        }

        static bool HasLabel(string[] labels, string expected)
        {
            if (labels == null)
                return false;
            for (int i = 0; i < labels.Length; i++)
            {
                if (string.Equals(labels[i], expected, StringComparison.Ordinal))
                    return true;
            }
            return false;
        }

        static ContentCatalogValidationResult Invalid(string reason)
        {
            return new ContentCatalogValidationResult(false, reason);
        }
    }
}
