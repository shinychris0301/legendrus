# Victory R5 — Rewards and Actions

Implemented 2026-09-15 in the existing Unity 6000.5.9f1 project. The user authorized the next recommended Victory refinement. Visual acceptance and pixel-identical reference fidelity are not inferred.

[Actual Unity portrait viewer](Victory_RewardsAndActions_R5_Review.html) · [Reference comparison](ReferenceComparison.md) · [Exact changed-file audit](QA/SourceAudit.json)

## Result

Reward cards are taller (242 reference units instead of 180), with larger illustrations and amounts. The enclosing wood panel extends around the live heading, score, reward status and story/video message. Continue now follows the reward area instead of leaving a large gap on tall screens. Independent emerald Continue, gold filmstrip/coin video-button and eagle-crest footer sprites replace the Victory-specific action surfaces. All wording remains live and localizable; video still promises coins only.

The real Victory prefab and its owner/runtime are at revision 5. Existing R4 title/ribbon/star assets, their geometry and the body scale/top formula are retained. R3 scenery, Firebird and header are retained. Current-run score, actual rewards, replay rules, energy clock, provider eligibility and existing navigation/services are unchanged. Other pages and all previous local edits are preserved.

## Artwork and provenance

New runtime sprites, under `Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/`:

| Asset | Final RGBA size | Purpose |
|---|---|---|
| `continue_r5.png` | 1512 × 619 | Blank emerald lacquer / carved gold primary action |
| `video_r5.png` | 2172 × 387 | Blank amber action, filmstrip play left, coins/purse right |
| `footer_r5.png` | 1963 × 249 | Low gold scrollwork with burgundy eagle crest |

[Exact initial prompts and input paths](Sources/GenerationPrompts.json), [footer refinement prompt/output](Sources/FooterRefinement.json), selected raw files in `Sources/`, and [reproducible transparency preparation](Sources/PrepareIsolatedAssets.py) are retained. Builtin imagegen created independent sprites. No screenshot region was extracted into runtime UI. The original reference was visual guidance for the two control edits; existing source assets remain unchanged.

All selected outputs contained painted checkerboards. Applied the user's existing **“Allow local transparency cleanup”** authorization: clear connected neutral backdrop, including two inspected enclosed footer holes, then trim transparent padding. No recoloring; retained RGB values are identical to each generated source. Opaque control faces remain opaque. Inspected every final sprite over black, white and saturated magenta. [Preparation measurements/hashes](QA/AssetPreparation.json) and all nine composites are retained. The first footer was too tall and was rejected; its refinement initially failed with a connection error, then the identical retry succeeded.

Exact-path importer rules retain native pixels, full-rect single sprites and existing production texture settings. New files receive new GUIDs; no existing metadata is changed. The local serif/gold material remains the font treatment; exact source-font identity is still unknown.

## Verification performed

Unity was closed before work. Each run used a separate temporary configuration/save directory and the existing Victory-only owner rebuild. Commands, launcher/Unity logs and XML are retained in `UnityVerification/History/`. [Machine-readable test history](QA/TestHistory.json).

| Run | Compile / Victory rebuild / readback | EditMode | PlayMode | Outcome |
|---|---|---|---|---|
| 1 | Passed | 6/6 | 9/14 | Five checks caught Continue touching live story/status text; reduced rim overlap to add clearance. |
| 2 | Passed | 6/6 | 14/14 | Captures then revealed heading on the carving and video covering the crest. Expanded the panel surface and shortened the action stack; added crest clearance assertion. |
| 3 — final | Passed | 6/6 | 14/14 | Zero failed/skipped tests. No subsequent production edits. |

Selected tests cover Victory/Pre-Level serialized bindings and revision, exact sprite identity/imports, native hit targets, RU/EN text overflow and simulated portrait safe areas, rewards/card/message clearance, footer visibility, all ten authored level rewards, current-result score, replay/no-result state, gold-only once-only domain operation, unavailable video, existing Continue/Shop flow, registered Gameplay win callback, live title geometry/refresh stability, star effects/order, energy countdown and Pre-Level regressions. The panel's painted lower rim is checked with its transparent bottom padding accounted for; readable content and button/crest clearance remain explicit assertions.

Retained 12 final Victory and 20 final Pre-Level PNG/JSON pairs. Visually inspected final RU 941 three-reward, EN 576 five-reward and RU 941 unavailable-video portraits against the original reference; inspected three earlier portraits during iteration. Captures are actual Unity rendering with simulated display/safe areas, not Android screenshots. Headless Chrome at 390×1100 rendered the default portrait; its screenshot was inspected. All 14 portrait choices and three local links resolve. Dropdown interaction was not exercised.

Before editing, verified/backed up all 1,766 entries in the R4 production manifest. Final audit: 1,758 unchanged, eight existing files changed, six new art/meta files. All 913 previous metas and 323 prior Victory object IDs remain; 325 object IDs now. All 93 retained R4 evidence hashes remain unchanged. Eight changed existing files are the Victory prefab, three Victory runtime classes, owner builder, importer and two Victory test fixtures; [audit lists exact paths](QA/SourceAudit.json). Final production and evidence hashes retained. Scoped C# whitespace check performed; serialized Unity prefab formatting retained.

## Limits and next action

Not performed: APK/AAB build, physical Android testing, full project test suite, successful live rewarded ad, full manual gem-swap win session, manual animation playback, prolonged-play/profiling, calibrated color measurement, exact font identification or visual inspection of every capture. No packages, settings, signing, Android guards, shared fonts or unrelated pages changed.

The design is closer in reward scale and action/footer grouping, but independently generated ornamentation and existing reward icon shapes differ from the reference. EN story text can still fall back to Russian; story/status and energy timer remain small on narrow phones. Tall screens retain scenery space between the video action and bottom crest. Next useful check is device-size text readability and motion, before another broad artwork pass.
