# v1.20 Step 3.3R.1 — Core Loop Reference Audit and Fidelity Gap Matrix

This checkpoint is an audit/correction-plan checkpoint. It does **not** change production runtime routing, gameplay, approved reference PNGs, canonical UI-kit PNGs, Packages, ProjectSettings, or Chapter 1 data.

## Audit result

- P0 core-loop surfaces audited: **14**
- Approved references in `Art/ReferenceOnly/ApprovedScreens`: **8**
- Provisional captured references still only in `Resources/DesignScreens`: **4**
- Missing approved visual references: **2**
- Step 3.3 production routes already cut over: **10**
- Reference-match sign-off: **NOT GRANTED**
- Screenshot deletion: **NOT AUTHORIZED**

## Why Step 3.3 looked different

The Step 3.3 builders reproduced hierarchy and interaction using generic kit sprites and gradients, but the approved screens derive most of their identity from painterly environment/hero compositions, exact page-specific ornaments, illustrated navigation/booster/reward art, and precise typography/layout. Therefore prefab existence and screenshot-free routing were incorrectly treated as sufficient for visual completion. Step 3.3R corrects that process.

## Page-by-page correction plan

### Splash / Title — BLOCKED_REFERENCE_MISSING
- **Reference:** missing — none designated
- **Current Step 3.3 state:** Not implemented by Step 3.3; Step 3.1 lists a target prefab only.
- **Reconstruction batch:** 3.3R.5
- **Required visual layers:** approved splash environment/brand plate; ornate logo/title lockup; entry CTA treatment; language entry control; corner/edge filigree
- **Composition corrections:** Define whether Splash and Title are one timed state or separate pages. Freeze exact title/logo scale and vertical placement after an approved reference exists.
- **Runtime bindings to preserve:** language selection entry; boot/loading transition

### Loading — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Loading.png`
- **Current Step 3.3 state:** Gradient background + generic header frame, title text, sapphire jewel, simple progress bar.
- **Reconstruction batch:** 3.3R.5
- **Required visual layers:** painterly sunset village environment plate; Ivan rear 3/4 character layer; Firebird full-body layer; ornate brand/logo frame; outer gold/sapphire border/corner filigree; parchment tip panel; ornate loading progress frame/fill jewels
- **Composition corrections:** Restore logo to upper quarter and hero/environment composition across center. Place tip parchment above loading label and ornate progress near footer. Remove generic center header/jewel layout.
- **Runtime bindings to preserve:** loading progress percent; tip/localized copy; automatic Home transition

### Home — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png`
- **Current Step 3.3 state:** Gradient backdrop + generic title/header + navy hero panel + four generic nav tiles + quest panel.
- **Reconstruction batch:** 3.3R.2
- **Required visual layers:** twilight Russian town environment plate; Ivan hero character layer; Firebird hero layer; exact ornate Legends of Rus logo lockup; portrait medallion + heraldic badge; full lacquer/gold top HUD frame; coin/gem/energy cells + green add buttons; settings medallion; four illustrated nav tile artworks; large emerald Play frame; mail medallion; reward chest medallion; bottom carved quest panel; blue gem reward icon
- **Composition corrections:** Hero art must dominate middle 55-60% of page instead of a generic navy panel. Logo overlays the upper environment below top HUD. Play button spans the hero-to-navigation transition. Four illustrated nav tiles sit in a single framed row. Bottom quest/mail/chest band matches reference proportions.
- **Runtime bindings to preserve:** player name/level/progress; coin/gem/energy values; mail/reward badges; quest text/progress/reward; Home navigation hotspots

### World Map — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/WorldMap.png`
- **Current Step 3.3 state:** Gradient + translucent navy map panel + generic map nodes + generic nav tiles.
- **Reconstruction batch:** 3.3R.2
- **Required visual layers:** full painterly world-map valley/river/castle environment plate; painted route/path light dots; 15 ornate numbered node medallions; per-node star rows; top HUD/profile frame; ornate world-map title plaque; bottom three circular navigation medallions; map/tasks/guild icon artworks; bottom filigree foundation
- **Composition corrections:** Remove opaque MapField panel; nodes must sit directly on painted terrain. Route must visually follow the river/road through the valley. Match 1→15 node positions and scale from reference. Bottom nav uses large circular medallions, not small generic tiles.
- **Runtime bindings to preserve:** player resources; node unlocked/completed state; node star counts; chapter selection/navigation

### Level Select / Chapter Map — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/ChapterMap.png`
- **Current Step 3.3 state:** Gradient + red generic button used as chapter header + translucent navy path panel + ten generic nodes.
- **Reconstruction batch:** 3.3R.2
- **Required visual layers:** Chapter 1 rural village environment plate; large red fabric chapter ribbon with gold filigree and sapphire centerpieces; chapter title typography treatment; stone/road path integrated into environment; ornate level node medallions + stars; top HUD/profile frame; large bottom emerald Play frame; round blue Back medallion; bottom carved filigree
- **Composition corrections:** Remove translucent PathPanel; level path must be embedded in scenery. Match visible reference node sequence/positions (10–16 in the captured composition). Chapter ribbon occupies upper scenic area below top HUD. Play and Back controls align to reference footer.
- **Runtime bindings to preserve:** selected/current level; star counts; locked/unlocked states; player resources; Pre-Level navigation

### Pre-Level — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/PreLevel.png`
- **Current Step 3.3 state:** Generic header + parchment goals + small moves jewel + generic booster panel/reward strip.
- **Reconstruction batch:** 3.3R.3
- **Required visual layers:** Chapter 1 village environment plate; red chapter ribbon; ornate navy Goals frame; large red/blue gem goal icons; large central moves medallion; ornate booster section frame; four illustrated booster medallions + quantity badges; reward frame; ornate reward chest illustration; coin/gem reward icons; large emerald Play button; round Back medallion; top HUD/profile frame
- **Composition corrections:** Use dark navy/gold goal and booster frames rather than parchment blocks. Moves medallion bridges goals and booster sections. Four boosters are large illustrated circles in one row. Reward box occupies lower third above Play.
- **Runtime bindings to preserve:** level goals/counts; moves; booster inventory/selection; reward values; resource HUD; start/back actions

### Gameplay — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Gameplay.png`
- **Current Step 3.3 state:** Gradient shell + generic goal frame, board frame, booster slots and score panel; functional Match3Board overlays it.
- **Reconstruction batch:** 3.3R.3
- **Required visual layers:** twilight village gameplay environment plate; profile medallion/header frame; star/progress meter; large moves medallion; goals frame with exact gem icons/counts; ornate board outer frame + cell lattice; approved gameplay gem/special-gem visual set; booster medallions + quantity badges; pause/settings medallion; ornate score footer; match/special-gem glow/VFX overlays
- **Composition corrections:** Board must occupy the same central footprint as reference with environment visible around it. Top HUD hierarchy: profile left, star meter center, goals right, moves medallion centered beneath/within header. Booster bar uses large illustrated circles under board. Score footer is integrated into bottom ornamental frame.
- **Runtime bindings to preserve:** live Match3Board cells; moves/score/goals; booster availability/actions; tutorial presenter; pause action; existing swipe/match/cascade/VFX callbacks

### Tutorial Overlay — CRITICAL_REFERENCE_MISSING
- **Reference:** missing — none designated
- **Current Step 3.3 state:** Runtime-generated flat dark-blue rectangle with Unity Outline and a plain red button.
- **Reconstruction batch:** 3.3R.5
- **Required visual layers:** ornate tutorial panel frame; sapphire/filigree title ornament; pointer/hand indicator; spotlight/highlight mask treatment; primary/secondary tutorial button state; mechanic icon slot
- **Composition corrections:** Overlay must spotlight the relevant gameplay area instead of only dimming the entire screen. Panel placement must adapt so it never covers the target interaction.
- **Runtime bindings to preserve:** authored tutorial title/body/mechanic; advance/complete action; persisted tutorial completion; analytics/review hooks

### Pause — CRITICAL
- **Reference:** provisional_capture — `Assets/LegendsOfRus/Resources/DesignScreens/Pause.png`
- **Current Step 3.3 state:** Generic modal on a gradient page rather than an ornate overlay over the live gameplay page.
- **Reconstruction batch:** 3.3R.3
- **Required visual layers:** live Gameplay underlay + dim blocker; large navy/gold pause modal frame; ornate PAUSE header plaque; goal summary frame with gem icons/counts; booster row with four illustrated icons + quantities; emerald Continue button; blue Restart button; blue Settings button; red Exit button
- **Composition corrections:** Pause must overlay the live board/HUD, preserving the frozen gameplay composition beneath. Modal scale/vertical placement must match captured reference. Goal and booster sections are part of modal, not omitted.
- **Runtime bindings to preserve:** current level goals/progress; booster inventory; resume/restart/settings/exit actions; pause state continuity

### Victory — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Victory.png`
- **Current Step 3.3 state:** Gradient + generic header, three stars, parchment rewards, two buttons.
- **Reconstruction batch:** 3.3R.4
- **Required visual layers:** celebration village environment plate; flying Firebird hero layer; confetti/particle layer; top currency/energy bar; large red fabric Victory ribbon; three luminous star assets + burst glow; wood/dark reward panel; coin stack reward art; blue gem reward art; ornate egg/relic reward art; emerald Continue button; gold rewarded x2 button; bottom heraldic crest/filigree
- **Composition corrections:** Hero Firebird occupies upper half; result ribbon overlaps center. Stars sit immediately under ribbon with luminous burst. Reward row contains three distinct illustrated tiles. Continue and rewarded x2 controls match reference hierarchy.
- **Runtime bindings to preserve:** reward amounts; star count; currency/energy HUD; continue action; rewarded x2 availability/result

### Fail — CRITICAL
- **Reference:** approved — `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Fail.png`
- **Current Step 3.3 state:** Red/navy gradient + generic header + parchment advice + retry/map buttons.
- **Reconstruction batch:** 3.3R.4
- **Required visual layers:** burning village environment plate; Ivan failure hero layer; top HUD/profile frame; ornate navy defeat title plaque + sapphire crown ornament; incomplete-goals panel with three goal icons/counts; emerald Retry button; +5 moves purchase panel; ad-continue panel; recommended boosters section; four booster medallions + labels/quantities; bottom frame/filigree
- **Composition corrections:** Failure hero composition fills upper-middle page. Replace generic AdvicePanel with actual incomplete-goals summary. Include both +5-moves currency offer and ad-continue alternative. Include recommended booster row.
- **Runtime bindings to preserve:** failed goal progress; resource HUD; retry; exit/map; extra-moves cost/availability; rewarded continue availability; recommended boosters

### Chapter Complete — CRITICAL
- **Reference:** provisional_capture — `Assets/LegendsOfRus/Resources/DesignScreens/ChapterComplete.png`
- **Current Step 3.3 state:** Gradient + generic chapter header, jewel, stars, navy summary and two buttons.
- **Reconstruction batch:** 3.3R.4
- **Required visual layers:** sunset village environment plate; Ivan hero layer; Firebird hero layer; ornate Chapter Complete title plaque; red chapter ribbon; three-star + heraldic crest victory composition; reward row with coin/gem/energy/chest; emerald Next button; blue Map button; Next Chapter parchment card; Chapter 2 forest thumbnail/plate; outer wood/gold/sapphire frame
- **Composition corrections:** Hero scene occupies center; stars/crest overlap lower hero area. Rewards and dual buttons use carved dark-wood section. Next Chapter parchment card occupies footer with image at right.
- **Runtime bindings to preserve:** chapter completion state; reward values; next chapter metadata/title/copy; next/map actions

### Out of Energy — UNRECONSTRUCTED_P0
- **Reference:** provisional_capture — `Assets/LegendsOfRus/Resources/DesignScreens/OutOfEnergy.png`
- **Current Step 3.3 state:** Still screenshot-backed modal; not reconstructed in Step 3.3.
- **Reconstruction batch:** 3.3R.5
- **Required visual layers:** live underlying page + dim blocker; large navy/gold modal; energy medallion with zero badge; countdown panel; buy-energy row + green price button; ask-friends row + blue action; watch-ad row + blue action; close medallion
- **Composition corrections:** Must be a true modal over the originating page, not a full-screen capture. Preserve the dimmed chapter/home composition underneath.
- **Runtime bindings to preserve:** energy balance; refill countdown; purchase availability; friends action availability; rewarded-ad availability; close/resume flow

### Booster Select Popup — UNRECONSTRUCTED_P0
- **Reference:** provisional_capture — `Assets/LegendsOfRus/Resources/DesignScreens/BoosterSelect.png`
- **Current Step 3.3 state:** Still screenshot-backed modal; not reconstructed in Step 3.3.
- **Reconstruction batch:** 3.3R.5
- **Required visual layers:** live gameplay/pre-level underlay + dim blocker; large ornate navy modal frame; title ornament + close medallion; 8 booster medallions/icons; quantity badges; selection toggles; booster names/descriptions; large emerald Play/Confirm button
- **Composition corrections:** Use 4x2 booster grid matching capture. Modal must preserve live screen behind it and use scrolling only if localization requires.
- **Runtime bindings to preserve:** booster catalog/inventory; selection toggles; descriptions/localization; confirm/close action

## Fidelity sign-off gate

- No Resources/DesignScreens dependency in the production visual tree.
- All named reference elements are present; no generic substitute panel or gradient may stand in for painterly or ornamental reference art.
- Reference-resolution QA capture at 941x1672 is reviewed with overlay comparison before sign-off.
- Key landmark bounds are aligned within 1.5% of reference width/height unless intentionally responsive.
- Cyrillic labels remain runtime text/TMP where practical; no new full-screen flattened UI is introduced.
- 16:9, 18:9, 19.5:9, and 20:9 portrait layouts preserve hierarchy and safe-area access.
- Existing navigation/gameplay/save/economy behavior remains unchanged unless separately authorized.

Pause, Chapter Complete, Out of Energy, and Booster Select currently have only provisional runtime captures. Splash/Title and Tutorial Overlay have no approved visual reference in the current project. These six surfaces cannot receive final reference-match approval until their target references are explicitly approved/designated.

## Execution order

1. **Step 3.3R.2:** Home, World Map, Chapter Map — reconstruct clean environment/hero/page-specific art, rebuild prefabs, compare against approved references.
2. **Step 3.3R.3:** Pre-Level, Gameplay shell, Pause — preserve live board/data while correcting presentation.
3. **Step 3.3R.4:** Victory, Fail, Chapter Complete — outcome hero art, ribbons, rewards, offer panels, next-chapter card.
4. **Step 3.3R.5:** Loading, Splash/Title, Tutorial Overlay, Out of Energy, Booster Select — finish entry/tutorial/modal P0 surfaces; obtain/confirm missing or provisional references before sign-off.

## Hard rule going forward

No page may be described as design-complete merely because a production prefab exists or because it no longer renders a screenshot. **Reference match is a separate gate and must pass before visual completion is claimed.**
