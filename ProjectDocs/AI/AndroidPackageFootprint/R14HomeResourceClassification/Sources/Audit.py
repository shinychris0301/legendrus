"""Read-only inventory of all remaining legacy Home constructor sprites."""
from pathlib import Path
import hashlib,json,re,struct,sys
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
def sha(p):
    with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
def save(name,v):(QA/name).write_text(json.dumps(v,indent=2)+'\n')
def hashes():
    return {str(p.relative_to(ROOT)):sha(p) for folder in ('Assets','Packages','ProjectSettings') for p in (ROOT/folder).rglob('*') if p.is_file() and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
before=json.loads((QA/'BeforeHashes.json').read_text())
source=(ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeRuntime.cs').read_text()
names=re.findall(r'"([^"]+)"',re.search(r'static readonly string\[\] TextureNames = \{(.*?)\};',source,re.S).group(1));assert len(names)==21
packed=json.loads((BASE.parent/'R13HomeEnvironmentPlayCleanup/QA/PackedAssets.json').read_text())['assets'];assets=[]
for name in names:
    p=ROOT/('Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned/'+name+'.png');meta=p.with_suffix('.png.meta').read_text();guid=re.search(r'^guid: (\w+)',meta,re.M).group(1)
    with p.open('rb') as f:header=f.read(26)
    w,h=struct.unpack('>II',header[16:24]);rows=[r for r in packed if r['guid']==guid]
    android=re.search(r'    buildTarget: Android\n(.*?)(?=  - serializedVersion:|  spriteSheet:)',meta,re.S).group(1)
    assets.append({'name':name,'path':str(p.relative_to(ROOT)),'guid':guid,'sha256':sha(p),'metaSha256':sha(p.with_suffix('.png.meta')),'width':w,'height':h,'pngColorType':header[25],'sourceBytes':p.stat().st_size,'mipmaps':re.search(r'enableMipMap: (\d+)',meta).group(1)=='1','android':{k:int(re.search(r'    '+k+r': (-?\d+)',android).group(1)) for k in ('maxTextureSize','textureFormat','overridden')},'packedRows':rows,'packedObjectBytes':sum(r['bytes'] for r in rows)})
save('Assets.json',assets)
references=[];loaders=[];scanned=0
extensions={'.cs','.prefab','.unity','.asset','.mat','.json','.meta','.txt','.shader','.controller','.anim','.spriteatlas'}
patterns={a['name']:re.compile(r'(?<!\w)'+re.escape(a['name'])+r'(?!\w)') for a in assets}
for relative in before:
    p=ROOT/relative
    if p.suffix not in extensions:continue
    try:content=p.read_text()
    except UnicodeDecodeError:continue
    scanned+=1
    for line,value in enumerate(content.splitlines(),1):
        matches=[a['name'] for a in assets if a['guid'] in value or patterns[a['name']].search(value)]
        if matches:references.append({'path':relative,'line':line,'matches':matches,'text':value.strip()})
        if p.suffix=='.cs' and any(s in value for s in ('HomeAligned/','HomeRuntime.ArtRoot','Resources.LoadAll','GetTextureResourceKeys','HasGeneratedControlBindings','HasReferenceBackdropBindings')):loaders.append({'path':relative,'line':line,'text':value.strip()})
save('References.json',{'scannedTextFiles':scanned,'references':references});save('LoaderSites.json',loaders)
prior=json.loads((BASE.parent/'R13HomeEnvironmentPlayCleanup/QA/Artifact.json').read_text());apk=ROOT/prior['path'];apkhash=sha(apk);assert apkhash==prior['sha256'];save('ArtifactBaseline.json',{'path':prior['path'],'bytes':apk.stat().st_size,'sha256':apkhash,'unchanged':True})
if '--final' in sys.argv:
    after=hashes();scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items()),'apkUnchanged':True};save('Scope.json',scope);assert not(scope['changed'] or scope['added'] or scope['deleted']);print(json.dumps(scope))
print('Inventoried',len(assets),'assets;',scanned,'text files;',sum(a['packedObjectBytes'] for a in assets),'packed object bytes (not compressed savings).')
