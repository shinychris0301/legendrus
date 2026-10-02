"""Reproduce the user's previously authorized alpha-only cleanup of two R8 gems.

The generator painted a neutral checkerboard into RGB. Remove only neutral
background connected to the canvas edges. Retain every RGB channel unchanged,
including transparent pixels, and trim to the silhouette plus four clear pixels.
No resize, recolor, sharpen, screenshot crop, or replacement artwork is involved.
Run with the project's existing Pillow/numpy/scipy diagnostic Python environment.
"""
from pathlib import Path
import hashlib
import json
import shutil

import numpy as np
from PIL import Image
from scipy import ndimage

ROOT = Path(__file__).resolve().parents[5]
DOCS = ROOT / "ProjectDocs/AI/GameplayStyleAlignment/SourcePaletteR8"
ART = ROOT / "Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned"
SOURCES = DOCS / "Sources"
QA = DOCS / "QA"
GENERATED = Path("/home/k2/.codex/generated_images/01a09624-160e-7f10-a32e-7cc91172628e")
FILES = {
    "gem_emerald_r8": "exec-d5c73bfc-99c1-4928-8167-e4bea5043927.png",
    "gem_amethyst_r8": "exec-86b11001-43bd-4d93-a84c-105ac3ddc0d6.png",
    
}


def sha256(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def prepare(name, filename):
    raw = GENERATED / filename
    source = SOURCES / (name + ".png")
    if not source.exists():
        shutil.copy2(raw, source)
    if raw.exists():
        assert sha256(source) == sha256(raw), "Source copy must be unchanged"

    original = np.array(Image.open(source).convert("RGBA"))
    rgb = original[:, :, :3].astype(np.int16)
    # Dark, chromatic jewel rims enclose pale interior glints. The connectivity
    # restriction keeps interior near-white facets intact.
    eligible = ((rgb.max(2) - rgb.min(2)) < 45) & (rgb.min(2) > 20)
    edge = np.zeros(eligible.shape, dtype=bool)
    edge[0, :] = edge[-1, :] = True
    edge[:, 0] = edge[:, -1] = True
    background = ndimage.binary_propagation(edge & eligible, mask=eligible)
    result = original.copy()
    result[:, :, 3] = np.where(background, 0, 255)
    assert np.array_equal(result[:, :, :3], original[:, :, :3])

    alpha = result[:, :, 3]
    bounds = Image.fromarray(alpha).getbbox()
    assert bounds is not None
    padding = 4
    x0 = max(0, bounds[0] - padding)
    y0 = max(0, bounds[1] - padding)
    x1 = min(result.shape[1], bounds[2] + padding)
    y1 = min(result.shape[0], bounds[3] + padding)
    final = Image.fromarray(result[y0:y1, x0:x1])
    target = ART / (name + ".png")
    final.save(target)
    final_array = np.array(Image.open(target).convert("RGBA"))
    assert np.array_equal(final_array[:, :, :3], original[y0:y1, x0:x1, :3])
    assert not np.any(final_array[[0, -1], :, 3])
    assert not np.any(final_array[:, [0, -1], 3])
    labels, count = ndimage.label(alpha > 0)
    sizes = np.bincount(labels.ravel())[1:]

    proofs = []
    for bg_name, color in [("black", (0, 0, 0)), ("white", (255, 255, 255)),
                           ("magenta", (255, 0, 255))]:
        proof = Image.new("RGBA", final.size, color + (255,))
        proof.alpha_composite(final)
        path = QA / (name + "_" + bg_name + ".png")
        proof.convert("RGB").save(path)
        proofs.append(str(path.relative_to(ROOT)))

    return {
        "name": name,
        "generatedSource": str(raw),
        "sourceCopy": str(source.relative_to(ROOT)),
        "sourceSHA256": sha256(source),
        "sourceDimensions": [int(original.shape[1]), int(original.shape[0])],
        "runtimePath": str(target.relative_to(ROOT)),
        "runtimeSHA256": sha256(target),
        "runtimeDimensions": list(final.size),
        "alphaCleanup": "Previously user-authorized alpha-only cleanup. Four-connected edge flood of near-neutral RGB pixels (channel range <45, minimum >20), background alpha 0, enclosed artwork alpha 255. Retained RGB unchanged. No resampling.",
        "transparentMarginTrim": [x0, y0, x1, y1],
        "transparentPaddingPixels": padding,
        "changedRGBPixels": 0,
        "changedAlphaPixels": int(np.count_nonzero(result[:, :, 3] != original[:, :, 3])),
        "retainedArtworkPixels": int(np.count_nonzero(alpha)),
        "foregroundConnectedComponents": int(count),
        "foregroundComponentSizes": sorted([int(x) for x in sizes], reverse=True),
        "boundaryAlphaZero": True,
        "proofs": proofs,
    }


if __name__ == "__main__":
    SOURCES.mkdir(parents=True, exist_ok=True)
    QA.mkdir(parents=True, exist_ok=True)
    records = [prepare(name, filename) for name, filename in FILES.items()]
    (QA / "MaskedGemPreparation.json").write_text(json.dumps(records, indent=2) + "\n")
    print(json.dumps(records, indent=2))
