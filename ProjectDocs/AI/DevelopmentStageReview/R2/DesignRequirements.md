# Design requirements and acceptance review

The latest user approvals and root AGENTS.md take precedence over the August source-document proposals. Keep the installed Unity 6000.5.9f1 and the existing project/startup/services. This review authorizes no engine upgrade, graphics-setting change or redesign of accepted screens.

| Requirement | Current decision / implementation | Acceptance still needed |
|---|---|---|
| Visual identity | Approved Russian-fairy-tale family: navy/black lacquer, antique-gold filigree, sapphire, controlled emerald, parchment, carved wood and painterly scenery | New live states must reuse it; no generic replacement kit or brighter unrelated icon family |
| Splash / Title | ColorFix06 + PrefabFix07 accepted baseline | Preserve logo, dimensions and colors; use its actual materials for Loading alignment |
| Layout baseline | Current reference-based pages use 941×1672; old spec proposes 1080×1920 and dp/sp sizes | Do not rescale globally from the old document. Evaluate rendered phone-size type, safe areas, touch bounds and actual device density |
| Background and layers | Clean environment plates, isolated sprite edges, intentional opaque surfaces, native controls | Verify black/white/saturated backgrounds for modified cutouts; protect parchment corners, frame ornaments, progress ends and footer bounds |
| Shared assets | Shared header includes coins/gems/energy; reuse approved gameplay gems, shared booster rims/badges and existing gift/mail assets | Live bindings should replace data, not create duplicate headers or regenerate assets |
| Typography | Current font assignments, text-clarity work and Victory caption R38 retained | Exact source-font identity unverified. Broad readability is not accepted by one passing caption test; avoid tiny auto-fit and blurred bitmap enlargement |
| Localization | RU primary; EN selection persisted; campaign locale catalogs populated | Numerous page labels still use inline RU/EN strings. Externalization and an all-page language audit remain, including live errors, prices and plural forms |
| Original lettering | User explicitly approved extracting certain titles/buttons with original Russian text; several pages switch to blank art plus live EN text | Record as deliberate visual exceptions. Preserve approvals; dynamic values must be live/localizable. Verify alternate text, scalable rendering and future-language fallback before release |
| Navigation | Home shortcuts to ranking/team/friends and native back controls implemented; preserve selected routes | Recheck authenticated/unauthenticated transitions, reconnect and expired sessions without duplicate Canvas/EventSystem |
| Honest service states | Preview/unavailable notes deliberately shown on example rankings, events, friends, mail, offers and login | Remove only when corresponding live behavior works; real error/loading/empty/expired states must replace sample data |
| Phone interaction | Prior safe-area and responsive audits exist; native controls/raycast tests retained | Current Android touch targets, keyboard/chat composition, long names, large balances, 16:9–20:9 and cutouts |
| Performance | Lightweight mobile effects and readable board; current artwork accepted | Device frame time, overdraw, memory, startup/download/build size and network latency budgets need measured baselines |
| Audio and haptics | Local cues and settings exist | Chapter music/ambience/voice scope, volume mix, device vibration and interruption behavior |
| Effects | Existing effects remain; all further animation/effect polish is explicitly last | Final review must cover creation finish, distinct activation/combo mechanics, colored gem shards, line travel, stage-specific obstacle damage, controlled glow; do not restore rejected circular board rings |

## Conflicts to resolve without undoing approvals

- Original requirements suggest warm red/gold/forest-green UI; later page references and approved navy/gold system govern current work.
- The source spec's Russian-only release statement is superseded by implemented/user-reviewed RU/EN behavior. Full externalization remains incomplete even where switching works.
- Reference screenshots show balances, reward quantities, timers, rankings and prices. They are visual examples, not economy authority or evidence of active seasons.
- “Hero choice,” “pre-level booster toggles,” and “continue” on a reference are requested behavior, not proof the underlying system is present. Maintain explicit service/rules tasks.
- Full-panel Tutorial extraction was specifically requested. Preserve that accepted composition and substituted gameplay gems; assess text scalability/localization as a documented exception rather than silently replacing it with a full-screen hotspot implementation.
- The old documents' proposed engines, backend vendors, ad networks and price ranges are options. No provider or price is selected by quoting them.

## Design acceptance for the live phase

For each connected page: existing native prefab and owner builder stay in agreement; real values and operations are wired; RU/EN success/failure/loading/empty/offline states are reviewable; visible and actual touch bounds fit phones; unavailable functionality remains truthful. Capture **actual Unity/device output**, label fixture/mock/offline previews, and keep a portable evidence set. A working button or a static screenshot alone does not pass the feature.

Use the existing approved page unless a new flow is genuinely required. Any new recap/account-conflict/transaction-recovery layout gets a standalone portrait design review before implementation. This audit itself contains no new visual design approval.
