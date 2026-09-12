# v1.20 Step 9.4 — Wave 3 Refined Runtime Capture, Reference-Authority Intake, and Conditional Acceptance Readiness

Step 9.4 is an evidence/readiness checkpoint. It does not activate any Wave 3 page and it does not enable production LiveOps, leaderboard, team, chat, friends, or mail backends.

## Runtime evidence
- Build the Step 9.3 refined Wave 3 candidates.
- Capture 8 pages at 4 resolutions (32 captures) through Unity Play Mode / Camera + RenderTexture.
- No UI action, social action, event claim, mail reward, provider call, or economy mutation is invoked by the capture harness.

## Reference authority
Only canonical files placed under `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens` can become direct production-reference authority. Existing `Resources/DesignScreens/Events.png` and `Mail.png` remain demo/composition guidance and are never auto-promoted.

Canonical Wave 3 approved filenames: `Events.png`, `EventDetail.png`, `IndividualRanking.png`, `TeamHub.png`, `TeamRanking.png`, `TeamChat.png`, `FriendsList.png`, `Mail.png`. Approved references must be 941x1672.

## Conditional readiness
A page with a direct approved reference must pass runtime correlation >= 0.80 and MAE <= 0.10. Partial readiness is allowed only for the subset that has direct approved authority and passes. Full-wave readiness requires 8/8 direct approved references, 8/8 runtime fidelity passes, 32/32 captures, and the Social/LiveOps authority gate. Step 9.4 still performs no active cutover.
