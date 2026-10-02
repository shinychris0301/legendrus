# Chapter 1 — full installed chapter

The latest user priority is to complete Chapter 1 before the proposed live-service milestone. The chapter definition already specifies 500 rounds. This pass appends 201–500 and a real finale at 500; earlier rounds and historical candidates are retained.

## Installed behavior

- Normal Home → Chapter Map loads all 500 rounds through `ActiveCampaignContent`. Progress and prerequisites still determine which nodes are playable. A completed round-200 save can continue at 201 without replaying earlier rounds.
- Existing gems, match-created items, layered ice/chains/crates and feather delivery remain the gameplay vocabulary. No new animation, booster, damage rule or purchase flow is introduced.
- Six new 50-round sectors use ten puzzle families with deterministic geometric variations and authored goals. Each ten-round block includes recovery and harder rounds. These are 300 distinct configurations, not a claim of 300 independently hand-drawn boards or human-balanced levels.
- Thirty additional milestones and 600 victory/failure reactions have Russian and English text. The village arc proceeds through the river wards, hillside, dusk patrol, last approaches and departure. The original intro, 27 milestones and 400 reactions remain intact.
- Round 500's five-line finale leads to the existing Chapter Complete page. Completion acknowledgement is separate from the round reward. The summary reports rewards already claimed; it does not grant them again. The wider fading remains unresolved and Chapter 2 is unavailable.
- The recorded Active playtest menu exposes all 50 ten-round blocks, including 491–500. Its disposable earlier-progress fixture is separate from normal saves. Its end-of-block page is intentionally distinct from normal Chapter Complete navigation.

## Review and reproduction

`ChapterPlan.md` records scope and acceptance criteria. `RoundBriefs.json` contains every new round's full configuration and intent. `Sources/AuthorChapter.py` regenerates the appended data from task-start snapshots while asserting old-record preservation. The immutable `Chapter01CompleteR44` candidate matches the four installed JSON files exactly. Keep prior candidates as historical evidence.

Runtime changes explicitly enable the validated full-chapter finale and allow the recorded fixture to recognize its completion contract. Pending-story lookup skips already-acknowledged milestones before loading their round data, avoiding repeated full-catalog deserialization for later blocks. Recorded setup detects a repeated pending key rather than looping indefinitely. Existing round selection, services, save storage and UI routes remain the owners of progress. No parallel campaign startup or new UI prefab is needed.

## Remaining acceptance work

Human difficulty, story pacing and repetition across a 500-round chapter still need review. Retained human sessions cover rounds 11–30; automated solutions do not substitute for later human sessions. New story lines use approved characters and concise localized captions, but literary quality is not established by structural validation. Returning players with missing acknowledgements may still receive a long catch-up sequence; optional recap remains separate work.

Current Android build/device acceptance and the outstanding R43 runtime regression failures remain open. Live accounts, purchases, social services and same-attempt continuation are not delivered by completing the chapter. Resume the revised live-service plan after Chapter 1 acceptance; keep animation/effect redesign for the final phase.

See `TestResults.md` for actual execution outcomes and their limits.
