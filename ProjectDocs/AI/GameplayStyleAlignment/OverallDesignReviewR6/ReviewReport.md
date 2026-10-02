# Gameplay R6 — overall design review

R6 establishes the requested structure, but the board, gem materials and HUD ornament still differ substantially from the latest reference. The user's “looks great” acknowledges the direction; the same message requests further correction. This review does not declare the design finished or implement a new revision.

[Inspect the existing Unity portraits](Gameplay_R6_Design_Review.html). These are unchanged R6 captures, not new renders or proposed artwork.

## Reference and current implementation

The comparison target is the newer 941×1672 night-village image attached in the conversation: eight columns, seven rows, red moves panel, parchment missions, wooden score panel and four wooden booster cards. No verified local copy of this upload is available in the R6 evidence folder. The older `ApprovedScreens/Gameplay.png` is a different design. Findings below are visual comparisons with the conversation attachment, not pixel-difference measurements.

Project root: `/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL`. Recorded Unity: **6000.5.9f1 (b57deb96f08d)**. Active page: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab`; owner: `Step20ProductionUiFix01SerializedPrefabBuilder.cs`, using `ProductionGameplayRuntime`, `ProductionGameplayHudStyle`, `ProductionGameplayView` and `ProductionGameplayLayout`. Runtime resources are under `Resources/ProductionUI/GameplayAligned/`. Gameplay remains revision 6.

The historical ConversationHandoff still describes Splash/Loading. Current source, R6 evidence and the latest user requests establish the active Gameplay scope; historical corrections should not be replayed. Existing local changes were inspected and retained.

## Findings and proposed correction

| Area | Confirmed difference | Recommended refinement |
|---|---|---|
| Booster artwork | Each card contains another complete circular medallion: gold rim plus blue backing. This shrinks the actual object and adds competing frames. | Remove the outer medallion rim **and its backing**, retaining the central hammer, firebird, lion shield and bottle, including their own metal decoration. Use isolated Gameplay-only derivatives on the wood cards. |
| Gem silhouettes | Blue is narrow and straight; green is a tall rectangle. The reference blue is broader, and green is closer to a square emerald cut. Red/purple shapes are broadly related but not exact. | Refine the six individual sprites as a coherent family; match silhouettes before adjusting color. Preserve aspect ratio and tile identities. |
| Gem materials | Oversized relief emblems, heavy gold edges and broad flat faces make the current pieces look like enamel badges. The reference uses smaller relief details and more visible cut-glass facets. Gold looks particularly metallic and orange has an oversized sun. | Use smaller emblems, finer rims and several broad, softly lit facets. Keep the calmer lighting; adding dense fractures or persistent glow would undermine the comfort goal. |
| Board | Count is correctly **8×7**. Current mount is 808×707, outer frame 820×719. Gems nearly touch cell edges. The reference board is visibly deeper, with a stronger carved rim and corner curls. | Rebalance board height and gem padding together. Add convincing dark wood/recess detail and a substantial antique-gold outer frame while keeping internal joints quiet. Keep full-cell input. |
| Header materials | Major sections are present, but the goals parchment is pale/plain, moves decoration is simplified, and name/round plaques reuse a flattened score-panel sprite. The 1402×826 source is stretched to 236×48, 204×40 and 205×174 without protected corners. | Improve the separate red lacquer, parchment and wood surfaces; make purpose-shaped plaques and match frame weight/corners. Keep all text live. |
| Portrait/name | The existing portrait is preserved, but its hanging blue pendant and broad nameplate form a different silhouette from the compact reference portrait and attached badge. | Retain Ivan's exact face; adjust the Gameplay portrait surround/nameplate as a separate component. A hero-level badge must bind real progression data before being added; do not paint in the reference's sample “32.” |
| Score/stars | The score bar is a simple chamfered fill with no milestone divisions; outlined stars have little material depth. Star tips extend below the wood panel, as in the reference, but their integration differs. | Add framed milestone detail and more legible empty/earned star materials. Preserve actual score thresholds and fill semantics; the reference's example score/fill is not live game data. |
| Missions | Two/three slots work, but counts sit below every icon; reference counts sit toward the lower-right. Score/feather/crate mission symbols also differ in apparent size and finish. | Give icons/counts deliberate optical alignment and consistent visual weight, including non-gem missions. Verify both slot counts and longer values such as “2 060.” |
| Footer | Current cards have simpler ornament, narrow gaps and shallow label wells. Long RU names crowd the bottom curls; English also needs two lines. Settings uses a different gear treatment. | Give cards stronger carved edges, more spacing and a label well designed for the actual names. Enlarge isolated objects after rim removal; keep stock badges clear. Refine settings to the same wood/antique-gold family. |
| Typography | Current regular serif strokes are lighter than the reference's condensed display lettering. Small footer text suffers most at phone width. | Tune weight, spacing and label allocation with the authorized font approximation. Confirm glyph readability at final displayed size rather than relying on a large preview. |
| Background/composition | R6 has a broad lake and hilltop castle; the reference has a denser luminous village/canal skyline and different tree framing. Tall-safe-area captures introduce large scenery gaps around the board. | Bring scenery density and framing closer in a clean environment plate, and rebalance vertical spacing for tall phones. Do not stretch a complete UI screenshot into place. |
| Specials/obstacles | Round 6/10 captures still show old bright specials with detached miniature identity gems. Ice adds busy translucent texture; crates/chain belong to the older detailed family. | A subsequent pass should unify special identities, obstacle damage states and effects with the calmer board. Removing footer rims alone does not finish these designs. |

## What removing the booster rims actually requires

`ProductionGameplayArt.Regions` loads `mace`, `firebird` and `livinglight` from the user-supplied isolated atlas `boosters.png`. `seapearl` resolves to `isolated_seapearl.png`. Their permanent rims and blue fields are baked into the pixels. Disabling an Image border component cannot remove them.

The separate armed-selection halo is gameplay feedback and should remain distinguishable. Each current 150×150 booster face sits on a 184×252 card. Creating a rimless object derivative lets the central object occupy more of that area. Preserve fine object edges and its own gold trim; reconstruct an edge only if the backing overlaps it. Inspect resulting alpha on black, white and saturated backgrounds before installation. No screenshot-region art is needed.

Existing named alternatives were checked: Pre-Level artwork still has medallions; standalone hammer/firebird drawings in `obstacles.png` are different designs. There is no verified identical rimless set ready to relink. Old Wave02 UI-region snippets are unsuitable production replacements.

Keep the existing labels, live stock and actions: Bogatyr Mace clears a selected row/column; Firebird targets a 3×3 area; Living Light clears the selected color despite its shield illustration; Sea King's Pearl adds five moves. Do not substitute the reference's different shield/shuffle mechanics.

## Implementation priorities

1. **Board and six normal gems:** establish matching shape, material, padding and carved frame at actual cell size. Current cell anchors already support rectangular display cells; the logical 8×7 grid need not change. Board mount/frame dimensions and responsive-fit constants must change together.
2. **Footer as a complete component:** isolate the existing central booster objects, then finish card ornament, spacing, count badges and two-line label wells. This is the next focused correction to the user's rim suggestion.
3. **Header and scene integration:** refine materials, mission alignment, portrait/name/round grouping, score/stars, then rebalance the background and tall-phone gaps. Keep two missions in rounds 1–5 and three in rounds 6–10.
4. **Specials/obstacle states and animation:** inspect actual activation sequences once the static board family is settled.

Future implementation must update the active prefab and owning builder/runtime together, preserve GUIDs and existing bindings, and verify RU/EN, two/three missions and both reference/tall-phone layouts. These recommendations are not yet generated, implemented or tested as a new revision.

## Checks performed in this review

- Read root instructions, handoff, CurrentTask, reference provenance and R6 comparison/report; located the sole project AGENTS.md and verified ProjectVersion.txt. Inspected current tracked/untracked working-tree state before writing documentation.
- Verified **1,675/1,675** recorded R6 production hashes and **63/63** recorded evidence hashes before review. Rechecked after the documentation changes; see [preservation evidence](QA/PreservationChecks.json).
- Directly inspected four unchanged actual Unity portraits: reference-size RU, 576×1280 simulated-safe-area RU, round 6 RU and round 10 EN. Inspected runtime/builder sources, board sizing, sprite mapping and font configuration. An independent booster audit directly inspected 15 artwork files and checked SeaPearl alpha components. A small lower-right gold shape in SeaPearl may be a neighboring-art remnant; verify during isolation rather than treating component connectivity as proof of a clean object.
- Checked the new review viewer's four existing portraits, local links and 360px browser layout; [browser results](QA/StandaloneReviewBrowserChecks.json). This is a documentation viewer check, not Unity rendering.

The shared typography asset currently has unassigned RegularFont/StartFont and uses a development fallback resolver. Thus the displayed serif is an approximation and cross-device/release font consistency remains unverified. The user's unknown-font approval permits an appearance match; it does not establish a packaged, identical font. No font/settings changes were made.

**Not performed:** image generation/editing, production edits, prefab rebuild, new Unity compilation/EditMode/PlayMode tests, fresh captures, pixel-difference analysis against the missing local target, animation/video inspection, performance or long-session comfort testing, Android build or physical-phone testing. R6's earlier 7/7 EditMode and 25/25 PlayMode results are historical evidence and were not rerun here.

Changed files for this review: this report, the standalone review HTML and its QA script/results, plus a prepended entry in `CurrentTask.md` and `SessionLog.md`. All prior decisions and implementation records remain.
