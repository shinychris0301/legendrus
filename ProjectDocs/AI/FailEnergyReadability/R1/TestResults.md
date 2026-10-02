# Fail and Out of Energy readability R1 — 2026-09-30

**Final targeted Unity verification: 21 passed, 0 failed, 0 skipped.** [Before/after portrait review](Fail_Energy_Readability_R1_Review.html).

## Implemented

- **Fail, revision 14:** larger live goal counts (31→36 reference units), unavailable statuses (18→30), map label (19→32), booster heading (26→32) and help text (18→28). Moved the larger map control beside the proportionally scaled Retry button to avoid covering booster artwork. English booster names increase modestly (20→23); existing Russian extracted lettering, shared header and booster assets remain in use.
- **Out of Energy, revision 3:** larger message/timer captions, live Russian/English action headings, and descriptions moved to their own full-width lines (20→30 reference units). Dialog can scale to 1.12 within the safe area. The close target is enlarged; the medallion and row icons are fitted to preserve clear text space. Retained the intentionally opaque navy panel and original Russian button lettering.
- Disabled availability labels are fitted and centered inside the actual aspect-preserved button image. Unity aligns these images to their top-left pivot; centering against the larger Button rectangle caused visible overflow. Tests now check the painted left/right edges, not just the control width.
- Canonical prefabs, owning runtime builders, Out of Energy builder/cutover revision and revision tests agree. No image pixels, source GUIDs, font assets, gameplay rules, reward/energy policy, save schema or offer capabilities changed.

## Final executed checks

| Check | Result | Evidence |
|---|---|---|
| Unity Play Mode: both page suites | 11 passed, 0 failed/skipped | [FaceFitPlayMode](QA/FaceFitPlayMode/summary.json) |
| Unity Edit Mode: both canonical page suites | 10 passed, 0 failed/skipped | [FaceFitEditMode](QA/FaceFitEditMode/summary.json) |
| Specific Fail prefab rebuild | Exit 0 | [BuildFailFinal](QA/BuildFailFinal/result.json) |
| Specific Out of Energy prefab rebuild | Exit 0 | [BuildEnergyFaceFit](QA/BuildEnergyFaceFit/result.json) |
| Task scope and snapshots | 12 changed, 7,773 unchanged; 0 added/deleted | [Scope](QA/Scope.json) |
| Changed-file whitespace and Python runner syntax | Passed | Sources/AuditScope.py |

Unity **6000.5.9f1**; disposable product-storage roots and existing licensing-directory links. Previously authorized Vulkan was used for the verification processes only. No project graphics/backend/package change.

Runtime coverage includes Russian/English at 360×640 and tall portrait captures with simulated safe areas; real router bindings and button raycasts; actual failed-goal progress/stock; incomplete or missing-result states; free retry; map return; all four booster-help descriptions at 360 pixels in both languages without stock/resource spending; disabled offers rejecting callbacks/grants; the energy clock, full-capacity state, modal blocking, close/reopen and standalone-route return. Edit Mode checks verify canonical resources, import settings, binding completeness and repeated builder identity.

New layout checks enforce at least 10.5 effective render pixels for Fail's targeted availability/map labels, 11.8 for Out of Energy's explanatory details, and a 44-pixel close target at tested sizes. Text must clear row borders and other controls; button-label preferred width and actual painted-face placement must fit. These thresholds do not establish a universal minimum for every label.

Actual final captures inspected: Fail Russian at 360×640 and English booster help at 360×640; Out of Energy Russian/English at 360×640 and English at 576×1280. Earlier same-layout Fail English at 576×1280 and Out of Energy full-state captures were also inspected. The review displays a single actual capture with page/language/before-after selectors; it is not a replacement production page.

## Iterations retained

The baseline passed all 11 existing tests despite visibly small captions. New sizes exposed Russian message wrapping, which was corrected. A legacy Fail text-fit assertion incorrectly tested its hidden Russian fallback behind extracted artwork; it now checks rendered labels. Capture inspection found border contact and the aspect-preserved button-face mismatch; geometry checks were strengthened and the layouts corrected. An initial Edit Mode revision update accidentally changed the expected button count as well; restored the independent count of 13. The final 11+10 acceptance results above supersede intermediate runs; counts are not summed across reruns. [Run inventory](QA/RunInventory.json).

## Limits and next action

No APK build/install, phone execution, full-project sweep, human difficulty test, hosting or effects work ran. Phone work remains explicitly postponed. Existing small embedded lettering is retained; this is a targeted readability/layout improvement, not a claim that all game text is large or that every interactive element meets a universal touch-size threshold. Exact reference-font identity remains unverified.

Next: audit **PreLevel and Pause** captions and controls at small Russian/English portrait sizes, fixing demonstrated problems. Keep physical-phone lifecycle/performance/readability acceptance, human rounds 31–500 pacing, optional recap and the clean post-correction broad sweep open. Hosting/online activation is deferred; animation/effect redesign remains last.

Task-only source/prefab patch: [Changes.patch](Sources/Changes.patch). Every changed file has a snapshot matching its task-start hash; unrelated local changes are preserved. Generated Unity test scenes are absent from the final scope. No commit or deployment.
