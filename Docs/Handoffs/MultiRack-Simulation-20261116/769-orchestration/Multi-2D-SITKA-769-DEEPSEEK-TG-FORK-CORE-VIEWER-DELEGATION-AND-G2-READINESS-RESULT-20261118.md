# Multi #769 — DeepSeek TG Fork Core+Viewer delegation and consolidated G2 readiness result

**18 November 2026 · [Multi #769](https://github.com/jermur788/CCF/issues/769) · Manager [#730](https://github.com/jermur788/CCF/issues/730) · overall [#425](https://github.com/jermur788/CCF/issues/425)**

**Status: PHASE A COMPLETE (docs-only orchestration). G2 SOURCE STILL HELD.** This is the single consolidated result for the corrected Multi #769 trial. It reports what was actually done, by which real models, what was produced, and the one actionable future G2 grant package with its remaining unsigned conditions. It is **not** an implementation, a test run, a browser qualification, a security PASS, or a self-approval.

**Routing note.** The earlier `Sen-1-769-…COMPACT30…` packet is **SUPERSEDED** and was not dispatched. #769 is Multi standalone 2D Sitka H0-A under Manager #730. No #731 adoption was required or sought for this work. Separate Sen-1 Unity #770 remains under #731 and was untouched.

---

## 1. Actual registration

| Field | Verified value |
| --- | --- |
| Actor / role | DeepSeek native orchestrator (parent), Phase A docs-only; no source/file ownership for children |
| Model | `opencode-go/deepseek-v4.1-flash`, variant **high** |
| Provider | `opencode` (OpenCode provider instance) |
| Harness / host | T3 Code through the OpenCode Code Mode harness, native Linux, `jer-laptop`; TG Fork orchestration surface (`orchestrator_capabilities`, `delegate_task`) |
| Native session | T3 parent thread `3325d55e-b948-4569-b8db-4ea574220d57` |
| Fresh GitHub `main` | `f6e52be18399637ec93a20a53b937b7bd097b39e`, via `git fetch origin main` |
| Parent docs branch | `docs/multi-769-deepseek-tg-fork-core-viewer-orchestration-20261118-v1` (worktree `/tmp/opencode/ccf-769`) |
| Registered `[RUNNING]` | [comment 6988709937](https://github.com/jermur788/CCF/issues/769#issuecomment-6988709937), 19:08:19Z, before any edit |
| Child claims registered | [comment 6988752650](https://github.com/jermur788/CCF/issues/769#issuecomment-6988752650), 19:10:04Z, before child authoring |
| Forbidden models | GPT-6.2 Sol/Astra never selected, escalated, diagnosed or used as fallback |

---

## 2. Genuine TG Fork delegation

Two disjoint substantive authoring tasks were dispatched through the native TG Fork `delegate_task` surface, each on its own model, branch and sole path. Children authored Markdown only and returned it to the parent; the parent committed each document to the child's branch. No child held repo write, source, fixture, protected-read or approval authority.

| Slice | Actual child model | Provider | Child thread | Branch @ commit | Sole path |
| --- | --- | --- | --- | --- | --- |
| **A1 Core design + static acceptance/test vectors** | **GPT-6.1 Sol** (`gpt-6.1-sol`, effort **high**) | `codex_c75d8114-7118-4224-8bf2-b9b62637e1fb` | `thread:delegated-task:command%3Amcp%3A168e4aa7…%3Adelegate-task%3Ac3727c1c-7201-4f29-a634-0e141decd40c` | `docs/multi-769-a1-core-design-dossier-20261118-v1` @ `4bae503cc03e16818c04f4bddb1f64f1147db3f3` | `769-orchestration/Multi-2D-SITKA-769-A1-CORE-DESIGN-AND-TEST-VECTOR-DOSSIER-20261118.md` |
| **A2 Viewer design + accessibility/browser acceptance** | **Claude Sonnet 5.5** (`claude-sonnet-5-5`) | `claudeAgent` | `thread:delegated-task:command%3Amcp%3A168e4aa7…%3Adelegate-task%3A3ac71cc2-2ac2-4391-b1e0-e7326a088448` | `docs/multi-769-a2-viewer-design-dossier-20261118-v1` @ `b571cf0e209527be5bf189d7cc98c92dc445c84c` | `769-orchestration/Multi-2D-SITKA-769-A2-VIEWER-DESIGN-AND-ACCESSIBILITY-DOSSIER-20261118.md` |

Both branches were created from the same fresh main `f6e52be1`. No path, branch or file overlaps between A1 and A2. Neither child is a reviewer or approver of the other.

---

## 3. Child handoff summaries

### A1 — Core (GPT-6.1 Sol)
[Full dossier](https://github.com/jermur788/CCF/blob/4bae503cc03e16818c04f4bddb1f64f1147db3f3/Docs/Handoffs/MultiRack-Simulation-20261116/769-orchestration/Multi-2D-SITKA-769-A1-CORE-DESIGN-AND-TEST-VECTOR-DOSSIER-20261118.md)

- Maps the adopted `ccf.h0a.core-read` v1.0.0 to the five allowed Core paths and their exact responsibilities, with forbidden contents per file.
- Specifies the private fixed trust root (32,567 B / `bcaaa5c3…757c4`), digest-before-parse, single frozen global `CcfYear0Core`, and the full S0→S12 admission order with closed refusal codes.
- Provides a bit-level codec dossier (f32 decode, forward X/Z via `Math.fround`, `ConversionBitMismatch`, no crown recompute) and FIPS 180-4 SHA-256 KATs.
- Provides a 22-row static test-vector dossier (**C-01…C-22**) labelled PROD vs CORE-TEST, each with exact mutation, expected stage/code, and an explicit “does not prove” column.
- Defines packaging trust-negatives (positive allowlist, emitted-byte inspection, deterministic assembly, manifest fields) and F1/F3/roster acceptance.
- Ends with a ready-to-execute brief whose branch/base are deliberately **unsupplied** — a STOP, not a grant.

### A2 — Viewer (Claude Sonnet 5.5)
[Full dossier](https://github.com/jermur788/CCF/blob/b571cf0e209527be5bf189d7cc98c92dc445c84c/Docs/Handoffs/MultiRack-Simulation-20261116/769-orchestration/Multi-2D-SITKA-769-A2-VIEWER-DESIGN-AND-ACCESSIBILITY-DOSSIER-20261118.md)

- Maps the Viewer allowlist (`shell.template.html`, `viewer.css`, ≤3 `viewer-*.js`, `tests/viewer/**`) with per-module responsibility and prohibited content.
- Defines the data flow: strict version gate → `openGenesis()` → bounded refusal or admitted render; immutable public-schema doubles only.
- Specifies Canvas projection, dpr backing store, fit/clamp/zoom math, crown **radius** drawing, hit-testing with ordinal tie-break, chooser, keyboard listbox inspector with exact-key display, and stale/foreign rejection via `isCurrentSelection`.
- Provides an accessibility matrix (keyboard path, ARIA, colour-independent cues, live announcements, contrast, reduced motion) and 14 proposed browser/offline checks (**V-01…V-14**) including CSP enforcement, zero extra requests, all-100-ID keyboard reachability, and no global leaks.
- Ends with a ready-to-execute brief whose branch/base are custodian-supplied and explicitly preconditioned on Core freeze **Cf**.

---

## 4. Consolidated future G2 grant package (ONE actionable request — unsigned)

This integrates Core C, Viewer A, nonwriter baseline B, independent reviewer Q and orchestrator D. **No grant below is issued by this document.** Names, branches and SHAs are the pre-agreed shape; actual actor identity, confinement evidence and signatures are still required.

Physical root: `R = /media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/` (isolated, remote-less, per #755; both lanes already cone-sparse for `Tools/MultiRackSitkaYear0Viewer`). Base for Core work: `34185b8b71ba280c89091290345e537ef107bdf4`, **not** this docs branch's main.

| Grant | Actor (proposed) | Lane / branch | Exact allowlist / scope | Still missing before it binds |
| --- | --- | --- | --- | --- |
| **D Orchestration** | This DeepSeek session (registered) | none (docs only) | TG Fork routing, child claims, evidence | Manager execution claim accepting this registration; no source rights |
| **C Core WRITE + fixture READ** | One actual **GPT-6.1 Sol** or **Claude** worker, newly registered | `R/ccf-h0a-core` · `feature/multi-h0a-core-year0-admission-20261118-v1` | `core/year0-core.js`, `core/CONTRACT.md`, `fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json`, `build/assemble-year0-viewer.py`, `tests/core/**` (test-only, excluded from production) | Signed Manager C grant; fresh identity/host/effort; lane claim; accepted pre-baseline; **verified effective confinement**; capacity check |
| **C fixture READ (separate)** | Same C actor only | canonical store `/media/jer/Files/CCF/Repository.git` | Exactly blob `8bf81a4364a5a6bacf104d72d354e74635d3b26c` (`Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` at public #639 tip `e302f15b`); assert 32,567 B and SHA-256 `bcaaa5c3…757c4`; one byte-exact 0444 destination copy; STOP on mismatch | Explicit actor-specific fixture-read grant; no P0/#710/#725 fallback |
| **A Viewer WRITE** | A **different** actual Claude or GPT-6.1 Sol worker | `R/ccf-h0a-viewer` · `feature/multi-h0a-a-year0-viewer-20261118-v1`, started only after custodian fast-forward to Core freeze **Cf** | `viewer/shell.template.html`, `viewer/viewer.css`, ≤3 `viewer/viewer-*.js`, `tests/viewer/**` | Signed Manager A grant after reviewed Cf; custody fast-forward; fresh identity/confinement; no concurrent Core edit |
| **B nonwriter baseline (pre/post)** | One actual native custodian, **not** C or A | read-only; receipts under `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/d5/{pre,post}/` | Exactly one file: `/media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/ccf-687-child-a/Tools/MultiRackSitkaLab/src/Core/Engine.cs`, no-follow, SHA-256(bytes) pre once / post once; plus B-1/B-2/B-5 metadata | Manager + owner/security byte-read grant naming B; **#687 holder consent or explicit owner override**; pinned hashing method; owner one-file **partial** coverage acceptance (policy recorded, custodian grant not) |
| **Q independent qualification** | A **different** actual Claude or GPT-6.1 Sol reviewer, nonauthor | exclusive sequential reuse of Core lane after C releases it; output in separately named artifact dir | Read exact source/package, isolated nonauthor rebuild, offline Linux Firefox + Chromium checks, V-01…V-14 / C-01…C-22 evidence review | Fresh Q claim/identity; exact candidate and package SHA; explicit browser/offline isolation permission; must not become an author |

**Missing conditions, stated plainly (not fabricated):**
1. **Effective writer confinement is unproven.** The observed child/parent sessions are `full-access`. Git hooks are bypassable. No OS/tool sandbox has been demonstrated for C or A.
2. **The #687 one-file read is not yet granted.** Owner approved the *policy/override*; no specific custodian grant exists, and the holder's live consent is missing.
3. **No protected byte baseline has been taken.** D5 is BLOCKED until the above signatures and a pinned hashing method exist.
4. **No physical Ops reservation** for C/A writers has been staged.
5. **`.NET L4 FAIL38 remains UNWAIVED.**
6. **No source, test, package or browser evidence exists** for H0-A; child documents are design only.

---

## 5. Decision needed (one request, no new review cascade)

Manager #730 (with owner/security where indicated) should:

1. Accept or amend this single G2 grant package and the two child dossiers as the design baseline.
2. Name the actual C, A, B, Q actors/sessions and their capacity; issue **separate** signed C-then-A grants (A after reviewed Cf), plus the distinct B byte-read grant and the C fixture-read grant.
3. Accept the owner-recorded one-file **partial** #687 coverage (already policy-approved) and confirm the pinned hashing method.
4. Require demonstrated effective confinement before admitting C/A, or record an explicit accepted residual risk.
5. Keep D5 BLOCKED and L4 FAIL38 unwaived until then.

No contract redesign, product reapproval or general security re-approval is needed.

---

## 6. Measured orchestration evidence (observed, not billed)

Wall-clock times from GitHub comment and Git commit timestamps on `jer-laptop`; these are observed latency, not provider token/cost/quota claims.

| Event | UTC |
| --- | --- |
| Parent `[RUNNING]` registered | 19:08:19 |
| Child claims registered; both children dispatched together | 19:10:04 |
| A2 (Claude) dossier committed | 19:12:58 |
| A1 (GPT-6.1 Sol) dossier committed | 19:14:15 |
| Parent result authored | 19:1x |

- **Delegation pattern:** two children dispatched in one parallel batch (`Promise.allSettled`), no serial re-dispatch.
- **Retries / interrupts / fallbacks:** 0 / 0 / 0.
- **Model substitutions:** none; no GPT-6.2 selection or fallback at any point.
- **Observed authoring latency:** A2 ≈ 2 min 54 s, A1 ≈ 4 min 11 s after dispatch; both produced ~2,000+ word substantive documents, i.e. real work rather than a smoke test.
- **Overhead vs. manual:** roughly 2 minutes of parent time to register, branch, dispatch, commit three docs and prepare the grant package; the substantive content came from the two children, not the parent.
- **Quota/capacity:** exact remaining provider quota/reset is **UNKNOWN** (not exposed); no invented metrics.

---

## 7. What was NOT done (explicit)

- No application/source code, HTML/JS, fixture copy or import, assembler, tests, builds, Node/.NET, Unity, Windows, Host, save, network or browser run.
- No protected P0 / #710 / #725 access, copy or hash; no #687 byte read.
- No Ops mutation, no `active-tasks.md` edit, no `main` edit, no public production source.
- No edits to the four-file approved SPEC, #758 contract, #760 or #761 documents.
- No L4 waiver; no integration, install or release claim.
- No self-approval; Manager #730 verdict remains separate.

---

## 8. Readback

- Files added: exactly **three** Markdown documents — A1 on `docs/multi-769-a1-…`, A2 on `docs/multi-769-a2-…`, and this parent result on `docs/multi-769-deepseek-tg-fork-core-viewer-orchestration-20261118-v1`. No other path, source, coordination or config change on any of the three branches.
- Each branch was created from main `f6e52be1` and contains only its own single added Markdown.
- Dashboard/Drive synchronization: **NOT VERIFIED** (not attempted).

---

**[HANDED-OFF] #769 → #730. G2 SOURCE HELD pending the signed actor-specific C/A/B/Q grants and demonstrated confinement above.** Phase A is complete, substantive and non-fabricated; the next executable step is the Manager's signed grant package, not more preparation.
