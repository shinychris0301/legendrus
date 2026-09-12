# v1.20 Step 4 — Complete Demo Game Flow and Functional Logic Foundation

This checkpoint intentionally prioritizes game functionality over final visual fidelity. Existing demo/captured design pages may remain while the functional route graph is verified.

## Key implementation

- Fixed map Back routing through a shared `ScreenNavigationPolicy`: `ChapterMap -> WorldMap -> Home`.
- Hardware Escape/Android Back uses the same screen-parent policy.
- Preserved legacy `ScreenId` numeric values 0-21 and appended new demo routes only.
- Added functional demo routes for Event Detail, Individual Ranking, Team Hub, Team Ranking, Team Chat, Friends, Inventory, Special Offer, Language, Login, Tutorial/Help, Offline, Maintenance and Empty/Error states.
- Added Splash/Title as the demo boot entry before Loading/Home.
- Events -> Event Detail, Profile -> Ranking/Team/Inventory/Login, Team -> Ranking/Chat/Friends, Shop -> Special Offer, Settings -> Language/Tutorial/System-state demos.
- Social/LiveOps/account actions are local-session demonstrations only where production authority is unavailable.
- Demo event claims and friend-energy requests do not grant persistent economy resources.
- Existing save-v6, Chapter 1 gameplay, Match-3 board, energy, boosters, daily reward, victory/fail, story/chapter completion and fail-closed monetization logic are retained.

## Design policy

Final production design implementation is paused until the complete game flow is accepted. The current demo design remains usable for functional QA. `V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.*` defines the future art/resource requirements for all 36 final game surfaces.

## Unity checks

1. Confirm Splash/Title -> Loading -> Home.
2. Home -> World Map -> Chapter Map.
3. Verify Chapter Map Back -> World Map and World Map Back -> Home.
4. Verify Profile demo routes, Team routes, Event Detail, Shop Special Offer, Settings Language/Tutorial/System states.
5. Run `Legends of Rus > Tests > Run v1.20 Step 4 Functional Flow Self-Tests`.
6. Run full EditMode and PlayMode suites.
