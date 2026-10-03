# Rounds 201–210 — recording review

The next human acceptance block is ready for collection. The user confirmed on2026-10-03 that it has **not been recorded yet**. The current review reads the five available historical exports and finds **zero qualifying outcomes** for the installed201–210block. Every round remains pending; no balancing or acceptance is inferred.

Record through Unity6000.5.9f1 → Play Mode → Home → **Legends of Rus → Development → Campaign Active Playtest (Recorded)** →201–210→**Start installed-campaign recording**. Use actual human moves, retain losses/free retries and rate Difficulty/Goal clarity/Special opportunity before Continue/Retry. Use Save now and Copy recording path at completion. [Full collection instructions](../R1/CollectionInstructions.md) explain the existing30energy/zero-starting-booster in-memory fixture and its limits.

Hard rounds205/210 and recovery201/206 are authored labels, not measured difficulty. [Current per-round checklist](QA/CurrentRecordingsFinal/Rounds.csv) includes actual move budgets and explicit pending states. Fill the201–210 row in the existing [block observations](../R1/BlockObservations.csv), especially the hard→recovery transition and the round210 story checkpoint. Do not substitute solver results or synthetic QA preset progress for human play.

Review the actual exported JSON with:

```bash
PYTHONDONTWRITEBYTECODE=1 python3 ProjectDocs/AI/ChapterOneHumanAcceptance/R2RecordingReview/Sources/ReviewRecording.py \
  --session '/absolute/path/to/playtest-SESSION_ID.json' \
  --output /tmp/lor-human-201-review-01
```

Use an existing regular JSON recorder export and a **new output directory**. Repeat `--session` for independent sessions. Exact copies count once; divergent snapshots with the same session ID require selecting the canonical export rather than choosing by filesystem time. Omitting `--session` scans only the documented workstation's `manual_campaign` JSON folder. No backup/journal import, game launch or network operation occurs. Inputs and native evidence remain read-only; existing outputs and product/recorder storage destinations are refused.

The tool writes `Review.json` and `Rounds.csv`. It checks the live five-resource campaign fingerprint against retained native per-round evidence, manual provenance, exact block/fixture context, rules, settled outcomes, dates and known booster use. Automated/sandbox/stale/truncated/duplicate-attempt evidence is excluded. Interrupted and active attempts remain visible but do not count as completed outcomes. Losses/retries remain in the counts; booster-assisted and unassisted outcomes are separate. Ratings remain pending when absent. Missing measurements are null, never fabricated defaults.

Exit0 means **ready for human review**, including recorded wins for all10rounds and ratings on retained eligible outcomes. It does not grant acceptance, verify human identity, measure continuous campaign pacing or recommend tuning automatically. Exit2 means missing evidence, incomplete play, missing ratings or input/conflict issues; consult the JSON status. Block observations and human assessment are still required. The actual current scan exits2 with `needs_human_recording`, as expected.

**Validation:** final30Python boundary tests pass, including loss/retry retention, duplicate/conflict handling, malformed inputs, preserved measurements and refusal to write product files. [Test log](QA/FinalReviewChecks.txt) · [Actual pending review](QA/CurrentRecordingsFinal/Review.json) · [Scope/final validation](QA/FinalValidation.json). The earlier28-case run is retained separately;30is the final total, not58.

Only the review tool/tests/evidence and five handoff/collection documents change. Existing acceptance decisions, approved art/prefabs/settings/guards and R5APK remain unchanged. Inspection found an existing test explicitly preserving hidden default Fail captions, so this task does not turn that reviewed presentation into an unapproved UI change. No Unity compilation/runtime test, new APK, gameplay capture, physical testing, hosting, normal personal journal access or Git commit/push was performed. Phone/hosting work remains deferred and final effects remain last.
