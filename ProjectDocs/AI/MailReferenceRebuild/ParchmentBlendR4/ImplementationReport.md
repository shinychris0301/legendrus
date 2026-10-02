# Mail R4 — parchment color continuity

The replacement parchment was lighter than the original paper next to the frame. Restoring the scrollwork in R3 exposed that remaining color seam. Both preview and opened-letter fills now match the darker warm paper at the border, with a gradual transition around the corner shadows.

The existing clean donor grain is blended against the original inner-edge lighting. Following the user’s additional report of detached corners, the mask now includes the paper inside all four restored corner patches while excluding the gold carving. Its correction fades smoothly at the boundary; the lighter patches are blended into the same paper surface. All original alpha and pixels outside the mask are verified unchanged. No new art generation or Scenario request was needed. The two R3 source sprites remain unchanged; two new PNG/meta pairs supply the R4 surfaces.

Runtime sprite references, view revision, owning builder, active Mail prefab and cutover are synchronized at revision 5. Text, buttons, rewards, layout and mail-service behavior are unchanged. Exact assets, masks, source hashes and method are recorded in Sources/Provenance.json and Sources/BlendParchment.py.

## Verification

Final Verify02 in Unity 6000.5.9f1 passed rebuild/readback, 2 Mail EditMode and 6 Mail PlayMode tests (8 total; zero failures/skips). Sixteen native captures and 20 browser views passed; no browser errors/overflow. Five existing Assets files changed, two PNG/meta pairs added, no deletions. Final results and paths are recorded in QA/ScopeAndVerification.json. Actual Unity captures are labelled separately from original and prior-version images. Alpha composites and all four enlarged corners of both panels were inspected. The initial run passed behavior checks, then the corner blend was extended before a second verification run. Targeted Mail tests cover prefab ownership/bindings, RU/EN and safe areas, opening letters, tabs, routes and guarded actions.

No Android build, physical-device test, full project suite or backend test. Paper texture and obscured lower corners remain reconstructed; exact source-image identity is not claimed.

Corner diagnostics: [preview, all four corners](QA/preview_AllFourCorners.png) and [opened letter, all four corners](QA/letter_AllFourCorners.png). Left is R3; right is R4.
