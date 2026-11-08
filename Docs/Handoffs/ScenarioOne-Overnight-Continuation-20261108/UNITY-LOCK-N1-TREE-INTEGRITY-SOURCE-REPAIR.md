# #343 — N-1 tree-object integrity repair (source only, not deployed)

**Status:** SOURCE REPAIR HANDED OFF for #344 delta review. **Not approved, not deployed.** No deployed `unity-lock`, real lease, Unity, protected worktree, shared Git object, cleanup, integration or build was touched.
**Author:** Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), effort numeric UNKNOWN. Single writer.

## Identity
| Item | Value |
|---|---|
| Base (#340 tested source) | `6b111b123dc5d03258e0481c499b0ca634b11a1d` |
| **Tested source** | `11cee0c26c81b916c77a5310370d7ce42f2a203b` |
| Branch / lane | `fix/unity-lease-n1-tree-integrity-20261108-v1`, `/media/jer/Files/CCF/Operations/CCF-UnityLeaseN1TreeIntegrity-20261108` |
| Report-only successor | the commit that adds this file (published in the #343 HANDED-OFF comment; it changes this one path only) |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-UnityLeaseN1TreeIntegrity-20261108/` (`SHA256SUMS.txt`) |

Changed paths (base → tested source), exactly the three allowed:
`Tools/Verification/UnityLeaseExactAdmission/unity-lock.proposed`, `.../unity-lock-exact-admission.patch` (regenerated: `diff -u` of the unchanged deployed baseline vs proposed), `.../test_tree_object_integrity.py` (new).
Blob ids at tested source: proposed `4c16b6e7…`, patch `7ff09074…`, new test `8a5129f3…`. Deployed-baseline copies, `_ccf` copy, original tests, profile constants, #318 manifest hash, the 8 dirty hashes, strict default path, lock/release/status/flock/PID logic and `post-transition head=None` are byte-unchanged (baseline copies compared with `cmp`).

## Finding that sharpens N-1 (measured)
Git re-hashes an object only on some read paths (`parse_object`). Forged **ROOT tree** and forged **HEAD commit** objects already failed closed on the old source (`hash mismatch … not a tree object`). The false accept is every tree **below the root**: `ls-tree -r HEAD` read them with no hash check. The repair covers all of them.

## The change
In `unity-lock.proposed`:
- `verified_head_inventory(lane, head, algo)`: drives `git cat-file --batch`, and for the pinned HEAD commit and each reachable tree re-computes `hash(<type> <size>\0<payload>)` under the repository object format (sha1/sha256) and compares it with the requested id. Wrong type, missing object, malformed header, truncation or hash mismatch → `ValueError` → exit 2.
- Tree payloads are parsed from raw bytes. Refused: submodule/unusual modes, empty/`.`/`..`/`.git`(any case)/slash-containing/non-UTF-8 names, repeated names. Iterative walk, each distinct tree verified once; bounds `MAX_GRAPH_OBJECTS` 2,000,000, `MAX_GRAPH_ENTRIES` 5,000,000, `MAX_GRAPH_OBJECT_BYTES` 64 MiB (a hash-valid tree DAG cannot expand into an unbounded listing).
- `prove_raw_working_tree` resolves `HEAD^{commit}`, builds the verified inventory, then still runs `ls-tree` (now against the resolved commit id) and refuses unless the two agree on every path, mode and blob id. Working-tree bytes are hashed against ids that came from verified trees.
- Reviewer O-1: all admission Git processes now run with `core.commitGraph=false` (one shared `exact_git_invocation`).
- Proof stats now record `head_tree`, `verified_commit`, `verified_trees`, `verified_tree_bytes`.

Blobs are deliberately not re-hashed in the graph walk: a blob id is only ever compared with the hash of the raw working bytes, so a blob object's stored content is never trusted.

## Evidence (all disposable fixtures under the Files evidence dir; real store untouched)
| Check | Old `6b111b12` | Repaired `11cee0c2` |
|---|---|---|
| N-1 forged loose **Assets** subtree + matching drift/index (`test_N1_forged_loose_subtree_object`) | **FALSE ACCEPT** (exit 0, lock created) | exit 2 "does not match its object id", no lock, no event |
| Independently varied: forged **Tools** subtree, different file/blob (`…second_subtree_object`) | **FALSE ACCEPT** | exit 2, no lock, no event |
| Forged ROOT tree; forged HEAD commit | refused (Git itself) | refused |
| Deleted tree object | refused | refused |
| Honest loose and honest **packed** objects | admit | admit; stats recorded |
| Direct graph tests: inventory == `ls-tree`; wrong type; `..`/`.GIT`/`a/b`/submodule names/modes; repeated name; entry and object bounds | n/a | 6 pass |

Red: `red-old-source.txt` (7 run: 2 FALSE ACCEPT failures; the stats-field test errors only because the old source lacks the new fields). Green: `green-new-source.txt` (13 run, OK).
**Controls, one final pass only:** existing `test_exact_admission.py` 77 OK and `test_raw_byte_admission.py` 40 OK on the tested source, which include F1 replace refs/grafts, F2 clean filter and autocrlf, F3 non-racy same-size drift, strict default, held/malformed lock and patch-reproduces-proposed. The full 77+40 programme was not repeated at multiple stages.

Run: `CCF_LEASE_TEST_TMP=<Files scratch> python3 -I -B test_tree_object_integrity.py -v`; old-source red: `CCF_LEASE_TEST_PROPOSED=<old unity-lock.proposed> … ForgedObjects`.

## Timing (Files ext4 only; not exFAT, not ZX20)
Synthetic disposable repo, 24,000 files / 1,800 trees: verified walk ≈ 0.18 s (best 179 ms, median 189 ms) beside `ls-tree` 29 ms best. Negligible vs the raw scan. This does **not** address O-3 (global flock held during the 1.138-GiB raw proof, no read timeout): still a separate deployment gate.

## Not covered / residual (stated, not claimed)
- Forgery inside a **pack file** is not tested: a fixture cannot forge one honestly in small scope. The hash check is source-agnostic (loose, packed, alternates), and an honest packed repository is a tested positive.
- Alternates: objects from an alternate are accepted if they hash correctly; no separate alternates refusal was added.
- A concurrent writer altering objects between the verification walk and the `ls-tree` call is not defended (same class as the existing file-read window); the comparison narrows but does not remove it.
- Blob content is verified through working-byte hashing only, as above.
- Unchanged and still unapproved: author-proposed 2026-11-11 UTC expiry; `post-transition` UNSUPPORTED; default strict-clean path uses Git status; O-2 unpinned Unix mode and tolerated empty directories.
