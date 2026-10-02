# Out of Energy R1 — visual comparison

Compared original `Assets/LegendsOfRus/Resources/DesignScreens/OutOfEnergy.png` (941×1672) with actual Unity captures.

| Area | Implementation | Difference |
|---|---|---|
| Main panel | Upright navy/gold panel, sapphire crest and lower jewel, complete corner ornaments, countdown frame and three option rows | Scenario reconstruction approximates fine engraving and navy texture; native centering places the panel about 6px lower at 941×1672 |
| Heading/body | Live RU/EN serif text in reference-like positions | Source font identity remains unknown; exact engraved lettering/bevel is not reproduced; body copy describes current availability accurately |
| Energy emblem | Reference-driven gold/amber medallion with separate live count | Local alpha contour cleanup; the shared raised count badge is emerald rather than the reference's navy |
| Countdown | Original-style hourglass, live `MM:SS` from persisted energy timestamps | Current refill interval is 30 minutes; 05:37 is only a test timestamp, never a hardcoded UI timer |
| Purchase | Gold energy icon and empty emerald/gold button with live label | Existing icon differs slightly; no verified price is available, so 99 ₽ is not displayed and control is unavailable |
| Friends/ad | Fresh source-RGB icons, sapphire/gold buttons, live titles/status | Some fine icon edges are locally masked; both controls remain unavailable rather than pretending the services work |
| Underlying scene | Real Pre-Level page in modal mode; clean existing village plate in standalone mode | Existing scenery/portrait/layout differ from the old reference; standalone mode intentionally has no fake underlying navigation |
| Close/input | Isolated reference cross icon and native Button; safe-area fit | Phone interaction and hardware Back still need device verification |

The restored/full-energy captures are actual native UI states from controlled test saves. They verify display and existing clock behavior, not new reward offers. No new store, ad SDK, social service or gameplay design was added.
