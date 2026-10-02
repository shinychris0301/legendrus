# Mail — Responsive Audit R6

Mail already positions its shared player header and carved footer at the simulated safe edges. The checked Inbox and opened-letter states do not require a production layout repair. The installed revision-6 prefab, builder, runtime, artwork and fonts remain unchanged.

## Checks added

`Assets/LegendsOfRus/Tests/PlayMode/MailAlignmentPlayModeTests.cs` now asserts exact header/footer safe-edge placement, parchment clearance, complete aspect-preserved footer sprites, non-overlapping footer controls and minimum 44-screen-pixel targets at the tested sizes. Back retains its existing state-specific bottom clearance. Opened-letter attachments clear the closing text.

A new transition test covers Inbox → guild letter → Inbox → letter without attachments → Back in Russian and English on tall displays, including saved-state preservation and the absence of Claim for letters without attachments. Existing routing, raycast, localization and modal checks still run. Capture output precedes geometry assertions so a future failed layout leaves visual evidence.

## Results

Unity 6000.5.9f1: 2 EditMode + 7 PlayMode checks passed; zero failures or skips. Tests used an isolated save configuration and the installed prefab, without a rebuild. Retained 23 actual Unity captures with display metadata. The standalone review's 12 views load in Chrome at 430×900 without script errors or horizontal overflow.

Three matching Inbox captures (941×1672 Russian, 1080×2400 Russian safe area, 576×1280 English safe area) are pixel-identical to R5. This confirms those views remain unchanged; it does not prove exact reference fidelity.

Scope hashing found only the Mail PlayMode test file changed under Assets, with 7,175 files unchanged, no additions and no deletions. No generation or Scenario requests. See QA/ScopeAndVerification.json, QA/BaselineComparison.json and QA/Verify01 for evidence.

## Limits and next action

Tall opened letters retain substantial parchment space because attachments follow the bottom edge. Existing source-art reconstructions, typography differences, approved shared rewards and Home scenery remain. Mail still uses its existing sample/unavailable behavior; this audit does not introduce a backend or working reward claims.

No Android build, physical-device test, backend test, performance test or full-project suite was run. Next recommended page: Events responsive/footer audit, preserving approved artwork and page-specific navigation.
