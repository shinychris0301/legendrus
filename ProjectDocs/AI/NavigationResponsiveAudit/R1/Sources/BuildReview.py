from pathlib import Path
import json,shutil,hashlib,xml.etree.ElementTree as ET,html
root=Path.cwd(); base=root/'ProjectDocs/AI/NavigationResponsiveAudit/R1'; runs=Path('/tmp/lor-navigation-audit-r1');qa=base/'QA'; images=base/'Captures';images.mkdir(exist_ok=True)
results=[];latest={}
for run in ['Verify01','Harness02']:
 source=runs/run/'PlayMode'; dest=qa/run;dest.mkdir(exist_ok=True)
 for name in ['results.xml','unity.log','command.json','launcher.log']:shutil.copy2(source/name,dest/name)
 tree=ET.parse(source/'results.xml').getroot();results.append({'run':run,**tree.attrib})
 for case in tree.iter('test-case'):latest[case.get('fullname')]=case.get('result')
for folder in (runs/'Verify01').iterdir():
 if not folder.is_dir() or folder.name in ['PlayMode','test-config']:continue
 for p in folder.iterdir():
  if p.suffix in ['.png','.json'] and not p.name.startswith('Diagnostic_'):shutil.copy2(p,images/p.name)
before=json.loads((qa/'BeforeAssetsHashes.json').read_text());after={str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for p in (root/'Assets').rglob('*') if p.is_file()}
changed=[k for k,v in before.items() if after.get(k)!=v];added=sorted(set(after)-set(before));deleted=sorted(set(before)-set(after))
assert changed==['Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs'],changed
assert not added and not deleted,(added,deleted)
summary={'runs':results,'latestUniqueResults':{'passed':sum(v=='Passed' for v in latest.values()),'total':len(latest)},'captureCount':len(list(images.glob('*.png'))),'changedAssets':changed,'addedAssets':added,'deletedAssets':deleted,'unchangedAssets':len(before)-len(changed),'productionArtPrefabsRuntimeUnchanged':True}
(qa/'VerificationSummary.json').write_text(json.dumps(summary,indent=2)+'\n')
rows=[]
for suite in sorted(set(k.rsplit('.',1)[0] for k in latest)):
 tests={k:v for k,v in latest.items() if k.rsplit('.',1)[0]==suite};rows.append(f"| {suite.rsplit('.',1)[1]} | {len(tests)} | {sum(v=='Passed' for v in tests.values())} |")
report='''# Navigation and responsive-layout audit R1 — 2026-09-25

Audited the installed game after Characters R3. No production assets, prefabs, runtime code, builders, settings or revisions changed. This is a navigation/layout assessment, not a new screen or an exact-reference sign-off.

## Findings and next work

1. **Home and World Map: prioritize safe-edge layout.** Both actual 1080×2400 captures show a large scenery gap above the header and below the footer. Both use the shared header, but their active `SourceComposition` branches return after resizing only the environment. The edge-positioning code below that return does not run. At safe width990, the 1672-unit stage occupies about1759px of the2160px safe height, leaving about200px at each end. Fix each owning layout/builder/prefab coherently; preserve approved source artwork and the941×1672 composition. Expand World Map's scrolling area, while keeping its later-chapters-above ordering and border-only current-chapter highlight.
2. **Friends List: right-hand frame join.** The tall capture shows a gap between the upper right scroll ornament and the lower corner. Check the stretching/slicing of the list frame; its background and footer already extend. This is visual, not a failed route.
3. **Tall-content balance remains uneven.** Team Hub has a large gap before its footer; Inventory separates its grid and detail panel; Characters shows empty collection space after all available rows fit. These are visible composition differences, not proof of clipping or missing data. Preserve truthful preview labels and do not fill them with invented progress.
4. **Existing tests did not require edge anchoring.** Containment and raycast checks can pass while navigation floats far from an edge. Add explicit safe-top/safe-bottom distance assertions with the Home/World Map repair.

## Navigation and responsive coverage

Home's existing native tests exercised visible controls and social shortcut returns (including caller-aware Friends return), live shared balances, maximum values, repeated routing and language changes. World Map and Chapter Map covered locked content, selected-level routes, scrolling/500-slot recycling, press/drag cancellation and shared-header fitting. Friends, Team Hub, Inventory, Profile, Settings and Characters tests exercised their existing routes, state integrity, modal blocking, localized layouts and repeated binding.

The legacy smoke fixture now checks native interactive buttons rather than requiring obsolete screenshot hotspots. It explicitly enters Friends from Team Hub; tests the current Back face against a44px target at576px phone width instead of an obsolete100-unit square; and checks Error as a modal preserving its underlying screen. Added an isolated-save guard and removed unused hotspot helpers. Direct `router.Show` smoke coverage establishes construction/visibility, not user discoverability. Actual click paths are covered separately by the page fixtures.

| Fixture | Latest tests | Passed |
|---|---:|---:|
'''+ '\n'.join(rows)+'''

## Verification and limits

Verify01:72 tests,70 passed and2 obsolete-harness expectations failed (Error modal route and World Map Back height). Harness02 reran the corrected navigation fixture:4/4 passed. Latest results cover72 unique tests, all passed; the whole72-test suite was not repeated after the test-only correction. No ignored tests. Unity6000.5.9f1 compiled the updated tests and rendered the installed pages. XML, full logs and exact commands retained underQA.

'''+f"Retained {summary['captureCount']} actual Unity PNG captures, with available geometry/state sidecars. Reviewed tall RU captures for all nine page-specific fixtures. The HTML displays one portrait at a time; it is a capture viewer, not an interactive implementation. Other retained images were generated by the tests and are available for review, not all manually inspected pixel by pixel.\n\n{summary['unchangedAssets']} existing Assets files unchanged; only the existing navigation test changed. No new/deleted Assets or generation/Scenario calls. No physical Android device, on-screen keyboard, backend, purchase, performance or complete36-page regression audit. Existing ASTC desktop-decompression and unverified-font warnings remain; passing tests do not verify exact font identity.\n\nNext recommended implementation: Home safe edges first, then World Map safe edges/expanded scroll, preserving existing art.\n"
(base/'ImplementationReport.md').write_text(report)
(qa/'ReferenceFidelityReview.md').write_text('# Scope of visual review\n\nThis pass compared page behavior across display shapes, not every page against its original artwork. See the report for the confirmed Home/World Map edge gap, Friends frame join and remaining tall-content spacing. No exact-reference equivalence is claimed.\n')
files=sorted(images.glob('*.png')); first=images/'WorldMap_1080x2400_safearea_RU.png'; files.remove(first);files.insert(0,first)
options=''.join(f'<option value="Captures/{html.escape(p.name)}">{html.escape(p.stem)}</option>' for p in files)
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Navigation and responsive audit R1</title><style>body{margin:0;padding:18px;background:#090f18;color:#e9d7ac;font:16px/1.5 system-ui}main{max-width:760px;margin:auto}select{width:100%;padding:12px;background:#162237;color:#fff;border:1px solid #997642}img{display:block;width:100%;max-width:470px;height:auto;margin:20px auto}a{color:#9acaff}p{max-width:70ch}</style><main><h1>Navigation &amp; responsive audit</h1><p>Actual Unity Play Mode captures • simulated display and safe area • isolated test save.</p><p>72 unique checks pass across the two recorded runs. Home and World Map still need safe-edge positioning on tall screens; Friends List has a visible right-frame join.</p><p><a href="ImplementationReport.md">Findings and test coverage</a> · <a href="QA/VerificationSummary.json">Verification evidence</a></p><label for="view">Page / test state</label><select id="view">'''+options+'''</select><img id="page" alt="Actual Unity page capture" src="Captures/WorldMap_1080x2400_safearea_RU.png"><p>This viewer shows the existing implementation. No visual fixes were applied during the audit. Safe-area simulations are not physical-phone tests.</p></main><script>document.getElementById('view').addEventListener('change',e=>document.getElementById('page').src=e.target.value)</script></html>'''
(base/'Navigation_Responsive_Audit_R1_Review.html').write_text(page)
print(json.dumps({k:v for k,v in summary.items() if k!='runs'},indent=2))
