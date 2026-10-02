from pathlib import Path
from PIL import Image
import json,hashlib
b=Path(__file__).resolve().parent;s=Image.open('ProjectDocs/AI/EventDetailReferenceRebuild/R1/Sources/OriginalEventDetailReference.png').convert('RGBA')
p=b/'BeforeAssetsHashes.json'
if not p.exists():p.write_text(json.dumps({str(p):hashlib.sha256(p.read_bytes()).hexdigest() for p in Path('Assets').rglob('*') if p.is_file()},indent=2))
# Atlas coordinates retain native pixel resolution for source RGB restoration.
items={
'title':([154,90,791,443],[20,20]),'play':([211,1495,733,1668],[20,407]),'activate':([476,1410,896,1484],[20,610]),
'chest0':([56,883,195,991],[20,740]),'chest1':([225,880,366,991],[200,740]),'chest2':([389,869,542,991],[380,740]),'chest3':([563,871,714,991],[570,740]),'chest4':([731,856,892,991],[760,740]),
'gem_blue':([88,1067,171,1159],[710,30]),'gem_red':([258,1065,337,1159],[820,30]),'feather':([426,1065,507,1159],[710,160]),'potion':([594,1063,671,1159],[820,160]),'medal':([755,1063,843,1159],[710,290]),
'premium_chest':([459,1290,610,1405],[580,440]),'pass':([791,1296,870,1405],[800,440]),'hourglass':([261,690,294,735],[890,290]),
}
a=Image.new('RGBA',(1024,1024),(48,48,48,255))
for k,(box,xy) in items.items():
 im=s.crop(box);a.paste(im,tuple(xy));items[k]={'sourceBox':box,'atlasBox':[*xy,xy[0]+im.width,xy[1]+im.height]};im.save(b/(k+'_source.png'))
a.convert('RGB').save(b/'IsolateGuidance.png')
s.crop((108,402,818,745)).save(b/'FirebirdGuidance.png')
surfaces={
'progress':([17,834,925,1184],[16,10]),'objectives':([17,739,925,841],[16,380]),'leaders':([17,1180,447,1498],[16,510]),'premium':([447,1180,925,1498],[470,510]),
'blank_title':([154,90,791,443],[16,850]),'blank_play':([211,1495,733,1668],[16,1235]),'blank_activate':([476,1410,896,1484],[570,1245]),
}
a=Image.new('RGBA',(1024,1536),(48,48,48,255))
for k,(box,xy) in surfaces.items():
 im=s.crop(box);a.paste(im,tuple(xy));surfaces[k]={'sourceBox':box,'atlasBox':[*xy,xy[0]+im.width,xy[1]+im.height]};im.save(b/(k+'_source.png'))
a.convert('RGB').save(b/'SurfaceGuidance.png')
(b/'SourceBoxes.json').write_text(json.dumps({'isolated':items,'surfaces':surfaces,'firebird':[108,402,818,745]},indent=2))
