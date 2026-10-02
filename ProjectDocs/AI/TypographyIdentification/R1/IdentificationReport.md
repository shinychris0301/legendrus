# Typeface identification investigation — R1

**Result: exact typeface identity remains unverified.** This was a real image-matching and specimen investigation, not confirmation of the existing fallback. No font is installed or assigned to the game by this work. The reference may use a real font, altered lettering, or generated letterforms; its origin has not been established and this investigation does not prove any of those explanations.

## Reference and scope

Original: `/home/k2/Downloads/match3final/individual ranking.png`, 941×1672; SHA-256 `56ec682e5e5982688789ce43afa05bbd4c51768a943829ecb2331d37b9164611`. PNG metadata is empty; it contains no embedded font name. Thirteen original-pixel sample crops cover the title, four player names, six rank values, one score and the Season tab. Five additional monochrome/3× analysis versions reduce color/shading for detector input. These are diagnostic images, never runtime resources.

The shared Unity typography still has no bound RegularFont/StartFont; prior R3 runtime evidence identifies Liberation Serif as the development fallback. Matching this Ranking reference does not establish that every other page uses the same original typeface. No shared typography or other page was changed.

## Image-based tools actually used

### WhatTheFont

[Official service](https://www.myfonts.com/pages/whatthefont). A direct HTTP request initially returned403, but Playwright/Chromium opened the public interface. Actual sample uploads and Identify actions produced result lists, saved under ToolEvidence. The interface explicitly limits detection to Latin text, so Cyrillic results are weak evidence. Both automatic regions and manually selected full-line regions were tried; this matters because automatic detection initially selected only one word or one number group.

| Input/query | Returned suggestions (examples) | Assessment |
|---|---|---|
| Original player-name automatic region | Monkton Book Condensed, Esperanto Condensed Small Caps, PN Macabre Millionaire | Suggestions, not an identification. The [foundry's Monkton glyph sheet](https://www.clubtype.co.uk/resources/pdf/glyphs/Monkton%20Book%20Condensed.pdf) has no Cyrillic alphabet; it cannot supply the exact shown Russian names as published. |
| Monochrome name, complete selection | Streltsi, Donskoi, Skreech Caps, TC Wonderling Condensed Light | Inconsistent with earlier results and no verified Cyrillic/reference match. No inference from the Russian-sounding family names. |
| Monochrome title, complete selection | Retablo Antiguo, LD Eleanor Rae, Gessetto Roman, F2F MadZine Dirt | Detector responds to rough/embossed edges; none confirmed as the reference family. |
| Monochrome score, complete selection | Evoque Narrow Light, Minion Pro Condensed Display Regular, Galadali Light, Evoque Narrow, Kepler Light Semicondensed Subhead | More plausible numeral leads, but a numeral match alone does not establish Cyrillic identity or the rank-badge font. |

**Cross-check of actual suggestions:**

- [Evoque Narrow Light](https://www.myfonts.com/products/narrow-light-evoque-478512): entered the original Russian names and numerals in its official specimen. Numerals render, while Cyrillic renders as missing-character boxes. Saved `ToolEvidence/evoque-text.png`. This rules it out as the unmodified font for those Russian names, although it can still resemble the digits.
- [Minion Pro Condensed Display Regular](https://www.myfonts.com/products/cn-disp-regular-minion-38420): official specimen successfully renders the Russian names and numbers. [Adobe confirms Cyrillic coverage](https://fonts.adobe.com/fonts/minion). Compared the actual name specimen to original letter silhouettes in `MinionNameComparison.png`: thin diagonal/serif treatment and proportions still differ, particularly И, М and the lowercase run. The observed Regular specimen is not an established exact match; other weights/tracking remain possible, unverified variants.
- Also rendered [Minion Pro Bold Condensed Display](https://www.myfonts.com/products/cn-disp-bold-minion-38350) for the exact title. Its Й breve, relative letter spacing and stroke shapes do not establish the reference title as this font. Specimen saved; no commercial font binary was downloaded or purchased.

### Font Squirrel Matcherator

[Official service](https://www.fontsquirrel.com/matcherator). Its public upload endpoint returnedHTTP200 and accepted a sample. However, in the browser the required combined JavaScript returnedHTTP202 with an empty body; the interface did not process glyphs or return usable matching results. Upload acceptance is not font recognition. Saved screenshot and relevant response-status record in ToolEvidence. No attempt was made to bypass access controls.

## Independent Cyrillic comparison

Rendered **114 font styles/weight instances** with the actual required characters, checking font character maps before including them. Candidates comprise25 downloaded family directories from the official Google Fonts repository plus local serif families (some candidate files without required glyphs were excluded). Font files remained under `/tmp/lor-font-identification/fonts` or their existing system location; **none were copied into Assets or distributed with the report**. `CandidateSources.json` records official download URLs and hashes. `ComparisonMetrics.json` records each eligible file/instance and each sample result.

Separate screenings used:

- Title: `ЛИЧНЫЙ РЕЙТИНГ` including the Й accents.
- Names: `Маша`, `Илья Муромец`, `Василиса`, `Кощей`.
- Ranks: `1`, `2`, `3`, `4`, `10`, `12`; score and tab were sampled separately as additional evidence.

| Area | Stronger local screening leads | Status |
|---|---|---|
| Title | Lora700, Roboto Serif600, FreeSerif Bold | Similarity leads only; metal relief, outlines, Й accents and spacing prevent exact confirmation. |
| Names | Spectral Medium, Roboto Serif500, Vollkorn500–700; PT Serif also examined | Closer normalized silhouettes than the current Liberation Serif, but word proportions/terminals still differ. |
| Ranks | Noto Serif condensed Medium variants, Spectral Medium, Bitter600, STIX Two Text | Small samples and metallic highlights make ranking unreliable as identity evidence. No exact numeral family confirmed. |

`Sources/CompareFonts.py` normalizes each rendered word to its reference silhouette bounds, then measures overlap and symmetric distance. This intentionally permits width fitting to screen possible shapes. It does not optimize all tracking/hinting/optical-size settings, preserve native widths in the comparison sheets, or convert similarity into identification confidence. Word-specific stretching, low source resolution, shadow remnants and threshold sensitivity can favor an incorrect font. **The top-scoring candidate is not proof of the original font.** Earlier diagnostic masks that failed to include the silver2 were corrected before the saved final114-instance results.

## Letterform versus effect

The title has a thick bright bevel, inner shaded face, fine distressing and an offset dark extrusion. These alter apparent stroke thickness but are not the underlying typeface. Names have narrower proportions, smaller gold edging and shadows; rank numbers have their own rim/highlight treatment. Matching only font color or adding a generic shadow cannot reproduce these features. Both raw color crops and monochrome diagnostic masks are retained; masks are approximations, not recovered vector outlines.

## Outcome and next evidence needed

Do not label Liberation Serif, Minion, Lora or any other candidate as exact. Do not purchase or deploy a suggested font based on these results. The strongest next evidence is an editable design file with text layers, a font list from the source creator, or confirmation of how the references were produced. An asynchronous question about source origin was sent; no answer had arrived when this report was written. Higher-resolution original text can improve further matching but does not guarantee identification. Until confirmed, preserve the original lettering where exact fixed-word appearance matters and keep any future dynamic-font choice explicitly labelled as an approximation; this report does not authorize or implement that replacement.

## Verification

Performed: source dimensions/hash/metadata, thirteen separate sample crops, browser uploads/selection/results, Cyrillic specimen checks, published glyph-sheet check,114-instance local comparison, manual source/specimen review, Python syntax, local review links, production hash comparison. Production scope unchanged. No Unity compile/rebuild/new captures/tests, APK/device checks, font embedding, Scenario calls/credits, purchase, or public forum posting. Playwright and its browser were installed in temporary/cache locations for this research. CurrentTask and SessionLog updated.

Local review browser check: all10 evidence selector options loaded, no missing images or page errors. CurrentTask/SessionLog whitespace check passed after removing an extra trailing blank line.
