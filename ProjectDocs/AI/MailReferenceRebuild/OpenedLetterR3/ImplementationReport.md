# Mail R3 — Opened-letter hierarchy

The opened letter now uses the original circular guild emblem and separate sender, role, subject, greeting, body and signature fields. Original parchment rules and the central-diamond divider restore the reference's grouping. Approved rewards, shared navigation and guarded sample-mail behavior remain intact.

## Assets

One Scenario background-removal request, 3 CU, no retries or RGB generation. Existing Team Hub/Chat/Ranking guild emblems were inspected; their shield silhouettes differ from this letter's circular medallion, so the original letter emblem was extracted. Its enlarged source RGB is combined with the unmodified returned alpha. Two divider sprites isolate original ink strokes through bounded luminance masks. The R2 attachment divider, Claim controls, gems and boosters are reused unchanged. Existing paper/frame assets supply the base for bounded surface repairs.

Five new PNG/meta pairs: emblem, two dividers, and two repaired panel surfaces. Original bounds, source hashes, scripts, request/status/billing and output hashes are under `Sources/`. `QA/AlphaBackgrounds.png` is the black/white/magenta asset diagnostic. No generated artwork is presented as extracted source content.

## Panel-background correction requested during this pass

The earlier paper preparation used a rectangular replacement mask starting inside the ornaments. Inspection of `MailStyleAlignment/R1/Sources/PreparePaper.py` and original corner pixels confirmed that it had painted across inward-facing scrollwork in both the Inbox preview and full letter. This was baked into the sprites, not a runtime z-order defect.

Restored the original decorative corner pixels with bounded contour-shaped collars and a short feather into the existing clean paper. The full letter's lower corners reuse mirrored original upper corners because source Back/Inbox controls obscure those areas; this continues the earlier reconstruction approach without claiming exact recovery. Existing alpha and every pixel outside the repair masks remain identical. Previous assets were not overwritten. No additional Scenario request was needed for this source-pixel repair.

See `Sources/PanelSurfaceRepairs.json`, the two repair masks and `QA/*CornerBeforeAfterSource.png` (left: before, center: repaired, right: original). `QA/PanelSurfacesOnBackgrounds.png` verifies black/white/magenta composites. The final runtime consumes the repaired surfaces. Initial comparisons before this fix found default Inbox captures identical to R2 and one header rasterization pixel different in each narrow capture; the requested corner fix now intentionally changes preview pixels.

## Native implementation

- `ProductionMailRuntime.cs`: dedicated opened-letter layout and localized live content; compact Inbox preview remains independent. Source guild emblem replaces the inbox shield only in the opened guild letter. Separate subject, greeting/body/signature and attachment ornaments; adjusted reward placement.
- `ProductionMailView.cs`: current revision 4.
- `ProductionMailPrefabBuilder.cs`: rebuilds the real existing Mail prefab and writes matching cutover revision 4; saved prefab bindings read back.
- `Pages/Mail.prefab` and `MailCutoverState.txt`: serialized through the existing owning builder, preserving GUIDs.

The full guild message is an explicitly labelled design example. Existing tab/read state, Back hierarchy, routes and blocked Claim/Claim All/send/delete semantics are retained. No backend, currency grant, expiry countdown or real timestamp was added. Existing responsive layout continues to anchor header/footer to safe edges.

## Verification

Final Verify04 completed in Unity 6000.5.9f1: rebuild/readback succeeded, 4 EditMode and 25 PlayMode tests passed (29 total, zero failures/skips). Sixteen Mail captures and 19 browser views passed, with no browser errors or horizontal overflow. Five existing Assets files changed and five PNG/meta pairs were added; no deletions. Exact results and changed files are in `QA/ScopeAndVerification.json`. The review contains actual Unity Play Mode captures with isolated saves and simulated phone/safe-area sizes, alongside labelled original and R2 comparison views.

Targeted verification includes prefab bindings/idempotence, live header values and navigation, all five letters, tabs and unread state, layered Back, locale/phone text and hit targets, repeated routes, unavailable claims and save preservation, plus Home/Friends regression. Existing tests were reused. Alpha backgrounds and final native RU/tall-safe EN were inspected. The first run passed behavior checks; visual review found two short rules caused by aspect-ratio constraints, corrected before final verification.

No Android build, physical device, performance, full project suite or live-mail/backend test. Fine contours, paper material, heavier fallback typography, tall-screen whitespace and remaining title/footer differences are documented in `QA/ReferenceFidelityReview.md`.
