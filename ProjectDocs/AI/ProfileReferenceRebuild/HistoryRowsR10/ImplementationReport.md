# Profile R10 — History Row Hierarchy

History previously placed each level heading and its statistics in one heavy, two-line text block. Each row now separates the heading from three aligned columns: stars, best score and attempts. Secondary labels use muted gold; values use the existing approved numeral font, with bounded fitting for long numbers. All labels remain live and localized in Russian and English.

The six-row layout, original navy/gold row surfaces, row bounds, saved record ordering, pager and empty state are retained. Values come from the existing saved level records. No new progression data, service or ownership behavior is introduced. Main Profile and collection content are outside this refinement.

View, builder, active prefab and cutover now agree on revision11. No art or font resources were added or altered; Scenario usage is0 calls/0CU because this pass changes native text hierarchy only.

The supplied Profile reference depicts the collection state, not History. This is a native readability adaptation using the approved materials, not a claim of an exact History reference match.

## Verification

Final run `/tmp/lor-profile-rows-r10/Verify01`, Unity6000.5.9f1: prefab rebuild succeeded;3 EditMode+11 PlayMode=14passed,0failed/skipped. The new test covers maximum integer score/attempt counts and zero values, exact displayed data, separate columns, label/value separation, bottom-frame clearance and unchanged saves in RU/EN at tall-screen dimensions. Existing navigation, pagination boundaries, press feedback, localization, safe-area and text-fit checks also passed.

44 actual Unity Play Mode captures were produced. All46 review options decoded in a430px browser without script errors or horizontal overflow. Visually inspected standard Russian History and tall Russian/English maximum-value captures. Labels and values remain inside the row frames, with six records visible and the pager separate.

SHA checks preserved5,575 existing protected artwork/font/meta/other-page files. Standard ProfileRU/EN and ItemsRU captures are pixel-identical toR9. The matched HistoryRU capture differs only inside the six-row region. Source/cutover whitespace checks passed. The broader check flags Unity-generated empty YAML fields in the prefab; serialization was retained.

No Android/device/full-suite/backend/performance testing. Captures use isolated test saves and simulated phone dimensions/safe areas.

## Changed Assets files


- `Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/ProfileCutoverState.txt`
- `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Profile.prefab`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionProfilePrefabBuilder.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileView.cs`
- `Assets/LegendsOfRus/Tests/PlayMode/ProfileAlignmentPlayModeTests.cs`

Six existing Assets files changed; none added/deleted. Review scripts, evidence and task logs accompany the implementation.

[Scope and results](QA/ScopeAndVerification.json) · [Integrity](QA/IntegrityChecks.json) · [Browser](QA/BrowserChecks.json) · [Fidelity](QA/ReferenceFidelityReview.md)
