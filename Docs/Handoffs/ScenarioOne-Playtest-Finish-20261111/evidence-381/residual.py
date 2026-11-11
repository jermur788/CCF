from pathlib import Path
import os,subprocess,json,time,shutil
r=Path(__file__).parent;e=r/'run/evidence';env=dict(os.environ,DISPLAY=':86')
def cmd(d):subprocess.run(['python3',str(r/'command.py'),json.dumps(d)],check=True)
def native(*args):subprocess.run(['xdotool',*args],env=env,check=True);time.sleep(.7)
paths=(e/'paths.txt').read_text();assert str(r/'run/config') in paths and 'F5/F9=' in paths and 'prefs=' in paths
cmd({'action':'pick','x':300,'y':680,'arg':'planting-disabled'})
native('mousemove','820','666','click','1');cmd({'action':'dump','arg':'planting-native-click'})
native('mousemove','1000','640','click','--repeat','8','--delay','100','5')
cmd({'action':'capture','arg':'objective1-lower-native-scroll'})
native('mousemove','850','620','click','1');cmd({'action':'capture','arg':'objective1-lower-native-click'})
cmd({'action':'size','width':1920,'height':1080});cmd({'action':'capture','arg':'objective1-1080'})
cmd({'action':'show','arg':'forest'});cmd({'action':'focus'});native('key','F5')
p=r/'run/config/unity3d/DefaultCompany/CCF/forest-save.json';assert p.exists();before=p.read_bytes();native('key','F9');assert p.read_bytes()==before
cmd({'action':'dump','arg':'native-disposable-roundtrip'})
fixture=Path('/media/jer/Files/CCF/Verification/CCF-369-RuntimeCoverage-20261110/year2-genuine-three-crop-trees.json');shutil.copy2(fixture,p)
native('key','F9');cmd({'action':'dump','arg':'genuine-year2-loaded'})
cmd({'action':'aim','arg':'P0710'});cmd({'action':'focus'});native('key','e');cmd({'action':'size','width':1280,'height':720});cmd({'action':'capture','arg':'rank-focus-720'})
native('key','v');cmd({'action':'dump','arg':'v-forward'});native('keydown','Shift_L');native('key','v');native('keyup','Shift_L');cmd({'action':'capture','arg':'held-shift-v-720'})
cmd({'action':'size','width':1920,'height':1080});cmd({'action':'capture','arg':'rank-focus-1080'})
print('RESIDUAL_ACTIONS_COMPLETE',flush=True)
