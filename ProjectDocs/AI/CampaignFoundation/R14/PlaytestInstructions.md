# R14 recorded playtest: rounds 31–40

1. Enter Unity Play Mode and reach Home.
2. Choose **Legends of Rus → Development → Campaign Playtest 31-40 R14 Riverbank (Recorded)**.
3. Read the granary handoff, then play 31–40 **without inventory boosters**. Disposable prior-progress transactions consume 20 of 50 test energy, leaving **30 energy, one earned Mace and one earned Firebird**. Leave both boosters unused; any consumption is recorded separately. Live saves remain untouched.
4. Rate each result before Continue/Retry. Focus on delivery on 33/35/39, recovery on 36, the row+column combination on 37 and mixed objectives on 40. On round 37, the adjacent items are optional; you can swap them together, activate separately, or make other moves.
5. After winning 40, read the riverbank story. Save Recording, then use the monitor's Show recording or Copy recording path and share the JSON/path. Exit Playtest ends the preview; Replay 40 is optional. Round 41 and Chapter 2 remain unavailable.

The menu is a disposable candidate, not the production campaign. Earlier R9/R12/R13 menus remain available. Recordings survive Play Mode in `Application.persistentDataPath/CampaignPlaytests/Sessions/manual_campaign` and carry a `CampaignSector01R14-…` fingerprint.

```bash
PYTHONDONTWRITEBYTECODE=1 python3 ProjectDocs/AI/CampaignFoundation/R14/Sources/AnalyzeSessions.py '/path/to/CampaignPlaytests/Sessions/manual_campaign' --output /tmp/campaign-r14-results.json
```

The analyzer excludes older/automated recordings and separates boosted attempts. No R14 human results exist yet. Next after evidence review: 41–50 and the beacon milestone. All animation/effect refinement remains deferred to the final development phase.
