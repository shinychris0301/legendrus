# Full reference comparison — Team Ranking R2

Reviewed 2026-09-23 against the dedicated 941×1672 original and stored R2 native Russian capture at the same resolution. Checked the current layout source and revision-3 cutover. This is a review of the stored verified implementation, not a fresh Unity run.

**Verdict: recognizable composition, but not a faithful visual finish.** R2 resolves the reported overlap by adding clearance; it also makes the reward content too small. The earlier overflow-fix statement should not be interpreted as reference fidelity acceptance.

## Findings

| Area | Observed difference | Priority |
| --- | --- | --- |
| Podium reward icons/counts | Smaller than the original, with excess empty space. Separate heavy rectangular slot frames differ from the reference’s shaped, closely fitted reward enclosure. | High |
| Ranking rewards | Strongest scale mismatch: icons and counts are much smaller than the source. The highlighted rails are visible now, but shrinking all contents reduced legibility and visual emphasis. | High |
| Podium ribbons | Names clear the folded ends now. However, the first-place ribbon is flat navy/black instead of the original warm reddish textured surface; ribbon lighting and the heavier live lettering differ. | High |
| Table surface/borders | Current repeated rows have thick bronze borders and patterned grey/navy faces; the original is darker, cleaner and has finer continuous gold separators. | High |
| Guild names/type | Several names wrap to two lines, unlike the source. Live headers and numbers are wider/heavier. Selected font files remain approved approximations; exact identity is not established. | Medium |
| Player-guild highlight | Border is unobstructed, but the bright blue patterned fill and long example plaque differ from the original subtler dark face and short title. Preserve the example disclosure while refining its placement. | Medium |
| Tabs | Four standalone rectangular frames differ from the source’s paired elongated tabs and ornamental ends. Current selection state is a functional preview choice. | Medium |
| Scenery/footer | Forest background and generic carved footer differ from the original navy surface, side scrollwork, red banners and candles. Back is an intentional functional addition. | Later pass |

## Closest elements

Original Russian page-title lettering and ornamentation, the three large guild crests/rank medallions, seven row crests, podium arrangement, and the extracted Details button retain the recognizable reference appearance. This does not establish exact alpha or texture fidelity. Major table and footer positions are close at the reference aspect ratio.

## Intentional adaptations

Shared Home header and approved sapphire reward asset are retained by user instruction. Saved header values differ from the reference’s example values. Preview notices, unknown season dates and non-live ranking data are honest feature-state disclosures, not visual defects to remove. English remains live/localizable; safe-area positioning adapts on taller phones.

## Recommended next implementation

**R3 — Reward Proportions and Ribbon/Frame Restoration.** First restore the ribbon faces and the original reward enclosure geometry. Then enlarge icons and counts toward the reference within measured usable interiors, preserving space around the highlight rails. Use the existing approved gems, rainbow rewards and chests; do not regenerate them or simply undo R2’s padding. Refine table materials and single-line guild-name fitting next.

Review only: no runtime/assets changed, no Scenario requests, no new Unity tests. R2’s 26 passing tests prove the covered behavior and clearance, not visual similarity.

---

## Earlier targeted overflow assessment (retained)

# Targeted fidelity review — R2

Compared actual native Russian and narrow English captures with the preceding R1 and dedicated original.

- **Podium names:** smaller fitted text stays on the ribbon face, clear of folded ends and upper/lower ornamentation.
- **Podium rewards:** icons and counts are separated and inset; counts no longer touch the lower border.
- **Ranking rewards:** inset cells and smaller icons/counts leave the row's upper/lower gold rails visible, including rank 7's highlight.

This resolves the reported layout overflow. Artwork was not modified. Existing differences in ribbon-face texture, table carving, selected-font weight, wrapped table guild names and surrounding scenery remain. Full-page reference identity is not claimed. Narrow captures verify fit at simulated phone sizes; physical-device readability has not been tested.

Recommended later refinement remains original table-frame material and single-line guild-name fit, preserving the clearance established here.
