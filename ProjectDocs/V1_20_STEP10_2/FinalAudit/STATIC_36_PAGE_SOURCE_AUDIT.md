# v1.20 Step 10.2 — Static Final 36-Page Reconstruction Audit

Package-time source audit. Runtime evidence is intentionally not pre-generated.

## Summary

- Functional page contracts: **36/36**
- Source reconstruction candidate coverage: **36/36**
- Functional asset requirements: **254/254**
- Direct approved references: **10/36**
- Active production routes in packaged baseline: **10/36**
- Unity runtime-evidence pages in packaged baseline: **0/36** (generated only by real Unity runs)

## Status semantics

- **Source candidate** means the asset/candidate-builder lane exists and requirement coverage is complete.
- **Runtime evidence** means the appropriate four-aspect Unity capture manifest exists.
- **Direct approved reference** excludes demo/design guidance.
- **Active production route** means an accepted production registry/cutover state currently selects the page.

Source-complete does **not** imply runtime-verified, reference-approved, or cutover-approved.

## Wave 1 — 12 pages

| Page | Runtime evidence | Direct approved reference | Active route |
|---|---:|---:|---:|
| SplashTitle | False | False | False |
| Loading | False | True | True |
| Home | False | True | True |
| WorldMap | False | True | True |
| LevelSelect | False | True | True |
| PreLevel | False | True | True |
| Gameplay | False | True | True |
| Pause | False | False | True |
| Victory | False | True | True |
| Fail | False | True | True |
| ChapterComplete | False | False | True |
| Dialogue | False | True | False |

## Wave 2 — 9 pages

| Page | Runtime evidence | Direct approved reference | Active route |
|---|---:|---:|---:|
| CharacterCollection | False | False | False |
| DailyReward | False | False | False |
| Profile | False | False | False |
| Inventory | False | False | False |
| Shop | False | True | False |
| SpecialOffer | False | False | False |
| OutOfEnergy | False | False | False |
| BoosterSelect | False | False | False |
| RewardClaim | False | False | False |

## Wave 3 — 8 pages

| Page | Runtime evidence | Direct approved reference | Active route |
|---|---:|---:|---:|
| EventsHub | False | False | False |
| EventDetail | False | False | False |
| IndividualRanking | False | False | False |
| TeamHub | False | False | False |
| TeamRanking | False | False | False |
| TeamChat | False | False | False |
| FriendsList | False | False | False |
| Mail | False | False | False |

## Wave 4 — 7 pages

| Page | Runtime evidence | Direct approved reference | Active route |
|---|---:|---:|---:|
| Settings | False | False | False |
| LanguageSelect | False | False | False |
| Login | False | False | False |
| TutorialOverlay | False | False | False |
| OfflineState | False | False | False |
| MaintenanceState | False | False | False |
| EmptyErrorStates | False | False | False |

