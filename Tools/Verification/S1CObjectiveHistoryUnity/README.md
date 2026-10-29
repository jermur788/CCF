# S1-C T-05 Unity qualification

Disposable verification only. Production candidate: `4fb5ba99de52384932456d68da112432c899480a`.
The pre-existing untracked `S1CObjectiveHistoryUnityVerification.cs` was preserved unchanged when this executor began.
Its assertions are not weakened by this runner. `HashFixtures.cs` is staged unchanged from the existing offline suite.

Run `python3 Tools/Verification/S1CObjectiveHistoryUnity/run_gate.py --help` for arguments.
Requires an exact clean detached U1 and an already-held machine-wide lock owned by `T3-S1C-T05`, task `S1-C-T05`.
Evidence must be under `/media/jer/Files/CCF/Verification`. Each invocation requires an unused output directory.

Run `compile` first. Only proceed after its exit 0 / pass receipt. For `objective`, provide `--fixtures` pointing to a
manifest of genuine pre-v22 save files: each entry has `name`, `file`, `note`; `v20-y5` is required by the evidence-gap
fixture. Preserve originals; record source paths and SHA-256 digests. The harness uses real manager/evaluator/save/UI paths.

Each gate is staged into `Assets/ForestPrototype`, run in Unity 6000.6.0f1 with isolated XDG configuration and a private
Xephyr display, then removed with its generated `.meta`. Host save digests are compared. Known shared VS Code settings
and SectionFive material import changes are recorded and restored byte-for-byte. Isolated leftover save bytes are hashed
in the receipt then removed; no player save is intentionally written. Copied licences are removed from evidence.
Do not run another Editor concurrently. Stop on a production defect; do not fix production or alter assertions here.

Historical reference/progress/completion/density/interaction gates explicitly select Legacy40 and a valid legacy rack
fixture using the unchanged `TeachingCapGateSetup` companion. Objective history runs the production Rack V2 path.
`completion-model1` explicitly requests the existing historical model-1 reconstruction; `density-save` selects the
unchanged runtime `ANCHORS,SAVELOAD` phases, including the full unfiltered JSON key-set comparison at DensityQualification.cs:952.
No Windows build is performed. The handoff lists actual results and any unrun gates.

`render` is a supplemental gate using the real AnnualReviewView and saved outputs from `02-objective/evidence`. It loads
silently through the production typed controller route, scrolls the full objectives card and asserts every history line is
fully inside the viewport before capture. The original objective harness remains byte-for-byte unchanged.

`baseline-hash` compares every archive hash through Unity JsonUtility against pre-v22 source from `aca74ac`.
`PreV22SaveTypes.cs` contains that commit's DTOs with type names prefixed; `PreV22Archive.cs` contains its unchanged hashing
methods. See `baseline-provenance.json`. Corresponding new-writer fixtures use their original v21 header in the baseline;
explicit v20 and parsed v21 inputs retain their own headers. This constructs different save-schema inputs without changing
any anchor or existing assertion. `interaction` forwards to the existing protected interaction gate, including the explicitly
configured `BFC55473C1506067` lifecycle assertion. No broad legacy integration fixture is substituted for that configuration.

## Observed blockers on the exact candidate

The unchanged main harness completed 542 checks but has two overly specific expected-refusal-wording assertions:
a v20 fixture also carries Rack V2 state (that incompatibility is refused first), and strict duplicate-key detection
occurs first in geometry JSON validation. Both refusals preserve world and disk. Do not change those assertions in this task.

The unchanged completion harness reaches completion and its hash checks, then rejects its synthetic v14 fixture because
it retains v22 objective records after lowering `version`. This fixture is not a genuine v14 save. The assertion and fixture
remain unchanged; no production validation change is justified. See the handoff for overall failed-gate verdicts.
