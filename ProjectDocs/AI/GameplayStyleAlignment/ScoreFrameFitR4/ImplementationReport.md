# Gameplay score-frame fit R4

The score panel was distorted: its 1501×489 artwork was drawn as one stretched 941×194 rectangle. Relative to its horizontal scale, it lost about 37% of its height. This flattened the sapphires, broadened the leaves and pushed the ornament to both screen edges.

The active Gameplay prefab now preserves the proportions of both carved ends and the center ornaments. Only two plain sections of lacquer, bronze and straight rails extend horizontally. The panel is inset by 32 reference pixels on each side, with bounds 877×181 at reference position (32,1486). The live heading and score were adjusted vertically to fit the writing areas. Original artwork, colors, PNG bytes, sprite GUID and font settings remain unchanged; no generated art or screenshot crops were used.

[Open the corrected Unity portrait](Gameplay_ScoreFrameFit_R4_Review.html). The viewer offers five new captures, previous R3 and the canonical reference separately. Carving and font identity still differ from the reference; this correction addresses fit, not a new artwork replica.

ProductionGameplayScoreFrame is a Gameplay-only native mesh effect on the existing score Image. It renders five adjoining strips using the original texture: three uniformly scaled ornament sections and two extensible plain sections. Existing controls, score binding, other HUD regions, board, scenery and responsive positioning remain. The prefab, owning builder and view agree on revision **4**.

Five existing files changed: Gameplay.prefab, Step20ProductionUiFix01SerializedPrefabBuilder.cs, ProductionGameplayHudStyle.cs, ProductionGameplayView.cs and the existing GameplayAlignmentEditModeTests.cs revision assertion. ProductionGameplayScoreFrame.cs and its metadata were added. [Exact manifest](SourceChangeManifest.json), [source diff](AppliedSourceChanges.diff) and [source audit](FinalSourceAudit.json) are retained. All **303** previous Gameplay prefab object IDs and **851** recorded metadata files/GUIDs remain. **1,637/1,642** baseline paths are unchanged, five modified and two added; **1,644 final hashes**. All image resources, other pages, rule/VFX sources, packages, ProjectSettings and Android guard/signing sources match the local R3 baseline.

Checks performed:

- Unity **6000.5.9f1 / OpenGLCore** compiled, imported, saved and read back the canonical Gameplay prefab at revision 4, exit 0.
- Final focused [Edit Mode](UnityVerification/EditMode.xml): **4/4 passed**, zero failures/skips. Existing tests cover repeat application, retained controls/bindings, runtime dependencies, supplied art mappings/imports and prior-page revisions/fonts. The first run was 3/4 because its assertion still expected revision 3; updated that assertion and reran. No new test cases were added.
- Focused [Play Mode](UnityVerification/PlayMode.xml): **1/1 passed**, zero failures/skips, exit 0. Ran the existing `CaptureReferenceAndPhonePortraits` case only. It checks real cell/button raycasts, safe-area containment, environment coverage and board/control layout while capturing Russian, English, RU roundtrip and two simulated phone displays.
- Retained **five unedited PNGs and five metadata records**. Directly inspected RU/EN 941×1672, 576×1280 safe area, and 1080×2400 safe area. Decorations are proportional and the live text fits the inset; no visible strip seams were found. The roundtrip image was compared numerically, not separately inspected.
- [Pixel-scope comparison](QA/VisualScopeComparison.json): outside the score region, 18 pixels differ by at most 1/255 from R3. RU roundtrip differs by 24 pixels at most 1/255. No pixel-identical claim is made.
- Local browser validation: seven portrait choices, three viewer links and 360px width. [Browser results](QA/StandaloneReviewBrowserChecks.json) and [browser screenshot](QA/ReviewBrowser_360px.png) validate the evidence viewer, not additional Unity rendering.

[Run commands, XML history and raw-log hashes](UnityVerification/VerificationRuns.json) are retained. Raw Editor/licensing logs remain outside the delivery. The original R3 resource and all existing metadata are unchanged, so no new transparency preparation was needed.

Not performed: remaining Gameplay Play Mode cases, fresh all-level/action/obstacle/message/Pause validation, earlier-page recaptures, full-project/pure-core suites, new art generation, APK/AAB/Android build, phone installation/render/touch, performance/memory or release/signing checks. Existing historical failures remain recorded and were not cleared by this scoped correction. CurrentTask.md and SessionLog.md record the correction; visual acceptance remains with the user.
