# Chapter Complete R1 — reference fidelity review

Review requested after R1 delivery. Result: the major source-derived artwork matches closely, but the complete page does not match the reference exactly. Passing functional tests does not establish visual equivalence.

## Evidence checked this review

- Approved `Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/ChapterComplete.png` and its recorded source hash.
- Actual Unity captures `ChapterComplete_941x1672_RU.png` and `ChapterComplete_576x1280_safe_RU.png` from R1 `UnityVerification/ChapterComplete/`, visually examined against the reference.
- Current runtime builder/view/layout and active prefab, working-tree status and Unity version 6000.5.9f1.
- All 4,557 final production file hashes and all 39 evidence hashes still match the delivered R1 manifests. Thus the saved captures remain relevant to the unchanged implementation; they are not freshly rendered this review.
- Source-alpha boundary statistics and preserve-aspect reward-card dimensions.

## Matches worth retaining

Russian main title, chapter ribbon, Ivan's pose/clothing, firebird body, three-star ceremonial crest, Victory ribbon, green/blue buttons and framed Chapter 2 teaser use the extracted reference artwork. Their principal reference-size positions are retained. Buttons are not horizontally stretched. There is no reason to regenerate these complete elements.

## Remaining discrepancies, in priority order

1. **Header/frame silhouette — correction needed.** The current title asset ends abruptly at reference y=193, forming a straight horizontal edge. Its bottom row contains 940/941 pixels with alpha above 128. The reference continues into curved corner ornaments and narrow side framing. Restore that missing silhouette with isolated source ornament assets, preserving the existing title face.
2. **Firebird lower-right edge — correction needed.** The foreground crop ends at reference x=818. Its final pixel column retains alpha above 128 for 63 consecutive rows (local y589–651; reference y981–1043), supporting the visible vertical cutoff. Recover the missing feather/tail contour and clean residual edge pixels; retain the approved character and bird RGB.
3. **Reward hierarchy — substantial visual difference.** The reference has a prominent short “НАГРАДЫ” heading and four larger cards. R1 adds a completion line and a long heading, uses six smaller cards and adds an explanatory line below the buttons. A 134×139 source card is displayed at approximately 114.7×119 within its 124×119 preserve-aspect rect: about 14% smaller in each dimension. Together with the missing decorative dividers, this makes the lower section denser and its live text visually weaker. Restore the short heading/dividers and simplify the summary hierarchy while preserving truthful reward values. Six actual reward categories versus four sample categories is an intentional data adaptation, not a missing reward-system implementation; any consolidation must preserve access to all six actual amounts.
4. **Scenery and framing — visible difference.** The regenerated cottage, church, vegetation and sunset are positioned differently. The reference has a more enclosed scene framed by side branches and carved edges. R1's village has more open sky and a different light distribution. Refine framing/composition first; do not globally recolor the retained source character/buttons to compensate for a different background.
5. **Typography and footer surface — remaining polish.** Dynamic headings/counts use the existing plain serif rather than the reference's embossed lettering. Reconstructed wood and missing internal lines are less detailed. The exact reference font remains unidentified. The current long headings and notes are particularly small in the 576×1280 capture; automated fit tests establish containment, not comfortable device readability.

Tall-screen composition also differs from the reference ratio: the whole layout is centered within the safe area, exposing additional scenery above and below. No obvious clipping of controls is visible in the reviewed tall capture. Physical-device legibility remains unverified.

## Intentional functional differences

Reward amounts summarize configured base first-clear rewards already received. They are not the reference's sample 12,540/350/25/1 values, current wallet balances, or a new chapter bonus. Energy/chest rewards are unsupported by the chapter service. The three gold crest stars remain decorative; actual earned stars are reported numerically. Chapter 2 is a teaser marked coming soon; both buttons retain World Map navigation. These differences should remain explicitly distinguished from unfinished artwork.

## Recommended next pass

**R2 — frame/silhouette cleanup and reward hierarchy.** Restore header side ornaments, repair the firebird cutoff, bring back the short reward heading and decorative separators, and improve live-summary readability without changing reward grants, routing or accepted artwork. Then recapture reference and tall portraits before considering any further Scenario background generation.

No production files were edited in this review. No Scenario/API calls, Unity rebuild, fresh captures, new test run, APK, device test, browser interaction, performance test or calibrated color measurement were performed. Earlier R1 test results remain historical evidence only. Documentation updated: this review, CurrentTask.md and SessionLog.md.
