# Current milestone: v1.20 Step 7

Start with `README_V1_20_STEP07.md`. Wave 0 shared production assets and Wave 1 12-page candidate reconstruction are authored; active runtime cutover remains disabled pending executable and visual/device acceptance.

# Current milestone: v1.20 Step 6

Start with `README_V1_20_STEP06.md`. This checkpoint freezes the Step 5 functionality baseline and prepares the 36-page production asset reconstruction program. Source acceptance passes; Unity/Android executable acceptance remains required before runtime visual cutover.

# Current milestone: v1.20 Step 5

Start with `README_V1_20_STEP05.md`. This functionality-first QA checkpoint hardens the real World Map/Chapter Map Back hotspots, fixes Friends List -> Team Hub parent routing, adds executable PlayMode coverage for the actual Back controls and all extended demo routes, and audits the retained 36-page asset-resource plan. Final production-art reconstruction remains deferred until the functional flow is accepted.

# Current milestone: v1.20 Step 3.3R-FIX01

Start with `README_V1_20_STEP03_3R_FIX01.md`. This recovery ships ten physical runtime page prefabs and 59 standalone Resources assets, adds direct asset-based runtime fallback, and blocks the old Step 3.3/R2/R3 builders from leaving stale prefabs behind. Final Unity/device visual sign-off remains pending; the reference-derived environment/hero art still requires final artist-grade cleanup.

# Current milestone: v1.20 Step 3.3R.2

Start with `README_V1_20_STEP03_3R2.md`. Home, World Map, and Chapter Map now have page-specific reference-match production components and a deterministic Unity prefab builder. Static component reconstruction matches the pinned 941x1672 references exactly; final Unity/device visual signoff remains pending. Next: v1.20 Step 3.3R.3 Pre-Level / Gameplay / Pause Visual Rebuild.

# Current milestone: v1.20 Step 3.3R.1

Start with `README_V1_20_STEP03_3R1.md`. The Step 3.3 visual mismatch has been audited page-by-page. Reference-match sign-off is now a separate mandatory gate. No runtime, gameplay, PNG, package, or project-setting change is made in this audit checkpoint. Next: v1.20 Step 3.3R.2 Home / World Map / Chapter Map Visual Rebuild.

# Current milestone: v1.20 Step 3.3

Start with `README_V1_20_STEP03_3.md`. Ten core-loop routes now use screenshot-free production page prefabs built from the canonical UI kit. The original screenshot files remain as QA references and for later non-core waves, but the ten cutover routes cannot fall back to them.

# Current milestone: v1.20 Step 3.2

Start with `README_V1_20_STEP03_2.md`. The canonical production UI kit now contains 34 separate no-text PNG sprites and a deterministic Unity prefab builder for 21 reusable components. The 22 screenshot-backed routes remain unchanged until Step 3.3 page reconstruction and visual verification.

# Current milestone: v1.20 Step 3.1

Start with `README_V1_20_STEP03_1.md`. The production UI placeholder audit and replacement matrix are implemented. Runtime screenshot cutover, screenshot deletion, locked-art overwrite, and prefab generation remain unauthorized. The next checkpoint is v1.20 Step 3.2 Canonical UI Kit Reconstruction.

# Current milestone: v1.20 Step 2

Start with `README_V1_20_STEP02.md`. Chapter 1 canonical-package generation and byte-equivalence dry-run tooling are implemented. Runtime still uses the legacy Resources payloads; Addressables mutation, runtime cutover, locked-art changes, and production publication remain unauthorized. The revised next checkpoint is v1.20 Step 3 Production UI Asset Reconstruction and Screenshot Removal.

# Current milestone: v1.20 Step 1

Start with `README_V1_20_STEP01.md`. The 20-chapter content-pipeline contract, templates, legacy Chapter 1 validation boundary, ownership rules, and QA tooling are implemented in source. Chapter migration, Addressables mutation, runtime cutover, automatic art generation, approved-art overwrite, and production content publication remain unauthorized.

# Current milestone: v1.19 Step 8

See `README_V1_19_STEP08.md`. v1.19 operating-baseline closure and v1.20 transition review are implemented as fail-closed local tooling. Production Analytics, Remote Config, LiveOps publishing, automatic mutations, and v1.20 preparation remain unauthorized in the shipped source.
# Current milestone: v1.19 Step 7

See `README_V1_19_STEP07.md`. Cycle retrospective, archive integrity, and next-cadence planning are implemented as fail-closed local tooling; production Analytics, Remote Config, archive upload, LiveOps publishing, and automatic mutations remain disabled.

# Current milestone: v1.19 Step 6

See `README_V1_19_STEP06.md`. LiveOps outcome, experiment-decision, and rotation-closeout review are implemented as fail-closed tooling; production analytics, Remote Config, automatic mutations, experiment traffic changes, and publishing remain disabled.

# Current milestone: v1.19 Step 5

See `README_V1_19_STEP04.md`. LiveOps cadence, aggregate content health, and experiment-readiness review are implemented as fail-closed tooling; production analytics, Remote Config, live mutation, traffic allocation, and publishing remain disabled.

# Current milestone: v1.19 Step 3

See `README_V1_19_STEP03.md`. Post-launch trend, D1/D7/D30 retention, and economy guardrail review are implemented as aggregate-only fail-closed tooling; live economy and analytics mutations remain disabled.

# Legends of Rus — Commercial Vertical Slice v1.18 Step 6 — Closed Test Upload, Go/No-Go, and Rollout Readiness

Start with `README_V1_18_STEP06.md` and
`Assets/LegendsOfRus/Documentation/V1_18_STEP06_CLOSED_TEST_UPLOAD_GO_NO_GO_ROLLOUT_READINESS.md`.

This checkpoint adds a privacy-minimal, fail-closed local decision layer for closed-test release preparation, closed-test health/go-no-go review, conditional Google Play production-access readiness, and production rollout readiness. It does not build or upload an AAB, access Play credentials, create a Play release, publish to testers, apply for production access, or start/halt/resume a production rollout.

The candidate retains Addressables 2.8.1, all prior compile/test hotfixes, the v1.17 privacy/compliance boundary, and the v1.18 Step 1–5 QA/release/freeze foundations.


## v1.18 Step 7 — Closed Test Execution, Feedback Triage, and Production Access Evidence

See `README_V1_18_STEP07.md`. Source tooling is implemented; real closed-test, feedback, production-access decision, and publishing evidence remain fail-closed.


## v1.19 Step 1 — Production Launch Operations and Post-Launch Observability

See `README_V1_19_STEP01.md`. Source tooling is implemented; real launch authorization, Android-vitals/incident evidence, Play actions, rollout changes, and rollback actions remain external and fail-closed.

## v1.20 Step 3.3R.3 — Pre-Level / Gameplay / Pause Visual Rebuild

The second reference-match correction wave is implemented. Pre-Level is reconstructed from 12 page-specific components; Gameplay uses five fixed-shell components around the live Match-3 board; Pause uses seven central-modal components over the real gameplay page. Source-side component proof is exact within the declared scopes, while final Unity/device visual sign-off remains pending. See `README_V1_20_STEP03_3R3.md`.


## v1.20 Step 4 — Functionality-first pivot

Final visual reconstruction is temporarily deprioritized. Step 4 completes the demo navigation/function route graph first, fixes Map Back routing, and records the final asset requirements for every page. See `README_V1_20_STEP04.md`.


## v1.20 Step 7.2 — Candidate Prefab Build / Four-Aspect QA / Cutover Decision

Twelve screenshot-free Wave 1 candidate prefabs are now physically serialized under `Resources/ProductionUI/Wave01Pages`. Four-aspect source containment passes 48/48, but the active visual cutover is intentionally **NO-GO** because reference fidelity, three missing direct references, production Cyrillic typography, Unity captures, and Android signoff are not complete. Active runtime logic/prefabs remain frozen.

## v1.20 Step 7.3 — Reference-Accurate Wave 1 Refinement

Step 7.3 adds the inactive `Art/Production/Wave01Refined` candidate set, reference-measured layout reconstruction, live Cyrillic typography hierarchy, and new Unity QA/cutover gates. The accepted functional runtime and active production registry remain unchanged. See `README_V1_20_STEP07_3.md`.

## v1.20 Step 7.4

Step 7.4 adds Unity Play-Mode four-aspect candidate capture, live World/Chapter map progression-state decomposition, runtime-reference fidelity analysis, and an explicit rollback-safe eight-page conditional development cutover. See `README_V1_20_STEP07_4.md`. The distributed project keeps the active Step 7.3/accepted runtime visuals unchanged until the Unity evidence gates pass and the developer explicitly applies the cutover.

## v1.20 Step 8 — Wave 2 production asset reconstruction
Adds parallel screenshot-free candidates for Characters, Daily Reward, Profile, Inventory, Shop, Special Offer, Out of Energy, Booster Select, and Reward Claim. Active runtime UI/gameplay/navigation remains frozen; see `Assets/LegendsOfRus/Documentation/V1_20_STEP08_PRODUCTION_ASSET_RECONSTRUCTION_WAVE02.md`.


## v1.20 Step 8.2 — Wave 2 Runtime QA / Provider Boundary / Visual Acceptance
Adds a 36-capture Unity Play-Mode QA harness, exact critical-authority source freeze checks, and a strict Shop approved-reference runtime visual gate. Full Wave 2 cutover remains NO-GO and is not implemented in this milestone. See `README_V1_20_STEP08_2.md`.


## v1.20 Step 8.3 — Reference-Accurate Wave 2 Reconstruction Refinement

Wave 2 candidate visuals are refined using semantic sectional production components. Shop now passes the strict source approved-reference proxy; six demo-guided pages are informational, Inventory/Special Offer remain style-system-derived, provider/economy authority is frozen, and active Wave 2 cutover remains disabled. See `README_V1_20_STEP08_3.md`.

## v1.20 Step 8.4 — Wave 2 Runtime Capture / Approved Shop Gate / Conditional Cutover Readiness

Adds actual Unity Play-Mode four-aspect capture tooling for the nine refined Wave 2 candidates, a strict direct-approved Shop runtime visual gate, provider/economy boundary re-verification, and a Shop-only conditional development-cutover readiness evaluator. The distributed project performs no active Wave 2 cutover; full Wave 2 remains blocked because direct approved reference authority is 1/9. See `Assets/LegendsOfRus/Documentation/V1_20_STEP08_4_RUNTIME_CAPTURE_APPROVED_SHOP_GATE_CUTOVER_READINESS.md`.

## v1.20 Step 8.5
Adds an evidence-gated, rollback-safe Shop-only development cutover plus explicit post-cutover functional and physical Android regression acceptance. Full Wave 2 cutover remains unauthorized (direct approved authority 1/9). See README_V1_20_STEP08_5.md.


## v1.20 Step 9 — Production Asset Reconstruction Wave 3
Eight parallel screenshot-free Events/Rankings/Social candidate pages are authored. Active runtime and server-authority boundaries remain unchanged; 0/8 direct approved references means cutover is blocked pending Step 9.2+.

## v1.20 Step 9.2 — Wave 3 Runtime QA / Social-LiveOps Boundary / Visual Acceptance
- Adds 32-capture Unity runtime QA for the 8 Wave 3 candidate pages at four target aspect families.
- Adds frozen-authority verification for ScreenRouter, DemoFeatureState, GameState, core/meta production registries.
- Candidate prefabs remain presentation-only: zero Button/EventTrigger behavior and no reference/design/QA dependencies.
- Events Hub and Mail runtime comparisons are informational demo-guidance only; direct approved reference authority remains 0/8.
- Active Wave 3 cutover remains blocked.


## v1.20 Step 9.3 — Reference-Accurate Wave 3 Reconstruction Refinement
Refines all eight Wave 3 candidate pages. Events/Mail use demo-guided semantic sectional reconstruction with live text; six pages remain locked-style derived. Social/LiveOps authority and active production UI remain frozen. Full Wave 3 cutover remains NO-GO because direct approved-reference coverage is 0/8.


## v1.20 Step 9.4 — Wave 3 Refined Runtime Capture / Reference Authority / Conditional Acceptance
Adds 32-capture refined Wave 3 runtime evidence, explicit approved-reference intake, page-level approved-reference fidelity gates, and conditional acceptance readiness. Existing Events/Mail DesignScreens remain non-authoritative; active Wave 3 cutover is not performed. See `README_V1_20_STEP09_4.md`.

## v1.20 Step 10 — Wave 4 account/language/system reconstruction
Adds parallel screenshot-free production candidates for Settings, Language Select, Login, Tutorial Overlay, Offline State, Maintenance State, and Empty/Error State. Settings is demo-guided; six pages are locked-style-derived. Runtime/backend authority and active production routing remain unchanged.

## v1.20 Step 10.2 — Wave 4 Runtime QA / Account-System Boundary / Final 36-Page Audit
Adds 28-capture Unity runtime QA for the 7 Wave 4 account/language/system pages, frozen ownership checks for Settings/guest/language/tutorial/system-state behavior, and a final 36-page audit that reports source reconstruction, runtime evidence, direct approved-reference authority, and active cutover as separate statuses. The package ships with no pre-generated runtime PASS evidence and performs no Wave 4 cutover. See `README_V1_20_STEP10_2.md`.


## v1.20 Step 10.3 — Final Runtime Integration, 36-Page Cutover Readiness, and Full Android Regression

This milestone keeps the accepted ten-route core production registry frozen while adding a generalized, page-scoped meta cutover lane for evidence-qualified non-core pages. Runtime evidence, approved-reference fidelity, authority freezes, rollback snapshots, and physical Android regression are evaluated separately. No new page is activated by the distributed package.


## v1.20 Step 11 — Controlled Production UI Cutover / Full Regression / RC Stabilization

Adds evidence-qualified controlled cutover orchestration, full functional/Android regression validation against the current integration fingerprint, and a development release-candidate freeze manifest. Store-release readiness remains a stricter separate gate requiring 36/36 production routes and direct approved runtime-fidelity evidence. No additional page is activated by the distributed package. See `README_V1_20_STEP11.md`.

## v1.20 Step 11.2 — Approved Reference Closure / Production Route Expansion / Store-RC Readiness

Adds an explicit 36-page approved-reference closure workflow, cumulative rollback-controlled production-route expansion batches, and a strict Store-RC freeze gate. Missing candidate art is never self-approved: canonical 941x1672 references require an explicit user-authored approval manifest and interactive promotion. The current package remains 10/36 active routes and 10/36 direct approved functional references; Pause and ChapterComplete are the highest-priority active authority gaps. See `README_V1_20_STEP11_2.md`.


## v1.20 Step 11.2A — Pause + Chapter Complete Reference Authority Closure

The exact existing 941x1672 Pause and ChapterComplete design pages were explicitly approved by the user and promoted to `Art/ReferenceOnly/ApprovedScreens`. Direct functional reference coverage is now 12/36 and all 10 currently active core production routes have direct approved visual authority. Runtime fidelity is still a separate gate; no production route or gameplay logic changed. See `README_V1_20_STEP11_2A.md`.

## v1.20 Step 11.2A Runtime Closeout — Pause + Chapter Complete

Adds a focused 8-capture Unity runtime-fidelity path for the two newly approved active Wave 1 pages. The final 36-page evidence model accepts this supplemental evidence only for Pause and ChapterComplete; all other Wave 1 pages retain the full 48-capture contract. The package ships with no runtime PASS evidence and performs no route or gameplay changes. See `README_V1_20_STEP11_2A_RUNTIME_CLOSEOUT.md`.

## v1.20 Visual Page 01 — Splash / Title production implementation
- User-approved 941x1672 SplashTitle reference promoted to ApprovedScreens/SplashTitle.png.
- Runtime is layered and screenshot-free: separate background, title logo, language frame, Start frame, Settings/Account/Help icons, live version/language/tagline/legal text.
- Added low-amplitude 2.5D motion for Ivan/Masha, Firebird shimmer, background parallax, logo float and Start glow using unscaled time (no Animator dependency).
- SplashTitle is enabled as a rollback-safe meta production cutover. Remove SplashTitleCutoverState.txt to revert to the inherited demo route.
- Start → Loading; Language → LanguageSelect; Settings → Settings; Account → Login; Help → TutorialOverlay.

## v1.20 Visual Page 01-R — Splash/Title Reference-Accurate Reconstruction

Corrective production rebuild of Splash/Title. The previous rectangular-crop composition is replaced by non-rectangular separated assets positioned at exact 941x1672 reference geometry. The approved full-screen image remains reference-only. Static reconstruction correlation is ~0.99835 with MAE ~0.00183 before Unity rendering. Character life uses restrained upper-body/glow overlays so the approved composition does not drift.
