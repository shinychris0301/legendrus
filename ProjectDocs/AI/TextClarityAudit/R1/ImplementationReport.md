# Text Clarity Audit R1

Completed 2026-09-26. User reported hazy text in both HTML reviews and Unity across many pages. Fixed text refresh at Canvas scale changes and the synchronous review capture path. Retained the existing fonts, font sizes, wording and all artwork.

[Before/after portrait review](Text_Clarity_R1_Review.html) · [Scope and test results](QA/ScopeAndVerification.json) · [Browser checks](QA/BrowserChecks.json)

## Findings

The new Core Journey and Persistence/Lifecycle capture helper switched Canvas scale and render camera synchronously without refreshing fixed-size Text geometry. Some labels retained glyph meshes rasterized for the smaller Editor view. The earlier Settings capture fixture already refreshed text twice after layout; it produces much sharper lettering with the same font assets. Comparing the old and regenerated English Settings capture confirms this visible difference in the heading, rows and version label.

The saved Unity Game view layout inspected before verification reported a render target of 371×660 with 1× zoom. After the user closed Unity, the saved layout reported 304×541. These are saved Editor settings, not a measurement of every live frame or a phone capture. An aspect-ratio preset can therefore render the entire interface at a very small resolution. Enlarging that image later cannot recover fine glyph detail.

Unity UI Text uses Canvas scale to select dynamic glyph density. Updating the Canvas scale does not necessarily change fixed-size child RectTransform dimensions, so relying on each label receiving a size-change event is insufficient. Runtime refresh now follows actual Canvas scale changes in LateUpdate, after CanvasScaler applies its setting. A focused test checks fixed-size labels at increasing and decreasing scale, and verifies stable frames do not request repeated full text refreshes.

## Changes

- `Assets/LegendsOfRus/Scripts/Runtime/UI/AdaptiveCanvasScaler.cs`: tracks actual Canvas scale and dirties descendant text vertices once when that scale changes. No font replacement, new shadows, permanent per-frame text rebuild or altered layout scaling.
- `Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs`: refreshes all descendant Text meshes before capture, then refreshes again after responsive layout settles. Added Russian/English capture coverage for Settings, Profile, Friends List, Mail, Inventory, Events, Team Hub and Characters.
- `Assets/LegendsOfRus/Tests/PlayMode/AudioHapticsFidelityVerificationPlayModeTests.cs`: adds fixed-size text refresh coverage for growing/shrinking Canvas scale and stable-frame behavior.

The Canvas is constructed by ScreenRouter and already uses AdaptiveCanvasScaler. No owning page prefab/builder or cutover revision changes are needed for this shared component behavior. All 7,193 other Assets files remain unchanged relative to the task-start hash snapshot; none added/deleted. Approved Splash art, font assignments, image lettering, GUIDs and local Editor settings were not edited.

## Verification

The initial run was blocked because Unity held the project lock. After the user confirmed the Editor closed, TextClarityAudit Verify01 passed all 24 PlayMode checks with zero failures/skips: one separate-process save writer, one reader and 22 regression checks. The latter include the eight representative pages in both languages, the new text-scale regression, the existing safe-area scaler test, navigation/lifecycle/energy checks and save migration/recovery checks. No separate EditMode suite or prefab rebuild was run.

Generated 82 actual Unity captures at 576×1280 with a simulated safe area. The review offers earlier Persistence/Lifecycle captures for comparison where available (66 pairs); the 16 added representative-page captures have no matching earlier state in that review. All 82 views and available before/after images decoded in browser checks, the native-pixel toggle passed, and there was no page overflow or JavaScript error. Corrected Settings, Mail and Events were visually inspected. This is a screenshot viewer, not a playable web implementation. Earlier evidence is preserved and is not silently overwritten.

## How to assess remaining softness

In Unity, use a fixed phone resolution such as 1080×1920 and inspect the Game view at 1×. An aspect-only preview at roughly 300–370 pixels wide necessarily has less readable small text. In the HTML review use “View at native pixels” when comparing edges; browser fit/zoom resamples the image. These are viewing recommendations; the user's Editor layout has not been changed.

This correction addresses stale live-text meshes and misleading review captures. Lettering baked into extracted image assets retains its original resolution, and very small captions can still be limited by available screen pixels. No claim is made that every page, every extracted label or physical device is now equally crisp. There was no Android/physical-device test. If text still looks soft at fixed native resolution after this correction, the next step is a page-specific check of small font sizes, local transform scaling and embedded lettering against that capture.
