# Victory rebuild: baseline and reference assessment

User approved moving to Victory after Gameplay R11. Two different local originals exist; the user subsequently selected the Downloads original. This assessment describes the pre-change baseline; see ImplementationReport.md for the completed rebuild.

The earlier approved reference has a resource bar, a smaller Firebird above a red Victory ribbon, three stars and a wood reward board. The newer Downloads candidate has a navy title at the top, a much larger spread-wing Firebird, larger stars and a dark score/reward board with a separate booster row. These require different assets and composition; filenames alone do not establish which one the user intends.

Baseline01 used Unity 6000.5.9f1 with isolated saves and passed six Edit Mode and fourteen Play Mode checks. Fresh actual native captures are under QA/Baseline01/Victory. No Android or device checks ran. Current native rendering still differs from both originals: regenerated Firebird/banner/control ornament, live typeface, star facets, reward composition and scenery placement. Actual score/rewards, replay handling, earned-star count, existing booster identities, navigation and honest ad availability must remain connected to the existing services.

The selected reference is the Downloads original. Isolate recoverable original art through Scenario, retaining RGB; repair hidden backgrounds only when necessary. Build the real Victory prefab and owner together, then compare fresh RU/EN and safe-area captures. Preserve the newly completed Gameplay screen.
