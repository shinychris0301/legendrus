"""Pin the refreshed player and extend offline evidence checks to all transaction owners."""
from pathlib import Path
import json,re
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent
artifact=json.loads((BASE/'QA/Artifact.json').read_text())
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text()
s=s.replace("BUILD = Path('ProjectDocs/AI/DailyRewardRecovery/R2AndroidRefresh/QA')","BUILD = Path('ProjectDocs/AI/SpendingRecovery/R2AndroidRefresh/QA')")
s=re.sub(r"PIN = \{'bytes': \d+,\n       'sha256': '[a-f0-9]+'\}",f"PIN = {{'bytes': {artifact['bytes']},\n       'sha256': '{artifact['sha256']}'}}",s,count=1)
methods=list(json.loads((BASE/'QA/AfterNative.json').read_text())['methods'])
spends=['GameState_'+n for n in ('SpendGold','SpendGems','SpendEnergy','TryUseBooster')]
overflows=['GameState_'+n for n in ('ClaimDailyReward','TryDoubleLastLevelGoldReward','TryCompleteLevel','TryCredit','TrySettleBoosterCommand')]
needle="PRESETS = ('stock-999', 'stock-1000', 'stock-9999', 'stock-max', 'story-early', 'story-finale')"
s=s.replace(needle,needle+'\nNATIVE_METHODS = '+repr(tuple(methods))+'\nATOMIC_SPENDS = '+repr(tuple(spends))+'\nCHECKED_CREDITS = '+repr(tuple(overflows)))
s=s.replace("'NativeClaimVerification.json'","'AfterNative.json'")
start=s.index("            and native.get('compiledOverflowRejectionPackaged') is True")
end=s.index("            and verification.get('presetRuntimeTypes'",start)
s=s[:start]+'''            and final.get('atomicSpendingAndDurableBoosterRefundRecoveryPackaged') is True
            and final.get('doubledGoldAndFirstClearRecoveryPackaged') is True
            and final.get('ordinaryCreditRecoveryPackaged') is True
            and final.get('dailyRewardOverflowRejectionPackaged') is True
            and final.get('nativeMethodCount') == 22
            and native.get('stage') == 'After'
            and native.get('validatedReceiptAndRecoveryMetadataPresent') is True
            and native.get('packagedLibraryMatchesGradleInput') is True
            and set(native.get('methods', {})) == set(NATIVE_METHODS)
            and all(native.get('methods', {}).get(owner, {}).get('packagedMethodBytesMatchCompiledOutput') is True
                    for owner in NATIVE_METHODS)
            and all(native.get('methods', {}).get(owner, {}).get('generatedOverflowCatch') is True
                    for owner in CHECKED_CREDITS)
            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False
                    for owner in ATOMIC_SPENDS)
            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')
            and isinstance(final.get('nativeRecoveryLibrarySha256'), str)
            and re.fullmatch('[a-f0-9]{64}', final.get('nativeRecoveryLibrarySha256')) is not None
''' +s[end:]
s=s.replace('dailyRewardOverflowCatchPackaged=bool(matches),','dailyRewardOverflowCatchPackaged=bool(matches),\n                      atomicSpendingAndBoosterRefundRecoveryPackaged=bool(matches),\n                      accumulatedRewardAndCreditRecoveryPackaged=bool(matches),')
p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text().replace("'NativeClaimVerification.json'","'AfterNative.json'").replace("'nativeClaimLibrarySha256'","'nativeRecoveryLibrarySha256'")
a=s.index("            'BuildValidation.json':");b=s.index("            'PackedAssets.json':",a)
s=s[:a]+'''            'BuildValidation.json': {'result': 'Passed', 'artifactSha256': pin['sha256'], 'artifactBytes': pin['bytes'],
                'nativeRecoveryLibrarySha256': 'b' * 64, 'nativeMethodCount': 22,
                'atomicSpendingAndDurableBoosterRefundRecoveryPackaged': True,
                'doubledGoldAndFirstClearRecoveryPackaged': True, 'ordinaryCreditRecoveryPackaged': True,
                'dailyRewardOverflowRejectionPackaged': True},
            'AfterNative.json': {'stage': 'After', 'validatedReceiptAndRecoveryMetadataPresent': True,
                'packagedLibraryMatchesGradleInput': True, 'packagedLibrarySha256': 'b' * 64,
                'methods': {owner: {'packagedMethodBytesMatchCompiledOutput': True,
                    'generatedOverflowCatch': owner in tool.CHECKED_CREDITS,
                    'callsAtomicBalanceCommit': owner in tool.ATOMIC_SPENDS, 'callsPersist': False}
                    for owner in tool.NATIVE_METHODS}},
''' +s[b:]
a=s.index("        ('unverified compiled overflow rejection'");b=s.index("        ('stale daily owner'",a)
s=s[:a]+'''        ('unverified compiled recovery', 'BuildValidation.json', 'atomicSpendingAndDurableBoosterRefundRecoveryPackaged', False),
        ('missing native owners', 'AfterNative.json', 'methods', {}),
        ('wrong native library evidence hash', 'BuildValidation.json', 'nativeRecoveryLibrarySha256', '0' * 64),
        ('missing native receipt validation', 'AfterNative.json', 'validatedReceiptAndRecoveryMetadataPresent', False),
        ('missing ordinary credit recovery', 'BuildValidation.json', 'ordinaryCreditRecoveryPackaged', False),
        ('missing doubled and first-clear recovery', 'BuildValidation.json', 'doubledGoldAndFirstClearRecoveryPackaged', False),
        ('missing daily rejection', 'BuildValidation.json', 'dailyRewardOverflowRejectionPackaged', False),
        ('wrong native method count', 'BuildValidation.json', 'nativeMethodCount', 21),
        ('stale native stage', 'AfterNative.json', 'stage', 'Before'),
        ('wrong Gradle library evidence', 'AfterNative.json', 'packagedLibraryMatchesGradleInput', False),
''' +s[b:]
needle="    data = seed(); data['ApkVerification.json']['runtimeMetadataMarkers']['ProductionGameplayStockLayout'] = False"
insertion='''    for owner, field, value in (
            ('GameState_SpendGold', 'packagedMethodBytesMatchCompiledOutput', False),
            ('GameState_SpendEnergy', 'callsAtomicBalanceCommit', False),
            ('GameState_TryUseBooster', 'callsPersist', True),
            ('GameState_TrySettleBoosterCommand', 'generatedOverflowCatch', False),
            ('GameState_TryCompleteLevel', 'generatedOverflowCatch', False),
            ('GameState_TryCredit', 'generatedOverflowCatch', False)):
        data = seed(); data['AfterNative.json']['methods'][owner][field] = value
        (qa / 'AfterNative.json').write_text(json.dumps(data['AfterNative.json']))
        passed(owner + ' invalid ' + field + ' blocks readiness', inspect()['status'] == 'build_evidence_requires_review')
'''
assert needle in s;s=s.replace(needle,insertion+needle);s=s.replace('missing native claim evidence blocks readiness','missing native recovery evidence blocks readiness');p.write_text(s)
print('Canonical preflight pins new APK and all22native transaction owners; synthetic coverage extended.')
