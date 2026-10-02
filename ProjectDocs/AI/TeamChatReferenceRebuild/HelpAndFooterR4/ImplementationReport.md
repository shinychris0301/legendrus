# Team Chat R4 — Help Buttons and Footer

Restored the original Russian Help button, including its lettering, and the central sapphire footer carving. Both Help buttons remain native interactive controls that open the existing explanation of unavailable online help. English uses a live localized caption. The shortened local-only message count moves left to clear the ornament; the existing session limit and service behavior remain unchanged.

## Asset workflow

Two Scenario background-removal jobs succeeded: **6 CU total, no resubmissions**. Source RGB is retained for the Russian button and carving. The footer input excludes the neighboring plaque; returned alpha is refined to remove dark backing between scrolls. English uses the same border/alpha with a bounded repair of the hidden lettering area. Three new sprites and their metas were added. See [provenance](Sources/AssetProvenance.json), [billing audit](QA/ScenarioAudit.json) and [black/white/magenta inspection](QA/AssetAlpha.png).

## Implementation

- `ProductionTeamChatRuntime.cs`: source button sprites/proportions, original-lettered Russian mode, live English mode, footer ornament and left status geometry.
- `ProductionTeamChatView.cs`: revision 5 and compact localized session count.
- `ProductionTeamChatLayout.cs`: safe-bottom anchoring of the new ornament and relocated status.
- `ProductionTeamChatPrefabBuilder.cs`: R4 readback marker; cutover revision now derives from the view constant instead of a duplicated literal.
- Active `TeamChat.prefab` and `TeamChatCutoverState.txt` rebuilt together.
- Existing Play Mode coverage extended to verify Help actions/modal blocking, sprite aspect, language-specific lettering and status/ornament clearance.

## Verification

See [final checks and changed-file list](QA/ScopeAndVerification.json) and [preservation checks](QA/PreservationChecks.json). Verify01 passed all 28 targeted Unity tests; final source inspection found a stale cutover revision literal. The builder was corrected and verified again in Verify02. Final Verify02: Unity 6000.5.9f1 rebuild/readback, 13 EditMode + 15 PlayMode tests passed, zero failures/skips. Tests use isolated saves and cover Team Chat, Team Hub and Team Ranking. Actual Russian and narrow English captures and sprite alpha backgrounds were inspected.

The standalone review uses actual Unity Play Mode captures, not an offline reconstruction. All 12 browser views and the original-comparison toggle passed without script errors or horizontal overflow. Scoped C# whitespace checks passed. No Android build, physical-device/keyboard, performance, full-suite or live-backend test was performed.

See [remaining visual differences](QA/ReferenceFidelityReview.md). Fonts, reconstructed green/paper surfaces, alpha and surrounding scenery are not claimed to match every source pixel.
