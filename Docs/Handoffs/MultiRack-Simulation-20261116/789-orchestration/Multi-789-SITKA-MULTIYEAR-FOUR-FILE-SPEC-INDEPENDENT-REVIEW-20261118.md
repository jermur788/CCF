# Multi #730 / #789 — independent nonauthor review of the Year2–10 pure-Sitka four-file specification

**18 November 2026 UTC. VERDICT: NEEDS CHANGES (0 BLOCKING / 1 MAJOR / 8 MINOR / 5 NIT).**

> **This is a DOCUMENTATION REVIEW. It is not implementation evidence, not test evidence, not source access, not a native qualification and not a security acceptance.** No protected source, fixture, .NET build or Year2 run was touched. Approval of the specification, by me or anyone, would not be source access, implementation or qualification.

## 1. Header

| Field | Value |
|---|---|
| Reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code harness inside T3 Code, native Linux |
| Host | `jer-laptop` |
| Session id / reasoning effort | Not observable to me; neither is claimed |
| Review worktree | `/media/jer/ZX20/CCF-789-SpecReview-20261118` (pre-existing, clean at start) |
| Review branch / base | `review/multi-789-sitka-multiyear-four-file-spec-20261118-v1` from `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` |
| **Commit reviewed** | **`d468e989dcc1c58f3a6910c79c8b2b209ed0ea77`** on `docs/multi-789-sitka-multiyear-four-file-spec-20261118-v1` (commit time 2026-11-18T23:21:07Z). Diff against base `a69b3e2e` is exactly four added files. |
| Files as published | `spec.md` 47,476 B · `plan.md` 12,514 B · `tasks.md` 10,646 B · `checklists/requirements.md` 27,216 B. Sizes equal the brief. Read with `git show d468e989:<path>`; nothing modified. |
| Spec author (not me) | OpenAI GPT-6.1 Sol, Codex in T3 Code (per the spec header) |
| Independence | I am not the author. I share only a model name with the #780 A2 author (a different session); I neither wrote nor reviewed A2, and I rely on #780 only as quoted by S-04/S-05 and the Manager verdict. No subagent was used. No GPT-6.2. |

### 1.1 What I read, and at which ref

| # | Authority | Ref read | Completeness |
|---|---|---|---|
| 1 | Orchestration packet | `2dcd2144` | whole |
| 2 | #737 four-part DRAFT | `da37b23b` | whole |
| 3 | #760 decision/errata DRAFT | `149b4c80` | whole |
| 4 | #780 parent result; #730 research verdict | `d2381e7b`; `229992e3` | both whole (A1/A2/Q child files not read) |
| 5 | #725 / #729 / #732 | `6e51dcb6` / `8310a380` / `88e82cd0` | all whole |
| 6 | Owner-approved P1 four-file baseline | `90583811` | `spec.md` §0–§3, §4 head, §5.2–§5.3; **all of** `checklists/requirements.md`; approval receipt skimmed. **Not read:** P1 `plan.md`, `tasks.md`, spec §4.1, §5.1, §6–§9 |
| 7 | Main `decision-log.md` D-082, D-084 | `a69b3e2e` (still `main` per `git ls-remote` at review time) | rows read |
| 8 | #779 result; Manager G1b verdict; **#790** result and recovery packet | `77f708af`; `fd3d342c`; `3fc98d6e`; `b78af459` | #779/G1b whole; #790 result whole; #790 packet through Stage 3 start |

Paths in this document that contain `725` are published GitHub document names read from Git objects. I did not read, search or open any protected local source tree.

## 2. Verdict on the specification

**NEEDS CHANGES.** The four files are a careful, honest and largely consistent specification. All four owner decisions are implemented without drift; the Year1 witnesses are bound byte for byte; M-1 is a hard first gate with an independent changed-lines numeric review; every number I could recompute reproduces; nothing is presented as run, approved or granted; and the security gates are stated as gates the spec cannot grant. I found **no BLOCKING defect**.

I cannot recommend APPROVED because of **one MAJOR defect in the very gate the spec calls its hard prerequisite** (R-01). The only M-1 discriminator the spec names (S-03 §4 / S-04 V-01…V-03) runs at **nonpositive pressure**, where the spec itself says source mortality work is skipped (§5.2 "Pressure" row). Such a vector cannot detect a dead row leaking into mean vulnerability, the hazard candidate list, roll selection or boundary ordering, which are three of the nine participant exclusions that requirement R-07 demands. A G2 pass on T-02 as written could therefore admit a repair that silently changes Year2+ victims. Eight MINOR and five NIT items are mostly evidence-tag, authority-register and cross-reference hygiene.

Every correction below is a bounded text change inside the existing four files. None needs a new owner decision, a new number, or source access. Suggested recheck scope: a changed-lines check of R-01 plus text confirmation of R-02…R-09.

## 3. Findings

Severity: **BLOCKING** stops approval; **MAJOR** must be fixed before approval; **MINOR** should be fixed, does not by itself stop approval; **NIT** is optional polish.

| ID | Sev | Location | Defect | Permissible correction | Acceptance test that would catch it |
|---|---|---|---|---|---|
| **R-01** | **MAJOR** | spec §3 [P] ¶ "T-02 must compare…"; §5.2 "Pressure" row; checklist R-07, R-09, T-02, T-03, T-04, T-13; tasks B-02/B-03; plan G2 | R-07 requires dead rows to be excluded from nine participants, including "mean/hazard/boundary". The only named G2 A/B construction is S-03 §4 / S-04 V-01…V-03, and S-04 states for V-01 "`p=0`, no roll, no death". §5.2 says "Nonpositive pressure skips source mortality work". With P≤0, the code that could admit a dead row into mean(S⁴), the hazard candidate list, roll selection or boundary sort never runs, so a bit-identical A/B proves nothing about it. T-03 requires only a comparison to "a living-set-only calculation" and does not require P>0 or RD>1. T-04 and T-13 contain no dead row. T-07 compares repeated runs of the same engine, which would leak identically. So no test in T-00…T-18 requires an independent or living-set-only comparison for a dead row held in a positive-pressure or RD>1 state. *Illustration only (not a spec value):* in the V-01 geometry a wrongly admitted dead 50 cm target would have CI 0.5667, S⁴ ≈ 1.07e-4, and would join mean(S⁴), visible only when mortality runs. | Require, inside T-02 or a companion T-02b, at least two controls where the dead row would matter if admitted: **(a)** positive pressure with a living tree at 0<p<0.5; **(b)** RD>1 on hazard survivors, with a dead row whose wrongly computed S would add or reorder a boundary victim. Expected values must be derived independently from the lawfully inspected source at G1 (the spec rightly invents none). Add a mutant clause: implementations that admit the dead row at each of the three points must fail. | T-02b: clean versus dead-row-present states give bit-identical living p, mean S⁴, roll keys and outcomes, hazard victims, boundary order and running-RD sequence. B-03's reviewer mutants (dead row admitted to mean, to candidates, to boundary) each fail. |
| R-02 | MINOR | spec §1.1 S-01…S-07, S-11; §3 ¶ "Required discriminator"; R-08, T-02 | S-01…S-07 link to **branch heads** and S-11 to **`main`**; only S-08…S-10 are commit-pinned. These move, so the register cannot freeze what the packet or D-082 said when approval is given. Separately, the discriminator values appear without their construction ("Full construction is in S-03 §4"). The construction is T = 20 cm at (0,0), L = 25 cm at (4,0), dead D = 50 cm at (1,0). "Target CI 0.3125" is the 20 cm tree's; the 25 cm tree's CI is 0.2000000030. A reader of the four files alone cannot tell which tree is meant. | Pin every S-nn to a commit SHA (the refs I read are in §1.1) and restate the three-tree construction in one line in §3 or R-08. | T-00 audit: each register link resolves at its pinned SHA, and the restated construction reproduces the four quoted values (§4 below does this). |
| R-03 | MINOR | spec §1.2 D-1 row; §5.1 G; §5.3 ¶2; §9.1 "S4 phase G"; R-13; T-15 | The rules "hazard is not waived for singleton" and "preserve source conditional hazard logic" omit two source-derived facts that decide what T-15 must assert. **(i)** A singleton has CI = 0, so S = 0, mean S⁴ = 0 and p = 0 with no Roll. S-09 §6: "singleton: S = 0 → mean vulnerability 0 → p = 0, roll null, under pressure 96"; S-04 V-08: "`p=0`". So the singleton is hazard-immune by the source formula, not by a new policy. **(ii)** At the trusted 0.09 ha the `count≤1`-while-RD>1 guard is **unreachable**: my arithmetic gives max RD 0.434 for N=1 and 0.868 for N=2 at the 200 cm clamp, and a singleton needs Dq ≈ 299.8 cm for RD=1. S-07 §0 m04 covers only the 40/80 squares; the spec says "tiny-area probe" but not that the trusted 30 m domain cannot reach it either. Consequence: zero-living is reachable only by simultaneous hazard loss of every one of N≥2 living trees. | State (i) and (ii) in §5.3 as evidenced or proposed facts (or ND if G1 source reading disagrees) and align T-15's expected result. | T-15 asserts: real-path singleton at P>0 has p=0 and no Roll call; the RD>1 guard is exercised only through the labelled probe. T-06 notes how zero-living is actually reached. |
| R-04 | MINOR | spec §1.2 (table header "[A] Accepted product intent", Consequence column) | The Consequence cells carry spec-derived inferences under the [A] tag. Example: D-1 "A step that kills the last adults may commit; a subsequent empty step cannot." S-01 says only "If zero living adults remain, another annual advance is refused". D-4 "natural mortality remains this engine's only population loss" is likewise derived. The inferences are reasonable and consistent but are [P]. | Tag the Consequence column [P], or quote S-01 where it is literal (the D-2/D-3 consequences are). | T-00 evidence-tag audit of §1.2. |
| R-05 | MINOR | spec §1.1 S-06; §7; plan §3, §4; tasks §1 | **#790 is absent from all four files** although it was published (21:54–22:03Z) before the spec commit (23:21Z). #790 records two further unmet admission facts. First, the confined-writer model routes are blocked: `openai/gpt-6.1-sol` HTTP 401 (stored OAuth expired 2026-11-13), `anthropic/claude-sonnet-5-5` "invalid x-api-key", console route "Reconnect OpenCode Console". The frozen launcher keeps its own credential copy, so an owner re-login on the host will not reach it. Second, the owner's storage correction (recorded in #790, originating on #784): no new root-drive clones, worktrees, builds or large caches (root 99 % used, 3.6 GiB free). §7 lists the gates the spec cannot grant but not these two. Plan §3 says only that durable evidence goes under `/media/jer/Files/CCF/`; it does not say .NET build, NuGet and `obj` output must stay off root. | Add #790 to the S-06 register row and to the §7 gate list as unmet facts (evidence, not solved). Add a one-line "no build, restore or cache output on the root disk" precondition. | T-18 dossier must include a per-actor confined model-route PASS receipt and a storage-placement receipt before any native read, build or test. |
| R-06 | MINOR | spec §1.1 S-08, §2.2 last [E] ¶, §7 | S-08's author receipts (43/43, second-call checks) are tagged [E] with no caveat. #732 states "the network-free status of the author's .NET build/test receipts is unproven in the same way" (the #725 lane holds the same `api.nuget.org` http-cache and `sdk-advertising` artefacts). §7 cites #732's NuGet stop but not this consequence for S-08. Only the #732 offline resume (an OS network namespace, per #732) is network-isolated evidence. | Carry #732's caveat in the S-08 row and in §2.2. | T-00 evidence audit: S-08 is not cited as network-clean. |
| R-07 | MINOR | spec §2.1 ("the abandoned custom viewer sandbox") | No cited authority says "abandoned". S-06 (#779) describes a frozen launcher v1.9 with residuals R1–R4 and says only that #776 "rightly rejected" the "optional per-command `bwrap`"; #790 still exercises the frozen launcher on 18 Nov 22:03Z. This is an untagged status claim. It has no safety effect (§7 takes no position and proposes no reuse), but it is unevidenced. | Use neutral wording ("the #776/#779/#790 viewer confinement work") with an evidence tag, or drop the clause. | T-00 evidence-tag audit. |
| R-08 | MINOR | spec §3 last sentence; §5.2 last ¶; §7 banner "G2 SOURCE HELD"; §8 PR-06; plan §2 | Two different gates are both called "G2" in one document: **plan G2** (M-1) in §3 and §8 PR-06, and **Manager G2** (source permit) in the bold §7 banner. §7's last ¶ and plan §2 disclose the clash, so this is mitigated. The failure it invites, someone reading "G2 passed" as source permission, is exactly what the gates exist to prevent. | Qualify every occurrence ("plan G2" / "Manager G2") or rename the plan gates. | T-00 grep: no unqualified "G2" in the four files. |
| R-09 | MINOR | plan G0 exit; spec §10; checklist R-01, T-00 | G0 requires "explicit **applicable** product-owner/spec approval plus Manager disposition". "Applicable" is never defined. S-01 Stage A needs only a Manager APPROVED/NEEDS CHANGES/REJECTED; S-07's R-8 requires an owner decision for the P1 spec. Two readings of the same gate. | The Manager states which approver set G0 requires. | T-00 evidence names the approver(s) and cites their record. |
| R-10 | NIT | checklist §2 vs §3 | Cross-listing asymmetries. R-03 cites T-18, but T-18's "Maps" omits R-03. T-03→R-07, T-04→R-21, T-06→R-14/R-25, T-07→R-21, T-11→R-20, T-13→R-26, T-14→R-16, T-15→R-19 are not echoed in the R items. I verified every R has ≥1 test and every T has ≥1 R, so nothing is untested. | Add the reciprocal references. | Scripted two-way trace check. |
| R-11 | NIT | spec §1.2, §8, §9 | ID namespaces overlap S-04's. Spec D-1…D-4 versus S-04 §9 D-1…D-8 (D-5…D-8 are different items). Spec ND-08/ND-09 split S-04's single ND-08. Phase letters A–H versus #737 A–I (mapped in §9.1). The renumbering is not disclosed. | One sentence stating the mapping. | T-00 read-through. |
| R-12 | NIT | plan G3 entry vs tasks B-04 | Plan G3 says "PR-03/04/05 private state/refusal/diagnostic choices approved, PR-01/02 resolved for actual exposure". Refusal classes are PR-02 (spec §8), and tasks B-04 requires "needed PR-02" because D-1's typed terminal reason needs a private class. | Align plan G3 with tasks B-04. | T-00 cross-file check. |
| R-13 | NIT | spec §5.3 ¶1; PR-04 | S-07 §5.3 already fixes the baseline empty-stand rule: "empty stand H=null, N=B=0, Dq=null, source RD=0". The spec defers all empty-final representation to PR-04 without citing it. Compatible, not cited. | Cite S-07 §5.3 as the baseline; keep representation as PR-04. | T-06 compares to the cited baseline. |
| R-14 | NIT | spec §1.1 S-07 row; §4.1; PR-03/PR-07 | The spec departs from S-07 §5.2's event envelope (sequence, command ID, pre/post state hash, previous-event digest, `PlanStale`/`ReservedTreeDied`). This is authorised by S-01 §0's "not adopted" list but never tabulated, while the S-07 row says §5 "controls". | A short divergence table with each reason (D-4, PR-03, PR-07). | T-00 read-through. |

**Count: BLOCKING 0 · MAJOR 1 · MINOR 8 · NIT 5.**

### 3.1 Dimension scorecard (A–K)

| Dim | Subject | Result | Findings |
|---|---|---|---|
| A | Four owner decisions | PASS | R-03, R-04 |
| B | Year1 preservation | PASS | none |
| C | M-1 completeness / first-ness | **PARTIAL** | **R-01**, R-02 |
| D | Independent arithmetic | PASS (all recomputable) | none; see §4 |
| E | Biology and honesty | PASS | R-03 |
| F | Determinism, identity, concurrency, rollback, refusal | PASS | none |
| G | Authority hygiene | PARTIAL | R-02, R-07, R-08, R-14 |
| H | Security and admission honesty | PARTIAL (completeness only) | R-05, R-06, R-07 |
| I | Evidence classification | PARTIAL | R-04, R-06, R-07 |
| J | Internal consistency | PASS | R-10, R-12 |
| K | Executability | PARTIAL | R-01 (§8) |

### 3.2 Integrity checks behind the PASS results (mechanical)

- **Quoted constants.** Both SHA-256 values, `45,980`, `32,567` and `3db851ec` appear in the four files exactly as in S-01 (string comparison). I did **not** re-hash anything.
- **URLs.** All 15 URLs in `spec.md` resolve to a real file at the stated ref (checked with `git cat-file` at each branch head or SHA).
- **Checkboxes.** 30 unchecked, 0 checked in `requirements.md`.
- **Test coverage.** 30/30 R items cite ≥1 T; 19/19 T items map to ≥1 R (asymmetries in R-10).
- **Crosswalk spot-checks (well over the six required; all accurate).** I compared the destination rows against the source clauses for: #737 S1.1–S1.5 → §2.1/§2.2/§3; the S2 bullets (binding, chronology, roster, ledger, numbers, successor identity, dead-stem); S3 → §3; S4 phases A–I → §5.1 A–H (letter shift disclosed); S4 empty/singleton → D-1 supersession; Part II G0–G5 → plan G0–G5; Part III B-01…B-07 → tasks B-01…B-08; B-R01…R08; B-D01…D07; T01…T12; #760 B-A01…B-O06 (14 of 14 IDs present); #760 B760-T01…T12; and the #780 AC-01…AC-12 and T-01…T-12 mappings in checklist §4. The #737 S5/S6 rows were checked for existence and destination coherence only. No dangling reference found.
- **Statements that something ran.** None found. Every status line, plan gate and checklist row says NOT RUN / unchecked / not approved.

## 4. Numeric recheck

Method. `python3 -I -B`, standard library only, no fixture, no source. "f32" means each operation computed in binary64 and rounded once to binary32; for + − × ÷ √ that is exactly the correctly rounded binary32 result (53 ≥ 2·24+2). "f64" means CPython `float` with `math.pow` (platform libm). Script and output: `recheck789.py` / `recheck789.out` in my session scratchpad, not committed.

### 4.1 Discriminator (spec §3, R-08, T-02; S-03 §4 / S-04 V-01…V-03)

| # | Quantity | Quoted | My recomputation | Width | Verdict |
|---|---|---|---|---|---|
| N-01 | CI(T), living only | `0.3125` | `0.3125` (`0x3ea00000`) | f32 | **PASS** |
| N-02 | CI(T), dead D wrongly included | `2.8125` | `2.8125` (`0x40340000`) | f32 | **PASS**; same value whether D or L is summed first |
| N-03 | Requested DBH gain, CI 0.3125 | `0.9411764740943909` | `0.9411764740943909` (`0x3f70f0f1`) | f32 | **PASS** in both the DIV and the reciprocal profile |
| N-04 | Requested DBH gain, CI 2.8125 | `0.6399999856948853` | `0.6399999856948853` (`0x3f23d70a`) | f32 | **PASS** in both profiles |
| N-05 | Post DBH of T (context; S-04 V-01) | `20.9411773681640625` | `0x41a78788` = that value | f32 | PASS |
| N-06 | CI(L) and post DBH of L (context; S-04 V-01) | `0.20000000298023224`; `25.9134616851806640625` | `0x3e4ccccd`; `25.913461685180664` (`0x41cf4ec5`) | f32 | PASS |

Intermediate roundings recorded (f32, T = 20 cm; T←L = (25/max(1,20))/max(0.5,4); T←D = (50/max(1,20))/max(0.5,1)):

| Step | Value | Bits |
|---|---|---|
| dist(T,L), dist(T,D) | `4.0`, `1.0` | `40800000`, `3f800000` |
| term T←L = 1.25 ÷ 4 | `0.3125` (exact) | `3ea00000` |
| term T←D = 2.5 ÷ 1 | `2.5` (exact) | `40200000` |
| D/120 *(rounded)* | `0.1666666716337204` | `3e2aaaab` |
| 1 − D/120 *(rounded)* | `0.8333333134651184` | `3f555555` |
| 1.2f × 1f × clamp01 *(rounded)* | `1.0` | `3f800000` |
| CI/5; 1 + CI/5 (clean) | `0.0625`; `1.0625` (exact) | `3d800000`; `3f880000` |
| gain, DIV and reciprocal (clean) | `0.9411764740943909` | `3f70f0f1` |
| CI/5; 1 + CI/5 (wrong) | `0.5625`; `1.5625` (exact) | `3f100000`; `3fc80000` |
| gain, DIV and reciprocal (wrong) | `0.6399999856948853` | `3f23d70a` |

Observation: the DIV and reciprocal profiles (S-04 ND-04) give identical bits for **both** discriminating vectors, so the quoted values do not depend on that last-bit ambiguity. The exact-real values are 0.94117647058… and 0.64, so the quoted figures are f32-rounded gains, as the spec labels them ("requested DBH growth").

### 4.2 Stand arithmetic and the #725 RD values (spec §5.2 "Stand", "Relative density", "Pressure")

The four files **do not quote** the RD values; the brief does. I verified them from #725's published inputs. The 100 DBHs are in the protected fixture, so **Dq itself is not recomputed**.

| # | Quantity | Published (#725) | My recomputation | Width | Verdict |
|---|---|---|---|---|---|
| N-07 | Declared area bits | `3db851ec` | `f32(900f/10000f)` = `0.0900000035762787`, `0x3db851ec` | f32 | **PASS** |
| N-08 | Initial N/ha | `1111.111083984375` | `f32(100f/area_f32)` = `1111.111083984375` (`0x448ae38e`) | f32 | **PASS**. A binary64 `100/0.09` gives `1111.111111111111`. |
| N-09 | Initial RD from Dq `25.303288374529917` | `0.609795828454288` | `0.609795828454288` using f32 N/ha widened, formula `Nha·pow(Dq/25,2.063)/1868` | f64 | **PASS (reproducible)**. With binary64 N/ha: `0.6097958433418817` (+2.44e-8 relative). |
| N-10 | Post-growth RD from Dq `25.855791791844318` | `0.6375836886879149` | `0.6375836886879149` (same method). With binary64 N/ha: `0.6375837042539233`. | f64 | **PASS (reproducible)** |
| N-11 | Pressure | `0.09395922171978738` | `(0.6375836886879149−0.6)/0.4` = `0.09395922171978738` | f64 | **PASS (exact)** |
| N-12 | BA/ha initial and post-growth | `55.87290918844456`; `58.33954510260964` | `Σd²·π/40000 ÷ area_f32` with Σd² = 100·Dq² gives both **exactly**. Using exact `0.09` gives `55.8729114086345` and `58.33954742081489` (+3.97e-8 rel.), equal to the #729 §5 "wrong area" figure. | f64 | **PASS**. Confirms the spec's "declared f32 area widened without decimal replacement". |
| N-13 | Σp (uncapped identity N·0.08·P²) | `0.070626682769505345` (#725 "sum of hazards") | `0.07062668276950532`, relative difference −3.9e-16 | f64 | PASS (a consistency identity, not the same accumulation; within #725's stated few-ULP drift) |

Evaluation-order sensitivity found (supports the spec, not a defect): for the RD formula, `Nha·pow/1868`, `(Nha/1868)·pow` and `Nha·pow·(1/1868)` reproduce the published bits, while `Nha·(pow/1868)` is one ULP off (−1.1e-16). The spec's §5.2 statement "algebraic equivalence alone is insufficient", and its deferral of exact sites to G1 (ND-04, PR-06), are therefore necessary and correct.

### 4.3 Singleton arithmetic (supports R-03; model-derived, not a spec quote)

| Case at 0.09 ha, every tree at the 200 cm clamp | N/ha (f32→f64) | RD | vs 1 |
|---|---|---|---|
| N = 1 | 11.1111 | 0.433966 | ≤ 1 |
| N = 2 | 22.2222 | 0.867931 | ≤ 1 |
| N = 3 | 33.3333 | 1.301897 | > 1 |
| N = 4 | 44.4444 | 1.735862 | > 1 |

A singleton needs Dq ≈ **299.755 cm** for RD = 1, beyond the 200 cm clamp. The guard "stop at count ≤ 1 while RD > 1" would need N = 2 with RD > 1; the maximum is 0.868.

### 4.4 Is the numeric staging consistent and unambiguous?

**Consistent: yes.** The staging (f32 tree state/CI/area/N·ha⁻¹ division, f64 stand and mortality reductions, inclusive 8 m, 0.5 m floor, ordinal accumulation, pre-growth CI reused for suppression, post-growth RD for pressure) agrees with S-07 §3, S-09 §3 and the #725 compiled numbers, and the pieces I can reproduce do reproduce. **Unambiguous at last-bit sites: not fully, by design**, and the spec says so (ND-04, PR-06): where the f32 square of a DBH is formed in the Σd² reduction, the association of the RD product, DIV versus reciprocal elsewhere, and how the boundary phase forms its starting sums from hazard survivors. These are correctly deferred to the G1 source map, not silently chosen.

## 5. Conformance table

| Requirement | Quoted clause (location) | Result | Notes |
|---|---|---|---|
| **D-1** zero-living terminal refusal, no mutation | §1.2: "Another step refuses with an explicit typed reason and no mutation." §5.1 B: "If none, typed terminal refusal with NO year/revision/event/roster mutation." §5.3: "D-1 refuses an advance from zero living without evaluating undefined Dq/mean or inventing zero-tree chronology… no NaN/Infinity, fake tree or fabricated next year." §6: "An audit of a refused call must not mutate authoritative state or consume a generation." R-12, T-06 | **PASS** | Supersession of #737 S4 and #760 B-O01 is accurate. Typed name `NoLivingAdultsTerminal` is PROPOSED (PR-02). See R-03, R-04. |
| **D-2** dead-row freeze | §4.3: "Freeze final post-growth DBH, stored height, crown and chronological age; retain source cause `self-thinning` and… `hazard` or `boundary`, separately from ecological `deathYear=targetYear`… No repeat event, growth, age increment, roll, disappearance or resurrection… a pre-inventory dead row never acquires an inferred cause or death year." "…retains **age 32 / deathYear 2**." R-10, R-11, T-05 | **PASS** | Hypothetical age-30 case is labelled "not a claim about any actual Year2 tree". Matches S-07 checklist REQ-E10 ("Natural-dead ages freeze thereafter"). |
| **D-3** ten sequential steps, verification only | §1.2: "ten sequential one-year verification commits only… No game lifespan cap, fast-forward feature, ten-year jump or Irish forecast." §9.1: "D-3 ten commits, no new batch API." R-14, T-07 | **PASS** | T-07 forbids "ten fresh Year1 runs, a jump, codec/hash demand or fabricated later rows". |
| **D-4** zero interventions | §1.2: "No marks, thinning, felling, recruitment, planting, approved/executed Work Plans, save/load or gameplay integration." §2.1 repeats and adds Host, network, persistence. R-06, T-17 | **PASS** | In-memory state across calls (§2.2 [P]) is not persistence. |
| **Year1 preservation** | §2.2: stdout "45,980 bytes, SHA-256 `574d8746…dd53`"; fixture "32,567 bytes, SHA-256 `bcaaa5c3…57c4`, declared binary32 area bits `3db851ec`"; "Still refuses `AnnualAlreadyAdvanced`"; "Still refuses annual with `OriginalAnnualRefused`"; "[P] Multi-year capability must use a separate explicit versioned opt-in path… A fresh old CLI invocation always creates another Year1". T-01, R-02…R-05 | **PASS** | All constants equal S-01. The opt-in rule is tagged [P] though S-01 Stage D makes it [A]; harmless. |
| **M-1** | §3: "Before enabling any 1→2… excluded as CI **targets and neighbours**, and from growth, crown, age, all stand/density stages…, mean vulnerability, hazard candidates/rolls and boundary selection. Retain every identity in the immutable archival roster." Plan G2: "**No 1→2 may be enabled before this exit passes.**" Tasks B-03 independent changed-lines review. Discriminator 0.3125 / 0.9411764740943909 versus 2.8125 / 0.6399999856948853, labelled "synthetic two-tree controls, **not** qualified 100-tree stand outputs" | **PARTIAL** | Requirement text, ordering and labels are right and the numbers are exact (§4.1). The acceptance vector is blind to the mortality phases: **R-01 (MAJOR)**. |
| **Trust / refusal** | §6.1 ¶: "Existing `AnnualAlreadyAdvanced`, `OriginalAnnualRefused` and specific legacy admission strings keep their original meanings and precedence. This table grants no new public vocabulary." "T-10 must never weaken old trust-first refusal to make a new class pass." Table of new names: "**PROPOSED — requires Manager/owner decision**" | **PASS** | Admission order trust → expected identity → terminal → calculation → commit is labelled proposed (multi-fault precedence PR-02). |
| **Determinism / rollback** | §5.2 RNG: "Pinned `SimulationRandom.Roll`, independent RNG version, fixed seed, target ecological year and literal `ADULT-MORT-` + full verbatim ID… No stateful RNG cursor." Hazard: "p=0 means no roll"; "Death iff roll<p"; "Strict `RD>1`". §6: one commit gate; "exactly one publish and one typed stale/duplicate refusal"; "A pre-commit exception, invalid numeric result, invariant failure or cancellation before the commit point leaves the identical prior state reference"; "Prepare reporting before commit". T-04, T-08, T-09 | **PASS** | Matches S-09 §3 (post-increment year key, roll skipped at p=0). Refused advance cannot emit a committed-labelled report (§6.1 `AnnualStepNotCommitted`: "no fabricated committed result"; T-10). In-memory atomicity only, stated. |
| **Authority hygiene** | §6.1 "Every proposed name below is **PROPOSED**"; §4.1 "not a public lineage token, cryptographic chain, historical digest, codec or persistence format"; §4.2 "Do not adopt E #740 `ObservationKeyV1` or any public read ABI"; §8.2 ND-06 "never invent numbers"; §1.1 S-11 "Main D-084 is Scenario One Compact30 and supplies NO Multi authority" | **PARTIAL** | No invented API names, literals, digests, codecs, coefficients or goldens; private/read separation and Genesis/Before/PostGrowth/AfterMortality labels are kept. Hygiene gaps are R-02, R-07, R-08, R-14. |
| **Security honesty** | §7: "**Default .NET L4 FAIL38 UNWAIVED. G2 SOURCE HELD.**" "Local non-public, offline-only, fixed-runtime experiment is the boundary, not a security waiver." R1–R4 listed exactly as S-06. "This spec selects no workaround, weakens no analyzer and accepts no residual risk." The §7 gate list the spec "cannot grant": independent approval of the four files; named custodian grant; physical single-writer lease; qualified confined offline CLI with an actor-specific Manager security/source grant; separate nonauthor review and offline-qualification permission. "A T3-delegated full-access child cannot become a protected-source writer." | **PARTIAL** (completeness only) | Everything stated is accurate and nothing is solved by assertion. Missing: #790 facts (R-05), S-08 network caveat (R-06). No workaround and no reuse of the #779 launcher is proposed. |

## 6. What the spec correctly refuses to decide (PROPOSED register)

All eight PR items carry "PROPOSED — requires Manager/owner decision", and all nine ND items are listed as unavailable with the right handling. I checked that none of these is quietly assumed elsewhere in the four files.

| Item | What is deliberately unchosen | Blocks **spec** approval? | Must be settled by |
|---|---|---|---|
| PR-01 | Public runner/API/command name; opt-in path stays private-seam-only | **No** | before any public exposure |
| PR-02 | Typed refusal names, literals, multi-fault precedence | **No** (D-1's *semantic* class is already required) | a private disposition before B-04, since D-1 needs a typed reason (R-12) |
| PR-03 | Private experiment/predecessor identity, year/generation; no digest/lineage/codec | **No** | before G3 state work |
| PR-04 | Private stage diagnostics, typed absence, empty-final metrics | **No** | before result representation (cite S-07 §5.3, R-13) |
| PR-05 | Compare-and-commit seam, locking, fault seam | **No** | G0/G1 before code |
| PR-06 | Fixed-runtime numeric-site policy, reconciliation arithmetic | **No** | G1 deliverable; independent numeric reviewer before G2 tests |
| PR-07 | Persistence, resume, history codec (excluded by D-4) | **No** | only if a future slice is commissioned |
| PR-08 | New calibration, Year2–10 goldens, GROWFOR use (excluded) | **No** | owner/science reviewer for any future study |
| ND-01…ND-09 | RNG numerators/victims; Class III and later-year height; crown response; native evaluation sites; future victims; native Year2–10 bytes/goldens; history codecs; Irish predictive validity; parity/durability/security status | **No**; honestly listed | lawful source mapping, independent oracles, or separate authority |
| Security/source admission | Held external gates, not PR items | **No**; correctly outside the spec's power | the Manager and custodians |

**Conclusion:** no PROPOSED or NOT DERIVABLE item is itself a reason to withhold spec approval. The one thing I would not leave to later is R-01, which is a verification-design defect rather than an open decision.

## 7. What I did not, and could not, verify

- **No source, no fixture, no build.** I did not read, copy or search any protected local tree, the #639 fixture, or any local path containing `725`, `710`, `687` or `MultiRackSitkaP1Lab`. I ran no `dotnet`, NuGet, Unity or build. I did not recompute either published SHA-256 or any stdout byte; those are string comparisons with S-01.
- **No Year2 run and no native result.** Nothing here shows 1→2 works, M-1 is repaired, a Year2–10 trajectory exists, or any RNG numerator or victim set. All Year2–10 numerics remain NOT DERIVABLE (the spec says so).
- **RD figures from published Dq, not from the fixture.** I verified RD, pressure, BA/ha, Nha and Σp from #725's published Dq/N/area, not from the 100 protected DBHs. The discriminator values I recomputed from first principles.
- **Not read:** P1 `plan.md`, `tasks.md`, spec §4.1, §5.1, §6–§9; #780 child files (A1, A2, Q); GitHub issue/comment threads (e.g. #730, #732 Manager comment 6978632144); Dashboard/Drive synchronization (not verified).
- **Platform facts.** `math.pow` here is the host libm; that it matched #725's published bits supports, but does not prove, cross-runtime equality. No Unity/Mono/Windows parity is claimed.
- **Approval boundary.** A future APPROVED verdict on these four files would approve a documentary specification only. It would **not** be source access, a physical lease, a security acceptance, an implementation, an independent qualification or a release. #779/G1b (G2 source held, confinement not proven), #790 (confined model auth blocked, storage rule) and default L4 FAIL38 (unwaived) all stand untouched by this review.

## 8. Highest-risk ambiguity (dimension K)

**The M-1 acceptance vector cannot see the mortality phases (R-01).** Two competent implementers would both pass T-02 as written (CI, growth and stand totals are bit-identical) yet could differ on whether a dead row feeds mean(S⁴), the hazard candidate list or boundary ordering. That difference would first appear as different Year2+ victims, after G2 has already been accepted. It is the only place where a defect in the repair could pass the hard gate unseen, and it is cheap to close with the positive-pressure and RD>1 controls described in R-01.

Status: HANDED-OFF TO PARENT/Manager #730 — SPEC VERDICT ONLY.
