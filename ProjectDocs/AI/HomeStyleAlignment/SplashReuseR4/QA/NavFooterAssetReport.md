# Home R4 navigation and footer sprite review

Six selected individual sprites were newly painted with built-in imagegen after direct inspection of the whole original Home reference and the approved Splash crest. All selected PNGs are 1254 × 1254 RGBA and exact copies of tool output bytes. No source image was cropped or extracted, and no generated PNG pixels were edited. Runtime replacement and Unity verification belong to the integration work.

| Asset | Reference features targeted |
| --- | --- |
| `nav_map_generated.png` | Rolled ochre parchment map, blue waterways, open brass compass, heavy clipped-corner gold frame |
| `nav_shop_generated.png` | Three-quarter carved wooden stall with sapphire awning, gold trim and small treasures |
| `nav_events_generated.png` | Angled red parchment scroll, gold spindle, gold sun-wheel seal |
| `nav_characters_generated.png` | Bronze-gold Slavic helmet, sapphire bands, nose guard and engraved cheek plates |
| `mail_medallion_generated.png` | Simple broad circular gold rim, navy center, ochre envelope and red wax seal |
| `reward_medallion_generated.png` | Matching circular rim, closed wood chest with sapphire straps and gold hardware |

Selected files: `../Sources/`. Full prompts, built-in output paths, selected/source SHA-256 values and rejected candidates: `../NavFooterPrompts.json`.

Performed: PNG header inspection; exact generated/source byte comparison; read-only decoded alpha histograms and near-opaque bounds; direct visual review on black, white and saturated magenta at 203 × 222 navigation and 175 × 175 footer diagnostic sizes. Captures are `NavFooterReview_panels.png` and `NavFooterReview_small.png`; these are offline sprite diagnostics, not a page implementation or Unity captures. `NavCaptionGeometry.json` records measured blank bands for live labels.

All selected sources have true transparent exteriors. Navy centers are consistently near opaque (alpha 252–253); native opaque navy backing surfaces must remain. Some nearly invisible alpha 1 pixels extend beyond the artwork, so layout uses the alpha >= 245 bounds rather than any-nonzero bounds. No screenshot scenery or painted checkerboard appears in selected sources.

The first whole-reference-guided map output had RGB color type 2 and a painted checkerboard; it was rejected. A fresh RGBA map with a narrow label plate was superseded by the selected wider plate. Both candidates remain under `Candidates/` for provenance. Eight generation calls produced six selected outputs and two rejected/superseded outputs. CLI/API fallback was not used.

The hand-painted details and metal finish are approximations guided by the reference; this report does not claim pixel-exact color identity or user visual approval. Final live-label fit, page composition and Unity rendering require the integration capture/tests.
