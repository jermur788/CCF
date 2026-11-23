#!/bin/bash
# Assemble review-probe-r3b.mjs. Legacy sections are my own R2 probe, line ranges unchanged, with ONE patch applied to its two wait
# conditions (selectById / waitCompetition): a ".comp-empty" paragraph only counts when it is not the "Reading competition…" placeholder.
O=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4OwnerUX-R2Review-Docs-20261123/Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/review-probe-r2-20261123/review-probe.mjs
D=/mnt/c/Users/hp/AppData/Local/Temp/review-r3/probe
OUT=$D/review-probe-r3b.mjs
r() { sed -n "$1,$2p" "$O"; }
{
  echo "// Independent re-review probe for MULTI Stage 4 owner UX repair, Phase A, candidate 0e777455 (R3 re-review). Windows node v26."
  echo "// Written by the reviewer. Legacy sections (P0 P0b P3 P4 P4b P5 P6 P7 P7b P9 PN PZ) are the reviewer's own R2 probe, line-for-line,"
  echo "// except that selectById/waitCompetition no longer accept the 'Reading competition…' placeholder as a loaded competition (R3 patch)."
  echo "// The R3 sections (RS0-RS4 RDOC RADD RINJ RESC RF07 RVP RCARD2 RA11Y) are new. It does not import the writer's scenario or driver."
  echo "//   node review-probe-r3b.mjs <outDir> <fileHoldingBaseUrl> <W>x<H> [sectionList]"
  r 5 111
  r 284 298
  r 494 520
  cat $D/part-A-sweeps.mjs.part
  r 112 175
  cat $D/part-B-card.mjs.part
  r 305 353
  r 354 492
  r 521 589
  cat $D/part-C-esc.mjs.part
  cat $D/part-D-vp.mjs.part
  cat $D/part-D2-card2.mjs.part
  cat $D/part-D3-a11y.mjs.part
  r 591 647
  r 649 771
  r 773 818
  r 820 877
  r 879 915
} | sed "s#!!document.querySelector('\#competition-content .comp-empty')#(!!document.querySelector('\#competition-content .comp-empty') \&\& !/Reading competition/.test(document.getElementById('competition-content').textContent))#g" > "$OUT"
sed -i 's/\r$//' "$OUT"
echo "lines: $(wc -l < $OUT)"
echo "patched wait conditions: $(grep -c "Reading competition/.test(document.getElementById('competition-content')" $OUT)"
grep -c "await section('RVP'\|await section('RCARD2'\|await section('RA11Y'" "$OUT"
