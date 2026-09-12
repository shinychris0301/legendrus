# v1.18 Step 3 — Store Listing Assets and Release Checklist Foundation

## Status

Implemented in source. Unity/Test Runner execution and all external Play Console,
legal, signing, AAB, device, and publishing evidence remain separate prerequisites.

## Google Play requirements snapshot

Requirements were rechecked on 2026-09-09 against current Google Play Console Help.
The source validator pins the following currently documented requirements:

- App name: maximum 30 characters.
- Short description: maximum 80 characters.
- Full description: maximum 4000 characters.
- App icon: 32-bit PNG with alpha, 512 x 512, maximum 1024 KB.
- Feature graphic: JPEG or 24-bit PNG without alpha, 1024 x 500.
- Screenshots: at least 2 across supported device types, up to 8 per device type,
  minimum dimension 320 px, maximum dimension 3840 px, and the long dimension may
  not exceed twice the short dimension.
- Project game-promotion target: at least 3 portrait 1080 x 1920 screenshots (or
  the corresponding landscape recommendation), based on the current Google Play
  game recommendation guidance.
- Screenshot/graphic alt text: project validator caps it at the currently documented
  140-character guidance.

Official references checked:

- https://support.google.com/googleplay/android-developer/answer/9859152
- https://support.google.com/googleplay/android-developer/answer/9866151

These values are a dated source snapshot. They must be rechecked if Play Console
requirements change before release.

## Implemented

### Engine-free store release core

`LegendsOfRus.StoreReleaseCore` contains:

- store text length validation;
- icon/feature/screenshot format and dimension validation;
- runtime-screenshot and alt-text project policy checks;
- separate publication-minimum and game-recommendation results;
- deterministic release checklist evidence and blocker codes;
- separate decisions for creating a controlled-test artifact and uploading a
  closed-test release.

### Store listing staging boundary

`ProjectDocs/StoreListing/` contains:

- draft metadata with blank unapproved descriptions;
- an intentionally empty final-asset staging folder;
- explicit instructions that reference design screens are not store-release evidence.

The retained 22 design screens are not copied into the staging directory and are not
counted as final screenshots. Final screenshots must represent the actual current
runtime experience.

### Release checklist

The checklist independently tracks:

- source/static verification;
- Unity compile/self-test/EditMode/PlayMode evidence;
- representative device matrix evidence;
- Android development-build evidence;
- store listing text and preview-asset approval;
- Privacy/Data safety approval;
- content rating;
- target audience;
- ads declaration;
- app-access declaration;
- production signing;
- AAB evidence;
- release notes;
- controlled-test build authorization;
- explicit Play publishing authorization.

No missing item can be inferred from another item.

## Explicitly blocked

- final app icon approval;
- final feature graphic approval;
- final runtime screenshot approval;
- final store description approval;
- final Privacy/Data safety approval;
- content-rating/target-audience/ads/app-access completion;
- production signing;
- AAB creation/upload authorization;
- controlled-test build authorization;
- Play publishing;
- production analytics/IAP/rewarded-ad activation.

## Unity verification

1. Open in Unity 6000.5.9f1 and confirm zero compile errors.
2. Run `Legends of Rus > Tests > Run v1.18 Step 3 Self-Tests`.
3. Expected result: `30/30`.
4. Run complete EditMode and PlayMode test suites.
5. Run `Legends of Rus > QA > Print v1.18 Step 3 Store Release Checklist`.
6. Confirm the current candidate remains blocked for signed artifact creation/upload.

This source checkpoint is successful when its validation foundation works and fails
closed. It is not successful by pretending missing release evidence is approved.
