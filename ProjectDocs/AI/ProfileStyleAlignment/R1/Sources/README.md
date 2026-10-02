# Source workflow evidence

OriginalReference.png is the unchanged review reference, never a runtime page. Assessment.json records final source bounds. PrepareSources.py records the initial input preparation; do not rerun it over submitted evidence. ScenarioRequests.py uses exclusive submission markers; collect an accepted job rather than submitting it again.

Initial footer_history/footer_trophy crops were rejected for clipping. Their submitted PNGs and job records remain immutable. Wider footer_history_wide/footer_trophy_wide submissions supply final alpha; final crop/native files use the corrected bounds recorded in Assessment.json.

RefineNativeInputs.py applies final documented source-donor caption/level cleanup without touching submitted inputs. PrepareFinal.py combines original/native RGB with Scenario alpha, restores intentionally opaque faces and original stars, removes neighboring title fragments, creates the English title variant, generates QA backgrounds and copies independent sprites into ProfileAligned. AssetProvenance.json hashes final output. ScenarioAudit.json independently verifies each submitted input hash and actual billing.

No complete reference screenshot is shipped as this production page. Only framed illustration interiors and intentionally opaque panel surfaces retain their original backgrounds; clean full-page scenery is reused from IndividualRankingAligned.
