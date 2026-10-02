# Error Dialog — Parchment Tone R2

Confirmed the user's finding: separate source-texture patches used to erase text/emblem/controls had different low-frequency brightness. Their rectangular boundaries produced a dual-tone parchment face. This was an RGB texture defect, not a Canvas overlay or alpha/import problem.

Only `Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned/panel.png` changed in production. The interior now uses one continuous warm base sampled from clean original parchment, with normalized original texture and a smooth transition to retained edge shadows. A rounded interior mask protects the projecting corner ornaments. Header and all pixels outside the mask remain byte-identical; the complete alpha channel is unchanged. Existing GUID/meta, prefab, owning builder, layout, controls, fonts and behavior remain unchanged, with no rebuild required.

This is a local correction of the existing Scenario-derived resource: **zero new Scenario calls / 0CU**. The earlier five extraction jobs remain the source provenance. It does not reconstruct unknown pixels exactly. The recorded interior low-frequency luminance variation fell from8.66 to0.46 within the same central evaluation region; this metric supports the visual comparison, not a claim of pixel-exact reference matching.

Checks performed:

- Unity6000.5.9f1 texture reimport succeeded.
- **2 Edit Mode +5 Play Mode =7 existing Error Dialog tests passed**, no failures/skips.
- Five new actual Unity captures: RU/EN941×1672, safeEN576×1280, safeRU1080×2400 and callback-enabled RU.
- Existing checks cover native bindings/idempotence, safe-area/raycast/text completeness, localization, modal blocking, Close/Back, single deferred Retry, navigation cancellation and unchanged save/underlying page.
- Before/after alpha composites inspected over black, white and magenta. Final RU reference-size and EN phone captures inspected.
- Browser390px: all7views loaded without errors, missing links or horizontal overflow; repair-script syntax verified.
- Pixel checks confirm alpha/header/outside-mask identity; full production hash audit confirms only the panel PNG changed.

Not performed: prefab rebuild (unnecessary), new behavior tests, adjacent/full-project test suites, APK build, physical phone/touch, backend/support requests or profiling. Existing support/backend/Gameplay integration limitations from R1 remain.

Sources: `Sources/PanelBefore.png`, `Sources/RepairMask.png`, `Sources/RepairParchment.py`, `Sources/RepairRecord.json`; prior original and Scenario records remain in `../R1/Sources/`.
