# R23 — Quick formation, moving heads and material impact

The user asked to move closer to Royal Kingdom after rejecting R22's animation quality. We examined Dream Games' own animated Rocket and Dynamite demonstrations, then changed the emphasis from a long decorative formation to quick item recognition followed by tangible launch/contact/debris. [Reference observations and source links](ReferenceAnalysis.md).

## Runtime changes

- Creation uses the solid approved sprite with a quick scale reveal and completion overshoot. Gather is0.20s; completion0.24s, including a final exact full-sprite hold. Same-wave items finish together. The rest of the resolution timeline remains authoritative.
- Directional activation has two independently rendered sapphire tips, attached stretched exhaust and small shed streaks. Heads travel at the existing `Charge + Sweep × distance/span` cell-contact clock. Trail orientation follows row/column; effects are clipped at board edges. The previous fused projectile stamp is no longer scheduled.
- Bombs retain the central blast, with six separately placed flame shapes that expand along the radius clock and disperse with shell debris. Actual footprint/damage rules are unchanged.
- R4 ordinary gems break into six substantial regions of their actual approved sprite. The pieces move independently, tumble, fall and fade over0.62s. This replaces repeated generated fragment-cluster stamps. Special-item cells still do not emit hidden ordinary-gem fragments.

Reuses the approved item/gem textures plus existing R19/R22 effect textures. No new bitmap generation or competitor art import. New DirectionalLaunchView consists of two reusable non-interactive Canvas batches under the board mask, capped at32launches per batch. At maximum:2048trail vertices and256head vertices; each gem-color batch remains capped at16breaks/384vertices. These are code limits, not Android measurements. Recovery, disable and destruction clear/unsubscribe effects.

Integration is in the existing runtime-created R4 sandbox presentation, through SpecialSequenceView.Attach. No owning production prefab/builder needs regeneration. Presentation session identifier isR23. Campaign remains Legacy; no matching, moves, scoring, obstacle rules, core timing schedule, packages or graphics settings were changed. Firebird/color activation families still need their own later activation pass.

## Capture improvement

The prior recorder compressed PNGs and recreated its camera on each sampled frame. The observed row-launch samples were about0.08–0.18s apart, hiding short poses. The SequenceArt test recorder now reuses camera/targets, buffers RGB frames during playback, and encodes PNGs afterward. This is test-only code; the live game's runtime does not retain screenshot buffers. Recorded sample timestamps determine animated playback duration. Capture overhead still exists; this is not a fixed30fps or phone performance claim.

The review contains actual Unity row/column activation, bomb, combined chain, and five creation sequences. Both creation and activation clips now use buffering. Normal and half-speed controls, individual frames and R22 creation comparisons are provided. No interpolated animation or audio recording. Synthetic49/4move captions are the pre-existing test fixture HUD mismatch.

## Verification

See [QA/Verification.json](QA/Verification.json), [browser checks](QA/BrowserVerification.json), and [task-start scope](QA/Scope.json). Unity6000.5.9f1 uses the already authorized process-only Vulkan override and descriptor limit4096, with isolated test configuration. No EditMode run or physical Android profiling in this pass.

The initial art run passed8/8. The first broad run passed84/85; the expiry test still waited0.60s for the newly lengthened0.62s effect. Its final wait was corrected to0.72s. The next broad run passed83/85: the old recorder missed the shorter final hold, and expiry assertion could resume before Update. Creation recording was also buffered and the expiry fixture now yields through the cleanup update. Final focused validation covers these corrections, the six-chunk change and both recorders. New tests cover horizontal/vertical/bomb outcomes, contact-distance schedule, bounded meshes under overload, clipping, recovery/disable cleanup and input release.

This is an implemented visual candidate, not a claim of matching Royal Kingdom's production quality. Next: review normal-speed launch/contact clips, then refine the remaining color/Firebird activation families and profile on Android before campaign rollout.

Final focused run:23/23 passed, zero failures/skips. All85 distinct selected checks have a passing latest result across runs; the broad run was83/85 before the two test-harness corrections, not a single final85/85 run. Scope:9modified source/test files,2new script/meta files,0deleted,7434unchanged. No textures/prefabs/packages/project settings changed.

Review validation430/1440:596still loads,36animations,10R22comparisons; no JS errors or overflow.298actual Unity portraits and18animations.1403 unreferenced numbered PNGs pruned with SHA manifest; review/before-after evidence and logs retained.
