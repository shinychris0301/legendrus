# Events R3 — Shared Home Background, Scenery and Glow

The active Events page now uses **the exact Home background**, as the user requested during this pass. The separate title skyline is disabled; the existing Home sprite uses white tint and centered cover. The four event cards retain their own themed artwork.

## Implemented

Four independent scenery plates follow the original visible architecture and framing more closely. They combine original source scenery outside recorded repair masks with Scenario reconstruction where foregrounds obscure it. Existing Firebird, Baba Yaga, Merchant and Knight sprites were not regenerated or modified. Foreground and scenery share coordinate registration.

Added independent Firebird spark/flame and soft halo layers. The former uses original RGB and a bounded emission alpha excluding the subject core, UI corners and architectural/sunset contamination. The halo is explicitly reconstructed from source emission/core alpha. Layer order is scenery → halo → original subject → sparks; effect Images do not intercept input and remain clipped by the rounded card stencil.

Preserved the original Russian title, shared player header, R2 frames/reward sizing and event preview behavior. Runtime, owning builder, active prefab and cutover now agree on internal **revision6**.

## Scenario and provenance

Two generation jobs were accepted before the user's Home-background instruction: card scenery **12CU** and heading skyline **12CU**, total **24CU**. The skyline result is retained only as workflow evidence; **it is not installed or referenced at runtime**. No repeat generation was submitted. The card reconstruction was accepted for the obscured regions. Existing source masks were reused for local extraction/composition.

[Requests, jobs and billing](Sources/Scenario) · [Source/repair provenance](QA/AssetProvenance.json) · [Protected resources](QA/SourceIntegrity.json). Four source subjects, title and Home environment remain byte-identical. The Firebird scene has a large repair footprint because both the bird and surrounding flames obscure the background; source pixel preservation is reported per plate, not as a whole-page fidelity percentage.

Alpha was inspected over black, white and magenta. The first local spark mask retained broad sunset/architecture fragments; those were excluded before installing the final layer. [Final effect alpha](QA/FirebirdAlpha.png). [Layer-composite diagnostic](QA/LayerComposites.png) is an offline asset check, not a native page capture.

## Verification

Unity6000.5.9f1 Verify01 import/rebuild succeeded; **4 EditMode +13 PlayMode =17passed**, zero failed/skipped. Added checks compare Events' background sprite/tint directly with the active Home prefab, require the old title skyline to be inactive, and verify common scene/subject/effect registration and effect draw order/input safety. Existing coverage includes native bindings, repeat-build idempotence, RU/EN label fit, standard/narrow/tall safe-area layout, balances, raycasts/navigation, preview/empty categories and unchanged save/claim state. Shop/Out of Energy regressions also passed.

Eight actual Events Unity captures are included; standard RU and narrow EN were visually inspected. Browser review verification checked10 views and the comparison toggle, with no script errors or horizontal overflow at430px. [Final Unity evidence](QA/Verify01) · [Browser evidence](QA/BrowserChecks.json).

Seven existing Assets files changed:5C#files, Events prefab and cutover. Six PNGs plus their metas were added; no deletions or existing asset-art replacements. Home/shared header resources and other page prefabs were unchanged. [Exact scope](QA/ScopeAndVerification.json).

No Android build, physical-device test or full-suite run. [Remaining visual differences](QA/ReferenceFidelityReview.md) include selected-tab texture, live typography, fine carving and footer surround. Event scheduling/participation/reward delivery remain unimplemented; preview labels remain honest.

[Open the native page review](Events_SceneryAndGlow_R3_Review.html).
