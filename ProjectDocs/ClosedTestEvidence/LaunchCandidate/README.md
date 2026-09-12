# v1.18 Step 8 launch-candidate evidence staging

This folder is for reviewed, privacy-minimal launch evidence only.

Expected operator-retained files (not shipped in the source candidate):

- `../production_access_decision.json` — reviewed Play production-access decision, or explicit reviewed evidence that the special production-access decision is not required for this account.
- `../launch_candidate_readiness.json` — final reviewed launch-readiness booleans and the exact candidate/AAB/freeze hashes.
- `../CANDIDATE_FREEZE_MANIFEST.json` and its SHA-256 from Step 5.

Do not place Play credentials, service-account keys, keystore passwords, tester identities, raw feedback transcripts, Android IDs, advertising IDs, IMEIs, or device serials in this evidence set.

Step 8 does not upload to Play Console and does not start a production rollout. The final production release and store-publishing authorization flags are separate manual gates and ship `false`.
