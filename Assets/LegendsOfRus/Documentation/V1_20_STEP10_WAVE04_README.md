# v1.20 Step 10 — Production Asset Reconstruction Wave 4

Wave 4 completes the source reconstruction lane for the remaining seven account/language/tutorial/system-state surfaces: Settings, Language Select, Login, Tutorial Overlay, Offline State, Maintenance State, and Empty/Error State.

## Safety policy
- Candidate prefabs are parallel/inactive.
- No full-screen DesignScreens or ReferenceOnly asset can be a runtime dependency.
- Text remains live Unity UI text.
- Settings state remains owned by save-v6/GameState.
- Login remains guest/demo only; identity/cloud providers are not activated.
- Offline/Maintenance/Error are presentation states only; no network or remote-config authority is simulated.
- Tutorial completion remains owned by the existing data-driven tutorial system.
- No font binary is bundled.

## Reference authority
Settings has one existing demo/design guidance screen. It is not direct approved authority. The other six Wave 4 pages are derived from the locked Russian fairy-tale design system. Production cutover remains blocked pending runtime QA/reference authority.

## Unity commands
1. `Legends of Rus > Production UI > Build/Repair v1.20 Step 10 Wave 4 Candidates`
2. `Legends of Rus > Production UI > Validate v1.20 Step 10 Wave 4 Candidates`
3. `Legends of Rus > Tests > Run v1.20 Step 10 Wave 4 Asset Reconstruction Self-Test`
4. `Legends of Rus > Tests > Run v1.20 Step 10 Wave 4 Four-Aspect Source QA`

Expected: 7/7 candidates and 28/28 source-aspect checks. Runtime capture and Android acceptance remain a later evidence gate.
