# Events — Footer Touch R5

The existing header and footer already follow the safe edges. The narrow-phone review revealed a small Back touch target: the 66×64 reference-pixel artwork occupies about 40×39 screen pixels in the 576×1280 capture. Back now accepts taps in the clear margin around the icon without resizing or recoloring it.

## Installed change

Added native Image raycast padding: 6 reference pixels on either side and 12 above, with no expansion below. The resulting 78×76 reference-pixel target occupies about 48×47 screen pixels in the narrow capture. It remains inside the safe area and clears the footer ornament. These are simulated screen-pixel measurements, not Android dp certification.

Runtime, owning builder, active prefab and cutover now agree at revision 8. The serialized prefab diff contains only the revision and Back raycast padding. Header, title, cards, Home scenery, footer sprite, typography and sprite metadata are unchanged. No new graphics, overlays, hotspots or generation requests were used.

## Verification

Unity 6000.5.9f1 rebuild/readback succeeded. All 8 tests passed: 2 EditMode and 6 PlayMode, zero failures/skips. Tests use isolated save storage.

Expanded assertions verify exact safe-edge clearance, complete aspect-preserved footer artwork, separation between Back input and the decorative strip, and raycast hits near the expanded target edges. The new test cycles all three categories in Russian and English on a narrow tall phone, returns from Event Detail on a larger tall phone, clicks above the visible Back icon through the native pointer-click handler, and verifies one Home route plus unchanged saved state. Existing live balances, empty states, double-click guards, native header navigation and reward-availability checks also passed.

Retained 19 native captures with simulated display/safe-area metadata. Ten review views load in Chrome at 430×900 without script errors or horizontal overflow. Seven matching R4 captures are pixel-identical. The eighth (narrow Russian) differs by seven pixels in header text, maximum channel difference 1; no footer or card changes appear in that comparison.

Scope: six existing Assets files modified; 7,170 unchanged; none added or deleted. Exact paths and results are recorded in QA/ScopeAndVerification.json. No artwork or fonts changed.

## Limits and next step

The finite four-card list leaves space above the footer on tall screens. Existing approximate typography, reconstructed scene details and source-frame differences remain. Upcoming previews and example rewards remain clearly labelled; there is no invented event schedule or participation service.

No Android build, physical-device, backend, performance or full-project suite was run. Next recommended page: Event Detail responsive and footer audit.
