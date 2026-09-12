# v1.20 Step 2 — Chapter 1 non-destructive migration dry run

Step 2 wraps the existing Chapter 1 level/story/Russian-localization data in a canonical staging package without changing the payload bytes. The active runtime path remains the existing Resources path.

The staging package proves:
- 10 legacy levels retained exactly;
- 7 story milestones retained exactly;
- all Russian localization entries retained exactly;
- source and staged SHA-256 values match for all three payloads;
- no Addressables mutation or runtime cutover occurred;
- no locked art, level balance, rewards, or progression changed.

This checkpoint does **not** authorize Chapter 1 migration. It provides evidence for later controlled cutover work after the revised Step 3/4 UI reconstruction and visual-regression work.
