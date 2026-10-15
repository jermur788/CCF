# Source in a dedicated Bash task shell; no global cache/account configuration.
CCF_OPS_ENV_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd -P)"
if ! python3 -B - "$CCF_OPS_ENV_DIR" <<'PY_CHECK'
import sys
from pathlib import Path
ops=Path(sys.argv[1]);sys.path.insert(0,str(ops/'scripts'))
from _ccf import config_read,require_storage,on_storage
cfg,_=config_read(ops/'lanes.json');health=require_storage(cfg)
for name in ['CCF-Temp','CCF-Builds','CCF-Verification']:
 p=on_storage(Path(cfg['storage']['root'])/name)
 if not p.is_dir():raise ValueError('missing output directory: '+str(p))
print(f"PASS: {health['backing_device']} rw; free={health['free_bytes']} bytes; minimum={health['minimum_free_bytes']} bytes")
PY_CHECK
then
  return 1 2>/dev/null || exit 1
fi
export ZX20_ROOT=/media/jer/ZX20
export TMPDIR="$ZX20_ROOT/CCF-Temp" TMP="$ZX20_ROOT/CCF-Temp" TEMP="$ZX20_ROOT/CCF-Temp"
export XDG_CACHE_HOME="$ZX20_ROOT/CCF-Temp/cache" PYTHONDONTWRITEBYTECODE=1
