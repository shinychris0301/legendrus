# Hotfix/application note — v1.20 Step 8.5

Apply this hotfix over **v1.20 Step 8.4** (or the Step 8.4 tree containing your generated runtime evidence). It does not overwrite Step 8.4 Unity runtime capture files or generated runtime PASS locks because those evidence files are unchanged by the hotfix.

After import, run the Step 8.5 Source Readiness Self-Test. If Step 8.4 already reports `ELIGIBLE_SHOP_DEVELOPMENT_CUTOVER_READINESS`, evaluate and explicitly apply the rollback-safe Shop cutover. Otherwise complete the Step 8.4 capture/Shop/provider/readiness sequence first.
