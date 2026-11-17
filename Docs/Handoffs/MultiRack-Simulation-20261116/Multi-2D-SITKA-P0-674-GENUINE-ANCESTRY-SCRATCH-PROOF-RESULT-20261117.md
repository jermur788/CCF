# Multi #674 — isolated genuine P0 ancestry scratch proof result

**17 November 2026. Worker: independent `opencode-go / DeepSeek V4.1 Flash` (T3 Code, native Linux).**
**Verdict: PASS — scoped local historical ancestry proof only. H is NOT integrated, NOT current-main, NOT qualified.**

This report proves one narrow Git fact: a genuine two-parent merge commit **H** exists whose first parent is the fixed target **M**, whose second parent is the qualified P0 commit **Q**, and whose tree is **exactly M plus the 52 pinned `Tools/MultiRackSitkaLab/**` blobs from Q**, with every other target path, mode and absence unchanged. It also proves the original untouched L0 path-history predicate is empty and L1 is the exact qualified P0 directory tree.

It does **not** claim integration, current-main compatibility, a qualified candidate, a build, a runtime result, or any P0/P1 operational readiness. Default `.NET` **L4 FAIL 38** is unresolved and the **first ecological year remains HOLD**.

## 1. Authority and identity

- Worker: `opencode-go` / `deepseek-v4.1-flash` (DeepSeek V4.1 Flash), T3 Code via OpenCode, native **jer-laptop** (Linux Mint 22.3 Zena, kernel `7.0.0-42-generic`, x86_64); no WSL, no Windows.
- Independence: this worker authored neither the qualified P0 source (original Claude author) nor the P1 feature source; no delegate or subagent was used.
- Authority chain: [original #674 conditional permit](https://github.com/jermur788/CCF/blob/docs/multi-p0-674-conditional-ancestry-proof-permit-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P0-674-CONDITIONAL-GENUINE-ANCESTRY-SCRATCH-PROOF-PERMIT-20261117.md), the [re-pin/custody addendum](https://github.com/jermur788/CCF/blob/docs/multi-p0-674-conditional-ancestry-proof-permit-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P0-674-MANAGER-REPIN-AND-LEASE-RECEIPT-ADDENDUM-20261117.md), and the [fixed-snapshot policy amendment](https://github.com/jermur788/CCF/blob/docs/multi-p0-674-fixed-snapshot-policy-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P0-674-FIXED-SNAPSHOT-PROOF-POLICY-ADDENDUM-20261117.md), with the binding immutable target issued in [Manager #641's final re-pin](https://github.com/jermur788/CCF/issues/674#issuecomment-6954755871).
- Registration: [RUNNING — RESUMED, FIXED SNAPSHOT](https://github.com/jermur788/CCF/issues/674#issuecomment-6955018018) was posted **before** the M fetch, worktree creation and any merge/object-writing operation.

## 2. Pins (all revalidated inside the isolated store)

| Pin | Object | Value |
|---|---|---|
| Fixed target M | commit | `e38e1cf35cd686d98f525ffdb95c7e183a4c05eb` |
| Qualified P0 Q | commit | `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866` |
| Real merge base B | commit | `fc18190b976f1363f1b5d1b0881810db2f8c6e39` |
| Q root tree | tree | `dfbf5f9df729f2433e1836dbf6c6ee3b33a7043e` |
| **Q P0 directory tree** | tree | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| Feature F (read-only, never a parent) | commit | `e302f15bf6188605a2c2cbeddbd973b3d8ac0f66` |
| Context snapshot | commit | `a5541b8a193883792db290539b7a3b877b589ff2` |

M was read as live `origin/main` at 2026-11-17T16:51:50Z and matched the binding comment exactly. M contains **neither** `Tools/MultiRackSitkaLab` nor `Tools/MultiRackSitkaP1Lab`. Q is **not** an ancestor of M. `git merge-base M Q` = `fc18190b…`, matching the preflight. History is complete and **nonshallow** (`is-shallow-repository false`, no `shallow` file), so no boundary truncates the proof.

## 3. Custody and environment

- Lane: `/media/jer/Files/CCF/Operations/CCF-MultiP0-AncestryProof-20261117` — external ext4 `/dev/sdb4`, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`.
- Isolated object store: `…/git/CCF-MultiP0-AncestryProof.git` — bare, repo-local `ccf.taskId=674`, `ccf.worker="opencode-go/DeepSeek V4.1 Flash"`, `ccf.isolated=true`, `ccf.expires=2026-11-18T18:00:00Z`. The shared `Repository.git` and the legacy store were **never** written.
- Worktree: `…/ccf-multi-p0-674` (the only worktree of the isolated store).
- Branch: `integrate/multi-p0-q-main-ancestry-proof-20261117-v1`, local only, never pushed.
- Lease receipt: `…/receipts/CCF-MULTI-674-custodian-reservation-20261117.json` SHA-256 `7484be43c675175945a90609d52ff78a4470123b21bddf52a5b510b86b0fd90d`; deployed board row `CCF-674-MULTI-P0-ANCESTRY` and `lanes.json` `MultiP0Proof674` unchanged; `lane-status --json` exit 0, storage PASS, no conflict; 57.7 GiB free at resumption.
- No Unity lease, Editor, dotnet, strace or build process was used at any point.

## 4. Exact operations performed

1. Fetched `main` and the Q/F branches into the isolated store only; complete nonshallow ancestry.
2. Selected the exact M **commit object** (not a moving ref) and created the single worktree/branch from it; `git status` clean including untracked before mutation; 5,862 tracked paths.
3. Ran a genuine merge attempt: `git merge --no-ff --no-commit Q`. Result: **automatic merge, no textual conflict**, staging **63 additions** — the 52 P0 files plus exactly 11 unapproved Q-era documentation/spec additions.
4. Deliberately rebuilt the index to the permit's allowlist: `git read-tree --reset M` then `git read-tree --prefix=Tools/MultiRackSitkaLab/ Q:Tools/MultiRackSitkaLab`. The 11 unapproved additions were **excluded from H's tree**; Q remains a real parent, so its history is preserved without importing its later working-tree content.
5. Verified the staged allowlist, then created exactly one merge commit **H** (MERGE_HEAD = Q), first parent M.
6. Ran the unchanged predicates and a `--full-history` diagnostic (diagnostic only, never a substitute).

## 5. Candidate H

```
H     = 5aa7b6efd10902ac36bb412bdf3411f4163aab19
tree  = 3b5024acd9a0798ffba4eeec377893691770bfd2
parents = e38e1cf35cd686d98f525ffdb95c7e183a4c05eb   (M, first)
          a7f41983c2f2f233b8fe999d1a5b10e72bdd2866   (Q, second)
tracked paths: M 5,862 -> H 5,914 (+52, no deletions)
```

## 6. Predicates (native results)

| # | Predicate | Expected | Observed |
|---|---|---|---|
| 1 | `git rev-list --parents -n1 H` | H M Q | **H M Q** |
| 2 | `git merge-base --is-ancestor Q H` | exit 0 | **exit 0 (real Q ancestry)** |
| 3 | `git merge-base --is-ancestor M H` | exit 0 | **exit 0** |
| 4 | `git rev-parse H:Tools/MultiRackSitkaLab` | `f212c8f6…` | **`f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (L1 exact)** |
| 5 | `git log --oneline Q..H -- Tools/MultiRackSitkaLab` (original flags) | empty | **empty, 0 lines, exit 0 (L0 passes)** |
| 6 | `git diff --name-status M H` | 52 additions only | **52 × `A`, all under `Tools/MultiRackSitkaLab/`** |
| 7 | non-P0 diff `M H -- . ':(exclude)Tools/MultiRackSitkaLab'` | empty | **0 lines** |
| 8 | P0 entries vs Q | identical | **`git ls-tree -r` identical (modes + blobs)** |
| 9 | Q-era docs/specs in H | absent | **all 11 absent** |
| 10 | canonical subtrees `Docs Assets Packages ProjectSettings specs Tools/{Build,Calibration,Diagnostics,ProjectContext,Verification}` | identical to M | **all identical** |
| 11 | history completeness | nonshallow | **complete, no `shallow` file** |

**Diagnostic only:** `git log --full-history --oneline Q..H -- Tools/MultiRackSitkaLab` returns **1** entry (H itself). This is the documented default-simplification behaviour and is reported for completeness; the original unaltered L0 command above is the predicate, and it is empty.

## 7. Excluded Q-era additions (present in Q, deliberately absent from H)

Seven Multi documentation files and four spec files:

```
Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-008-WINDOWS-WSL2-QUALIFICATION-ACCEPTANCE-20261114.md
Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-001-SITKA-2D-LAB-P0-USER-APPROVAL-RECORD-20261114.md
Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-HANDOFF-20261114.md
Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-004-SITKA-LAB-P0-HARDENING-IMPLEMENTATION-HANDOFF-20261114.md
Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-WRITE-SAFETY-REPAIR-HANDOFF-20261114.md
Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-WSL2-UBUNTU-QUALIFICATION-HANDOFF-20261114.md
Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-Accurate-Post-Save-HTTP-Response-Repair-20261116.md
specs/multirack-sitka-2d-reference-lab-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}
```

These remain untracked files left in the disposable worktree by the merge attempt; nothing was deleted, stashed or discarded.

## 8. Live-main movement after M (freshness gate, not an abort)

M was live main when selected. Later, before publication, live main advanced:

```
M..tip: 2 commits, sole changed path Docs/Project/coordination/active-tasks.md
  a9fb2ca7 docs(coordination): record delegated Compact save-policy authoring #677 and #681 independent review gate
  12711c99 coordination(multi): reserve #682 independent browser-only L4 evidence triage
```

Coordination-only; no P0/P1 source, no ownership overlap and no custody change, so the fixed-snapshot policy applies and the historical proof proceeds. **H must not be treated as current-main compatible.** Any future incorporation requires a fresh merge-base/target diff, D-085-and-later canonical reconciliation, independent review and a new Manager permit. H is never to be silently rebased.

## 9. Evidence artifacts

Durable root `/media/jer/Files/CCF/Verification/CCF-MultiP0-AncestryProof-20261117/`:

| Artifact | SHA-256 |
|---|---|
| `pins-and-graph.txt` | `7835745fea04ffc41a813fc9515a8b196e14e054737fdb46013b18c9f411fb86` |
| `H-commit-object.txt` | `02064bdb3a99218f216128e98742054345d2c539c836b5f054c19f9231baf670` |
| `H-parents.txt` | `37870c55587b72997f0c6397cbd722549e22f8636e8c4289dd0ddfc25a17c5a2` |
| `M-to-H-52-additions.txt` | `247a2722a3b01072824883bed85eac59ca83e60651fa59450b2f9171379ae5f1` |
| `L0-original-empty.txt` | `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855` |
| `L0-diagnostic-full-history.txt` | `655a0d723f66c162055668898b8b4524d3a32ad64099e46204c14d7e5649e444` |
| `L1-H-P0-dir-entries.txt` | `f18054d3f3bb3158cb463ac80cee335f3ffec1a3fc8e90b9e990c2b048d002d0` |
| `L0-L1-predicates.txt` | `a57e49ffe44c541e72796006fe1378112c67b6153350a853b059b860f9f24893` |

## 10. Not performed / limitations

- No P1 import, no P1 branch parent, no F merge; F remained a read-only comparison.
- No push of H, no merge into `main`, no remote source commit; H is local-only.
- No build, `dotnet`, `strace`, Unity, Windows, Player, runtime or ecological-year action; no test weakening or acceptance change.
- No destructive cleanup; shared store, legacy store, `#639` author lane, dirty `CCF-Primary` and Scenario One work are untouched.
- **This is not an integration acceptance.** Independent non-author review of H, its parents and this evidence is required before Manager #641 may consider any later packet.

## 11. Suggested next gates

1. Independent non-author review of H (`5aa7b6ef…`): two real parents, 52-file allowlist, non-P0 byte equality, L0/L1 and ancestry.
2. Manager acceptance of this proof **as historical evidence only**.
3. If a current-main P0 prerequisite is desired later: fresh target selection, explicit D-085/later canonical reconciliation, a **new** merge-base diff, and a separate source/qualification permit — never a silent rebase of H.
