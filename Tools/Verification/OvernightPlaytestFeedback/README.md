# Overnight playtest feedback — source baseline (#260)

Written against published player-test source commit `72b4efbd1c653fa2231f632a79030a83ff76f833`, independently of fixes #255/#256. Python 3 standard library and Git only; no installation required.

From this checkout's root:

```bash
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py
```

Every result is **SOURCE-STATIC**. The checker reads the current checkout, quotes exact C# literal fragments / expressions with file and line, and compares tracked production files against the recorded commit. It never changes source, existing tests, preferences, saves or Unity assets, and never launches Unity. It is independent of the working directory. Output has no timestamps or changing verification-branch HEAD, so identical source produces identical output.

Exit 1 means at least one feedback assertion is unmet, including the deliberately retained Objective 1 baseline failure; exit 0 means the lexical assertions pass, not runtime qualification. NOT-TESTABLE observations never count as PASS. Exit 2 means missing source/anchor or another read/parse failure: inspect before interpreting any partial tally. An added/untracked production file is outside the tracked Git comparison; the C# inventory still reads it.

Checks cover the V2 Objective 1 concept's compact metre formatting, exact requested heading literal, source-material phrase, the Tree Marks legend's separate orange rack-removal and red Fell entries, Pro Silva mentions, rotation snap expressions, next-instruction path, primary CTA source and shared map/minimap source paths. The requested “duration” is actually rack spacing in metres. Both ASCII hyphen and the candidate's en dash are accepted; the disputed space before m is rejected.

The phrase inventory scans ordinary/verbatim C# string tokens under `Assets/ForestPrototype/`, ignoring comments. It is deliberately conservative: a Pro Silva mention fails for human review rather than guessing whether it asserts a formal standard. It is not a full C# parser, evaluates no expressions/conditional compilation/interpolation, joins no dynamically composed text, and scans no serialized assets, localisation, external resources or binaries. The orange/Fell assertion is specifically the existing legend and separation sentence; it is not an exhaustive semantic language detector. Missing expected anchors fail with exit 2 rather than silently passing. Any later refactor needs deliberate checker review; do not relax it to hide a baseline failure.

**Nothing here proves visual layout, input behaviour or rendered correctness.** Exact heading presence alone cannot prove a heading is displayed. A shared map source cannot prove trail parity. Source `btn-primary` is not a measurement of user attention or keyboard focus. No forestry convention, Pro Silva standard, compile gate, simulation, persistence or platform qualification is established.

`baseline-output.txt` is the raw stdout of the baseline run (exit 1), preserved unchanged. Two executions produced byte-identical output and no stderr. Later runs should be saved separately inside an owned evidence path; do not overwrite this historic baseline. Manual Unity steps and the full limitations are in `Docs/Handoffs/Overnight-Continuation-20261104/BASELINE-VERIFICATION-HANDOFF.md`.
