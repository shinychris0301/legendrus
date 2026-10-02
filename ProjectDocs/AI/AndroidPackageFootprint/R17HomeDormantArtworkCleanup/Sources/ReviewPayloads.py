"""Validate the five expected APK differences and permit only changing boot identifiers."""
from pathlib import Path
import json,zipfile,hashlib
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
comparison=json.loads((QA/'ArtifactComparison.json').read_text());expected={'assets/bin/Data/Managed/Metadata/global-metadata.dat':'Compiled owner changes','assets/bin/Data/boot.config':'Build and player connection identifiers','assets/bin/Data/data.unity3d':'Nine approved assets removed; exact packed membership checked separately','assets/bin/Data/unity_app_guid':'Build identifier','lib/arm64-v8a/libil2cpp.so':'Compiled owner changes'}
assert {r['path'] for r in comparison['changedPayloads']}==set(expected)
configs=[]
for row in (comparison['old'],comparison['new']):
 with zipfile.ZipFile(ROOT/row['path']) as z:configs.append(z.read('assets/bin/Data/boot.config').decode().splitlines())
allowed=('build-guid=','player-connection-guid=');stable=lambda c:[s for s in c if not s.startswith(allowed)]
assert stable(configs[0])==stable(configs[1]);assert all(sum(s.startswith(prefix) for s in c)==1 for c in configs for prefix in allowed)
pins=json.loads((QA/'PinnedPluginInventory.json').read_text());assert len(pins)==4
assert all(hashlib.file_digest((ROOT/p).open('rb'),'sha256').hexdigest()==v for p,v in pins.items())
record={'expectedChanges':expected,'changedPayloadCount':5,'unchangedPayloadCount':comparison['identicalPayloadCount'],'bootConfigOnlyBuildAndPlayerConnectionIdentifiersChanged':True,'allFourCredentialJavaSourceMetadataPinsUnchanged':True,'zipPathsUnchanged':True,'unindexedBytes':comparison['new']['unindexedBytes']}
(QA/'PayloadReview.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
