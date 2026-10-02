# Reference fidelity — Reward Received rebuild R1

The rebuilt modal now follows the original frame, title, chest, button proportions and gold/navy hierarchy much more closely. This is an actual native Unity implementation, not a full-reference screenshot with hotspots.

| Element | Result | Remaining difference |
|---|---|---|
| Outer frame and sapphire ornaments | Original perimeter retained; no rectangular scenery crop | Fine extraction alpha and repaired interior are approximate |
| Russian heading and Collect button | Original lettering RGB retained | English uses live project typography |
| Chest and celebration | Original chest RGB with new Scenario alpha; source glow behind chest | Source glow isolation is approximate, not an independent original layer |
| Reward cards | Source fine borders, centered icons and live quantities | Actual grants replace the nine illustrative rewards |
| Background/header | Real underlying page and shared player header dimmed by modal | Daily Reward remains behind the receipt instead of reference Home |
| Safe-area layout | Standard and tall native captures; proportions retained | Extra scenery margins on tall devices |

Final evidence: `Verify02/RewardReceived/`, `ScopeAndVerification.json`, `BrowserChecks.json`, `OriginalRGBCheck.json`. 19 targeted Unity tests and 11 browser review views passed. No Android-device test. Recommended next screen after this review: Out of Energy.
