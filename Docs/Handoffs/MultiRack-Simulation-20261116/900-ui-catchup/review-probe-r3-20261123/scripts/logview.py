import re, sys
tag, pat = sys.argv[1], sys.argv[2]
mx = int(sys.argv[3]) if len(sys.argv) > 3 else 700
p = f'/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out/{tag}/run.log'
rx = re.compile(pat)
sec = ''
for line in open(p, encoding='utf-8', errors='replace'):
    line = line.rstrip('\n')
    line = re.sub(r'/s/[0-9a-f]{64}/', '/s/<capability>/', line)
    if line.startswith('--- section'):
        sec = line[12:]; print('####', sec); continue
    if rx.search(line):
        print(line[:mx] + (' …' if len(line) > mx else ''))
