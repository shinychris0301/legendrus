# Victory R2 — reward surfaces, original lettering and celebration glow

Implemented in the canonical native Victory prefab, revision 10. The user approved the R1 review recommendation, then requested original title/button lettering, all original ad-button text, and visible Firebird/star glow. The selected original remains `match3final/victory.png`.

## Result

- Six cleared faces now use a continuous Scenario-repaired navy/emerald/sapphire material instead of conspicuous mirrored donor repeats. Exterior alpha and existing borders are retained. Original heading flourishes and booster quantity dividers are recovered from the source.
- Resource cards are centered, compact 231×104; booster cards recover their source 131×171 proportions, centered artwork and separate counts. The feedback line sits between rows, clearing the lower carving. Score remains live with gold shading.
- Russian title, Continue and the complete ad button retain original RGB and existing Scenario isolation alpha. Russian SCORE/REWARDS/BOOSTERS headings use source lettering composited onto the repaired board. No duplicate live lettering is drawn over these assets. English uses blank faces and live localized text.
- The eligible Russian ad button displays all original wording, including `УДВОИТЬ НАГРАДУ`, `СМОТРЕТЬ РЕКЛАМУ`, and `x2`. A separate live note explains that the video bonus doubles coins only. This note temporarily takes priority over the existing story line while the bonus is offered. Unavailable/already-doubled states use live status text and the blank button; they do not retain an active-looking baked offer.
- A separate transparent Firebird bloom follows the existing isolated silhouette, drawn behind the original bird and title. Earned-star halos are wider, warmer and contain 48 rays and 28 fixed sparks each. Existing reveal timing remains; unearned stars have no halo. The original bird and star face images are unchanged.

## Scenario and source provenance

One accepted Scenario `model_openai-gpt-image-2` bounded surface-edit job, **12 CU**, no duplicate submissions or paid retries. See [request](Sources/Scenario/faces/Request.json), [billing](Sources/Scenario/faces/Billing.json), [face provenance](Sources/AssetProvenance.json), [lettering provenance](Sources/LetteringProvenance.json), and [glow derivation](Sources/GlowProvenance.json). Source RGB is exact across the entire title/Continue/ad sprite rectangles; existing isolated alpha is unchanged, verified in [source checks](QA/SourceLetteringChecks.json). Existing Scenario masks were sufficient for lettering; no additional paid isolation was needed.

Generated faces provide only bounded interior repairs, not runtime scenery crops or a screen image. The original exterior borders, original Firebird and star art, and production environment remain intact. A generated inner rail and source numeral fragment were found during visual review and excluded locally. Source gold headings are recovered through a local gold mask; fine anti-aliasing there is approximate.

## Verification

Unity 6000.5.9f1 final **Verify05**: canonical import/rebuild/readback succeeded; **6 Edit Mode + 14 Play Mode = 20 passed**, zero failures/skips. Checks cover Victory and Pre-Level regressions, actual reward/score values, replay and no-result states, bonus state changes, existing routes/input, safe-area layout, card clearance and Russian/English switching. The glow layer cannot intercept input, and earned-star visibility remains tied to the result.

Native 941×1672 three-star Russian, one-star replay and 576×1280 safe-area English captures were visually inspected. Transparent assets were inspected on black, white and magenta. Browser review: **13 choices**, no decode/script errors or horizontal overflow. All earlier run records are retained in `QA/History`; Verify03 found an English/live-title safe-face clearance issue during positioning work, corrected before final validation. Passing source/behavior tests do not establish exact pixel matching.

[Scope audit](QA/ScopeAndVerification.json): 12 existing Assets files modified, 10 files added (five PNGs and their metas), none deleted. All 357 prior prefab records retained; 362 total. Owning `ProductionVictoryReferenceStyle`, view revision and canonical prefab agree. Other page prefabs, Gameplay R11, all pre-existing metas, environment, Firebird and star sprites are unchanged from the start-of-task hash snapshot. No new scene, Canvas, EventSystem or reward service.

## Limits and next action

Original Russian fixed lettering is artwork, not an identified font. English and dynamic numeric lettering still approximate the reference. Repaired faces, reconstructed R1 scenery, Firebird bloom and star particles are not pixel-identical to the source. English story text may still use the existing Russian fallback. No Android APK/device, full-suite or successful live-ad-provider test was performed; the ad service gate and coin-only reward behavior were preserved.

Next: review the actual R2 native captures, especially complete source button wording, glow strength and reward framing. No further automatic art generation is scheduled.
