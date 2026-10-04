from pathlib import Path
import subprocess,json,hashlib,zipfile,struct,re,sys
root=Path.cwd();base=root/'ProjectDocs/AI/NarrativeRecovery/R2AndroidRefresh';qa=base/'QA';qa.mkdir(exist_ok=True)
apk=root/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
assert apk.is_file(), 'Current audit APK missing'
tools=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/build-tools/36.0.0')
def command(name,args):
    r=subprocess.run([str(a) for a in args],text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
    (qa/(name+'.txt')).write_text(r.stdout)
    assert r.returncode==0, name+' failed; see QA evidence'
    return r.stdout
signature=command('ApkSignature',[tools/'apksigner','verify','--print-certs',apk])
certificate=re.search(r'certificate SHA-256 digest: ([a-f0-9]+)',signature).group(1)
previous=root/'Builds/Verification/v1_17_step01/BeforeNarrativeRefreshR2/LegendsOfRus-v1.17-step01-development.apk'
previous_signature=command('PreviousApkSignature',[tools/'apksigner','verify','--print-certs',previous])
previous_certificate=re.search(r'certificate SHA-256 digest: ([a-f0-9]+)',previous_signature).group(1)
assert certificate == previous_certificate, 'Development certificate changed'
badging=command('ApkManifest',[tools/'aapt','dump','badging',apk])
assert "package: name='com.example.legendsofrus.v117debug'" in badging
assert "versionCode='1'" in badging and "versionName='0.0.0'" in badging
assert "native-code: 'arm64-v8a'" in badging and 'application-debuggable' in badging
assert "sdkVersion:'26'" in badging and "targetSdkVersion:'36'" in badging
manifest=command('ApkManifestXml',[tools/'aapt','dump','xmltree',apk,'AndroidManifest.xml'])
assert re.search(r'android:screenOrientation.*0x1\b',manifest), 'Portrait orientation missing'
command('ZipAlignment',[tools/'zipalign','-c','-P','16','4',apk])
libraries=[]
with zipfile.ZipFile(apk) as z:
    assert z.testzip() is None, 'APK ZIP CRC failure'
    for n in z.namelist():
        if n.startswith('lib/') and n.endswith('.so'):
            data=z.read(n);assert data[:4]==b'\x7fELF' and data[4]==2 and data[5]==1
            offset=struct.unpack_from('<Q',data,32)[0];stride,count=struct.unpack_from('<HH',data,54)
            alignments=[struct.unpack_from('<Q',data,offset+i*stride+48)[0] for i in range(count) if struct.unpack_from('<I',data,offset+i*stride)[0]==1]
            assert alignments and min(alignments)>=16384, n+' load segment alignment'
            libraries.append({'path':n,'loadSegmentAlignments':alignments})
    metadata=[n for n in z.namelist() if n.endswith('global-metadata.dat')]
    assert metadata, 'IL2CPP metadata missing'
    data=z.read(metadata[0]);markers={key:key.encode() in data for key in ('AndroidBackendCredentialStore','BackendAccountService','CredentialProbeProtocol','AndroidCredentialProbeHost','AndroidCredentialDevelopmentProbe','DevelopmentGameplayProfile','DevelopmentGameplayProfileBadge','lor_gameplay_qa','LegendsOfRus_GameplayQA_v1_','ProductionControlTouchTarget','ProductionGameplayNumberFit','ProductionGameplayCaptionFit','ProductionHomePanel','ApplyCaptionReadability','ExpandedCaption','ProductionGameplayStockLayout','WideBacking','ApplyNow','LateUpdate','ProductionStoryRecapView','StoryRecapEntry','OpenStoryRecap','SceneIdProvider','SpeakerKeyProvider')}
    assert all(markers.values()), 'Expected profile/runtime metadata missing'
    recap_types={name:(name+'\0').encode() in data for name in ('ProductionStoryRecapView','StoryRecap','StoryRecapEntry')}
    assert all(recap_types.values()), 'Recap runtime types missing from IL2CPP metadata'
    assert b'StoryRecapRefreshBuildAudit' not in data and b'StoryRecapPlayModeTests' not in data, 'Editor/test helper leaked into player'
    preset_types={name:(name+'\0').encode() in data for name in ('DevelopmentGameplayFixtures',)}
    sanity,version=struct.unpack_from('<II',data,0)
    assert sanity==0xFAB11BAF and version==107, 'Unknown metadata header version'
    offset,size,count,literal_offset,literal_bytes,literal_count=struct.unpack_from('<6I',data,8)
    assert size==count*4 and offset+size<=len(data) and literal_offset+literal_bytes<=len(data)
    indices=struct.unpack_from('<'+str(count)+'I',data,offset)
    assert indices[0]==0 and indices[-1]==literal_bytes and all(a<=b for a,b in zip(indices,indices[1:]))
    literals={data[literal_offset+a:literal_offset+b].decode('utf-8',errors='replace') for a,b in zip(indices,indices[1:])}
    preset_selectors={name:name in literals for name in ('stock-999','stock-1000','stock-9999','stock-max','story-early','story-finale')}
    assert all(preset_types.values()) and all(preset_selectors.values()), 'Fixed preset factory/selectors absent'
    assert b'NarrativeRefreshBuildAudit' not in data and b'DevelopmentGameplayFixturePlayModeTests' not in data and b'DevelopmentGameplayFixtureEditModeTests' not in data, 'Temporary/test helper leaked into player'
    entries=len(z.infolist())
result={'apk':str(apk.relative_to(root)),'bytes':apk.stat().st_size,'sha256':hashlib.file_digest(apk.open('rb'),'sha256').hexdigest(),'package':'com.example.legendsofrus.v117debug','versionName':'0.0.0','versionCode':1,'debuggable':True,'abi':'arm64-v8a','minimumApi':26,'targetApi':36,'portrait':True,'signatureVerified':True,'certificateSha256':certificate,'sameCertificateAsPriorLocalApk':certificate==previous_certificate,'zipCrcPassed':True,'zipEntries':entries,'zip16KiBAlignmentPassed':True,'elfLibraries':libraries,'runtimeMetadataMarkers':markers,'recapRuntimeTypes':recap_types,'presetRuntimeTypes':preset_types,'presetSelectors':preset_selectors,'editorAndTestHelpersAbsentFromPlayerMetadata':True,'installedAppCertificateCompared':False,'physicalDeviceTested':False}
(qa/'ApkVerification.json').write_text(json.dumps(result,indent=2)+'\n')
print('APK signature, manifest, ZIP, ELF and runtime metadata inspection saved.')
