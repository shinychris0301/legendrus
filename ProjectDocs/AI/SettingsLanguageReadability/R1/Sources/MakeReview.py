from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
captures={}
for run in ['CaptionFitPlayMode','Baseline']:
    captures[run]=[str(p.relative_to(base)) for p in sorted((base/'QA'/run/'Captures').rglob('*.png'))]
    assert captures[run],run
html='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Settings and Language Select — readability R1</title>
<style>body{margin:0;background:#111720;color:#f0e3cd;font:16px/1.5 system-ui,sans-serif}main{max-width:920px;margin:auto;padding:24px}h1{font-size:24px}a{color:#e9c986}label{display:inline-block;margin:0 16px 12px 0}select{font:inherit;color:inherit;background:#202b3b;padding:5px;max-width:100%}figure{margin:14px 0}img{display:block;width:360px;max-width:100%;height:auto;margin:auto}figcaption{text-align:center;margin:12px}.native{overflow:auto}.native img{max-width:none}</style>
<main><h1>Settings and Language Select — readability R1</h1>
<p>Actual Unity Play Mode captures from the installed prefabs. Test saves and display sizes/safe areas are simulated; these are not physical-phone captures. Select one portrait at a time.</p>
<label>Version <select id="version"><option value="CaptionFitPlayMode">Implemented</option><option value="Baseline">Before</option></select></label>
<label>Capture <select id="choice"></select></label>
<label><input id="native" type="checkbox"> Native capture pixels</label>
<figure id="frame"><img id="capture" alt="Settings or Language Select actual Unity capture"><figcaption id="caption"></figcaption></figure>
<p>Settings: larger captions and explanation text, a parchment panel with preserved corners, and separated toggle targets at least 44 pixels high in the tested 360-pixel portrait. Native preference and unavailable-service behavior remains unchanged.</p>
<p>Language Select: larger choices and an expanded close target in both Settings and the standalone dialog. The approved Splash backdrop and existing artwork remain in use.</p>
<p><a href="TestResults.md">Test results and limits</a>. These are targeted improvements; other shared captions and Settings navigation rows are not all subject to a universal minimum text/touch size. Physical-phone readability and touch verification remain deferred.</p></main>
<script>
const captures=CAPTURES;
const version=document.getElementById('version'),choice=document.getElementById('choice'),native=document.getElementById('native'),capture=document.getElementById('capture'),frame=document.getElementById('frame'),caption=document.getElementById('caption');
function show(){capture.src=choice.value;capture.alt=choice.selectedOptions[0].text;caption.textContent=version.selectedOptions[0].text+' · '+choice.selectedOptions[0].text;frame.classList.toggle('native',native.checked);capture.style.width=native.checked?'auto':'360px';}
function populate(){const prior=choice.value.split('/').pop()||'Settings_360_RU.png';choice.replaceChildren();captures[version.value].forEach(path=>{const option=document.createElement('option');option.value=path;option.text=path.split('/').pop().replace('.png','').replaceAll('_',' ');choice.append(option);if(path.endsWith('/'+prior))choice.value=path;});show();}
version.addEventListener('change',populate);choice.addEventListener('change',show);native.addEventListener('change',show);populate();
</script></html>'''.replace('CAPTURES',json.dumps(captures))
(base/'Settings_Language_Readability_R1_Review.html').write_text(html)
print('Review capture links validated:',sum(map(len,captures.values())))
