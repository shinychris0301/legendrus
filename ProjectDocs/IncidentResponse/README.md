# v1.19 Step 2 incident / hotfix / rollout evidence

This directory contains templates only. Do not rename a template to a live evidence filename until the evidence exists and has been reviewed.

Live filenames consumed by the Editor runner:

- `incident_register.json`
- `hotfix_candidate_evidence.json`
- `rollout_control_review.json`

The retained schemas intentionally contain no tester identity, device identity, Play Console credentials, signing passwords, raw crash logs, or unrestricted free-form feedback.

The Step 2 tooling never builds, signs, uploads, halts, resumes, increases, or replaces a Play release. It evaluates evidence and reports whether a requested human action is valid and separately authorized.

Google Play behavior rechecked 2026-09-09:

- staged rollouts for updates can be halted and later resumed;
- halting stops additional users receiving that staged version, but users who already received it remain on it;
- a bad bundle should be replaced by a new fixed release;
- many fully rolled-out releases can be halted so an eligible previous version is served, but the first release on a track cannot be halted because there is no prior version to restore.
