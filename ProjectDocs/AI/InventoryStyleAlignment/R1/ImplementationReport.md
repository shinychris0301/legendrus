# Visual Page27 — Inventory R1

Implemented in Unity6000.5.9f1. User approved the recommended native Inventory: saved booster quantities and clearly labelled collectible previews. The dedicated original was found among generic download filenames, copied unchanged and registered. Existing local edits and accepted pages preserved.

## Native implementation

Active `Assets/LegendsOfRus/Resources/ProductionUI/Pages/Inventory.prefab`, owning `ProductionInventoryPrefabBuilder`, runtime/view/layout and cutover revision1. ScreenRouter binds native controls, dismisses the explanation first on Back and suppresses the duplicate generic HUD. Legacy Wave02 art/prefab stays unchanged but is superseded by the registered native page.

Four tabs: Artifacts (all20 reference examples), Boosters (four existing working types), Materials (three examples), Quest (four examples). Categories for unimplemented items are illustrative. Every card can be selected; title, illustration, quantity and availability copy update. Selected item receives a gold outline. Source sample quantities are removed and replaced by live em-dashes for previews. The source coin illustration is explicitly not the wallet balance.

Booster quantities read `boosterMace`, `boosterFirebird`, `boosterLivingLight`, `boosterSeaPearl`; existing artwork/names/functions remain unchanged. Negative presentation values clamp to zero without changing the save; live GameState changes refresh counts. This page never consumes a booster or grants items. Use is disabled; Details explains preview status or use during gameplay. No unimplemented crafting/ownership is claimed.

Live account header; RU/EN selected embedded fonts and source Russian title. Back goes Home, header goes Profile/Settings; resource plus buttons go Shop coin/gem category or Out of Energy. Footer Offers goes Special Offer, Gems/Bundles select the actual Shop category, Boosters opens the owned booster tab, VIP shows an unavailable explanation. Repeated route clicks commit once; modal explanations block underlying controls.

## Source artwork / Scenario

25 successful source-alpha isolation jobs plus one bounded paper repair generation: **26 jobs, 88CU** total.26 runtime sprites including English blank-title derivative. Original title,20 framed cards, feather detail panel, buttons and outer paper frame retained. Local numeric/lettering masks and source RGB preservation are recorded. Scenario alpha supplies exterior silhouettes; intentionally opaque card surfaces are restored. Source resampling adds no original detail.

One item download was truncated; the incomplete file was archived and its existing accepted job recollected, without another paid submission. Broad paper inpainting left ghosts or damaged texture, so it was rejected. One Scenario generation repairs only the covered parchment interior; original frame/color boundaries remain source pixels outside the recorded blend mask. See [provenance](AssetProvenance.json), [workflow notes](Sources/README.md) and [actual billing](QA/ScenarioAudit.json). Approved clean scenery, header, shop footer and working booster artwork are reused, with no source-file changes.

## Checks performed

- Unity import/C# compilation, scoped prefab rebuild and serialized readback passed.
- Broad7 Edit Mode +18 Play Mode across Inventory/Profile/Mail passed; final focused2 Edit +6 Play Inventory checks passed.25 distinct latest checks; no failures/skips. Earlier setup runs caught malformed escaped text and a renamed test-helper identifier; both corrected. No failing evidence erased.
- Checked prefab bindings/idempotency, no screenshot dependencies or duplicate Canvas/hotspots; all20 item selections; tab filtering; real/zero/negative/maximum quantities; live inventory refresh; save preservation; route targets/shop categories/repeated clicks; modal/Back behavior; RU/EN text meshes/fit, full-screen background and simulated safe-area bounds/center raycasts.
- 14 final actual Inventory Unity captures at941×1672,576×1280 and1080×2400, plus31 Profile/Mail regression captures. Tests use isolated save storage and restore state.
- Visual review compared the original with main RU/EN, tall-phone booster/detail states, materials and maximum values. Original quantity remnants, lost frame alpha and parchment repair defects corrected before final delivery.
- Black/white/magenta asset alpha inspected. Original RGB outside repair masks, immutable submitted input hashes, runtime hashes, source Python syntax and scoped whitespace checked. Scope audit:69 new Assets files including metas; only pre-existing ScreenRouter changed. All other pre-existing Assets/GUID/meta files/packages/ProjectSettings unchanged from task-start snapshot.

Browser check at 390px loaded all 15 review views without JavaScript errors, missing report links or horizontal overflow. Evidence: `QA/BrowserReview.json`.

## Not performed / deferred

No APK build, physical-phone/Android touch test, performance profiling, full test suite/all pages, live collectible backend/crafting/use or calibrated color/pixel comparison. Exact original typeface is unverified. Source-derived art/layout is close but not an exact replica; see [reference review](QA/ReferenceFidelityReview.md).

Feature phase after UI: define artifact/material/quest ownership, item catalog and acquisition, authoritative quantities/recipes/effects, transactional consumption and sync. Preserve current gameplay booster semantics; do not infer recipes or effects from decorative reference art. Existing SMS and Failure continuation deferrals remain unchanged. Next: user review of R1 at phone scale.
