# Visual Page 01-R — Splash/Title Production Reconstruction

This corrective rebuild replaces the previous square/rectangular crop implementation.

## Rules
- `ApprovedScreens/SplashTitle.png` stays reference-only.
- Runtime reconstructs the 941x1672 composition from separated sprites in `Resources/ProductionUI/SplashTitleFinal`.
- Character and Firebird production layers use non-rectangular alpha masks derived from the exact reference geometry.
- Russian version/language/start/settings/account/help/tagline/legal typography is preserved as separate exact reference sprites so the base capture is not degraded by an approximate fallback font.
- Character life is provided by low-alpha upper-body overlays and glow motion; the static hero silhouette stays locked to the approved composition.
- No full-screen runtime screenshot dependency is introduced.

## Static reconstruction metric
- Correlation: ~0.99835
- MAE: ~0.00183

These metrics compare the source-layer reconstruction at exact 941x1672 geometry against the approved reference before Unity rendering.

## Unity validation
Run `Legends of Rus > Tests > Run v1.20 Visual Page 01-R Splash-Title Reference-Accurate Self-Test`, then capture the page at 941x1672 and compare it to `ApprovedScreens/SplashTitle.png` before continuing to Page 02.
