# Friends List R2 — Joined Tabs, Guild Flags and Outer Frame

Restored the connected tab rails and central sapphire, replaced the generic guild shields with five original hanging flags, and fitted the original curled outer frame and right sapphire rail around the list. The red flag is reused for both members of the same guild. The active tab remains sapphire blue, and all three tabs use live localized labels.

The search field is slightly shorter and moved below the joined strip. Guild labels were inset to clear the larger flags. Existing shared header, saved balances, source-lettered action/footer controls, row spacing, Home scenery, search/sort behavior and honest social-action previews are retained. No external messages or invitations were sent.

## Source workflow

Three Scenario background-removal requests succeeded: one sheet containing five flags, the prepared tab strip, and the prepared outer enclosure. **9 CU total, no resubmissions.** No new RGB artwork was generated. Existing approved blue tab material is reused for selected states.

Scenario retained some flag backing, so local metal-outline masks replace the unusable backing matte. The winter flag's top finial was recovered from an expanded original crop. The original flag RGB is retained. Navy panel surfaces, the badge-covered tab corner and interrupted rails were repaired from source donors. Returned panel alpha received bounded restoration to preserve intentionally opaque faces and thin borders. These repairs remain approximate.

Twelve new sprites and metas are added: five flags, six tab states and one outer frame. [Provenance](Sources/AssetProvenance.json), [Scenario audit](QA/ScenarioAudit.json), and [black/white/magenta inspection](QA/AssetAlpha.png) document the process. Earlier approved sprites are preserved.

## Native implementation

- `ProductionFriendsListRuntime.cs`: connected tab geometry, original flag assignments and clearance, shorter search field, source outer frame.
- `ProductionFriendsListView.cs`: revision3 and per-tab normal/selected sprite switching.
- `ProductionFriendsListLayout.cs`: outer frame expands to the same safe-bottom controls while keeping source corners.
- `ProductionFriendsListPrefabBuilder.cs`: source frame slicing/import settings, updated verification marker and rebuilt active prefab/cutover.
- Existing Play Mode assertions now check gap-free tabs, tab/search separation and flag/label clearance.

## Verification

See [scope and final results](QA/ScopeAndVerification.json). Verify01 passed all26targeted tests; visual alpha/native review then caught a hollow purple flag and stale tab-bottom residue in the outer frame. Both were repaired locally before the final Verify02 rebuild and captures. The service was not called again.

Final Verify02: Unity6000.5.9f1 rebuild/readback,10EditMode+16PlayMode=26passed,zero failures/skips. Checks use isolated saves and include Friends List, Team Hub and Team Chat. Native Russian, English and Requests captures are provided in the standalone review, alongside both original references and the previous R1 capture. All15browserviews and comparison toggle passed with no script errors or horizontal overflow. Scoped C# whitespace checks passed. No Android build, physical-device/keyboard, performance, full-suite or live-backend testing was performed.

See [remaining differences](QA/ReferenceFidelityReview.md); this pass does not establish full pixel identity.
