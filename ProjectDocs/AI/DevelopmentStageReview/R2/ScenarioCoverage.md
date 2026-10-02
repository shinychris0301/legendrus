# Scenario coverage and missing structure

The full story describes an arc; it is not a complete executable scenario. The approved production target is **20 chapters × 500 rounds**. The older “10,000+ rounds per chapter / 200,000+ total” proposal is superseded. Chapter 1 contains 200 installed rounds, 28 scenes (including intro), 27 milestones and 400 victory/failure reactions. Its completion field is null and the finale remains planned at round 500.

The installed package and framework are real progress since the R1 review's ten-level baseline. Do not repeat its old counts, missing-English claim or premature ten-round-finale assumption. See [fresh inventory](QA/CurrentInventory.json).

## Chapter coverage

| Chapters | Source narrative / mechanics | Delivered scope and gap |
|---|---|---|
| 1 Native Village | Wounded Firebird, Matching Magic, gradual restoration, watcher, departure into forest | Rounds 1–200 and intermediate scenes; 201–500, complete village restoration, departure/finale and next-chapter release missing |
| 2 Dense Forest | Leshy, illusions/moving trees, night defense, sacred-oak gift | World-map definition/preview; levels, timed/night rules, encounters, ally reward and scenes not delivered |
| 3 Yaga's Hut | Three distinct trials, mortar gift, mentor and Koschei warning | Definition/preview; trial rules, acquisition and dialogue content missing |
| 4 Firebird Kingdom | Captive birds, dark vines, multi-stage queen purification | Definition/preview; rescue state, boss phases and companion restoration missing |
| 5 Milky Shores | Vasilisa, friendship/romance choices, magic doll | Definition/preview; relationship state, branch/rejoin and companion booster missing |
| 6 Underwater Kingdom | Currents/floating tiles/pearls, rescue, Sea Tsar alliance | Definition/preview; new board rules, rescue choice and boss absent; existing SeaPearl adds moves, not literal time freeze |
| 7 Urals | Ilya training, chains/stone/platforms, golem, strength | Basic chains exist elsewhere; chapter encounter/physics/platform/boss/ability sequence missing |
| 8–9 Taiga / Aurora | Bear cub rescue, blizzard survival, Morozko/Snegurochka, frost alliance | Generic survival/ice is not these authored scenarios; chapter levels, restoration and character arcs missing |
| 10 Moscow | Siege, three bogatyrs, multi-character climax | Map definition only; siege/party mechanics and payoff missing |
| 11–12 Northern Venice / Taurida | Relationships, spirit ball/librarian, prophecy and Chernobog vision | Chapter scripts, choice consequences and playable content missing |
| 13–15 Caucasus / Baikal / Far East | Honor choices, vertical boards, truth about greed, eastern alliance and Zmey | Branching/scrolling boards, revelation continuity, multi-head boss and all chapter content missing |
| 16–18 Roots / Trunk / Heavenly Halls | Sacrifice, reunion of allies, council/blessing/ultimate light | Persistent sacrifice/return state, ally prerequisites, gameplay rewards and scenes missing |
| 19–20 Dark Realm / Rebirth | Koschei nested objects, Chernobog finale, return/restoration/epilogue | Nested-object boss, complete finale and postgame transition missing |
| Postgame | Endless challenges, seasonal stories, Side Legends of 50–200 rounds | Source proposals only; no delivered season cadence or repeatable content pipeline acceptance |

## Structural omissions needing explicit contracts

1. **Milestone pacing and content endings.** A 50-round technical sector is not automatically a story act. Define introduction, escalation, relief and climax per chapter; keep installed-content exhaustion distinct from victory/finale. Do not unlock absent content or market 20 playable chapters.
2. **Returning-player catch-up.** Current 200-round content can yield 27 unread milestones / 33 lines excluding intro. They can interrupt successive map/start requests; they are not automatically one continuous movie. R22's 17-scene count is stale. The R34 recap/read/later proposal remains unimplemented and not user-validated. It needs original-history access, exact represented-scene acknowledgement, no reward/progress effects, and interruption-safe persistence. [R34 specification](../../CampaignFoundation/R34CheckpointAudit/ReturningPlayerFlow.md).
3. **Choice and relationship schema.** Stable choice IDs, preconditions, consequences, branch rejoin points, companion relationships and migration are missing. Cosmetic dialogue choices and persistent consequential choices need separate definitions. Online/cloud merge must preserve whichever state is adopted.
4. **Hero identity.** Source requests customizable name, male/female appearance and voice. The current presentation uses Ivan. Define the intended launch subset before changing accepted portraits or dialogues; name/account identity is not hero equipment ownership.
5. **Companion/booster unlock progression.** Define acquisition round, stock versus permanent ability, tutorial, upgrade and story dependencies. Distinguish match-generated special pieces from inventory consumables. Preserve the four working tools; preview icons do not authorize new combat rules.
6. **Boss and chapter rules.** Define board transitions, health/stages, damage sources, win/loss, persistence and rewards before boss art. Timed defense, currents, scrolling boards, nested objects and multi-character play have no complete implemented scenario contracts.
7. **Restoration.** Narrative claims of returning light need persisted region/companion restoration states, scenery variants and map feedback. Existing win reactions do not constitute a restoration system.
8. **Continuity.** Reconcile approved early Dobrynya with later bogatyr introductions; preserve accepted early scenes. Track the crow's allegiance and Nightingale's first encounter. Resolve the roots-versus-final-dark-realm battle geography and why the bogatyrs appear as spirits later. Avoid revealing Baikal's truth early or calling the village fully healed at round 200.
9. **Rewards and losses.** Specify first-clear versus repeat rewards, chapter finale grants (if any), failure continuation, event participation and companion rewards. Do not infer a bonus from a celebration panel. Prices and defeat pressure in the source are proposals; user found earlier difficulty normal and did not request monetization-driven difficulty increases.
10. **Localization/editorial.** Key coverage is not literary acceptance. R36–42 revised 51–100 reactions; earlier repeated failure lines and later repeated mechanical hints remain. Global glossary, gender/voice variants, authored recap, legal/service copy and native-language pacing need review.
11. **Live-story connections.** Events, mail and seasons need canonical ownership and expiry rules: what changes the main story, what remains optional, and whether a missed seasonal event can block progression. Keep chat and SMS from silently becoming chapter gates.

The live-services milestone does not require authoring all 20 chapters first. Keep 1–200 as the integration content set, collect human difficulty evidence for 31–200, and return to 201–500 only after its narrative/level plan is defined. Final effects stay last.
