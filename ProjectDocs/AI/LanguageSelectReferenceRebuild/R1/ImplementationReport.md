# Visual Page 29 — Language Select reference rebuild R1

The standalone selector contained a stale Splash copy with missing current Start-button lettering. Its local backdrop is now rebuilt from the approved revision9 Splash prefab. The selector also refreshes its modal hierarchy from current Settings and gives Russian, English and Close the same native press feedback. The actual Splash and Settings prefabs remain unchanged.

The view now explicitly validates the backdrop's current Splash bindings; the runtime refuses incomplete source prefabs. The owning builder updates the actual LanguageSelect prefab and cutover to revision2. Rebuilding affects the selector's local copies only, with no duplicate Canvas or EventSystem.

## Behavior and artwork

Russian/English selection uses the existing persisted language preference and leaves the selector open. Close and Back preserve return-to-origin behavior. The backdrop remains unbound, non-interactive and shielded from pointers. Viewing/pressing the controls does not change gameplay saves or spend resources. No language service or persistence implementation was replaced.

No new Scenario work was needed: **0 calls, 0 CU**. Existing Scenario-isolated Settings modal resources and approved current Splash resources are reused, with no PNG/font changes. Russian heading uses the original isolated lettering; choice labels and English heading remain live. The original reference shows the modal over Settings, while this previously approved standalone route intentionally uses Splash.

## Verification

Unity6000.5.9f1 rebuilt and read back the native prefab. Verify01 passed **4 Edit Mode + 10 Play Mode = 14 checks**, zero failures/skips, covering Language Select and Settings. New checks verify current backdrop sprites/bindings, unbound backdrop actions, modal-source sharing and reversible press feedback without geometry/language/save changes. Existing checks cover actual pointer selection, stored preference reload, Settings synchronization, modal blocking, safe areas, origin return and paused-board preservation.

Six actual standalone captures plus two Settings language-modal captures are shown alongside the original reference and the previous standalone page. The previous captures come from the immediately preceding Settings Verify03 regression run. All ten browser views load at430px without errors or horizontal overflow. Native RU and tallEN were visually examined.

Six existing Assets files changed: runtime, view, builder, prefab, cutover and Play Mode tests. No Assets files added/deleted. All5,615 protected existing art/font/meta/other-page prefab files remain unchanged, including Splash and Settings. Scoped source/cutover whitespace check passed; generated prefab YAML excluded. See QA/ScopeAndVerification.json and QA/IntegrityChecks.json.

No Android build, physical-device test, fresh-process persistence rerun, full test suite or performance profiling was performed. Existing persistence was exercised through its saved-preference/session reload checks.

## Next

Visual Page30 — Login / Account is next. Remaining language-modal fidelity differences concern live font weight/embossing and earlier repaired blank surfaces; these are documented in QA/ReferenceFidelityReview.md.
