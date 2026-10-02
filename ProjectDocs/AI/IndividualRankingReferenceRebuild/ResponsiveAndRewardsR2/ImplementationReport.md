# Individual Ranking — Responsive and Rewards R2

Reward chests were drawn at a fixed one-third source scale, making several sprites taller than their ranking rows. Purple chest artwork crossed gold separators and the highlighted player-row edge. Each approved chest now fits uniformly within its row with at least four reference pixels above and below; the rewards share the reward-column centre.

Only placement and scale changed. Whole source sprites, transparency, colors, names, source medals/title, original Russian Rules button, shared header and existing footer remain. No sprite was cropped, regenerated or overwritten. The selected user fonts and preview disclosure are preserved.

## Installed scope

Runtime reference-layout owner, builder, view, active prefab and cutover agree at revision 8. Six existing Assets files changed: ProductionIndividualRankingReferenceLayout.cs, ProductionIndividualRankingView.cs, ProductionIndividualRankingPrefabBuilder.cs, IndividualRanking.prefab, IndividualRankingCutoverState.txt and IndividualRankingAlignmentPlayModeTests.cs. All other 7,190 Assets files are unchanged; no additions/deletions. Exact paths/hashes are recorded in QA/ScopeAndVerification.json and Sources/AssetsBefore.json.

## Verification

Unity 6000.5.9f1 rebuilt/read back the active prefab and passed all 9 checks: 2 EditMode + 7 PlayMode, zero failed/skipped. Isolated saves were used. Twenty-one actual Unity captures cover reference/tall/narrow phones, Russian/English, all three periods, maximum balances and Rules open/closed. Eleven review views and native-pixel viewing passed in Chrome at 430×900 without script errors or document overflow.

Added checks for reward separation from row rails/scores, full aspect-preserved sprites, exact footer clearances, separate Back/Rules controls and adequate footer touch targets at tested sizes. The new language/period test verifies the tall Rules shield at both safe edges and over footer controls, closes it through a native pointer-click handler and checks unchanged saved progress. Existing route, typography, sample-data and modal Back behavior checks also pass.

Final Russian reference-size and tall English Rules captures were visually inspected. QA/VisualChangeScope.json records the pixel-change region against R1. This is a scope comparison, not an original-reference similarity score.

## Limits and next action

Standings and prizes remain disclosed examples. No live rankings, payouts or season dates were added. The fixed table leaves extra scenery above the footer on tall phones; existing frame surfaces, portrait overlap and source-font differences remain. No Android build, physical-device, backend, performance or full-project suite was run. No generation/Scenario request was made.

Next recommended page: Team Hub responsive/footer audit.
