# Home badge-free icons R2

The gift icon exposed a doubled, diagonally broken gold rim when its daily badge was hidden. The Event exclamation was baked into both language sprites, so disabling its native badge object could not hide it.

## Changes

- Repaired only the badge-covered upper-right gift rim and Event corner, using reflected original ornament from each panel's opposite side. Removed a detached gift alpha sliver. Existing illustration and lettering remain unchanged elsewhere.
- Extracted the original Event exclamation into a separate 53×53 sprite, attached to the existing native EventsBadge at its original source position. Russian and English now use badge-free base cards.
- Gift availability still follows the real saved daily reward. Event remains the existing static reference decoration by default; hiding its native object now works and survives language refresh. No event availability service was invented.
- Updated the owning source composition/resource list and canonical Home prefab to revision17. All433 prior prefab records and all12 native action controls preserved; four records added for the separate Event image. Existing GUIDs preserved.

## Verification

Unity6000.5.9f1 import, compilation, Home-only rebuild and serialized read-back succeeded. **14EditMode +18PlayMode checks passed**, zero failures/skips. Expanded the actual daily-claim test to render both badges hidden in Russian, English and a tall safe-area layout; language refresh preserves the explicitly hidden Event state. Existing tests cover all12 control raycasts/routes, localization, real daily availability, measured resources, safe layouts and Splash/Loading transitions.

Inspected native Russian badge-free/default portraits and tall English badge-free capture, plus isolated sprites against black, white and magenta. Review HTML points to actual native captures; image paths validated locally. Ten Home captures retained. No browser automation, APK or physical-device test.

## Scope and provenance

Seven existing production/test files changed, one PNG and its meta added, zero files deleted. [Scope audit](QA/ScopeAudit.json) confirms no other Assets files changed, including Splash, Loading and Gameplay. [Repair recipe](Sources/Repair.py), source snapshots and [repair descriptions](Sources/AssetRepairs.json) retained. Existing Scenario extraction reused; **zero new Scenario calls or credits**. Small hidden rim/corner details are reconstructed from original donors, not pixel-exact recovery of obscured artwork. Default screenshot values and claimed-state values differ because the test actually grants the daily reward in isolated storage.
