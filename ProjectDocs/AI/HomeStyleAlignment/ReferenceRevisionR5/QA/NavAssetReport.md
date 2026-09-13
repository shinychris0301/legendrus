# R5 navigation asset delivery

Four independently generated PNG sprites are selected in `../Sources/nav_{map,shop,events,characters}_generated.png`. Each is 1254×1254 with real RGBA exterior transparency and intentionally near-opaque navy interiors. `../NavPrompts.json` records exact prompts, original built-in tool files, hashes and source-byte equality. No screenshot extraction, cropping, pixel editing or CLI/API fallback occurred.

The original whole Home reference and actual R4 Unity portrait were inspected directly. The new subjects use an upright rolled map and compass, three-quarter carved market stall with blue awning, angled red parchment scroll, and dark steel/Russian helmet with engraved gold. Their broader cast corners, shadowed bronze-gold rails and navy lacquer have more depth than R4. Details remain independently painted approximations; this does not establish user visual acceptance or exact numerical reference colors.

Four first text-only candidates were superseded because their narrow caption plates would make live labels too small. A targeted edit of the first map sprite yielded an RGB painted checkerboard and was rejected. Four fresh text-only calls generated the selected wide-caption variants. Rejected/superseded bytes and prompts remain in `Candidates/` and `../NavPrompts.json`.

## Performed checks

- Decoded PNG color type, alpha histogram, near-opaque bounds and corner values in local Chrome. Selected sprites have genuine transparent exterior pixels and alpha-bearing PNG color type6.
- Directly viewed `NavCardReview_panels.png`: unchanged whole sources rendered at203×222CSS pixels over black, white and saturated magenta. Exterior edges are clean; no checkerboard or scenery contaminates the selected art.
- Measured blank caption ink rectangles from source pixels, including eight-pixel margins. `NavCaptionGeometry.json` records exact source and normalized coordinates for native glyph containment checks.
- Compared each selected source to the original generated output byte-for-byte; all four match.
- First sandboxed Chrome start timed out. The same local read-only diagnostic succeeded with reviewed escalation. No source edit was needed to bypass this environment limitation.

At200px visible frame width, safe label widths are176.3px(map),181.8px(shop),178.3px(events) and172.5px(characters). At222px whole-source height, corresponding safe ink heights are21.2,21.2,18.9 and20.9px. The UI owner has these bounds and must validate actual live RU/EN glyphs after import. All artwork must be mapped as the complete source PNG without cropping.

## Not performed by this subtask

Production asset or prefab edits; Unity import, compilation or runtime tests; actual Unity capture; APK/AAB build; physical-phone tests; reference pixel-identity/color-identity checks. Root owns integration and final verification. Source alpha QA does not establish final page fidelity.
