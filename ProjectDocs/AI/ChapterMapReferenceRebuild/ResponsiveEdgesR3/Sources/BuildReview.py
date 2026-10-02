from pathlib import Path
import html
b=Path(__file__).resolve().parent.parent
views=[
('QA/ChapterMap/ChapterMap_576x1280_safearea_RU.png','Unity · tall phone / safe edges'),
('QA/ChapterMap/ChapterMap_941x1672_RU.png','Unity · Russian / 11 rounds'),
('QA/ChapterMap/ChapterMap_941x1672_EN.png','Unity · English'),
('QA/ChapterMap/ChapterMap_1080x2400_safearea_RU.png','Unity · tall phone / side and vertical insets'),
('QA/ChapterMap/ChapterMap_Progress_941x1672_RU.png','Unity · saved stars fixture'),
('QA/ChapterMap/ChapterMap_SectionBoundary_RU.png','Unity · section boundary / unpublished notice'),
('QA/ChapterMap/ChapterMap_End500_RU.png','Unity · planned round 500 / unpublished notice'),
('Sources/Scenario/chapter01_landmarks/booster_medallion_trial01.png','Scenario · new landmark scenery'),
('../ScrollingR2/QA/ChapterMap/ChapterMap_576x1280_safearea_RU.png','Previous R2 · comparison only'),
('../../WorldMapReferenceRebuild/DesignR1/Assets/region_01.png','Approved World Map Chapter 1 painting')]
options=''.join(f'<option value="{html.escape(p)}">{html.escape(label)}</option>' for p,label in views)
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Chapter Map — staggered route and safe edges</title><style>body{margin:0;background:#080e17;color:#ead6ad;font:16px/1.5 system-ui,sans-serif}header,footer{max-width:850px;margin:22px auto;padding:0 18px}h1{font-size:24px;margin-bottom:6px}p{margin:8px 0}select{background:#182438;color:#ffe2a8;padding:10px;border:1px solid #947343;border-radius:5px;max-width:100%}main{max-width:520px;margin:20px auto}img{width:100%;height:auto;display:block}a{color:#a9d9ff}.note{color:#b4bbc7;font-size:14px}</style><header><h1>Chapter Map · landmark scenery and staggered rounds</h1><p>A richer Chapter 1 village with river, bridge, chapel and ancient oak. Eleven staggered rounds at a glance. Header and footer anchored to the phone’s safe-area edges; original controls keep their proportions.</p><select id="view" aria-label="Review image">OPTIONS</select><p class="note">Actual Unity captures use isolated test saves. 33 targeted Unity checks passed. The map has 500 planned positions; only ten authored rounds are playable. The “Previous R2” option shows the earlier layout for comparison.</p></header><main><img id="capture" src="QA/ChapterMap/ChapterMap_576x1280_safearea_RU.png" alt="Selected native map capture or clearly labelled source and effect fixture"></main><footer><a href="ImplementationReport.md">Implementation and verification report</a><p class="note">One additional Scenario job, 13 CU. Earlier art is preserved. Scenery is a new composition derived from the approved World Map painting. Other chapter backgrounds, content authoring and campaign progression migration remain subsequent work.</p></footer><script>document.querySelector('#view').addEventListener('change',e=>document.querySelector('#capture').src=e.target.value);</script></html>'''
(b/'ChapterMap_StaggeredRoute_R3_Review.html').write_text(page.replace('OPTIONS',options))
