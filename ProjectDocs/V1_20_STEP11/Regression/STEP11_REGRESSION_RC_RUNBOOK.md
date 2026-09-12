# v1.20 Step 11 — Regression / RC Runbook

1. Run Step 11 source stabilization self-test.
2. Generate/review the Step 10.3 36-page readiness matrix and complete missing runtime captures.
3. Optionally apply only evidence-eligible non-core cutovers.
4. Run the full EditMode + PlayMode + Step 5 flow regression.
5. Record Step 10.3 functional regression PASS.
6. Run the full physical Android checklist on the exact current integration.
7. Record Step 10.3 physical Android PASS and evaluate final Android acceptance.
8. Run Step 11 regression-evidence validation.
9. Evaluate Step 11 RC stabilization status.
10. If Development RC is ready, create the Development RC freeze.
11. Validate the freeze before every build handed to QA. Any later cutover/rollback invalidates the integration fingerprint and therefore invalidates the freeze.

Store-release RC remains blocked until every functional page is an active production route with direct approved visual authority and passing runtime fidelity.
