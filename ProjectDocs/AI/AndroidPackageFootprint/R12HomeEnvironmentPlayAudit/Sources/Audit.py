"""Inventory the two legacy constructor assets and verify a read-only final scope."""
from pathlib import Path
import hashlib,json,re,struct
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
def sha(p):
    with p.open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
def save(name,v):(QA/name).write_text(json.dumps(v,indent=2)+'\n')
before=json.loads((QA/'BeforeHashes.json').read_text())
packed=json.loads((BASE.parent/'R11HomeHeroArchive/QA/PackedAssets.json').read_text())['assets']
names=('home_environment_generated','home_play_button');assets=[]
for name in names:
    p=ROOT/('Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/'+name+'.png');meta=p.with_suffix('.png.meta').read_text();guid=re.search(r'^guid: (\w+)',meta,re.M).group(1)
    with p.open('rb') as f:header=f.read(26)
    width,height=struct.unpack('>II',header[16:24]);rows=[r for r in packed if r['guid']==guid];digest=sha(p)
    android=re.search(r'    buildTarget: Android\n(.*?)(?=  - serializedVersion:|  spriteSheet:)',meta,re.S).group(1)
    assets.append({'name':name,'path':str(p.relative_to(ROOT)),'guid':guid,'sha256':digest,'metaSha256':sha(p.with_suffix('.png.meta')),'width':width,'height':height,'pngColorType':header[25],'sourceBytes':p.stat().st_size,'mipmaps':re.search(r'enableMipMap: (\d+)',meta).group(1)=='1','android':{k:int(re.search(r'    '+k+r': (-?\d+)',android).group(1)) for k in ('maxTextureSize','textureFormat','overridden')},'packedRows':rows,'packedObjectBytes':sum(r['bytes'] for r in rows),'byteIdenticalOtherPngPaths':[r for r,h in before.items() if r.endswith('.png') and h==digest and r!=str(p.relative_to(ROOT))]})
save('Assets.json',assets)
references=[];loaders=[];scanned=0
extensions={'.cs','.prefab','.unity','.asset','.mat','.json','.meta','.txt','.shader','.controller','.anim','.spriteatlas'}
for relative in before:
    p=ROOT/relative
    if p.suffix not in extensions:continue
    try:content=p.read_text()
    except UnicodeDecodeError:continue
    scanned+=1
    for line,value in enumerate(content.splitlines(),1):
        matches=[a['name'] for a in assets if a['guid'] in value or a['name'] in value]
        if matches:references.append({'path':relative,'line':line,'matches':matches,'text':value.strip()})
        if p.suffix=='.cs' and any(s in value for s in ('HomeAligned/','HomeRuntime.ArtRoot','Resources.LoadAll','GetTextureResourceKeys','HasReferenceForegroundBindings')):loaders.append({'path':relative,'line':line,'text':value.strip()})
save('References.json',{'scannedTextFiles':scanned,'references':references});save('LoaderSites.json',loaders)
after={str(p.relative_to(ROOT)):sha(p) for f in ('Assets','Packages','ProjectSettings') for p in (ROOT/f).rglob('*') if p.is_file() and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())}
prior=json.loads((BASE.parent/'R11HomeHeroArchive/QA/Artifact.json').read_text());apk=ROOT/prior['path'];scope.update(apkBytes=apk.stat().st_size,apkSha256=sha(apk));scope['apkUnchanged']=scope['apkSha256']==prior['sha256'];save('Scope.json',scope)
assert not(scope['changed'] or scope['added'] or scope['deleted']) and scope['apkUnchanged']
print(json.dumps({'assets':assets,'scannedTextFiles':scanned,'scope':scope},indent=2))
