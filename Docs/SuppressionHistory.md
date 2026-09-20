# Age, size and recorded suppression

Chronological age remains years since establishment. Inspection now describes
size using DBH alone: small below 10 cm, medium from 10 to below 30 cm, large
from 30 cm. These are deliberately chosen display bands, not empirical Sitka
maturity thresholds, harvest recommendations or timber-quality grades.
Height still selects the visual asset. Species age-based reproduction is unchanged.

Current suppression is `1 - 1 / (1 + CI / Ci50)`: the fraction of potential
diameter growth withheld by the existing competition response. One annual
step adds this fraction to `EquivalentSuppressedYears`. For example, two
years at 50% suppression record one equivalent suppressed year.

This is a simulation diagnostic, not an empirical measure of physiological
damage. It introduces no further growth penalty. Thinning changes competition
and subsequent growth without resetting age, diameter or recorded history.
Felled trees retain their history and stop accumulating it.

History starts at zero on initial generation or promotion to an individual tree.
Earlier competition, including the regeneration-cell phase, is not reconstructed.
Version 6 saves preserve this value for existing and respawned trees. Loading
versions 1–5 explicitly resets recorded history to zero, even if the current
session has already accumulated history. Zero means none recorded, not proof
that a tree was never suppressed.

## Verification

The accompanying regression harness exercises the actual ForestTest scene,
harvesting, save/load including missing-tree respawn and version-5 migration,
two 80-year lifecycle runs, same-age trees of different sizes, thinning release,
and unchanged growth under different accumulated histories.

Run in a disposable checkout or otherwise idle Forestry worktree. Copy
`Tools/Verification/CCFIntegrationVerificationTemp.cs` into
`Assets/ForestPrototype/`, then launch Unity 6000.6.0f1 with
`-batchmode -nographics -projectPath <checkout> -executeMethod CCFIntegrationVerificationTemp.Begin -logFile <log>`.
The harness restores the previous forest-save.json on normal success/failure.
Remove the temporary Assets script and generated .meta after the run; the
harness must not remain in a playable build because it starts automatically.
The lifecycle hash assertion is specific to the previously verified Editor
configuration. Compare hashes only within the same build configuration.
