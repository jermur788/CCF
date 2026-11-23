import sys
# Patch the reviewer's own R2 P7 follow-up check (premise changed once selectById waits for the competition read to finish).
p = sys.argv[1]
s = open(p, encoding='utf-8').read()
old_pre = "        const srvBefore = await rvx(`rv.snap()`);\n        await page.click('#crop-button'); await sleep(900);"
new_pre = ("        const srvBefore = await rvx(`rv.snap()`);\n"
           "        const healedBefore = await ev(`(() => { const tr = [...document.querySelectorAll('#tree-rows tr')].find((r) => r.textContent.includes(${J(C0)})); return tr ? /crop/i.test(tr.textContent) : null; })()`);\n"
           "        await page.click('#crop-button'); await sleep(900);")
assert s.count(old_pre) == 1, 'pre-anchor'
s = s.replace(old_pre, new_pre)
start = s.index("        check(fo.shown && /Refused|stale|PlayerPlanStateStale|PlayerSourceStale/i.test(fo.alert)")
end = s.index("revAfter: srvAfter.rev });", start) + len("revAfter: srvAfter.rev });")
new_chk = ("        const okRefused = fo.shown && /Refused|stale|PlayerPlanStateStale|PlayerSourceStale/i.test(fo.alert) && JSON.stringify(srvAfter.crops) === JSON.stringify(srvBefore.crops) && srvAfter.rev === srvBefore.rev;\n"
           "        const okHealed = !fo.shown && healedBefore === true && srvAfter.rev === srvBefore.rev + 1;\n"
           "        check(okRefused || okHealed, 'AC15/DEFECT-2 (R3 probe wording): after a stale REFUSED answer was dropped and ' + R1 + ' was then selected, the next Crop is either refused by the server with nothing written (page still on the older view) or accepted because the selection read self-healed the view first (the page already showed the newer Crop designation): never a write on a stale fence', { outcome: okRefused ? 'refused, fail closed' : okHealed ? 'self-healed, then accepted' : 'NEITHER', pageAlreadyShowedNewerCropBeforeClick: healedBefore, alert: fo.alert, revBefore: srvBefore.rev, revAfter: srvAfter.rev });")
s = s[:start] + new_chk + s[end:]
open(p, 'w', encoding='utf-8', newline='\n').write(s)
print('patched P7 follow-up')
