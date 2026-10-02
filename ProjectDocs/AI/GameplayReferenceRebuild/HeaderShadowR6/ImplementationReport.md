# Gameplay R6 — header shadow cleanup

The detached-looking black band was present in the header sprite itself, especially below the parchment and beside the outer metal trim. It was not a Unity drop-shadow component or a dark patch in the environment image. R5's contour mask retained that band as opaque artwork.

R6 clears the baked dark matte within bounded outer gutters, preserving all original RGB and the lit gold alpha. Dark material inside the panel faces and divider recesses remains intentional. A narrow contact edge remains beside shaded metal; the broad black band now reveals the environment. Header position, size, connected rails, portrait scrollwork, booster artwork/captions, board and gameplay behavior are unchanged.

## Scope and asset evidence

- New versioned sprite: `Assets/LegendsOfRus/Resources/ProductionUI/GameplayHeaderR6/header.png`.
- **7,160 alpha pixels changed; zero RGB pixels changed; zero changes outside the repair support; zero lit-gold alpha changes.** [Provenance](Sources/AssetProvenance.json), [support mask](Sources/RepairSupport.png), [preparation script](Sources/PrepareAssets.py).
- Reused the existing Scenario-derived artwork. No new API calls, credits or generated assets.
- Four existing Assets files modified (binding, revision, builder marker, canonical prefab), three added (new PNG and metadata), zero deleted. All405 prefab records preserved. Other sprites, fonts, metadata and page prefabs retain their snapshot hashes. [File audit](QA/ScopeAndVerification.json).

## Verification

Unity6000.5.9f1 Build01 compiled, rebuilt and read back the active Gameplay prefab at **revision17**. Verify01 passed **9 Edit Mode +31 Play Mode =40 checks**, zero failed/skipped. Native captures cover Russian/English, portrait/tall safe areas, two/three missions, small-phone play, large-value fixtures, boosters and effects.

Inspected alpha over black, white and magenta, the environment plate without the header, the repaired native941px/576px views, and before/after edge details. The review includes actual Unity header crops alongside the standalone full-page captures. The offline edge composites under Sources are asset inspection aids, not Unity captures.

All16 browser views load at390px without errors or horizontal overflow. No Android build, physical-device verification or full-project regression suite ran. Existing font fallback and reconstructed source regions remain limitations; this is not a whole-screen pixel-identity claim.

Next: user visual review of the repaired header edge.
