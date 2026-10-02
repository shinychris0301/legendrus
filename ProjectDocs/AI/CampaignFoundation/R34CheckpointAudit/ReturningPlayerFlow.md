# Returning-player catch-up: review specification

Status: proposed behavior only. No recap UI, save field, automatic acknowledgement or production art was implemented in R34.

## Current behavior confirmed from source

A save completed through 200 can contain 27 unread milestones (33 lines; intro excluded). `ScreenRouter.TryShowPendingChapterFlow` opens the earliest eligible unread scene. Finishing that scene returns to the Chapter Map. The next map entry or level-start request checks again. Thus the backlog is 27 potential interruptions of navigation, not a single automatically chained 27-scene movie. The historical R22 count of 17 applied to its 100-round state.

Eligibility is a completed milestone round with its matching story trigger, not highest-unlocked or last-completed metadata alone. Scene acknowledgement happens after its last line. Interrupting a multi-line scene restarts it from the first line; earlier acknowledged scenes stay read. R34 tests cover these behaviors, including non-contiguous read history. They do not establish pacing or pointer/visual quality.

## Proposed choice

When at least three eligible unread milestones exist, offer a compact choice before entering catch-up. A normal newly earned milestone continues through the existing full dialogue.

- «Читать по порядку» / “Read in order”: retain every original scene, with an explicit return-to-map option between scenes.
- «Кратко о событиях» / “Story recap”: summarize only eligible older unread milestones. Keep the newest eligible milestone as its original scene, so the current objective and transition remain intact.
- «Позже» / “Later”: continue to the selected level or map for this visit. Leave unread history untouched. Do not immediately reopen the choice on the same navigation request.

The threshold is a design starting point requiring user/pacing validation, not a tested acceptance criterion. Preserve the accepted intro and original scenes; summaries supplement them rather than overwriting them. Offer the originals from a story-history route before treating a recap as a permanent substitute; that route also needs implementation.

## Persistence contract

Opening, switching language, leaving, cancelling or killing the app must not acknowledge any story. Only an explicit recap-finish action acknowledges the exact older eligible scene IDs represented in the displayed snapshot. Revalidate that snapshot against current content/progress before committing. An invalid or incomplete write keeps the story unread and offers retry. Never acknowledge a future scene, intro, finale or chapter completion through a recap; never pay rewards, consume energy, unlock levels or change selected progress. A scene added after the snapshot stays unread. Use the existing save journal; test interruption/reload, idempotence and write failure.

## Narrative coverage

Generate neither arbitrary summaries at runtime nor generic text based solely on the highest round. Author localized coverage by known story IDs: wounded Firebird and unidentified raven; temporary relief at well/granary/riverbank/beacon; routes between homes and well; mill/storehouse access; orchard/field access and unresolved shadow near the old wards. Recap only facts in eligible original scenes. The village is not fully healed, the raven's master is not revealed, and Chapter 1 has not ended. Its planned finale is round 500, which is not installed.

Before implementing the visual controls, review a standalone portrait draft using the approved navy/gold/parchment family and existing shared header/buttons. Label that artifact as a design preview. Runtime captures, both locales, small-screen text fit and native navigation checks are separate acceptance evidence.
