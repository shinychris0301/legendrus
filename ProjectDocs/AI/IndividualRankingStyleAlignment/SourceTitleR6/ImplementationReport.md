# Individual Ranking — Original Title R6

User requested direct extraction of the original title. Russian now uses the original “ЛИЧНЫЙ РЕЙТИНГ” gold lettering together with its plaque and crest. Native Text rendering is disabled only for this Russian heading, avoiding a duplicate font overprint. Its semantic/localization string is retained. English continues to use the existing blank plaque and selected FreeSerif Bold live title. Switching back to Russian restores the source title.

## Extraction

Original source: `/home/k2/Downloads/match3final/individual ranking.png`,941×1672; SHA-256 recorded in AssetProvenance.json. One Scenario `model_pixa-background-removal` job supplied alpha, costing3CU. No image generation, inpainting, recoloring or font approximation was used for the Russian lettering.

The Scenario output incorrectly made portions of the intentionally opaque plaque transparent. Local alpha cleanup restores that surface, bounds the exterior to the title, and excludes neighboring profile/header/timer details. An early cleanup contour clipped crest curves and was rejected before integration; it was widened. The initial staging bounds were113,107–856,276; the final source bounds extend four pixels upward to113,103–856,276 to retain the original crest tip. A bounded source-color mask recovers that tip. No RGB pixels were repainted. Alpha boundaries remain estimated, especially around dark filigree; this is not a claim of recovering an original layered file.

Native743×173 PNG RGB equals the original expanded source region exactly. Runtime1486×346 RGB equals its2× Lanczos interpolation. The lettering body is fully opaque. Enlargement adds no original detail. QA was inspected over black, white and saturated magenta.

## Integration

Added `title_original_ru.png` with its own GUID, retaining the existing `title.png` for English. The runtime view owns the serialized original-title sprite and chooses the sprite, position and live-heading visibility by locale. The layout/builder and native prefab/cutover state use revision6. The Russian rectangle matches original source coordinates and aspect. Fonts, R5 medals, names, balances, navigation and other pages are preserved.

## Checks

Source-RGB equality, opaque-lettering-body assertions and transparency visual QA passed. The first Unity import caught an accidental RewardsHeading identifier replacement (CS0103); it was corrected before rebuilding. No tests ran during that failed import. Final rebuild/test evidence is stored in UnityVerification/ and VerificationSummary.json.

Not performed: APK build, physical-phone testing, all-page regressions or live backend verification. CurrentTask.md and SessionLog.md record the scoped outcome.

Final Unity6000.5.9f1 results: rebuild PASS,2/2 Edit Mode PASS,6/6 Play Mode PASS. Ten actual screenshots saved. Added assertions cover Russian source sprite/no Text overprint, English live title/blank sprite, and return to Russian after a locale round-trip. Existing font, layout, safe-area, raycast, navigation and idempotence checks remain passing. Visually inspected final Russian941×1672 and English576×1280 safe-area captures.

Scope audit:6 existing production/test files changed; one new sprite and its meta added. All existing artwork, fonts, meta/GUID files, other page prefabs, Packages and ProjectSettings are unchanged against the R6 starting snapshot.

Review browser check:12 capture options loaded; no horizontal overflow at390px. Scoped git diff whitespace check passed.
