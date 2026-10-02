"""Explicit development-device probe. Reads only the redacted probe report; never exports credentials."""
from pathlib import Path
import argparse, datetime, json, subprocess, time
ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
ADB=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb')
PACKAGE='com.example.legendsofrus.v117debug'
COMPONENT=PACKAGE+'/com.unity3d.player.UnityPlayerGameActivity'
REPORT='no_backup/lor-credential-probe-report.json'
EXPECTED={'prepare':'awaiting_restart','restart':'awaiting_force_stop','verify':'passed','cleanup':'cleaned'}
CHECKS={'pristine_read','write','read_back','process_restart_read','interrupted_write_recovery','corruption_rejected','owned_fixture_restored','missing_key_rejected','owned_fixture_cleanup'}
PHASE_CHECKS={'prepare':['pristine_read','write','read_back'], 'restart':['process_restart_read'], 'verify':['interrupted_write_recovery','corruption_rejected','owned_fixture_restored','missing_key_rejected','owned_fixture_cleanup'], 'cleanup':[]}
def validate_report(raw,phase):
    if len(raw)>8192:raise ValueError('Invalid probe report size')
    value=json.loads(raw)
    if not isinstance(value,dict) or set(value)!={'version','command','status','checks','authenticatedSessionTested'}:raise ValueError('Unexpected report schema')
    if type(value['version']) is not int or value['version']!=1 or value['command']!=phase or value['status'] not in {EXPECTED[phase],'refused','failed_retained'} or value['authenticatedSessionTested'] is not False:raise ValueError('Unexpected report result')
    if not isinstance(value['checks'],list) or any(not isinstance(c,str) or c not in CHECKS for c in value['checks']):raise ValueError('Unexpected report checks')
    if value['status']==EXPECTED[phase] and value['checks']!=PHASE_CHECKS[phase]:raise ValueError('Incomplete successful report')
    return value

def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--phase',choices=['check','all',*EXPECTED],default='check')
    parser.add_argument('--serial')
    parser.add_argument('--install',action='store_true',help='Replace the development APK with adb install -r; never uninstall or clear data')
    parser.add_argument('--apk',type=Path,default=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk')
    args=parser.parse_args()
    if args.install and args.phase=='check':parser.error('--install requires an explicit test phase')
    listing=subprocess.run([str(ADB),'devices'],capture_output=True,text=True,timeout=15)
    if listing.returncode:raise RuntimeError('adb device enumeration failed')
    rows=[line.split() for line in listing.stdout.splitlines()[1:] if line.strip()]
    devices=[r[0] for r in rows if len(r)>1 and r[1]=='device']
    status={'attachedCount':len(rows),'authorizedCount':len(devices),'installAttempted':False,'credentialProbeExecuted':False}
    if args.phase=='check':
        (BASE/'QA/DeviceStatus.json').write_text(json.dumps(status,indent=2)+'\n')
        print(json.dumps(status));return
    if args.serial:
        if args.serial not in devices:raise RuntimeError('Requested device is not connected and authorized')
        serial=args.serial
    else:
        if len(devices)!=1:raise RuntimeError('Connect exactly one authorized Android test device, or select --serial')
        serial=devices[0]
    def adb(*command,check=True,timeout=30):
        result=subprocess.run([str(ADB),'-s',serial,*map(str,command)],capture_output=True,text=True,timeout=timeout)
        if check and result.returncode:raise RuntimeError('ADB operation failed; no application data was exported')
        return result
    # Refuse non-debuggable/wrong-package tools; run-as must work after any requested install.
    if args.install:
        if not args.apk.is_file():raise RuntimeError('Development APK missing')
        adb('install','-r',args.apk,timeout=300);status['installAttempted']=True
    adb('shell','run-as',PACKAGE,'pwd')
    out=BASE/'QA/DeviceRuns'/datetime.datetime.now(datetime.timezone.utc).strftime('%Y%m%dT%H%M%S%fZ')
    out.mkdir(parents=True)
    status['credentialProbeExecuted']=True
    try:
        for phase in ['prepare','restart','verify'] if args.phase=='all' else [args.phase]:
            # Kill the prior process before clearing only our result file, eliminating stale reports.
            adb('shell','am','force-stop',PACKAGE)
            adb('shell','run-as',PACKAGE,'rm','-f',REPORT)
            launched=adb('shell','am','start','-W','-n',COMPONENT,'--es','lor_credential_probe',phase)
            if 'Error:' in launched.stdout:raise RuntimeError('Development activity did not start')
            deadline=time.monotonic()+90;report=None
            while time.monotonic()<deadline:
                response=adb('shell','run-as',PACKAGE,'cat',REPORT,check=False)
                if response.returncode==0:
                    try:report=validate_report(response.stdout,phase)
                    except (ValueError,TypeError):raise RuntimeError('Invalid or unexpected probe report; raw contents not saved')
                    break
                time.sleep(0.4)
            if report is None:raise RuntimeError('No report within 90 seconds; no storage success inferred')
            (out/(phase+'.json')).write_text(json.dumps(report,indent=2)+'\n')
            print(phase+': '+report['status'],flush=True)
            if report['status']!=EXPECTED[phase]:raise RuntimeError('Probe stopped with '+report['status']+'; ownership retained where applicable')
        status['requestedPhasesPassed']=True
    finally:
        (out/'RunStatus.json').write_text(json.dumps(status,indent=2)+'\n')
if __name__=='__main__':
    try:main()
    except (RuntimeError, subprocess.TimeoutExpired) as error:
        # Fixed/controlled messages only, never raw native failures or application output.
        print(str(error) if isinstance(error,RuntimeError) else 'ADB timeout; no success inferred')
        raise SystemExit(1)
