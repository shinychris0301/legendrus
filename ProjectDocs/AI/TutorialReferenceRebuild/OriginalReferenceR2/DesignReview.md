# Tutorial original reference — design review R2

The user supplied a dedicated Tutorial screenshot and requested design review before implementation. Found and visually verified `/home/k2/Downloads/1/Fantasy Match-3 Tutorial Screen.png`; it matches the conversation attachment. This replaces the earlier DesignR2-generated concept as the visual target. New image generation is unnecessary because a source now exists.

## Recommended design

Adopt the source's shared player header, navy “ОБУЧЕНИЕ” plaque, three parchment sections, rewards tray and green/blue bottom actions. Keep the navy/antique-gold/sapphire/parchment material family. Preserve the whole panel silhouette and ornaments; avoid background patches covering rails or captions touching trim.

The review displays the unchanged source and the prior native Unity page separately, one portrait at a time. It is a design assessment, not a generated mockup or a new Unity capture. Proposed factual corrections are listed beside the source rather than silently editing it.

## Gameplay corrections

Verified `Scripts/GameplayCore/BoardModel.cs::DetermineSpecialCreations`: straight 4 produces Row/Column; straight 5+ produces Color; intersecting runs produce Burst. Consequently the source's Firebird/hammer combination labels and images cannot be copied as tutorial rules. Use the current special-piece illustrations and labels. Inventory boosters remain separate consumable items.

The supplied shield objective is not a supported “protect shield” goal. Use existing crate/collection examples. Keep the move counter illustrative and label it as an example. Rewards are examples, not guaranteed chest/currency grants. All visible account resources must come from the existing save.

## Native implementation after review

- Update the existing TutorialOverlay runtime, view, layout, builder and active prefab together; keep cutover revision consistent.
- Reuse `ProductionPlayerHeader`, Gameplay gems/grid, approved special pieces, Home gift and currency assets; do not regenerate these.
- Isolate original title, parchment/frame, section plaque, reward tray and button contours using the established Scenario workflow. Preserve intended opaque panel faces, remove original instructional content only in bounded regions, and inspect alpha over black/white/magenta. Derive a clean plate; never use the whole reference as a runtime page.
- Main entry becomes the three-section overview. Retain the existing detailed explanations through a secondary guide view rather than deleting booster/obstacle information; keep it visually subordinate to the overview.
- Keep heading/body/button labels localizable, with the approved fonts. Exact source-font identity remains unverified.
- Done/Back return to origin. First Level uses the existing pre-level route, progress and energy rules. Do not spend energy on opening Help or pre-level. Avoid silently discarding a paused round; use a localized return-to-round action in that context.
- Fit to safe edges and let the parchment content scroll on constrained screens; preserve ornament clearance and touchable controls.
- Verify RU/EN, first-level route and energy gating, paused-round preservation, source/overlay return routes, unchanged inventory/progress, native button presses and panel boundaries. Save real Unity captures and distinguish them from this review.

## Current phase and scope

Design review complete and ready for user review. No production edits, source extraction, Scenario jobs or Unity run in this phase. The previous Tutorial R1 remains active. Original and current capture were copied into this task folder without modification. Production hashes checked against the snapshot.
