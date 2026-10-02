from pathlib import Path
import hashlib,json,shutil,subprocess,difflib
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
names=subprocess.check_output(['ps','-eo','comm='],text=True).splitlines()
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in names),'Wait for Unity before restoring generated build files'
before=json.loads((base/'Sources/BeforeHashes.json').read_text());cleaned=[]
for relative in ['Assets/AddressableAssetsData/Android/addressables_content_state.bin','Assets/AddressableAssetsData/Android/addressables_content_state.bin.meta','Assets/AddressableAssetsData/link.xml','Assets/AddressableAssetsData/link.xml.meta']:
    path=root/relative;saved=base/'Sources/Before'/relative
    if path.exists() and hashlib.file_digest(path.open('rb'),'sha256').hexdigest()==before.get(relative):continue
    if saved.exists():shutil.copy2(saved,path);cleaned.append({'path':relative,'action':'restored task-start generated build metadata'})
    elif relative not in before and path.exists():path.unlink();cleaned.append({'path':relative,'action':'removed newly generated build metadata'})
for path in (root/'Assets').glob('InitTestScene*'):
    relative=str(path.relative_to(root))
    if relative not in before and (path.name.endswith('.unity') or path.name.endswith('.unity.meta')):
        path.unlink();cleaned.append({'path':relative,'action':'removed temporary test scene'})
after={str(p.relative_to(root)):hashlib.file_digest(p.open('rb'),'sha256').hexdigest() for d in ['Assets','Packages','ProjectSettings','Backend'] for p in (root/d).rglob('*') if p.is_file() and '__pycache__' not in p.parts}
scope={'changed':sorted(p for p in before if p in after and before[p]!=after[p]),'added':sorted(set(after)-set(before)),'deleted':sorted(set(before)-set(after)),'unchanged':sum(after.get(p)==h for p,h in before.items())}
(base/'QA/Scope.json').write_text(json.dumps(scope,indent=2)+'\n');(base/'QA/GeneratedFileCleanup.json').write_text(json.dumps(cleaned,indent=2)+'\n')
patch=''
for relative in scope['changed']+scope['added']:
    target=base/'Sources/After'/relative;target.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(root/relative,target)
    old=(base/'Sources/Before'/relative).read_text() if relative in before else ''
    patch+=''.join(difflib.unified_diff(old.splitlines(True),(root/relative).read_text().splitlines(True),fromfile='a/'+relative if old else '/dev/null',tofile='b/'+relative))
(base/'Sources/TaskChanges.patch').write_text(patch)
print(json.dumps(scope,indent=2))
