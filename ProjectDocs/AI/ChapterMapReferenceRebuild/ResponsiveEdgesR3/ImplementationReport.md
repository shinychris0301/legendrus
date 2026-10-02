# Chapter Map — landmark scenery, staggered route and safe edges R3

Chapter Map revision 4 responds to the user's requests for navigation at the phone edges, a more impressive chapter environment, and staggered rounds that show more at once.

## Changes

- Header and title follow the **top safe-area edge**. Bottom navigation, carving, current-round shortcut and range caption follow the **bottom safe-area edge**. The map expands between them without stretching buttons, portraits, lettering or gems. Its scroll position is preserved as spacing changes with aspect ratio.
- Alternating left/right round positions show **11 complete controls** at the initial position, compared with seven before. A subtle native gold dotted route connects them. Fourteen objects are reused across 500 planned positions. Original medallions, stars, lock state, selected-round highlight and saved progression are preserved. Offscreen nodes are hidden before their stars can appear alone.
- A new Scenario portrait expands the approved World Map Chapter 1 village with a carved timber chapel, lantern-lit arched bridge, river/cascades, ancient oak and illuminated village well. These landmarks replace the earlier repetition of similar houses. The original World Map painting and earlier portrait remain intact.

One additional accepted `model_openai-gpt-image-2` job, **13 CU**, no retries. Together with the earlier R2 portrait this is two accepted jobs / 26 CU for the two requested scenery passes. Prompt, input hash, job, billing and unmodified output are under Sources/Scenario/chapter01_landmarks. Native file: `environment_worldmap01_landmarks.png`; no baked UI, local color edits or text. This is a new composition in the approved style, not pixel-exact extraction of the wide painting.

## Verification

Unity 6000.5.9f1 import, compile, canonical prefab rebuild/readback passed. **Final Verify02: 12 EditMode + 21 PlayMode = 33 passed, zero failures/skips.** This includes ChapterMap plus unchanged Home/WorldMap regressions. Verify01 also passed; visual inspection then caught isolated stars from a clipped node, fixed before final verification.

Checks cover exact header/footer safe-edge registration, map filling the space above the footer, at least ten complete visible controls while scrolling, real button raycasts, no stars overlapping adjacent faces, repeatable responsive layout, owned-pointer dragging, bounds/current jump, pool reuse, unpublished-round clicks, save preservation, RU/EN and simulated portrait safe areas. Final native captures are in QA/ChapterMap. Tall-phone and reference-size views were visually inspected. Ten browser review choices passed at390×844, with no missing images, errors or horizontal overflow. Home/WorldMap native captures are pixel-identical to R2. Existing gameplay effects were not changed in R3; the previous R2 effects verification remains recorded separately.

Eight existing Assets files changed; four added (one PNG/meta and route graphic/meta), none deleted. All 780 prior prefab records retained; 976 total after adding four pooled nodes and their native controls. Existing art GUIDs and other page prefabs/art remain untouched. QA/ScopeAudit.json lists the exact files.

No Android build, physical phone test, hardware input test, whole-project suite or device profiling. Captures use isolated test saves; their sample balances are not newly granted rewards.

## Remaining work

The map has **500 planned positions, ten authored playable rounds**. The nineteen other chapters still need their own portrait expansions from their corresponding approved World Map paintings. One Chapter 1 painting currently repeats along the route; this is not 500 unique environments. The native dotted route indicates progression independently of the painted roads.

The catalog/story migration remains a separate next stage: the legacy finale still refers to round 10 and must be relocated before releasing a 500-round chapter. Then author and validate the first 50 rounds before scaling content. No fabricated unlocks or new gameplay levels were added in this visual refinement. User visual acceptance is pending.
