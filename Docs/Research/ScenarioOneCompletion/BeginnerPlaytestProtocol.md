# Beginner playtest protocol (Workstream S)

**Purpose:** find out whether people with **little or no forestry knowledge** understand what Scenario One is teaching, not just whether buttons work.

**Status:** protocol for the first external playtest. Run it when the **BEGINNER PLAYTEST READY** gate in `ScenarioOneDefinitionOfDone.md` is met (at minimum P1 copy, P2, P3 and the S1 cash warning).

## 1. Participants and set-up

| Item | Specification |
|---|---|
| Participants | 5–6 adults with no forestry training (screen out foresters, ecologists, forestry students). Mix of gamers and non-gamers (at least 2 who rarely play 3D games) |
| Session | 75 minutes: 5 consent/intro, 50 play, 15 interview, 5 buffer |
| Build | One fixed main commit (recorded), new game, Scenario One |
| Machine | Fixed resolution **1600 × 900** (also record one session at 1280 × 720) |
| Profile | **Fresh learning/help preferences per participant**: clear the `CCF.MenuHelp.v1.*` and `CCF.Learning.v1.*` PlayerPrefs before each session (stall point S14). Delete or back up `forest-save.json` |
| Recording | Screen + voice (think-aloud), with consent. Observer notes with timestamps |
| Observer | One facilitator (talks) + one note-taker (silent) where possible |
| Save points | The facilitator may F5 at key moments for later review. The participant is not told about F5/F9 unless the game tells them |

## 2. Instructions to the participant (read verbatim)

> "This is a game about managing a small forest. We are testing the game, not you. There are no wrong answers. Please think aloud: say what you are looking at, what you think it means and what you are trying to do. I can't help you with the game, but I may ask you what you are thinking."

## 3. Tasks (given one at a time, on a card)

| # | Task card | Time box | Observer checks (do not prompt) |
|---|---|---|---|
| T1 | "Get to know your forest." | 8 min | Do they read the intro? Use the map? Walk? Inspect a tree? |
| T2 | "Choose some trees you would like to keep and grow into good timber." | 8 min | Inspection before C? Do they look at neighbours? Which trees (biggest? edge?) |
| T3 | "Decide whether any trees should be removed to help the ones you are keeping, and plan that work." | 10 min | Fell marks near Crop Trees? Do they read the forecast / "What you are leaving"? |
| T4 | "Carry out your plan and let a year pass. Then find out what happened." | 6 min | Approve/advance; Annual Review read or skipped; do they walk back? |
| T5 | "Keep managing for as long as you like. Try to keep this a continuous-cover forest." | 15 min | Planting? Shelters? Clearance? Second intervention? Advance pace |
| T6 | (Only if not reached by Year 6) "Advance to Year 10 and look at your forest." | 3 min | Do they notice change? Use history? |

**Cash-trap guard:** if cash drops below €3,000 before any thinning (stall point S1, if not yet fixed), note it, then let the session continue. Record whether the game warned them. This is a finding, not participant error.

## 4. Things the observer must NOT explain

CCF; Crop Tree; competitor/competition; DBH; basal area; what to cut; why thinning lost money; what the Annual Review means; why regeneration is or is not happening; whether to clear or plant; shelters; the map's purpose; keys (unless the participant is stuck on a *control* for > 2 minutes; then say only the key and log it as **HELP-CONTROL**).

If asked "What should I do?", reply: "What do you think you could do?" If asked "Is this right?", reply: "What makes you unsure?"

## 5. Concepts and success criteria

Measured in the interview (§7) and through observed behaviour. Each scored **E** (explains correctly in own words) / **P** (partly) / **N** (not, or wrong).

| Concept | Success looks like | Session success criterion |
|---|---|---|
| CCF | Keep tree cover while harvesting selectively and renewing; repeated | ≥ 4 of 6 participants E or P |
| Crop Tree | A tree chosen to keep and favour | ≥ 5 of 6 E |
| Competitor | A neighbour that limits a chosen tree's growth; bigger/closer matters more | ≥ 4 of 6 E or P |
| DBH | Trunk diameter at chest height | ≥ 4 of 6 E |
| Why they thinned | Names their Crop Trees or growth, not just "too many trees" or "to make money" | ≥ 4 of 6 E or P |
| Work Plan | Where planned work is reviewed, costed and approved; it does not decide where | ≥ 4 of 6 E or P |
| Annual Review | What happened this year and where to look | ≥ 4 of 6 E or P |
| Why regeneration failed / succeeded | Names light, seed or browsing correctly for a place they visited | ≥ 3 of 6 E or P |
| When clearance is useful | Conditional answer ("where … but it removes young trees too"), not "always"/"never" | ≥ 3 of 6 P or better (Model 2 builds only; else ask "What does clearance do here?") |
| Why another intervention is needed | The forest keeps changing; trees grow back into the space; CCF is repeated | ≥ 3 of 6 E or P |
| Thinning loses money | Small forest / minimum charge, not "I did it wrong" | ≥ 3 of 6 E or P |

**Playtest passes** if 8 of the 11 criteria are met and no participant hits an unexplained dead end (S1) or abandons in confusion.

## 6. Misunderstanding codes (tag observations)

| Code | Meaning |
|---|---|
| **M-SCORE** | Treats a number as a score or instruction ("it says 12 %, so cut it") |
| **M-CLEANUP** | Removes the smallest/suppressed trees as "tidying" |
| **M-BIGGEST** | Picks Crop Trees only by size, including edge trees, without considering neighbours |
| **M-MAPEDIT** | Tries to act from the map (mark, plant, clear) |
| **M-APPROVE=DONE** | Thinks approval executed the work |
| **M-SKIPREVIEW** | Closes the Annual Review unread (after the first) |
| **M-CLEAR-DEFAULT** | Clears vegetation because the preview is there, not from diagnosis |
| **M-PLANT-DEFAULT** | Plants everywhere / plants because it is "good" |
| **M-ERROR-BLAME** | Thinks a money loss means they did something wrong |
| **M-ONEANDDONE** | Believes one thinning completes CCF |
| **M-CAUSAL** | Attributes an outcome to the wrong cause (e.g. clearance made trees grow; storm proved a plan wrong) |
| **M-TERM** | Misreads a term (DBH, basal area, cohort, Crop Tree) |
| **M-STUCK** | Cannot find how to progress (> 2 min) |
| **M-LOST** | Spatially lost; cannot relate map to forest |
| **M-READ** | Cannot read text (size/contrast/placement) |
| **HELP-CONTROL** | Facilitator gave a key binding |

## 7. Post-session interview (15 min, recorded)

Ask in this order; do not correct answers.

1. "In your own words, what was this game asking you to do?" *(CCF)*
2. "What is a Crop Tree?" *(Crop Tree)*
3. "How did you decide which trees to remove?" *(Why thinned; competitor)*
4. "Show me a tree and tell me what DBH means for it." *(DBH)*
5. "Why did the thinning cost you money?" *(Economy)*
6. "What do you get from the trees you didn't cut?" *(Capital)*
7. "What is the Work Plan for? And the Annual Review?" *(Work Plan; Annual Review)*
8. "Take me to a place with young trees (or without). Why are they there (or not)?" *(Regeneration)*
9. "When would you clear vegetation? When wouldn't you?" *(Clearance)*
10. "Would you come back and work in this forest again? When, and why?" *(Second intervention)*
11. "Was any number or message confusing or misleading?" *(Readability; M-SCORE)*
12. "If you could change one thing, what would it be?"
13. Ease rating 1–5; "would you play on?" yes/no.

## 8. Data-capture template (one row per participant, plus an event log)

```
PARTICIPANT: P#   DATE:   BUILD SHA:   RESOLUTION:   GAMER: frequent/occasional/rare
SESSION LENGTH:   YEAR REACHED:   CASH LOWEST:   S1 HIT: y/n (warned: y/n)

ACTIONS (from save + notes)
 Crop Trees designated: n   (inspected before C: n)
 Thinnings: years [...]   trees per thinning [...]   Fell marks within 8 m of a Crop Tree: x/y
 Read "What you are leaving": y/n   Neighbour list opened: y/n
 Annual Reviews opened/read (est.): n/n   PLACES waypoints followed: n
 Planting: years, species, n, shelters y/n   Clearance: cells n   Pruning: n
 Second intervention: y/n (year)

CONCEPT SCORES (E/P/N): CCF _ CropTree _ Competitor _ DBH _ WhyThin _ WorkPlan _
 Review _ Regen _ Clearance _ Repeat _ Economy _

MISUNDERSTANDING CODES (timestamp, code, quote):
 00:07:12  M-CLEANUP  "I'll get rid of the scrawny ones"

QUOTES WORTH KEEPING:
STUCK MOMENTS (timestamp, where, how resolved):
EASE 1–5:   PLAY ON: y/n   ONE CHANGE:
```

Also keep each participant's final save (`forest-save.json`, copied with a participant id) for objective re-analysis.

## 9. Analysis and output

- Per concept: count E/P/N against §5.
- Code frequency table; each code with ≥ 2 participants becomes a fix candidate for **P8 (playtest fixes)**.
- Map every stuck moment to `PlayerStallPoints.md` (new IDs if new).
- Report: "Beginner Playtest 1 — findings", committed beside this protocol. No participant names. Quotes anonymised.

## 10. Ethics and practicalities

Informed consent, the right to stop at any time, recordings stored locally and deleted after analysis unless consent says otherwise; no personal data in the repository.
