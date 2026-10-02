from pathlib import Path
import csv, json

BASE=Path(__file__).resolve().parent.parent
verified=BASE/'QA/PlayVerified'
result=json.loads((verified/'summary.json').read_text())
assert result['exitCode']==0 and result['result']['result']=='Passed'
rows=[]
for quantity in ('0','999','1000','9999','999999','2147483647'):
    for language in ('ru','en'):
        for width in (360,576,941,1080):
            name=f'Stock_{quantity}_{language}_{width}'
            data=json.loads((verified/('CaptionMeasurements_'+name+'.json')).read_text())['captions']
            assert len(data)==4
            assert (verified/'GAMEPLAY'/(name+'.png')).exists()
            rows.append({'quantity':quantity,'language':language,'width':width,
                         'allExactCharactersVisible':all(r['visibleCharacters']>=len(r['text']) and r['lines']==1 for r in data),
                         'minimumActualGlyphHeightPixels':min(r['glyphBounds']['height'] for r in data),
                         'maximumActualGlyphWidthPixels':max(r['glyphBounds']['width'] for r in data),
                         'nominalFontPixelsBeforeNumericMeshFit':min(r['fontPixels'] for r in data),
                         'capture':f'QA/PlayVerified/GAMEPLAY/{name}.png'})
assert len(rows)==48 and all(r['allExactCharactersVisible'] for r in rows)
assert min(r['minimumActualGlyphHeightPixels'] for r in rows if r['quantity'] not in ('0','999'))>=7.5
comparison=[]
for quantity in ('9999','2147483647'):
    data=json.loads((verified/f'CaptionMeasurements_Stock_PreviousCircular_{quantity}.json').read_text())['captions']
    current=next(r for r in rows if r['quantity']==quantity and r['language']=='ru' and r['width']==360)
    old=min(r['glyphBounds']['height'] for r in data)
    comparison.append({'quantity':quantity,'oldGlyphHeightPixels':old,
                       'newGlyphHeightPixels':current['minimumActualGlyphHeightPixels'],
                       'heightRatio':current['minimumActualGlyphHeightPixels']/old,
                       'controlScope':'Actual Unity capture with the preceding circular-count settings applied temporarily in memory; not a historical binary run'})
(BASE/'QA/StockMeasurements.json').write_text(json.dumps({'configurations':rows,'comparison':comparison,
    'scope':'Simulated Editor viewports, synthetic quantities; actual mesh ink measured after fitting, not human or device acceptance'},indent=2)+'\n')
with (BASE/'QA/StockMeasurements.csv').open('w',newline='') as stream:
    writer=csv.DictWriter(stream,fieldnames=list(rows[0]));writer.writeheader();writer.writerows(rows)
html='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>Gameplay exact stock readability R5</title><style>
body{background:#101824;color:#e8dfce;font:16px/1.5 system-ui,sans-serif;margin:0}main{max-width:1050px;padding:24px;margin:auto}h1{color:#dfbf78;font-size:26px}a{color:#dfbf78}label{display:inline-block;margin:0 12px 12px 0}select{font:inherit;background:#213047;color:#fff;padding:8px;border:1px solid #8a99aa;border-radius:4px}img{max-width:100%;height:auto;display:block;border:1px solid #665232;margin:18px auto}p{max-width:85ch}.small{font-size:14px;color:#b7c5d8}
</style><main><h1>Gameplay · exact inventory counts</h1>
<p>Counts from 1,000 use the existing lower card well for a wider live stock face. Counts up to 999 retain the circular badge. Exact quantities, fonts, controls and artwork are retained; the active prefab and owner use revision 24.</p>
<p class="small">One actual Unity Play Mode portrait at a time · simulated safe areas · synthetic quantities for fit measurement. Previous-circular controls were applied temporarily in the same current scene. These captures are not human playtests or physical-phone acceptance.</p>
<label>Quantity <select id="quantity"><option>0</option><option>999</option><option value="1000">1 000</option><option value="9999" selected>9 999</option><option value="999999">999 999</option><option value="2147483647">2 147 483 647</option></select></label>
<label>Language <select id="language"><option value="ru">Russian</option><option value="en">English</option></select></label>
<label>Viewport <select id="width"><option>360</option><option>576</option><option>941</option><option>1080</option></select></label>
<label>Fit <select id="fit"><option value="current">Current</option><option value="previous">Previous circular control</option></select></label>
<p id="measurement" aria-live="polite"></p><img id="portrait" alt="Actual Unity Gameplay portrait with live exact stock quantities">
<p>At 360×640, 9 999 grows from 3.94 to 11.00 pixels of actual glyph height; Int32.MaxValue grows from 1.61 to 7.61 pixels. The maximum remains small and does not establish a universal readability minimum. Existing 999 fit remains unchanged. No artwork/font generation or copying.</p>
<p><a href="TestResults.md">Results and limitations</a> · <a href="QA/StockMeasurements.csv">Measured stock matrix</a> · <a href="QA/Scope.json">Change scope</a></p>
</main><script id="data" type="application/json">__DATA__</script><script>
'use strict';const data=JSON.parse(document.getElementById('data').textContent),el=id=>document.getElementById(id);
function render(){const q=el('quantity').value,l=el('language').value,w=Number(el('width').value),old=l==='ru'&&w===360&&['9999','2147483647'].includes(q);
el('fit').options[1].disabled=!old;if(!old)el('fit').value='current';const previous=el('fit').value==='previous';
const row=data.configurations.find(r=>r.quantity===q&&r.language===l&&r.width===w);
el('portrait').src=previous?`QA/PlayVerified/GAMEPLAY/Stock_PreviousCircular_${q}.png`:row.capture;
const height=previous?data.comparison.find(r=>r.quantity===q).oldGlyphHeightPixels:row.minimumActualGlyphHeightPixels;
el('measurement').textContent=`Actual glyph height: ${height.toFixed(2)} pixels · all exact characters retained · ${previous?'previous circular control':'current native fit'}`;}
['quantity','language','width','fit'].forEach(id=>el(id).addEventListener('change',render));render();
</script></html>'''
embedded=json.dumps({'configurations':rows,'comparison':comparison}).replace('<','\\u003c')
(BASE/'GameplayStock_R5_Review.html').write_text(html.replace('__DATA__',embedded))
print('48 final native configurations, 192 stock observations, two circular controls; standalone review written')
