"""Read-only source/importer/reference audit, paired with Unity dependency probes."""
from pathlib import Path
import collections
import hashlib
import json
import re
import struct

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
names=('progress_fill','progress_frame')

def sha(path):
    with path.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()

def save(name,value):(QA/name).write_text(json.dumps(value,indent=2)+'\n')

def mip_bytes(width,height):
    total=0
    while True:
        total+=width*height*4
        if width==height==1:return total
        width=max(1,width//2);height=max(1,height//2)

packed=json.loads((BASE.parent/'R1/QA/PackedAssetsRounded.json').read_text())
assets={}
for name in names:
    path=ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned'/(name+'.png')
    meta=path.with_suffix('.png.meta').read_text()
    with path.open('rb') as stream:header=stream.read(26)
    assert header[:8]==b'\x89PNG\r\n\x1a\n'
    width,height=struct.unpack('>II',header[16:24])
    android=re.search(r'    buildTarget: Android\n(.*?)(?=  - serializedVersion:|  spriteSheet:)',meta,re.S).group(1)
    assets[name]={'path':str(path.relative_to(ROOT)),'sha256':sha(path),'metaSha256':sha(path.with_suffix('.png.meta')),
        'guid':re.search(r'^guid: (\w+)',meta,re.M).group(1),'width':width,'height':height,'pngColorType':header[25],
        'sourceFileBytes':path.stat().st_size,'mipmaps':re.search(r'enableMipMap: (\d+)',meta).group(1)=='1',
        'readable':re.search(r'isReadable: (\d+)',meta).group(1)=='1',
        'android':{key:int(re.search(r'    '+key+r': (-?\d+)',android).group(1)) for key in ('maxTextureSize','textureFormat','textureCompression','overridden')},
        'rgba32MipPayloadEstimateBytes':mip_bytes(width,height),
        'historicalPackedAssetReport':next(x for x in packed if x['path']==str(path.relative_to(ROOT)))}
save('Assets.json',assets)

references=[];source=[];serialized=[];scanned=collections.Counter()
for path in (ROOT/'Assets').rglob('*'):
    if not path.is_file() or path.suffix not in ('.cs','.prefab','.unity','.asset','.mat','.json','.meta','.shader','.compute'):
        continue
    try:text=path.read_text()
    except UnicodeDecodeError:continue
    scanned[path.suffix]+=1
    rel=str(path.relative_to(ROOT))
    for name,asset in assets.items():
        if asset['guid'] in text:
            for i,line in enumerate(text.splitlines(),1):
                if asset['guid'] in line:references.append({'asset':name,'path':rel,'line':i,'text':line.strip()})
        if path.suffix=='.cs':
            for i,line in enumerate(text.splitlines(),1):
                if re.search(r'(?<![\w])'+name+r'(?![\w])',line):source.append({'asset':name,'path':rel,'line':i,'text':line.strip()})
    if path.suffix not in ('.prefab','.unity') or not any(v['guid'] in text for v in assets.values()):continue
    objects={m.group(2):(int(m.group(1)),m.group(3)) for m in re.finditer(r'^--- !u!(\d+) &(-?\d+)\n(.*?)(?=^--- !u!|\Z)',text,re.M|re.S)}
    def field(body,key):
        m=re.search(r'^  '+re.escape(key)+r': (.+)$',body,re.M)
        return m.group(1) if m else None
    def fileid(body,key):
        value=field(body,key)
        return re.search(r'fileID: (-?\d+)',value).group(1) if value else None
    rects={fileid(body,'m_GameObject'):(oid,body) for oid,(typ,body) in objects.items() if typ==224}
    for oid,(typ,body) in objects.items():
        if typ!=114:continue
        for name,asset in assets.items():
            if not re.search(r'm_Sprite:.*'+asset['guid'],body):continue
            goid=fileid(body,'m_GameObject');go=objects[goid][1];rid,rect=rects[goid]
            ancestors=[];active=True;cursor=rid;visited=set()
            while cursor and cursor!='0':
                assert cursor not in visited;visited.add(cursor)
                rb=objects[cursor][1];gb=objects[fileid(rb,'m_GameObject')][1]
                ancestors.insert(0,field(gb,'m_Name'));active=active and field(gb,'m_IsActive')=='1'
                cursor=fileid(rb,'m_Father')
            serialized.append({'asset':name,'prefab':rel,'componentFileId':oid,'hierarchy':'/'.join(ancestors),
                'enabled':field(body,'m_Enabled')=='1','activeThroughAncestors':active,'sizeDelta':field(rect,'m_SizeDelta'),
                'localScale':field(rect,'m_LocalScale'),'preserveAspect':field(body,'m_PreserveAspect'),
                'imageType':field(body,'m_Type'),'fillAmount':field(body,'m_FillAmount')})
save('GuidReferences.json',references);save('SourceReferences.json',source);save('SerializedImages.json',serialized)
save('ScanCoverage.json',{'textFilesByExtension':dict(scanned),'guidReferenceLines':len(references),'sourceReferenceLines':len(source),
    'serializedImageComponents':len(serialized),'limitations':'Literal/GUID search and traced loaders plus Unity Edit Mode dependency/build probes; R3 runtime samples are historical, not proof about arbitrary future dynamic paths or every gameplay state.'})

