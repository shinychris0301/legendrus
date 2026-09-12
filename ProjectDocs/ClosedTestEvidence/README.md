# v1.18 Step 5 retained evidence staging

This directory is a local evidence intake boundary for the controlled-test candidate freeze.
It is intentionally empty of real signed artifacts and physical-device attestations in the shipped source candidate.

Expected operator-supplied files after the exact candidate passes Unity verification:

- `Artifacts/<candidate>.aab` — exactly one signed AAB for the candidate.
- `signed_aab_evidence.json` — metadata/attestations for that AAB. The Editor runner recomputes file name, size, and SHA-256 from the actual AAB and does not trust those values from JSON.
- `physical_device_evidence.json` — five privacy-minimized physical-device profile records. Each record must reference the same AAB SHA-256.

The physical-device evidence schema intentionally does not contain device model, serial number, Android ID, advertising ID, IMEI, tester name, email, or free-form logs.

Do not place keystores, passwords, Play account credentials, signing secrets, or personal tester information in this folder.
