# R5 panel resources — integration measurements

Four newly painted source PNGs are saved under `../Sources/`. `R5PanelPrompts.json` records the complete prompts, tool output paths, SHA-256 hashes, and exact-original-byte comparisons. Built-in `image_gen.imagegen` was used once per asset. No screenshot crops, extraction, or post-generation pixel edits.

All four sources are 2172 × 724. Measurements below use alpha ≥ 245 to exclude negligible fringe pixels. Rectangles use inclusive source coordinates.

| Asset | Near-opaque artwork bounds | Surface / placement |
| --- | --- | --- |
| `hud_frame.png` | Entire 2172 × 724 canvas | Intentionally opaque navy-black lacquer; direct full-width header backing, 941 × 130 logical pixels. |
| `resource_capsule.png` | x26–2145, y99–577 | Whole sprite must account for 13.7% top and 20.2% lower transparent padding. Scale source to desired visible capsule width/0.9761 and visible height/0.6616; offset by scaled x26/y99. No crop. |
| `quest_panel.png` | x26–2145, y49–658 | To obtain a 587 × 166 visible panel, use a whole-sprite rect approximately 601.4 × 197.0, shifted −7.2 horizontally and −13.3 vertically from desired visible top-left. |
| `footer_base_generated.png` | Entire 2172 × 724 canvas | Intentionally opaque connected base behind separate mail, quest and gift; display 941 × 222. Carving enters around source y56%; central sapphire spans y72–89% (roughly footer y160–198). |

The quest well should end around footer y166 so the continuous bottom carving and central sapphire remain visible. Existing separate mailbox/gift circles may overlap the base; the underlying connected carved foundation extends through their gaps to the physical bottom. Keep the panel's quiet central field for live localizable heading, progress and reward values.

Decoded alpha checks: capsule has 592,897 zero-alpha pixels, quest 354,643. Both have alpha253 at center; every central50% pixel is alpha≥245. Header and footer are fully opaque RGB by design. Transparent corners of capsule/quest are all alpha0. All original source bytes are preserved.

Direct black/white/magenta inspection passed in `PanelAssetsAlphaReview.png`; no visible rectangular matte or lost opaque well. This is offline source-resource QA, not a Unity screenshot or page implementation. Unity layout and readability remain for the integrating owner to verify.
