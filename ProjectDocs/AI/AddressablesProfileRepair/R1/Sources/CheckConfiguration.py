from pathlib import Path
import json,re
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
relative='Assets/AddressableAssetsData/AddressableAssetSettings.asset'
before=(base/'Sources/Before'/relative).read_text();after=(root/relative).read_text()
expected=before.replace('  m_RemoteCatalogBuildPath:\n    m_Id: \n','  m_RemoteCatalogBuildPath:\n    m_Id: a6384b3b5eba4466eba72fa53deca722\n').replace('  m_RemoteCatalogLoadPath:\n    m_Id: \n','  m_RemoteCatalogLoadPath:\n    m_Id: 526b2c53bf82a967c9b847fb776cc8b4\n')
assert after==expected, 'Unexpected serialized Addressables settings change'
assert '  m_BuildRemoteCatalog: 0' in after
build=(base/'QA/Build/unity.log').read_text()
assert 'ProfileValueReference: GetValue called with empty id.' not in build
assert 'Addressable content successfully built' in build
assert 'v1.17 Step 5 Addressables local foundation validation passed.' in (base/'QA/FoundationValidation/unity.log').read_text()
prior=(root/'ProjectDocs/AI/AndroidInputCompatibility/R1/QA/Build/unity.log').read_text()
assert 'ProfileValueReference: GetValue called with empty id.' in prior
record={'onlySettingsChanges':'Remote catalog build/load IDs now reference existing Remote.BuildPath and Remote.LoadPath entries','remoteCatalogEnabled':False,'remoteLoadValue':'<undefined> (unchanged; hosting not configured)','profileValuesAndGroupMembershipUnchanged':True,'priorBuildEmptyIdWarnings':prior.count('ProfileValueReference: GetValue called with empty id.'),'currentBuildEmptyIdWarnings':0,'addressableBuildSucceeded':True,'foundationValidatorPassed':True,'packageSourceChanged':False,'phoneExecution':False,'diagnosis':'BuildLayoutGenerationTask.GenerateBuildLayout evaluates RemoteCatalogBuildPath even when BuildRemoteCatalog is false. Serialized empty IDs bypass the installed package getter null check.'}
(base/'QA/ConfigurationEvidence.json').write_text(json.dumps(record,indent=2)+'\n')
print(json.dumps(record,indent=2))
