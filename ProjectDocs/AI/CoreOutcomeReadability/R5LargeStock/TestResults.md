# Gameplay large stock readability R5

Continued the next independent core UI task after confirming the five normal manual campaign recordings are unchanged. Chapter 1 tuning still requires human outcomes for 31–500. This task corrects the separately documented large Gameplay stock display limit without changing campaign balance.

**Implemented and verified: 14 Edit Mode + 22 Play Mode cases passed, zero failed/skipped in the final runs.** [Standalone actual Unity portrait review](GameplayStock_R5_Review.html). The new appearance is ready for review; approval of an unseen capture is not inferred from the instruction to continue.

## Installed behavior

Gameplay quantities with four or more digits move from the small circular badge into a native emerald/gold stock face in the existing unused lower well of their own wooden card. Counts through 999 keep the preceding circular layout. Each existing live Text retains its font, exact invariant-grouped number, parent and identity; no rounding, abbreviation or maximum-value truncation. All four quantities support Int32.MaxValue. No additional Button or event route was added. The wider decoration does not receive raycasts; the existing card still forwards pointer events to its existing booster Button.

The actual Gameplay prefab and its owning source helpers agree on **revision 24**. Repeated fresh/current construction installs a single backing/layout per count. Inventory callbacks update presentation immediately; the observer also handles direct text changes in the next LateUpdate. Both normal GameState and the existing campaign provider retain their previous count/transaction ownership. No art generation, recoloring, cropping, font copying, gameplay-rule or economic change.

The retained [prefab proof](QA/PrefabPreservation.json) confirms **429 original serialized objects retained, 20 added, none removed**. Exactly nine existing objects differ: the view revision, four count GameObjects receiving a layout component, and four badge RectTransforms receiving a backing child. All original transform geometry, sprites, fonts, text colors, Button targets and other bindings are unchanged. The default serialized quantities keep their original circular badge and label settings.

## Native measurements

Measured **48 configurations / 192 stock observations**: six quantities (`0`, `999`, `1 000`, `9 999`, `999 999`, `2 147 483 647`), RU/EN, and 360×640, 576×1280, 941×1672, 1080×2400 with the existing simulated safe areas. All exact characters remain on one line, inside their own label and stock face. The new face remains inside its wooden card and clears its icon and caption. Board hashes and normalized player saves remain unchanged by the presentation matrix.

| Quantity at 360×640, RU | Prior circular control: actual ink height | Current: actual ink height |
|---|---:|---:|
| 9 999 | 3.94 px | 11.00 px |
| 2 147 483 647 | 1.61 px | 7.61 px |

The comparison controls temporarily apply the preceding circular settings in the current native scene; they are not a separate historical binary run. Nominal font size alone reports approximately 13 pixels in both cases because the numeric mesh fitter can shrink ink afterward. The table and enforced minimum use actual post-fit mesh bounds. Full ten-digit values remain small; this is an improvement in the sampled Editor displays, not a universal physical-phone readability standard. Existing 999 ink height remains approximately 6.30 pixels in the small view.

[48-row measurements](QA/StockMeasurements.json) · [CSV](QA/StockMeasurements.csv). The final Play run retains **97 actual Unity PNGs**, including 48 stock matrix pages, two in-memory circular controls, the live transaction transition and existing regression captures. Directly inspected final small RU `9 999`, small EN maximum and the `999` transaction transition. The latter retains the existing Sea Pearl effect; effects were not changed. Browser screenshots are review-page documentation, not additional Unity or phone evidence.

## Tests and reconciliation

Unity **6000.5.9f1**; product configuration/storage isolated beneath `/tmp/lor-gameplay-large-stock-r5/`; Vulkan is a process option for Play Mode, not a project backend change.

- Install: idle Editor applied only the precise owning helper to the active Gameplay prefab, exit 0; temporary helper/meta removed.
- [Edit result](QA/Edit/summary.json): **14 passed**, zero failed/skipped. Two new fresh/current stock transition cases plus existing Gameplay, caption-owner and BoosterSelection checks cover repeated construction, native object/control/font identity, exact numbers and serialized bindings.
- Initial [Play result](QA/Play/summary.json): **17 passed / four failed**. Three BoosterSelection fixtures lacked their capture directory in the new launcher. The new Sea Pearl fixture incorrectly expected target arming; existing Sea Pearl immediately consumes one item and adds five moves. Corrected the launcher and that expectation, keeping runtime booster behavior unchanged. Initial failures remain recorded.
- Final [Play result](QA/PlayVerified/summary.json): **22 passed**, zero failed/skipped, exit 0. Full Gameplay and BoosterSelection groups plus the existing campaign play/win/continue integration case. The actual native pointer path through the wide count area starts Sea Pearl; a repeated click while busy does not consume a second item. Inventory changes from 1,000 to 999, moves increase by five, gem hash stays unchanged and presentation returns to the original circular badge. The 48-view matrix enforces all characters/containment and actual wide-count ink height ≥7.5 pixels in sampled views.

There is no new broad R6 runtime sweep or Android/device acceptance. R6's results and the Chapter 1 evidence audit remain historical. Human moves/ratings, arbitrary mid-round resume, final effects and phone lifecycle/performance remain outside this correction.

Seven isolated [browser checks](QA/Browser/Checks.json) passed with zero JavaScript exceptions: default native portrait/actual-ink metric, previous circular control, RU/EN switch, exact maximum quantity, tall-view selection and evidence links. [Final validation](QA/FinalValidation.json) checks all native review paths, result counts, scope/identity proof, unchanged normal recordings and the APK/checksum sidecar.

## Change scope and artifact

[Scope](QA/Scope.json) · [Task-only patch](Sources/TaskChanges.patch). Of **7,821** task-start Assets/Packages/ProjectSettings/Backend files, **7,817 unchanged; four changed; four added; zero deleted**. Changed: Gameplay prefab, Gameplay source controls/view, and its existing Play fixture. Added: stock-layout source and two-case Edit fixture, each with metadata. All existing metadata/GUIDs, source artwork/material/font bytes, other prefabs, settings/packages, backend, approved Splash and prior user edits preserved. Reverse patch check passes. Generated test scenes and temporary installer removed.

Current R4 APK is unchanged at **765,772,146 bytes**, SHA256 `924ecabcdbdb58f30512e6e2b134db4246be22c8f9ecc6b6086551e4ad937eff`; checksum sidecar matches. **It still contains Gameplay revision 23, not this revision-24 improvement.** No APK build, physical-phone execution, hosting, deployment or commit. Normal manual recordings remain unchanged; no new human results or balance adjustment were invented.

**Next:** refresh the guarded development APK with installed Gameplay revision 24, first preserving and verifying the R4 artifact. Rerun current Android policy/content guards and compare identity, signing, permissions, ZIP/ELF alignment and packed membership. Phone execution/hosting remain deferred. Human recording 201–210 remains the separate next Chapter 1 acceptance action; final gameplay effects stay last.
