"""BroadLynch2006_Sitka — isolated verification implementation.

Reproduces the Sitka spruce stand model published in:

  Broad, L.R. & Lynch, T. (2006), "Growth Models for Sitka Spruce in Ireland".

Coefficients and equation forms are taken from the supplied extraction
`GROWFOR_Technical_Evidence_Extraction_v1.md` (which cites G2 page/appendix
locators). This is a VERIFICATION-ONLY benchmark implementation. It is NOT a
CCF production growth engine and must not be wired into gameplay or treated as
"current GROWFOR": whether the distributed GROWFOR executable uses these exact
2006 coefficients is unverified (extraction section 12).

No numpy dependency: all linear algebra is small, fixed 3x3, pure Python.
"""

from __future__ import annotations

import math

# ---------------------------------------------------------------------------
# Height / site model — extraction section 5 (G2 p.12).
# Polymorphic Bertalanffy-Richards; site index S = top height at age 30.
# ---------------------------------------------------------------------------
HEIGHT_A = 47.0216        # asymptotic height parameter
HEIGHT_C = 0.4571         # Richards shape parameter
HEIGHT_T0 = -5.0          # pinned time origin
HEIGHT_H0 = 0.0           # assumed height at t0
SITE_REFERENCE_AGE = 30.0

# ---------------------------------------------------------------------------
# Growth-component coefficients — extraction section 6 (G2 Box 1 p.27, Box 2 p.28).
# Each model: P (left eigenvectors), P^-1, Lambda (diagonal eigenvalues),
# C (power transform), C^-1, a (transformed-system asymptote A^-1 b), t0.
# The bold asymptote vector `a` is NOT the scalar height asymptote HEIGHT_A.
# ---------------------------------------------------------------------------
THINNED = {
    "name": "thinned",
    "P": [[1.0, 366.9413, -24.5087],
          [6.7548e-4, 1.0, -1.1514e-2],
          [0.0, 0.0, 1.0]],
    "Lambda": [-0.367892, -9.118596e-2, -1.0],
    "Pinv": [[1.3295, -487.8635, 26.9678],
             [-8.9807e-4, 1.3295, -6.7016e-3],
             [0.0, 0.0, 1.0]],
    "C": [[0.5342, 6.9520e-2, 0.4843],
          [0.0, -0.2642, 0.0],
          [0.0, 0.0, 0.4571]],
    "Cinv": [[1.8718, 0.4926, -1.9834],
             [0.0, -3.7852, 0.0],
             [0.0, 0.0, 2.1878]],
    "a": [0.0, 0.620599, 5.812518],
    "t0": -5.0,
}

UNTHINNED = {
    "name": "unthinned",
    "P": [[1.0, 15.5501, -14.5477],
          [-6.2924e-3, 1.0, 4.3626e-2],
          [0.0, 0.0, 1.0]],
    "Lambda": [-0.500102, 4.893415e-2, -1.0],
    "Pinv": [[0.9109, -14.1642, 13.8690],
             [5.7316e-3, 0.9109, 4.3644e-2],
             [0.0, 0.0, 1.0]],
    "C": [[0.6948, -9.3764e-2, 0.4711],
          [0.0, 8.1093e-2, 0.0],
          [0.0, 0.0, 0.4571]],
    "Cinv": [[1.4393, 1.6641, -1.4835],
             [0.0, 12.3314, 0.0],
             [0.0, 0.0, 2.1878]],
    "a": [0.0, 7.068916, 5.812518],
    "t0": -5.0,
}

# ---------------------------------------------------------------------------
# Stand-volume equation — extraction section 8.2 (G2 pp.20-21).
# V = B (b0 + b1 H + b2 H/sqrt(N) + b3 N H / B + b6 B / H)
# ---------------------------------------------------------------------------
VOLUME_BETA0 = 1.0013
VOLUME_BETA1 = 0.5295
VOLUME_BETA2 = -1.6483
VOLUME_BETA3 = -0.0010
VOLUME_BETA6 = -0.2384

# ---------------------------------------------------------------------------
# Thinning-transition coefficients — extraction section 9.3 (G2 p.21).
# Integrated form (extraction section 9.2):
#   ln B = -(1/b) ln[ B0^-b - (a b / c) H^d (N^c - N0^c) ]
# NOTE: a,b,c,d here are the thinning coefficients, not growth-model objects.
#
# SOURCE FIDELITY: the PRINTSOURCE coefficients are c = -0.2877 and d = -0.5585
# (both negative). The signs matter: with c > 0 the term (N^c - N0^c) changes
# sign and the transition returns a nonphysical near-zero basal area. With the
# printed negative signs the published reference case (B0=30 m2/ha, N0=1800
# stems/ha, H=16 m, N=1200 stems/ha) gives B ~= 23.7697 m2/ha.
#
# Term-by-term check of (a b / c) H^d (N^c - N0^c) for B0=30,N0=1800,H=16,N=1200:
#   a = 10.0991, b = 0.2371  -> a*b = 2.39449661
#   a*b/c = 2.39449661 / -0.2877 = -8.32289402 (negative)
#   H^d = 16^-0.5585 = 0.21256854 (positive)
#   N^c = 1200^-0.2877 = 0.13005275, N0^c = 1800^-0.2877 = 0.11573324
#   N^c - N0^c = 0.01431952 (positive because c < 0 and N < N0)
#   product = (-8.32289402)(0.21256854)(0.01431952) = -0.02533388
#   B0^-b = 30^-0.2371 = 0.44645177
#   inner = 0.44645177 - (-0.02533388) = 0.47178565
#   ln B = -(1/0.2371) ln(0.47178565) -> B ~= 23.7697 m2/ha.
# ---------------------------------------------------------------------------
THIN_A = 10.0991
THIN_B = 0.2371
THIN_C = -0.2877
THIN_D = -0.5585

# ---------------------------------------------------------------------------
# Assortment UNML coefficients — extraction sections 10.3-10.4 (G2 pp.22,29).
# Each entry: (predictor-name, coefficient). Predictors that need a post-thin
# state (D1,B1,N1) are thinning-only; for a crop (no-thin) assortment they are
# dropped (contribute 0). See assortment() and the module docstring caveat.
# ---------------------------------------------------------------------------
ASSORT_CAT1 = [   # proportion with SED >= 20 cm
    ("intercept", 27.1307),
    ("D0", -0.6992),
    ("1/(D0/100)", -3.2105),
    ("D0^2/100", 0.8237),
    ("D1-D0", -3.8369),
    ("(D1-D0)^2", 0.4853),
    ("(D1-D0)/(H/10)", 3.3854),
    ("1/((D1-D0)H)", -4.8838),
    ("H/(100(D1-D0)^2)", 0.1969),
    ("B0-B1", 0.3571),
    ("(N0-N1)/1000", -10.7710),
]
ASSORT_CAT2 = [   # proportion with 14 <= SED < 20 cm
    ("intercept", -23.4200),
    ("D0", 0.6734),
    ("1/(D0/100)", 2.1322),
    ("D0^2/100", -0.1435),
    ("D0/H", 11.9817),
    ("1/((D0/100)(H/10))", -1.6478),
    ("D0^2/H", -0.3680),
    ("D1-D0", -0.5903),
    ("(D1-D0)^2", 0.1036),
    ("B0-B1", 0.0961),
    ("(N0-N1)/1000", -1.8719),
]
THINNING_ONLY_PREDICTORS = {"D1-D0", "(D1-D0)^2", "(D1-D0)/(H/10)", "1/((D1-D0)H)",
                            "H/(100(D1-D0)^2)", "B0-B1", "(N0-N1)/1000"}


# ---------------------------------------------------------------------------
# Small fixed-size linear algebra (pure Python).
# ---------------------------------------------------------------------------
def mat3_vec3(m, v):
    return [sum(m[i][j] * v[j] for j in range(3)) for i in range(3)]


def mat3_mul(a, b):
    return [[sum(a[i][k] * b[k][j] for k in range(3)) for j in range(3)] for i in range(3)]


def mat3_inverse(m):
    (a, b, c), (d, e, f), (g, h, i) = m
    det = a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g)
    if det == 0.0:
        raise ValueError("singular 3x3 matrix")
    inv_det = 1.0 / det
    return [
        [(e * i - f * h) * inv_det, (c * h - b * i) * inv_det, (b * f - c * e) * inv_det],
        [(f * g - d * i) * inv_det, (a * i - c * g) * inv_det, (c * d - a * f) * inv_det],
        [(d * h - e * g) * inv_det, (b * g - a * h) * inv_det, (a * e - b * d) * inv_det],
    ]


def _power(x, C):
    """x^C = exp(C ln x), element-by-element (extraction section 4.3)."""
    lx = [math.log(v) for v in x]
    return [math.exp(sum(C[i][j] * lx[j] for j in range(3))) for i in range(3)]


# ---------------------------------------------------------------------------
# Height / site.
# ---------------------------------------------------------------------------
def b_from_site_index(site_index, a=HEIGHT_A, c=HEIGHT_C, t0=HEIGHT_T0, h0=HEIGHT_H0,
                      reference_age=SITE_REFERENCE_AGE):
    """Site-scaling b from a site index (top height at reference age) — extraction 4.6."""
    return -(1.0 / (reference_age - t0)) * math.log((a ** c - site_index ** c) / (a ** c - h0 ** c))


def height(age, site_index, a=HEIGHT_A, c=HEIGHT_C, t0=HEIGHT_T0, h0=HEIGHT_H0):
    """Top height at `age` for a given site index (extraction 4.5), polymorphic form."""
    b = b_from_site_index(site_index, a, c, t0, h0)
    return a * (1.0 - (1.0 - (h0 / a) ** c) * math.exp(-b * (age - t0))) ** (1.0 / c)


def site_index_from_height(age, top_height, a=HEIGHT_A, c=HEIGHT_C, t0=HEIGHT_T0, h0=HEIGHT_H0,
                           reference_age=SITE_REFERENCE_AGE):
    """Infer site index from an observed (age, top height) pair (inverts extraction 4.5)."""
    b = -math.log(1.0 - (top_height / a) ** c) / (age - t0)
    return a * (1.0 - (1.0 - (h0 / a) ** c) * math.exp(-b * (reference_age - t0))) ** (1.0 / c)


# ---------------------------------------------------------------------------
# Stand-state transition between thinnings — extraction section 4.8.
# x(t2) = { a + P^-1 exp[Lambda b (t2-t1)] P [ x(t1)^C - a ] }^(C^-1)
# ---------------------------------------------------------------------------
def transition(x, model, b, dt_years):
    y = _power(x, model["C"])
    z = mat3_vec3(model["P"], [y[i] - model["a"][i] for i in range(3)])
    z = [math.exp(model["Lambda"][i] * b * dt_years) * z[i] for i in range(3)]
    y2 = [model["a"][i] + mat3_vec3(model["Pinv"], z)[i] for i in range(3)]
    return _power(y2, model["Cinv"])


def project(x0, site_index, years, model=UNTHINNED, dt=1.0, thinning_years=None):
    """Project a stand state year by year for a given site index.

    Returns a list of (year, B, N, H, V, Dq) starting at year 0 = x0.
    `thinning_years` is an optional mapping {year: N_after} applying the published
    thinning transition (basal-area/stocking) at that year, H held fixed.
    """
    b = b_from_site_index(site_index)
    B, N, H = x0
    rows = [(0, B, N, H, volume(B, N, H), qmd(B, N))]
    for year in range(1, years + 1):
        B, N, H = transition([B, N, H], model, b, dt)
        if thinning_years and year in thinning_years:
            B = thin_basal_area(B, N, H, thinning_years[year])
            N = thinning_years[year]
        rows.append((year, B, N, H, volume(B, N, H), qmd(B, N)))
    return rows


# ---------------------------------------------------------------------------
# Stand volume — extraction section 8.
# ---------------------------------------------------------------------------
def volume(B, N, H):
    if B <= 0 or N <= 0 or H <= 0:
        return 0.0
    return B * (VOLUME_BETA0 + VOLUME_BETA1 * H + VOLUME_BETA2 * H / math.sqrt(N)
                + VOLUME_BETA3 * N * H / B + VOLUME_BETA6 * B / H)


def qmd(B, N):
    """Quadratic mean diameter, cm, from basal area (m2/ha) and stocking (stems/ha)."""
    if B <= 0 or N <= 0:
        return 0.0
    return math.sqrt((B / N) * (40000.0 / math.pi))


# ---------------------------------------------------------------------------
# Thinning transition — extraction section 9.2.
# ln B = -(1/b) ln[ B0^-b - (a b / c) H^d (N^c - N0^c) ]
# ---------------------------------------------------------------------------
def thin_basal_area(B0, N0, H, N_after):
    inner = B0 ** (-THIN_B) - (THIN_A * THIN_B / THIN_C) * (H ** THIN_D) * (N_after ** THIN_C - N0 ** THIN_C)
    if inner <= 0:
        raise ValueError("thinning transition domain error: inner <= 0")
    return math.exp(-(1.0 / THIN_B) * math.log(inner))


# ---------------------------------------------------------------------------
# Assortment (UNML) — extraction section 10.
# ---------------------------------------------------------------------------
def _predictor(name, D0, D1, B0, B1, N0, N1, H):
    # Lazy dispatch: unlike a dict literal, only the requested predictor is evaluated,
    # so a crop assortment never divides by (D1-D0) == 0.
    if name == "intercept":
        return 1.0
    if name == "D0":
        return D0
    if name == "1/(D0/100)":
        return 100.0 / D0
    if name == "D0^2/100":
        return D0 * D0 / 100.0
    if name == "D0/H":
        return D0 / H
    if name == "1/((D0/100)(H/10))":
        return 100.0 / (D0 * H / 10.0)
    if name == "D0^2/H":
        return D0 * D0 / H
    if name == "D1-D0":
        return D1 - D0
    if name == "(D1-D0)^2":
        return (D1 - D0) ** 2
    if name == "(D1-D0)/(H/10)":
        return (D1 - D0) / (H / 10.0)
    if name == "1/((D1-D0)H)":
        return 1.0 / ((D1 - D0) * H)
    if name == "H/(100(D1-D0)^2)":
        return H / (100.0 * (D1 - D0) ** 2)
    if name == "B0-B1":
        return B0 - B1
    if name == "(N0-N1)/1000":
        return (N0 - N1) / 1000.0
    raise KeyError("unknown assortment predictor: " + name)


def _eta(coefficients, D0, D1, B0, B1, N0, N1, H, thinning):
    total = 0.0
    for name, coef in coefficients:
        if not thinning and name in THINNING_ONLY_PREDICTORS:
            continue  # post-thin predictors are thinning-only (extraction 10.1)
        total += coef * _predictor(name, D0, D1, B0, B1, N0, N1, H)
    return total


def assortment(B0, N0, H, B1=None, N1=None, thinning=None):
    """UNML assortment proportions (p1 >=20cm, p2 14-20cm, p3 7-14cm by difference).

    D0/D1 are quadratic mean diameters before/after thinning. When `thinning` is
    omitted it is inferred from whether post-thin B1/N1 are supplied. For a crop
    (no-thin) assortment, thinning-only predictors are dropped (source gap: the
    extraction does not print the exact crop predictor set — see README).
    """
    if not (B0 > 0.0 and N0 > 0.0 and H > 0.0):
        raise ValueError("assortment domain error: B0, N0 and H must all be positive")
    B1 = B0 if B1 is None else B1
    N1 = N0 if N1 is None else N1
    thinning = (B1 != B0 or N1 != N0) if thinning is None else thinning
    if thinning and not (B1 > 0.0 and N1 > 0.0):
        raise ValueError("assortment domain error: a thinning assortment requires positive B1 and N1")
    D0 = qmd(B0, N0)
    D1 = qmd(B1, N1)
    if thinning and D1 == D0:
        raise ValueError("thinning assortment is undefined when D1 == D0 "
                         "(the published predictors divide by (D1-D0))")
    eta1 = _eta(ASSORT_CAT1, D0, D1, B0, B1, N0, N1, H, thinning)
    eta2 = _eta(ASSORT_CAT2, D0, D1, B0, B1, N0, N1, H, thinning)
    e1, e2 = math.exp(eta1), math.exp(eta2)
    denom = 1.0 + e1 + e2
    p1, p2 = e1 / denom, e2 / denom
    return {"p_ge20": p1, "p_14_20": p2, "p_7_14": 1.0 - p1 - p2}
