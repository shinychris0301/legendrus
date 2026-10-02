"""Prove pixel equality from matching PNG headers and inflated scanlines; no image edits."""
from pathlib import Path
import hashlib,json,struct,zlib
BASE=Path(__file__).resolve().parent.parent
OLD=BASE/'QA/PlayBefore'
NEW=BASE/'QA/PlayFinal'
def png(path):
    data=path.read_bytes();assert data[:8]==b'\x89PNG\r\n\x1a\n'
    offset=8;payload=[];settings={}
    while offset<len(data):
        size=struct.unpack_from('>I',data,offset)[0];kind=data[offset+4:offset+8];value=data[offset+8:offset+8+size]
        assert zlib.crc32(kind+value)&0xffffffff==struct.unpack_from('>I',data,offset+8+size)[0]
        if kind==b'IDAT':payload.append(value)
        elif kind in (b'IHDR',b'gAMA',b'cHRM',b'sRGB',b'iCCP'):settings[kind]=value
        offset+=12+size
    width,height=struct.unpack_from('>II',settings[b'IHDR']);return settings,zlib.decompress(b''.join(payload)),width,height
rows=[];unpaired=[]
for p in sorted(NEW.rglob('*.png')):
    rel=p.relative_to(NEW);old=OLD/rel
    if not old.exists():unpaired.append(str(rel));continue
    a,ar,aw,ah=png(old);b,br,bw,bh=png(p);assert (aw,ah)==(bw,bh)
    proven=a==b and ar==br
    rows.append({'path':str(rel),'width':aw,'height':ah,'pixelIdentityProven':proven,'samePngSettings':a==b,'sameInflatedScanlines':ar==br,'beforeScanlineSha256':hashlib.sha256(ar).hexdigest(),'afterScanlineSha256':hashlib.sha256(br).hexdigest()})
result={'comparisonBaseline':'R17HomeDormantArtworkCleanup/QA/PlayBefore','pairedCount':len(rows),'pixelIdentityProvenCount':sum(r['pixelIdentityProven'] for r in rows),'unpaired':unpaired,'rows':rows,'physicalDeviceComparison':False,'separateEditorRuns':True,'method':'Equal PNG settings plus equal inflated filtered scanlines imply equal decoded pixels. Unequal encodings are recorded without claiming a pixel difference.'}
(BASE/'QA/CaptureComparison.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({k:v for k,v in result.items() if k not in ('rows','unpaired')},indent=2))
print('Identity unproven:',[r['path'] for r in rows if not r['pixelIdentityProven']])
