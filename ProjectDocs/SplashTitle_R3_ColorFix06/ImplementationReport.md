# SplashTitle R3 ColorFix06

Implemented user-requested changes:

- Darkened non-character Splash foreground assets by 10% (RGB multiplier 0.9) while leaving the background and the Ivan/Masha character assets unchanged.
- Increased the Settings, Account, and Help icon art size by 15% (110 -> 126.5 px in the 941x1672 reference layout).
- Reduced spacing between the bottom icon groups by approximately 10% by shifting the outer groups inward (Settings +11 px, Help -11 px; Account unchanged).
- Shifted the associated bottom text label rects with the icon groups.
- Bumped SplashTitleView.CurrentRevision from 4 to 5 so the canonical Splash prefab will be rebuilt from the updated authoritative layout JSON when the project reloads.

Changed files:
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/legends_of_rus_ornate_crest.png
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/majestic_fiery_phoenix_in_flight.png
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_emerald_fantasy_ui_button.png
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_gear_settings_emblem.png
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_navy_language_selector.png
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_question_mark_button.png
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/ornate_gold_user_medallion.png
- Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/SplashTitleLayout.json
- Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleView.cs
- Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs
