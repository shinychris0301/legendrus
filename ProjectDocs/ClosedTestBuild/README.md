# v1.18 Step 4 Controlled Test Build staging

This directory is for sanitized preflight/evidence files only.

Do not place keystores, passwords, upload keys, service-account credentials, Play Console exports containing account data, or signing secrets in this project.

The Step 4 source candidate does not invoke `BuildPipeline.BuildPlayer`, does not create a signed AAB, and does not upload to Google Play. Final build and upload remain explicit gated actions after required evidence and authorization are supplied.
