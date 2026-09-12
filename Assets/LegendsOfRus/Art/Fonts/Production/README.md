# Production Cyrillic font slot — v1.20 Step 7.3

This folder is intentionally delivered **without font binaries**.

The Step 7.3 Wave 1 candidate builder resolves the first Unity `Font` asset imported into this folder and applies the finalized live-text treatment (gold display/heading, HUD, parchment body, outline and shadow hierarchy). If no production font has been imported, Unity's built-in `LegacyRuntime.ttf` is used only as a development fallback.

For release signoff, import a properly licensed Cyrillic font family here and rerun:

`Legends of Rus > Production UI > Build/Repair v1.20 Step 7.3 Reference-Accurate Wave 1 Candidates`

Do not convert localized runtime strings into raster text. The game logo and other intentionally authored decorative lettering may remain art assets.
