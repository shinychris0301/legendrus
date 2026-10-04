from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent
old=ROOT/'ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh/Sources'
for name in ['VerifyApk.py','Compare.py','CompareNativeEngine.py','InspectCredentialDex.py','ComparePacked.py','ReviewPayloads.py','PreparePacking.py','RecordScope.py','Build.py','DailyRewardRefreshBuildAudit.cs']:
 s=(old/name).read_text()
 s=s.replace('ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh','ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh')
 s=s.replace('lor-daily-reward-android-refresh-r2','lor-spending-android-refresh-r2').replace('BeforeDailyRewardRefreshR2','BeforeSpendingRefreshR2')
 s=s.replace('DailyRewardRefreshBuildAudit','SpendingRefreshBuildAudit')
 if name=='Build.py':
  s=s.replace("ROOT / 'ProjectDocs/AI/AndroidGameplayProfile/R5AndroidRefresh/QA/Artifact.json'", "ROOT / 'ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh/QA/Artifact.json'")
  s=s.replace("(QA/test_run/'summary.json')", "(ROOT/'ProjectDocs/AI/SpendingRecovery/R1/QA'/test_run/'summary.json')")
 if name=='ComparePacked.py':
  s=s.replace("ROOT / 'ProjectDocs/AI/AndroidGameplayProfile/R5AndroidRefresh/QA/PackedAssets.json'", "ROOT / 'ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh/QA/PackedAssets.json'")
 if name=='ReviewPayloads.py':
  s=s.replace('IL2CPP metadata; no new service method signature or UI binding','IL2CPP metadata for atomic spending/credit APIs, validated optional booster-command receipt, and refund recovery')
  s=s.replace('Compiled daily-reward overflow rejection; native claim bytes checked separately','Compiled accumulated reward, credit, spending and durable booster-command recovery; native method bytes checked separately')
 name=name.replace('DailyRewardRefreshBuildAudit','SpendingRefreshBuildAudit')
 (BASE/'Sources'/name).write_text(s)
print('Existing guarded packaging and audit tools adapted; no project helper installed or build started.')
