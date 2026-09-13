# Gameplay R1 — comparison with the supplied material

> Follow-up design audit: alignment is partial. [Confirmed missing styling and evidence](QA/DesignAudit/DesignImplementationAudit.md). This historical implementation report does not establish visual acceptance.

The reference is `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Gameplay.png` (941×1672). The user’s five 1448×1086 sheets were found locally, visually matched to the attachments and preserved under `Sources/`; exact paths and hashes are in `UserAssetProvenance.json`.

| Reference feature | Implementation |
| --- | --- |
| Five distinct faceted coloured gems | Exact supplied ruby, sapphire, emerald, amber and amethyst atlas regions; source RGB/alpha unchanged. Normal tiles retain their distinctive shapes. |
| Shield and medallion tile designs | Supplied blue-and-gold shield represents the existing sixth tile type. Extra brown shields/medals were not introduced as new rules or interchangeable colour identities. |
| Thin gold grid, 8×9 board | Native lacquer cells and gold bevels; full cell input roots retained. Board dimensions remain data-driven and keep their reference proportions on taller displays. |
| Ivan portrait, gold ornament, dark dusk town | Whole accepted Home portrait/ring with the same native circular mask. New continuous environment and empty panel; darker environment tint recorded in the owning builder. |
| Moves, selected level, two goals, score | Live labels from the actual board and selected level. Goals show the correct gem, obstacle, feather, score or survival identity. Example level 28 and 15/18 targets are not hardcoded. |
| Four ornate booster medallions and raised green counts | Supplied mace, Firebird, shield and potion artwork; saved stock, existing targeting/consumption, double-invocation protection and native raised emerald badges. |
| Ice, chains, crates, feathers | Supplied ice/crates/feather; separate generated chain overlay avoids covering every gem with a baked red gem. Layer counts reflect actual durability; feathers move with their cell visual. |
| Row, column, ruby burst and colour special | Supplied individual special art, attached to the existing special-kind state. A small untinted gem marker preserves the underlying match colour because the special artwork has fixed colours. Existing motion/VFX timing and pooled effects retained. |
| Bright cross-shaped ruby action | The reference depicts a mid-action effect. No permanent giant flash or invented action is painted over an idle board. Alternate glow-state sheets remain available as source material for a later VFX refinement. |

Normal art is not repainted or colour-shifted. Three runtime atlases are byte-for-byte copies of the supplied sheets. Four crowded asset regions use separate isolated PNGs: only foreign alpha components are removed, without RGB changes or resampling. They are asset-sheet extractions, not screenshot regions. Originals and alpha proofs remain reviewable.

The result follows the reference composition and material family; it is not a pixel-identical copy. New scenery, panel filigree and chain overlay are recreated. Exact source-font identity remains unverified; approved local font assignments and gold text treatment are reused. The existing tutorial and Pause modal retain their earlier visual design. The current page is implemented for review; user acceptance is pending.
