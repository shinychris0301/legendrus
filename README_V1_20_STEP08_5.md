# v1.20 Step 8.5 — Rollback-Safe Shop Development Cutover and Android Regression Acceptance

This milestone adds the explicit Shop-only cutover/rollback mechanism and post-cutover Android acceptance gate.

## Important
- The packaged project is pre-cutover.
- Step 8.4 runtime evidence remains mandatory; no runtime PASS evidence is fabricated by this package.
- Shop is not added to the accepted 10-route ProductionCoreLoopPageRegistry.
- Shop activation uses a separate `ProductionMetaPageCutoverRegistry` Resources state.
- The other eight Wave 2 pages remain candidate-only/non-authorized because approved direct reference coverage is 1/9.

See `ProjectDocs/V1_20_STEP08_5/EXECUTION_ORDER.md`.
