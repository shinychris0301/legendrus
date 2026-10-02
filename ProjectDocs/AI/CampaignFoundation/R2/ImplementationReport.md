# Campaign Foundation R2 — local package adapter and save rehearsal

## Implemented

The runtime assembly now contains an explicitly constructed `ValidatedCampaignCatalog`. It consumes actual level/story/RU/EN JSON and implements `IChapterLevelSource`, the new explicit-source overload of the existing `ChapterLevelCatalog.Find` boundary. It does not install itself in startup, scan Resources, select content for the player, or substitute Chapter 1 when requested content is missing.

Level and story validation now expose separate campaign entry points. Existing callers still use the legacy ten-level Chapter 1 and its major milestones unchanged. Campaign checks support chapter IDs 1–20 and 1–500 contiguous rounds. Partial packages cannot claim chapter completion or route to the finale. Complete campaign finales require round 500. Every future campaign package must resolve its story references and reactions in both RU and EN, use the canonical Russian chapter name, and match the campaign content revision.

The explicitly tagged legacy bridge validates the actual accepted Chapter 1 and its current English dialogue coverage. Its previously documented 23 missing English reaction/completion keys are not silently declared fixed; this exception cannot publish a campaign finale. Converting the legacy source directly into a campaign package is rejected until its finale and localization are adapted.

The adapter snapshots accepted JSON, rejects duplicate/replacement packages, and returns fresh catalog objects. R1 progression consumes metadata generated from these validated assets rather than arbitrary world-map positions.

## Migration rehearsal

`CampaignMigrationRehearsal` uses the existing strict v6 payload decoder and R1 progress planner. It preserves the exact entire source JSON inside a canonical candidate plus its SHA-256, mapped round IDs, claim flags and selection. Legacy finale acknowledgements remain inside the legacy payload; campaign finales start empty.

The rehearsal uses the existing `SaveJournalRepository`/`SaveEnvelopeCodec` with an exclusively in-memory store. It injects interruption before each of eight write/flush/delete mutations, discards unflushed data, reloads a fresh repository, validates complete old/new payload recovery, retries migration and checks a second migration makes no writes. Tampered candidates, unsupported source versions and unknown progress are rejected.

Envelope payload version 7 is **a rehearsal marker**, not an activated new PlayerSaveData schema. The candidate codec is not a production player-save replacement. No real PlayerPrefs, journal, settings, balance or currency is changed by the rehearsal. A production migration still requires the full target codec, explicit story mapping, persistent-device interruption tests and readback acceptance.

## Verification and scope

See `QA/Verification.json` for actual Unity results, `QA/Scope.json` for before/after hashes and `QA/BrowserChecks.json` for review checks. Test Chapter 2 and 500-round datasets are synthetic fixtures constructed in memory from real LevelData shapes. They verify schema/selection/gating boundaries, **not authored or balanced Chapter 2 gameplay**. No new levels were added to Resources.

The existing native win/fail journeys are rerun as regression coverage. This milestone does not claim a whole-project suite, all-level solvability, physical Android acceptance, backend integration or live cutover.

Changed existing production files: LevelData.cs (explicit source interface/overload and campaign validator), StoryData.cs (campaign validation entry point), Runtime.asmdef (reference to existing ContentPipelineCore). Added ValidatedCampaignCatalog.cs, CampaignMigrationRehearsal.cs, their metadata, a dedicated Editor test assembly and tests. Artwork, prefabs, accepted source catalogs, GameState, SaveService, activation flags, Android settings and the v1 pipeline policy are preserved.

## Next step

Campaign Foundation R3: define and implement the actual campaign save payload and one-time migration transaction, then integrate the validated source with chapter selection/progression in an isolated development configuration. Proposed narrative mapping: preserve the ten-level story as the legacy opening arc; do not award campaign completion at round 10; author a distinct finale at 500. That mapping needs a concrete story review before changing accepted dialogue. The subsequent first 50-round sector needs deliberate narrative, difficulty and reward authoring, not bulk copies of the test fixture.

## Final results

Verify01 passed 152 EditMode tests. After adding localization lookup and fully validated finale-to-Chapter-2 checks, Verify02 passed **154/154 EditMode** (26 adapter/rehearsal + 128 existing foundation/pipeline) and **2/2 native PlayMode journeys**, zero failures/skips. Retained 36 actual Unity captures.

Scope hashes: 7226 pre-existing files unchanged, three expected source/assembly files modified, 9 new source/test/metadata files, zero deletions. Temporary Addressables link.xml/meta were restored from exact pre-run backups after automatic Editor cleanup. No art, prefab, live catalog, save or activation changes.
