using System;
using System.Collections;
using System.Collections.Generic;
using LegendsOfRus.ContentDelivery;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegendsOfRus
{
    public readonly struct ContentSpriteLoadResult
    {
        public readonly ContentLoadStatus Status;
        public readonly ContentLoadSource Source;
        public readonly Sprite Sprite;
        public readonly string Reason;

        public ContentSpriteLoadResult(
            ContentLoadStatus status,
            ContentLoadSource source,
            Sprite sprite,
            string reason)
        {
            Status = status;
            Source = source;
            Sprite = sprite;
            Reason = reason ?? string.Empty;
        }
    }

    public sealed class ContentDeliveryService : MonoBehaviour
    {
        sealed class SpriteLease
        {
            public AsyncOperationHandle<Sprite> Handle;
            public int References;
        }

        readonly Dictionary<string, SpriteLease> activeSprites =
            new Dictionary<string, SpriteLease>(StringComparer.Ordinal);

        RuntimeContentCatalog catalog;
        ContentDeliveryActivationSnapshot activation;

        public static ContentDeliveryService Instance { get; private set; }
        public bool CatalogReady => catalog != null;
        public bool LocalAddressablesActive => activation.MayUseLocalAddressables;
        public bool RemoteDeliveryActive => false;
        public int ActiveAddressableSpriteLeases => activeSprites.Count;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            activation = ContentDeliveryActivation.Load();
            if (!RuntimeContentCatalog.TryLoad(out catalog, out string reason))
            {
                catalog = null;
                Debug.LogWarning("Content delivery manifest unavailable: " + reason);
            }
        }

        public IEnumerator LoadSprite(
            string address,
            Action<ContentSpriteLoadResult> completed)
        {
            if (!ContentDeliveryPolicy.IsAddressValid(address))
            {
                completed?.Invoke(Failed(ContentLoadStatus.InvalidAddress, "address_invalid"));
                yield break;
            }

            if (catalog == null || !catalog.TryGet(address, out ContentCatalogEntry entry))
            {
                completed?.Invoke(Failed(ContentLoadStatus.UnknownAddress, "address_unknown"));
                yield break;
            }

            if (entry.kind != ContentAssetKind.Sprite)
            {
                completed?.Invoke(Failed(ContentLoadStatus.TypeMismatch, "asset_type_mismatch"));
                yield break;
            }

            if (activation.MayUseLocalAddressables && entry.addressableReady)
            {
                if (activeSprites.TryGetValue(address, out SpriteLease existing) &&
                    existing.Handle.IsValid() && existing.Handle.Status == AsyncOperationStatus.Succeeded)
                {
                    existing.References++;
                    completed?.Invoke(new ContentSpriteLoadResult(
                        ContentLoadStatus.Succeeded,
                        ContentLoadSource.AddressablesLocal,
                        existing.Handle.Result,
                        "ok_cached"
                    ));
                    yield break;
                }

                AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);
                yield return handle;
                if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
                {
                    activeSprites[address] = new SpriteLease
                    {
                        Handle = handle,
                        References = 1
                    };
                    completed?.Invoke(new ContentSpriteLoadResult(
                        ContentLoadStatus.Succeeded,
                        ContentLoadSource.AddressablesLocal,
                        handle.Result,
                        "ok"
                    ));
                    yield break;
                }

                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            Sprite fallback = string.IsNullOrWhiteSpace(entry.legacyResourceKey)
                ? null
                : Resources.Load<Sprite>(entry.legacyResourceKey);
            if (fallback != null)
            {
                completed?.Invoke(new ContentSpriteLoadResult(
                    ContentLoadStatus.FallbackSucceeded,
                    ContentLoadSource.LegacyResourcesFallback,
                    fallback,
                    activation.MayUseLocalAddressables
                        ? "addressable_unavailable_fallback_ok"
                        : "local_addressables_not_approved_fallback_ok"
                ));
                yield break;
            }

            completed?.Invoke(Failed(ContentLoadStatus.Unavailable, "content_unavailable"));
        }

        public bool ReleaseSprite(string address)
        {
            if (string.IsNullOrEmpty(address) ||
                !activeSprites.TryGetValue(address, out SpriteLease lease))
                return false;

            lease.References--;
            if (lease.References > 0)
                return true;

            if (lease.Handle.IsValid())
                Addressables.Release(lease.Handle);
            activeSprites.Remove(address);
            return true;
        }

        void OnDestroy()
        {
            foreach (KeyValuePair<string, SpriteLease> item in activeSprites)
            {
                if (item.Value.Handle.IsValid())
                    Addressables.Release(item.Value.Handle);
            }
            activeSprites.Clear();
            if (Instance == this)
                Instance = null;
        }

        static ContentSpriteLoadResult Failed(ContentLoadStatus status, string reason)
        {
            return new ContentSpriteLoadResult(status, ContentLoadSource.None, null, reason);
        }
    }
}
