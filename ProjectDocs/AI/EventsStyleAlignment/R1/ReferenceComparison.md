# Events R1 — comparison with the reference

Reference inspected: `Assets/LegendsOfRus/Resources/DesignScreens/Events.png` (941×1672). New implementation: native `Pages/Events.prefab`, live text and controls, separate Scenario artwork. The screenshot is not a runtime dependency.

| Area | Implemented | Difference / reason |
|---|---|---|
| Player/resource header | Accepted navigation header and portrait copied from the real Shop prefab; live name, level, balances and native controls | Existing shared portrait, typography and resource widths retained |
| Events plaque | New empty navy/gold/sapphire Scenario asset; live RU/EN heading | Exact filigree and beveled source lettering remain approximate |
| Tabs | Native Active, Upcoming and Completed; fitted sliced navy/gold faces | Upcoming opens by default because no event is currently scheduled; no fake notification badge |
| Four illustrations | New Firebird, Baba Yaga, merchant and ice-knight paintings guided by individual reference regions | Pose, architecture and fine detail differ; the paintings are individual assets, never screen crops |
| Card frames | New thin gold/lacquer frame with preserved corners through nine-slicing | Equal-height rows replace the reference's unequal row heights |
| Dates and availability | Live localized date-pending labels; clear empty Active/Completed states | No invented countdowns, participation or premium status |
| Rewards | Localized “Rewards to be announced” | Decorative reward icons/amounts omitted because no authoritative recipe exists; no implied grants |
| Actions | First card offers a preview of the existing EventDetail demo; remaining actions disabled as Coming soon | Existing local demo is not a live event; the EventDetail page itself has not been redesigned |
| Back and footer | Accepted circular gold Back icon; existing gold footer ornament | Back added visibly; footer states that dates/rewards will follow |
| Environment | Existing clean dusk environment reused and toned within this page | Background scenery differs from the reference, with no source-art changes |

No exact-match claim or new live-event feature claim. Existing event demo claim state and all balances remain unchanged by this page. The local detail demo still does not grant currency. A future feature phase needs an authoritative event catalog, dates, eligibility and reward recipes before participation or event rewards can be enabled. That future work is not implemented in this visual pass.

Source font identity remains unknown. Project Cyrillic serif and existing fallback fonts retained; no OS fonts distributed. Gameplay and other approved pages remain unchanged.

Detailed subsequent audit: [ReferenceFidelityReview](QA/ReferenceFidelityReview.md). R1 is a thematic match, not a close reproduction; geometry and missing visual structures require refinement.
