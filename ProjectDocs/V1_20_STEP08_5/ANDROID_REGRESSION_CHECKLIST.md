# Step 8.5 physical Android regression checklist — active Shop cutover

All items must pass on a physical Android device before recording the Step 8.5 Android PASS lock.

## Launch and routing
- Cold launch reaches Home with no new Console/device-log errors.
- Home → Shop opens the Step 8.5 production Shop, not the old flattened DesignScreens Shop.
- Visible Back from Shop returns to Home.
- Android system Back from Shop returns to Home using the same parent policy.
- Shop → Special Offer opens the existing Special Offer route; Back returns safely.

## Layout and touch
- Shop remains portrait-safe at the device aspect ratio.
- Top HUD, title, product cards, support strip, and bottom navigation stay within safe area/cutout bounds.
- Notch/camera cutout does not obscure resource values or Back affordance.
- Pack 1/2/3 hotspots respond once per intentional tap; there are no dead zones or duplicated taps.
- Decorative candidate images/text do not intercept ScreenRouter hotspots.
- Background/resume does not leave duplicate Shop visual or interaction layers.

## Text
- Cyrillic headings/product names render without missing glyph boxes.
- Prices/quantities remain live text and are not baked into one full-screen screenshot.
- Text remains legible without clipping on the tested device.

## Commercial/economy authority
- With production IAP provider unavailable/unverified, purchase attempts fail closed.
- No coins, gems, energy, items, entitlements, or rewards are granted from a visual prefab or unverified result.
- A purchase/reward can only fulfill after the accepted provider reports verified-and-durably-committed authority.
- Special Offer and Out-of-Energy behavior remain unchanged by the Shop visual cutover.

## Regression
- Run one normal Chapter 1 gameplay attempt after visiting Shop.
- Swipe input, pause/resume, victory/fail, save/load, energy, and Back navigation remain functional.
- No new Android-only exception, missing resource, or UI scale regression appears.

Any failure => do not record PASS; roll back Shop with the Step 8.5 rollback command.
