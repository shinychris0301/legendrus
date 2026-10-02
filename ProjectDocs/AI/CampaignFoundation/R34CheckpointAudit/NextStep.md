# Next step: collect evidence from the installed campaign

Keep normal content at rounds 1–200. The next implementation should make the current campaign recordable in bounded blocks before authoring 201–210. This is a plan, not an implemented recorder extension.

## Verified collection gap

`CampaignDevelopmentPreviewMenu.cs` exposes recorded candidates 11–20 R9/R12, 21–30 R13 and 31–40 R14. The latest menu reads `EditorContent/CampaignCandidates/Chapter01Sector01R14`; it does not read the installed R33 campaign. `CampaignPlaytestWindow.cs` also directs players to R14. The recorder supports configurable start/end rounds, but the menu supplies only those historical blocks. No `CampaignPlaytestRecorder.Attach` call was found in normal production gameplay. Playing later rounds normally therefore does not produce this campaign recording automatically.

The default export directory currently contains five valid manual JSON files covering 11–30. This is not proof that no external files exist. Asking for 41–200 recordings without preparing an appropriate current-content route would leave a tooling gap.

## Bounded implementation proposal

1. Add an Editor-only recorded preview that loads the exact four installed CampaignActive JSONs and definition. Preserve historical menu entries and the existing isolated `CampaignSaveSession` path; never write artificial progress into the player's normal save.
2. Select a ten-round block from the installed range; initially recommend 31–40, then 41–50. Stamp the export with the current content fingerprint and chosen range. Fail clearly for missing/invalid content instead of silently using an old candidate.
3. Verify that the preview uses the same PrototypeR4 matching rules, level data and current gem/item/obstacle artwork as normal gameplay. A recorded development preview remains distinct from a native UI/device acceptance run.
4. Prepare earlier progress as an explicit disposable fixture, acknowledge only earlier fixture story, start with 30 energy and zero inventory boosters, and make fixture actions distinguishable from actual attempts. Do not fabricate wins as player evidence or count setup toward measured difficulty.
5. Update the monitor to report the chosen source/range. Preserve recording on Stop Play Mode, outcome-before-rating and completion-before-replay behavior. Validate selected block boundaries, exact fingerprint, no modifications to live saves, failed/abandoned attempts, final-round/replay behavior and export recovery.
6. Human sessions rate difficulty, goal clarity and special creation after each round. Record retries, moves and boosters; preserve failures. Expand collection in ten-round blocks through 200. A pilot sample establishes local feedback, not a population win rate. Do not tighten budgets from the solver alone.

After collection tools: implement the reviewed returning-player catch-up choice and bounded reaction-copy passes, then current Android build/device and native UI acceptance. The old Android UI-audit APK is historical evidence, not a current 200-round build. A fresh read-only ADB query found no devices. Do not install, clear data or bypass existing build guards as part of this audit.

Rounds 201–250 remain a coarse river/old-wards outline. Define a distinct puzzle proposition and concrete 201–210 layouts before implementation; do not add a ward mechanic by implication. Effects and animations remain the final phase.
