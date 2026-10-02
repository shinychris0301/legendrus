# Home R7 — Shortcut tone and label clarity

User reported that the new Ranking, Team and Friends icons were brighter than Home Map/Shop and their captions appeared blurry.

The three shortcut Images now use an opaque RGB tint of 198/190/180 (approximately 78% red, 75% green, 71% blue) for restrained bronze/gold highlights. The image files and alpha remain unchanged. This is a reversible Unity rendering adjustment; no Scenario calls or new generation were needed.

Shortcut captions use the existing approved PlayerNames font also used for live Home navigation captions. The earlier heavy Title font is replaced only for these three captions. Each caption now renders at native size 18 and unit local scale, instead of size 36 at half scale. Text remains live and localizable; the caption box is centered in the navy plaque, with single-line rendering. No outlines/shadows were added. Actual font identity relative to the original remains unverified.

Updated the existing owning Home source composition, view revision 21, and actual Home prefab. Existing builder calls the same composition and verifies current revision/bindings. Layout positions, icons, routes, Back behavior, feedback and Loading resources are retained.

Initial Verify01 passed behavior checks, but visual inspection found a separate capture-fixture defect: its explicit label array omitted the three new shortcut captions. Synchronous Canvas size changes kept stale low-resolution glyphs, especially in the initial Russian capture. The fixture now discovers all descendant Text components for scale refresh and containment checks. This fixes the review evidence and broadens existing label checks; it is not a new font asset. Final results follow after fresh captures. No Android/device or full-suite testing is claimed.

Final Verify02: Unity 6000.5.9f1 prefab rebuild/readback plus 8 Edit Mode and 26 Play Mode tests passed (34 total; no failures/skips). Captures now refresh all descendant Text components at each simulated screen size. Final default RU and tall-safe EN inspected: captions readable and within plaques, darker metal closer to Map/Shop. Eight browser views passed, including two explicitly labelled R6 comparison views. Source whitespace passed.

Four existing Assets files changed: Home prefab, source composition, view revision, and PlayMode capture fixture. No Assets added or deleted. All existing PNGs, font files, metadata and other-page prefabs unchanged by SHA. Earlier Verify01 behavioral pass and blurry capture evidence retained under QA; it is superseded by Verify02. No physical-device, Android, full-suite or backend verification.
