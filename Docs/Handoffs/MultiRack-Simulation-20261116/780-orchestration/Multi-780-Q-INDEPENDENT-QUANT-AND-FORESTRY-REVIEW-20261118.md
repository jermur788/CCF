# Multi #780 Q — independent nonauthor quantitative and forestry review of A1 and A2

**Status: RESEARCH-QUALITY REVIEW ONLY. NOT implementation acceptance. NOT calibration approval. NOT spec approval.**
This verdict accepts no coefficient, approves no Year2 continuation, and authorises no source access, fixture read, engine/.NET/Unity run or Ops/Host action. Default **L4 FAIL38 remains UNWAIVED**.

| Field | Value |
| --- | --- |
| Task | [#780](https://github.com/jermur788/CCF/issues/780), role **Q** (independent nonauthor reviewer), dispatched by a DeepSeek V4.1 Flash parent |
| Reviewer | Claude Opus 5.5, model id `claude-opus-5-5`; provider Claude B (Pro) via T3 Code; harness Claude Code (Agent SDK); worked alone, no subagents |
| Host / time | `jer-laptop`, Linux `7.0.0-42-generic` x86_64; registration observed 2026-11-18T20:28:09Z |
| Registration | [RUNNING comment, posted before any file write](https://github.com/jermur788/CCF/issues/780#issuecomment-6990683286) |
| Base | `origin/main` = `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` (ls-remote at 20:28Z and again at 20:33Z; unchanged) |
| Branch / single file | `docs/multi-780-q-opus-review-20261118-v1` / this one added Markdown, published through the GitHub API |
| Scratch | `/tmp/opencode/780-q` only |

## 0. Independence statement

This is a fresh session that did not write either deliverable. **A1** was written by OpenAI `gpt-6.1-sol` (Codex), a different model family from the reviewer. **A2** was written by Anthropic `claude-sonnet-5-5`. The reviewer is a different model (`claude-opus-5-5`) but is **in the same Anthropic family as A2**. Independence from A2 is therefore model-level and session-level only, **not cross-family**. A parent wanting cross-family coverage of A2 should route it to a non-Anthropic reviewer.

All recomputation below comes from the reviewer's own stdlib Python scripts (§9). They were written from A1 §2's stated numeric contract, the quoted #729/#737 rule text, and the R1/R2/R6 formulas. They are **not** copies of either author's Appendix B. A1's Appendix B was opened only after my numbers were produced, to locate one ordering discrepancy (A1-N1).

## 1. Exact reviewed objects and readback

| Object | Commit | Path | Bytes | SHA-256 (verified by Q) | Compare with base |
| --- | --- | --- | --- | --- | --- |
| A1 | `892d9be765820f599bb4bdb3e489fc7e23b9712a` | `Docs/Handoffs/MultiRack-Simulation-20261116/780-orchestration/Multi-780-A1-YEAR2-10-NUMERICAL-TEST-ORACLE-RESULT-20261118.md` | 85,144 ✓ | `2849ee8e25c2f7c00a14753176b9f0c3153f19c9fcc17d0f71dd4d7e46b5112a` ✓ | ahead 1, behind 0, exactly this one file |
| A2 | `7f58bb5c9ab2f85cf00a0d0882562a955727f1fd` | `Docs/Handoffs/MultiRack-Simulation-20261116/780-orchestration/Multi-780-A2-IRISH-SITKA-GROWTH-DENSITY-EVIDENCE-RESULT-20261118.md` | 54,283 ✓ | `bd700db793cad5e9c214f3d1ee0a9e61f8d75eda209a52775b29805952e5daeb` ✓ | ahead 1, behind 0, exactly this one file |

The pinned commits were read through GitHub Contents (`?ref=<commit>`), not through branch tips.

Supporting published Markdown, retrieved at the commits both authors cite, used only to check what they quote:

| Doc | Commit | Bytes | SHA-256 (Q readback) | Matches author-cited hash |
| --- | --- | --- | --- | --- |
| #737 draft (R9) | `da37b23b…` | 44,375 | `f9e64dd3b2d23fa4963858f74d63295b0ce2b5480bada364a7e9f083b9b48ebc` | A1 ✓, A2 ✓ |
| #760 draft (R10) | `149b4c80…` | 39,363 | `3952c9a50a3923a64fe39b66c7b624b33fe9da0f97dd3e938160483160776b8f` | A1 ✓, A2 ✓ |
| #729 review | `8310a380…` | 16,996 | `caef9c5c90d87d658d14bd24de7d6dbfff78d4f1c725a9a53bc2ce62cbc8593e` | A1 ✓ |

The local durable research copies were read read-only under `/media/jer/Files/CCF-Primary/Docs/Research/…`. That checkout's HEAD is `46247486`, not `a69b3e2e`, so for each of A2's eight sources I compared `git hash-object <file>` with `git rev-parse a69b3e2e:<path>`. **All eight blobs are identical**, and each equals the blob ID printed in A2 §0.1. R1's SHA-256 `2704b0aa…f29f73b` and size of 35,555 B also match A2. `CCF_Ecology_and_Regeneration_Evidence_Composite.md` is absent from the `a69b3e2e` tree, which confirms A2 §0.2's MISSING statement.

## 2. A1 — arithmetic reproducibility

My model: `f(x)=struct.unpack('>f',struct.pack('>f',x))`, rounded immediately after every f32 operation as A1 §2 declares. Hegyi CI is accumulated in f32 in ordinal neighbour-ID order. A neighbour is skipped only when `dist > f(8)`. The CI term is `f(f(Dj/max(1,Dt))/max(0.5,dist))`. DBH growth uses the DIV and RECIP profiles. Stand metrics use `Nha=f(N/area32)`, a binary64 ΣD², binary64 Dq/RD, `BA=(ΣD²·π)/40000`, and `BAha=BA/double(area32)`. S, V and p are binary64 from `q=f(CI/f(5))`.

| Vector | Quantity / operation | Q value | A1 value | Result |
| --- | --- | --- | --- | --- |
| V-01 | CI(L), CI(T) | `0x3e4ccccd`, `0x3ea00000` (0.3125) | same | **MATCH** |
| V-01 | post DBH L / T (DIV and RECIP) | `0x41cf4ec5` 25.9134616851806640625 / `0x41a78788` 20.9411773681640625 | same | **MATCH** |
| V-01 | conditional r=1 crown T / L | `0x40052ffc` / `0x4013b641` | same | **MATCH** |
| V-01 | Before BA, Dq, RD | 0.08050331174823845, 22.638462845343543, 0.0008724732341742041 | same | **MATCH** |
| V-01 | PostGrowth BA, Dq, RD; P | 0.08718236962276603, 23.5588667604602, 0.0009472342760715116; P=0 | same | **MATCH** |
| V-01 | S(L), S(T), meanV | 0.03846153763492077, 0.05882352941176472, 7.080667631107775e-06 | same | **MATCH** |
| V-02 | A-state (D dead at x=1) CI(L), CI(T) | `0x3e4ccccd`, `0x3ea00000`, identical to V-01 | "identical to V-01" | **MATCH** (A/B equality holds) |
| V-02 | wrong-inclusion discriminators T / L | `0x40340000` 2.8125 / `0x3f5dddde` 0.86666667461395263671875 | same | **MATCH** |
| V-02 | D's own BA excluded | 2500π/40000 = 0.19634954084936207 m² not in stand BA | same claim | **MATCH** |
| V-04a | Eover.x = next f32 after 8 | `0x41000001` | same | **MATCH** |
| V-04a | E8 included (dist 8.0, term 0.125); Eover excluded; Fquarter 0.25→0.5 floor, term 2; CI, count | CI `0x40080000` = 2.125, count 2 | same | **MATCH** (also equals #729 §6 row `:123`) |
| V-04b | ordinal A,B,C running sums | 20 → `0x41a00ccd` → **`0x41a02667`** 20.0187511444091796875 | same | **MATCH** |
| V-04b | permuted B,C,A | **`0x41a02666`** 20.018749237060546875 | same | **MATCH** |
| V-04b | all 6 orders (extra) | ABC/ACB/BAC/CAB → `…67`; BCA/CBA → `…66` | not stated | extra check, consistent |
| V-04b | double accumulation then f32 (wrong path) | 20.018750000279397 → `0x41a02666` | "double accumulation" listed as wrong | consistent: the wrong path is discriminated |
| V-05a | area32, D(T), D(L), CI(T) | `3dcccccd`, `41a0cccd`, `41ca6666`, `3ea11d4b` | same | **MATCH** |
| V-05a | Nha f32 / binary64 N/double(area32) | 20 `0x41a00000` / 19.99999970197678 | same | **MATCH** |
| V-05a | RD correct / RD with f64 Nha / difference | 0.00889248020590604 / 0.00889248007339776 / 1.3250828e-10 | same (≈1.32508e-10) | **MATCH** |
| V-05a | ΣD², BA, BAha, Dq | 1044.0999767303474, 0.08200342041273333, 0.8200341919078716, 22.848413257055153 | same | **MATCH** |
| V-05a | BAha with decimal 0.1 (discriminator) | 0.8200342041273332 | same | **MATCH** (Nha coincidence confirmed) |
| V-05b | L.DBH; CI(T) | `0x421acccd`; `0x3f77ae15` | same | **MATCH** |
| V-05b | post DBH DIV = RECIP / all-f64-then-final-f32 | `0x412ebf1e` / `0x412ebf1d` | same | **MATCH** |
| V-06 | area, CI, post DBH, Nha, RD, P, S, V, p | `3abb3ee7`, 0.25, `41cf3cf4`, 1400, 0.8064975705849358, 0.5162439264623394, 0.04761904829483721, 5.141890759335496e-06, **0.021320623328740265** (`0x1.5d512cfb58757p-6`) | same | **MATCH** |
| V-06 | bracket 357700/2²⁴ < p < 357701/2²⁴ | 0.021320581436157227 < p < 0.021320641040802002 | same | **MATCH** (arithmetic). The numerator→roll mapping is **NOT INDEPENDENTLY CHECKED** |
| V-07a | area, Nha, RD, `RD>1`, P | `3b0c5584`, 1868 `0x44e98000`, **exactly 1.0**, false, 1.0 | same | **MATCH**: no boundary victim |
| V-07b | area − 1 ULP, Nha, RD, `RD>1` | `3b0c5583`, 1868.000244140625 `0x44e98002` (2 ULPs at 1868, ULP 2⁻¹³), 1.000000130696266, true | same | **MATCH** |
| V-07b | after removing A: N3 Nha, RD | 1401.0001220703125 `0x44af2001`, 0.750000065348133 → stop | same | **MATCH** |
| V-07c | RD at N=4/3/2/1 | 1.873661670235546 / 1.4052462526766596 / 0.936830835117773 / 0.4684154175588865 | same | **MATCH**: remove A, B; stop at 2 |
| V-07d/V-08 | singleton Nha, RD, P | 999.99993896484375 `0x4479ffff`, 39.05689775126388, 96.1422443781597 | same | **MATCH** |
| V-12 row 0→1 | CI(L), CI(T), DBH(L), DBH(T), RD | `3e4ccccd`, `3ea00000`, `41cf4ec5`, `41a78788`, `0x1.f09fa1e6261c5p-11` | same | **MATCH** |
| V-12 row 1→2 | CI(L), CI(T), DBH(L), DBH(T), RD | `3e4ee0ee`, `3e9e6474`, `41d68ad4`, `41aefe0c`, `0x1.0c8e3df45880bp-10` | same | **MATCH** |
| V-12 rows 2→3 … 9→10 (extra) | all CI/DBH words and RD hex | identical to every A1 table cell | — | **MATCH** (all ten rows) |
| V-12 gain diagnostics | L @Y2 DIV/RECIP `3f6781dd`/`3f6781dc`; T @Y3 `3f6cb16c`/`3f6cb16b`; no other step differs | same; "two increment diagnostics differ" | **MATCH** |
| V-12 death branch | Year3 T alone gain / post; stale cached CI post | `0x3f7b33d4` 0.98125958… / `41b6d7ab`; stale `41b66292`; all-living `41b66397` | same three distinct words | **MATCH** |

**Contract consistency checks**

- **f32 rounding points and summation order.** These are stated in §2 and applied consistently in every vector I recomputed. The one exception is a *documentation-only* ordering slip in V-05's stand trace (A1-N1), which has no numeric effect.
- **8.0 inclusion and next-f32 exclusion:** applied correctly (V-04a). **0.5 m floor:** applied (Fquarter, and A in V-04b). **`max(1,DBH)`:** applied in formula but no vector reaches it (A1-N2).
- **Dead-neighbour A/B equality (V-02):** confirmed. With dead D present, both living CIs are bit-identical to V-01. The two wrong-inclusion CIs are correct and discriminating (2.8125 vs 0.3125; 0.8667 vs 0.2).
- **NOT DERIVABLE handling:** confirmed. Roll values and victims are never invented. V-06 gives only comparator-injected brackets explicitly "neither asserted to be the RNG output". V-06 final N is left as {2,1,0}. V-07/V-10 deaths are labelled test injections. Hpost and response-based crown are NOT DERIVABLE (§5 ND-01…ND-08).
- **V-12 is sequential:** confirmed. My implementation feeds each committed DBH into the next CI. All ten rows match A1, and the rows differ from one another, so they are not stitched independent Year1 runs.
- **Discriminator CIs:** V-02 (both), V-04b (the ordinal word and the permuted/double-accumulation words), V-05b (the final-bit word) and the V-12 death branch (three words) all discriminate as claimed.

## 3. A2 — provenance sample (12 numbers) and class codes

Each value was located in the accessible local copy at `a69b3e2e`. "Line" is the line in that file.

| # | A2 row / value | Source location and exact source text (abridged) | Present? | Class defensible? |
| --- | --- | --- | --- | --- |
| 1 | S-02: III 20.4 m (±2.8 %) | R1 l.163: "III 20.4 m (±2.8%)" [Synthesis p.3] | ✓ | P† IE ✓ |
| 2 | S-03: III 34.228 / 0.042 / 1.563 (q .50) | R1 l.167: "III (0.50) 34.228 / 0.042 / 1.563" | ✓ | P† IE (rounded) ✓ |
| 3 | S-05: MAI/CAI 0.89…0.45 m/yr | R1 l.173: "MAI/CAI intersections 0.89, 0.82, 0.75, 0.58 and 0.45 m/yr" | ✓ | P† IE ✓ |
| 4 | S-09: a 47.0216, c 0.4571, t0 −5.0; 749 traj./225 plots; LL 1318.48 vs 1261.87 | R2 l.283, 289–291 | ✓ | P† IE ✓ (version caveat carried) |
| 5 | D-01: NFI BA 32.97 (6.69–74.80, SD 12.78); stocking 1,662 (240–4,967, SD 911); 107/93; 183/200 ingrowth | R2 l.688–701 | ✓ | P† IE ✓ |
| 6 | D-02: Frenchpark 1,762/23.7/0.44 … heavy 616/32.4/0.85 | R1 l.192–202 | ✓ | P† IE ✓ |
| 7 | D-03 / F-01: "~55 m²/ha" untreated | R1 l.76 (Jones et al. 2026); l.412 labelled IRISH DIRECT; manifest l.33 G2: "mostly absent Irish data; transfer/modelled" | ✓ | Downgrade to S with geography unverified is **defensible and conservative**; F-01's contradiction is real |
| 8 | D-05: slope −2.063, SDImax 1,868, Dq 25 cm | R6: "slope … −2.063 … maximum SDI is 1,868 … SDI reference Dq = 25 cm"; R8 Comeau row | ✓ | P† GB ✓ |
| 9 | D-13: GROWFOR vs NFI BA −1.96 / 12.81 / 0.81; vol −1.41 / 18.80 / 0.84; no-thin BA −2.47 / 11.80 / 0.77 | R2 l.709, 710, 717 | ✓ | P† IE ✓ |
| 10 | G-02 / F-04: CCF 1.28/1.38 vs "observed 1.26/1.37" labelled moderate/heavy | R8 `dbh` row: "moderate/heavy vs unthinned … 1.28/1.38 vs observed 1.26/1.37" | ✓ | G CCF ✓. F-04 is real: 29.8/23.7 = 1.2574 is **light**, not moderate (medium = 1.3122) |
| 11 | C-01: 188.30 + 15.27·DBH; r 0.93; 73 trees ages 9–71, 72 used, DBH 7–112 (7–80 in regression) | R1 l.217, 324–325 | ✓ | P† GB ✓; radius = width/200 confirmed identical |
| 12 | C-03: 154 Sitka + 125 birch; 0.75→0.77; 0.67→0.74; 0.58557 / 0.30910 | R1 l.328–331; R8 crown row `0.109 Hegyi7` | ✓ | P† GB ✓ |
| 13 | M-07 constants 0.6 / 0.08 / S⁴ / 0.5 / RD ≤ 1 | R6 "Selected [C]" table | ✓ | G CCF ✓ |
| 14 | M-09: RD 0.435→0.69→0.91→0.97–0.99; 65 % vs 14 %; spikes 22–50 % | R6 "Production check"; R6 "Single-year spikes … 22–50 %" | ✓ | G CCF ✓, **but "609 cumulative deaths in 120 yr" misstates the time base** (A2-S1) |
| 15 | D-11: 2,100 stems/ha, Dq 15.79, BA 41.1, RD 0.435 | R7 starting-stand table; R6 "RD 0.435" | ✓ | G CCF ✓ |
| 16 | D-10 inputs: 100 stems, 30×30 m, RD 0.609795828454288 → 0.6375836886879149 | R9 l.26 "Protected Year-1 witness" (also gives area bits `3db851ec`) | ✓ | M (from G) ✓ |

**P-marking.** §1 defines every P as P† ("cited via secondary synthesis; primary not re-read"), and §0.2 states it again. Every P row I sampled carries †. **No P without † was found.**

**No new coefficient accepted.** Confirmed: §10 options are conditional and marked "not adopted", and §11.C states it explicitly. **U cells stay unfilled:** S-13, G-06, C-05, M-03, M-06, CM-06/10 (measured)/16/17/18/19/20 are all blank or "—". **No Irish forecast:** stated in the header, §9 frame and §11.C. Nothing I read contradicts that.

### 3.1 A2 independent recomputation

| Row | Q value | A2 value | Result |
| --- | --- | --- | --- |
| D-10 N/ha (decimal 0.09) | 1111.111…; with declared binary32 area `3db851ec` → f32 1111.111083984375 | 1,111.1 | **MATCH** |
| D-10 Dq Y0 / post-Y1 | 25.303288 / 25.855791 (binary32 area: 25.855792) | ≈25.30 / 25.86 | **MATCH** |
| D-10 BA Y0 / post-Y1 | 55.8729 / 58.3395 m²/ha | ≈55.9 / 58.3 | **MATCH** |
| D-10 Dq and BA at RD 1 | 32.1592 cm, 90.252 m²/ha | 32.16, 90.3 | **MATCH** |
| D-06 BA at RD 1, Dq 15/20/25/30/40/50/60 | 94.69/92.99/91.70/90.65/89.02/87.78/86.77; Nmax 5358.6/2960.1/1868/1282.4/708.4/447.0/306.9 | 94.7/93.0/91.7/90.6/89.0/87.8/86.8; 5,359/2,960/1,868/1,282/708/447/307 | **MATCH** (90.65 is printed as 90.6, which is within rounding) |
| D-06 BA at RD 0.6 | 56.82/55.80/55.02/54.39/53.41/52.67/52.06 | 56.8/55.8/55.0/54.4/53.4/52.7/52.1 | **MATCH** |
| D-06b Frenchpark lower bounds | BA ≥ 77.731/64.516/57.809/50.788; RD ≥ 0.8449/0.7114/0.6392/0.5630 | 77.7/64.5/57.8/50.8; 0.845/0.711/0.639/0.563 | **MATCH**. "Lower bound" is mathematically valid because arithmetic-mean DBH ≤ Dq and both BA and RD increase with Dq |
| G-01 ratios | 1.2574/1.3122/1.3671 | 1.257/1.312/1.367 | **MATCH** |
| F-06 product of NFI means | 45.451 m²/ha | 45.5 | **MATCH** (see A2-N6 on wording) |
| S-03 rounded H30 diffs / H100 | −2.192/−0.987/−0.079/−0.604/−1.947; 51.84/44.51/33.43/27.91/22.41 | −2.19/−0.99/−0.08/−0.60/−1.95; 51.8/44.5/33.4/27.9/22.4 | **MATCH** (also equals R4 table) |
| S-04 b0; H20…H120 | 34.3616; 14.206/20.400/24.892/30.139/32.514/33.560/34.015 | 34.362; 14.21/20.40/24.89/30.14/32.51/33.56/34.01 | **MATCH** (also equals R4 and R3 "34.362") |
| S-05 max MAI | 0.7103 m/yr at age 20 (continuous 19.89) | 0.710 at 20 | **MATCH** |
| S-10 GROWFOR b; H(t) | b 0.032797; 13.201/20.400/26.651/35.672/40.919/43.797 | 0.03280; 13.20/20.40/26.65/35.67/40.92/43.80 | **MATCH**. I checked the equation form against R2 §4.5–4.6 eq. (4)/(5) with H0 = 0 |
| S-10 divergence vs S-04 | −1.005 (−7.07 %), 0, +1.759 (+7.07 %), +5.532 (+18.36 %), +8.405 (+25.85 %), +10.238 (+30.51 %); 30→40 increment 4.492 vs 6.251 | −1.00 (−7.1 %), 0, +1.76 (+7.1 %), +5.53 (+18.4 %), +8.41 (+25.9 %), +10.24 (+30.5 %); 4.49 vs 6.25 | **MATCH** |
| M-08 P; expected Year-1 hazard deaths | P = 0.093959; 100·0.08·P² = **0.07063** stems; Poisson P(0) ≈ 0.932 | "= 0.094, i.e. about 0.07" | **MATCH** (see A2-N1 on wording). The cap cannot bind: max(S⁴)/mean(S⁴) ≤ N = 100, so every p_i ≤ 0.0706 < 0.5 |
| M-08 stand rates RD 0.7/0.8/0.9/1.0 | 0.5 %/2.0 %/4.5 %/8.0 % | same | **MATCH** |
| C-01 radius at DBH 15…60 | 2.087/2.850/3.232/3.996/4.759/5.523 | 2.09/2.85/3.23/4.00/4.76/5.52 | **MATCH** |
| C-02 | r 2.8732 m, 25.934 m², 288.2 % | 2.87, 25.9, 288 % | **MATCH** |
| T-04 ten-step gains (CI 0/2/5/8.2/15) | 9.055/6.551/4.630/3.527/2.341; RD10 1.1463/0.9806/0.8625/0.7982/0.7319; BA10 103.00/88.53/78.17/72.52/66.67 | 9.06/6.55/4.63/3.53/2.34; 1.146/0.981/0.862/0.798/0.732; 103.0/88.5/78.2/72.5/66.7 | **MATCH** (see A2-N3 on labelling) |
| T-06 | E(40)/E(30) = 1.22022; 1 − 0.85¹⁰ = 0.8031 | 1.2202; 80.3 % | **MATCH** |
| T-07 | Dq at RD 1 = 32.159; +6.856 over exact Dq0 | 32.16; +6.9 | **MATCH** |

## 4. Cross-deliverable consistency

| Check | Result |
| --- | --- |
| A1 synthetic rules vs A2 G labels | **Consistent.** Every rule constant A1 exercises (Hegyi 8 m cutoff, 0.5 m floor, Ci50 = 5, 1.2·site·(1−D/120), crown 0.9415 + 0.07635·D with 15 % relax, RD onset 0.6, strength 0.08, S⁴/mean, 0.5 cap, RD ≤ 1 boundary) is labelled **G** by A2 (C-04, C-06, C-07, M-07, §6.2, §7.2). The Tabbush crown envelope is P† GB in A2, and A1 treats it as a rule term, not evidence. A1 §7 calls these "simplified source-inspired experimental adult Sitka rules, not established ecological measurements or an Irish forestry forecast". **No conflict.** |
| A2 D-10 derived only from printed numbers | **Confirmed.** The inputs (100 stems, 30×30 m, RD 0.609795828454288 / 0.6375836886879149) are printed in #737 l.26. A2's Appendix B uses only those literals, and my recomputation needs nothing else. No fixture bytes are required or claimed. The decimal 0.09 ha vs declared binary32 `3db851ec` difference has no effect at printed precision (shown in §3.1). |
| M-1 living-only target + neighbour exclusion | **A1:** explicit and tested (V-02/V-03, §2, A1-M1). **A2:** nothing in A2 contradicts M-1, and T-07/T-09 use living stems for BA. However, A2 §9 Group A does not state the dead-as-target/neighbour exclusion (A2-N5). |
| Year1 0→1 boundary preserved, not re-run or re-hashed | **A1:** §3 V-12 "Protected Year1 anchor, boundary-of-evidence only … did not read/hash those bytes". The 45,980 B / `574d8746…` values are quoted from #729/#732/#737 and are consistent with #737 l.26 and #760 l.23. **A2:** uses only the printed Year-0/Year-1 RD, and §0.2 and §12.2 disclaim any fixture or CLI output read or re-hash. Neither claims to re-run Year1. Both are **consistent**. A1's two-tree synthetic 0→1 vector is explicitly not the protected witness. |
| A1 Year-1 RD vs A2 D-10 | Both quote 0.6375836886879149 post-Year-1 for the protected stand. **Consistent.** |

## 5. Permission and scope audit

I searched both documents for source access, fixture read/hash, .NET/P1 execution, Unity, Ops, main edit, Host/save, viewer #776, #779, #770 and GPT-6.2.

| Item | A1 | A2 |
| --- | --- | --- |
| Product/P0/#710/#725/#687 source access | Disclaimed (§1 "Not read or used", §7) | Disclaimed (§0.2, §11.C, §12.2) |
| Fixture read/hash (32,567 B), stdout (45,980 B) | Disclaimed: "neither re-read nor re-hashed" | Disclaimed: "neither read nor re-hashed" |
| .NET / P1 execution | Disclaimed: "No .NET/P1 execution occurred" | Disclaimed: "no execution of any product or .NET build"; "No test of the Multi-P1 growth model 2" |
| Unity | Disclaimed (§7, ND-08) | Disclaimed (§12.2) |
| Ops board/lanes | Disclaimed (§1, §7) | Disclaimed (header work class) |
| main edit | Disclaimed ("main … untouched") | Disclaimed (work class, checkout note) |
| Host/save | Disclaimed (§1) | Disclaimed (§12.2) |
| Viewer #776 | Disclaimed generically ("viewer"), not by number (A1-N4) | Disclaimed by number |
| #779 | Disclaimed generically ("security task"), not by number (A1-N4) | Disclaimed by number |
| #770 | Disclaimed by number | Disclaimed by number |
| GPT-6.2 | Absent | Absent |
| L4 FAIL38 | "UNWAIVED" stated (§1, ND-08, §6) | "remains unwaived" (§11.C) |
| Positive executions claimed | CPython stdlib arithmetic only | stdlib arithmetic only |

**No unauthorised access or execution claim was found in either document.**

## 6. Findings

Severity scale: BLOCKER = invalidates the research claim; SHOULD = factual or provenance error to correct before the document is used as evidence; NIT = clarity or completeness only. **There are no numeric MISMATCHes in either document.**

### A1

| ID | Sev. | Location | Finding | Reachable failure case / evidence | Bounded correction |
| --- | --- | --- | --- | --- | --- |
| A1-N1 | NIT | Appendix A "V05 square/sum 1/2"; Appendix B `stand([d1,d2],area)` with d1 = T(20.1), d2 = L(25.3) | §2 declares "ordinal `sumSq += …`", but the V-05 stand trace sums T before L. Ordinal order is L, T, which V-01 follows. | With two terms, IEEE addition is exactly commutative. I checked: ΣD² = 1044.0999767303474 in both orders. There is no numeric effect, but an implementer copying the trace order could think non-ordinal stand summation is acceptable. | Reorder the V-05 trace to L then T, or add a note: "two-term sum; order immaterial here". No value changes. |
| A1-N2 | NIT | §2 CI sequence `max(f(1),Dt)`; DBH clamp `[1,200]` | The target-DBH floor `max(1,Dt)` and the lower DBH clamp at 1 are declared, but no vector has a target with D < 1. A1-SPATIAL lists "8 inclusive, next-f32, .25→.5 floor" but not this term. | The floor is reachable only if a stored DBH < 1 cm is admissible, which the clamp suggests it is not. An implementation that omits `max(1,·)` would pass every A1 vector. | State that `max(1,Dt)` is unreachable for clamp-valid state (and therefore untested), or add a typed-refusal or inert probe. |
| A1-N3 | NIT | V-04b roster: `A`=(.25,0,D200) next to T at (0,0) | The non-associativity vector places a 200 cm (2 m diameter) stem centred 0.25 m from T, which is physically overlapping. V-04a carries the caveat "public geometry admissibility is not asserted"; V-04b does not. | A future engine with a minimum-spacing or stem-overlap guard would refuse V-04b, and the ordinal-sum test would silently lose its only discriminator. | Repeat the V-04a admissibility caveat in V-04b, or note that a non-overlapping alternative is needed if geometry is validated. |
| A1-N4 | NIT | §1 "Not read or used" | The viewer and security task are disclaimed generically ("viewer, security task") rather than as #776/#779. #770 is named. | Audit trail only. | Name #776 and #779. |

### A2

| ID | Sev. | Location | Finding | Reachable failure case / evidence | Bounded correction |
| --- | --- | --- | --- | --- | --- |
| **A2-S1** | **SHOULD** | §7.1 M-09 | "609 cumulative deaths **in 120 yr**" misstates the time base. R6 *Production check*: "**Deaths over 100 years:** unthinned 609, light 562, moderate 520, heavy/selective 473". R3 Unthinned table: the run starts at **age 20** (row `20 … 0` deaths) and the `Deaths (cum.)` column reads 609 at **age 120**. | A reader converting to an annual rate gets 609/120 instead of 609/100, a 17 % understatement, for a G-class benchmark that A2 itself cites as long-run rule output. The R3 column is an unqualified count (not per ha), and 609 exceeds the 336 initial stems of that 40×40 m stand. The population basis (planted only vs including recruits, per stand vs per ha) is therefore **not stated in accessible text and NOT CHECKED**. | Replace with: "609 cumulative deaths by age 120, i.e. over 100 simulated years from age 20 (count as printed in R3/R6; per-stand vs per-ha and recruit inclusion not stated)". No other numbers change. |
| A2-N1 | NIT | §7.1 M-08 | "… with `P` from RD 0.6376 = 0.094, i.e. about 0.07 stems" reads as if 100·0.08·P² = 0.094. | My values: P = 0.093959; 100·0.08·P² = 0.07063. The arithmetic is right; the sentence is ambiguous. | Write "P = 0.0940; expected deaths = 100·0.08·P² = 0.0706". Optionally add that the 0.5 cap provably cannot bind here (max p_i ≤ N·0.08·P² = 0.0706). |
| A2-N2 | NIT | §9 Group B E-03 | Classed "P† GB". The claim that a stand near RD 0.6 at age 30 "is expected to remain at or below" RD 1 through age 40 is an inference about a trajectory. The Comeau abstract (as relayed in R6) gives only a boundary position. Under the CCF engine the boundary is enforced by rule (G). | A later lane could cite E-03 as primary evidence of a trajectory, which it is not. | Reclass as "S (inference from P† GB boundary)". |
| A2-N3 | NIT | §9 Group A T-04 | The ten-step uniform-tree illustration reports "no deaths" RD10 values up to 1.146 (CI 0) and 0.981 (CI 2). These cannot be reached under A2's own T-07 boundary rule, and every row exceeds the RD 0.6 hazard onset, so "no deaths" is a counterfactual. CI 8.2 equals R7's **Scenario One** mean Hegyi CI (a different stand, flagged by A2's own F-09), and its source is not given. | A reader could take RD10 = 1.146 as an engine outcome, or CI 8.2 as a Multi-P1 property. | Label T-04 RD10/BA10 as "pre-mortality counterfactual; the boundary would cap RD at 1", and give CI 8.2 as "illustrative (R7 Scenario One mean CI; not Multi-P1)". |
| A2-N4 | NIT | §3 S-08 | The Conry & Clinch exclusion is truncated. R1 §10 reads "excludes very small sub-compartments and generally mixed stands **unless one species exceeds 80% of the canopy**". | Minor loss of population definition. | Restore the qualifier. |
| A2-N5 | NIT | §9 Group A | The #760 M-1 contract (dead trees excluded as Hegyi target **and** neighbour, and from N/BA/Dq/RD/mean(S⁴)/rolls) is not listed. Year2 is the first step where it matters. | Not a contradiction; T-07/T-09 already imply living-only BA. The completeness gap sits exactly at the Year1→Year2 seam. | Add a G-class row citing #760 §3 S3 M-1. |
| A2-N6 | NIT | §2 F-06 | The headline "NFI Table 1 means are not mutually consistent" overstates the issue. That the product of means (45.45 m²/ha) differs from the mean of products (32.97) is expected, and A2's own parenthetical explains why. | Could be read as an error in McCullagh et al. Table 1. | Reword as "cannot be combined: means of products ≠ products of means …". |

**Severity counts: BLOCKER 0, SHOULD 1 (A2-S1), NIT 10 (A1: 4; A2: 6).**

## 7. Verdict

| Document | Verdict | Coverage | NOT CHECKED |
| --- | --- | --- | --- |
| **A1** | **APPROVE (research quality only)**. The NITs are optional and do not affect any value. | Independently recomputed V-01, V-02 (A/B and both discriminators), V-04a, V-04b (all six orders), V-05a/b, V-06 (except roll mapping), V-07a/b/c/d, V-08 stand, **all ten** V-12 rows plus gains and the death branch. All are MATCH. Checked NOT DERIVABLE discipline, sequential V-12, Year1 boundary disclaimer and scope disclaimers. | V-03/V-09/V-10/V-11/V-13 (discrete or policy, no new arithmetic); A1's Appendix A crown traces for V-05 (V-01 crowns were checked); numerator→roll mapping and RNG key composition; whether the §2 numeric profile equals native .NET/Mono evaluation sites (A1 itself marks this conditional); the A1-cited #723/#732/receipt documents (not re-read; only #729/#737/#760 were). |
| **A2** | **NEEDS BOUNDED CORRECTION (one SHOULD, A2-S1: a one-line wording fix in M-09; no derived number changes).** Otherwise research-quality acceptable. | 16 provenance numbers sampled across productivity, density, growth, crown and mortality, all present at cited locations, with defensible class codes. Independently recomputed D-10, D-06, D-06b, S-03, S-04, S-05, S-10, M-08, C-01, C-02, G-01, F-06, T-04, T-06, T-07. All are MATCH. Checked P† marking, U cells, no coefficient adoption and no forecast. | Primary PDFs (none accessible). R2 §6 coefficient matrices (A2 does not use them). R5 content beyond its superseded status. D-12, S-06, S-07, M-05 and D-08 were spot-read in R1/R2 but not exhaustively. Population basis of the R3/R6 "609" deaths. Whether "growth model 2" equals model 1 (A2 F-08, left open). |
| **Overall** | **NEEDS BOUNDED CORRECTION (A2 only)**. A1 needs no correction. The parent may route A2-S1, and optionally the NITs, to the A2 author on the same branch/file. | — | — |

**This verdict does not accept any coefficient, approve Year2 or any continuation spec, approve calibration, or authorise source, fixture, engine, Unity, Ops or Host access.** It is a research-quality opinion on two documents.

## 8. NOT RUN

- No engine, native, .NET, P1, Mono or Unity run of any kind. No Year2–10 simulation. No GROWFOR reconstruction.
- No product, P0, #710/#725/#687/#639 source or fixture byte was read. No fixture or stdout was hashed.
- No primary paper or PDF was re-read. All primary-study numbers were checked only against the CCF secondary syntheses.
- No Ops board/lanes, Host, save, viewer #776, #779, #770 or Scenario One work. No edits to A1, A2, the primary checkout, main or any path other than this one file.
- The only computation was my own stdlib CPython 3.12.3 arithmetic (§9). **Nothing in this review constitutes implementation verification**, engine qualification or native parity.

## 9. Reviewer scripts (standard library only; independent of both authors' appendices)

Run: `python3 -I q_a1.py` and `python3 -I q_a2.py`. Both read no files and use no network.
Local SHA-256 values: `q_a1.py` `3e6694e5dd1f96faba9ff5b01f9fe561014f9be689a60788211eb8ceb48dabd0`, output `50bf3a87385ccc4450408f4c9f78c4229feb839cb2f257c7448d668bd9c4cc31`; `q_a2.py` `ce24403c1e7a37b25805f5352401ed0c498cf18dbeea978aecb7afc1a925d4ac`, output `f4698a4afbf235d1ff51f829444191ca83249fbfe15cc73a8564202892818cf2`.

### 9.1 `q_a1.py`

```python
#!/usr/bin/env python3
# Q reviewer's own independent reimplementation (stdlib only). Written from A1 §2 contract
# text and the quoted #729/#737 rules, NOT from A1 Appendix B.
import struct, math
def f(x): return struct.unpack('>f', struct.pack('>f', x))[0]
def bits(x): return struct.pack('>f', x).hex()
def s32(x): return f"{x!r}/0x{bits(x)}"
F8, FH, F1, F5 = f(8.0), f(0.5), f(1.0), f(5.0)
class T:
    def __init__(s, id, x, z, d, live=True): s.id, s.x, s.z, s.d, s.live = id, f(x), f(z), f(d), live
def ci(target, roster, order=None):
    nb = [t for t in roster if t.id != target.id and t.live]
    nb = sorted(nb, key=lambda t: t.id) if order is None else [next(t for t in nb if t.id == o) for o in order]
    acc = f(0.0); cnt = 0; trace = []
    for j in nb:
        dx = f(target.x - j.x); dz = f(target.z - j.z)
        q = f(f(dx*dx) + f(dz*dz)); dist = f(math.sqrt(q))
        if dist > F8: trace.append((j.id, 'excl', dist)); continue
        ratio = f(j.d / max(F1, target.d)); term = f(ratio / max(FH, dist))
        acc = f(acc + term); cnt += 1; trace.append((j.id, dist, term, acc))
    return acc, cnt, trace
def grow(D, CI, site=1.0, recip=False):
    a = f(D / f(120.0)); b = min(max(f(1.0 - a), 0.0), 1.0)
    n = f(f(f(1.2)*f(site))*b); q = f(CI / F5); den = f(1.0 + q)
    g = f(n * f(1.0/den)) if recip else f(n/den)
    return g, min(max(f(D + g), 1.0), 200.0)
def stand(ds, area):
    N = len(ds); area = f(area); nha = f(N / area)
    ss = 0.0
    for d in ds: ss += float(d)*float(d)
    dq = math.sqrt(ss / N); BA = (ss*math.pi)/40000.0
    rd = (float(nha)*math.pow(dq/25.0, 2.063))/1868.0
    return dict(N=N, area=area, nha=nha, ss=ss, BA=BA, BAha=BA/float(area), dq=dq, rd=rd, P=max(0.0,(rd-0.6)/0.4))
def S(CI): q = f(CI/F5); return 1.0 - 1.0/(1.0 + float(q))
def crown_r1(Dpost, C):
    m = f(f(0.07635)*Dpost); base = max(f(0.1), f(f(0.9415)+m)); t = f(base*1.0)
    return max(f(0.1), f(C + f(f(t - C)*f(0.15))))
out = []
def P(*a): out.append(' '.join(str(x) for x in a)); print(*a)
# V-01
L = T('L', 4, 0, 25); Tt = T('T', 0, 0, 20); B = [L, Tt]
ciL = ci(L, B)[0]; ciT = ci(Tt, B)[0]
gL, dL = grow(L.d, ciL); gT, dT = grow(Tt.d, ciT)
P('V01 CI(L)', s32(ciL), 'CI(T)', s32(ciT)); P('V01 post L', s32(dL), 'T', s32(dT))
P('V01 recip L', s32(grow(L.d, ciL, recip=True)[1]), 'T', s32(grow(Tt.d, ciT, recip=True)[1]))
P('V01 crown r1 T', s32(crown_r1(dT, f(2.0))), 'L', s32(crown_r1(dL, f(2.2))))
sb = stand([L.d, Tt.d], 1.0); sp = stand([dL, dT], 1.0)
P('V01 before BA', repr(sb['BA']), 'Dq', repr(sb['dq']), 'RD', repr(sb['rd']))
P('V01 post   BA', repr(sp['BA']), 'Dq', repr(sp['dq']), 'RD', repr(sp['rd']), 'P', sp['P'])
VL, VT = S(ciL)**4, S(ciT)**4; P('V01 S L', repr(S(ciL)), 'S T', repr(S(ciT)), 'meanV', repr((VL+VT)/2))
# V-02 dead neighbour
D = T('D', 1, 0, 50, live=False)
A = [D, L, Tt]
P('V02 A-state CI(L)', s32(ci(L, A)[0]), 'CI(T)', s32(ci(Tt, A)[0]), '(dead excluded)')
Dl = T('D', 1, 0, 50, live=True); Aw = [Dl, L, Tt]
P('V02 wrong-inclusion CI(T)', s32(ci(Tt, Aw)[0]), 'CI(L)', s32(ci(L, Aw)[0]))
P('V02 D BA not added', repr(2500*math.pi/40000))
# V-04a
E8 = T('E8', 8, 0, 20); Eo = T('Eover', 8.00000095367431640625, 0, 20); Fq = T('Fquarter', .25, 0, 20)
P('V04a Eover.x', s32(Eo.x), 'next after 8?', bits(Eo.x) == '41000001')
c, n, tr = ci(Tt, [Tt, E8, Eo, Fq]); P('V04a CI', s32(c), 'count', n, tr)
# V-04b
Aa = T('A', .25, 0, 200); Bb = T('B', 8, 0, 1); Cc = T('C', -8, 0, 2)
r4 = [Tt, Aa, Bb, Cc]
c1, n1, tr1 = ci(Tt, r4, ['A', 'B', 'C']); c2, n2, tr2 = ci(Tt, r4, ['B', 'C', 'A'])
P('V04b ABC', s32(c1), tr1); P('V04b BCA', s32(c2), tr2)
P('V04b all 6 perms', {''.join(p): bits(ci(Tt, r4, list(p))[0]) for p in __import__('itertools').permutations('ABC')})
# exact rational sum and double accumulation
P('V04b double-accum', repr(20.0 + f(1/160) + f(2/160)), 'f32 of exact', s32(f(20 + 0.00625 + 0.0125)))
# V-05a
a01 = f(0.1); T5 = T('T', 0, 0, 20.1); L5 = T('L', 4, 0, 25.3)
c5 = ci(T5, [T5, L5])[0]; P('V05a area', s32(a01), 'D(T)', s32(T5.d), 'D(L)', s32(L5.d), 'CI(T)', s32(c5))
s5 = stand([T5.d, L5.d], 0.1)  # ordinal order: L before T
s5o = stand([L5.d, T5.d], 0.1)
P('V05a Nha', s32(s5['nha']), 'ss', repr(s5['ss']), 'ss(L,T order)', repr(s5o['ss']), 'BA', repr(s5['BA']), 'BAha', repr(s5['BAha']), 'Dq', repr(s5['dq']), 'RD', repr(s5['rd']))
nha64 = 2/float(a01); rd64 = (nha64*math.pow(s5['dq']/25, 2.063))/1868
P('V05a Nha_f64', repr(nha64), 'RD_f64', repr(rd64), 'diff', repr(s5['rd']-rd64), 'BA/0.1', repr(s5['BA']/0.1))
# V-05b
T5b = T('T', 0, 0, 10); L5b = T('L', 4, 0, 38.7)
c5b = ci(T5b, [T5b, L5b])[0]; P('V05b L.D', s32(L5b.d), 'CI', s32(c5b))
P('V05b post DIV', s32(grow(T5b.d, c5b)[1]), 'RECIP', s32(grow(T5b.d, c5b, recip=True)[1]))
ci64 = (float(L5b.d)/10.0)/4.0
post64 = 10.0 + (1.2*(1-10.0/120))/(1+ci64/5)
P('V05b all-f64 CI', repr(ci64), 'post', repr(post64), '->f32', s32(f(post64)))
# V-06
a6 = f(2/1400); A6 = T('A', 0, 0, 25); B6 = T('B', 4, 0, 25)
c6 = ci(A6, [A6, B6])[0]; g6, d6 = grow(A6.d, c6); s6 = stand([d6, d6], a6)
s_ = S(c6); V = s_**4; p6 = min(0.5, 0.08*s6['P']**2*V/V)
P('V06 area', s32(a6), 'CI', s32(c6), 'post', s32(d6), 'Nha', s6['nha'], 'RD', repr(s6['rd']), 'P', repr(s6['P']), 'S', repr(s_), 'V', repr(V), 'p', repr(p6), p6.hex())
P('V06 rolls', repr(357700/2**24), repr(357701/2**24), 357700/2**24 < p6 < 357701/2**24)
# V-07
a7 = f(4/1868); s7 = stand([f(25.0)]*4, a7)
P('V07a area', s32(a7), 'Nha', s32(s7['nha']), 'RD', repr(s7['rd']), 'RD>1', s7['rd'] > 1, 'P', repr(s7['P']))
a7b = struct.unpack('>f', struct.pack('>I', struct.unpack('>I', struct.pack('>f', a7))[0]-1))[0]
s7b = stand([f(25.0)]*4, a7b); s7b3 = stand([f(25.0)]*3, a7b)
P('V07b area', s32(a7b), 'Nha', s32(s7b['nha']), 'RD', repr(s7b['rd']), 'RD>1', s7b['rd'] > 1, '| N3 Nha', s32(s7b3['nha']), 'RD', repr(s7b3['rd']))
P('V07b nha ulp at 1868', 2.0**(10-23))
a7c = f(4/3500)
P('V07c area', s32(a7c), [repr(stand([f(25.0)]*k, a7c)['rd']) for k in (4, 3, 2, 1)])
s8 = stand([f(200.0)], 0.001); P('V07d/V08 Nha', s32(s8['nha']), 'RD', repr(s8['rd']), 'P', repr(s8['P']))
# V-12
dL, dT = L.d, Tt.d; rows = []
for k in range(10):
    Lk, Tk = T('L', 4, 0, dL), T('T', 0, 0, dT)
    cL, cT = ci(Lk, [Lk, Tk])[0], ci(Tk, [Lk, Tk])[0]
    gLd, nL = grow(dL, cL); gLr, nLr = grow(dL, cL, recip=True)
    gTd, nT = grow(dT, cT); gTr, nTr = grow(dT, cT, recip=True)
    assert nL == nLr and nT == nTr
    dL, dT = nL, nT; rd = stand([dL, dT], 1.0)['rd']
    P(f'V12 {k}->{k+1} CI(L) {s32(cL)} CI(T) {s32(cT)} D(L) {s32(dL)} D(T) {s32(dT)} RD {rd!r} {rd.hex()} gainsL {bits(gLd)}/{bits(gLr)} gainsT {bits(gTd)}/{bits(gTr)}')
    if k == 1: dL2, dT2, cT2 = dL, dT, cT
# V-12 death branch: L dies after Year2 growth; Year3 T alone
g3, d3 = grow(dT2, 0.0); P('V12 death Year3 T alone gain', s32(g3), 'post', s32(d3))
P('V12 stale CI', s32(grow(dT2, cT2)[1]))
```

### 9.2 `q_a2.py`

```python
#!/usr/bin/env python3
# Q reviewer's own recomputation of A2 derived (M) values; stdlib; formulas from R1/R2/R6 text, not A2 App. B.
import math, struct
def f(x): return struct.unpack('>f', struct.pack('>f', x))[0]
ba = lambda n, dq: n*math.pi*(dq/200.0)**2          # m2/ha: n * pi * r^2, r in m
rd = lambda n, dq: n*(dq/25.0)**2.063/1868.0
dq_from = lambda n, r: 25.0*(r*1868.0/n)**(1/2.063)
# D-10 / CM-05 / CM-08 / CM-10
for label, area in (('decimal 0.09', 0.09), ('binary32 3db851ec', struct.unpack('>f', bytes.fromhex('3db851ec'))[0])):
    n = 100/area; nf = f(100/area) if 'binary' in label else n
    out = [label, 'area', repr(area), 'N/ha', repr(n), 'f32 N/ha', repr(nf)]
    for r in (0.609795828454288, 0.6375836886879149):
        d = dq_from(nf, r); out += ['RD', r, 'Dq %.6f' % d, 'BA %.4f' % ba(nf, d)]
    out += ['Dq@RD1 %.4f' % dq_from(nf, 1.0), 'BA@RD1 %.3f' % ba(nf, dq_from(nf, 1.0))]
    print('D-10', *out)
# D-06 British line
for d in (15, 20, 25, 30, 40, 50, 60):
    nmax = 1868*(d/25)**-2.063
    print('D-06 Dq %d Nmax %.1f BA1 %.2f BA0.6 %.2f' % (d, nmax, ba(nmax, d), ba(0.6*nmax, d)))
# D-06b Frenchpark lower bounds
for name, n, d in (('unthinned', 1762, 23.7), ('light', 925, 29.8), ('medium', 761, 31.1), ('heavy', 616, 32.4)):
    print('D-06b %-9s BA>= %.3f RD>= %.4f ratio %.4f' % (name, ba(n, d), rd(n, d), d/23.7))
print('F-06 NFI product-of-means BA %.3f' % ba(1662, 18.66))
# S-03/S-04 Lekwadi
lek = {'I': (58.371, .024, 1.247, 27.6), 'II': (49.523, .025, 1.247, 23.3), 'III': (34.228, .042, 1.563, 20.4), 'IV': (28.272, .051, 2.134, 17.4), 'V': (22.557, .060, 2.619, 16.0)}
H = lambda b0, b2, b3, t: b0*(1-math.exp(-b2*t))**b3
for k, (b0, b2, b3, a) in lek.items():
    print('S-03 %-3s H30 %.3f diff %.3f H100 %.2f' % (k, H(b0, b2, b3, 30), H(b0, b2, b3, 30)-a, H(b0, b2, b3, 100)))
b0 = 20.4/(1-math.exp(-.042*30))**1.563; E = lambda t: H(b0, .042, 1.563, t)
print('S-04 b0 %.4f' % b0, {t: round(E(t), 3) for t in (20, 30, 40, 60, 80, 100, 120)})
mai = max((E(t)/t, t) for t in range(1, 200)); print('S-05 max MAI %.4f at %d' % mai)
# finer search for MAI/CAI intersection
best = max((E(t/100)/(t/100), t/100) for t in range(500, 6000)); print('S-05 continuous max MAI %.4f at %.2f' % best)
# S-10 GROWFOR (H0=0)
a, c, t0, S = 47.0216, 0.4571, -5.0, 20.4
b = -math.log(1-(S/a)**c)/(30-t0)
G = lambda t: a*(1-math.exp(-b*(t-t0)))**(1/c)
print('S-10 b %.6f' % b)
for t in (20, 30, 40, 60, 80, 100):
    print('S-10 t %3d GF %.3f Lek %.3f diff %+.3f (%+.2f%%)' % (t, G(t), E(t), G(t)-E(t), 100*(G(t)-E(t))/E(t)))
print('S-10 incr30-40 Lek %.3f GF %.3f' % (E(40)-E(30), G(40)-G(30)))
# M-08
P = (0.6375836886879149-0.6)/0.4
print('M-08 P %.6f expected deaths %.5f P(0 deaths|Poisson) %.4f' % (P, 100*0.08*P*P, math.exp(-100*0.08*P*P)))
for r in (0.7, 0.8, 0.9, 1.0): print('M-08 RD %.1f rate %.4f' % (r, 0.08*((r-0.6)/0.4)**2))
# C-01/C-02
for d in (15, 25, 30, 40, 50, 60): print('C-01 D %d r %.4f width/2/100 %.4f' % (d, 0.9415+0.07635*d, (188.30+15.27*d)/200))
r = 0.9415+0.07635*25.30; print('C-02 r %.4f area %.3f pct %.1f' % (r, math.pi*r*r, 100*100*math.pi*r*r/900))
# T-04 / T-06 / T-07
def grow(d, ci, y):
    for _ in range(y): d += 1.2*max(0, min(1, 1-d/120))/(1+ci/5)
    return d
n = 100/0.09
for ci in (0, 2, 5, 8.2, 15):
    d10 = grow(25.30, ci, 10); print('T-04 CI %4s gain %.3f RD10 %.4f BA10 %.2f' % (ci, d10-25.30, 0.609795828454288*(d10/25.30)**2.063, ba(n, d10)))
d0 = dq_from(n, 0.609795828454288)
d10 = grow(d0, 0, 10); print('T-04 using exact Dq0 %.5f CI0 gain %.4f' % (d0, d10-d0))
print('T-06 E40/E30 %.5f gapclosed %.4f' % (E(40)/E(30), 1-0.85**10))
print('T-07 Dq@RD1 %.3f delta from Dq0 %.3f' % (dq_from(n, 1.0), dq_from(n, 1.0)-d0))
```
