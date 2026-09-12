# Legends of Rus — Commercial Vertical Slice v1.6

## Purpose

This milestone moves beyond compatibility fixes into the actual playable commercial loop.

## Implemented

- Approved 9:16 design pages remain the visual source of truth.
- Safe-area + fixed design viewport prevents phone-aspect zoom/crop.
- Home navigation:
  - Play / Chapter map
  - World map
  - Shop
  - Events
  - Characters
  - Mail
  - Daily reward
  - Profile
  - Settings
- Pre-Level:
  - booster selection modal
  - energy gate
- Gameplay:
  - 8x8 runtime board
  - tap + swipe
  - moves / goal / score live values
  - cascades
  - no-move reshuffle
  - 4 usable story boosters
  - pause overlay without destroying board
- Energy:
  - max 5
  - one energy regenerates every 30 minutes
  - Out of Energy modal
  - development-only purchase/ad/friend refill flows
- Progression:
  - victory / fail
  - story milestone dialogue
  - chapter-complete page at Chapter 1 level 10
- Daily reward:
  - once per UTC day
  - reward-claim modal
- Settings:
  - music
  - SFX
  - vibration
  - notifications
- QA:
  - F12 screenshot capture
  - approved-reference comparison tooling retained

## Important commercialization boundary

Store purchases and rewarded advertisements are development mocks only.
They do not charge real money or contact an ad network.

Before production release, replace MonetizationService stubs with the selected real IAP/ad SDK,
configure product IDs and consent/legal flows, and validate regional pricing.
