# Step 8.5 execution order

## If Step 8.4 runtime evidence already exists in this project

1. Apply the Step 8.5 hotfix. Do not delete `ProjectDocs/V1_20_STEP08_4/UnityRuntimeCaptures` or the Step 8.4 generated readiness JSON.
2. Run `Legends of Rus > Tests > Run v1.20 Step 8.5 Source Readiness Self-Test`.
3. Run `Legends of Rus > Production UI > v1.20 Step 8.5 > Evaluate Rollback-Safe Shop Development Cutover`.
4. Require `ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER`.
5. Review the approved Shop runtime capture and then run `Apply Rollback-Safe Shop Development Cutover`.
6. Run `Run v1.20 Step 8.5 Post-Cutover Pre-Android Regression Gate`.
7. Run all EditMode tests, all PlayMode tests, and the Step 5 complete demo-flow regression against the ACTIVE Shop cutover.
8. If clean, run `Record Post-Cutover Functional Regression PASS`.
9. Build/install to a physical Android device and execute ANDROID_REGRESSION_CHECKLIST.md.
10. If clean, run `Record Physical Android Regression PASS`.
11. Run `Evaluate Final Shop Development Cutover Acceptance`.
12. Require `ACCEPTED_SHOP_DEVELOPMENT_CUTOVER`.

If any regression occurs after Apply, use `Roll Back Shop Development Cutover` before continuing development.

## If using the Step 8.5 FULL project fresh

The distributed project intentionally contains no fabricated Step 8.4 runtime PASS evidence. Run the complete Step 8.4 sequence first (36 captures, approved Shop gate, provider gate, conditional readiness evaluator), then follow the steps above.
