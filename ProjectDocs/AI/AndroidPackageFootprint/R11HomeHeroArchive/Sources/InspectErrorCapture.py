"""Decode the unmatched RGBA capture to locate and quantify differences without dependencies."""
from pathlib import Path
import json,struct
# Read helper definitions without executing the finished whole-run comparison.
s=Path(__file__).with_name('CompareCaptures.py');env={'__file__':str(s)}
exec(compile(s.read_text().split('rows=[];unpaired=[]')[0],str(s),'exec'),env)
BASE=s.parent.parent
old=BASE.parent/'R9FirebirdArchive/QA/PlayAccepted/LOADING/Loading_error_941x1672.png'
new=BASE/'QA/PlayAccepted/LOADING/Loading_error_941x1672.png'
def decode(path):
    cfg,raw,w,h=env['png'](path);assert cfg[b'IHDR'][8:]==bytes([8,6,0,0,0]);stride=w*4;out=bytearray();previous=bytearray(stride)
    for y in range(h):
        start=y*(stride+1);filter_type=raw[start];row=bytearray(raw[start+1:start+1+stride])
        for x in range(stride):
            a=row[x-4] if x>=4 else 0;b=previous[x];c=previous[x-4] if x>=4 else 0
            if filter_type==1:value=a
            elif filter_type==2:value=b
            elif filter_type==3:value=(a+b)//2
            elif filter_type==4:
                p=a+b-c;pa=abs(p-a);pb=abs(p-b);pc=abs(p-c);value=a if pa<=pb and pa<=pc else b if pb<=pc else c
            else:assert filter_type==0;value=0
            row[x]=(row[x]+value)&255
        out.extend(row);previous=row
    return out,w,h
a,w,h=decode(old);b,_,_=decode(new);count=0;maximum=0;total=0;bounds=[w,h,-1,-1]
for pixel in range(w*h):
    av=a[pixel*4:pixel*4+4];bv=b[pixel*4:pixel*4+4]
    if av==bv:continue
    count+=1;y,x=divmod(pixel,w);bounds=[min(bounds[0],x),min(bounds[1],y),max(bounds[2],x),max(bounds[3],y)]
    for va,vb in zip(av,bv):d=abs(va-vb);maximum=max(maximum,d);total+=d
record={'path':'LOADING/Loading_error_941x1672.png','width':w,'height':h,'changedPixels':count,'inclusiveBoundsTopLeft':bounds if count else None,'maxChannelDifference':maximum,'meanAbsoluteChannelDifference':total/(w*h*4),'geometryJsonIdentical':json.loads(old.with_suffix('.json').read_text())==json.loads(new.with_suffix('.json').read_text()),'pixelIdentical':count==0}
(BASE/'QA/ErrorCaptureComparison.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
