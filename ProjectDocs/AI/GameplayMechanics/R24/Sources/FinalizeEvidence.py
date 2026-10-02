from pathlib import Path
import json,hashlib,xml.etree.ElementTree as ET,re
base=Path(__file__).resolve().parent.parent
runs=[]
for path in sorted((base/'QA').glob('Verify*/*/results.xml')):
 r=ET.parse(path).getroot();runs.append({'path':str(path.relative_to(base)),**{k:r.attrib.get(k) for k in ['result','total','passed','failed','skipped','duration']}})
(base/'QA/Verification.json').write_text(json.dumps({'unity':'6000.5.9f1','runs':runs,'final_run':'Verify02/HaloFinal — after pooled halo suppression','earlier_broad_run':'Verify01/All — before final pooled presenter change','process_overrides':'Previously approved Vulkan; descriptor limit 4096; isolated test configuration','not_run':['EditMode','Physical Android performance'],'source_diff_check':'passed'},indent=2))
keep={p for i in json.loads((base/'QA/ReviewCaptures.json').read_text()) for p in i['frames']}
removed=[]
for p in (base/'QA').glob('Verify*/*/Journey/*.png'):
 if re.search(r'_\d+\.png$',p.name) and str(p.relative_to(base)) not in keep:
  removed.append({'path':str(p.relative_to(base)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()});p.unlink()
manifest=base/'QA/PrunedFrames.json'
if removed:manifest.write_text(json.dumps({'reason':'Unreferenced task-owned numbered frames; final review frames and before/after evidence retained','frames':removed},indent=2))
print(json.dumps({'runs':runs,'pruned':len(removed),'review_frames':len(keep)},indent=2))
