#!/bin/bash
# Final assembly -> review-probe-r3c.mjs  (assemble2 builds r3b; then the P7 follow-up patch; the as-run files are never overwritten).
bash /mnt/c/Users/hp/AppData/Local/Temp/review-r3/assemble2.sh > /dev/null
D=/mnt/c/Users/hp/AppData/Local/Temp/review-r3/probe
sed -e "s#(RS0-RS4 RDOC RADD RINJ RESC RF07 RVP RCARD2 RA11Y)#(RS0-RS4 RDOC RADD RINJ RESC RF07 RVP RCARD2 RA11Y RMETA)#" -e "s#review-probe-r3b.mjs#review-probe-r3c.mjs#" $D/review-probe-r3b.mjs > $D/review-probe-r3c.mjs
python3 -I /mnt/c/Users/hp/AppData/Local/Temp/review-r3/patch-legacy.py $D/review-probe-r3c.mjs
echo "lines: $(wc -l < $D/review-probe-r3c.mjs); sections: $(grep -o "await section('[A-Za-z0-9]*'" $D/review-probe-r3c.mjs | wc -l)"
grep -c "await section('RMETA'" $D/review-probe-r3c.mjs
grep -c "createElement('style')" $D/page-helpers.js || true
