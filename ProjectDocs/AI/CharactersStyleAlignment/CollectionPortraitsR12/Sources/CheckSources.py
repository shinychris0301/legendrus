from pathlib import Path
from PIL import Image
import hashlib,json,numpy as np,ast,subprocess
b=Path(__file__).resolve().parent;qa=b.parent/'QA';assessment=json.loads((b/'Assessment.json').read_text());original=Path(assessment['original']);checks=[]
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
assert sha(original)==assessment['sha256'];checks.append('Original reference hash')
im=Image.open(original).convert('RGB')
for n,box in assessment['guidanceBounds'].items():
 p=b/'Inputs'/(n+'.png');assert np.array_equal(np.array(im.crop(box)),np.array(Image.open(p).convert('RGB')))
 ref=json.loads((b/'Scenario'/n/'Reference.json').read_text());assert ref['sourceSHA256']==sha(p)
 o=b/'Scenario'/n/'booster_medallion_trial01.png';assert json.loads((b/'Scenario'/n/'Output.json').read_text())['sha256']==sha(o)
checks.append('Two exact source guidance crops/upload hashes and downloaded output hashes')
prov=json.loads((b.parent/'AssetProvenance.json').read_text())
for r in prov['assets']:
 p=Path(r['asset']);source=Path(r['source']);assert sha(p)==r['sha256'];assert sha(source)==r['sourceSha256']
 if r.get('originalRGBPreserved'):
  a=np.array(Image.open(p).convert('RGBA'));old=np.array(Image.open(source).convert('RGBA'));assert np.array_equal(a[:,:,:3],old[:,:,:3]);assert np.array_equal(a[:-8],old[:-8]);assert (a[:,:,3]<=old[:,:,3]).all()
 else:assert p.read_bytes()==source.read_bytes()
checks.append('Ten production hashes; eight portrait RGB arrays and alpha above the bottom eight pixels preserved; two plates byte-identical to Scenario outputs')
for p in b.glob('*.py'):ast.parse(p.read_text())
checks.append('All source Python files parsed')
paths=['Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCharactersRuntime.cs','Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCharactersView.cs','Assets/LegendsOfRus/Scripts/Editor/ProductionCharactersPrefabBuilder.cs','Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/CharactersAlignmentEditModeTests.cs']
subprocess.run(['git','diff','--check','--',*paths],check=True);checks.append('Scoped git diff whitespace')
(qa/'SourceChecks.json').write_text(json.dumps({'passed':checks},indent=2));print('Source/hash/RGB/alpha integrity and Python/whitespace checks passed.')
