# v1.20 Step 3.2 — Canonical UI Kit Reconstruction

## Purpose
Replace the flattened-screen design dependency with a real reusable component language before individual page cutover begins.

## Implemented source assets
- 34 separate PNG sprites under `Assets/LegendsOfRus/Art/Production/UI/CanonicalKit`.
- No baked UI labels; text stays editable/localizable.
- Locked palette: lacquer navy, antique gold, sapphire, parchment, emerald, danger red.
- 9-slice metadata defined for buttons, panels, bars, tabs, navigation/reward/booster/ranking surfaces, progress components, toggle track, goal/board frames.

## Prefab generation
Run `Legends of Rus > Production UI > Build v1.20 Step 3.2 Canonical UI Kit`.
It deterministically builds reusable prefabs under `Assets/LegendsOfRus/Prefabs/UI/Kit`.
The builder also enforces Sprite import, Read/Write off, mipmaps off, 100 PPU, and Android ASTC 6x6.

Required prefabs include PrimaryButton, SecondaryButton, DangerButton, NavyPanel, ParchmentPanel, ModalFrame, HeaderFrame, TopBar, TabBar, CurrencyBar, BottomNavigation, ProgressBar, RewardTile, BoosterSlot, RankingRow, IconButton, NotificationBadge, Toggle, GoalPanel, BoardFrame and MapNode.

## Safety boundary
Step 3.2 does **not** change `ScreenRouter` or `DesignScreenRegistry`. All 22 screenshot placeholders remain present until a replacement page passes Step 3.3 visual/runtime verification. Approved reference screens are untouched.

## Next checkpoint
`v1.20 Step 3.3 — Core Loop Page Reconstruction` will start replacing screenshot-backed pages with these real components, beginning with Loading/Home/World Map/Level Select/Pre-Level/Gameplay/Pause/Victory/Fail/Chapter Complete.
