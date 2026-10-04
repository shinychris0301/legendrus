"""Byte-verify reversible backups of generated packing caches before a fresh guarded build."""
from pathlib import Path
import hashlib,json,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5]; QA=Path(__file__).resolve().parent.parent/'QA'
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
target=Path('/dev/shm/lor-persistence-android-refresh-r2/BeforeFreshPacking'); target.mkdir(parents=True,exist_ok=False)
gradle='Library/Bee/Android/Prj/IL2CPP/Gradle/'
paths=['Library/PlayerDataCache/Android',gradle+'launcher/build/outputs/apk/debug',gradle+'unityLibrary/src/main/assets',gradle+'launcher/build/intermediates/compressed_assets',gradle+'launcher/build/intermediates/assets',gradle+'unityLibrary/build/intermediates/assets']
def inventory(folder):
    result=[]
    for p in sorted(folder.rglob('*')):
        assert not p.is_symlink(),str(p)
        if not p.is_file(): continue
        assert p.suffix.lower() not in ('.keystore','.jks','.pem','.key','.p12')
        with p.open('rb') as f: h=hashlib.file_digest(f,'sha256').hexdigest()
        result.append({'path':str(p.relative_to(folder)),'bytes':p.stat().st_size,'sha256':h})
    assert result
    return result
record=[]; initial=shutil.disk_usage(ROOT).free
for i,relative in enumerate(paths):
    source=ROOT/relative; assert source.is_dir() and not source.is_symlink()
    before=inventory(source); dest=target/str(i)
    assert shutil.disk_usage(target).free>sum(r['bytes'] for r in before)+1024**3
    shutil.copytree(source,dest); assert before==inventory(dest)
    assert before==inventory(source)
    shutil.rmtree(source)
    record.append({'source':relative,'preservedAt':str(dest),'files':before,'byteVerifiedBeforeOriginalRemoval':True})
    (QA/'FreshPackingCachePreservation.json').write_text(json.dumps(record,indent=2)+'\n')
    print('Preserved generated packing cache '+str(i+1)+'/6',flush=True)
final=shutil.disk_usage(ROOT).free
(QA/'PackingStorage.json').write_text(json.dumps({'initialFreeBytes':initial,'beforeBuildFreeBytes':final,'requiredFreeBytes':8*1024**3,'guardUnchanged':True,'signingOrCompilerCachesChanged':False,'preservationIsTemporary':True},indent=2)+'\n')
assert final>8*1024**3
print('Six caches preserved byte-for-byte under /dev/shm; unchanged 8 GiB disk guard satisfied.')
