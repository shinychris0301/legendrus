> Historical preparation report. Integration is now complete; see [ImplementationReport.md](ImplementationReport.md) and the actual Unity review.

# Splash reference rebuild R1 — prepared, not yet applied

User selected `/home/k2/Downloads/match3final/1.splash.png`: original Ivan and Firebird, large crest, framed Start, bottom version, no resource header. This new request authorizes revisiting the earlier approved Splash. Current production is revision7 / FrontEndHomeAlignment R1; the old handoff describes an earlier winter baseline and is not the current design authority.

## Prepared artwork

Five staged assets are in Sources/Final: original crest, joint Ivan/Firebird, blank original Start, transparent source outer frame, and a separately reconstructed clean environment. Scenario performed four alpha-isolation jobs (3 CU each) and one clean scenery generation (13 CU): **25 CU**, five accepted jobs, no retries/duplicates. The grouped corner result retained scenery rectangles and required bounded local contour/metal/sapphire alpha cleanup. It is not accepted unmodified.

Original RGB is preserved in crest/frame and outside recorded Start/version lettering repair masks. Existing text is locally inpainted only where live labels will replace it. The source Start/frame are excluded from the joint hero alpha; they will render as independent controls/art. Scenery behind logo/characters is reconstructed, not recovered exactly. Source font identity remains unverified. Black, white and magenta composites are in QA; tiny extraction/repair differences remain possible at final display scale.

## Staged Unity changes

Sources/IntegrationCandidate holds proposed revision8 configuration, owning builder/view/responsive-layout changes and adjusted source-composition tests. They are outside Assets and **not active production code**. Existing source object identities are retained by the migration. Start remains native and uses the existing measured Loading route. Reference-only composition hides Splash's four auxiliary controls; their route bindings and access through other game pages remain. Version uses Application.version. Existing embedded title/numeral fonts are bound on Splash itself, leaving shared typography/settings unchanged. Frame, characters and labels remain separate; no full-screen reference/hotspot implementation.

The candidate keeps the portrait composition proportional inside the safe stage, extends only clean scenery to the display edges, and avoids the old upward header offset for this selected reference. Active prefab rebuilding, compilation and actual captures still need to establish the result.

## Blocker and checks

Unity refused both baseline EditMode and PlayMode launches because another Editor instance holds this project open. Both exited1 before test execution; no XML test result or fresh capture exists. Logs and command records are in QA/BlockedBaselineAttempt. The process holding Temp/UnityLockfile was identified; no lock was deleted and no Editor process was terminated.

Performed: instructions/handoff/task/reference/worktree audit; selected-original visual inspection; native layout, builder, routes and existing test inspection; five Scenario jobs; alpha composites; original RGB/repair-mask/output hash checks; six Python syntax parses. Not performed: successful Unity import/rebuild/compile/tests, native screenshot review, APK/device testing, full regression suite, changes to any of the other six requested screens.

Next: once the user closes Unity, capture the existing baseline, apply the staged source/assets through the owning builder, verify Splash + measured Loading/Home transitions, inspect RU/EN and phone aspect captures, then deliver the actual Unity review. Keep subsequent screens in order: Loading, Home, World Map, Chapter Map, Pre-Level, Gameplay.
