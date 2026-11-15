# Final-round reproducible verification

Worktree: `/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115`.
Implementation head `60f97b06bd41f9a89fde4eb775527ebc6947560b`.
No Editor/game/Primary/live board write. Self-test stops before lock exercises;
no force/steal runs. Do not bypass its legitimate residual `/` failure.

From repository root:

```bash
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/final-round/verify-fixture-devices.py --ops CCF-Ops --output /media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/final-round/fixture-device-probes.json
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/round2/verify-storage-boundaries.py --ops CCF-Ops --output /media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/final-round/boundary-probes.json
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/verify-identity.py CCF-Ops/scripts CCF-Ops/lanes.json
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/continuation/prove-provenance.py --ops CCF-Ops --output /media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/final-round/provenance-probes.json
```

Required gates from `CCF-Ops/`:

```bash
python3 -B tests/self-test.py
python3 -B scripts/packet-lint --board board/active.md
python3 -B scripts/packet-lint tests/fixtures/valid.md
```

The three gates and provenance probes are repeated at the final publication
head; unfiltered final stdout/stderr/exits are preserved externally to avoid
a self-referential commit hash. Final gates and exact test diff are posted on
#548, which remains OPEN. All temporary probe copies contain only checked
scripts/config and are removed; never live board/ledger/state. Whole-tree
hashes and exact Git-blob checks are in adjacent receipts. Primary was neither
read nor written in this round.
