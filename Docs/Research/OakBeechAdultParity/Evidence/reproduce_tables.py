#!/usr/bin/env python3
"""Reproduces every number quoted in OakBeechAdultGrowthSiteSpecification_v1.md.

Stdlib only.  Run as:  python3 -I reproduce_tables.py [--csv expected_trajectories.csv]

This is research arithmetic, not production code.  It mirrors the *form* of
ForestEcologyController.GrowAdults (Assets/ForestPrototype/ForestEcologyController.cs,
lines 993-1024 at c1564b8) in double precision.  Production uses float32, so a
Unity harness should compare against these values with a small tolerance
(suggested: 1e-3 relative, or 0.05 cm / 0.05 m absolute).

PARAMETER STATUS: the PROPOSED rows below are PROVISIONAL [C] values chosen inside
transferred bands that could not be verified in the research session (see
spec section 9 and Appendix A).  After evidence reconciliation (spec step 0),
edit ONLY the PARAMS block and re-run to regenerate every table and the CSV.
"""
import math
import sys
from math import exp, sqrt, pi

# ----------------------------------------------------------------- parameters
# Current production law for non-Sitka species (Oak and Beech assets are identical).
CURRENT = {
    "oak":   dict(Pd=0.40, Dm=120.0, Ph=0.40, Hm=40.0, ci50=3.0),
    "beech": dict(Pd=0.40, Dm=120.0, Ph=0.40, Hm=40.0, ci50=3.0),
}
# PROVISIONAL proposed adult-growth rows (versioned, code-held, species-keyed).
PROPOSED = {
    "oak":   dict(Pd=0.65, Dm=120.0, Ph=0.35, Hm=33.0, ci50=3.0),
    "beech": dict(Pd=0.65, Dm=120.0, Ph=0.50, Hm=33.0, ci50=3.0),
}
# Ordinal response classes [C] and per-species soil-moisture tables [ORD->C].
CLASS = {"O": 1.0, "G": 0.8, "M": 0.6, "P": 0.4, "V": 0.2}
MOISTURE = {                      # index 0..4 = moisture class 1..5
    "oak":   ["M", "G", "O", "M", "P"],
    "beech": ["P", "G", "O", "P", "V"],
}
REFERENCE_MOISTURE_CLASS = 3      # Scenario One reference site [I] (Manager decision OD-1)

# Promotion state used by Scenario One planting (ForestEcologyController.cs:31-33 and
# JuvenileEcologyRules.cs:37): planted stock age 3, height 0.6 m; juvenile height
# growth 0.35 m/yr x light response (1.0 in good light); promotion at >= 3.5 m;
# DBH at promotion = clamp(1.5 x height, 2, 20) cm.
def promotion_state():
    h, n = 0.6, 0
    while h < 3.5:
        h += 0.35
        n += 1
    return dict(age=3 + n, height=h, dbh=min(20.0, max(2.0, 1.5 * h)), years_to_promotion=n)

PROMO = promotion_state()

# ------------------------------------------------------------------ the kernel
def trajectory(p, ci=0.0, site=1.0, start=None, years=160):
    """GrowAdults for a non-Sitka tree: state-based, age-independent.
    dD = Pd*site*clamp01(1-D/Dm)/(1+CI/ci50);  dH = Ph*site*clamp01(1-H/Hm)."""
    s = start or PROMO
    d, h, a = s["dbh"], s["height"], s["age"]
    out = {}
    for _ in range(years + 1):
        out[a] = (d, h)
        dg = p["Pd"] * site * max(0.0, min(1.0, 1 - d / p["Dm"])) / (1 + ci / p["ci50"])
        hg = p["Ph"] * site * max(0.0, min(1.0, 1 - h / p["Hm"]))
        d = min(200.0, max(1.0, d + dg))     # ForestTree.ApplyGrowth clamps
        h = min(60.0, max(0.1, h + hg))
        a += 1
    return out

def first_age(t, idx, target):
    for a in sorted(t):
        if t[a][idx] >= target:
            return a
    return None

def moisture_response(species, cls):
    return CLASS[MOISTURE[species][cls - 1]]

def site_factor(species, cls, ref=REFERENCE_MOISTURE_CLASS):
    """Normalised to the reference class so the reference site is exactly 1.0."""
    return moisture_response(species, cls) / moisture_response(species, ref)

def fit_height(h0, a0, a1, h1, a2, h2):
    """Fit Hm,k of H(t)=Hm-(Hm-h0)exp(-k(t-a0)) through (a1,h1),(a2,h2); returns Hm, Ph=k*Hm."""
    best = None
    for tenth in range(int(max(h1, h2) * 10) + 5, 800):
        hm = tenth / 10
        try:
            k = -math.log((hm - h1) / (hm - h0)) / (a1 - a0)
        except ValueError:
            continue
        err = abs(hm - (hm - h0) * exp(-k * (a2 - a0)) - h2)
        if best is None or err < best[0]:
            best = (err, hm, k)
    err, hm, k = best
    return hm, k * hm, err

# -------------------------------------------------------------------- reporting
def section(title):
    print("\n" + "=" * 78 + "\n" + title + "\n" + "=" * 78)

def main():
    section("T0. Promotion state used for all trajectories (Scenario One planted stock)")
    print(f"planted age 3, 0.6 m; first >=3.5 m after {PROMO['years_to_promotion']} juvenile years "
          f"-> age {PROMO['age']}, height {PROMO['height']:.2f} m, DBH {PROMO['dbh']:.3f} cm")

    section("T1. Reproduction of the reported 'DBH <= ~37.8 cm at age 112' (current law, CI=0, site=1)")
    k = CURRENT["oak"]["Pd"] / CURRENT["oak"]["Dm"]
    for d0, label in ((5.25, "promotion exactly at 3.5 m (DBH 5.25)"), (PROMO["dbh"], "planted-stock promotion path (DBH 5.625)")):
        d100 = 120 - (120 - d0) * exp(-k * 100)
        print(f"closed form D(100 adult years) = 120-(120-{d0:.3f})exp(-{k:.6f}*100) = {d100:.2f} cm   [{label}]")
    cur = trajectory(CURRENT["oak"])
    print("numeric (float-free recurrence):", {a: round(cur[a][0], 1) for a in (12, 27, 52, 77, 102, 112, 127, 152)})
    print("current law DBH increment at 5/20/38 cm, CI=0:", [round(0.4 * (1 - d / 120), 3) for d in (5, 20, 38)])
    print("size brake (1-D/120) at 38 cm =", round(1 - 38 / 120, 3))

    section("T2. Current vs PROPOSED single-tree trajectories at Scenario One Years 25/50/75/100 "
            "(planted Year 1: age 27/52/77/102)")
    ages = (27, 52, 77, 102, 127, 152)
    for label, params in (("CURRENT (Oak = Beech)", CURRENT), ("PROPOSED", PROPOSED)):
        for sp in ("oak", "beech"):
            if label.startswith("CURRENT") and sp == "beech":
                continue
            p = params[sp]
            print(f"\n-- {label} {sp}: {p}")
            for ci in (0, 1, 2, 3, 5, 10):
                t = trajectory(p, ci=ci)
                row = "  ".join(f"{a}:{t[a][0]:5.1f}/{t[a][1]:4.1f}" for a in ages)
                print(f"   CI={ci:>2}  age:DBH/H  {row}   | first age >=40cm:{first_age(t,0,40)}  >=47cm:{first_age(t,0,47)}  >=53cm:{first_age(t,0,53)}")

    section("T3. Managed-stand increment cross-check (repo stand/young range 0.3-0.5 cm/yr, BeechSpeciesParameters.md)")
    for label, params in (("CURRENT", CURRENT), ("PROPOSED", PROPOSED)):
        p = params["oak"]
        print(label, "DBH 20 cm increment by CI:", {ci: round(p["Pd"] * (1 - 20 / p["Dm"]) / (1 + ci / p["ci50"]), 3) for ci in (1, 1.5, 2, 3)})
    print("Expected Hegyi CI of a Poisson stand with equal-sized neighbours, cutoff 8 m: CI = 16*pi*lambda")
    for n in (150, 300, 450, 600, 1000, 2100):
        print(f"   {n:>4} stems/ha -> CI ~ {16 * pi * n / 10000:.1f}")

    section("T4. DBH-potential sensitivity (Pd), height held at Oak PROPOSED")
    for pd in (0.40, 0.50, 0.65, 0.80):
        cells = []
        for ci in (0, 1, 2, 3):
            t = trajectory(dict(Pd=pd, Dm=120.0, Ph=0.35, Hm=33.0, ci50=3.0), ci=ci)
            cells.append(f"CI{ci}: D102={t[102][0]:.0f} a47={first_age(t,0,47)}")
        print(f"Pd={pd:.2f}: " + " | ".join(cells))

    section("T5. Height pair (Ph, Hm) refits to alternative top-height anchors (age 50 / age 100); promotion 3.75 m at age 12")
    cases = {
        "Oak low   (H50 11.0, H100 18.0)": (11.0, 18.0), "Oak mid   (H50 13.5, H100 21.5)": (13.5, 21.5), "Oak high  (H50 16.0, H100 25.0)": (16.0, 25.0),
        "Beech low (H50 14.0, H100 22.0)": (14.0, 22.0), "Beech mid (H50 16.5, H100 25.5)": (16.5, 25.5), "Beech high(H50 19.0, H100 29.0)": (19.0, 29.0),
    }
    for name, (h50, h100) in cases.items():
        hm, ph, err = fit_height(PROMO["height"], PROMO["age"], 50, h50, 100, h100)
        print(f"{name}: Hm={hm:.1f}  Ph={ph:.2f}  (fit error {err:.2f} m)")
    print("\nPROPOSED height trajectories (no CI term in height):")
    for sp in ("oak", "beech"):
        for lab, params in (("current", CURRENT), ("proposed", PROPOSED)):
            t = trajectory(params[sp])
            print(f"   {sp:5s} {lab:8s} H at age 25/50/75/100/125/150:", [round(t[a][1], 1) for a in (25, 50, 75, 100, 125, 150)])

    section("T6. Mid-log diameter vs DBH under the TimberYield v1 paraboloid  d^2(z)=D^2 (H-z)/(H-1.3), stump 0.35 m")
    for H in (20, 25, 28):
        for L in (3.0, 4.9, 6.1):
            zmid = 0.35 + L / 2
            r = sqrt((H - zmid) / (H - 1.3))
            print(f"   H={H} m, {L} m log: mid/DBH={r:.3f}; DBH for 45 cm mid-log: over-bark basis {45/r:.1f} cm; "
                  f"if 45 cm is under-bark and bark ~8% of diameter [I] {45/r/0.92:.1f} cm")

    section("T7. Soil-moisture response (PROPOSED tables, normalised to reference class 3); trajectories at CI=0 and CI=2")
    print("response classes [C]:", CLASS)
    for sp in ("oak", "beech"):
        print(f"{sp} table (classes 1..5):", MOISTURE[sp], "->", [moisture_response(sp, c) for c in range(1, 6)])
    for sp in ("oak", "beech"):
        for cls in range(1, 6):
            s = site_factor(sp, cls)
            for ci in (0, 2):
                t = trajectory(PROPOSED[sp], ci=ci, site=s)
                print(f"   {sp:5s} class {cls} (x{s:.1f}) CI={ci}: age52 D/H={t[52][0]:.0f}/{t[52][1]:.0f}   age102 D/H={t[102][0]:.0f}/{t[102][1]:.0f}   age152 D/H={t[152][0]:.0f}/{t[152][1]:.0f}")

    section("T8. Boundary behaviour (PROPOSED): no negative increments, bounded, no jumps, age-independence")
    for sp in ("oak", "beech"):
        for s in (0.2, 1.0):
            t = trajectory(PROPOSED[sp], ci=0, site=s, years=500)
            a = sorted(t)
            dd = [t[x + 1][0] - t[x][0] for x in a[:-1]]
            dh = [t[x + 1][1] - t[x][1] for x in a[:-1]]
            print(f"   {sp:5s} site={s}: min dD={min(dd):.4f} min dH={min(dh):.4f} max dD={max(dd):.3f} max dH={max(dh):.3f}; "
                  f"age 512 D={t[512][0]:.1f} H={t[512][1]:.2f} (soft maxima 120/33)")
    print("   age-independence: kernel reads only (D, H, CI, site); the recorded age never enters (test: perturb age +/-10, results identical).")

    section("T9. Promotion hand-off discontinuity in height increment (juvenile light response -> adult law, PROPOSED)")
    print("   Oak juvenile increment = 0.35 x LightResponse(light); Beech juvenile = 0.35 x LightResponse(light); asset curves")
    oak_curve = ([0, .01, .05, .10, .15, .20, .30, .40, .50, 1.0], [0, 0, .02, .07, .15, .30, .60, .80, 1.0, 1.0])
    beech_curve = ([0, .02, .05, .10, .20, .35, 1.0], [0, 0, .15, .35, .65, 1.0, 1.0])
    def interp(light, xs, ys):
        for i in range(1, len(xs)):
            if light <= xs[i]:
                f = (light - xs[i - 1]) / (xs[i] - xs[i - 1])
                return ys[i - 1] + f * (ys[i] - ys[i - 1])
        return ys[-1]
    for sp, curve in (("oak", oak_curve), ("beech", beech_curve)):
        p = PROPOSED[sp]
        adult = p["Ph"] * (1 - PROMO["height"] / p["Hm"])
        for light in ((0.2, 0.3, 0.5) if sp == "oak" else (0.1, 0.2, 0.35)):
            juv = 0.35 * interp(light, *curve)
            print(f"   {sp:5s} light {light:.2f}: last juvenile increment {juv:.3f} m/yr -> first adult increment {adult:.3f} m/yr (x{adult/juv:.1f})")

    if "--csv" in sys.argv:
        path = sys.argv[sys.argv.index("--csv") + 1]
        with open(path, "w", newline="\n") as f:
            f.write("species,moisture_class,ci,age,dbh_cm,height_m\n")
            for sp in ("oak", "beech"):
                for cls in (3, 5):
                    for ci in (0, 2, 5):
                        t = trajectory(PROPOSED[sp], ci=ci, site=site_factor(sp, cls))
                        for age in sorted(t):
                            f.write(f"{sp},{cls},{ci},{age},{t[age][0]:.4f},{t[age][1]:.4f}\n")
        print(f"\nwrote {path}")

if __name__ == "__main__":
    main()
