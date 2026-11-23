#!/bin/bash
# review-probe-r3d.mjs = review-probe-r3c.mjs + the small RALERT section (inserted before the legacy P6 section). r3c is left exactly as run.
D=/mnt/c/Users/hp/AppData/Local/Temp/review-r3/probe
python3 -I - "$D" <<'PY'
import sys
d = sys.argv[1]
s = open(d + '/review-probe-r3c.mjs', encoding='utf-8').read()
part = open(d + '/part-D4-alert.mjs.part', encoding='utf-8').read()
anchor = "    // ===== P6: layout with Inspect ground ON"
assert s.count(anchor) == 1, 'anchor'
s = s.replace(anchor, part + "\n" + anchor)
s = s.replace('review-probe-r3c.mjs', 'review-probe-r3d.mjs').replace('RA11Y RMETA)', 'RA11Y RMETA RALERT)')
open(d + '/review-probe-r3d.mjs', 'w', encoding='utf-8', newline='\n').write(s)
print('r3d lines', s.count('\n'))
PY
echo "RALERT sections: $(grep -c "await section('RALERT'" $D/review-probe-r3d.mjs)"
