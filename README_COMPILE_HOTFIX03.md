# v1.17 Step 5 Compile Hotfix 03 — ContentDeliveryCore PlayMode asmdef reference

Fixes Unity compile error CS0012 in:

`Assets/LegendsOfRus/Tests/PlayMode/ContentDeliveryFoundationPlayModeTests.cs`

The PlayMode test assembly references `LegendsOfRus.Runtime`, but `RuntimeContentCatalog.TryGet(...)` exposes `ContentCatalogEntry` from `LegendsOfRus.ContentDeliveryCore`. Unity assembly definitions require the consuming test assembly to reference that assembly directly.

## Change

Adds this direct reference to:

`Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef`

```json
"LegendsOfRus.ContentDeliveryCore"
```

No runtime, gameplay, save, artwork, economy, monetization, or Addressables behavior is changed.

Apply this after Step 5 Compile Hotfix 02 (Addressables 2.8.1), or to any Step 5 project already using the corrected Addressables version.
