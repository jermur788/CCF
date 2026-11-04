# Scenario One Verification-Debt Repair (#223) — Manager Review — 2026-11-02

**Verdict: APPROVED**

## Reviewed identity

- Claim: #223
- Returned branch: `verify/scenario-one-debt-repair-v1`
- Starting integration candidate: `3acdf9cb635cdca79606335801e97f9d7dcce8b0`
- Verification commit: `ad9a8f761c062e97895838fb0f12db1aa71a6ff3`
- Returned handoff tip before this review: `7e7ba70290b7ac881a3c05f328f45da7e2033ae8`
- Durable handoff: `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/HANDOFF.md`
- Binding packet: `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/TASK-PACKET.md`

## Scope review

The returned repair stays within the authorised verification-only boundary.

`3acdf9cb..ad9a8f76` changes only verification/calibration-contract tooling under `Tools/`; there are no changes under `Assets`, `Packages` or `ProjectSettings`. No production gameplay, UI, save semantics/schema, ecology/economy/calibration, protected anchor or Reference Future content changed.

The changes correspond to the packet-authorised stale v24 pin/hash reconciliation, history harness compile repair, `MaxRemovedBasalShare` scanner narrowing, and guided-v2 sequence/rack expectation reconciliation. The additional history/guided v23 pins were exposed by required gates and were repaired under the same accepted v24/D-066 authority rather than by inventing new product intent.

The density scanner repair does not waive the biological guard: the three accepted pure-Beech reduced-simulator R2 misses remain visible against the unchanged 0.40 criterion.

## Verification assessment

Accepted returned evidence:

- SiteInfrastructure offline: PASS, 63 checks.
- Oak/Beech integrated calibration contract: PASS, 360 checkpoints / 7 protected files.
- Three-species timber offline: PASS, 73 checks.
- Three-species pruning: PASS, 3,946 checks.
- Three-species history offline: PASS, 74 checks.
- Density scanner/isolation: repaired false positive removed; only the three previously accepted pure-Beech R2 misses remain.
- Unity MixedDensity Legacy40: PASS, 34 checks.
- Unity BroadleafTimber Legacy40: PASS, 66 assertions.
- Unity BroadleafTimber Enlarged80: PASS, 66 assertions.
- Unity ScenarioHistory: PASS, 43 assertions.
- guided-v2: 533 PASS / 2 FAIL, with both remaining failures exactly the separately owned #224 `current objective fits` rendered-layout defect at 1280×720 and 1920×1080.

The packet explicitly required the objective-fit assertion to remain red if #224 had not yet been repaired. It was not weakened, suppressed or reclassified. No new failure family is reported.

The Unity lock lifecycle and cleanup are acceptable: the lock was acquired/released normally, the returned worktree was clean, and no Windows build was performed. The documented licence-source divergence and killed-run Temp hygiene are tooling/environment notes, not accepted product failures.

The requested SOL/CODEX delegation failed with `Invalid credential`; the worker recorded this as a provider/environment failure and continued on the local DeepSeek lane. This affects routing evidence, not the technical verdict.

## Decision

**#223 is APPROVED.**

The verification-debt repair may be fast-forwarded onto `integration/scenario-one-player-test-ready-v1` preserving the reviewed commits. This approval does **not** make guided-v2 globally green and does **not** release MFM-12 by itself.

The remaining blocking product defect is #224, the Objectives-window `current objective fits` overflow. #224 must be reproduced/repaired/reviewed on the exact post-#223 integrated head. After #224 is accepted and integrated, Browser Manager #200 must repin #225/MFM-12 to that exact resulting integration head.

## Accepted boundaries retained

- Three pure-Beech R2 misses remain an accepted C-level limitation; 0.40 criterion unchanged.
- Frozen Reference Future v1 / Legacy40 compatibility remain protected.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.
- #194 remains NO ACTION.
- MFM-12 has no production-repair authority.

**Manager token:** `SCENARIO ONE VERIFICATION DEBT REPAIR APPROVED — #224 NEXT`
