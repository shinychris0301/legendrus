from pathlib import Path
import hashlib,json,sys
from PIL import Image
import numpy as np
b=Path(__file__).resolve().parents[1];before=json.loads((b/'Sources/BeforeAssetsHashes.json').read_text())
allowedMeta="Assets/LegendsOfRus/Resources/ProductionUI/SettingsAligned/frame.png.meta"
protected=[p for p in before if p!=allowedMeta and (p.endswith(('.png','.meta','.ttf','.otf')) or '/Pages/' in p and p.endswith('.prefab') and not p.endswith('/Settings.prefab'))]
changed=[p for p in protected if not Path(p).exists() or hashlib.sha256(Path(p).read_bytes()).hexdigest()!=before[p]]
assert not changed
restored=Path(allowedMeta).read_text().replace("spriteBorder: {x: 60, y: 220, z: 60, w: 90}","spriteBorder: {x: 0, y: 0, z: 0, w: 0}")
assert hashlib.sha256(restored.encode()).hexdigest()==before[allowedMeta]
report={'protectedExistingFiles':len(protected),'changedProtectedFiles':changed,'intentionalMetaEdit':allowedMeta,'onlySpriteBorderChanged':True,'comparisons':{}}
(b/'QA/IntegrityChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report,indent=2))
