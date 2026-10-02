# Gameplay Mechanics R11 — impact audio

After R10 art alignment, the next bounded presentation step is sound synchronized with the sandbox effects. Inspection found an existing ProductionFeedbackProvider and PlatformHapticProvider, but none of the `Audio/SFX/Gameplay/sfx_game_*` clips referenced by the provider exist in Assets. This pass supplies board-local sound for the current non-Legacy sandbox. It does not migrate campaign rules or change the approved round difficulty.

[Listen and review](Gameplay_Mechanics_R11_Review.html).

## Implemented

- Sixteen original synthesized mono cues: swap, rejection, gem shatter, special formation, bomb, directional sweep, Firebird flight/arrival, rainbow/Living Light, mace, Firebird booster, Sea Pearl renewal, wood, ice, chain metal and runic seal. Generated from deterministic oscillators/noise, with no imported recordings or Royal Kingdom audio. Generation script and peak/RMS/hash manifest are included.
- `PrototypeBoardAudio` attaches with the existing non-Legacy presentation components. Four precreated AudioSources and a bounded 24-entry queue; no source creation during commands. Resources preload once per board; PCM imports avoid first-use compressed decoding. Total WAV bytes: 485,804 (about 474 KiB); decoded Unity memory was not profiled.
- Sounds consume actual motion/obstacle events. Bomb/line impact follows the existing 0.12-second anticipation; Firebird arrival follows its 0.52-second flight. Gem shatter follows each clear wave; cascade pitch rises modestly and is capped. Actual inventory booster kind selects its cue.
- First obstacle hits use lower gain/pitch; final breaks use full material gain. No sound for an unchanged layer count. Same-type simultaneous hits coalesce, keeping the stronger break when damage and destruction share one beat. Repeated cues have a short cooldown; higher-priority impacts can replace quieter lower-priority voices. Stale queued sounds are discarded rather than replayed after a frame stall.
- Reads the existing SFX setting, clears pending/current sound on mute, focus loss, pause, disable and command recovery. No delayed replay when returning. Does not create an AudioListener or a second settings/service system.
- Shared feedback publication gains an overload that bypasses only the generic SFX sink when the board owns sound. Existing vibration routing, settings checks and signal observers remain in place. No new vibration patterns or platform APIs in this pass.
- Session content version is `MechanicsRoundsR4-PresentationR11`, distinguishing new player recordings from R10 and older results.

The owning runtime board initializer and the actual sandbox path are updated together. These components are constructed at runtime; no prefab rebuild is needed. All approved art, obstacle stages, puzzle rules, layouts, move budgets, inventory costs and Legacy campaign initialization remain unchanged.

## Verification

Unity 6000.5.9f1, process-only Vulkan override authorized earlier by the user, child-only soft descriptor limit 4096, isolated config directory. Project backend/settings remain unchanged.

- Verify01: 6/6 new audio PlayMode tests passed.
- Verify02: 54/55 combined checks passed. The new bomb timing assertion checked only the most recent cue after waiting; the next match cue had already played under the combined run. Replaced this test assumption with per-cue playback timestamps. Audio scheduling was not changed to satisfy the test.
- Verify03 final combined result: **55/55 passed**, zero failures/skips. Includes six new audio checks, all 24 R10 mechanics/storage/round/impact checks, and 25 existing shared audio/haptic/fidelity checks. See [Verification.json](QA/Verification.json).
- Browser: 32 sound/viewport playback-load checks across 430/1440 widths, damage playback-rate check, sequence controls and no page overflow/JavaScript errors. These check APIs and files, not perceived audio quality.
- Actual Unity capture in the review shows the R11 sandbox. Still images are not audio synchronization evidence; runtime event/timestamp tests provide that check. Source audio previews are explicitly distinguished from a recording of Unity's mixed output.

[Scope](QA/Scope.json) records exact differences from the task-start inventory, preserving pre-existing local changes. Temporary test scenes/linker artifacts are cleaned back to their initial state.

## Remaining acceptance

No human listening judgment, new manual session, physical-phone speaker/haptics check, Android build or performance profile is claimed. The sounds are original synthesized first-pass designs and need player review at ordinary listening volume. Existing generic production/meta audio resource gaps remain outside this sandbox pass.

Next: replay the five rounds and stocked fixtures with sound enabled, checking impact timing, sound fatigue, first-hit/final-break distinction and music balance. Supply the new session and specific notes. Use that evidence before campaign rollout; do not change accepted difficulty just to improve presentation.
