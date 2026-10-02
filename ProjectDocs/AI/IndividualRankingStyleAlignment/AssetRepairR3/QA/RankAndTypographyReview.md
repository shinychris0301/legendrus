# Rank badges and typography review — 2026-09-17

Compared supplied badge screenshot, original reference and saved R3 941×1672 RU Unity capture. Inspected installed gold/silver/plain sprites, R2 PrepareFinal.py, active layout/runtime/font resolver, typography asset and prior PlayMode log. No production edits or new runtime verification.

## Findings

- R2 numeral removal replaced the inner badge faces with a synthetic low-detail gradient. This visibly removes metallic texture and depth.
- Medal alpha uses a brightness/color threshold plus an ellipse. Dark shaded laurel pixels disappear; foliage looks fragmented. Restore continuous shaded silhouettes, not only bright gold/silver pixels.
- Rank numerals look too low in the saved capture. Ordinary badge rect center is row-local x53.5, while the text rect center is x57. The plain sprite has additional transparent margins and preserveAspect sizing; account for its visible rim when aligning the text. Bronze row is shorter than its medal artwork; inspect overlap/draw order too.
- The typography asset has no RegularFont or StartFont bound; no .ttf/.otf files found under Assets. Ranking resolves the shared development-only OS fallback. Current text is therefore not an intentionally matched embedded font. Exact reference identity remains unknown.
- Source lettering has different glyph proportions, narrower body text, metallic relief and numeral placement. Font size/color changes alone will not reproduce it.

## Recommended bounded repair

Repair the five badge bases from original pixels, preserving metal shading, and remove numerals only within narrow masks; use Scenario for missing surface reconstruction if necessary. Center live rank numerals optically and check single/two-digit values at phone size. For typography, select and license a distributable Cyrillic family for dynamic text, use a Ranking-specific assignment, and tune width/weight/spacing/baselines/shading. Preserve original lettering as isolated, locale-specific art for fixed Russian headings/captions where feasible under the approved extraction workflow; keep changing values and names live. Do not replace the shared typography asset or claim the unknown source font is identified.

## Checks and limits

Performed saved-image/source/asset/configuration inspection and read prior Unity logs. No new Unity compilation/capture/test, APK/device test, font installation, image edit or Scenario API call. No credits spent. CurrentTask and SessionLog updated. R3 remains the active implementation; rank and font repairs are outstanding.

Prior R3 runtime font evidence:

> SplashTitle typography is NOT reference-approved. Rendering with OS Liberation Serif — DEVELOPMENT ONLY. RegularFont and StartFont are not assigned. Open Legends of Rus > Production UI > Splash R3 > Select Typography Settings. Bind project font assets and verify their identity against the reference. An image-generated reference may not have an identifiable source typeface; do not mark a substitute as exact. This notice is emitted once per unchanged status per Play session.
