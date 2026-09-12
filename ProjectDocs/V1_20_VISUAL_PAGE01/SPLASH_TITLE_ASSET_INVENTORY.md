# Splash / Title — separated runtime asset inventory

Approved full-page reference: `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/SplashTitle.png` (941×1672). It is reference-only and is never loaded by the production runtime.

Runtime resource root: `Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleFinal/`

- `background_clean.png` — UI-neutralized cinematic environment with the primary characters/castle retained.
- `title_logo.png` — title/logo plaque layer.
- `language_frame.png` — language selector frame with live language text.
- `start_button_frame.png` — emerald CTA shell with live label.
- `icon_settings.png` — Settings medallion.
- `icon_account.png` — Account medallion.
- `icon_help.png` — Help medallion.
- `tagline_ornament.png` — gold divider/ornament; tagline stays live text.
- `character_male_micro.png` — Ivan micro-motion overlay.
- `character_female_micro.png` — Masha micro-motion overlay.
- `firebird_shimmer.png` — Firebird living-light overlay.
- `start_glow.png` — CTA breathing glow.
- `firebird_aura.png`, `character_rim_glow.png` — optional VFX resources retained for tuning.

Live Unity text: version, language, Start, Settings, Account, Help, tagline, copyright/legal line.

Motion is deliberately low-amplitude: character breathing/head drift, Firebird shimmer, background parallax, logo float, Start glow. It uses unscaled time and no Animator package dependency.
