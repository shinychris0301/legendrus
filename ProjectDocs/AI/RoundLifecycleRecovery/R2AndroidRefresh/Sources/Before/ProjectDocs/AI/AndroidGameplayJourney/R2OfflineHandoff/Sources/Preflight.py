"""Pinned APK readiness; offline by default, optional read-only device identity checks."""
from pathlib import Path
import argparse
import datetime
import hashlib
import json
import re
import subprocess

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
BUILD = Path('ProjectDocs/AI/SelectionRecovery/R2AndroidRefresh/QA')
APK = Path('Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk')
PIN = {'bytes': 766023250,
       'sha256': '0c7c454dc83154e622fff646ac865a2c03fee5b5852ab5f46be24703bf48fb7a'}
PRESETS = ('stock-999', 'stock-1000', 'stock-9999', 'stock-max', 'story-early', 'story-finale')
NATIVE_METHODS = ('GameState_RefreshEnergyFromClock', 'GameState_TryRefreshEnergyFromClock', 'GameState_AdvanceEnergyClock', 'GameState_EnergyRemaining', 'GameState_SpendEnergy', 'GameState_AddEnergy', 'GameState_TryAddEnergy', 'GameState_RefillEnergy', 'GameState_TryRefillEnergy', 'GameState_TryAddGold', 'GameState_TryCredit', 'GameState_TryCommitBalance', 'GameState_SpendGold', 'GameState_TryAddGems', 'GameState_SpendGems', 'GameState_CompleteLevel', 'GameState_TryCompleteLevel', 'GameState_TryDoubleLastLevelGoldReward', 'GameState_TryUseBooster', 'GameState_TryBeginBoosterCommand', 'GameState_TrySettleBoosterCommand', 'GameState_TryRecoverPendingBoosterCommand', 'GameState_TryAddBooster', 'GameState_ClaimDailyReward', 'Match3Board_ConsumeCommandBooster', 'Match3Board_RefundCommand', 'Match3Board_RecoverActiveCommand', 'Match3Board_TryResume', 'ScreenRouter_StartLevel', 'ScreenRouter_CommitGameplayVictory','GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion','StoryService_AdvanceDialogue','ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete','GameState_CompleteTutorial','LevelTutorialPresenter_Advance','LevelTutorialPresenter_BuildOverlay','ScreenRouter_ShowGameplayTutorialError','ScreenRouter_BuildGameplayOverlay','ScreenRouter_NavigateBack','ScreenRouter_RetireModalRootImmediately','GameState_SelectLevel','GameState_EnsureSelectedLevelIsUnlocked','GameState_TryEnsureSelectedLevelIsUnlocked','GameState_NormalizeLevelProgress','ScreenRouter_OpenSelectedLevel','ProductionChapterMapView_BindNavigation','ScreenRouter_BuildInteractionLayer')
ATOMIC_SPENDS = ('GameState_SpendGold', 'GameState_SpendGems', 'GameState_SpendEnergy', 'GameState_TryUseBooster')
CHECKED_CREDITS = ('GameState_ClaimDailyReward', 'GameState_TryDoubleLastLevelGoldReward', 'GameState_TryCompleteLevel', 'GameState_TryCredit', 'GameState_TrySettleBoosterCommand')
ATOMIC_ENERGY_APIS = ('GameState_TryAddEnergy', 'GameState_TryRefillEnergy', 'GameState_TryRefreshEnergyFromClock')
WIDE_ENERGY_MATH = ('GameState_TryAddEnergy', 'GameState_AdvanceEnergyClock')
SHARED_ENERGY_CLOCK = ('GameState_SpendEnergy', 'GameState_TryRefreshEnergyFromClock')
ENERGY_WRAPPERS = ('GameState_AddEnergy', 'GameState_RefillEnergy', 'GameState_RefreshEnergyFromClock')
ATOMIC_NARRATIVE_APIS = ('GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion')
NARRATIVE_ROUTES = ('ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete')
ATOMIC_TUTORIAL_APIS = ('GameState_CompleteTutorial',)
TUTORIAL_FLAGS = {'LevelTutorialPresenter_Advance': ('rejectsBeforeClearingTutorialStep', 'guardsInactiveTutorialPresenter'), 'LevelTutorialPresenter_BuildOverlay': ('capturesPresentedOverlayAndStep', 'ownedContinueCallbackCompiledAndPackaged'), 'ScreenRouter_ShowGameplayTutorialError': ('scopedTutorialErrorOwner',), 'ScreenRouter_BuildGameplayOverlay': ('registersAcknowledgementRejected', 'ownedErrorCallbackCompiledAndPackaged'), 'ScreenRouter_NavigateBack': ('tutorialBackOwnerPriority',), 'ScreenRouter_RetireModalRootImmediately': ('clearsTutorialErrorOwner',)}
ATOMIC_SELECTION_APIS = ('GameState_SelectLevel','GameState_TryEnsureSelectedLevelIsUnlocked')
SELECTION_FLAGS = {'GameState_SelectLevel': ('respectsSelectionWriteLock',), 'GameState_TryEnsureSelectedLevelIsUnlocked': ('respectsSelectionWriteLock', 'normalizesCandidateBeforeCommit'), 'GameState_EnsureSelectedLevelIsUnlocked': ('delegatesToAcknowledgedNormalization',), 'GameState_NormalizeLevelProgress': ('normalizesExplicitCandidate',), 'ScreenRouter_StartLevel': ('normalizationCheckedBeforeEnergySpend',), 'ScreenRouter_OpenSelectedLevel': ('normalizationCheckedBeforeStoryRouting',), 'ProductionChapterMapView_BindNavigation': ('acknowledgedMapCallbacksCompiledAndPackaged',), 'ScreenRouter_BuildInteractionLayer': ('bindsAcknowledgedMapOpenAndError',)}
PACKAGE = 'com.example.legendsofrus.v117debug'
ACTIVITY = 'com.unity3d.player.UnityPlayerGameActivity'
ADB = Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb')


def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def inspect_local(root=ROOT, pin=PIN):
    result = {'localApkExists': (root / APK).is_file(), 'package': PACKAGE,
              'launchActivity': ACTIVITY, 'buildEvidence': str(BUILD),
              'expectedApkSha256': pin['sha256'], 'expectedApkBytes': pin['bytes'],
              'gameplayRevision': 24, 'stockLayouts': 4, 'victoryRevision': 12,
              'profileRevision': 13, 'dialogueRevision': 2, 'expectedRecapIncluded': True,
              'signatureRecheckedThisRun': False, 'compiledMarkersAreNotDeviceAcceptance': True}
    try:
        if not result['localApkExists']:
            result['status'] = 'local_apk_missing'
            return result
        apk = root / APK
        result.update(localApkBytes=apk.stat().st_size, localApkSha256=sha(apk))
        if result['localApkBytes'] != pin['bytes'] or result['localApkSha256'] != pin['sha256']:
            result['status'] = 'local_apk_pin_mismatch'
            return result
        if apk.with_suffix('.apk.sha256').read_text().strip() != pin['sha256'] + '  ' + apk.name:
            result['status'] = 'local_checksum_mismatch'
            return result
        read = lambda name: json.loads((root / BUILD / name).read_text())
        artifact, verification, final, packed, native = [read(name) for name in
            ('Artifact.json', 'ApkVerification.json', 'BuildValidation.json', 'PackedAssets.json', 'AfterNative.json')]
        required = ('signatureVerified', 'sameCertificateAsPriorLocalApk', 'zipCrcPassed',
                    'zip16KiBAlignmentPassed', 'debuggable', 'portrait')
        markers = ('DevelopmentGameplayProfile', 'DevelopmentGameplayProfileBadge',
                   'lor_gameplay_qa', 'LegendsOfRus_GameplayQA_v1_', 'ProductionGameplayStockLayout')
        matches = (
            artifact.get('path') == str(APK)
            and all(record.get(key) == value for record in (artifact, verification) for key, value in pin.items())
            and verification.get('package') == PACKAGE
            and verification.get('versionName') == '0.0.0' and verification.get('versionCode') == 1
            and verification.get('abi') == 'arm64-v8a'
            and verification.get('minimumApi') == 26 and verification.get('targetApi') == 36
            and all(verification.get(key) is True for key in required)
            and all(verification.get('runtimeMetadataMarkers', {}).get(key) is True for key in markers)
            and final.get('result') == 'Passed' and final.get('artifactSha256') == pin['sha256']
            and final.get('artifactBytes') == pin['bytes']
            and packed.get('result') == 'Succeeded' and packed.get('gameplayRevision') == 24
            and packed.get('stockLayouts') == 4 and packed.get('unityVersion') == '6000.5.9f1'
            and packed.get('profileRevision') == 13 and packed.get('dialogueRevision') == 2
            and packed.get('victoryRevision') == 12
            and packed.get('dailyRewardRevision') == 5 and packed.get('receiptRevision') == 2
            and final.get('atomicSpendingAndDurableBoosterRefundRecoveryPackaged') is True
            and final.get('doubledGoldAndFirstClearRecoveryPackaged') is True
            and final.get('ordinaryCreditRecoveryPackaged') is True
            and final.get('dailyRewardOverflowRejectionPackaged') is True
            and final.get('nativeMethodCount') == 49
            and final.get('atomicSelectionNormalizationRecoveryPackaged') is True
            and native.get('atomicSelectionNormalizationRecoveryPackaged') is True
            and final.get('atomicTutorialAcknowledgementRecoveryPackaged') is True
            and native.get('atomicTutorialAcknowledgementRecoveryPackaged') is True
            and final.get('atomicNarrativeAcknowledgementRecoveryPackaged') is True
            and native.get('atomicNarrativeAcknowledgementRecoveryPackaged') is True
            and final.get('cappedAtomicEnergyAndClockRecoveryPackaged') is True
            and native.get('cappedAtomicEnergyAndClockRecoveryPackaged') is True
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
            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False
                    for owner in ATOMIC_ENERGY_APIS)
            and all(native.get('methods', {}).get(owner, {}).get('hasWideCapMath') is True
                    for owner in WIDE_ENERGY_MATH)
            and all(native.get('methods', {}).get(owner, {}).get('callsSharedEnergyClock') is True
                    for owner in SHARED_ENERGY_CLOCK)
            and all(native.get('methods', {}).get(owner, {}).get('callsTryEnergyApi') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False
                    for owner in ENERGY_WRAPPERS)
            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False
                    for owner in ATOMIC_NARRATIVE_APIS)
            and native.get('methods', {}).get('StoryService_AdvanceDialogue', {}).get('callsStoryCommit') is True
            and native.get('methods', {}).get('StoryService_AdvanceDialogue', {}).get('rejectsBeforeClearingFinalSequence') is True
            and all(native.get('methods', {}).get(owner, {}).get('callsErrorDialog') is True for owner in NARRATIVE_ROUTES)
            and native.get('methods', {}).get('ScreenRouter_ContinueAfterChapterComplete', {}).get('callsChapterAcknowledgement') is True
            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False for owner in ATOMIC_TUTORIAL_APIS)
            and all(native.get('methods', {}).get(owner, {}).get(flag) is True for owner,flags in TUTORIAL_FLAGS.items() for flag in flags)
            and set(native.get('tutorialCallbacks', {})) == {'OwnedTutorialContinue','OwnedTutorialError'}
            and all(row.get('packagedMethodBytesMatchCompiledOutput') is True for row in native.get('tutorialCallbacks', {}).values())
            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False for owner in ATOMIC_SELECTION_APIS)
            and all(native.get('methods', {}).get(owner, {}).get(flag) is True for owner,flags in SELECTION_FLAGS.items() for flag in flags)
            and set(native.get('selectionCallbacks', {})) == {'AcknowledgedMapPlay','AcknowledgedMapNode','SelectionError'}
            and all(row.get('packagedMethodBytesMatchCompiledOutput') is True for row in native.get('selectionCallbacks', {}).values())
            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')
            and isinstance(final.get('nativeRecoveryLibrarySha256'), str)
            and re.fullmatch('[a-f0-9]{64}', final.get('nativeRecoveryLibrarySha256')) is not None
            and verification.get('presetRuntimeTypes', {}).get('DevelopmentGameplayFixtures') is True
            and all(verification.get('presetSelectors', {}).get(key) is True for key in PRESETS)
            and [row.get('selector') for row in packed.get('fixedPresets', [])] == list(PRESETS)
            and all(verification.get('recapRuntimeTypes', {}).get(key) is True
                    for key in ('ProductionStoryRecapView', 'StoryRecap', 'StoryRecapEntry'))
        )
        result.update(matchesVerifiedRecapApk=bool(matches), recapPackaged=bool(matches), checksumSidecarMatches=True,
                      matchesVerifiedPresetApk=bool(matches), fixedPresetsPackaged=bool(matches),
                      dailyRewardOverflowCatchPackaged=bool(matches),
                      atomicSpendingAndBoosterRefundRecoveryPackaged=bool(matches),
                      accumulatedRewardAndCreditRecoveryPackaged=bool(matches),
                      atomicSelectionNormalizationRecoveryPackaged=bool(matches),
                      atomicTutorialAcknowledgementRecoveryPackaged=bool(matches),
                      atomicNarrativeAcknowledgementRecoveryPackaged=bool(matches),
                      cappedAtomicEnergyAndClockRecoveryPackaged=bool(matches),
                      recordedSignatureAndCompiledInclusionVerified=bool(matches))
        result['status'] = 'offline_prepared_device_deferred' if matches else 'build_evidence_requires_review'
    except (OSError, ValueError, TypeError, KeyError, AttributeError):
        result['status'] = 'local_evidence_unavailable_or_invalid'
    return result


def run_adb(args):
    return subprocess.run([str(ADB), *args], capture_output=True, text=True, timeout=30)


def inspect_device(local, run=run_adb):
    result = dict(local)
    result.update(deviceEnumerationAttempted=False, deviceExecutionDeferred=False)
    if local.get('status') != 'offline_prepared_device_deferred':
        return result
    try:
        result['deviceEnumerationAttempted'] = True
        listing = run(['devices'])
        if listing.returncode:
            result['status'] = 'adb_enumeration_failed'
            return result
        rows = [line.split() for line in listing.stdout.splitlines()[1:] if line.strip()]
        devices = [row[0] for row in rows if len(row) == 2 and row[1] == 'device']
        result.update(attachedCount=len(rows), authorizedCount=len(devices))
        if len(rows) != 1 or len(devices) != 1:
            result['status'] = 'connect_one_authorized_device'
            return result
        # Device identifiers are used only for selection, never written to the report.
        def device(*args):
            return run(['-s', devices[0], *args])
        for prop, key in [('ro.product.model', 'deviceModel'), ('ro.build.version.sdk', 'androidApi')]:
            response = device('shell', 'getprop', prop)
            if response.returncode:
                result['status'] = 'device_property_query_failed'
                return result
            result[key] = response.stdout.strip()
        api = result['androidApi']
        if not api.isdecimal() or int(api) < 26:
            result['status'] = 'device_api_unsupported_or_unknown'
            return result
        response = device('shell', 'pm', 'path', PACKAGE)
        if response.returncode:
            result['status'] = 'package_query_failed'
            return result
        paths = [line.removeprefix('package:') for line in response.stdout.splitlines()
                 if line.startswith('package:')]
        if not paths:
            result['status'] = 'development_app_not_installed'
            return result
        if len(paths) != 1:
            result['status'] = 'split_or_ambiguous_package_requires_review'
            return result
        base = paths[0]
        if not re.fullmatch(r'/data/app/[A-Za-z0-9_/=+~.\-]+/base\.apk', base):
            result['status'] = 'unexpected_package_path'
            return result
        hashed = device('shell', 'sha256sum', base)
        fields = hashed.stdout.split()
        digest = fields[0] if hashed.returncode == 0 and fields else ''
        if not re.fullmatch('[a-f0-9]{64}', digest):
            result['status'] = 'installed_apk_hash_unavailable'
            return result
        result['installedApkMatchesLocal'] = digest == local['localApkSha256']
        result['status'] = ('device_identity_verified_qa_activation_pending'
                            if result['installedApkMatchesLocal'] else 'installed_build_requires_review')
    except (OSError, subprocess.TimeoutExpired):
        result['status'] = 'adb_unavailable_or_timed_out'
    return result


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--device', action='store_true', help='Opt in to read-only ADB identity queries.')
    parser.add_argument('--output', type=Path, help='JSON evidence destination; default is this milestone QA folder.')
    args = parser.parse_args(argv)
    result = inspect_local()
    result.update(physicalGameplayTested=False, appLaunched=False, appDataReadOrChanged=False,
                  deviceEnumerationAttempted=False, deviceExecutionDeferred=not args.device,
                  qaActivationVerified=False, checkedAtUtc=datetime.datetime.now(datetime.timezone.utc).isoformat())
    if args.device:
        result = inspect_device(result)
    output = args.output or ROOT / BUILD / ('DevicePreflight.json' if args.device else 'OfflinePreflight.json')
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result, indent=2))
    return 0 if result['status'] in ('offline_prepared_device_deferred',
                                    'device_identity_verified_qa_activation_pending') else 2


if __name__ == '__main__':
    raise SystemExit(main())
