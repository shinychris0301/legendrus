from pathlib import Path
import ast, difflib, hashlib, json, re, subprocess

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
before=json.loads((BASE/'Sources/BeforeHashes.json').read_text())
after={str(p.relative_to(ROOT)):hashlib.sha256(p.read_bytes()).hexdigest() for directory in ['Assets','Packages','ProjectSettings','Backend'] for p in (ROOT/directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts and p.suffix.lower() not in ('.keystore','.jks','.p12','.pem','.key') and not p.name.startswith('.env')}
changed=sorted(k for k in before.keys()&after.keys() if before[k]!=after[k])
added=sorted(after.keys()-before.keys());deleted=sorted(before.keys()-after.keys())
expected=['Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab',
          'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplaySourceControls.cs',
          'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayView.cs',
          'Assets/LegendsOfRus/Tests/PlayMode/GameplayAlignmentPlayModeTests.cs']
new=['Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayStockLayout.cs',
     'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/GameplayStockLayoutEditModeTests.cs']
assert changed==sorted(expected) and added==sorted(new+[p+'.meta' for p in new]) and not deleted
assert all(before[k]==after[k] for k in before if k.endswith('.meta'))
assert all(before[k]==after[k] for k in before if k.endswith(('.png','.ttf','.otf','.mat')))
record={'baselineFiles':len(before),'unchanged':sum(before[k]==after[k] for k in before.keys()&after.keys()),'changed':changed,'added':added,'deleted':deleted,'allExistingMetaArtFontMaterialBytesUnchanged':True}
(BASE/'QA/Scope.json').write_text(json.dumps(record,indent=2)+'\n')
patch=[]
for path in changed:
    saved=BASE/'Sources/Before'/path
    assert hashlib.sha256(saved.read_bytes()).hexdigest()==before[path]
    patch.extend(difflib.unified_diff(saved.read_text().splitlines(True),(ROOT/path).read_text().splitlines(True),fromfile='a/'+path,tofile='b/'+path))
for path in added:
    patch.extend(difflib.unified_diff([], (ROOT/path).read_text().splitlines(True),fromfile='/dev/null',tofile='b/'+path))
(BASE/'Sources/TaskChanges.patch').write_text(''.join(patch))
subprocess.run(['git','apply','--reverse','--check',str(BASE/'Sources/TaskChanges.patch')],cwd=ROOT,check=True)
path='Assets/LegendsOfRus/Resources/ProductionUI/Pages/Gameplay.prefab'
# Keys are object IDs; class IDs and serialized bodies are preserved separately.
def objects(text):
    return {m.group(2):(m.group(1),m.group(3)) for m in re.finditer(r'^--- !u!(\d+) &(-?\d+)\n(.*?)(?=^--- |\Z)',text,re.M|re.S)}
old=objects((BASE/'Sources/Before'/path).read_text());current=objects((ROOT/path).read_text())
assert set(old)<=set(current)
diffs={key:(old[key][0],old[key][1],current[key][1]) for key in old if old[key]!=current[key]}
assert len(diffs)==9 and len(current)-len(old)==20
for key,(kind,a,z) in diffs.items():
    if kind=='114':assert a.replace('  Revision: 23\n','  Revision: 24\n')==z
    elif kind=='1':
        a_components=re.findall(r'^  - component: \{fileID: (-?\d+)\}',a,re.M)
        z_components=re.findall(r'^  - component: \{fileID: (-?\d+)\}',z,re.M)
        assert z_components[:-1]==a_components and len(z_components)==len(a_components)+1
        assert re.sub(r'^  - component: .*\n','',a,flags=re.M)==re.sub(r'^  - component: .*\n','',z,flags=re.M)
    elif kind=='224':
        assert re.sub(r'^  m_Children:\n(?:  - \{fileID: .*\}\n)*','',a,flags=re.M)==re.sub(r'^  m_Children:\n(?:  - \{fileID: .*\}\n)*','',z,flags=re.M)
    else:raise AssertionError((key,kind))
proof={'originalObjects':len(old),'currentObjects':len(current),'retainedOriginalIds':True,'newObjects':20,
       'changedOriginalObjects':9,'onlyRevisionNewCountComponentsAndBackingChildrenChanged':True,
       'originalGeometryFontsSpritesTextColorsTargetsAndOtherBindingsUnchanged':True}
(BASE/'QA/PrefabPreservation.json').write_text(json.dumps(proof,indent=2)+'\n')
for script in (BASE/'Sources').glob('*.py'):ast.parse(script.read_text())
print(json.dumps(record,indent=2))
