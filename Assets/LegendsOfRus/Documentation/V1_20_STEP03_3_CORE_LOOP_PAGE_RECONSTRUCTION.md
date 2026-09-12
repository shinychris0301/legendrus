# v1.20 Step 3.3 — Core Loop Page Reconstruction

This checkpoint performs the first actual runtime screenshot cutover. Ten core-loop routes now prefer screenshot-free production prefabs reconstructed from the Step 3.2 canonical UI kit. The prefabs are built deterministically by Unity into `Assets/LegendsOfRus/Resources/ProductionUI/Pages`.

Cutover routes: Loading, Home, WorldMap, ChapterMap, PreLevel, Gameplay, Pause, Victory, Fail, ChapterComplete.

The 22 `Resources/DesignScreens` PNGs are retained byte-identically for QA comparison and for later non-core waves, but the ten cutover routes are no longer allowed to use them at runtime. Missing production prefabs use a screenshot-free emergency surface.

Run in Unity 6000.5.9f1:

1. `Legends of Rus > Production UI > Build v1.20 Step 3.3 Core Loop Pages`
2. `Legends of Rus > Production UI > Validate v1.20 Step 3.3 Core Loop Pages`
3. `Legends of Rus > Tests > Run v1.20 Step 3.3 Self-Tests`
4. Run the complete EditMode and PlayMode suites.
5. Exercise Loading -> Home -> ChapterMap -> PreLevel -> Gameplay -> Pause -> Victory/Fail -> ChapterComplete on at least 16:9, 19.5:9, and 20:9 portrait device shapes.

No gameplay logic, save schema, chapter data, reward rules, economy balance, monetization fulfillment, Packages, or ProjectSettings are changed by this visual cutover.

Next checkpoint: v1.20 Step 3.4 — Meta/LiveOps Page Reconstruction.
