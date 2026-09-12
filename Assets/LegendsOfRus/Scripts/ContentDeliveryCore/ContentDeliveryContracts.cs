using System;

namespace LegendsOfRus.ContentDelivery
{
    public enum ContentDeliveryTier
    {
        BootLocal = 0,
        SharedDeferred = 1,
        ChapterDeferred = 2,
        ValidationLocal = 3
    }

    public enum ContentAssetKind
    {
        Text = 0,
        Sprite = 1
    }

    public enum ContentLoadSource
    {
        None = 0,
        AddressablesLocal = 1,
        LegacyResourcesFallback = 2
    }

    public enum ContentLoadStatus
    {
        Succeeded = 0,
        FallbackSucceeded = 1,
        InvalidAddress = 2,
        UnknownAddress = 3,
        TypeMismatch = 4,
        Unavailable = 5,
        Failed = 6
    }

    [Serializable]
    public sealed class ContentCatalogEntry
    {
        public string address;
        public string legacyResourceKey;
        public string group;
        public string[] labels;
        public ContentDeliveryTier tier;
        public ContentAssetKind kind;
        public bool addressableReady;

        public ContentCatalogEntry()
        {
            labels = Array.Empty<string>();
        }
    }

    public readonly struct ContentCatalogValidationResult
    {
        public readonly bool IsValid;
        public readonly string Reason;

        public ContentCatalogValidationResult(bool isValid, string reason)
        {
            IsValid = isValid;
            Reason = reason ?? string.Empty;
        }
    }
}
