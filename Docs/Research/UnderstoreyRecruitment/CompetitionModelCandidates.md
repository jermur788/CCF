# Competition candidates and decision

Status: CALIBRATION READY — MODEL DECISION REQUIRED. No production model selected. Recommended next experiment: a competitor-specific, bounded annual survival response, shared by natural and planted juveniles, with separate vegetation-loss accounting. This has the closest connection to the strongest transferred survival evidence and one process to calibrate. It is a recommendation for a diagnostic trial, not a validated adoption.

| Process | Evidence | Player meaning | Burden / limitation |
|---|---|---|---|
| A establishment | B/I, plausible seedbed bottleneck | Fewer new juveniles | Could incorrectly apply oak seedling-survival evidence to germination; needs requested-before/after vegetation ledger |
| B survival | B strongest direction from oak study | Established juveniles disappear | One additional loss term; perennial compounding can be severe |
| C height growth | B/I mechanism; no coefficient here | Juveniles remain small longer | Promotion timing sensitive; mortality effect not captured |
| D establishment + growth | I | Fewer and slower juveniles | Two interacting parameters; overfitting risk |
| E survival + growth | B/I | Fewer survivors, delayed escape | Two parameters; double-counting the same observed bottleneck |
| F species-specific combination | Insufficient quantitative species evidence | Different species success | More degrees of freedom than evidence; defer numeric species coefficients |

The offline sweep compares five response forms across A–E: linear q=c; saturation q=1.3c/(0.3+c); threshold saturation q=1.3max(0,c−0.2)/(0.3+max(0,c−0.2)); type-weighted pressure (requires independently known competitor cover); light interaction q=c(0.4+0.6L). Penalty e=min(0.95,kq); k=0.15/0.35/0.60. All constants C. Saturation shapes are not fitted field relationships. The type-weighted scalar is mathematically identical to linear in this virtual sweep: its benefit is correct biological input, which is NOT measured by this sweep.

Smallest defensible input proposal: separate bramble, bracken and competitive graminoid cover, each 0..1, then a bounded aggregate max or weighted pressure. Do not sum overlapping cover as if it were a physical area partition. Weights and the max assumption would be C/I. Existing moss/fungi/herb cover excluded. A dedicated input requires persisted state if it evolves independently: this reaches the explicit schema stop; no new field/version implemented.

Alternative for manager review: accept a declared, coarse derived pressure from current fields for a limited diagnostic trial, with no save change. It must explicitly redefine selected fields as competitor proxies, preserve decorative ordinary-fern/bilberry separation, handle partial planting circles, and remain a calibration approximation. The current visual palette cannot be used as measured biological cover. No scientifically supported automatic winner emerges between that shortcut and competitor-specific state.

Gate assessment: one abstraction has a stronger recommendation but does not clearly win numerically; a universal coefficient is not defensible; competitor-specific persistent state needs a save proposal; area clearance integrates cleanly but partial circles need explicit area treatment; candidate world promotion/economy/save behaviour has not been verified. Production gate is therefore NOT met. Current baseline regression can establish safety of the unchanged game, not safety of unimplemented competition.
