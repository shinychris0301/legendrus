# Visual Page23 — Team Chat R1

Native Guild Chat now opens from Team Hub. Original portraits, title, guild chest, parchment bubble, composer, send frame and footer controls are separate sprites; texts and actions are native Unity UI. Account balances/level remain live. Eight reference-inspired messages are labelled samples. Typed messages are local-session previews, never network messages.

## Actual behavior

- Scrollable masked history; all eight sample rows fit initially. New messages scroll to the bottom.
- Editable multiline composer,240 UTF-16 character limit, blank-message rejection and literal text (rich-text parsing disabled). Original Cyrillic messages and English translations are live labels.
- Send adds exactly one message, clears/deactivates the input, records the existing demo counter and keeps at most20 local messages. Local history survives page navigation, resets with runtime/session reset, and never writes chat contents to player saves.
- Profile, Settings, Shop, Energy and Back use existing routes. Repeated binding/clicks do not duplicate route commits. Back returns to Team Hub; it first closes any open explanation overlay.
- Chest, Gifts, Help Requests, Stickers and individual Help buttons show an honest unavailable-feature explanation. No energy, currency, reward or guild-membership changes occur. Composer cannot send behind the overlay.
- Sample timestamps and baked green portrait dots/Help badge3 are illustrative source art, not real presence or pending-request counts. New local messages use the device's local HH:mm time.

## Source and Scenario workflow

Dedicated source: `ChatGPT Image Aug 25, 2026, 09_00_51 PM (4).png`,941×1672. Original copy/hash/bounds are in `Sources/Assessment.json` and the project reference registry.

**17 successful Scenario alpha-isolation jobs,51CU total**, including three rejected tight footer crops. Their clipped rims were replaced by complete footer-button assemblies. No RGB generation. Scenario masks are applied to source RGB, with documented local text removal, face restoration and neighboring-fragment cleanup.16 runtime sprites:14 accepted isolations plus an English blank-title variant and the composer's own smile extracted as a separate native button graphic. Three submitted footer inputs were archived and verified against immutable request hashes after the working face repairs changed. [Job/cost audit](QA/ScenarioAudit.json), [asset provenance](AssetProvenance.json), [submitted-input audit](QA/SubmittedInputAudit.json).

Body/names use the selected embedded Roboto Serif600; live English heading and footer captions use selected FreeSerif Bold. Exact reference typeface identity remains unknown. Russian fixed title retains original lettering. Existing clean Ranking scenery, account controls, guild crest, table/panel surfaces, Ivan portrait and Back asset are reused. No OS fonts copied. No full screenshot or scenery-contaminated crop is runtime UI.

## Changed production files

New native owner/view/runtime/layout:
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatView.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatRuntime.cs`
- `Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionTeamChatLayout.cs`
- `Assets/LegendsOfRus/Scripts/Editor/ProductionTeamChatPrefabBuilder.cs`

New active prefab `Assets/LegendsOfRus/Resources/ProductionUI/Pages/TeamChat.prefab`, revision1, and corresponding `CutoverState/TeamChatCutoverState.txt`.16 PNGs under `ProductionUI/TeamChatAligned/`, with new metas. New TeamChat Edit/Play Mode fixtures in the existing test assemblies.

Only pre-existing production file changed: `ScreenRouter.cs`, for native binding, page-owned account header and Back-overlay handling. [Scope audit](QA/ScopeAudit.json) lists49 new Assets files, zero deletions. All other pre-existing Assets, metas/GUIDs, packages and ProjectSettings match task-start hashes. Gameplay and other page assets/prefabs remain unchanged. Legacy demo method remains unused; no parallel startup/Canvas/EventSystem added.

## Verification performed

Unity6000.5.9f1 batch compilation, native prefab rebuild and serialized readback passed. The broad verified run passed **6 Edit Mode +15 Play Mode** checks across Team Chat, Team Ranking and Team Hub. After the final footer-only typography adjustment, focused Team Chat **2 Edit Mode +6 Play Mode** checks passed again. Latest results across21 distinct tests are passing, zero failures/skips. Commands, XML and logs are in `UnityVerification/Final` and `UnityVerification/Regression`.

Tests cover bindings/dependencies, idempotent construction, live account values, RU/EN layouts at941×1672,576×1280 and1080×2400 with safe areas, background coverage, visible glyph fitting, row separation, accessible button raycasts, local send deduplication/literal content/blank rejection,240-character input,20-message retention, navigation persistence, long-message bubble fitting/scroll-to-bottom, modal input guards, save invariance, Team Hub entry and maximum balances. Ten final Team Chat captures plus eight Team Hub/nine Team Ranking regression captures retained.

Initial checks exposed the reused header's missing font binding and insufficient long-message bubble height; both were fixed. The capture test now excludes intentionally culled offscreen text from glyph checks. Footer captions were fitted into their actual plaque faces; stale neighboring art fragments and a footer-ornament overlap were removed. Rejected footer crops and earlier results remain historical evidence in the session workspace.

Source/native RGB equality outside recorded repair masks,16 runtime PNG hashes, black/white/magenta alpha inspection, source Python syntax and scoped whitespace checks passed. Final captures are reviewed against the original; browser review results are in `QA/BrowserReview.json`.

## Not performed / next feature stage

No APK build, physical-phone or Android soft-keyboard/IME test, device performance profiling, full-project/all-pages test suite, calibrated color comparison or online chat/backend test. Batch captures simulate portrait displays and safe areas. Production real-time messaging still requires authenticated guild membership, transport/reconnect/history, rate limits, moderation/reporting, presence and server-authoritative help/gift/chest transactions. Sticker delivery is also deferred. No messages were transmitted externally during this task.

This is an initial visual implementation, not an exact replica. [Remaining visual differences](QA/ReferenceFidelityReview.md). Next: user review of R1; then physical-device keyboard/scroll validation before treating chat interaction as Android-ready.
