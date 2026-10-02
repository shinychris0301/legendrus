# Chapter Complete R2 — reference comparison

R2 addresses the user-approved header/firebird edge defects and reward hierarchy. The default [review portrait](ChapterComplete_EdgesAndRewards_R2_Review.html) is an actual final Unity capture, with separate choices for R1 and the approved reference.

| Area | Result |
|---|---|
| Header | Extended source extraction from 941×193 to 941×390 restores the curved corner ornaments. The former solid bottom crop edge is gone. Original title RGB is unchanged; opaque navy face retained. |
| Firebird | Expanded foreground from 789×695 to 883×695 at the same origin, recovering the feather/tail tips without scaling Ivan. The right crop edge now has no pixels with alpha above128, versus63 in R1. 1,589 opaque source pixels lie beyond the previous right boundary. Attached footer rails were excluded because the footer remains a separate panel. |
| Rewards heading | Original Russian “НАГРАДЫ” lettering and gold divider ornaments isolated from the source. English remains live on the same ornament spacing. |
| Reward cards | All six existing categories remain visible. Cards are now their native134×139 size, versus approximately114.7×119 in R1; icons80×80 versus68×68; quantities30px versus27px. No horizontal distortion. |
| Live summary | One received-reward/completion line sits below the buttons. The long competing heading and separate no-bonus note are no longer shown. Values and already-received semantics remain unchanged. |
| Divider | Source gold line restored between the summary and next-chapter heading. |
| Unchanged composition | Chapter ribbon, buttons, teaser, clean R1 background, navigation and actual reward logic retained. |

Visually inspected final run2 reference-size Russian and tall English captures, plus reference-size English, tall Russian and in-progress captures. Automated layout, safe-area, text-fit, raycast and non-overlap checks cover all six capture variants. Assets inspected over black, white and magenta; Russian header/foreground source RGB verified byte-for-byte at the pixel-array level.

Remaining differences: regenerated village lighting/building placement, incomplete full-height side framing below the repaired header corners, reconstructed footer wood grain, approximate live/English typography and fine alpha-edge softness. Six actual reward categories differ intentionally from the reference's four samples. The crest's three gold stars remain decorative; the actual earned total is shown in the live summary. The source font is not identified. R2 is ready for review, not an exact-match or user-acceptance claim.
