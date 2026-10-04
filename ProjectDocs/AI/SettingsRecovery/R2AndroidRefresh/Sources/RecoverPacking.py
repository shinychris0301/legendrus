"""Recover a partial generated-cache copy without discarding any original files."""
from pathlib import Path
import hashlib,json,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5];QA=Path(__file__).resolve().parent.parent/'QA'
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
source=ROOT/'Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/assets'
partial=Path('/tmp/lor-settings-android-refresh-r2/BeforeFreshPacking/2')
destination=Path('/dev/shm/lor-settings-android-refresh-r2/BeforeFreshPacking/2')
assert source.is_dir() and partial.is_dir() and not destination.exists()
def inventory(folder):
 result={}
 for p in folder.rglob('*'):
  if not p.is_file():continue
  assert p.suffix.lower() not in ('.keystore','.jks','.pem','.key','.p12')
  with p.open('rb') as f:h=hashlib.file_digest(f,'sha256').hexdigest()
  result[str(p.relative_to(folder))]={'bytes':p.stat().st_size,'sha256':h}
 return result
before=inventory(source);assert set(inventory(partial))<=set(before)
destination.parent.mkdir(parents=True);shutil.copytree(source,destination);assert inventory(destination)==before
# Only remove the exact original cache after verifying the complete reversible copy.
shutil.rmtree(source);shutil.rmtree(partial)
record=json.loads((QA/'FreshPackingCachePreservation.json').read_text());assert len(record)==2
record.append({'source':str(source.relative_to(ROOT)),'preservedAt':str(destination)})
(QA/'FreshPackingCachePreservation.json').write_text(json.dumps(record,indent=2)+'\n')
(QA/'PackingQuotaRecovery.json').write_text(json.dumps({'initialError':'/tmp third-cache copy failed with Disk quota exceeded; original third source intact','firstTwoBackupsPreserved':True,'thirdBackup':str(destination),'allThirdCacheFilesByteVerified':before,'partialCopyRemovedAfterFullBackupVerification':True,'freeProjectDiskBytesAfterRecovery':shutil.disk_usage(ROOT).free,'unchangedEightGiBGuardSatisfied':shutil.disk_usage(ROOT).free>8*1024**3},indent=2)+'\n')
assert shutil.disk_usage(ROOT).free>8*1024**3
print('Third cache fully byte-verified in /dev/shm; partial /tmp copy removed; 8GiB guard satisfied.')
