using System;
using System.Collections.Generic;
using System.Linq;

// Draws the FOREST of the map: every tree at its real position, as a crown of its real radius, over the ground. This is the picture
// that makes the map read as forest structure (dense conifer, a broadleaf group, a gap where a rack was cut) instead of a data grid.
//
// What is real and what is only drawing:
//   REAL      where each tree stands, how wide its crown is (the simulation's crown radius), its species class, and whether it is
//             living, standing dead or a stump. Tall crowns are drawn over short ones.
//   DRAWING   the shading that makes a crown look round (light from the north-west), its slightly irregular outline and the leaf
//             texture, and the small colour differences between neighbours. These come from a number made from each tree's id and
//             carry no information; they only stop 1,300 crowns looking stamped from one mould.
//
// Plain numbers with no UnityEngine (like the rest of the shared map logic), so it can be checked and looked at offline. It draws into
// a byte array of RGBA pixels whose first row is the SOUTH edge of the property, so the bytes load straight into a texture that is then
// shown north-up. One call draws one presentation: the full map and the minimap each call it for their own size, with the same trees.
//
// Cost: a crown "sprite" (the lit, textured picture of one crown of one size) is made once and stamped for every tree that size, so a
// redraw of 1,300 trees is a few thousand sprite copies, not a few million shaded pixels.
public static class StandMapCanopyRaster
{
    // Pixel sides the presentations use. The full map is drawn larger than it is shown so it stays sharp at 1080p.
    public const int FullSidePx = 768;
    public const int MiniSidePx = 256;

    private const int Variants = 5;
    // Light from the north-west and above (the usual map convention), so crowns look lit on their north-west side.
    private const float LightX = -0.52f, LightY = 0.56f, LightZ = 0.64f;
    private const float Ambient = 0.56f, Diffuse = 0.66f;

    private sealed class Sprite
    {
        public int Size;
        // Straight (not premultiplied) RGBA, rows from the south.
        public byte[] Rgba;
        // Shadow only: one alpha byte per pixel.
        public byte[] Alpha;
        // For each row, the first and one-past-the-last column that has any colour. A crown is round, so most of the square sprite is
        // empty corners; stamping only the span roughly halves the work.
        public int[] SpanStart, SpanEnd;
    }

    private static readonly Dictionary<long, Sprite> crowns = new Dictionary<long, Sprite>();
    private static readonly Dictionary<long, Sprite> shadows = new Dictionary<long, Sprite>();
    private static byte[] groundA, groundB;

    // The number of crown sprites made so far (a verification reads it to show a redraw reuses them).
    public static int CrownSpritesMade => crowns.Count;

    public static byte[] Render(StandMapModel model, int sidePx, MapDetail detail)
    {
        var rgba = new byte[sidePx * sidePx * 4];
        new Job(model, sidePx, detail, rgba).Finish();
        return rgba;
    }

    public static void Render(StandMapModel model, int sidePx, MapDetail detail, byte[] rgba) => new Job(model, sidePx, detail, rgba).Finish();

    // One redraw of the canopy picture, done a few trees at a time so a forest change never stalls a frame: the presentation calls Step()
    // each frame until it returns true (or Finish() to do the rest at once, for example when the map is opened part-way through).
    // The job takes its own copy of the trees, so the live list can change under it; the presentation then starts a new job.
    public sealed class Job
    {
        private readonly MapTree[] trees;
        private readonly float minX, minZ, ppm;
        private readonly int sidePx;
        private readonly bool mini, hasProperty;
        private readonly byte[] rgba;
        private int groundRow, nextTree;

        public bool Done { get; private set; }
        public int SidePx => sidePx;
        public byte[] Rgba => rgba;
        public int TreeCount => trees.Length;

        public Job(StandMapModel model, int sidePx, MapDetail detail, byte[] rgba)
        {
            if (rgba == null || rgba.Length < sidePx * sidePx * 4) throw new ArgumentException("buffer too small");
            this.sidePx = sidePx;
            this.rgba = rgba;
            mini = detail == MapDetail.Mini;
            hasProperty = model != null && model.HasProperty && model.SizeM > 0f;
            trees = hasProperty ? model.Trees.ToArray() : new MapTree[0];
            if (hasProperty) { minX = model.MinX; minZ = model.MinZ; ppm = sidePx / model.SizeM; }
        }

        // Draws the next slice: some ground rows first, then some trees, shortest first. Returns true once the picture is complete.
        public bool Step(int treesPerStep = 64, int groundRowsPerStep = 192)
        {
            if (Done) return true;
            if (groundRow < sidePx)
            {
                FillGround(sidePx, rgba, groundRow, Math.Min(sidePx, groundRow + groundRowsPerStep));
                groundRow += groundRowsPerStep;
                return false;
            }
            int end = Math.Min(trees.Length, nextTree + treesPerStep);
            for (; nextTree < end; nextTree++) DrawTree(trees[nextTree]);
            Done = nextTree >= trees.Length;
            return Done;
        }

        public void Finish() { while (!Step(int.MaxValue / 2, sidePx)) { } }

        // Each tree draws its own shadow and then itself, shortest first, so a tall tree's shadow lands on the shorter crowns around it.
        private void DrawTree(MapTree tree)
        {
            float cx = (tree.X - minX) * ppm, cy = (tree.Z - minZ) * ppm;
            if (tree.State == MapTreeState.Stump)
            {
                if (!mini) Disc(rgba, sidePx, cx, cy, Math.Max(1.4f, 0.16f * ppm), StandMapSymbols.StumpColour, 0.92f);
                return;
            }
            float radius = Math.Max(1.6f, tree.CrownRadiusM * ppm);
            // Round the size so trees of nearly the same size share one sprite: to a whole pixel on a big crown, half a pixel on a small one.
            float bucket = radius >= 8f ? (float)Math.Round(radius) : (float)Math.Round(radius * 2.0) / 2f;
            int variant = (int)((uint)tree.Seed % Variants);
            if (bucket >= 4f)
            {
                int shift = (int)Math.Round(bucket * 0.30f);
                Stamp(rgba, sidePx, ShadowSprite(bucket), (int)Math.Round(cx) + shift, (int)Math.Round(cy) - shift, true);
            }
            Stamp(rgba, sidePx, CrownSprite(tree.Kind, tree.State, bucket, variant, mini), (int)Math.Round(cx), (int)Math.Round(cy), false);
        }
    }

    // One crown on a transparent square, for the legend: the very crown the map draws, at swatch size (rows from the south, like the picture).
    public static byte[] RenderCrownSwatch(MapTreeKind kind, MapTreeState state, int sidePx)
    {
        var rgba = new byte[sidePx * sidePx * 4];
        float radius = sidePx * 0.44f;
        float bucket = (float)Math.Round(radius * 2.0) / 2f;
        Sprite crown = CrownSprite(kind, state, bucket, 1, false);
        int x0 = sidePx / 2 - crown.Size / 2, y0 = sidePx / 2 - crown.Size / 2;
        for (int sy = 0; sy < crown.Size; sy++)
        {
            int y = y0 + sy;
            if (y < 0 || y >= sidePx) continue;
            for (int sx = 0; sx < crown.Size; sx++)
            {
                int x = x0 + sx;
                if (x < 0 || x >= sidePx) continue;
                int s = (sy * crown.Size + sx) * 4, d = (y * sidePx + x) * 4;
                rgba[d] = crown.Rgba[s]; rgba[d + 1] = crown.Rgba[s + 1]; rgba[d + 2] = crown.Rgba[s + 2]; rgba[d + 3] = crown.Rgba[s + 3];
            }
        }
        return rgba;
    }

    // ---------- ground ----------
    private static void FillGround(int sidePx, byte[] rgba, int rowFrom, int rowTo)
    {
        if (groundA == null) { groundA = NoiseTile(128, 0x51ed270bu); groundB = NoiseTile(97, 0x9e3779b9u); }
        MapColour g = StandMapSymbols.Ground;
        int i = rowFrom * sidePx * 4;
        for (int y = rowFrom; y < rowTo; y++)
        {
            int rowA = (y % 128) * 128, rowB = (y % 97) * 97;
            for (int x = 0; x < sidePx; x++)
            {
                // Two tiles of different periods, so the pattern does not repeat visibly.
                int n = (groundA[rowA + (x & 127)] - 128) * 3 / 5 + (groundB[rowB + x % 97] - 128) * 2 / 5;
                rgba[i++] = Clamp(g.R + n / 7);
                rgba[i++] = Clamp(g.G + n / 6);
                rgba[i++] = Clamp(g.B + n / 8);
                rgba[i++] = 255;
            }
        }
    }

    private static byte[] NoiseTile(int size, uint seed)
    {
        var tile = new byte[size * size];
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // Value noise on a coarse periodic lattice, two octaves.
                float n = 0.65f * LatticeNoise(x, y, size, 14, seed) + 0.35f * LatticeNoise(x, y, size, 5, seed ^ 0xabcdu);
                tile[y * size + x] = (byte)Math.Max(0, Math.Min(255, (int)(n * 255f)));
            }
        return tile;
    }

    private static float LatticeNoise(int x, int y, int size, int cells, uint seed)
    {
        float fx = (float)x * cells / size, fy = (float)y * cells / size;
        int ix = (int)Math.Floor(fx), iy = (int)Math.Floor(fy);
        float tx = Smooth(fx - ix), ty = Smooth(fy - iy);
        float a = Rand01(Hash((uint)(ix % cells), (uint)(iy % cells), seed)), b = Rand01(Hash((uint)((ix + 1) % cells), (uint)(iy % cells), seed));
        float c = Rand01(Hash((uint)(ix % cells), (uint)((iy + 1) % cells), seed)), d = Rand01(Hash((uint)((ix + 1) % cells), (uint)((iy + 1) % cells), seed));
        return Lerp(Lerp(a, b, tx), Lerp(c, d, tx), ty);
    }

    // ---------- sprites ----------
    private static Sprite CrownSprite(MapTreeKind kind, MapTreeState state, float radius, int variant, bool mini)
    {
        long key = ((long)(int)kind << 40) | ((long)(int)state << 36) | ((long)(radius * 2f) << 8) | ((long)variant << 1) | (mini ? 1L : 0L);
        if (crowns.TryGetValue(key, out Sprite found)) return found;
        Sprite made = MakeCrown(kind, state, radius, variant, mini);
        crowns[key] = made;
        return made;
    }

    private static Sprite ShadowSprite(float radius)
    {
        long key = (long)(radius * 2f);
        if (shadows.TryGetValue(key, out Sprite found)) return found;
        int size = (int)Math.Ceiling(radius * 2f + 6f);
        if ((size & 1) == 1) size++;
        var alpha = new byte[size * size];
        float half = size * 0.5f;
        for (int iy = 0; iy < size; iy++)
            for (int ix = 0; ix < size; ix++)
            {
                float px = ix + 0.5f - half, py = iy + 0.5f - half;
                float d = (float)Math.Sqrt(px * px + py * py) / radius;
                // A soft disc: strongest in the middle, fading across the rim.
                float a = 0.40f * (1f - SmoothStep(0.55f, 1.15f, d));
                alpha[iy * size + ix] = (byte)(a * 255f);
            }
        var sprite = new Sprite { Size = size, Alpha = alpha };
        ComputeSpans(sprite, true);
        shadows[key] = sprite;
        return sprite;
    }

    private static Sprite MakeCrown(MapTreeKind kind, MapTreeState state, float radius, int variant, bool mini)
    {
        uint seed = Hash((uint)kind * 977u + (uint)state * 131u, (uint)(radius * 2f), (uint)variant + 7u);
        bool dead = state == MapTreeState.StandingDead;
        bool conifer = kind == MapTreeKind.Conifer;
        // A standing dead tree has lost its foliage: a thinner, ragged crown of bare branches.
        float r = dead ? radius * 0.80f : radius;
        int size = (int)Math.Ceiling(radius * 2.7f + 4f);
        if ((size & 1) == 1) size++;
        var rgba = new byte[size * size * 4];
        float half = size * 0.5f;
        MapColour b = dead ? StandMapSymbols.DeadBase : StandMapSymbols.CanopyBase(kind);

        // Per-crown variety, all from the tree's own seed: a tint, a warm/cool lean, a slightly oval and turned shape, a ragged outline
        // and where the leaf pattern starts. None of it is data.
        float tint = 0.87f + 0.26f * Rand01(Hash(seed, 1u, 2u));
        float warm = -0.07f + 0.14f * Rand01(Hash(seed, 3u, 4u));
        float phase1 = 6.2832f * Rand01(Hash(seed, 5u, 6u)), phase2 = 6.2832f * Rand01(Hash(seed, 7u, 8u));
        float stretch = 0.90f + 0.20f * Rand01(Hash(seed, 13u, 14u));
        float turn = 6.2832f * Rand01(Hash(seed, 15u, 16u));
        float cosT = (float)Math.Cos(turn), sinT = (float)Math.Sin(turn);
        float ox = 40f * Rand01(Hash(seed, 9u, 10u)), oy = 40f * Rand01(Hash(seed, 11u, 12u));
        int lobes1, lobes2;
        float amp1, amp2, clump, speckle, spokes;
        if (conifer) { lobes1 = 7 + (variant % 5); lobes2 = 13; amp1 = 0.075f; amp2 = 0.040f; clump = 0.17f; speckle = 0.08f; spokes = 0.035f; }
        else if (dead) { lobes1 = 5; lobes2 = 9; amp1 = 0.10f; amp2 = 0.06f; clump = 0.10f; speckle = 0.05f; spokes = 0f; }
        else if (kind == MapTreeKind.Oak) { lobes1 = 6 + (variant % 3); lobes2 = 11; amp1 = 0.15f; amp2 = 0.07f; clump = 0.20f; speckle = 0.07f; spokes = 0f; }
        else { lobes1 = 5 + (variant % 3); lobes2 = 9; amp1 = 0.12f; amp2 = 0.06f; clump = 0.17f; speckle = 0.06f; spokes = 0f; }
        // On a very small crown (the minimap) the fine detail is under a pixel: keep the shading, soften the texture.
        if (mini || radius < 3.2f) { clump *= 0.5f; speckle = 0f; spokes = 0f; }

        for (int iy = 0; iy < size; iy++)
            for (int ix = 0; ix < size; ix++)
            {
                float px = ix + 0.5f - half, py = iy + 0.5f - half;
                // The crown's own frame: turned and slightly oval.
                float qx = (px * cosT + py * sinT) / stretch, qy = (-px * sinT + py * cosT) * stretch;
                float dist = (float)Math.Sqrt(qx * qx + qy * qy);
                float theta = (float)Math.Atan2(qy, qx);
                float effective = r * (1f + amp1 * (float)Math.Cos(lobes1 * theta + phase1) + amp2 * (float)Math.Cos(lobes2 * theta + phase2));
                float coverage = Clamp01(effective - dist + 0.5f);
                if (coverage <= 0f) continue;

                float t = Math.Min(1f, dist / effective);
                float w = (float)Math.Sqrt(Math.Max(0f, 1f - t * t));
                float lambert = Math.Max(0f, (px / r) * LightX + (py / r) * LightY + w * LightZ);
                float shade = (Ambient + Diffuse * lambert) * (1f - 0.36f * SmoothStep(0.62f, 1f, t));

                float texture;
                float alpha = coverage;
                if (dead)
                {
                    // Bare branches radiating from the trunk: a ragged star, translucent between them.
                    float branch = (float)Math.Abs(Math.Cos(theta * 6f + phase2));
                    branch = branch * branch * branch;
                    float rag = LumpNoise(qx / r * 7f + ox, qy / r * 7f + oy, seed);
                    alpha = coverage * Clamp01(0.42f + 0.38f * branch + 0.30f * (rag - 0.5f)) * (1f - 0.30f * t);
                    shade = 0.62f + 0.26f * lambert;
                    texture = 1f + clump * (rag - 0.5f) * 2f;
                }
                else
                {
                    float tuft = LumpNoise(qx / r * (conifer ? 4.6f : 3.4f) + ox, qy / r * (conifer ? 4.6f : 3.4f) + oy, seed);
                    float fine = LumpNoise(qx / r * 11f + oy, qy / r * 11f + ox, seed ^ 0x5bd1e995u);
                    float star = (float)Math.Cos(theta * (lobes1 * 2) + phase2) * (0.30f + 0.70f * t);
                    texture = 1f + clump * (tuft - 0.5f) * 2f + speckle * (fine - 0.5f) * 2f + spokes * star;
                    // A faint brightening at the crown's top (the leader / central tuft).
                    shade += 0.10f * (1f - SmoothStep(0f, 0.45f, t)) * lambert;
                }

                float m = shade * texture * tint;
                int o = (iy * size + ix) * 4;
                rgba[o] = Clamp(b.R * m * (1f + warm));
                rgba[o + 1] = Clamp(b.G * m);
                rgba[o + 2] = Clamp(b.B * m * (1f - warm * 0.9f));
                rgba[o + 3] = (byte)(Clamp01(alpha) * 255f);
            }
        var made = new Sprite { Size = size, Rgba = rgba };
        ComputeSpans(made, false);
        return made;
    }

    private static float LumpNoise(float x, float y, uint seed)
    {
        int ix = (int)Math.Floor(x), iy = (int)Math.Floor(y);
        float tx = Smooth(x - ix), ty = Smooth(y - iy);
        float a = Rand01(Hash((uint)ix, (uint)iy, seed)), b = Rand01(Hash((uint)(ix + 1), (uint)iy, seed));
        float c = Rand01(Hash((uint)ix, (uint)(iy + 1), seed)), d = Rand01(Hash((uint)(ix + 1), (uint)(iy + 1), seed));
        return Lerp(Lerp(a, b, tx), Lerp(c, d, tx), ty);
    }

    // ---------- copying a sprite onto the picture ----------
    private static void Stamp(byte[] rgba, int sidePx, Sprite sprite, int cx, int cy, bool shadow)
    {
        int size = sprite.Size, half = size / 2;
        int x0 = cx - half, y0 = cy - half;
        for (int sy = 0; sy < size; sy++)
        {
            int y = y0 + sy;
            if (y < 0 || y >= sidePx) continue;
            int from = Math.Max(sprite.SpanStart[sy], -x0), to = Math.Min(sprite.SpanEnd[sy], sidePx - x0);
            if (from >= to) continue;
            int d = (y * sidePx + x0 + from) * 4;
            if (shadow)
            {
                int a0 = sy * size;
                for (int sx = from; sx < to; sx++, d += 4)
                {
                    int a = sprite.Alpha[a0 + sx];
                    if (a == 0) continue;
                    int keep = 255 - a;
                    rgba[d] = (byte)(rgba[d] * keep / 255);
                    rgba[d + 1] = (byte)(rgba[d + 1] * keep / 255);
                    rgba[d + 2] = (byte)(rgba[d + 2] * keep / 255);
                }
            }
            else
            {
                int s0 = sy * size * 4 + from * 4;
                byte[] src = sprite.Rgba;
                for (int sx = from; sx < to; sx++, d += 4, s0 += 4)
                {
                    int a = src[s0 + 3];
                    if (a == 0) continue;
                    if (a == 255) { rgba[d] = src[s0]; rgba[d + 1] = src[s0 + 1]; rgba[d + 2] = src[s0 + 2]; continue; }
                    int keep = 255 - a;
                    rgba[d] = (byte)((src[s0] * a + rgba[d] * keep) / 255);
                    rgba[d + 1] = (byte)((src[s0 + 1] * a + rgba[d + 1] * keep) / 255);
                    rgba[d + 2] = (byte)((src[s0 + 2] * a + rgba[d + 2] * keep) / 255);
                }
            }
        }
    }

    private static void ComputeSpans(Sprite sprite, bool useAlphaArray)
    {
        int size = sprite.Size;
        sprite.SpanStart = new int[size];
        sprite.SpanEnd = new int[size];
        for (int y = 0; y < size; y++)
        {
            int first = size, last = -1;
            for (int x = 0; x < size; x++)
            {
                bool any = useAlphaArray ? sprite.Alpha[y * size + x] != 0 : sprite.Rgba[(y * size + x) * 4 + 3] != 0;
                if (!any) continue;
                if (x < first) first = x;
                last = x;
            }
            sprite.SpanStart[y] = first;
            sprite.SpanEnd[y] = last + 1;
        }
    }

    private static void Disc(byte[] rgba, int sidePx, float cx, float cy, float radius, MapColour colour, float opacity)
    {
        int x0 = (int)Math.Floor(cx - radius - 1), x1 = (int)Math.Ceiling(cx + radius + 1), y0 = (int)Math.Floor(cy - radius - 1), y1 = (int)Math.Ceiling(cy + radius + 1);
        for (int y = Math.Max(0, y0); y <= Math.Min(sidePx - 1, y1); y++)
            for (int x = Math.Max(0, x0); x <= Math.Min(sidePx - 1, x1); x++)
            {
                float dx = x + 0.5f - cx, dy = y + 0.5f - cy;
                float a = Clamp01(radius - (float)Math.Sqrt(dx * dx + dy * dy) + 0.5f) * opacity;
                if (a <= 0f) continue;
                int d = (y * sidePx + x) * 4;
                rgba[d] = Clamp(rgba[d] + (colour.R - rgba[d]) * a);
                rgba[d + 1] = Clamp(rgba[d + 1] + (colour.G - rgba[d + 1]) * a);
                rgba[d + 2] = Clamp(rgba[d + 2] + (colour.B - rgba[d + 2]) * a);
            }
    }

    // ---------- small helpers ----------
    private static byte Clamp(float v) => (byte)Math.Max(0f, Math.Min(255f, v));
    private static byte Clamp(int v) => (byte)Math.Max(0, Math.Min(255, v));
    private static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
    private static float Lerp(float a, float b, float t) => a + (b - a) * t;
    private static float Smooth(float t) => t * t * (3f - 2f * t);
    private static float SmoothStep(float edge0, float edge1, float x) { float t = Clamp01((x - edge0) / (edge1 - edge0)); return t * t * (3f - 2f * t); }

    // A small deterministic integer hash (so every machine draws the same canopy from the same trees).
    private static uint Hash(uint a, uint b, uint c)
    {
        unchecked
        {
            uint h = a * 0x9e3779b1u + b * 0x85ebca6bu + c * 0xc2b2ae35u + 0x27d4eb2fu;
            h ^= h >> 15; h *= 0x2c1b3c6du; h ^= h >> 12; h *= 0x297a2d39u; h ^= h >> 15;
            return h;
        }
    }
    private static float Rand01(uint h) => (h & 0xFFFFFFu) / 16777216f;
}
