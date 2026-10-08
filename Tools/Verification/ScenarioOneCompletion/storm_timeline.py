#!/usr/bin/env python3
"""Offline storm timeline for the *proposed* Storms v1 roll (teaching design aid).

Reproduces SimulationRandom.Roll (RNG model 1) from
Assets/ForestPrototype/SimulationRandom.cs at a8596df and applies the event
roll proposed in Docs/Research/WindthrowV1/StormEventArchitecture.md
(task/windthrow-readiness @ 9a9f4fb): occurrence id "STORM-OCCURS-v1",
severity id "STORM-SEVERITY-v1", default p = 0.08, severity weights
moderate/severe/extreme = 0.70/0.25/0.05, grace years = 3.

Nothing here is implemented in production. Parameters are [C] proposals. The
severity pick order (cumulative moderate -> severe -> extreme) is an
assumption about the future implementation; occurrence years do not depend on it.
Standard library only.
"""
import sys

MASK64 = (1 << 64) - 1
GOLDEN = 0x9E3779B97F4A7C15


def mix(z):
    z = (z + GOLDEN) & MASK64
    z = ((z ^ (z >> 30)) * 0xBF58476D1CE4E5B9) & MASK64
    z = ((z ^ (z >> 27)) * 0x94D049BB133111EB) & MASK64
    return z ^ (z >> 31)


def roll(identifier, year, seed):
    h = 2166136261
    for ch in identifier:
        h = ((h ^ ord(ch)) * 16777619) & 0xFFFFFFFF
    state = mix(((h << 32) & MASK64) ^ (year & 0xFFFFFFFF))
    state = mix(state ^ (((seed & 0xFFFFFFFF) * GOLDEN) & MASK64))
    return (state >> 40) / 16777216.0


def timeline(seed, p, weights, grace, last_year):
    events = []
    for year in range(grace, last_year + 1):
        if roll("STORM-OCCURS-v1", year, seed) < p:
            u = roll("STORM-SEVERITY-v1", year, seed)
            cumulative, label = 0.0, "extreme"
            for name, weight in weights:
                cumulative += weight
                if u < cumulative:
                    label = name
                    break
            events.append((year, label))
    return events


def main():
    seed = int(sys.argv[1]) if len(sys.argv) > 1 else 20260914
    weights = [("moderate", 0.70), ("severe", 0.25), ("extreme", 0.05)]
    for p in (0.05, 0.08, 0.12):
        events = timeline(seed, p, weights, 3, 100)
        within25 = [e for e in events if e[0] <= 25]
        print(f"seed={seed} p={p}: storms<=Y25={within25} all<=Y100={events}")
    p, horizon = 0.08, 23  # years 3..25 inclusive
    any_storm = 1 - (1 - p) ** horizon
    any_severe = 1 - (1 - p * 0.30) ** horizon
    print(f"analytic p=0.08 years3-25: P(any storm)={any_storm:.3f} P(any severe-or-extreme)={any_severe:.3f}")
    # Variety if the seed were drawn per new game (for the product decision).
    firsts = []
    for s in range(1000, 1200):
        events = timeline(s, 0.08, weights, 3, 100)
        firsts.append(events[0][0] if events else 999)
    firsts.sort()
    print(f"200 alternative seeds, p=0.08: first-storm year median={firsts[len(firsts)//2]} "
          f"p10={firsts[len(firsts)//10]} p90={firsts[len(firsts)*9//10]}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
