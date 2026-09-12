# v1.20 Step 9.3 — Reference-Accurate Wave 3 Reconstruction Refinement

## Scope
Events Hub, Event Detail, Individual Ranking, Team Hub, Team Ranking, Team Chat, Friends List, and Mail.

## Reference authority
- **DIRECT APPROVED:** 0/8 pages.
- **DEMO DESIGN GUIDANCE:** Events Hub and Mail.
- **LOCKED STYLE SYSTEM DERIVED:** Event Detail, Individual Ranking, Team Hub, Team Ranking, Team Chat, Friends List.

Demo-guided similarity is informational only. It must never be promoted to approved production authority.

## Refinement policy
- Runtime candidates remain parallel/inactive under `Resources/ProductionUI/Wave03Pages`.
- `ReferenceOnly`, `DesignScreens`, and `ProjectDocs` remain forbidden prefab dependencies.
- No full-screen design/reference image is emitted as a runtime dependency.
- Events Hub and Mail are decomposed into semantic sections (HUD/header/tabs/cards or rows/detail/footer); configurable strings remain live Unity `Text`.
- Six no-reference pages use the locked navy/antique-gold Russian fairy-tale system with refined background/header/ornament layers.
- Visual prefabs contain no `Button` or EventTrigger authority and cannot grant rewards, publish chat, mutate teams/friends, or provide leaderboard/LiveOps authority.

## Source guidance improvement
Step 9 -> Step 9.3 informational source proxy:
- Events Hub correlation: **0.082 -> 0.707**, MAE **0.173 -> 0.061**.
- Mail correlation: **0.426 -> 0.782**, MAE **0.171 -> 0.063**.

These metrics compare source QA previews to demo/design guidance only. They are not approval gates.

## Cutover
**NO_GO_FULL_WAVE03_CUTOVER** remains mandatory because direct approved-reference coverage is 0/8. Step 9.3 refines candidates only.

## Unity verification
1. `Legends of Rus > Production UI > Build/Repair v1.20 Step 9.3 Refined Wave 3 Candidates`
2. `Legends of Rus > Production UI > Validate v1.20 Step 9.3 Refined Wave 3 Candidates`
3. `Legends of Rus > Tests > Run v1.20 Step 9.3 Reference-Accurate Wave 3 Self-Test`
4. `Legends of Rus > Tests > Run v1.20 Step 9.3 Wave 3 Four-Aspect Refined Candidate QA`
5. Re-run `v1.20 Step 9.2 Social-LiveOps Boundary Verification`.
6. For actual runtime evidence, re-run the Step 9.2 32-capture Play Mode workflow against the rebuilt Step 9.3 candidates.
