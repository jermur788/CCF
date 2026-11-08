# #335 — Option B Linux SDK 1.1.1 package source PROPOSAL (author result)

**Status:** HANDED-OFF for the independent #336 review, followed by the Manager #200 decision.
**What this is:** a source-only **PROPOSAL**, built offline.
**What this is not:**
- not adopted package policy;
- not a self-approval;
- not an Editor import, compile or Unity qualification;
- not permission to touch any registered lane.

**Claim:** [#335](https://github.com/jermur788/CCF/issues/335). [RUNNING receipt](https://github.com/jermur788/CCF/issues/335#issuecomment-6718938318) was posted before any edit.
**Binding packet:** [MANAGER-PACKAGE-PROVENANCE-OPTION-B-REVIEW-PACKET-20261108.md](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-PACKAGE-PROVENANCE-OPTION-B-REVIEW-PACKET-20261108.md), read at `origin/main` `2bc48839`.
**Author (actual):** Claude Opus 5.5 (`claude-opus-5-5`), Anthropic, through Claude Code in T3 Code.
- Requested effort: High. The session cannot verify its own effort setting.
- One sole source writer, no delegation. Git user `jermur788`.

**Lane:**
- branch `proposal/linux-sdk-111-provenance-20261108-v1` (new);
- worktree `/media/jer/Files/CCF/Operations/CCF-PackageOptionB-20261108` (new, Files);
- common store `/media/jer/Files/CCF/Repository.git`.

**Evidence:** `/media/jer/Files/CCF/Verification/CCF-PackageOptionB-20261108/` holds the scripts, JSON outputs, exit files and `SHA256SUMS.txt`.

## Identity

| Item | Value |
|---|---|
| Starting HEAD (exact published Workbench base) | `03b484d10b36382ab86d5e345ece9acac96c9620` = live `origin/fix/workbench-receipt-finalization-20261107-v1` at start |
| Source-tested Workbench predecessor | `a4019e6b96b50e03d2b71d92c44fc30a0f7ace29`, an ancestor of the base. Its diff to the base is one result Markdown only |
| **Tested proposal source commit** | **`46bc1fd618ae5b214d7de296a03ac7efec27cb7c`**. Changes exactly `Packages/manifest.json` and `Packages/packages-lock.json` |
| Report-only publication commit | the child of `46bc1fd6` that adds only this file. Its SHA is in the #335 HANDED-OFF comment |
| Preflight (07:4xZ) | Branch and worktree absent locally and on origin. Fresh worktree: tracked/untracked/ignored status empty. No Unity process. Files 25.44 GB free. ZX20 not touched |

## Verdict: PROPOSAL PRODUCED (not BLOCKED)

**Why a lock could be produced without Unity, network or guessing:** no lock entry was hand-constructed. The two proposed files are **byte-identical copies of the files Unity 6000.6.0f1's Package Manager itself wrote** in the #287 registered lane. Those bytes were preserved losslessly by #318 Phase 0 and are now on Files:
- `Workbench287MorningClosure-20261107/dirty-backup/Packages/`;
- `SHA256SUMS.txt` sha256 `9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74`, re-verified now.

UPM wrote them from a manifest/lock that is **byte-identical to this base**:
- blobs `ca552487…` and `8c0c0978…` are the same at `4d95e452` (lane HEAD), `a4019e6b` and `03b484d1`;
- sha256 `c533b915…` and `013b9f99…` equal the preserved `head-versions/`.

So the proposal is exactly the Editor's own resolver output for this package input. It is not an inference.

### Exact delta (machine-readable copy in `04-committed-validation.json`)

| File | Key | Old | New |
|---|---|---|---|
| manifest | `/dependencies/com.unity.sdk.linux-arm64` | 1.1.0 | 1.1.1 |
| manifest | `/dependencies/com.unity.sdk.linux-x86_64` | 1.1.0 | 1.1.1 |
| manifest | `/dependencies/com.unity.toolchain.linux-x86_64-linux` | 1.1.0 | 1.1.1 |
| lock | `/dependencies/com.unity.sdk.linux-arm64/version` and `…/dependencies/com.unity.sysroot.base` | 1.1.0 | 1.1.1 |
| lock | `/dependencies/com.unity.sdk.linux-x86_64/version` and `…/dependencies/com.unity.sysroot.base` | 1.1.0 | 1.1.1 |
| lock | `/dependencies/com.unity.toolchain.linux-x86_64-linux/version` and `…/dependencies/com.unity.sysroot.base` | 1.1.0 | 1.1.1 |
| lock | `/dependencies/com.unity.sysroot.base/version` (depth 1, transitive) | 1.1.0 | 1.1.1 |

The table covers 3 manifest keys and 7 lock keys. **Nothing else changes:**
- the same byte length (2,297 / 14,223) and line count;
- the same key order and 65 lock entries;
- `depth`, `source` and `url` unchanged;
- the git package `com.kitwright.unity.mcp` keeps hash `585ada203ff25b1fbea23821e159afa4a930d481`;
- every other version and pin is unchanged.

Sha256 values:
- new manifest `f2ff5f95f9e93716947391a8818dbe642d81f0b49068516441a58def966c2c1c`;
- new lock `f43d9894af3c6dc054e86e98335dbd248a9fb97527b49c7b5952d96e037e52b1`;
- old manifest `c533b915d47d0dc1a5797c9b7e63197bfd3cb552639a60c8c0115f02abb00215`;
- old lock `013b9f990eb8b9c58b8f40099cb4fd9df92a01dcf5e1d6e09155bb67ddb60d9f`.

### Provenance chain (every link measured offline)

1. The Editor log `Workbench287Tier2-20261107/compile.log` (sha256 `a70cf548…66b1`) has exactly two `Registered 65 packages` blocks.
   - Block 1 (after a 41.28 s resolve): **every** registry/builtin version equals the base lock (C15). The Linux 4 are at 1.1.0, with cache dirs `@df9ffedfa568`, `@6d12be270055`, `@d0308bf9d559` and `@3559fe1aa613`.
   - Block 2 (40.56 s, `Lock file was modified`): every registry/builtin version equals **the proposed lock** (C16). The Linux 4 are at 1.1.1: `sdk.linux-arm64@5043731cb64f`, `sdk.linux-x86_64@920bc08129cd`, `toolchain…@95ffda8fe8d9` and `sysroot.base@0a249da1e1ab`.
   - The two blocks differ in **only those four packages** (C17).
2. #333 measured that the lane's `Library/PackageCache` now holds only those four 1.1.1 dirs, with `package.json` = 1.1.1. I did not re-read the ZX20 lane: that is cited evidence, not re-measured.
3. Manifest/lock mtimes are 00:49:10Z, during the #287 cold import, under the lease (per #333).

## Offline validation (bounded, static; no Unity, network or package resolution)

Script `scripts/validate_proposal.py`, run with `python3 -I`. It reads git objects, the preserved backup and the compile log only.

| Run | Target | Result | Exit |
|---|---|---|---|
| 01 negative control | base vs base (no change) | 26 PASS / **5 FAIL** (C04 ×2, C06, C07, C16), as designed | 1 |
| 02 | uncommitted working tree | **25/25 PASS** | 0 |
| 04 | committed `46bc1fd6` vs `03b484d1` | **31/31 PASS** | 0 |
| 03 mutation controls | 7 in-memory mutants of the candidate | **7/7 killed by genuine FAIL verdicts** (2–4 FAILs each) | 0 |

The 7 mutants:
- manifest bumped but lock left at 1.1.0;
- sysroot version left at 1.1.0;
- one direct package left at 1.1.0;
- an extra unrelated bump;
- a sysroot dependency ref left at 1.1.0;
- CRLF reformat;
- 1.1.2.

My first mutation run counted harness exceptions as kills. I found that, fixed it so only `rc==1` with FAIL lines counts as a kill, and reran. Only the corrected result is reported.

Checks C01–C21:
- backup integrity;
- base = preserved HEAD bytes;
- candidate = preserved Editor bytes;
- the exact key diff;
- byte, line and order shape;
- git-package hash unchanged;
- lock structure: every direct dependency at depth 0 with a matching version, and every dependency reference present;
- no new requested-vs-locked mismatch. The 13 pre-existing higher-resolved transitive notes are identical before and after;
- the log cross-checks;
- changed paths ⊆ the allowed 3;
- tree ids unchanged at base and candidate, and equal to `a4019e6b`:
  - `Tools/Verification` `d6e960ca3b5fb8dabe51fcd92f7b27bbe1854636`;
  - `Assets` `6d8ac19f03917e71a9e6eb6cde0ef628fcdd03e5`;
  - `ProjectSettings` `aa08457b80d4299389ef387fd6d9afc655a63e7b`;
  - `.vscode` unchanged;
- file mode 100644 kept.

`git diff --check 03b484d1 46bc1fd6`: clean (exit 0).

**Workbench verifier bytes, R1 and R324-01:** the whole `Tools/Verification` tree is identical. `WorkbenchRetirement/` sha256:
- `run_workbench_retirement.py` `d7df6401…6d1a`;
- `test_runner.py` `1b683522…b9`;
- `WorkbenchRetirementVerification.cs` `f6bf3e6d…87c`.

R1 compares post-Editor lane state against the lane's own git HEAD, with no pinned package bytes or versions. So its fail-closed rule is unchanged:
- **inference:** on a candidate carrying these bytes, an Editor that again writes exactly 1.1.1 produces no package drift;
- any *other* rewrite still FAILs.

No test or gate was weakened.

## Risk inventory

**Why 1.1.1 is plausible (evidence):** the pinned Editor 6000.6.0f1, in the only observed cold import of these exact package inputs, re-resolved to 1.1.1 on its own and rewrote both files. The proposal records exactly that output.

**Why the trigger stays UNPROVEN** (not inferred here):
- the Editor built-in manifest pins all four at `version` and `minimumVersion` 1.1.0;
- the level-2 log names no requester;
- #333 ruled out in-project `InitializeOnLoad` candidates.

Consequences:
- **R-1:** if the trigger is "upgrade to the newest available patch", committing 1.1.1 only holds until Unity publishes a newer patch (for example 1.1.2), and the drift would return. A future G-env run must still STOP on any manifest/lock change.
- **R-2:** if the trigger is network- or registry-state dependent, an offline or other machine might resolve differently.
- **R-3:** it is unknown whether the 1.1.0 first resolve was required before the upgrade, and that cannot be tested offline.

Other risks:
- **Toolchain behaviour that could differ (UNKNOWN, no changelog read offline):**
  - these packages provide the Linux sysroot/toolchain for Linux Player builds (IL2CPP/native link);
  - glibc/sysroot contents, linker/clang versions, the arm64 SDK and build-time behaviour could change Linux Player output.
  - They are Editor/build packages. **Assumption, not verified:** they should not change Editor-play simulation, saves or determinism.
- **Prior Unity results NOT inheritable across this change:**
  - every Unity result qualified with tracked 1.1.0 is in a different package environment on toolchain grounds (MFM, Objective1 native captures, S1B Linux Player builds, Reference-archive and Legacy40 save tests). Their source-level conclusions stand; their environment identity does not carry over;
  - #287's cold import PASS ran at 1.1.1 after the rewrite, but at old HEAD `4d95e452` and with failed lifecycle gates;
  - nothing is qualified by this proposal.
- **CCF boundaries preserved:** frozen Scenario One Reference Future v1 (`.bytes` unchanged), Legacy40 fixtures, saves, SDK/Reference/save acceptance and the 15 GiB floor are all untouched. Re-qualification of those boundaries under 1.1.1 is a separate future Manager gate.
- **Operational:** the three other warm ZX20 Libraries (MainIntegration, U1, UIQualification) hold 1.1.0. Importing this candidate there would swap about 0.37 GiB of package dirs and reimport them. Only the registered Workbench lane's Library already holds 1.1.1 (per #333).
- **Stage-specific checker:** `Tools/Verification/ScenarioTwoSpeciesFoundation/run_species_foundation_check.py` lists both package files as PROTECTED against its fixed BASE `8f9ed802`. On any integrated tree with this change it reports by design. That needs Manager handling like #334 D-1, never test weakening.
- **Materials — NOT modified, deliberately separate:**
  - the 5 URP 17.6.0 validation hunks (`_MainTex` alias ×2, `_AlphaToMask` ×2, `_Color` float round-trip ×3) stay a **future native-render control**;
  - visual equivalence is unproven;
  - no material normalisation, scene, RNG, tree or default-package/cache change was made.
- **Option B adoption still needs:** #336, a Manager policy verdict, a future G-env with verbose UPM log, and a STOP rule on any manifest/lock rewrite.

## Preservation

- The original registered lane `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106` was **not opened, read, listed or written** by this task, and its 8 dirty paths and HEAD were not touched. Only the Files-side preserved copies were read.
- The user primary `/media/jer/Files/CCF-Primary` has 2 pre-existing modified files (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`). They are unrelated and left untouched.
- `active-tasks.md`, PR #300, main and the Overall-Dev mirrors are not edited.

## NOT RUN

The following were not run:
- the Unity Editor or Player (any mode), the licence, the `unity-lock` lease, or creation, import or deletion of any Library;
- UPM resolve, package download or install, or network registry queries;
- package cache inspection on ZX20;
- Linux or Windows Player builds;
- the Workbench `test_runner.py` mocks: their bytes are unchanged, so prior #328/#329 41/41 evidence applies unchanged, but they were not re-executed here;
- the Roslyn compile;
- material, scene or ProjectSettings edits;
- cleanup or reset of any lane;
- integration, merge or rebase;
- active-tasks edits.

**Nothing in this report is Unity-verified. Source-only manifest/lock consistency is not Editor import validation.**

## For #336 (independent, cross-provider, read-only)

Reproduce with a disposable clone at `46bc1fd6`:

```
python3 -I validate_proposal.py <clone> 03b484d10b36382ab86d5e345ece9acac96c9620 46bc1fd618ae5b214d7de296a03ac7efec27cb7c out.json
```

Scripts are in the evidence dir. They read only Files paths.

Things to falsify independently:
- that the base bytes equal the preserved lane HEAD bytes;
- the compile-log block equality;
- the absence of any extra graph change or downgrade;
- that nothing claims the requester is known.

The Manager decides adoption.
