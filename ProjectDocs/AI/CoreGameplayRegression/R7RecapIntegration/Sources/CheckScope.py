"""Check task-start preservation and the narrowly scoped conditional recap fixture change."""
from pathlib import Path
import ast
import difflib
import hashlib
import json

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent


def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream,'sha256').hexdigest()


def inspect():
    before=json.loads((BASE/'Sources/BeforeHashes.json').read_text())
    after={str(p.relative_to(ROOT)):sha(p) for directory in ['Assets','Packages','ProjectSettings','Backend']
           for p in (ROOT/directory).rglob('*') if p.is_file() and '__pycache__' not in p.parts
           and p.suffix.lower() not in ['.keystore','.jks','.p12','.pem','.key'] and not p.name.startswith('.env')}
    expected=['Assets/LegendsOfRus/Tests/PlayMode/StoryRecapPlayModeTests.cs']
    changed=sorted(path for path in before.keys()&after.keys() if before[path]!=after[path])
    added=sorted(after.keys()-before.keys());deleted=sorted(before.keys()-after.keys())
    assert changed==expected and not added and not deleted
    assert len(before)==len(after)==7833
    original=(BASE/'Sources/Before'/expected[0]).read_text()
    current=(ROOT/expected[0]).read_text()
    anchor='            Assert.That(Environment.GetEnvironmentVariable("LOR_GAMEPLAY_QA"),Is.EqualTo("fresh"));'
    guard='            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOR_GAMEPLAY_QA")))\n                Assert.Ignore("Requires the explicit isolated fresh gameplay-QA profile.");\n'
    assert original.count(anchor)==1 and current==original.replace(anchor,guard+anchor)
    assert all(before[p]==after[p] for p in before if p.endswith('.meta'))
    result={'baselineFiles':7833,'finalFiles':7833,'unchanged':7832,'changed':changed,'added':added,'deleted':deleted,
            'onlyExplicitQaFixturePreconditionAdded':True,'allRuntimeArtworkFontsPrefabsMetadataSettingsPackagesBackendGuardsUnchanged':True}
    (BASE/'QA/Scope.json').write_text(json.dumps(result,indent=2)+'\n')
    for p in (BASE/'Sources').glob('*.py'):
        ast.parse(p.read_text(),filename=str(p))
    assert not list(BASE.rglob('__pycache__'))
    return result


if __name__=='__main__':
    print(json.dumps(inspect(),indent=2))
