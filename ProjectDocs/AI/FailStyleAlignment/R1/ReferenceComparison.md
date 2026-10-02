# Failure R1 reference comparison

Inspected the original `ApprovedScreens/Fail.png`, the real Failure route and final RU941 / EN576 actual Unity captures.

| Area | Implemented | Remaining difference |
| --- | --- | --- |
| Scene and Ivan | Independent dark damaged-village/castle plate and recognizable navy/burgundy Ivan portrait. | Pose, facial rendering, castle placement and ember lighting are not pixel-identical; hair rim remains bright. |
| HUD | Existing profile, currencies, energy and settings artwork with live values. | Actual save data replaces reference examples; no fabricated level/XP. |
| Outcome panel | Navy/gold blank surfaces, live serif title, protected corner/crest geometry. | Reference lettering is more engraved; filigree and exact panel contours differ. |
| Goals | Current failed-round progress, corresponding gameplay art, completed goals filtered. | Current gameplay gem/obstacle/score icons are used; reference counts are not copied. |
| Actions | Natural-proportioned emerald retry and live chapter-map return. | Reference paid extra moves/ad continuation are not supported by the existing Fail flow and are omitted. |
| Boosters | Existing four gameplay types, real stock, readable names and explanations. | Existing unframed art and original names replace the reference's different circular medallions/types. |

No complete reference screenshot or screenshot-region art is used as runtime UI. The existing free-retry behavior is preserved, not newly introduced. Review at phone size is the next useful check before more visual refinement; acceptance remains pending.
