from pathlib import Path
import subprocess,os,time,json
import sys
ROOT=Path(sys.argv[1])
OUT=ROOT/'evidence'
STATE=OUT/'real-input-state.txt'
ENV=dict(os.environ,DISPLAY=':84')
actions=[]
def state():
 try:return dict(line.split('=',1) for line in STATE.read_text().splitlines() if '=' in line)
 except FileNotFoundError:return {}
def record(label):
 item={'time':time.time(),'action':label,'state':state()}
 actions.append(item);(ROOT/'real-input-actions.json').write_text(json.dumps(actions,indent=2)+'\n')
def wait(check, seconds=45):
 end=time.monotonic()+seconds
 while time.monotonic()<end:
  if check(state()):return
  time.sleep(.25)
 record('WAIT FAILED');raise RuntimeError('State wait failed: '+str(state()))
def x(*args):
 subprocess.run(['xdotool',*map(str,args)],env=ENV,check=True)
 record('xdotool '+' '.join(map(str,args)))
 time.sleep(.45)
wait(lambda s:s.get('phase')=='WAIT_REAL_INPUT',360)
wins=subprocess.check_output(['xdotool','search','--onlyvisible','--class','Unity'],env=ENV,text=True).split()
x('windowactivate','--sync',wins[-1])
x('key','ctrl+space')
time.sleep(2.5)
x('key','b')
wait(lambda s:s.get('help')=='True' or s.get('active')=='True')
if state().get('help')=='True':x('key','Escape')
wait(lambda s:s.get('active')=='True')
x('click',1)
wait(lambda s:s.get('origin')=='True')
(OUT/'state-first-origin.txt').write_text(STATE.read_text())
wait(lambda state:(OUT/'05-origin-1-1280x720.png').exists())
time.sleep(.7)
angle=float(state()['angle'])
x('mousemove_relative','--sync',160,0)
def turned(s): return abs((float(s.get('angle',angle))-angle+180)%360-180)>10
extra = not turned(state())
if extra: x('mousemove_relative','--sync',100,0)
wait(turned)
(OUT/'state-swivel.txt').write_text(STATE.read_text())
wait(lambda state:(OUT/'06-swivel-fixed-origin-1280x720.png').exists())
time.sleep(1)
x('mousemove_relative','--sync','--',-100 if extra else -160,0)
x('click',3)
wait(lambda s:s.get('origin')=='False')
x('click',1)
wait(lambda s:s.get('origin')=='True')
wait(lambda state:(OUT/'05-origin-2-1280x720.png').exists())
time.sleep(.7)
x('key','Escape')
wait(lambda s:s.get('active')=='False')
x('key','b')
wait(lambda s:s.get('active')=='True')
x('click',1)
wait(lambda s:s.get('origin')=='True' and s.get('valid')=='True')
wait(lambda state:(OUT/'05-origin-3-1280x720.png').exists())
time.sleep(.7)
(OUT/'state-before-confirm.txt').write_text(STATE.read_text())
x('click',1)
record('Real input sequence delivered; Unity fixture independently checks confirmation/progression')
print('REAL_INPUT_SEQUENCE_DELIVERED',flush=True)
