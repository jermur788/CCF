#!/usr/bin/env python3
"""Build spec 14.1 comparison sheets: APPROVED REFERENCE | ACTUAL UNITY RENDER.

Each output is one PNG derived from a real harness screenshot: the actual capture is pasted
unmodified at its native pixel size on the right, the locked reference is fitted and letterboxed
(maintaining aspect) on the left, and both sides carry clear labels and source SHA-256 prefixes.

Never fabricates an actual panel: if the renders directory has no PNG screenshots, this exits
without creating any comparison image or manifest.

Example:
  python3 make_comparisons.py \
    --reference specs/scenario-one-ui-visual-system-v2/references/griffins-peak-ui-reference.png \
    --renders /media/jer/Files/CCF/Verification/ScenarioOneUiVisualSystemV2-<stamp>/evidence \
    --out /media/jer/Files/CCF/Verification/ScenarioOneUiVisualSystemV2-<stamp>/comparisons
"""
from __future__ import annotations

import argparse
import hashlib
import json
import os
import sys
from datetime import datetime, timezone
from pathlib import Path

try:
    from PIL import Image, ImageDraw, ImageFont
except ImportError:
    print("ERROR: Pillow is required (pip install Pillow). No files were written.", file=sys.stderr)
    sys.exit(2)

# Frozen primary reference from specs/scenario-one-ui-visual-system-v2/references/reference-manifest.md.
LOCKED_REFERENCE_SHA256 = "1f93c9062b0188834f968c4caeb4a63d70ed48c1ca7f05ddc8c6cd84557b7c4a"
BACKGROUND = (18, 20, 22)
HEADER = (32, 37, 42)
LETTERBOX = (12, 14, 16)
INK = (238, 240, 242)
MUTED = (172, 181, 188)
RULE = (86, 96, 104)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def fail(message: str) -> None:
    print("ERROR: " + message, file=sys.stderr)
    sys.exit(2)


def load_font(size: int, bold: bool = False):
    candidates = [
        "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf" if bold else "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf" if bold else "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
        "/usr/share/fonts/TTF/DejaVuSans-Bold.ttf" if bold else "/usr/share/fonts/TTF/DejaVuSans.ttf",
    ]
    for path in candidates:
        if os.path.isfile(path):
            try:
                return ImageFont.truetype(path, size)
            except OSError:
                pass
    return ImageFont.load_default()


def fit_text(draw: ImageDraw.ImageDraw, text: str, font, max_width: int) -> str:
    if max_width <= 0:
        return ""
    if draw.textlength(text, font=font) <= max_width:
        return text
    ellipsis = "..."
    while text and draw.textlength(text + ellipsis, font=font) > max_width:
        text = text[:-1]
    return (text + ellipsis) if text else ""


def load_rgb(path: Path, label: str) -> Image.Image:
    try:
        with Image.open(path) as probe:
            probe.verify()
        with Image.open(path) as image:
            return image.convert("RGB")
    except Exception as error:  # noqa: BLE001 - report the concrete unreadable source
        fail(f"{label} is not a readable PNG: {path} ({error})")


def actual_pngs(renders: Path) -> list[Path]:
    if not renders.is_dir():
        fail(f"--renders is not a directory: {renders}")
    paths = sorted(
        path for path in renders.glob("*.png")
        if path.is_file() and not path.name.endswith("-comparison.png")
    )
    if not paths:
        fail(f"no real harness PNG screenshots found in {renders}; refusing to create anything")
    return paths


def build_sheet(reference: Image.Image, actual: Image.Image, reference_name: str,
                reference_hash: str, actual_name: str, actual_hash: str) -> Image.Image:
    width, height = actual.size
    gap = max(2, width // 640)
    label_size = max(12, min(22, height // 36))
    small_size = max(10, min(16, height // 64))
    pad = max(4, min(12, height // 80))
    header_height = pad * 3 + label_size + small_size

    canvas = Image.new("RGB", (width * 2 + gap, height + header_height), BACKGROUND)
    draw = ImageDraw.Draw(canvas)
    bold = load_font(label_size, bold=True)
    small = load_font(small_size)

    draw.rectangle((0, 0, canvas.width, header_height), fill=HEADER)
    draw.line((width + gap // 2, 0, width + gap // 2, canvas.height), fill=RULE, width=1)

    left_x, right_x = pad, width + gap + pad
    max_text = max(20, width - pad * 2)
    left_title = "APPROVED REFERENCE" if width >= 260 else "REFERENCE"
    right_title = "ACTUAL UNITY RENDER" if width >= 260 else "ACTUAL"
    draw.text((left_x, pad), fit_text(draw, left_title, bold, max_text), fill=INK, font=bold)
    draw.text((left_x, pad + label_size + 2),
              fit_text(draw, f"{reference_name} · SHA-256 {reference_hash[:16]}...", small, max_text),
              fill=MUTED, font=small)
    draw.text((right_x, pad), fit_text(draw, right_title, bold, max_text), fill=INK, font=bold)
    draw.text((right_x, pad + label_size + 2),
              fit_text(draw, f"{actual_name} · {width}x{height} · SHA-256 {actual_hash[:16]}...", small, max_text),
              fill=MUTED, font=small)

    # Fit the locked reference inside the left panel, preserving aspect, with neutral letterboxing.
    panel = Image.new("RGB", (width, height), LETTERBOX)
    scale = min(width / reference.width, height / reference.height)
    resampling = getattr(Image, "Resampling", Image)
    fitted = reference.resize(
        (max(1, int(reference.width * scale)), max(1, int(reference.height * scale))),
        resampling.LANCZOS,
    )
    panel.paste(fitted, ((width - fitted.width) // 2, (height - fitted.height) // 2))
    canvas.paste(panel, (0, header_height))
    # The actual capture is pasted at its native size, never rescaled or retouched.
    canvas.paste(actual, (width + gap, header_height))
    return canvas


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--reference", required=True, type=Path, help="exact locked approved reference PNG")
    parser.add_argument("--renders", required=True, type=Path, help="directory containing real harness screenshot PNGs")
    parser.add_argument("--out", required=True, type=Path, help="durable evidence directory for comparison PNGs and manifest")
    parser.add_argument("--expected-reference-sha256", default=LOCKED_REFERENCE_SHA256,
                        help="expected SHA-256 of the locked reference (defaults to the frozen manifest hash)")
    args = parser.parse_args()

    reference_path = args.reference.expanduser().resolve()
    renders = args.renders.expanduser().resolve()
    out = args.out.expanduser().resolve()

    if not reference_path.is_file():
        fail(f"reference PNG not found: {reference_path}")
    if not reference_path.name.lower().endswith(".png"):
        fail(f"reference is not a .png: {reference_path}")
    if looks_like_comparison_reference(reference_path):
        fail("the reference must be the approved map image, not a generated comparison sheet")
    if out == renders:
        fail("--out must differ from --renders; keep generated comparisons out of the raw render folder")

    reference_hash = sha256(reference_path)
    if args.expected_reference_sha256 and reference_hash.lower() != args.expected_reference_sha256.lower():
        fail("reference SHA-256 does not match the locked manifest; stop rather than compare against a substitute\n"
             f"  expected: {args.expected_reference_sha256}\n  actual:   {reference_hash}")

    paths = actual_pngs(renders)
    # Validate every actual source before creating the output directory or any image.
    actual_images: list[tuple[Path, str, Image.Image]] = []
    for path in paths:
        actual_images.append((path, sha256(path), load_rgb(path, "actual render")))
    reference = load_rgb(reference_path, "approved reference")

    out.mkdir(parents=True, exist_ok=True)
    comparisons = []
    sha_lines = [f"{reference_hash}  {reference_path}"]
    for path, actual_hash, actual in actual_images:
        output_name = path.stem + "-comparison.png"
        output = out / output_name
        sheet = build_sheet(reference, actual, reference_path.name, reference_hash, path.name, actual_hash)
        sheet.save(output, format="PNG", compress_level=6)
        comparison_hash = sha256(output)
        comparisons.append({
            "actual": path.name,
            "actual_path": str(path),
            "actual_sha256": actual_hash,
            "actual_width": actual.width,
            "actual_height": actual.height,
            "comparison": output_name,
            "comparison_path": str(output),
            "comparison_sha256": comparison_hash,
        })
        sha_lines.append(f"{actual_hash}  {path}")
        sha_lines.append(f"{comparison_hash}  {output}")
        print(f"WROTE {output}  ({actual.width}x{actual.height} actual, native)")

    manifest = {
        "generated_by": "make_comparisons.py",
        "generated_at_utc": datetime.now(timezone.utc).isoformat(),
        "reference": {
            "path": str(reference_path),
            "sha256": reference_hash,
            "width": reference.width,
            "height": reference.height,
            "role": "APPROVED REFERENCE — normative locked input",
        },
        "renders_directory": str(renders),
        "comparison_directory": str(out),
        "note": "Actual panels are the unmodified real Unity harness PNGs; no actual image was generated, "
                "substituted or retouched. Reference is fitted with aspect-preserving letterboxing.",
        "count": len(comparisons),
        "comparisons": comparisons,
    }
    (out / "comparison-manifest.json").write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")
    (out / "comparison-sha256.txt").write_text("\n".join(sha_lines) + "\n", encoding="utf-8")
    print(f"OK comparisons={len(comparisons)} manifest={out / 'comparison-manifest.json'}")
    return 0


def looks_like_comparison_reference(path: Path) -> bool:
    # Guard against using a previously generated composite as the approved authority.
    return path.stem.endswith("-comparison") or "comparison" in path.name.lower()


if __name__ == "__main__":
    sys.exit(main())
