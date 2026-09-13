# Independent avatar and Firebird QA

Inspected both full selected sources and the unchanged sprites rendered over black, white and saturated magenta in `AvatarFirebirdReview_panels.png` and `AvatarFirebirdReview_small.png`. Both PNGs have actual RGBA transparency. No source pixels were edited. These are offline diagnostic captures, not Unity page captures.

`avatar_ring.png` is 1161 × 1355. Its transparent opening is enclosed and clean. A read-only four-connected flood fill at alpha <= 20 found source bounds `(191,176)` through `(970,986)`, width780 and height811. At a180 × 210 frame this maps approximately to `(29.61,27.28,120.93,125.70)` with center `(90,90.2)`. The existing134 × 134 portrait mask covers that opening geometrically. Verify that the frame draws above the portrait: a portrait mask beneath a frame Image in the hierarchy renders after the parent Image and can cover the inner rim. This draw-order concern was sent to integration for verification/correction.

`home_firebird_generated.png` is1024 × 1536. Its current430 × 645 layout preserves the exact2:3 source aspect ratio. At this scale, source pixels with alpha >= 32 occupy local bounds approximately `(15.96,6.30)` through `(424.96,637.44)`; alpha >= 245 occupies `(47.03,7.14)` through `(419.92,634.92)`. Approximate hand-observed local placement landmarks are claw center `(172,274)` and beak `(75,71)`. At current page position `(470,490)`, its near-opaque right edge is890 and tail bottom about1127, inside the941-wide design canvas. Feather wisps and small light particles intentionally use partial alpha.

The long tail, gold/orange feather layers and sapphire eye accents read clearly at430 × 645. No source stretching or alpha edit is recommended. Final overlap with Ivan, controls and safe areas still depends on the Unity capture.

Exact decoded alpha counts/bounds and avatar scan lines: `AvatarFirebirdGeometry.json`; PNG hashes, histograms and diagnostic review: `AvatarFirebirdAlphaChecks.json`. Only QA additions were made in this subtask. No production edits, Unity tests or APK build were performed.
