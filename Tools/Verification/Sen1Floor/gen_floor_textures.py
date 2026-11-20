#!/usr/bin/env python3
"""CCF #836: first-party procedural Sitka forest-floor texture family.

Python 3 + Pillow only. No photographs, downloads, numpy or Unity invocation.
Run this file directly; FLOOR_OUT optionally redirects the same four outputs.
The palette, densities and relief are art calibration, not field measurements.
"""

from functools import lru_cache
import hashlib
import math
import os
from pathlib import Path
import random
import time

from PIL import Image, ImageChops, ImageDraw, ImageFilter, ImageStat, PngImagePlugin

SEED = 836_2026
SEEDS = {
    "soil": SEED + 1,
    "mat": SEED + 12,
    "grain": SEED + 3,
    "buried_needles": SEED + 40,
    "surface_needles": SEED + 4,
    "twigs": SEED + 11,
    "floor_moss": SEED + 41,
    "grit": SEED + 42,
    "macro": SEED + 21,
    "moss": SEED + 31,
}
SS = 2
ALBEDO_METRES = 4.0
# One world-space catalogue, sampled at the two prescribed material periods.
CATALOGUE_METRES = ALBEDO_METRES
NORMAL_METRES = 1.6
HEIGHT_RANGE_METRES = 0.006
OUT_DIR = Path(os.environ.get(
    "FLOOR_OUT",
    "/media/jer/ZX20/CCF-Sen1-836-Floor-20261119/Assets/ForestPrototype/Art/Sen1SpruceFloor/Textures",
))
NAMES = (
    "Sen1_SpruceFloor_Base_2048.png",
    "Sen1_SpruceFloor_Normal_1024.png",
    "Sen1_SpruceFloor_Macro_512.png",
    "Sen1_FloorMoss_512.png",
)
RESAMPLE = Image.Resampling

# Weathered warm brown; luminance stays approximately equal to value.
# Fixed channel offsets avoid grey/khaki litter under blue ambient.
def warm_colour(value):
    return (clamp(round(value + 12)), clamp(round(value - 3)),
            clamp(round(value - 19)))


def clamp(value, low=0, high=255):
    return max(low, min(high, value))


def mix(a, b, t):
    t = clamp(t, 0.0, 1.0)
    return tuple(round(x + (y - x) * t) for x, y in zip(a, b))


def tint(colour, delta):
    # A shared delta changes brightness without speckled random channel hues.
    return tuple(clamp(round(c + delta)) for c in colour)


def periodic_noise(size, cells, seed):
    """Smooth seamless lattice noise, including the interpolation stencil.

    Repeat the random lattice 3x3 BEFORE bicubic resizing and keep the centre
    tile. Its four boundaries then have real wrapped neighbours on both sides.
    Appending one repeated row/column and resizing does not provide that stencil
    and can stretch the boundary cells into a visible tile grid.
    """
    rnd = random.Random(seed)
    lattice = Image.new("L", (cells, cells))
    lattice.putdata([rnd.randrange(256) for _ in range(cells * cells)])
    padded = Image.new("L", (cells * 3, cells * 3))
    for y in range(3):
        for x in range(3):
            padded.paste(lattice, (x * cells, y * cells))
    return padded.resize((size * 3, size * 3), RESAMPLE.BICUBIC).crop(
        (size, size, size * 2, size * 2)
    )


def fractal(size, cells, seed, octaves=4, gain=0.53):
    weights = [gain ** i for i in range(octaves)]
    total = sum(weights)
    result = Image.new("L", (size, size))
    for i, weight in enumerate(weights):
        band = periodic_noise(size, cells * 2 ** i, seed + 977 * i)
        band = band.point([round(v * weight / total) for v in range(256)])
        result = ImageChops.add(result, band)
    return result


def wrap_pad(img, pad):
    """Periodic border for filters, rather than Pillow's clamped edge pixels."""
    w, h = img.size
    result = Image.new(img.mode, (w + 2 * pad, h + 2 * pad))
    for oy in (-h, 0, h):
        for ox in (-w, 0, w):
            result.paste(img, (pad + ox, pad + oy))
    return result


def periodic_blur(img, radius):
    pad = math.ceil(radius * 4) + 2
    return wrap_pad(img, pad).filter(ImageFilter.GaussianBlur(radius)).crop(
        (pad, pad, img.width + pad, img.height + pad)
    )


def finish_strokes(img, size):
    """2x strokes preserve subpixel needle widths and broken tapered tips.

    Lanczos blends coverage rather than leaving stair-step one-pixel lines.
    The filter also gets wrapped neighbours so it cannot invent an edge seam.
    """
    pad = 8 * SS
    return wrap_pad(img, pad).resize(
        (size + 16, size + 16), RESAMPLE.LANCZOS
    ).crop((8, 8, size + 8, size + 8))


def wrap_offsets(cx, cy, radius, size):
    """Include diagonal copies as well as side copies for corner crossings."""
    xs, ys = [0], [0]
    if cx - radius < 0:
        xs.append(size)
    if cx + radius >= size:
        xs.append(-size)
    if cy - radius < 0:
        ys.append(size)
    if cy + radius >= size:
        ys.append(-size)
    return ((ox, oy) for ox in xs for oy in ys)


@lru_cache(maxsize=1)
def drift_field():
    """Only gentle 2-20 cm fan-density modulation, never an empty-ground mask."""
    return fractal(512, 48, SEEDS["mat"], octaves=2, gain=0.5)


def drift_at(x, y):
    field = drift_field()
    return field.getpixel((int((x % CATALOGUE_METRES) / CATALOGUE_METRES * 512) % 512,
                           int((y % CATALOGUE_METRES) / CATALOGUE_METRES * 512) % 512)) / 255


@lru_cache(maxsize=2)
def needle_field(surface):
    """Shared physical fans, sampled identically by albedo and height.

    Fans are a readable litter arrangement, not intact botanical bundles.
    Width/length are art calibration at 512 px/m after 2x downsampling.
    """
    rnd = random.Random(SEEDS["surface_needles" if surface else "buried_needles"])
    needles = []
    for _ in range(42000 if surface else 32000):
        cx, cy = rnd.random() * CATALOGUE_METRES, rnd.random() * CATALOGUE_METRES
        density = drift_at(cx, cy)
        # Bernoulli thinning of uniform candidates by a bounded, continuous
        # probability: no thresholded spatial mask, no bare drift regions.
        chance = 0.75 + 0.25 * density
        if rnd.random() > chance:
            continue
        angle = rnd.uniform(0, math.tau)
        fan_bias = rnd.uniform(-2, 2)
        for _ in range(rnd.randint(4, 7)):
            a = angle + rnd.uniform(-0.65, 0.65)
            length = rnd.uniform(8.5, 13.0) / 512
            x = (cx + rnd.uniform(-0.006, 0.006)) % CATALOGUE_METRES
            y = (cy + rnd.uniform(-0.006, 0.006)) % CATALOGUE_METRES
            # Each needle varies independently within the fan, and along its
            # length below. Bleached upper needles remain a small minority.
            roll = rnd.random()
            if roll < 0.20:
                value = rnd.uniform(118, 138)
            elif roll < 0.70:
                value = rnd.uniform(78, 105)
            else:
                value = rnd.uniform(52, 74)
            value += fan_bias
            width = rnd.uniform(1.8, 2.35) / 512
            relief = rnd.uniform(0.00082, 0.00148) if surface else rnd.uniform(0.00029, 0.00070)
            needles.append((x, y, a, length, width, value, relief, rnd.uniform(-3, 3)))
    return tuple(needles)


def paint_needles(img, tile_metres, surface, height=False):
    draw = ImageDraw.Draw(img)
    scale = img.width / tile_metres
    # Both maps consume the same seeded physical catalogue. The normal takes
    # its 1.6 m window; the different requested repeat periods prevent exact
    # registration over the entire material. Neither rescales needle geometry.
    for x, y, angle, length, width, value, relief, gradient in needle_field(surface):
        if x >= tile_metres or y >= tile_metres:
            continue
        cx = x * scale
        cy = y * scale
        dx, dy = math.cos(angle) * length * scale, math.sin(angle) * length * scale
        stroke_width = max(1, round(width * scale))
        for ox, oy in wrap_offsets(cx, cy, length * scale + 4, img.width):
            start = (cx + ox - dx * 0.25, cy + oy - dy * 0.25)
            if height:
                colour = round(64 + relief / HEIGHT_RANGE_METRES * 255)
                end = (start[0] + dx, start[1] + dy)
                draw.line((start, end), fill=colour, width=stroke_width)
            else:
                for segment in range(4):
                    t0, t1 = segment / 4, (segment + 1) / 4
                    colour = warm_colour(value + gradient * (2 * t0 - 1))
                    draw.line(((start[0] + dx * t0, start[1] + dy * t0),
                               (start[0] + dx * t1, start[1] + dy * t1)),
                              fill=colour, width=stroke_width)
            # The final narrow tip is identical in colour and relief.
            end = (start[0] + dx, start[1] + dy)
            tip = (end[0] + dx * 0.05, end[1] + dy * 0.05)
            draw.line((end, tip), fill=colour, width=max(1, stroke_width // 2))


def twig_field():
    rnd = random.Random(SEEDS["twigs"])
    for _ in range(26):
        x, y = rnd.random() * 4, rnd.random() * 4
        angle = rnd.uniform(0, math.tau)
        length = rnd.uniform(0.025, 0.075)
        width = rnd.uniform(0.0020, 0.0038)
        yield x, y, angle, length, width, rnd.uniform(-5, 7)


def paint_twigs(img, tile_metres, height=False):
    draw = ImageDraw.Draw(img)
    scale = img.width / tile_metres
    for x, y, angle, length, width, variation in twig_field():
        if x >= tile_metres or y >= tile_metres:
            continue
        cx, cy = x * scale, y * scale
        dx, dy = math.cos(angle) * length * scale, math.sin(angle) * length * scale
        w = max(2, round(width * scale))
        for ox, oy in wrap_offsets(cx, cy, length * scale + w, img.width):
            p0 = (cx + ox - dx / 2, cy + oy - dy / 2)
            pm = (cx + ox + dy * 0.035, cy + oy - dx * 0.035)
            p1 = (cx + ox + dx / 2, cy + oy + dy / 2)
            colour = 64 + round(width / HEIGHT_RANGE_METRES * 255) if height else tint((66, 62, 51), variation)
            draw.line((p0, pm, p1), fill=colour, width=w)
            if not height:
                draw.line((p0, pm, p1), fill=tint((78, 72, 64), variation), width=max(1, w // 3))
                # A short pale broken end, never a large attention-catching log.
                end = (p1[0] - dx * 0.025, p1[1] - dy * 0.025)
                draw.line((end, p1), fill=tint((92, 84, 74), variation), width=max(1, w - 1))


def soil_colour(size):
    # Compacted fine litter, with no large-scale tone/density coupling.
    grain = periodic_noise(size, size // 2, SEEDS["grain"])
    fibre = fractal(size, 160, SEEDS["soil"], octaves=2)
    pixels = bytearray(size * size * 3)
    for i, (g, f) in enumerate(zip(grain.tobytes(), fibre.tobytes())):
        value = 85 + (g - 128) * 0.08 + (f - 128) * 0.09
        pixels[i * 3:i * 3 + 3] = bytes(warm_colour(value))
    return Image.frombytes("RGB", (size, size), bytes(pixels))


def moss_colour(size):
    patch = fractal(size, 32, SEEDS["moss"] + 1, octaves=2)
    grain = periodic_noise(size, size // 2, SEEDS["moss"] + 2)
    pixels = bytearray(size * size * 3)
    for i, (p, g) in enumerate(zip(patch.tobytes(), grain.tobytes())):
        value = 59 + (p - 128) * 0.035 + (g - 128) * 0.055
        # Slight olive: green just above red, with a lower blue channel.
        pixels[i * 3:i * 3 + 3] = bytes(tint((58, 63, 39), value - 59))
    return Image.frombytes("RGB", (size, size), bytes(pixels)), patch


def build_moss(size=512):
    img, patch = moss_colour(size)
    img = img.resize((size * SS, size * SS), RESAMPLE.BICUBIC)
    draw = ImageDraw.Draw(img)
    rnd = random.Random(SEEDS["moss"])
    pp = patch.load()
    for _ in range(37000):
        cx, cy = rnd.random() * img.width, rnd.random() * img.height
        density = pp[int(cx / SS), int(cy / SS)] / 255
        if rnd.random() > 0.75 + density * 0.25:
            continue
        angle = rnd.uniform(0, math.tau)
        radius = rnd.uniform(0.55, 1.8) * SS
        value = rnd.uniform(62, 98)
        colour = tint((58, 63, 39), value - 59)
        for ox, oy in wrap_offsets(cx, cy, radius + 3, img.width):
            draw.ellipse((cx + ox - SS * 0.5, cy + oy - SS * 0.5,
                          cx + ox + SS * 0.5, cy + oy + SS * 0.5), fill=tint(colour, -30))
            for k in range(4):
                a = angle + k * math.tau / 4
                tip = (cx + ox + math.cos(a) * radius, cy + oy + math.sin(a) * radius)
                draw.line(((cx + ox, cy + oy), tip), fill=colour, width=2)
                draw.point(tip, fill=tint(colour, 7))
    return finish_strokes(img, size)


def build_albedo(size=2048):
    soil = soil_colour(size)
    img = soil.resize((size * SS, size * SS), RESAMPLE.BICUBIC)
    paint_needles(img, ALBEDO_METRES, surface=False)
    paint_needles(img, ALBEDO_METRES, surface=True)
    paint_twigs(img, ALBEDO_METRES)

    # Irregular, internally textured olive islands: about 1-2% silhouette
    # coverage, with broken edges and dark gaps between the short shoots.
    moss = build_moss().resize(img.size, RESAMPLE.BICUBIC)
    mask = Image.new("L", img.size)
    md = ImageDraw.Draw(mask)
    rnd = random.Random(SEEDS["floor_moss"])
    for _ in range(62):
        cx, cy = rnd.random() * img.width, rnd.random() * img.height
        radius = rnd.uniform(0.019, 0.043) * img.width / 4
        for _ in range(rnd.randint(7, 15)):
            x, y = cx + rnd.gauss(0, radius * 0.55), cy + rnd.gauss(0, radius * 0.55)
            r = rnd.uniform(0.18, 0.48) * radius
            opacity = rnd.randint(150, 225)
            for ox, oy in wrap_offsets(x, y, r + 2, img.width):
                md.ellipse((x + ox - r, y + oy - r, x + ox + r, y + oy + r), fill=opacity)
    mask = periodic_blur(mask, 1.0)
    img = Image.composite(moss, img, mask)

    draw = ImageDraw.Draw(img)
    rnd = random.Random(SEEDS["grit"])
    for _ in range(140):
        cx, cy = rnd.random() * img.width, rnd.random() * img.height
        r = rnd.uniform(0.35, 0.85) * SS
        colour = tint((115, 113, 101), rnd.uniform(-12, 12))
        for ox, oy in wrap_offsets(cx, cy, r + 1, img.width):
            draw.ellipse((cx + ox - r, cy + oy - r, cx + ox + r, cy + oy + r), fill=colour)
    img = finish_strokes(img, size)
    return img


def build_normal(size=1024):
    high_size = size * SS
    # Grayscale height encodes a 6 mm range. Needle ridges rise 1.0-1.8 mm;
    # it is not a colour-to-normal conversion or arbitrary normal strength.
    height = fractal(high_size, 8, SEEDS["soil"]).point(
        [round(50 + v * 0.11) for v in range(256)]
    )
    paint_needles(height, NORMAL_METRES, surface=False, height=True)
    paint_needles(height, NORMAL_METRES, surface=True, height=True)
    paint_twigs(height, NORMAL_METRES, height=True)
    height = periodic_blur(finish_strokes(height, size), 0.48)
    hp = height.load()
    pixels = bytearray(size * size * 3)
    # Central difference: divide height change in metres by a two-texel
    # horizontal distance in metres to obtain a physical surface slope.
    slope_scale = HEIGHT_RANGE_METRES / 255 / (2 * NORMAL_METRES / size)
    for y in range(size):
        for x in range(size):
            dx = (hp[(x + 1) % size, y] - hp[(x - 1) % size, y]) * slope_scale
            dy = (hp[x, (y + 1) % size] - hp[x, (y - 1) % size]) * slope_scale
            inv = 1 / math.sqrt(1 + dx * dx + dy * dy)
            # Image rows go DOWN, tangent V goes UP: flip the derivative's
            # y sign before negating it. Thus +dy becomes +green (OpenGL).
            rgb = (round(127.5 - dx * inv * 127.5),
                   round(127.5 + dy * inv * 127.5),
                   round(127.5 + inv * 127.5))
            i = (y * size + x) * 3
            pixels[i:i + 3] = bytes(rgb)
    return Image.frombytes("RGB", (size, size), bytes(pixels))


def build_macro(size=512):
    # 30/6 and 30/17 give dominant ~5 m and ~1.8 m organic patches.
    damp = fractal(size, 6, SEEDS["macro"], octaves=3)
    drift = fractal(size, 17, SEEDS["macro"] + 1, octaves=2)
    dp, sp = damp.load(), drift.load()
    out = Image.new("L", (size, size))
    pixels = out.load()
    for y in range(size):
        wy = ((y + 0.5) / size - 0.5) * 30
        for x in range(size):
            wx = ((x + 0.5) / size - 0.5) * 30
            field = 0.75 * dp[x, y] / 255 + 0.25 * sp[x, y] / 255
            value = 0.56 + 0.40 * field
            edge = clamp((max(abs(wx), abs(wy)) - 9) / 6, 0, 1)
            edge = edge * edge * (3 - 2 * edge)  # gentle smoothstep margin
            pixels[x, y] = round(255 * clamp(value + edge * 0.14, 0.55, 1.0))
    # This map covers the stand once and is clamped, so no periodic filter here.
    return out.filter(ImageFilter.GaussianBlur(0.8))


def save_png(img, path, srgb):
    # PNG metadata documents intent; Unity's importer must ALSO use the right
    # sRGB setting (and Normal Map type for the normal) at integration time.
    info = PngImagePlugin.PngInfo()
    if srgb:
        info.add(b"sRGB", b"\x00")
        info.add(b"gAMA", (45455).to_bytes(4, "big"))
    else:
        info.add(b"gAMA", (100000).to_bytes(4, "big"))
    img.save(path, pnginfo=info, compress_level=9, optimize=False)


def measured_stats(img):
    """Pillow sRGB greyscale (BT.601), population SD, nearest-rank tails."""
    luminance = img.convert("L")
    histogram = luminance.histogram()
    total = sum(histogram)

    def percentile(fraction):
        cumulative = 0
        for value, count in enumerate(histogram):
            cumulative += count
            if cumulative >= total * fraction:
                return value

    stats = ImageStat.Stat(luminance)
    radius = 100 if img.width == 2048 else 32
    blurred = luminance.filter(ImageFilter.GaussianBlur(radius))
    small = luminance.filter(ImageFilter.GaussianBlur(8))
    total_delta = total_squared = 0
    for value, smooth in zip(luminance.tobytes(), small.tobytes()):
        delta = value - smooth
        total_delta += delta
        total_squared += delta * delta
    return {"blurred_sd": round(ImageStat.Stat(blurred).stddev[0], 4),
            "high_pass_sd": round(math.sqrt(total_squared / total - (total_delta / total) ** 2), 4),
            "luminance_mean": round(stats.mean[0], 4),
            "luminance_sd": round(stats.stddev[0], 4),
            "p5": percentile(0.05), "p95": percentile(0.95),
            "mean_RGB": tuple(round(v, 4) for v in ImageStat.Stat(img).mean)}


def main():
    start = time.perf_counter()
    OUT_DIR.mkdir(parents=True, exist_ok=True)
    builders = (build_albedo, build_normal, build_macro, build_moss)
    for name, builder, srgb in zip(NAMES, builders, (True, False, False, True)):
        if name == NAMES[2] and (OUT_DIR / name).exists():
            print(f"{name}: unchanged (existing macro retained)", flush=True)
            continue
        phase = time.perf_counter()
        img = builder()
        path = OUT_DIR / name
        save_png(img, path, srgb)
        data = path.read_bytes()
        if name in (NAMES[0], NAMES[3]):
            print(f"MEASURED {name}: {measured_stats(img)}", flush=True)
        print(f"{name}: {img.mode} {img.size}, {len(data)} bytes, "
              f"sha256 {hashlib.sha256(data).hexdigest()}, "
              f"{time.perf_counter() - phase:.2f} s", flush=True)
    print(f"TOTAL_SECONDS={time.perf_counter() - start:.2f}", flush=True)
    print(f"SEEDS={SEEDS}", flush=True)


if __name__ == "__main__":
    main()
