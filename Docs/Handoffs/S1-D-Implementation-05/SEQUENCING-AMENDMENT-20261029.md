# S1-D T-05 — Sequencing Amendment (2026-10-29)

This amendment supplements `TASK-PACKET.md`; all approved S1-D T-05 product requirements and verification remain unchanged.

## Updated dependency/API rule

- #104 execution claim is now `[HANDED-OFF]` and live S1-C T-05 ownership is released, but its later umbrella integration remains reported on local/unpushed `ac876a6...`. The task still requires an exact current/published S1-C history API before canonical integration claims.
- MFM-07 will introduce the prescription-approval → pending ordinary work boundary and provenance API. **T-05 should start only after MFM-07 is accepted/integrated**, so its Pending new / Pending carried-over grouping can classify the real generated ordinary work rather than an interim interface.
- T-05 must land **before MFM-09** on the shared Work Plan presentation. MFM-09 then consumes the existing Approved / Pending new / Pending carried-over grouping and adds teaching/explanation for prescription-generated pending work; it must not create a parallel grouping/history surface.
- S1-C T-04/T-06 ownership on Annual Review/history must be released or explicitly partitioned before T-05 edits any shared navigation surface.
- At dispatch, use `Docs/Handoffs/PlayerTestReadiness-Overnight-01/CROSS-FEATURE-OWNERSHIP-AND-SEQUENCING.md` with the original packet.

No Windows build.

## Updated copy/paste dispatch line

`Run S1-D T-05 using Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md plus SEQUENCING-AMENDMENT-20261029.md and the current cross-feature sequencing disposition. Start only after MFM-07 is accepted and the exact S1-C history/Work Plan APIs are available on the current integration candidate; land before MFM-09. Use Claude Sonnet 5.5 Medium. Reuse the S1-C history surface and MFM pending-work provenance; no new persistence, no duplicate history UI, no automatic approval, and no Windows build.`
