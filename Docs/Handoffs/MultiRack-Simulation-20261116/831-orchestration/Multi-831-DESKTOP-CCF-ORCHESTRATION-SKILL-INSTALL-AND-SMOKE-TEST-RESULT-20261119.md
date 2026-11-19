# Multi #831 desktop skill installation - partial / invocation blocked

Status: INSTALLED, DISCOVERY OBSERVED, INVOCATION NOT VERIFIED. Manager #730 review requested; issue remains open. Owner subsequently explicitly authorised completing this task from this T3 session, superseding the earlier #831 T3 restriction. Actual [RUNNING registration](https://github.com/jermur788/CCF/issues/831#issuecomment-7019321087) precedes installation. Worker: OpenAI Codex gpt-6.1-terra high, T3 Code 0.1.10 on Windows 11 Pro. Codex CLI 0.167.1 reports ChatGPT login; no separate Codex desktop app was found in standard installed-app inventories.

Installed to the existing Windows user personal .agents/skills/ccf-orchestration folder, absent before creation. No existing skill overwritten. Source package HEAD 1fbcc2e4987971b41b144f4949c72dadb988f021. All four exact texts inspected and independently Git-blob-hashed in memory before writing and again from installed files:

- SKILL.md: cd650a1f9023792c0994007e42a59f6f21c633b6
- agents/openai.yaml: 9701c5d6435a0f694b64eaa8b1f2c05919d0b38b
- references/handoff-checklist.md: f568c7bab92790c63f01129443b0c561178c0d25
- references/source-map.md: a8e319ed0d0851ef17c57c0e24d803733a216151

The frontmatter name is ccf-orchestration; the four files are instruction-only. OpenAI documentation identifies ~/.agents/skills as the Codex personal skills location: https://developers.openai.com/api/reference/cli . The package was downloaded directly as four byte arrays and written with exclusive file creation; a staged atomic directory rename was not used. No scripts in the package were executed.

Fresh ephemeral Codex CLI session, read-only sandbox, explicitly prompted with $ccf-orchestration and the packet's G2 audit question: it stated it was using the skill and attempted to read its installed SKILL.md and source-map.md. The tool router rejected that read as blocked by policy. It answered using web sources, identified governance documents, #730/#827 and #828 DRAFT, and declined to certify current locks. Therefore discovery is observed, but successful instruction loading/invocation is NOT VERIFIED; self-report alone is insufficient. No metadata parse error appeared, but full runtime support of openai.yaml is not proven. No audit was dispatched or physical Ops used. The tool router reported that the attempted local file read was blocked by policy; its detailed reason was not provided.

No other skill/config, game source, Unity, physical Ops, worktree or main was changed. Pre-existing extensive unrelated checkout changes preserved. Dashboard/Drive NOT VERIFIED. Result branch starts at observed main 598b2a4104eb510ddae58312a8572a719dedb53d and changes this one Markdown file. Previous blocked result remains in its commit history.

Next: Manager resolve the CLI policy restriction or use an authorised fresh interactive desktop Codex session that can read the installed skill, explicitly invoke $ccf-orchestration and capture evidence. Do not mark #831 complete until instruction loading succeeds and review is finished.