from pathlib import Path
import os,json,time,subprocess,shutil
r=Path(__file__).parent;e=r/"run/evidence";env=dict(os.environ,DISPLAY=":86")
def command(d):subprocess.run(["python3",str(r/"command.py"),json.dumps(d)],check=True)
def key(k):
 subprocess.run(["xdotool","key","--clearmodifiers","--delay","120",k],env=env,check=True);time.sleep(1)
for i in range(900):
 if (e/"READY").exists():break
 time.sleep(1)
else:raise RuntimeError("No ready state")
paths=(e/"paths.txt").read_text();assert str(r/"run/config") in paths and "F5/F9=" in paths and "prefs=" in paths
print("PATHS_PROVEN",paths,flush=True)
command({"action":"focus"})
fixture=Path("/media/jer/Files/CCF/Verification/CCF-369-RuntimeCoverage-20261110/year2-genuine-three-crop-trees.json")
shutil.copy2(fixture,r/"run/config/unity3d/DefaultCompany/CCF/forest-save.json")
key("F9");command({"action":"show","arg":"forest"});command({"action":"aim","arg":"P0709"})
assert "aimed=P0709" in (e/"aim-P0709-state.txt").read_text()
command({"action":"focus"});key("x");command({"action":"dump","arg":"native-marked"})
assert "FELL 1" in (e/"native-marked-ui.tsv").read_text()
key("Tab");command({"action":"dismiss"});command({"action":"dump","arg":"pending-before-approve"})
assert "1 pending" in (e/"pending-before-approve-ui.tsv").read_text()
command({"action":"click","arg":"workplan-approve"});command({"action":"dump","arg":"approved-before-advance"})
assert "year=2" in (e/"approved-before-advance-state.txt").read_text() and "1 approved" in (e/"approved-before-advance-ui.tsv").read_text()
command({"action":"click","arg":"workplan-advance"});command({"action":"show","arg":"review"});command({"action":"dismiss"})
for width,height in [(1280,720),(1920,1080)]:
 command({"action":"size","width":width,"height":height})
 stem="01-annual-year3-repaired-"+str(width)
 command({"action":"annualfit","arg":stem})
 state=(e/(stem+f"-{width}x{height}-state.txt")).read_text();assert "year=3" in state and "unread=True" in state
 text=(e/(stem+f"-{width}x{height}-ui.tsv")).read_text()
 for expected in ["Timber income","Contractor work","Recorded work materials","Income less contractor work","calculated from records","2496.39","7729.15"]:assert expected in text,expected
(r/"first-gate-actions-complete").write_text("Both raw captures and money-fit measurements produced; operator visual judgement still required")
print("FIRST_GATE_ACTIONS_COMPLETE",flush=True)
