"""Summarize Unity-rendered comparisons; no PNG/source image transformation."""
from pathlib import Path
import json
import sys

BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
name=sys.argv[1] if len(sys.argv)>1 else 'Candidate512'
before=json.loads((QA/(sys.argv[2] if len(sys.argv)>2 else 'Baseline')/'RuntimeMeasurements.json').read_text())
after=json.loads((QA/name/'RuntimeMeasurements.json').read_text())
def key(row):return tuple(row[k] for k in ('page','language','screenWidth','screenHeight','hierarchy'))
a={key(r):r for r in before['rows']};b={key(r):r for r in after['rows']}
assert set(a)==set(b) and len(a)==40
geometry=[]
for k,row in a.items():
    for field in ('enabled','active','culled','hasMesh','alpha'):
        assert row[field]==b[k][field],str(k)+': '+field
    for field in ('rectPixels','meshPixels'):
        for axis in ('x','y','width','height'):
            difference=abs(row[field][axis]-b[k][field][axis])
            if difference>.02:geometry.append({'key':k,'field':field,'axis':axis,'difference':difference})
recorded=after['comparisons'];assert len(recorded)==52
# This preserved first candidate used camera clear colour behind an opaque native
# backdrop. Its page comparisons are valid; those 12 alpha captures are excluded.
alpha_valid=name!='Candidate512'
comparisons=recorded if alpha_valid else [r for r in recorded if not r['capture'].startswith('CoinAlpha')]
result={'candidate':name,'recordedComparisonCount':len(recorded),'validComparisonCount':len(comparisons),
    'nativePageComparisons':40,'alphaBackgroundComparisons':12 if alpha_valid else 0,
    'excludedInitialAlphaCaptures':0 if alpha_valid else 12,
    'geometryChangesBeyondPoint02Pixels':geometry,'maxMeanAbsoluteRgbDifference':max(r['meanAbsoluteRgbDifference'] for r in comparisons),
    'maxP99ChannelDifference':max(r['p99ChannelDifference'] for r in comparisons),
    'maxChannelDifference':max(r['maximumChannelDifference'] for r in comparisons),
    'screeningFailures':[r for r in comparisons if not r['screeningPassed']],
    'allValidComparisonsPassScreening':all(r['screeningPassed'] for r in comparisons),
    'criteria':'Per-capture coin region plus 2px margin: mean absolute RGB difference <=3/255 and p99 per-pixel maximum channel difference <=24/255. Geometry tolerance .02px. These are screening limits, not automatic visual acceptance.',
    'importedTextureDimensions':sorted({(r['textureWidth'],r['textureHeight'],r['mipCount']) for r in after['rows']}),
    'largestDifferences':sorted(comparisons,key=lambda r:r['meanAbsoluteRgbDifference'],reverse=True)[:8]}
(QA/(name+'Comparison.json')).write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({k:v for k,v in result.items() if k not in ('screeningFailures','largestDifferences')},indent=2));print('Screening failures:',len(result['screeningFailures']))
