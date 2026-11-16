# Multi — independent source/security review of P0 save-response correction (#586)

**Date:** 16 November 2026. **Verdict: APPROVED FOR SOURCE/SECURITY ONLY** on the exact #567 candidate. **Not runtime-qualified by this reviewer; not integrated; no P1 implementation-base selection.** The reviewed change correctly separates a successful rename-committed write from an optional later save-list refresh for the trusted production file-access implementation. The approval is not a claim of power-loss durability or successful HTTP delivery under all networking failures.

## 1. Independence, lane, authority and restrictions

- **Actual nonauthor reviewer:** OpenAI ChatGPT GPT-6, separate GitHub-only conversation, not Claude Opus 5.5 (the #567 source author). Effective thinking tier not exposed; no claim of High effort.
- **Connected GitHub account:** jermur788; execution: ChatGPT cloud/GitHub connector, source-only. **No local worktree exists in this reviewer environment.** Proposed /media/jer/Files/CCF/Operations/CCF-P0-567-IndependentSecurityReview-20261116 was **not created, mounted or verified**. No local git-worktree, process, disk, lease or dirty-state verification is claimed.
- **Review branch and starting source HEAD:** review/multi-sitka-p0-567-save-response-security-20261116-v1, initially a7f41983c2f2f233b8fe999d1a5b10e72bdd2866. Review started after reading main's live active-tasks and #586, verifying this remote branch tip, and posting an actual [RUNNING] claim on #586.
- **Candidate:** fix/multi-sitka-p0-save-response-r3-20261116-v1 at a7f41983c2f2f233b8fe999d1a5b10e72bdd2866. Accepted earlier P0 README-only base: 861b81d3540429d241bba0732a610212626f6828; the original code change is 046838b06e500258de45db1ac98bf508421dd96d.
- **Scope:** standalone 2D Sitka lab only; no Unity, Scenario One, adult-growth/P1, user-save modifications, Windows build, WSL2 operations or production source edits.
- **Write allowlist:** this one new report only; status/notification comments to existing GitHub issues are administrative, outside the source tree.

## 2. Source change boundary and independent checks

The GitHub compare from the pinned qualified P0 baseline to #567 has **exactly three changed paths**: MODIFIED Tools/MultiRackSitkaLab/src/Host/LabHost.cs (+26/−10); MODIFIED Tools/MultiRackSitkaLab/tests/Host.Tests/HostTests.cs (+133/−4); ADDED Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-Accurate-Post-Save-HTTP-Response-Repair-20261116.md. No Core, NoFollowFiles, UI, Program, schemas, fixtures or game sources are changed.

**Independently observed, non-executable GitHub source checks (11/11 pattern/structure assertions matched):** production Program does not set FileAccess; Build passes the optional in-process seam into LabState; native rename precedes success; no native stage hook runs after rename; save and list have separate catches; success returns fileName and list status; UI guards null files; test-only AfterWrite can throw; both deterministic post-commit and the pre-commit tests exist; prior Host/Origin/CSRF Guard block is byte-identical to baseline. The Host test source contains 16 annotated test methods. These are **static source checks, NOT a .NET build, executed unit suite, mutation suite, network attack run or independent dynamic validation**.

**No independent executable tests were run.** The reviewer environment has Git and Python but no dotnet executable, no cloned isolated repository and no mounted local operator evidence directory. There are no GitHub workflow runs or combined statuses for this candidate in the checked connector result. The author's native-Linux logs were described in the published handoff but the local log folder was not independently inspected.

Key immutable source views:
- [Host save handler, response and guards](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/src/Host/LabHost.cs#L16-L29)
- [Host save endpoint](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/src/Host/LabHost.cs#L198-L254)
- [Core store](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/src/Core/LabDataStore.cs#L94-L147)
- [Native atomic write](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/src/Core/NoFollowFiles.cs#L143-L173)
- [Negative Host tests](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/tests/Host.Tests/HostTests.cs#L440-L537)
- [Author handoff/test claims](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-Accurate-Post-Save-HTTP-Response-Repair-20261116.md)

## 3. Independent review gates A–F

### A — trusted save commit truth: PASS for default production implementation

LabDataStore.WriteSave validates the 1–64-character bare base name, checks the root and existing final entry, then calls ILabFileAccess.WriteAtomic (LabDataStore.cs:129–147). The real implementation delegates to NoFollowFiles.WriteAtomic. That method requires Linux no-follow support, opens one no-follow directory descriptor (148), creates a uniquely named temporary file relative to that descriptor (152–154), writes and checks close errors (159–161), invokes any *internal pre-rename* stage hook (162), then renameat from that descriptor to the final name (163), and only then sets renamed=true (164). Its final cleanup and directory close ignore return results (166–172). Nothing in the default RealLabFileAccess path after successful rename creates a managed failure before WriteSave returns the already-known bare file name.

The source therefore supports **rename-committed within the originally opened directory** only. It does not guarantee current path reachability after a concurrent directory exchange, delivered HTTP response, file/directory fsync, power-loss persistence or independent WSL/arm64 success. The earlier one-try baseline had the optional ListSaves inside the same expression as WriteSave (old Host.cs:213–219), explaining the false 403/500 shape.

A pre-rename LabException is still handled as a refused save; unexpected pre-commit exceptions still reach the unchanged Guard generic 500. Note minor boundary qualification S-1 below concerning injectable nonproduction implementations.

### B — post-commit exception scope, error honesty and leakage: PASS with scope caveat

Host.cs:214–235 has a bounded first try around WriteSave/serialization and a **separate** try around **only** st.Store.ListSaves. The second catches LabException as listCode=ex.Code and other Exception as the literal opaque ListFailed. On an exception files remains null, listRefreshed is false, and success is 200/ok:true only after WriteSave has returned. The status is not taken from a later path-based listing failure. The response never uses ex.Message or stack text for this listing error. The normal Core file-access paths throw known fixed LabException codes (LabDataStore.cs:74–147; NoFollowFiles.cs:178–189), so the exposed listCode is not a path/exception in the standard bootstrap.

The broad Exception catch includes programming/IO/resource/cancellation exception subclasses if actually emitted by the listing; this is a conscious choice to keep an already committed write from being reclassified. The real path-based, synchronous listing has no cancellation token and the catch surrounds no write action. This does not weaken GET /api/saves, which still returns refusal/error independently. It can reduce diagnostic observability and does not prevent failure during construction/serialization of an HTTP response; those are ordinary transport/host failures, not proof that the file was uncommitted. A malicious **in-process** injected ILabFileAccess could throw a LabException with an arbitrary Code, because LabException's constructor accepts any string (Models.cs:177–180); in the trusted standard path the code vocabulary is internal. No remotely controlled injection route was found.

### C — public test injection seam and threat model: PASS for current distribution

Host.cs:16–29 makes LabHostOptions.FileAccess public and nullable; LabState(string, ILabFileAccess?) passes it to the existing public LabDataStore constructor (Host.cs:41–49, 65–68). The actual production Program.cs:20 constructs options from FindToolRoot and port only; **it does not set FileAccess**. No HTTP request body, query, route or server registration selects a file-access object. Host binds 127.0.0.1, and the previously established Host/Origin/CSRF/request guards at Host.cs:99–142 are unchanged.

The trust boundary is **code running inside the server process**, not the HTTP client. A local untrusted assembly with execution privileges could already use its process filesystem permissions; adding this public constructor does not expose an HTTP injection endpoint. The larger public API surface is not ideal, but a deliberate test seam used by an isolated Host test assembly is acceptable here, rather than reopening the author's reported internal/InternalsVisibleTo warning collision without proof that a safer testable alternative builds. Do not treat this seam as a general untrusted-plugin extension contract.

### D — negative tests, determinism and race: SOURCE COVERAGE PASS; EXECUTION NOT INDEPENDENTLY VERIFIED

HostTests.cs:68–85 defines HookedFileAccess delegating to RealLabFileAccess. BeforeWrite is one-shot before the real call; AfterWrite is one-shot **after** the real call. The hooks inject actual directory manipulations around real file operations, not mocked success data.

- Lines 459–488: AfterWrite performs renameat2(RENAME_EXCHANGE) of a real lab directory and a symlink to an alternate sentinel folder. Source assertions require 200/ok:true, correct fileName, listRefreshed:false, SymlinkRejected code, files:null, no path/exception body; GET /api/saves still 403 while swapped; after swap-back alternate is untouched, the real directory contains exactly one committed file and HTTP load succeeds.
- Lines 490–508: after a real commit the directory is moved and its path replaced by a plain file. The next list throws an ordinary exception, which must give 200 with opaque ListFailed and no leaked path, then the original folder is restored with the file.
- Lines 510–537: BeforeWrite exchanges the folder before the actual no-follow open. Expected response is 403 SymlinkRejected, no success-file metadata and no writes in either directory; an undisturbed control must then save and refresh.
- Lines 386–438: older 3-second concurrent exchange remains a stochastic supplementary control. Its checks demand only 200:ok, 403:SymlinkRejected or 500:WriteFailed, zero alternate overwrites, zero wrong success filenames and zero temp orphans. It does not replace deterministic tests. Exact swap and response counts cannot be inferred from source.

The tests *would* distinguish the baseline one-try false 403/500: the old handler puts both operations inside one catch; however RED/GREEN observations are author-only without independent execution. The source test itself does not directly inject a throwing AfterWrite callback or a replacement by a **different ordinary directory** (residuals S-1/S-2).

### E — API, UI, controls and save safety: PASS for changed source; conditional platform limits

Status mapping on precommit store refusals is unchanged (Host.cs:174–184). GET /api/saves retains its independent failure path (198–202). The baseline Host/Origin/CSRF, method, content-type, loopback and response-header Guard block is identical in the checked source. There are no changes to the byte schema, save names, fixture rules, Core access, Program or UI. The successful POST still has ok, code, message, fileName and files; it adds listRefreshed and listCode, with files:null on failed refresh. The existing app.js:355–359 performs if(r.files) fillSaves(r.files), then displays say(!!r.ok, r.code, r.message). Thus the UI does not crash on null and explicitly displays the saving outcome, though it retains previous dropdown choices until a later successful refresh.

Bare file naming and directory-anchored writes are still enforced by the unchanged Core paths. A rejected symlink does not yield an unvalidated successful list; alternate-directory overwrites and temp-orphan absence are asserted by the test source and author-reported executions, not independently runtime-certified here.

### F — evidence, platform and scope: PASS for documentation honesty; runtime gate still open

**Author-reported only:** RED baseline 13 passed/3 failed; after fix Host 16/16, Core 82/82, schema check exit 0, headless Chromium 15/15, existing mutation controls 35/35, six specific X1–X6 mutants caught. Three-second swap sample reported 329,228 exchanges, 2,740 403 responses, 92 200 saves, zero alternate overwrites and 64 successful saves with failed optional list refresh. Those are author observations from native Linux x86_64, .NET SDK 8.0.133, **not reviewer executions**. The source tests contain assertions consistent with the claims, but the exact local output logs, temporary mutation scripts and scratch state were not available through this GitHub-only lane.

**Not qualified here:** compilation or live endpoint tests on the reviewer host, Windows Edge/Ubuntu WSL2, Linux arm64, Chrome, file/directory fsync or crash/power-loss persistence. The earlier P0 baseline's WSL acceptance must not be silently inherited by this changed source.

## 4. Nonblocking findings / residual risks

**S-1 — MINOR / boundary-documentation issue, not production-path blocker.** Host.cs:220 says any WriteSave failure happens before rename. This is true for the default native implementation, **not for arbitrary injected ILabFileAccess**. In HostTests.cs:79–84 the AfterWrite callback executes after the real atomic rename; if that callback throws, WriteSave does not return, and the first catch/Guard can report 403/500 despite the real commit. There is no such callback in production Program.cs; no HTTP client can create one. **Minimum follow-up if the test seam ever becomes a supported extension:** document a no-throw-after-commit ILabFileAccess contract and add a focused injected-exception negative case, or constrain the seam. Do not generalise the Host.cs comment to arbitrary injected providers. This is outside the current trusted runtime fix.

**S-2 — MINOR / pre-existing identity-semantic limitation, not escape.** A same-user process can replace the lab path with another **ordinary directory**, not just a symlink. Because ListSaves reopens the path independently (LabDataStore.cs:95–105), it may validly return files from that *new* directory and set listRefreshed:true even though the committed file is in the original descriptor-anchored directory. That boolean therefore means **the current path was enumerated successfully**, not **the committed directory was positively reidentified**. The symlink negative covers refusal, not an ordinary-folder identity exchange. No alternate write follows from this and the pre-existing path-based GET/load interface has the same identity limit. **Minimum follow-up if identity-at-response becomes a requirement:** add a real-directory exchange test and a design decision for directory identity/reachability; do not promise committed-directory identity under the current response contract.

**S-3 — NIT / diagnosability.** The generic post-list catch has no server-side diagnostic record (Host.cs:232), so operational reasons for ListFailed can be difficult to troubleshoot. No exposure of details to HTTP is justified; any opt-in internal logging requires a separately scoped privacy/security review. No need to change for this fix.

**Blocking findings:** 0. **Major findings:** 0. **Minor findings:** 2. **Nits:** 1. These are independently reasoned source caveats, not observed exploit executions, and do not alter the approval boundary.

## 5. Verdict, next gate, and explicit non-claims

**APPROVED — independent source/security review of #567 for the trusted production host code path only.** The code separates a completed native rename from the optional path-based listing, keeps refusal checks, preserves HTTP/UI compatibility, and introduces no observed remote test-hook attack surface. It does **not** imply local/running verification, independent mutation validation, final P0 requalification, integration, Manager acceptance, P1 base selection, save durability or a Windows build.

**Next decision belongs to the Manager:** inspect this nonauthor result and decide whether the author’s native Linux test evidence is enough for source acceptance and whether a bounded Windows Edge/Ubuntu WSL2 regression is mandatory before wider P0 qualification/re-pin. Preserve independent product/runtime gates; no automatic merge or repin. #586 may close as HANDED-OFF because the asked-for source-only independent verdict is published, while the P0 requalification status remains open to the Manager. User action: **none for this source review**. Dashboard ingestion and Google Drive synchronization: **not verified**.
