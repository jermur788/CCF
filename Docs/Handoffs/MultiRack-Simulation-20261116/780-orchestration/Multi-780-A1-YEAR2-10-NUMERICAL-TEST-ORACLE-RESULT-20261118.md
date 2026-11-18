# Multi #780 A1 — synthetic Year2–10 numerical test-oracle pack

**DRAFT RESEARCH / NOT RUN / NO PRODUCT SOURCE AUTHORITY**  
**All future engine tests in this document: NOT RUN.** Independent synthetic arithmetic was calculated in Python; no product engine, .NET build/runtime/test, Unity or fixture was executed or consumed. This is a partial numeric oracle with explicit unknowns, not a complete native-output golden or an approved continuation specification.

Worker: **gpt-6.1-sol**, provider **Codex via T3 Code**, Codex harness, high reasoning effort; host `jer-laptop`, Linux `7.0.0-42-generic`, x86_64. UTC preflight `2026-11-18T20:15:12Z`. [Registration before any file write](https://github.com/jermur788/CCF/issues/780#issuecomment-6990361765). Working alone; no agents spawned. Fresh GitHub main base: `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`. Exclusive branch: `docs/multi-780-a1-sol-numeric-oracle-20261118-v1`. Exactly one added Markdown: `Docs/Handoffs/MultiRack-Simulation-20261116/780-orchestration/Multi-780-A1-YEAR2-10-NUMERICAL-TEST-ORACLE-RESULT-20261118.md`. Publication uses GitHub API; scratch only `/tmp/opencode/780-a1`.

## 1. Evidence and status

These six exact published Markdown documents were retrieved using authenticated `gh api` Contents reads. Branch tips were resolved to full commits before content reads. References below are immutable; references inside those documents were **not followed**. Reading an implementation report is not direct source inspection.

- **[737](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md)**, commit `da37b23bf97553e114ff7d0bab855612abcd9b9a`, path `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md`. Read for: S2–S6 chronology, arithmetic, rollback and output proposals. Retrieved Markdown SHA-256 `f9e64dd3b2d23fa4963858f74d63295b0ce2b5480bada364a7e9f083b9b48ebc`.

- **[760](https://github.com/jermur788/CCF/blob/149b4c80760d07e47c45458234520bd2e7aaa102/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-760-LANE-B-YEAR2-ANNUAL-DECISION-CLOSURE-DRAFT-RESULT-20261118.md)**, commit `149b4c80760d07e47c45458234520bd2e7aaa102`, path `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-760-LANE-B-YEAR2-ANNUAL-DECISION-CLOSURE-DRAFT-RESULT-20261118.md`. Read for: §2 B-A02/P01/P03/P04/O01–O03; §3 errata; §4 T/L/D vector. Retrieved Markdown SHA-256 `3952c9a50a3923a64fe39b66c7b624b33fe9da0f97dd3e938160483160776b8f`.

- **[729](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md)**, commit `8310a38086425a5b223315c8f83181dd77eee1eb`, path `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md`. Read for: §3 storage/order, §5 numeric types, §6 edges, §7 M-1. Retrieved Markdown SHA-256 `caef9c5c90d87d658d14bd24de7d6dbfff78d4f1c725a9a53bc2ce62cbc8593e`.

- **[732](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md)**, commit `88e82cd0cc3e60dcc777720332200a88231d4b8e`, path `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md`. Read for: original STOP and offline resumption: Year1 boundary only. Retrieved Markdown SHA-256 `8c2b35e3c6ded83e187c4af15d2ff792ba397d059e0e91d5f05dc85b846f6be1`.

- **[723](https://github.com/jermur788/CCF/blob/8890a730d349cb7082be34c213503055b8059b1f/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-723-FIRST-YEAR-IMPLEMENTATION-READINESS-AND-OVERNIGHT-RESULT-20261118.md)**, commit `8890a730d349cb7082be34c213503055b8059b1f`, path `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-723-FIRST-YEAR-IMPLEMENTATION-READINESS-AND-OVERNIGHT-RESULT-20261118.md`. Read for: Year1 oracle, abbreviated height law and numeric-limit disclosure only. Retrieved Markdown SHA-256 `bc3b7919e7919f30d6784aedb4741e2480156db42f42d474d1079a0d1a88d6a4`.

- **[receipt](https://github.com/jermur788/CCF/blob/90583811c4fb88508a4bf05c14365732e24a21ac/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md)**, commit `90583811c4fb88508a4bf05c14365732e24a21ac`, path `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md`. Read for: bounded owner four-file approval; no continuation/source access grant. Retrieved Markdown SHA-256 `b0a5866ab00d802a33a186ebe2e8b99348d60500e07de1b4dbfbe140e3bbe290`.

Repository entry context was also read locally, without edits: `Docs/Project/{AgentWorkflow.md,decision-log.md,current-milestone.md,game-brief.md,unity-project-overview.md,research-index.md}` at checkout HEAD `46247486b37904f6b36825bb40fb6ed7bea35e88`; latest context-changing commit observed `9f06398c068cbaed5b307229241c3640c5646d6d`. Live `Docs/Project/coordination/active-tasks.md` was dirty and identified itself as an October snapshot, not current November authority. These are workflow context, not additional numerical sources. User's exact exclusive-file packet controls this task. Local unrelated modified `AGENTS.md`, storage architecture and coordination file, untracked operations notes and coordination archive were preserved. No dashboard/Drive synchronization was attempted or verified; the packet forbids coordination writes, so that operational item remains for the parent.

**Not read or used:** any #710/#725 product source, original P0 source, #687 file, #639 fixture bytes, any local fixture, the 32,567-byte canonical fixture or the old 45,980-byte stdout (neither re-read nor re-hashed). No retained reviewer scripts or numeric CSVs were accessed. No new underlying source, approved spec files linked inside the receipt, unrelated issue comments, Ops board/lanes, Host/save, viewer, security task, Scenario One or #770 was read or changed. The source-report references to those things are not evidence of access here. Default **L4 FAIL38 is UNWAIVED**.

### 1.1 Exact published clauses used

From [#729 §3](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md#3-source-faithfulness-ccf-controller-vs-adapter), source Hegyi cell, quoted exactly:

> float `dx,dz`; `Vector2.Distance` = `(float)Math.Sqrt(dx*dx+dz*dz)`; `> 8` skip (inclusive 8 m); `(Dj/max(1,Dt))/max(0.5,dist)` float sum

Same table, order cell:

> `FindTrees` ordinal ID sort

Same table, hazard cell:

> `S = 1−1/(1+CI/5)` (float CI/5, then double), mean of S⁴ over living, `p>0 && Roll(...)<p`, key `ADULT-MORT-`+ID, post-increment year

From [#737 S4](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md#s4-annual-chronological-transaction-proposed-normative-order), quoted exactly:

> Same-source rule values are those in approved P1 spec §3: DBH `1.2*site*clamp01(1-D/120)/(1+CI/5)`; source Class III relative height; crown target `max(0.1,0.9415+0.07635*D_post)*response`, relax 15%; RD `(N/ha)*(Dq/25)^2.063/1868`; `P=max(0,(RD-0.6)/0.4)`; `S=1-1/(1+CI/5)`; `p=min(0.5,0.08*P^2*S^4/mean(S^4))` when denominators valid. Their exact float evaluation sites and source function linkage, not rewritten algebraic simplification, govern any production implementation.

From [#760 §3 S3 M-1 precondition](https://github.com/jermur788/CCF/blob/149b4c80760d07e47c45458234520bd2e7aaa102/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-760-LANE-B-YEAR2-ANNUAL-DECISION-CLOSURE-DRAFT-RESULT-20261118.md#3-exact-replacement--errata-clauses-for-original-737-draft-only), quoted exactly (quotation marks around the table cell omitted):

> **Before enabling the first `1→2` transition**, form the ordinal living-at-start set. Exclude any dead tree both as a **competition target and a neighbour**, and from growth, ageing, crown relaxation, stand `Before/PostGrowth/AfterMortality` N/BA/Dq/RD, mean vulnerability, hazard rolls, and density-boundary candidates. Retain **every original dead tree ID** in the immutable roster and audit without inventing new death events. Establish a controlled dead-neighbour oracle and preserve **exact existing 0→1 output bytes**.

From [#760 §3 S4 phase labels](https://github.com/jermur788/CCF/blob/149b4c80760d07e47c45458234520bd2e7aaa102/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-760-LANE-B-YEAR2-ANNUAL-DECISION-CLOSURE-DRAFT-RESULT-20261118.md#3-exact-replacement--errata-clauses-for-original-737-draft-only), quoted exactly:

> `Genesis` is a **static initial state** without an executed step. For a real accepted annual transition from `y` to `y+1`, `Before` contains the input **state year `y`**, while its **target year is `y+1`**. `PostGrowth` and `AfterMortality` belong to **that same completed target-year step**, and only `AfterMortality` is the committed successor. If the model reports a pre-growth CI after growth, it remains tagged as **calculated during Before**, never newly computed postmortality. No partial staged frame is published before full atomic commit.

From [#729 §3](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md#3-source-faithfulness-ccf-controller-vs-adapter), stand/source and boundary/source cells, quoted exactly:

> `count / StandAreaHectares` (int/float → float), binary64 Σ D², `Math.Sqrt`, `RelativeDensity`

> survivors only; RD > 1; sort by S descending, then ordinal ID; running sums; stop at RD ≤ 1 or count ≤ 1

From [#737 S5](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md#s5-seeded-determinism-identity-rollback-and-refusal), quoted exactly:

> One write/commit authority: only the engine changes biological state. A concurrent pair with the same expected revision yields one committed successor and one stale/conflict refusal; no double year. Retry after a pre-commit throw computes afresh against the identical prior committed snapshot. Retry after a successful commit with the same request identity **never applies biology again**.

Derived operational sequence (not an additional rule): derive target year once; freeze ordinal living-at-start IDs; pre-growth CI; living DBH/height growth and age+1; crown from post-growth DBH with cached pre-growth CI; post-growth living density; simultaneous hazard using that density and cached CI; survivors-only strict RD>1 boundary sorted by decreasing cached suppression then ordinal ID, recomputing until RD≤1 or count≤1; validate and atomically commit once. Dead contribute neither target nor neighbour, size growth, ageing, vulnerability, roll, N, BA, Dq or RD. Source order is [#737 S4](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md#s4-annual-chronological-transaction-proposed-normative-order); rollback/identity is [S5](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md#s5-seeded-determinism-identity-rollback-and-refusal); diagnostic ownership is [S6](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md#s6-annual-output-and-shared-read-boundary). The #760 errata supersede the draft recommendation of unconditional empty-state progression.

## 2. Numeric contract of this independent model

`f(x)` means `struct.unpack('>f', struct.pack('>f', x))[0]`. Inputs and f32 literals are converted before arithmetic; every indicated f32 operation is rounded immediately, nearest ties-to-even. Python floats hold the resulting f32 exactly in binary64. No numpy, FMA, compensated sum or algebraic reassociation is used. Negative zeros can be represented; these vectors use positive zero inputs. Decimal f32 values below are exact; hex words are the authoritative 32-bit payload, independent of byte order. f64 decimal strings are shortest round-trip encodings, **not exact decimal expansions**; the accompanying `float.hex()` value specifies the exact binary64 number.

Environment used for synthetic arithmetic:

```text
{
  "python": "3.12.3 (main, Aug 31 2026, 10:18:26) [GCC 13.3.0]",
  "implementation": "CPython",
  "machine": "x86_64",
  "platform": "Linux-7.0.0-42-generic-x86_64-with-glibc2.39",
  "float_info": "sys.float_info(max=1.7976931348623157e+308, max_exp=1024, max_10_exp=308, min=2.2250738585072014e-308, min_exp=-1021, min_10_exp=-307, dig=15, mant_dig=53, epsilon=2.220446049250313e-16, radix=2, rounds=1)",
  "rounding": "struct >f, IEEE nearest ties-to-even; no FMA, no numpy"
}
```

This is an **independent model, not the native runtime**. Python-double-then-f32 is a defensible emulation for these normal finite probes, not a universal correctly rounded binary32 emulator for every possible double-rounding midpoint. Axis-aligned distances deliberately make all cutoffs exact. `sqrt`/`pow` and f64 operation order are a declared emulation policy; .NET/Unity last bits are not inferred from CPython/libm.

CI sequence: `dx=f(xt-xj)`, `dz=f(zt-zj)`, `xx=f(dx*dx)`, `zz=f(dz*dz)`, `q=f(xx+zz)`, `distance=f(sqrt(q))`; reject only when `distance>f(8)`. `ratio=f(Dj/max(f(1),Dt))`; `term=f(ratio/max(f(.5),distance))`; `CI=f(CI+term)` in ordinal neighbour-ID order. Exclude self by stable ID and exclude dead on both sides.

DBH profile **DIV** follows #737's printed division: `a=f(D/f(120))`; `b=f(1-a)`; clamp b to [0,1]; `n=f(f(f(1.2)*site)*b)`; `q=f(CI/f(5))`; `den=f(1+q)`; `g=f(n/den)`; `Dpost=clamp(f(D+g),1,200)`. Profile **RECIP** additionally computes `r=f(1/den); g=f(n*r)`, reflecting #729's source cell written with `·1/(...)`. Both are printed. The published reports do not identify every expression-parenthesization site for every future implementation; **do not use differing gain bits as native goldens before lawful source confirmation**. V-01 post sizes and all ten V-12 committed DBHs agree under both profiles; two increment diagnostics differ (listed below).

Suppression is **not all-f32**: `q=f(CI/f(5))`, then binary64 `S=1-1/(1+float(q))`, `V=pow(S,4)`, ordinal binary64 sum/mean. Stand profile: ordinal `sumSq += double(D)*double(D)`; `Nha=f(N/area32)`; `Dq=sqrt(sumSq/N)` in double; `RD=(double(Nha)*pow(Dq/25,2.063))/1868`. BA profile is `(sumSq*pi)/40000` m², BAha `BA/double(area32)`. This BA grouping and exact pow/sum evaluation sites are independent-model policy; #729 establishes types and formula, not a general cross-runtime last-bit promise. Pressure and hazard are binary64 with the printed formula and left-to-right multiplication. No source-derived height or crown-response coefficients are invented.

Crown unit-probe profile: `m=f(f(.07635)*Dpost)`; `base=max(f(.1),f(f(.9415)+m))`; for **injected input response r**, `target=f(base*r)`; `Cpost=max(f(.1),f(C+f(f(target-C)*f(.15))))`. Numbers marked `r=1` are exact **conditional arithmetic probes**, not claims that real competition response equals one. Actual Hpost and response-based Cpost are **NOT DERIVABLE** from the allowed material. Crown equality A/B is still mandatory for identical living inputs and cached CI.

Tolerance classes for every vector: **E32** = zero ULP for declared f32 arithmetic/inputs, conditional on confirmed evaluation sites; **Ediscrete** = exact IDs, years, ages, counts, absence/evaluation flags and event counts; **E64-model** = exact hex against this Python profile only. For future native f64 BA/Dq/RD/S/V/p comparisons, proposed tolerance `max(8*ulp(expected), 1e-12*abs(expected))` (exact zero remains zero), pending written runtime policy. Do not apply that tolerance to discrete RD>1 decisions: V-07 exact one is exact; the above-one probe is 1.30696e-7 away and must trigger. No tolerance is assigned to NOT DERIVABLE fields. E32 storage differences are not excused with an f64 tolerance.

## 3. Stable synthetic vector register

All rosters are internal proposed test inputs, **never trusted public fixture replacements**. Units: x/z/H/crown radius m, DBH cm, area ha, BA m², BAha m²/ha, Dq cm, age biological years. Each unspecified tree uses Sitka / Mature-equivalent / Reference / site=f(1), H=f(20), crown=f(2), age30, living=true. Every coordinate and scalar is stored through f. Unless stated, area=f(1)=`1` / `0x3f800000`; seed `20261116`, growth-version2, RNG-version1 are declared synthetic bindings, not access to a real catalog. One-year annual-shaped vectors use checkpoint stateYear1→targetYear2 with input age30 (a synthetic checkpoint, not a claim of the published genesis). Mortality/CI-only vectors stop at that named seam and do **not** assert complete annual state. This scope is intentional where unknown H/response/RNG prevents a lawful complete numeric golden.

Base roster B:

| ID | x | z | DBH | H | Crown radius | Age | isLiving |
| --- | --- | --- | --- | --- | --- | --- | --- |
| L | 4 | 0 | 25 | 21 | f(2.2)=2.2000000476837158203125 / 0x400ccccd | 30 | true |
| T | 0 | 0 | 20 | 20 | 2 | 30 | true |

Historical D extension: ID `D`, x1,z0, DBH50,H25,crown3,age31,living=false. For V-02 use explicit synthetic historical cause=self-thinning, mechanism=hazard, deathYear1; inputYear1. These are input metadata, not an event fabricated in this transition. V-03/V-09/V-10 separately specify pre-inventory-dead null metadata. All roster fields not explicitly changed remain exact, including IDs, positions and living/dead metadata.

### V-01 — all-living two-tree control (B, area1)

Purpose: #760 §4 exact T/L geometry and sizes; preserve all-living control. Before IDs {L,T}, N2; each sees one neighbour. CI(T)=`0.3125` / `3ea00000`; CI(L)=`0.20000000298023223876953125` / `3e4ccccd`. Age becomes31 for both. Post DBH T=`20.9411773681640625` / `41a78788`, L=`25.9134616851806640625` / `41cf4ec5`. Exact gain and crown-base arithmetic is in Appendix A. Actual Hpost and actual crown Cpost: **NOT DERIVABLE**; conditional r=1 crown T=`2.08105373382568359375` / `40052ffc`, L=`2.3079988956451416015625` / `4013b641`.

Before BA=`0.08050331174823845`, Dq=`22.638462845343543`, RD=`0.0008724732341742041`; PostGrowth BA=`0.08718236962276603`, Dq=`23.5588667604602`, RD=`0.0009472342760715116`, Nha2. BAha equals BA at area1. P=0, p(L)=p(T)=0, roll=null/notEvaluated, evaluated-roll count0, hazard deaths0, boundary deaths0, AfterMortality stand=PostGrowth stand, final IDs {L,T}. Those outcomes follow without inventing RNG. E32/Ediscrete/E64-model as above. Wrong implementations: post-growth CI, f64-only DBH path, roll-at-zero, crown using pre-growth DBH or second growth after mortality.

### V-02 — M-1 dead-neighbour A/B (A=B+D versus B; area1)

Both states use identical living IDs/seed/year/area/model bits. Expected **every living CI, growth, H/crown, hazard p/roll, density and outcome identical to V-01**; known numeric fields are the exact V-01 values. A has full roster {D,L,T}, B {L,T}; both have living {L,T}. D remains DBH50/H25/crown3/age31/dead with its input metadata bit-for-bit; no D CI evaluation, growth, age increment, hazard roll or death event. D's own geometric basal area `2500*pi/40000` is **not** added to stand BA. N=2, never3; new event count0; no timber/fell award.

Counterfactual wrong inclusion: ordinal D then L gives T CI=`2.8125` / `40340000` instead of0.3125. Ordinal D then T gives L CI=`0.86666667461395263671875` / `3f5dddde` instead of0.20000000298023224. Appendix A shows every division and accumulation. E32/Ediscrete equality A/B is exact even for H/crown whose standalone golden is unknown. Discriminator: filter neighbours only but still grow dead targets; filter only mortality; delete D from immutable roster; include D's DBH in sums or mean vulnerability.

### V-03 — dead target / inert exclusion (D alone and A; area1)

Use D's sizes/age/position above, but pre-inventory-dead cause=null, mechanism=null, deathYear=null. On the internal CI/growth enumerator, ID D is **not evaluated**, not calculated CI0: its target row has absent CI/gain/p, no roll and no new event; its DBH/H/crown/age remain50/25/3/31. As part of A, stand and living result equal V-01. As the sole roster entry, living N0 and BA0, no roll/death/growth; annual advancement is governed by V-09 OPEN policy, not silently committed. If a future explicit “evaluate target D” test seam exists, either a typed proposed `NonLivingTarget` refusal with unchanged state or inert not-evaluated result must be selected in the approved contract; no public API/name is adopted here. Ediscrete. Wrong: publish a computed zero as an actual dead-target CI, accrue age32, new deathYear2, drop D, or contaminate BA.

### V-04 — distance edges, floor and ordinal f32 sum (CI-only; area1)

V-04a roster: T from B plus live `E8`=(8,0,D20), `Eover`=(8.00000095367431640625,0,D20), `Fquarter`=(.25,0,D20); all three use H20/crown2/age30. Eover.x=`0x41000001`, the next representable positive f32 after8. Target T: E8 distance8 included → term.125; Eover distance8.00000095367431640625 excluded; Fquarter distance.25 floored to.5 → term2; CI=2.125 / `40080000`, count2. No annual growth/mortality is invoked by this CI unit probe. A same-position, different-ID neighbour would also be floored to.5; public geometry admissibility is not asserted here.

V-04b roster: T plus live `A`=(.25,0,D200), `B`=(8,0,D1), `C`=(-8,0,D2); defaults H20/crown2/age30. Target T, neighbours in ordinal A,B,C: terms20, f(.00625), f(.0125). First accumulation20, then `20.0062503814697265625` / `41a00ccd`, then **`20.0187511444091796875` / `41a02667`**. Summing B,C,A instead gives **`20.018749237060546875` / `41a02666`**, one f32 ULP smaller. This is an actual positive legal-size three-neighbour counterexample, not a claim that arbitrary sums always differ. E32/Ediscrete; wrong: `<8`, no floor, double accumulation or dictionary/insertion-order iteration. Appendix A has full distance/term trace.

### V-05 — f32 storage versus f64 stand reductions (area=f(.1))

V-05a roster: T=(0,0,D=f(20.1),H=f(20.1),crown=f(2.2),age30,live); L=(4,0,D=f(25.3),H20,crown2,age30,live). This is a **stand-before and target-growth unit probe**. Area=`0.100000001490116119384765625` / `3dcccccd`; D(T)=`20.1000003814697265625` / `41a0cccd`; D(L)=`25.299999237060546875` / `41ca6666`; H(T) has the same bits as its DBH, crown(T)=`400ccccd`. CI(T)=`0.3146766126155853271484375` / `3ea11d4b`.

Nha source-f32=`20` / `41a00000`. Binary64 N/double(area32) instead is `19.99999970197678`; f64 RD consequently `0.00889248007339776` versus profile-correct `0.00889248020590604`, a difference about1.32508e-10. ΣD²=`1044.0999767303474`, BA=`0.08200342041273333`, BAha=`0.8200341919078716`, Dq=`22.848413257055153`. Replacing the area with exact decimal0.1 makes Nha20 here **by coincidence**, so RD does not discriminate that particular mistake; BAha does: `BA/0.1` gives `0.8200342041273332` versus profile `0.8200341919078716` (binary64 evaluation). Never infer equivalence from a coincidental rounded Nha. The script prints exact hex and operations.

V-05b final-storage discriminator: T=(0,0,D10,H20,crown2,age30,live), L=(4,0,D=f(38.7),H20,crown2,age30,live), area1. L.DBH=`38.700000762939453125`. Target T CI=`0.967500030994415283203125` / `3f77ae15`; postDBH=`10.9216594696044921875` / **`412ebf1e`** under DIV and RECIP. Silently using binary64 throughout and rounding only final DBH gives `10.92165851593017578125` / **`412ebf1d`**. That one-bit output difference detects final-only rounding. Heights/crowns remain f32 input fields; actual future values unknown. E32/E64-model; wrong: exact-decimal area, double stems/ha, f32 squares/BA reduction, final-only growth rounding. No 100-tree fixture calculation is involved.

### V-06 — zero, uncapped and capped hazard; conditional RNG (annual-shaped, no asserted victims)

Zero-p control is V-01: pressure0, p0, evaluated-roll count0 and roll=null/notEvaluated. A high-pressure zero-denominator case is V-08: S=V=meanV=0, p0 despite P>0; still no roll.

Nonzero roster: A=(0,0,D25), B=(4,0,D25), defaults H20,crown2,age30,live; declared area=f(2/1400)=`0.001428571413271129131317138671875` / `3abb3ee7`. Both pre-CI.25, both postDBH=`25.90476226806640625` / `41cf3cf4`, age31. Nha1400; post Dq=`25.904762268066406`; RD=`0.8064975705849358`; P=`0.5162439264623394`. Each S=`0.04761904829483721`, V=meanV=`5.141890759335496e-06`, uncapped p=**`0.021320623328740265`** / `0x1.5d512cfb58757p-6`, below.5. Conditional evaluated-roll count2, keys `ADULT-MORT-A` and `ADULT-MORT-B`, seed20261116, targetYear2, RNGversion1. **Actual roll/numerator and actual victims NOT DERIVABLE.** Therefore final N/BA and final death rows are not invented. With zero/one/two hazard victims, possible final N is2/1/0 respectively; no boundary removal for any nonempty subset because the initial postgrowth RD<1.

Comparator-only injected 24-bit numerators: 357700 gives roll=`0.021320581436157227` < p (select); 357701 gives `0.021320641040802002` > p (survive). These bracket p; **neither is asserted to be the RNG output**. Strict comparator equality can separately inject roll=p and must survive, though this particular p is not on the 24-bit grid. Capped probe: V-07's equal-CI=6 four-stem postGrowth seam at area `3a95cbec` has p=.5 each; this is a mortality-only calculation, not a full annual realization.

RNG properties for later tests: same full key/seed/year/version gives identical numerator; roster reorder or unrelated/dead ID cannot consume a shared cursor or change a live ID's roll. Changing year/ID/seed/version changes key material and must exercise the documented hash; **different keys need not guarantee unequal 24-bit outputs**, because collisions are possible. Exact expected changed-key values are NOT DERIVABLE. All engine assertions NOT RUN; Ediscrete and E64-model as declared. Wrong: unconditional roll at zero, random shared cursor, reset year1 every call, assume universal.5 cap, invent a hash from its family name.

### V-07 — strict RD boundary and hazard exclusion (mortality-stage seam)

These are **postGrowth/survivors inputs**, not claimed complete annual trajectories. Roster A,B,C,D: positions (0,0),(.5,0),(0,.5),(.5,.5); each postGrowth DBH25,H20,crown2,age31,live. Supply equal cached pre-growth CI=6 for the boundary-only seam; this is explicitly injected audit input, not asserted CI from the displayed square. Only equality and stable ordinal priority matter for these boundary probes. S values are equal under the declared suppression formula. No real hazard outcome is predicted.

V-07a area=f(4/1868)=`0.002141327597200870513916015625` / `3b0c5584`: survivors N4, ΣD²2500, Dq25, f32 Nha1868, pow(1,2.063)=1, RD=**exactly1**, P1. Condition `RD>1` false → no boundary victim, survivors {A,B,C,D}. This does **not** assert hazard had no victims; the boundary input explicitly supplies four survivors. Under equal vulnerability the pre-boundary hazard probability would be .08, so an uninjected annual victim set is unknown.

V-07b reduce area one f32 step to `0.00214132736437022686004638671875` / `3b0c5583`: Nha=`1868.000244140625` / `44e98002` (two Nha ULPs here); RD=`1.000000130696266`. Remove **A** first by equal-S ordinal tie; recompute N3, Nha=`1401.0001220703125`, Dq25, RD=`0.750000065348133`; stop. Final living {B,C,D}, A dead, one boundary event, no second victim. Do not round RD to display1 before comparing.

V-07c area=f(4/3500)=`0.0011428571306169033050537109375` / `3a95cbec`: boundary survivors start A,B,C,D. RD N4=`1.873661670235546`, N3=`1.4052462526766596`, N2=`0.936830835117773`; remove A then B, stop with C,D. The unused singleton density at same area is `.4684154175588865`; continuing to one would be a bug. Conditional hazard-exclusion variant: assume the separately injected hazard phase already killed **A**, record one hazard event, and feed {B,C,D} to boundary. N3 RD1.4052462526766596; remove **B** only; final {C,D}, one hazard+one boundary event, disjoint victim sets. Re-selecting A or including it in survivor density fails. Events carry targetYear2 and `cause=self-thinning`, with separate mechanisms.

V-07d use V-08's sole survivor D200 at area=f(.001): RD39.05689775126388>1; count1 guard prevents removal. Exact one and discrete tie/set expectations are exact; E64-model values/declared native tolerance for the rest. Wrong: `>=1`, stale density after removal, descending ordinal tie, boundary over original candidates, double event, or forced last-survivor extinction. Appendix A includes all N/area, sums, BA, Dq, power and RD operations.

### V-08 — singleton (annual-shaped, distinct from an empty stand)

Roster S=(0,0,D200,H20,crown2,age30,live); area=f(.001)=`0.001000000047497451305389404296875` / `3a83126f`. CI is genuinely evaluated with empty neighbour set →0/count0. DBH clamp/growth gives gain0, postDBH200, age31. Nha=`999.99993896484375`, ΣD²40000, Dq200, RD=`39.05689775126388`, P=`96.1422443781597`. S=0,V=0,meanV0,p0, roll=null/notEvaluated, no hazard; boundary preserves count1 despite RD>1. Hpost/actual crown NOT DERIVABLE; conditional r=1 crown is in arithmetic output. Final N1, IDs{S}, no events. Source singleton guard is established by #729 §3/§6 and #760 §3. **Owner continuation/API policy remains OPEN**: approve a normal singleton successor with these source calculations, or adopt a separately explicit typed terminal/refusal policy leaving the input unchanged. Do not quietly apply an unresolved empty-stand rule to singleton. No adopted new refusal is claimed. E32/Ediscrete/E64-model. Wrong: force extinction because RD>1, fabricate a singleton neighbour, or draw a roll despite zero mean vulnerability.

### V-09 — zero-living alternatives: OPEN owner choice

Roster D only, same exact dead fields as V-03, null cause/year/provenance=pre-inventory-dead; area1. Living IDs empty, N0,Nha0,BA0, no target/CI/growth/roll/death; D unchanged. **B-O01/OPEN-B2 is unresolved.** Alternative Z-STOP: typed terminal/empty refusal, inputYear1 and revision r unchanged, no successor/report marked committed. Alternative Z-PROGRESS, only if approved: one explicitly labelled empty step1→2/revision r→r+1, no biological or mortality rows, N/BA0; proposed RD0 and Dq/top-height not-applicable/unavailable, not fabricated finite measurements. RD/absence encoding is part of that approval, not assumed from the ordinary nonempty formula (which divides by count). The script emits RD=null for the unchosen zero branch rather than silently adopting RD0. No NaN/Infinity, recruitment or made-up tree. Ediscrete; wrong: automatic year increment while policy open, Dage32, computed CI0 row for D, or NaN committed.

### V-10 — biological age versus ecological death year (conditional policy oracle)

Use B at genesisYear0, age30; use V-12 numeric DBH history and a **test-injected L death at end of Year2**, never a predicted stochastic victim (V-12 pressure0). At Year1 both age31; Year2 both grow once and become32 before mortality. Dying L must carry **age32, deathYear2**, postDBH=`26.81778717041015625` / `41d68ad4`, final H/crown=actual calculated values (NOT DERIVABLE here), cause=self-thinning and the injected phase's mechanism. Under **OPEN-B3 proposed freeze**, later L remains age32, same size and metadata, with exactly one historical event and zero new events for L; T continues age33 at Year3. If freeze is not approved, no numerical later dead-age contract is invented: stop at the owner decision. A pre-inventory D(age31, DBH50,H25,crown3,dead) instead retains null cause/year and no fabricated death event. Neither ecological age2 nor invented deathYear from currentYear is acceptable. Ediscrete/E32 for known DBH; unknown H/crown compare bit equality to the actual death snapshot, not to guessed values.

### V-11 — atomic transaction, refusal and duplicate retry (proposed internal contract)

Roster B, area1, beforeYear1, revision17, opaque lineage E-A and source numeric/model binding K. Bind expectedYear1, expectedRevision17 to **one** proposed request Q; no public field/error vocabulary is adopted. Exact known successful biology is V-01. Capture the original immutable state object, all roster scalar bits, year1, revision17, event-list identity/count0, and published view identity before each case.

| Case | Input / injected condition | Exact observable expected, all NOT RUN |
| --- | --- | --- |
| stale | expectedRevision16 | typed stale refusal; same original object/year/revision/roster/events/view; no roll/computation if checked at admission |
| wrong year | expectedYear0 or request target3 instead of2 | typed wrong-year/jump refusal; identical committed state; no partial report labelled committed |
| precommit failure | correct Q; throw/cancel after all calculations but before swap | original state object/year1/revision17/events0/view unchanged; staged rows never published; no false committed result |
| retry after failure | same expected state Q after previous throw | recompute from identical original; one successor year2/revision18; exact known V-01 values; no carried partial mortality |
| success then duplicate Q | repeat Q with old expected revision/year | either approved typed duplicate/stale refusal or separately approved idempotent return of the same result; **never another biology/commit**; year2/revision18 remains, event count unchanged |
| synchronized overlap | barrier releases two requests for same year1/revision17 | exactly one successor reference swap; other stale/duplicate outcome; zero partial mortality rows; never year3 |

Success report publication follows commit, and stage observations remain attached to that single1→2 transaction. Unknown public result codes/request IDs/digest codecs stay OPEN-B-O05/O06. A future failure vector with actual mortality must additionally stage and discard all candidate events; this no-death B control alone cannot prove mortality rollback. Use V-07's injected event staging under an approved internal seam, preserving original event list/count on failure. Ediscrete; wrong: early year++ publication, partial event append, two successors, per-ID mutation before commit, or reapply-after-success.

### V-12 — ten sequential one-year transitions, no-work proof horizon

B at genesisYear0, both age30, area1. Ten calls use exact prior committed DBH and fresh CI; no display round-trip, forecast, fast-forward, “Year10” interpolation or game cap. This is an exact **DBH/CI/density projection** under the declared profiles, not a complete H/crown/history-byte oracle. H/crown have no feedback into these displayed rules, so their unknown future values do not block the known projection; they still block a complete native-output golden. Both living every step, N2, pressure0, both p0, no rolls/events. PostGrowth=AfterMortality stand; stable positions/IDs/lineage. Age=30+targetYear.

| Transition | CI(L) f32 | CI(T) f32 | DBH(L) cm f32 | DBH(T) cm f32 | RD post / after (f64-model) |
| --- | --- | --- | --- | --- | --- |
| 0→1 | `0.20000000298023223876953125` (`0x3e4ccccd`) | `0.3125` (`0x3ea00000`) | `25.9134616851806640625` (`0x41cf4ec5`) | `20.9411773681640625` (`0x41a78788`) | `0.0009472342760715116` (`0x1.f09fa1e6261c5p-11`) |
| 1→2 | `0.2020299136638641357421875` (`0x3e4ee0ee`) | `0.30936014652252197265625` (`0x3e9e6474`) | `26.81778717041015625` (`0x41d68ad4`) | `21.87404632568359375` (`0x41aefe0c`) | `0.001024458438302125` (`0x1.0c8e3df45880bp-10`) |
| 2→3 | `0.2039135992527008056640625` (`0x3e50ceba`) | `0.3065023720264434814453125` (`0x3e9cede1`) | `27.71309661865234375` (`0x41ddb46c`) | `22.7986278533935546875` (`0x41b66397`) | `0.0011040702576720059` (`0x1.216ce698c49f9p-10`) |
| 3→4 | `0.20566655695438385009765625` (`0x3e529a41`) | `0.3038899600505828857421875` (`0x3e9b9777`) | `28.5995044708251953125` (`0x41e4cbc9`) | `23.7149486541748046875` (`0x41bdb837`) | `0.001185995330693001` (`0x1.36e6cca257097p-10`) |
| 4→5 | `0.207302093505859375` (`0x3e544700`) | `0.3014923632144927978515625` (`0x3e9a5d35`) | `29.477123260498046875` (`0x41ebd126`) | `24.623043060302734375` (`0x41c4fbfe`) | `0.0012701606321448994` (`0x1.4cf70980bf1c5p-10`) |
| 5→6 | `0.20883180201053619384765625` (`0x3e55d801`) | `0.299283921718597412109375` (`0x3e993bbe`) | `30.3460597991943359375` (`0x41f2c4bb`) | `25.5229473114013671875` (`0x41cc2eff`) | `0.0013564941265041502` (`0x1.6398c7a46796ep-10`) |
| 6→7 | `0.21026574075222015380859375` (`0x3e574fe7`) | `0.2972429096698760986328125` (`0x3e983039`) | `31.2064189910888671875` (`0x41f9a6bf`) | `26.414703369140625` (`0x41d35150`) | `0.0014449251521394507` (`0x1.7ac749182c5f3p-10`) |
| 7→8 | `0.21161274611949920654296875` (`0x3e58b103`) | `0.295350849628448486328125` (`0x3e97383a`) | `32.058300018310546875` (`0x42003bb3`) | `27.298358917236328125` (`0x41da630a`) | `0.00153538425432956` (`0x1.927de4a2f5488p-10`) |
| 8→9 | `0.2128805816173553466796875` (`0x3e59fd5e`) | `0.2935918271541595458984375` (`0x3e9651ab`) | `32.90180206298828125` (`0x42039b72`) | `28.173961639404296875` (`0x41e16446`) | `0.0016278032500738145` (`0x1.aab806e4c03a7p-10`) |
| 9→10 | `0.2140761315822601318359375` (`0x3e5b36c6`) | `0.2919522225856781005859375` (`0x3e957ac3`) | `33.73702239990234375` (`0x4206f2b6`) | `29.0415630340576171875` (`0x41e8551f`) | `0.0017221153233496118` (`0x1.c37133f993955p-10`) |

Every row is independently calculated from the immediately preceding row, not a fresh Year1 run. For targetYear2 L's DIV gain=`0.904325306415557861328125` / `3f6781dd`, RECIP=`0.9043252468109130859375` / `3f6781dc`; both commit DBH=`41d68ad4`. For targetYear3 T's DIV gain=`0.9245822429656982421875` / `3f6cb16c`, RECIP=`0.924582183361053466796875` / `3f6cb16b`; both commit DBH=`41b66397`. All ten committed DBH states in the table agree under both profiles. Exact native **gain diagnostic** oracle at those sites remains conditional on source confirmation.

Death-present chronology branch: conditionally inject L's single death after its Year2 growth (V-10). It still competed in **Year2 pre-growth**: T CI=`0.30936014652252197265625` / `3e9e6474`, T postDBH=`21.87404632568359375` / `41aefe0c`. Do not release T for a second Year2 growth. At Year3 living set={T}, recompute CI0; gain=`0.9812595844268798828125`, postDBH=`22.8553066253662109375` / `41b6d7ab`, age33. Wrong cached Year2 CI yields postDBH=`22.798130035400390625` / `41b66292`; recomputing but erroneously retaining dead L yields the all-living Year3 table value=`22.7986278533935546875` / `41b66397`. These are three distinct, discriminating numbers. This death is a **phase-seam test injection**, impossible as a natural hazard outcome at this area's p0, not a claimed engine mortality result.

Protected Year1 anchor, boundary-of-evidence only: published #729/#732 state 100 living age31, zero deaths, RD0.6375836886879149 and old one-year stdout45,980 bytes, SHA-256 `574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`. This pack **did not** read/hash those bytes or recalculate that stand. A later lawful native comparison must preserve that separately qualified0→1 path and old second-call refusal; these two-tree synthetic0→1 numbers neither replace nor claim to reproduce it. E32/Ediscrete/E64-model; wrong: years reset, cross-year cached CI/pressure, display-rounded feedback, second mortality event, or stitched independent Year1 invocations.

### V-13 — identity and permutation (B/A, area1)

Same B scalars/IDs and binding K with input list [T,L] versus [L,T] must sort to ordinal [L,T] and give V-01. Add/remove historical dead D as V-02: live results unchanged, full roster identity appropriately different. Changing a tree ID's case or full spelling changes identity/key material, even if geometry is identical; exact new roll is NOT DERIVABLE. Duplicate ID T/T must be refused before commit, not merged by a dictionary. Two private lineages E-A/E-B with equal fixture/model/seed/year/revision17 remain distinct; an E-A expected token cannot advance E-B merely because their display/revision matches. Wrong seed/RNG/growth/numeric-policy binding refuses with unchanged state. Digest serialization/byte equality is NOT DERIVABLE. Ediscrete; wrong: identify trees by array index/position, fold ID case, accept a bare UI revision across runs, use a display JSON hash as canonical lineage.

## 4. Fully worked arithmetic appendix A

Each operation below is from the **independent synthetic model**. `bits` mark a rounded f32 result, `0x1...p...` a binary64 result. A square/running sum in a stand trace is binary64 after widening DBH. Where the source report leaves grouping unclear, the profile is explicitly conditional; these are not claimed native bits. The script in Appendix B reproduces all entries and additional V-06/V-08/V-12 diagnostics.

#### V01_L CI

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `0.20000000298023223876953125` (`0x3e4ccccd`) |

#### V01_L every spatial/CI operation

Neighbour `T`: included.

| Operation | Value |
| --- | --- |
| dx | `4` (`0x40800000`) |
| dz | `0` (`0x00000000`) |
| xx | `16` (`0x41800000`) |
| zz | `0` (`0x00000000`) |
| sq | `16` (`0x41800000`) |
| distance | `4` (`0x40800000`) |
| den | `25` (`0x41c80000`) |
| ratio | `0.800000011920928955078125` (`0x3f4ccccd`) |
| floor | `4` (`0x40800000`) |
| term | `0.20000000298023223876953125` (`0x3e4ccccd`) |
| acc | `0.20000000298023223876953125` (`0x3e4ccccd`) |

#### V01_L every DBH/crown operation (r=1 injected)

| Operation / result | Expected independent-model value |
| --- | --- |
| div120 | `0.20833332836627960205078125` (`0x3e555555`) |
| oneMinus | `0.791666686534881591796875` (`0x3f4aaaab`) |
| clamp | `0.791666686534881591796875` (`0x3f4aaaab`) |
| siteProduct | `1.2000000476837158203125` (`0x3f99999a`) |
| numerator | `0.9500000476837158203125` (`0x3f733334`) |
| ciDiv5 | `0.039999999105930328369140625` (`0x3d23d70a`) |
| denominator | `1.03999996185302734375` (`0x3f851eb8`) |
| gain_div | `0.913461625576019287109375` (`0x3f69d89f`) |
| reciprocal | `0.961538493633270263671875` (`0x3f762763`) |
| gain_mul_reciprocal | `0.913461625576019287109375` (`0x3f69d89f`) |
| postDBH_div | `25.9134616851806640625` (`0x41cf4ec5`) |
| postDBH_mul_reciprocal | `25.9134616851806640625` (`0x41cf4ec5`) |
| crown_product | `1.97849285602569580078125` (`0x3ffd3f41`) |
| crown_base | `2.919992923736572265625` (`0x403ae12a`) |
| crown_r1_target | `2.919992923736572265625` (`0x403ae12a`) |
| crown_r1_difference | `0.7199928760528564453125` (`0x3f385174`) |
| crown_r1_relax | `0.1079989373683929443359375` (`0x3ddd2e8c`) |
| crown_r1_final | `2.3079988956451416015625` (`0x4013b641`) |

#### V01_T CI

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `0.3125` (`0x3ea00000`) |

#### V01_T every spatial/CI operation

Neighbour `L`: included.

| Operation | Value |
| --- | --- |
| dx | `-4` (`0xc0800000`) |
| dz | `0` (`0x00000000`) |
| xx | `16` (`0x41800000`) |
| zz | `0` (`0x00000000`) |
| sq | `16` (`0x41800000`) |
| distance | `4` (`0x40800000`) |
| den | `20` (`0x41a00000`) |
| ratio | `1.25` (`0x3fa00000`) |
| floor | `4` (`0x40800000`) |
| term | `0.3125` (`0x3ea00000`) |
| acc | `0.3125` (`0x3ea00000`) |

#### V01_T every DBH/crown operation (r=1 injected)

| Operation / result | Expected independent-model value |
| --- | --- |
| div120 | `0.16666667163372039794921875` (`0x3e2aaaab`) |
| oneMinus | `0.833333313465118408203125` (`0x3f555555`) |
| clamp | `0.833333313465118408203125` (`0x3f555555`) |
| siteProduct | `1.2000000476837158203125` (`0x3f99999a`) |
| numerator | `1` (`0x3f800000`) |
| ciDiv5 | `0.0625` (`0x3d800000`) |
| denominator | `1.0625` (`0x3f880000`) |
| gain_div | `0.941176474094390869140625` (`0x3f70f0f1`) |
| reciprocal | `0.941176474094390869140625` (`0x3f70f0f1`) |
| gain_mul_reciprocal | `0.941176474094390869140625` (`0x3f70f0f1`) |
| postDBH_div | `20.9411773681640625` (`0x41a78788`) |
| postDBH_mul_reciprocal | `20.9411773681640625` (`0x41a78788`) |
| crown_product | `1.59885895252227783203125` (`0x3fcca769`) |
| crown_base | `2.540359020233154296875` (`0x4022953e`) |
| crown_r1_target | `2.540359020233154296875` (`0x4022953e`) |
| crown_r1_difference | `0.540359020233154296875` (`0x3f0a54f8`) |
| crown_r1_relax | `0.08105385303497314453125` (`0x3da5ff90`) |
| crown_r1_final | `2.08105373382568359375` (`0x40052ffc`) |

#### V01_before aggregate operations

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `2` |
| area | `1` (`0x3f800000`) |
| Nha | `2` (`0x40000000`) |
| sumSquares | `1025.0` (`0x1.0040000000000p+10`) |
| BA | `0.08050331174823845` (`0x1.49bdd732daa19p-4`) |
| BAha | `0.08050331174823845` (`0x1.49bdd732daa19p-4`) |
| meanSquare | `512.5` (`0x1.0040000000000p+9`) |
| Dq | `22.638462845343543` (`0x1.6a3724d10762dp+4`) |
| DqDiv25 | `0.9055385138137417` (`0x1.cfa2be7c326a0p-1`) |
| power | `0.8148900007187067` (`0x1.a139431dd938dp-1`) |
| densityProduct | `1.6297800014374133` (`0x1.a139431dd938dp+0`) |
| RD | `0.0008724732341742041` (`0x1.c96d600f2ed6bp-11`) |
| P | `0.0` (`0x0.0p+0`) |

#### V01_before square/sum 1

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `625.0` (`0x1.3880000000000p+9`) |

#### V01_before square/sum 2

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `20` (`0x41a00000`) |
| square | `400.0` (`0x1.9000000000000p+8`) |
| runningSum | `1025.0` (`0x1.0040000000000p+10`) |

#### V01_post aggregate operations

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `2` |
| area | `1` (`0x3f800000`) |
| Nha | `2` (`0x40000000`) |
| sumSquares | `1110.040406074233` (`0x1.158296035bd90p+10`) |
| BA | `0.08718236962276603` (`0x1.6519572514bdfp-4`) |
| BAha | `0.08718236962276603` (`0x1.6519572514bdfp-4`) |
| meanSquare | `555.0202030371165` (`0x1.158296035bd90p+9`) |
| Dq | `23.5588667604602` (`0x1.78f11e45aff7fp+4`) |
| DqDiv25 | `0.942354670418408` (`0x1.e27c4fb55c1eap-1`) |
| power | `0.8847168138507918` (`0x1.c4f99a2b6bc2dp-1`) |
| densityProduct | `1.7694336277015836` (`0x1.c4f99a2b6bc2dp+0`) |
| RD | `0.0009472342760715116` (`0x1.f09fa1e6261c5p-11`) |
| P | `0.0` (`0x0.0p+0`) |

#### V01_post square/sum 1

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25.9134616851806640625` (`0x41cf4ec5`) |
| square | `671.5074965093263` (`0x1.4fc0f5a547320p+9`) |
| runningSum | `671.5074965093263` (`0x1.4fc0f5a547320p+9`) |

#### V01_post square/sum 2

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `20.9411773681640625` (`0x41a78788`) |
| square | `438.53290956490673` (`0x1.b6886cc2e1000p+8`) |
| runningSum | `1110.040406074233` (`0x1.158296035bd90p+10`) |

#### V01 L suppression/hazard arithmetic

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `0.20000000298023223876953125` (`0x3e4ccccd`) |
| ciDiv5 | `0.039999999105930328369140625` (`0x3d23d70a`) |
| S | `0.03846153763492077` (`0x1.3b13b0c977ab0p-5`) |
| V | `2.188298540911915e-06` (`0x1.25b55d86b6910p-19`) |
| meanV | `7.080667631107775e-06` (`0x1.db2cf1923f719p-18`) |
| uncapped | `0.0` (`0x0.0p+0`) |
| p | `0.0` (`0x0.0p+0`) |

#### V01 T suppression/hazard arithmetic

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `0.3125` (`0x3ea00000`) |
| ciDiv5 | `0.0625` (`0x3d800000`) |
| S | `0.05882352941176472` (`0x1.e1e1e1e1e1e20p-5`) |
| V | `1.1973036721303634e-05` (`0x1.91bf9a3091cd5p-17`) |
| meanV | `7.080667631107775e-06` (`0x1.db2cf1923f719p-18`) |
| uncapped | `0.0` (`0x0.0p+0`) |
| p | `0.0` (`0x0.0p+0`) |

V-02 correct A excludes D *before* arithmetic, so its entire live trace is exactly the V-01 trace. Wrong inclusion is traced next; this is a counterfactual, not an allowed implementation.

#### V02_wrong_L

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `0.86666667461395263671875` (`0x3f5dddde`) |

#### V02_wrong_L incorrect inclusion operations

Neighbour `D`: included.

| Operation | Value |
| --- | --- |
| dx | `3` (`0x40400000`) |
| dz | `0` (`0x00000000`) |
| xx | `9` (`0x41100000`) |
| zz | `0` (`0x00000000`) |
| sq | `9` (`0x41100000`) |
| distance | `3` (`0x40400000`) |
| den | `25` (`0x41c80000`) |
| ratio | `2` (`0x40000000`) |
| floor | `3` (`0x40400000`) |
| term | `0.666666686534881591796875` (`0x3f2aaaab`) |
| acc | `0.666666686534881591796875` (`0x3f2aaaab`) |

Neighbour `T`: included.

| Operation | Value |
| --- | --- |
| dx | `4` (`0x40800000`) |
| dz | `0` (`0x00000000`) |
| xx | `16` (`0x41800000`) |
| zz | `0` (`0x00000000`) |
| sq | `16` (`0x41800000`) |
| distance | `4` (`0x40800000`) |
| den | `25` (`0x41c80000`) |
| ratio | `0.800000011920928955078125` (`0x3f4ccccd`) |
| floor | `4` (`0x40800000`) |
| term | `0.20000000298023223876953125` (`0x3e4ccccd`) |
| acc | `0.86666667461395263671875` (`0x3f5dddde`) |

#### V02_wrong_T

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `2.8125` (`0x40340000`) |

#### V02_wrong_T incorrect inclusion operations

Neighbour `D`: included.

| Operation | Value |
| --- | --- |
| dx | `-1` (`0xbf800000`) |
| dz | `0` (`0x00000000`) |
| xx | `1` (`0x3f800000`) |
| zz | `0` (`0x00000000`) |
| sq | `1` (`0x3f800000`) |
| distance | `1` (`0x3f800000`) |
| den | `20` (`0x41a00000`) |
| ratio | `2.5` (`0x40200000`) |
| floor | `1` (`0x3f800000`) |
| term | `2.5` (`0x40200000`) |
| acc | `2.5` (`0x40200000`) |

Neighbour `L`: included.

| Operation | Value |
| --- | --- |
| dx | `-4` (`0xc0800000`) |
| dz | `0` (`0x00000000`) |
| xx | `16` (`0x41800000`) |
| zz | `0` (`0x00000000`) |
| sq | `16` (`0x41800000`) |
| distance | `4` (`0x40800000`) |
| den | `20` (`0x41a00000`) |
| ratio | `1.25` (`0x3fa00000`) |
| floor | `4` (`0x40800000`) |
| term | `0.3125` (`0x3ea00000`) |
| acc | `2.8125` (`0x40340000`) |

#### V04_edge

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `2.125` (`0x40080000`) |

#### V04_edge distance/ratio/floor/accumulation operations

Neighbour `E8`: included.

| Operation | Value |
| --- | --- |
| dx | `-8` (`0xc1000000`) |
| dz | `0` (`0x00000000`) |
| xx | `64` (`0x42800000`) |
| zz | `0` (`0x00000000`) |
| sq | `64` (`0x42800000`) |
| distance | `8` (`0x41000000`) |
| den | `20` (`0x41a00000`) |
| ratio | `1` (`0x3f800000`) |
| floor | `8` (`0x41000000`) |
| term | `0.125` (`0x3e000000`) |
| acc | `0.125` (`0x3e000000`) |

Neighbour `Eover`: excluded.

| Operation | Value |
| --- | --- |
| dx | `-8.00000095367431640625` (`0xc1000001`) |
| dz | `0` (`0x00000000`) |
| xx | `64.0000152587890625` (`0x42800002`) |
| zz | `0` (`0x00000000`) |
| sq | `64.0000152587890625` (`0x42800002`) |
| distance | `8.00000095367431640625` (`0x41000001`) |

Neighbour `Fquarter`: included.

| Operation | Value |
| --- | --- |
| dx | `-0.25` (`0xbe800000`) |
| dz | `0` (`0x00000000`) |
| xx | `0.0625` (`0x3d800000`) |
| zz | `0` (`0x00000000`) |
| sq | `0.0625` (`0x3d800000`) |
| distance | `0.25` (`0x3e800000`) |
| den | `20` (`0x41a00000`) |
| ratio | `1` (`0x3f800000`) |
| floor | `0.5` (`0x3f000000`) |
| term | `2` (`0x40000000`) |
| acc | `2.125` (`0x40080000`) |

#### V04_order

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `20.0187511444091796875` (`0x41a02667`) |
| BCthenA | `20.018749237060546875` (`0x41a02666`) |

#### V04_order distance/ratio/floor/accumulation operations

Neighbour `A`: included.

| Operation | Value |
| --- | --- |
| dx | `-0.25` (`0xbe800000`) |
| dz | `0` (`0x00000000`) |
| xx | `0.0625` (`0x3d800000`) |
| zz | `0` (`0x00000000`) |
| sq | `0.0625` (`0x3d800000`) |
| distance | `0.25` (`0x3e800000`) |
| den | `20` (`0x41a00000`) |
| ratio | `10` (`0x41200000`) |
| floor | `0.5` (`0x3f000000`) |
| term | `20` (`0x41a00000`) |
| acc | `20` (`0x41a00000`) |

Neighbour `B`: included.

| Operation | Value |
| --- | --- |
| dx | `-8` (`0xc1000000`) |
| dz | `0` (`0x00000000`) |
| xx | `64` (`0x42800000`) |
| zz | `0` (`0x00000000`) |
| sq | `64` (`0x42800000`) |
| distance | `8` (`0x41000000`) |
| den | `20` (`0x41a00000`) |
| ratio | `0.0500000007450580596923828125` (`0x3d4ccccd`) |
| floor | `8` (`0x41000000`) |
| term | `0.0062500000931322574615478515625` (`0x3bcccccd`) |
| acc | `20.0062503814697265625` (`0x41a00ccd`) |

Neighbour `C`: included.

| Operation | Value |
| --- | --- |
| dx | `8` (`0x41000000`) |
| dz | `0` (`0x00000000`) |
| xx | `64` (`0x42800000`) |
| zz | `0` (`0x00000000`) |
| sq | `64` (`0x42800000`) |
| distance | `8` (`0x41000000`) |
| den | `20` (`0x41a00000`) |
| ratio | `0.100000001490116119384765625` (`0x3dcccccd`) |
| floor | `8` (`0x41000000`) |
| term | `0.012500000186264514923095703125` (`0x3c4ccccd`) |
| acc | `20.0187511444091796875` (`0x41a02667`) |

#### V05 inputs

| Operation / result | Expected independent-model value |
| --- | --- |
| area | `0.100000001490116119384765625` (`0x3dcccccd`) |
| D1 | `20.1000003814697265625` (`0x41a0cccd`) |
| D2 | `25.299999237060546875` (`0x41ca6666`) |
| H | `20.1000003814697265625` (`0x41a0cccd`) |
| crown | `2.2000000476837158203125` (`0x400ccccd`) |

#### V05 correct stand operations

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `2` |
| area | `0.100000001490116119384765625` (`0x3dcccccd`) |
| Nha | `20` (`0x41a00000`) |
| sumSquares | `1044.0999767303474` (`0x1.05066604cccd0p+10`) |
| BA | `0.08200342041273333` (`0x1.4fe2d18d5343fp-4`) |
| BAha | `0.8200341919078716` (`0x1.a3db8587b1339p-1`) |
| meanSquare | `522.0499883651737` (`0x1.05066604cccd0p+9`) |
| Dq | `22.848413257055153` (`0x1.6d9319c788b73p+4`) |
| DqDiv25 | `0.9139365302822061` (`0x1.d3ef7d285d137p-1`) |
| power | `0.8305576512316241` (`0x1.a93eda3af6fd1p-1`) |
| densityProduct | `16.611153024632483` (`0x1.09c74864da5e3p+4`) |
| RD | `0.00889248020590604` (`0x1.236387d514afap-7`) |
| P | `0.0` (`0x0.0p+0`) |

#### V05 square/sum 1

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `20.1000003814697265625` (`0x41a0cccd`) |
| square | `404.01001533508315` (`0x1.9402905d70a40p+8`) |
| runningSum | `404.01001533508315` (`0x1.9402905d70a40p+8`) |

#### V05 square/sum 2

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25.299999237060546875` (`0x41ca6666`) |
| square | `640.0899613952643` (`0x1.400b83dae1480p+9`) |
| runningSum | `1044.0999767303474` (`0x1.05066604cccd0p+10`) |

#### V05 substituted-f64 counterfactuals

| Operation / result | Expected independent-model value |
| --- | --- |
| Nha_f64 | `19.99999970197678` (`0x1.3fffffb000001p+4`) |
| RD_f64_Nha | `0.00889248007339776` (`0x1.2363878c3bcdbp-7`) |
| Nha_decimal_area | `20.0` (`0x1.4000000000000p+4`) |
| RD_decimal_area | `0.00889248020590604` (`0x1.236387d514afap-7`) |
| CI_f32 | `0.3146766126155853271484375` (`0x3ea11d4b`) |
| CI_f64 | `0.31467660145400694` (`0x1.423a95403edbfp-2`) |
| CI_f64_then_f32 | `0.3146766126155853271484375` (`0x3ea11d4b`) |
| DBH_all_f64_then_f32 | `21.0398502349853515625` (`0x41a8519d`) |
| DBH_all_f64 | `21.039850602941332` (`0x1.50a33a62c5be1p+4`) |

#### V05 every target growth/crown-unit operation

| Operation / result | Expected independent-model value |
| --- | --- |
| div120 | `0.16750000417232513427734375` (`0x3e2b851f`) |
| oneMinus | `0.832499980926513671875` (`0x3f551eb8`) |
| clamp | `0.832499980926513671875` (`0x3f551eb8`) |
| siteProduct | `1.2000000476837158203125` (`0x3f99999a`) |
| numerator | `0.999000012874603271484375` (`0x3f7fbe77`) |
| ciDiv5 | `0.0629353225231170654296875` (`0x3d80e43c`) |
| denominator | `1.062935352325439453125` (`0x3f880e44`) |
| gain_div | `0.93985021114349365234375` (`0x3f709a06`) |
| reciprocal | `0.94079101085662841796875` (`0x3f70d7ae`) |
| gain_mul_reciprocal | `0.93985021114349365234375` (`0x3f709a06`) |
| postDBH_div | `21.0398502349853515625` (`0x41a8519d`) |
| postDBH_mul_reciprocal | `21.0398502349853515625` (`0x41a8519d`) |
| crown_product | `1.6063926219940185546875` (`0x3fcd9e46`) |
| crown_base | `2.54789257049560546875` (`0x402310ac`) |
| crown_r1_target | `2.54789257049560546875` (`0x402310ac`) |
| crown_r1_difference | `0.3478925228118896484375` (`0x3eb21ef8`) |
| crown_r1_relax | `0.05218388140201568603515625` (`0x3d55bec4`) |
| crown_r1_final | `2.2521839141845703125` (`0x401023c8`) |

#### V05 final-bit counterexample

| Operation / result | Expected independent-model value |
| --- | --- |
| CI | `0.967500030994415283203125` (`0x3f77ae15`) |
| all64_CI | `0.9675000190734864` (`0x1.ef5c29999999ap-1`) |
| all64_post | `10.92165851593017578125` (`0x412ebf1d`) |

#### V05 final-bit growth/crown-unit operations

| Operation / result | Expected independent-model value |
| --- | --- |
| div120 | `0.083333335816860198974609375` (`0x3daaaaab`) |
| oneMinus | `0.916666686534881591796875` (`0x3f6aaaab`) |
| clamp | `0.916666686534881591796875` (`0x3f6aaaab`) |
| siteProduct | `1.2000000476837158203125` (`0x3f99999a`) |
| numerator | `1.10000002384185791015625` (`0x3f8ccccd`) |
| ciDiv5 | `0.1935000121593475341796875` (`0x3e4624de`) |
| denominator | `1.193500041961669921875` (`0x3f98c49c`) |
| gain_div | `0.921658992767333984375` (`0x3f6bf1d8`) |
| reciprocal | `0.8378717899322509765625` (`0x3f567ec4`) |
| gain_mul_reciprocal | `0.921658992767333984375` (`0x3f6bf1d8`) |
| postDBH_div | `10.9216594696044921875` (`0x412ebf1e`) |
| postDBH_mul_reciprocal | `10.9216594696044921875` (`0x412ebf1e`) |
| crown_product | `0.8338687419891357421875` (`0x3f55786c`) |
| crown_base | `1.77536869049072265625` (`0x3fe33f48`) |
| crown_r1_target | `1.77536869049072265625` (`0x3fe33f48`) |
| crown_r1_difference | `-0.22463130950927734375` (`0xbe6605c0`) |
| crown_r1_relax | `-0.03369469940662384033203125` (`0xbd0a0374`) |
| crown_r1_final | `1.966305255889892578125` (`0x3ffbafe4`) |

#### V07_exact every stand operation

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `4` |
| area | `0.002141327597200870513916015625` (`0x3b0c5584`) |
| Nha | `1868` (`0x44e98000`) |
| sumSquares | `2500.0` (`0x1.3880000000000p+11`) |
| BA | `0.19634954084936207` (`0x1.921fb54442d18p-3`) |
| BAha | `91.69523668682406` (`0x1.6ec7ec20438e1p+6`) |
| meanSquare | `625.0` (`0x1.3880000000000p+9`) |
| Dq | `25.0` (`0x1.9000000000000p+4`) |
| DqDiv25 | `1.0` (`0x1.0000000000000p+0`) |
| power | `1.0` (`0x1.0000000000000p+0`) |
| densityProduct | `1868.0` (`0x1.d300000000000p+10`) |
| RD | `1.0` (`0x1.0000000000000p+0`) |
| P | `1.0` (`0x1.0000000000000p+0`) |

#### V07_exact square/sum 1

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `625.0` (`0x1.3880000000000p+9`) |

#### V07_exact square/sum 2

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `1250.0` (`0x1.3880000000000p+10`) |

#### V07_exact square/sum 3

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `1875.0` (`0x1.d4c0000000000p+10`) |

#### V07_exact square/sum 4

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `2500.0` (`0x1.3880000000000p+11`) |

#### V07_above every stand operation

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `4` |
| area | `0.00214132736437022686004638671875` (`0x3b0c5583`) |
| Nha | `1868.000244140625` (`0x44e98002`) |
| sumSquares | `2500.0` (`0x1.3880000000000p+11`) |
| BA | `0.19634954084936207` (`0x1.921fb54442d18p-3`) |
| BAha | `91.69524665702355` (`0x1.6ec7eebd5a472p+6`) |
| meanSquare | `625.0` (`0x1.3880000000000p+9`) |
| Dq | `25.0` (`0x1.9000000000000p+4`) |
| DqDiv25 | `1.0` (`0x1.0000000000000p+0`) |
| power | `1.0` (`0x1.0000000000000p+0`) |
| densityProduct | `1868.000244140625` (`0x1.d300040000000p+10`) |
| RD | `1.000000130696266` (`0x1.0000023156107p+0`) |
| P | `1.000000326740665` (`0x1.0000057b57291p+0`) |

#### V07_above square/sum 1

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `625.0` (`0x1.3880000000000p+9`) |

#### V07_above square/sum 2

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `1250.0` (`0x1.3880000000000p+10`) |

#### V07_above square/sum 3

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `1875.0` (`0x1.d4c0000000000p+10`) |

#### V07_above square/sum 4

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `2500.0` (`0x1.3880000000000p+11`) |

#### V07_after_one every stand operation

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `3` |
| area | `0.00214132736437022686004638671875` (`0x3b0c5583`) |
| Nha | `1401.0001220703125` (`0x44af2001`) |
| sumSquares | `1875.0` (`0x1.d4c0000000000p+10`) |
| BA | `0.14726215563702155` (`0x1.2d97c7f3321d2p-3`) |
| BAha | `68.77143499276765` (`0x1.1315f30e03b55p+6`) |
| meanSquare | `625.0` (`0x1.3880000000000p+9`) |
| Dq | `25.0` (`0x1.9000000000000p+4`) |
| DqDiv25 | `1.0` (`0x1.0000000000000p+0`) |
| power | `1.0` (`0x1.0000000000000p+0`) |
| densityProduct | `1401.0001220703125` (`0x1.5e40020000000p+10`) |
| RD | `0.750000065348133` (`0x1.8000023156107p-1`) |
| P | `0.3750001633703326` (`0x1.80000af6ae524p-2`) |

#### V07_after_one square/sum 1

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `625.0` (`0x1.3880000000000p+9`) |

#### V07_after_one square/sum 2

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `1250.0` (`0x1.3880000000000p+10`) |

#### V07_after_one square/sum 3

| Operation / result | Expected independent-model value |
| --- | --- |
| d | `25` (`0x41c80000`) |
| square | `625.0` (`0x1.3880000000000p+9`) |
| runningSum | `1875.0` (`0x1.d4c0000000000p+10`) |

#### V07 loop survivors=4

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `4` |
| area | `0.0011428571306169033050537109375` (`0x3a95cbec`) |
| Nha | `3500` (`0x455ac000`) |
| sumSquares | `2500.0` (`0x1.3880000000000p+11`) |
| BA | `0.19634954084936207` (`0x1.921fb54442d18p-3`) |
| BAha | `171.80585008326847` (`0x1.579c9861d23bep+7`) |
| meanSquare | `625.0` (`0x1.3880000000000p+9`) |
| Dq | `25.0` (`0x1.9000000000000p+4`) |
| DqDiv25 | `1.0` (`0x1.0000000000000p+0`) |
| power | `1.0` (`0x1.0000000000000p+0`) |
| densityProduct | `3500.0` (`0x1.b580000000000p+11`) |
| RD | `1.873661670235546` (`0x1.dfa84a8d6e2f2p+0`) |
| P | `3.1841541755888647` (`0x1.97925d30c9baep+1`) |

#### V07 loop survivors=3

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `3` |
| area | `0.0011428571306169033050537109375` (`0x3a95cbec`) |
| Nha | `2625` (`0x45241000`) |
| sumSquares | `1875.0` (`0x1.d4c0000000000p+10`) |
| BA | `0.14726215563702155` (`0x1.2d97c7f3321d2p-3`) |
| BAha | `128.85438756245136` (`0x1.01b572495dacfp+7`) |
| meanSquare | `625.0` (`0x1.3880000000000p+9`) |
| Dq | `25.0` (`0x1.9000000000000p+4`) |
| DqDiv25 | `1.0` (`0x1.0000000000000p+0`) |
| power | `1.0` (`0x1.0000000000000p+0`) |
| densityProduct | `2625.0` (`0x1.4820000000000p+11`) |
| RD | `1.4052462526766596` (`0x1.67be37ea12a36p+0`) |
| P | `2.0131156316916488` (`0x1.01adc5e4974c3p+1`) |

#### V07 loop survivors=2

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `2` |
| area | `0.0011428571306169033050537109375` (`0x3a95cbec`) |
| Nha | `1750` (`0x44dac000`) |
| sumSquares | `1250.0` (`0x1.3880000000000p+10`) |
| BA | `0.09817477042468103` (`0x1.921fb54442d18p-4`) |
| BAha | `85.90292504163423` (`0x1.579c9861d23bep+6`) |
| meanSquare | `625.0` (`0x1.3880000000000p+9`) |
| Dq | `25.0` (`0x1.9000000000000p+4`) |
| DqDiv25 | `1.0` (`0x1.0000000000000p+0`) |
| power | `1.0` (`0x1.0000000000000p+0`) |
| densityProduct | `1750.0` (`0x1.b580000000000p+10`) |
| RD | `0.936830835117773` (`0x1.dfa84a8d6e2f2p-1`) |
| P | `0.8420770877944325` (`0x1.af24ba619375dp-1`) |

#### V07 loop survivors=1

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `1` |
| area | `0.0011428571306169033050537109375` (`0x3a95cbec`) |
| Nha | `875` (`0x445ac000`) |
| sumSquares | `625.0` (`0x1.3880000000000p+9`) |
| BA | `0.04908738521234052` (`0x1.921fb54442d18p-5`) |
| BAha | `42.95146252081712` (`0x1.579c9861d23bep+5`) |
| meanSquare | `625.0` (`0x1.3880000000000p+9`) |
| Dq | `25.0` (`0x1.9000000000000p+4`) |
| DqDiv25 | `1.0` (`0x1.0000000000000p+0`) |
| power | `1.0` (`0x1.0000000000000p+0`) |
| densityProduct | `875.0` (`0x1.b580000000000p+9`) |
| RD | `0.4684154175588865` (`0x1.dfa84a8d6e2f2p-2`) |
| P | `0.0` (`0x0.0p+0`) |

#### V07 last survivor

| Operation / result | Expected independent-model value |
| --- | --- |
| N | `1` |
| area | `0.001000000047497451305389404296875` (`0x3a83126f`) |
| Nha | `999.99993896484375` (`0x4479ffff`) |
| sumSquares | `40000.0` (`0x1.3880000000000p+15`) |
| BA | `3.141592653589793` (`0x1.921fb54442d18p+1`) |
| BAha | `3141.5925043721563` (`0x1.88b2f5cbbaa4cp+11`) |
| meanSquare | `40000.0` (`0x1.3880000000000p+15`) |
| Dq | `200.0` (`0x1.9000000000000p+7`) |
| DqDiv25 | `8.0` (`0x1.0000000000000p+3`) |
| power | `72.95828945238152` (`0x1.23d549d488525p+6`) |
| densityProduct | `72958.28499936093` (`0x1.1cfe48f5b7d69p+16`) |
| RD | `39.05689775126388` (`0x1.387486cee7279p+5`) |
| P | `96.1422443781597` (`0x1.8091a882a0f17p+6`) |

For equal-D25 boundary stems every individual widened square is exactly625, so the loop's running binary64 sum is N*625 (2500→1875→1250→625), mean625, sqrt25 and Dq/25=1. Thus pow=1 and RD=Nha/1868; the above tables expand each count. Recompute at each removal, not once at the start. The one-ULP-smaller area is a concrete representable way to cross the strict boundary, with no search over the canonical fixture.

## 5. NOT DERIVABLE register and open decisions

| ID | Missing datum / scope | Why no exact native value is claimed; required later resolution |
| --- | --- | --- |
| ND-01 | arbitrary `SimulationRandom.Roll` hash/key packing, mixed integer and 24-bit numerator | Reports name FNV/SplitMix and publish a few Year1 witnesses, not a complete general key-composition algorithm for arbitrary synthetic A/B/T/L/year2–10. Exact rolls/victims are NOT DERIVABLE; do not extrapolate the published P30-001/year1 numerator to other IDs or years. Later lawful pinned source inspection plus independent integer derivation is required. |
| ND-02 | exact Class III relative-height function/coefficients, argument/cast sites | #723 prints an abbreviated `34.362…` and one anchor; #729 gives NextHeight then delta/apply/clamp, not every coefficient/evaluation. These do not define synthetic Hpost bits. Source coefficients and f32/double evaluation sites are required. |
| ND-03 | actual crown `response` from cached CI/site/model and clamps | A symbol and qualitative dependence do not define a function. `r=1` probes are conditional arithmetic, not biology. Later source review must pin response and numeric sites, then replace UNKNOWN expected C with independently derived values. A/B equality remains exact now. |
| ND-04 | every native DBH/BA/pow evaluation site and JIT/libm rounding | DIV/RECIP gain differences demonstrate why algebra alone is insufficient; reports specify important storage kinds but not every future evaluation choice. Native gain last bits at the noted sites, BA grouping and general f64 transcendental last bits remain conditional. Written numeric/runtime policy and lawful inspection needed. |
| ND-05 | actual V-06 annual survivors and any synthetic natural Year2 victim | Nonzero probability does not determine the deterministic roll; no victim set is inferred. V-07/V-10 mortality seam injections are clearly labelled, not stochastic outcomes. |
| ND-06 | complete ten-step per-tree H/crown and native output/report bytes | The closed DBH/CI/RD projection is derived, not a full native future report. No ten-year digest or fixture golden is invented. |
| ND-07 | canonical state/history hash, lineage encoding, public API/error names, request dedup form | #737 S2/S5 and #760 B-O05/O06 leave codecs/contracts open. Compare actual object/scalars/events without defining an unapproved public ABI. No save/history-byte promise. |
| OPEN-01 | zero-living stop versus labelled empty-year progression and absence/RD representation | #760 B-O01, §3 S4 supersede earlier unconditional continuation prose; owner choice required. V-09 gives separate branches. |
| OPEN-02 | freeze-at-death and pre-inventory dead representation | #760 B-P04/F4, B-O02/OPEN-B3: age32 versus ecological deathYear2 is semantic; continued freeze policy needs adoption. No actual Year2 death evidenced. |
| OPEN-03 | singleton continuation command policy; ten-step scope versus product duration | Source last-survivor behavior is supported; new API/terminal policy is not. #760 B-O03 recommends a minimum proof horizon, not a game cap or fast-forward. |
| ND-08 | field accuracy, Unity/Mono/cross-platform parity, security release, durable rollback | Synthetic formulas and in-memory transaction expectations establish none of these; L4 remains FAIL38 UNWAIVED. Out of this task. |

## 6. Future acceptance criteria — every engine test NOT RUN

A later implementer may extract Appendix B as a **test oracle aid**, not product code or a public synthetic-fixture admission path. First resolve the open behavior choices and exact numeric sites. Then an independent reviewer should reproduce the synthetic arithmetic, especially V-04 non-associativity and V-07 exact-one construction. Model-output reproduction alone is not engine verification.

| Proposed targeted future test | Required observation, NOT RUN |
| --- | --- |
| A1-M1 | V-01/V-02/V-03: living A/B equality for every scalar/diagnostic and all three stand stages; dead ID retained; no dead target/neighbour/growth/age/roll/event/BA contribution. Include a formerly large/high-CI dead input as well as a dead target. |
| A1-SPATIAL | V-04 two separate rosters: inclusive8, next-f32 excluded, .25→.5 floor, exact neighbour count and ordinal sum word41a02667; insertion/permutation cannot alter it. |
| A1-PRECISION | V-05 input storage bits, float stems/ha, double sum/Dq/RD, final DBH bit412ebf1e; native source profile confirmed first. Record any deliberate policy mismatch instead of widening tolerance to hide it. |
| A1-HAZARD | V-06 genuine p0 means no Roll call and null/notEvaluated; meanV0 guard; uncapped p and cap branch. Compare injected bracket rolls only in a legal internal comparator seam; source-derived real keys/numerators need ND-01 resolved. |
| A1-BOUNDARY | V-07 survivor seam: exact1 no removal, above1 A removed, high-RD A then B, hazard A excluded, count1 retained; all victim sets disjoint and density freshly recomputed. |
| A1-AGE-EMPTY | V-08–10: approved singleton/empty choice, age32/deathYear2, proposed freeze only after approval, null pre-inventory cause/year, no repeat event or fake computed fields. |
| A1-ATOMIC | V-11 synchronized overlap actually guaranteed by a barrier; immutable-state identity unchanged on fault; no staged mortality rows/view leaked; retry/duplicate cannot create a second successor. |
| A1-HORIZON | V-12 ten actual one-year commits0→1→…→10, fresh CI/pressure each step, complete native H/crown only after ND closure, death at k changes neighbour set only at k+1; no second within-year release growth. |
| A1-IDENTITY | V-13 full lineage, version/seed/numeric binding and case-sensitive ordinal IDs; duplicate IDs refused; order-independent roster input yields sorted source arithmetic. |
| A1-YEAR1-PROTECTION | Under separate lawful scope only, old one-year trusted command bytes/refusals remain the published qualified anchor, not a regenerated golden. This task has not run or re-hashed that witness. |

Preconditions for a later **lawful native run**: independently reviewed/approved continuation spec, owner choices resolved, named source writer and real physical lock/grant for a new candidate, exact candidate SHA and permitted test allowlist, independent source/numeric review of M-1 before1→2, exact runtime/numeric policy, and a specifically authorized offline/non-release security scope. Retained source custody or #732's old no-network amendment grants no blanket new access. Keep synthetic seams internal; never change public trust/catalog/fixture bytes to make these vectors runnable. Default L4 FAIL38 remains unwaived and cannot be bypassed to publish/release product source. A separate authorized native qualifier must report actual commands, evidence and limitations; no build/engine command is proposed to run now.

## 7. Provenance, assumptions, limitations and honesty

**Published rules** are linked and quoted in §1. **Mathematical derivations** use only the synthetic rosters and the declared arithmetic in §2/Appendix B. **Test proposals** include internal seam injection, request/error examples and future tolerances; they are not approved interfaces or executed tests. **Unknowns** remain in §5 without fabricated numbers. Model-generated arithmetic is not independent qualification of the implementation it has never read.

These are simplified source-inspired experimental adult Sitka rules, not established ecological measurements or an Irish forestry forecast. Site1 and fixed synthetic stands are test choices; empty/age freeze and ten-step policy are not settled here. No field claim, new ecological coefficient, regeneration, management, recruitment, package or product architecture is introduced.

Publication scope is exactly one added Markdown; primary checkout, existing files, product source, fixtures, output witnesses, Ops and main are untouched by this worker. Publication readback can establish byte identity of **this document only**; it cannot establish correctness of an engine. Parent remains responsible for independent review, required operational synchronization and any later approved implementation. **No engine test passed is claimed. No .NET/P1 execution occurred. No Year2–10 native result, integration, empirical validation, Unity parity or security waiver is claimed.**

## Appendix B. Reproduce the synthetic arithmetic (standard library only)

Copy the single fenced Python block to `/tmp/opencode/780-a1/reproduce.py` and run:

```bash
python3 --version
python3 -I /tmp/opencode/780-a1/reproduce.py > /tmp/opencode/780-a1/reproduced-numbers.json
```

This command operates exclusively on constants embedded below, prints an independent synthetic arithmetic transcript, imports no repository module and reads no fixture or prior stdout. The worker executed this arithmetic model with CPython3.12.3; all proposed native engine tests remain **NOT RUN**. An initial scratch syntax error (unmatched closing parenthesis) was corrected before generating the numerical tables; no product code or native test was involved. The JSON contains f32 exact-decimal/bits and f64 round-trip/hex pairs; it is intentionally not an approved engine schema. Crown `r=1` outputs are conditional unit probes. `V07` is a mortality-only seam; `V12_release` is a conditional injected-death continuation, not an RNG prediction. Roster defaults and all override fields are in §3.

```python
#!/usr/bin/env python3
# Independent SYNTHETIC arithmetic only; never imports or executes project code.
import math, struct, sys, platform, json

def f(x): return struct.unpack('>f',struct.pack('>f',x))[0]
def bits(x): return struct.pack('>f',f(x)).hex()
def frombits(n): return struct.unpack('>f',struct.pack('>I',n))[0]
def nf(x): return frombits(int(bits(x),16)+1)
def pf(x): return frombits(int(bits(x),16)-1)
def F(x):
 x=f(x)
 return {'decimal':format(x,'.60g'),'bits':bits(x)}
def D(x): return {'decimal':repr(x),'hex':x.hex()}
def row(i,x,z,d,h=20,c=2,age=30,live=True):
 return dict(id=i,x=f(x),z=f(z),d=f(d),h=f(h),c=f(c),age=age,live=live)
T=row('T',0,0,20); L=row('L',4,0,25,21,2.2)
DEAD=row('D',1,0,50,25,3,31,False)

def ci(target,roster,include_dead=False):
 if not target['live'] and not include_dead: return None,[]
 trace=[]; acc=f(0)
 for j in sorted(roster,key=lambda q:q['id']):
  if j['id']==target['id'] or (not j['live'] and not include_dead): continue
  dx=f(target['x']-j['x']); dz=f(target['z']-j['z'])
  xx=f(dx*dx); zz=f(dz*dz); sq=f(xx+zz); dist=f(math.sqrt(sq))
  if dist>f(8):
   trace.append(dict(id=j['id'],dx=F(dx),dz=F(dz),xx=F(xx),zz=F(zz),sq=F(sq),distance=F(dist),included=False)); continue
  den=f(max(f(1),target['d'])); ratio=f(j['d']/den); floor=f(max(f(.5),dist)); term=f(ratio/floor)
  acc=f(acc+term)
  trace.append(dict(id=j['id'],dx=F(dx),dz=F(dz),xx=F(xx),zz=F(zz),sq=F(sq),distance=F(dist),included=True,den=F(den),ratio=F(ratio),floor=F(floor),term=F(term),acc=F(acc)))
 return acc,trace

def grow(d,ci,c):
 q=f(d/f(120)); sub=f(f(1)-q); cl=f(max(0,min(1,sub)))
 a=f(f(1.2)*f(1)); num=f(a*cl); cq=f(ci/f(5)); den=f(f(1)+cq)
 gain=f(num/den); reciprocal=f(f(1)/den); gainR=f(num*reciprocal)
 post=f(max(f(1),min(f(200),f(d+gain))))
 postR=f(max(f(1),min(f(200),f(d+gainR))))
 mul=f(f(.07635)*post); base=f(f(.9415)+mul); base=f(max(f(.1),base))
 # response=1 is an explicitly INJECTED crown-unit-probe input, not an engine assertion.
 target=f(base*f(1)); diff=f(target-c); relax=f(diff*f(.15)); crown=f(max(f(.1),f(c+relax)))
 return dict(div120=F(q),oneMinus=F(sub),clamp=F(cl),siteProduct=F(a),numerator=F(num),ciDiv5=F(cq),denominator=F(den),gain_div=F(gain),reciprocal=F(reciprocal),gain_mul_reciprocal=F(gainR),postDBH_div=F(post),postDBH_mul_reciprocal=F(postR),crown_product=F(mul),crown_base=F(base),crown_r1_target=F(target),crown_r1_difference=F(diff),crown_r1_relax=F(relax),crown_r1_final=F(crown))

def stand(ds,area):
 ds=[float(d) for d in ds]; area=f(area); n=len(ds); sums=0.; trace=[]
 for d in ds:
  prod=d*d; sums=sums+prod; trace.append(dict(d=F(d),square=D(prod),runningSum=D(sums)))
 if not n: return dict(N=0,area=F(area),Nha=F(0),sumSquares=D(0.),BA=D(0.),BAha=D(0.),Dq=None,RD=None,trace=trace)
 nh=f(n/area); dq=math.sqrt(sums/n); ratio=dq/25.; power=math.pow(ratio,2.063); rd=(float(nh)*power)/1868.
 ba=(sums*math.pi)/40000.; baha=ba/float(area); p=max(0.,(rd-.6)/.4)
 return dict(N=n,area=F(area),Nha=F(nh),sumSquares=D(sums),BA=D(ba),BAha=D(baha),meanSquare=D(sums/n),Dq=D(dq),DqDiv25=D(ratio),power=D(power),densityProduct=D(float(nh)*power),RD=D(rd),P=D(p),trace=trace)

def hazard(cis,st):
 ss=[1.-1./(1.+f(c/f(5))) for c in cis]; vv=[math.pow(s,4.) for s in ss]; total=0.
 for v in vv: total+=v
 mean=total/len(vv) if vv else None; p=float(st['P']['decimal'])
 return [dict(CI=F(c),ciDiv5=F(f(c/f(5))),S=D(s),V=D(v),meanV=D(mean),uncapped=D(.08*math.pow(p,2.)*v/mean) if mean and p>0 else D(0.),p=D(min(.5,.08*math.pow(p,2.)*v/mean)) if mean and p>0 else D(0.)) for c,s,v in zip(cis,ss,vv)]

out={}
out['environment']=dict(python=sys.version,implementation=platform.python_implementation(),machine=platform.machine(),platform=platform.platform(),float_info=str(sys.float_info),rounding='struct >f, IEEE nearest ties-to-even; no FMA, no numpy')
cs={}; gs={}
for q in [L,T]:
 c,tr=ci(q,[T,L]); cs[q['id']]=c; gs[q['id']]=grow(q['d'],c,q['c'])
 out['V01_'+q['id']]=dict(CI=F(c),trace=tr,growth=gs[q['id']])
post=[float(gs[k]['postDBH_div']['decimal']) for k in ['L','T']]
out['V01_before']=stand([25,20],1); out['V01_post']=stand(post,1); out['V01_hazard']=hazard([cs['L'],cs['T']],out['V01_post'])
for q in [L,T]:
 c,tr=ci(q,[T,L,DEAD],True); out['V02_wrong_'+q['id']]=dict(CI=F(c),trace=tr)
out['V02_dead']=DEAD
# Edge/floor separate from non-associativity roster.
e=[row('E8',8,0,20),row('Eover',nf(8.),0,20),row('Fquarter',.25,0,20),T]
c,tr=ci(T,e);out['V04_edge']=dict(CI=F(c),trace=tr)
o=[row('A',.25,0,200),row('B',8,0,1),row('C',-8,0,2),T]
c,tr=ci(T,o);terms=[float(t['term']['decimal']) for t in tr]; rev=f(f(terms[1]+terms[2])+terms[0]);out['V04_order']=dict(CI=F(c),trace=tr,BCthenA=F(rev))
# Synthetic area 0.1, two source f32 DBHs. No canonical fixture data.
area=f(.1); d1=f(20.1); d2=f(25.3);out['V05_input']={'area':F(area),'D1':F(d1),'D2':F(d2),'H':F(20.1),'crown':F(2.2)}
out['V05_correct']=stand([d1,d2],area)
sum64=d1*d1+d2*d2; exactN=2./float(area); dq=math.sqrt(sum64/2)
rdWrongN=exactN*math.pow(dq/25.,2.063)/1868.
rdWrongArea=(2./.1)*math.pow(dq/25.,2.063)/1868.
c,tr=ci(row('T',0,0,d1),[row('T',0,0,d1),row('L',4,0,d2)])
c64=(d2/d1)/4
out['V05_divergences']=dict(Nha_f64=D(exactN),RD_f64_Nha=D(rdWrongN),Nha_decimal_area=D(2./.1),RD_decimal_area=D(rdWrongArea),CI_f32=F(c),CI_f64=D(c64),CI_f64_then_f32=F(c64),growth_f32=grow(d1,c,f(2.2)),DBH_all_f64_then_f32=F(d1+1.2*(1.-d1/120.)/(1.+c64/5.)),DBH_all_f64=D(d1+1.2*(1.-d1/120.)/(1.+c64/5.)))
# Additional final-storage discriminator, input f32 DBH 10 and 38.7.
x=f(10); j=f(38.7); cc=f(f(j/x)/f(4)); cc64=(j/x)/4.
out['V05_final_bit']=dict(target=row('T',0,0,10),neighbour=row('L',4,0,38.7),CI=F(cc),growth=grow(x,cc,f(2)),all64_CI=D(cc64),all64_post=F(x+1.2*(1.-x/120.)/(1.+cc64/5.)))
# Low but nonzero hazard, equal trees -> V/mean=1.
he=[row('A',0,0,25),row('B',4,0,25)]
hc=[ci(q,he)[0] for q in he]; hp=[float(grow(q['d'],c,q['c'])['postDBH_div']['decimal']) for q,c in zip(he,hc)]
ha=f(2./1400.); hs=stand(hp,ha);out['V06']=dict(area=F(ha),CI=[F(c) for c in hc],postDBH=[F(d) for d in hp],stand=hs,hazard=hazard(hc,hs))
pp=float(out['V06']['hazard'][0]['p']['decimal']); kk=math.floor(pp*(1<<24))
out['V06_roll_bracket']=dict(lowerNumerator=kk,lowerRoll=D(kk/(1<<24)),upperNumerator=kk+1,upperRoll=D((kk+1)/(1<<24)),actualRoll='NOT DERIVABLE')
# Boundary stage probes, post-growth inputs, NOT full annual predictions.
a1=f(4./1868.);out['V07_exact']=stand([25]*4,a1);out['V07_above']=stand([25]*4,pf(a1));out['V07_after_one']=stand([25]*3,pf(a1))
a2=f(4./3500.);out['V07_loop']={str(n):stand([25]*n,a2) for n in [4,3,2,1]}
out['V07_singleton_high']=stand([200],f(.001))
out['V08']=dict(input=row('S',0,0,200),growth=grow(f(200),f(0),f(2)),stand=stand([200],f(.001)),hazard=hazard([f(0)],stand([200],f(.001))))
# Ten sequential DBH/CI/density projections only; exact heights/crowns remain unknown.
roster=[dict(L),dict(T)]; ten=[]
for y in range(10):
 before=[dict(q) for q in roster]; data=[]
 for q in roster:
  c,tr=ci(q,before); g=grow(q['d'],c,q['c']); q['d']=float(g['postDBH_div']['decimal']);q['age']+=1
  data.append(dict(id=q['id'],CI=F(c),gain=g['gain_div'],gain_reciprocal=g['gain_mul_reciprocal'],DBH=F(q['d']),DBH_reciprocal=g['postDBH_mul_reciprocal'],age=q['age'],H='NOT DERIVABLE',crown='NOT DERIVABLE',roll=None))
 st=stand([q['d'] for q in roster],1);ten.append(dict(beforeYear=y,targetYear=y+1,afterYear=y+1,N=2,trees=data,RD=st['RD'],P=st['P'],deaths=[]))
out['V12_ten']=ten
# Conditional death L at Year2, then independent recompute for T at Year3.
d2=float(ten[1]['trees'][1]['DBH']['decimal']); oldci=float(ten[1]['trees'][1]['CI']['decimal'])
out['V12_release']=dict(T_at_Year2=F(d2),correct_Year3_CI=F(0),correct_Year3_growth=grow(d2,f(0),f(2)),wrong_cached_Year3_CI=F(oldci),wrong_cached_Year3_growth=grow(d2,oldci,f(2)))
print(json.dumps(out,indent=2,sort_keys=False,allow_nan=False))
```
