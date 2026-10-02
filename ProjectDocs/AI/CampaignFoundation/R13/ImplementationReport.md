# Campaign R13 — granary candidate, rounds21–30

The user finished R12 and asked to proceed. The new local recording shows ten unassisted first-attempt wins and matches the tuned candidate. Follow-up user feedback: “yes. the difficulty is normal.” This supports retaining the R12 settings while authoring the next block; it is not population balance acceptance. See [human review](PlaytestReview.md) and [recorded qualitative feedback](QA/UserFeedback.json).

## Content and scenario

A separate editor-only `Chapter01Sector01R13` package extends R12's levels to30 and preserves its first20 level definitions exactly. Earlier R9/R12 assets and production Resources are unchanged. Full story/localization files preserve existing scenes, milestones, reactions and keys, adding the planned granary milestone after30. The single-line Ivan scene states that supplies are safe and points toward the crossing. It does not heal the World Tree, restore the Firebird fully, end the chapter or unlock Chapter2. Round31 remains unavailable.

|Round|Role|Moves / colors|Authored board intent|
|---|---|---|---|
|21|Recovery|33 /5|Open red/green collection after20|
|22|Normal|29 /5|Eight staggered crates, accessible side lanes|
|23|Normal|30 /5|Two feather columns and four chains; free bottom exits|
|24|Teach|31 /5|Guarded optional horizontal-five opportunity for a Color item|
|25|Hard|26 /6|Central twelve-crate block with accessible outer faces|
|26|Recovery|33 /5|Four separated crates and open matching space|
|27|Normal|29 /5|Two feather lanes with six crates; no blocked exit cells|
|28|Normal|28 /5|Eight two-layer ice cells in side patches|
|29|Normal|28 /6|Ten chain cells separated by open lanes|
|30|Hard|27 /6|Ten crates, two feathers and green collection|

Budgets/goals/rewards come from the R7 briefs and remain provisional for new content. New seeds are3121–3130. Round24's optional swap is(3,2)→(3,3); guards prevent random neighbors from changing the intended Color creation. There is no forced tutorial overlay. Round-specific reactions and names are bilingual content; no new reaction overlay or dedicated granary artwork was added. Existing Dialogue/Game UI renders the new data.

First-clear21–30 rewards total **1830 gold,6 gems and one Firebird booster**. No scene/sector bonus or replay reward is introduced. Content contract revision remains1; R13 is the experiment identity, not production publication.

## Preview and collection

New Home/Play Mode menu: **Legends of Rus → Development → Campaign Playtest 21-30 R13 Granary (Recorded)**. Earlier menus are preserved. It uses the existing in-memory preview store, legacy fixtures only through10, then actual campaign start/win/ack transactions for11–20. Fixture energy/rewards leave20test energy and one Mace; instructions explicitly request no inventory use. Earlier story scenes are acknowledged as fixtures, leaving the well scene as the opening handoff to21. Fixtures are established before the observer attaches, so they are not recorded as human attempts.

The recorder now exposes start/end bounds; completion text and optional replay buttons use those bounds. R13 ends after30's story with Replay30 and an unavailable31 message. R12 still ends after20 with Replay20. Existing save-first Exit handling and repeated-ack idempotency are retained. The monitor identifies the content and can save/reveal/copy the recording. R13 analysis covers21–30 only and requires an exact fingerprint; old R12 evidence is excluded.

## Checks and limits

[Verification](QA/Verification.json) · [Scope](QA/Scope.json) · [Solvability screening](QA/BalanceScreening.json) · [Content preservation](QA/ContentChecks.json).

Targeted Unity tests validate the catalog and first20 equality, ninety deterministic legal starts without initial matches, Color creation on24 across three seeds, and thirty inventory-free solution/replay witnesses for21–30. Animated round21 and30 wins exercise21→22, victory→granary dialogue→completion, repeated recovery without rewards/energy changes, explicit Replay30, and unavailable31/Chapter2. Earlier progress for captures is a declared transaction fixture, not human play.

Captures are actual Unity rendering. Story uses existing approved scenery and portrait. Completion surrounds are black in the capture harness. Runtime effects/artwork are unchanged. Automated proof of solvability is not a measure of enjoyment or difficulty; there are no new human R13 or Android results, no full-suite or manual editor-menu walkthrough claims. Verification uses the already authorized process-only Vulkan/descriptor4096 settings; project graphics settings remain unchanged.

## Next

Use [the recorded playtest](PlaytestInstructions.md), especially delivery23/27, Color24, recovery26 and mixed30. Review its human evidence before31–40/riverbank. Production activation, remaining campaign content and Android acceptance remain later work. All animation/effect refinement stays at the final step.
