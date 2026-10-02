from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
path=ROOT/'ProjectDocs/AI/AndroidGameplayProfile/R1/Usage.md'
text=path.read_text()
text=text.replace('Select one command at **cold process launch**, before any game state loads:', 'Select one command at **cold process launch**, before any game state loads. The installed Editor/source now supports the six fixed presets below; the current R3 APK still supports only `fresh`, `resume` and `empty-energy` until the separate guarded refresh.')
text=text.replace('| No command |', '| `stock-999`, `stock-1000`, `stock-9999`, `stock-max` | Replace owned QA state with round 1/default balances, energy 4 and exactly 999, 1 000, 9 999 or 2 147 483 647 of each of the four boosters. Installed Editor/source only until APK refresh. |\n| `story-early` | Eight synthetically completed rounds, 24 stars, intro and two acknowledged milestone scenes; selected/unlocked round 9, energy 4. Installed Editor/source only until APK refresh. |\n| `story-finale` | All 500 rounds synthetically completed, 1 500 stars, all 58 scenes and Chapter 1 acknowledged; selected/unlocked round 500, energy 4. Chapter 2 remains unavailable. Installed Editor/source only until APK refresh. |\n| No command |')
text=text.replace('To repeat a fixture, end the QA process and explicitly launch `fresh` again.', 'To repeat a fixture, end the QA process and explicitly launch its reset command again.')
text=text.replace('`LOR_GAMEPLAY_QA=fresh`, `resume` or `empty-energy` is read when entering Play Mode.', '`LOR_GAMEPLAY_QA` accepts `fresh`, `resume`, `empty-energy` and the six exact preset names above when entering Play Mode.')
text=text.replace('Only `fresh`/`empty-energy` resets QA data.', 'The reset commands (`fresh`, `empty-energy` and the six fixed presets) replace QA data; `resume` preserves it.')
start=text.index('The packaged command set has no stock-quantity')
end=text.index('\n## Storage boundary',start)
text=text[:start]+'''The current R3 APK has no fixed-stock presets yet. Installed Editor/source now supports the four exact stock names, with no arbitrary quantity, JSON, path or save-import command. Use `stock-1000` for the native 1 000→999 Sea Pearl boundary, then `resume` to check durability. Stock fixtures have no completed rounds or acknowledged scenes. All presets reset language/consent as fresh QA state; ordinary language controls remain available.

`stock-max` is restricted to display/consumption testing. Existing ordinary first-clear reward additions do not guard booster overflow; avoid reward-producing actions with this maximum fixture. This implementation does not change the normal economy or introduce a stock-cap policy. Exact-count Editor rendering/input tests and cold persistence do not establish physical legibility or Android activation.
''' +text[end:]
start=text.index('Optional recap is reached through')
text=text[:start]+'''Optional recap is reached through Profile → History → Story. Eligible scenes must be acknowledged and milestone eligibility requires its completed round. Installed `story-early` and `story-finale` create bounded synthetic ledgers matching the independently rehearsed R3 service outcomes, including ordinary first-clear balances/stock and claimed rewards; replay cannot claim those grants again. These are synthetic QA completions, not human play or earned phone evidence. The current APK cannot use these selectors until refreshed.

## Fixed fixtures — installed Editor/source, APK refresh next

[Implementation and checks R4](../R4FixedPresets/TestResults.md) installs the six bounded selectors in the existing profile. Candidates are prepared from validated active Chapter 1 content before owned-slot deletion. New presets reject a present foreign or empty ownership marker before mutation; no marker or the exact existing owner is allowed. The candidate commits through the existing QA SaveService journal before publishing ownership. Content/commit failures remain blocked for that process and never read normal data. Existing `fresh`, `resume`, `empty-energy` and no-command behavior is preserved, including the older fresh reset semantics.

The factory is compiled only for Editor or Android development builds. Android intent/package/debuggable guards and cold-process selection remain unchanged. Current reference DTO exports have no supported import route. A separately verified APK refresh and resumed device execution are still prerequisites for Android preset use; all physical rows remain pending.
'''
path.write_text(text)
