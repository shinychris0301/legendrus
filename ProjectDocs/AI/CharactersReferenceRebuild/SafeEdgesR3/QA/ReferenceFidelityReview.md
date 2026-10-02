# Characters R3 — Responsive layout review

The original 941×1672 composition and R2 lettering remain the visual baseline. R3 adapts placement to the actual simulated safe rectangle.

| Area | Assessment |
| --- | --- |
| Header / footer | Safe top and bottom are asserted within1px at every captured size; no detached centered navigation bars on tall phones. |
| Collection | Taller viewport reveals more cards; reference-size view still clips the future row until scrolling. No card/portrait stretching. |
| Frame | Upper ornaments unchanged; lower ornaments follow footer; thin side trims extend between them. |
| Modal | Centered vertically; expanded shade blocks header and footer; Close remains accessible. |
| Art / source text | All PNGs preserved byte-for-byte. Original source title/buttons and exclusive Ivan highlight retained. |

Inspected tall RU, short RU and tall Info captures. Source-reference-size comparison is in `ReferenceSizeComparison.json`; full native captures and before comparison are available in the standalone review. Existing live font/reconstructed scenery differences remain. Tall layouts may show spare dark collection space after the final available row. This is native Unity output at simulated phone sizes, not physical-device verification.
