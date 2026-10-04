"""Relocate only known generated cache backups to recover temporary filesystem writes."""
from pathlib import Path
import hashlib,json,os,shutil,signal
ROOT=Path(__file__).resolve().parents[5];QA=Path(__file__).resolve().parent.parent/'QA'
pid=1995414
cmd=Path(f'/proc/{pid}/cmdline');assert cmd.exists() and b'/dev/shm/lor-persistence-recovery-r1/CoreFinal/unity.log' in cmd.read_bytes().split(b'\0')
os.killpg(pid,signal.SIGTERM)
def inventory(folder):
 rows={}
 for p in folder.rglob('*'):
  if not p.is_file():continue
  assert p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key')
  with p.open('rb') as f:hash=hashlib.file_digest(f,'sha256').hexdigest()
  rows[str(p.relative_to(folder))]={'bytes':p.stat().st_size,'sha256':hash}
 return rows
moves=[]
for n in ('0','1'):
 source=Path('/tmp/lor-settings-android-refresh-r2/BeforeFreshPacking')/n
 destination=Path('/dev/shm/lor-persistence-recovery-r1/PreservedSettingsPacking')/n
 assert source.is_dir() and not source.is_symlink() and not destination.exists()
 before=inventory(source);destination.parent.mkdir(parents=True,exist_ok=True);shutil.copytree(source,destination);assert inventory(destination)==before
 shutil.rmtree(source);source.symlink_to(destination,target_is_directory=True)
 moves.append({'originalPath':str(source),'preservedAt':str(destination),'originalPathStillResolves':source.is_dir(),'byteVerifiedFiles':before})
 (QA/'TemporaryStorageRecovery.json').write_text(json.dumps({'priorGeneratedBackups':moves,'licenseOrCredentialFilesAccessed':False,'projectInputsChanged':False,'scope':'Only two Settings R2 generated packing-cache backups; original evidence paths retained as symlinks.','initialDisposableSQLiteProbe':{'tmp':'disk I/O error','devShm':'Passed','project':'Passed'}},indent=2)+'\n')
if cmd.exists():
 assert b'/dev/shm/lor-persistence-recovery-r1/CoreFinal/unity.log' in cmd.read_bytes().split(b'\0');os.killpg(pid,signal.SIGKILL)
p=QA/'CoreLauncherRecovery.json';record=json.loads(p.read_text());record['secondStalledRun']='CoreFinal';record['secondTaskUnityPidTerminated']=pid;record['secondReason']='Disposable SQLite probe reports disk I/O error in /tmp; task launcher also reports disk I/O error. Both licensing children exited before tests.';record['retry']='CoreVerified';p.write_text(json.dumps(record,indent=2)+'\n')
print('Two generated backups byte-verified and relocated; original evidence paths retained as links.')
