# Login / Account — Panel Edges R2

The apparent background residue was principally an exposed strip of the clean environment: R1 removed a rectangle from the Ivan/Firebird layer, but the panel is narrower and has curved corners. The resulting top and side gaps did not follow the source composition. R1’s coarse panel mask also clipped the sapphire crest and some top trim.

## Repair

Restored the narrow, visible original cloak/feather strips around the panel’s true silhouette. Restored the complete top crest/trim using the existing Scenario panel alpha and original reference RGB. Applied the same contour to both Russian and blank English panel sprites. Preserved opaque wood/parchment, dimensions, color treatment, controls and source artwork elsewhere.

Only `LoginAligned/panel.png`, `panel_blank.png` and `subject.png` changed in production. All existing metas/GUIDs, prefab, owning builder, revision, runtime code, other art, packages and ProjectSettings match the task-start snapshot. The existing builder was rerun successfully and produced no serialized changes: this is a resource-only correction. Temporary texture diagnostic code was removed.

[Scope audit](QA/ScopeAudit.json) · [Asset provenance](AssetProvenance.json) · [Reproducible repair](Sources/RepairEdges.py).

No new Scenario call or credit charge; existing R1 Scenario isolation was reused. Submitted inputs and historical R1 evidence were preserved. No regeneration or screenshot used as runtime UI. The source-covered character anatomy remains absent behind the panel.

## Verification performed

- Fresh baseline in Unity 6000.5.9f1 reproduced the fringe before changes.
- Imported GPU texture readback inspected against source alpha; the underlying extraction geometry, rather than missing PNG transparency, explains the visible gaps.
- Final Unity import/rebuild/prefab readback, 2 Edit Mode and 4 Play Mode tests passed, zero failures/skips. Checks cover bindings, idempotence, RU/EN safe layouts, field/button raycasts, password visibility, unavailable-action shielding, credential clearing, unchanged progress, guest entry and return navigation.
- Seven final actual Unity captures at 941×1672, 576×1280 and 1080×2400, with simulated safe areas, retained. Russian full-size and English tall capture visually inspected.
- Repaired resources composited over black, white and magenta for alpha review. Original RGB outside declared repair regions and unchanged production-file scope verified.
- Python repair script syntax checked; browser review validation recorded separately in QA/BrowserReview.json.

## Limits

No APK build, physical Android keyboard/device test, full-project suite, performance profiling or real authentication/backend test. Existing source/reference differences in environment and live typography remain; no exact overall replica is claimed. Existing outer corner artwork and full-page layout were not redesigned. Online authentication/account linking remain deferred to AUTH-ACCOUNT-01.
