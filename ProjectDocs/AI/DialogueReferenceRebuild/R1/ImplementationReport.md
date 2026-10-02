# Story Dialogues — Reference Rebuild R1

Completed Visual Page 35 after the user's continuation from Error Dialog. This is the real native Dialogue page, not a screenshot with hotspots.

## Changes
- Restored the original Russian ПРОДОЛЖИТЬ button pixels and ДОБРЫНЯ nameplate lettering from the existing approved Dialogue reference.
- Dobrynya lettering appears only for `speaker.dobrynya` in Russian. Ivan, other speakers, English and inactive-story states retain live labels. Dialogue body text remains the existing localized story catalog, not the reference sample paragraph.
- Added the established native press tint. Pointer exit restores the button without advancing the story; inactive story keeps Continue disabled and dimmed.
- Updated the runtime, owning builder, active Dialogue prefab and cutover to revision 2. Preserved character art, hall/forest environments, repaired parchment, source alpha, layout and story routes.

## Artwork provenance
Reference: `Sources/Reference.png`, copied exactly from `DialogueStyleAlignment/R1/Sources/OriginalReference.png`. Source SHA-256: `c88e0b0978108d0e09f1671fd6a15929678c3013e1491988fa5e1a4670431e9a`.

`Sources/ExtractLettering.cjs` restores original RGB within the nameplate face and complete button crop, using the existing accepted silhouette alpha. Two new sprites, no existing artwork overwritten. All panel pixels outside the bounded name face are unchanged. Black, white and magenta composites were inspected. Zero Scenario jobs, zero generation calls/credits.

## Verification
- Baseline01: 3 Edit Mode + 4 Play Mode tests passed before edits.
- Verify01: Unity 6000.5.9f1 imported, compiled and rebuilt the active prefab; 3 Edit Mode + 5 Play Mode tests passed, none skipped or failed.
- Checks include native bindings, screenshot-free dependencies, builder idempotence, every Chapter 1 story line in RU/EN, single-step advancement, milestone completion/return routes, disabled/rebound controls, source-name switching and cancelled press behavior.
- Eight actual Unity Play Mode captures include RU/EN, tall safe areas, Ivan, Firebird, finale and pressed button. Saves/config isolated from the user's data. Native RU, tall EN and Ivan captures inspected visually.
- Chrome checked all ten review views at 430px with no JS errors or horizontal overflow.
- Source RGB/alpha/outside-patch assertions passed; 5,671 protected existing art/meta/font/other-page files unchanged. Six existing Assets files modified, two PNG/meta pairs added, none deleted. Exact list: `QA/ScopeAndVerification.json`.
- Scoped `git diff --check` passed after removing whitespace from three generated empty YAML name fields; this formatting-only cleanup does not alter serialized values.
- No Android build, physical-device check, backend verification or full test suite was run.

## Limits and next step
The screen is closer, not pixel-identical. The reused generated hall differs in lamp/tapestry/table details, live body typography is heavier, the native divider is simplified and repaired parchment has more regular texture. Other speaker artwork remains limited to the established Ivan/Dobrynya/Firebird set; narrator/elder/raven scenes use scenery without a falsely identified portrait. See `QA/ReferenceFidelityReview.md`.

Next page in sequence: Visual Page 36 — Characters / Hero Collection, after user continuation. If Dialogue needs another fidelity pass, prioritize body typography and the source divider.
