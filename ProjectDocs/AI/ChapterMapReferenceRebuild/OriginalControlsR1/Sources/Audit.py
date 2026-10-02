from pathlib import Path
import json,hashlib,re,ast
import numpy as np
from PIL import Image
p=Path(__file__).resolve().parent.parent;root=Path.cwd();prod=root/'Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapReferenceAligned'
checks={}
for n in ['banner','play','back','footer']:
 src=np.array(Image.open(p/'Sources/Inputs'/f'{n}.png').convert('RGB'));out=np.array(Image.open(prod/f'{n}.png').convert('RGBA'));checks[n+'_original_rgb_exact']=bool(np.all(src==out[:,:,:3]))
for n,source in [('banner_blank','banner'),('play_blank','play'),('node_blank','node')]:
 src=np.array(Image.open(p/'Sources/Inputs'/f'{source}.png').convert('RGB'));out=np.array(Image.open(prod/f'{n}.png').convert('RGBA'));mask=np.array(Image.open(p/'Sources'/f'{n}_repair_mask.png'))>0;checks[n+'_original_rgb_outside_repair']=bool(np.all(src[~mask]==out[:,:,:3][~mask]))
src=np.array(Image.open(p/'Sources/OriginalReference.png').convert('RGB'));out=np.array(Image.open(prod/'environment.png').convert('RGB'));mask=np.array(Image.open(p/'Sources/EnvironmentRepairSupport.png'))>0;checks['environment_original_pixels_outside_repair']=bool(np.all(src[~mask]==out[~mask]));checks['no_script_parse_errors']=all(ast.parse(f.read_text()) is not None for f in (p/'Sources').glob('*.py'))
assert all(checks.values()),checks
old=json.loads((p/'Sources/BeforeAssetsHashes.json').read_text());new={str(x.resolve()):hashlib.sha256(x.read_bytes()).hexdigest()for x in Path('Assets').rglob('*')if x.is_file()};changed=[k for k,v in old.items()if new.get(k)!=v];added=[k for k in new if k not in old];before=(p/'Sources/ChapterMap_before.prefab.txt').read_text();after=Path('Assets/LegendsOfRus/Resources/ProductionUI/Pages/ChapterMap.prefab').read_text();ids=lambda s:set(re.findall(r'^--- !u!\d+ &(\d+)',s,re.M));a=ids(before);b=ids(after)
report={'checks':checks,'changed':[str(Path(k).relative_to(root)) for k in changed],'added':[str(Path(k).relative_to(root)) for k in added],'prefabRecords':{'before':len(a),'after':len(b),'removed':list(a-b)},'scenarioAcceptedJobs':8,'scenarioCU':sum(json.loads(x.read_text())['cuCost'] for x in (p/'Sources/Scenario').glob('*/Billing.json'))};(p/'QA/ChangeAudit.json').write_text(json.dumps(report,indent=2));print(json.dumps({k:v for k,v in report.items()if k not in ['changed','added']},indent=2))
