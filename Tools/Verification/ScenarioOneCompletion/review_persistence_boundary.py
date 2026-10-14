#!/usr/bin/env python3
"""Offline source/persistence indistinguishability proof, not a completion implementation."""
from pathlib import Path
import json,re
ROOT=Path(__file__).resolve().parents[3]
manager=(ROOT/'Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs').read_text()
match=re.search(r'public void MarkAnnualReviewSeen\(\)\s*\{([^}]+)\}',manager)
assert match
body=match.group(1)
assert 'annualReviewSeen = true' in body and 'Record' not in body
assert 'annualReviewSeen = annualReviewSeen' in manager
assert 'annualReviewSeen = data.annualReviewSeen' in manager
assert 'AnnualReviewAcknowledged' not in (ROOT/'Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs').read_text()
assert 'CurrentVersion = 20' in (ROOT/'Assets/ForestPrototype/ForestSaveData.cs').read_text()
# Abstract persistence witness: all other fields held equal. The unrecorded choice
# of acknowledgement time changes the required verdict but not persisted bytes.
def observed(acknowledged_year):
    return dict(version=20,annualReviewSeen=acknowledged_year is not None,
                resolvedYears=[10,25],events=[dict(eventId=100,year=10,eventType='WorkResolved',outcome='Succeeded'),dict(eventId=200,year=25,eventType='WorkResolved',outcome='Succeeded')])
a,b=observed(1),observed(10)
assert json.dumps(a,sort_keys=True)==json.dumps(b,sort_keys=True)
assert not (10<=1<25) and (10<=10<25)
print('REVIEW_PERSISTENCE_BOUNDARY_CONFIRMED source_checks=6 identical_records_different_required_verdict=True')
print('NOT_UNITY_VERIFIED; no completion implementation or save-contract extension')
