# Legends of Rus — project instructions

## Start here
Read `ProjectDocs/AI/ConversationHandoff.md` and `ProjectDocs/AI/CurrentTask.md` before work. Inspect the current local files and working-tree changes; this handoff is historical context, not proof that every earlier patch is installed. Read relevant nested project instructions too.

## Project and approved baseline
This is the existing Unity Android game «Легенды Руси: Три в Ряд», not a new project. The reported Editor is 6000.5.9f1; verify ProjectSettings/ProjectVersion.txt. Preserve Assets/LegendsOfRus/, existing startup/services/routing, GUIDs, .meta files, working local settings, and user edits.
The user accepted Splash/Title after ColorFix06 + PrefabFix07. Do not redesign, recolor, resize, regenerate, or rebuild that approved screen unnecessarily. Reuse its actual logo and visual materials for the Loading alignment.

## Art and UI constraints
Use only the approved Russian-fairy-tale family: dark navy/black lacquer, antique-gold filigree, sapphire accents, controlled emerald buttons, parchment where appropriate, carved wood, painterly scenery, and coherent Cyrillic typography. No AETHERIA styling, generic substitute kits, or new logo variants.
A production page must use a clean environment plate, genuinely isolated sprites, live text, and interactive controls. Never use a full-screen reference image with invisible hotspots or screenshot-region crops contaminated by scenery. Preserve intentionally opaque panel surfaces. Inspect transparent resources over black, white, and saturated backgrounds before accepting them.
Keep labels localizable, preserve approved wording, and use Application.version for version text. Exact source-font identity remains unverified; do not relabel a fallback as exact. Reuse local font assignments and do not distribute OS/container font files.
For design review provide one standalone portrait page, not a collage posing as an implementation. Distinguish actual Unity captures from offline previews or generated images.

## Implementation and verification
Modify the real active prefab and its owning builder/configuration together; preserve serialized references and keep revision checks in agreement. No parallel startup system, duplicate Canvas/EventSystem, placeholder restoration, or editor scene operations during Play Mode.
Inspect existing services before changing progress, navigation, input, or loading. Do not introduce timers presented as actual progress. Do not upgrade Unity/packages, change backend/color space, or bypass Android approval guards unless separately authorized and justified by a verified blocker.
Use a narrow, reviewable change set. Keep generated caches (Library/, Temp/, obj/, Logs/) out of the patch. Do not read, print, commit, or package secrets, keystores, or passwords.
Run applicable checks when the local environment supports them. A successful source check is not a Unity compilation or runtime test; list precisely what ran and what did not. Do not claim a design/asset/build exists without examining it.
Update CurrentTask.md and SessionLog.md after each substantial task. Record actual changed files, tests and outcomes, approvals, unresolved issues, and next action. Do not erase earlier acceptance decisions.
