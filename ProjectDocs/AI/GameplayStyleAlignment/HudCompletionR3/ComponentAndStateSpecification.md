# Gameplay component and state specification

The user authorized starting the completeness review's recommendation. This revision implements the first bounded pass: full upper/lower HUD presentation and Gameplay status messages. The action and obstacle states below define the next pass; their artwork/effects are not implemented or visually approved by this document.

| Component | First-pass specification |
|---|---|
| Header | Separate blank opaque charcoal/navy lacquer panel, thin layered antique-gold rails and carved leaf ends. Reuse current Ivan portrait/ring, enlarge the visible portrait and balance name, level, stars and moves. Preserve character source pixels. |
| Moves | Reuse the R2 medallion with balanced centering and a larger cream-gold live count; no static reference number. |
| Goals | Two live slots with larger supplied symbols, counts at the lower right. Score uses a faceted gold star; survival an hourglass in the existing dark gold-rimmed medallion. Counts and completion remain authoritative. |
| Footer | Keep four supplied booster medallions and controls. Raised emerald stock badges use larger cream faces with a short dark relief shadow. A separate score frame carries large sapphire side jewels, carved leaves and an opaque textured bronze well. A dedicated circular gold cog replaces the visually thin Gameplay gear; Splash's resource remains unchanged. |
| Messages | One native lacquer slot above the board, below the upper medallion, outside touch controls. New text replaces existing text; no stacked coroutines/rectangles. Arming instructions persist while armed; transient messages expire after 2.2 seconds, the existing display duration. Russian wording retained; known Gameplay messages have English equivalents and update with language. Clear on page retirement and silent disarming. |
| Live semantics | Keep moves, stock, remaining goals, score and thresholds bound to existing services. The bar under Ivan continues to mean average goal progress; this pass does not redefine it as hero XP. |

The board mount, cell dimensions, six tile identities, normal gem colors, supplied booster imagery, environment, level data, resolver and navigation remain. Header/footer assets are independent generated components, not sampled reference regions. Transparent exterior cleanup preserves retained source RGB; all writing surfaces stay opaque. Existing fonts are reused; exact reference font identity is not established.

The following state plan is the continuation specification, not finished runtime work:

| Supported item | Target/readiness | Activation and completion |
|---|---|---|
| Mace | Show a narrow row preview for tap and a column preview for vertical swipe; retain hammer medallion. | A short hammer impact, gold directional trail, contact sparks and restrained fade. Emphasize only affected cells. |
| Firebird | Mark the real 3×3 footprint with a warm feather edge; keep unaffected cells readable. | Feather/bird motion, shaped flame burst and dissipating embers centered on the chosen cell. |
| Living Light | Keep the reference lion-shield medallion. Preview the selected gem color to communicate its actual clear-color action. | Pale-gold rays to actual affected gems, brief matched-color edge response and small completion flash; no opaque board wash. |
| Sea Pearl | Keep the reference blue bottle medallion and instant-use behavior. | Blue stream from button toward the moves medallion; localizable +5 accent, then restore the stable counter. No targeting step. |
| Row/column specials | Prefer the supplied matching horizontal/vertical blue-gold, red-tipped rocket pair for a consistent directional family. Keep a readable indication of the underlying match color. | Directional launch/light trails and matching impacts. The supplied blue banded crystal is an alternative family, not one direction of the rocket family. |
| Burst/color specials | Keep the gold ruby and rainbow medallion identities. | Separate creation, anticipation, triggering and removal effects. Use the supplied glow sheet as artwork guidance; never paste the reference's full central cross over an idle board. |
| Normal gems | Preserve the supplied unmounted distinct silhouettes. | Local selection rim, small material glint, and matching hit/clear state; controlled brightness instead of tinting the entire board. |
| Ice | Three readable durability stages without losing the gem beneath. | Edge frost, cracks, hit response and final shards. |
| Chain | Visible intact, strained and nearly broken stages. | Link movement/sparks and a clear unlocking/break release. |
| Crate | Wood for the final layer, visibly different reinforced three-/two-layer stages. | Contact dent/splinter response, reinforcement loss, final break pieces. |
| Feather objective | Preserve the actual movable objective and its exit route. | Collection trail from the real exit to its goal slot, then a small counter confirmation. |

Every booster also needs explicit armed, canceled, disabled, empty-stock and rejected-target treatments. Keep existing refund/spend semantics, input locking and VFX budgets. Some motion events currently omit affected-cell lists; presentation must obtain actual affected positions before adding per-cell trails. Stone and spiderweb remain separately scoped features because no corresponding obstacle mechanics exist. Bomb/lightning alternatives are available source drawings, not new gameplay kinds.

After this HUD pass, implement the supported action/obstacle states, then the tutorial and Pause overlay styling. Final verification must cover action/damage states, short/tall safe areas, Russian/English, input access, lifecycle cleanup and source/GUID preservation, alongside a whole portrait reference comparison.
