# Victory caption readability correction

The reference layout assigned Reaction, Status and RewardStatus 14 reference-pixel text in a 19-pixel strip between currency rewards and the booster heading. At 576 pixels wide that produced approximately 8.6-pixel type; long text had virtually no wrapping space.

The canonical Victory style now reuses those same Text objects in a dedicated footer writing area above Continue. Type is 34 reference pixels, with wrapping, fixed size (no automatic shrinking), pale cream text and the existing MessageInset as a navy background. The writing rectangle is 685 × 112 reference units. Existing mutually exclusive reaction, reward-bonus explanation and status behavior is retained.

The composition reserves 128 additional reference units for captions, fitting uniformly inside the safe area on shorter displays. On the 576 × 1280 test viewport, the text is about 20.8 pixels rather than 8.6. On the short 360 × 640 viewport with a 600-pixel safe height it is about 11.3 pixels rather than 5.0. Captures must be assessed at their actual size; simulated resolution is not physical-device/DPI validation.

The existing prefab owner invokes ProductionVictoryRuntime.ApplyCurrentStyle and the modified ProductionVictoryReferenceStyle. The runtime builder now finds MessageInset through its serialized binding before the legacy Rewards child lookup, so repeated builds reuse the moved object. Revision is 11 in source and the rebuilt canonical Pages/Victory.prefab. All 362 serialized prefab object IDs are preserved. A four-reference-unit gap separates Continue and the bonus button, avoiding touching bounds after fractional scaling.

No new font, image, scene, service or UI object is introduced. Existing sprites, Cyrillic/English wording, rewards and navigation are preserved. The page still uses its existing typography and rendering components.

Verification includes the existing Victory suite and a new regression that checks every installed victory reaction in both languages for wrapping, captures 360 × 640, 576 × 1280 and 941 × 1672, tests unavailable-video status in both languages, and compares player state excluding only the independent energy-clock timestamp. Captures use service-result fixtures; they are not claims of manually played wins.

Initial checks exposed a duplicate MessageInset during rebuild; the serialized-binding lookup fixes that. The next run exposed touching footer button bounds after scaling; explicit spacing fixes that. The full Victory suite also exposed an old capture assumption: its final English image was named round 10 although the chapter now ends at 200, and it captured immediately after that last page opened. The test now waits for the existing star reveal before capturing and names the image for the last authored round. No animation code was changed. Earlier evidence is retained under QA/Verify01 through QA/Verify04.
