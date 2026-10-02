# Friends List — World Map Footer R6

The user requested World Map metal borders instead of the older Friends List footer medallions.

## Implementation

- Reuses World Map `nav_guild_blank`, `nav_shop_blank` and `nav_map_blank` sprites directly; no copied or regenerated metalwork.
- Shop retains its complete approved icon. Map, Events, Friends and Characters compose approved symbol interiors over the shared rims through a native masked mesh. The old donor rims and Events notification badge are excluded.
- Only Friends uses the blue selected rim. The other four destinations use antique-gold rims.
- Russian and English captions use the installed title font as live localized text. This does not claim exact reference-font identification.
- Footer medallions retain their original proportions, scaled uniformly to 174 reference pixels high to fit the Friends footer. They stay six reference pixels above the safe bottom edge.
- Preserves World Map, Shop, Events, Friends and Characters routes and existing sample-data behavior.
- Active prefab, runtime, owning builder and cutover revision are 7. No PNG or font files changed; World Map itself is unchanged.

## Verification

Final Verify06 passed **15 Unity checks (2 Edit Mode + 13 Play Mode)**, including World Map regressions and all five footer routes. Fourteen actual Friends List captures retained; reference Russian and tall English visually inspected. All nine HTML viewer choices loaded without browser errors or horizontal overflow. Seven existing Assets files changed, one C# component/meta pair added, no Assets deleted. All PNGs, fonts and World Map assets/prefab remain unchanged.

Iteration history: Verify01 caught a live caption exceeding its rectangle. Verify02 passed the previous checks, but visual inspection caught missing CanvasRenderers on the emblem overlays (the three original guild crests remained visible). Added the required renderers and an actual mesh-presence assertion. Verify03 caught an outdated test API signature; updated to Unity 6’s parameterless CanvasRenderer.GetMesh. Verify04/05 exposed display-scale-dependent glyph rounding. Captions now recalculate a single-line font size when display scale changes and reserve clearance inside the metal nameplate.

No Android build, physical-device, backend or full-project test run is included. This change deliberately adapts the Friends footer away from the original Friends reference to the user-approved World Map metalwork; it is not a claim of pixel identity with the original Friends image.
