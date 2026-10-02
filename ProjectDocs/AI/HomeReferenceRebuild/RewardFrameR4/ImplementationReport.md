# Home reward frame R4

The smaller plaque under the gemstone had a separate artwork defect from the progress-track cutoff repaired in R3. R1's sample-reward removal rectangle extended through the plaque's lower gold rail. Its inpainting also pulled cyan from the gemstone into the blank amount area.

Restored the lower plaque directly from the original reference. Cleared only the painted amount using a clean brown texture donor from the same plaque's side face, keeping the repair above its lower gold border. The live reward label remains native and displays a dash while quest reward data is unavailable.

Only the existing RU/EN quest PNGs changed, within panel coordinates `(434,118)–(549,168)`. Their dimensions and alpha are unchanged. The gemstone, prior progress-track fix, all other pixels, GUIDs, native layout and revision17 prefab/configuration remain unchanged. Existing references automatically use the repaired textures; no prefab geometry or code modification was needed.

## Verification

Unity6000.5.9f1 import/compilation and **6EditMode +12PlayMode tests passed**, with zero failures/skips. Checks cover prefab/resource agreement, controls, actual daily-reward badge state, localization, and safe layouts. Inspected the actual native enlarged Russian panel and tall English capture, plus asset transparency over black/white/magenta. Pixel audit confirms the lower gold border matches original RGB, the gemstone and previous progress repair are preserved, and all pixels outside the repair rectangle are unchanged.

[Scope and pixel audit](QA/ScopeAndPixelAudit.json) · [Test summary](QA/TestSummary.json) · [Repair recipe](Sources/Repair.py) · [Provenance](Sources/RepairProvenance.json).

Existing Scenario-extracted artwork reused; **zero new Scenario calls or credits**. The empty amount surface is a local reconstruction using original texture; the restored border uses original pixels. No browser automation, full-project suite, APK or physical-device check.
