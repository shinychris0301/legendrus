# Individual Ranking — Reference Layout R2

The verified original from `/home/k2/Downloads/match3final/individual ranking.png` is now the design authority. R2 replaces R1’s sparse Overall/Friends layout with the original Day/Week/Season arrangement, three prize cards, fourteen illustrative standings, four columns, distinct portraits and medals, reward chests, a highlighted sample Ivan row, Back and Rules controls.

The canonical `Assets/LegendsOfRus/Resources/ProductionUI/Pages/IndividualRanking.prefab`, runtime/view, owning builder and cutover state agree on revision2. The new page-specific reference-layout helper migrates existing objects in place. All490 prior serialized object IDs and existing GUID/meta files remain intact. Gameplay and all other page prefabs/artwork were preserved.

## Source assets and Scenario

[Scenario audit](QA/ScenarioAudit.json):16 successful jobs,58 recorded CU—15 alpha requests at3CU and one clean birch-forest generation at13CU. No new character, chest or button artwork was regenerated. Two grouped extraction requests and a row-chest group retained neighboring fragments; local cleanup and seven individual chest requests followed. The bronze prize crop initially omitted its left edge and needed one corrected extraction. An interrupted CDN download was collected again without another paid submission.

41 new PNG resources use independent source portraits, medals, chests, icons, blank frames and a separate generated environment plate. Local masks remove neighboring labels/rails; source lettering and hidden panel faces were repaired before native labels were placed. Opaque portrait/panel interiors are retained. Repair-mask checks verify original RGB outside declared repairs before interpolation. Enlargement does not add source detail. [Asset provenance](AssetProvenance.json), [source assessment](Sources/AssetAssessment.json), [final processing](Sources/PrepareFinal.py).

## Behavior

- Native Day, Week and Season controls visibly switch their active state. All three intentionally show the same clearly labelled sample snapshot until a leaderboard exists.
- Example ranks, power and rewards match the supplied illustration; the highlighted12/512430 row is also a sample, not an asserted real player placement. Wallet balances and player level remain live and are independent of power.
- Season dates remain unannounced; no fabricated countdown, experience progress, prize claim or backend ranking.
- Rules opens a native explanatory overlay. Its shield and action guards block underlying controls; Close and system Back dismiss it before navigation.
- Existing Profile shortcut, Back→Home and profile/settings/shop/energy header routes remain working. Labels and counts are live RU/EN text using the existing font family.

## Verification performed

Unity6000.5.9f1 compiled/imported and rebuilt/read back the prefab in three runs. Run1:4 EditMode passed;8/11 PlayMode passed. Three new layout assertions caught English prize headings wrapping. Best-fit title bounds and source-frame slice density were corrected.

Run2:4 EditMode+11 PlayMode passed, zero failed/skipped. This includes7 Event Detail regression tests. After the final prize-card positions, column alignment and row-gap/ellipsis refinement, focused run3 passed2 Ranking EditMode+6 Ranking PlayMode, zero failed/skipped. Every latest result across15 distinct targeted tests passed. [Result summary](QA/UnityResults.json); commands/results are under `UnityVerification/run1..run3`. Full machine logs remain under `/tmp/lor-ranking-r2/`; only build-result excerpts and test evidence are included here.

Ten final Ranking capture pairs cover941×1672 RU/EN,576×1280 safe-area RU/EN,1080×2400 safe-area RU, maximum balances, Day RU, Week safe EN and Rules RU/safe EN. Six Event Detail capture pairs from run2 are retained as regression evidence. Capture tests checked text fit, safe areas, button raycasts, background coverage/aspect ratio, row separation, live balances/localization, no save/reward mutation, repeated binding, navigation and Rules behavior.

Manual review: original reference; alpha resources over black, white and magenta; clean forest output; run1 RU/Rules RU; run2 RU/tall EN; final run3 RU and Rules safe EN. Other final captures were checked automatically. Source hashes,16 Scenario output hashes/billing,41 installed asset hashes, Python AST, prefab identities, scope, whitespace and review links checked.

Not performed: APK build, physical Android test, full-project test suite, browser execution of the review, performance/memory profiling, calibrated color comparison, exact-font identification, or live leaderboard integration. Passing tests do not establish an exact visual match.

## Remaining differences

See [reference comparison](ReferenceComparison.md). Typeface/bevel, repaired panel texture, fine medal/portrait edges, regenerated background, reused header/footer and real live values still differ from the original. The design now follows the original composition and source artwork; it is not a pixel-perfect replica. Await user visual review before treating R2 as accepted. Authoritative ranking/scoring, season configuration, XP progression and actual rewards remain future feature work.
