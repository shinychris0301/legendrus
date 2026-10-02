from pathlib import Path
from PIL import Image
import hashlib,json,numpy as np,sys
b=Path(__file__).resolve().parents[1];art=Path('Assets/LegendsOfRus/Resources/ProductionUI/ProfileAligned');before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text());records=json.loads((b/'Sources/Provenance.json').read_text())
protected=[p for p in before if p.endswith(('.png','.meta','.otf','.ttf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/Profile.prefab')]
assert all(Path(p).is_file() and hashlib.sha256(Path(p).read_bytes()).hexdigest()==before[p] for p in protected)
report={'protectedFiles':len(protected),'changedProtectedFiles':[],'assetChecks':{}}
for item in records:
 p=art/(item['name']+'.png');assert hashlib.sha256(p.read_bytes()).hexdigest()==item['sha256'];report['assetChecks'][item['name']]={'matchesProvenanceHash':True}
a=np.array(Image.open(art/'detail_surface_r1.png'));c=np.array(Image.open(art/'detail_r3_frame.png'));assert np.array_equal(a,c);report['detailFramePixelsEqualR1']=True
ref=Image.open(b.parent/'R1/Sources/Reference.png').convert('RGB');src=np.array(ref.crop((56,1270,260,1509)).resize((408,478),Image.Resampling.LANCZOS));a=np.array(Image.open(art/'detail_sword_r3.png'));assert np.array_equal(src[a[:,:,3]>0],a[:,:,:3][a[:,:,3]>0]);report['swordVisibleRgbEqualsSource']=True
strip=np.array(Image.open(art/'footer_strip_r3.png'))
for i,(x,w) in enumerate(((20,292),(322,162),(496,196))):
 a=np.array(Image.open(art/('footer_cell_'+str(i)+'_r3.png')));assert np.array_equal(strip[30:214,2*x:2*(x+w),:3],a[:,:,:3]);active=np.array(Image.open(art/('footer_cell_'+str(i)+'_active_r3.png')));edge=np.zeros(a.shape[:2],bool);edge[[0,-1],:]=True;edge[:,[0,-1]]=True;assert np.array_equal(a[edge],active[edge])
report['footerFacesAlignAndSelectedEdgesUnchanged']=True
run=Path('/tmp/lor-profile-footer-r3')/sys.argv[1];old=b.parent/'RelicFramesAndCaptionsR2/UnityCaptures';report['nativeComparisons']={}
for name in ('Profile_941x1672_RU.png','Profile_941x1672_EN.png','Profile_Items_RU.png','Profile_History_RU.png'):
 a=np.array(Image.open(old/name));c=np.array(Image.open(run/'Profile'/name));d=np.any(a!=c,axis=2);report['nativeComparisons'][name]={'changedPixels':int(d.sum()),'aboveDetail1266ChangedPixels':int(d[:1266].sum()),'beforeFooter1518ChangedPixels':int(d[:1518].sum())}
report['billing']=json.loads((b/'Sources/Scenario/footer_detail/Billing.json').read_text());(b/'QA/IntegrityChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
