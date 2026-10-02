# Team Chat R1 — reference comparison

R1 follows the original Guild Chat hierarchy and uses original title/portrait/chest/footer artwork. It does not reproduce every source pixel.

| Area | Implemented | Remaining difference |
|---|---|---|
| Title | Original Russian plaque and lettering; live English variant | Estimated edge alpha and repaired blank English face |
| Account header | Accepted live account controls | Portrait/XP treatment and live values differ from the illustrated reference |
| Guild chest | Original chest, example level/progress and reused guild crest | Reused frame and progress plaque; no real progress/claim service |
| Messages | Six original portraits reused for eight sample rows, timestamps, parchment, help actions and system row | Live font is heavier; text/content spacing and shared background differ; source lettering removed and parchment repaired locally |
| Energy example | Live text explicitly labels the +5 gift as an example | Large blue energy medallion and decorative emoji from the reference are not reproduced |
| Composer | Source frame and native input, smile control, source emerald Send frame | Back replaces the reference chat symbol; local-only status added; soft keyboard not tested on a phone |
| Footer | Source gift/handshake/sticker assemblies with live captions | Bounded label repairs and some border/occlusion edges remain approximate; dense continuous bottom ornament omitted |
| Environment | Reused clean dark forest plate | Original navy/wood abstract surround differs |

The original green portrait dots and Help badge3 are fixed sample artwork, not live indicators. All chest/progress/messages/actions are explicitly marked as previews; local text is never transmitted. Selected fonts are approved local families, not verified source-font identities.

Inspected source and alpha assets over black, white and magenta; inspected actual Unity RU main, safe EN main and long local message evidence. No full-screen reference is used at runtime. Source upscaling adds no recovered detail. Further refinement should be bounded to actual observed differences, without regenerating the existing character artwork.
