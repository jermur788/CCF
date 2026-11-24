# 2D Phase 1 A1 — Manager Decision and Physical Ops Custody Readback Request

**Issued:** 2026-11-24 after DeepSeek A1 admission preflight.  
**State:** **A1 SOURCE START BLOCKED — PHYSICAL OPS REGISTRY CUSTODY UNKNOWN**.  
**Overall Manager decision:** **ROUTE A REQUESTED — obtain an attributable, real Ops-host readback. NO physical-custody waiver issued.**  
**Recipient:** current custodian with real access to the original Ops host and established `CCF-Ops/lanes.json` registry; not the DeepSeek orchestrator on `DESKTOP-DL1BQI7`.  
**Difficulty:** LOW read-only verification; high consequence if custody is guessed.  
**No source writer, branch or physical A1 worktree is admitted by this handoff.**

## Evidence and current authority

- [DeepSeek blocked-admission handoff](https://github.com/jermur788/CCF/blob/c26bf61270c08a0abef0c20af77c31f70fb0791a/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-A1-ADMISSION-BLOCKED-PHYSICAL-OPS-CUSTODY-20261124.md): exact Map Pass2 `4b6d8070fe709f8b6600baedd39bb37d0cf5322d` and A0 `8db5e09538474cec01374066be08d73f240de82c` present; reviewed additions noncolliding; all inspected Map/A0 worktrees clean; no local source runner or A1 branch/worktree. Physical Ops ledger and tools **not available** on WSL2 host `DESKTOP-DL1BQI7`.
- [Map Pass1+2 owner-accepted unmerged closure/release](https://github.com/jermur788/CCF/blob/60d0c29d60840ef7e46687f94dbbf682806a749b/Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass2/MULTI-MAP-INTEGRATION-PASS2-CLOSURE-AND-SHARED-FILE-RELEASE-20261124.md): map source paths released; prior physical Ops registration was unavailable to that host. This does **not** establish the absence of a historical holder in the separate Ops registry.
- [Manager conditional G4 A1 source-scope grant](https://github.com/jermur788/CCF/blob/c0537748f63077e99efcbe966cb4472a4e4299de/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-A1-MANAGER-G4-CONDITIONAL-SHARED-FILE-GRANT-20261124.md) explicitly **holds source start** until real operational custody is verified. It remains approved in all other respects.
- [Live task coordination](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md) is Manager-controlled, not an authoritative replacement for the physical Ops registry. The coordinator correctly did **not** register RUNNING.
- [Historical OPS-UNITY-LOCK-01 task packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/OPS-UNITY-LOCK-01/TASK-PACKET.md) identifies the machine-local registry as `/media/jer/ZX20/CCF-Primary/CCF-Ops/lanes.json` and normal established Ops/lock tooling, but the path and schema **must be confirmed by the custodian on the actual host**. That packet was a Unity-lock repair task and does not itself confirm live source-only lane custody.

## Required single read-only Ops-host action

The authorised physical Ops custodian should, **on the host holding the established authoritative registry**, perform **only read/status operations** with the normal documented tooling, without editing state or acquiring a Unity/source lock for this inquiry.

Record and return:

1. **Host and authority:** actual hostname, custodian identity/role, UTC timestamp, verified registry path/real mount identity, normal status tool available. Do not paste secrets or unrelated credentials.
2. **Live lane inventory relevant to A1:** exact IDs for any historical/current **Map Pass1, Map Pass2, Phase 1 A0, prospective A1, or broad `Tools/MultiRackSitkaP1Lab/**`** reservations; each recorded owner and status, exact file/worktree/branch scope, and any existing concurrent claim. If none exist, state explicitly `NO MATCHING ENTRY` from the real registry rather than inferring it.
3. **Authority of holders and release:** if a prior claim persists, give its named holder and **documented safe release/handoff** status. Do not expire ownership based on elapsed hours; D-091 disallows arbitrary time expiry and forced auto reclaim. Do not overwrite or remove holder records. If another holder remains, A1 stays blocked until legitimate release.
4. **A1 eligibility:** whether normal Ops policy permits a **new dedicated, single-writer, .NET/C# source-only, non-Unity lane** on `DESKTOP-DL1BQI7` with no Unity/serialized asset edits. State specifically whether Ops registration must happen on the Ops host, who can register it, and what exact normal receipt will be returned. A readback proving no conflicting claim is **not itself** a registration/lease if policy requires a separate grant.
5. **Readback receipt:** save a short GitHub Markdown or issue comment with the actual registry/status proof (redacted to relevant rows), timestamp and custodian; return its **direct permanent GitHub URL** to the Overall Manager/DeepSeek. This is documentary evidence of read-only status, not permission to change the registry.

**Do not** run `unity-lock acquire`, use `--force` or bypass flags, kill another lane's servers, launch Unity, create/edit a second `lanes.json`, write production code, edit `main` source, or touch any existing Ops holder. If the real registry/mount is broken or unavailable on its own host, return that exact blocker for a separately scoped Ops repair; do not invent a local replacement.

## Admission decisions following the result

- **A — conflict-free and normally reservable:** after receipt, DeepSeek reruns the A1 admission preflight. The duly authorised local/physical custodian registers the single writer/branch/worktree and obtains whatever real Ops receipt is required. DeepSeek updates live GitHub coordination to RUNNING **only on actual admission** and begins source work within the already issued conditional G4 packet; no new product-design approval is needed.
- **B — conflicting active holder or unknown release:** keep A1 HELD. Obtain specific documented release from the real holder, not automatic stale takeover. Continue read-only A1 planning/review if useful; do not write source.
- **C — physical registry inaccessible or no safe normal registration path:** keep A1 HELD. Escalate one bounded owner decision regarding a source-only exception, or a concrete Ops-host access/repair action with effort and consequence; **no implied waiver** exists. Any exception must be owner-approved explicitly and recorded as an amendment before source launch.
- **Phase B:** does **not** inherit an A1 source grant or any future source-only exception. Separate branch/owner/physical lane and UI integration approval remain mandatory.
- **3D/Unity:** the source-only A1 task neither needs nor receives a Unity machine-wide lock or permission to bypass existing Unity lock rules. No Windows build.

## One complete handoff for Ops custodian

**Purpose:** establish actual physical registry custody for A1, not start implementation.  
**Provider:** named Ops-host-capable custodian (UNKNOWN until assigned).  
**Lane:** **read-only status inquiry**; no new local writable worktree or Git branch/HEAD.  
**Source context:** A1 proposed source `4b6d8070...` plus A0 `8db5e095...`, no A1 branch.  
**Granted writes:** none to Ops registry or source; only a new durable GitHub receipt, on a separately authorised coordination/docs lane if needed.  
**Must stop on:** registry inaccessible, unclear holder, conflicting claim, incompatible mount identity, permission failure, unexpected dirty state.  
**Checks:** live host path/registry and exact claim inventory/status with timestamp; no Unity run, no local build.  
**Owner action:** route this request to the actual Ops-host custodian. No owner approval to waive is being requested unless Route A proves unavailable.

**Verdict:** `A1 PHYSICAL ADMISSION HELD — ROUTE A CUSTODY READBACK REQUESTED; NO WAIVER`.
