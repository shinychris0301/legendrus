# Independent final R6 visual and source review

The final Home page is substantially closer to the whole original reference, and the specific defects found in the intermediate Unity diagnostics have been corrected. The independently repainted artwork and live typography still differ. This review does not establish exact reference reproduction or user approval.

## Evidence examined

Directly inspected five retained **actual Unity** portraits in `../UnityVerification/Home/`: `Home_941x1672_RU.png`, `Home_941x1672_EN.png`, `Home_576x1280_safearea_RU.png`, `Home_576x1280_maxbalances.png`, and `Home_1080x2400_safearea_RU.png`. Compared them with the complete original `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png` and the earlier retained intermediate diagnostics. Also read capture metadata and the final test result XML files.

The capture metadata identifies Unity 6000.5.9f1, the active Home prefab on the existing router Canvas, and simulated display/safe-area fixtures. These are not screenshots from an Android phone. The 1080×2400 image uses a deliberately large simulated safe area: x45, y100, width990, height2160.

## Corrections confirmed

- **Tall character cutoff:** the straight horizontal cloak/character termination visible in the intermediate tall portrait has been replaced by a soft lower fade into the scene. No hard crop edge or uncovered bottom gap was found in the inspected final sizes.
- **Navigation:** all four RU captions and their EN counterparts are complete and sit within their navy caption areas. Their baselines agree visually. The prior crowding at the right edge of the Cyrillic characters label is resolved. No new control-blocking overlap was observed.
- **Start shape:** the emerald surface now reaches the reference's vertical extent. The same read-only color-row heuristic measures y1055–1193, 139px, in both the final 941px capture and original reference; the intermediate version measured 123px. This establishes that extent only, not identical shapes or pixels.
- **Start lettering:** the live word now has a gold light-to-dark treatment instead of the earlier flat fill. Both ИГРАТЬ and PLAY are readable and centered. Its precise letterforms and bevel treatment remain different from the original.
- **Quest heading and reward inset:** the heading has greater weight and fits the left area; EN also fits. The reward frame is shorter and stays inside the central footer composition instead of protruding below the panel rail. The gem and unavailable value are clear and separated.
- **Footer transition:** the hard flat-black strip identified around the former upper footer boundary is gone. Scene detail now remains visible between navigation and the mail/quest/gift controls, with a continuous carved base below.
- **Top resources:** the gem and energy bolt have taller, clearer silhouettes than the diagnostic version. Ordinary balances remain readable. The maximum-balance fixture shows the complete two-line numbers clear of the plus controls, though those extreme values are necessarily small at 576px width.

## Remaining visual differences

The dominant reference family and composition are present: dusk blue/purple scenery, warm gold architecture, a clean-shaven Ivan in navy/red clothing, orange Firebird, uppercase crest, emerald Play button, four illustrated navigation cards, and a carved wood footer. Within that family, the final assets remain repaints rather than exact replicas:

- Ivan is more upright and shows more of his full body, hands and decorated waist than the reference. His costume's embroidery, belt, facial details and brighter royal-blue folds differ. The lower fade fixes an edge artifact without reproducing the original pose.
- The Firebird has different feather paths and jewels and a denser orange tail mass. Its source painting cannot be made identical through a whole-composite transform.
- Architectural details, background lighting, sky texture and foliage differ at local points, despite the much closer overall dusk palette and layout.
- The crest has different glyph contours, filigree and highlights. The live button and captions use the project's available typography; exact source-font identity remains unverified. The Start gradient improves material continuity but does not reproduce every engraved highlight and shadow in the reference word.
- Navigation frame ornamentation is more uniform and brighter than the original. Map markings, shop contents, scroll emblem and helmet engraving differ. The mailbox envelope remains somewhat larger inside its disk; the chest fittings and small jewels also differ.
- The extreme1080×2400 safe-area fixture leaves an additional unoccupied wood band below the live footer controls so the decorative plate reaches the physical bottom while controls remain inside the safe area. It is covered and coherent, but is an adaptation absent from the original941×1672 composition.

Missing XP/quest state continues to display unavailable values rather than fabricated reference measurements. Those live-state differences are recorded separately from artwork fidelity and were not treated as a reason to invent progress or rewards.

## Verification boundaries

Independently parsed final XML: **36/36 Edit Mode and 21/21 Play Mode passed**, with zero failures, skips or inconclusive cases. The auditor did not launch Unity; these are verified results from the root's final runs.

The final read-only source audit used `/tmp/lor-home-r6/import4/unity.log`, which records Home revision 9 saved and read back. `IndependentSourceAudit.json` passes: 1,487 baseline files, 1,458 unchanged, 29 intended modifications, and exactly two new `ProductionHomeGoldText` files. All 771 existing metadata files remain byte-identical. The new GUID is distinct and referenced by Home; there are no duplicate GUIDs among 772 audited metadata records. All 336 prior prefab IDs remain, with 33 additions. Protected Splash, Loading, package, ProjectSettings and Android groups are unchanged. All 23 selected generated assets match their original/source/runtime bytes and recorded hashes; both retained progress assets match the baseline.

Performed here: source/hash/GUID/prefab-ID audit, direct whole-image comparison of the five listed final portraits, final test XML inspection, capture metadata inspection and the read-only button extent measurement. No production files or pixels were edited. No new Unity execution, APK/AAB build, physical-phone test, performance test, exact font identification, or exhaustive pixel/color identity verification was performed by this reviewer. Root's separate Splash/Loading pixel comparisons and browser checks are not claimed as work by this reviewer.
