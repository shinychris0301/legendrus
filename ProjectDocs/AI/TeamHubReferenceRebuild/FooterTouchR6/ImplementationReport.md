# Team Hub — Footer Touch R6

The guild-information icon was a 48×48 reference-pixel control, about 29×29 screen pixels in the narrow-phone capture. Its native raycast area now extends 14 reference pixels in each direction, producing a 76×76 target (about 47×47 screen pixels in that capture). The source icon, caption, motto, crest and layout are unchanged. These are simulated screen-pixel measurements, not Android dp certification.

The expanded target clears Friends and remains inside the safe viewport. Taps outside the visible icon open the existing information panel; there is no additional overlay or route handler. Preview guild state and unavailable actions remain unchanged.

## Production

Reference-layout owner, view, builder, active TeamHub prefab and cutover agree at revision 7. No textures, fonts, colors or sprite metadata changed. The production prefab change is limited to information-icon raycast padding and revision. Exact paths and hashes are recorded in QA/ScopeAndVerification.json.

## Validation

Checks cover the expanded icon's edge raycasts, safe-area placement and separation from Friends. A new Russian/English test opens the information panel through a native click outside the visible icon, checks its full-height input shield, changes language while open, closes through the native button handler, and opens Friends while verifying one route and unchanged saved state. Existing navigation, localization, text fit, sample progress, font and repeat-build checks also run.

Verify01 revealed a test assumption: the established Friends button uses a sliced panel, unlike the simple icon sprites. The new whole-icon assertion was restricted to the actual icon/crest/motto sprites. Friends artwork was not changed. Final verification is Verify02, with isolated saves in Unity 6000.5.9f1. Native captures and test XML/logs are retained in this review folder.

Final Verify02 passed all 11 Unity tests (6 EditMode + 5 PlayMode), with zero failures or skips. Rebuild/readback passed; 17 native captures were retained. All 11 browser review states and the native-pixel viewing toggle passed, without document overflow or browser errors.

Six matching R5 captures are pixel-identical. The two remaining comparisons differ by only four and two text pixels, respectively, with a maximum channel delta of one. There is no visible artwork/layout change. Scope: six existing Assets files changed, 7,190 unchanged, none added or deleted.

No image generation or Scenario calls. No Android build, physical-device, backend, performance or full-project suite. Existing source-font uncertainty, reconstructed surfaces and tall-page scenery spacing remain. Next recommended page: Team Ranking responsive/footer audit.

Source/documentation whitespace checks passed. Including the Unity-serialized prefab reports trailing spaces on empty `m_Name` fields; left the native serializer output intact.
