"""Restore only the tracked generated compiler fingerprint snapshotted before this task."""
from pathlib import Path
import json,hashlib,shutil
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
r=json.loads((QA/'BeforeTrackedFingerprint.json').read_text());path=ROOT/r['path'];backup=Path(r['backup']);before=backup.read_bytes();assert hashlib.sha256(before).hexdigest()==r['sha256']
generated=path.read_bytes();changed=generated!=before;destination=Path('/tmp/lor-settings-android-refresh-r2/GeneratedFingerprint.bin')
if changed:
 assert not destination.exists();shutil.copy2(path,destination);path.write_bytes(before)
record={'path':r['path'],'generatedCopyPreservedAt':str(destination) if changed else None,'changedByTask':changed,'beforeSha256':r['sha256'],'generatedSha256':hashlib.sha256(generated).hexdigest(),'restoredSha256':hashlib.sha256(path.read_bytes()).hexdigest()}
assert record['beforeSha256']==record['restoredSha256'];(QA/'GeneratedFingerprintCleanup.json').write_text(json.dumps(record,indent=2)+'\n')
print('Tracked generated fingerprint checked/restored; generated copy preserved if changed.')
