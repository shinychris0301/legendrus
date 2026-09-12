# v1.20 Step 8.4 QA evidence

This directory receives actual Unity Play-Mode runtime captures when the Step 8.4 menu command is executed.

Expected runtime evidence after user execution:

- `UnityRuntimeCaptures/UNITY_RUNTIME_CAPTURE_EXECUTED.json`
- 36 PNG captures (9 Wave 2 pages x 4 aspect targets)
- `UnityRuntimeCaptures/WAVE02_RUNTIME_VISUAL_METRICS.csv`
- `UnityRuntimeCaptures/SHOP_APPROVED_REFERENCE_RUNTIME_PASS.lock` only when Shop passes the approved-reference runtime gate

The shipped source package intentionally does not pre-populate runtime PASS evidence. Source previews from Step 8.3 are not substituted for Unity runtime captures.
