# Gameplay corrections R10 — implemented, visual acceptance pending

The canonical Gameplay prefab and its owner now deliver zero starting score, a revised header without the character name, continuous shared grid rails/junctions, colored line-special objects and an animated automatic reshuffle. The user's latest clarification is implemented: horizontal/vertical placement alone identifies the line-clear direction. There are no added arrows or direction marks.

[Standalone Unity review](Gameplay_Corrections_R10_Review.html) · [Reference comparison](ReferenceComparison.md) · [Exact changed files / preservation audit](QA/SourceAudit.json)

## Behavior

- All ten authored rounds and the fallback start at **0**. Score-goal targets and three star thresholds each lose the same former 12,540 offset, preserving the points that must be earned. For example, level 6's score goal is now 2,060 instead of 14,600. Existing saved best scores and previously awarded stars are not migrated or erased.
- The live meter shows no earned stars at zero. The existing minimum one-star reward for successfully completing a level remains in the completion rules.
- Independent moves, parchment-goals, wood-score and portrait-ring artwork replaces the header's previous assets. The original character is preserved. Character-name text and plate are inactive, with their serialized identities retained. Text remains live and localizable; exact font identity is still unverified.
- The real **8×7** board keeps its full-cell input targets. One shared mesh draws each rail and its 72 intersections once, instead of four inset corner ornaments crowding every junction.
- Six colored line-special sprites replace the fixed-color row/column resources at display time. The same sprite is placed horizontally for row clears and rotated 90° for column clears. A consistent 2.1:1 display footprint improves board-size readability. No `SpecialMatchColor` child is created. Burst and rainbow specials also lose the side fragment; their existing central art remains.
- Automatic no-move recovery now gathers/fades the tile visuals for 0.28 seconds, applies the existing deterministic snapshot while hidden, and releases them over 0.38 seconds. Stationary obstacle layers stay in place. Input remains locked under the existing command transaction; the animation introduces no additional move cost or random choices. The completion message is RU/EN localized.
- The four inventory booster cards retain R9 artwork, names, quantities and actions. Earlier pages and Android/project settings are unchanged.

## Assets and provenance

Ten new PNG/meta pairs are saved under `Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned/`: `hud_{moves,goals,score,portrait}_r10` and `special_{ruby,sapphire,emerald,gold,amethyst,orange}_r10`.

Used built-in imagegen, one call per asset/variant. [Header prompts](GenerationPrompts.json), [special prompts](SpecialGenerationPrompts.json), raw `Sources/`, [alpha preparation](QA/PrepareArt.py), and [source/runtime hashes and retained-RGB evidence](ArtProvenance.json) are recorded. Four generated header images contained painted checkerboards; prior user authorization covers local alpha-only cleanup. The ring's inner opening was also cleared. Six special sprites retained generated alpha, with disconnected specks removed using the same authorized cleanup. All retained RGB is identical to its source. Opaque panel surfaces remain opaque. No screenshot crops or complete screenshots are runtime assets.

Inspected all ten final resources on black, white and magenta in [alpha proofs](QA/AlphaProofs.png). Generated ornamentation is an approximation, not a pixel-identical reproduction of the reference.

## Verification performed

- Unity **6000.5.9f1**, real canonical Gameplay owner rebuild/readback at revision **10**, final run5 succeeded. Preserved all **395 Gameplay** and **476 Pre-Level** serialized object IDs and all **886** prior meta files. Of 1,713 baseline files, **1,701** remain byte-identical; 12 existing files changed. Added ten PNG/meta pairs. [Final 1,733-file manifest](FinalProductionHashes.json).
- Final targeted UI EditMode: **8/8 passed**, no skips. Final Gameplay/Pre-Level/goal/progression PlayMode: **27/27 passed**, no skips. Fixtures check all ten zero-score starts and staged goals, missing miniature fragments, six color mappings and both orientations, shared grid, inventory/input/pause/localization/routing, plus reference/tall-phone capture layouts. The automatic reshuffle fixture checks nonzero visual movement, input rejection, unchanged obstacle placement/layers, no added turn cost, reset transforms/opacity, and the final planned hash with a legal move and no initial matches.
- Focused core rules: **29/29 passed**, no skips, covering swap/match creation including four/five matches, specials, obstacles and resolution plans. Its initial run was 28/29: an existing 3×3 crate fixture left insufficient playable space after clearing. Expanded only that test fixture to 4×4, retaining its single-hit-per-wave assertion. Production core rules remain byte-identical.
- **25 final capture pairs**: 16 Gameplay and 9 Pre-Level. Directly inspected final English941, Russian level6/three missions, Russian576 safe area and the arranged special-specimen portrait. Earlier run4 Russian941 and special specimens were inspected and led to a wider booster silhouette and corrected live zero-star display. Captures are actual Unity renders with simulated display dimensions, not phone tests. The special-specimen image uses explicit QA data (50 moves and targets999), separate from authored levels.
- C# `git diff --check` passed. Source audit verifies the score-threshold/goal subtraction and that other level data is unchanged. Full-project YAML whitespace is not represented as clean.
- Chrome checked all18 portrait choices, all4 navigation links (HTTP200), and no horizontal overflow at360px. The standalone browser screenshot was inspected. Verified65 final evidence hashes and all66 original R9 evidence hashes; records are in `QA/BrowserChecks.json` and `UnityVerification/EvidenceHashes.json`.

Earlier run history is retained under `QA/EarlierRuns/`: run1 rebuild passed, EditMode7/8 failed on the old moves-art expectation; run2 stopped on a misspelled test helper; run3 EditMode8/8, PlayMode22/27 (old nonempty-meter expectation and ineffective reshuffle fixture); run4 EditMode8/8, PlayMode26/27 (movement probe inspected a stationary crate). These were corrected rather than suppressed. Run5 is the final runtime/visual evidence. Only the independent core fixture changed afterward, followed by its successful 29-test run.

Existing development-font fallback and Editor shutdown/memory diagnostics remain; this is not an Android performance assessment.

## Wooden obstacles and chapter coverage

| Content | Current implementation |
|---|---|
| Plain wooden crate | One damaging hit |
| Reinforced wooden crate | Two damaging hits; the visible layer count starts at2 |
| Damage source | Orthogonally adjacent matches or a direct booster/special footprint; at most one layer per crate per resolution wave |
| Move count | Not fixed: cascades can produce several waves during one move |
| Chapter1 — Родная деревня | Playable authored levels1–10; two missions in1–5, three in6–10 |
| Later chapters | No authored playable level catalog; World Map keeps them unavailable |

## Not performed / remaining limits

No APK/AAB build, Android device run, frame-time or memory profiling, full-project suite, prolonged comfort test, calibrated color measurement, complete human playthrough, exhaustive obstacle/special combinations, or new captures of earlier pages beyond Pre-Level. No reshuffle video was recorded; movement and transaction behavior were verified by the runtime fixture. Existing scenery, ordinary gem art, footer artwork and central burst/rainbow art still differ from the uploaded target. Further visual refinement requires review, not an assumption of approval.
