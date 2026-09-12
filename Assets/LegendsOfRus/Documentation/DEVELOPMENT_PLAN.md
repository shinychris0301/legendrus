# Development Plan — From Approved Design to Commercial Game

## Product principle
The approved design remains the visual source of truth. Runtime systems must reproduce it;
development is not allowed to invent a competing visual language.

## Milestone 1 — Commercial vertical slice (started in this project)
Goal: one polished, measurable, monetization-ready Chapter 1 loop.

Deliverables:
- exact approved Loading/Home/World Map/Chapter Map/Pre-Level/Game/Result/Dialogue/Shop shells
- responsive 9:16 viewport inside Android safe areas
- functional 8×8 Match-3 board
- swaps, matches, cascades, score, moves and goals
- save/progression
- energy spend/refill path
- story milestone routing
- monetization service abstraction and development purchase/reward mocks
- analytics event abstraction
- runtime screenshot/reference comparison workflow

Exit criteria:
- player can start the app, enter Chapter 1, play, win/fail, advance story, return to map
- no screen visually drifts from the approved family
- 16:9 through 20:9 device layouts keep the 9:16 design intact and interactable
- no blocking exceptions in Play Mode

## Milestone 2 — Replace baked dynamic UI
Rebuild the dynamic zones of the approved screens as real Unity components:
- player name/level
- gold/gems/energy
- level numbers/stars/locks
- goals/moves/score
- shop prices/content
- timers/progress bars
- localized narrative text

Do this one screen at a time and compare runtime captures to the approved references.

## Milestone 3 — Chapter 1 production content
- 300–600 story-path level framework with authored early levels and data-driven later templates
- tutorial sequence
- Firebird arrival
- village elder rescue
- dark crow foreshadowing
- chapter-complete emotional beat
- chapter-specific backgrounds/props/obstacles
- booster introduction and balanced economy

## Milestone 4 — Retention and attachment
- Firebird visual recovery progression
- character relationship/unlock system
- story beat every 10–15 story levels
- chapter mystery teasers on map
- daily rewards
- event hub + character rescue events
- achievements and collections
- optional dialogue journal

## Milestone 5 — Monetization production integration
Revenue is built around transparent value and player trust:
- energy refill
- fail/continue
- booster packs
- chapter-finale packages
- optional rewarded ads
- cosmetic character/avatar/frame sales
- seasonal event offers
- battle/event pass only after retention proves healthy

Production work:
- Unity IAP or current supported store SDK
- rewarded/interstitial provider
- remote-config product catalog and pricing
- receipt validation / backend
- analytics funnels
- consent/privacy/legal implementation

Do not use deceptive countdowns, disguised purchases or forced-spend gates.

## Milestone 6 — Backend / live operations
- cloud save/auth
- analytics
- remote config
- events
- leaderboards
- inbox/rewards
- offer segmentation
- A/B testing of economy values, never of deceptive UI

## Milestone 7 — Scale to Chapters 2–20
The chapter story bible is authoritative for content identity.
Each chapter ships as addressable/downloadable content with:
- environment set
- map
- obstacles/goal variants
- story scenes
- chapter characters
- boss/milestone
- chapter reward set
- music/audio
- QA captures

## Curiosity and emotional attachment pillars
1. Wounded Firebird: immediate emotional responsibility.
2. Fading color: every successful match visibly heals the world.
3. Foreshadowing: the crow and Chernobog are seen before they are understood.
4. Companions: unlocks change gameplay and story.
5. Map mystery: locked chapter art hints at future realms without revealing the full story.
6. Restoration: before/after chapter states provide visible long-term progress.
7. Character memory: journal and dialogue callbacks make relationships cumulative.
