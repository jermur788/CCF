# RACK-V2-02B-R headland gates

Disposable Tier-2 gates for the Rack V2 headland / feeder / pad fresh-world work (RACK-V2-02B-R,
qualified candidate `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd`). Both are **byte-exact copies** of the
sources RACK-V2-02B-R ran, which that lane kept only in its evidence archive
(`/media/jer/Files/CCF/Verification/RackV2-02B-R-20261027/`). They are committed here so the gates can be
re-run from the repository. They were not edited.

| File | Entry method | Pass token | Baseline |
|---|---|---|---|
| `RackV2HeadlandQualification.cs` | `RackV2HeadlandQualification.Begin` | `HEADLAND_QUALIFICATION_PASS checks=124` | 124 checks |
| `RackV2AccessVerification.cs` | `RackV2AccessVerification.Begin` | `HEADLAND_ACCESS_PASS checks=8` | 8 checks |

Both need a graphical Editor (screen captures; GameView resize). They never write the player's save:
state round trips are in memory. Run them under the machine-wide Unity lock in a Unity lane, with
`CCF_TEST_DIRECT_ENTRY=1`, `CCF_DIAG_DIR` and `CCF_ACCEPTANCE_OUTPUT` set, using the repo's
copy -> run -> remove workflow (stage into `Assets/ForestPrototype/`, remove with the generated `.meta`).

Harness notes carried from the 02B-R handoff: the access gate tries standing points inside the 2.6 m
headland width because `ForestPlayer.interactionDistance` is 3.5 m, and `PO1204` is only reachable from
the east part of the strip; the headland gate restores the player's start position before regenerating
model 0 because the stand generator clears trees around the player.
