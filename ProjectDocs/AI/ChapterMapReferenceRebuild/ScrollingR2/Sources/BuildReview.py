from pathlib import Path
import html
b=Path(__file__).resolve().parent.parent
views=[
('QA/ChapterMap/ChapterMap_941x1672_RU.png','Unity · Chapter 1 / Russian'),
('QA/ChapterMap/ChapterMap_941x1672_EN.png','Unity · English'),
('QA/ChapterMap/ChapterMap_576x1280_safearea_RU.png','Unity · tall phone / safe area'),
('QA/ChapterMap/ChapterMap_Progress_941x1672_RU.png','Unity · earned stars fixture'),
('QA/ChapterMap/ChapterMap_SectionBoundary_RU.png','Unity · section boundary'),
('QA/ChapterMap/ChapterMap_End500_RU.png','Unity · final planned positions'),
('Sources/Scenario/chapter01/booster_medallion_trial01.png','Scenario · clean Chapter 1 painting'),
('../../WorldMapReferenceRebuild/DesignR1/Assets/region_01.png','Approved World Map Chapter 1 source'),
('QA/VFX/Vfx_1.png','Unity VFX fixture · match facets'),
('QA/VFX/Vfx_2.png','Unity VFX fixture · bomb impact'),
('QA/VFX/Vfx_3.png','Unity VFX fixture · row activation'),
('QA/VFX/Vfx_4.png','Unity VFX fixture · column activation'),
('QA/VFX/Vfx_5.png','Unity VFX fixture · Firebird activation')]
options=''.join(f'<option value="{html.escape(p)}">{html.escape(label)}</option>' for p,label in views)
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Chapter Map — scrolling and chapter scenery</title><style>body{margin:0;background:#080e17;color:#ead6ad;font:16px/1.5 system-ui,sans-serif}header,footer{max-width:850px;margin:22px auto;padding:0 18px}h1{font-size:24px;margin-bottom:6px}p{margin:8px 0}select{background:#182438;color:#ffe2a8;padding:10px;border:1px solid #947343;border-radius:5px;max-width:100%}main{max-width:520px;margin:20px auto}img{width:100%;height:auto;display:block}a{color:#a9d9ff}.note{color:#b4bbc7;font-size:14px}</style><header><h1>Chapter Map · scrolling and chapter scenery</h1><p>Chapter 1’s approved World Map village expanded through Scenario into a portrait map. Shared header, original controls, seven visible rounds, recycled level buttons and a current-level shortcut.</p><select id="view" aria-label="Review image">OPTIONS</select><p class="note">Actual Unity captures use isolated test saves. 500 planned positions; only ten authored rounds are playable. Effects images use a controlled 8×9 QA stress fixture with hit-area gems hidden to expose the effect; production remains 8×7. This is a first effects pass, not a full gameplay visual rebuild.</p></header><main><img id="capture" src="QA/ChapterMap/ChapterMap_941x1672_RU.png" alt="Selected native map capture or clearly labelled source and effect fixture"></main><footer><a href="ImplementationReport.md">Implementation and verification report</a><p class="note">One Scenario job, 13 CU. Original World Map painting retained. Portrait scenery is a new composition, repeated along the scrolling route; other chapter expansions and 500-round catalog/story migration remain subsequent work.</p></footer><script>document.querySelector('#view').addEventListener('change',e=>document.querySelector('#capture').src=e.target.value);</script></html>'''
(b/'ChapterMap_ScrollingAndScenery_R2_Review.html').write_text(page.replace('OPTIONS',options))
