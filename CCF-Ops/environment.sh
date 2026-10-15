# Source in a dedicated CCF task shell. No credentials or account setup.
if ! python3 -B - <<'PY_CHECK'
import json, subprocess, pathlib
p=pathlib.Path('/media/jer/ZX20').resolve(strict=True)
i=json.loads(subprocess.check_output(['findmnt','-J','-T',str(p),'-o','TARGET,FSTYPE,UUID'],text=True))['filesystems'][0]
assert i['target']==str(p) and i['fstype']=='exfat' and (i['uuid'] or '').upper()=='18D6-896E', 'ZX20 mount mismatch'
for name in ['CCF-Temp','CCF-Builds','CCF-Verification']:
 q=p/name
 assert q.is_dir() and q.resolve().is_relative_to(p) and q.stat().st_dev==p.stat().st_dev, 'unsafe/missing output path: '+str(q)
PY_CHECK
then
  return 1 2>/dev/null || exit 1
fi
export ZX20_ROOT=/media/jer/ZX20
export TMPDIR="$ZX20_ROOT/CCF-Temp" TMP="$ZX20_ROOT/CCF-Temp" TEMP="$ZX20_ROOT/CCF-Temp"
export XDG_CACHE_HOME="$ZX20_ROOT/CCF-Temp/cache" PYTHONDONTWRITEBYTECODE=1
