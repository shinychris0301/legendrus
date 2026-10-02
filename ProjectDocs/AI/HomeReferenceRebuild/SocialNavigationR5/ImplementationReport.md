# Home R5 — Ranking, Team and Friends entry points

The active Home prefab had twelve controls, with no visible Ranking, Team or Friends buttons and no destinations for those pages in its navigation bindings. Team Hub had routes to Friends and Team Ranking, but Home did not expose those entries directly.

Home now has three visible, labelled shortcuts along the left side of the scene, below the logo and above Play:

| Icon | Russian / English label | Destination |
| --- | --- | --- |
| Trophy | РЕЙТИНГ / RANKING | Individual Ranking |
| Eagle crest | КОМАНДА / TEAM | Team Hub |
| People | ДРУЗЬЯ / FRIENDS | Friends List |

Team Ranking and Team Chat remain accessible inside Team Hub. The existing Home controls retain their source geometry and routes. The new shortcuts use real Unity Buttons, localized labels and the existing press/audio/haptic feedback adapter. Repeated activation commits one navigation action. Friends opened directly from Home returns to Home; Friends opened from Team Hub retains Team Hub as its Back destination. An open Friends explanation closes before navigating back.

## Asset reuse and implementation

Four approved sprite resources are referenced directly: Team Hub's navy frame and trophy, Team Ranking's eagle crest, and Out of Energy's people icon. No new images, copied assets, importer changes or Scenario calls are needed. Loading's measured Home resource list includes all four resources, avoiding new untracked loads at Home entry.

Updated the owning source composition/runtime/view, Home-only prefab builder verification message, real Home prefab at revision 19, and the router's transient Friends return target. The existing main navigation, shared header, scenery, logo, hero and Play composition are preserved.

## Verification

The first batch attempt was blocked because the Unity editor was open. The user then closed Unity and authorized continuing. Final results and actual Unity captures are recorded in QA and the standalone review. No Android/device, performance, full-suite or live social-backend verification is claimed.

Final Verify03: Unity 6000.5.9f1 prefab rebuild, 8 Edit Mode and 26 Play Mode tests passed (34 total; zero failures/skips). Verify02 found one outdated resource-count assertion (27 versus 31), corrected before final verification. Six browser review views passed. Native RU and tall EN/RU inspected. Eight existing Assets files changed; no Assets added/deleted and all existing images/metas/other-page prefabs preserved by SHA. Scoped C# whitespace check passed; Unity-serialized prefab contains its usual empty-value trailing spaces. User subsequently requested closer Home-style icons; continue with a separate art revision.
