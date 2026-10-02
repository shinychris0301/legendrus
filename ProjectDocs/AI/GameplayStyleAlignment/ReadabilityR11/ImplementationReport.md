# Gameplay readability R11

[Actual Unity review](Gameplay_Readability_R11_Review.html) · [Changed files and preservation audit](QA/SourceAudit.json)

The grid junctions and line-special silhouettes have been strengthened for the 438×780 size of the user's screenshot. Canonical Gameplay prefab and owning builder are revision11. Visual acceptance is pending.

## Findings and changes

R10's 1.45-reference-unit gold rails were about 0.67pixels thick at 438 px, and its 6.2-unit rivets about 2.9pixels across. This makes raster sampling a plausible contributor to the apparent gaps. Runtime source also placed pooled effects above the grid, allowing match washes to obscure it. A screenshot alone cannot prove which contribution caused each reported gap; no persistent grid deletion was reproduced.

Gold rails are now 2.6units with stronger shading, and rivets 10units across. One shared grid still owns all 72junctions. Its layer stays above pooled match effects during initialization, reuse and bring-to-front operations; raycasts remain disabled. Input and matching rules are unchanged.

Line-special bodies now occupy 58% of the gem visual's height using relative anchors. This is thicker than R10's 2.1:1 footprint, and remains valid if layout has not yet resolved or changes later. The same existing colored sprite rotates 90° for columns. No arrows, secondary fragments, new glow or replacement artwork were added. The generated sprite files remain byte-identical.

## Five-gem rules, verified from production code

| Match | Created booster | Function |
|---|---|---|
| Four in a horizontal line | Row special | Clears its row when activated |
| Four in a vertical line | Column special | Clears its column when activated |
| Five in a straight line | Rainbow / color special | Swap with a normal gem to clear that gem's color across the board |
| T or L intersection | Burst special | Clears a 3×3 footprint when activated; board edges clip the footprint |
| Two rainbow specials swapped together | Combined color special | Clears the board's nonempty cells through existing obstacle rules |

A regular row/column/burst special activates through matching or a special combination, rather than simply tapping it. The crossed gold shapes in the supplied screenshot are chain obstacles. Existing central rainbow/burst artwork and all obstacle rules were retained.

## Checks performed

- Verified all 1,733 R10 baseline hashes before editing; saved backups and working documentation. Project version and actual Editor 6000.5.9f1 verified; Editor closed before batch work.
- Two canonical Gameplay rebuild/readback runs succeeded. Final UI EditMode **8/8** and Gameplay/Pre-Level/goal/progression PlayMode **28/28**, no skips. Run 1 also passed; inspection found a capture still showing transient feedback, so final run 2 separately captures that state and fully settled play.
- The new runtime fixture performs 12 real resolved swaps, verifies persistent grid visibility and ordering, and captures438×780. Rendered-pixel checks verify visible gold at all 42 interior intersections. Both active-feedback and settled captures pass. Arranged six-color row/column specimens verify rotation, no companion fragments and a short-side footprint exceeding 22 pixels at 438 px.
- Five focused rule tests **5/5**: straight-five creation, T/L burst creation, rainbow with a normal gem, and rainbow with rainbow. Two focused VFX tests **2/2**: bounded prewarmed pool and non-raycast, clipped effect-layer placement below the grid. Updated the old layer-order assertion to match the intentional new persistent-grid ordering.
- Saved **28 capture pairs**:19 Gameplay and9 Pre-Level. Directly inspected final 438 px settled play,438 px special specimens,438 px feedback and941 px normal play. QA captures use50 starting moves and targets 999; they are explicitly labeled test data, not authored levels.
- Nine existing files changed; 1,724 baseline files remain byte-identical. All 896 existing metas/GUIDs,395 Gameplay and476 Pre-Level object IDs preserved. No new production assets, settings, packages, Android guards or earlier-page changes. C# whitespace check passes. Final production and evidence hashes are recorded alongside this report.
- Standalone browser results: `QA/BrowserChecks.json`; screenshot: `QA/StandaloneReview.png`.

## Not performed

No APK/AAB build, physical Android run, full-project suite, extended comfort test, performance profiling, exhaustive combinations or full human playthrough. The uploaded running build was not directly inspected. This is a verified Unity presentation correction, not a claim of device-level acceptance. Existing font identity, scenery/gem reference differences and central burst/rainbow visual polish remain outside this focused change.
