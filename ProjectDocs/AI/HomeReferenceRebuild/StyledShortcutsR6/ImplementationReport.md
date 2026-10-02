# Home R6 — Styled Ranking, Team and Friends icons

The R5 routes worked, but their reused Team Hub frame and differently scaled pictograms did not match Home's richer navigation controls. The user requested icons matching the implemented style.

Created a coordinated set using Home's approved Shop and Characters buttons as style references: antique-gold chamfered frames, sapphire finials, opaque navy interiors and blank caption plaques. Ranking uses a trophy, Team uses a blue eagle shield, and Friends uses paired Slavic adventurer portraits. These are generated interpretations of the approved style, not exact source extractions.

Three new versioned Home PNGs and import metadata replace only the new shortcut artwork. Approved existing artwork is unchanged. Each sprite includes its own frame and subject; the old separate pictogram layer is disabled and its reference cleared. Button identities, navigation and feedback are retained. Live Russian/English labels fit the caption plaques. Home view revision 20, owning source composition and actual prefab are rebuilt together; measured Loading preparation includes the three icons.

One Scenario API generation request produced the three-control sheet, costing 48 CU. No resubmission or extra background removal was necessary: Scenario returned an RGBA mask. Split by recorded thirds, cropped to alpha bounds with eight-pixel margins; alpha below 8 cleared and near-opaque alpha raised to 255. RGB is preserved. Black, white and saturated pink diagnostic composites were inspected for edge remnants and missing ornaments.

Full prompt and parameters: [Request.json](Sources/Scenario/icons/Request.json). Job/billing: [ScenarioAudit.json](QA/ScenarioAudit.json). Split bounds/output hashes: [AssetProvenance.json](Sources/AssetProvenance.json). No OpenAI direct CLI or built-in generator used; the user's established Scenario workflow was used.

Verification results will be appended after the fresh Unity run. No Android/device/full-suite/performance/backend verification is claimed.

Final Verify01: Unity 6000.5.9f1 actual prefab rebuild/readback, 8 Edit Mode + 26 Play Mode tests passed (34 total, no failures/skips). Coverage includes fifteen Home controls via pointer clicks, one action on repeated taps, all three destinations and their Back behavior, safe-area containment, localization roundtrip, Loading resources and Friends regression. Actual default RU and tall safe-area EN captures visually inspected; no caption/frame clipping observed. Six browser review views passed. Scoped C# whitespace passed.

Scope against R5 snapshot: five existing Assets files modified (Home prefab, composition/runtime/view and Home test resource contract); three PNGs plus three metadata files added, none deleted. All pre-existing PNGs/metas/fonts/other-page prefabs remain unchanged by SHA. The combined R5+R6 navigation feature also changes ScreenRouter, the existing Home builder verification message and EditMode coverage as recorded in R5.

Saved assets:
- `Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/shortcut_ranking_r6.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/shortcut_team_r6.png`
- `Assets/LegendsOfRus/Resources/ProductionUI/HomeReferenceAligned/shortcut_friends_r6.png`
