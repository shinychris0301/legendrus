# Loading original-reference rebuild R1

The canonical Loading prefab is now revision5, based on `/home/k2/Downloads/match3final/2.loading.png`. The existing owning builder migrates the active prefab in place. Original source crest, rear-facing Ivan, standing Firebird, outer ornaments and complete parchment tip panel are independent sprites over a clean generated environment. Russian panel/title/body and ЗАГРУЗКА lettering are extracted together as requested. No full-screen reference or invisible-hotspot page is used.

Six Scenario alpha-isolation jobs and one hidden-village reconstruction cost31CU, seven accepted jobs, zero duplicates. Original RGB is retained for crest, character sprites, RU panel, status and outer frame; masks remove overlaid UI and scenery. The ornament sheet initially retained opaque scenery, corrected locally with bounded contour/metal masks. Reference parts hidden by the panel stay hidden behind the same separate panel. Original English lettering does not exist: a repaired blank parchment supports live translation. Prepared sprites were inspected over black/white/magenta. The clean environment was inspected independently.

The loading bar uses original gold track and sapphire source texture. A source navy donor replaces the fixed75% inside the track; native clipped fill and live percentage follow the existing measured HomeLoadingPreparation service. Unknown-duration animation remains indeterminate, with no numeric percent. Failure has localized status and an actual Retry button using the approved Splash blank frame; retry gating and routing are unchanged. No fabricated loading timer was added. Static Russian panels preserve original lettering while English uses embedded selected fonts; exact source font identity remains unverified.

## Verification

Unity6000.5.9f1 compiled/imported, rebuilt and read back revision5. FinalVerify04 passed9EditMode+6PlayMode=15checks, zero skips/failures. Includes correct serialized artwork, Russian/English panel switching, proportional portrait/safe-area layout, glyph fit for visible English labels, native bar pixel color/filled-versus-empty contrast, RU/EN/RU roundtrip, Splash pointer/doubletap, measured Loading/Home transition, error/retry and single Canvas/EventSystem. Nine Loading captures include explicit42% layout fixtures, error and actual measured-work evidence. Seven Splash captures remain pixel-identical to approved Splash R2. All eight browser review options at390×844 loaded without script/resource errors or horizontal overflow.

Earlier iterations retained: Verify01 exposed a stale preferredHeight assertion on invisible Russian live text and prompted smaller English text; Verify02's error-state track sample hit the relocated Retry control, so the error check now verifies the visible safe/interactable control while normal-state pixel checks remain. Native review removed a residual fixed75% fill edge. Verify03 passed15checks but retained the old plain Retry design; finalVerify04 includes the polished source frame. No APK/physical-phone/touch/hardwareBack/full-suite/profiling/backend testing.

## Scope and remaining differences

Seven existing files changed: active Loading prefab; sole owning builder; Loading runtime/view/progress graphic; Loading PlayMode test; Splash cross-page binding test. Ten new PNGs and metas plus folder meta. No deletions. Actual changed paths in QA/ScopeChanges.json. Splash production prefab/code/assets, Home/otherpages, startup/router/services, packages/settings and Android guards remain unchanged.

The hidden village is reconstructed, so buildings/clouds/lighting are not exact source pixels. Fine hair/feather/frame alpha contours remain approximate. English parchment repairs are smoother than original paper; translated fonts are approximate. Native progress differs from the painted reference bar and reports real work. Tall displays proportionally fit the original composition, extending scenery rather than stretching the artwork. This is not a claim of pixel-identical reconstruction.

Next: review Loading; then Home is the next screen in the user's approved sequence.
