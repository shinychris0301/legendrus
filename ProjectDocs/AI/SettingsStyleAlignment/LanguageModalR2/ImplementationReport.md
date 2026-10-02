# Settings — Language Modal R2

The language row now opens the supplied navy-and-gold modal over the existing Settings page. Its original Russian heading, sapphire crest, button borders and red Close icon are individual sprites. RU/EN controls use live labels, and the selected language has the blue face. The shield dims the underlying page and blocks its controls.

Selecting a language refreshes the existing session state while keeping the modal open. Close or Back dismisses it; subsequent Back returns to the originating page or Pause. Existing preferences, navigation and paused gameplay remain intact. Language persistence across application restarts remains deferred.

## Source and implementation

Source: `setting-language select.png`, 941×1672, copied unchanged to Sources/OriginalReference.png. Four accepted Scenario `model_pixa-background-removal` jobs cost **12CU** in total; no new RGB generation. Five runtime sprites include a blank-heading derivative for English. Immutable submitted inputs, repair masks, Scenario records and output hashes are retained in Sources and AssetProvenance.json.

Source colors are preserved outside bounded repairs. Local cleanup removes occupied button text and panel controls, then restores deliberately opaque panel interiors. Assets are scaled 2× for import; this does not create additional detail. Initial lettering shadows and a transparent top strip were found in verification and corrected before the final run.

Settings runtime/view/builder, active prefab and cutover state are revision2. The builder upgrades the existing prefab in place: all **351 prior object IDs** and all existing meta files remain intact. Six existing Assets files changed; five PNGs and five metas added. Exact paths are in QA/ScopeAudit.json. No other production assets, packages or ProjectSettings changed from this task's starting snapshot.

## Verification performed

- Unity6000.5.9f1 import, compilation, prefab rebuild and serialized readback.
- Final Verify03: **6 Edit Mode + 14 Play Mode tests passed**, zero failures/skips, across Settings, Inventory and Pause. Earlier two runs retained; these are repetitions, not additional distinct tests.
- Preference persistence, language state/highlight switching, Close/Back behavior, origin routes, repeated binding and preserved paused board/moves/score covered by the targeted fixtures.
- **14 actual Settings captures** plus **20 Inventory/Pause captures**, including RU/EN, 941×1672, 576×1280 and 1080×2400 simulated safe areas. Final RU modal, tall EN modal and Pause-origin modal visually inspected.
- Scenario source/input/runtime hash audit, Python syntax, scoped source whitespace, existing GUID/meta preservation and black/white/magenta alpha review.
- Four closed-page RU/EN/all-off/mixed captures are pixel-identical to Settings R1 (QA/ClosedPageComparison.json). Browser at390px loaded all15 views without errors, missing links or horizontal overflow (QA/BrowserReview.json).

Not performed: APK build, physical-phone testing, full project test suite, performance profiling, durable language storage, live account/cloud integrations or calibrated perceptual reference scoring.

## Remaining reference differences

The supplied image's underlying Settings page is a different composition. This task applies its language modal to the existing R1 Settings page; it does not replace that page. Live language labels and English heading use the established embedded fonts; they differ from the reference's narrow embossed lettering. Exact source font remains unknown. Repaired text-free face regions have less texture than the source. Modal boundaries, selected state, crest and red X follow the reference, but pixel-perfect equivalence is not claimed.

Next: review the new modal at phone scale in Russian and English.
