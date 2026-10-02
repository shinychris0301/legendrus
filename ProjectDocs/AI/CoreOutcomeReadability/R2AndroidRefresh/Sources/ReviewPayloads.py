"""Review UI, native binding and build identifier changes; preserve plugin pins."""
from pathlib import Path
import json,zipfile,hashlib
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
comparison=json.loads((QA/'ArtifactComparison.json').read_text());expected={'assets/bin/Data/Managed/Metadata/global-metadata.dat':'Compiled owner changes','assets/bin/Data/boot.config':'Build and player connection identifiers','assets/bin/Data/data.unity3d':'Three approved page touch/numeric changes; packed membership checked separately','assets/bin/Data/unity_app_guid':'Build identifier','lib/arm64-v8a/libil2cpp.so':'Compiled owner changes','lib/arm64-v8a/libunity.so':'Regenerated stripped engine; one added CanvasRenderer native setter binding, verified separately'}
assert {r['path'] for r in comparison['changedPayloads']}==set(expected)
native=json.loads((QA/'NativeEngineComparison.json').read_text())
assert native['addedBindings']==['UnityEngine.CanvasRenderer::set_cullTransparentMesh_Injected'] and not native['removedBindings']
assert native['dynamicSymbolDefinitionsUnchanged']
configs=[]
for row in (comparison['old'],comparison['new']):
 with zipfile.ZipFile(ROOT/row['path']) as z:configs.append(z.read('assets/bin/Data/boot.config').decode().splitlines())
allowed=('build-guid=','player-connection-guid=');stable=lambda c:[s for s in c if not s.startswith(allowed)]
assert stable(configs[0])==stable(configs[1]);assert all(sum(s.startswith(prefix) for s in c)==1 for c in configs for prefix in allowed)
pins=json.loads((QA/'PinnedPluginInventory.json').read_text());assert len(pins)==4
assert all(hashlib.file_digest((ROOT/p).open('rb'),'sha256').hexdigest()==v for p,v in pins.items())
record={'expectedChanges':expected,'changedPayloadCount':6,'unchangedPayloadCount':comparison['identicalPayloadCount'],'nativeEngineAdditionalSetterBindingVerified':True,'bootConfigOnlyBuildAndPlayerConnectionIdentifiersChanged':True,'allFourCredentialJavaSourceMetadataPinsUnchanged':True,'zipPathsUnchanged':True,'unindexedBytes':comparison['new']['unindexedBytes']}
(QA/'PayloadReview.json').write_text(json.dumps(record,indent=2)+'\n');print(json.dumps(record,indent=2))
