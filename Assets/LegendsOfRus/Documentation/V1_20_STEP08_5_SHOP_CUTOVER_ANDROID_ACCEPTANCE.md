# v1.20 Step 8.5 — Shop cutover + Android acceptance

Step 8.5 introduces a separate opt-in production-meta cutover boundary for Shop. It does not change the accepted ten-route `ProductionCoreLoopPageRegistry` and does not authorize the other eight Wave 2 pages.

The distributed source package is intentionally pre-cutover. Step 8.4 actual Unity evidence must exist and pass before the Step 8.5 Apply command can succeed.

After Apply, both a complete functional-regression signoff and a physical Android-regression signoff are mandatory before final Shop development-cutover acceptance.

Commercial authority is unchanged: ScreenRouter interaction hotspots continue to call the existing MonetizationService / verified entitlement boundary. The production Shop visual prefab contains no Button component and no purchase/reward callback.
