"""Read-only APK/source inventory; no Unity, repacking, signing or device actions."""
from pathlib import Path
import collections,csv,hashlib,json,re,struct,zipfile
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent;qa=base/'QA'
apk=root/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
old=root/'Builds/Verification/v1_17_step01/BeforeAddressablesProfileR1'/apk.name
log=root/'ProjectDocs/AI/AndroidInputCompatibility/R1/QA/Build/unity.log'
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
def save(n,v):(qa/n).write_text(json.dumps(v,indent=2)+'\n')
# Capture current project baseline. No player storage, backend credentials or signing files read.
def project_hashes():
 return {str(p.relative_to(root)):sha(p) for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file() and p.suffix not in ('.keystore','.jks','.p12','.pem','.key')}
before=project_hashes();save('BeforeHashes.json',before)
prior_evidence=json.loads((root/'ProjectDocs/AI/AndroidInputCompatibility/R1/QA/Artifact.json').read_text())
assert sha(old)==prior_evidence['sha256'],'Archived APK must match the full-build task evidence'
entries=[];gaps=[];header_bytes=0;groups=collections.Counter()
with zipfile.ZipFile(apk) as z,apk.open('rb') as f:
 previous=0
 for i in sorted(z.infolist(),key=lambda i:i.header_offset):
  if i.header_offset>previous:
   start=previous;pos=start;dummy=0;complete=True
   # Zipflinger's deleted-entry space is represented by unnamed zero-size
   # local records. Count exactly; no assumption based only on file size.
   while pos<i.header_offset:
    f.seek(pos);h=f.read(30)
    if len(h)!=30 or h[:4]!=b'PK\x03\x04':complete=False;break
    csize,usize,nlen,xlen=struct.unpack_from('<IIHH',h,18)
    if csize or usize or nlen or pos+30+xlen>i.header_offset:complete=False;break
    pos+=30+xlen;dummy+=1
   gaps.append({'offset':start,'bytes':i.header_offset-start,'beforeEntry':i.filename,'unnamedZeroSizeLocalRecords':dummy,'fullyAccountedByUnnamedLocalRecords':complete and pos==i.header_offset})
  f.seek(i.header_offset);h=f.read(30);nlen,xlen=struct.unpack_from('<HH',h,26)
  header_bytes+=30+nlen+xlen;previous=i.header_offset+30+nlen+xlen+i.compress_size
  entries.append({'path':i.filename,'compressedBytes':i.compress_size,'uncompressedBytes':i.file_size,'crc32':f'{i.CRC:08x}','method':i.compress_type})
  key='Unity data.unity3d' if i.filename.endswith('/data.unity3d') else 'Native libraries' if i.filename.startswith('lib/') else 'DEX' if re.fullmatch(r'classes\d*\.dex',i.filename) else 'Other listed payload'
  groups[key]+=i.compress_size
 central=z.start_dir;last_end=previous
 payload=sum(i.compress_size for i in z.infolist());gap_bytes=sum(g['bytes'] for g in gaps);tail=apk.stat().st_size-last_end
 assert payload+header_bytes+gap_bytes+tail==apk.stat().st_size
 assert all(g['fullyAccountedByUnnamedLocalRecords'] for g in gaps)
 data_path='assets/bin/Data/data.unity3d'
 with z.open(data_path) as source:payload_hash=hashlib.file_digest(source,'sha256').hexdigest()
 with zipfile.ZipFile(old) as prior,prior.open(data_path) as source:prior_hash=hashlib.file_digest(source,'sha256').hexdigest()
 assert payload_hash==prior_hash,'Historical full asset report no longer describes the current Unity payload'
 apk_crc=z.testzip();assert apk_crc is None
save('ApkEntries.json',sorted(entries,key=lambda x:x['compressedBytes'],reverse=True));save('UnindexedLocalRecords.json',gaps)
summary={'apk':str(apk.relative_to(root)),'sha256':sha(apk),'fileBytes':apk.stat().st_size,'listedCompressedPayloadBytes':payload,'listedUncompressedPayloadBytes':sum(i['uncompressedBytes'] for i in entries),'listedLocalHeadersAndExtraBytes':header_bytes,'unindexedLocalRecordsBytes':gap_bytes,'unindexedRecordCount':sum(g['unnamedZeroSizeLocalRecords'] for g in gaps),'trailingSigningAndCentralDirectoryBytes':tail,'centralDirectoryOffset':central,'listedEntries':len(entries),'payloadGroups':dict(groups),'zipCrcPassed':True,'byteAccountingExact':True,'unityPayloadSha256':payload_hash,'unityPayloadMatchesPriorFullBuild':True,'historicalFullBuildLog':str(log.relative_to(root)),'historicalFullBuildLogSha256':sha(log),'scope':'Read-only audit. No build, repack, source/importer change or device action. Uncompressed per-asset amounts are rounded historical Unity report figures, NOT per-asset APK compressed sizes.'}
# Rounded asset sizes from the latest matching full build, not the script-only report.
assets=[];sizes={'b':1,'kb':1024,'mb':1024**2,'gb':1024**3}
for size,unit,path in re.findall(r'^\s*([\d.]+) (b|kb|mb|gb)\s+[\d.]+% (Assets/[^\r\n]+)$',log.read_text(),re.M):
 path=path.strip();p=root/path;row={'path':path,'reportedSize':size+' '+unit,'estimatedUncompressedBytes':round(float(size)*sizes[unit])}
 if p.suffix.lower()=='.png' and p.exists():
  with p.open('rb') as f:header=f.read(24)
  if header[:8]==b'\x89PNG\r\n\x1a\n':row['sourceWidth'],row['sourceHeight']=struct.unpack('>II',header[16:24])
  meta=p.with_suffix('.png.meta').read_text();section=re.search(r'    buildTarget: Android\n(.*?)(?=  - serializedVersion:|  spriteSheet:)',meta,re.S)
  row['mipmaps']=re.search(r'    enableMipMap: (\d+)',meta).group(1)=='1'
  row['readable']=re.search(r'  isReadable: (\d+)',meta).group(1)=='1'
  if section:
   for k in ['maxTextureSize','textureFormat','textureCompression','overridden']:
    v=re.search(r'    '+k+r': (-?\d+)',section.group(1));row['android_'+k]=int(v.group(1)) if v else None
 assets.append(row)
assert len(assets)>500
save('PackedAssetsRounded.json',assets)
byfolder=collections.Counter()
for a in assets:
 path=Path(a['path']);folder=str(path.parent);byfolder[folder]+=a['estimatedUncompressedBytes']
save('PackedFoldersRounded.json',[{'folder':p,'estimatedUncompressedBytes':v} for p,v in byfolder.most_common()])
# Source-byte-identical images are investigation candidates, not proof of identical
# imported assets or safe deletion (GUIDs, importer data, and dynamic loads differ).
duplicates=collections.defaultdict(list)
for p in (root/'Assets').rglob('*.png'):
 if 'Resources' in p.parts:duplicates[sha(p)].append(str(p.relative_to(root)))
dup=[{'sha256':k,'sourceBytesPerCopy':(root/v[0]).stat().st_size,'paths':v} for k,v in duplicates.items() if len(v)>1]
save('IdenticalSourceCandidates.json',sorted(dup,key=lambda d:d['sourceBytesPerCopy']*(len(d['paths'])-1),reverse=True))
summary.update({'packedAssetReportRows':len(assets),'reportedAssetApproxBytes':sum(a['estimatedUncompressedBytes'] for a in assets),'reportedTexturesPercent':99.1,'sourceIdenticalPngGroups':len(dup),'sourceIdenticalExcessBytes':sum(d['sourceBytesPerCopy']*(len(d['paths'])-1) for d in dup),'pngReportRows':sum('sourceWidth' in a for a in assets),'explicitAndroidRgba32Rows':sum(a.get('android_textureFormat')==4 and a.get('android_overridden')==1 for a in assets)})
save('Summary.json',summary)
after=project_hashes();assert before==after,'Production files changed during audit'
save('Scope.json',{'unchanged':len(before),'changed':[],'added':[],'deleted':[],'roots':['Assets','Packages','ProjectSettings'],'apkUnchanged':sha(apk)==summary['sha256'],'unityRun':False,'deviceRun':False})
print(json.dumps(summary,indent=2))
