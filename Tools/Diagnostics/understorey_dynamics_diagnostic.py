#!/usr/bin/env python3
"""Understorey Dynamics diagnostic (Docs/UnderstoreyDynamicsCalibration.md).

Diagnostic only. It does not read or change Unity runtime mechanics. It runs
the candidate causal relationships from Understorey_Dynamics_Irish_CCF_Report
(1 Oct 2026) against simple light trajectories, coupled to the Browsing &
Protection v1 parameters exactly as implemented in JuvenileEcologyRules, so a
future production task starts from quantified, bounded candidates.

Standard library only; deterministic (no random numbers). Usage:
    python3 Tools/Diagnostics/understorey_dynamics_diagnostic.py
"""

# ---------------------------------------------------------------- parameters
# Tags: [E] empirical, [G] guidance, [I] inference, [S] abstraction, [C] calibration.

# Relative-light anchors (RLI 0-1) -> target cover 0-1 on a suitable site with
# a source present. Bands from the report's light-response table; the bramble
# 5-7% light / ~20% cover point is the transferred empirical anchor [E].
LIGHT_TARGET = {
    "bramble":   [(0.00, 0.05), (0.06, 0.20), (0.15, 0.40), (0.30, 0.70), (0.60, 0.85), (1.00, 0.85)],  # [E anchor, S bands]
    "bracken":   [(0.00, 0.03), (0.05, 0.08), (0.15, 0.30), (0.30, 0.70), (0.60, 0.85), (1.00, 0.85)],  # [E transferred shape, S]
    "graminoid": [(0.00, 0.00), (0.05, 0.02), (0.15, 0.10), (0.30, 0.35), (0.60, 0.75), (1.00, 0.85)],  # [G + S]
}
# Annual approach to target (rise when below, fall when above) [C]. Fall rates
# below rise rates give the report's required hysteresis ("persists after light
# falls", rhizome persistence for bracken).
RISE = {"bramble": 0.35, "bracken": 0.30, "graminoid": 0.50}
FALL = {"bramble": 0.15, "bracken": 0.08, "graminoid": 0.30}
# Neighbour influx for rhizomatous bracken: 1-2 m/yr lateral spread [E
# transferred] across a 5 m cell ~ 0.2-0.4 of a cell width a year [I]; applied
# as a fraction of the neighbour's cover excess. Bramble weaker [S].
SPREAD = {"bramble": 0.10, "bracken": 0.25, "graminoid": 0.0}

# Shared competition: sum(cover x strength x clamp(1 - h / overtop)) [report §7].
STRENGTH = {"bramble": 0.60, "bracken": 0.70, "graminoid": 0.50}   # [C] ordering per report table
OVERTOP_M = {"bramble": 1.2, "bracken": 1.5, "graminoid": 0.5}      # [S] group-specific overtop heights
COMPETITION_CAP = 0.8                                              # [C] max fraction of growth removed

# Bramble concealment (only bramble in v1, report §8). Exposure multiplier
# 1 - min(CONCEAL_CAP, cover x heightFactor). Report table: dense protective
# vegetation 0.5 x exposure, range 0.25-0.9 [E direction, S value].
CONCEAL_CAP = 0.5
def bramble_height_m(cover):  # [S] derived height; ~0.6 m dense bramble [E transferred, Harmer 2010]
    return 0.2 + 0.8 * cover

# Browsing & Protection v1, as implemented (JuvenileEcologyRules / Sessile oak asset).
BROWSE_CAP, INCREMENT_LOSS, EXTRA_MORTALITY = 0.95, 0.9, 0.04
OAK = dict(palatability=1.0, full=1.2, escape=1.8, growth=0.35, promotion=3.5)
def oak_light_response(light):  # legacy juvenile light response anchors (TreeSpeciesDefinition defaults)
    return interp([(0.0, 0.0), (0.1, 0.1), (0.2, 0.3), (0.25, 0.5), (0.55, 0.9), (1.0, 0.8)], light)

# ----------------------------------------------------------------- functions

def interp(anchors, x):
    if x <= anchors[0][0]:
        return anchors[0][1]
    for (x0, y0), (x1, y1) in zip(anchors, anchors[1:]):
        if x <= x1:
            return y0 + (y1 - y0) * (x - x0) / (x1 - x0)
    return anchors[-1][1]

def step_cover(cover, group, light, source=1.0, neighbour_influx=0.0):
    target = interp(LIGHT_TARGET[group], light) * source
    rate = RISE[group] if target > cover else FALL[group]
    return min(1.0, max(0.0, cover + rate * (target - cover) + neighbour_influx))

def competition(covers, height):
    total = sum(covers[g] * STRENGTH[g] * max(0.0, min(1.0, 1 - height / OVERTOP_M[g])) for g in covers)
    return min(COMPETITION_CAP, total)

def exposure(bramble_cover, height):
    factor = min(1.0, bramble_height_m(bramble_cover) / max(0.05, height))
    return 1.0 - min(CONCEAL_CAP, bramble_cover * factor)

def vulnerability(height):
    if height <= OAK["full"]:
        return 1.0
    if height >= OAK["escape"]:
        return 0.0
    return 1 - (height - OAK["full"]) / (OAK["escape"] - OAK["full"])

# Oak's distinct survival response (SessileOak.asset) is approximated here by
# the legacy poor-light rule: 20%/yr loss below light response 0.15 [D, existing].
def oak_light_survival(light):
    return 0.8 if oak_light_response(light) < 0.15 else 1.0

def oak_juvenile(light, pressure, covers_by_year, fenced=False, years=30, h0=0.6, form="growth"):
    """Expected-value oak juvenile (cohort convention): years to promotion,
    survival, cumulative expected browse events, height at 5/10.
    form="growth": competition multiplies height growth (report's equation).
    form="light": overtopping vegetation intercepts light; the juvenile's
    effective light feeds the existing light growth AND survival responses."""
    h, survival, events, promoted, h5, h10 = h0, 1.0, 0.0, None, None, None
    for y in range(1, years + 1):
        covers = covers_by_year(y)
        exp_ = exposure(covers["bramble"], h)
        p = 0.0 if fenced else min(BROWSE_CAP, pressure * OAK["palatability"] * vulnerability(h) * exp_)
        comp = competition(covers, h)
        effective_light = light * (1 - comp) if form == "light" else light
        growth_multiplier = 1.0 if form == "light" else (1 - comp)
        potential = OAK["growth"] * oak_light_response(effective_light)
        h += potential * growth_multiplier * (1 - p * INCREMENT_LOSS)
        survival *= oak_light_survival(effective_light) * (1 - p * EXTRA_MORTALITY)
        events += p
        if y == 5: h5 = h
        if y == 10: h10 = h
        if promoted is None and h >= OAK["promotion"] and light >= 0.2:
            promoted = y
    return promoted, survival, events, h5, h10

def trajectory(group, lights, c0=0.0, source=1.0):
    c, out = c0, []
    for light in lights:
        c = step_cover(c, group, light, source)
        out.append(c)
    return out

def fmt(xs, idx):
    return " ".join(f"{xs[i - 1]:.2f}" for i in idx)

# --------------------------------------------------------------- experiments

def main():
    print("UNDERSTOREY_DIAGNOSTIC v1 (diagnostic only; candidate parameters, not production)")

    print("\n[A] Closed spruce (RLI 0.02) vs small gap (0.25, closing to 0.10 over years 10-20): cover at years 5/20/50")
    closed = [0.02] * 50
    gap = [0.25 if y <= 10 else max(0.10, 0.25 - 0.015 * (y - 10)) for y in range(1, 51)]
    for g in LIGHT_TARGET:
        print(f"  {g:9s} closed {fmt(trajectory(g, closed, 0.03), (5, 20, 50))} | gap {fmt(trajectory(g, gap, 0.03), (5, 20, 50))}"
              f" | gap, no bracken source {fmt(trajectory(g, gap, 0.0, 0.0 if g == 'bracken' else 1.0), (5, 20, 50))}")

    print("\n[E] Canopy-closure hysteresis: RLI 0.40 for 8 years, then 0.05 - years for cover to fall below half its peak")
    lights = [0.40] * 8 + [0.05] * 30
    for g in LIGHT_TARGET:
        t = trajectory(g, lights, 0.03)
        peak = t[7]
        half = next((y - 8 for y in range(9, 39) if t[y - 1] < 0.5 * peak), None)
        print(f"  {g:9s} peak {peak:.2f}  years-to-half after closure {half}  cover 10 yr after closure {t[17]:.2f}")

    print("\n[D] Bracken spread along a row of 6 cells (RLI 0.35, source only in cell 0): cover by cell at years 3/10/20")
    cells = [0.6] + [0.0] * 5
    snapshots = {}
    for y in range(1, 21):
        nxt = []
        for i, c in enumerate(cells):
            neighbours = [cells[j] for j in (i - 1, i + 1) if 0 <= j < len(cells)]
            influx = SPREAD["bracken"] * max(0.0, max(neighbours) - c) * 0.5
            # A cell without its own rhizome source can only gain via influx.
            source = 1.0 if c > 0.01 or i == 0 else 0.0
            nxt.append(step_cover(c, "bracken", 0.35, source, influx))
        cells = nxt
        if y in (3, 10, 20):
            snapshots[y] = list(cells)
    for y, row in snapshots.items():
        print(f"  year {y:2d}: " + " ".join(f"{c:.2f}" for c in row))

    print("\n[B/C] Oak juvenile (0.6 m), RLI 0.35, Browsing v1 parameters; bramble/graminoid cover from a fresh gap")
    def gap_covers(clear_every=None):
        cover = {"bramble": 0.05, "bracken": 0.0, "graminoid": 0.02}
        history = {}
        for y in range(1, 31):
            for g in cover:
                cover[g] = step_cover(cover[g], g, 0.35, 0.0 if g == "bracken" else 1.0)
            local = dict(cover)
            if clear_every and (y - 1) % clear_every == 0:
                local = {g: 0.0 for g in cover}          # spot control this season [G]
            elif clear_every:
                since = (y - 1) % clear_every         # regrowth toward cell cover [C]
                local = {g: cover[g] * min(1.0, 0.5 * since) for g in cover}
            history[y] = local
        return lambda y: history[y]
    none = lambda y: {"bramble": 0.0, "bracken": 0.0, "graminoid": 0.0}
    cases = [
        ("no vegetation (browsing-only reference)", none),
        ("uncleared vegetation", gap_covers()),
        ("spot-cleared once (year 1)", gap_covers(clear_every=99)),
        ("spot-cleared every 2 years", gap_covers(clear_every=2)),
    ]
    for form in ("growth", "light"):
        for h0, label in ((0.6, "planted 0.6 m"), (0.18, "natural seedling 0.18 m")):
            print(f"  -- competition form={form}, {label}")
            print("  case                                   pressure fenced  promoYr  survival  browseEvents  h5    h10")
            for pressure in (0.0, 0.5, 0.85):
                for name, covers in cases:
                    for fenced in (False, True):
                        if pressure == 0.0 and fenced:
                            continue
                        promo, surv, ev, h5, h10 = oak_juvenile(0.35, pressure, covers, fenced, h0=h0, form=form)
                        print(f"  {name:38s} {pressure:5.2f}   {str(fenced):5s}   {str(promo):6s}   {surv:.3f}     {ev:5.2f}     {h5:.2f}  {h10:.2f}")

    print("\n[Concealment] exposure multiplier by bramble cover and juvenile height")
    for cover in (0.0, 0.2, 0.4, 0.7, 0.9):
        print(f"  cover {cover:.1f}: " + " ".join(f"h{h:.1f}={exposure(cover, h):.2f}" for h in (0.3, 0.6, 1.0, 1.5)))

    print("\n[Competition] growth removed by height for dense covers (bramble 0.8, bracken 0, graminoid 0.6)")
    print("  " + " ".join(f"h{h:.1f}={competition({'bramble': 0.8, 'bracken': 0.0, 'graminoid': 0.6}, h):.2f}" for h in (0.2, 0.5, 0.8, 1.2, 1.6)))

def killarney_sweep():
    """Which competition strength / concealment cap reproduce the Killarney
    direction? Weeded oak browsed far more (49% vs 11% [E]) yet later did
    better because competition outweighed concealment [E]. Natural seedling
    0.18 m, light-interception form, dense persistent vegetation (bramble 0.8,
    graminoid 0.5), 25 years."""
    global CONCEAL_CAP
    base_strength = dict(STRENGTH)
    dense = lambda y: {"bramble": 0.8, "bracken": 0.0, "graminoid": 0.5}
    bare = lambda y: {"bramble": 0.0, "bracken": 0.0, "graminoid": 0.0}
    print("\n[Killarney sweep] seedling 0.18 m; ratio = year-1 browse p vegetated / weeded (Killarney 11/49 = 0.22)")
    print("  light conceal strengthx | browseRatio | fenced: weeded promo/surv  vegetated promo/surv | unfenced p=0.85: weeded promo/surv  vegetated promo/surv | sign")
    for light in (0.2, 0.35):
        for cap in (0.5, 0.78):
            CONCEAL_CAP = cap
            for scale in (1.0, 1.5, 2.0, 3.0):
                for g in STRENGTH:
                    STRENGTH[g] = base_strength[g] * scale
                ratio = exposure(0.8, 0.18)
                fw = oak_juvenile(light, 0.0, bare, True, years=25, h0=0.18, form="light")
                fv = oak_juvenile(light, 0.0, dense, True, years=25, h0=0.18, form="light")
                uw = oak_juvenile(light, 0.85, bare, False, years=25, h0=0.18, form="light")
                uv = oak_juvenile(light, 0.85, dense, False, years=25, h0=0.18, form="light")
                # Killarney direction: under browsing the weeded seedlings end up
                # surviving better despite being browsed more.
                sign = "weeded better" if uw[1] > uv[1] + 0.02 else ("vegetated better" if uv[1] > uw[1] + 0.02 else "similar")
                print(f"  {light:.2f}  {cap:.2f}   {scale:.1f}      |   {ratio:.2f}     |   {str(fw[0]):4s}/{fw[1]:.2f}             {str(fv[0]):4s}/{fv[1]:.2f}        |   {str(uw[0]):4s}/{uw[1]:.2f}             {str(uv[0]):4s}/{uv[1]:.2f}         | {sign}")
    for g in STRENGTH:
        STRENGTH[g] = base_strength[g]
    CONCEAL_CAP = 0.5

if __name__ == "__main__":
    main()
    killarney_sweep()
