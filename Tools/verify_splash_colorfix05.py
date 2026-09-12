#!/usr/bin/env python3
"""Verify the shipped ColorFix05 foreground textures. No Unity API is invoked.

Default: verify with Python's standard library only.
--rebuild: explicitly reconstruct the nine PNGs from the preserved ORIGINAL
artwork and both fixed grades. Requires numpy and Pillow. Repeating this operation
does not darken already-graded files again. It never changes the background,
.meta files, the prefab, fonts, packages, or Android build settings.
"""
from __future__ import annotations

import argparse
import hashlib
import json
import re
import sys
from pathlib import Path

DOCS = Path('ProjectDocs/SplashTitle_R3_ColorFix05')
PREFAB = Path('Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab')


def digest(path: Path) -> str:
    hasher = hashlib.sha256()
    with path.open('rb') as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b''):
            hasher.update(chunk)
    return hasher.hexdigest()


def safe_path(root: Path, relative: str) -> Path:
    path = (root / relative).resolve()
    if not path.is_relative_to(root):
        raise ValueError('Manifest path escapes the project: ' + relative)
    return path


def rebuild(root: Path, manifest: dict) -> None:
    try:
        import numpy as np
        from PIL import Image
    except ImportError as exc:
        raise RuntimeError('--rebuild requires numpy and Pillow; verification does not.') from exc

    weights = np.array([.2126, .7152, .0722], dtype=np.float32)

    def grade(a, ev: float, sat: float):
        c = a[..., :3].astype(np.float32) / 255
        lin = np.where(c <= .04045, c / 12.92, ((c + .055) / 1.055) ** 2.4)
        y = (lin * weights).sum(axis=-1, keepdims=True)
        q = (y + (lin - y) * sat) * (2.0 ** ev)
        encoded = np.where(q <= .0031308, q * 12.92,
                           1.055 * np.maximum(q, 0) ** (1 / 2.4) - .055)
        output = a.copy()
        output[..., :3] = np.rint(np.clip(encoded, 0, 1) * 255).astype(np.uint8)
        output[a[..., 3] == 0, :3] = a[a[..., 3] == 0, :3]
        return output

    # Validate all inputs before changing any current texture.
    prepared = []
    for spec in manifest['profiles']:
        source = safe_path(root, spec['original_source'])
        if digest(source) != spec['original_sha256']:
            raise ValueError('Original source changed; rebuild refused: ' + spec['original_source'])
        with Image.open(source) as im:
            original = np.array(im.convert('RGBA'))
        stage = spec['fixed_first_stage']
        fixed02 = grade(original, stage['exposure_ev'], stage['saturation_multiplier'])
        if hashlib.sha256(fixed02.tobytes()).hexdigest() != spec['baseline_pixel_sha256']:
            raise ValueError('FidelityFix02 pixel reconstruction differs: ' + spec['file'])
        stage = spec['additional_grade']
        output = grade(fixed02, stage['exposure_ev'], stage['saturation_multiplier'])
        if hashlib.sha256(output.tobytes()).hexdigest() != spec['pixel_sha256']:
            raise ValueError('ColorFix05 pixel reconstruction differs: ' + spec['file'])
        if not np.array_equal(output[..., 3], original[..., 3]):
            raise ValueError('Unexpected alpha modification: ' + spec['file'])
        prepared.append((safe_path(root, spec['file']), output))
    for target, output in prepared:
        temporary = target.with_suffix('.colorfix05.tmp')
        try:
            Image.fromarray(output).save(temporary, format='PNG', compress_level=6)
            temporary.replace(target)
        finally:
            temporary.unlink(missing_ok=True)
    print('Reconstructed nine foreground textures from fixed original sources.')
    print('PNG compression may vary with Pillow version; pixel hashes are authoritative for rebuilt files.')


def verify(root: Path, manifest: dict, allow_pixel_equivalent: bool) -> dict:
    failures: list[str] = []
    notes: list[str] = []
    prefab_path = root / PREFAB
    if not prefab_path.is_file():
        failures.append('Canonical Splash prefab is missing.')
        prefab = ''
    else:
        prefab = prefab_path.read_text(encoding='utf-8')
    for spec in manifest['profiles']:
        path = safe_path(root, spec['file'])
        if not path.is_file():
            failures.append('Missing texture: ' + spec['file'])
            continue
        if digest(path) != spec['sha256']:
            equivalent = False
            if allow_pixel_equivalent:
                try:
                    from PIL import Image
                    with Image.open(path) as im:
                        equivalent = (im.mode == 'RGBA' and im.size == (spec['width'], spec['height'])
                                      and hashlib.sha256(im.tobytes()).hexdigest() == spec['pixel_sha256'])
                except (OSError, ImportError):
                    pass
            if not equivalent:
                failures.append('Texture hash differs from ColorFix05: ' + spec['file'])
            else:
                notes.append('Equivalent decoded pixels; different PNG compression: ' + spec['file'])
        meta = Path(str(path) + '.meta')
        if not meta.is_file():
            failures.append('Existing sprite metadata is missing: ' + spec['file'])
            continue
        match = re.search(r'^guid:\s*(\w+)', meta.read_text(encoding='utf-8'), re.M)
        if not match or not re.search(r'm_Sprite:.*guid:\s*' + match.group(1), prefab):
            failures.append('Canonical prefab does not reference this sprite GUID: ' + spec['file'])
        if digest(meta) != spec['meta_sha256']:
            notes.append('Metadata differs from shipped baseline (may be a local Unity reserialization): ' + spec['file'])
    background = manifest['background']
    path = safe_path(root, background['file'])
    if not path.is_file() or digest(path) != background['sha256']:
        failures.append('Background differs from the preserved FidelityFix02 environment.')
    return dict(revision=manifest['revision'], passed=not failures,
                checked_foreground_textures=len(manifest['profiles']),
                background_unchanged=not any('Background differs' in x for x in failures),
                failures=failures, notes=notes, unity_executed=False,
                visual_match_verified=False,
                scope='File contents and prefab sprite references only; not Play Mode, build, or color-accuracy approval.')


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--project', type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument('--rebuild', action='store_true', help='Explicitly replace ONLY the nine foreground PNGs.')
    parser.add_argument('--json', type=Path, help='Optional path for a machine-readable verification report.')
    args = parser.parse_args()
    try:
        root = args.project.resolve()
        manifest = json.loads((root / DOCS / 'ColorCalibration.json').read_text(encoding='utf-8'))
        if args.rebuild:
            rebuild(root, manifest)
        result = verify(root, manifest, allow_pixel_equivalent=args.rebuild)
        output = json.dumps(result, ensure_ascii=False, indent=2)
        print(output)
        if args.json:
            args.json.parent.mkdir(parents=True, exist_ok=True)
            args.json.write_text(output + '\n', encoding='utf-8')
        return 0 if result['passed'] else 1
    except (OSError, ValueError, KeyError, RuntimeError) as exc:
        print('ColorFix05: ' + str(exc), file=sys.stderr)
        return 2


if __name__ == '__main__':
    raise SystemExit(main())
