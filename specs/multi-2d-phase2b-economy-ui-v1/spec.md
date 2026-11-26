# CCF Multi 2D — Phase 2B-2a Economy UI & `/multi` Routes — Specification

- **Spec ID:** `SPEC-multi-2d-phase2b-economy-ui-V1`
- **Feature ID:** `multi-2d-phase2b-economy-ui-v1`
- **Status:** `IMPLEMENTING` (Authorized under `T-2B2A-R1:20261126T1710Z`, amended by owner decisions D1b, D2a, D3, D4)
- **Base Commit:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim`)
- **Branch:** `feat/multi-2d-phase2b2a-economy-ui-20261126-v1`
- **Normative Upstream Engine Contract:** `specs/multi-2d-phase2b-economy-v1/{spec,plan,tasks}.md` and `Docs/Handoffs/MultiRack-Simulation-20261116/phase2b-economy/PHASE2B1-IMPLEMENTATION-HANDOFF-20261126.md`
- **Scope:** UI HTTP endpoints and `/multi` frontend (`Tools/MultiRackSitkaP1Lab/src/Ui/{MultiRoutes.cs, MultiApi.*.cs, PlayerApi.cs, multi/**}`) plus headless Edge economy tests (`Tools/MultiRackSitkaP1Lab/tests/phase2b2a-economy-ui/**`). Zero edits to `src/Core/**`, `tests/Core/**`, `src/Ui/SitkaP1.Ui.csproj`, `src/Ui/wwwroot/**`, or Unity `Assets/`.

---

## 1. Locked Owner Decisions (D1b, D2a, D3, D4)

| Decision | Binding Rule |
|---|---|
| **D1b — No Felling Dispositions in 2B-2a** | No felling disposition selector, no `KeepForUse` m³, and no retained-timber strip ship in Phase 2B-2a. The UI states honestly that felled timber is priced and settled through the existing sell-and-extract engine path. Timber dispositions are deferred to a post-Phase-2A-1 Core+UI addendum (alongside 2B-2b Save v3). |
| **D2a — Launch-Time Money On/Off Opt-In** | Money On/Off is a launch-time opt-in that starts a **fresh Year-0 session** of the active stand (`ma-1` or `ma-1-mixed`) using the existing genesis constructor (`MultiAreaModelSelection.Phase2WithEconomy` + `EconomyOptions.Default` for `ma-1`, plus the `MultiAreaDemoExperiments.CreateMixedDemo()` options for `ma-1-mixed`). Default is **Off** on first launch. If the player is past Year 0 or has pending/approved/executed work (or local drafts), an explicit unsaved-progress confirmation is required before resetting. |
| **D3 — Endpoint Layer** | All `/multi` economy HTTP routes live in `Tools/MultiRackSitkaP1Lab/src/Ui/MultiRoutes.cs` + `MultiApi.*.cs` with minimal lock-safe session wiring in `PlayerApi.cs` only. Zero changes to single-area `/api/*` behaviour, Host/Origin/capability/CSP guards, or single-listener rules. |
| **D4 — Module Count Preserved (14 Modules)** | Reuse the existing 14 ES modules under `Tools/MultiRackSitkaP1Lab/src/Ui/multi/`. Add **zero** new JS modules. `src/Ui/SitkaP1.Ui.csproj` and `MultiRoutes.MultiPageModules` remain byte-unchanged. |

---

## 2. Presentation Authority & Zero-JS-Math Rule (D-020)

- JavaScript computes **zero** economic arithmetic: no euro sums, no cent-to-euro conversions, no harvest rate calculations, no minimum-floor checks, no hour deductions, no net cash deltas, and no assortment pricing in the browser.
- Every euro figure, signed delta, landowner hour readout (`X / 40 hrs`), harvest stage label, minimum-floor highlight flag, upfront-cash eligibility verdict, assortment breakdown, and ledger entry is formatted and serialized by the C# server from authoritative `SitkaP1.Core` engine calls (`ReadEconomyView`, `QuoteWorkPlan`, `ReadEconomyHistoryYear`, `SetOrderExecutor`, `Approve`, `Execute`, `Advance`).
- When Economy is **Off**, no `"economy"` property is emitted on `/multi` view or command payloads (`JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)`), keeping the Off JSON payload byte-comparable and preserving all G5 transport budgets (`<= 524,288 B` raw view, `<= 153,600 B` ecology).

---

## 3. Server-Authoritative `/multi` Economy Endpoints

Every endpoint runs under the unchanged `/s/<capability>/` prefix, Host, Origin, Content-Type, `MaxBodyBytes` (`16 KiB`), unique JSON member, and bounded text guards:

| Method & Route | Purpose | Refusal & Safety Contract |
|---|---|---|
| `GET api/multi/economy/view` | Read-only revision-fenced Economy snapshot (formatted cash, hours, stage, executors, active quote, latest settlement, evidence). | 409 `EconomyModelRequired` when Economy is Off; zero mutation. |
| `POST api/multi/economy/session` | Launch-time Money On/Off toggle (`fence`, `enabled`, `confirmReset`). Atomically swaps `MultiSessionPair` under `multiSwapSync` to a fresh Year-0 session of the active stand. | 409 on stale fence; 409 `EconomyResetConfirmationRequired` if dirty (Year > 0 or pending/approved/executed work) and `confirmReset != true`. Old fences are refused after swap (`PlayerSessionMismatch` + `+economy-v1` token change). |
| `POST api/multi/economy/quote` | Pure read-only Work Plan quote (`fence`) via `QuoteWorkPlan`. | 409 `EconomyModelRequired` when Off; 409 on stale fence; zero state mutation. |
| `POST api/multi/economy/executor` | Assigns `Contractor` or `Landowner` to a pending order (`fence`, `kind`, `areaId`, `targetKey`, `executor`). | 409 `EconomyFellContractorOnly` for `Fell` → `Landowner`; 409 `WorkPlanAlreadyApproved` while approved; 409 `EconomyLandownerTaskIneligible` if ineligible; 409 `EconomyModelRequired` when Off; state unchanged on refusal. |
| `POST api/multi/economy/history/year` | Read-only per-year financial ledger (`fence`, `year`) via `ReadEconomyHistoryYear`. | Returns `NOT RECORDED` + `null` cents when Off; returns recorded settlements and ledger rows when On; 409 on stale fence or invalid year. |

Existing endpoints (`GET api/multi/view`, `POST api/multi/plan/approve`, `POST api/multi/plan/cancel`, `POST api/multi/plan/execute`, `POST api/multi/year/advance`, `POST api/multi/year/skip`, `GET api/multi/saves`, etc.):
- When Economy is **On**, view and command payloads include the compact, server-formatted `economy` projection; when **Off**, `economy` is `null` and omitted.
- `GET api/multi/saves` catches `SaveEconomyRequiresV3` cleanly when Economy is On and returns HTTP 200 with empty slots and typed `refusal` notice (zero HTTP 500s, zero stderr). `POST api/multi/save` and `POST api/multi/load` continue to return 409 `SaveEconomyRequiresV3`.

---

## 4. UI Design, Layout & Honesty Rules

1. **Dark Woodland Theme & Layout Discipline:**
   - Preserves existing palette, typography, and 3-column desktop grid (`map | legend | side`). Zero 4th permanent column.
   - Clean at both **1920×1080** and **1280×720** with zero horizontal scrolling (`scrollWidth <= innerWidth + 1`), single-row `.topbar`, and preserved R3 5-competitor above-the-fold geometry when Money is Off.
   - Top bar includes compact `Money: Off` / `Money: On` toggle button (`#economy-toggle`).
   - Top economy strip (`#economy-strip`) is `hidden` when Money is Off (0 px impact) and shows **Cash** (`€12,000.00`), **Landowner hours** (`40 / 40 hrs`), engine-derived selective thinning stage, and honest sell-and-extract / Save v3 note when Money is On.
2. **Work Plan Quote Panel & Pending → Quote → Approve → Execute Flow:**
   - Displayed in `#pane-plan` when Money is On (`#economy-quote-panel`), populated exclusively from server-formatted values:
     - Grouped contractor harvest fee with engine-derived selective CCF thinning stage (`FirstThinning €21/t`, `SecondThinning €23/t`, `LaterThinning €20/t` — **no clearfell, no player stage override**).
     - Explicit visual highlight whenever the **€2,500 minimum harvest job floor** (`minimumJobFloorApplied`) applies, showing both variable cost and floor adjustment.
     - Non-harvest contractor labour (`€45/hr` pro-rated) and on-demand materials/saplings (`tree-shelter €5.00`, `beech-sapling €4.50`, `sessile-oak-sapling €5.50`).
     - Expected timber income by species and D-062 assortment grade (sell-and-extract path; zero disposition selector).
     - Net cash impact, required landowner hours, and upfront cash eligibility verdict.
   - **Upfront-cash & allowance blocking:** When upfront cash is insufficient (or landowner hours are insufficient), `#approve` and `#execute` are blocked with a plain-English explanation stating that upfront cash must cover all external costs before approval/execution, expected timber revenue cannot pre-pay work, and no borrowing or overdraft is allowed.
   - **Contractor-only Fell:** Pending Fell rows show a locked `Contractor only` badge with a short tooltip explaining commercial production felling is not ordinary owner DIY. Non-harvest pending rows offer `Contractor` / `Landowner` assignment before approval (locked once approved).
   - **Calm, honest disclaimer:** Explicitly states these are calibrated teaching estimates, small cuts on small woods can net a loss under the €2,500 harvest floor, no automatic grants are modelled, and felled timber is priced/settled via the sell-and-extract engine path (dispositions deferred).
3. **Single History Drawer:**
   - Zero second History button.
   - When Money is **Off**: History drawer behaves identically to Phase 1 Step 2 and keeps the existing `FinancialLedger: NOT RECORDED` disclosure with zero currency words or symbols in `#history-detail-pane`.
   - When Money is **On**: selecting a recorded year in the same History drawer displays a `Financial Ledger — Year Y` (`RECORDED`) card with server-formatted opening/closing cash, external costs, timber revenue, net delta, landowner time used, and ledger rows.
4. **Save v3 Deferral (Explicit):**
   - Save v3 economy persistence is **deferred to Phase 2B-2b**. While Money is On, the Saved games dialog surfaces `SaveEconomyRequiresV3` honestly, disables saving to v2 slots, and advises switching Money Off to use Save v2 slots.

---

## 5. Acceptance IDs (`2B2a-01..08` + `2B2a-REG`)

| ID | Requirement (Amended per D1b, D2a, D3, D4) |
|---|---|
| **2B2a-01** | Money defaults to **Off** on launch; Off payloads omit `economy`, preserve G5 budgets, keep History drawer `FinancialLedger: NOT RECORDED` with zero currency strings, and preserve all existing UI/save/history behaviours. |
| **2B2a-02** | Top bar shows `Money: Off/On` toggle; when On, top strip displays server-formatted Cash (`€12,000.00`) and landowner hours (`X / 40 hrs`), clean at 1920×1080 and 1280×720 with zero horizontal scrolling and zero 4th permanent column. |
| **2B2a-03** | *(Amended per D1b)* Work Plan Quote renders every figure strictly from server responses (zero JS math): grouped contractor harvest fee by engine-derived selective thinning stage (no clearfell, no stage override), €2,500 minimum floor highlight, on-demand materials/saplings, expected timber income by species/grade through the sell-and-extract engine path (no disposition selector, no `KeepForUse` m³, no retained-timber strip), net cash impact, landowner hours, and calm honest disclaimer. |
| **2B2a-04** | *(Amended per D2a)* Launch-time Money On/Off toggle starts a fresh Year-0 session of the active stand (`ma-1` or `ma-1-mixed`) via the genesis path, requires explicit unsaved-progress confirmation when past Year 0 or when pending/approved/executed work exists, drops stale fences, and surfaces Save v3 deferral honestly. |
| **2B2a-05** | Per-order `Contractor` / `Landowner` assignment works on pending non-harvest orders, locks while approved (`WorkPlanAlreadyApproved`), and keeps `Fell` strictly contractor-only with tooltip + server refusal (`EconomyFellContractorOnly`). |
| **2B2a-06** | Upfront-cash guard blocks Approve/Execute in UI and server (`EconomyInsufficientCash`) with plain-English explanation (expected revenue cannot pre-pay; no borrowing). |
| **2B2a-07** | Full `Pending → Quote → Approve → Execute → Settle` flow updates Cash and landowner hours atomically, returns the stored settlement on replay with zero double charge, and resets landowner hours to `40 / 40 hrs` after `Advance 1 Year`. |
| **2B2a-08** | Single History drawer folds recorded financial ledger rows into the existing year-detail view when Money is On, with zero console errors, zero CSP violations, zero 404s, and zero 5xx responses across both 1920×1080 and 1280×720. |
| **2B2a-REG** | Zero regressions across `SitkaP1.Core.Tests` (`291/291`), `Phase1Save.Tests` (`50/50`), `ui-smoke-multiarea.sh` (`363/0`), `ui-smoke.sh` (`250` pass, only known `{L01,L02,L06,J31}` fail), `run-edge-e2e.sh` (`63/63`), `run-step2-edge.sh` (`67/67`), and `build/2d-gate.sh --headless-only` (exit 0). |
