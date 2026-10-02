# Splash original-reference rebuild R1

Implemented in the canonical native SplashTitle prefab at revision8 using the user-selected `1.splash.png`. Original crest, joint Ivan/Firebird, Start frame and outer gold frame are separate sprite resources. The clean environment is a Scenario reconstruction, not a reference screenshot. Source proportions are preserved inside the safe stage; the environment fills taller screens. There is one visible native Start control. The four former Splash auxiliary controls remain serialized but hidden to match the chosen original; their destinations remain available elsewhere. There is no resource header or Masha in this selected composition.

## Artwork and typography

Four Scenario source-alpha jobs plus one hidden-environment generation cost25CU during preparation; no additional requests during integration. Five sprites retain the original visible artwork RGB outside documented button/version text repair masks. Corner isolation needed local contour cleanup. Original source assets and GUIDs remain preserved. Black/white/magenta alpha composites and original-color checks are retained in QA.

Russian Start reads «НАЖМИТЕ, ЧТОБЫ НАЧАТЬ»; English reads “TAP TO START”. Both remain live text. Existing embedded selected title/numeral fonts are bound only to Splash; shared Home typography was not changed. The final Start size31 fits inside the source ornament (first native inspection caught overflow at36). Version uses Application.version and currently displays v0.0.0. Exact reference typeface identity remains unknown.

## Verification

Unity6000.5.9f1 imported/compiled the changes, rebuilt and read back the active prefab at revision8. Baseline02 passed8EditMode+2PlayMode. Final Verify02 passed9EditMode+6PlayMode=15, no failures or skips. Coverage includes serialized bindings/source sprite identity, unchanged Home branding, proportional portrait/safe-area layout, RU/EN/RU switching, one Canvas/EventSystem, actual GraphicRaycaster Start click, duplicate-tap rejection, measured Loading success/error/retry and Home transition. Seven final Splash captures and unchanged Loading captures are retained. Russian reference-size and tall-safe-area captures were visually inspected; all eight browser review options at390×844 loaded without script/resource errors or horizontal overflow.

Verify01 passed9EditMode and5/6PlayMode. Its screenshot equality assertion detected nine one-byte shader rounding differences in live lettering after atlas repacking. Final check permits at most16 such pixels with maximum per-channel difference1; geometric assertions remain strict. The earlier project-lock attempts did not execute tests and are retained as blocked evidence. No APK, physical phone/touch/hardware-Back, full-suite, profiling or backend tests.

## Scope

See QA/ScopeChanges.json: eight existing files changed (owning builder, view, foreground layout, canonical prefab, layout JSON and three test files), five new PNGs with five metas and one folder meta, zero deletions. Other screens, settings/packages, startup/router/services, existing fonts/materials/textures and Android guards are unchanged against the pre-task snapshot. Loading was regression-tested but not redesigned. See QA/SourceChecks.json for final asset/candidate checks.

## Remaining visual differences

The generated environment has different clouds, building details and lighting from the original. Source alpha extraction can differ at fine hair, feather and corner edges; the frame's inner corner contours merit close review. The Start face was repaired where baked text was removed and is not an exact recovery of hidden texture. Live lettering and current application version intentionally replace painted text. Tall displays preserve the original aspect ratio with additional scenery above/below rather than stretching the artwork. This is a close source-based native implementation, not a claim of pixel-identical reconstruction.

Next: review this Splash page, then proceed to Loading as the second screen in the approved sequence.
