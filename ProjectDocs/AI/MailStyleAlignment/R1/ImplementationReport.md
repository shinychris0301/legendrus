# Visual Page25 — Mail / Inbox R1

Native Mail replaces the legacy ClaimAll/Back hotspots. Original title, senders, attachment icons, arrow, green claim frame and footer controls are isolated sprites; labels/actions are native Unity UI. Inbox, Sent, System and opened-letter views are implemented. Account balances and level remain live; messages and rewards are explicitly examples.

## Behavior

- Inbox contains five sample letters; System filters the system example; Sent displays an honest empty state. No outgoing message is fabricated.
- Three example unread markers and tab badges update locally when those letters open. The read state lasts only while this page instance exists and is never server presence or saved mailbox state.
- Five row controls open full letter details. Back returns to the list before leaving Mail. The list includes a compact selected-letter preview. Envelope resets to Inbox.
- Claim, Claim All, Compose and Delete open an unavailable-service explanation; they do not grant attachments, mutate currency or remove/send messages. The explanation blocks underlying controls, and Back closes it first.
- Source-inspired attachment examples:200 gems;5 potions; guild letter5,000 coins/100 gems/one hammer; system letter without attachments;3 books. These are illustrative, not balances or promised rewards. The list shows up to two attachment icons; the full letter shows all three guild attachments.
- No invented delivery times, expiry timer, mailbox capacity, update-version claim or live unread count. Russian/English labels use the chosen embedded fonts. Routes to Profile, Settings, Shop, Energy and Home are preserved and commit once.

## Reference and Scenario workflow

Originals: `/home/k2/Downloads/match3final/mail box.png` and `mail open.png`, both941×1672. Copies, source bounds/hashes and extraction assessment are in `Sources`, with registry entries in `References/REFERENCE_PROVENANCE.json`.

**20 successful Scenario alpha-isolation jobs,60CU total; no RGB-generation jobs.** Two tight hammer crops were rejected because the handle/pommel was clipped; a third complete crop replaced them. An incomplete sender download was fetched again from its existing job without another submission.18 accepted isolations plus an English blank-title derivative produce19 runtime PNGs. [Cost/job audit](QA/ScenarioAudit.json), [asset provenance](AssetProvenance.json).

Scenario provides alpha; runtime RGB retains original pixels outside declared local repairs. Title/button lettering is cleared for live labels. Writing areas use a lighting-normalized mirrored clean-paper donor from the opened-letter reference. Overlapping title/footer-button fragments on the letter frame are repaired from the same frame's clean border. Original submitted Scenario input files remain immutable and hash-verified; post-isolation local repairs have explicit masks. Portrait interiors and panel faces remain opaque. Source enlargement adds no missing detail.

Shared approved account header, clean Ranking scenery, list/tab surfaces, Back and native raised unread badges are reused. Body uses selected embedded Roboto Serif600; English title/claim text uses FreeSerif Bold. Exact source typeface identity remains unverified. Russian title retains its original lettering. No full reference screenshot or scenery-contaminated region is used as runtime UI.

## Production files

New `ProductionMailView.cs`, `ProductionMailRuntime.cs`, `ProductionMailLayout.cs` under `Assets/LegendsOfRus/Scripts/Runtime/UI/`; owner `Scripts/Editor/ProductionMailPrefabBuilder.cs`. New active `Resources/ProductionUI/Pages/Mail.prefab`, MailCutoverState and19 sprites in `ProductionUI/MailAligned/`. View/builder/cutover revision1 agree. Two new test fixtures in the existing Edit/Play assemblies.

Only pre-existing production file changed: `ScreenRouter.cs`, to bind the native page, omit the duplicate shared HUD and handle Back through letter/overlay states.55 new Assets files including metas. [Scope audit](QA/ScopeAudit.json): no deletions; all other existing Assets, metas/GUIDs, Packages and ProjectSettings match task-start hashes. Gameplay, Splash and other page assets/prefabs preserved. No duplicate Canvas/EventSystem or startup system.

## Verification performed

Unity6000.5.9f1 compilation, scoped prefab rebuild and serialized readback passed. Broad regression: **6 Edit Mode +18 Play Mode**,24 distinct tests across Mail, Friends List and Team Chat, all passed. After final source-frame/hammer/unread/layout refinements, focused Mail **2 Edit Mode +6 Play Mode** passed again, zero failures/skips. Exact commands/logs/XML are in `UnityVerification/Regression` and `UnityVerification/Final`; [test names](QA/TestSummary.json).

Mail checks cover bindings/dependencies, idempotent construction, live/max account values, RU/EN glyph fitting,941×1672,576×1280 and1080×2400 displays with simulated safe areas, full background coverage, button-center raycasts, all tabs, empty Sent, all five opened letters, correct no-attachment state, local unread updates, layered Back behavior, claim/compose/delete guards, save invariance, routing/double-tap protection and rebinding.16 final Mail captures plus22 neighboring-page captures retained.

Visual inspection caught defects beyond the automated layout checks: cropped hammer ends, reused parchment mismatch and nine-slice inflation of the original frames. Final frames render without that inflation; text/portrait/attachment margins were adjusted. Alpha inspected on black/white/magenta. Original RGB outside local repair masks, submitted-input and runtime hashes, task-start scope, source syntax and scoped whitespace verified. Browser results are in `QA/BrowserReview.json`.

## Not performed / next feature stage

No APK build, physical-phone test, performance profiling, full project/all-pages suite, live mailbox/backend test or calibrated pixel/color equivalence. Desktop Unity and simulated safe areas are not device acceptance. Actual Home-to-Mail tapping was source-inspected; focused Mail tests enter via the real router and test Back to Home.

After UI acceptance: authenticated mail list/detail, pagination and unread persistence, server timestamps/expiry, attachment definitions, idempotent authoritative claim/claim-all and inventory delivery, deletion policies, send/compose rules, offline/retry/error handling. Keep claims unavailable until these are integrated and verified. No messages or rewards were sent during this task.

Next: user review of the standalone R1 inbox and opened letters. [Remaining reference differences](QA/ReferenceFidelityReview.md).
