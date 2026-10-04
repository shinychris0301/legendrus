"""Preserve two historical generated packaging backups after the initial disk guard stopped."""
from pathlib import Path
import json,shutil,subprocess
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
p=QA/'FreshPackingCachePreservation.json';rows=json.loads(p.read_text());assert len(rows)==3
for n in ('BeforeCoreCaptionRefreshR4','BeforeCoreOutcomeUiRefreshR2'):
 source=ROOT/'Library/LegendsOfRusPackagingBackups'/n
 files=[f for f in source.rglob('*') if f.is_file()]
 assert len(files)==6 and not any(f.suffix.lower() in ('.keystore','.jks','.pem','.key','.p12') for f in files)
 destination=Path('/tmp/lor-narrative-android-refresh-r2/BeforeFreshPacking')/n
 assert not destination.exists()
 shutil.move(str(source),destination)
 rows.append({'source':str(source.relative_to(ROOT)),'preservedAt':str(destination),'reason':'Historical generated packaging backup; additional room required by unchanged 8GiB build guard'})
 p.write_text(json.dumps(rows,indent=2)+'\n')
assert shutil.disk_usage(ROOT).free > 8*1024**3
print('Five generated cache directories preserved; existing build disk guard satisfied.')
