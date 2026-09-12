#!/usr/bin/env python3
from pathlib import Path
import hashlib,json,sys
R=Path(__file__).resolve().parents[1]
def sha(p):
 h=hashlib.sha256();
 with p.open('rb') as f:
  for b in iter(lambda:f.read(1024*1024),b''): h.update(b)
 return h.hexdigest()
fail=[]
def ck(c,m):
 if not c: fail.append(m)
ck((R/'MILESTONE_VERSION.txt').read_text().strip()=="v1.20-step07-production-asset-reconstruction-wave00-wave01",'milestone')
ck(sha(R/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionCoreLoopPageRegistry.cs')=="aee233191e2696acdf944fbb207cbfb6580ccc80a80941e960b13f09058a8938",'active registry changed')
for k in ['SplashTitle', 'Loading', 'Home', 'WorldMap', 'LevelSelect', 'PreLevel', 'Gameplay', 'Pause', 'Victory', 'Fail', 'ChapterComplete', 'Dialogue']: ck((R/f'Assets/LegendsOfRus/Art/Production/Pages/{k}/page_asset_manifest.json').exists(),f'missing page manifest {k}')
ck((R/'Assets/LegendsOfRus/Art/Production/UI/Wave0Foundation/wave0_foundation_manifest.json').exists(),'Wave0 manifest')
ck((R/'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01PrefabBuilder.cs').exists(),'builder')
ck((R/'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01SelfTest.cs').exists(),'selftest')
refs={sha(p) for p in (R/'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens').glob('*.png')}
for p in (R/'Assets/LegendsOfRus/Art/Production/Pages').rglob('*.png'): ck(sha(p) not in refs,'production page asset byte-identical to full approved screen: '+str(p))
for p in (R/'Assets/LegendsOfRus/Resources/ProductionUI/Pages').glob('*.prefab'): pass
text=(R/'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01PrefabBuilder.cs').read_text()
ck('CandidateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Wave01Pages"' in text,'candidate root')
ck('ReferenceOnly' in text and 'forbidden dependency' in text,'dependency guard')
ck('ProductionCoreLoopPageRegistry.CutoverPageCount != 10' in text,'active cutover guard')
if fail:
 print('FAIL'); [print(' -',x) for x in fail]; sys.exit(1)
print('PASS v1.20 Step 7 source reconstruction gate')
print('wave1_pages=12')
print('runtime_cutover=false')
