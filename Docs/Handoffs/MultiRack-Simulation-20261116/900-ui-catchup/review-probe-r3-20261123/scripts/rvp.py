import re, json
p = '/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out/fin2-vp-1280/run.log'
for line in open(p, encoding='utf-8', errors='replace'):
    if 'viewport and pixel-ratio sampling' in line:
        j = json.loads(line.split(' :: ', 1)[1])
        print(f"{'size':<10} {'note':<52} {'fit':<7} {'worstOverlapPx':>14} {'rows':>7} {'add':>6} {'freeRoomBelowDockPx':>20}")
        for r in j:
            print(f"{r['size']:<10} {(r['note'] or '-'):<52} {r['treesFitting']:<7} {r['worstRowOverlapPx']:>14} {r['rowsReachable']:>7} {r['addReachable']:>6} {r['minFreeRoomBelowDockPx']:>20}")
