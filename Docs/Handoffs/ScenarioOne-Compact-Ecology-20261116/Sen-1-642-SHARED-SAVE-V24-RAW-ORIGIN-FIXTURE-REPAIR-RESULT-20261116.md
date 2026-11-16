# Sen-1 #642 — Shared-save v24/raw-origin fixture-repair result

**2026-11-16 — BLOCKED / NOT RUNNING. No fixture repair performed.** The required prospective exclusive verification-file ownership has not been recorded. This is the requested blocked Markdown handoff, not an implementation or verification PASS.

## Actual worker and registration

- OpenAI **gpt-6.1-sol High**, Codex harness through T3 Code, configured Codex provider, Linux host `jer-laptop`.
- [Actual BLOCKED registration](https://github.com/jermur788/CCF/issues/642#issuecomment-6927967161) precedes documentation publication. No fixture [RUNNING] claim or ownership is asserted.
- Complete [binding #640/#642 packet](https://github.com/jermur788/CCF/blob/docs/sen-1-640-638-verdict-642-fixture-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-640-MANAGER-638-APPROVAL-AND-642-FIXTURE-REPAIR-PACKET-20261116.md) and current #642/#544/#638/#640 claims inspected read-only.
- Actual session anchor: `/media/jer/Files/CCF-Primary`, `ops/workflow-foundation-v1` at `46247486b37904f6b36825bb40fb6ed7bea35e88`; original dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` preserved.
- Source checkout inspected read-only: `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115`, branch `feature/scen1-shared-three-profile-save-v2`, local and fresh remote **`410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35`**, clean including untracked files.
- New isolated fixture worktree and proposed `verify/sen-1-642-save-v24-raw-origin-fixtures-20261116-v1` branch **NOT CREATED** while custody is unresolved.
- Only publication lane: remote documentation-only `docs/sen-1-642-blocked-ownership-preflight-20261116-v1`, starting at `410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35`. No local worktree used for publication. Sole changed repository path is this Markdown; the shared-save branch and main remain untouched.

## Blocking ownership evidence

At `2026-11-16T20:13:25.438321+00:00`, refreshed authoritative `main:Docs/Project/coordination/active-tasks.md`, main `5cefe7242d2f4e3f5ee9dd7b6a3e70a34f623494`, content blob `812031bbaf13bcd2a98b85ee9cbc932b1532aeff`. It contains **no #642 ownership/reservation entry**. Re-fetched before declaring the reservation absent. #642 has the prepared READY/NOT RUNNING packet, not a later exact-file custody grant. #640's current issue comments explicitly retain the pending prospective #425-to-#640 main-board transfer. Current recorded board custodian is #425; this worker cannot write that record or infer a transfer.

Physical `/media/jer/ZX20/CCF-Ops/board/active.md` and `lanes.json` likewise contain no #642 registration. Board SHA-256 `80bb872a00ac25b2ef2e9a96b4c7d721eb1e462fc5235a45149b9e3d8ebdc416`; lanes SHA-256 `1b6d70eb1a884d21dffc1e9d92db20a57cb728dd7a534aeec52ab8386cecaf21`. No physical Ops mutation or lease was attempted. The former #638 reservation covers only three production geometry paths and does not confer ownership over these verification files.

**Required next ownership action:** #425, or #640 after an explicit prospective board transfer, records a single exclusive #642 fixture writer and the exact inspected verification-file set, with no active overlap. The dispatch expressly says not to assume this permission; no source edit was made.

## Read-only fixture findings and proposed initial file scope

| Inspected tracked path | Observed source mismatch; no runtime reproduction |
|---|---|
| `Tools/Verification/StandGeometry/StandGeometryVerification.cs` | At line 123, capture assertion requires both captured version and CurrentVersion to be 23; line 133 checks JSON version 23, and line 162 expects rewrite version 23. Actual production `Assets/ForestPrototype/ForestSaveData.cs:7` declares CurrentVersion 24. Legitimate current-roundtrip inputs at line 147 and synthetic old-layout input at line 159 are parsed DTOs passed to LoadData. Malformed-geometry DTO refusals can stop at provenance instead of proving geometry admission refusal. |
| `Tools/Verification/ScenarioReferenceVerification.cs` | Year-50 continuation at line 505 clones a typed archive DTO and passes it to LoadData; a similar typed clone exists at line 354. Intended continuation/tree/history assertions remain present. These call sites need provenance-aware inspection, not wholesale replacement of all JsonUtility clones (hash-only clones have different intent). |

Production `ForestSaveController.LoadData` routes through `RestoreCapturedData` / `TryPrepareCapture` (lines 597-633). That path checks original captured object identity, selected context/profile/path and unchanged serialized bytes before preparing admitted data. Parsing/cloning a DTO does not create a trusted same-session capture. This is the existing production contract, **not a defect to bypass for a test**. Raw-origin loading must use the already approved admission path; no fabricated registration/provenance is authorised.

The geometry fixture's `V19Layout(start)` is derived from current captured data. It can be a clearly labelled layout regression input, but must not be described as a genuine archived old save. A future repair must preserve that distinction and use genuine captured/archive evidence for claimed historical compatibility; no old fixture was loaded or fabricated here.

These two tracked verification paths are the **proposed initial exact ownership set**, not a granted allowlist. Any necessary helper/test-documentation expansion must be inspected and reserved before editing; no guessed production path or blanket verification-subtree ownership is assumed.

## Actual checks and results

- Read-only source/remote HEAD/branch/cleanliness checks: PASS at the pinned source base.
- Authoritative ownership gate: **FAIL / ABSENT**, therefore stop before source mutation.
- Existing #638 outcome inspected: Manager accepted source-only incorporation, issue CLOSED; its reviewed three-file +29/-1 change and separate result remain on the unmerged shared-save line.
- Verification fixture files changed: **0**. Production files changed: **0**. Compiler/runtime/fixture tests run: **0**. New positive/negative behavioral results: **NONE**. No assertion was weakened or claimed repaired.
- No native old-save, Reference continuation, world, disk, movement, runtime or protected-hash qualification is claimed. No fixture failure was reproduced in Unity or a C# runtime during this blocked preflight.
- This publication adds **one Markdown only** on a separate remote docs branch. Exact commit, compare and content readback are published on #642 after creation, with local receipts.

## Resume criteria and remaining gates

After exclusive ownership is actually recorded, the nominated worker can recheck the shared-save target/head and current claims, create a clean new isolated Linux fixture worktree/branch, and register actual provider/model/worktree/branch/start HEAD **[RUNNING] before fixture edits**. Repair current-schema expectations and positive input origins while preserving genuine old-save/Reference identities, typed/clone provenance refusal controls, malformed/foreign/unknown geometry controls and exact pre-mutation refusal checks. If genuine allowed raw saves fail or Reference anchors differ, stop for Manager decision; do not edit production or rebaseline.

Then run proportionate reproducible offline checks, explicitly distinguishing fixture/source coverage from Unity execution and reporting real counts/exceptions/compile applicability. Later native qualification still needs separate fixture review and #548 deployed/self-test-green lease plus authorised clean exclusive Linux Unity lane/Library/Prefs/isolated saves and exact-candidate permit. The packet's legacy and frozen Reference gates remain outstanding. No new user product decision is requested to resolve the ownership registration.

## Preservation and operational handoff

No source worktree switch, new source branch, fixture/production edit, main/shared-save push, source integration, Unity executable/lease, save/Prefs access, Compact admission, asset/scene/package change, protected Reference hash change or Windows build. The original two dirty anchor files and physical Ops files remain unchanged; fixture file hashes retained for final comparison.

Evidence: `/media/jer/Files/CCF/Verification/ScenarioOne642OwnershipPreflight-20261116T201300Z/` (`preflight.json`, fresh main snapshot, relevant issue snapshots, BLOCKED registration, this report and publication/readback receipts).

**Status remains BLOCKED / NOT RUNNING; repair not complete. DASHBOARD/DRIVE NOT VERIFIED.** #425/current authorised custodian must record the file reservation and subsequent status; this worker did not edit live coordination, physical Ops or generated dashboard state. The documentation handoff can be reviewed while the fixture task stays open.
