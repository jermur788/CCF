#!/bin/bash
# DISPOSABLE. Independent re-derivation of headline numbers straight from the raw fixtures, WITHOUT mfm_a_vs_c_offline.py and
# without #253's script. Each block is a self-contained python3 -I -B one-liner reading only raw JSON.
V=/media/jer/Files/CCF/Verification
D=$V/ScenarioOneFiveRackReference-20261103T2210Z/measurements/design_candidate.json
T=$V/ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/trees.json
M=$V/ScenarioOneFiveRackReference-20261103T2210Z/measurements/mfm_candidate.json
S=$V/ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/mfm.json
echo "== sha256 of raw inputs"; sha256sum $D $T $M $S
echo; echo "== R1: R1/R2 corridor RECTANGLES do not overlap (gap) but centreline distance < 2.4 m editor rule"
python3 -I -B - $D <<'P'
import json,math,sys
d=json.load(open(sys.argv[1]))['racks']; r={x['id']:x for x in d}
def rect(x):
    a=(x['junction']['x'],x['junction']['z']); b=(x['far']['x'],x['far']['z'])
    L=math.dist(a,b); ux,uz=(b[0]-a[0])/L,(b[1]-a[1])/L; nx,nz=-uz,ux; h=1.2
    return [(a[0]+nx*h,a[1]+nz*h),(b[0]+nx*h,b[1]+nz*h),(b[0]-nx*h,b[1]-nz*h),(a[0]-nx*h,a[1]-nz*h)]
def pd(p,a,b):
    t=max(0,min(1,((p[0]-a[0])*(b[0]-a[0])+(p[1]-a[1])*(b[1]-a[1]))/((b[0]-a[0])**2+(b[1]-a[1])**2)))
    return math.dist(p,(a[0]+t*(b[0]-a[0]),a[1]+t*(b[1]-a[1])))
A,B=rect(r['R1']),rect(r['R2'])
gap=min(min(pd(p,B[i],B[(i+1)%4]) for i in range(4)) for p in A)
gap=min(gap,min(min(pd(p,A[i],A[(i+1)%4]) for i in range(4)) for p in B))
def sd(a,b,c,dd):
    pts=[(a,c,dd),(b,c,dd),(c,a,b),(dd,a,b)]
    return min(pd(p,q,w) for p,q,w in pts)
cl=sd((r['R1']['junction']['x'],r['R1']['junction']['z']),(r['R1']['far']['x'],r['R1']['far']['z']),(r['R2']['junction']['x'],r['R2']['junction']['z']),(r['R2']['far']['x'],r['R2']['far']['z']))
print('rectangle-rectangle min distance (R1,R2) = %.6f m (positive => NOT overlapping)'%gap)
print('junction-to-junction distance R1-R2     = %.6f m'%math.dist((r['R1']['junction']['x'],r['R1']['junction']['z']),(r['R2']['junction']['x'],r['R2']['junction']['z'])))
print('centreline segment distance R1-R2       = %.6f m  (< 2.4 => editor OverlapClearanceM refuses)'%cl)
P
echo; echo "== R2: 45-degree single-family arithmetic from the raw spine"
python3 -I -B - $D <<'P'
import json,math,sys
sp=json.load(open(sys.argv[1]))['spine']
z0=sp['existing_polyline'][0][1]; z1=sp['continuation_end']['z']; ext=z1-z0
print('spine z-extent = %.6f m'%ext)
print('64/sin45 = %.5f m ; asin(64/extent) = %.5f deg ; extent*sin45/4 = %.5f m (max equal gap, 5 racks at 45 deg)'%(64/math.sin(math.pi/4),math.degrees(math.asin(64/ext)),ext*math.sin(math.pi/4)/4))
print('racks fitting at 45 deg for gap g: floor(extent*sin45/g)+1 -> g=16: %d ; g=13.7: %d ; g=2.4: %d'%tuple(int(ext*math.sin(math.pi/4)/g+1e-9)+1 for g in (16,13.7,2.4)))
P
echo; echo "== R3: stand ground, corridor union and nearest-rack reach on 0.5 m lattice (3+2 reference), from raw racks only"
python3 -I -B - $D <<'P'
import json,math,sys
rk=json.load(open(sys.argv[1]))['racks']
S=[((x['junction']['x'],x['junction']['z']),(x['far']['x'],x['far']['z'])) for x in rk]
def info(a,b):
    L=math.dist(a,b); return a,b,L,(b[0]-a[0])/L,(b[1]-a[1])/L
S=[info(a,b) for a,b in S]
def cor(x,z):
    for a,b,L,ux,uz in S:
        px,pz=x-a[0],z-a[1]; al=px*ux+pz*uz; ac=-px*uz+pz*ux
        if -1e-4<=al<=L+1e-4 and abs(ac)<=1.2+1e-4: return True
    return False
def dist(x,z):
    best=1e9
    for a,b,L,ux,uz in S:
        px,pz=x-a[0],z-a[1]; t=max(0,min(L,px*ux+pz*uz)); best=min(best,math.hypot(px-t*ux,pz-t*uz))
    return best
n=160; tot=cor_=0; ge={9:0,12:0,20:0}
for i in range(n):
    for j in range(n):
        x=-39.75+0.5*i; z=-39.75+0.5*j
        if cor(x,z): cor_+=1; continue
        tot+=1; d=dist(x,z)
        for c in ge:
            if d>c: ge[c]+=1
print('corridor union = %.2f m2 ; non-corridor = %.2f m2'%(cor_*.25,tot*.25))
print('nearest-rack ground beyond 9/12/20 m = %.2f / %.2f / %.2f m2'%tuple(ge[c]*.25 for c in (9,12,20)))
P
echo; echo "== R4: stems in rack corridors and legacy-baseline stems per area vs recorded C# run"
python3 -I -B - $D $T $M <<'P'
import json,math,sys
rk=json.load(open(sys.argv[1]))['racks']; tr=[t for t in json.load(open(sys.argv[2]))['trees'] if t['living'] and not t['stump']]
S=[]
for x in rk:
    a=(x['junction']['x'],x['junction']['z']);b=(x['far']['x'],x['far']['z']);L=math.dist(a,b);S.append((a,L,(b[0]-a[0])/L,(b[1]-a[1])/L))
n=0
for t in tr:
    for a,L,ux,uz in S:
        px,pz=t['x']-a[0],t['z']-a[1];al=px*ux+pz*uz;ac=-px*uz+pz*ux
        if -1e-4<=al<=L+1e-4 and abs(ac)<=1.2+1e-4: n+=1;break
print('living stems = %d ; stems with centre inside any of the five reference corridors = %d (published #251: 103)'%(len(tr),n))
base=json.load(open(sys.argv[3]))['baseline']
print('recorded C# baseline: covered %d/%d, sourceStems %d, sum of area stems = %d'%(base['coveredCount'],base['nonSourceCount'],base['sourceStems'],sum(a['stems'] for a in base['areas'])))
P
