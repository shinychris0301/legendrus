from pathlib import Path
import json,hashlib,zipfile,struct,subprocess,os,re
root=Path.cwd();out=root/'ProjectDocs/AI/AndroidPhoneTest/FailureR9_2026-09-15';apk=root/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk';previous=apk.parent/'BeforeFailureR9_2026-09-15'/apk.name
sdk=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer');bt=sdk/'SDK/build-tools/36.0.0';env=os.environ.copy();env['JAVA_HOME']=str(sdk/'OpenJDK')
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
assert sha(apk)!=sha(previous),'Expected a new APK'
records={}
def command(name,args):
 p=subprocess.run([str(x) for x in args],capture_output=True,text=True,env=env);records[name]={'command':[str(x) for x in args],'exitCode':p.returncode,'output':p.stdout+p.stderr};assert p.returncode==0,name;return p.stdout+p.stderr
badging=command('aapt',[bt/'aapt','dump','badging',apk]);manifest=command('manifest',[bt/'aapt','dump','xmltree',apk,'AndroidManifest.xml']);sig=command('signature',[bt/'apksigner','verify','--verbose','--print-certs',apk]);oldSig=command('previousSignature',[bt/'apksigner','verify','--print-certs',previous]);command('zipalign',[bt/'zipalign','-c','-P','16','4',apk])
fingerprint=lambda s:re.search(r'certificate SHA-256 digest: (\w+)',s).group(1)
same_signer=fingerprint(sig)==fingerprint(oldSig)
native=[]
with zipfile.ZipFile(apk) as z:
 assert z.testzip() is None
 entries=z.infolist();metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat');markers={s:s.encode() in metadata or s.encode('utf-16le') in metadata for s in ['ProductionFailView','paid_r9','ad_r9']};assert all(markers.values()),markers
 for p in entries:
  if p.filename.startswith('lib/') and p.filename.endswith('.so'):
   b=z.read(p);assert b[:4]==b'\x7fELF' and b[4]==2 and b[5]==1
   machine=struct.unpack_from('<H',b,18)[0];phoff=struct.unpack_from('<Q',b,32)[0];entsize,count=struct.unpack_from('<HH',b,54);align=[]
   for i in range(count):
    t=struct.unpack_from('<IIQQQQQQ',b,phoff+i*entsize)
    if t[0]==1:align.append(t[-1])
   assert machine==183 and all(x>=16384 for x in align),(p.filename,align)
   native.append({'entry':p.filename,'machine':machine,'loadAlignments':align})
 # Unreferenced spans between ZIP entries catch bloated stale incremental packaging.
 with apk.open('rb') as f:
  spans=[]
  for p in sorted(entries,key=lambda x:x.header_offset):
   f.seek(p.header_offset+26);n,e=struct.unpack('<HH',f.read(4));spans.append((p.header_offset,p.header_offset+30+n+e+p.compress_size))
 gaps=[{'start':a[1],'bytes':b[0]-a[1]} for a,b in zip(spans,spans[1:]) if b[0]-a[1]>65536]
record={'apk':str(apk.relative_to(root)),'bytes':apk.stat().st_size,'sha256':sha(apk),'entryCount':len(entries),'zipCrcPassed':True,'signatureMatchesPrevious':same_signer,'certificateSha256':fingerprint(sig),'nativeLibraries':native,'il2cppMarkers':markers,'largeUnreferencedZipSpans':gaps,'manifestSummary':[x for x in badging.splitlines() if x.startswith(('package:','sdkVersion:','targetSdkVersion:','native-code:','application-debuggable','launchable-activity:'))],'orientationFields':[x.strip() for x in manifest.splitlines() if 'screenOrientation' in x],'commands':records,'deviceTestingPerformed':False}
(out/'ApkVerification.json').write_text(json.dumps(record,indent=2)+'\n');Path(str(apk)+'.sha256').write_text(record['sha256']+'  '+apk.name+'\n');print(json.dumps({k:v for k,v in record.items() if k not in ['commands','nativeLibraries']},indent=2))
