# Campaign contract v2 — Foundation R1

Status: implemented and testable **pre-activation foundation**, not installed campaign content. The manifest is outside Resources and is not loaded by the game. Existing v1 tools, the ten-level runtime catalog and save schema 6 remain authoritative until a separately verified cutover.

## Approved product scope

- Twenty chapters, each with 500 main-path rounds: 10,000 total.
- Ten sectors per chapter, 50 rounds per sector. Sector is a navigation/content unit; narrative acts and milestone placement require authoring review.
- Chapter names/order in `campaign_definition.json` reuse the approved world-map definitions from the full story document. Reference artwork captions are not a second naming authority.
- The old scenario's 10,000+ rounds *per chapter* is superseded for the main campaign. Optional/seasonal/endless content is outside this count.
- Stable chapter IDs remain `chapter01`…`chapter20`. New campaign round keys are `chapter01.round001`…`chapter20.round500`. Existing integer `(chapterId, levelId)` keys 1:1…1:10 retain their identity. Do not rename legacy level JSON IDs during this milestone.

## Availability and progression

A manifest entry describes a planned chapter. It does not unlock content. The progression core accepts package metadata only from an adapter that has successfully validated the actual level/story/localization assets. R1 tests supply synthetic package metadata to exercise the contract; they are not authored playable levels.

Each admitted package declares a positive campaign content revision and contiguous validated round IDs from one. Partial releases are supported. All installed package metadata must match the selected campaign revision; mixed revisions, gaps, duplicates, over-500 counts and invalid IDs fail closed. Revisions are campaign-wide in R1; per-sector downloading/versioning is deferred.

Availability order: invalid identity → unavailable package/round → locked progression → playable. First chapter starts at round 1. Subsequent rounds require a contiguous prefix of wins, including legacy records with earned stars. Attempts/failures alone do not unlock anything. Chapter N starts only after the prior chapters' 500 rounds, published finales, and acknowledged campaign finales. A legacy level-ten finale never meets this condition.

Keep published predecessor metadata available when later chapter content is installed; R1 does not implement eviction, remote catalogs or downloads. Completed content may be replayed without issuing another first-clear reward; the existing reward ledger remains the fulfillment authority. This core never grants rewards or charges energy.

## Runtime cutover design (next implementation)

1. Add a validated adapter from installed chapter packages to the existing LevelData and StoryData consumers. Separate generic content validation from the accepted Chapter 1 demo teaching-order validator. Keep v1 migration byte-equivalence rules intact.
2. Replace Chapter-1-only Find/ResolveSelected/Next calls through the existing catalog boundary, not a parallel startup system. Resolve explicit chapter IDs and fail closed for absent content; never silently load Chapter 1 in response to a missing later chapter.
3. Add campaign progress/selection storage with versioned journal migration (see SaveMigration.md). Migrate a copy, validate it, then atomically commit; preserve the old journal and back up before cutover.
4. Route WorldMap/ChapterMap, Profile, Pre-Level, StoryService and ChapterComplete through the same availability/completion contract. Prefabs need no visual redesign; update builders/revisions only if their serialized bindings change.
5. Test Chapter 1 preservation, Chapter 2 transition, partial-release end, missing/corrupt package, interrupted migration, all 500 round boundaries, replay reward protection and final chapter termination.
6. Only then switch the runtime content source through the existing explicit activation mechanism. This R1 does not change activation flags, SaveService, GameState or Resources catalogs.

## First-sector authoring gate

Before generating rounds 11–50: agree the role of the accepted 1–10 finale (demo-only or rewritten campaign milestone); map teaching order and story beats; define difficulty peaks/recovery, goals, reward budget and representative board variety. Retain source seeds and content revision for reproducibility. Validate legal starts, playable moves, goal reachability and playtest difficulty. Do not infer balance or solvability from schema validity.

Chapter 2 can use a small development-only test package to prove integration before its 500 rounds are authored. Such fixtures must not be published or mistaken for campaign completion.
