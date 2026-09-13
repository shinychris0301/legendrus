# Independent Home R4 Unity visual review

**Final recommendation: retain the corrected shoulder-perch pose and numeric spacing.** The final actual Unity portraits resolve the two concrete implementation issues identified below. The source-art/reference differences remain explicit and are not converted into an exact-match or user-approval claim.

## Final corrected Unity captures

Independently inspected five whole portraits from `/tmp/lor-home-r4/playmode-perch/Home/`: reference-size Russian, 576 × 1280 with simulated safe area, reference-size English, 1080 × 2400 with simulated safe area, and 576 × 1280 maximum balances. The observed files and SHA-256 hashes are recorded in `IndependentUnityVisualReview.json`.

- **Firebird:** the full unchanged sprite at `(436,473,430,645)` with local Z rotation `−15°` now visibly rests its claws on Ivan's fur shoulder. The beak remains clear of his hair and face. Its curved tail stays visible across the cloak, and the Start control draws clearly in front. Contact remains coherent in the tall safe-area capture. Retain this pose.
- **Top section:** ordinary values remain readable. Both maximum gold and gem values now display fully on two lines with visible clearance before their plus buttons. The avatar's complete gold inner rim overlays the portrait correctly; its pendant remains intact.
- **Navigation/footer:** Russian and English captions fit inside the blank navy bands. The mailbox, collection section and gift chest remain contained and readable. No scenery-contaminated panel, checkerboard, obvious alpha fringe, protruding backing, new clipping or bottom seam was observed.

Read the completed final Play Mode XML result: **21 passed, 0 failed, 0 skipped**. This reviewer did not execute that run. A separate read-only decoded PNG comparison between final Russian and Russian-roundtrip captures found **1,557 differing pixels, maximum channel difference 2/255**, bounded to `(345,44)`–`(852,97)` in the resource capsules; therefore the two captures are not claimed to be pixel-identical. Evidence is `IndependentFinalRoundtripComparison.json`. The Firebird and other regions are identical in this comparison. The capsule difference is visually minor in these reviewed portraits.

The intermediate translation-only pose at `x415` improved spacing but still left the claws above open scenery. It was superseded by the final rotation. The earlier numerical overlap was fixed by reserving a gap before each plus and strengthening the runtime glyph check to exclude the plus region.

This review supports the final placement and usability correction. It does not establish exact reference colors, exact font identity, physical-phone appearance or user visual approval. The preserved bright winter background and newly painted details still differ from the original dusk reference; the original alternate logo/hero/Play design is superseded by the latest explicit Splash reuse instruction.

## Initial captured revision — before the final placement/readability corrections

Reviewed actual Unity captures from `/tmp/lor-home-r4/playmode-final/Home/`: `Home_941x1672_RU.png`, `Home_576x1280_safearea_RU.png`, `Home_941x1672_EN.png`, `Home_1080x2400_safearea_RU.png` and `Home_576x1280_maxbalances.png`. Their metadata identifies Unity6000.5.9f1, OpenGLCore, the active Home prefab and simulated display/safe-area fixtures. The21/21 passing Play Mode result was read, but is not treated as proof of visual fidelity.

Compared the whole portraits with the original whole `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png` and the latest explicit instruction to reuse the approved Splash character, logo and Start. The latter intentionally supersedes the original reference's alternate character, uppercase crest and Play lettering.

Observed improvements: all top resource counters and ordinary balances are legible; the avatar's gold inner rim and lower pendant correctly render in front of the portrait; the four navigation labels fit their blank bands without touching ornaments; mail, gem collection and gift sections remain visible within the simulated safe area. No baked checkerboard, scenery rectangle, backing protrusion, obvious alpha halo or bottom background seam appears. The Firebird is richly feathered with a readable curved tail, and its source aspect is preserved.

Two concrete issues were sent to root/integration before the final correction:

1. **Firebird perch:** at `(470,490,430,645)` its claws end over open castle/sky about40–60 design pixels to the right of Ivan's fur shoulder. The bird appears suspended while the reference shows a shoulder perch. Root selected a55-pixel left move to `x415`, preserving source aspect and other geometry. The next actual capture must verify contact and face clearance.
2. **Maximum balance and plus overlap:** the576-pixel maximum-balance capture shows the final gold digit crowded by the green plus. Source inspection confirms numeric label bounds `[58,width−38]` overlap plus bounds `[width−43,width]` by5 logical pixels. The initial glyph check constrains digits to the capsule but does not exclude the plus. Recommended correction: reserve3–4 logical pixels before the plus and assert against its actual left edge. Ordinary balances were clear.

Remaining reference differences are explicit: the retained bright winter environment differs from the original dark dusk village; independently painted filigree/metal details and the shared live font are approximations; the map caption is smaller than the three shorter captions so it fits; unavailable experience/quest/reward values use dashes rather than fabricated reference numbers. Reused Splash character/crest/Start artwork differs from the old Home reference by the latest user instruction. None of this constitutes user visual acceptance or pixel-exact color identity.

This reviewer changed only QA documentation and inspected existing captures/source. It did not alter production files, run Unity tests, build an APK, or test a physical phone. The final corrected portrait review is recorded above.
