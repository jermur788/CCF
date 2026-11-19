# Multi #730 / #789 — bounded delta recheck of the corrected Year2–10 pure-Sitka four-file specification

**19 November 2026 UTC (00:29Z at start of writing). VERDICT: APPROVED on `a2509922` as a documentation delta (all 14 prior findings CLOSED; R-01 CLOSED; 0 new BLOCKING / MAJOR / MINOR; 4 optional NIT).**

> **This is a DELTA DOCUMENTATION CHECK. It is not implementation evidence, not test evidence, not source access, not a native qualification and not a security acceptance.** No protected source, fixture, .NET build or Year2 run was touched. It is **not** the G0 approval the four files themselves require (that needs explicit product-owner approval and a Manager APPROVED disposition naming candidate and approvers; see §5 and spec §10).

## 1. Header

| Field | Value |
|---|---|
| Reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code harness inside T3 Code, native Linux |
| Host | `jer-laptop` |
| Provider instance / account | **"Claude B — Pro"**, a separate authenticated account from the first Claude account, **as stated by the dispatch brief.** I cannot observe my own authentication from inside the session, so the account name is the brief's statement, not something I verified. |
| Session id / reasoning effort | Not observable to me; neither is claimed |
| **Continuation disclosure** | This recheck **continues a rate-limited same-model session on a separate account.** The brief says the previous recheck session on the first account hit its 5-hour API limit **before producing any output**, so I inherit no partial verdict, notes or draft. I started cold from the committed prior review and the two commits. I am the same *model* as the original review's author but a different session; I am not the spec author. |
| Independence | Spec author is OpenAI GPT-6.1 Sol (Codex in T3 Code) per the spec header. I authored nothing in the spec. No subagent used. No GPT-6.2. |
| Review worktree / branch | `/media/jer/ZX20/CCF-789-SpecReview-20261118`, `review/multi-789-sitka-multiyear-four-file-spec-20261118-v1`, head at start `b9b4a248` (the prior review commit; clean tree) |
| **Spec before** | `d468e989dcc1c58f3a6910c79c8b2b209ed0ea77` (2026-11-18T23:21:07Z) |
| **Spec after (reviewed)** | **`a2509922bd7a0ed38bb936de3b1790ba86ef4ca7`** on `docs/multi-789-sitka-multiyear-four-file-spec-20261118-v1` (2026-11-18T23:48:35Z, one commit after `d468e989`). Fetched with `git fetch origin docs/multi-789-sitka-multiyear-four-file-spec-20261118-v1`. |
| Delta scope | `git diff d468e989 a2509922` touches exactly the four files under `specs/multirack-sitka-multiyear-annual-v1/` (114 insertions, 81 deletions) and nothing else. Sizes: `spec.md` 47,476→**56,877** B; `plan.md` 12,514→**14,333** B; `tasks.md` 10,646→**11,956** B; `checklists/requirements.md` 27,216→**33,592** B. Read from Git objects with `git show a2509922:<path>`; nothing modified. |
| Prior review | `Multi-789-SITKA-MULTIYEAR-FOUR-FILE-SPEC-INDEPENDENT-REVIEW-20261118.md` (33,745 B, commit `b9b4a248`, 0 BLOCKING / 1 MAJOR / 8 MINOR / 5 NIT) |

Method: whole-file read of all four corrected files, a word-level diff against `d468e989`, mechanical scans (G2 classification, checkbox counts, witness-string counts, byte-for-byte row comparisons, a two-way R↔T trace parsed from the checklist, URL/SHA resolution), spot-checks of the cited documents at their pinned SHAs, and Python arithmetic (binary32 emulated with `struct`; standard library only, `python3 -I -B`). Scripts and intermediates were in my session scratchpad and are **not** committed.

Documents read from Git objects at their pinned SHAs (published document names; no local protected tree opened): #789 packet `2dcd2144` (S-01), #780 parent `d2381e7b` (S-04), P1 spec and checklist `90583811` (S-07), #729 `8310a380` (S-09), #732 `88e82cd0` (S-10), `decision-log.md` `a69b3e2e` (S-11), #790 `3fc98d6e` (S-12), #784 re-entry `18cfe289` (S-13), #760 `149b4c80` (S-03, §4 only). For S-08 (#725 receipt, `6e51dcb6`) I only confirmed that the path exists at the pin (`git cat-file -e/-s`); I did not read its content.

## 2. Verdict on `a2509922`

**APPROVED, as a documentation delta.** All 14 findings from the prior review are CLOSED. The MAJOR (R-01) is closed by text that is mandatory, ordered before any 1→2, bound consistently across all four files, testable, and reachable at the trusted 0.09 ha domain (my arithmetic in §4.6). The corrections are real and bounded: every changed line traces to one of R-01…R-14, no number was added or removed anywhere (no decimal of three or more places appears or disappears in any of the four files), no API/error literal, golden, calibration or persistence/Host/viewer/Unity scope was introduced, and everything frozen is byte-identical (the Year1 witnesses, D-1…D-4 and the PR/ND register). The `G2` ambiguity is gone (0 bare of 43). The four files remain an unapproved DRAFT with 32 unchecked boxes and 0 checked.

I found no new BLOCKING, MAJOR or MINOR defect. Four NITs (§6) are optional. This verdict is spec-level only: it says the corrections do what the prior review asked, not that the specification is approved for implementation (G0 requires more, §5), and not that any test, source map or oracle exists.

## 3. Per-finding table

| ID | Prior sev | Result | Evidence (file:line at `a2509922`) |
|---|---|---|---|
| **R-01** | MAJOR | **CLOSED** | spec.md:83 (T-02 zero-pressure insufficient; both controls mandatory before any 1→2), :85 (T-19), :87 (T-20), :89 (synthetic-only, B-03 mutants, missing proof blocks plan G2 (M-1)); requirements.md:34 (R-07 cites T-19/T-20), :44 (R-31), :45 (R-32), :70, :76, :93, :94; plan.md:23 (plan G2 (M-1) row), :36; tasks.md:22 (B-02), :23 (B-03), :27 (B-07), :36. Full argument §4. |
| R-02 | MINOR | **CLOSED** | All 18 links in spec.md:22–38 and :352 are 40-hex SHA pins; all 18 resolve and the file exists at the pin (list in §3.1). Zero branch-head or `main` URLs remain. Construction restated at spec.md:81 and requirements.md:76. Spot-checks §3.1. |
| R-03 | MINOR | **CLOSED** | spec.md:141 (§5.1 G), :168–172 (§5.3); requirements.md:40 (R-13), :80 (T-06), :89 (T-15). Evidence tags `[P/E: S-09 §6; S-04 V-08]` and `[P/U]`; pending G1; "not … a new immortality policy". Arithmetic reproduces (§4.6). |
| R-04 | MINOR | **CLOSED** | spec.md:44: column header is now `[P] Derived consequence (not an additional owner decision)`; the `[A]` column and D-1…D-4 cells are byte-identical to before. |
| R-05 | MINOR | **CLOSED** | S-12 row spec.md:37; §7 spec.md:206 (Sol HTTP 401, Sonnet invalid API key, console reconnect, private credential copy, storage PASS with root-cache note, "not permanent"), :208 (no new root worktree/clone/build/restore/NuGet/bin/obj/scratch), :216 (per-actor route PASS + storage receipts as a gate); plan.md:22, :38; tasks.md:21 (B-01); requirements.md:63 (R-27), :92 (T-18). Claims verified against #790 (§3.1). |
| R-06 | MINOR | **CLOSED** | S-08 row spec.md:33; §2.2 spec.md:75; §7 spec.md:202. Supported by #732 (§3.1). |
| R-07 | MINOR | **CLOSED** | "abandoned" no longer appears in any of the four files. spec.md:57 now says "the #776/#779/#790 viewer confinement work"; spec.md:59 and S-13 row :38 carry a sourced, viewer-scoped `[E/A: S-13]` statement; spec.md:204 and plan.md:22 say it grants this engine nothing. Supported by S-13 (§3.1). |
| R-08 | MINOR | **CLOSED** | Bare `G2` count **0** (§3.2). |
| R-09 | MINOR | **CLOSED** | One approver set, stated identically at spec.md:350 (§10), :212 (§7 first bullet), plan.md:21 (G0), requirements.md:11, :25 (R-01), :74 (T-00): supporting independent review + explicit product-owner approval of the exact four files + Manager APPROVED naming candidate and approvers, retaining S-07 R-8. Basis check in §3.3. |
| R-10 | NIT | **CLOSED** | Two-way trace parsed from requirements.md: 32 R items, 21 T rows, **0** asymmetries (old commit: 10). R-03↔T-18 now reciprocal (T-18 Maps `R-03/27/28/30`, requirements.md:92). |
| R-11 | NIT | **CLOSED** | spec.md:258 states the mapping. Accurate against S-04 (§3.4). |
| R-12 | NIT | **CLOSED** | plan.md:24 (G3 entry) now requires "required PR-02 private typed-refusal disposition"; tasks.md:24 (B-04) says "needed PR-02/06 dispositions". PR-01/02 *public* naming is separately "resolved or explicitly deferred" in G3 and "needed PR-01/02 interface disposition or explicitly private-only approved seam" in B-05. Aligned. |
| R-13 | NIT | **CLOSED** | spec.md:168 cites S-07 spec §5.3 and quotes `H=null, N=B=0, Dq=null, source RD=0`; requirements.md:80 (T-06) compares to it. Quote verified at S-07 `spec.md` §5.3 (heading at its line 304). |
| R-14 | NIT | **CLOSED** | spec.md:101–109: four-row divergence table (event envelope/sequence/command-ID; pre/post hashes and previous-event digest; PlanStale/ReservedTreeDied; save/load/replay and epoch), each with reason (D-4, PR-01/03/07, S-01 not-adopted list). S-07 §5.2 (heading at its line 268) does contain the envelope and `PlanStale`; S-01 §0 line 18 does contain the not-adopted list. |

**Closed 14 · Partial 0 · Still open 0.**

### 3.1 R-02 / R-05 / R-06 / R-07 spot-checks of cited text at the pinned SHA

All 18 `blob/<sha>/<path>` links in `spec.md` resolve with `git cat-file` (commit objects, files present, none `main` or a branch name): S-01 `2dcd2144`; S-02 `da37b23b`; S-03 `149b4c80`; S-04 `d2381e7b`; S-05 `229992e3`; S-06 `77f708af` and `fd3d342c`; S-07 ×4 at `90583811`; S-08 `6e51dcb6`; S-09 `8310a380`; S-10 `88e82cd0`; S-11 `a69b3e2e`; S-12 `3fc98d6e`; S-13 `18cfe289`; and the prior review at `b9b4a248` (also on `origin` under the review branch). The refs equal the ones I used in the first review, so the register now freezes what I read.

Text actually supports the claim (not just that the file exists):

- **S-01**: contains both Year1 witnesses (`574d8746…dd53`, `bcaaa5c3…57c4`), `45,980`, `32,567`, `AnnualAlreadyAdvanced`, `OriginalAnnualRefused`; §0 line 13 (D-1) and line 16 (D-4) match the spec's D-1/D-4 wording; line 18 is the "Not adopted" list cited by §4.1; Stage A names only "Manager APPROVED / NEEDS CHANGES / REJECTED".
- **S-03 §4** (heading line 65 of that file) gives the T/L construction (T 20 cm at (0,0), L 25 cm at (4,0)) that spec.md:81 restates; spec adds D = 50 cm at (1,0), which S-04 states in its summary ("a dead 50 cm stem at 1 m from a 20 cm living target") and uses in V-02.
- **S-04**: V-01…V-03, V-08 present; the `0.3125 / 2.8125` and `0.9411764740943909 / 0.6399999856948853` pairs present; ND-01…ND-07 match the spec's ND-01…ND-07 by subject; ND-08 is the single combined row that the spec splits into ND-08/ND-09.
- **S-09**: line 104 (§6 test table) "singleton: S = 0 → mean vulnerability 0 → p = 0, roll null"; §7 (Findings) M-1 is the dead-input finding; line 103 shows a singleton at RD 1.51 in a tiny-area control, which supports "tiny-area probes".
- **S-10** (#732): line 50 "the #725 author build receipts were also produced with these network calls"; line 200 "the original #725 author builds remain *not demonstrated network-free* … This resume does not change that." Both the S-08 row and §2.2/§7 carry exactly that, and keep the offline resumption as the only network-isolated evidence.
- **S-12** (#790): Sol HTTP 401 (OAuth expired 2026-11-13); Sonnet `invalid x-api-key`; `opencode/claude-sonnet-5-5` "Reconnect OpenCode Console"; §2.1 "the frozen launcher reads its own credential copy"; storage "PASS with one bounded note" (launcher data/cache hard-coded under `/tmp`, small, pre-existing); header "NO SOURCE". Spec paraphrase matches.
- **S-13** (#784 re-entry): "bespoke #779/#790 confinement sandbox should **no longer be used** for this task … It does **not** waive repository ownership, physical worktree custody, protected source, fixture integrity, non-publication or independent qualification"; "Never create new source worktrees, clones, large test output, package caches or substantial scratch under root". The spec's viewer-scoped reading is faithful. **Disclosure:** S-13 (22:10:49Z) predates the original spec (23:21Z) but I did not read it at the first review, so my R-07 statement "no cited authority says abandoned" was true of the cited set only. S-13 is the missing authority, and the correction cites it properly.
- **S-11**: `decision-log.md` at `a69b3e2e` has D-082 (standalone Sitka experiment) and D-084 (Compact30), so "Main D-084 is Scenario One Compact30 and supplies NO Multi authority" holds.

Dated facts: the spec calls #790 "dated" and pins it to a commit (22:03:47Z 18 Nov), but does not write the observation date in the text (NIT N-3).

### 3.2 R-08 — unqualified `G2`

Classified every occurrence of `G2` in the four files by exact qualifier string:

| File | `G2` total | `plan G2 (M-1)` | `Manager G2 (protected-source/security admission)` | Bare |
|---|---|---|---|---|
| spec.md | 10 | 7 | 3 | 0 |
| plan.md | 7 | 4 | 3 | 0 |
| tasks.md | 3 | 3 | 0 | 0 |
| checklists/requirements.md | 23 | 22 | 1 | 0 |
| **Total** | **43** | **36** | **7** | **0** |

Variants `G-2`, `G 2` and "Gate 2": none. The old commit had 38 `G2` of which 37 matched neither qualified form. plan.md:17 now spells out the distinction and the S-01 stage mapping (`G0/G1/plan G2 (M-1)/G3/G4–G5`); spec.md:219 and plan.md:44 keep the vocabulary warning.

### 3.3 R-09 — is the approver set grounded?

S-01 Stage A names only a Manager verdict ("return one coherent spec for Manager APPROVED / NEEDS CHANGES / REJECTED. Pending such approval Stage B cannot start"). S-07's R-8 (spec line 11 and 374; checklist REQ-A07 line 29) says approval requires an independent review plus delta **and** an explicit product-owner decision, "a Manager verdict alone never substitutes". The corrected text takes the stricter combined set, cites R-8 as its basis, and says the Manager's disposition must name the candidate and approvers. That removes the two-reading ambiguity, cannot lower any bar, and leaves the Manager free to confirm or relax it at the G0 disposition. It is tagged `[A/P]`. See NIT N-4 on the tag in the §7 list.

### 3.4 R-11 — mapping accuracy

spec.md:258 says D-1…D-4 match S-04 §9 D-1…D-4 and that S-04's D-5…D-8 remain unadopted; ND-01…ND-07 follow S-04; S-04's ND-08 is split across spec ND-08/ND-09; phases A–H consolidate #737 A–I. Checked against S-04 §9 (D-1 zero-living, D-2 freeze, D-3 ten-year horizon, D-4 work between years; D-5 public step/identity/history, D-6 GROWFOR, D-7 stand/calibration, D-8 evidence gaps) and S-04 §4.3 (ND table). Accurate.

## 4. Full R-01 closure argument

**State: R-01 CLOSED.**

### 4.1 What was wrong

The only M-1 acceptance vector (S-03 §4 / S-04 V-01…V-03) runs at nonpositive pressure, where the spec's own §5.2 says mortality work is skipped. A bit-identical A/B there cannot see a dead row leaking into mean(S⁴), the hazard candidate list or roll selection, or boundary ordering.

### 4.2 What the spec now requires

| Requirement from the brief | Where, and what it says | Met |
|---|---|---|
| (a) Positive-pressure control, living `0<p<0.5`, where a wrongly admitted dead row would change mean(S⁴), hazard candidates/order, computed p, roll keys or selected victims | **T-19 / R-31** (spec.md:85; requirements.md:44, :93): "a synthetic living stand with genuine post-growth P>0 and at least one living 0<p<0.5, plus an already-dead high-DBH/high-suppression row whose wrongful admission would change the vulnerability mean and hazard candidates/order, and discriminate computed p, conditional roll selection and selected victims." | **Yes** |
| (b) `RD>1` boundary control where a wrongly admitted dead row would add or reorder a boundary victim | **T-20 / R-32** (spec.md:87; requirements.md:45, :94): "post-growth RD>1 and hazard-survivor RD>1 with enough survivors to exercise the boundary … a dead row positioned/sized so wrongful CI participation or admission of its would-be pre-growth suppression into boundary ordering changes the victim set or removal order"; strict RD>1 must genuinely trigger; last-survivor guard unchanged. | **Yes** |
| Bit-identical A/B | T-19: "Clean A/B must match **bit-for-bit** on living CI/growth/crown/age, Before/PostGrowth/AfterMortality metrics, pressure, mean(S^4), per-living p, ordered actual Roll keys/living IDs and exact 24-bit numerators/rolls, selected hazard and final victim IDs." T-20: identical boundary candidate ordering, victim set, removal order and running-RD sequence (see N-2 on the missing word "bit-"). A has the dead row; B has the same living roster without it, with matching bits, IDs, positions, area, seed, target year and versions. | **Yes** (N-2) |
| No roll/event for the dead row | T-19: "D receives no roll or event and remains unchanged"; T-20: "no D removal/roll/event or archive mutation". | **Yes** |
| Independently source-derived oracles at G1 | spec.md:85 "Independently derive at G1"; :87 "Independently derive …"; :89 "Independently confirm native input bits, source oracles and branch activation at G1 under PR-06/ND-01/04/05"; T-19 requires "Independently source-derived actual rolls/victims … no injected-roll substitute, invented future golden or public state input"; tasks.md:23 (B-03) "T-19/T-20 independently derived lawful-source oracles". The spec invents no roll or victim value ("never invent a roll/victim golden now"). | **Yes** |
| Mutants that must fail | T-19: "Separately admit D only to mean and only to hazard candidates/roll selection: each bounded mutant must fail this control." T-20: "Separately admitting D to boundary selection/order must fail the independent oracle." plan.md:23: B-03 "rejects separate dead-to-mean, dead-to-hazard/roll and dead-to-boundary mutants." tasks.md:23 repeats all three, "each detected". R-31 and R-32 carry the same. | **Yes** (all three) |
| Cited by B-02 / B-03 / plan G2 (M-1) | tasks.md:22 (B-02 proof lists T-19 and T-20 with what each compares); tasks.md:23 (B-03; also "**All-living golden plus zero-pressure control cannot pass this gate.**" and "Manager explicitly accepts plan G2 (M-1) before 1→2"); plan.md:23 (G2 row: "mandatory T-19 … and T-20 …", "**An all-living golden plus a zero-pressure M-1 vector is insufficient for this exit**"); tasks.md:27 (B-07 re-runs both natively); tasks.md:36. | **Yes** |
| Still a first gate | tasks.md §1: "B-02 and B-03 must pass before B-04/B-05 enables ANY 1→2"; plan G2 stop column "**No 1→2 may be enabled before this exit passes.**"; spec.md:89 "missing proof blocks plan G2 (M-1), never substitutes the zero-pressure control". | **Yes** |
| Testable | Each control has a setup, a clean-A/B equality list and a negative control (the mutant). Source-dependent inputs are deferred to B-01's reviewed manifest (requirements.md §1; plan.md §3), which is honest and matches the spec's own PR-06/ND-01/04/05 handling. | **Yes** (see §4.7) |
| Reciprocal tracing | R-07 (requirements.md:34) cites `T-02/T-03/T-19/T-20` and "R-31/R-32"; R-31↔T-19, R-32↔T-20, both with `plan G2 (M-1)/G4; B-02/B-03` in T Maps; the parsed R↔T trace shows 0 asymmetries. | **Yes** |

Also confirmed: requirements.md:70 now reads "T-02, T-19, T-20 and T-01 gate M-1 before 1→2", and T-02's own row says "This zero-pressure vector cannot satisfy R-31/R-32" (requirements.md:76), so a reader cannot count the original discriminator as sufficient.

### 4.3 Why each mutant is caught

- **Dead-to-mean only.** The dead row's S⁴ enters the f64 ordinal mean. Every living p scales as 1/mean, so *mean(S⁴)* and *per-living p* differ from B. T-19 compares both bit-for-bit, so it fails even if no victim changes.
- **Dead-to-hazard/roll only.** The dead row becomes a candidate with S>0 and a roll key `ADULT-MORT-<ID>`. The ordered roll-key list and its 24-bit numerators gain an element that B does not have, and D could be re-killed. T-19 compares the ordered keys and "D receives no roll or event".
- **Dead-to-boundary.** The dead row enters the strict-RD>1 candidate ordering with a high would-be suppression, so it sorts early, appears in the victim set and shifts the running-RD sequence. T-20 compares candidate ordering, victim set, removal order and running RD.
- **Dead-as-neighbour, dead-as-stand-total** at positive pressure are covered by the all-stage comparisons in T-19 (CI, growth and every stand stage identical) as well as by T-02.

### 4.4 Both channels are expressible

A dead row can matter in two ways: as a *neighbour* it changes living CI and so S; as a *candidate* it adds its own S⁴ to the mean and takes a roll. A wrongly admitted dead tree surrounded by larger neighbours has a large S⁴ relative to living trees, so "high-DBH/high-suppression" in T-19 is satisfiable (§4.6, last table).

### 4.5 What T-19/T-20 are not

Both are explicitly labelled synthetic controlled vectors, "not the qualified 100-tree stand or actual Year2 observations", with no public untrusted-fixture admission path (spec.md:89). T-20's injected-survivor seam, if used, must be labelled and "cannot replace T-19's actual conditional-roll comparison". That is the right asymmetry: the actual-roll proof is not allowed to degrade to injection.

### 4.6 Physical reachability at the trusted 0.09 ha domain (my recomputation)

Method: area `f32(900/10000)` = `0.09000000357627869` (`0x3db851ec`, matches the frozen witness); `Nha = f32(N/area)` widened; `RD = Nha·(Dq/25)^2.063/1868` in binary64; `P = max(0,(RD−0.6)/0.4)`; DBH clamp 200 cm.

**Discriminator values the corrected text relies on (all reproduce):**

| Quantity | Quoted | Recomputed (binary32 emulation) |
|---|---|---|
| CI(T), living only | 0.3125 | `0.3125` (`0x3ea00000`) |
| CI(T), dead D wrongly included | 2.8125 | `2.8125` (`0x40340000`) |
| Requested gain (clean) | 0.9411764740943909 | `0.9411764740943909` |
| Requested gain (wrong) | 0.6399999856948853 | `0.6399999856948853` |
| CI(L) for the 25 cm tree, for attribution | not quoted | `0.20000000298023224` (`0x3e4ccccd`): the quoted 0.3125 is T's, as the corrected text now says |

**Density ceilings at 0.09 ha with every tree at the 200 cm clamp (R-03):**

| N | Nha | RD at Dq = 200 | RD > 1? |
|---|---|---|---|
| 1 | 11.1111 | **0.433966** | No |
| 2 | 22.2222 | **0.867931** | No |
| 3 | 33.3333 | 1.301897 | Yes |
| 4 | 44.4444 | 1.735862 | Yes |

RD = 1 needs Dq ≈ **299.755 cm** for a singleton (beyond the clamp) and ≈ 214.214 cm for N = 2 (also beyond it). The smallest N with RD>1 at the clamp is 3; the smallest N with P>0 is 2. So spec.md:172's conclusion ("the trusted 30 m singleton and two-survivor states cannot reach RD>1") reproduces, and its evidence tag `[P/U]` pending G1 is the right one.

**`P>0` and `RD>1` are reachable for the controls (illustrative synthetic stands, not spec values):**

| Stand | RD | P |
|---|---|---|
| Published trusted post-growth RD `0.6375836886879149` | — | `0.09395922171978738` (reproduces #725/#729): positive pressure already occurs in the real fixture |
| N = 8, Dq = 90 cm | 0.6685 | 0.1713 (P>0, RD<1: T-19 shape) |
| N = 10, Dq = 100 cm | 1.0386 | 1.0964 |
| N = 12, Dq = 100 cm | 1.2463 | 1.6157 |
| N = 20, Dq = 100 cm | 2.0771 | 3.6928 (RD stays >1 after realistic hazard losses: T-20 shape) |
| N = 100 (trusted size) | needs Dq > 32.16 cm for RD>1 | — |

**Illustration that a wrongly admitted dead row is observable at P>0 (mine, not a spec value, not an oracle, no roll numerators):** 8 living trees on a 6 m grid inside the 30 m square (DBH 70–110 cm, Dq 89.0 cm → RD 0.6534, P 0.1334), plus a dead 120 cm stem at (12 m, 6 m). Living computed p lies in 1.7e-4…4.3e-3, so `0<p<0.5` holds. Admitting the dead row to the mean alone moves mean(S⁴) from 5.533e-5 to 7.290e-5 (×1.318) and lowers every living p by about 24%. As a candidate it would carry S = 0.121 (S⁴ = 2.13e-4, ≈3.9× the clean mean) and would take a roll key. So the controls can discriminate both channels at a physically admissible geometry. A real selected-victim discriminator additionally needs IDs whose native roll falls between the clean and wrong p; that is a G1 search over a pure function once ND-01 is resolved, which is exactly what T-19 defers.

### 4.7 Residual caveats (none keeps R-01 open)

1. **Not a claim that the current source leaks.** #729 §7 says the existing source "checks `IsLiving` in mortality" while `Calculate` does not filter. A clean T-19/T-20 on the repaired code is therefore the expected outcome. Their value is as regression guards for the new living-partition refactor, and the mutant requirement is what proves they discriminate. The spec words them as "wrongful admission *would* change", which is accurate.
2. **Observability seam.** T-19 compares mean(S⁴), per-living p and ordered roll keys. These need a private diagnostics/roll-call seam. The spec routes this through B-01's reviewed command/input manifest and "Internal synthetic seams require a reviewed lawful scope" (plan.md:36). Whether PR-04's private diagnostics decision is needed before plan G2 (M-1) rather than before G3 is for the Manager at G1; I do not call it a defect.
3. **ND-01 dependency is honest.** If the native RNG key packing cannot be exercised at G1, T-19 cannot be satisfied and, by the spec's own text, plan G2 (M-1) stays blocked. That is conservative.

## 5. What this verdict does and does not approve

It approves, at documentation level, that `a2509922` answers the prior review's findings. It is **not** the G0 approval defined at spec.md:350 / plan.md:21 (explicit product-owner approval of the exact four files plus Manager APPROVED naming candidate and approvers), and it is not source access, a lease, a custodian grant, a security acceptance, an implementation or any test result. Default L4 FAIL38 stays unwaived and Manager G2 (protected-source/security admission) stays SOURCE HELD, exactly as the spec states.

## 6. No-regression results (brief item 8)

| Check | Result |
|---|---|
| Year1 stdout witness | `574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`, 45,980 B: present, count per file identical to the old commit (spec 1, requirements 2). Also equals S-01. |
| Fixture witness | `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, 32,567 B: identical counts (spec 1, requirements 2). Also equals S-01. |
| Area bits / refusals | `3db851ec` (spec 1, requirements 2), `AnnualAlreadyAdvanced` (spec 2, requirements 2), `OriginalAnnualRefused` (spec 2, requirements 2): counts identical to old. §2.2 frozen-witness table is **byte-identical** old vs new. Area bits independently recomputed (§4.6). I did not re-hash anything. |
| D-1…D-4 | The four `[A]` rows are **byte-identical** to `d468e989`. Only the second column header changed (`Consequence` → `[P] Derived consequence (not an additional owner decision)`, R-04). |
| PR/ND register | PR-01…05, PR-07, PR-08 and ND-01…ND-09 rows byte-identical. PR-06 differs only by the label `plan G2 (M-1)`. |
| Draft header | Line 1 of all four files unchanged: "DRAFT — PROPOSED SPECIFICATION, NOT APPROVED, NOT IMPLEMENTED, NOT TESTED." |
| Checkboxes | `requirements.md`: **`[ ]` = 32, `[x]` = 0** (30 + the new R-31 and R-32). `spec.md`, `plan.md`, `tasks.md`: no checkbox tokens. |
| Test/requirement counts | T-00…T-20 (21 rows, all "NOT RUN"), R-01…R-32; no stale "T-18"/"30" counters; no old register range "S-01…S-11" remains. |
| No new numbers | No decimal of three or more places added or dropped in any of the four files. The only new numerals are the labels T-19, T-20, R-31, R-32, S-12, S-13, and the thresholds `P>0`, `0<p<0.5`, `RD>1`, which come from §5.2's own formulas/cap. |
| No new scope | No Year2–10 golden, no roll numerator or victim, no API/error literal (§6.1 table unchanged and still "PROPOSED"), no coefficient or calibration, no persistence/Host/viewer/Unity scope. S-13 is used only to *exclude* the viewer decision from this engine. |
| Statements that something ran | None. Every new control is "NOT RUN", "mandatory before", or "must". |
| Added text ↔ findings | Every hunk maps to R-01…R-14: register pins and S-12/S-13 (R-02, R-05, R-07), caveats (R-06), tag change (R-04), construction and T-19/T-20/R-31/R-32 (R-01, R-02), divergence table (R-14), §5.1/§5.3 (R-03, R-13), G2 qualifiers everywhere (R-08), approver set (R-09), reciprocal Maps edits (R-10), §9 sentence (R-11), plan G3 vs B-04 (R-12). I found no change attributable to nothing. |

## 7. New defects (bounded scan of changed lines)

No BLOCKING, MAJOR or MINOR. Optional NITs:

| ID | Sev | Location | Observation | Suggested wording |
|---|---|---|---|---|
| N-1 | NIT | spec.md:172 | "the reviewer derives" the trusted-domain RD>1 unreachability but carries neither the figures nor a pointer, so a G1 reader must re-derive. §10 links the review but §4.3 of that review is not named. | Add "(prior review §4.3: max RD 0.434 at N=1 and 0.868 at N=2 at the 200 cm clamp; singleton RD=1 needs Dq≈299.8 cm)". |
| N-2 | NIT | spec.md:87; requirements.md:94 | T-19 says "bit-for-bit"; T-20 says only "identical" for the running-RD sequence. Read with the checklist's bit-comparison convention they mean the same, but the asymmetry invites a tolerance. | Say "bit-identical" in T-20. |
| N-3 | NIT | spec.md:37, :206 | S-12 is labelled "dated" but no date is written (observation 18 Nov 2026 ~22:03Z; OAuth expiry 13 Nov). Recoverable from the pinned commit. | Add "observed 18 November 2026". |
| N-4 | NIT | spec.md:210–212 | The §7 gate list is tagged plain `[A]`, but its first bullet (the combined G0 approver set) extends S-07 R-8 to this successor, whereas S-01 Stage A names only a Manager verdict. §10 tags it `[A/P]`. Harmless (it only adds approvals), but tag drift. | Tag the bullet `[A/P]` or cite S-07 R-8 inline. |

Cosmetic, not counted: R-31/R-32 are placed between R-15 and R-16 (requirements.md:44–45) to avoid renumbering; they are cross-referenced by number everywhere, so nothing is lost.

## 8. What I still cannot verify

- **No source, no fixture, no build, no .NET, no Year2 run.** I did not read, search or open any protected local tree or any path containing `725`, `710`, `687` or `MultiRackSitkaP1Lab`. All of T-00…T-20 remain NOT RUN; nothing here shows 1→2 works, M-1 is repaired, or any roll numerator or victim set exists.
- **Whether the current source really excludes dead rows in mortality** (#729 says it checks `IsLiving` there; I did not see the code). That is why T-19/T-20 must be judged by their mutants, not by a clean pass.
- **Whether T-19/T-20 inputs can actually be built**: that needs the native RNG key packing (ND-01), the evaluation-site map (ND-04/PR-06) and a diagnostics seam. All are G1 deliverables. My §4.6 toy is binary32 for CI and binary64 elsewhere, an illustration of plausibility, not an oracle.
- **The `0.434` / `0.868` / `299.8` figures use published formulas and constants**; the 100 protected DBHs were not read, so Dq of the real stand is taken from #725/#729 as published.
- **#725 (S-08) content** was not read (existence at the pin only). I did not recompute either published SHA-256 or any byte count; those are string comparisons with S-01.
- **GitHub issue/comment threads, Dashboard/Drive sync, and any Manager or product-owner approval state** are not verified.
- **Platform facts**: Python `math.pow` here is the host libm; that it reproduced the published RD bits supports but does not prove cross-runtime equality. No Unity/Mono/Windows parity is claimed.
- **Unchanged sections** were not re-audited beyond checks that they are byte-identical or untouched by a change (per the brief).
- **Approval boundary**: this APPROVED covers the corrections at documentation level only (§5).

Status: HANDED-OFF TO PARENT/Manager #730 — DELTA RECHECK, SPEC VERDICT ONLY.
