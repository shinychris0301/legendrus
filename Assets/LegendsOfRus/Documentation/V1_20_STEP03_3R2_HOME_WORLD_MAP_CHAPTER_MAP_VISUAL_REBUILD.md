# v1.20 Step 3.3R.2 — Home / World Map / Chapter Map Visual Rebuild

## Objective

Correct the three highest-impact Step 3.3 pages against the locked approved reference images before continuing the remaining core-loop visual wave.

## Pinned references

| Page | Reference | SHA-256 |
| --- | --- | --- |
| Home | `Art/ReferenceOnly/ApprovedScreens/Home.png` | `7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8` |
| World Map | `Art/ReferenceOnly/ApprovedScreens/WorldMap.png` | `b13e01a68e3ef8641e6094abefabe34faa3c1a0d2e2cec9f7fb2a5afc7993e19` |
| Chapter Map | `Art/ReferenceOnly/ApprovedScreens/ChapterMap.png` | `e69143c5ec1d2122fed9e177c8b0cc48bee0cdc657d50080a2cf7113023122bf` |

## Production decomposition

Home uses six role-specific components covering the profile/resource HUD, ornate title/logo scene, Ivan/Firebird hero scene, emerald Play region, illustrated navigation row, and quest footer.

World Map uses four components covering the HUD, ornate page title, full painted valley/river/castle map field, and large three-medallion navigation footer.

Chapter Map uses four components covering the HUD, Chapter 1 red ribbon/title treatment, painted village/level-path field, and back/Play footer.

No production component is a full 941x1672 page. The Runtime prefab dependency graph must not include `Resources/DesignScreens` or `Art/ReferenceOnly`.

## Why reference-derived components are used

The locked archive available to this project contains flattened approved screens rather than original painter/PSD/character source layers. Source-side componentization is therefore used to guarantee visual parity while replacing the single full-screen placeholder dependency with a stable production prefab contract. This is explicitly recorded rather than represented as newly recovered source-layer art.

## Reference-match gate

Source reconstruction is exact at 941x1672: all three page component sets reproduce their pinned reference with zero differing pixels. Final signoff is still blocked until the Unity-generated prefabs are rendered and reviewed because runtime resource masks, text rendering, safe-area behavior, import compression, and aspect-ratio scaling can alter the final output.

## Safety boundaries

- approved references immutable;
- screenshot deletion unauthorized;
- generic style substitution forbidden;
- no gameplay/save/economy mutation;
- no production publication action;
- no final visual signoff from static verification alone.
