import subprocess,os,json,time
from pathlib import Path
r=Path(__file__).parent
env=dict(os.environ,DISPLAY=":86")
def cmd(d):subprocess.run(["python3",str(r/"command.py"),json.dumps(d)],check=True)
def key(k):
 subprocess.run(["xdotool","key","--clearmodifiers",k],env=env,check=True);time.sleep(.5)
for n in range(1,6):
 key(str(n));cmd({"action":"dump","arg":f"native-rank-{n}"})
key("e");key("Tab")
cmd({"action":"capture","arg":"workplan-year2-native-tab"})
cmd({"action":"size","width":1920,"height":1080})
cmd({"action":"capture","arg":"workplan-year2-native-tab"})
print("WORKPLAN_CAPTURED")
