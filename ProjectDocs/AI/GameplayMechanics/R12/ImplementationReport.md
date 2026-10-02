# Gameplay Mechanics R12 — special-item interaction

Implemented the first stage of the approved matching/animation recommendation: a new sandbox rule version, `PrototypeR4`. Unity verification completed after the user closed the Editor: 76 EditMode and 58 PlayMode tests passed, with zero failures/skips. Actual Unity captures are available in the review. No animation improvements are claimed in this rule milestone.

## Behavior

- Row, column and bomb items now activate when swapped with an adjacent ordinary gem, without requiring a color match. Spark and rainbow retain tap/drag activation. Blocked cells still reject invalid moves.
- Specials no longer count as ordinary gems with an invisible color when detecting runs or squares. An actual blast can still trigger another special.
- Rainbow combinations select the most common visible ordinary-gem color. Ties select the lowest color ID deterministically. Already-special cells are excluded from conversions, and consumed partners do not activate an extra time.
- Ordinary-gem collection goals no longer receive credit for a special's hidden color.
- The development sandbox defaults to the new rules. Historic rule versions remain selectable for replay. Normal campaign routes still use Legacy.

Existing board art, animation/audio presentation, five round layouts and move budgets remain unchanged. Effective difficulty can change with these interaction rules; automated solvability is not player balance acceptance. Session content identifies `MechanicsRoundsR4-RulesR4-PresentationR11`.

## Evidence

| Check | Result | Limit |
|---|---|---|
| Actual core plus new NUnit rule cases, compiled/run using Unity-bundled Mono/C# outside the Editor | 38 passed, 0 failed | Not Unity compilation/runtime verification |
| Historic Legacy/R1/R2/R3 examples captured before edits | All 244 plan and final-state hash records match byte-for-byte | Representative compatibility corpus, not exhaustive proof |
| Five-round bounded solver | All five solved within unchanged budgets | Data declarations exclude the unused Unity resource catalog; no human playtest |
| Unity EditMode attempt, Verify01 | Blocked: another Editor has project open | No tests executed |
| Unity EditMode Verify02 | 76 passed, 0 failed/skipped | Includes the 244-record compatibility corpus |
| Unity PlayMode Verify03 | 58 passed, 0 failed/skipped | Includes actual drags, sessions, five rounds, and existing effects/audio regression suites |

Round witnesses: rounds 1–5 solved in 2/4, 2/5, 2/4, 2/5 and 3/10 moves respectively. These are found solutions, not claims of optimality.

Evidence: [standalone summary](QA/StandaloneSummary.json), [38 cases](QA/StandaloneTests.txt), [historic baseline](QA/LegacyGolden.tsv), [current compatibility results](QA/LegacyCurrent.tsv), [round search](QA/StandaloneRounds.txt), [scope](QA/Scope.json).

## Scope and completed verification

Nine runtime/core sources changed, two existing test sources updated, two new test classes plus their metadata added. Exact paths are in Scope.json. No art, prefabs, round data, packages or project settings changed. Runtime-generated board components need no prefab rebuild. Historic recording witnesses explicitly retain their old rule version.

After the user confirmed Editor closure, these runs completed successfully:

```sh
python3 ProjectDocs/AI/GameplayMechanics/R12/Sources/Verify.py Verify02 EditMode --vulkan-verification --file-limit 4096
python3 ProjectDocs/AI/GameplayMechanics/R12/Sources/Verify.py Verify03 All --vulkan-verification --file-limit 4096
```

Vulkan is a previously authorized verification-process override only. Results are in [Verification.json](QA/Verification.json). Selected bomb/rainbow captures were visually inspected; the review shows individual actual portraits. No runtime fixes were needed during this verification. Task-owned test artifacts were cleaned and linker state restored after Unity exited. Scope remains 11 existing sources and 4 added test/meta paths. There is no physical-device or human acceptance claim. The historic baseline generator must not be rerun after code changes; use RunGoldenAfter.py for comparison.

## Following pass

The rules have passed Unity verification. Next implement causal animation sequencing: gather/merge when creating items, visible rainbow conversions, payload-specific Spark flight/impact, and chain reactions triggered at visible contact. Material-specific shattering follows. Current R11 effects remain in use; this rule pass does not address their remaining visual limitations.

Review verification: 24 capture/viewport checks passed at 430 and 1440 px, with no script errors or horizontal overflow. Mobile HTML review visually inspected. See [BrowserVerification.json](QA/BrowserVerification.json).
