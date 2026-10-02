# Mail R2 — Joined tabs and letter preview

Mail now uses the original joined tab trim and a larger source portrait with its navy name plaque. The parchment preview follows the source's subject divider, body column, featured attachment, reward divider and lower Claim layout. Russian and English tab/name/body captions are live text using the already installed title serif; exact font identity remains unverified.

## Asset work

One Scenario background-removal sheet request cost 3 CU; no duplicate requests or RGB generation. The returned mask retained much of the staging rectangles, so bounded local silhouettes removed exterior parchment/scenery and disconnected foliage. Original RGB is retained for visible portrait/metal/ink details. Tab badge occlusions and erased caption faces are reconstructed from same-tab rails and approved navy material, not recovered hidden pixels. The selected face uses a clean textured area of the approved Individual Ranking tab. Existing gem, booster, currency, mail controls, shared Home header and scenery assets are reused unchanged.

Eight new sprites: joined tab trim, inactive/selected tab faces, portrait with blank plaque, separate blank plaque for other senders, subject rule, reward divider and a repaired copy of the existing parchment. Source bounds, masks, scripts, job/status/billing and hashes are recorded under `Sources/`. `QA/AlphaBackgrounds.png` is an asset diagnostic on black, white and magenta, not a production screenshot. Uniform enlargement adds no source detail.

The System-tab capture exposed an old reward-slot fragment on the paper’s bottom rail. Only that bounded footprint was replaced with adjacent clean paper/rail pixels; the original asset and its alpha remain unchanged.

## Native implementation

- `ProductionMailRuntime.cs`: native joined-tab controls, live captions, source portrait/plaque and preview geometry. The first sample letter's preview uses the original greeting/body and a clearly labelled reward example.
- `ProductionMailView.cs`: joined-tab binding and detail visibility, native selected-face switching; revision 3.
- `ProductionMailPrefabBuilder.cs`: rebuilds the real existing Mail prefab and writes matching cutover revision 3; saved references read back.
- `Pages/Mail.prefab` and `MailCutoverState.txt`: updated through the owning builder, existing GUIDs retained.
- The existing responsive layout still anchors the header/footer to safe edges and grows the paper on taller displays. No navigation or save-service changes.

The inbox remains sample content. Claim, Claim All, Compose and Delete show the existing unavailable-service explanation. No mail backend, reward grants or real timestamps are introduced.

## Validation

**Final Verify03: Unity 6000.5.9f1 prefab rebuild/readback passed; 4 Edit Mode + 25 Play Mode = 29 passed, zero failures/skips. 16 native Mail captures; 19 browser views passed.** Five existing Assets files changed, eight PNG/meta pairs added, no deletions. All pre-existing PNG/font/meta files and other-page prefabs retained their task-start SHA hashes. Earlier Verify01/02 behavior checks also passed; visual inspection prompted tab-edge/material and parchment-remnant corrections before the final run.

Final results and exact changed files: `QA/ScopeAndVerification.json`. The review's Unity images are actual Play Mode captures from isolated saves and simulated display/safe rectangles. Original references and previous R1 views are explicitly labelled.

Targeted checks cover native binding/import, repeated building, mail tab selection, all five letters, Back hierarchy, locale/phone layouts, header routing and live balances, repeated actions and blocked claims without save changes. Home and Friends regression checks also ran. Transparency and final RU/EN page captures were inspected visually. Browser checks decode all review views at a narrow viewport; scoped C# whitespace checks passed.

No Android build, physical-phone, performance, full project suite or live-mail/backend tests. Native captures and automated layout checks do not establish pixel-perfect source fidelity; see `QA/ReferenceFidelityReview.md`.
