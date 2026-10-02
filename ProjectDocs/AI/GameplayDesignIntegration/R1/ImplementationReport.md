# Current board design and rules integrated; further effects deferred

Normal gameplay and the campaign preview now use the current board-item artwork (line, bomb, color and supported Spark) and staged obstacle artwork. Ordinary gem textures and shared inventory booster icons were already shared and remain so. Obstacles show damaged/intact sprites without numeric layer badges.

Previously these designs were tied to a non-Legacy rule version, so they appeared only in the mechanics sandbox. `Match3Board.UsesCurrentBoardArt` now makes visual selection independent of gameplay rules. Normal and campaign initialization opt into the current art. The sandbox retains its original Legacy visual comparison and the current art for newer profiles.

After clarification, the user explicitly selected artwork AND newer matching rules. Normal/campaign starts now select the existing PrototypeR4 profile: square creations, standalone specials, tap activation and its existing special/combination behavior. Existing R25 presentation is attached through one shared board initialization path. There is no new animation/effect refinement. Save formats, historical records/reward claims and level data remain unchanged; new attempts now use the adopted rules. Historical Legacy stays selectable in the sandbox.

The board is constructed at runtime by Match3Board, including its obstacle images. The same owner now selects current sprites; no serialized page prefab or separate builder requires regeneration. Existing resources and GUIDs are reused. No new art, package or graphics-setting changes.

[Next functional phase](NextPhase.md): campaign story handoff, then authored sector content and progression. The rollout is explicitly authorized by the user’s selection of newer matching rules. The earlier artwork-only verification is retained as historical evidence and superseded by the final rule-adoption run.

Verification results and actual Unity captures are recorded in QA. Physical Android validation remains outstanding. No Git commit or release deployment is part of this task.

## Final verification

The initial artwork-only run passed10 checks. After the user explicitly selected newer rules, Verify02 passed8 campaign/integration tests and2 actual native Home-to-game win/fail journeys. Verify03 passed a further content test covering all10 authored levels across3 attempt seeds each: playable openings, no initial automatic matches, enabled square/standalone-special rules and deterministic generation. These establish integration and starting-board validity, not full-campaign difficulty balance. Test move planners now use the board's actual rule version instead of assuming Legacy.

Unity6000.5.9f1, previously authorized process-only Vulkan/descriptor4096, isolated configuration. No EditMode or physical-device run. Browser430/1440:8 capture loads, zero JavaScript errors or overflow. Actual intact/damaged artwork and normal-route captures inspected. Runtime scope: Match3Board shared initialization and art selection; GameplayRuleVersion comment updated to reflect adoption while preserving stable replay identifiers. Tests: CampaignUIIntegrationTests, CampaignBoardIntegrationTests and FunctionalDemoNavigationPlayModeTests. No textures/prefabs/packages/settings or save-format changes. See Scope.json for task-start hashes.
