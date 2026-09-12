# v1.13 Step 3 — Chapter Finale and Completion Flow

## Finale contract

Story schema v3 adds an explicit Chapter Complete presentation contract. It
identifies level 10 as the terminal level, `chapter01.finale` as its finale
scene, `chapter.complete` as the unique completion trigger, and the localized
copy used by the saved-progress summary.

The finale now contains five localized lines in this order: Narrator, Raven,
Firebird, Dobrynya, and Ivan. Its only return route is Chapter Complete. The
validator requires level 10 to own both the finale dialogue and completion
triggers, requires its milestone to be major, and rejects any second scene or
level that attempts to complete the chapter.

## Persistent state machine

Save schema v6 adds only `acknowledgedChapterCompletions`. Chapter completion
itself is derived from existing, authoritative progress instead of duplicated.

| Level 10 progress | Finale scene | Acknowledged | Phase | Next presentation |
| --- | --- | --- | --- | --- |
| Incomplete | Incomplete | No | In progress | Normal level flow |
| Complete | Incomplete | No | Finale pending | Finale dialogue |
| Complete | Complete | No | Presentation pending | Chapter Complete |
| Complete | Complete | Yes | Acknowledged | Chapter map / replay flow |

Finishing level 10 grants only its authored level reward. Finishing the final
dialogue persists the scene before routing to Chapter Complete, so closing the
app at Victory, during the dialogue, or on the completion screen cannot lose
the handoff. The completion acknowledgement is saved only when Continue, Map,
or Android Back leaves Chapter Complete.

Replaying level 10 after acknowledgement grants no first-clear reward, does not
repeat the finale, and does not reopen Chapter Complete.

## Runtime presentation

The approved `ChapterComplete.png` remains the visual source of truth and is
not modified. A non-raycast navy-and-gold runtime panel covers the baked reward
strip with truthful saved progress: completed levels, earned stars, maximum
stars, and a note that the final-level reward was already granted on Victory.
Both visible completion buttons and Android Back acknowledge the presentation
and route to World Map because Chapter 2 is not implemented yet.

## Full-route acceptance

Automated coverage runs the first-launch intro, all ten level completions, all
seven deterministic post-victory scenes, the five-line finale, Chapter Complete
recovery, acknowledgement, and replay suppression. The completed route contains
exactly eight persistent story scenes and requires no editor intervention.

Gameplay mechanics, balance, tutorials, economy rewards, and approved PNGs are
unchanged from v1.13 Step 2.
