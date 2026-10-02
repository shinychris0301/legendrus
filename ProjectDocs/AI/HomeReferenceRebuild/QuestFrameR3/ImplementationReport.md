# Home gem-collection frame R3

The missing section was baked into both quest-panel PNGs. R1's rectangle used to remove the sample progress count extended through the lower gold rail, creating a black notch. Its empty-track replacement also left a thin remnant of the original blue fill.

Restored the original track and gold contour from `3.home.png`, then rebuilt only the empty interior from its own unfilled dark-brown texture. A bounded color mask clears remaining cyan edge pixels. The live progress label continues to show unavailable data as dashes; no fictional quest progress was added.

Only `source_quest_ru.png` and `source_quest_blank.png` changed. Sprite dimensions, all alpha pixels and all pixels outside the 380×53 repair rectangle are unchanged. The existing active prefab and owning configuration already reference these textures; revision17 and GUIDs remain valid, with no layout or code changes required. Badges and all other screens remain unchanged.

## Verification

Unity6000.5.9f1 import/compilation and **6EditMode +12PlayMode checks passed**, zero failures/skips. Existing checks cover prefab/config agreement, sprite dimensions, native interactions, daily-reward badge behavior, RU/EN switching and responsive captures. Final Russian full-size and English tall-safe captures inspected, including an enlarged lower-panel crop. Black/white/magenta alpha inspected. Earlier Verify01 passed functional checks but exposed a faint blue edge during visual inspection; final Verify02 follows its cleanup.

[Scope and pixel audit](QA/ScopeAndPixelAudit.json) · [Test summary](QA/TestSummary.json) · [Repair recipe](Sources/Repair.py) · [Provenance](Sources/RepairProvenance.json).

Existing Scenario/source extraction reused; zero new API calls or credits. Source border restored, empty interior locally reconstructed. No full-project suite, browser automation, APK or physical-device check. Reference comparison is for the panel artwork; actual save values and unavailable quest data intentionally differ.
