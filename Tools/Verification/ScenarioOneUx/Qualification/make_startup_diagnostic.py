#!/usr/bin/env python3
"""Derive StartupScreenDiagnostic.cs from the shipped StartupScreenVerification.cs. Changes NO assertion:
  - the class names are renamed so it can be staged beside nothing else;
  - the failure message of "no key opened a forest panel or advanced time" carries the four values it tests;
  - one log line and one screenshot are added just BEFORE the key loop, to show the state the keys start from.
Usage: make_startup_diagnostic.py <output.cs>
"""
import sys
from pathlib import Path
ROOT = Path(__file__).resolve().parents[4]
text = (ROOT / "Tools/Verification/Startup/StartupScreenVerification.cs").read_text().replace("StartupScreenVerification", "StartupScreenDiagnostic")
msg = '"no key opened a forest panel or advanced time");'
loop = "foreach (Key key in new[] { Key.F10, Key.W, Key.E, Key.Tab, Key.M, Key.O, Key.X, Key.C, Key.F1, Key.F5, Key.F9, Key.Escape, Key.Space, Key.Digit1 })"
assert text.count(msg) == 1 and text.count(loop) == 1
text = text.replace(msg, '"no key opened a forest panel or advanced time" + $" [year={manager.CurrentEcologicalYear} workPlanOpen={manager.WorkPlanOpen} screen={ui.CurrentScreen} helpOpen={ui.Help.IsOpen} helpMenu={ui.Help.CurrentMenu} menuOpen={StandaloneSessionMenu.IsOpen} startupActive={menu.StartupScreenActive}]");')
before = ('{ string dir = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT"); Directory.CreateDirectory(dir); yield return new WaitForEndOfFrame(); '
          'Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture(); File.WriteAllBytes(Path.Combine(dir, "fresh-device-title-before-any-key.png"), shot.EncodeToPNG()); UnityEngine.Object.Destroy(shot); }\n            '
          'Debug.Log("DIAG_BEFORE_KEYS helpOpen=" + ui.Help.IsOpen + " helpMenu=" + ui.Help.CurrentMenu + " uiEnabled=" + ui.enabled + " introducedWalkingHud=" + MenuHelpView.Introduced(MenuHelpView.Menu.WalkingHud));\n            ')
Path(sys.argv[1]).write_text("// DERIVED DIAGNOSTIC - see make_startup_diagnostic.py. No assertion changed.\n" + text.replace(loop, before + loop))
print("wrote", sys.argv[1])
