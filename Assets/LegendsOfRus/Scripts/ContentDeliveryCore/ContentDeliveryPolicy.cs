using System;

namespace LegendsOfRus.ContentDelivery
{
    public static class ContentDeliveryPolicy
    {
        public const int ManifestSchemaVersion = 1;
        public const int ActivationSchemaVersion = 1;
        public const int MaximumEntries = 128;
        public const int MaximumAddressLength = 96;

        public const string BootLocalGroup = "LOR_BootLocal";
        public const string SharedDeferredGroup = "LOR_SharedDeferred";
        public const string Chapter01DeferredGroup = "LOR_Chapter01Deferred";
        public const string ValidationLocalGroup = "LOR_ValidationLocal";

        public const string LocalLabel = "lor.local";
        public const string BootLabel = "lor.boot";
        public const string ScreenLabel = "lor.screen";
        public const string GameplayLabel = "lor.gameplay";
        public const string Chapter01Label = "lor.chapter.01";
        public const string ValidationLabel = "lor.validation";

        public const string ValidationProbeAddress = "lor.validation.local-probe";

        public static bool IsAddressValid(string address)
        {
            if (string.IsNullOrWhiteSpace(address) ||
                address.Length > MaximumAddressLength)
                return false;

            for (int i = 0; i < address.Length; i++)
            {
                char value = address[i];
                bool allowed =
                    (value >= 'a' && value <= 'z') ||
                    (value >= '0' && value <= '9') ||
                    value == '.' || value == '-' || value == '/' || value == '_';
                if (!allowed)
                    return false;
            }

            return !address.StartsWith("/", StringComparison.Ordinal) &&
                !address.EndsWith("/", StringComparison.Ordinal) &&
                address.IndexOf("//", StringComparison.Ordinal) < 0;
        }

        public static bool IsKnownGroup(string group)
        {
            return string.Equals(group, BootLocalGroup, StringComparison.Ordinal) ||
                string.Equals(group, SharedDeferredGroup, StringComparison.Ordinal) ||
                string.Equals(group, Chapter01DeferredGroup, StringComparison.Ordinal) ||
                string.Equals(group, ValidationLocalGroup, StringComparison.Ordinal);
        }
    }
}
