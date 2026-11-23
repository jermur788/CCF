import json, re
for vp, tag in (('1280x720', 'fin2-ft-1280'), ('1920x1080', 'fin2-ft-1920')):
    p = f'/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out/{tag}/run.log'
    print('=' * 8, vp)
    for line in open(p, encoding='utf-8', errors='replace'):
        line = line.rstrip('\n')
        if 'D4 (not a requirement)' in line:
            print(line[:600])
        if 'light index text by injected value' in line:
            rows = json.loads(line.split(' :: ', 1)[1])
            print('L, summary, technical, announcement, word, noLight')
            for r in rows:
                print(f"  {r['L']:<11} summary={r['summaryIndex']!r:<9} technical={r['technicalIndex']!r:<8} announced={r['announcedIndex']!r:<9} word={r['word']!r:<14} noLight={r['noLight']}")
        if line.startswith(('PASS', 'FAIL')) and 'F-07' in line:
            print(line[:200])
        if line.startswith(('PASS', 'FAIL')) and 'F-03' in line:
            print(line[:260])
