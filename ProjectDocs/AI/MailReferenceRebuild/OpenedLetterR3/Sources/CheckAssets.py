from pathlib import Path
from PIL import Image
import numpy as np,json,sys,difflib
b=Path(__file__).resolve().parents[1];run=Path('/tmp/lor-mail-opened-r3')/sys.argv[1];before=b.parent/'TabsAndPreviewR2/UnityCaptures';out=Path('Assets/LegendsOfRus/Resources/ProductionUI/MailAligned/guild_emblem_r3.png')
a=np.array(Image.open(out).convert('RGBA'));src=np.array(Image.open(b/'Sources/Sheet.png').convert('RGB'));raw=np.array(Image.open(b/'Sources/Scenario/guild_emblem/booster_medallion_trial01.png').convert('RGBA'))
assert np.array_equal(a[:,:,:3],src);assert np.array_equal(a[:,:,3],raw[:,:,3]);record={'guildOriginalRgbUnchanged':True,'guildScenarioAlphaUnchanged':True,'inboxPixelComparisons':{}}
for n in ['Mail_941x1672_RU.png','Mail_941x1672_EN.png','Mail_576x1280_safe_RU.png','Mail_576x1280_safe_EN.png','Mail_System_RU.png','Mail_EmptySent_RU.png']:
 old=np.array(Image.open(before/n));new=np.array(Image.open(run/'Mail'/n));changed=np.any(old!=new,axis=2);ys,xs=np.where(changed);record['inboxPixelComparisons'][n]={'changedPixels':int(changed.sum()),'bounds':None if len(xs)==0 else [int(xs.min()),int(ys.min()),int(xs.max()),int(ys.max())],'note':'Preview surface repair intentionally changes parchment corners; occasional single-pixel header rasterization differences observed before this repair.'}
(b/'QA/AssetAndRegressionChecks.json').write_text(json.dumps(record,indent=2));diff=[]
for p in (b/'Sources').glob('*.cs.before'):
 current=Path('Assets/LegendsOfRus/Scripts/Runtime/UI')/p.name.removesuffix('.before');diff.extend(difflib.unified_diff(p.read_text().splitlines(True),current.read_text().splitlines(True),fromfile='R2/'+current.name,tofile='R3/'+current.name))
(b/'QA/RuntimeChanges.diff').write_text(''.join(diff));print('Source RGB/alpha preserved; six Inbox/preview differences recorded against R2.')
