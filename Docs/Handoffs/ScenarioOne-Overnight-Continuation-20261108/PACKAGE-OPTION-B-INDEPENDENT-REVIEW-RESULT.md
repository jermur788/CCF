# #336 — Independent source-only review: Option B Linux SDK 1.1.1 package provenance proposal

**Date:** 8 November 2026
**Reviewer lane:** `docs/linux-sdk-111-independent-review-20261108-v1` in worktree
`/media/jer/Files/CCF/Operations/CCF-PackageOptionBReview-20261108` (clean; starting main HEAD
`8a751791ef0523102a1516bee3cd18f5e1cfd867` = `origin/main` at review admission — both verified locally).
**Evidence:** `/media/jer/Files/CCF/Verification/CCF-PackageOptionBIndependent-20261108/`
(README.md indexes every raw command/output; SHA256SUMS.txt covers 49 files, all verified OK).

## Status — what this is / what this is not

**This is** an independent, offline, source-only provenance review of the Manager-approved #335
package PROPOSAL that aligns `com.unity.sdk.linux-arm64`, `com.unity.sdk.linux-x86_64`,
`com.unity.toolchain.linux-x86_64-linux` and transitive `com.unity.sysroot.base` from 1.1.0 to 1.1.1
in `Packages/manifest.json` + `Packages/packages-lock.json`, reproducing bytes observed in the
Editor-written #287 dirty lane state.

**This is NOT:** adoption of 1.1.1 as canonical package policy; Unity qualification; an Editor import
or package resolve; a release; permission to touch any protected lane; a source fix (none was made);
or a package-policy approval. The verdict below is a recommendation to Manager #200 only.

## Reviewer identity (disclosure)

**Reviewer (actual): MiMo-V2.6-Pro (`xiaomi-token-plan-ams/mimo-v2.6-pro`), Xiaomi, via a
fresh-context T3 Code review subagent; effort: UNKNOWN** (this launch path exposes no effort knob;
platform default). I am a **DISCLOSED SUBSTITUTE** under the binding handoff's clause "If
inaccessible, a comparable genuinely non-Anthropic independent local code reviewer may substitute
with model/provider/effort disclosed": the handoff-preferred OpenAI GPT-6.1 Sol High proved
inaccessible with **eight** recorded failed launch attempts — two ChatGPT/Codex subscription-quota
failures, one OpenCode-router usage-limit failure on `openai/gpt-6.1-sol`, three OpenCode
console-connection failures on delegate launches, one OpenCode console-connection failure on an
`opencode/mistral-large-4` subagent launch, and one usage-limit failure on an
`openai/gpt-6.1-sol` subagent launch. None of those attempts was counted as review work, per the
rule "do not count an exhausted/quota-blocked attempt as completed". Transparent limitation:
orchestration/publication/GitHub claims are performed by the MiMo-V2.6-Pro parent T3 Code session,
while **all review checks and the verdict in this document are mine**, produced in this separate
fresh-context session; the reviewer is a separate session but of the same model family as the
orchestrator, and the Manager may weigh that residual limitation. The independence that matters for
this task is intact: I am **non-Anthropic and completely separate from the #335 author (Anthropic
Claude Opus 5.5)**; I hold no author/write authority on #335 and made no source change.

## Exact identities reviewed (each re-derived locally from git/file evidence)

| Dimension | Identity | How verified |
|---|---|---|
| Base (source-approved Workbench published head) | `03b484d10b36382ab86d5e345ece9acac96c9620` | `git rev-parse`, `git cat-file -t`, log (`raw/08`) |
| Tested package source | `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` | parent chain: exact child of 03b484d1 (`raw/08`) |
| Published/report head | `d39278ef10001f8536b598fef2a7d68864096c92` | parent chain: exact child of 46bc1fd6 (`raw/08`) |
| Candidate branch | `proposal/linux-sdk-111-provenance-20261108-v1` | named in author report; commits above reachable from the bare store |
| Reviewer branch / worktree / start HEAD | `docs/linux-sdk-111-independent-review-20261108-v1`, `/media/jer/Files/CCF/Operations/CCF-PackageOptionBReview-20261108`, `8a751791ef0523102a1516bee3cd18f5e1cfd867` | `git status`/`rev-parse`/`branch --show-current`, `origin/main` = `8a751791` (`raw/09`, `raw/11`, `raw/20`) |
| Canonical bare Git store | `/media/jer/Files/CCF/Repository.git` | all reads against it or the reviewer clone's alternates |

Ancestry: `03b484d1` is an ancestor of `46bc1fd6`, which is an ancestor of `d39278ef`
(`raw/11`). The chain is strictly linear with those exact parents (`raw/08`).

---

## Per-check findings

### Check 1 — two-file source delta, report-only publication, exact 10-value enumeration — **CONFIRMED**

- `git diff --name-status 03b484d1 46bc1fd6` = exactly `M Packages/manifest.json`,
  `M Packages/packages-lock.json`. `git diff --name-status 46bc1fd6 d39278ef` = exactly
  `A …/PACKAGE-OPTION-B-SOURCE-PROPOSAL-RESULT.md` (report-only publication, 203 added lines).
  No `Assets/`, `Tools/Verification/`, `ProjectSettings/`, `.vscode/`, material, save or other path.
  (`raw/06`, `raw/07`, `raw/01`–`raw/03`, `raw/04-diff-base-to-source-full.patch`,
  `raw/05-diff-source-to-pub-full.patch`)
- **My own independent JSON leaf-diff enumeration** (reviewer-owned script
  `scripts/reviewer_json_delta_enum.py`, output `analysis/reviewer-independent-json-delta-enum.json`)
  enumerates **exactly 10 changed values**, matching the expected shape precisely:
  - manifest (3): `com.unity.sdk.linux-arm64`, `com.unity.sdk.linux-x86_64`,
    `com.unity.toolchain.linux-x86_64-linux` versions 1.1.0 → 1.1.1;
  - lock (7): the same 3 direct `version` values, their **3** `com.unity.sysroot.base` dependency
    refs (1.1.0 → 1.1.1), and the transitive `com.unity.sysroot.base` `version` (depth 1),
    1.1.0 → 1.1.1.
  - Every changed value is 1.1.0 → 1.1.1; **no** added/removed key anywhere; **no** other value moved.
- Lock graph: **65 → 65 entries**, identical name sets, depths 0–3, sources `{builtin: 50,
  registry: 14, git: 1}`, all `depth`/`source`/`url` fields unchanged, registry URL
  `https://packages.unity.com` unchanged on all 14 registry entries, and the one git entry
  `com.kitwright.unity.mcp` keeps pin/hash
  `585ada203ff25b1fbea23821e159afa4a930d481` unchanged. (`analysis/reviewer-lock-entry-summary.txt`)
- **Conclusion: I independently confirm the 3 + 7 = 10-value shape, 65 lock entries, and zero
  extra package dependency drift or unlisted modified paths.** The GitHub-side parse quoted in the
  Manager handoff is corroborated.

### Check 2 — byte identity to UPM-written dirty backups, SHA256SUMS, tracked HEAD versions, two 65-package compile.log blocks — **CONFIRMED**

(All read from the Files-side #318 preserved copies; the registered ZX20 lane was **not** opened.)

- Proposed source bytes == preserved UPM-written dirty backups, byte-for-byte (`cmp`, `raw/12`):
  - `manifest.json`: sha256 `f2ff5f95f9e93716947391a8818dbe642d81f0b49068516441a58def966c2c1c`
    (dirty-backup == 46bc1fd6 blob);
  - `packages-lock.json`: sha256 `f43d9894af3c6dc054e86e98335dbd248a9fb97527b49c7b5952d96e037e52b1`
    (dirty-backup == 46bc1fd6 blob).
- `Workbench287MorningClosure-20261107/SHA256SUMS.txt` (sha256
  `9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74`, matching the author's cited
  value) verifies **16/16 OK** including all eight protected dirty files (`raw/13`).
- Original tracked HEAD versions: `head-versions/Packages/*` == base-commit blobs, sha256
  `c533b915…00215` / `013b9f99…0d9f` (also the blobs at `4d95e452`, `a4019e6b`, `03b484d1`
  per `raw/18`) — i.e. the input UPM rewrote from is exactly this base.
- Archived #287 `Workbench287Tier2-20261107/compile.log` sha256
  `a70cf5485f4645427e214e8c4df1f73d30c4c24e66ee949336578a7181d966b1` matches the cited
  `a70cf548…66b1`. My own parser (`scripts/reviewer_compile_log_block_parse.py`,
  `analysis/reviewer-compile-log-block-compare.json`) finds **exactly two** `Registered 65 packages`
  blocks:
  - **Block 1 (41.28 s resolve): 65 entries whose name@version set equals the base lock exactly**
    (old input; the four Linux packages at 1.1.0, PackageCache dirs `@df9ffedfa568`, `@6d12be270055`,
    `@d0308bf9d559`, `@3559fe1aa613`);
  - **Block 2 (40.56 s, followed by `Lock file was modified`): 65 entries equal to the proposed
    lock exactly** (the four Linux packages at 1.1.1, cache dirs `@5043731cb64f`, `@920bc08129cd`,
    `@95ffda8fe8d9`, `@0a249da1e1ab`);
  - the two blocks differ **only** in those four packages (versions and their PackageCache hash
    dirs); the other 61 entries and the git pin `585ada203ff2` are unchanged in both blocks.
- **Conclusion: the proposed files are exactly the preserved Editor-written output for exactly this
  input; first block = old input, second block = proposed output; only the four Linux package
  versions change.** Residual chain-of-custody limit: the act of capturing the dirty bytes in #318
  cannot be re-witnessed without the protected ZX20 lane (forbidden here); it is accepted as
  SHA256SUMS-anchored preserved evidence with mtimes 00:49 inside the #287 import window, consistent
  with #333. No inconsistency found.

### Check 3 — validator/proof audit and reproduction (31/31, red control, mutations) — **CONFIRMED**

I audited both author scripts before trusting them (`raw/14` confirms my copies are byte-identical to
the author originals; author originals untouched):

- `validate_proposal.py`: 21 named checks expanding to 31 executions in commit mode (25 in WORKTREE
  mode). Core anchors: C01/C02/C05 pin the backup manifest and compile.log by sha256; C03 base bytes
  == preserved lane HEAD bytes; C04 candidate bytes == preserved Editor-written bytes; C06/C07 exact
  3+7 key diffs; C09 byte/line shape; C10 git pin; C12/C13 lock structural consistency; C14–C17 the
  two log blocks; C18–C21 scope guards (changed paths ⊆ 3 allowed; `Tools/Verification`, `Assets`,
  `ProjectSettings`, `.vscode` tree ids; file modes). No resolution, no network.
- `mutation_controls.py`: **corrected** kill criterion `killed = rc == 1 and len(fails) > 0`, with
  exceptions recorded separately and **not** counted as kills. This is the corrected run's logic; the
  superseded exception-counting run is not present in the evidence tree and I did not rely on it.

**Reproduction table** (reviewer-owned disposable clone `reviewer-clone/`, detached at `46bc1fd6`,
`git clone --shared` with object alternates — read-only):

| Run | Target | Expected | Reviewer result | Exit | Genuine FAIL vs exception |
|---|---|---|---|---|---|
| R1 committed validation | `46bc1fd6` vs `03b484d1` | 31/31 PASS | **31 PASS / 0 FAIL** | 0 | n/a |
| R2 red negative control | base vs base (no change) | FAILs by design | **26 PASS / 5 FAIL** (C04×2, C06, C07, C16) | 1 | genuine FAIL verdicts |
| R3 mutation controls | 7 in-memory mutants | 7/7 killed | **7/7 KILLED** (2–4 FAILs each) | 0 | all rc==1 + FAIL lines; **zero exceptions** |

(`raw/R1-committed-validation.*`, `raw/R2-negative-control*`, `raw/R3-mutation-controls.*`,
`raw/R3-kill-quality-audit.txt`.) Every mutant's kill is a genuine checker FAIL verdict
(`rc == 1`, FAIL lines present, `exception=False` for all seven — `raw/R3-kill-quality-audit.txt`).
The R2 red control fails exactly the expected five checks (byte-identity to Editor bytes, both key-diff
checks, and the block-2 equality), which is the correct behaviour for an unchanged source baseline.

**Weaknesses/noted limitations of the 31-check suite** (none blocking; all checks reproduced):
- C04 (byte-identity to the preserved Editor-written files) is the dominant discriminator — every
  mutant trips it. However, **every mutant also trips at least one independent semantic check**
  (C06/C07/C09/C12/C13), so no kill rests on byte-identity alone.
- C12/C13 classify transitive requested-vs-locked version mismatches as *notes*, not problems
  (they are caught by C07/C13 in the mutants exercised). Deliberately lenient but safe in practice here.
- C06/C07 hard-code the expected 1.1.0→1.1.1 shape; correct for this proposal, not general.
- **No offline check can verify that 1.1.1 actually exists in the registry** or that its package
  content is sane — inherent to the no-network constraint and never claimed.
- No check was unreproducible; none looked tautological in a way that would admit a wrong lock.

### Check 4 — Workbench verification byte identity, R1 and R324-01 safeguards — **CONFIRMED, no relaxation**

- Whole `Tools/Verification` tree id `d6e960ca3b5fb8dabe51fcd92f7b27bbe1854636` **identical** at
  base and source (and at `a4019e6b`); `Assets`, `ProjectSettings`, `.vscode`, `Docs` trees all
  identical (`raw/15`, `raw/18`). Nothing outside the two package files + report moved.
- `WorkbenchRetirement/` blob ids identical base↔source (`raw/16`):
  `run_workbench_retirement.py` `9352503a…`, `test_runner.py` `c00b43b3…`,
  `WorkbenchRetirementVerification.cs` `511c2223…` (sha256s `d7df6401…6d1a`, `1b683522…b9`,
  `f6bf3e6d…87c`, matching the author's cited values — `raw/18`).
- **R1 fail-closed post-Editor provenance present and unchanged**: `test_R1_P7_postrun_manifest_and_lock_rewrite_fails`
  (mock Editor rewriting manifest/lock after clean admission must produce `postrun_lane_drift:2`
  failure with per-path hashes) and `test_R1_postrun_status_error_fails` (status errors fail closed,
  not ignored) — `raw/16`.
- **R324-01 PROVISIONAL/FINAL receipt safeguards present and unchanged**: six `test_R324*` controls
  including ENOSPC and real-permission failure cases where both JSON and fallback append must fail,
  write-failure stays provisional and exits nonzero, and authoritative success requires matching JSON
  **and** FINAL markers (`raw/16`).
- **No test or safeguard relaxation**: identical tree/blob ids prove it structurally.
- **The 41 runner mocks were counted (41 `def test_` functions), NOT re-executed here, and are not
  claimed to have run.** Prior #328/#329 execution evidence is referenced only as unchanged-byte
  history.

### Check 5 — over-claim audit and risk assessment

**Over-claim audit of the static checks and author report:** no static check implies Unity 1.1.1
import/resolve, Linux Player toolchain compatibility, determinism, package immutability or root
cause. The author report explicitly states "Nothing in this report is Unity-verified. Source-only
manifest/lock consistency is not Editor import validation", keeps the resolve trigger **UNPROVEN**,
labels toolchain-compatibility statements UNKNOWN/assumption, and marks #333 items as cited rather
than re-measured. One wording nuance to keep visible for the Manager: the sentence "the proposal is
exactly the Editor's own resolver output for this package input. It is not an inference." is sound as
*provenance of bytes* (given the preserved backups and log blocks) but should not be read as
qualification that a **future** Editor run will re-resolve to 1.1.1 — the second-resolve trigger
remains unknown, and the report itself says so. The R1 "inference" paragraph in the author report is
likewise correctly labelled as inference. No flaw.

**Risk inventory (carried forward for the Manager decision):**

1. **Another patch upgrade (R-1):** if the Editor trigger is "upgrade to newest available patch",
   committing 1.1.1 only holds until Unity publishes e.g. **1.1.2**, and the same rewrite/drift
   would return. Any future environment run must keep a STOP-on-drift rule for both package files.
2. **Registry/machine dependence (R-2):** if the trigger is network- or registry-state dependent,
   an offline or different machine could resolve differently (1.1.0 first, 1.1.1 second — or
   something else). The three other warm ZX20 Libraries (MainIntegration, U1, UIQualification) hold
   1.1.0, so environments already disagree until policy is applied uniformly.
3. **Linux sysroot/clang/linker impact (unknown):** these packages provide the Linux Player
   toolchain/sysroot (IL2CPP, native link, glibc surface, arm64 SDK). 1.1.0→1.1.1 can change Linux
   Player build output and behaviour. It is an **unverified assumption** that Editor-play simulation,
   saves and determinism are unaffected. No changelog was read (offline).
4. **Prior Unity results not inheritable:** every Unity result qualified under tracked 1.1.0 runs in
   a different package environment on toolchain grounds (MFM, Objective-1 native captures, S1B Linux
   Player builds, Reference-archive and Legacy40 save tests). #287's cold import ran at 1.1.1 after
   the rewrite but at old HEAD `4d95e452` with failed lifecycle gates. Nothing is qualified by this
   proposal.
5. **Historical ScenarioTwoSpeciesFoundation stage-specific package-scope failure** (`raw/17`):
   `Tools/Verification/ScenarioTwoSpeciesFoundation/run_species_foundation_check.py` pins
   `Packages/manifest.json` + `Packages/packages-lock.json` in its PROTECTED list byte-identical to
   its fixed BASE `8f9ed8025243254fe7378b6d7771a5f1e7efaccc`, and its scope check requires the
   diff-vs-BASE set to be exactly its ALLOWED list. Any later legitimate package change (including
   this 1.1.1 proposal) makes that checker FAIL **by design**; corroboration:
   `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04/HANDOFF.md` line 147 records it
   "already cannot pass on current `main`". Per Manager #332 D-1 precedent this must be handled with
   integration-specific byte-identity witnesses, **never** by relabelling FAIL as PASS or muting the
   test. The author report flags the same item; I concur.
6. **Requirement for future actual verbose UPM logging / stop-on-drift:** a future separately
   admitted environment run must capture verbose UPM logs to identify the second-resolve requester
   and must STOP on any manifest/lock rewrite not matching the approved policy. Until then the
   trigger stays unknown and 1.1.1 adoption is unqualified.
7. **Materials remain deliberately separate:** the 5 URP 17.6.0 material hunks observed in #287 are
   untouched by this proposal and stay a future native-render control; visual equivalence unproven.

### Check 6 — explicit NOT RUN list (carried in this result)

Not run by this review (and therefore not qualified by it):

- Unity Editor of any kind; Unity licence; the `unity-lock` lease; any Library creation/import;
- UPM resolve, package installation, package download, or any network/registry access;
- any build (Linux or Windows Player), material rendering, Roslyn compile;
- source-gameplay or native UI checks; frozen Reference Future v1 / Legacy40 / user saves /
  baseline rehash (untouched);
- the original registered Workbench C–G environment gates; integrated A–O (remains **0/15**);
- the 41 `WorkbenchRetirement` runner mocks (counted only, not executed);
- package-cache inspection on ZX20 (the protected lane was never opened).

No package-policy approval and no Editor/lane permission is given or implied by this review.

---

## Reproduction summary table

| Control | Result | Evidence |
|---|---|---|
| Committed validation (31 controls) | **31/31 PASS**, exit 0 | `raw/R1-committed-validation.*` |
| Red base-vs-base negative control | **26 PASS / 5 FAIL** (C04×2, C06, C07, C16), exit 1 — genuine FAILs | `raw/R2-negative-control*` |
| Mutant M1 manifest bumped / lock stale | KILLED, rc=1, 4 FAILs | `raw/R3-mutation-controls.json` |
| Mutant M2 sysroot version left 1.1.0 | KILLED, rc=1, 4 FAILs | same |
| Mutant M3 arm64 left 1.1.0 in manifest | KILLED, rc=1, 3 FAILs | same |
| Mutant M4 extra unrelated bump (inputsystem) | KILLED, rc=1, 4 FAILs | same |
| Mutant M5 sysroot dep ref left 1.1.0 | KILLED, rc=1, 3 FAILs | same |
| Mutant M6 CRLF reformat | KILLED, rc=1, 2 FAILs | same |
| Mutant M7 version 1.1.2 | KILLED, rc=1, 4 FAILs | same |
| Exceptions counted as kills | **zero** in this run; superseded run not used | `raw/R3-kill-quality-audit.txt` |

## Agreement with the author's claimed values

My independently recomputed numbers (`analysis/reviewer-independent-json-delta-enum.json`,
`analysis/reviewer-compile-log-block-compare.json`) **agree with every author-reported value checked**:
10 changed values (3 manifest + 7 lock), 65 lock entries, byte lengths 2,297/14,223, the four sha256
pairs, git pin `585ada20…`, the 13 identical transitive notes, 25/25 working-tree and 31/31 committed
runs, 5-designer-FAIL red control, 7/7 genuine-failure mutants, tree ids and blob ids
(`raw/18`–`raw/20`). **Disagreements: none found.** Two transparency notes (not disagreements):
(a) the "UPM wrote these bytes" link inherits from the #318 capture chain (verified here only via its
SHA256SUMS manifest and internal consistency); (b) the superseded exception-counting mutant run is
absent from the evidence tree — only the corrected run exists, and my reproduction is independent of
it.

## LIMITS

- Everything here is source/provenance evidence only. No Unity, no runtime, no build behaviour is
  verified by me.
- The reviewer is of the same model family as the orchestrator session (disclosed above); the
  Manager may weigh this. I am non-Anthropic and independent of the author.
- The protected ZX20 lane was never opened; provenance of the preserved backups rests on the #318
  Files-side copies and their SHA256SUMS manifest.
- Offline by design: registry existence/content of 1.1.1 is not verifiable here.
- No source fix was made; no test or guardrail was changed; nothing was deleted or cleaned up.

## VERDICT

**APPROVED (SOURCE PROPOSAL ONLY).**

The proposed 1.1.1 manifest/lock at `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` is an **exact,
minimal, auditable reproduction** of the Editor-observed package rewrite: exactly two files and
exactly ten 1.1.0→1.1.1 values, byte-identical to the preserved UPM-written output, with both 65-entry
compile.log registration blocks matching old input and proposed output respectively, no other graph
drift, no verifier/test/safeguard change, and a validator whose controls reproduce 31/31 with a
correctly red negative control and 7/7 genuine-failure mutants. **This is explicitly NOT Unity
qualification and NOT adoption** of 1.1.1: it is a recommendation that the proposal is suitable for a
Manager package-policy decision. No material package source-proposal flaw was demonstrated.

## HANDOFF TO MANAGER #200

- **Manager #200 decides package policy.** This review approves the proposal as source evidence only;
  adoption of 1.1.1 as the canonical project package baseline remains a separate deliberate Manager
  ruling, with the risks above (esp. 1.1.2 patch drift, registry/machine variance, Linux
  sysroot/clang/linker impact, non-inheritability of prior Unity results).
- **Only after** a package-policy decision, consider a **separate** warm-lane environmental
  qualification / B3 witness packet (8/8 protected-byte re-verification, 20 GiB STOP / 15 GiB hard
  floor, exclusive Unity lease, verbose UPM logging and STOP-on-drift). Future warm-lane reuse is not
  approved by anything in this review.
- **B3 runtime pose remains unproven** (offline replay is a falsifiable hypothesis only).
- **Integrated A–O remains 0/15**; no integration, PR #300, merge or main change is authorised here.
- Note for the future integrator: the ScenarioTwoSpeciesFoundation stage checker (and similar
  stage-specific package-scope assertions) will FAIL by design on any tree carrying this change;
  handle per Manager #332 D-1 with byte-identity witnesses — never weaken or mute them.

*End of independent review. Reviewer lane released; result file left untracked; STOP for Manager #200.*
