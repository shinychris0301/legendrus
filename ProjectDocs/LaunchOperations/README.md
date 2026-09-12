# v1.19 Step 1 — Launch Operations Evidence

This directory is an operator-owned evidence boundary. The source package ships templates only.

After a real production release exists, create reviewed copies named:

- `android_vitals_snapshot.json`
- `launch_incident_summary.json`
- `launch_window_review.json`

Do not paste tester identities, Play credentials, keystore material, raw support conversations, device identifiers, or unrestricted logs into these files. Retain only aggregate/reviewed values that match the schema.

The source candidate intentionally contains no live evidence and therefore remains NO-GO.
