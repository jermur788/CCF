# Multi P1 #662 — four-record identity-only TrustTables status change: result

**17 November 2026. Status: BLOCKED AT VERIFICATION, as the packet requires: existing tests assert PENDING.**
The source change is **prepared, built and checked locally but NOT pushed**. No ecological year; no test, P0, fixture,
Unity, save or Windows change.

| Item | Value |
|---|---|
| Packet | [#662 implementation packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-641-661-manager-acceptance-and-662-dispatch-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-662-FOUR-RECORD-TRUST-STATUS-PROMOTION-IMPLEMENTATION-PACKET-20261117.md); [Manager #641 per-record verdict](https://github.com/jermur788/CCF/blob/docs/multi-p1-641-661-manager-acceptance-and-662-dispatch-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-641-661-FOUR-RECORD-MANAGER-VERDICT-20261117.md); [#661 independent review](https://github.com/jermur788/CCF/blob/review/multi-p1-661-four-record-trust-evidence-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-661-FOUR-RECORD-TRUST-EVIDENCE-INDEPENDENT-REVIEW-RESULT-20261116.md) |
| Board gate | live main `310f6ada`: (A) `TrustTables.cs` four status/ref literals plus adjacent comments, (B) this file |
| Claim | [#662 RUNNING](https://github.com/jermur788/CCF/issues/662#issuecomment-6933391942), posted before any edit, with the physical receipt |
| Worker / lane | Claude Code `claude-opus-5-5`, effort low; `jer-laptop` native Ubuntu 24.04 x64; `/home/jer/ccf-worktrees/multi-p1-639-f0-30m`; branch `feature/multi-p1-639-f0-30m-foundation-20261116-v1` |
| Base HEAD | `72efdedc8673186a197756b0fe905157ecae9ae7`; `TrustTables.cs` blob `2ecc1a71…` (unchanged; only this result doc is committed) |
| Candidate | `TrustTables.cs` blob **`b4017188718fe091ca36fe86b5e36a7e96e547f9`** (+18/−6, one file). The patch below re-applies cleanly to the base |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-662-P1-F0-TrustStatusCandidate-20261117/` (21 files; `SHA256SUMS` does not list itself and verifies) |

## 1. Physical custody receipt (read-only, before edits)

- `/media/jer/ZX20/CCF-Ops/board/active.md` and `lanes.json` (16 lanes) contain no row or lane referencing this
  worktree, branch or P1 files. This P1 lane has always run outside the Ops lanes.
- `state/unity` has no active lock file. No Unity, dotnet or strace process was running.
- `lsof +D` on the worktree: 0 processes. The branch is checked out only in this worktree.
- Worktree clean at base; P0 tree `f212c8f6`, P0 status empty.

## 2. The candidate change (exactly the reserved scope)

Four records changed: `oracle-quarter-ha`, `sitka-flat-demo-v1`, `sitka-flat-perf-2000-v1` (original) and
`p1d-30m-sitka-adult-v1` (derived).
- `ReviewStatus.PendingIndependentReview` → `ReviewStatus.Accepted`.
- `ReviewRef` null → the #661 review URL; `ManagerAcceptanceRef` null → the Manager #641 verdict URL.
- Both URLs were checked to resolve to existing documents. The verdict approves exactly these four IDs, identity only.
- The two "PENDING …" comments were replaced with identity-only, no-annual-rights wording.
- Unchanged: every ID, file name, length, hash, snapshot hash, P0 commit and tree, geometry, area bits, purpose, derivation, version and code line.

```diff
diff --git a/Tools/MultiRackSitkaP1Lab/src/Core/TrustTables.cs b/Tools/MultiRackSitkaP1Lab/src/Core/TrustTables.cs
index 2ecc1a71..b4017188 100644
--- a/Tools/MultiRackSitkaP1Lab/src/Core/TrustTables.cs
+++ b/Tools/MultiRackSitkaP1Lab/src/Core/TrustTables.cs
@@ -47,21 +47,29 @@ public static class OriginalTrustTable
 
     // Values prepared by the #639 author from the blobs at P0QualifiedCommit (git cat-file), NOT copied from P0's
     // MANIFEST.sha256. Snapshot hashes are recomputed with P0's own CanonicalJson.SnapshotHash by the test suite.
-    // ALL RECORDS ARE PENDING INDEPENDENT REVIEW AND MANAGER ACCEPTANCE: real inspection refuses them until then.
+    // ACCEPTED, IDENTITY ONLY (#662): independent review #661 and Manager #641 verdict, cited per record. Acceptance covers
+    // the file/snapshot identities only, i.e. read-only inspection. Original P0 fixtures never gain annual or work rights from
+    // it (AnnualGate refuses OriginalPolygonExploratoryV1 regardless), and it implies no modelling calibration.
     public static readonly IReadOnlyList<OriginalTrustRecord> Records = new[]
     {
         new OriginalTrustRecord("oracle-quarter-ha", "oracle-quarter-ha.sitkalab.json", 969,
             "b1691934fa4a41cfd63462f4021811e0871742f0e1f23ecea77fe8da40b5cf62",
             "810030c4187c3132cafb22d46ff87c0b8dbfa43bc382a72463c686f8c554152c",
-            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.PendingIndependentReview, null, null),
+            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.Accepted,
+            "https://github.com/jermur788/CCF/blob/review/multi-p1-661-four-record-trust-evidence-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-661-FOUR-RECORD-TRUST-EVIDENCE-INDEPENDENT-REVIEW-RESULT-20261116.md",
+            "https://github.com/jermur788/CCF/blob/docs/multi-p1-641-661-manager-acceptance-and-662-dispatch-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-641-661-FOUR-RECORD-MANAGER-VERDICT-20261117.md"),
         new OriginalTrustRecord("sitka-flat-demo-v1", "sitka-flat-demo-v1.sitkalab.json", 26675,
             "d0eed68979b2d214555e84a59e80ff6866ccd5e7d1774883e7898fcf8fb4f2ed",
             "b407c725e655dc80ca5fec22f4ad84540136e89928577ad97b3dce31b234f5b1",
-            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.PendingIndependentReview, null, null),
+            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.Accepted,
+            "https://github.com/jermur788/CCF/blob/review/multi-p1-661-four-record-trust-evidence-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-661-FOUR-RECORD-TRUST-EVIDENCE-INDEPENDENT-REVIEW-RESULT-20261116.md",
+            "https://github.com/jermur788/CCF/blob/docs/multi-p1-641-661-manager-acceptance-and-662-dispatch-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-641-661-FOUR-RECORD-MANAGER-VERDICT-20261117.md"),
         new OriginalTrustRecord("sitka-flat-perf-2000-v1", "sitka-flat-perf-2000-v1.sitkalab.json", 324929,
             "d12db11126de2843ff21538f26feaab09242ccaf6abd04e3f25c8014054790df",
             "e4cebf0206c23977abc4b08857d706bdb1c110aa853b08241997081ccfde8726",
-            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.PendingIndependentReview, null, null),
+            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.Accepted,
+            "https://github.com/jermur788/CCF/blob/review/multi-p1-661-four-record-trust-evidence-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-661-FOUR-RECORD-TRUST-EVIDENCE-INDEPENDENT-REVIEW-RESULT-20261116.md",
+            "https://github.com/jermur788/CCF/blob/docs/multi-p1-641-661-manager-acceptance-and-662-dispatch-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-641-661-FOUR-RECORD-MANAGER-VERDICT-20261117.md"),
     };
 
     public static OriginalTrustRecord? Find(string fixtureId) =>
@@ -83,7 +91,9 @@ public static class OriginalTrustTable
 public static class DerivedCatalog
 {
     // The 30 m manifest's file hash and content hash are filled from the generator output and checked by the test suite.
-    // PENDING INDEPENDENT REVIEW AND MANAGER ACCEPTANCE: no ecological year may run until this record is Accepted.
+    // ACCEPTED, IDENTITY ONLY (#662): independent review #661 and Manager #641 verdict, cited below. Acceptance covers the
+    // manifest identity, not ecological calibration. No ecological year runs: AnnualGate still refuses with
+    // AnnualEngineNotImplemented, and any annual engine needs its own approved spec and task.
     public static readonly IReadOnlyList<DerivedCatalogRecord> Records = new[]
     {
         new DerivedCatalogRecord("p1d-30m-sitka-adult-v1", P1Domain.DerivedP1ThirtyMetreExperimentV1,
@@ -91,7 +101,9 @@ public static class DerivedCatalog
             "bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4",
             "b7dee8a34b4e66b4cc443b533604b5962f5fe4c1d2f28832270a0c2b02ba6627",
             DerivedManifest.GeometryModel, DerivedManifest.AreaHaBits, "experiment30m", "designed",
-            ReviewStatus.PendingIndependentReview, null, null),
+            ReviewStatus.Accepted,
+            "https://github.com/jermur788/CCF/blob/review/multi-p1-661-four-record-trust-evidence-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-661-FOUR-RECORD-TRUST-EVIDENCE-INDEPENDENT-REVIEW-RESULT-20261116.md",
+            "https://github.com/jermur788/CCF/blob/docs/multi-p1-641-661-manager-acceptance-and-662-dispatch-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-641-661-FOUR-RECORD-MANAGER-VERDICT-20261117.md"),
     };
 
     public static DerivedCatalogRecord? Find(string id) =>
```

## 3. Post-change digests (candidate; recomputed by the CLI from the changed source)

| Digest | Before (PENDING, base) | After (candidate) |
|---|---|---|
| `OriginalTrustTable.Digest()` | `38572effa59bbc6df09479ab8de6339b4567b27d8f61ad578b51fe5d5264c5ba` | `15dca9f18a4e93dbb3a5df9baaefda08f811476df22622ce396ca24788253b05` |
| `DerivedCatalog.Digest()` | `c507763dc779eae37af8df10eb60690b3f6bcc2048867dbe1b1eaf4463c66646` | `143a253a81818b5867acba87041d3a54618b3d4c4f3d9a5f1dd5b3d1d4cfa6f7` |

The digests change as expected, because each digest includes status and refs. The fixture is unchanged
(`bcaaa5c3…f757c4`).

## 4. Verification with the candidate applied (real exit codes)

**Build:** exit 0, 0 warnings (warnings are errors).

**Existing tests, unchanged: 27/33, exit 1.** The 6 failures are exactly the tests that assert PENDING. These are
expected and are not defects:

| Failing test | Assertion (Program.cs line) | Why it now fails |
|---|---|---|
| derived: pending catalog record refuses normal load… | L171 `ManifestNotAccepted`, L173 `UNTRUSTED-PENDING-REVIEW` label | the record is Accepted, so the load succeeds with `TRUSTED` |
| original: trust records match pinned P0 blobs… | L192 `Eq(PendingIndependentReview, r.Status)` | the status is Accepted (all hash/length/snapshot checks in this test still pass before that line) |
| original: pending trust refuses real inspection… | L198–199 `OriginalTrustNotAccepted` | the record is Accepted |
| original: all three fixtures inspect read-only… | L213 `UNTRUSTED-PENDING-REVIEW` label | the label is now `TRUSTED` (counts and `AnnualAdvanceAllowed=false` still checked before it) |
| annual: original refused, … pending 30 m … | L301 `ManifestNotAccepted` | the compiled record is Accepted, so the gate gives `AnnualEngineNotImplemented` (lines 299/300/302/304/306 unaffected) |
| 644 M-02: … pending status refuses before opening | L352 `ManifestNotAccepted` on `/nonexistent-dir` | Accepted, so it proceeds to open and refuses `DirectoryOpenFailed` |

**Focused CLI checks with the candidate** (all as intended):

| Check | Result |
|---|---|
| `inspect-original <P0 fixtures> <id>` normal (accepted-only) × 3 | exit 0; `TRUSTED`; live 3 / 153 / 1893; `AnnualAdvanceAllowed = False` |
| `verify-derived p1d-30m-sitka-adult-v1 fixtures/derived` | exit 0, `TRUSTED`, catalog digest `143a253a…` |
| `generate-manifest --check fixtures/derived` | exit 0, identical (`bcaaa5c3…`) |
| `annual OriginalPolygonExploratoryV1 …` (both IDs) | exit 2 `OriginalAnnualRefused`: accepted status gives original P0 no annual rights |
| `annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1` | exit 2 `AnnualEngineNotImplemented`: no year possible |
| `annual DerivedUnitySquareParityV1 …` | exit 2 `DomainUnavailable` |
| Accepted record, mutated byte (scratch copy) | exit 2 `FileHashMismatch` |
| Accepted record, appended byte (scratch copy) | exit 2 `FileSizeMismatch` |
| Accepted record, symlinked file (scratch) | exit 2 `SymlinkRefused` |
| Unknown ID (wrong case), derived ID via original import, original ID via derived | exit 2 `UnknownOriginalFixture` / `UnknownOriginalFixture` / `NotDerivedFixtureId` |
| Mutated derived fixture (scratch copy) | exit 2 `FileHashMismatch` |
| P0 | tracked digest `9e218ca7…` before = after; P0 status empty |

Not run, as the packet requires: strace or L4, Host, saves, Unity, Windows. Default .NET **L4 FAIL with 38 unresolved
remains open**. The first ecological year stays on HOLD.

## 5. Why BLOCKED, and the exact unblock

The packet says: "If existing tests expect statuses PENDING, STOP and report test adaptations needed; no out-of-ceiling
test edits or weakening." `tests/Core.Tests/Program.cs` is outside the #662 ceiling. So the source change is **not
pushed**: pushing it would leave the branch with 6 red tests. The base `TrustTables.cs` is restored, and the candidate
is preserved as the patch above, plus the blob ID and evidence copy.

**Requested from Manager #641:**
- Reserve **`Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs`** for the same writer, for these 6 adaptations only.
- Then the same writer pushes the candidate (blob `b4017188`) together with the test adaptations as one commit, for independent changed-lines review.

The proposed adaptations keep every refusal path covered by using a **test-local pending record**: `InspectOriginal`,
`LoadDerived`, `ReadDerived` and `AnnualGate.Check` all already accept a caller-supplied table or record. Nothing is
weakened.

1. L171/173 → the compiled derived record loads with `RequireAccepted` and label `TRUSTED`. A `rec with { Status = PendingIndependentReview, ReviewRef = null, ManagerAcceptanceRef = null }` test catalog still refuses `ManifestNotAccepted`.
2. L192 → assert `Accepted` and both exact reference URLs per record (all hash checks unchanged).
3. L198–199 → the same two refusals using a test-local **pending** copy of the oracle record (including refusal before opening `/nonexistent-dir`). Add a positive normal inspection of the compiled accepted record.
4. L213 → expect `TRUSTED` for compiled records; the counts and `AnnualAdvanceAllowed=false` stay.
5. L301 → `ManifestNotAccepted` using a pending copy of the record; the compiled record now gives `AnnualEngineNotImplemented` (already asserted at L304 for an accepted copy).
6. L352 → `ReadDerived(…, RequireAccepted, pendingCatalog)` on `/nonexistent-dir` still gives `ManifestNotAccepted` before opening.

Alternatively, the Manager may rule differently (for example a separate test-owner task). This writer makes no test
edit without that reservation.
