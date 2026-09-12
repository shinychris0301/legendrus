# v1.17 Step 5 — Addressables and Content Delivery Foundation

## Status

`IMPLEMENTED_IN_SOURCE_PENDING_UNITY_ADDRESSABLES_AND_DEVICE_VERIFICATION`

This checkpoint introduces the package and source boundaries needed for future
content delivery while deliberately preserving the current runtime Resources
path for all approved production art.

## Package pin

`Packages/manifest.json` pins `com.unity.addressables` to `2.8.1`, the released
Addressables line documented for Unity 6000.0. The inherited Android production
package-lock approval gate remains false. Unity must resolve and return the new
`Packages/packages-lock.json` before any Android preflight can be repinned.

## Deterministic content contract

`LegendsOfRus.ContentDeliveryCore` is engine-free and defines:

- stable lowercase content addresses;
- four content groups: `LOR_BootLocal`, `LOR_SharedDeferred`,
  `LOR_Chapter01Deferred`, and `LOR_ValidationLocal`;
- stable labels: `lor.local`, `lor.boot`, `lor.screen`, `lor.gameplay`,
  `lor.chapter.01`, and `lor.validation`;
- deterministic duplicate/group/label/fallback validation;
- a hard rule that only the validation probe may be `addressableReady=true`
  during this checkpoint.

`Resources/Config/content_delivery_manifest.json` contains 31 entries. Thirty
production entries remain explicit legacy-Resources fallbacks. The only active
Addressables seed is the tiny non-Resources text probe at
`lor.validation.local-probe`.

## Editor Addressables configuration

After the package compiles,
`Step17AddressablesContentDeliveryConfigurator` uses the default Addressables
settings object to create the four deterministic groups and six labels. It
registers only the validation probe. It does not move or mark any current
`Resources` asset.

This matters because the existing approved screen sprites are currently under
`Resources/DesignScreens`; migrating them prematurely could create duplicate
shipping paths or change runtime identity. Their migration is a later reviewed
checkpoint after local load/release evidence exists.

## Runtime foundation

`ContentDeliveryService` is installed on the existing persistent runtime root.
It provides:

- validated address lookup;
- optional local Addressables sprite loading;
- reference-counted release of successful Addressables sprite handles;
- deterministic fallback to the existing `Resources.Load<Sprite>` key;
- fail-closed unknown/type/unavailable results;
- no remote catalog update call, download-dependencies call, URL, HTTP client,
  persistence, analytics upload, or provider SDK.

The shipped activation config keeps local Addressables use false as well as all
remote/catalog-update/final activation flags. Existing gameplay therefore stays
on the same Resources behavior until local migration is explicitly approved.

## Unity verification

1. Open the exact candidate in Unity `6000.5.9f1` and allow Package Manager to
   resolve Addressables 2.8.1.
2. Confirm zero compile errors.
3. Run `Legends of Rus > Content Delivery > Sync v1.17 Step 5 Local Foundation`.
4. Run `Legends of Rus > Content Delivery > Validate v1.17 Step 5 Local Foundation`.
5. Run `Legends of Rus > Tests > Run v1.17 Step 5 Self-Tests` and expect `16/16`.
6. Run the complete EditMode and PlayMode suites.
7. Open Addressables Groups and verify the four Step 5 groups exist and that the
   validation group contains only `lor.validation.local-probe`.
8. Verify Home → Pre-Level → Gameplay → Victory/Fail → Home remains visually and
   functionally unchanged.
9. Retain the Unity-generated `Packages/packages-lock.json` for the next package
   lock review. Do not enable remote profile paths or build/upload remote content.

## Hard stops

This step does not authorize production Resources migration, remote catalogs,
remote bundles, CCD/CDN, Play Asset Delivery, catalog updates, production crash
transport, economy/commercial activation, Android production signing/AAB, or
publishing.
