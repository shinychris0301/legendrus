# Failure — source extraction R11

Implemented in the canonical Unity Failure prefab, runtime and owning builder at revision11. This pass uses Scenario API for isolation and original reference RGB for eleven UI resources. It is an implemented, verified refinement; it is not a claim of exact full-screen reproduction or user acceptance.

[Portrait review](Fail_SourceExtraction_R11_Review.html) · [Reference comparison](ReferenceComparison.md) · [Production audit](QA/SourceAudit.json) · [Unity results](QA/UnityResults.json) · [Extraction audit](QA/ExtractionAudit.json).

## Result

- Original title silhouette, crest, antique-gold borders and navy face; original retry button at native aspect. No separate projecting title-side ornaments were added.
- Original red/blue offer proportions and deeper emerald faces. Embedded gem/lightning/play artwork appears once. The price, captions and unavailable labels remain live.
- Original goal frame, heading plaque, nameplate and four illustrated booster medallions. Original green/gold badge faces carry live inventory counts. Existing reference aliases and actual booster functions are unchanged.
- Main R10 panel geometry/art retained, with a200/255 neutral tint to bring its brightness closer to the dark reference. Original goal frame now supplies the inner/outer goal enclosure and booster-section frame. Native corners use the existing protected mesh fitting where required.
- R10 independent environment/Ivan retained. Existing startup, navigation, saved progress and other pages unchanged. No full-screen reference image or scenery-contaminated rectangular crop enters runtime.

## Scenario and local preparation

Source: `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Fail.png`,941×1672; SHA256 `97c89cb094ddf6fe5e6a9baeab9630828b6fbaab46e4d11411a0a3119aba3ce3`.

The user's latest extraction-first instruction explicitly authorizes clean original-pixel isolation, superseding the earlier blanket prohibition on reference crops. The source remains unchanged. Crop coordinates, masks, requests and outputs are retained under Sources; final assets have new GUIDs.

**Actual billed total:62 creative units**, from14 completed Scenario jobs:

| Requests | Cost | Outcome |
|---|---:|---|
| Two GPT Image2 masked repair trials |26CU|Rejected: source artwork was redrawn/rearranged. No generated RGB from these outputs is used.|
| One Pixa atlas removal |3CU|Rejected: retained contaminated rectangular crop backgrounds.|
| Title plus ten individual Pixa removals |33CU|Used as mask sources with local inspection/corrections; heading required local opaque-face restoration.|

An earlier Photoroom text-removal call returned403 (model unavailable on plan), with no accepted job/output returned. It was not retried. Previous429 creative-unit blocking did not recur in these accepted jobs. No provider switch, automatic replacement submissions, account-balance claim or ongoing monitor.

[Per-job billing](QA/ScenarioCosts.json). Job requests/status and downloaded originals are retained under `Sources/Scenario/`. Pixa's documented operation: [Scenario background removal](https://docs.scenario.com/get-started/generation/background-removal-models/background-removal-models-pixa).

Scenario alpha was mapped back to each crop's native dimensions; original RGB was retained. Local original-surface texture cloning, fitted green badge surfaces and Poisson seam blending repaired the baked labels/counts and goal inset. These repairs are explicitly bounded, not described as exact recovery of hidden pixels. Disconnected neighbouring details were removed from masks, with panel surfaces kept opaque. The thin heading face needed an explicit local silhouette because automatic removal kept only its lettering. Medallion/nameplate overlaps in the flattened source limit recovery at the bottom edge; their layered runtime placement covers that join.

`Sources/PrepareSprites.py` and pinned `Sources/requirements.txt` reproduce staging and pixel audit. Its OpenCV mask copy is intentional: seamlessClone mutates its input mask. No OS font files were copied. Exact source font identity is still unknown.

## Files and preservation

Six existing files changed relative to the R11 start: Failure prefab, owner builder, Failure Runtime/View, and Failure Edit/Play test fixtures. Eleven PNGs plus eleven new meta files added under `Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/`:
`title_r11`, `retry_r11`, `paid_r11`, `ad_r11`, `nameplate_r11`, `goals_r11`, `heading_r11`, `mace_r11`, `firebird_r11`, `shield_r11`, `potion_r11`.

The1,843-file baseline audit found1,837 unchanged existing files. All952 prior meta files and all511 prefab object IDs remain; no new prefab objects. All108 prior R10 evidence hashes are unchanged. Final production ledger contains1,865 files. Prior assets/acceptance history were not removed. See `AppliedSourceChanges.diff` for this pass's changes independently of the pre-existing dirty working tree.

CurrentTask/SessionLog and FAIL-CONT-01/DevelopmentSchedule updated. Paid and ad continuation remain disabled design controls, scheduled for the post-UI gameplay/monetization stage; no actual purchase/ad/resume integration was included.

## Checks actually performed

- Confirmed Unity6000.5.9f1, Editor closed before launch, inspected workspace/local changes and required handoff/task/provenance instructions.
- Scenario API submission/status/download and numeric billing verification. Original pixels outside recorded RGB-repair masks unchanged for all eleven sprites; native dimensions retained. Inspected all eleven alpha resources over black, white and magenta.
- Actual Unity compile/import, Failure-only prefab rebuild and revision11 save/readback. Run1 and finalrun2 each passed10/10 EditMode and18/18 PlayMode tests, zero failures/skips, using isolated test saves/configuration.
- Existing Failure/Victory/Pre-Level fixtures cover page bindings, repeated build reuse, routes, localization, snapshot/live goal data, inventory, unavailable offers, text bounds, safe areas and raycast coverage. Updated assertions verify source booster bindings, single badge/icon layers and native offer aspect/price containment.
- Final run saved9 Failure,12 Victory and20 Pre-Level PNG/JSON pairs. Visually inspected final941RU, three-goal576EN and booster-guideRU, and earlier run1 tall576RU/three-goalEN/941RU. Not every saved capture was visually inspected.
- Production/GUID/object-ID/prior-evidence hashes, changed-C# whitespace, Python syntax and review link/option checks. See QA delivery records.

## Not performed

No APK/AAB build, Android install/device rendering/touch test, release/signing change, full project test suite, extended manual play/loss/animation session, profiling, browser rendering/dropdown interaction, live purchase/ad/provider sandbox test, exact-font identification or calibrated color/perceptual-blur measurement. No Unity/package/color-space/Android guard changes. Automated desktop captures do not establish phone acceptance.

Next: review the R11 portrait and remaining differences, then phone readability/touch verification. Further artwork work should isolate a specific remaining discrepancy rather than resubmit complete redesigns.
