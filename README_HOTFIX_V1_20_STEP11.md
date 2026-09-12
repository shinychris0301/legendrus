# v1.20 Step 11 Hotfix / Milestone Upgrade

Apply this archive over a clean **v1.20 Step 10.3 FINAL** project and overwrite matching files.

The hotfix adds Step 11 Editor-only RC stabilization tooling and documentation. It does not activate additional production pages, does not create regression PASS evidence, and does not create a Development RC freeze.

After import, run:

1. `Legends of Rus > Tests > Run v1.20 Step 11 Release Candidate Stabilization Self-Test`
2. `Legends of Rus > Release Candidate > v1.20 Step 11 > Evaluate RC Stabilization Status`
3. Complete any missing Step 10.3 runtime/cutover/regression/Android gates.
4. `Legends of Rus > Tests > v1.20 Step 11 > Validate Full Functional + Android Regression Evidence`
5. If eligible, `Legends of Rus > Release Candidate > v1.20 Step 11 > Create Development RC Freeze`
6. Validate the freeze before handing the build to QA.

Any later cutover or rollback changes the integration fingerprint and invalidates the freeze.
