#!/usr/bin/env python3
"""Rebuild FidelityFix02 PNGs from preserved R3 originals, not from already-graded PNGs.
Requires Pillow and numpy. Does not rebuild Unity prefabs or approve visual fidelity.
"""
from pathlib import Path
import argparse
import hashlib
import json
import numpy as np
from PIL import Image

def decode(x):
    return np.where(x <= .04045, x / 12.92, ((x + .055) / 1.055) ** 2.4)

def encode(x):
    return np.where(x <= .0031308, x * 12.92, 1.055 * np.maximum(x, 0) ** (1 / 2.4) - .055)

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--project", type=Path, default=Path(__file__).resolve().parents[1])
    parser.add_argument("--apply", action="store_true", help="Write reconstructed PNGs and refresh their content hashes")
    args = parser.parse_args()
    root = args.project.resolve()
    docs = root / "ProjectDocs/SplashTitle_R3_FidelityFix02"
    art = root / "Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed"
    manifest = json.loads((docs / "ColorCalibration.json").read_text(encoding="utf-8"))
    for profile in manifest["profiles"]:
        name = profile["file"]
        original = np.array(Image.open(docs / "Before" / name).convert("RGBA"))
        linear = decode(original[..., :3].astype(np.float32) / 255)
        luminance = (linear * np.array([.2126, .7152, .0722], dtype=np.float32)).sum(axis=-1, keepdims=True)
        adjusted = (luminance + (linear - luminance) * profile["saturation"]) * (2 ** profile["exposureEV"])
        output = original.copy()
        output[..., :3] = np.rint(np.clip(encode(adjusted), 0, 1) * 255).astype(np.uint8)
        output[original[..., 3] == 0, :3] = original[original[..., 3] == 0, :3]
        if not np.array_equal(output[..., 3], original[..., 3]):
            raise RuntimeError("Alpha changed: " + name)
        print(("WRITE " if args.apply else "CHECK ") + name)
        if args.apply:
            Image.fromarray(output).save(art / name)
    name = "golden_fortress_at_winter_sunrise.png"
    original = np.array(Image.open(docs / "Before" / name).convert("RGB"))
    mask = np.array(Image.open(docs / "BackgroundRepairMask.png").convert("L")).astype(np.float32)[..., None] / 255
    if mask.shape[:2] != original.shape[:2]:
        raise ValueError("Repair mask dimensions do not match the source plate")
    donor = np.roll(original, -manifest["background"]["donorOffsetX"], axis=1)
    output = np.rint(original * (1 - mask) + donor * mask).astype(np.uint8)
    if args.apply:
        Image.fromarray(output).save(art / name)
        inventory_path = docs / "asset_inventory.json"
        inventory = json.loads(inventory_path.read_text(encoding="utf-8"))
        for item in inventory:
            item["sha256"] = hashlib.sha256((root / item["file"]).read_bytes()).hexdigest()
            item["visual_approval"] = "Rebuilt locally; requires new visual review"
        inventory_path.write_text(json.dumps(inventory, ensure_ascii=False, indent=2), encoding="utf-8")
    print("No Unity execution or exact-font identification performed.")
    if not args.apply:
        print("Dry run only. Pass --apply to write the PNGs.")

if __name__ == "__main__":
    main()
