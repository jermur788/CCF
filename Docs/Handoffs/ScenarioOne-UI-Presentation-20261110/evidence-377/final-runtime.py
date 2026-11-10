import subprocess,os,json,time
from pathlib import Path
r=Path(__file__).parent
env=dict(os.environ,DISPLAY=":86")
def cmd(d):subprocess.run(["python3",str(r/"command.py"),json.dumps(d)],check=True)
def key(k):subprocess.run(["xdotool","key","--clearmodifiers",k],env=env,check=True);time.sleep(.7)
cmd({"action":"show","arg":"review"})
cmd({"action":"click","arg":"annual-review-acknowledge"})
cmd({"action":"dump","arg":"annual-after-ack"})
cmd({"action":"show","arg":"forest"})
key("F5")
cmd({"action":"trees"})
key("F9");key("Escape")
cmd({"action":"dump","arg":"after-native-save-reload"})
cmd({"action":"aim","arg":"P0710"});key("e");key("Escape")
cmd({"action":"capture","arg":"reopened-after-save"})
# A held Shift keeps modifier state across multiple Unity frames.
subprocess.run(["xdotool","keydown","Shift_L"],env=env,check=True);time.sleep(.7)
subprocess.run(["xdotool","key","v"],env=env,check=True);time.sleep(.7)
subprocess.run(["xdotool","keyup","Shift_L"],env=env,check=True)
cmd({"action":"capture","arg":"held-shift-previous"})
print("FINAL_RUNTIME_COMPLETE")
