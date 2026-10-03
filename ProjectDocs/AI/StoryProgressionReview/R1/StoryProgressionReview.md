# Implemented story cadence and visual coverage

Reviewed 2026-10-03 against the active Chapter 1 package, normal ScreenRouter/StoryService, native Dialogue/ChapterComplete/Victory/Fail views, active prefabs and retained Unity captures. This is a read-only product audit; no new gameplay, Unity run, asset generation or visual acceptance occurred.

## What the player experiences

Chapter 1 is «Родная Деревня» (Native Village), covering rounds 1–500. Dobrynya tells Ivan that the ancient fire is fading and darkness threatens the village. Ivan discovers a wounded Firebird, follows signs of a hostile raven and river shadow, and helps restore light, safe paths and protective wards. At the finale the village is safe enough to leave; Ivan promises to return and sets out for the forest with the Firebird. This is the implemented village story, not the whole twenty-chapter source-document saga.

| When | Narrative presentation |
|---|---|
| First opening of the selected level, before round 1 for a new player | Three opening dialogue lines: Dobrynya → Ivan → Dobrynya. `OpenSelectedLevel()` gates entry through the unread intro before PreLevel. The trigger's name `first_launch` does not mean an automatic cutscene on application startup. |
| After wins at rounds 1, 2, 3, 4, 6, 8, 9 and 10 | Early story scenes establish the Firebird, apple blossom, raven, elder's request, fading embers, river shadow, Living Flame and recovered hearth. Rounds 5 and 7 have no separate milestone dialogue. |
| After rounds 20, 30, 40 … 490 | One milestone every ten rounds. Nearly all later scenes are a single line about the next village task; these are brief dialogue updates, not long cinematics or a distinct cinematic every fifty rounds. |
| After round 500 | Five finale lines, then the native Chapter Complete page with actual chapter progress/already received rewards and a coming-soon Chapter 2 teaser. |
| Optional Profile → History story reader | Replay already acknowledged/earned scenes; latest opens first, Earlier/Later browse and Continue advances. Reading history does not advance normal progression or award rewards again. |

The normal sequence is **win → Victory → Continue → eligible Dialogue → ChapterMap**, with the finale returning to ChapterComplete. Story does not interrupt a live board mid-round. Each line is advanced by the visible Continue button, without a story timer. Scenes are acknowledged after their final line and are not automatically shown again merely because a completed round is replayed. Unread earned scenes can be offered on subsequent entry; interrupted scenes remain unread. No dedicated Skip control was identified in the active Dialogue bindings.

There are **58 scenes: one intro plus 57 post-win milestones, with 70 total dialogue lines**. Fifty scenes have one line, six have two, the intro has three and the finale has five. RU/EN text is present. A full per-scene trigger/text/visual inventory is in [StoryCadence.csv](StoryCadence.csv).

Victory comments are separate from main plot progression: the active package contains one victory and one failure reaction for each of 500 rounds. Normal Victory uses the localized reaction, subject to its status-message visibility. **Do not claim that both comments currently appear after every outcome:** the current native Fail view hides Reaction by default (`inspected=-1`), and when a booster is inspected it replaces that field with booster instructions. Failure reaction text is authored but does not presently receive a visible default presentation in that view. This is a source finding, not a freshly reproduced runtime failure; no fix is made here. Losing does not earn a new plot milestone.

## Chapter 1's ten story sections

Each section has updates at ten-round boundaries; these divisions describe the text, not ten installed background sets.

| Rounds | Main story work |
|---|---|
| 1–50 | Meet the Firebird, discover the threat, recover the hearth/well/granary/river access and light a beacon. |
| 51–100 | Restore access to the far street, courtyards, hearths and well. |
| 101–150 | Secure the mill, storehouse and supply routes. |
| 151–200 | Clear orchard/field paths and reconnect them with homes. |
| 201–250 | Follow river traces, reach the old wards and secure the ford. |
| 251–300 | Reconnect ward, well and house paths. |
| 301–350 | Explore the slope, patterned stone and ridge; strengthen the hillside ward. |
| 351–400 | Evening patrol of village roads, granary and outer lights. |
| 401–450 | Open the approaches to the forest road and check remaining village paths. |
| 451–500 | Farewells, final wards, departure from the village. |

## Which visuals are actually installed

| Story component | Implementation and limit |
|---|---|
| Opening | Native Dialogue revision 2 uses the carved hall, isolated Ivan/Dobrynya sprites, parchment panel, localized live dialogue and working Continue. Hall composition is specific to the intro. |
| Subsequent dialogue | Same native screen switches to the shared outdoor plate from `ProductionUI/LoadingReferenceAligned/clean_environment`. The field is named Forest, but the inspected capture shows village/wooded scenery. It does not select an orchard, mill, river, hillside or before/after village image by milestone. |
| Speaking characters | Ivan, Dobrynya and Firebird artwork is bound. Non-intro lines show the matching portrait for those three speaker keys. Elder/raven lines use the background and live speaker/text without a portrait. Narrator also uses scenery without a character portrait; that can be an intentional narration design. The code avoids falsely labeling a reused portrait as another speaker. |
| Finale | Five lines reuse the outdoor dialogue design, switching among Ivan/Firebird/Dobrynya. No separate departure illustration or cinematic selector is bound. |
| Chapter Complete | Native revision 9 has a distinct village completion composition with Ivan/Firebird, rewards summary, controls and forest teaser. This is implemented UI art, not a persistent world restoration system. |
| Later chapters | Twenty chapter definitions/source story plans exist. The active catalog only loads a complete Chapter 1 package; Chapter 2 remains unavailable. A teaser/map representation does not establish implemented later-chapter dialogue or location art. |

**Visual conclusion: story text/trigger coverage exists for all Chapter 1 milestones; distinct visual coverage for each narrative location/event is not complete.** The implementation uses a reusable reader with two dialogue environments and three character sprites, plus the separate chapter-completion design. It does not implement 58 individually illustrated story scenes, progressive construction/decorating or a visually changing restored village.

Retained **actual Unity Editor captures**, inspected during this review (not new captures or physical-phone evidence): [opening](../../DialogueReferenceRebuild/R1/UnityCaptures/Dialogue_941x1672_RU.png), [round-2 narrator recap](../../ReturningPlayerRecap/R2/QA/NativeEntryVerified/Captures/LOR_RECAP_CAPTURE_DIR/Recap_Latest_RU.png), [chapter completion](../../ChapterCompleteReferenceRebuild/PhoneCaptionsR2/QA/PlayMode/Captures/LOR_CHAPTER_COMPLETE_CAPTURE_DIR/ChapterComplete_Current_1080_ru.png).

## Evidence and next decisions

[Source audit](AuditEvidence.json) validates the exact cadence, every milestone's level trigger, locale keys, scene counts and the active prefab's hall/outdoor/three-character references. This is a JSON/source/reference check, not Unity compilation or runtime verification. Earlier R7/recap/Dialogue Unity tests remain historical; phone acceptance remains open. Existing uncommitted RuStore review artifacts and continuity edits are retained.

The user's question does not authorize creating new story art or changing cadence. Before adding art, decide which milestones deserve distinct location/character illustrations, then implement those mappings through the existing native screen/owner and preserve the approved art family. Before shipping, review the sparse one-line pacing with human players and decide whether/how failure reactions should appear. Existing next implementation remains the guarded fixed-preset APK refresh; phone/hosting deferrals and effects-last preference remain intact.
