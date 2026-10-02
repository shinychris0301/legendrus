# Booster Selection R1 — reference comparison

Reference: `Assets/LegendsOfRus/Resources/DesignScreens/BoosterSelect.png` (941×1672), inspected directly. Current production: native `Pages/BoosterSelect.prefab` with separate isolated assets, live text, stock counts and buttons.

| Element | Implementation | Remaining difference |
|---|---|---|
| Outer panel | Scenario reconstruction of empty navy/gold panel; opaque face, sapphire crest/divider, gold corners | Fine engraving, lacquer pattern and proportions approximate the source |
| Close | Original reference RGB, Scenario alpha and local contour cleanup | Adjacent rails removed; source-resolution edge limits remain |
| Play | Scenario reconstruction of empty emerald/gold control; live RU/EN label; preserved sprite aspect | Engraving and flat live lettering differ from source |
| Booster artwork | Four accepted Pre-Level medallions reused unchanged | The reference shows eight tools; the runtime inventory supports four |
| Layout | Two columns, two rows; readable actual names and effects | Intentionally differs from the reference's four-column, two-row arrangement |
| Counts | Actual saved inventory in raised emerald badges | No illustrative counts or fake grants |
| Selection | Inspect tools; Play invokes the existing level-start energy gate | No pre-equipping toggles, plus-purchase buttons or invented tool slots |
| Underlay | Live Pre-Level behind modal; clean existing environment for direct page route | Reference depicts an older Gameplay composition; Gameplay remains excluded |
| Typography | Existing project's Cyrillic serif and language fallback | Exact reference font identity remains unknown; no claim of exact lettering |

Entry: Pre-Level → tap a booster → **ВСЕ БУСТЕРЫ / ALL BOOSTERS**. The accepted inline guide and its Back action remain available.

An optional question about pre-equipping received no answer during implementation. Proceeded with the stated assumption: inspect the four existing tools before Play. Nothing is equipped or spent by inspection. Empty inventory remains inspectable and explicitly reads **НЕТ В ЗАПАСЕ / NO STOCK**. The game consumes tools only through its existing in-round actions.

No exact-match or user-acceptance claim. No extra asset-generation retries were needed. Existing deferred monetization, Failure continuation and SMS follow-ups are unchanged.
