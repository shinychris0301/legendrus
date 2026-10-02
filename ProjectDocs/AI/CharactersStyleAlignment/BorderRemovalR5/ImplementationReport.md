# Characters — bright border removed R5

Removed the explicit bright-yellow rectangular perimeter introduced in R4, as requested. Restored the previous subtle current-hero glow behind the antique-gold frame. Kept R4 title/button alignment, cleaned surfaces, locked-row scrolling and all behavior.

Updated native runtime, revision5 view/builder/cutover and rebuilt the existing prefab. Removed the obsolete bright-edge test assertions. No new tests, art edits or Scenario requests.

Unity6000.5.9f1 import/compile/rebuild/readback passed;2Edit+6Play tests passed with0failures/skips. Nine fresh actual Unity captures retained; narrowEN visually inspected to confirm removal. Existing scroll/preview/current-hero/save/navigation/localization checks passed. Browser review results inQA/BrowserCheck.json. Scope audit:6existing Assets files changed, no new/deleted assets or existing meta/GUID/art/settings changes.

Not performed: APK, physical phone/touch, full-project suite or profiling. Other documented reference differences remain.

[Review](Characters_BorderRemoval_R5_Review.html).
