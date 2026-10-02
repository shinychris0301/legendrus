# Map interactions and route spacing R4

The Chapter Map and World Map now provide visible press feedback. Tapping an available round selects it and opens the established story/pre-level flow. Locked-round notices use the approved navy panel surface and gold decorative frame. Chapter Map staggering is narrower, with more vertical space between rounds.

## Implementation

- Both active page prefabs and their source-composition builders are revision 5. New `ProductionMapButtonFeedback` shades the button's artwork, lettering and ornaments while held, then restores them over 120 ms. Hit rectangles and source-art proportions do not move. Pointer ownership, release, exit, disable and Chapter Map drag cancellation are handled. Existing sound/haptic feedback remains attached. Other screens do not receive this component.
- One `ProductionMapNotice` per page reuses `WorldMapReferenceAligned/card_surface` and `card_frame`, with live Russian/English text. It replaces the unframed map toast, updates rather than stacks, fades after 3.5 seconds, never blocks raycasts and clears when the page closes. Authored-but-locked rounds explain the prerequisite; unpublished rounds explain that content is coming later.
- Available node taps save the tapped round and invoke the same guarded `OpenSelectedLevel` route used by Play. Intro/pending story scenes, pre-level booster selection and gameplay energy checks remain intact. Locked or dragged nodes do not start a round. The footer Play button is retained.
- Vertical step increases from 88 to 112 reference units. Alternating horizontal gaps shrink from roughly 245–325 to 140–170 units. Nine complete controls appear at the initial position, with at least eight complete controls while scrolling. Tall phones expand the vertical spacing. The 14-node pool, 500-position range, end bounds and current-round jump are preserved.

No image generation, Scenario API calls or credits were needed: this change reuses approved source assets. No scenery, navigation artwork, font files or gameplay rules were changed. The background remains the approved R3 Scenario landmark painting.

## Verification

Unity **6000.5.9f1** compiled and rebuilt/read back both canonical prefabs. Final **Verify02: 12 EditMode + 23 PlayMode = 35 passed, zero failures/skips**. Results, commands and logs are under `QA/Verify02`; native captures are in `QA/ChapterMap`, `QA/WorldMap` and `QA/Home`.

Checks include:

- Held/released/exit states, pointer ownership, unchanged hit geometry, cancellation while dragging, and feedback bindings on both maps.
- Styled locked/unpublished notices, single-instance reuse, expiry and input transparency.
- Available round 2 → pre-level → actual gameplay for level 2, with one energy deducted; intro routing and existing footer/HUD routes.
- Locked-state/save preservation, pooled node recycling through round 500, scroll bounds/current jump, star separation, shared Home header, RU/EN, raycast reachability and simulated phone safe areas.

The first rebuild found a Unity missing-component/null-coalescing issue in the new notice installer; explicit Unity-object checks fixed it before delivery. Verify01 passed 34/35 checks; its remaining failure was a test fixture with an unconsumed round-1 story milestone. The fixture now records that milestone as seen before testing direct pre-level entry. Production story routing was preserved. Earlier diagnostic logs are retained.

Native captures were visually inspected for spacing, text fit, pressed states, panel style and tall-screen composition. All 11 browser review choices load at 390×844 with no script errors or horizontal overflow. `QA/AssetScope.json` records ten modified Assets files and four new files, zero deletions. Only ChapterMap and WorldMap prefabs changed; existing asset GUID/meta files and all artwork hashes are unchanged from the start of this task.

No Android build, physical-phone test, full-project test suite or hardware-input test was performed.

## Limits and next work

There are still **ten authored playable rounds**, subject to saved progression, within **500 planned map positions**. This change does not fabricate level content or unlock later rounds. Campaign/story migration and authoring the first 50 rounds remain separate work before expanding playable content. Visual acceptance of this R4 spacing and feedback remains with the user.
