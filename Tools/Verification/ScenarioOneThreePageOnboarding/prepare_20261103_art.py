#!/usr/bin/env python3
"""Extract approved art regions, without generating/repainting forest content.

Input screenshots remain immutable. Runtime atlases contain only the blurred
outside margins and forestry regions; titles, captions and controls are omitted.
"""
import hashlib
import json
from pathlib import Path
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[3]
REF = ROOT / 'specs/scenario-one-three-page-onboarding-v1/references'
GAME = (156, 122, 1517, 886)
SIZE = (1672, 941)
REGIONS = {
    1: [(85, 272, 1586, 594)],
    2: [(86, 365, 381, 677), (400, 365, 685, 677),
        (703, 365, 983, 677), (1003, 365, 1278, 677), (1297, 365, 1586, 677)],
    3: [(83, 264, 663, 590), (961, 225, 1553, 590)],
}

def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()

def main():
    records = []
    for page in range(1, 4):
        original = REF / f'20261103-page-{page}-approved.png'
        source = Image.open(original).convert('RGB')
        assert source.size == SIZE
        game = source.crop(GAME).resize(SIZE, Image.Resampling.LANCZOS)
        atlas = Image.new('RGB', SIZE, (21, 28, 30))
        # Keep only the outside backdrop. A live opaque panel covers its boundary.
        right, bottom = (1574, 905) if page == 3 else (1607, 891)
        margins = [(0, 0, 1672, 63), (0, bottom + 1, 1672, 941),
                   (0, 63, 63, bottom + 1), (right + 1, 63, 1672, bottom + 1)]
        for box in margins + REGIONS[page]:
            atlas.paste(game.crop(box), box)
        if page == 3:
            # Only the two mature Sitka tips rise above the divider. Exclude the
            # neighbouring live lead sentence and divider from the runtime atlas.
            mask = Image.new('L', (592, 31), 0)
            pen = ImageDraw.Draw(mask)
            pen.polygon([(238, 0), (211, 31), (266, 31)], fill=255)
            pen.polygon([(385, 0), (359, 31), (412, 31)], fill=255)
            tips = game.crop((961, 225, 1553, 256))
            clean = Image.new('RGB', tips.size, (21, 28, 30))
            clean.paste(tips, (0, 0), mask)
            atlas.paste(clean, (961, 225))
        destination = ROOT / f'Assets/ForestPrototype/UI/Resources/Onboarding/page-{page}.png'
        atlas.save(destination, optimize=True)
        records.append(dict(page=page, source=original.name, source_sha256=sha(original),
                            game_crop=list(GAME), atlas_size=list(SIZE), regions=REGIONS[page],
                            atlas=str(destination.relative_to(ROOT)), atlas_sha256=sha(destination),
                            atlas_bytes=destination.stat().st_size))
    (REF / '20261103-art-manifest.json').write_text(json.dumps(records, indent=2) + '\n')
    print(json.dumps(records, indent=2))

if __name__ == '__main__':
    main()
