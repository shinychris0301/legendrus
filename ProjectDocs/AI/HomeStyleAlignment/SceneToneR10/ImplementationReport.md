# Home R10 — subtle character and background tone

Home revision **13** applies gentle neutral darkening to the existing scene artwork. Ivan's clothing and the village sit more quietly behind the controls, while the face and Firebird remain luminous. The latest request authorizes this presentation adjustment; it does not establish new visual acceptance of R9 or R10.

[Actual Unity before/after/reference review](Home_SceneTone_R10_Review.html) · [Final Russian portrait](UnityVerification/Home/Home_941x1672_RU.png)

| Layer | Native RGB multiplier | Alpha |
| --- | --- | --- |
| Main Ivan and Firebird, one combined isolated sprite | 242/255, approximately 5.1% darker | Unchanged, including the existing bottom fade |
| Environment | 235/255, approximately 7.8% darker | Unchanged |
| Logo, HUD portrait, controls, badges, foliage and live text | Existing values retained | Unchanged |

These percentages describe RGB multiplication, not photographic exposure stops. The existing R9 lower-background gradient remains intact. The runtime uses the two existing Images' `color` properties, applied after their normal artwork assignment; no new graphic layer, shader, texture, camera or post-processing effect was added. The original PNGs and GUIDs remain unchanged. No image generation, screenshot crops, raster editing or font copying occurred.

`ProductionHomeRuntime.cs` owns the two constants and assignments. `ProductionHomeView.cs` requires revision 13 and the matching serialized colors. The existing `Step20ProductionUiFix01SerializedPrefabBuilder.RebuildHomeAlignment` compiled, saved and read back the active Home prefab. Its source already uses the runtime owner/current revision and required no edit. The prefab diff contains exactly three field changes: revision and the two RGB colors. Geometry, references, alpha, scene layering and existing child order are unchanged. The shared character fade preserves its incoming RGB and retains its existing alpha curve.

Five existing production files changed: the active Home prefab, the Runtime/View classes above, `HomeAlignmentDeliveryEditModeTests.cs` and `HomeAlignmentPlayModeTests.cs`. No production files were added. [SourceChangeManifest.json](SourceChangeManifest.json) lists exact paths and hashes; [AppliedSourceChanges.diff](AppliedSourceChanges.diff) records the complete local-baseline diff. Existing loading still measures **27 actual resource operations**; routing/state behavior remains intact.

Checks performed:

- Verified Unity **6000.5.9f1 (b57deb96f08d)** and all **1,497 R9 production hashes** before edits. Inspected current instructions, historical handoff/provenance, local changes, active owner, layers and fade behavior. The historical Loading-only handoff is superseded by the subsequent Home work.
- The first batch owner launch exited 1 because Unity reported the project already open. A Unity Editor process was observed, then observed to have exited; no process was killed and no lock deleted. The retry compiled, saved and read back Home revision 13 successfully. This launch history is retained in `UnityVerification/Import.json`.
- **38/38 focused Edit Mode and 24/24 Play Mode tests passed**, zero failures/skips. No C# or shader compile errors were found. The new rendered tint comparison temporarily restores R9 white scene colors, then restores the new values. It checks gentle reduction in six scenery/hero/Firebird regions, ten protected opaque UI/avatar/logo/badge regions, unchanged alpha and no brightening. The character fade is checked with both white and tinted RGB. Existing R9 foliage/background, R8 lettering, portrait/safe-area, all twelve controls, state/language, loading/retry/cancel and once-only routing regressions remain active.
- Retained **39 unedited capture/fixture files**, including seven final Home portraits, with SHA-256 hashes. Directly reviewed final RU/EN, narrow phone and tall safe-area portraits. An independent reviewer compared complete R9/R10 reference-size and narrow-phone portraits and recommended retaining these strengths: face/embroidery detail and Firebird sparkle remain clear, with no apparent new hue shift, halo, crushed shadows or seam. Captures are actual Editor renders with simulated display conditions, not physical-device screenshots.
- Decoded R9/R10 reference comparison: **894,152 changed pixels**, maximum RGB decrease **21/255**, **zero channel increases and zero alpha changes**. Final Home RU roundtrip after display/language changes is pixel-identical. Splash and Loading reference-size captures are pixel-identical to R9.
- Local Chrome review: all **six portrait selections and three scene-detail selections passed** without horizontal overflow. Detail mode magnifies the complete capture through a viewport; it creates no runtime crop asset.
- Preservation audit passed: **1,497 baseline paths, 1,492 unchanged, five intended modifications, no additions**. All **776 existing metadata files/GUIDs**, **27 Home PNGs**, **383 prefab IDs** and **86 complete RectTransform blocks** remain intact. Protected Splash, Loading, packages, ProjectSettings and Android build/signing source groups are byte-identical. Independent source review found no material issue.

Evidence: `UnityVerification/FinalEditMode.json`, `FinalPlayMode.json`, `Import.json`, `TestRunHistory.json`, `Reproduction.json`, `CaptureManifest.json`, the four pixel-comparison records, `QA/StandaloneReviewBrowserChecks.json` and [FinalSourceAudit.json](FinalSourceAudit.json). CurrentTask and SessionLog were updated while preserving prior acceptance history; the R9 review links to this revision.

Not performed: APK/AAB or Android shader-variant build, physical-phone install/render/input, full-project regression suite, performance/memory profiling, physical display-resolution changes, custom mask/fade stress tests or release/signing checks. Exact source-font identity remains unverified. The existing APK predates this refinement.

Next action: review the final R9/R10 portrait toggle. Gold/blue UI material balance, mailbox proportions and unavailable XP/quest/event state remain previously documented separate work; this pass does not claim complete reference identity.
