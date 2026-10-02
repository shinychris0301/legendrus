# Campaign R14 — riverbank candidate, rounds31–40

The user finished the granary playtest and requested the next step. The exact R13 recording shows ten unassisted wins with one retry on25; all goal/special clarity feedback is clear. Preserve the first30 and proceed with the planned riverbank block. [Human evidence review](PlaytestReview.md).

## Content

Separate editor-only `Chapter01Sector01R14` extends the R13 package to40. Earlier first30 level definitions, scenes, milestones, reactions and localized values are preserved exactly. Old candidate assets and production Resources are unchanged. New seeds4131–4140; goals, budgets and rewards follow the provisional R7 briefs.

|Round|Role|Moves / colors|Board intent|
|---|---|---|---|
|31|Recovery|33 /5|Open blue/green collection|
|32|Normal|29 /5|Two chain pockets and blue collection|
|33|Normal|30 /5|Two feathers with ice below their lanes|
|34|Normal|29 /5|Three separated vertical crate groups|
|35|Hard|26 /6|Three feather lanes and ten chain cells|
|36|Recovery|33 /5|One central feather with an open exit|
|37|Teach|30 /5|Adjacent row/column items, optional combination, eight ice cells|
|38|Normal|29 /5|Separate crate and chain groups|
|39|Normal|28 /6|Two feathers at different heights and green collection|
|40|Hard|27 /6|Ten layered ice cells, eight crates and blue collection|

Round37 places a row item at(3,3) and column item at(4,3). Swapping them combines them; they can also be activated individually or left while playing other moves. No forced input or new tutorial overlay is added. No new reeds, currents, reverse gravity, bosses or visual effects are implied by round names. All boards use existing mechanics and artwork. Existing scenery and portraits render the story.

The riverbank milestone after40 uses the R7 Firebird line: the bank is brighter but safety is temporary; light the beacon so villagers can find their way. It does not complete the chapter or promise departure from the village. Round41 and Chapter2 remain unavailable. New round reactions/names are bilingual data; no new automatic reaction overlay.

First-clear31–40 total1830gold,6gems and one Living Light booster; no scene/sector/replay bonus. Content contract remains revision1, with R14 as an experiment identity rather than a published release.

## Preview

**Legends of Rus → Development → Campaign Playtest 31-40 R14 Riverbank (Recorded)** starts from Home in Play Mode. It uses the existing in-memory preview store and recording observer. Historical fixtures1–10 are imported as before; explicit campaign transactions11–30 create later disposable progress before recording attaches. The R14-only test allowance50 leaves30energy after20fixture starts. One Mace and one Firebird earned in that fixture remain visible; instructions request no inventory use. No live save/economy mutation.

The existing recorder bounds supply31–40 completion wording, Replay40 and unavailable41. Prior menus are retained. The new analyzer covers31–40 and requires the exact R14 fingerprint. Older R13 human evidence is reviewed separately, never counted as new play.

## Verification

See [verification](QA/Verification.json), [content preservation](QA/ContentChecks.json), [scope](QA/Scope.json) and [solvability screening](QA/BalanceScreening.json). Initial Verify01 was blocked before compilation by an open Unity project; no test success is inferred from that invocation.

Unity verification passed 28 distinct targeted tests: 14 Riverbank checks and 14 existing Granary regression checks, with zero failures or skips. These cover 40 boards × 3 attempt seeds, thirty new no-inventory completion/replay witnesses, optional row+column behavior without an automatic full win, actual animated 31/40 victories and story/completion transitions, fixture allowances, unchanged earlier content and unavailable 41/Chapter2. Captured earlier progress is explicitly fixture data, not human play. Three Python analysis tests and content-preservation/source-whitespace checks passed. The original human recording remains byte-identical to its preserved snapshot.

No new human R14, Android, full-suite or manual editor-menu walkthrough claims. No art/effect changes. Process-only Vulkan/descriptor4096 verification uses earlier authorization and leaves project graphics settings unchanged.

## Next

[Playtest31–40](PlaytestInstructions.md), rate delivery and combination clarity, then share the recording. Review before41–50/beacon. Production activation and device acceptance remain later work; all animation/effect refinement stays last.
