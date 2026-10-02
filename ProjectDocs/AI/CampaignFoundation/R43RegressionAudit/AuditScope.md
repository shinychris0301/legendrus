# R43 regression audit scope

Unity 6000.5.9f1; project configuration unchanged. The prior process-only Vulkan approval applies; each verification process uses descriptor limit 4096. No Android build, installation or publishing is performed.

Inventory: 40 project test assemblies: 32 Editor-only and 8 Play Mode assemblies. The initial filter `LegendsOfRus.Tests` misses the older `LegendsOfRus.BalanceReview.Tests` namespace. The final Edit Mode sweep includes `LegendsOfRus.` and covers the 13 omitted balance-review cases. All eight runtime assemblies use the original runtime filter. Package test suites are outside this project regression audit.

Each ordinary run uses a fresh `/tmp/lor-regression-r43/<run>/config` for both XDG_CONFIG_HOME and LOR_LOADING_TEST_CONFIG_ROOT. Save and language restart phases share only their own dedicated isolated storage. Original player preferences and progress are not used. The tests retain their own fake-store isolation as applicable.

Per-page capture directories enable existing layout checks. Optional gameplay frame-sequence export is disabled; effects are not being redesigned. Campaign tests may write evidence to Unity's temporary cache fallback. The active campaign's 600 witness files were copied to QA/PlayMode01/ActiveWitnesses after the round-200 third witness completed, before the historical candidate suites ran.

Passing tests does not establish visual acceptance, human difficulty, mobile performance, live-service readiness or story pacing. Historical candidate tests also do not expand the installed campaign beyond 200 rounds.

The initial Edit Mode sweep exposed ten obsolete UI assertions and one inconsistent replacement-save fixture. RefreshRegressionAssertions.py records the bounded test-only corrections with original files retained. Production layouts, artwork, matching rules and save-recovery safeguards are preserved.
