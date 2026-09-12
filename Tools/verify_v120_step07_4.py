#!/usr/bin/env python3
from pathlib import Path
import hashlib, json, re, sys

ROOT = Path(__file__).resolve().parents[1]
failures=[]

def require(rel):
    p=ROOT/rel
    if not p.exists(): failures.append(f"missing: {rel}")
    return p

def sha(p):
    h=hashlib.sha256()
    with p.open('rb') as f:
        for chunk in iter(lambda:f.read(1024*1024), b''): h.update(chunk)
    return h.hexdigest()

milestone=require('MILESTONE_VERSION.txt')
if milestone.exists() and milestone.read_text().strip()!='v1.20-step07.4-runtime-capture-dynamic-map-conditional-cutover':
    failures.append('milestone version mismatch')

decision=require('Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_CUTOVER_DECISION.json')
dynamic=require('Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_DYNAMIC_MAP_DECOMPOSITION.json')
checklist=require('ProjectDocs/V1_20_STEP07_4/STEP07_4_EXECUTION_CHECKLIST.json')
for p in (decision,dynamic,checklist):
    if p.exists():
        try: json.loads(p.read_text())
        except Exception as e: failures.append(f'invalid json {p.relative_to(ROOT)}: {e}')
if decision.exists():
    d=json.loads(decision.read_text())
    if d.get('decision')!='NO_GO': failures.append('packaged cutover decision must be NO_GO')
    if d.get('developmentCutoverApplied'): failures.append('cutover must not be pre-applied')

for forbidden in (
 'Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_CUTOVER_APPLIED.json',
 'Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_ANDROID_QA_PASS.lock',
 'ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_CAPTURE_EXECUTED.json',
 'ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures/UNITY_RUNTIME_VISUAL_QA_PASS.lock',
 'Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_MAP_STATE_QA_PASS.lock'):
    if (ROOT/forbidden).exists(): failures.append(f'packaged evidence must be absent: {forbidden}')

required_scripts=[
 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionMapVisualStateController.cs',
 'Assets/LegendsOfRus/Scripts/Runtime/UI/Step74RuntimeCaptureHarness.cs',
 'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01Step74RuntimeCapture.cs',
 'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01Step74RuntimeFidelityAnalyzer.cs',
 'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01Step74MapStateQa.cs',
 'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01Step74ConditionalCutover.cs',
 'Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01Step74SelfTest.cs']
for rel in required_scripts: require(rel); require(rel+'.meta')

for page in ('WorldMap','ChapterMap'):
    for name in ('dynamic_node_frame.png','star_filled.png','star_empty.png','selected_halo.png'):
        rel=f'Assets/LegendsOfRus/Art/Production/Wave01DynamicMaps/{page}/{name}'
        require(rel); require(rel+'.meta')

# Unique Unity GUID audit.
guids={}
for p in ROOT.rglob('*.meta'):
    m=re.search(r'^guid:\s*([0-9a-fA-F]{32})\s*$', p.read_text(errors='ignore'), re.M)
    if not m: continue
    g=m.group(1).lower()
    if g in guids: failures.append(f'duplicate meta guid {g}: {guids[g]} and {p.relative_to(ROOT)}')
    else: guids[g]=p.relative_to(ROOT)

# No rejected destructive/inpaint map artifacts.
for p in ROOT.rglob('*'):
    if p.is_file() and ('neutralized' in p.name.lower() or 'removed_state_mask' in p.name.lower()):
        failures.append(f'rejected map artifact present: {p.relative_to(ROOT)}')

# Static policy audit on the conditional cutover source.
cutover=require('Assets/LegendsOfRus/Scripts/Editor/Step20ProductionAssetWave01Step74ConditionalCutover.cs')
if cutover.exists():
    t=cutover.read_text()
    for needle in ('Application.isBatchMode', 'File.Exists(AppliedPath)', 'BackupActivePrefabs()', 'Roll Back Eligible 8-Page Cutover'):
        if needle not in t: failures.append('cutover safety policy missing: '+needle)

print('v1.20 Step 7.4 static source verifier')
print('root:', ROOT)
print('meta GUIDs:', len(guids))
if failures:
    print('FAIL', len(failures))
    for f in failures: print('-',f)
    sys.exit(1)
print('PASS: source/package readiness checks only; Unity runtime and physical Android evidence are intentionally not claimed.')
