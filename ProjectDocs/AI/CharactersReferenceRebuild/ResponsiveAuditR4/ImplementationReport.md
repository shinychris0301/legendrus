# Characters — Responsive Audit R4

Continued after Story Dialogues. Fixed undersized resource, rarity-tab and collection-info touch targets while preserving the approved sprites, source lettering, portraits and footer geometry. The runtime/view/layout, owning builder, active Characters prefab and cutover now use revision 16.

## Changes

- Shared-header plus controls use 76×76 reference-pixel targets instead of 44×47. Coins/gems expand symmetrically; energy expands left only to avoid Settings. This changes only the Characters instance.
- Rarity tabs retain their 254×58 art but expand their native targets vertically to 254×76.
- The collection-info plaque retains its 461×35 art. Native raycast padding expands its target to 461×76, upwards, with its lower edge inset four pixels to avoid the footer medallions. The viewport is 45 reference pixels shorter (579 plus extra phone height), leaving a one-pixel separation from the info target. This prevents the enlarged target from intercepting collection drags.
- Collection scroll extent and offset continue to clamp on resize/filter changes. Future cards remain fully revealable by scrolling; tall phones still show all available rows without stretching portraits. No footer/header artwork was moved or resized.

The exclusive Ivan highlight, preview-only selection, disabled upgrades/ownership actions, live balances and real routes remain unchanged.

## Verification

Verify01: Unity 6000.5.9f1 import/compile/rebuild/readback and 14 tests passed (3 EditMode + 11 PlayMode), zero failed/skipped, isolated saves/config. Added native raycast edge taps for resource controls, rarity filters and Info; native card preview/Close, relocalization preserving selection/filter, footer/header/Back navigation and duplicate-action rejection. Target assertions check minimum 44 rendered pixels on the simulated narrow display, safe containment, fixed-control non-overlap and separation from scrolling. Existing drag/reveal, masked-card input, modal blocking, save, current-hero highlight, source lettering, press-cancel and binding tests passed.

Thirty actual Unity captures. Thirteen browser review views and native-pixel toggle passed without script errors/overflow. Inspected reference-size Russian, narrow English and scrolled future cards. All 12 matching R3 captures were compared: six pixel-identical, one differs by nine pixels with max channel delta one, and five reflect the intentionally shortened/scrolled collection. Outside the collection band, differences are limited to tiny one-channel raster variations. Exact counts/bounds: QA/BaselineComparison.json. This is not a claim of full-screen pixel identity for the changed viewport.

Scoped whitespace checking passed after formatting-only normalization of Unity's generated empty `m_Name` values. No further runtime edits followed verification.

## Scope and limits

Seven existing Assets files modified: ProductionCharactersRuntime.cs, ProductionCharactersView.cs, ProductionCharactersLayout.cs, ProductionCharactersPrefabBuilder.cs, Characters.prefab, CharactersCutoverState.txt and CharactersAlignmentPlayModeTests.cs. Other 7,189 files unchanged; none added/deleted. Added this review/scripts/captures/reports/QA evidence and updated CurrentTask.md/SessionLog.md. All image/font/meta resources remain unchanged. No generation or Scenario calls.

No Android build, physical-device, hardware-Back, backend, performance or full-suite testing. Existing scenery/surface/font differences and intentional tall collection space remain. Ownership/upgrades are not implemented; preview taps do not change the active hero or spend resources. Exact source-font identity remains unverified.

Recommended next step: an end-to-end navigation regression through Home → World Map → Chapter Map → Pre-Level → Gameplay → result/return, including relevant modal interruption paths, to check how the audited pages work together.
