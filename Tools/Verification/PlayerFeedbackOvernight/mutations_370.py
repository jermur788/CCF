#!/usr/bin/env python3
"""#370 named mutation: AdvanceYear without the unread reset must die on the specific Year 2 unread assertion.
Usage: mutations_370.py <check.dll> <out dir> <source root>"""
import pathlib, subprocess, sys
sys.dont_write_bytecode = True
here = pathlib.Path(__file__).resolve().parent
assembly, out, root = (pathlib.Path(a).resolve() for a in sys.argv[1:4])
name, expected = 'advance-year-omits-unread-reset', 'R370_YEAR2_NO_WORK_REPORT_UNREAD'
d = out/name; d.mkdir(parents=True, exist_ok=True)
proc = subprocess.run([sys.executable, '-B', str(here/'review_reset.py'), str(assembly), str(d), str(root), '--mutant', name], capture_output=True, text=True)
log = proc.stdout + proc.stderr; (d/'assertions.txt').write_text(log)
assert proc.returncode != 0 and 'Unhandled exception.' in log and expected in log, 'R370_MUTANT_SURVIVED_OR_WRONG_FAILURE: ' + name + '\n' + log[-800:]
print(name + ': KILLED by "' + expected + '" (mutated production slice compiled)')
