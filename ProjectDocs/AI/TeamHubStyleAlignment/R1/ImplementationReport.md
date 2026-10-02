# Team Hub — Visual Page 21 / R1

Implemented a native Guild hub accessible from World Map → Guild and the existing Profile team shortcut. The page uses independent source-derived sprites and live UI; no full-screen reference or invisible-hotspot interface enters runtime.

## Source and decisions

The dedicated original was found among generically named images: `/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_00_51 PM (3).png` (941×1672; source SHA256 in Sources/Assessment.json). This resolves the initial missing-reference question. Historical handoff Splash instructions remain historical; the active approved task is Visual Page21.

Original banner, title, navigation/task icons and five portrait faces are retained. Scenario isolation produced alpha masks; original RGB was applied outside declared repairs. Russian title uses source lettering; English uses live FreeSerif Bold on a blank source plaque. Body/names use the user's selected Roboto Serif600 resource. These are selected matching families, not a verified identification of the source font.

Scenario recorded18 successful jobs,74CU:16 alpha-isolation jobs (48CU), one clean scenery job (13CU), one five-portrait lower-cap repair job (13CU). Two grouped atlases failed to isolate all components correctly; nine individual icon jobs replaced the rejected grouped icon/local segmentation drafts. The grouped portrait output was replaced by local exterior masks plus the bounded cap repair. Only missing lower portrait regions were composited; generated faces were discarded. Full outputs, request parameters, billing, source bounds and output hashes remain under Sources and QA/ScenarioAudit.json. No automatic paid retries occurred.

Twenty-one new runtime PNGs: original source sprites, repaired blank surfaces, five portrait composites and clean background. Panel label/icon footprints are cleared with source lacquer donors; the donation face receives an emerald tint within its recorded repair mask. English title glyph area is replaced with source lacquer. Original RGB outside repair masks was verified before interpolation. Alpha is estimated; enlargement cannot add source detail. Background is generated scenery and receives a0.68 runtime tint. Portrait lower-rim seams are not claimed to be exact restoration.

## Behavior and ownership

- `ProductionTeamHubView`, `ProductionTeamHubRuntime`, `ProductionTeamHubLayout`, `ProductionTeamHubPrefabBuilder` own the page; revision1 prefab and cutover state agree.
- `ScreenRouter` binds native controls, suppresses its legacy resource overlay for Team Hub, and closes the information overlay before Back navigation.
- Profile, Settings, currency Shop, energy, Team Chat, Team Ranking, Friends and Back routes work. Repeated clicks commit only once. Child destinations retain their existing implementation stages; this task does not implement online chat/rankings.
- Guild name,32/40 membership, power125840, season7560/10000, tasks/reward values and the five-member roster are explicitly labelled examples. Live account balances remain distinct.
- Task, donation and chest controls show an honest availability explanation. They spend no currency, grant no reward and mutate no save. Online membership, donation validation, task progress, chest claims, authoritative standings and chat are future feature work.
- No duplicate Canvas/EventSystem/startup path; no screenshot dependencies; safe areas constrain controls while the clean environment covers the canvas.

## Verification performed

Unity6000.5.9f1 native prefab rebuild/import and readback passed. Final4/4 Edit Mode +10/10 Play Mode tests passed: TeamHub2+4 and IndividualRanking2+6 regression checks. Isolated save storage used throughout.

TeamHub covers complete sprite/binding checks; screenshot-free dependencies; idempotent native build; live balance updates; RU/EN titles and labels;941×1672,576×1280 and1080×2400 with simulated safe areas; text-height and visible-glyph checks; row separation; full-background coverage; button-center raycasts; nine navigation destinations; double-click route guards; repeated listener binding; unavailable-action save invariance; information-overlay input blocking/Back dismissal; World Map entry; maximum balances; absence of duplicate legacy HUD.

Eight actual TeamHub captures and ten Ranking regression captures saved in UnityVerification. Visually inspected the original, clean environment, foreground assets over black/white/magenta, Russian portrait layout, English tall-phone layout and the explanation overlay. Browser review and scope audit are recorded separately in QA.

Initial failures were corrected: missing built-in progress sprite, chest-label overflow, duplicate legacy HUD found visually, and an invisible guild-power text line. Progress now uses native gradient geometry without ornament lines through the number. One interim test incorrectly assumed a one-character glyph required more than four vertices; corrected to require visible geometry. One final import process aborted in Unity Mono native pipe handling (`ToFileDescriptor` assertion); a fresh invocation rebuilt and passed all tests without package/settings changes. The crash log is retained in QA/ImportProcessFailure.log. Failed intermediate results remain in `/tmp/lor-teamhub-r1/attempt1` through `attempt4`; final evidence is under UnityVerification.

## Not performed / limits

No Android APK build, installation or physical-phone check. No full project/all-pages suite, real guild backend, chat network test, donation transaction, reward claim or membership flow. Scenario outputs and Unity captures do not prove pixel-exact fidelity. See QA/ReferenceFidelityReview.md for remaining differences. Gameplay, approved Splash, other page assets/prefabs, packages and ProjectSettings are unchanged against the task-start snapshot.
