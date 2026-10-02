# R9 art coherence review — 2026-09-26

**Result: revision needed.** R9's art follows the surrounding Russian-fantasy UI palette, but does not yet form a coherent board-piece family with the approved gameplay gems. Passing mechanics tests did not establish visual acceptance.

Reviewed existing actual Unity captures: [mixed obstacles](../Captures/Mechanics_ObstaclesR9_Mixed.png), [chain cells](../Captures/Mechanics_ObstaclesR9_Chains.png), [damaged seal](../Captures/Mechanics_Seal_Cracked.png), and bomb/directional states. Compared the current sapphire sprite with the generated bomb, directional piece and crate at source size, then checked render mappings and obstacle-layer code. These are R9 captures, not a new runtime verification.

| Priority | Finding | Recommended correction |
|---|---|---|
| High | Gems use large saturated color areas, broad facets and clear edges. Bomb/crate/seal use dense engraved metal, surface grain and numerous small highlights. Those details merge into speckled texture at board size. | Use the actual approved gem sprites as visual references. Simplify surface detail, keep broad highlights, a dominant material/color and one large emblem. Retain the fairy-tale theme without transferring panel-scale filigree onto every piece. |
| High | Bomb and directional piece read smaller than neighboring gems. Internal transparent margins and their silhouette proportions produce inconsistent apparent size even with a stretched UI rectangle. | Normalize visible silhouette coverage per piece against the actual gem at in-game size. Keep a compact bomb and a thick, unmistakably directional crystal form; prevent tips/fuse from forcing the main body too small. |
| High | Outlined cream numbers sit on obstacle corners, competing with both the obstacle edge and board rails. | Remove on-board layer numerals together with improved damage-state artwork. Retain objective counts in the header and actual remaining-hit logic. |
| High | The damaged state is currently the same sprite with a procedural fissure; ice additionally changes opacity. This is too subtle in the captured board to carry remaining-stage information alone. | Make material-specific intact/damaged appearances: reinforced crate to visibly broken band/exposed planks; thick ice to a thinner, broadly fractured shell; intact seal to broken rim and split rune; where a chain has two layers, show a broken link/remaining fastening. Single-layer obstacles start in their final removable state. |
| Medium | Chain and seal read like tiny ornamental panels; the crate's illustrated depth and microtexture differ from the gems' simpler presentation. | Use a consistent front-facing camera, lighting and edge treatment. Preserve an open chain center and readable gem beneath ice. Obstacles should still read as blockers, not become interchangeable with matchable gems. |
| Medium | The new family is incomplete: Burst/line use R9 art, Spark reuses the inventory Firebird, and Color still uses the earlier special asset. | Assess all on-board specials together. Preserve approved inventory icons at their intended UI size; create consistent board-scale variants only where necessary. |

## Code evidence

- `GameplayCellMechanics.cs`: creates `ObstacleLayers`, 22-point bold text with a 1.5-pixel dark outline; enables it for obstacles above one layer and for RunicSeal even at one layer. The numbers are live overlays, not baked into generated art.
- `PrototypeObstacleArt.cs`: loads a single sprite for each kind; toggles `DamageFissures` at one layer. Ice uses 0.67/0.46 opacity for intact/final layer.
- `PrototypeObstacleDamage.cs`: crate and seal largely share the same zigzag crack; chain has a small lock scratch. Distinct damaged illustrations do not yet exist.
- `Match3Board.RefreshCell`: hides the underlying gem for specials, routes Burst/line to R9, Spark to shared Firebird, and Color to the existing color-special sprite.
- Current sapphire and R9 bomb both use bilinear filtering with mipmaps disabled. There is no evidence here that a filter toggle alone would solve the aesthetic mismatch. Imported platform overrides also differ; the R9 report's blanket 512-pixel importer description should not be treated as a verified Android limit.

## Next pass: R10 — board-piece style and damage readability

Start with one bomb, one directional piece, and a two-stage crate using the approved gems as image references. Review them in a real board composition at normal phone scale before extending the treatment to ice, chain, seal and other specials. Use simple, readable silhouettes and matched apparent size, lighting, edge sharpness and highlight density. Check transparent assets against black, white and saturated backgrounds.

Remove obstacle numbers when intact/damaged states can be distinguished without them. Verify both hit stages and final removal, plus goal totals and special identity. Keep the accepted difficulty, layouts, move budgets and R9 animation behavior unchanged.

This task is a review only: no artwork, runtime source, prefabs, import settings or game rules were changed. No new Unity test run was needed or claimed. The next art pass remains unimplemented.
