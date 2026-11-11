from pathlib import Path
import json,time,subprocess as s,os,re
r=Path(__file__).parent;e=r/'residual2/evidence';env=dict(os.environ,DISPLAY=':89')
def cmd(d):
 p=e/'commands'/(str(time.time_ns())+'.json');p.with_suffix('.tmp').write_text(json.dumps(d));p.with_suffix('.tmp').rename(p)
 for i in range(450):
  if Path(str(p)+'.failed').exists():raise Exception(Path(str(p)+'.error').read_text())
  if Path(str(p)+'.done').exists():return
  time.sleep(.2)
 raise Exception('command timeout '+str(d))
def native(*a):s.run(['xdotool',*a],env=env,check=True,timeout=10);time.sleep(.7)
def dump(n):cmd({'action':'dump','arg':n});return (e/(n+'-state.txt')).read_text()
def identity(t):return re.search(r'orders=.*? help=',t).group().split(' context=')[0]
while not (e/'READY').exists():time.sleep(1)
try:
 for w,h in [(1280,720),(1920,1080)]:
  cmd({'action':'size','width':w,'height':h});cmd({'action':'show','arg':'workplan'});cmd({'action':'focus'})
  baseline=identity(dump('before-'+str(w)))
  for card in ['Planting','Pruning','Nursery — planting stock']:
   tag=card.split()[0]+'-'+str(w);cmd({'action':'scrollcard','arg':card});cmd({'action':'target','arg':card})
   x,y=map(float,(e/'target.txt').read_text().split(','));native('mousemove',str(round(x)),str(round(y)),'click','1')
   cmd({'action':'mousepick','arg':tag});after=dump(tag);assert identity(after)==baseline,(card,after)
   cmd({'action':'capture','arg':tag})
 cmd({'action':'progress'});cmd({'action':'click','arg':'Treatment review'});cmd({'action':'candidates'});candidate=(e/'candidates.txt').read_text().splitlines()[0]
 for w,h in [(1280,720),(1920,1080)]:
  cmd({'action':'size','width':w,'height':h});cmd({'action':'aim','arg':candidate});cmd({'action':'focus'});native('key','e');cmd({'action':'dismiss'});cmd({'action':'focus'})
  for key,label,expected in [('c','confirm','Confirmed'),('c','undo','Open'),('n','reject','Rejected'),('n','reopen','Open')]:
   native('key',key);state=dump(label+'-'+str(w));assert 'context='+expected in state,state
   cmd({'action':'capture','arg':label+'-'+str(w)})
  native('key','Escape')
 cmd({'action':'aim','arg':candidate});cmd({'action':'focus'});native('key','x');cmd({'action':'show','arg':'workplan'});cmd({'action':'capture','arg':'third-cycle-pending'})
 for year in [3,4]:
  cmd({'action':'click','arg':'workplan-advance'});cmd({'action':'click','arg':'Advance anyway'});cmd({'action':'capture','arg':'idle-year-'+str(year)});assert 'year='+str(year)+' ' in dump('idle-'+str(year))
  cmd({'action':'click','arg':"I've read the annual results"});cmd({'action':'show','arg':'workplan'})
 cmd({'action':'capture','arg':'stale-pending-year4'});cmd({'action':'click','arg':'workplan-approve'});cmd({'action':'click','arg':'workplan-advance'});cmd({'action':'capture','arg':'third-cycle-executed'});assert 'year=5 ' in dump('third-executed')
 cmd({'action':'click','arg':"I've read the annual results"});cmd({'action':'show','arg':'review'});before=identity(dump('before-history'))
 for i in range(3):cmd({'action':'click','arg':'◀ Earlier year'})
 cmd({'action':'capture','arg':'historical-year2'});assert identity(dump('after-history'))==before
 cmd({'action':'show','arg':'forest'});cmd({'action':'focus'});native('key','F5');before=identity(dump('before-reload'));native('key','F9');assert identity(dump('after-reload'))==before
 print('FOCUSED_RESIDUAL_PASS pointer identity checks, native C/N both sizes, idle/carried third cycle, historical read-only, native save/load',flush=True)
finally:
 try: cmd({'action':'exit'})
 except Exception as ex: print('exit acknowledgement:',ex,flush=True)
