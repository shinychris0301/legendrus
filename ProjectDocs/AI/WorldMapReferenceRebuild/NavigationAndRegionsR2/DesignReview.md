# World Map R2 — reference chapter cards

This is an **interactive browser design review**, not an implemented Unity page or a Unity capture.

[Open the portrait preview](WorldMap_ReferenceCards_R2_Review.html).

## Applied direction

The latest supplied image is the local `chapter select.png`. The user explicitly selected its framed cards, superseding the earlier borderless request. Its original Russian title, card border, five bottom navigation medallions, Back, Continue and map action artwork now guide the composition. Static lettering stays with the extracted panels. The original Home player/resource sprites are reused, with the lacquer strip extended behind the portrait to close the exposed cut. No Home runtime assets were modified.

The four-card view is the default. Twenty approved story-document chapter names and R1 paintings are preserved. Later chapters are positioned above; both mouse and actual touch dragging downward reveal them. Back occupies the lower action row. The external **Reading order** control also offers the original image's top-to-bottom order for visual comparison. This comparison setting is not a proposed game control.

Completed chapters can be revisited. The current playable chapter has the source-style illuminated frame and Continue treatment. Future chapters are dimmed, locked and cannot enter a chapter. This follows the newly selected reference's active/inactive distinction. The earlier separate number-medallion direction was superseded by the reference's `ГЛАВА N` captions.

## Assets and extraction

- Scenario Pixa isolated the original title, frame, navigation group, Back, Continue and map action. Foreground RGB comes from the original crops, not the Pixa RGB output.
- Restored intentionally opaque dark faces inside the title, navigation and controls. Removed attached fragments from neighboring UI. Transparent samples were inspected on black, white and magenta.
- The card frame has an open central window. The reported separation defect was confirmed: a rectangular cut retained scenery and lacquer strips at the frame edges. A bounded metal/jewel alpha mask now removes these strips, preserves the dark metal rail channels, and leaves the ornament separate from the card face. Fine edge alpha remains approximate. Approved chapter paintings and a blank navy surface render separately. The blank surface uses a mirrored clean patch from the original panel; it is reconstructed, not an exact recovery beneath lettering.
- The star plaque retains original artwork; example digits were removed for live text. Its original lock was isolated locally. No fake playable chapters were added.
- One Scenario environment reconstruction fills the hidden background. Visible source scenery outside expanded UI exclusion masks is retained. The clean plate is separate from the interactive controls.
- Original title and lower navigation sit at source proportions. Card heights adapt slightly to show four complete cards, including the larger active card and longer canonical chapter names.

API audit: **9 accepted jobs, 46 CU, no duplicate submissions**. This includes one generated number medallion and its isolation, costing 15 CU, submitted before the user selected the framed reference direction; these are retained as unused provenance and do not appear in the final design. Seven used jobs cost 31 CU. Twenty existing paintings were not regenerated.

## Verification

`QA/BrowserChecks.json`: **22 checks passed**, including all 20 cards, four complete cards at the start, real mouse/touch downward dragging, final chapter reachability, no accidental opening after drag, locked/open actions, all five navigation controls, bottom Back, title bounds, progress fixtures, caption fitting and 390-pixel portrait overflow. Portrait captures in QA are browser renders only.

`QA/ProductionPreservation.json`: all project Assets match the pre-design baseline. No Unity compilation, prefab rebuild, runtime test, APK or physical-phone test was performed. Production remains unchanged pending design review, as requested.

## Exactness and remaining differences

This is **not a pixel-identical reproduction of the whole screenshot**. Extracted fixed artwork preserves original pixels, but the previously approved 20 paintings and story chapter names differ from the four example cards. Dynamic text uses the user's selected embedded typefaces; exact source font identity remains unknown. The blank card surface, hidden environment, active glow and fine alpha edges are reconstructed. Repeated cards and responsive fitting also change some proportions.

The `15/15`, `9/15`, profile balances and progress fixtures are explicitly illustrative design data. The actual game currently has Chapter 1 with 10 authored levels; these values must be replaced by saved progress/content catalog data during native integration. Preview navigation shows explanatory dialogs rather than claiming to call Unity routes.

Next step after visual review: implement the approved native World Map using shared navigation assets/components, real content gating and saved progress, then verify Unity rendering and input. Home and other screens must remain unchanged until their scoped work is authorized.
