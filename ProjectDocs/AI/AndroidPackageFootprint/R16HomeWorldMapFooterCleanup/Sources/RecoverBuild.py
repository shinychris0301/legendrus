"""Recover evidence after the runner terminated following a successful Unity report."""
from Build import ROOT,BASE,QA,LOGS,UNITY,require_idle,sha,save
import json
require_idle()
report=json.loads((QA/'PackedAssets.json').read_text());assert report['result']=='Succeeded' and report['assets']
policy=json.loads((ROOT/'Assets/LegendsOfRus/Editor/AndroidBuild/Config/android_build_foundation_policy.json').read_text());apk=ROOT/policy['developmentOutputRelativePath']
assert apk.is_file();assert str(apk.resolve())==str(__import__('pathlib').Path(report['output']).resolve())
previous=json.loads((QA/'PreviousArtifact.json').read_text());old=ROOT/previous['path'];assert sha(old)==previous['sha256'];digest=sha(apk);assert digest!=previous['sha256']
assert not any(r['guid']=='51e28d72f0853db939602ec4f6546f7c' for r in report['assets'])
log=LOGS/'Build.unity.log';assert 'Exiting batchmode successfully now!' in log.read_text(errors='replace')
helper=ROOT/'Assets/LegendsOfRus/Scripts/Editor/HomeWorldMapFooterCleanupBuildAudit.cs';assert helper.read_bytes()==(BASE/'Sources'/helper.name).read_bytes();helper.unlink()
meta=helper.with_suffix('.cs.meta');metadata_removed=meta.exists()
if metadata_removed:meta.unlink()
save('Build/summary.json',{'command':[str(UNITY),'-batchmode','-nographics','-projectPath',str(ROOT),'-logFile',str(log),'-executeMethod','HomeWorldMapFooterCleanupBuildAudit.Build','-quit'],'exitCode':None,'runnerExitCode':143,'unityReportedBuildResult':'Succeeded','unitySuccessShutdownMarker':True,'recoveredEvidence':True,'note':'Runner terminated before its post-Unity summary/artifact/finally work. Unity exit code was not captured. Successful actual packed report and shutdown marker plus a distinct output APK were verified; complete package checks follow.'})
save('BuildRecovery.json',{'runnerExitCode':143,'unityExitCodeCaptured':False,'successfulBuildReportVerified':True,'priorApkHashVerified':True,'newApkExistsAndDiffers':True,'helperBytesMatchedBeforeRemoval':True,'helperRemoved':True,'helperMetadataRemoved':metadata_removed,'guardsChanged':False})
save('Artifact.json',{'path':str(apk.relative_to(ROOT)),'bytes':apk.stat().st_size,'sha256':digest,'installed':False,'deviceTested':False,'buildEvidenceRecovered':True})
apk.with_suffix('.apk.sha256').write_text(digest+'  '+apk.name+'\n')
print('Recovered successful Unity report and distinct APK, wrote actual hash/summary, removed byte-verified helper/meta. Package/signature/scope checks pending.')
