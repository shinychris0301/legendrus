# Mail R5 — opened-letter title, outer frame and footer

Opened letters now use their dedicated original Russian title, sapphire crest and carved backing. The parchment extends toward the page edges as in the reference. Back and Inbox are the only two visible footer controls in the opened view; returning to the Inbox restores the existing composition and controls. The R4 paper surfaces are reused unchanged.

## Source artwork

One Scenario background-removal sheet request (3 CU; no resubmissions) isolates the original title and footer controls. The returned mask retained neighboring panels; bounded material/silhouette cleanup was required. Original Russian title and icon RGB are preserved beneath the cleaned alpha. The English title retains live text over a donor-repaired navy face. The intentionally opaque title backing uses the original right-side carving mirrored over the avatar-occluded left, approved navy under the independent title, and a source rail. This backing and hidden lettering surface are reconstructions.

Five new PNG/meta pairs. Sources/Sheet.json records original and staging bounds; Sources/Provenance.json records asset methods/hashes. The previous assets, corrected parchment and shared resources were not overwritten.

## Native behavior

Runtime, view, responsive layout, owning builder, active prefab and cutover agree at revision 6. The shared player header remains live. The title jewel overlaps the paper top rail; the information modal stays above it. Header/footer still use safe edges; paper stretches and attachments follow the bottom on tall displays.

Back dismisses an opened letter first; Inbox returns to the Inbox tab. Compose/Delete/Claim All remain available in the Inbox but are hidden while reading. The footer displays a localized sample/unavailable status instead of a fabricated expiry timer. Existing sample-read state and blocked claims are retained.

## Verification

Final Verify03 in Unity 6000.5.9f1 passed rebuild/readback, 2 EditMode and 6 PlayMode tests (8 total, zero failures/skips), producing 16 native Mail captures. All 20 browser views passed without errors or overflow. Seven existing Assets files changed and five PNG/meta pairs were added, with no deletions. QA/ScopeAndVerification.json records exact paths and results. Existing Mail PlayMode tests now also verify that the reading footer exposes Back/Inbox, returning restores the Inbox controls, and the state-specific title backing toggles correctly. Actual Unity captures and original/before views are labelled separately in the review.

No Android/device/full-suite/backend/performance test. Fine alpha contours, reconstructed carving and English face, live typography, approved shared rewards and Home scenery differ from the reference. Initial behavior runs passed, but visual inspection prompted title-backing seam, lower-rail clearance and sample-notice placement corrections before final verification.

Visible original RGB is unchanged in all three source-lettered/icon assets. Five checked Inbox captures are pixel-identical to R4; the narrow English capture differs by one shared-header rasterization pixel. R4 parchment hashes are unchanged. Evidence: QA/IntegrityChecks.json.
