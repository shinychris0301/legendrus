"""Reassemble existing R16 gem samples; discard earlier colliding timestamp segment."""
from pathlib import Path
from PIL import Image
import json
root=Path(__file__).resolve().parents[1]
src=root.parent/'R16/QA/Verify02/All/Journey'
segments=[]
for row in (src/'Gems_Frames.tsv').read_text().splitlines():
    index,seconds=row.split('\t');index=int(index)
    if index==0: segments.append([])
    segments[-1].append((index,float(seconds)))
segment=segments[-1]
prefix='Mechanics_SpectacleR15_Gems_'
paths=[src/(prefix+'Before.png')]+[src/(prefix+f'{i:02}.png') for i,t in segment]+[src/(prefix+'After.png')]
durations=[600]+[max(20,round((segment[i+1][1]-t)*1000)) if i+1<len(segment) else 100 for i,(_,t) in enumerate(segment)]+[900]
frames=[Image.open(p).convert('RGB') for p in paths]
frames[0].save(root/'Captures/R16_Gems_Reassembled.webp',save_all=True,append_images=frames[1:],duration=durations,loop=0,quality=90,method=4)
(root/'QA/BaselineReassembly.json').write_text(json.dumps({'source':'Existing R16 Unity PNGs; no new capture','timestampSegments':[len(s) for s in segments],'selectedSegment':'last reset-to-zero segment; matches Spectacle test samples','frames':[str(p.relative_to(root.parent)) for p in paths],'durationsMs':durations,'limitation':'Sampled intervals with added review holds and final settled-state jump; not continuous gameplay recording.'},indent=2))
print(f'Reassembled {len(frames)} existing portraits; timestamp segment lengths: {[len(s) for s in segments]}')
