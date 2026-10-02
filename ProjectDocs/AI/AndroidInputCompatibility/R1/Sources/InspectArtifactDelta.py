from pathlib import Path
import json,subprocess,zipfile,hashlib
r=Path(__file__).resolve().parents[5];b=Path(__file__).resolve().parent.parent
folder=r/'Builds/Verification/v1_17_step01';old=folder/'BeforeAndroidInputR1/LegendsOfRus-v1.17-step01-development.apk';new=folder/'LegendsOfRus-v1.17-step01-development.apk'
aapt='/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/build-tools/36.0.0/aapt'
a=subprocess.check_output([aapt,'dump','permissions',str(old)],text=True);c=subprocess.check_output([aapt,'dump','permissions',str(new)],text=True)
assert a==c,'Permissions changed'
with zipfile.ZipFile(old) as x,zipfile.ZipFile(new) as y:
    prior={i.filename:(i.CRC,i.file_size,i.compress_size) for i in x.infolist()};current={i.filename:(i.CRC,i.file_size,i.compress_size) for i in y.infolist()}
    delta=[{'path':n,'oldSize':prior.get(n,(0,0))[1],'newSize':v[1]} for n,v in current.items() if prior.get(n)!=v]
pins=json.loads((r/'ProjectDocs/AI/AndroidCredentialStorage/R2DeviceBuild/QA/PinnedPluginInventory.json').read_text())
assert all(hashlib.file_digest((r/name).open('rb'),'sha256').hexdigest()==digest for name,digest in pins.items())
(b/'QA/ArtifactDelta.json').write_text(json.dumps({'permissionsUnchanged':True,'permissions':c.splitlines(),'reviewedJavaAndMetadataUnchanged':True,'changedZipEntries':delta},indent=2)+'\n')
print('Permissions and reviewed plugins unchanged;',len(delta),'APK entries changed.')
