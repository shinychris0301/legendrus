#!/usr/bin/env python3
"""Static source/archive checks only. Does NOT compile C# or run Unity.

Usage:
  python Tools/verify_splash_r_integration_fix01.py --baseline /path/to/original.zip
The optional baseline verifies that all existing artwork, metadata, prefabs,
scenes, package files, and project settings remain byte-for-byte unchanged.
"""
from __future__ import annotations
import argparse
import hashlib
import json
import re
import zipfile
from pathlib import Path

ROUTER = 'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs'
SELFTEST = 'Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleProductionSelfTest.cs'
RUNTIME = 'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSplashTitleRuntime.cs'
OUT = 'ProjectDocs/SplashTitle_R_IntegrationFix01'


def sha(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--project', type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument('--baseline', type=Path)
    args = parser.parse_args()
    root = args.project.resolve()
    router = (root / ROUTER).read_text(encoding='utf-8')
    test = (root / SELFTEST).read_text(encoding='utf-8')
    runtime = (root / RUNTIME).read_text(encoding='utf-8')
    dispatch = router.split('bool BuildDesignLayer(', 1)[1].split('void BuildInteractionLayer(', 1)[0]
    splash = router.split('case ScreenId.SplashTitle:', 1)[1].split('case ScreenId.EventDetail:', 1)[0]
    tests: dict[str, bool] = {}
    tests['production_dispatch_core_before_meta_before_legacy'] = (
        dispatch.index('ProductionCoreLoopPageRuntime.TryBuild') <
        dispatch.index('ProductionMetaPageRuntime.TryBuild') <
        dispatch.index('DesignScreenRegistry.ResourceKey'))
    tests['all_five_splash_actions_guarded'] = splash.count('GuardSplashAction(') == 5
    for dest in ('Loading', 'LanguageSelect', 'Settings', 'Login', 'TutorialOverlay'):
        tests['splash_destination_' + dest] = ('ScreenId.' + dest) in splash
    tests['guard_checks_screen_and_generation'] = (
        'current != ScreenId.SplashTitle' in router and
        'routeGeneration != expectedGeneration' in router)
    tests['guard_checks_active_root_and_modal'] = (
        '!contentRoot.activeInHierarchy' in router and 'modalRoot != null' in router.split('Action GuardSplashAction', 1)[1].split('void EnsureEventSystem', 1)[0])
    tests['auxiliary_override_cleared_on_any_departure'] = (
        'splashAuxiliaryRoot.HasValue && id != splashAuxiliaryRoot.Value' in router)
    tests['source_bound_back_respects_splash'] = (
        'splashAuxiliaryRoot.HasValue && expectedSource == splashAuxiliaryRoot.Value' in router)
    tests['existing_loading_completion_guard_retained'] = (
        'generation == routeGeneration && current == ScreenId.Loading' in router)
    tests['no_new_splash_canvas_or_event_system'] = (
        router.count('void BuildCanvas()') == 1 and router.count('void EnsureEventSystem()') == 1)
    tests['readiness_validator_no_longer_forbids_live_text'] = (
        'instead of approximate live-font recreation' not in test and
        'runtime.Contains("AddText(")' not in test)
    tests['readiness_validator_no_longer_accepts_pixel_metric_as_proof'] = (
        '0.998346' not in test and 'production acceptance is BLOCKED' in test)
    tests['no_runtime_reference_path_introduced'] = (
        '/Art/ReferenceOnly/' not in runtime and 'ApprovedScreens' not in runtime)

    baseline = None
    if args.baseline:
        protected, changed, missing, preserved_meta = [], [], [], 0
        with zipfile.ZipFile(args.baseline) as z:
            for info in z.infolist():
                if info.is_dir():
                    continue
                target = root / info.filename
                if not target.is_file():
                    missing.append(info.filename)
                    continue
                old = z.read(info)
                same = sha(old) == sha(target.read_bytes())
                if not same:
                    changed.append(info.filename)
                if info.filename.endswith('.meta') and same:
                    preserved_meta += 1
                suffix = Path(info.filename).suffix.lower()
                is_protected = (
                    suffix in ('.png', '.jpg', '.jpeg', '.webp', '.ttf', '.otf', '.meta', '.prefab', '.unity') or
                    info.filename.startswith(('Packages/', 'ProjectSettings/')))
                if is_protected and not same:
                    protected.append(info.filename)
        tests['only_two_expected_existing_source_files_changed'] = set(changed) == {ROUTER, SELFTEST}
        tests['all_baseline_files_preserved'] = not missing
        tests['all_art_metadata_prefabs_scenes_packages_settings_unchanged'] = not protected
        baseline = {
            'archive': args.baseline.name,
            'existing_files_changed': changed,
            'missing_files': missing,
            'protected_files_changed': protected,
            'existing_meta_files_preserved': preserved_meta,
        }
    r2_names = ('start_button_base.png', 'language_selector_base.png', 'tagline_divider.png', 'Step20SplashTitleR3PrefabRebuilder.cs')
    discovered = {name: [str(p.relative_to(root)) for p in root.rglob(name)] for name in r2_names}
    result = {
        'scope': 'SOURCE-ONLY navigation patch; NOT completed R2 visual implementation',
        'configured_unity_version': (root / 'ProjectSettings/ProjectVersion.txt').read_text().strip(),
        'static_assertions': tests,
        'static_assertion_count': len(tests),
        'static_assertions_passed': sum(tests.values()),
        'baseline': baseline,
        'previously_described_r2_r3_names_found': discovered,
        'unity_compilation': 'NOT_RUN: Unity is not installed in the inspection workspace',
        'unity_runtime_navigation_tests': 'NOT_RUN',
        'unity_responsive_layout_tests': 'NOT_RUN',
        'unity_runtime_screenshots': 'NOT_CAPTURED',
        'visual_acceptance': 'BLOCKED: supplied archive contains legacy 01-R cutouts and raster labels, not the requested R2 resources',
    }
    dest = root / OUT / 'source_verification.json'
    dest.parent.mkdir(parents=True, exist_ok=True)
    dest.write_text(json.dumps(result, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(result, ensure_ascii=False, indent=2))
    return 0 if all(tests.values()) else 1


if __name__ == '__main__':
    raise SystemExit(main())
