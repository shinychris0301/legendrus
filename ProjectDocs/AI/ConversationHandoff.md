# Conversation handoff — Legends of Rus
Prepared 2026-09-12. Source: the current user/assistant conversation and supplied project artifacts. This is a focused decision summary, not a verbatim transcript or an independently verified local Unity audit.

## Current user intent
Continue the existing Unity project in Codex inside VS Code without losing the accepted artwork, architecture, and page-by-page development decisions. The user requires implemented Unity resources/prefabs, not just plans, checklists, or generated promotional mockups.

## Latest accepted milestone
The user said the Splash looks very good after the ColorFix06 and PrefabFix07 sequence, then agreed to proceed to Loading. Preserve that local working baseline.
- ColorFix06 darkened seven non-human-character foreground PNGs with an additional RGB multiplier of 0.9. Ivan, Masha, and the environment were unchanged in that pass. Do not apply the darkening again.
- Settings, Account, and Help artwork increased from 110 to 126.5 reference pixels (15%). The outer groups were moved inward. The prior description called the spacing change approximately 10%; preserve accepted serialized positions rather than applying another 10%.
- PrefabFix07 repaired a script/prefab mismatch: the script required revision 5 while the old serialized prefab still held revision 4. The corrected delivery included a revision-5 canonical prefab and improved diagnostics/repair scheduling. Verify this in the actual local project instead of assuming installation.
- FidelityFix02 previously addressed text density, visible-glyph alignment, footer readability, and a localized foliage repair. Later color changes are cumulative. No new changes to Splash are requested now.

## Unity environment and architecture
Reported Unity Editor: 6000.5.9f1; check ProjectSettings/ProjectVersion.txt locally. The screenshots showed Linux/Ubuntu. Target: Android, portrait. Use the actual installed configuration rather than switching versions/platforms casually.
Reference composition: 941 x 1672. Taller portrait targets previously requested: 9:19.5 and 9:20, including safe areas. A wide Free Aspect Game view once caused a misleading landscape preview; do not resize the approved design to compensate for an Editor preview.
Expected established route (must trace actual source):
Bootstrap -> RuntimeBootstrap -> ScreenRouter -> Splash/Title -> Start -> Loading -> Home.
Auxiliary Splash actions use the existing Language Select, Settings, Login/account, and Tutorial Overlay/help routes.

## Important paths from delivered artifacts
These are known project paths to locate and verify, not instructions to recreate missing files under guessed names.
- Assets/Scenes/Bootstrap.unity
- Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/SplashTitleLayout.json
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/SplashTitleTypography.asset
- Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSplashTitleRuntime.cs
- Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleView.cs
- Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleTypography.cs
- Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleResponsiveLayout.cs
- Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs
- Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMetaPageRuntime.cs
- Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs
- Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Loading.png
Loading.prefab variants existed under Resources/ProductionUI/Pages/ and Wave01Pages/. Inspect the runtime path to determine the owner; do not assume the existence of a prefab means it is active.

## Canonical Splash source filenames
All under Resources/ProductionUI/SplashTitleReconstructed/ relative to Assets/LegendsOfRus/:
- golden_fortress_at_winter_sunrise.png (environment)
- legends_of_rus_ornate_crest.png (logo)
- majestic_fiery_phoenix_in_flight.png (Firebird)
- slavic_fantasy_prince_warrior_portrait.png (Ivan)
- regal_slavic_princess_in_traditional_finery.png (Masha)
- ornate_emerald_fantasy_ui_button.png (Start base)
- ornate_gold_navy_language_selector.png
- ornate_gold_gear_settings_emblem.png
- ornate_gold_user_medallion.png
- ornate_gold_question_mark_button.png
Some supplied controls combine frame and icon. Do not claim they are separately layered when they are not. Old SplashTitleFinal crops were rejected and must not become the runtime fallback.

## Current page: Visual Page 02 — Loading Style Alignment
The old Loading reference was displayed from ApprovedScreens/Loading.png. The user then flagged a style mismatch with the accepted Splash. The assistant agreed and proposed aligning Loading BEFORE treating it as the final implementation target. The user authorized starting that alignment.
No completed, user-approved replacement Loading reference or verified Loading Unity implementation is established by the subsequent visible conversation. An empty response is not evidence of a delivered design. Search the local workspace for newer artifacts, then verify their provenance/approval before using them.
The old Loading concept can inform the journey scene, but it is not the final approved style target. Specifically:
- Reuse the actual mixed-case Splash logo with its navy plaque, gold lettering/filigree, sapphire jewels, and castle silhouette; do not introduce the alternate uppercase Loading logo.
- Remove or substantially simplify the enclosing ornate screen border and large corner jewels.
- Keep an open full-screen painted environment, controlled warm/cool balance, and the toned-down materials of the accepted Splash.
- A journey scene or different character pose is permitted; clothing, face/material treatment, and Firebird rendering must retain continuity.
- Parchment tips and a sapphire progress fill can fit the shared style; match materials and ornament weight rather than copying the old frame wholesale.
- Share the existing typography configuration rather than introducing an unrelated font.

## Unresolved items — do not convert them into false completion
Exact source font: no definitive typeface identity was established. Live labels had development fallbacks; the logo lettering is preserved in PNG artwork. The local project may contain newer user assignments. Inspect them, preserve them, and never label a lookalike as the exact original without evidence.
Android: earlier logs involved content builds during Play Mode, a non-development request, AutoRunPlayer, and over-restrictive custom option guards. BuildOptionsFix04 introduced a dedicated Development APK (No Run) flow allowing Development plus LZ4/LZ4HC, StrictMode, and ShowBuiltPlayer. Independent package-approval/fingerprint checks remained unresolved in the delivered source, including ResolvedPackageLockApproved=false. Do not flip approval Booleans or delete guards to hide the issue; inspect current diagnostics and real package files when build work is explicitly requested.
Prior archive did not contain all local package-resolution/Player settings, which Unity may have generated locally. Do not replace the user's current Packages/ or ProjectSettings/ with an older archive.
Earlier assistant deliveries reported static/offline checks but not execution in Unity. User screenshots and confirmations establish user-observed rendering, not an Android APK pass or every navigation test.

## Evidence precedence
1. Latest explicit user approvals and current local project/resources.
2. Verified current Unity screenshots and logs.
3. Supplied artifacts with exact provenance.
4. This historical handoff and prior delivery descriptions.
The folder name ApprovedScreens alone is not an approval decision for the new Loading style. Generated showcase images with claims such as "licensed font" or "ready for testing" are not verification evidence.

## Next action
Read CurrentTask.md, inspect the local project, and report actual Loading ownership, usable resources, missing resources, and any conflict with this handoff. Preserve Splash. Prepare the aligned Loading reference for review before declaring visual acceptance or advancing to Home.
