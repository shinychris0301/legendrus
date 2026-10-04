"""Preserve one task-regenerated tracked compiler cache, then restore its clean Git baseline."""
from pathlib import Path
import hashlib,json,subprocess,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
relative='.utmp/Debug/4u1s6d3l/arm64-v8a/configure_fingerprint.bin'
for n in ('WorkingTreeTaskStart.txt','WorkingTreeBefore.txt'):
 assert relative not in (QA/n).read_text()
path=ROOT/relative;target=Path('/tmp/lor-spending-android-refresh-r2/GeneratedFingerprint.bin');assert not target.exists()
shutil.copy2(path,target)
original=subprocess.check_output(['git','show','HEAD:'+relative],cwd=ROOT)
changed=path.read_bytes();assert original!=changed
path.write_bytes(original)
record={'path':relative,'action':'restored originally clean tracked generated compiler cache','generatedCopyPreservedAt':str(target),'beforeSha256':hashlib.sha256(original).hexdigest(),'generatedSha256':hashlib.sha256(changed).hexdigest(),'restoredSha256':hashlib.sha256(path.read_bytes()).hexdigest(),'priorUserEditsAbsentVerified':True}
assert record['beforeSha256']==record['restoredSha256']
(QA/'GeneratedFingerprintCleanup.json').write_text(json.dumps(record,indent=2)+'\n')
print('Regenerated compiler fingerprint preserved under /tmp; original clean tracked version restored.')
