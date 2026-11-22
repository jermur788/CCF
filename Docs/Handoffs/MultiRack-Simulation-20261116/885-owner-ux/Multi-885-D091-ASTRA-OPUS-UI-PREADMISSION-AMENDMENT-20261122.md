# Multi #885 — D-091 pre-admission AMENDMENT: tool-chosen lane path, one compile, one bounded /multi runtime

**AMENDS** the receipt `Multi-885-D091-ASTRA-OPUS-UI-PREADMISSION-RECEIPT-20261122.md` (commit `ce3417fb`). Custody only: nothing was launched, built, edited or created by the custodian.

Issued **2026-11-22T09:28Z** (observed clock `2026-11-22T09:28:02+00:00`).

Canonical URL: https://github.com/jermur788/CCF/blob/docs/multi-885-astra-d091-amendment-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-D091-ASTRA-OPUS-UI-PREADMISSION-AMENDMENT-20261122.md

## 0. Design publication verified (this closes the original receipt's stop condition)

| Check | Verified |
|---|---|
| Design branch | `docs/multi-885-astra-final-forest-ui-20261122-v1` @ **`305582e2c6edd08f2aece72ed85dd93e5bdb9640`** |
| Diff vs `main` | ahead 1 / behind 0, **six files, all under `Docs/`** — docs-only confirmed by path |
| Design document | `…/885-owner-ux/Multi-885-ASTRA-FINAL-DESIGN-AND-OPUS55-IMPLEMENTATION-PACKET-20261122.md`, blob **`1ea7d8daef3ca20ebbb7ce6c8a7791048b15447e`**, 30 410 bytes, read back |
| Bundle | one 96-line design HTML + four PNG references in the sibling `astra-final-design-20261122/` directory |
| Consistency with this custody receipt | packet names the same baseline `a692e1bf`, the same three literal interiors, the same single result filename, the same masking-script-as-code-block rule, and defers lane branch/path to "the exact branch/path assigned by the fresh D-091 custodian receipt" |

The design stop condition in the original receipt is therefore **satisfied**. The packet's own non-claims stand: design chosen, not implemented, not runtime-verified, not owner-accepted; its HTML is a design render, not production code, and its hardcoded trees/geometry must not be transplanted.

## Technical blocker check: NONE

Before granting, I checked whether the requested rights are technically possible on this host:

| Requirement | Verified |
|---|---|
| Pinned SDK | `/home/jer/.dotnet/dotnet` **8.0.134**, runtime 8.0.34 (system PATH `10.0.115` remains forbidden) |
| Package acquisition | baseline `src/Ui/SitkaP1.Ui.csproj` has **one `ProjectReference` to Core and zero `PackageReference`**; `src/Core` likewise — no package restore is expected; `~/.nuget/packages` does not exist, consistent with that |
| P0 as a binary | baseline `SitkaP1.Core.csproj:8,16-19` references `$(P1Root).p0ref/SitkaLab.Core.dll` as a **binary Reference**, and target `VerifyP0CoreReference` runs **BeforeTargets ResolveAssemblyReferences** — the build itself is a hash guard |
| Prebuilt P0 available | integration lane `.p0ref`: pin **`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`**, mode `r--r--r--` |
| Ephemeral listener | `src/Ui/Program.cs` uses `Listen(IPAddress.Loopback, 0)` |
| Evidence path | `/home/jer/Development/CCF-885-Astra-UI-Evidence-20261122` **absent** (free) |
| Limit | ordinary UI compile in a fresh lane is feasible and bounded. No blocker exists |

## 1. Amendment — lane path is decided by the tool, not pre-reserved

`t3_worktree_handoff` exposes no path argument. Therefore:

- The **reserved path** in the original receipt is **withdrawn as a requirement**. It becomes, at most, a naming suggestion.
- The **lane is defined by identity, not by path**: branch `feat/multi-885-astra-opus-ui-20261122-v1`, HEAD **exactly** `a692e1bf5d21304d06e696db1f47d1988ff80ee1`, created by `t3_worktree_handoff` with `startFromOrigin:false`, `runSetupScript:false`.
- Astra **records the returned `worktreePath`** and verifies, before dispatching the child: HEAD equals the baseline; `git status --porcelain --untracked-files=all` is empty; the path is **not** inside any existing lane (OwnerUX, demo, integration, author, reviewer, #877, #882, #867, or any other registered worktree); exactly **one** lane is created. If any check fails → stop.
- The recorded path is the lane for all purposes of the original receipt; the child may not create a second checkout by shell.

## 2. Amendment — ONE ordinary UI compile is GRANTED

Previously prohibited; now permitted **once**, in the new lane only:

- Command: `/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` from the new lane's `Tools/MultiRackSitkaP1Lab`, with `TMPDIR` set to the external evidence directory. Pinned SDK only.
- Scope: this necessarily builds `SitkaP1.Core` transitively. Output confined to the lane's own `bin/` and `obj/`.
- **P0 stays binary and read-only**: copy the three files from `…/CCF-Multi-885-Integration-20261121/Tools/MultiRackSitkaP1Lab/.p0ref/` into the new lane's own `.p0ref/`, re-hash the **destination** (`9a99f781…`), `chmod 444`. **Forbidden:** `build/build-p0-core-ref.sh`, `tests/p0-isolation.sh`, any cold isolation or P0 regeneration.
- Restore: no packages are referenced, so no package acquisition is expected. **If the build attempts to acquire any package or reach the network, stop and report** — a build may use only the pinned SDK's bundled components. No new dependency, no new tool, no `dotnet` workload, no test-suite run.
- One compile. A second attempt after a real defect is permitted **only** for the same defect and must be recorded; a clean rebuild loop is not.

## 3. Amendment — ONE bounded fresh `/multi` runtime is GRANTED

- One process, started from the lane, loopback **OS-assigned ephemeral port**; record endpoint, PID, session and source SHA.
- Purpose: the **affected UI checks** — the `/multi` page that embeds these literals — plus screenshots if a browser is already available **without installing anything**.
- Stop that **own PID** when the checks are done. No parking, no second listener, no binding or proxying to `127.0.0.1:33767`, `:43649` or `:46593`. Existing owner listeners are untouched.
- If no browser tooling is available, the implementer must say so and rely on served HTML/JSON evidence plus Astra's diff review — that is not a failure, only weaker evidence.

## 4. Amendment — read-only dependency inspection is GRANTED

The original §5 blanket "must not be read" was too strict for compilation and for the P0 hash guard. Read-only access is now granted to: `Tools/MultiRackSitkaP1Lab/src/Core/**` including `SitkaP1.Core.csproj`, `Directory.Build.props`, `global.json`, `src/Ui/SitkaP1.Ui.csproj`, the lane's `.p0ref` (for hashing), and the compile-linked `Assets/ForestPrototype/{SitkaGrowthModel,SimulationRandom,TreeSpeciesDefinition}.cs` + `Species/SitkaSpruce.asset`.

**Writes remain exactly three literals.** Read permission is not write permission: no edit to Core, csproj, props, wwwroot, P0, fixtures, goldens or Assets is permitted.

## 5. Owned outputs and evidence

- Owned: the lane's `bin/`/`obj/`, and `/home/jer/Development/CCF-885-Astra-UI-Evidence-20261122` (create mode `700`) for build logs, HTTP/JSON captures and screenshots.
- The ephemeral capability must never be written to the evidence root in a form that could be published, never appear in the result Markdown, and never be pasted into GitHub.
- No deletion of any other lane's outputs; the evidence directory is the implementer's own.

## 6. Frozen result artefact (matches this custodian's receipt)

One new file, exactly:
`Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-ASTRA-OPUS-UI-IMPLEMENTATION-RESULT-20261122.md`
on the feature branch. The literal-masking script is versioned as a **fenced code block inside that Markdown**, alongside `before.masked.sha256` and `after.masked.sha256` (must be equal) and the design-document URL implemented. Astra re-runs the script independently.

## 7. Dirty-working-tree discrepancy — clarified and preserved

Two different observers measure two different things:

| Observer | Reading | Meaning |
|---|---|---|
| `git` **for Windows** pointed at the WSL store (e.g. `\\wsl.localhost\…`) | hundreds of "modified" tracked paths (Astra reported 559; I reproduced the same effect earlier in this session) | cross-view artefact: Windows `core.autocrlf`/CRLF, 9p attribute translation and index metadata do not describe the Linux working tree |
| **WSL-native** `git -C /home/jer/Development/CCF-HP status` | **0 lines** (checked again at 09:28Z) | the Linux checkout has **no** tracked or untracked changes |
| Windows-native clone `C:/Users/hp/Documents/GitHub/CCF` | 0 lines | that separate clone is clean too |

So the primary checkout is **not** dirty in the Linux sense; the 559-path reading is a line-ending/attribute artefact of asking Windows Git about a Linux filesystem. **Nothing has been normalised, reset, cleaned or rewritten**, and nothing will be: preserve regardless. For all work under this receipt the **WSL-native** reading governs, and the new Opus lane is a fresh Linux worktree where the question does not arise.

## 8. Unchanged

Source writes remain **only** the three literal interiors in `PlayerMultiApi.cs` (baseline blob `7a589996a6a56b1852b8fff2e8ad0d85e6256b3e`). No new dependencies. No QA programme. No additional author or reviewer. No source edits by the custodian. No `main`, no PR, no board edit. No ownership transfer. No arbitrary expiry. The design document is now published and verified (§0), so it is the child's controlling specification: **`305582e2c6edd08f2aece72ed85dd93e5bdb9640`**, packet blob `1ea7d8da…`.

## 9. Non-claims

No implementation, review, acceptance or qualification is asserted here. This amendment widens exactly three things — tool-chosen lane path, one compile, one bounded `/multi` runtime — and the read permission needed to build honestly.
