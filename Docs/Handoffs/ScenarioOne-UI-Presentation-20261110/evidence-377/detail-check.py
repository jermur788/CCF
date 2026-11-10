import subprocess,os,json,re,time
from pathlib import Path
root=Path(__file__).parent
env=dict(os.environ,DISPLAY=":86")
def cmd(d): subprocess.run(["python3",str(root/"command.py"),json.dumps(d)],check=True)
def key(k):
 subprocess.run(["xdotool","key","--clearmodifiers",k],env=env,check=True)
 time.sleep(.35)
def bounds(stem,size):
 path=root/"run/evidence"/(stem+"-"+size+"-ui.tsv")
 lines=path.read_text().splitlines()
 panel=next(x for x in lines if "\tinspect-column\t" in x)
 def rect(x): return list(map(float,re.search(r"\(x:([\d.-]+), y:([\d.-]+), width:([\d.-]+), height:([\d.-]+)\)",x).groups()))
 x,y,w,h=rect(panel)
 failures=[]
 active=False
 for line in lines[lines.index(panel)+1:]:
  if "\tinspection-page\t" in line: active=True
  if active and line.startswith(("Label\t","Button\t")):
   a,b,c,d=rect(line)
   if b<y-.5 or b+d>y+h+.5 or a<x-.5 or a+c>x+w+.5: failures.append(line)
 record={"capture":stem+"-"+size,"panel":panel,"overflow":failures}
 with (root/"detail-bounds.jsonl").open("a") as f:f.write(json.dumps(record)+"\n")
 if failures: raise RuntimeError("STOP: clipping "+str(record))
for width,height in [(1920,1080),(1280,720)]:
 cmd({"action":"size","width":width,"height":height})
 for page in range(2,18):
  key("v")
  stem="detail-page-%02d"%page
  cmd({"action":"capture","arg":stem})
  bounds(stem,f"{width}x{height}")
 key("v")
 cmd({"action":"dump","arg":f"wrapped-{width}"})
 key("shift+v")
 cmd({"action":"dump","arg":f"reverse-{width}"})
 key("v")
print("DETAIL_COMPLETE")
