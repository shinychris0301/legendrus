"""Inventory the candidate and prove the read-only audit's final scope."""
from pathlib import Path
import hashlib
import json
import re
import struct

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
def sha(path):
    with path.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
def save(name,value):(QA/name).write_text(json.dumps(value,indent=2)+'\n')
path=ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/home_firebird_generated.png'
meta=path.with_suffix('.png.meta').read_text()
guid=re.search(r'^guid: (\w+)',meta,re.M).group(1)
with path.open('rb') as stream:header=stream.read(26)
width,height=struct.unpack('>II',header[16:24])
packed=json.loads((BASE.parent/'R7ProgressCleanup/QA/PackedAssets.json').read_text())
rows=[r for r in packed['assets'] if r['guid']==guid]
android=re.search(r'    buildTarget: Android\n(.*?)(?=  - serializedVersion:|  spriteSheet:)',meta,re.S).group(1)
before=json.loads((QA/'BeforeHashes.json').read_text())
digest=sha(path)
asset={'path':str(path.relative_to(ROOT)),'guid':guid,'sha256':digest,'metaSha256':sha(path.with_suffix('.png.meta')),
    'width':width,'height':height,'pngColorType':header[25],'sourceBytes':path.stat().st_size,
    'mipmaps':re.search(r'enableMipMap: (\d+)',meta).group(1)=='1',
    'android':{k:int(re.search(r'    '+k+r': (-?\d+)',android).group(1)) for k in ('maxTextureSize','textureFormat','overridden')},
    'packedObjectBytes':sum(r['bytes'] for r in rows),'packedRows':rows,
    'byteIdenticalOtherPngPaths':[p for p,h in before.items() if p.endswith('.png') and h==digest and p!=str(path.relative_to(ROOT))]}
save('Asset.json',asset)
references=[];loaders=[];scanned=0
extensions={'.cs','.prefab','.unity','.asset','.mat','.json','.meta','.txt','.shader','.controller','.anim','.spriteatlas'}
for folder in ('Assets','Packages','ProjectSettings'):
    for p in (ROOT/folder).rglob('*'):
        if not p.is_file() or p.suffix not in extensions or p.name.startswith('FirebirdResourceAuditEditModeTests.cs'):continue
        try:text=p.read_text()
        except UnicodeDecodeError:continue
        scanned+=1
        for line,value in enumerate(text.splitlines(),1):
            if guid in value or 'home_firebird_generated' in value or 'firebird_generated' in value:
                references.append({'path':str(p.relative_to(ROOT)),'line':line,'text':value.strip()})
            if p.suffix=='.cs' and ('HomeAligned/' in value or 'HomeRuntime.ArtRoot' in value or 'Resources.LoadAll' in value):
                loaders.append({'path':str(p.relative_to(ROOT)),'line':line,'text':value.strip()})
save('References.json',{'scannedTextFiles':scanned,'references':references})
save('LoaderSites.json',loaders)
assert len(references)==1 and references[0]['path']==str(path.with_suffix('.png.meta').relative_to(ROOT))
after={str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings') for p in (ROOT/folder).rglob('*')
    if p.is_file() and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),
    'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())}
apk=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
prior=json.loads((BASE.parent/'R7ProgressCleanup/QA/Artifact.json').read_text())
scope.update(apkBytes=apk.stat().st_size,apkSha256=sha(apk));scope['apkUnchanged']=scope['apkSha256']==prior['sha256']
save('Scope.json',scope)
assert not(scope['changed'] or scope['added'] or scope['deleted']) and scope['apkUnchanged']
print(json.dumps({'asset':asset,'scannedTextFiles':scanned,'scope':scope},indent=2))
