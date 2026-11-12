import datetime
import hashlib
import json
import shutil
import subprocess
import sys
from pathlib import Path

LANE = Path('/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual')
ROOT = Path('/media/jer/Files/CCF/Verification/D073-FR10-427-20261112')
PRIOR = Path('/media/jer/Files/CCF/Verification/D073-YearComparison-417-20261112')
LOCK = '/media/jer/ZX20/CCF-Ops/scripts/unity-lock'
BRANCH = 'qualify/scenario-one-d073-fr10-visible-provenance-unity-20261112-v1'
SOURCE = '339517ed2d49bb3d5716fc9cf029d9ef0eaeee72'
OWNER = 'OpenAI Codex gpt-6.1-sol HIGH #427'
TASK = 'CCF-427-D073-FR10'

def git(*arguments):
    return subprocess.check_output(['git', *arguments], cwd=LANE, text=True)

def manifest(paths, base=Path('/')):
    return {str(path): hashlib.sha256((base/path).read_bytes()).hexdigest() if (base/path).is_file() else None for path in paths}

def write(name, value):
    (ROOT/name).write_text(json.dumps(value, indent=2)+'\n')

def state():
    return json.loads(subprocess.check_output([LOCK,'status','--json'],text=True))

operation = sys.argv[1]
if operation == 'baseline':
    assert not (ROOT/'host-before.json').exists()
    write('host-before.json',manifest(json.loads((PRIOR/'custody-before.json').read_text())))
    write('production-before.json',manifest(json.loads((PRIOR/'production-before.json').read_text()),LANE))
    assert len(json.loads((ROOT/'host-before.json').read_text())) == 37
    assert len(json.loads((ROOT/'production-before.json').read_text())) == 4011
elif operation == 'start':
    tag = sys.argv[2]
    current = state(); assert current['status']=='free' and not current['unity_editor_pids'],current
    assert git('status','--porcelain=v1','-uall') == ''
    assert git('diff',SOURCE,'--','Assets','Packages','ProjectSettings') == ''
    assert shutil.disk_usage(LANE).free > 15*2**30
    before=json.loads((ROOT/'host-before.json').read_text()); assert manifest(before)==before
    head=git('rev-parse','HEAD').strip()
    board=Path('/media/jer/ZX20/CCF-Ops/board/active.md')
    lines=board.read_text().splitlines(keepends=True)
    position=next(index for index,line in enumerate(lines) if line.startswith('| CCF-412-PREDATOR-FABLE-A-D073 |'))
    cells=lines[position].split('|');assert cells[7].strip()=='ResearchCopyQual391'
    cells[8]=' '+BRANCH+' ';cells[9]=' '+head+' ';lines[position]='|'.join(cells)
    board.write_text(''.join(lines));(ROOT/(tag+'-board.md')).write_text(''.join(lines))
    subprocess.run(['/media/jer/ZX20/CCF-Ops/scripts/packet-lint','--board',str(board)],check=True)
    command=[LOCK,'acquire','--owner',OWNER,'--task-id',TASK,'--lane',str(LANE),'--branch',BRANCH,'--head',head,'--eta',(datetime.datetime.now(datetime.timezone.utc)+datetime.timedelta(hours=2)).isoformat(),'--notes','#427 targeted FR10 oracle; candidate 339517ed; sole lane; no builds; isolated saves; full matrix continues after failures','--json']
    result=subprocess.run(command,capture_output=True,text=True)
    (ROOT/(tag+'-acquire.json')).write_text(result.stdout)
    assert result.returncode==0,result.stderr
    print(result.stdout)
elif operation == 'finish':
    tag=sys.argv[2];current=state();assert not current['unity_editor_pids'],current
    meta=current['metadata'];assert meta['owner']==OWNER and meta['task_id']==TASK
    before=json.loads((ROOT/'host-before.json').read_text());after=manifest(before)
    write(tag+'-host-after.json',after)
    (ROOT/(tag+'-Editor-drift.patch')).write_text(git('diff','--binary'))
    paths=git('diff','--name-only').splitlines()
    permitted={'.vscode/settings.json',*[f'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_{part}.mat' for part in ['Flower_Centre','Leaves','Stems']]}
    assert set(paths)<=permitted,paths
    for path in paths:
        destination=ROOT/(tag+'-drift-bytes')/path;destination.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(LANE/path,destination)
    if paths: subprocess.run(['git','checkout','--',*paths],cwd=LANE,check=True)
    production=json.loads((ROOT/'production-before.json').read_text());actual=manifest(production,LANE)
    write(tag+'-production-after.json',actual)
    write(tag+'-custody-result.json',{'host_count':len(after),'host_identical':before==after,'production_count':len(actual),'production_identical':production==actual,'restored_paths':paths,'clean':git('status','--porcelain=v1','-uall')==''})
    result=subprocess.run([LOCK,'release','--owner',OWNER,'--task-id',TASK,'--lock-id',meta['lock_id'],'--reason','Editor exited; staged harness/meta removed; known drift archived and restored; host and production manifests recorded','--json'],capture_output=True,text=True)
    (ROOT/(tag+'-release.json')).write_text(result.stdout);assert result.returncode==0,result.stderr;print(result.stdout)
    assert before==after and production==actual
    assert git('status','--porcelain=v1','-uall')==''
else:
    raise ValueError(operation)
