# Pre-Level R4 — clearer lettering, raised stock badges, darker scenery

Compared the actual R3 portrait with the original Pre-Level reference. The reference's smooth gold strokes and prominent ivory quantity digits read more clearly than R3's beveled small text and46-unit stock badges. R4 addresses those differences in the canonical Unity page.

[Open the full-portrait comparison](PreLevel_Readability_R4_Review.html): twenty final actual Unity captures, previous R3 at reference/phone sizes, and the original reference. These are Editor renders with simulated display/safe areas, not phone screenshots.

## Changes

- **Clearer type treatment:** removed the large Play-style bevel/material from other Pre-Level text. Live headings, moves, goals, rewards and HUD text now have solid warm-gold strokes with a restrained dark shadow. Regular serif lettering follows the reference more closely; quantity digits use bold ivory. The large Play lettering retains its approved treatment. No new font was installed, copied or assigned; exact reference-font identity and existing device fallback remain unverified.
- **Stronger booster stock badges:** enlarged emerald badges from46 to58reference units and increased ordinary digit size up to32units. Their gold rim/dome lighting is preserved. An inset text area and width-aware fitting keep0,7,10and999on one line inside the rim. The badge renderer is shared with Gameplay and remains byte-identical; geometry/text changes apply only to Pre-Level.
- **Darker background:** scenery RGB multiplier changes from0.88to0.70relative to the shared Chapter Map source, preserving alpha. It is derived from the source on each alignment, not multiplied repeatedly. This is a rendering parameter, not a measured claim about perceived brightness. Character, scenery PNG, panels and booster artwork files remain unchanged.

The R3 guide, original names/actions, live inventory, level data, energy/Play behavior and navigation remain intact. No new pre-game loadout, consumption rule or art resource was introduced. Overall layout remains as before, apart from the badge geometry.

## Implementation and checks

Unity6000.5.9f1 (b57deb96f08d), native Pre-Level revision4. Updated the actual prefab and its runtime/builder owner together. Six existing files changed: PreLevel.prefab; ProductionPreLevelRuntime.cs; ProductionPreLevelView.cs; Step20ProductionUiFix01SerializedPrefabBuilder.cs; PreLevelAlignmentEditModeTests.cs; PreLevelAlignmentPlayModeTests.cs. Full paths and changes are in [SourceAudit.json](QA/SourceAudit.json) and [the task diff](AppliedSourceChanges.diff).

Final run3: owner rebuild/save/readback succeeded; **3/3 EditMode and6/6 PlayMode tests passed**, no skips/failures. Existing tests cover all ten authored levels, 2/3objectives, replay/rewards, guide/localization/stock updates, energy/double-start and native input/layout. Added phone-size0/7/10/999stock coverage with rendered-mesh rim containment, minimum badge size and single-line assertions. Updated shared-resource assertions to allow page-specific lettering and darker tint while preserving Home's original material.

Saved20final PNG/JSONpairs. Directly inspected final EN941, RUlevel8/three goals, and RU576stock specimen; directly inspected initial R4RU941andRU576to assess lettering/badge/scenery changes. Compared these against R3 and the original reference. No exact pixel-replica or exact-font claim.

[Test history](QA/TestHistory.json): all three owner runs and EditMode runs passed. Run1andrun2PlayMode5/6:999crossed the rim. Padding alone did not fix it because text still allowed horizontal overflow. Corrected width-aware fitting and added the one-line assertion; final6/6passed. The containment threshold was not relaxed.

[Preservation audit](QA/SourceAudit.json):1,727of1,733baseline production files unchanged; all896metas and all500priorPre-Level object IDs retained (531now). No production files added. Gameplay, shared renderer/fonts/artwork, earlier pages, packages/settings/signing and Android guards unchanged. Final1,733production hashes,51final evidence hashes and49priorR3evidence hashes verified. Changed C# whitespace passes. Chrome23portrait selections/3links pass, no360pxhorizontal overflow; browser screenshot inspected. Test evidence and checks are retained in QA/UnityVerification.

Not performed: APK/AAB, physical Android touch/rendering, full-project suite, profiling, long-session comfort/accessibility study, calibrated color measurement, new art/alpha checks, exhaustive inventory magnitudes, or fresh captures of other pages. Existing font fallback limits device equivalence. Remaining distinct refinements include explanations for non-gem objectives and tall-screen spacing; no new visual acceptance is inferred.
