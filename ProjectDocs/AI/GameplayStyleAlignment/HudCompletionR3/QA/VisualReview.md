# Visual review record

Directly inspected the canonical Gameplay reference and the R3 composition. The corrected moves medallion centers its visible circle and live number. The header uses an opaque lacquer surface, labels remain readable, and the score plaque has distinct sapphire ornaments and a bronze well. Goal counts fit their slots, including the four-digit score target. The survival hourglass is readable, though its native geometric treatment is simpler than the raster gem artwork.

Eight final-run Unity portraits were directly viewed:

- `Gameplay_941x1672_RU.png`
- `Gameplay_941x1672_EN.png`
- `Gameplay_Status_RU.png`
- `Gameplay_Status_EN.png`
- `Gameplay_576x1280_safearea_RU.png`
- `Gameplay_Status_1080x2400_safearea_EN.png`
- `Gameplay_Level06_RU.png`
- `Gameplay_Level09_RU.png`

The Russian Living Light instruction fits two lines; English fits one. Both sit above the board. The narrow and tall views retain complete scenery coverage and safe-area-contained HUD/footer controls. Taller layouts retain the existing extra scenery between header, board and footer; this pass does not change responsive board sizing. The remaining seven final portraits were captured and checked by runtime assertions and browser loading, but were not individually assessed by direct image inspection.

Also inspected initial/corrected iteration portraits, including the original off-center moves image. The preserved initial image in UnityVerification/Iterations explains that correction. All three generated component assets were directly inspected over black, white and saturated magenta before import; nine background proofs are retained here. Opaque panel surfaces remain opaque; no painted checkerboard was accepted as transparency.

[Local Chrome results](StandaloneReviewBrowserChecks.json): all **17** portrait choices loaded, all **4** viewer links resolved, and the **360px** view had no horizontal overflow. [Browser screenshot](ReviewBrowser_360px.png) was directly inspected. These checks validate the HTML evidence viewer; they are not additional Unity or phone tests.

The reference still has finer distressing and smaller carved ornament, different portrait/scenery treatment and different activation lighting. Those differences are stated in ReferenceComparison.md. No exact visual acceptance is inferred.
