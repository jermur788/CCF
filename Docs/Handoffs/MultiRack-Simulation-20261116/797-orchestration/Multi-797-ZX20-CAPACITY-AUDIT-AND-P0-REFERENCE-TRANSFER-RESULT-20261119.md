# Multi #797 — ZX20 transfer-capacity audit and P0 reference custody transfer: result

**19 November 2026, 08:40 UTC.**

**Outcome 1 — ZX20 future-copy feasibility: PASS for the Year0 viewer; CONDITIONAL but quantity-trivial for the Year2–10 engine (not yet built).**
**Outcome 2 — P0 three-file custody transfer: PASS, hash-verified, original unchanged.**

No bulk copy, no archive, no migration, no `.NET`, no source edit, no worktree change.

## Actors

| Role | Identity |
|---|---|
| Ops/custody actor | GPT-6.1 Sol, Codex, T3 Code, `jer-laptop`; claim [#797 08:33Z](https://github.com/jermur788/CCF/issues/797#issuecomment-7003967792) |
| Parent / coordinator | DeepSeek V4.1 Flash, native OpenCode |
| Base | `origin/main` `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` |

All figures below were measured twice: once by the custodian, once independently by the parent. Differences found were apparent-vs-allocated accounting, not contradictions.

## 1. Live storage state (measured, not inferred)

| | |
|---|---|
| Mount | `/media/jer/ZX20` ← `/dev/sdc2`, **exFAT**, UUID `18D6-896E`, `rw`, uid/gid 1000, `errors=remount-ro` |
| Capacity | 512,075,235,328 B (476.91 GiB) |
| Used | 144,244,998,144 B (134.34 GiB, **28.2 %**) |
| **Free** | **367,830,237,184 B (342.57 GiB)** |
| Files volume (source side) | 38,601,433,088 B free before transfer, 38,601,199,616 after |

The "45 GB free" figure that shaped earlier handoffs is **stale and materially wrong**. ZX20 is roughly two-thirds empty.

## 2. Portable inventory (deduplicated)

| Item | Path | Apparent B | Allocated B | Overlap handling | Archive form |
|---|---|---:|---:|---|---|
| Viewer bare store (shared by both worktrees) | `Operations/CCF-H0A-Year0Viewer-20261118/git/CCF-H0A-Year0Viewer.git` | 1,013,506,296 | **1,014,190,080** | counted **once**; `.git` in the worktrees are pointer files, not copies | Git bundle |
| Core worktree | `…/ccf-h0a-core` | 156,814 | 225,280 | includes untracked 0444 fixture (32,567 B) | tar snapshot |
| Viewer worktree | `…/ccf-h0a-viewer` | 152,300 | 221,184 | — | tar snapshot |
| Artifacts (package, manifest, contract, reference bundle) | `…/artifacts` | 1,167,134 | 1,331,200 | package `year0-viewer.html` = 127,831 B | tar snapshot |
| Operation remainder (hooks, dirs) | `…/CCF-H0A-Year0Viewer-20261118` | — | 16,384 | — | tar |
| 784 verification evidence | `Verification/CCF-H0A-Year0Viewer-20261118` | 28,802,000 | 28,160,000 | screenshots, logs, receipts | tar snapshot |
| Engine lane today (incl. own `.git`) | `Operations/CCF-789-Y2-10-20261119` | 334,779 | 884,736 | fresh repo, **no Unity ancestry** | Git bundle + tar |
| P0 reference bundle (added today) | `…/ccf-y2-10-src/Tools/MultiRackSitkaP1Lab/.p0ref` | 122,485 | 233,472 | binary + sidecar + provenance | tar (inside snapshot) |
| **Portable total today** | | | **1,045,262,336 (996.8 MiB)** | | |

**Why the store dominates:** the viewer branches descend from the Unity repository, so a full-history bundle carries the art ancestry — largest single blob is a 26.5 MB sessile-oak FBX, and the pack is 964.63 MiB across 6,368 objects. The viewer's own source is ~300 KB. Worktree directory size is therefore **not** portable completeness, exactly as the packet warned.

## 3. Excluded from this transport

| Item | Allocated B | Reason |
|---|---:|---|
| #725 retained original and its bare store | 1,019,088,896 | protected custody to 2026-11-27T18:00Z; requires explicit new owner permission, excluded silently-including |
| #710 retained assets | — | protected; separate custody |
| Main shared store `Files/CCF/Repository.git` | 4,394,524,672 | project-wide store, not part of either transport set; a viewer bundle already carries the reachable subset |

## 4. Capacity arithmetic

Planning budget for the not-yet-built engine: **500 MiB** (source + tests + `bin/obj` + Q/review evidence + Git growth), explicitly a labelled planning allowance, not a measurement.

| Case | Bytes | GiB |
|---|---:|---:|
| (A) realistic: portable total + future budget | 1,569,550,336 | 1.46 |
| (B) conservative: uncompressed data **plus** an independent full Git bundle of the viewer store + future budget | 2,583,740,416 | 2.41 |
| (B+10 %) worst case used for headroom | 2,842,114,457 | 2.65 |

| Floor / headroom | Bytes | GiB |
|---|---:|---:|
| ZX20 total | 512,075,235,328 | 476.91 |
| 16 % post-migration reserve floor | 81,932,037,652 | 76.31 |
| Max migratable under that floor | 285,898,199,532 | 266.26 |
| Free after worst-case copy | 364,988,122,727 | 339.92 |
| Headroom over the floor | 283,056,085,075 | 263.62 |
| Capacity / requirement ratio | 100.6 × | — |

**Verdict: PASS.** After the worst-case copy, 71.3 % of ZX20 remains free against the 16 % (76.3 GiB) floor. The engine outcome is **CONDITIONAL** only because its final size does not exist yet; the engine lane is a fresh repository with no Unity ancestry, so even a hundredfold growth (≈88 MB) is immaterial.

## 5. exFAT is a carrier, not a workspace

Binding constraints if transport is later authorised:

- **No POSIX fidelity.** exFAT stores no uid/gid, no mode bits, no symlinks, no hard links, no sparse files. A copied the worktree **loses executable bits and cannot host a Git worktree**. `tar` with `--numeric-owner -p` preserves metadata inside the archive; extraction on ext4 restores it.
- **Case-insensitive.** A payload with `Foo.cs` and `foo.cs` collides on write. Check the manifest for case collisions before extracting.
- **Timestamps.** 10 ms resolution and local-time quirks; **hashes are authoritative, never mtimes**.
- **File size.** exFAT allows 16 EiB, so the 1 GiB bundle is fine (FAT32's 4 GiB limit would not be).
- **Never run `git` against the exFAT copy.** Object permissions, case handling and lock files are unsafe.

### Proposed archive set (not created)

For each transported unit:

1. `<unit>-<date>.gitbundle` — `git bundle create --all`, carrying every local ref and **all unpushed commits**.
2. `<unit>-<date>.snapshot.tar` — `tar --format=pax --numeric-owner -p`, containing worktrees (with the 0444 fixture), artifacts, package and essential evidence.
3. `SHA256SUMS`, `MANIFEST.txt` (per-file byte counts) and `RESTORE.md`.
4. Verify on the carrier with `sha256sum -c`; verify after extraction with a second `sha256sum -c` plus `tar --compare`.

### Restore

- **Linux / WSL2:** extract to an **ext4** path, `git clone <bundle>` then `git worktree add`; `tar -xpf` as the owning user; run `sha256sum -c`; confirm `git fsck --full`.
- **Windows host:** NTFS work area with WSL2-native ext4 for the Git worktree; do not use the exFAT copy directly; line-ending and mode differences must be normalised before any build.
- **Either:** verify the bundle with `git bundle verify` before cloning; verify the `.p0ref` DLL against `SitkaLab.Core.dll.sha256` and `provenance.txt` after restore.

### Partial-transfer risk

A 1 GiB contiguous bundle on a 342 GiB free volume is low-risk; the realistic hazards are an interrupted copy (mitigated by `SHA256SUMS` re-verification, not mtime) and silent case collision (mitigated by a manifest check). Copying directories without bundles would lose refs and unpushed commits; that is the main failure mode this plan avoids.

### Go / no-go

- **Viewer:** **GO** for archive transport, subject to a later owner/Manager transfer grant. Capacity is not a constraint.
- **Engine:** **NOT YET** — it does not exist at final size. Re-run this audit after implementation; the expected delta is ~0.5 GiB against ~340 GiB free.
- **Now:** **NO-GO for any copy.** Nothing was written to ZX20.

## 6. P0 three-file custody transfer (completed)

Authorised by the owner at 08:22:14 UTC: exactly one read-only transfer of the retained Year1 P0 reference bundle, no P0 source export or rebuild.

| File | Bytes | SHA-256 | Mode |
|---|---:|---|---|
| `SitkaLab.Core.dll` | 120,832 | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | 0600 |
| `SitkaLab.Core.dll.sha256` | 84 | `c0670d30203d6f0e18493a402b77ae7ca5d583e503c58ed45a75989f8c4b33f5` | 0600 |
| `provenance.txt` | 1,569 | `556317ad2e471698c133c598280364ac6d12e2a69975ca7a93445b57da701324` | 0600 |

- **Source:** `…/CCF-725-F0Annual-20261118/ccf-725-src/Tools/MultiRackSitkaP1Lab/.p0ref/`, opened read-only.
- **Destination:** `…/CCF-789-Y2-10-20261119/ccf-y2-10-src/Tools/MultiRackSitkaP1Lab/.p0ref/`, created once, `O_EXCL`, no clobber, no symlink traversal.
- **Provenance accepted:** `p0QualifiedCommit=a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, `p0QualifiedTree=f212c8f64fb28b83aa870c7b5b7c72e03051e46a`, `sdk=8.0.133`, 15 input hashes; DLL digest matches both the sidecar and the provenance.
- **Source unchanged:** HEAD `df467dcc`, working tree clean, all three files retain their 2026-11-18 07:33:46 UTC mtimes and identical hashes. The custodian could not snapshot mtimes before copying, so mtime equality is inferred from the timestamps predating this task by a day; content equality is directly proven.
- **Lane unchanged:** HEAD `4b81e71eaa3d0d614c319a38afe8b9e030c61bda`, `git status` clean — `.p0ref/` is declared in the lane's own `.gitignore` (line 8), so the binaries are deliberately untracked.
- The first 30-file/fixture transfer was **not** repeated.

## 7. Receipts

| Item | Path |
|---|---|
| Custodian measurements and transfer logs | `/media/jer/Files/CCF/Verification/Multi-797-Capacity-20261119/logs/` |
| Transfer receipt | `…/p0-transfer-receipt.json` |
| Transfer script | `…/p0_transfer.py` |

## 8. Not done

No bulk copy, tar, zip, bundle or archive created. No file written to ZX20 or Videos (free space byte-identical before and after). No worktree created, moved or removed. No `.NET`, restore, build or test. No source edit or new tracked file. No git push, no remote, no branch published from the lane. No change to the shared Ops tool, board, lanes or state. No #770 Scenario One, #784 source-byte, #710 or #687 access beyond the size metadata listed above. No Unity, Windows, Host, main merge or release. No GPT-6.2. `L4 FAIL38` UNWAIVED. Dashboard/Drive sync not verified.

## 9. Next grant the Manager should issue

1. **Transfer grant** (separate, later): permission to create the bundle/tar/SHA256SUMS set and place it on ZX20, with a named carrier path and a restore rehearsal. Not requested by this task and not needed until the engine is complete.
2. **C + offline .NET + reviewer/Q grants** for #789, now that the lane holds both the P1 closure and the verified P0 reference.
