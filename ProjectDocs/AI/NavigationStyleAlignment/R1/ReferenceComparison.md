# Navigation reference comparison

Scope approved: both top headers and navigation controls across implemented pages. The user subsequently excluded Gameplay. This revision changes eight native pages. It does not claim an exact whole-screen reproduction.

| Page | Finding and action |
|---|---|
| Home | Replaced the older generated header/resource/settings visuals with the shared reference-derived set. Preserved the accepted portrait, logo, scene lighting, Play and illustrated lower navigation. |
| World Map | Shared header; replaced the flat procedural Back arrow with the isolated gold/navy medallion. Removed the broad wood strip behind navigation, retaining a slim bottom ornament and existing Map/Tasks/Guild actions. The three older illustrations still differ in detail from the reference. |
| Chapter Map | Shared header and three-dimensional Back medallion. Removed the broad wood strip obscuring the scenery beneath the Play/Back controls. |
| Pre-Level | Same header and Back corrections as Chapter Map; retained rewards, guide, Play artwork and service behavior. |
| Failure | Shared top header, isolated source coin/gem/lightning/settings/plus artwork, consistent balance spacing. Retained the previously refined failure panels and lower actions. |
| Shop | Enlarged the existing portrait/pendant, left-aligned player labels, shared header and resource sizing. Replaced the text Back tab with a circular arrow. Gave lower category labels gold-bordered navy plaques instead of quantity plates. Kept the separate Special Offer entry and truthful purchase availability. |
| Special Offer | Shared blank header material, source coin/gem icons and circular Back control. Preserved the previously hidden upper-right energy indicator. No standalone approved reference exists; the Shop family remains its style authority. |
| Daily Reward | Enlarged the existing portrait/pendant and aligned player/resource labels. Shared header/icon set. Retained R2 Back medallion and the working seven-day schedule. |
| Victory | Reviewed its distinct three-resource header and Continue/video actions. Preserved the existing page-specific treatment; no player portrait/header was added. Fine filigree and lettering remain approximate. |
| Chapter Complete | Reviewed the source-extracted title/actions; preserved R7 geometry and repaired frame corners. |
| Pause | Reviewed its native modal. Preserved the currently rendered Gameplay underneath, as explicitly requested. The historical reference shows the older Gameplay HUD and cannot be used to force an excluded Gameplay change. |
| Splash / Loading | Reviewed the accepted FrontEndHomeAlignment captures. Preserved their page-specific controls/art. Loading has no user-facing navigation header to replace. |
| Gameplay | Explicitly excluded. No Gameplay code, prefab, assets or configuration modified. |

## Visual authority and limits

The common top-header authority is `Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png`; the same player/resource arrangement appears in Home, WorldMap, ChapterMap, PreLevel, Failure and Shop references. Initial comparisons in `../AuditR1` use older recorded Unity captures, not fresh verification. The Gameplay diagnostic there predates the explicit exclusion and uses a historical reference; it is not a design target for this change.

The five navigation icons retain original source RGB with Scenario alpha plus local contour cleanup. The new empty lacquer frame is a Scenario reconstruction, not an exact extraction. The capsule uses existing isolated Home artwork with its transparent padding trimmed. The circular Back icon reuses the already verified Scenario extraction from Daily Reward R2.

Remaining differences include portrait pose/rim engraving, exact header-end scrollwork, capsule engraving and page-specific lower illustrations. The original font identity remains unknown; live localized text continues to use the existing font resolver. Resource amounts, player level and purchase availability come from the game rather than the reference's sample numbers. No XP progress was fabricated.
