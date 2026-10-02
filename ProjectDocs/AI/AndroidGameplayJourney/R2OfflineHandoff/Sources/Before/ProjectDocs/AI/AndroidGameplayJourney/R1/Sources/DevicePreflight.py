"""Read-only device/APK readiness. Never launches, installs, clears data or exports saves."""
from pathlib import Path
import hashlib,json,re,subprocess

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
ADB='/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb'
PACKAGE='com.example.legendsofrus.v117debug'
APK=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'

def run(args):
    return subprocess.run([ADB,*args],capture_output=True,text=True,timeout=30)

def main():
    result={'physicalGameplayTested':False,'appLaunched':False,'appDataReadOrChanged':False,
            'localApkExists':APK.is_file(),'package':PACKAGE}
    if APK.is_file():
        with APK.open('rb') as stream:result['localApkSha256']=hashlib.file_digest(stream,'sha256').hexdigest()
        previous=json.loads((ROOT/'ProjectDocs/AI/AndroidCredentialStorage/R5DeviceExecution/QA/ApkVerification.json').read_text())
        result['matchesR5VerifiedApk']=result['localApkSha256']==previous['sha256']
    listing=run(['devices'])
    if listing.returncode:
        result['status']='adb_enumeration_failed'
    else:
        rows=[line.split() for line in listing.stdout.splitlines()[1:] if line.strip()]
        devices=[r[0] for r in rows if len(r)==2 and r[1]=='device']
        result.update(attachedCount=len(rows),authorizedCount=len(devices))
        if len(devices)!=1:
            result['status']='connect_one_authorized_device'
        else:
            serial=devices[0]
            def device(*args):return run(['-s',serial,*args])
            for prop,key in [('ro.product.model','deviceModel'),('ro.build.version.sdk','androidApi')]:
                response=device('shell','getprop',prop)
                if response.returncode:raise RuntimeError('Device property query failed')
                result[key]=response.stdout.strip()
            response=device('shell','pm','path',PACKAGE)
            paths=[line.removeprefix('package:') for line in response.stdout.splitlines() if line.startswith('package:')]
            base=next((p for p in paths if p.endswith('/base.apk')),None)
            if not base:
                result['status']='development_app_not_installed'
            elif not re.fullmatch(r'/data/app/[A-Za-z0-9_/=+~.\-]+/base\.apk',base):
                result['status']='unexpected_package_path'
            else:
                hashed=device('shell','sha256sum',base)
                digest=hashed.stdout.split()[0] if hashed.returncode==0 and hashed.stdout.split() else ''
                if not re.fullmatch('[a-f0-9]{64}',digest):
                    result['status']='installed_apk_hash_unavailable'
                else:
                    result['installedApkMatchesLocal']=digest==result.get('localApkSha256')
                    result['status']='ready_for_isolation_check' if result['installedApkMatchesLocal'] and result.get('matchesR5VerifiedApk') else 'build_identity_requires_review'
    (BASE/'QA').mkdir(parents=True,exist_ok=True)
    (BASE/'QA/DevicePreflight.json').write_text(json.dumps(result,indent=2)+'\n')
    print(json.dumps(result,indent=2))
    return 0 if result['status']=='ready_for_isolation_check' else 2

if __name__=='__main__':raise SystemExit(main())
