# Events — Original Buttons R6

Russian action buttons now retain their original green surfaces and Cyrillic lettering: «УЧАСТВОВАТЬ», «НАЧАТЬ», «ОТКРЫТЬ», «В БОЙ». The three category tabs use source lettering over the approved normal/selected tab surfaces. Live Russian text overlays are hidden, preventing doubled lettering. English remains live and localizable.

The user explicitly chose original action wording with adjacent preview/unavailable notes. The first action still opens the existing Event Detail preview; the other three remain unavailable. Notes appear above each button, clear of its metalwork. No participation service, event dates or rewards were added.

## Extraction

Four 225×80 source crops retain original visible RGB without recoloring, generated lettering or upscaling. They reuse the approved action silhouette, bounded to exclude the neighboring card ornament and cleaned of disconnected specks. The opaque green face remains intact. Tab lettering is isolated within text-only source bounds and composited into six state sprites; those composite surfaces are adaptations, not whole original tab crops. The source alert badge is not restored because no live alert exists.

Original source coordinates, hashes and button RGB comparisons are in Sources/Provenance.json. QA/ButtonAlpha.png and QA/TabAlpha.png show black, white and magenta inspections. The existing source textures were not overwritten. No image generation or Scenario request was made.

## Text rendering

Removed oversized, downscaled dynamic text on this page in favor of native glyph sizing. The original font assignments remain; exact source-font identity is still unverified. The review now offers a native-pixel view so browser fitting does not obscure text inspection. A bitmap button cannot recover resolution beyond its source image.

An initial higher-density text trial passed functional checks but looked too thin at narrow phone size; it was replaced before final delivery. Final validation and captures use Verify02.

## Production and verification

Runtime, owning builder, active prefab and cutover agree at revision 9. Existing routes, safe-edge layout and expanded Back target are retained. The prefab owns the availability notes; repeated rebuild checks verify stable object counts.

Unity 6000.5.9f1 rebuilt/read back the prefab and passed 2 EditMode + 6 PlayMode tests. Checks cover language/state sprite selection, hidden duplicate labels, note clearance, text fit, category changes, expanded Back input, Event Detail return, shared header routes and unchanged gameplay balances. Actual Unity captures are retained separately from alpha inspections.

No Android build, physical-device, backend, performance or full-project suite was run. Existing scenery/frame reconstructions and source-font differences remain. See QA/ScopeAndVerification.json for the exact changed files and QA/BrowserChecks.json for review-viewer validation.
