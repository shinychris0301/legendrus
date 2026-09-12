# v1.20 Step 3.3R.3 — Pre-Level / Gameplay / Pause Visual Rebuild

## Intent

Correct the second core-loop visual-fidelity batch without replacing dynamic gameplay with captured pixels.

## Reference contract

- Pre-Level approved reference SHA-256: `32e05c72c56f403ea065a327aea0ee9bdef33b8bb22330807baff117c0b40195`
- Gameplay approved reference SHA-256: `d8e73bb76006bd0b4c6428196657ddce47193d3056362562a68d571b74c1d800`
- Pause provisional captured reference SHA-256: `31ae057a57b556ba296d4415678cb6aaa1faf72e8bbe4eb6325e42bd64c55716`
- Reference coordinate system: 941x1672 portrait.

## Dynamic boundaries

Gameplay keeps the runtime board at `x=79, y=357, width=786, height=942` and reconstructs only the fixed shell around it. Pause remains a modal over that same live gameplay state and uses a runtime dim overlay.

## Acceptance boundary

Static component reconstruction may pass before Unity render review. Final reference-match sign-off may not be granted until Unity-generated captures verify composition, dynamic masks/text, board integration, animations, and multiple phone aspect ratios.
