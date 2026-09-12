# v1.13 Step 2 — Story Milestones and Character Reactions

## Deterministic story schedule

Story schema v2 maps post-victory scenes directly to completed Chapter 1
levels. Story presentation no longer depends on trigger-array order.

| Completed level | Scene | Major milestone | Return route |
| ---: | --- | --- | --- |
| 1 | Firebird found | Yes | Chapter Map |
| 3 | Raven watches | Yes | Chapter Map |
| 4 | Elder's request | No | Chapter Map |
| 6 | Embers fade | Yes | Chapter Map |
| 8 | River shadow | No | Chapter Map |
| 9 | Living Flame | No | Chapter Map |
| 10 | Chapter finale | Yes | Chapter Complete |

Levels 2, 5, and 7 return directly to the map after Victory. Persistent scene
IDs and return routes are unchanged, so save-version 5 replay suppression and
the Step 1 intro continue to work without migration.

Pending mapped scenes are resolved from completed level progress in ascending
milestone order. If the player quits or the runtime resets after Victory but
before Continue, opening the selected level resumes the earliest unfinished
story beat. An unfinished active scene cannot be replaced by a different one,
and Escape is consumed while Dialogue is open so progress cannot be abandoned.

## Outcome reactions

The same story catalog contains twenty reaction records: one victory and one
failure reaction for each of the ten released levels. `ScreenRouter` captures
the played `LevelData` before completion can advance the selected level and
before screen routing clears the active board. It then resolves the Russian
speaker and body text for that exact outcome.

Reactions are transient, deterministic, repeatable UI state. They never enter
`completedStoryScenes`. A compact non-raycast plaque is rendered above the
result title on the existing Victory or Fail screen. Missing reaction data
degrades safely to the unchanged result screen.

## Validation contract

Validation rejects unordered or duplicate level mappings, missing scenes,
scene triggers not authored on the mapped level, orphan dialogue scenes,
incorrect major milestone levels, duplicate reaction IDs or level/outcome
pairs, unsupported outcomes, missing per-level coverage, and missing Russian
localization keys.

PlayMode coverage verifies the exact schedule, levels without interruptions,
major milestone set, complete reaction matrix, deterministic localized lookup,
captured victory/failure context, replay suppression, compatibility predicate,
runtime-reset recovery, active-scene protection, and invalid-data rejection.

## Visual boundary

No approved PNG is added or modified. Victory, Fail, Dialogue, Gameplay, and
all other screen skins remain byte-identical to v1.13 Step 1. Runtime reaction
panels use Unity UI primitives and do not overlap result controls. Dialogue
text masks are explicitly non-raycast so they cannot intercept Continue input.
