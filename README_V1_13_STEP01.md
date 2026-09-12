# Legends of Rus — v1.13 Step 1

This milestone moves Chapter 1 story structure and Russian narrative copy into
validated runtime data, adds a first-launch Chapter 1 intro before the pre-level
screen, and resolves every released `dialogue.*` level trigger through a
persistent localized story sequence.

Open this folder directly in Unity 6000.5.9f1. Run:

1. `Legends of Rus > Validate Commercial Vertical Slice`
2. `Legends of Rus > Tests > Run v1.13 Step 1 Self-Tests`
3. EditMode and PlayMode tests from Unity Test Runner

Implementation details are in
`Assets/LegendsOfRus/Documentation/V1_13_STEP01_STORY_DATA_CHAPTER_INTRO.md`.

Package revision 1 restores the retained
`StoryService.ShouldShowDialogueAfterVictory()` compatibility contract and
updates the legacy save-version assertion for the v5 story-progress schema.
