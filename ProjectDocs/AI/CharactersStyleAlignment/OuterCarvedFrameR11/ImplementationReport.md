# Characters R11 — Outer Carved Frame

Implemented the visible original carved ornaments around the title and lower collection corners, plus narrow side trims. Six independent sprites sit behind the existing native content and never receive input. The active Characters prefab, runtime, view, owning editor builder and cutover all use revision 11.

## Artwork and Scenario

The original reference is `../R1/Sources/OriginalReference.png`; bounds and hash are recorded in `Sources/Assessment.json`. Four ornament crops went through Scenario Pixa background removal. The first upper-left alpha selected the avatar decoration instead of the carving, so it was rejected. One corrected input excluded overlapping controls and was submitted once. **Five accepted jobs cost 15 CU**; one rejected mask, one corrected-input job, no duplicate submissions and no RGB generation. Two narrow, clean opaque UI trims retain exact original pixels directly.

Final sprites retain original RGB. Local alpha masks exclude avatar, title, tabs, card and Back fragments; disconnected specks were removed. Intentionally opaque carved surfaces remain opaque. The first Unity capture exposed oversized title exclusions; final masks follow the accepted title sprite's actual alpha, restoring carved backing at the seam. The original title lettering is outside these ornament bounds. Original inputs, job outputs, exclusion/seam masks, billing and final hashes are retained in Sources, AssetProvenance.json and QA/ScenarioAudit.json.

## Preserved behavior and scope

R10 panel geometry, R9 live header/title, R8 footer and R7 main hero scene remain. Scrolling, filtering, localization, real saved balances, preview labels, unavailable hero actions and existing navigation retain their behavior. Only the current Ivan card has the existing highlighted frame.

QA/ScopeAudit.json lists six changed existing production/test files, six new PNGs and six new metas, with no deletions. Existing artwork and GUIDs, other pages including Gameplay and Splash, packages, project settings and Android guards match the session-start snapshot. Documentation and review artifacts are additional changes.

## Verification performed

- Unity 6000.5.9f1 imported/compiled, rebuilt and read back the native prefab and revision-11 cutover successfully.
- Final Verify02: **2 EditMode and 6 PlayMode tests passed**, no failures or skips. Coverage includes builder idempotence, complete native bindings, noninteractive frame ordering, current-hero highlight, locked-card scrolling, modal input blocking, localization/simulated safe areas, real navigation routes, preview ownership safety and callback cleanup.
- Verify01 also passed eight tests; visual title-seam findings prompted the asset correction and final Verify02 rerun. Both evidence sets are retained.
- Nine final actual Unity captures were produced. RU 941×1672, narrow safe-area EN and scrolled safe-area EN were visually inspected. Sprite alpha was inspected over black, white and magenta; opaque side trims were inspected separately.
- Original/input/request/final hashes, original RGB preservation, three source Python syntax parses and scoped whitespace checks passed.

Browser review results are recorded in QA/BrowserCheck.json: eleven image choices, including R10 and the original comparison, are checked at 390×844 for image loading, missing links, browser errors and horizontal overflow.

## Not performed and remaining differences

No APK build, physical-phone/touch/hardware-Back test, full project suite, profiling, backend or hero-service testing. Simulated captures do not replace physical-device checks. Exact original typefaces remain unidentified; header portrait/art, alternative hero resolution and collection scenery still differ. Hidden scenery and the earlier footer backing remain reconstructed; this revision preserves visible original carving and does not recover hidden artwork. Hero ownership, progression and upgrades remain previews/unavailable.

Next: user review of this R11 portrait. A later focused refinement can address collection-card scenery and portrait consistency; it is not included in R11 or presumed approved.
