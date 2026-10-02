# R18 — Artwork-based special-item sequence

[Open animated portrait review](Gameplay_SpecialSequence_R18_Review.html) · [Recorded preview](Captures/Sequence.webm)

The user approved starting the revised full-sequence recommendation. This deliverable is the artwork and animated composition for the pre-implementation review. It is **not a Unity capture or production integration**.

## Delivered

- Two original generated transparent PNG atlases, 32 sprite regions total. Crystal formation, bomb assembly, shaped fire/smoke, directional projectiles, all six gem-color fragments, wood, chain, ice and shell pieces.
- A 10.2-second scripted portrait composition: four-match gathers into a directional item; an L-match gathers into a bomb; the powers converge and release three rows plus three columns; a secondary bomb chains; a wooden crate visibly changes from intact to damaged to destroyed; the board refills.
- Pause, replay, scrubbing, three playback speeds, six beat shortcuts and optional synthesized timing audio. Reduced-motion preference starts playback paused. A silent browser recording is also provided.
- A sprite inspection view with black, white and saturated-pink backgrounds. Actual gameplay gems, idle items, crate states, scenery, board frame and metal rails are reused from the project.

## Art and motion choices

The attack is carried by rendered crystalline bodies and textured blast shapes. Debris uses shaded fracture artwork, with different colors and material faces, instead of slicing every sprite into the same eight triangles. The first obstacle hit leaves its damaged artwork visible for roughly a second before the second blast destroys it.

Generation used the existing bomb and directional artwork as references. The first effect atlas lacked sufficient gutters; a second generation improved isolation. Generated row spacing is not perfectly uniform, so the composition uses measured row boundaries and alpha-bounded sprite regions rather than assuming sixteen equal crops. This also corrected the lower edges of the bomb-assembly frames. Final region checks found no boundary alpha above16 in all32 regions. Both final sheets were visually inspected over all three backings; no opaque rectangular matte was observed. Low-opacity glow remains intentional.

The source images are unchanged. No Python pixel edits or recoloring were applied to generated art. Python inspected alpha/region boundaries; the Canvas renderer samples the atlas regions to compose the animation.

## Relationship to gameplay

The R4 planner's RowWithBurst combination schedules three rows and three columns. The review models that shape, with first contacts derived from the illustrated moving fronts. The secondary explosion uses a two-cell square radius. This is still a scripted art demonstration: it does not execute the C# model, charge moves, persist rewards or prove legal round balance. The refill is authored for the preview. No claim of a recorded human play session is made.

Normal campaign initialization remains Legacy. No campaign or sandbox code was changed in this task. The eventual implementation should consume existing resolution/contact events and retain outcome hashes, rather than move rule decisions into effect playback.

## Limitations and next implementation work

This is the first artwork-based sequence candidate. Four key poses per formation/blast/projectile sequence are blended with motion; these are not finished high-frame-count flipbooks. Formation ornamentation has small differences from the exact idle sprites, which remain the approved project originals. Additional in-between artwork and registration work are needed after the direction is accepted. The optional audio is synthesized timing reference, not finished sound design; no haptics are implemented.

Only the wooden obstacle is animated through both stages here. Chain/ice assets are available for later studies; Firebird, rainbow and the full booster/combination set are not presented as completed.

Next: review this candidate's formation, burst shape, color, density and item identity. After acceptance, refine the animation frames and integrate the approved sequence into the existing R4 sandbox, fix the prior capture timestamp collision, then run Unity outcome/contact/recovery tests and Android profiling. Extend to the remaining families and plan explicit campaign integration after those checks. No rendering-backend/package change is required by this proposal.

## Verification

- JS syntax check passed.
- Browser review at430px and1440px: all15 source images load; six timeline beats render; replay advances; pause works; sound button enables/disables the WebAudio state; no script errors or horizontal overflow. Sound quality was not independently listening-tested.
- Black/white/pink inspection of both atlases; measured custom regions avoid the original equal-grid clipping.
- Actual browser screenshots of all six beats were inspected. The rail atlas was corrected to sample the same rail/junction regions as ProductionGameplayCell, and projectile body offsets were corrected for contact placement.
- Browser MediaRecorder exports the offline composition; its requested30fps is not a claim about Unity/device performance. `ffprobe` is unavailable; media metadata/playback is checked in Chrome instead.
- Source scope: compare Assets/Packages/ProjectSettings against task-start hashes in [Scope.json](QA/Scope.json).
- No Unity compilation/runtime tests, Android build, performance test or production migration occurred during this art-review stage.

[Verification](QA/Verification.json) · [Atlas regions](QA/AtlasRegions.json) · [Generation provenance](QA/ArtProvenance.json)
