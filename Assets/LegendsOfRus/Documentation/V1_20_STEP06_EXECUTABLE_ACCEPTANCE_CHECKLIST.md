# v1.20 Step 6 executable acceptance checklist

Use the exact Step 6 candidate. Static verification is not a substitute for these checks.

- [ ] Unity 6000.5.9f1 opens and compiles with zero errors.
- [ ] Step 5 Functional QA Self-Tests pass.
- [ ] Step 6 Functional Acceptance + Asset Readiness Gate passes.
- [ ] Complete EditMode suite passes.
- [ ] Complete PlayMode suite passes.
- [ ] Boot -> Home demo flow passes.
- [ ] ChapterMap -> WorldMap -> Home passes using visible Back hotspots.
- [ ] PreLevel -> ChapterMap visible Back passes.
- [ ] Gameplay pause/settings/resume keeps the live board state.
- [ ] Victory/fail retry/continue flows pass.
- [ ] Events/social/language/login/system-state routes have no dead ends.
- [ ] Shop/rewarded/provider-unavailable paths fail closed with no unverified grant.
- [ ] Android visible World Map Back returns Home.
- [ ] Android system Back follows the same deterministic parent policy.
- [ ] No unexpected save/progression/economy mutation occurs during navigation QA.

If any functional item fails, stop production-art runtime cutover and repair functionality first.
