# Pre-Level reference rebuild R1

The actual PreLevel prefab now uses the original-reference panel stack and four booster medallions, the shared Home/World Map player header, extracted chapter banner/Play/Back controls, and responsive safe-area placement. Objective counts, moves, inventory quantities and rewards remain native, localized values from the selected round and saved state.

The user's follow-up identified extraction defects. The final repair removes the rectangular overlap above the moves medallion, excludes neighbouring scenery from its silhouette, closes the reward frame's missing top edge with a source gold rail, and restores the full booster rims from the original pixels. The count badges remain separate live controls; original sample quantities are erased from the underlying art so the guide cannot display a stale inventory count.

## Artwork and fidelity

- Original: `/home/k2/Downloads/match3final/Pre-level.png`, SHA-256 `32e05c72c56f403ea065a327aea0ee9bdef33b8bb22330807baff117c0b40195`.
- Scenario was used: nine initial Pixa alpha jobs, one focused reward-frame extraction after the first output omitted the frame, and one hidden-scenery reconstruction. **11 accepted jobs, 43 CU**; no duplicate submissions. The later edge corrections use local original-pixel/mask repairs and cost no further credits.
- Original RGB is retained under the extracted alpha except bounded text/occupied-surface repairs. Fully opaque panel faces are preserved. Supersampled booster/moves contours repair the initial masks; reward closure and obscured goal rails are reconstructed from nearby original rails/colours, not claimed as recovery of hidden pixels.
- The clean environment combines original visible scenery with Scenario infill where interface elements covered it. 281,831 original scenery pixels remain unchanged outside the repair support.
- Submitted Scenario requests, outputs, billing and jobs are retained in `Sources/Scenario`. The focused reward-frame request used an interim repaired face; subsequent asset builds update `Sources/Inputs/rewards_frame.png`, so the request manifest records the historical submitted version. Its alpha is reused; no replacement request was sent.
- Alpha resources were inspected over black, white and magenta, then in actual Unity captures. `QA/AlphaFinal.jpg` is inspection evidence, not a standalone production screen.

Remaining differences are explicit: live text uses the approved project fonts, not a verified exact source font; goal gems retain approved gameplay art; actual saved values differ from reference examples; reconstructed panel faces and hidden scenery are approximations. The booster heading remains “БУСТЕРЫ В ИГРЕ” / “IN-GAME BOOSTERS” because tapping opens the existing guide rather than equipping a booster. No gameplay rules were changed.

## Implementation

`ProductionPreLevelSourceControls` owns source composition, imported sprites, live text placement, shared-header attachment, and press feedback. PreLevel view revision 5 requires those bindings; runtime migration and the serialized prefab builder agree. The builder explicitly imports single, full-rect, uncompressed 2D sprites without mipmaps. This fixes an initial import problem where minimal new metadata was interpreted as cubemap textures and sprite bindings were null.

`ProductionPreLevelLayout` pins navigation to the safe-area edges and distributes spare height between the content sections. `ProductionNavigationStyle` bypasses its old Pre-Level override when the source composition is installed. Existing references, selected-level routing, story flow, energy gating/debit, reward-claimed state, inventory ownership and guide actions are retained.

## Verification

Unity 6000.5.9f1 compiled and rebuilt/read back the canonical PreLevel prefab. Final targeted run: **15 EditMode + 29 PlayMode = 44 passed**, zero failures/skips. Evidence: `QA/EditMode`, `QA/PlayMode`, `QA/PreLevel` and `QA/ScopeAndVerification.json`.

Coverage includes canonical binding/import checks, two/three objectives, level rewards and claimed states, all four booster guides in RU/EN, multi-digit/empty inventory, selected-round start and energy gating, reference/tall-phone safe areas, plus Home/World Map/Chapter Map regressions. Native screenshots were inspected separately from test results; tests do not establish pixel-exact fidelity.

Initial sprite-import/build failures and an incorrect command namespace in Build04 are retained as diagnostic logs; the subsequent canonical rebuild succeeded. No Android APK, physical-device test, or complete project test suite was run. The report does not claim otherwise.

## Scope

Final audit: eight existing Assets files modified, 23 added (ten sprites/metas, folder meta, source-controls class/meta), zero deleted. All 548 prior prefab records are retained; 570 now exist. All 12 review choices load at 390×844 without browser errors or horizontal overflow.

The machine-readable scope audit compares Assets against the start-of-task hash snapshot, including all earlier user changes. Only the PreLevel page prefab is intentionally rebuilt. Existing Home, World Map, Chapter Map, Splash, Loading and Gameplay prefabs/artwork are preserved. New art lives in `Resources/ProductionUI/PreLevelReferenceAligned`, with distinct GUIDs. A shared-header overload adds PreLevel support without replacing the other pages.

Next: user visual review of the native Pre-Level captures. Gameplay reference rebuilding remains a separate subsequent screen.
