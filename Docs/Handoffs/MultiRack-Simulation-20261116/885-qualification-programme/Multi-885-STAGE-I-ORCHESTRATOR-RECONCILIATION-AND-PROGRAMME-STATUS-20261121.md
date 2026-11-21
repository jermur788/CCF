# Multi #885 — Stage I reconciliation and programme status

**Verdict adopted: NEEDS CHANGES for technical integration qualification.**
The integration ref is delivered and unchanged; four of six gates genuinely PASS; two are incomplete for reasons that are **contract conflicts, not product defects**. The hard gate to Stage Q remains closed.

| Item | Value |
|---|---|
| Reconciled by | DeepSeek V4.1 Flash — orchestration, read-only evidence inspection. **Not** the integrator, **not** the implementer, **not** the independent reviewer |
| Reconciles | Stage I result, documented by GPT-6.1 Sol (codex_3), branch `docs/multi-885-stage1-integration-result-20261121-v1`, commit `b08aaa28ba65b96c338be966ad65373732d7ab05`, blob `2779b866ab3ec25da94628ea76f6268a8788ab6b`, readback SHA-256 `5576526788bbb5ae13e3329c78bded751018ccb2758ec2781738da154ccf414e` |
| Routing decision | `dec-885-stage1-integ-20261121T1648Z`, attempt `att-885-stage1-integ-20261121T1648Z` |
| Machine | `DESKTOP-DL1BQI7`, WSL2 Ubuntu, uid 1000 `jer` |

---

## 1. Delivered artefact

| Item | Value |
|---|---|
| **Integrated SHA** | **`4cecf87339bea4d32907955a8d5625d1f27f96bd`** |
| Branch | `integration/multi-885-ma1-two-area-20261121-v1` — **confirmed present on origin at that exact SHA** (read-only observation) |
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121` — `HEAD 4cecf873…`, `git status --porcelain` **0 paths** |
| Fast-forward | `e11dd519` → `4cecf873`, no merge commit; `ac4c7b51` **not** included |
| Byte identity | diff against `4cecf873` **empty**; exactly **16 paths** vs base (15 code/test + 1 author Markdown) |
| SDK | `/home/jer/.dotnet/dotnet` **8.0.134** |

**This is `INTEGRATED` as a branch ref only.** It is **not** merged to `main`, **not** owner-accepted, and **not** released to Stage Q.

---

## 2. Gate matrix — as actually observed

| Gate | Required | Actual | Verdict |
|---|---|---|---|
| Solution build | clean | `Build succeeded`, 0 warnings, 0 errors | **PASS** |
| Core | 135/135 | **135/135 passed, 0 failed** | **PASS** |
| UI build | clean | `Build succeeded`, 0 warnings, 0 errors | **PASS** |
| Single-area smoke | 287/287 | **`OVERALL: PASS (287 passed, 0 failed)`** | **PASS** |
| Multi-area smoke | 127/127 | **`OVERALL: PASS (127 passed, 0 failed)`** | **PASS** |
| P0 reference pin | 4/4 values agree | all four = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`; reference **copied, never rebuilt** | **PASS** |
| Generated P0 isolation | run the pinned isolation check | **NOT RUN — 0 checks.** Harness deletes `.p0ref` and rebuilds it | **F-I1 BLOCKED** |
| N1 history wording — populated branch | actual runtime witness | served-JS witness, one real HTTP year advance | **PASS** |
| N1 history wording — naturally-empty branch | actual runtime witness | branch **not reachable** through the shipped API | **F-I2 NOT TESTED** |

**Independent orchestrator observations (read-only, `2026-11-21T17:01:04Z`)**, performed so the blocked gate is not left entirely dark:

| Layer | Check | Observed |
|---|---|---|
| L0 | no commit in `a7f41983..HEAD` touches `Tools/MultiRackSitkaLab` | commit list **empty** → **PASS** |
| L1 | `HEAD:Tools/MultiRackSitkaLab` tree identity | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` **== pin** → **PASS** |
| L2 | P0 status incl. `--ignored --untracked` | **0 lines** → **PASS** |
| L3 | ignored/untracked inventory under P0 | **0 files** → **PASS** |
| L4 | cold-rebuild write trace | **NOT TESTED** |

These L0–L3 layers are the substantive "P1 did not write into P0" property for the delivered tree, and they are clean. They are **not** a substitute for L4 and are not recorded as such.

Protected lanes survived untouched: pid **1750123** (`#877`, `:33767`) and pid **1806361** (`#882`, `:43649`) alive with unchanged identity; pid **390578** (`#867`) still absent and never restarted. All four of the integrator's own listeners ended cleanly by PID: `1060852/38653`, `1074088/36165`, `1075184/36129`, `1075349/45113`.

---

## 3. Finding F-I1 — P0 isolation contract conflict (gate-blocking)

`Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh` line 39 unconditionally executes:

```
rm -rf -- "$p1_root/.p0ref" "$p1_root/src/Core/bin" ... "$p1_root/tests/Core.Tests/obj"
```

and then, for its cold run, invokes `build/build-p0-core-ref.sh`, which **regenerates** the `.p0ref` reference from the pinned P0 tree object.

Three governing texts disagree:

| Source | Text | Reading |
|---|---|---|
| D-091 admission receipt §5, prohibited list | "rebuilding protected P0 references **except through** the pinned `build-p0-core-ref.sh`" | rebuilding **via that script** is the sanctioned mechanism |
| D-091 admission receipt §7 step 4 | "pinned P0 ref script `build/build-p0-core-ref.sh` **and** `tests/p0-isolation.sh` only as receipt permits" | the receipt anticipated **both** scripts running |
| P1 spec §3.5 / `MultiRackSitkaP1Lab/README.md` | "On this path it is never rebuilt … **Do not run** `build/build-p0-core-ref.sh`" | no rebuild at all on this path |

The integrator applied the **conservative** reading and stopped at inspection — it did not patch the script, wrap it to bypass its work, or credit it as passing. **That was the correct behaviour.** It is a real conflict between a physical grant and a product spec, and resolving it requires one bounded ruling, not an agent's preference.

**Minimal options for the ruling** (any one closes the gate):

- **(a) Disposable tree.** Run the unchanged `tests/p0-isolation.sh` inside a throwaway detached worktree at `/tmp`, created from `4cecf873` and removed afterwards. The delivered lane and every other lane stay untouched; `.p0ref` is regenerated only inside the throwaway tree. *Least invasive; still a physical act needing custodian approval.*
- **(b) Receipt exemption.** Ruling that the §5 exemption governs: the pinned script may regenerate `.p0ref` inside the integration lane, because the script itself refuses unless the P0 subtree equals the qualified tree and its output must hash to the pin. *Touches the delivered lane's generated output.*
- **(c) Bounded disposition.** Manager accepts the observed pin agreement + L0–L3 + the two smoke suites' no-write assertions as sufficient for Stage I closure, with L4 recorded as **NOT TESTED** permanently. *Cheapest; leaves the deepest layer unproven.*

Orchestrator preference: **(a)**, with **(c)** as fallback if the custodian declines to create any new worktree.

---

## 4. Finding F-I2 — naturally-empty history wording not witnessed (gate-partial)

Both shipped areas begin populated. The HTTP process constructs only `MultiAreaWorkPlanAdapter.OpenSynthetic()` and exposes **no** custom-experiment, import or seed endpoint, and Execute refuses to fell a populated area to zero. The empty branch is therefore **not reachable** by a short shipped-API run.

What *was* genuinely witnessed, from the **served** JavaScript against an **unaltered** real API response, after exactly one real no-work HTTP year advance (MA-A 171 → 166 living, MA-B 73 → 73, both `noLivingStems` false):

> Latest year 0 → 1. Each area ran the unchanged annual rules on its own trees.

What was **not** witnessed — the unexercised served conditional, quoted from source text only:

> Each populated area ran the unchanged annual rules on its own trees; an area with no living stems was carried forward unchanged.

The Core suite's `MA-E05` reaches genuine natural extinction with a **test-only custom experiment** and checks a `null` carried-empty annual report — but that is an engine test, not a UI presentation witness. **This is a reachability limitation, not a product defect**, and no pass was invented.

**This can resolve itself legitimately.** Q0 W15 records that the hazard step has no last-survivor guard, so natural mortality *can* empty an area. A 50-year no-work control in Stage Q may therefore produce the empty branch from real simulation — which would supply the missing witness honestly, at no extra cost and with no source change. Manufacturing emptiness by editing source, fixture or payload is **not authorised under any option**.

---

## 5. Finding F-I3 — fixed-port reservations are not executable

`Program.cs` hard-codes `Listen(IPAddress.Loopback, 0)` with an empty configuration builder and ignores `ASPNETCORE_URLS`, `DOTNET_URLS`, `ASPNETCORE_HTTP_PORTS` and Kestrel endpoint configuration (the shipped smoke test sets all four to a hostile value specifically to prove they are ignored).

So **both** reserved fixed ports are non-executable without a source change:

| Reservation | Grantee | Executable? |
|---|---|---|
| `127.0.0.1:33811` | integration lane | **No** — the integrator used the disclosed fallback and recorded every real port |
| `127.0.0.1:33841` | Stage Q qualification lease | **No** — `blk-9fccec6a265b8e3f` |

Because both reservations fail for the same reason, **one custodial ruling fixes both**. Recommended amendment: *"loopback only, OS-assigned ephemeral port, every bound port recorded per run, no fixed-port requirement."* This changes no product code, matches how the two already-qualified smoke suites run, and preserves the properties that actually matter: loopback-only, never 5187, capability-gated, single listener, stopped by its owner.

---

## 6. No unauthorised action observed

No `main` merge, no PR, no force push, no tag, no history rewrite. No source, test, fixture, golden or P0-reference change. No Unity, no Windows build, no save-file operation. No touch of `#877`, `#882`, `#867`, or the author/reviewer worktrees — the only cross-lane access was a read-only copy of the qualified `.p0ref`, which the grant expressly permits.

Two self-corrected incidents, both disclosed and both without product change: an initial smoke prerequisite stop because `node` was not on `PATH` (resolved by locating the pre-existing Node v24.22.2 rather than installing anything), and a self-matching process audit (`tee` flagged itself; narrowed and rerun, nothing killed).

---

## 7. Programme status

| Stage | State | Detail |
|---|---|---|
| **I** integration | **NEEDS CHANGES** | ref `4cecf873` delivered; 4/6 gates PASS; F-I1 and F-I2 open |
| **P** Q1 protocol | **PRODUCED** (draft) | published; source-independent; not authorised |
| **Q1** execution packet | **PUBLISHED — BLOCKED** | exact SHA pinned; blocked on decisions 1–3 |
| **Q** experiments | **BLOCKED** | hard gate: no cleared Stage I and no execution grant |
| **R** independent review | **NOT STARTED** | reviewer will be a fresh, separate session from both the integrator and the run worker |
| **S** next improvement | **NOT STARTED** | depends on observed Stage Q results; will be a packet or four-file spec, never an implementation grant |
| **G2** owner acceptance | **PENDING** | owner's own playtest; no agent here claims it |

### Decisions needed from the Manager / D-091 custodian

1. **P0 isolation (F-I1):** option (a), (b) or (c) in §3.
2. **Port reservations (F-I3):** amend both leases to ephemeral loopback with recorded ports, or authorise a bounded source packet.
3. **N1 empty branch (F-I2):** accept the limitation and take the first natural extinction in Stage Q as the witness, or issue a written partial-evidence disposition.
4. Then: flip the Q1 execution packet to **APPROVED TO RUN** on `4cecf873`.

Until 1–3 land, nothing further in the programme can lawfully start. This is not a stall by choice — every remaining stage depends on the same two mechanical facts: whether the P0 isolation check may regenerate the reference, and whether a loopback port may be OS-assigned.

---

## 8. Claim ladder

- **PRODUCED:** this reconciliation; the Q1 protocol draft; the Q1 execution packet.
- **INTEGRATED:** `4cecf873` on `integration/multi-885-ma1-two-area-20261121-v1` — branch ref only.
- **VERIFIED:** four of six Stage I gates, plus pin agreement and L0–L3, on the exact delivered tree.
- **REVIEWED:** nothing in this programme has independent review yet.
- **OWNER ACCEPTED:** **never asserted.** G2 is the owner's own test.

**Scope.** Technical qualification of a **synthetic** closed-canopy Sitka model. Not ecological validation, not an Irish field forecast, not MA-2 empirical comparability (G1 NO-GO), not `main` integration, not owner acceptance. Regeneration, windthrow, browsing, canopy light, timber grades, economics and cross-area ecological interaction are **NOT MODELLED**.
