# Visual Page24 — Friends List R1

The existing Friends List route now displays native Friends, Requests and Find tabs, with reference portraits/title/action/footer art, live account values, bilingual labels, local search and sorting. Social records and actions are explicitly examples. This is an initial UI implementation; online friends are not connected.

## Behavior

- Friends: six reference-inspired sample people with framed portraits, source level badges, guilds, illustrative online/offline status and Energy/Visit controls.
- Requests: five example requests with the original red/blue/green gift art and Accept/Decline controls. Find: local sample-name/guild filtering and an Add Friend control.
- Search matches Russian and English names/guilds case-insensitively, limits input to40 characters and renders literal text. Sort switches default/name order. Empty results and filtered counts are live. Tabs reset the search; sorting is retained within the page.
- Energy, Visit, Add, Accept, Decline and Remove Inactive open a clear unavailable-feature explanation. They do not mutate friends, balances or rewards. The overlay blocks underlying navigation; Back closes it first.
- Team Hub opens this page. Back returns to Team Hub. Profile, Settings, resource plus controls and World Map/Shop/Events/Characters footer buttons use existing routes; Friends returns to the first tab. Repeated binding/double taps commit routes once.
- Source portrait levels and the Events exclamation badge are fixed example artwork. No fabricated real friend capacity, pending count, reward grant, presence connection or server search is claimed. Page state is not persisted to the player save.

## Art / workflow

Two941×1672 originals: `friends.png` roster and `friend.png` requests, from `/home/k2/Downloads/match3final/`. Copies and extraction bounds are in `Sources`; hashes are registered in `References/REFERENCE_PROVENANCE.json`.

**Scenario:24 successful alpha-isolation jobs,72CU total; zero RGB generation.** The accepted jobs were polled/downloaded again after transient network failures; no paid job was resubmitted. All submitted-input hashes match immutable Request records.25 runtime PNGs include the24 isolated assets and an English blank-title derivative. [Scenario audit](QA/ScenarioAudit.json), [asset provenance](AssetProvenance.json).

Original source RGB is preserved outside recorded local repair masks. Local caption clearing, English title-face cleanup, opaque-interior restoration and neighboring-fragment alpha cleanup are reproducible in `Sources/Prepare*.py`. Russian title and fixed sample portrait levels retain original lettering. Native dynamic text uses selected embedded Roboto Serif600, with FreeSerif Bold for English title/footer/action captions. Exact source-font identity remains unverified. Source resolution limits sharpness;3× enlargement adds no original detail.

Reuses the existing clean Ranking background, shared account header, table/tab/panel surfaces, guild emblems and Back control. References are not runtime dependencies; no full screenshot or invisible-hotspot implementation.

## Production scope

New `ProductionFriendsListView.cs`, `ProductionFriendsListRuntime.cs`, `ProductionFriendsListLayout.cs` under `Assets/LegendsOfRus/Scripts/Runtime/UI/`; owning `Scripts/Editor/ProductionFriendsListPrefabBuilder.cs`.

Active prefab: `Assets/LegendsOfRus/Resources/ProductionUI/Pages/FriendsList.prefab`. Owner/view/cutover revision1 agree.25 sprites under `ProductionUI/FriendsListAligned/`; native CutoverState/FriendsListCutoverState.txt and Edit/Play Mode fixtures added.67 new Assets files including metas. Existing ScreenRouter.cs is the sole pre-existing production file changed: native binding, page-owned header and Back-overlay handling. The legacy OutOfEnergy demo-energy path is preserved.

[Scope audit](QA/ScopeAudit.json): no deletions; all other existing Assets, metas/GUIDs, Packages and ProjectSettings match task-start hashes. Gameplay, Splash and other page prefabs/art remain unchanged. No duplicate Canvas/EventSystem/startup path.

## Checks performed

Unity6000.5.9f1 batch compilation, scoped prefab rebuild and serialized readback passed. **6/6 Edit Mode +16/16 Play Mode tests passed**,22 distinct checks across Friends List, Team Hub and Team Chat; zero failures/skips. Commands, logs and XML are retained in `UnityVerification`. [Test list](QA/TestSummary.json).

Friends checks cover bindings/dependencies, idempotent construction, live/max balances, RU/EN font fitting,941×1672,576×1280 and1080×2400 layouts with simulated safe areas, background coverage, row separation, button-center raycasts, local name/guild search, sort, empty state, all tabs, modal input guards, navigation/double taps/rebinding, Team Hub entry and save invariance.12 actual Friends List captures plus18 neighboring-page captures are retained.

The initial run passed21/22: a request-button raycast was attempted in the same frame as a language-triggered row rebuild. Captures now wait for native graphics/button-state settling before checking all raycasts; no hit-test assertion was removed. Footer text extended beyond its actual plaque faces despite fitting its Text rectangles; those rectangles were narrowed before the final successful run. English title inpaint color bleed was reduced within its declared repair mask.

Original/native RGB equality outside repair masks, submitted Scenario input hashes, runtime PNG hashes, black/white/magenta alpha inspection, task-start scope comparison, source syntax and scoped whitespace checks performed. Browser review evidence: `QA/BrowserReview.json`.

## Not performed / feature phase

No APK build, physical-phone or Android keyboard/IME validation, performance profiling, full-project/all-pages test suite, calibrated pixel/color equivalence or live social/backend test. Simulated safe areas do not establish device acceptance.

After UI review: implement authenticated friend identities and roster pagination, server search/invitations, accept/decline/remove operations, truthful presence, player visits, authoritative energy gifts/cooldowns/limits and failure/offline/retry states. Test server permissions, idempotency and reconciliation before enabling transactions. No contacts or messages were sent during this visual task.

Next: review the standalone R1 page and its Requests/Find states at phone scale. [Known reference differences](QA/ReferenceFidelityReview.md).
