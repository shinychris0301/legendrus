"""Exercise preflight boundaries with synthetic local files and mocked ADB only."""
from pathlib import Path
from contextlib import redirect_stdout
from unittest.mock import patch
import hashlib
import importlib.util
import io
import json
import subprocess
import tempfile

BASE = Path(__file__).resolve().parent.parent
spec = importlib.util.spec_from_file_location('preflight', BASE / 'Sources/Preflight.py')
tool = importlib.util.module_from_spec(spec)
spec.loader.exec_module(tool)
cases = []


def passed(name, condition):
    assert condition, name
    cases.append({'name': name, 'result': 'Passed'})


with tempfile.TemporaryDirectory(prefix='lor-offline-preflight-r2-') as temp:
    root = Path(temp)
    apk = root / tool.APK
    apk.parent.mkdir(parents=True)
    qa = root / tool.BUILD
    qa.mkdir(parents=True)
    payload = b'synthetic APK identity fixture; not a Unity player'
    pin = {'bytes': len(payload), 'sha256': hashlib.sha256(payload).hexdigest()}

    def seed():
        apk.write_bytes(payload)
        apk.with_suffix('.apk.sha256').write_text(pin['sha256'] + '  ' + apk.name + '\n')
        data = {
            'Artifact.json': {**pin, 'path': str(tool.APK)},
            'ApkVerification.json': {**pin, 'package': tool.PACKAGE, 'versionName': '0.0.0',
                'versionCode': 1, 'abi': 'arm64-v8a', 'minimumApi': 26, 'targetApi': 36,
                'recapRuntimeTypes': {key: True for key in ('ProductionStoryRecapView', 'StoryRecap', 'StoryRecapEntry')},
                'presetRuntimeTypes': {'DevelopmentGameplayFixtures': True},
                'presetSelectors': {key: True for key in tool.PRESETS},
                **{key: True for key in ('signatureVerified', 'sameCertificateAsPriorLocalApk',
                    'zipCrcPassed', 'zip16KiBAlignmentPassed', 'debuggable', 'portrait')},
                'runtimeMetadataMarkers': {key: True for key in ('DevelopmentGameplayProfile',
                    'DevelopmentGameplayProfileBadge', 'lor_gameplay_qa',
                    'LegendsOfRus_GameplayQA_v1_', 'ProductionGameplayStockLayout')}},
            'BuildValidation.json': {'result': 'Passed', 'artifactSha256': pin['sha256'], 'artifactBytes': pin['bytes'],
                'nativeRecoveryLibrarySha256': 'b' * 64, 'nativeMethodCount': 35,'atomicNarrativeAcknowledgementRecoveryPackaged': True, 'cappedAtomicEnergyAndClockRecoveryPackaged': True,
                'atomicSpendingAndDurableBoosterRefundRecoveryPackaged': True,
                'doubledGoldAndFirstClearRecoveryPackaged': True, 'ordinaryCreditRecoveryPackaged': True,
                'dailyRewardOverflowRejectionPackaged': True},
            'AfterNative.json': {'stage': 'After', 'atomicNarrativeAcknowledgementRecoveryPackaged': True, 'cappedAtomicEnergyAndClockRecoveryPackaged': True, 'validatedReceiptAndRecoveryMetadataPresent': True,
                'packagedLibraryMatchesGradleInput': True, 'packagedLibrarySha256': 'b' * 64,
                'methods': {owner: {'packagedMethodBytesMatchCompiledOutput': True,
                    'generatedOverflowCatch': owner in tool.CHECKED_CREDITS,
                    'callsAtomicBalanceCommit': owner in tool.ATOMIC_SPENDS + tool.ATOMIC_ENERGY_APIS + tool.ATOMIC_NARRATIVE_APIS, 'callsPersist': False,
                    'hasWideCapMath': owner in tool.WIDE_ENERGY_MATH,
                    'callsSharedEnergyClock': owner in tool.SHARED_ENERGY_CLOCK,
                    'callsStoryCommit':owner=='StoryService_AdvanceDialogue',
                    'rejectsBeforeClearingFinalSequence':owner=='StoryService_AdvanceDialogue',
                    'callsErrorDialog':owner in tool.NARRATIVE_ROUTES,
                    'callsChapterAcknowledgement':owner=='ScreenRouter_ContinueAfterChapterComplete',
                    'callsTryEnergyApi': owner in tool.ENERGY_WRAPPERS}
                    for owner in tool.NATIVE_METHODS}},
            'PackedAssets.json': {'result': 'Succeeded', 'gameplayRevision': 24,
                'stockLayouts': 4, 'unityVersion': '6000.5.9f1', 'profileRevision': 13, 'dialogueRevision': 2,
                'victoryRevision': 12, 'dailyRewardRevision': 5, 'receiptRevision': 2,
                'fixedPresets': [{'selector': key} for key in tool.PRESETS]}}
        for name, value in data.items():
            (qa / name).write_text(json.dumps(value))
        return data

    def inspect():
        return tool.inspect_local(root, pin)

    seed()
    local = inspect()
    passed('matching local artifact and inclusion evidence', local['status'] == 'offline_prepared_device_deferred')
    apk.unlink()
    passed('missing APK blocks readiness', inspect()['status'] == 'local_apk_missing')
    seed(); apk.write_bytes(b'changed')
    passed('changed APK blocks readiness', inspect()['status'] == 'local_apk_pin_mismatch')
    seed(); apk.with_suffix('.apk.sha256').write_text('wrong')
    passed('wrong checksum blocks readiness', inspect()['status'] == 'local_checksum_mismatch')
    seed(); apk.with_suffix('.apk.sha256').unlink()
    passed('missing checksum blocks readiness', inspect()['status'] == 'local_evidence_unavailable_or_invalid')
    seed(); (qa / 'BuildValidation.json').write_text('{')
    passed('malformed evidence blocks readiness', inspect()['status'] == 'local_evidence_unavailable_or_invalid')
    seed(); (qa / 'ApkVerification.json').write_text('[]')
    passed('wrong evidence structure blocks readiness', inspect()['status'] == 'local_evidence_unavailable_or_invalid')
    seed(); (qa / 'AfterNative.json').unlink()
    passed('missing native recovery evidence blocks readiness', inspect()['status'] == 'local_evidence_unavailable_or_invalid')
    mutations = [
        ('unverified narrative build proof','BuildValidation.json','atomicNarrativeAcknowledgementRecoveryPackaged',False),
        ('unverified narrative native proof','AfterNative.json','atomicNarrativeAcknowledgementRecoveryPackaged',False),
        ('unverified energy build proof', 'BuildValidation.json', 'cappedAtomicEnergyAndClockRecoveryPackaged', False),
        ('unverified energy native proof', 'AfterNative.json', 'cappedAtomicEnergyAndClockRecoveryPackaged', False),
        ('unverified compiled recovery', 'BuildValidation.json', 'atomicSpendingAndDurableBoosterRefundRecoveryPackaged', False),
        ('missing native owners', 'AfterNative.json', 'methods', {}),
        ('wrong native library evidence hash', 'BuildValidation.json', 'nativeRecoveryLibrarySha256', '0' * 64),
        ('missing native receipt validation', 'AfterNative.json', 'validatedReceiptAndRecoveryMetadataPresent', False),
        ('missing ordinary credit recovery', 'BuildValidation.json', 'ordinaryCreditRecoveryPackaged', False),
        ('missing doubled and first-clear recovery', 'BuildValidation.json', 'doubledGoldAndFirstClearRecoveryPackaged', False),
        ('missing daily rejection', 'BuildValidation.json', 'dailyRewardOverflowRejectionPackaged', False),
        ('wrong native method count', 'BuildValidation.json', 'nativeMethodCount', 29),
        ('stale native stage', 'AfterNative.json', 'stage', 'Before'),
        ('wrong Gradle library evidence', 'AfterNative.json', 'packagedLibraryMatchesGradleInput', False),
        ('stale daily owner', 'PackedAssets.json', 'dailyRewardRevision', 4),
        ('stale receipt owner', 'PackedAssets.json', 'receiptRevision', 1),
        ('missing fixed factory', 'ApkVerification.json', 'presetRuntimeTypes', {}),
        ('missing preset selector', 'ApkVerification.json', 'presetSelectors',
         {key: True for key in tool.PRESETS[:-1]}),
        ('false preset selector', 'ApkVerification.json', 'presetSelectors',
         {key: key != 'stock-1000' for key in tool.PRESETS}),
        ('missing packed fixture inventory', 'PackedAssets.json', 'fixedPresets', []),
        ('stale Victory revision', 'PackedAssets.json', 'victoryRevision', 11),
        ('wrong package', 'ApkVerification.json', 'package', 'different'),
        ('wrong recorded artifact hash', 'Artifact.json', 'sha256', '0' * 64),
        ('wrong recorded artifact size', 'Artifact.json', 'bytes', 1),
        ('release evidence', 'ApkVerification.json', 'debuggable', False),
        ('unsigned evidence', 'ApkVerification.json', 'signatureVerified', False),
        ('failed final reconciliation', 'BuildValidation.json', 'result', 'Failed'),
        ('stale Gameplay revision', 'PackedAssets.json', 'gameplayRevision', 23),
        ('missing stock reference', 'PackedAssets.json', 'stockLayouts', 3),
        ('failed build report', 'PackedAssets.json', 'result', 'Failed'),
        ('stale Profile revision', 'PackedAssets.json', 'profileRevision', 12),
        ('stale Dialogue revision', 'PackedAssets.json', 'dialogueRevision', 1),
        ('missing recap reader', 'ApkVerification.json', 'recapRuntimeTypes',
         {'StoryRecap': True, 'StoryRecapEntry': True}),
        ('missing recap projection', 'ApkVerification.json', 'recapRuntimeTypes',
         {'ProductionStoryRecapView': True, 'StoryRecapEntry': True})]
    for name, file, key, value in mutations:
        data = seed(); data[file][key] = value
        (qa / file).write_text(json.dumps(data[file]))
        passed(name + ' blocks readiness', inspect()['status'] == 'build_evidence_requires_review')
    for owner, field, value in (
            ('GameState_CompleteStoryScene','callsAtomicBalanceCommit',False),
            ('GameState_CompleteStoryScene','callsPersist',True),
            ('GameState_AcknowledgeChapterCompletion','callsAtomicBalanceCommit',False),
            ('StoryService_AdvanceDialogue','callsStoryCommit',False),
            ('StoryService_AdvanceDialogue','rejectsBeforeClearingFinalSequence',False),
            ('StoryService_AdvanceDialogue','packagedMethodBytesMatchCompiledOutput',False),
            ('ScreenRouter_ContinueAfterDialogue','callsErrorDialog',False),
            ('ScreenRouter_ContinueAfterChapterComplete','callsErrorDialog',False),
            ('ScreenRouter_ContinueAfterChapterComplete','callsChapterAcknowledgement',False),
            ('GameState_SpendGold', 'packagedMethodBytesMatchCompiledOutput', False),
            ('GameState_SpendEnergy', 'callsAtomicBalanceCommit', False),
            ('GameState_TryUseBooster', 'callsPersist', True),
            ('GameState_TrySettleBoosterCommand', 'generatedOverflowCatch', False),
            ('GameState_TryCompleteLevel', 'generatedOverflowCatch', False),
            ('GameState_TryCredit', 'generatedOverflowCatch', False),
            ('GameState_TryAddEnergy', 'callsAtomicBalanceCommit', False),
            ('GameState_TryRefillEnergy', 'callsPersist', True),
            ('GameState_TryRefreshEnergyFromClock', 'callsAtomicBalanceCommit', False),
            ('GameState_AdvanceEnergyClock', 'hasWideCapMath', False),
            ('GameState_TryAddEnergy', 'hasWideCapMath', False),
            ('GameState_SpendEnergy', 'callsSharedEnergyClock', False),
            ('GameState_TryRefreshEnergyFromClock', 'callsSharedEnergyClock', False),
            ('GameState_AddEnergy', 'callsTryEnergyApi', False),
            ('GameState_RefillEnergy', 'callsPersist', True),
            ('GameState_RefreshEnergyFromClock', 'callsTryEnergyApi', False),
            ('GameState_TryRefillEnergy', 'packagedMethodBytesMatchCompiledOutput', False)):
        data = seed(); data['AfterNative.json']['methods'][owner][field] = value
        (qa / 'AfterNative.json').write_text(json.dumps(data['AfterNative.json']))
        passed(owner + ' invalid ' + field + ' blocks readiness', inspect()['status'] == 'build_evidence_requires_review')
    data = seed(); data['ApkVerification.json']['runtimeMetadataMarkers']['ProductionGameplayStockLayout'] = False
    (qa / 'ApkVerification.json').write_text(json.dumps(data['ApkVerification.json']))
    passed('missing compiled stock marker blocks readiness', inspect()['status'] == 'build_evidence_requires_review')

    seed(); local = inspect()
    no_calls = []
    result = tool.inspect_device({'status': 'local_apk_pin_mismatch'}, lambda args: no_calls.append(args))
    passed('failed local pin prevents all device calls', result['status'] == 'local_apk_pin_mismatch' and not no_calls)

    def fake_run(overrides=None):
        overrides = overrides or {}
        calls = []
        defaults = {
            ('devices',): ('List of devices attached\nsynthetic-device\tdevice\n', 0),
            ('shell', 'getprop', 'ro.product.model'): ('Synthetic phone\n', 0),
            ('shell', 'getprop', 'ro.build.version.sdk'): ('36\n', 0),
            ('shell', 'pm', 'path', tool.PACKAGE): ('package:/data/app/~~example/qa-test/base.apk\n', 0),
            ('shell', 'sha256sum', '/data/app/~~example/qa-test/base.apk'): (pin['sha256'] + '  base.apk\n', 0)}

        def run(args):
            calls.append(args)
            key = tuple(args[2:] if args[:1] == ['-s'] else args)
            assert key in defaults, 'Unexpected ADB operation: ' + str(key)
            stdout, code = overrides.get(key, defaults[key])
            return subprocess.CompletedProcess(args, code, stdout, '')
        return run, calls

    scenarios = [
        ('one matching device remains QA pending', {}, 'device_identity_verified_qa_activation_pending'),
        ('no device', {('devices',): ('List of devices attached\n', 0)}, 'connect_one_authorized_device'),
        ('unauthorized device', {('devices',): ('List of devices attached\na\tunauthorized\n', 0)}, 'connect_one_authorized_device'),
        ('multiple devices', {('devices',): ('List of devices attached\na\tdevice\nb\tdevice\n', 0)}, 'connect_one_authorized_device'),
        ('mixed attachments', {('devices',): ('List of devices attached\na\tdevice\nb\tunauthorized\n', 0)}, 'connect_one_authorized_device'),
        ('ADB enumeration fails', {('devices',): ('', 1)}, 'adb_enumeration_failed'),
        ('device property fails', {('shell', 'getprop', 'ro.product.model'): ('', 1)}, 'device_property_query_failed'),
        ('old Android API', {('shell', 'getprop', 'ro.build.version.sdk'): ('25', 0)}, 'device_api_unsupported_or_unknown'),
        ('unknown Android API', {('shell', 'getprop', 'ro.build.version.sdk'): ('?', 0)}, 'device_api_unsupported_or_unknown'),
        ('package query fails', {('shell', 'pm', 'path', tool.PACKAGE): ('', 1)}, 'package_query_failed'),
        ('app absent', {('shell', 'pm', 'path', tool.PACKAGE): ('', 0)}, 'development_app_not_installed'),
        ('split package', {('shell', 'pm', 'path', tool.PACKAGE): ('package:/data/app/test/base.apk\npackage:/data/app/test/split.apk\n', 0)}, 'split_or_ambiguous_package_requires_review'),
        ('unsafe package path', {('shell', 'pm', 'path', tool.PACKAGE): ('package:/data/app/test/base.apk;unexpected\n', 0)}, 'unexpected_package_path'),
        ('installed hash unavailable', {('shell', 'sha256sum', '/data/app/~~example/qa-test/base.apk'): ('bad', 0)}, 'installed_apk_hash_unavailable'),
        ('installed hash mismatch', {('shell', 'sha256sum', '/data/app/~~example/qa-test/base.apk'): ('0' * 64, 0)}, 'installed_build_requires_review')]
    for name, overrides, status in scenarios:
        run, calls = fake_run(overrides)
        result = tool.inspect_device(local, run)
        passed(name, result['status'] == status and 'synthetic-device' not in json.dumps(result))
        if status == 'connect_one_authorized_device':
            assert calls == [['devices']]

    for exception in (FileNotFoundError('synthetic'), subprocess.TimeoutExpired('synthetic', 30)):
        def fail(args):
            raise exception
        passed(type(exception).__name__ + ' is reported',
               tool.inspect_device(local, fail)['status'] == 'adb_unavailable_or_timed_out')

    with patch.object(tool, 'inspect_local', return_value=local), \
         patch.object(tool, 'inspect_device', side_effect=AssertionError('Default mode reached device queries')), \
         patch.object(tool.subprocess, 'run', side_effect=AssertionError('Default mode reached subprocess')), \
         redirect_stdout(io.StringIO()):
        output = root / 'offline-result.json'
        code = tool.main(['--output', str(output)])
        record = json.loads(output.read_text())
    passed('default CLI never enumerates, launches or reads app data',
           code == 0 and record['deviceExecutionDeferred'] and not record['deviceEnumerationAttempted']
           and not record['physicalGameplayTested'] and not record['appLaunched']
           and not record['appDataReadOrChanged'] and not record['qaActivationVerified'])

summary = {'result': 'Passed', 'passed': len(cases), 'failed': 0, 'cases': cases,
           'realAdbInvocations': 0, 'realDeviceExecution': False, 'syntheticFilesOnly': True}
(tool.ROOT / tool.BUILD / 'PreflightBoundaryChecks.json').write_text(json.dumps(summary, indent=2) + '\n')
print(json.dumps({key: value for key, value in summary.items() if key != 'cases'}, indent=2))
