#!/usr/bin/env python3
"""Offline contract objects only; never executes Unity JsonUtility or LoadData."""
import json
from pathlib import Path
ROOT=Path(__file__).parent

def event(i,year,kind,order=0):
    return dict(eventId=i,year=year,eventType=kind,workOrderId=order,outcome=1 if kind==4 else 0,executionMethod=0,ownerMinutes=0)
def eligible(s):
    if s['outcome']==1: return True # grandfather persisted Completed
    if s['outcome']==2: return False
    work=[e for e in s['managementEvents'] if e['eventType']==4 and e['outcome']==1 and e['workOrderId']>0]
    reviews=[e for e in s['managementEvents'] if e['eventType']==9]
    return any(a['year']<b['year'] and a['eventId']<r['eventId']<b['eventId'] and a['year']<=r['year']<=b['year'] for a in work for b in work for r in reviews)
def main():
    base=dict(outcome=0,outcomeYear=-1,annualReviewSeen=True,managementEvents=[])
    cases={
      'legacy_active':(dict(base,managementEvents=[event(1,10,4,1),event(2,25,4,2)]),False),
      'legacy_completed':(dict(base,outcome=1,outcomeYear=25),True),
      'legacy_failed':(dict(base,outcome=2,outcomeYear=9),False),
      'new_v20_review_event':(dict(base,managementEvents=[event(1,10,4,1),event(2,10,9),event(3,25,4,2)]),True),
      'after_first_intervention':(dict(base,annualReviewSeen=False,managementEvents=[event(1,10,4,1)]),False),
      'after_review':(dict(base,managementEvents=[event(1,10,4,1),event(2,10,9)]),False),
      'before_second_intervention':(dict(base,managementEvents=[event(1,10,4,1),event(2,10,9),event(3,24,1,2)]),False),
      'review_before_first':(dict(base,managementEvents=[event(1,1,9),event(2,10,4,1),event(3,25,4,2)]),False),
      'same_year_two_orders':(dict(base,managementEvents=[event(1,10,4,1),event(2,10,9),event(3,10,4,2)]),False),
      'failed_only_second':(dict(base,managementEvents=[event(1,10,4,1),event(2,10,9),dict(event(3,25,4,2),outcome=2)]),False)
    }
    for name,(s,expected) in cases.items():
        obj=dict(version=20,growthModel=1,scenarioOne=s)
        assert eligible(s)==expected,name
        assert eligible(json.loads(json.dumps(obj))['scenarioOne'])==expected,name
        (ROOT/'fixtures').mkdir(exist_ok=True)
        (ROOT/'fixtures'/(name+'.json')).write_text(json.dumps(dict(testObject=obj,expectedChronologyEligible=expected,scope='partial offline contract object; not a validated production save; eight objectives assumed separately'),indent=2)+'\n')
    print(f'PASS {len(cases)} offline chronology objects and JSON round trips; no production loader exercised')
if __name__=='__main__': main()
