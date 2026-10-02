"""Run a temporary measurement fixture in the existing assembly with isolated saves."""
from pathlib import Path
import hashlib
import json
import os
import resource
import shutil
import subprocess
import sys
import time
import xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
QA=BASE/'QA'
run_name=sys.argv[1] if len(sys.argv)>1 else 'Runtime'
assert run_name.isalnum()
LOCAL=Path('/tmp/lor-shared-texture-r3')/run_name
source=BASE/'Sources/SharedTextureAuditPlayModeTests.cs'
target=ROOT/'Assets/LegendsOfRus/Tests/PlayMode'/source.name
assert not target.exists() and not target.with_suffix('.cs.meta').exists()
assert not any(n.strip() in ('Unity','Unity Main Thre') for n in subprocess.check_output(['ps','-eo','comm='],text=True).splitlines())
out=QA/run_name;out.mkdir(exist_ok=False)
config=LOCAL/'config';license_parent=config/'unity3d/Unity';license_parent.mkdir(parents=True)
(license_parent/'licenses').symlink_to('/home/k2/.config/unity3d/Unity/licenses',target_is_directory=True)
env={k:v for k,v in os.environ.items() if not k.startswith('LOR_')}
env.update(XDG_CONFIG_HOME=str(config),LOR_LOADING_TEST_CONFIG_ROOT=str(config),LOR_SHARED_TEXTURE_AUDIT_DIR=str(out))
command=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(ROOT),
    '-runTests','-testPlatform','PlayMode','-testFilter','LegendsOfRus.Tests.PlayMode.SharedTextureAuditPlayModeTests',
    '-testResults',str(out/'results.xml'),'-logFile',str(LOCAL/'unity.log'),'-force-vulkan']
def limits():resource.setrlimit(resource.RLIMIT_NOFILE,(4096,resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
shutil.copy2(source,target)
start=time.time()
try:
    with (LOCAL/'launcher.log').open('w') as log:
        code=subprocess.call(command,env=env,stdout=log,stderr=subprocess.STDOUT,preexec_fn=limits)
    record={'command':command,'exitCode':code,'durationSeconds':time.time()-start,'isolatedConfig':str(config),'processOnlyVulkan':True}
    if (out/'results.xml').exists():
        tree=ET.parse(out/'results.xml');record['result']=tree.getroot().attrib
        record['nonPassing']=[{'name':t.get('fullname'),'result':t.get('result'),'message':t.findtext('.//message')}
            for t in tree.findall('.//test-case') if t.get('result')!='Passed']
    (out/'summary.json').write_text(json.dumps(record,indent=2)+'\n')
    print(json.dumps(record,indent=2),flush=True)
finally:
    assert target.read_bytes()==source.read_bytes(),'Temporary fixture changed externally; retain for review'
    target.unlink()
    if target.with_suffix('.cs.meta').exists():target.with_suffix('.cs.meta').unlink()
    before=json.loads((QA/'BeforeHashes.json').read_text())
    cleaned=[]
    for p in (ROOT/'Assets').glob('InitTestScene*'):
        rel=str(p.relative_to(ROOT))
        if rel not in before and p.name.endswith(('.unity','.unity.meta')):
            p.unlink();cleaned.append(rel)
    (QA/'TemporaryFixtureCleanup.json').write_text(json.dumps({'fixtureRemoved':str(target.relative_to(ROOT)),'newTestScenesRemoved':cleaned},indent=2)+'\n')
assert code==0 and record.get('result',{}).get('result')=='Passed' and int(record['result']['passed'])>0
