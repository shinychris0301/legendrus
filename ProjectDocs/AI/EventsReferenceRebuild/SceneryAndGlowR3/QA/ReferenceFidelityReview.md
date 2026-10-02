# Events R3 — Shared Home Background, Scenery and Firebird Glow

The user approved the next scenery/glow refinement, then explicitly requested the **same background as Home**. That later instruction governs the page-wide environment.

## Updated

- Events references the exact Home `clean_environment` sprite with white tint and the same centered-cover behavior. The old separate skyline behind the title is disabled. This deliberately differs from the original Events skyline, following the user's instruction.
- Four card scenes now follow their original source crops: city silhouettes, Baba Yaga's hut/woods, merchant harbor/ship and icy castle. Existing character sprites are unchanged. Source RGB is retained outside documented repair regions; Scenario provides missing scenery behind foregrounds and near contaminated corner footprints.
- Firebird flame trails and sparks were separated from the original RGB into an independent masked layer. Architectural/sunset contamination was excluded during alpha inspection. A separate reconstructed amber halo supplies the softer bloom. Both use the same source coordinates as the preserved Firebird and repaired scenery.
- Source registration is shared across the scene and foreground, preventing architectural light fragments from drifting against the actor. Rounded card masks keep these layers inside the R2 frames.

## Comparison and limits

Original/R2/R3 Firebird details are in [the diagnostic comparison](SceneryAndGlowComparison.png). The native page now retains much more of the source flame silhouette, and the background structures are closer than R2. The hidden scenery is reconstructed, not exact original pixels; the Firebird repair covers most of its plate because flames occupy nearly the entire area. The source-preservation count does not imply that the whole scene is an exact match. Fine alpha and bloom intensity remain approximate.

The source Russian title, shared Home header, R2 frame repairs and reward layout remain intact. Current resources are still live. Upcoming/unconfirmed dates/example rewards and unavailable participation remain intentional.

Selected-tab texture, exact live typography, individual card carving and the footer surround still differ from the original. The new shared Home environment is an approved design choice and should not be reverted to match the old Events background.

Recommended next visual pass: **Events R4 — Tab Surfaces and Footer Ornamentation**, using the current shared Home background.
