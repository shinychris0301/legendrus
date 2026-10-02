# Returning-player story recap — bounded next implementation

The review found an optional narrative access gap: acknowledged scenes cannot be reread through the current normal routes. Progress restoration and pending-scene routing already exist. This document defines the proposed next implementation; **no recap control, reader or prefab change is installed by R1**. Its usefulness still needs human feedback.

## Entry and content

Add an optional live-text story action within the existing Profile History section, preserving its level-statistics paging and all existing controls. Do not force a welcome modal on startup, change the saved level selection after a replay, or infer an absence interval from the energy clock. Use established local fonts, panels and native controls; no new artwork or generated story summaries.

For the first delivery, show the latest acknowledged scene in catalogue order and allow browsing earlier acknowledged scenes. “Latest” means narrative order, since the current ledger contains scene IDs rather than acknowledgement timestamps. Use the installed Chapter 1 catalogue and its existing RU/EN lines, including the current round-500 finale; never substitute the archived ten-round catalogue.

Expose only known, acknowledged scenes. Require the corresponding completed-round evidence for a milestone; the intro requires its own acknowledgement. Empty history has truthful localized empty-state text. Ignore unknown archived IDs, duplicate IDs and ineligible acknowledgements without editing the ledger. Do not expose future/unread scenes, Chapter 2, victory/failure reaction lines, or unfinished dialogue as recap content.

## State and routing contract

Create a read-only projection and separate local `StorySequenceRuntime`. Reuse the actual `ProductionDialogueView` through its explicit line/scene/speaker providers, preserving its current art and localized text. The existing example in CampaignDevelopmentUI demonstrates provider-based rendering, but its `coordinator.CompleteStory` completion callback writes acknowledgements and must not be copied into recap.

Recap advance, finish, cancel, locale change and reentry must leave normal `StoryService` sequence/position unchanged. Do not call its `AdvanceDialogue`, `CompleteStoryScene`, campaign `AcknowledgeStory`, `AcknowledgeChapterComplete`, level selection, grants, energy spending or run operations. In particular, the finale's ordinary `ChapterComplete` return route must not be dispatched by the recap reader.

Finish and Back return to the originating Profile History section with its page preserved. The reader owns its navigation mode only for the current visit; clear its callbacks/state on exit. Preserve existing pending-scene gates, retained-failure/continuation guards, modal/input blockers and one-tap behavior. Recap must never replace the normal progression path or silently select a different save profile.

Production uses the normal `PlayerSaveData.completedStoryScenes` IDs. Development campaign sessions use catalogue-scoped ledger keys and legacy bridge rules. Do not merge these storage formats, migrate saves or point the production reader at the development session. R2 should support the production flow and its existing disposable QA namespace; extending the separate campaign development UI is outside this first delivery.

## Owning files and verification

Likely owning touchpoints: ProductionProfileView/Runtime and active Profile prefab for the live entry; ScreenRouter for reader mode and return ownership; a narrow read-only story projection/reader component; current ProductionDialogueView provider hooks. Any prefab revision/bindings changed must be updated in both active serialization and the owning builder. Preserve existing object IDs, GUIDs/metas, Splash, fonts/artwork and all current story/level/localization resources. Append ScreenId values only if a separate route is actually needed; preserve existing serialized numeric identities.

Verify empty/read/unknown/duplicate/sparse/out-of-order ledgers, old finale IDs, eligibility boundaries, all 58 scenes' existing RU/EN lines, and no future/unread disclosure. Exercise real native entry/advance/Back, rapid repeated taps, relocalization and origin paging restoration at small/tall safe-area portraits. Compare full save/journal and active normal story state before/after repeated recap, including the finale and a pending normal scene. Keep normal clock-based energy refresh distinct from any recap mutation. Rerun the affected pending-story, chapter-completion, Profile History and save-isolation cases.

For review provide one standalone native portrait and focused evidence. Physical readability, Android activation/lifecycle and human Chapter 1 pacing remain separate pending milestones. Do not build another APK merely to document the proposal; package after the native feature is implemented and verified. Phone execution/hosting remain deferred; final gameplay effects stay last.
