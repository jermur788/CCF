# Multi #780 — DeepSeek V4.1 Flash parent result: Year2–10 quantitative validation and Irish Sitka forestry evidence

**Status: RESEARCH HANDOFF / DRAFT / NOT RUN.** This is one implementation-ready *research* pack for a future Multi-Sitka multi-year engine. It approves no specification, accepts no coefficient, authorises no source and reports no executed engine test. `#737` and `#760` remain **DRAFT**. Default **L4 FAIL38 is UNWAIVED**.

| Field | Value |
|---|---|
| Task | [#780](https://github.com/jermur788/CCF/issues/780), under Multi Manager [#730](https://github.com/jermur788/CCF/issues/730) |
| Parent actor | DeepSeek V4.1 Flash, `opencode-go/deepseek-v4.1-flash`, OpenCode harness inside T3 Code, native orchestrator |
| Host | `jer-laptop`, Linux 7.0.0-42-generic x86_64; T3 environment `ac150346-a782-45eb-8cae-4e08fe9bab79`; parent thread `9bdbecee-1736-4bfd-8c55-9d22c48ed869` |
| Parent registration | [RUNNING comment on #780](https://github.com/jermur788/CCF/issues/780#issuecomment-6990312179), posted before dispatch |
| Fresh base | `origin/main` = `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`, fetched 2026-11-18T20:12Z |
| Parent branch / file | `docs/multi-780-deepseek-parent-20261118-v1` / this one new Markdown |
| Children | A1 GPT-6.1 Sol (numerical oracle); A2 Claude Sonnet 5.5 (Irish forestry evidence); Q Claude Opus 5.5 nonauthor reviewer |
| TG Fork skills | Actual laptop install verified: `~/.claude/skills/*` symlinks into `~/.agents/skills/*`; `orchestrate-work/SKILL.md` SHA-256 `6671dc241927743dc64648a14691735c4b2658cd8adc0b43831f9eaebd9d0402` |
| Exclusions in force | No #710/#725/P0/#687/#639 source, file or fixture bytes; no fixture re-read or re-hash; no .NET/P1 execution; no product source, Ops, main, Host/save, Unity, Windows, viewer #776, #779 or #770/Scenario One; no GPT-6.2 |
| Deliverables | A1 85,144 B; A2 56,567 B corrected (54,283 B as reviewed); Q review 41,089 B; this parent result |

> **No simulation was run. No engine output was produced. No test passed is claimed.** The parent result is a documentary synthesis of two independent research reports, a readback audit, the parent's own synthetic arithmetic checks, and an independent nonauthor review. Every future engine test named here is **NOT RUN**.

---

## 1. Outcome in one page

**Delivered.** A four-file, four-branch research pack, reviewed and corrected: a controlled numerical test-oracle register covering M-1 dead exclusion, chronology, binary32/binary64 boundaries, hazard and density-boundary edges, singleton/empty alternatives, age-at-death semantics, transactional refusal and ten sequential annual steps (A1); an Irish Sitka growth/density/competition/mortality evidence envelope with per-number provenance and explicit unsupported cells (A2); an independent Claude Opus 5.5 research-quality review (Q); and this consolidated result with a short owner decision sheet and targeted future tests.

**What is now genuinely implementation-ready.** The M-1 gate has a fully worked, reproducible synthetic oracle: a dead 50 cm stem at 1 m from a 20 cm living target must not change that target's CI, growth or stand totals, and a wrong implementation that admits it shifts CI from `0.3125` to `2.8125` and DBH growth from `0.9411764740943909` to `0.6399999856948853` cm. The source-order, float-storage, strict-RD>1, singleton-guard and single-commit requirements each now have discriminating vectors. The evidence envelope is usable as a *comparison* basis, not as a calibration source.

**What remains blocked by missing facts.** Exact `SimulationRandom.Roll` outputs and victims, the full Class III height function, the crown response function, several native evaluation/rounding sites, and all public identity/history codecs are **NOT DERIVABLE** from the allowed published material. No ten-year native digest, save/history hash or full per-tree H/crown oracle exists. On the biology side, measured dead stems, annual mortality rate, size/suppression bias of deaths, realised crown size and within-stand size variation are **UNSUPPORTED — NEEDS DECISION**, not estimated.

**What this does not change.** The accepted bounded P1 four-file lineage, the qualified Year1 `0→1` behaviour, the pinned fixture/CLI witnesses, D-084's Scenario One scope, and the prohibition on unapproved Year2 source. The ten-year horizon is treated as ten sequential one-year transitions — a proposed verification horizon, not a forecast, game lifespan, fast-forward feature or empirical Irish prediction.

**Manager action requested.** Accept or return this pack on documentary/research quality; route the §9 owner decisions; if accepted, commission the Year2 continuation-spec correction and independent spec review *before* any source permit. No source-writing packet is implied by this result.

---

## 2. Authority boundary preserved

Three states must not be conflated:

| State | Evidence | What it authorises here |
|---|---|---|
| **Accepted bounded P1** | Owner four-file approval receipt, pinned blob | The existing four-file P1 implementation lineage only; it does **not** approve a Year2+ public interface or multi-year implementation |
| **Implemented / reviewed / focused-qualified Year1** | #725 local implementation, #729 independent five-file source/numeric review, #732 focused offline native qualification, #723 Year1 oracle | Correct `0→1` behaviour for the trusted fixture under its recorded conditions. It is **not** evidence for `1→2` |
| **Unapproved Year2 draft** | #737 DRAFT spec, #760 DRAFT decision/errata | Planning text only. Neither file is an approved spec, neither confers source permission, and both still contain open owner decisions |

The published witnesses are quoted only as **boundary-of-evidence anchors**: fixture 32,567 bytes, SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`; initial RD `0.609795828454288`; Year1 post-growth RD `0.6375836886879149`; zero Year1 deaths; old CLI stdout 45,980 bytes, SHA-256 `574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`. **This task did not read, copy, execute or re-hash any of those bytes.** The digest is a one-year report anchor, never a future Year2/snapshot/save digest.

The M-1 finding is treated as an upstream **accepted gate**, not a new claim: current one-year code includes dead input trees in competition, growth and stand totals; that is unreachable in the Year1 fixture but must be repaired before any `1→2` step, covering dead **targets and neighbours**. Living trees below the P0 170 mm work threshold remain biological participants; work eligibility is a separate concern.

No protected #710/#725 source, original P0 source, #687 file, #639 fixture blob, Ops board/lane, main edit, viewer, security lane or Scenario One artifact was read or modified. The run was deliberately parallel to #776 and #779 and created no class of CPU or physical-lock collision.

---

## 3. Actors, branches and published artifacts

| Actor | Actual model / provider / harness | Registration | Branch | Commit | File | Bytes | SHA-256 |
|---|---|---|---|---|---|---|---|
| **Parent** | DeepSeek V4.1 Flash `opencode-go/deepseek-v4.1-flash` / OpenCode in T3 | [#780 comment](https://github.com/jermur788/CCF/issues/780#issuecomment-6990312179) | `docs/multi-780-deepseek-parent-20261118-v1` | publication commit | `Multi-780-DEEPSEEK-YEAR2-10-INDEPENDENT-VALIDATION-PARENT-RESULT-20261118.md` | self-referential; see §12 | self-referential; see §12 |
| **A1** | GPT-6.1 Sol `gpt-6.1-sol` / Codex in T3, high effort | [RUNNING](https://github.com/jermur788/CCF/issues/780#issuecomment-6990361765); [HANDED-OFF](https://github.com/jermur788/CCF/issues/780#issuecomment-6990608509) | `docs/multi-780-a1-sol-numeric-oracle-20261118-v1` | `892d9be765820f599bb4bdb3e489fc7e23b9712a` | `Multi-780-A1-YEAR2-10-NUMERICAL-TEST-ORACLE-RESULT-20261118.md` | 85,144 | `2849ee8e25c2f7c00a14753176b9f0c3153f19c9fcc17d0f71dd4d7e46b5112a` |
| **A2** | Claude Sonnet 5.5 `claude-sonnet-5-5` / Claude A, then correction on Claude B (Pro), high effort | [RUNNING](https://github.com/jermur788/CCF/issues/780#issuecomment-6990359354); [HANDED-OFF](https://github.com/jermur788/CCF/issues/780#issuecomment-6990561762); [CORRECTION](https://github.com/jermur788/CCF/issues/780#issuecomment-6990987196) | `docs/multi-780-a2-sonnet-irish-sitka-20261118-v1` | reviewed `7f58bb5c…`; corrected head `37279354a9685fe4c0030b9b4f58c3b212981579` | `Multi-780-A2-IRISH-SITKA-GROWTH-DENSITY-EVIDENCE-RESULT-20261118.md` | 56,567 | `403c587253a0cbc392b8e92668dc64d7538c237997b1aeaa2fe1830b7ad01c5d` |
| **Q** | Claude Opus 5.5 `claude-opus-5-5` / Claude B (Pro) in T3, high effort | [RUNNING](https://github.com/jermur788/CCF/issues/780#issuecomment-6990683286); [HANDED-OFF](https://github.com/jermur788/CCF/issues/780#issuecomment-6990900555) | `docs/multi-780-q-opus-review-20261118-v1` | `bf8fec885258cc4e35a29d754499a0556230dab5` | `Multi-780-Q-INDEPENDENT-QUANT-AND-FORESTRY-REVIEW-20261118.md` | 41,089 | `e01a7e319674930d4fffa94dee5b2ba9600b68b2c8b9e3666bce97a4fe6b7cbe` |

Each lane created exactly **one new Markdown** on its own branch from the same fresh main; no lane edited another lane, the primary checkout, or an existing file. A1 and A2 each registered before the first write and reported a one-file compare. The parent verified by raw readback that the committed bytes match the authors' claimed byte counts and SHA-256 values, and that each compare shows one added file with zero deletions. The A2 author read local research copies whose git blob IDs matched `origin/main` at `a69b3e2e`, and disclosed that the primary checkout HEAD was an older commit with unrelated dirty files.

**Provider note (disclosed, not silent).** The first Q attempt on the Claude A account failed before it could register, because that account had reached its five-hour limit. The review was re-dispatched to the separately authenticated Claude B (Pro) provider instance on the same actual model, Claude Opus 5.5, and that session registered its own identity. No model fallback (and never GPT-6.2) occurred.

---

## 4. A1 — controlled numerical oracle: what is now reproducible

A1 delivers thirteen synthetic vector families (`V-01`…`V-13`), a declared binary32 emulation profile, fully worked arithmetic, a NOT DERIVABLE register and a ten-step DBH/CI/RD projection. Everything is labelled **NOT RUN**. The following are the load-bearing results.

### 4.1 Vector register (condensed)

| Vector | Purpose | Load-bearing expected observation | Status |
|---|---|---|---|
| **V-01** | All-living control, #760 T/L pair | `CI(T)=0.3125` (`0x3ea00000`), `CI(L)=0.20000000298023224` (`0x3e4ccccd`); post DBH `20.9411773681640625` / `25.9134616851806640625`; `p=0`, no roll, no death | NOT RUN |
| **V-02** | **M-1 dead-neighbour A/B** (`A={D,L,T}`, `B={L,T}`) | Every living CI, growth, crown, stand stage, roll and outcome identical; dead D retained bit-for-bit; no D growth/age/roll/event/BA. Wrong inclusion changes `CI(T)` to `2.8125` and growth to `0.6399999856948853` | NOT RUN |
| **V-03** | Dead target / inert exclusion | Dead ID has absent CI/gain/p, no roll, no new event, unchanged 50/25/3/31; stand equals V-01; no computed-zero camouflage | NOT RUN |
| **V-04a/b** | 8 m edge, 0.5 m floor, ordinal accumulation | 8.0 included, next f32 `8.000000953674316` excluded, 0.25→0.5 floor; ordinal A,B,C sum `20.01875114440918` (`0x41a02667`) vs permuted B,C,A `20.018749237060547` (`0x41a02666`) | NOT RUN |
| **V-05** | f32 storage vs f64 stand reduction | Area `0.10000000149011612` (`0x3dcccccd`); f32 N/ha `20`; f64 RD `0.00889248020590604`; `BAha 0.8200341919078716` vs wrong `/0.1` `0.8200342041273332` | NOT RUN |
| **V-06** | Zero / uncapped / capped hazard, conditional RNG | `p=0` means no Roll call and `roll=null/notEvaluated`; nonzero `p=0.021320623328740265`; actual 24-bit numerator/victims **NOT DERIVABLE**; injected brackets `357700`/`357701` are comparator-only | NOT RUN |
| **V-07** | Strict RD boundary, hazard exclusion | Exact RD=1 removes nobody; one-ULP area drop gives RD `1.000000130696266` and removes A; tie by suppression then ordinal; hazard victims excluded from boundary; count-1 guard retained | NOT RUN |
| **V-08** | Singleton | Genuine CI 0 with empty neighbour set; growth defined; `p=0`; boundary preserves the last survivor even at RD `39.0569` | NOT RUN |
| **V-09** | Zero living, **OPEN-B2** | Two separate branches: typed terminal/refusal with unchanged state, or explicitly labelled empty-year progression; no automatic adoption; no fabricated RD/tree | NOT RUN |
| **V-10** | Biological age vs ecological deathYear | Initial age 30 dying in Year 2 ⇒ **age 32, deathYear 2**; post-growth size retained; freeze only if OPEN-B3 is approved; pre-inventory dead has null cause/year | NOT RUN |
| **V-11** | Atomicity, stale/duplicate/wrong-year | One successor, one reference swap; stale/duplicate/wrong-year refuse with unchanged roster/year/events; pre-commit throw publishes nothing; synchronized overlap cannot yield year 3 | NOT RUN |
| **V-12** | **Ten sequential annual transitions** | Ten real `0→1→…→10` steps, fresh CI each year, ages 30→40, N=2, no deaths, DBH sequence given exactly; independent Year1 invocations do not constitute history | NOT RUN |
| **V-13** | Identity and permutation | Input order `[T,L]` and `[L,T]` sort to ordinal `[L,T]` and give V-01; duplicate ID refused; lineage/seed/version binding distinguishes equal-looking runs | NOT RUN |

### 4.2 Parent arithmetic check (not native, not the reviewer)

The parent independently reproduced the load-bearing arithmetic with its own standard-library binary32 model, without reading A1's appendix code:

- V-01: `CI(T)=0.3125`, `CI(L)=0.20000000298023224`, T/L post-growth DBH `20.9411773681640625`/`25.9134616851806640625` — match.
- V-04b: ordinal sum `20.01875114440918` (`0x41a02667`), permuted `20.018749237060547` (`0x41a02666`) — match.
- V-05: f32 N/ha `20`, RD `0.00889248020590604`, BA `0.08200342041273333`, `BAha 0.8200341919078716`, wrong `/0.1` `0.8200342041273332` — match.
- V-07a/b: exact RD `1.0`; one f32 area step down gives N/ha `1868.000244140625` and RD `1.000000130696266` — match.
- V-12 row `0→1`: CI and DBH words match V-01 and the table — match.

This confirms that the report's arithmetic is not merely asserted. It does **not** turn synthetic arithmetic into a native engine result, and it does not replace Q's independent review in §6.

### 4.3 What A1 correctly refuses to invent

| ID | Missing | Consequence |
|---|---|---|
| ND-01 | Complete `SimulationRandom.Roll` key packing/hash and 24-bit numerator | No exact roll, victim or synthetic mortality outcome for arbitrary IDs/years |
| ND-02 | Full Class III relative-height function and evaluation sites | No exact future height golden |
| ND-03 | Actual crown `response` function | Only conditional `r=1` arithmetic; no biological crown prediction |
| ND-04 | Every native DBH/BA/pow evaluation site, JIT/libm last bits | DIV/RECIP gains can differ in the last bit; native goldens require source confirmation |
| ND-05 | Deterministic victim selection where `p>0` | Victim sets are injected seam tests, never predicted |
| ND-06 | Complete ten-step per-tree H/crown and report bytes | The ten-step table is a DBH/CI/RD projection only |
| ND-07 | Canonical state/history hash, lineage codec, public API/error names | No save/history/API claim |
| ND-08 | Field accuracy, Unity/Mono parity, durable rollback, security release | Nothing here qualifies a release |

A1 also flags that the #737/#729 material does not fully identify whether DBH growth is evaluated as `n/(1+CI/5)` or `n·(1/(1+CI/5))`; both profiles are printed, all ten committed DBH states agree, and two diagnostic gain bits differ. That is exactly the kind of ambiguity a future source review must settle rather than a test pack silently choosing.

---

## 5. A2 — Irish Sitka evidence envelope: ranges, provenance and limits

A2 read the accessible CCF research composites and eight local research files at blobs identical to `origin/main` `a69b3e2e`. It re-read **no primary paper or PDF**; every primary-study value is marked **P† = cited via secondary synthesis; primary not re-read**. It also records that `CCF_Ecology_and_Regeneration_Evidence_Composite.md` is **MISSING**, so it asserts no regeneration/recruitment evidence.

### 5.1 Evidence that can support comparison (condensed)

| Metric | Accessible value / range | Population and condition | Class |
|---|---|---|---|
| Age-30 top height by Irish productivity class | I 27.6, II 23.3, III 20.4, IV 17.4, V 16.0 m | Lekwadi et al. 2012; 578 plots, 4,190 top-height trees, 11,421 pairs; Irish Sitka | P† IE |
| Broader Irish age-30 site index | ≈4.5–28.9 m | Wet/nutrient-poor to very moist/nutrient-rich | S IE |
| NFI1 Sitka summary | BA 32.97 m²/ha (6.69–74.80); 1,662 stems/ha (240–4,967); mean DBH 18.66 cm; mean H 11.93 m; n=200, mean age 22.73 (9.10–49.00) | Irish NFI, even-aged pure Sitka; 107 thinned / 93 unthinned | P† IE |
| Frenchpark after 29 seasons | unthinned 1,762 stems/ha, 23.7 cm; light 925, 29.8; medium 761, 31.1; heavy 616, 32.4 cm | Irish thinning trial | P† IE |
| Derived Frenchpark BA lower bounds | ≥77.7 / 64.5 / 57.8 / 50.8 m²/ha | Mean DBH used as a Dq proxy, so lower bounds | M |
| British maximum size–density line | slope −2.063, max SDI 1,868, 25 cm reference; RD = N·(Dq/25)^2.063/1868 | Comeau et al. 2010, British Sitka; boundary not typical stand | P† GB |
| BA on that line at RD=1 | 86.8–94.7 m²/ha over Dq 15–60 cm; at RD=0.6, 52.1–56.8 m²/ha | Transferred boundary | M |
| Potential open-grown crown radius | 2.09 m at DBH 15 → 5.52 m at DBH 60 | Tabbush & White 1988, British open-grown; upper envelope only | P† GB |
| GROWFOR/Broad–Lynch height | polymorphic Bertalanffy–Richards; site index = top height at 30; a=47.0216, c=0.4571, t0=−5.0; 749 trajectories / 225 plots | Irish PSP, conventional even-aged | P† IE |
| GROWFOR vs NFI | volume bias −1.41%, RMSE 18.80%, R² 0.84; BA bias −1.96%, RMSE 12.81%, R² 0.81 | Cross-sectional, n=200 | P† IE |

### 5.2 The comparison table's unsupported cells

A2 leaves these explicitly **UNSUPPORTED — NEEDS DECISION**, not filled: measured dead stems per hectare; annual mortality rate at ages 30–40; mortality distribution by size or suppression; within-stand size variation; realised (as opposed to potential) crown radius; Irish BA/stand trajectories at ages 40–50; Sitka yield class and rotation length; the reconciliation of top-height definitions; and what "growth model 2" changes relative to model 1.

### 5.3 Material caveats a future calibration must carry

1. **The "≈55 m²/ha Irish untreated BA" claim is not safe.** The composite labels a Jones et al. 2026 figure as Irish, while its own manifest describes that source as mostly non-Irish transfer/modelled data; the composite's Frenchpark table independently implies **≥77.7 m²/ha** unthinned. A2 records the contradiction and refuses to use 55 as an Irish maximum.
2. **Top height must not be averaged across models.** The Lekwadi-form Class III curve and the GROWFOR height equation agree at age 30 by construction and diverge by **+7.1% at 40, +18.4% at 60 and +30.5% at 100** years. A2 reports both as a band.
3. **Rounded Lekwadi coefficients do not reproduce the published anchors** (miss by up to 2.19 m at age 30); the anchors are validation points, not a curve to freeze.
4. **GROWFOR is stand-level.** It validates aggregate B/N/H/V behaviour for pure even-aged Sitka under conventional thinning, not local Hegyi release, individual-tree growth, mortality bias, regeneration, mixed or irregular stands. Its validation score in the composite is 2/0/0.
5. **The S⁴ mortality normalisation is game calibration, not Irish law.** No accessible Irish source supplies a mortality hazard, onset or size bias. The CCF constants (RD 0.6 onset, 0.08 strength, S⁴, 0.5 cap, 0.15 crown relaxation) remain gameplay calibration.
6. **The synthetic Multi-P1 stand is authored, not measured.** A2 derives N/ha ≈ 1,111.1, Dq ≈ 25.30 → 25.86 cm, BA ≈ 55.9 → 58.3 m²/ha and ≈0.07 expected Year-1 hazard deaths from the **printed** #737/#760 count, area and RD only; it did not read fixture bytes. The parent independently reproduced all four values. They are mathematical derivations from gameplay inputs, not evidence.

---

## 6. Independent review (Q)

**Actor.** Claude Opus 5.5 `claude-opus-5-5`, provider Claude B (Pro) via T3 Code, fresh nonauthor session. It is a different model from A1 (OpenAI GPT-6.1 Sol) and a different model from A2 (Anthropic Claude Sonnet 5.5), and it shares the Anthropic family with A2; that overlap is disclosed rather than claimed as cross-family independence. The first attempt on the Claude A account was refused by a five-hour rate limit before registration; the re-dispatch and its provider switch are recorded in §3.

**Q publication.** [Registration](https://github.com/jermur788/CCF/issues/780#issuecomment-6990683286) · [HANDED-OFF](https://github.com/jermur788/CCF/issues/780#issuecomment-6990900555) · branch `docs/multi-780-q-opus-review-20261118-v1` · commit `bf8fec885258cc4e35a29d754499a0556230dab5` · 41,089 bytes · SHA-256 `e01a7e319674930d4fffa94dee5b2ba9600b68b2c8b9e3666bce97a4fe6b7cbe` · one added Markdown.

**Verdict.** **NEEDS BOUNDED CORRECTION, A2 only.** A1 is **APPROVE (research quality only)** with no value change required. Q found **0 BLOCKER, 1 SHOULD, 10 NIT**, and **no numeric mismatch in either document**. It independently recomputed V-01 through V-08 and all ten V-12 rows, including the `41a02667`/`41a02666` order discriminator, exact RD=1, one-ULP-above removal and the dead-neighbour A/B equality; and it checked 16 A2 numbers against the local research copies, recomputing D-10, D-06/D-06b, S-10, M-08 and eleven other derived values.

**The SHOULD (A2-S1).** A2's M-09 said "609 cumulative deaths in 120 yr". The actual accessible sources say 609 deaths **over 100 simulated years, from age 20 to age 120**. A reader would otherwise derive a 17% understated annual rate. The population basis (per stand vs per hectare, planted-only vs including recruits) is not stated in accessible text and remains NOT CHECKED.

**Correction routed.** A2-S1 plus the six A2 NITs were returned to the original A2 author on the same branch and file: correct the time base; disambiguate the M-08 arithmetic; reclass E-03 as an inference from a boundary; label T-04 as a pre-mortality counterfactual and CI 8.2 as Scenario One illustrative; restore the Conry & Clinch 80% canopy qualifier; add the M-1 target+neighbour contract row at the Year1→Year2 seam; and reword F-06 as "means cannot be combined". No derived number was to change.

**Correction outcome.** The original A2 session (Claude A) was rate-limited, so the bounded correction was executed by a fresh Claude Sonnet 5.5 session on the separately authenticated Claude B (Pro) instance, on the same document and branch; that switch is disclosed in A2 §13 and here. Commit `37279354a9685fe4c0030b9b4f58c3b212981579` (parent `7f58bb5c`, no amend, no force-push) changes one file, +17/−6; corrected readback is 56,567 bytes, SHA-256 `403c587253a0cbc392b8e92668dc64d7538c237997b1aeaa2fe1830b7ad01c5d`. All seven findings were addressed. No derived number changed; the only non-wording change is E-03's class from P† GB to S. The review comment is [#780 correction](https://github.com/jermur788/CCF/issues/780#issuecomment-6990987196).

**Focused non-Anthropic recheck.** A fresh GPT-6.1 Sol (OpenAI) session, nonauthor of A2, rechecked only the corrected lines against the pinned commits and the #760 M-1 text: **APPROVE CORRECTION, 0 BLOCKER / 0 SHOULD / 0 NIT**. It verified the +17/−6 one-file diff, the corrected 100-year time base, M-08 arithmetic, E-03 class, T-04 labels, S-08 qualifier, the new T-10 row against #760 §3 S3, F-06 wording and §13, and confirmed that no existing substantive number moved (`0.094→0.0940` is formatting; Appendix B unchanged). It notes that its model also authored A1, so it is independent of A2 but not of the whole pack. [RECHECK RUNNING](https://github.com/jermur788/CCF/issues/780#issuecomment-6991019774) · [RECHECK HANDED-OFF](https://github.com/jermur788/CCF/issues/780#issuecomment-6991098850). This closes Q's one SHOULD with a non-Anthropic check, as Q recommended.

**A1 NITs (recorded, not applied).** Q accepted A1 without correction. Its four NITs are retained as residual notes: the V-05 trace order is immaterial for a two-term sum; `max(1, DBH_target)` is unreachable for clamp-valid state and therefore untested; V-04b uses deliberately overlapping stems and needs a geometry-guard caveat or a non-overlapping substitute (carried into T-02 below); and #776/#779 are disclaimed generically rather than by number. None changes a value or the approval.

**Scope of the verdict.** Research quality only. It does not accept any coefficient, approve #737/#760, qualify an engine, waive L4 FAIL38 or authorise source. Its own review execution is a documentary/read-only check, not an engine run.

**Pack status after review.** A1 is approved as research quality with no correction. A2's one SHOULD was corrected on the same branch and approved by the non-Anthropic recheck. **No open BLOCKER, SHOULD or NIT remains** against either research document. The approval is documentary only and does not change the DRAFT status of #737/#760 or the source prohibition.

---

## 7. Acceptance criteria for a future Year2+ engine

These are proposed **testable acceptance criteria**, not an approved spec. Each maps to at least one A1 vector; all remain NOT RUN until a separately authorised native lane exists.

| ID | Criterion | Required observable | Vector |
|---|---|---|---|
| **AC-01 M-1** | Before any `1→2` step, dead trees are excluded as **competition targets and neighbours**, and from growth, ageing, crown, stand N/BA/Dq/RD, mean vulnerability, rolls and boundary candidates | A/B states differing only by a dead roster entry produce identical living CI/growth/stand/outcome; the dead row is retained unchanged | V-01/02/03 |
| **AC-02 Roster integrity** | Every original dead ID remains in the immutable roster with its historical metadata; no new death event, age increment, growth, roll or BA contribution | Full-roster diff, event count and per-ID fields | V-02/03/10 |
| **AC-03 Year1 no-drift** | The separately qualified `0→1` path remains byte-identical to the published anchor under its original lawful conditions | Old CLI bytes/hash reproduced only in a future authorised native run; this pack neither re-ran nor re-hashed it | published anchor |
| **AC-04 Chronology** | Each transition increments the ecological year exactly once; `targetYear=y+1`; ten steps are ten transactions, not one forecast | Year/age/step counts and per-step state lineage | V-12 |
| **AC-05 Numerics** | Source f32 tree DBH/H/crown/CI/area/stems-ha and f64 BA/Dq/RD boundaries are preserved; no silent conversion; declared evaluation sites | Bit-level vectors and a written numeric policy | V-05, V-12 |
| **AC-06 Hazard** | `p=0` performs no roll; `p>0` keys the roll by seed + target year + RNG version + full ordinal ID; death iff `roll<p`; cap 0.5; mean over current living | Null/not-evaluated flags, key isolation, injected comparator vectors | V-01, V-06 |
| **AC-07 Boundary** | Strict `RD>1`; order by decreasing pre-growth suppression then ordinal ID; recompute after each removal; stop at RD≤1 or count≤1; hazard and boundary victims disjoint | Exact RD=1 no-removal, above-one removal, tie order, singleton retention | V-07/08 |
| **AC-08 Age and death** | `age` is chronological biological age; `deathYear` is ecological step year; a Year-2 death of an age-30 tree is age 32/deathYear 2; no repeat death; pre-inventory dead has null cause/year | Stored fields and event uniqueness | V-10 |
| **AC-09 Atomicity** | Exactly one committed successor; stale, duplicate and wrong-year requests refuse with unchanged state; a pre-commit failure publishes nothing | Object identity, year, revision, events, roster hashes | V-11 |
| **AC-10 Continuity** | The next year starts from the full-precision committed living state; CI is recomputed; a Year-k death changes Year-(k+1) neighbours only; no same-year second growth | Cross-year vectors, distinct cached-vs-recomputed values | V-12 death branch |
| **AC-11 Identity/determinism** | Stable case-sensitive ordinal IDs; input order cannot change arithmetic; lineage, seed, model and numeric-version bindings distinguish otherwise identical runs | Permutation and lineage-refusal tests | V-13 |
| **AC-12 Evidence boundary** | No GROWFOR stand equation is imported as a tree-level rule; all calibration proposals are labelled; no Irish forecast is claimed; unsupported cells remain unsupported | Label audit and comparison-only restriction | A2 §8–12 |

A future implementation gate should also require: an approved Year2 spec, resolved §9 owner choices, a named source writer and physical/security grant, exact candidate SHA, independent M-1 source review before `1→2`, and a separate lawful offline/native test scope. None is created by this pack.

---

## 8. Targeted future test plan (all NOT RUN)

| Test | Vector(s) | Preconditions | Pass observable | Status |
|---|---|---|---|---|
| **T-01 M-1 dead target/neighbour** | V-01–V-03 | approved internal harness seam; no fixture change | A/B living equality; dead inert and retained; discriminator rejected | NOT RUN |
| **T-02 Spatial edges and order** | V-04a/b | pinned arithmetic policy; V-04b uses deliberately overlapping stems, so a future minimum-spacing/stem-overlap guard must be disabled for this internal seam or a validated non-overlapping substitute supplied | 8.0 included, next-f32 excluded, 0.5 floor, `0x41a02667` under ordinal order | NOT RUN |
| **T-03 Precision boundaries** | V-05, V-12 | confirmed native evaluation sites | f32 inputs exact; f64 BA/Dq/RD within declared policy; no silent f64 DBH path | NOT RUN |
| **T-04 Hazard conditionality** | V-01, V-06 | ND-01 resolved for real rolls | no roll at p=0; key isolation; strict `roll<p`; cap branch | NOT RUN |
| **T-05 RD boundary** | V-07a–d, V-08 | survivor seam | exact 1 no removal; above-1 A then B; hazard exclusion; singleton retained | NOT RUN |
| **T-06 Singleton/empty policy** | V-08, V-09 | owner OPEN-B2 decision | exactly one chosen branch; no automatic empty step; no NaN | NOT RUN |
| **T-07 Age/death freeze** | V-10 | owner OPEN-B3 decision | age 32/deathYear 2; one event; freeze as approved | NOT RUN |
| **T-08 Transaction/refusal** | V-11 | approved request identity | one successor; stale/duplicate unchanged; no partial mortality | NOT RUN |
| **T-09 Ten-step horizon** | V-12 | ten actual one-year commits | fresh CI each step; no cached pressure; death affects k+1; no second growth | NOT RUN |
| **T-10 Identity/permutation** | V-13 | approved lineage codec | order-independent roster; duplicate ID refused; lineage separation | NOT RUN |
| **T-11 Year1 protection** | published witness | separate lawful native scope | old `0→1` bytes/refusals unchanged; no regenerated golden | NOT RUN |
| **T-12 Evidence comparison** | A2 CM rows | calibration lane decision | report differences, do not tune to a target; U cells remain U | NOT RUN |

Test order matters: M-1 and Year1 no-drift precede any Year2 enablement; hazard/boundary precede multi-year runs; evidence comparison is last and cannot gate source correctness.

---

## 9. Owner decision sheet (short)

Each item is a real product/technical choice. The recommendation is **conditional** and is not adopted by this pack or by the parent. Choosing none means no Year2 code.

| # | Decision | Options | Conditional recommendation | What it does not decide |
|---|---|---|---|---|
| **D-1** | Zero living adults: continue as a labelled empty year or stop? | (a) terminal/typed refusal with unchanged state; (b) explicitly labelled empty-year progression with N/BA=0 and unavailable Dq/top height | (a) is the safe default until the owner explicitly wants empty history; (b) only with typed absence semantics | Does not set a product duration, save format or UI behaviour |
| **D-2** | Dead-row freeze and age-at-death | (a) freeze final post-growth size/age/cause/deathYear forever; (b) another explicit policy | (a), with `age` chronological and `deathYear` ecological; pre-inventory dead has null cause/year | Does not fabricate an actual Year2 death or a resurrection/recruitment rule |
| **D-3** | Ten-year horizon | (a) test-only proof horizon of ten sequential steps; (b) actual product duration/limit | (a) | Does not create a "Year 10" button, forecast or lifespan |
| **D-4** | Work between years | (a) no work in this slice; later only *committed ExecuteWork* may mutate before a step; (b) broaden now | (a); marks/proposals/approvals never fell a tree | Does not define the future work-command lifecycle |
| **D-5** | Public step/identity/history | (a) versioned separate opt-in one-year runner with expected year/revision and private lineage; (b) reuse/rename the old one-year command | (a); no persistence or canonical hash yet | Does not choose public names/error codes or a save schema |
| **D-6** | May GROWFOR be used at all? | (a) comparison-only aggregate benchmark for pure even-aged Sitka; (b) import its equations into gameplay | (a), labelled `BroadLynch2006_Sitka`, version-caveated, never tree-level | Does not accept a GROWFOR coefficient or validate the engine |
| **D-7** | Initial synthetic stand and calibration targets | (a) keep the frozen synthetic stand, labelled gameplay; (b) re-anchor to NFI/Frenchpark context; (c) wait for missing evidence | (a) for the M-1/chronology pack; (b) only as a labelled comparison, not a fitted target | Does not choose a site class, size distribution or mortality calibration |
| **D-8** | Mortality/realised-crown/within-stand evidence gaps | (a) accept them as unsupported and keep gameplay constants labelled; (b) commission new evidence before Year2 | (a) for the no-intervention numerical slice; (b) before any claim of biological realism | Does not accept the S⁴/0.08/0.6 constants as Irish law |

Additional questions the Manager may need to route, not owner-product choices: whether a Year2 continuation spec correction lane is commissioned; whether the old `0→1` command remains permanently separate; and which manager owns the future history/persistence contract.

---

## 10. Material policy decisions proposed by this pack (not adopted)

1. **Treat M-1 as a hard precondition, not a test-strength note.** Repair living-only target/neighbour filtering before enabling `1→2`, and prove it with V-02/V-03 plus the Year1 no-drift anchor.
2. **Do not manufacture unknown goldens.** Rolls, full height/crown outputs and history hashes stay NOT DERIVABLE until lawful source inspection and a written numeric policy exist. A test pack that invents them is worse than one that marks the gap.
3. **Preserve the numeric boundary.** Keep f32 tree/CI/area and f64 BA/Dq/RD storage; publish the evaluation sites; do not silently "clean up" arithmetic into one precision.
4. **Keep the ten-step horizon test-scoped.** Ten sequential transactions prove continuity and chronology; they do not forecast an Irish stand or define a game cap.
5. **Use GROWFOR as a comparison only.** Its stand-level B/N/H/V envelope is a plausibility benchmark for pure even-aged Sitka; it is not tree-level competition, mortality or regeneration evidence.
6. **Do not import a new ecological coefficient from this pack.** The S⁴ normalisation, RD onset, hazard strength, crown relaxation and Ci50 remain labelled gameplay calibration.
7. **Protect the qualified Year1 witness.** No fixture, CLI or digest change; any later native re-run is separately authorised and must reproduce the old bytes/refusals before any Year2 claim.
8. **Keep unresolved states unresolved.** Zero-living, freeze-at-death, public command/lineage and product horizon remain conditional until the owner decides; code must not silently pick a branch.

---

## 11. Explicit NOT RUN register

- No product engine invocation; no Year2, Year3 or Year2–10 simulation; no synthetic or real stand trajectory executed.
- No .NET build, native test, P1 binary, Unity, Mono or Windows execution.
- No read, copy or re-hash of the #639 canonical fixture, the old CLI stdout, any protected #710/#725/P0 source, #687 file or product source.
- No source inspection, source edit, branch in a production worktree, Ops board/lane write, main edit, Host/save, viewer #776, security #779, Scenario One or #770 work.
- No primary paper/PDF re-read; all P† values remain "cited via secondary synthesis; primary not re-read".
- No GROWFOR reconstruction, B/N/H/V run, thinning/volume calculation or calibration fit.
- No field validation, no Irish forecast, no tree-level validation, no Unity parity, no save/history encoding.
- No L4 waiver; FAIL38 remains unwaived.
- Dashboard/Drive synchronization NOT VERIFIED.

The only computations executed were (a) A1's independent synthetic binary32 arithmetic, (b) A2's standard-library derivations from printed numbers, (c) the parent's own binary32 spot-checks, and (d) Q's review arithmetic. None is an engine run.

---

## 12. Readback and integrity appendix

**Base and lane checks.** Fresh `origin/main` was `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`. A1's compare against it showed 1 commit ahead, 0 behind, exactly one added Markdown, +1,105/−0. A2's original reviewed commit showed 1 ahead, 0 behind, one added Markdown, +376/−0. The bounded correction added a second commit on the same branch; the compare from main still shows exactly one added file, +387/−0, and the correction-to-correction compare shows one file, +17/−6. Parent readback of A1 returned 85,144 bytes and SHA-256 `2849ee8e…112a`; A2 at the reviewed commit returned 54,283 bytes and `bd700db7…daeb`; A2 at the corrected head returned 56,567 bytes and `403c5872…1c5d`. All equal the authors' handoff values.

**Document integrity.** The A1 and A2 reports each carry their own status header, registration comment, base commit, scope/exclusion statement and NOT RUN list. Neither claims a source read, fixture hash or engine run. Both keep `#737`/`#760` DRAFT and preserve the qualified Year1 boundary.

**Parent file readback.** This parent document cannot contain its own final byte count and SHA-256 without changing them. The publication commit, byte count and digest are recorded in the companion [HANDED-OFF comment on #780](https://github.com/jermur788/CCF/issues/780) and in the return comment on #730; the branch head readback is the authoritative value. No claim is made here that a self-referential digest exists.

**Parent's own scratch.** `/tmp/opencode/multi780/parent_check.py` and `parent_check2.py` implement an independent `struct`-based binary32 model and reproduce the §4.2 values; they read no repository source or fixture and are not published as an approved harness. The parent also recomputed A2's `D-10` values (25.3033/25.8558 cm, 55.873/58.340 m²/ha), the RD=1 Dq `32.1592 cm`, and the expected Year-1 deaths `0.0706`.

**Independence.** A1, A2 and Q each used a distinct model and session; Q is the formal nonauthor reviewer and did not write either author's file. The parent wrote only its own file and the #780 claim comments; it does not self-approve child research. The provider switch for Q is disclosed in §3 and §6.

---

## 13. Return to Multi Manager #730 and next authorised step

**This is one complete parent result for #730.** It delivers the requested independent quantitative and forestry research pack without creating source authority. Q initially returned NEEDS BOUNDED CORRECTION for A2 only; the bounded correction was applied and passed a non-Anthropic recheck, so the pack now has no open review findings.

**Manager may now:** (1) accept or return the pack on documentary/research quality; (2) use §9 to obtain owner decisions; (3) if accepted, commission the Year2 continuation-spec correction and independent spec review; (4) only after those gates, consider a separately owned source-permission packet with physical/security grants.

**Manager may not infer from this pack:** an approved Year2 spec, a calibrated mortality/growth model, a valid ten-year forecast, a fixture/save/history digest, an engine test pass, an L4 waiver, or permission to read/write protected source, Ops, main, Unity or Windows.

**Recommended immediate next step:** accept the documentary pack, keep #737/#760 DRAFT, and route D-1/D-2/D-3/D-5 in one bounded owner decision sheet. Do not start Year2 source until M-1 is independently source-reviewed and the Year1 golden is re-proven under a lawful native scope.

— DeepSeek V4.1 Flash parent, 2026-11-18.
