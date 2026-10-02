# Victory — selected original reference rebuild R1

The user approved moving from Gameplay to Victory and explicitly selected `/home/k2/Downloads/match3final/victory.png`. This replaces the earlier red-ribbon/resource-header composition for this page. The actual Victory prefab now uses a navy top title, the original spread-wing Firebird, source-derived earned/empty stars, separate score/resource/booster rows, an emerald Continue control and a sapphire video control.

## Artwork and Scenario workflow

Eleven Scenario Pixa alpha-isolation jobs and one Scenario hidden-scenery reconstruction succeeded: **12 accepted jobs, 46 CU** (11 × 3 + 13). No duplicate submissions, rejected generations or paid resubmissions. Inputs, request parameters, job/status/billing records and unmodified outputs are retained under `Sources/Scenario/`; source rectangles are in `Sources/ExtractionPlan.json`.

Original RGB is retained in the Firebird, filled star, coin, gem and isolated footer emblem. Title/panel/button/card text and sample values were cleared locally for live localization; explicit RGB repair masks and pixel counts are in `QA/AssetChecks.json`. The empty-star face is a documented darkened variant of the original star. Hidden scenery is generated, not recovered source pixels.

The initial Firebird mask included neighboring title/stars; bounded exclusions and component cleanup removed those attachments. Covered feather ends are not claimed as recovered. Pixa selected the hammer instead of the booster frame, so the clean original frame was recovered locally with a bounded opaque panel contour. Opaque frame faces were restored where isolation removed them. The first surface donor contained ornament residue and was rejected locally; the final source-material repair removes that repeated gold pattern. Green/blue button faces use cleaned same-control material rather than retaining text-removal seams. Star-ray remnants outside the panel were removed; earned-star glow remains a separate native layer.

All twelve transparent assets were inspected over black, white and saturated magenta. These sheets are alpha diagnostics, not implementation mockups. Source cutout edges, rebuilt blank textures, empty stars and the hidden scenery remain approximations.

## Native behavior and layout

- Active `Resources/ProductionUI/Pages/Victory.prefab` and `ProductionVictoryView` agree on revision 9. The existing owning builder and narrowly scoped texture importer configure the same assets, with uncompressed source sprites and protected sliced card corners.
- `ProductionVictoryReferenceStyle` applies the selected source geometry to the existing objects and binds the extracted art. Existing object identities, actions and services remain. The old wallet header is inactive, matching the selected original.
- Score, earned stars and rewards use the current completion result. Replay/no-result states remain distinct. Only actual coins, gems and awarded established boosters appear; the illustrative energy/leaf rewards and reference-only booster types are not fabricated. An honest empty row appears when no boosters are awarded.
- Continue retains the existing story/chapter routing and once-only navigation guard. The video control retains verified-provider gating and gold-only doubling. Its caption explicitly says coins; the reference's broader reward claim is not substituted for unsupported behavior. A successful live ad integration is not part of this change.
- Russian and English text remain live. The title uses a straight baseline on the new plaque. The existing local typography/fallback remains; exact source-font identity is unverified.
- Native content stays together on tall displays, with uniform proportions, safe-area containment and the small footer emblem at the safe bottom. This deliberately avoids stretching the buttons or leaving a large internal gap above them.

## Verification

Fresh Baseline01 passed 6 Edit Mode and 14 Play Mode checks before production changes. Verify01 and Verify02 exposed a generic importer resetting Android source sprites to ASTC; a scoped import rule fixed the cause. Verify03 passed all Edit Mode checks but found a booster count/icon boundary overlap. The count clearance, dark video text, missing source frame and tall-screen gap were corrected. Verify04 and Verify05 passed all 20 checks. Final capture inspection prompted more clearance between booster quantities and the lower carved rim; Verify06 passed all 20 checks on that final adjustment, with zero failures or skips. Exact final results and scope are in `QA/ScopeAndVerification.json`.

Coverage includes active prefab/resource bindings, import dimensions, repeated setup, live RU/EN glyph stability, source star textures, reward amounts, icon/count separation, native hit targets, simulated safe areas, replay/no duplicate first-clear rewards, unavailable video preserving rewards, the domain's once-only gold bonus, the actual board-win routing callback and adjacent Pre-Level regression. Captures use isolated saves. The win callback test is not a manual gem-swap win; the bonus fixture is not a successful live ad.

Before/after source hashes cover the entire Assets tree. Eight existing Assets files changed, 29 were added, zero deleted; all 336 prior Victory prefab records remain (357 after adding three native labels). Prior artwork/metas, Gameplay R11 and every other page prefab are unchanged. Exact paths are in the scope JSON. No package, backend, project-settings, signing or Android approval guard edits were authored.

The review defaults to a standalone actual Unity portrait and offers original/before/RU/EN/tall/replay/reward variants. No full-screen reference image is used as runtime UI. The 390-pixel browser review loaded all 12 choices without missing images, JavaScript errors or horizontal overflow. Scoped C# whitespace checks passed; standard Unity-emitted empty-value spaces in serialized YAML were retained. Final Russian booster/count and English tall-phone captures were visually inspected.

## Limits and next action

No APK/AAB build, physical-device check, full-project regression suite, live ad success or exact-font identification. The EN story may retain the existing Russian fallback. Live values/actual booster identities, empty reward states, reconstructed surfaces/scenery and fine glow differ from the illustration. This is not a claim of pixel-identical reproduction.

Next action: review the delivered Victory page, especially the title/Firebird edges and actual reward rows. Visual acceptance remains with the user.
