# Daily Reward — progress ends R4

Fixed the monthly progress display ends in native revision 5. R3 tiled a middle rail segment and clipped it to diagonals, omitting the gold caps. R4 extracts the original left cap, mirrors it at the right, and preserves the frame while a separate tapered fill reflects saved monthly claims. Added transparent vertical padding so the small cap ornaments have space. No reward or navigation behavior changed.

Source cap: original DailyReward.png rectangle [158,1237,192,1270]. Only baked progress inside the cap was replaced with an exposed dark source donor. Center rail and gold fill also reuse original pixels. The right cap is a mirrored reconstruction, not an independently visible source element. **0 new Scenario calls / 0 CU**: this is directly recoverable source material. Alpha reviewed over black, white and magenta at empty/half/full fill.

Updated ProductionDailyRewardRuntime, View revision, owning builder, active prefab and cutover state together. Two new sprites and metas added; all369existingprefabrecords retained. Task-start asset audit:6existingfiles modified,4added,0deleted. Existing artwork/other page prefabs/shared header unchanged.

Unity6000.5.9f1 ProgressEndsR4Verify01: prefab rebuild/readback succeeded; **7Edit Mode +11Play Mode =18tests passed**. Extended monthly test checks matching rail/fill geometry and the native horizontal-filled image, alongside existing counts0/5/28/31, progress clamping, seven-day claims, persistence, RU/EN, safe-area input and Shop/Home regressions. Actual Unity rail crops reviewed at0,6and28monthlyclaims: both cap borders remain visible. Review HTML:10views decoded without errors or horizontal overflow at390×844. NoAndroid/device/full-suite test.

Remaining R3 full-page differences still apply: reconstructed hidden textures, live fonts/scenery, reduced Day7chest size for actual reward content and the unavailable monthly bonus notice. This pass addresses only the progress ends.
