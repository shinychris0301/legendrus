# Campaign v2 adapter and migration rehearsal (R2)

## Explicit source usage

Construct `ValidatedCampaignCatalog` with the validated canonical definition. Submit `CampaignPackageInput` containing a chapter ID, revision and four JSON payloads: levels, story, Russian localization and English localization. `TryAdd` admits the package only after the reusable gameplay/story validators pass. The caller must surface the returned diagnostics; it must not retry a failed package as legacy to bypass campaign rules.

Use `ChapterLevelCatalog.Find(source, chapterId, roundId)` for explicit chapter lookup. It returns null for absent chapters/rounds; the live `Find(chapterId, levelId)` still uses the accepted default source. Build `source.Progression(records, acknowledgedFinales)` for availability decisions. Lookup is not permission to start a round: routing must check progression and the existing energy gate before board creation.

`LegacyChapterOne=true` is restricted to Chapter 1, requires the exact ten-level teaching-order validators, and must have `FinaleReady=false`. It preserves the old demo's finale semantics and limited English dialogue coverage. Newly authored packages must set it false and pass all campaign RU/EN story coverage.

`FinaleReady=true` requires a complete 500-round release with valid final milestone, localization and chapter completion routes. Partial packages omit completion data and `chapter.complete` triggers. The next chapter is not playable merely because its valid package is present.

R2 admits metadata and gameplay/story/localization catalogs only. It does not implement Addressables publication, environment asset downloads, cryptographic package signing, streaming/eviction, background music selection or frame-prefab changes. Those remain cutover/content-delivery tasks under the existing approval guards.

## Migration rehearsal usage

`CampaignMigrationRehearsal.Prepare(sourceV6Json)` validates a supplied source copy and returns a deterministic candidate preserving its exact bytes. `Validate(candidate)` checks its source hash and reconstructs its projection to detect tampering. `Run(sourceV6Json, failAtMutation)` runs the existing journal against memory; 0 means no fault, 1–8 cover staged write/flush, backup write/flush, primary write/flush and staging deletion/flush.

These methods never read the actual player save. Callers must use fixtures or an explicitly supplied sanitized copy. The candidate's source hash is its migration identity. A matching valid target produces a no-op; incompatible source/target is blocked. Rehearsal envelope version 7 is not registration of a production save version.

Production integration remains blocked until a target player codec, explicit legacy-story mapping and the existing persistence lifecycle tests cover real candidate writes/readback. Preserve economy/tutorial/daily/settings fields from the existing payload; never reconstruct balances from level records. Do not allow legacy normalization to discard future chapters.
