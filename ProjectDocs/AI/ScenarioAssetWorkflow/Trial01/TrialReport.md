# Scenario trial 01 — empty booster medallion

2026-09-15. User authorization: “Proceed with one trial asset.” One generation request, one output, no production integration or broader batch.

## Result

- Original unmodified output: `booster_medallion_trial01.png`, 1024×1024 RGBA PNG.
- Provider: Scenario API; model `model_openai-gpt-image-2` (GPT Image 2), medium quality, `numOutputs: 1`, transparent-background option.
- Reported job billing: **12 compute units**, zero CU discount. This is the API's job cost, not an independently reconciled account balance or currency conversion.
- Exact prompt/settings: `Request.json`; model input definitions: `ModelSchema.json`; source reference/hash: `Reference.json`; job/output identity: `Job.json`, `Status.json`, `Output.json`; cost: `Billing.json`.
- Reference was the complete original `ApprovedScreens/Fail.png`, uploaded as style guidance. No screenshot region was extracted for runtime art. Scenario reused the existing asset identity for the identical uploaded source.

## Visual assessment

The generated frame has a clear circular silhouette, dimensional carved gold, a large blank navy face and separate transparent exterior. It contains no booster illustration, quantity, nameplate, text or scenery. Existing game booster artwork can be layered separately in a subsequent implementation.

It is a useful style trial, **not an exact reproduction or an approved production sprite**. The gold is brighter and more polished than the reference's subdued antique rim. Four sapphire ornaments are more prominent than requested; the top crest is larger and leaves less margin than the requested six percent. The navy surface is more visibly textured. A final row must be judged with the existing booster illustrations and live labels at phone size.

Alpha inspection found 44.202% fully transparent pixels, clear sampled exterior/corners, alpha range 0–254 and center alpha 252/255. Black/white/magenta composites show a clean exterior without the golden backdrop visible in RGB-only previews. The center is almost opaque but **not fully opaque**, so its subtle background dependence should be resolved before production acceptance. No alpha or color correction was performed on the downloaded asset. The three QA PNGs are diagnostic composites; they are not runtime sprites.

## API recovery and checks

Public model listing/schema lookup and reference upload succeeded. The public model-markdown URL returned an HTTP error; the authenticated model schema supplied the actual supported inputs, including transparency marked Preview.

The initial generation call was accepted remotely, but the helper expected the quick-start's `id` field rather than the live `jobId` response shape. Its exclusive submission marker prevented a second paid request. Job recovery initially found no literal prompt because Scenario stores the prompt as an asset ID; the reference was also deduplicated with an earlier upload. The unique job was identified by reference/model/output count plus its creation time within the single local submission window. Retrieved job status was success with exactly one output asset. The helper now accepts direct and nested `jobId` as well as nested `id`.

The first download stopped at the CDN's HTTP302. Download handling now permits HTTPS-only redirects on a separate **unauthenticated** connection; API authorization remains confined to the fixed Scenario API origin with redirects blocked. Download then succeeded. No second generation, background-removal request, upscale, badge or nameplate generation was made.

Performed:
- Existing task/handoff, root instructions, working-tree and source reference inspection.
- Python syntax and CLI-help checks; mocked one-output/reference binding, duplicate-submission refusal and job-status parsing.
- After recovery fixes: all three job-ID response shapes, duplicate refusal and HTTPS-only unauthenticated CDN redirect checks passed.
- Actual Scenario upload, one generation, job recovery/status, billing retrieval and PNG download.
- Original/reference visual comparison and black/white/magenta alpha-composite inspection; pixel alpha measurements retained in `QA/AlphaInspection.json`.
- All 1,799 recorded Failure R1 production hashes matched; no production file changed.

Not performed: new Unity import/prefab rebuild/compile/tests/captures, browser interaction with the review HTML, APK/device testing, exact calibrated color comparison, final phone-size composition review, account credit-balance reconciliation or any additional generation. User acceptance is pending.

## Next action

Review the trial's ornamentation and tone. If retained, resolve the slight center transparency and fit it with existing booster art before expanding the same style to badges and nameplates. Those follow-up assets are outside this one-generation authorization.
