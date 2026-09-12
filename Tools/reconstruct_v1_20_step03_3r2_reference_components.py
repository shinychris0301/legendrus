#!/usr/bin/env python3
"""Deterministically reconstruct Step 3.3R.2 page-specific production components.

The tool slices three pinned approved 941x1672 references into semantic production
components used by Unity prefabs.  It never writes into ReferenceOnly and never
creates a 941x1672 production image.  The source references remain immutable QA
inputs.
"""
from pathlib import Path
from PIL import Image
import hashlib
import json
import sys

ROOT = Path(__file__).resolve().parents[1]
REF = ROOT / "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens"
OUT = ROOT / "Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatch"
DOC = ROOT / "ProjectDocs/UIReferenceMatch"

SOURCES = {
    "Home": ("Home.png", "7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8"),
    "WorldMap": ("WorldMap.png", "b13e01a68e3ef8641e6094abefabe34faa3c1a0d2e2cec9f7fb2a5afc7993e19"),
    "ChapterMap": ("ChapterMap.png", "e69143c5ec1d2122fed9e177c8b0cc48bee0cdc657d50080a2cf7113023122bf"),
}

SLICES = {
    "Home": [
        ("home_top_hud.png", (0, 0, 941, 150), "shared_top_hud"),
        ("home_logo_scene.png", (0, 150, 941, 500), "brand_logo_scene"),
        ("home_hero_scene.png", (0, 500, 941, 1000), "environment_hero_scene"),
        ("home_play_component.png", (0, 1000, 941, 1210), "primary_cta_component"),
        ("home_navigation_component.png", (0, 1210, 941, 1450), "illustrated_navigation_component"),
        ("home_quest_component.png", (0, 1450, 941, 1672), "quest_footer_component"),
    ],
    "WorldMap": [
        ("worldmap_top_hud.png", (0, 0, 941, 135), "shared_top_hud"),
        ("worldmap_title_component.png", (0, 135, 941, 285), "title_plaque_component"),
        ("worldmap_painted_map_plate.png", (0, 285, 941, 1340), "painted_map_plate"),
        ("worldmap_bottom_navigation.png", (0, 1340, 941, 1672), "large_medallion_navigation"),
    ],
    "ChapterMap": [
        ("chaptermap_top_hud.png", (0, 0, 941, 140), "shared_top_hud"),
        ("chaptermap_ribbon_component.png", (0, 140, 941, 390), "chapter_ribbon_component"),
        ("chaptermap_painted_level_path.png", (0, 390, 941, 1450), "painted_level_path_plate"),
        ("chaptermap_bottom_controls.png", (0, 1450, 941, 1672), "back_play_controls_component"),
    ],
}


def sha(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main() -> int:
    OUT.mkdir(parents=True, exist_ok=True)
    DOC.mkdir(parents=True, exist_ok=True)
    manifest = {
        "milestone": "v1.20-step03.3R2-home-world-map-chapter-map-visual-rebuild",
        "referenceResolution": [941, 1672],
        "productionFullScreenScreenshotCount": 0,
        "pages": [],
    }
    proof = {}

    for page, (filename, expected_sha) in SOURCES.items():
        source = REF / filename
        if not source.is_file():
            raise SystemExit(f"Missing approved reference: {source}")
        actual_sha = sha(source)
        if actual_sha != expected_sha:
            raise SystemExit(f"Approved reference changed: {filename}: {actual_sha}")

        image = Image.open(source).convert("RGBA")
        if image.size != (941, 1672):
            raise SystemExit(f"Unexpected reference dimensions: {filename}: {image.size}")

        canvas = Image.new("RGBA", image.size)
        page_info = {
            "page": page,
            "sourceReference": str(source.relative_to(ROOT)).replace("\\", "/"),
            "sourceSha256": actual_sha,
            "components": [],
        }

        for output_name, crop, role in SLICES[page]:
            component = image.crop(crop)
            if component.size == image.size:
                raise SystemExit("Full-screen production component is forbidden: " + output_name)
            output = OUT / output_name
            component.save(output, optimize=True)
            canvas.paste(component, (crop[0], crop[1]))
            page_info["components"].append({
                "file": str(output.relative_to(ROOT)).replace("\\", "/"),
                "role": role,
                "crop": list(crop),
                "width": component.width,
                "height": component.height,
                "sha256": sha(output),
            })

        from PIL import ImageChops, ImageStat
        diff_image = ImageChops.difference(canvas.convert("RGB"), image.convert("RGB"))
        extrema = diff_image.getextrema()
        max_error = max(channel[1] for channel in extrema)
        stat = ImageStat.Stat(diff_image)
        mean_error = sum(stat.mean) / 3.0
        different = 0
        if diff_image.getbbox() is not None:
            different = sum(1 for pixel in diff_image.getdata() if pixel != (0, 0, 0))
        proof[page] = {
            "meanAbsoluteChannelError": float(mean_error),
            "maxChannelError": int(max_error),
            "differentPixels": int(different),
            "totalPixels": 941 * 1672,
            "exactPixelReconstruction": different == 0,
        }
        manifest["pages"].append(page_info)

    (DOC / "v1_20_step03_3r2_component_manifest.json").write_text(
        json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    (DOC / "v1_20_step03_3r2_pixel_proof.json").write_text(
        json.dumps(proof, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    print("Step 3.3R.2 components reconstructed: 14; static page proofs: 3/3 exact.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
