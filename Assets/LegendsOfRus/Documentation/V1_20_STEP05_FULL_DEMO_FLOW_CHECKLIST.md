# v1.20 Step 5 — Full Demo Flow Verification Checklist

Use the current demo design. This milestone verifies function and navigation only; production visual reconstruction remains deferred.

## A. Boot and main navigation

1. Launch -> Splash / Title.
2. Continue -> Loading -> Home.
3. Confirm Home resource HUD is visible and tappable navigation remains responsive.
4. Home -> World Map -> Chapter Map.
5. Tap Chapter Map Back: must return to World Map.
6. Tap World Map Back: must return to Home.
7. Repeat 4–6 using Android system Back / Editor Escape.

## B. Core playable loop

1. Home -> Play -> Chapter Map -> unlocked level -> Pre-Level. Verify the visible Pre-Level Back icon returns to Chapter Map, then reopen the level.
2. Open/close Booster Select without consuming a booster.
3. Start level and confirm energy is consumed only at the authoritative start boundary.
4. Verify single-drag four-direction swap, invalid-swap rejection, match/cascade, goals, moves, score, obstacles and special gems.
5. Pause -> Settings -> Back -> Pause -> Resume; board state must be unchanged.
6. Verify Restart and Exit paths do not duplicate rewards or progression.
7. Verify Victory -> story/chapter progression and Fail -> Retry/Exit.
8. Verify Chapter Complete -> World Map and relaunch-safe acknowledgement.

## C. Meta/economy demo

1. Daily Reward: first valid daily claim succeeds; repeat daily claim is blocked.
2. Reward Claim modal closes exactly once.
3. Inventory/boosters reflect GameState authority.
4. Shop pack purchase and Special Offer purchase fail closed without a production provider.
5. Rewarded-ad dependent operations never grant resources without verified fulfillment.
6. Out Of Energy paths return safely to the level flow; demo friend request never grants energy.

## D. Events, social and account demo

1. Events -> Event Detail -> first demo Claim acknowledged -> second Claim blocked -> Back -> Events.
2. Profile -> Team Hub -> Team Ranking -> Back -> Team Hub.
3. Team Hub -> Team Chat -> local demo Send -> Back -> Team Hub.
4. Team Hub -> Friends -> request energy once -> second request blocked -> Back -> Team Hub.
5. Profile -> Login -> Guest: local session state only; no cloud identity is implied.
6. Profile -> Individual Ranking -> Back -> Home; Profile -> Inventory -> Back -> Home. Neither route may dead-end.
7. Mail remains a safe stub and must not grant backend-owned rewards.

## E. Settings/system states

1. Settings toggles remain interactive.
2. Settings -> Language -> RU/EN -> Back -> Settings.
3. Settings -> Tutorial/Help -> safe exit.
4. Offline -> Retry/Home, Maintenance -> Back/Home, Empty/Error -> Retry/Home.
5. No screen may trap the user without a deterministic exit.

## F. Required Unity evidence

1. `Legends of Rus > Tests > Run v1.20 Step 5 Functional QA Self-Tests` passes.
2. Complete EditMode suite passes.
3. Complete PlayMode suite passes, including `FunctionalDemoNavigationPlayModeTests`.
4. Android device verifies system Back and touch Back on World Map.
5. Console contains no compile errors or uncaught runtime exceptions during the full flow.

A Step 5 package may be source/static verified before these Unity checks run, but it must not be called Unity-verified until the above executable evidence is captured.
