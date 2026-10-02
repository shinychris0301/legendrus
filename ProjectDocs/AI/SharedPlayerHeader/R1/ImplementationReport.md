# Shared player header R1 — Home and World Map

Home and World Map now use the same `ProductionPlayerHeader` component. One implementation owns the portrait, repaired lacquer background, resource capsules, plus/settings artwork, exact local geometry, Home typography/color, live RU/EN player/level labels, saved balances and maximum-value fitting. Both pages bind their existing serialized controls to this component. Their views retain existing event subscriptions, navigation actions and Home energy-clock handling.

Home revision18 and WorldMap revision4 were rebuilt through the existing owning builder with successful serialized readback. A combined builder command rebuilds Home then World Map. The component is reusable for later page revisions; this task migrates these two screens only. Gameplay and other pages were not migrated.

## Verification

Unity6000.5.9f1 compile/rebuild passed. Final Verify01: **9 Edit Mode +17 Play Mode =26passed**, zero failures/skips. Covers shared component/art/font/geometry parity, real saved values, maximum integer balances, RU/EN switching, Home and WorldMap actions/raycasts, repeated Home migration, responsive safe areas, locked chapters and drag scrolling. Native Home/WorldMap reference-size, Home tall-safe and WorldMap maximum-balance English captures inspected. Extreme ten-digit amounts deliberately shrink to fit, matching Home's existing policy.

All437 prior Home prefab records and2091 prior WorldMap records preserved; each adds one shared-header component. Scope:10existing Assets files changed, oneC# component/meta added, zero deletions. Changes comprise two prefabs, two source compositions, two views, existing builder, two test files and the shared header PNG. Original GUIDs preserved. WorldMap's active panel-border highlight and all other existing artwork are unchanged.

An initial builder attempt encountered Home's existing custom Graphic when trying to add another Image; the adapter now reuses that existing Graphic. Final build/tests passed. After verification, only empty-name trailing whitespace in Unity YAML was normalized; no serialized values changed. Scoped whitespace check passed.

## Artwork and limits

The shared background is the already-approved repaired joined header, copied to Home's established `source_header` resource. No generation, new Scenario API calls or credits. Other approved source artwork and embedded font assignments are reused. Exact source-font identity remains unverified. Page scenery and title layers surrounding the transparent header remain page-specific.

No full-suite, Android APK, physical-device, performance or backend validation. Review actual Unity captures in the standalone HTML. Next: review shared header, then continue Chapter Map in the approved screen sequence.
