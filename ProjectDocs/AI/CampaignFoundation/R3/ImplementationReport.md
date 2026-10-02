# Campaign Foundation R3 — campaign saves and persisted selection

Implemented a version-7 campaign player codec and explicitly constructed development save session. The session combines R2's validated local chapter source with saved selection and R1 progression rules. It is not registered in startup or the active UI.

## Save and migration

`CampaignPlayerSave` has an explicit campaign format identifier, campaign ID, content revision, player state, exact original v6 JSON and SHA-256 provenance. Version 7 is now a concrete campaign payload, distinct from R2's rehearsal candidate. The nested player DTO retains its existing v6 field shape; only this new outer codec owns campaign validation. Legacy GameState normalization is not applied.

Migration preserves balances, energy timestamp, boosters, preferences, daily state, tutorial acknowledgements, scores, stars, attempt/retry/failure/completion counts, selection and reward claims. Derived progression fields are recalculated. The old ten-level story scenes and finale acknowledgements remain in the immutable source archive, while active campaign narrative lists start empty. This prevents an old Chapter 1 finale from counting as 500-round campaign completion. It does not yet define the story-event mapping for live playback.

The session accepts an explicitly supplied legacy copy and writes only `lor.campaign.dev.primary`, `.backup` and `.staging` through the existing save journal. It never discovers or imports a real user's save automatically. A successful primary flush plus exact readback is required for a committed write. Repeated migration is a no-op; a conflicting source copy, malformed payload or unsupported version blocks writes. Hash provenance detects accidental mismatch, not malicious tampering or server-authoritative integrity.

The codec requires canonical JSON, validates campaign IDs/records/finale prerequisites and refuses regression of archived legacy reward claims and record counters. Progress remains stored even when its content package is absent. Large ledgers use bounded GZip/base64 storage, preserving the existing 1 Mi-character journal payload limit and imposing an 8 MiB expanded UTF-8 bound. Canonical JSON is intentionally not a general hand-edited import format.

## Selection integration

`CampaignSaveSession.SelectRound` evaluates the validated package and persisted progression before committing chapter/round selection. `ResolveSelectedRound` uses the existing explicit-source catalog lookup and returns no level for missing or locked content. Selection does not spend energy or grant rewards. Returned save objects are detached snapshots.

The development session is composed with a caller-supplied `ISaveJournalStore`, campaign definition and validated catalog. No default storage provider, PlayerPrefs registration, parallel startup, navigation cutover or new UI is installed. The next milestone must connect actual run outcomes, energy charges, rewards and story acknowledgements through an explicit transaction before any live cutover.

## Verification

Verify01: 178/179 EditMode checks passed. The full 10,000-record ledger exposed a redundant pass through the v6 importer's size limit. Removed that redundant parse: v7 canonical field validation and campaign semantic validation now own the expanded ledger. The legacy importer and shared journal limit remain unchanged.

Final Unity outcomes are recorded in `QA/Verification.json`, with raw commands, logs and XML under `QA/Verify02`. The 25 R3 cases cover all eight journal interruption points, retry/no-op migration, archived claims, corrupt-primary backup recovery, unsupported versions, malformed payloads, persisted selection with a fresh disk-store object, missing packages, detached snapshots and a 10,000-record compressed round trip. Existing campaign/pipeline regressions and two native win/fail journeys are included.

Interruption tests simulate loss of unflushed memory-store writes. The disk fixture checks reopening a private temporary directory, not OS power-loss durability. Full Chapter 1 and Chapter 2 campaign packages are synthetic in-memory fixtures, not authored or balanced new levels. Native journeys exercise the unchanged legacy runtime. No physical Android, IL2CPP campaign-save test, whole-project suite, live migration, remote content delivery or provider/backend activation is claimed.

## Changed production and test files

- Added `Scripts/Runtime/Services/CampaignPlayerSave.cs` and `CampaignSaveSession.cs`, with new metadata.
- Added `Tests/CampaignAdapterEditMode/CampaignSaveSessionTests.cs`, with metadata.
- Added exposed content revision to `Scripts/Runtime/Gameplay/ValidatedCampaignCatalog.cs` for session compatibility checks.
- Reused extracted fixture helpers in `Tests/CampaignAdapterEditMode/CampaignAdapterTests.cs`.

All paths above are under `Assets/LegendsOfRus`. `QA/Scope.json` compares this turn's pre-run hashes across Assets, Packages and ProjectSettings. Review documents and QA evidence are additional. Existing artwork, prefabs, catalog resources, activation settings and live save services are outside this change.

## Next step

Campaign Foundation R4: implement a development-only round lifecycle transaction for start, retry, failure, victory and reward acknowledgement, with duplicate-result and interrupted-write tests. Prepare the concrete mapping of the accepted ten-level opening arc into the 500-round chapter before altering accepted scenes. Author and balance the first 50-round sector after that narrative review; do not manufacture the remaining campaign by duplicating test fixtures.

## Final results

Unity Verify02 passed **179/179 EditMode** (25 new R3 plus 154 prior campaign/pipeline cases) and **2/2 native PlayMode journeys**, zero failures/skips. Retained 36 Unity captures; English Pause capture inspected. Browser disclosure behavior, containment at 430/1440 widths and four local links passed with no JavaScript errors.

Before/after hashes: 7,236 existing files unchanged, two expected files modified, six added, zero deleted. Restored temporary Addressables link.xml and its metadata from the exact pre-run backups after Editor cleanup. No art, prefab, catalog, activation, Android-setting or live-save-service change.
