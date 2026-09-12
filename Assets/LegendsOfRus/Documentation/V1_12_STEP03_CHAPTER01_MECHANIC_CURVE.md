# v1.12 Step 3 — Chapter 1 Mechanic Tutorials and Difficulty Curve

## Released rollout

| Level | Rating | New mechanic | Primary contract | Tutorial follow-up |
| ---: | ---: | --- | --- | --- |
| 1 | 1 | Basic matching | Two color collections | First successful move |
| 2 | 2 | Score | Collection + score | First goal progress |
| 3 | 4 | Ice | Clear ice + collection | First obstacle hit |
| 4 | 2 | Recovery | Light ice reinforcement | None |
| 5 | 4 | Chains | Clear chains + collection | First obstacle hit |
| 6 | 6 | Crates | Clear crates + score | First obstacle hit |
| 7 | 3 | Recovery | Light crate + ice mix | None |
| 8 | 4 | Bring-down | Two feathers + chains | First feather collected |
| 9 | 5 | Survival | Sixteen valid moves + ice | First successful move |
| 10 | 7 | Finale | Eight crates + three feathers | None |

The curve rises by at most two rating points between adjacent levels. Levels 4
and 7 drop by at least two points, grant at least two additional moves versus
the preceding peak, and introduce no new mechanic. Level 10 is the unique
highest-rated level.

## Tutorial runtime contract

Tutorial steps are authored in each `LevelData` record with a stable ID,
mechanic key, trigger, Russian title, and Russian body. `LevelTutorialSequence`
queues matching steps in authored order and never schedules the same step twice
within an attempt. `LevelTutorialPresenter` listens to board progress events,
waits until the board can accept input, pauses it, displays the queue, and then
resumes it. Dismissed IDs are stored in save-version 4 data.

Supported triggers:

- `level_start`
- `first_move`
- `first_goal_progress`
- `first_obstacle_hit`
- `first_item_collected`

## Validation contract

`Chapter01ProgressionValidator` verifies that all seven teaching stages appear
in order, only one new mechanic debuts on a level, each debut has a
`level_start` tutorial, recovery levels do not introduce mechanics, tutorial IDs
are globally unique, difficulty jumps stay bounded, and the finale remains the
highest-rated level. The generic schema validator separately checks tutorial
content, trigger keys, IDs, difficulty rating bounds, goals, board coordinates,
obstacle durability, objective items, rewards, and story triggers.

## Visual boundary

No image file is added or modified. The tutorial panel is generated from Unity
UI primitives above the immutable Gameplay skin and does not alter the locked
board rectangle, gem sprites, safe-area behavior, or approved screen assets.
