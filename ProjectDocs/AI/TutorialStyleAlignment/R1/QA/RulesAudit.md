# Instruction source audit

- `GameplayCore/BoardModel.cs::DetermineSpecialCreations`: four horizontal/vertical -> Row/Column; straight five+ -> Color; intersecting runs -> Burst.
- `GameplayCore/BoardResolutionPlanner.cs`: Row/Column clear their line, Burst uses balance radius1 (3×3), ColorWithNormal targets swapped gem type. Adjacent matched cells damage crates in four cardinal directions; direct clear damages any affected obstacle. Each wave deduplicates obstacle hits.
- `BoardModel.DamageObstacle`: removes one layer per hit. Hits are not a guaranteed count of player moves; cascades can create further waves.
- `Runtime/UI/ProductionPreLevelView.cs::RefreshBoosterDetails` and booster planner: Mace row tap/column vertical swipe; Firebird3×3; Living Light selected color; Sea King's Pearl+5moves.
- `Runtime/UI/ProductionGameplayArt.cs`: reuse current gems/line specials/burst/color and inventory art, plus existing crate/ice/chain resources.
- No game rules, level missions, in-round tutorials, inventory transactions or reshuffle behavior changed.
