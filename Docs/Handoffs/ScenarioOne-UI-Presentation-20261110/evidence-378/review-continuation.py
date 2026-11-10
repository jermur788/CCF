from pathlib import Path
import subprocess,os,json,time,re
r=Path(__file__).parent;e=r/'run/evidence';env=dict(os.environ,DISPLAY=':86')
def command(d):subprocess.run(['python3',str(r/'command.py'),json.dumps(d)],check=True)
def key(k):subprocess.run(['xdotool','key','--clearmodifiers','--delay','120',k],env=env,check=True);time.sleep(1)
def unread(stem,expected):assert ('unread='+str(expected)) in (e/(stem+'-state.txt')).read_text()
command({'action':'click','arg':'◀ Earlier year'})
command({'action':'dump','arg':'historical-before-sources'})
text=(e/'historical-before-sources-ui.tsv').read_text();title=next(line.split('\t')[-1] for line in text.splitlines() if 'Annual Review · Year' in line);assert 'Year 3 complete' not in title
assert 'Current Year 3' in text
assert not any('annual-review-acknowledge\t' in line for line in text.splitlines())
unread('historical-before-sources',True)
command({'action':'toggle','arg':'annual-show-data-sources','width':1})
for w,h in [(1920,1080),(1280,720)]:
 command({'action':'size','width':w,'height':h})
 command({'action':'annualfit','arg':'02-historical-sources-'+str(w)})
 text=(e/f'02-historical-sources-{w}-{w}x{h}-ui.tsv').read_text();assert title in text and 'Current Year 3' in text and 'prov-recorded' in text
command({'action':'click','arg':'Later year ▶'})
command({'action':'dump','arg':'latest-sources'})
assert 'Year 3 complete' in (e/'latest-sources-ui.tsv').read_text();unread('latest-sources',True)
command({'action':'foldout','arg':'Detailed annual records','width':1})
command({'action':'scroll','arg':'work-detail-toggle-done-FellTree'})
for w,h in [(1280,720),(1920,1080)]:
 command({'action':'size','width':w,'height':h})
 command({'action':'annualfit','arg':'03-expanded-records-'+str(w)})
command({'action':'show','arg':'review'})
key('Escape');command({'action':'dump','arg':'back-before-ack'});unread('back-before-ack',True)
command({'action':'show','arg':'review'});command({'action':'click','arg':'annual-review-acknowledge'});command({'action':'dump','arg':'after-explicit-ack'});unread('after-explicit-ack',False)
command({'action':'show','arg':'forest'});command({'action':'focus'});key('F5');command({'action':'trees'})
print('REVIEW_CONTINUATION_COMPLETE')
