# StartupScreenVerification, rendered mode: why it fails (pre-existing, not the UX patch)

| Run | Code | Display | Result |
|---|---|---|---|
| ux-rendered | UX patch 84e881a | live desktop | FAIL `no key opened a forest panel or advanced time` |
| ux-rendered-x | UX patch 84e881a | private nested display | FAIL, same message |
| ab-density-rendered | **a395e3f (no UX patch; pre-patch UI)** | private nested display | **FAIL, same message** |
| startup-diag (derived, message only) | UX patch | private nested display | `[year=0 workPlanOpen=False screen=None helpOpen=True helpMenu=WalkingHud menuOpen=True startupActive=True]` |
| startup-diag3 (derived, state before keys) | UX patch | private nested display | `DIAG_BEFORE_KEYS helpOpen=True helpMenu=WalkingHud uiEnabled=False introducedWalkingHud=False` |

Reading: before any key is pressed the first-use Walking HUD panel is already open (`helpOpen=True`, `uiEnabled=False`, `introducedWalkingHud=False`: a device that has never dismissed it). Year stays 0, no Work Plan, no screen: the keys change nothing. The gate asserts `!ui.Help.IsOpen`, which only holds on a device that has already seen that panel. The screenshot taken at that moment (`fresh-device-title-before-any-key.png`, local) shows the plain title with no panel visible, so a first-time player sees nothing wrong. The batch run of the same gate passes (it skips the key presses).
