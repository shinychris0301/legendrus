# Daily Reward R2 — Back icon

R1 incorrectly reused the rectangular text Back tab. Replaced it with the original reference's circular antique-gold arrow/navy medallion, using fresh Scenario isolation (one successful model_pixa-background-removal job,3CU). Original source RGB preserved. Scenario output269×256 alpha was resampled to original140×133; the initial dimension assertion caught this before import. No additional generation was needed.

Runtime uses source position(10,1504) and140×133 rectangle with aspect preservation. Existing Button, routing/listeners and serialized identities remain; old text label is inactive, still bound/localizable. Owner and saved prefab agree; revision1 unchanged. No reward-service changes.

Verified: final alpha on black/white/magenta; Unity6000.5.9f1 compile/import/rebuild/readback;7/7EditMode and9/9PlayMode,zero failed/skipped, including native Back navigation,raycasts,safeareas,claim persistence and Shop/Home regressions. SevenDailyReward and sevenShop capture pairs saved. Visually inspected final941RU and576safeEN;othercaptures automatic. SourceRGB/hash,Python syntax,review links and changedC# whitespace checked.

Snapshot4678:4675unchanged,3changed(runtime,builder,prefab),2new(sprite/meta),0deleted;all existingmetas intact. Otherart,services,guards/packages/settings preserved.

Notrun: APK/AAB,physicalphone,fullsuite,manualbrowser,extendedprofiling,calibratedcolor/exactfont. Source icon resolution limits enlargement; no whole-page exact-match claim.

[Unity review](DailyReward_BackIcon_R2_Review.html) · [Changed files](ChangedFiles.md) · [Provenance](Sources/Provenance.json).
