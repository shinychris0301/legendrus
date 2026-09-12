# v1.20 Step 5 — Functional QA, Edge Cases, and Full Demo Flow Verification

This checkpoint continues the functionality-first pivot. The current demo design remains intentionally acceptable; final production asset reconstruction is still deferred until the functional flow is accepted.

## Runtime fixes

- Hardened the visible World Map Back control with a larger touch target and a source-bound callback.
- Hardened the Chapter Map Back control the same way.
- Added the missing clickable Pre-Level Back hotspot and bound it to `PreLevel -> ChapterMap`.
- Retained the central hardware Back/Escape policy: `ChapterMap -> WorldMap -> Home`.
- Corrected the Friends List parent route to `TeamHub`, matching the Step 4 route graph and the route from which Friends is opened.
- Added a Step 5 route-acceptance graph with the omitted `IndividualRanking -> Home` source route; the Step 4 36-page asset plan itself remains unchanged.

## QA coverage added

- Added PlayMode coverage that activates the real `Hotspot_Back` controls and requires `ChapterMap -> WorldMap -> Home`.
- Added PlayMode coverage that renders all 15 extended demo routes and requires at least one interactive hotspot on each.
- Added Friends List -> Team Hub Back verification.
- Added a Step 5 Editor self-test covering navigation parents, map bindings, demo-safety boundaries, save-v6, Addressables 2.8.1, production-backend gates, and package safety.
- Added an executable full-demo manual verification checklist and a machine-readable QA matrix.

## Design/asset policy

`Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json/csv/md` remains the authoritative 36-page resource plan. It is intentionally retained rather than replaced during functional QA. Production visuals remain a later phase and must be rebuilt from standalone Unity assets rather than full-screen captures.

## Unity acceptance sequence

1. Open with Unity `6000.5.9f1`.
2. Let scripts compile with zero errors.
3. Run `Legends of Rus > Tests > Run v1.20 Step 5 Functional QA Self-Tests`.
4. Run the complete EditMode suite.
5. Run the complete PlayMode suite.
6. Execute `V1_20_STEP05_FULL_DEMO_FLOW_CHECKLIST.md`.
7. On Android, verify both the visible World Map Back button and the system Back gesture/button.

Static/package verification is included in this release. Unity/device execution remains `PENDING_USER_UNITY_EXECUTION` until those checks are run in the Editor/device.
