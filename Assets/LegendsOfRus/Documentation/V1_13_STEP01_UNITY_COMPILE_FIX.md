# v1.13 Step 1 — Unity Compile Compatibility Fix

The initial v1.13 Step 1 package replaced
`StoryService.ShouldShowDialogueAfterVictory()` with the stateful
`TryBeginPostVictoryDialogue()` entry point. Two retained PlayMode assertions
still compiled against the original predicate, causing CS0117 before tests
could run.

The original predicate is restored as a side-effect-free compatibility API. It
continues to report whether the last completed level contains a `dialogue.*`
trigger. `ScreenRouter` remains on `TryBeginPostVictoryDialogue()`, which
resolves that trigger through localized story data, starts the sequence, and
suppresses completed scenes.

The retained tutorial-persistence assertion now expects save version 5 and
checks that the new completed-story-scene collection exists. No art, gameplay,
balance, tutorial, or story content changed.
